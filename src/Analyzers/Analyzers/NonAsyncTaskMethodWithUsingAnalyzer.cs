using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace UsingAsync.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonAsyncTaskMethodWithUsingAnalyzer : DiagnosticAnalyzer
{
    internal const string Id = "UsingAsync1";

    public static readonly DiagnosticDescriptor DiagnosticDescriptor = new(
        id: Id,
        title: "Task might try to access disposed resource",
        messageFormat:
            "'{0}' is a non-async Task-returning method with at least one using-statement that might be disposed before the returned Task completes",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://raw.githubusercontent.com/ItaiTzur76/UsingAsync/main/analyzer-messages/UsingAsync1.html");

    private static readonly ImmutableArray<DiagnosticDescriptor> ReusableSupportedDiagnostics =
        ImmutableArray.Create(DiagnosticDescriptor);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ReusableSupportedDiagnostics;

    private static readonly HashSet<string> CompletedTaskSymbolNames
        = [Constants.CompletedTask, Constants.FromResult, "FromException", "FromCanceled"];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

        static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var contextNode = context.Node;
            var methodDeclarationSyntax = (MethodDeclarationSyntax)contextNode;
            var body = methodDeclarationSyntax.Body;
            if (body is null || methodDeclarationSyntax.Modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                return;
            }

            var containingSymbol = context.ContainingSymbol!;
            var compilation = context.Compilation;
            var taskClassSymbol = compilation.GetTaskClassType();
            var taskGenericSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            if (TypeIsNotTask(((IMethodSymbol)containingSymbol).ReturnType.OriginalDefinition))
            {
                return;
            }

            if (StatementsMightUseDisposedResource(body.Statements, usingEncountered: false))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DiagnosticDescriptor, contextNode.GetLocation(), containingSymbol.Name));
            }

            bool StatementsMightUseDisposedResource(
                IEnumerable<StatementSyntax> statementSyntaxes, bool usingEncountered)
            {
                foreach (var statementSyntax in statementSyntaxes)
                {
                    if (StatementMightUseDisposedResource(statementSyntax, ref usingEncountered))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool StatementMightUseDisposedResource(StatementSyntax statementSyntax, ref bool usingEncountered)
                => statementSyntax switch
                {
                    BlockSyntax blockSyntax =>
                        StatementsMightUseDisposedResource(blockSyntax.Statements, usingEncountered),
                    IfStatementSyntax ifStatementSyntax =>
                        IfStatementMightUseDisposedResource(ifStatementSyntax, usingEncountered),
                    WhileStatementSyntax whileStatementSyntax =>
                        StatementsMightUseDisposedResource([whileStatementSyntax.Statement], usingEncountered),
                    DoStatementSyntax doStatementSyntax =>
                        StatementsMightUseDisposedResource([doStatementSyntax.Statement], usingEncountered),
                    ForStatementSyntax forStatementSyntax =>
                        StatementsMightUseDisposedResource([forStatementSyntax.Statement], usingEncountered),
                    CommonForEachStatementSyntax forEachStatementSyntax =>
                        StatementsMightUseDisposedResource([forEachStatementSyntax.Statement], usingEncountered),
                    UsingStatementSyntax usingStatementSyntax =>
                        StatementsMightUseDisposedResource([usingStatementSyntax.Statement], usingEncountered: true),
                    ReturnStatementSyntax returnStatementSyntax =>
                        usingEncountered && ReturnStatementMightUseDisposedResource(returnStatementSyntax),
                    TryStatementSyntax tryStatementSyntax =>
                        TryStatementMightUseDisposedResource(tryStatementSyntax, usingEncountered),
                    SwitchStatementSyntax switchStatementSyntax =>
                        SwitchStatementMightUseDisposedResource(switchStatementSyntax, usingEncountered),
                    LocalDeclarationStatementSyntax localDeclarationStatementSyntax =>
                        LocalDeclarationStatementMightUseDisposedResource(localDeclarationStatementSyntax, ref usingEncountered),
                    _ => false
                };

            bool IfStatementMightUseDisposedResource(IfStatementSyntax ifStatementSyntax, bool usingEncountered)
            {
                List<StatementSyntax> statementSyntaxes = [ifStatementSyntax.Statement];
                var elseClauseSyntax = ifStatementSyntax.Else;
                if (elseClauseSyntax is not null)
                {
                    statementSyntaxes.Add(elseClauseSyntax.Statement);
                }

                return StatementsMightUseDisposedResource(statementSyntaxes, usingEncountered);
            }

            bool ReturnStatementMightUseDisposedResource(ReturnStatementSyntax returnStatementSyntax)
            {
                var returnStatementExpressionSyntax = returnStatementSyntax.Expression;
                var semanticModel = GetSemanticModel(returnStatementExpressionSyntax!.SyntaxTree);
                return GetExpressionSyntaxes(returnStatementExpressionSyntax)
                    .Select(expressionSyntax => semanticModel.GetSymbolInfo(expressionSyntax).Symbol)
                    .Any(symbol => symbol is not null
                                && (TypeIsNotTask(symbol.ContainingSymbol) ||
                                    !CompletedTaskSymbolNames.Contains(symbol.Name)));

                SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
#pragma warning disable RS1030 // Do not invoke Compilation.GetSemanticModel() method within a diagnostic analyzer
                    => compilation.GetSemanticModel(syntaxTree);
#pragma warning restore RS1030 // Do not invoke Compilation.GetSemanticModel() method within a diagnostic analyzer

                static IEnumerable<ExpressionSyntax> GetExpressionSyntaxes(ExpressionSyntax expressionSyntax)
                    => expressionSyntax switch
                    {
                        ParenthesizedExpressionSyntax parenthesizedExpressionSyntax =>
                            GetExpressionSyntaxes(parenthesizedExpressionSyntax.Expression),
                        ConditionalExpressionSyntax conditionalExpressionSyntax =>
                            GetExpressionSyntaxes(conditionalExpressionSyntax.WhenTrue).Concat(
                                GetExpressionSyntaxes(conditionalExpressionSyntax.WhenFalse)),
                        _ => [expressionSyntax]
                    };
            }

            bool TypeIsNotTask(ISymbol symbol)
                => !symbol.EqualsWithDefaultComparer(taskClassSymbol)
                && !symbol.EqualsWithDefaultComparer(taskGenericSymbol);

            bool TryStatementMightUseDisposedResource(TryStatementSyntax tryStatementSyntax, bool usingEncountered)
                => StatementsMightUseDisposedResource(
                    [
                        tryStatementSyntax.Block,
                        .. tryStatementSyntax.Catches.Select(static catchClauseSyntax => catchClauseSyntax.Block)
                    ],
                    usingEncountered);

            bool SwitchStatementMightUseDisposedResource(
                SwitchStatementSyntax switchStatementSyntax, bool usingEncountered)
            {
                foreach (var switchSectionSyntax in switchStatementSyntax.Sections)
                {
                    if (StatementsMightUseDisposedResource(switchSectionSyntax.Statements, usingEncountered))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool LocalDeclarationStatementMightUseDisposedResource(
                LocalDeclarationStatementSyntax localDeclarationStatementSyntax, ref bool usingEncountered)
            {
                if (!localDeclarationStatementSyntax.UsingKeyword.IsKind(SyntaxKind.None))
                {
                    usingEncountered = true;
                }

                return false;
            }
        }
    }
}

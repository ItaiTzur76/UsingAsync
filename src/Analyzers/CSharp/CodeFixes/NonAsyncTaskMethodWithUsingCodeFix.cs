using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UsingAsync.Analyzers;

namespace UsingAsync.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class NonAsyncTaskMethodWithUsingCodeFix : CodeFixProvider
{
    private static readonly ImmutableArray<string> ReusableFixableDiagnosticIds =
        [NonAsyncTaskMethodWithUsingAnalyzer.Id];

    public override ImmutableArray<string> FixableDiagnosticIds => ReusableFixableDiagnosticIds;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        context.RegisterCodeFix(new MakeMethodAsyncCodeAction(context.Document, diagnostic), diagnostic);
        return Task.FromResult<object?>(null);
    }

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private sealed class MakeMethodAsyncCodeAction(Document document, Diagnostic diagnostic) : CodeAction
    {
        private readonly Document _document = document;
        private readonly Diagnostic _diagnostic = diagnostic;

        public override string Title => "Make method async";

        public override string? EquivalenceKey => null;

        protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            var root = await _document.GetSyntaxRootAsync(cancellationToken)
                ?? throw new InvalidOperationException("No syntax root could be obtained from the document.");
            var methodDeclaration = (MethodDeclarationSyntax)root.FindNode(_diagnostic.Location.SourceSpan);
            var newMethodDeclaration = methodDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));
            var semanticModel = (await _document.GetSemanticModelAsync(cancellationToken))!;
            var taskClassSymbol = semanticModel.Compilation.GetTaskClassType()?.OriginalDefinition;
            var body = newMethodDeclaration.Body!;
            MakeMethodAsyncFixer fixer = (semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken) ?? throw new InvalidOperationException("Unable to get method symbol."))
                    .ReturnType.OriginalDefinition.EqualsWithDefaultComparer(taskClassSymbol)
                ? new MakeMethodAsyncFixerForTaskClass(semanticModel, taskClassSymbol, cancellationToken)
                : new MakeMethodAsyncFixerForTaskGeneric(semanticModel, taskClassSymbol, cancellationToken);
            return _document.WithSyntaxRoot(root.ReplaceNode(
                methodDeclaration,
                newMethodDeclaration.WithBody(SyntaxFactory.Block(
                    body.AttributeLists,
                    body.OpenBraceToken,
                    new(fixer.FixedSyntaxes(methodDeclaration.Body!.Statements, needToAppendReturn: false)),
                    body.CloseBraceToken))));
        }

        private abstract class MakeMethodAsyncFixer(
            SemanticModel semanticModel, INamedTypeSymbol? taskClassSymbol, CancellationToken cancellationToken)
        {
            private readonly SemanticModel _semanticModel = semanticModel;
            private readonly INamedTypeSymbol? _taskClassSymbol = taskClassSymbol;
            private readonly CancellationToken cancellationToken = cancellationToken;

            protected abstract bool IsTaskClass { get; }

            protected abstract List<StatementSyntax> FixedReturnSyntax(
                ReturnStatementSyntax returnStatementSyntax, bool needToAppendReturn);

            internal List<StatementSyntax> FixedSyntaxes(
                SyntaxList<StatementSyntax> statementSyntaxes, bool needToAppendReturn)
            {
                var max = statementSyntaxes.Count - 1;
                return max < 0 ? [] : FixedSyntaxes([.. statementSyntaxes]);

                List<StatementSyntax> FixedSyntaxes(StatementSyntax[] statementSyntaxArray)
                {
                    List<StatementSyntax> result = [];
                    var finalStatementSyntaxes = FixedSyntax(statementSyntaxArray[max], needToAppendReturn);
                    if (finalStatementSyntaxes.Count == 0)
                    {
                        var penultimate = max - 1;
                        for (var i = 0; i < penultimate; ++i)
                        {
                            result.AddRange(FixedSyntax(statementSyntaxArray[i], needToAppendReturn: IsTaskClass));
                        }

                        if (penultimate >= 0)
                        {
                            result.AddRange(FixedSyntax(statementSyntaxArray[penultimate], needToAppendReturn));
                        }
                    }
                    else
                    {
                        for (var i = 0; i < max; ++i)
                        {
                            result.AddRange(FixedSyntax(statementSyntaxArray[i], needToAppendReturn: IsTaskClass));
                        }

                        result.AddRange(finalStatementSyntaxes);
                    }

                    return result;
                }
            }

            private List<StatementSyntax> FixedSyntax(
                StatementSyntax statementSyntax, bool needToAppendReturn)
                => statementSyntax is ReturnStatementSyntax returnStatementSyntax
                    ? FixedReturnSyntax(returnStatementSyntax, needToAppendReturn)
                    : [FixedNonReturnSyntax(statementSyntax, needToAppendReturn)];

            protected bool IsCompletedTask(ExpressionSyntax returnExpressionSyntax, string completedTaskSymbolName)
            {
                var symbol = _semanticModel.GetSymbolInfo(returnExpressionSyntax, cancellationToken).Symbol;
                return symbol is not null
                    && symbol.ContainingSymbol.EqualsWithDefaultComparer(_taskClassSymbol)
                    && symbol.Name == completedTaskSymbolName;
            }

            private StatementSyntax FixedNonReturnSyntax(
                StatementSyntax statementSyntax, bool needToAppendReturn)
                => statementSyntax switch
                {
                    BlockSyntax blockSyntax => FixedBlockSyntax(blockSyntax, needToAppendReturn),
                    IfStatementSyntax ifStatementSyntax => FixedIfStatementSyntax(ifStatementSyntax, needToAppendReturn),
                    TryStatementSyntax tryStatementSyntax => FixedTryStatementSyntax(tryStatementSyntax, needToAppendReturn),
                    SwitchStatementSyntax switchStatementSyntax => FixedSwitchStatementSyntax(switchStatementSyntax),
                    WhileStatementSyntax whileStatementSyntax => FixedWhileStatementSyntax(whileStatementSyntax),
                    DoStatementSyntax doStatementSyntax => FixedDoStatementSyntax(doStatementSyntax),
                    ForStatementSyntax forStatementSyntax => FixedForStatementSyntax(forStatementSyntax),
                    ForEachStatementSyntax forEachStatementSyntax => FixedForEachStatementSyntax(forEachStatementSyntax),
                    ForEachVariableStatementSyntax forEachVariableStatementSyntax => FixedForEachVariableStatementSyntax(forEachVariableStatementSyntax),
                    UsingStatementSyntax usingStatementSyntax => FixedUsingStatementSyntax(usingStatementSyntax, needToAppendReturn),
                    _ => statementSyntax
                };

            private BlockSyntax FixedBlockSyntax(BlockSyntax blockSyntax, bool needToAppendReturn)
                => SyntaxFactory.Block(
                    blockSyntax.AttributeLists,
                    blockSyntax.OpenBraceToken,
                    new(FixedSyntaxes(blockSyntax.Statements, needToAppendReturn)),
                    blockSyntax.CloseBraceToken);

            private IfStatementSyntax FixedIfStatementSyntax(
                IfStatementSyntax ifStatementSyntax, bool needToAppendReturn)
                => SyntaxFactory.IfStatement(
                    ifStatementSyntax.AttributeLists,
                    ifStatementSyntax.IfKeyword,
                    ifStatementSyntax.OpenParenToken,
                    ifStatementSyntax.Condition,
                    ifStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(ifStatementSyntax.Statement, needToAppendReturn),
                    FixedElseClauseSyntax(ifStatementSyntax.Else, needToAppendReturn));

            private TryStatementSyntax FixedTryStatementSyntax(
                TryStatementSyntax tryStatementSyntax, bool needToAppendReturn)
                => SyntaxFactory.TryStatement(
                    tryStatementSyntax.AttributeLists,
                    tryStatementSyntax.TryKeyword,
                    FixedBlockSyntax(tryStatementSyntax.Block, needToAppendReturn),
                    new(tryStatementSyntax.Catches.Select(catchClauseSyntax => SyntaxFactory.CatchClause(
                        catchClauseSyntax.CatchKeyword,
                        catchClauseSyntax.Declaration,
                        catchClauseSyntax.Filter,
                        FixedBlockSyntax(catchClauseSyntax.Block, needToAppendReturn)))),
                    tryStatementSyntax.Finally);

            private ElseClauseSyntax? FixedElseClauseSyntax(ElseClauseSyntax? elseClauseSyntax, bool needToAppendReturn)
                => elseClauseSyntax is null
                    ? null
                    : SyntaxFactory.ElseClause(
                        elseClauseSyntax.ElseKeyword,
                        FixedSubStatementSyntax(elseClauseSyntax.Statement, needToAppendReturn));

            private WhileStatementSyntax FixedWhileStatementSyntax(WhileStatementSyntax whileStatementSyntax)
                => SyntaxFactory.WhileStatement(
                    whileStatementSyntax.AttributeLists,
                    whileStatementSyntax.WhileKeyword,
                    whileStatementSyntax.OpenParenToken,
                    whileStatementSyntax.Condition,
                    whileStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(whileStatementSyntax.Statement, needToAppendReturn: IsTaskClass));

            private DoStatementSyntax FixedDoStatementSyntax(DoStatementSyntax doStatementSyntax)
                => SyntaxFactory.DoStatement(
                    doStatementSyntax.AttributeLists,
                    doStatementSyntax.DoKeyword,
                    FixedSubStatementSyntax(doStatementSyntax.Statement, needToAppendReturn: IsTaskClass),
                    doStatementSyntax.WhileKeyword,
                    doStatementSyntax.OpenParenToken,
                    doStatementSyntax.Condition,
                    doStatementSyntax.CloseParenToken,
                    doStatementSyntax.SemicolonToken);

            private ForStatementSyntax FixedForStatementSyntax(ForStatementSyntax forStatementSyntax)
                => SyntaxFactory.ForStatement(
                    forStatementSyntax.AttributeLists,
                    forStatementSyntax.ForKeyword,
                    forStatementSyntax.OpenParenToken,
                    forStatementSyntax.Declaration,
                    forStatementSyntax.Initializers,
                    forStatementSyntax.FirstSemicolonToken,
                    forStatementSyntax.Condition,
                    forStatementSyntax.SecondSemicolonToken,
                    forStatementSyntax.Incrementors,
                    forStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(forStatementSyntax.Statement, needToAppendReturn: IsTaskClass));

            private ForEachStatementSyntax FixedForEachStatementSyntax(ForEachStatementSyntax forEachStatementSyntax)
                => SyntaxFactory.ForEachStatement(
                    forEachStatementSyntax.AttributeLists,
                    forEachStatementSyntax.AwaitKeyword,
                    forEachStatementSyntax.ForEachKeyword,
                    forEachStatementSyntax.OpenParenToken,
                    forEachStatementSyntax.Type,
                    forEachStatementSyntax.Identifier,
                    forEachStatementSyntax.InKeyword,
                    forEachStatementSyntax.Expression,
                    forEachStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(forEachStatementSyntax.Statement, needToAppendReturn: IsTaskClass));

            private ForEachVariableStatementSyntax FixedForEachVariableStatementSyntax(
                ForEachVariableStatementSyntax forEachVariableStatementSyntax)
                => SyntaxFactory.ForEachVariableStatement(
                    forEachVariableStatementSyntax.AttributeLists,
                    forEachVariableStatementSyntax.AwaitKeyword,
                    forEachVariableStatementSyntax.ForEachKeyword,
                    forEachVariableStatementSyntax.OpenParenToken,
                    forEachVariableStatementSyntax.Variable,
                    forEachVariableStatementSyntax.InKeyword,
                    forEachVariableStatementSyntax.Expression,
                    forEachVariableStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(
                        forEachVariableStatementSyntax.Statement, needToAppendReturn: IsTaskClass));

            private UsingStatementSyntax FixedUsingStatementSyntax(
                UsingStatementSyntax usingStatementSyntax, bool needToAppendReturn)
                => SyntaxFactory.UsingStatement(
                    usingStatementSyntax.AttributeLists,
                    usingStatementSyntax.AwaitKeyword,
                    usingStatementSyntax.UsingKeyword,
                    usingStatementSyntax.OpenParenToken,
                    usingStatementSyntax.Declaration,
                    usingStatementSyntax.Expression,
                    usingStatementSyntax.CloseParenToken,
                    FixedSubStatementSyntax(usingStatementSyntax.Statement, needToAppendReturn));

            private StatementSyntax FixedSubStatementSyntax(StatementSyntax statementSyntax, bool needToAppendReturn)
            {
                if (statementSyntax is ReturnStatementSyntax returnStatementSyntax)
                {
                    var fixedSyntaxes = FixedReturnSyntax(returnStatementSyntax, needToAppendReturn);
                    return fixedSyntaxes.Count switch
                    {
                        0 => SyntaxFactory.EmptyStatement(
                            statementSyntax.AttributeLists, returnStatementSyntax.SemicolonToken),
                        1 => fixedSyntaxes[0],
                        _ => SyntaxFactory.Block(fixedSyntaxes)
                    };
                }

                return FixedNonReturnSyntax(statementSyntax, needToAppendReturn);
            }

            private SwitchStatementSyntax FixedSwitchStatementSyntax(SwitchStatementSyntax switchStatementSyntax)
                => SyntaxFactory.SwitchStatement(
                    switchStatementSyntax.AttributeLists,
                    switchStatementSyntax.SwitchKeyword,
                    switchStatementSyntax.OpenParenToken,
                    switchStatementSyntax.Expression,
                    switchStatementSyntax.CloseParenToken,
                    switchStatementSyntax.OpenBraceToken,
                    new(switchStatementSyntax.Sections.Select(
                        switchSectionSyntax => SyntaxFactory.SwitchSection(
                            switchSectionSyntax.Labels,
                            new(FixedSyntaxes(switchSectionSyntax.Statements, needToAppendReturn: true))))),
                    switchStatementSyntax.CloseBraceToken);
        }

        private sealed class MakeMethodAsyncFixerForTaskClass(
            SemanticModel semanticModel, INamedTypeSymbol? taskClassSymbol, CancellationToken cancellationToken)
            : MakeMethodAsyncFixer(semanticModel, taskClassSymbol, cancellationToken)
        {
            protected override bool IsTaskClass => true;

            protected override List<StatementSyntax> FixedReturnSyntax(
                ReturnStatementSyntax returnStatementSyntax, bool needToAppendReturn)
            {
                var returnExpressionSyntax = returnStatementSyntax.Expression!;
                List<StatementSyntax> result = IsCompletedTask(returnExpressionSyntax, Constants.CompletedTask)
                    ? []
                    : [SyntaxFactory.ExpressionStatement(SyntaxFactory.AwaitExpression(returnExpressionSyntax))];
                if (needToAppendReturn)
                {
                    result.Add(SyntaxFactory.ReturnStatement(
                        returnStatementSyntax.AttributeLists,
                        SyntaxFactory.Token(SyntaxKind.ReturnKeyword),
                        null,
                        returnStatementSyntax.SemicolonToken));
                }

                return result;
            }
        }

        private sealed class MakeMethodAsyncFixerForTaskGeneric(
            SemanticModel semanticModel, INamedTypeSymbol? taskClassSymbol, CancellationToken cancellationToken)
            : MakeMethodAsyncFixer(semanticModel, taskClassSymbol, cancellationToken)
        {
            protected override bool IsTaskClass => false;

            protected override List<StatementSyntax> FixedReturnSyntax(
                ReturnStatementSyntax returnStatementSyntax, bool needToAppendReturn)
            {
                var returnExpressionSyntax = returnStatementSyntax.Expression!;
                return [
                    SyntaxFactory.ReturnStatement(
                        returnStatementSyntax.AttributeLists,
                        returnStatementSyntax.ReturnKeyword,
                        IsCompletedTask(returnExpressionSyntax, Constants.FromResult)
                            ? ((InvocationExpressionSyntax)returnExpressionSyntax).ArgumentList.Arguments[0].Expression
                            : SyntaxFactory.AwaitExpression(returnExpressionSyntax),
                        returnStatementSyntax.SemicolonToken)];
            }
        }
    }
}

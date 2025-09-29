Feature: ForEachVariableStatement

Analyzer for foreach-variable-statements in non-async Task-returning methods that might access disposed resources

Scenario Outline: a non-async method that returns some Task inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns some Task inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds that diagnostic result and suggests the proper code-fix
Examples:
| SyntaxKind     |
| method         |
| local function |

Scenario Outline: a non-async method that returns some generic-Task inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns some generic-Task inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds that diagnostic result and suggests the proper code-fix
Examples:
| SyntaxKind     |
| method         |
| local function |

Scenario Outline: a non-async method that returns some Task not inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns some Task not inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds no diagnostic results
Examples:
| SyntaxKind     |
| method         |
| local function |

Scenario Outline: a non-async method that returns some generic-Task not inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns some generic-Task not inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds no diagnostic results
Examples:
| SyntaxKind     |
| method         |
| local function |

Scenario Outline: a non-async method that returns a completed Task inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns a completed Task inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds no diagnostic results
Examples:
| SyntaxKind     |
| method         |
| local function |

Scenario Outline: a non-async method that returns a completed generic-Task inside a using-declaration scope in a foreach-variable-statement
	Given a non-async <SyntaxKind> that returns a completed generic-Task inside a using-declaration scope in a foreach-variable-statement
	When UsingAsync diagnostic is performed on the <SyntaxKind>
	Then the analyzer finds no diagnostic results
Examples:
| SyntaxKind     |
| method         |
| local function |

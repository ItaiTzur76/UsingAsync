Feature: IfStatement

Analyzer for if-statements in non-async Task-returning methods that might access disposed resources

Scenario: a non-async method that returns some Task inside a using-declaration scope in a terminal if-statement
	Given a non-async method that returns some Task inside a using-declaration scope in a terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some Task inside a using-declaration scope in a non-terminal if-statement
	Given a non-async method that returns some Task inside a using-declaration scope in a non-terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some generic-Task inside a using-declaration scope in an if-statement
	Given a non-async method that returns some generic-Task inside a using-declaration scope in an if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some Task not inside a using-declaration scope in a terminal if-statement
	Given a non-async method that returns some Task not inside a using-declaration scope in a terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns some Task not inside a using-declaration scope in a non-terminal if-statement
	Given a non-async method that returns some Task not inside a using-declaration scope in a non-terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns some generic-Task not inside a using-declaration scope in an if-statement
	Given a non-async method that returns some generic-Task not inside a using-declaration scope in an if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed Task inside a using-declaration scope in a terminal if-statement
	Given a non-async method that returns a completed Task inside a using-declaration scope in a terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal if-statement
	Given a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed generic-Task inside a using-declaration scope in an if-statement
	Given a non-async method that returns a completed generic-Task inside a using-declaration scope in an if-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns some Task inside a using-declaration scope in a terminal else-statement
	Given a non-async method that returns some Task inside a using-declaration scope in a terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some Task inside a using-declaration scope in a non-terminal else-statement
	Given a non-async method that returns some Task inside a using-declaration scope in a non-terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some generic-Task inside a using-declaration scope in an else-statement
	Given a non-async method that returns some generic-Task inside a using-declaration scope in an else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some Task not inside a using-declaration scope in a terminal else-statement
	Given a non-async method that returns some Task not inside a using-declaration scope in a terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns some Task not inside a using-declaration scope in a non-terminal else-statement
	Given a non-async method that returns some Task not inside a using-declaration scope in a non-terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns some generic-Task not inside a using-declaration scope in an else-statement
	Given a non-async method that returns some generic-Task not inside a using-declaration scope in an else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed Task inside a using-declaration scope in a terminal else-statement
	Given a non-async method that returns a completed Task inside a using-declaration scope in a terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal else-statement
	Given a non-async method that returns a completed Task inside a using-declaration scope in a non-terminal else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed generic-Task inside a using-declaration scope in an else-statement
	Given a non-async method that returns a completed generic-Task inside a using-declaration scope in an else-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

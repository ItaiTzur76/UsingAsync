Feature: UsingStatement

Analyzer for using-statements in non-async Task-returning methods that might access disposed resources

Scenario: a non-async method that returns some Task in a terminal using-statement
	Given a non-async method that returns some Task in a terminal using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some Task in a non-terminal using-statement
	Given a non-async method that returns some Task in a non-terminal using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns some generic-Task in a using-statement
	Given a non-async method that returns some generic-Task in a using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds that diagnostic result and suggests the proper code-fix

Scenario: a non-async method that returns a completed Task in a terminal using-statement
	Given a non-async method that returns a completed Task in a terminal using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed Task in a non-terminal using-statement
	Given a non-async method that returns a completed Task in a non-terminal using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

Scenario: a non-async method that returns a completed generic-Task in a using-statement
	Given a non-async method that returns a completed generic-Task in a using-statement
	When UsingAsync diagnostic is performed
	Then the analyzer finds no diagnostic results

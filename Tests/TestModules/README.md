## Running the EF Core Tests

You only need SQL Server installed and accessible on `localhost`. You need to have permissions to create and delete databases.
Open up the `EfCoreTestSuite.sln` solution.
Restore all the nuget pacakges from all projects.
Run the IDE Unit Test runner for all detected tests.

NOTE: For the short term, please make sure the `Skip` constant is set to `null` when you're running tests locally but when you're checking in any changes that its set to a string value until we can setup integration tests for the CI/CD (I'm very open for improvements).
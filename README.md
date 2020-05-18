A simple polling application

Pre-requisities
---------------
- Install the Cosmos emulator
- Set a user secret of "Cosmos:ConnectionString" to point at the Cosmos emulator

Run the PollApp.Web project in Visual Studio 2019.
Ensure you run with Kestrel and not IIS, the address should be https://localhost:5001/.

The poll results will not update automatically, that is because the PollApp.Worker
needs to be running. The PollApp.Worker listens to the Cosmos Change feed and calculates
the results in the background.

You can run both either setting multiple start-up projects or by using the
dotnet run command in two terminal windows, e.g.

```
dotnet run --project .\PollApp.Web\PollApp.Web.csproj
```
and
```
dotnet run --project .\PollApp.Worker\PollApp.Worker.csproj
```

Sample Cosmos Queries
---------------------
SELECT c.id FROM c WHERE c.Type = "polldocument"

SELECT c.id FROM c WHERE c.Type = "polldocument" and ARRAY_LENGTH(c.PossibleAnswers) > 3
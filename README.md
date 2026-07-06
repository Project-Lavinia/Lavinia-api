# Lavinia-api

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/3b6eab6e34ed49f8b4f376b787d05858)](https://www.codacy.com/gh/Project-Lavinia/Lavinia-api?utm_source=github.com&utm_medium=referral&utm_content=Project-Lavinia/Lavinia-api&utm_campaign=Badge_Grade)

## Prerequisites

- [.NET Core SDK 10.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

## Running the development build

1. Open your favourite terminal and navigate to the repository folder Lavinia-api which contains the file `Api.csproj`
2. Type `dotnet run` <kbd>Enter</kbd>

## Publishing a runnable, cross-platform .dll and running it

1. Repeat steps 1 through 2 from Running the Development build
2. Type `dotnet publish -c Release -o out`, where `out` can be any desired path or folder.
3. Navigate to the `out` directory specified in step 2., type `dotnet Api.dll` and the application should start on port `5000` for http and port `5001` for https.

In both running the development build and publishing a runnable, cross-platform .dll, there will be a terminal window that specifies what link to test the API at, usually in the form `localhost:<port>`. Navigate to `localhost:<port>` for API documentation and available requests.

## Export static client data

The API can export the data consumed by Lavinia-client as static JSON files.

1. Navigate to the `Lavinia-api` repository root.
2. Run:

```bash
dotnet run --project Api/Api.csproj -- --export-static ./assets/election-data
```

The export writes the following files:

- `years.json`
- `parties.json`
- `votes.json`
- `metrics.json`
- `parameters.json`

These files are intended for the client-side static data migration and match the payload shape currently returned by `api/v3.0.0` endpoints used by Lavinia-client.

## Release data bundle

On tagged releases (`*.*.*`), the GitHub Actions release workflow also exports and publishes static data artifacts:

- `election-data.zip` containing:
	- `years.json`
	- `parties.json`
	- `votes.json`
	- `metrics.json`
	- `parameters.json`
	- `manifest.json`
- `manifest.json` as a standalone release asset

The manifest includes:

- `dataVersion` (release tag)
- `sourceCommit` (git SHA)
- `generatedAt` (UTC timestamp)
- `schemaVersion`
- `checksums` (SHA-256 per data file)

This allows Lavinia-client deployments to fetch and pin a specific data bundle version for reproducible builds.

# Lavinia-api

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/3b6eab6e34ed49f8b4f376b787d05858)](https://www.codacy.com/gh/Project-Lavinia/Lavinia-api?utm_source=github.com&utm_medium=referral&utm_content=Project-Lavinia/Lavinia-api&utm_campaign=Badge_Grade)

## Prerequisites

- [.NET Core SDK 6.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Running the development build

1. Open your favourite terminal and navigate to the repository folder Lavinia-api which contains the file `Api.csproj`
2. Type `dotnet run` <kbd>Enter</kbd>

## Publishing a runnable, cross-platform .dll and running it

1. Repeat steps 1 through 2 from Running the Development build
2. Type `dotnet publish -c Release -o out`, where `out` can be any desired path or folder.
3. Navigate to the `out` directory specified in step 2., type `dotnet Api.dll` and the application should start on port `5000` for http and port `5001` for https.

In both running the development build and publishing a runnable, cross-platform .dll, there will be a terminal window that specifies what link to test the API at, usually in the form `localhost:<port>`. Navigate to `localhost:<port>` for API documentation and available requests.

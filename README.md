# Lavinia-api

## Prerequisites

- [.NET Core SDK 2.1.302](https://www.microsoft.com/net/download/dotnet-core/2.1)

## Running the development build

1. Open your favourite terminal and navigate to the repository folder Lavinia-api which contains the file `Lavinia-api.csproj`
2. Type `dotnet restore` <kbd>Enter</kbd>`
3. Type `dotnet run` <kbd>Enter</kbd>

## Publishing a runnable, cross-platform .dll and running it

1. Repeat steps 1 through 2 from Running the Development build
2. Type `dotnet publish -c Release -o out`, where `out` can be any desired path or folder.
3. Navigate to the `out` directory specified in step 2., type `dotnet Lavinia-api.dll` and the application should start on port 5000 for http and port 5001 for https.

In both running the development build and publishing a runnable, cross-platform .dll, there will be a terminal window that specifies what link to test the API at, usually in the form `localhost:<port>`. Navigate to `localhost:<port>` for API documentation and available requests.

## Docker

### Building the Docker image

To build the image use:

```sh
docker build -t lavinia-api .
```

This will pull down the default Dockerfile (Linux-container) and tag it "lavinia-api" which will become the name of the image, this can be changed if desirable.

### Running the Docker image

Lavinia-api exposes port 80, and so this is the port you have to publish on the host system.

```sh
docker run -d -p 8080:80 --name lavinia lavinia-api
```

This will start the image called "lavinia-api", forward its active port 80 in Docker to the host system's port 8080, and will be available on http://localhost:8080.

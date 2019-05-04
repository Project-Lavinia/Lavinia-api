FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

COPY ./Lavinia-api/*.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out
RUN ls -la

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/Lavinia-api/out .
ENTRYPOINT ["dotnet", "Lavinia-api.dll"]
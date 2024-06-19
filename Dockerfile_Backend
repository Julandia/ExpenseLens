FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /App

EXPOSE 8080
EXPOSE 8081
# Copy everything
COPY ./src/BackendService ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "BackendService.dll"]

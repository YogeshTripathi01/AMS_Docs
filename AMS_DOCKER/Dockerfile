# Uses a base image with the full sdk for package restoration and building the application. This particular app uses nodejs on top of dotnet.
# Use the Microsoft supplied image tagged with correct .netcore version Image:Version
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# Uses AMS internal Nuget feed (This is needed if you have any AMS nuget feeds)
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
ARG PAT
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS "{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/alcoholmonitoring/_packaging/ScramVSTS/nuget/v3/index.json\", \"username\":\"ArtifactsDocker\", \"password\":\"${PAT}\"}]}"
# Creates the container directory structure and copies the solution to the container
WORKDIR /build
COPY . ./
WORKDIR /build
# restores the app and install dependencies
RUN dotnet restore ScramNet.Ally.AssignVictimClient.sln
RUN dotnet build "./ScramNet.Ally.AssignVictimClient.sln" -c Debug -o /out

# Use another containter to publish the application
FROM build AS publish
RUN dotnet publish "./ScramNet.Ally.AssignVictimClient.sln" -c Debug -o /out

# Use a runtime image with the published app from the previous container. Again Image:Version
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
COPY --from=publish /out .
# Runs the dll on container runtime
ENTRYPOINT ["dotnet", "ScramNet.Ally.AssignVictimClient.dll"]

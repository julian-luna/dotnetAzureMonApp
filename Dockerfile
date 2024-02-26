# syntax=docker/dockerfile:1

# If you need help, visit the Dockerfile reference guide at
# https://docs.docker.com/go/dockerfile-reference/

# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md

# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

COPY . /source

WORKDIR /source

# Build the application.
RUN dotnet publish --use-current-runtime --self-contained false -o /app

# If you need to enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
################################################################################
# Create a new stage for running the application that contains the minimal
# runtime dependencies for the application. This often uses a different base
# image from the build stage where the necessary files are copied from the build
# stage.
#
# The example below uses an aspnet image as the foundation for running the app.
# It will also use whatever happens to be the most recent version of that tag when you
# build your Dockerfile. If reproducability is important, consider using a more specific
# version (e.g., aspnet:7.0.10-alpine-3.18),
# or SHA (e.g., mcr.microsoft.com/dotnet/aspnet@sha256:f3d99f54d504a21d38e4cc2f13ff47d67235efeeb85c109d3d1ff1808b38d034).
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

ENTRYPOINT ["dotnet", "aspnetcoreapp.dll"]

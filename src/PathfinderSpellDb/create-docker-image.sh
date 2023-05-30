#!/bin/bash

# Build in release mode and put the binaries in a dotnet runtime image.

export DOTNET_RUNNING_IN_CONTAINER=1
dotnet publish -f net7.0 -c Release

docker build -t pfsdb -f Dockerfile-release .

#!/bin/bash

yarn tailwindProd
yarn prod

# "webpack: command not found" ??
# dotnet fable src -c Release && webpack --mode production --env version=$1

docker build -t pfsdb-client -f Dockerfile-release .
#!/bin/bash

# I might need to run vite twice, since the first tailwind run might be against empty files?
# not sure yet.

# yarn prod:vite
# yarn tailwindProd
# yarn prod:vite

# "webpack: command not found" ??
# dotnet fable src -c Release && webpack --mode production --env version=$1

docker build -t pfsdb-client -f Dockerfile-release .
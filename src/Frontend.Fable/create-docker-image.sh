#!/bin/bash

yarn prod

docker build -t pfsdb-client -f Dockerfile-release .
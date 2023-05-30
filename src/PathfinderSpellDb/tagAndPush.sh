#!/bin/bash

docker tag $1 registry.digitalocean.com/mpaige-container-registry/pfsdb:latest
docker tag $1 registry.digitalocean.com/mpaige-container-registry/pfsdb:v$2

docker image ls

docker push registry.digitalocean.com/mpaige-container-registry/pfsdb:latest
docker push registry.digitalocean.com/mpaige-container-registry/pfsdb:v$2

#!/bin/bash

#!/bin/bash

docker tag $1 registry.digitalocean.com/mpaige-container-registry/pfsdb-client:latest
docker tag $1 registry.digitalocean.com/mpaige-container-registry/pfsdb-client:v$2

docker image ls

docker push registry.digitalocean.com/mpaige-container-registry/pfsdb-client:latest
docker push registry.digitalocean.com/mpaige-container-registry/pfsdb-client:v$2

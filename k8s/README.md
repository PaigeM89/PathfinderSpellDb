https://github.com/digitalocean/Kubernetes-Starter-Kit-Developers/blob/main/03-setup-ingress-controller/nginx.md


https://github.com/digitalocean/Kubernetes-Starter-Kit-Developers/blob/main/03-setup-ingress-controller/nginx.md#step-5---configuring-production-ready-tls-certificates-for-nginx

https://www.baeldung.com/ops/kubernetes-ingress-vs-load-balancer

https://github.com/nabsul/kcert

helm install kcert nabsul/kcert -n kcert --debug --set acmeTermsAccepted=true,acmeEmail=paige.m89@gmail.com
helm upgrade kcert nabsul/kcert -n kcert --debug --set acmeTermsAccepted=true,acmeEmail=paige.m89@gmail.com,acmeDirUrl=https://acme-v02.api.letsencrypt.org/directory
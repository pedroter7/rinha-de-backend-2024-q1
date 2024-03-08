FROM traefik:3.0
COPY traefik.yaml /etc/traefik/traefik.yaml
COPY traefik-provider.yaml /etc/traefik/provider.yaml
FROM mcr.microsoft.com/devcontainers/base:bookworm

ARG JUST_VERSION=1.59.0

ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false

RUN arch="$(uname -m)" \
 && curl -fsSL "https://github.com/casey/just/releases/download/${JUST_VERSION}/just-${JUST_VERSION}-${arch}-unknown-linux-musl.tar.gz" \
    | tar -xz -C /usr/local/bin just \
 && just --version

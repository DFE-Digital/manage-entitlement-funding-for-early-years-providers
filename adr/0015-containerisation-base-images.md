# ADR-0015: Containerisation standards

* Status: accepted

## Context and Problem Statement

As the service scales, inconsistent Dockerfile configurations, unoptimised image sizes,
and varying security baselines lead to slower build times, security vulnerabilities,
and deployment inconsistencies.

## Decision Outcome

We establish the following mandatory standards for all containerised applications:

### Base images

Use official, minimal base images (e.g. Alpine or Distroless). Pin
images to specific versions / digests. Floating tags like `latest` are prohibited.

### Multi-stage builds

All Dockerfiles must use multi-stage builds to separate build
dependencies from the runtime artefact, keeping production images lean.

### Security and non-root

Containers must run as a non-root user. Automated
vulnerability scanning (e.g. Trivy) must block CI/CD pipelines upon detecting
critical issues.

### Developer HTTPS certificate

Disable dev certificate generation during container builds. Dockerfile should have
the following step before running any `dotnet` restore or build steps:

```Dockerfile
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false
```

If using GitHub Actions or Azure Pipelines for deployment builds, declare the variable
globally at the top of the workflow file:

```yaml
# .github/workflows/ci.yml
env:
  DOTNET_GENERATE_ASPNET_CERTIFICATE: 'false'
```

set windows-shell := ["powershell.exe", "-NoLogo", "-NoProfile", "-Command"]

export ELEVENTY_ENV := "production"

restore:
    dotnet restore ManageEntitlementFunding.slnx

init signing_key: restore
    @dotnet user-secrets set "DevelopmentJwt:SigningKey" "{{signing_key}}" --project src/ManageEntitlementFunding.Api
    @dotnet user-secrets set "DevelopmentJwt:SigningKey" "{{signing_key}}" --project src/ManageEntitlementFunding.Web
    dotnet dev-certs https --trust

build configuration="Debug": restore (build-app configuration) build-docs

build-app configuration="Debug":
    dotnet build ManageEntitlementFunding.slnx --configuration "{{configuration}}" --no-restore /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary

build-docs:
    npm --prefix docs ci --ignore-scripts
    npm --prefix docs run build

run project="Web":
    dotnet {{ if project == "Web" { "watch" } else { "run" } }} --project "src/ManageEntitlementFunding.{{project}}" {{ if project == "Web" { "run" } else { "" } }} --launch-profile https

test configuration="Debug" *args:
    dotnet test ManageEntitlementFunding.slnx --configuration "{{configuration}}" --verbosity normal --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory coverage {{args}}

check: (build "Release") (test "Release" "--no-build" "--no-restore")

[windows]
doctor:
    @& { \
        function Check-Version { \
            param([string]$Name, [version]$Minimum); \
            try { \
                $tool = Get-Command $Name -ErrorAction Stop; \
                $output = (& $tool --version 2>&1 | Out-String).Trim(); \
                if ($LASTEXITCODE -ne 0) { throw 'version command failed' }; \
                $match = [regex]::Match($output, '\d+\.\d+\.\d+'); \
                if (-not $match.Success) { throw 'could not read version' }; \
                if ([version]$match.Value -lt $Minimum) { throw "found $($match.Value), need >= $Minimum" }; \
                Write-Host "OK: $Name $($match.Value) (>= $Minimum)"; \
                return $true; \
            } catch { \
                Write-Host "FAIL: ${Name}: $($_.Exception.Message)"; \
                return $false; \
            }; \
        }; \
        $results = @( \
            Check-Version dotnet 10.0.100; \
            Check-Version just 1.40.0; \
            Check-Version node 22.16.0; \
            Check-Version npm 10.0.0; \
        ); \
        if ($results -contains $false) { exit 1 }; \
    }

[unix]
doctor:
    @failed=0; \
    check_version() { \
        tool="$1"; minimum="$2"; \
        if ! command -v "$tool" >/dev/null 2>&1; then \
            printf 'FAIL: %s is missing (need >= %s)\n' "$tool" "$minimum"; failed=1; return; \
        fi; \
        if ! output=$("$tool" --version 2>/dev/null); then \
            printf 'FAIL: %s version command failed\n' "$tool"; failed=1; return; \
        fi; \
        version=$(printf '%s\n' "$output" | sed -n 's/^[^0-9]*\([0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*\).*$/\1/p' | head -n 1); \
        if [ -z "$version" ] || ! awk -v actual="$version" -v minimum="$minimum" 'BEGIN { split(actual, found, "."); split(minimum, needed, "."); for (part=1; part<=3; part++) { if (found[part]+0 > needed[part]+0) exit 0; if (found[part]+0 < needed[part]+0) exit 1 } exit 0 }'; then \
            printf 'FAIL: %s found %s, need >= %s\n' "$tool" "${version:-unknown}" "$minimum"; failed=1; \
        else \
            printf 'OK: %s %s (>= %s)\n' "$tool" "$version" "$minimum"; \
        fi; \
    }; \
    check_version dotnet 10.0.100; \
    check_version just 1.40.0; \
    check_version node 22.16.0; \
    check_version npm 10.0.0; \
    exit "$failed"
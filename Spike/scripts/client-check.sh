#!/usr/bin/env bash
# Runs HttpQuery.Client.ProxyChecks (the Intent-generated proxy) against a freshly started HttpQuery.Controllers.Swashbuckle API,
# then stops the API it started. Run from a short path (e.g. a subst drive) if the full path is near 260 characters.
# Usage: client-check.sh [port] [resultsDir]
set -u
here="$(cd "$(dirname "$0")/.." && pwd)"
port="${1:-5360}"
results="${2:-$TEMP/client-check}"
mkdir -p "$results"
(cd "$here/HttpQuery.Controllers.Swashbuckle/HttpQuery.Controllers.Swashbuckle.Api" && env ASPNETCORE_ENVIRONMENT=Development HTTPQUERY_ENSURE_DB=true dotnet run --no-build --no-launch-profile --urls "http://localhost:$port" > "$results/api.log" 2>&1) &
for i in $(seq 1 40); do
  code=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$port/swagger/v1/swagger.json")
  [ "$code" = "200" ] && break
  sleep 1
done
HTTPQUERY_API_URL="http://localhost:$port/" dotnet test "$here/HttpQuery.Client/HttpQuery.Client.ProxyChecks" --logger "trx;LogFileName=client-check.trx" --results-directory "$results" 2>&1 | tee "$results/console.txt" | grep -E "error|Passed!|Failed!|\[FAIL\]"
grep -E "HTTP (QUERY|POST)" "$results/api.log" | head -n 10
taskkill //PID $(powershell -NoProfile -Command "(Get-CimInstance Win32_Process | Where-Object { \$_.CommandLine -like '*--urls http://localhost:$port*' -and \$_.Name -eq 'dotnet.exe' }).ProcessId" | head -n1) //T //F > /dev/null 2>&1
exit 0

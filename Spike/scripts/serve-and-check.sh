#!/usr/bin/env bash
# Starts a Spike API, runs ui-check.js against one of its API-reference pages, saves the OpenAPI document, then stops the API it started.
# Usage: serve-and-check.sh <ApiProjectDir> <port> <docPath> <swagger|scalar> <uiPath> <outDir> [NAME=VALUE ...]
set -u
api_dir="$1"; port="$2"; doc="$3"; kind="$4"; ui="$5"; out="$6"; shift 6
mkdir -p "$out"
log="$out/api.log"
(cd "$api_dir" && env ASPNETCORE_ENVIRONMENT=Development HTTPQUERY_ENSURE_DB=true "$@" dotnet run --no-build --no-launch-profile --urls "http://localhost:$port" > "$log" 2>&1) &
code=000
for i in $(seq 1 40); do
  code=$(curl -s -o "$out/openapi.json" -w "%{http_code}" "http://localhost:$port$doc")
  [ "$code" != "000" ] && break
  sleep 1
done
echo "GET $doc -> $code"
target="http://localhost:$port$ui"
case "$kind" in *-cdn) target="http://localhost:$port|$doc" ;; esac
node "$(dirname "$0")/ui-check.js" "$kind" "$target" "$out/$kind.png" | tee "$out/$kind-report.json"
taskkill //PID $(powershell -NoProfile -Command "(Get-CimInstance Win32_Process | Where-Object { \$_.CommandLine -like '*--urls http://localhost:$port*' -and \$_.Name -eq 'dotnet.exe' }).ProcessId" | head -n1) //T //F > /dev/null 2>&1
exit 0

#!/usr/bin/env bash
# Starts a Spike API, fetches its OpenAPI document and lists each path's operations, then stops the API it started.
# Usage: probe.sh <ApiProjectDir> <port> <docPath> [outFile]
set -u
api_dir="$1"; port="$2"; doc="$3"; out="${4:-$TEMP/probe-openapi.json}"
log="$TEMP/probe-$port.log"
(cd "$api_dir" && ASPNETCORE_ENVIRONMENT=Development exec dotnet run --no-build --no-launch-profile --urls "http://localhost:$port" > "$log" 2>&1) &
pid=$!
code=000
for i in $(seq 1 40); do
  code=$(curl -s -o "$out" -w "%{http_code}" "http://localhost:$port$doc")
  [ "$code" != "000" ] && break
  sleep 1
done
echo "GET $doc -> $code"
if [ "$code" = "200" ]; then
  node -e '
    const d = JSON.parse(require("fs").readFileSync(process.argv[1], "utf8"));
    console.log("openapi:", d.openapi || d.swagger);
    for (const [p, v] of Object.entries(d.paths || {})) console.log(" ", p, Object.keys(v).join(","));
  ' "$out"
else
  grep -E "Exception|ERR|FTL" "$log" | head -n 8
fi
# Stop only the API this script started.
taskkill //PID $(powershell -NoProfile -Command "(Get-CimInstance Win32_Process | Where-Object { \$_.CommandLine -like '*--urls http://localhost:$port*' -and \$_.Name -eq 'dotnet.exe' }).ProcessId" | head -n1) //T //F > /dev/null 2>&1
kill $pid 2>/dev/null
exit 0

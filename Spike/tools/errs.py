import re, sys
seen = []
for line in open(sys.argv[1], encoding='utf-8', errors='replace'):
    m = re.search(r'([^\\/]+\\)*([^\\/]+\.(cs|csproj))\((\d+),(\d+)\): (error|warning) (\w+): (.*?)( \[.*)?$', line.strip())
    if m and (m.group(6) == 'error' or (len(sys.argv) > 2 and sys.argv[2] == 'w')):
        path = re.search(r'Spike\\(.*?\.(cs|csproj))\(', line)
        key = f"{path.group(1) if path else m.group(2)}({m.group(4)}): {m.group(6)} {m.group(7)}: {m.group(8)}"
        if key not in seen:
            seen.append(key)
    elif ': error ' in line and line.strip() not in seen and not m:
        seen.append(line.strip()[-300:])
for s in seen[:80]:
    print(s)
print(f'-- {len(seen)} unique')

"""Classifies every file difference between the Intent-generated MediatR apps (Spike/MediatorSpike) and the Mediator variant (Spike/mediator)."""
import difflib, os, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
B, M = os.path.join(ROOT, 'MediatorSpike'), os.path.join(ROOT, 'mediator')


def files(base):
    out = {}
    for d, dirs, fs in os.walk(base):
        dirs[:] = [x for x in dirs if x not in ('bin', 'obj', 'intent', '.agents', '.vs')]
        fs = [f for f in fs if f != '.gitignore']
        for f in fs:
            p = os.path.join(d, f)
            out[os.path.relpath(p, base).replace('\\', '/')] = p
    return out


b, m = files(B), files(M)
rows = []
for rel in sorted(set(b) | set(m)):
    if rel not in m:
        rows.append(('removed', rel, 0, 0))
    elif rel not in b:
        rows.append(('added', rel, len(open(m[rel], encoding='utf-8').read().splitlines()), 0))
    else:
        a = open(b[rel], encoding='utf-8').read().splitlines()
        c = open(m[rel], encoding='utf-8').read().splitlines()
        if a != c:
            diff = [l for l in difflib.unified_diff(a, c, lineterm='', n=0) if l[:1] in '+-' and not l.startswith(('+++', '---'))]
            rows.append(('changed', rel, sum(1 for l in diff if l.startswith('+')), sum(1 for l in diff if l.startswith('-'))))

unchanged = len(set(b) & set(m)) - sum(1 for r in rows if r[0] == 'changed')
mode = sys.argv[1] if len(sys.argv) > 1 else 'table'
if mode == 'table':
    for kind, rel, add, rem in rows:
        print(f'{kind:8} +{add:<3} -{rem:<3} {rel}')
    print(f'unchanged files: {unchanged}; changed: {sum(1 for r in rows if r[0]=="changed")}; added: {sum(1 for r in rows if r[0]=="added")}; removed: {sum(1 for r in rows if r[0]=="removed")}')
else:
    rel = mode
    a = open(os.path.join(B, rel), encoding='utf-8').read().splitlines()
    c = open(os.path.join(M, rel), encoding='utf-8').read().splitlines()
    print('\n'.join(difflib.unified_diff(a, c, 'MediatorSpike/' + rel, 'mediator/' + rel, lineterm='', n=1)))

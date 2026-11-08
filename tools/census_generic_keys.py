"""Census of generic RESULTS keys (no body: default, Srf*, Flying*, InSpace*, ...)
in the repo's GameData, per package and per file/experiment.

Usage: python -I census_generic_keys.py
"""
import re, io, os, collections
GD = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 'GameData')
GEN = re.compile(r'^(default|Srf|SrfLanded|SrfSplashed|Flying|FlyingLow|FlyingHigh|InSpace|InSpaceLow|InSpaceHigh)\**$')
def blocks(txt):
    for m in re.finditer(r'([^\s{}][^{}\n]*?)\s*\n?\s*\{', txt):
        hdr = m.group(1).strip(); i = m.end(); d = 1; j = i
        while d and j < len(txt):
            c = txt[j]; d += (c == '{') - (c == '}'); j += 1
        yield hdr, txt[i:j-1]
rows = collections.defaultdict(lambda: collections.defaultdict(list))  # (folder,file,exp,hdrcond) -> key -> [tags]
allkeys = collections.Counter()
for root, _, files in os.walk(GD):
    rel = os.path.relpath(root, GD); folder = rel.split(os.sep)[0]
    for f in files:
        if not f.endswith('.cfg'): continue
        txt = io.open(os.path.join(root, f), encoding='utf-8-sig', errors='replace').read()
        txt = re.sub(r'//[^\n]*', '', txt)
        for hdr, body in blocks(txt):
            if not hdr.lstrip('@+$%!-&|').startswith('EXPERIMENT_DEFINITION'): continue
            m = re.search(r'#id\[([^\]]+)\]', hdr) or re.search(r'^\s*id\s*=\s*(\S+)', body, re.M)
            if not m: continue
            exp = m.group(1)
            for h2, b2 in blocks(body):
                if h2.lstrip('@+$%!-&|') != 'RESULTS': continue
                for line in b2.splitlines():
                    if '=' not in line: continue
                    k, v = line.split('=', 1); k = k.strip(); v = v.strip()
                    op = k[0] if k and k[0] in '@%&|!-' else ''
                    kk = k.lstrip('@%&|!-')
                    if GEN.match(kk):
                        c = kk.rstrip('*') or kk
                        rows[(folder, os.path.join(rel, f).replace(os.sep, '/'), exp)][op + c].append(v)
                        allkeys[(folder, op + c)] += 1
print('== generic keys per package')
for (folder, c), n in sorted(allkeys.items()): print('  %-26s %-14s %5d' % (folder, c, n))
print()
print('== per file/experiment: key(n) ... | tag set comparison')
tot_dup = 0; tot_keep = 0
for (folder, path, exp), keys in sorted(rows.items()):
    sit = {k: v for k, v in keys.items() if k.lstrip('@%&|!-') != 'default'}
    if not sit and len(keys.get('default', [])) <= 1 and not any(k != 'default' for k in keys): 
        # single plain default: uninteresting
        if set(keys) == {'default'} : continue
    sets = {k: frozenset(v) for k, v in sit.items()}
    desc = ' '.join('%s(%d)' % (k, len(v)) for k, v in sorted(keys.items()))
    verdict = ''
    if len(sets) >= 2:
        uniq = set(sets.values())
        if len(uniq) == 1: verdict = 'IDENTICAL'; tot_dup += sum(len(v) for v in sit.values()) - len(next(iter(uniq)))
        else:
            common = frozenset.intersection(*sets.values()); union = frozenset.union(*sets.values())
            verdict = 'DIFFERENT (%d of %d shared)' % (len(common), len(union))
    elif len(sets) == 1:
        verdict = 'single situation'
    tags = [t for v in sit.values() for t in v]
    ndef = sum(1 for t in tags if '_default_' in t)
    extra = (' | tag _default_: %d/%d' % (ndef, len(tags))) if tags else ''
    print('%-24s %-62s %-22s %s  => %s%s' % (folder[:24], path.split('/',1)[1][-62:], exp[:22], desc, verdict, extra))
print()
print('rows removable if identical blocks became a single default:', tot_dup)

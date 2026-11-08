"""Simulate the KSOScienceResults tier selection on the repo ScienceDefs (+ stock).

Usage: python -I simulate_tiers.py --ksp <KSP install dir> [experiment@subject ...]
  The KSP install can also be given with the KSP_DIR environment variable.
  It provides the stock ScienceDefs (always read) and the JNSQ body configs
  (biome names, only needed without arguments; skipped if JNSQ is missing).
  no subjects: table of every DeepSpaceStories key with P(DSS) at chance 0.2/0.3/0.5
"""
import re, io, os, collections, sys, argparse
GD = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 'GameData')
ap = argparse.ArgumentParser(description='Simulate the KSOScienceResults tier selection.')
ap.add_argument('--ksp', default=os.environ.get('KSP_DIR'), help='KSP install directory (default: $KSP_DIR)')
ap.add_argument('subjects', nargs='*', metavar='experiment@subject')
ARGS = ap.parse_args()
if not ARGS.ksp: ap.error('KSP install not given: use --ksp or set KSP_DIR')
SQUAD = os.path.join(ARGS.ksp, 'GameData', 'Squad', 'Resources', 'ScienceDefs.cfg')
JNSQB = os.path.join(ARGS.ksp, 'GameData', 'JNSQ', 'JNSQ_Bodies')
if not os.path.isfile(SQUAD): ap.error('stock ScienceDefs not found: ' + SQUAD)
SITS = ['SrfLanded','SrfSplashed','FlyingLow','FlyingHigh','InSpaceLow','InSpaceHigh','InSpace','Flying','Srf']

def blocks(txt):
    """yield (header, body) for every top-level-or-nested node"""
    for m in re.finditer(r'([^\s{}][^{}\n]*?)\s*\n?\s*\{', txt):
        hdr = m.group(1).strip()
        i = m.end(); d = 1; j = i
        while d and j < len(txt):
            c = txt[j]; d += (c == '{') - (c == '}'); j += 1
        yield hdr, txt[i:j-1]

entries = []  # (exp, key, folder)
def harvest(path, folder):
    txt = io.open(path, encoding='utf-8-sig', errors='replace').read()
    txt = re.sub(r'//[^\n]*', '', txt)
    for hdr, body in blocks(txt):
        if not hdr.lstrip('@+$%!-&|').startswith('EXPERIMENT_DEFINITION'): continue
        m = re.search(r'#id\[([^\]]+)\]', hdr) or re.search(r'^\s*id\s*=\s*(\S+)', body, re.M)
        if not m: continue
        exp = m.group(1)
        for h2, b2 in blocks(body):
            if h2.lstrip('@+$%!-&|') != 'RESULTS': continue
            for line in b2.splitlines():
                if '=' in line:
                    k = line.split('=',1)[0].strip().lstrip('@%&|')
                    if k and not k.startswith('!') and not k.startswith('-'):
                        entries.append((exp, k, folder))
for root, _, files in os.walk(GD):
    folder = os.path.relpath(root, GD).split(os.sep)[0]
    for f in files:
        if f.endswith('.cfg'): harvest(os.path.join(root, f), folder)
harvest(SQUAD, 'Squad')

biomes = {}
for f in (os.listdir(JNSQB) if os.path.isdir(JNSQB) else []):
    if not f.endswith('.cfg'): continue
    t = io.open(os.path.join(JNSQB, f), encoding='utf-8-sig', errors='replace').read()
    bm = re.search(r'^\s*name\s*=\s*(\S+)', t, re.M)
    if not bm: continue
    names = [n.replace(' ', '') for n in re.findall(r'Biome\s*\{\s*name\s*=\s*([^\n]+)', t)]
    if names: biomes[bm.group(1)] = names

byexp = collections.defaultdict(list)
for exp, k, folder in entries: byexp[exp].append((k, folder))

def tiers(exp, text):
    out = collections.defaultdict(list); defaults = []
    for k, folder in byexp[exp]:
        c = k.rstrip('*')
        if c == 'default': defaults.append(folder); continue
        if c == '': r = 0
        elif c not in text: continue
        elif text.startswith(c): r = 1000000 + len(c)
        else: r = len(c)
        out[r].append(folder)
    rows = [(('P%d' % (r-1000000)) if r >= 1000000 else ('C%d' % r), out[r]) for r in sorted(out, reverse=True)]
    if defaults: rows.append(('default', defaults))
    return rows

short = {'KSO_DeepSpaceStories':'DSS','KSO_CrowdSourcedScience':'CSS','KSO_JNSQ':'JNSQ','KSO_GEP':'GEP','001_KSO_ScienceRefactor':'SR','Squad':'Squad'}
def fmt(fs):
    c = collections.Counter(short.get(f,f) for f in fs)
    return '+'.join('%s%d' % (k, v) for k, v in sorted(c.items(), key=lambda kv: -kv[1]))

# With arguments: tiers for the given experiment@subject pairs.
if ARGS.subjects:
    for arg in ARGS.subjects:
        exp, text = arg.split('@')
        if exp not in byexp: print(exp, 'NOT FOUND'); continue
        print('==', arg)
        for lab, fs in tiers(exp, text):
            print('  %-8s %4d  %s' % (lab, len(fs), fmt(fs)))
    sys.exit(0)

# Without arguments: every DeepSpaceStories key, its tiers and P(DSS text).
dss = collections.Counter((exp, k.rstrip('*')) for exp, k, f in entries if f == 'KSO_DeepSpaceStories')
P = [0.2, 0.3, 0.5]
print('%-22s %-20s %-30s %s' % ('experiment', 'DSS key (n)', 'subject', 'tiers (top first)  |  P(DSS) at chance ' + '/'.join(str(p) for p in P)))
for (exp, c), n in sorted(dss.items()):
    body = next((c[:-len(s)] for s in SITS if c.endswith(s)), None)
    sit = c[len(body):] if body else ''
    bl = biomes.get(body, [])
    text = c + (bl[0] if bl and sit in ('SrfLanded','SrfSplashed','FlyingLow','InSpaceLow','FlyingHigh') else '')
    rows = tiers(exp, text)
    # probability DSS shows for chance p: walk tiers
    probs = []
    for p in P:
        prob = 0.0; stay = 1.0
        for i, (lab, fs) in enumerate(rows):
            last = (i == len(rows)-1)
            pr_here = stay * (1.0 if last else (1-p))
            share = sum(1 for f in fs if f == 'KSO_DeepSpaceStories') / float(len(fs))
            prob += pr_here * share; stay *= p
        probs.append(prob)
    desc = ' > '.join('%s[%s]' % (lab, fmt(fs)) for lab, fs in rows)
    print('%-22s %-20s %-30s %s  |  %s' % (exp, '%s* (%d)' % (c, n), text, desc, ' / '.join('%.0f%%' % (100*q) for q in probs)))

"""Turn generic situation keys that carry a *_default_NN tag back into `default`,
dropping the duplicates created by the old Srf*/Flying*/InSpace* triplication.
Idempotent; dry run unless --write is given.
Usage: python -I generics_to_default.py [--write]"""
import re, io, os, sys
GD = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 'GameData')
FOLDERS = ['001_KSO_ScienceRefactor', 'KSO_CrowdSourcedScience']
GEN = re.compile(r'^(\s*)(?:Srf|SrfLanded|SrfSplashed|Flying|FlyingLow|FlyingHigh|InSpace|InSpaceLow|InSpaceHigh)\*+(\s*=\s*)(#(?:CSS|ScienceRefactor)_\S*_default_\d+)(.*)$', re.S)
DEF = re.compile(r'^\s*[@%&]?default\s*=\s*(#\S+)')
HDR = re.compile(r'^\s*(?:\ufeff)?[@%+$!&|-]?EXPERIMENT_DEFINITION')
write = '--write' in sys.argv
tot_conv = tot_drop = 0
for folder in FOLDERS:
    for root, _, files in os.walk(os.path.join(GD, folder)):
        for f in sorted(files):
            if not f.endswith('.cfg'): continue
            path = os.path.join(root, f)
            raw = io.open(path, 'rb').read().decode('utf-8')
            lines = raw.splitlines(True)
            out = []; seen = set(); conv = drop = 0
            for idx, line in enumerate(lines):
                body = line.rstrip('\r\n'); eol = line[len(body):]
                if HDR.match(body): seen = set()
                m = DEF.match(body)
                if m: seen.add(m.group(1))
                g = GEN.match(body)
                if not g:
                    out.append(line); continue
                indent, eq, tag, rest = g.groups()
                if tag in seen:
                    drop += 1
                    # a blank line before a dropped line, followed by a blank line or a closing brace: remove it
                    nxt = lines[idx + 1].strip() if idx + 1 < len(lines) else ''
                    if out and out[-1].strip() == '' and (nxt == '' or nxt.startswith('}')):
                        out.pop()
                    continue
                seen.add(tag); conv += 1
                out.append(indent + 'default' + eq + tag + rest + eol)
            if conv or drop:
                print('%-70s default +%-3d  removed %d' % (os.path.relpath(path, GD).replace(os.sep, '/')[-70:], conv, drop))
                tot_conv += conv; tot_drop += drop
                if write: io.open(path, 'wb').write(''.join(out).encode('utf-8'))
print('TOTAL: %d rows turned into default, %d duplicates removed%s' % (tot_conv, tot_drop, '' if write else '  (dry run)'))

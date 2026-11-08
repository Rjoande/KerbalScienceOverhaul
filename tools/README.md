# Maintenance tools

Python 3 scripts used to maintain the ScienceDefs in `GameData/`. They find
`GameData` relative to their own location, so they work from any directory.
Run them with `python -I`.

| Script | What it does |
|---|---|
| `simulate_tiers.py` | Simulates the `KSOScienceResults` plugin: for an `experiment@subject` it lists the matching RESULTS keys grouped into tiers (most specific first) with the packages they come from. Without subjects it prints every DeepSpaceStories key with the chance its text is shown. |
| `census_generic_keys.py` | Lists the generic RESULTS keys (`default`, `Srf*`, `Flying*`, `InSpace*`, ...) per package and per file. |
| `generics_to_default.py` | Turns generic situation keys carrying a `*_default_NN` tag into `default` and drops the duplicates. Dry run unless `--write` is given. |

`simulate_tiers.py` also reads the stock ScienceDefs and, without subjects,
the JNSQ body configs from a KSP install, given with `--ksp` or the
`KSP_DIR` environment variable:

```
python -I simulate_tiers.py --ksp "C:\Games\Kerbal Space Program" crewReport@KerbinSrfLandedLowlands
```

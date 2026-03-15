# Kerbal Science Overhaul

Refactored, localized, and AI-assisted science content for Kerbal Space Program

## Overview

Science experiment results are a central element of Kerbal Space Program’s sense of discovery and immersion. However, the stock game provides only a limited number of results, which quickly become repetitive during long playthroughs.

The well-known **CrowdSourced Science (CSS)** mod significantly expands this aspect of the game by adding thousands of additional experiment results. However, CSS has two structural limitations:

* it does **not support localization**
* it is **intrinsically tied to the stock planetary system**

At the same time, major planet packs such as **JNSQ** add many celestial bodies and biomes without providing corresponding science results. **Grannus Expansion Pack (GEP)** includes some science results, but coverage is only partial.

**Kerbal Science Overhaul (KSO)** was created to fill these gaps while also improving the compatibility and accessibility of several popular science-adding mods.

## Project Scope

| Category | Value |
|--------|--------|
| Generated science results | 21,636 |
| Results for Stock / JNSQ | 16,423 |
| Results for GEP | 5,213 |
| Experiments covered (generation) | 63 |
| Experiments covered (refactoring) | 79 |
| Celestial bodies covered | 48 |
| Biomes covered | 174 |
| Supported science mods | 7 |
| Mod components | 5 |

---

## What This Mod Does

Kerbal Science Overhaul operates on multiple levels.

The project is divided into two main components:

1. **ScienceRefactor** – structural improvements and compatibility fixes
2. **Expanded Science Results** – large-scale additions of new experiment results

---

# ScienceRefactor

The **ScienceRefactor** component standardizes and improves science results from stock KSP and several popular mods.

This component can be installed independently and provides the following improvements:

## 1. Refactoring and Wildcard Support

Some experiments—especially stock experiments—use rigid situation definitions that override results added by other mods.

ScienceRefactor introduces the use of **ModuleManager wildcard conditions (`*`)**, allowing different science result packages to coexist without overriding each other.

Additionally, KSP supports only a single `default` result per experiment. Any additional default entries are ignored by the game. ScienceRefactor converts these additional defaults into appropriate generic situations such as:

* `InSpace*`
* `Srf*`

This ensures that all results remain accessible.

Several minor patch errors in original mod configurations have also been corrected where they prevented results from appearing.

---

## 2. Localization

Many science-adding mods do not support localization.

ScienceRefactor adds **full localization support** to experiment results where this was not originally implemented.

---

## 3. Experiment Compatibility

Different mods sometimes introduce experiments that are conceptually similar but technically separate—for example, multiple types of **camera experiments** provided by different part packs.

This can lead to redundancy and clutter, especially when using tools such as **[x] Science!**.

ScienceRefactor resolves these conflicts through a **hierarchical patch system**, merging conceptually similar experiments into unified experiment definitions where appropriate.

---

# AI-Generated Science Results

The second major component of Kerbal Science Overhaul adds a large collection of newly generated science results.

This includes **over 21,000 new science results** covering most combinations of:

* celestial body
* biome
* experiment
* situation

for:

* **JNSQ**
* **Grannus Expansion Pack**
* stock experiments
* experiments added by supported mods

In addition to covering new planetary systems, these results also fill several gaps left by existing science packs such as CSS.

The generation of this dataset required several months of iterative work to:

* organize the reference material used by the model
* define generation constraints and stylistic guidelines
* develop reliable prompts
* balance creative freedom with strict procedural limits

All generated results were subsequently reviewed before integration (a small number of stray hyphens may still exist.)

## Deep Space Stories (Optional)

Kerbal Science Overhaul also includes a small collection titled **Deep Space Stories**, containing approximately **300 science results** inspired by classic science fiction.

These entries adopt a slightly more cinematic tone and occasionally evoke situations reminiscent of works such as *2001: A Space Odyssey* or *The Martian*.

They were generated using a separate set of stylistic instructions distinct from the main dataset and are intended as a small creative addition alongside the standard science results.

---

# AI Generation Methodology

Science results were generated using **GPT-5.2 Plus** within a controlled project environment.

The generation process relied on a **closed project context**, meaning the model operated only on project-specific files rather than external retrieval or memory systems.

The system included:

* **Core instructions** defining global stylistic and procedural rules
* **Project instructions** describing experiment-specific behavior
* **Reference datasets**, including:

  * body definitions
  * biome information
  * experiment masks
  * science definitions from supported mods
  * project glossary

Prompts were applied using a structured workflow:

* each **experiment** was generated in a dedicated chat
* each prompt targeted **1–5 celestial bodies** depending on biome complexity
* results were then generated and manually reviewed before integration

The project repository includes the datasets and prompt templates used during this process.

---

# Installation

Install the mod components into your **GameData** directory.

### ScienceRefactor

Place the folder:

```
001_KSO_ScienceRefactor
```

into:

```
Kerbal Space Program/GameData
```

This component can be installed independently.

---

### Localized CrowdSourced Science

Kerbal Science Overhaul includes a **localized and revised version of CrowdSourced Science**.

To install it:

1. Remove any existing **CrowdSourcedScience** installation from `GameData`
2. Copy the **CrowdSourcedScience** folder from the KSO release into `GameData`

Do **not merge or overwrite** existing installations, as the internal structure differs.

---

### Planet Pack Results

Install the appropriate science result packages depending on your planet packs:

```
KSO_JNSQ
KSO_GEP
```

Notably, **JNSQ_Science also contains many generic results for stock planets**, meaning it can benefit players even if they are not using JNSQ.

---

### Updating

When updating Kerbal Science Overhaul:

1. Remove the following folders from `GameData`:

```
001_KSO_ScienceRefactor
CrowdSourcedScience
KSO_DeepSpaceStories
KSO_GEP
KSO_JNSQ
```

2. Install the new versions from the release archive.

---

# Supported Mods

The following mods are affected by the **ScienceRefactor compatibility logic** and are also included in the **AI-generated science result datasets** provided in `JNSQ_Science` and `GEP_Science`.

ScienceRefactor may adjust experiment definitions, add wildcard compatibility, and unify conceptually similar experiments introduced by these mods.

Additionally, Kerbal Science Overhaul generates new science results for the experiments introduced by these mods when used with supported planetary systems.

Supported mods include:

* [Bluedog Design Bureau](https://github.com/CobaltWolf/Bluedog-Design-Bureau)
* [Far Future Technologies](https://github.com/post-kerbin-mining-corporation/FarFutureTechnologies)
* [HabTech 2](https://github.com/benjee10/HabTech2)
* [Station Parts Expansion Redux (SSPX)](https://github.com/post-kerbin-mining-corporation/StationPartsExpansionRedux)
* [Tantares](https://github.com/Tantares/Tantares) & [Tantares-SP](https://github.com/Tantares/TantaresSP)
* [Universal Storage 2](https://github.com/linuxgurugamer/universal-storage-2)

### CrowdSourced Science Integration

CrowdSourced Science (CSS) is **not treated as an external dependency**.

Kerbal Science Overhaul includes a **localized and internally revised version of CSS**, which replaces the original installation entirely.

For this reason, the original CSS package should **not be installed alongside KSO**. The version included with Kerbal Science Overhaul is already integrated into the refactored science result system.

---

# Contributing

Localization contributions are welcome. See the [provided documentation](https://github.com/Rjoande/KerbalScienceOverhaul/blob/main/translation/TRANSLATION.md) for information on contributing with translations or language dictionaries.

Kerbal Science Overhaul is a large data-driven project, and there are several other areas where community contributions can be particularly helpful, including:

- improving or expanding science results
- correcting grammar or localization issues
- reporting missing or incorrect experiment situations
- adding support for additional science mods
- expanding coverage for additional planet packs

Because the project relies on a specific workflow and dataset structure, contributors are encouraged to open an Issue or contact the author before preparing large pull requests.

This helps ensure that new contributions remain consistent with the project's structure and style guidelines.

---

# License

This project is released under the MIT License.

---

# Acknowledgements

Kerbal Science Overhaul builds upon the work of many mod authors in the Kerbal Space Program community, including the creators of:

* CrowdSourced Science
* Bluedog Design Bureau
* Tantares
* and many other science-related mods.

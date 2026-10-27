# Changelog

All notable changes to Kerbal Science Overhaul. Each entry matches the notes of the corresponding [GitHub release](https://github.com/Rjoande/KerbalScienceOverhaul/releases).

## v1.2.1 (2026-10-28)

- **Launch pad flame deflector:** Kerbal Konstructs launch pads with pad smoke (e.g. the Tundra Space Center pads) tag their flame deflector as `LaunchpadFX`, and stock KSP turns it into a separate "biome" with a raw, untranslated name. All launch pad results now apply there as well, and the biome is shown as "Launch Pad (Flame Deflector)", localized in Italian and Simplified Chinese. Subjects already in your save keep their old title.

- **Bug fix: misspelled biome keys.** 88 results keyed to `KreenwichSea` and one keyed to `ICeCaps` never matched, so the game showed generic results instead. They now use the correct `KrenwichSea` and `IceCaps` keys.

- **Italian localization fixes:** "CKS" corrected to "CSK" in 54 CrowdSourced Science results, plus a typo in the Tantares ultraviolet camera title.

- **Cleanup:** two `tantares_sp_laser_reflector - Copia.cfg` files renamed to drop the space and the copy suffix.

- **CKAN-ready:** every GameData folder now ships its own `LICENSE.txt`, and the core includes a KSP-AVC `.version` file (KSP 1.12.x). Once indexed, the mod will be available on CKAN as five modules: the core plus the optional CrowdSourced Science, JNSQ, GEP and Deep Space Stories packs.

## v1.2.0 (2026-05-21)

- **Simplified Chinese (zh-cn) localization for CrowdSourced Science:** full coverage of all 4,467 CSS results, contributed by Aebestach.

- **Bug fix:** 16 duplicate keys in `en-us` caused KSP to silently discard one entry per conflict. All affected keys have been corrected.

- **Aebestach contributions patch:** 502 new science results authored by Aebestach, covering bodies and situations absent from CSS, plus experiments CSS never addressed: all 34 Breaking Ground ROC types, deployed science instruments, asteroidSample, cometSample (all period classes), and infraredTelescope. Available in both en-us and zh-cn.

## v1.1.0 (2026-04-30)

Added 65 new science results for the Deep Space Stories pack, inspired by Andy Weir's *Project Hail Mary*.

The full storyline is best experienced with Bluedog Design Bureau and the Grannus Expansion Pack, though stock experiments and Kerbol system will still include selected entries.

## v1.0.1 (2026-03-24)

Small fix for 2 misspelled keys for `bd_camera` on Mun.

## v1.0.0 (2026-03-21)

Initial public release of **Kerbal Science Overhaul Project (KSO)**.

### Highlights

- Full rewrite and expansion of science results, based on CSS-style narrative consistency
- Unified experiment framework across multiple mods (Bluedog, Tantares, SSPX, etc.)
- Biome remapping system aligned with JNSQ bodies and custom taxonomy
- Standardized situation/biome masks and experiment logic
- Modular patch architecture (ScienceRefactor + compatibility layers)

### Content

- Thousands of new and adapted science results
- Consistent tone blending scientific plausibility and Kerbal humor
- Coverage for stock + modded experiments across multiple celestial bodies

### Technical

- Clean localization structure (EN-ready, IT-ready)
- Conflict-safe design with fallback logic
- Scalable JSON-driven generation pipeline

### Notes

- Designed for JNSQ-scale systems and multi-mod environments
- Safe to install or remove mid-career (see README for caveats)

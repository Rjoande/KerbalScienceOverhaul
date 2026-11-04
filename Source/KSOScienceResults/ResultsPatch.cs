// ResultsPatch - specificity-first selection of science result texts.
//
// Stock KSP 1.12.5 (ResearchAndDevelopment.GetResults) picks a result like this:
//   1. exact key match (Results[subject]);
//   2. otherwise every key k with subject.Contains(k.TrimEnd('*')) goes into one
//      pool and one entry is drawn uniformly;
//   3. otherwise Results["default"] (only the first one: duplicate keys are
//      renamed default*, default**, ... by ScienceExperiment.Load and never read);
//   4. otherwise experimentTitle + #autoLOC_6002268.
//
// Step 2 is the problem: a generic key such as "Srf*" is contained in every
// surface subject, so dozens of generic texts drown the two or three texts
// written for the biome. This Prefix replaces steps 1-4 with:
//   - the same candidate test (subject.Contains(clean key), ordinal);
//   - candidates grouped into tiers by specificity: keys that are a prefix of
//     the subject rank above contains-only keys, then longer keys rank higher.
//     An exact match is simply the top tier (and it also includes the
//     "subject*" duplicates stock ignored); a bare "*" is the lowest tier;
//   - all "default" keys form the bottom tier, so several defaults do get used;
//   - starting from the top tier, while a lower tier exists and
//     Random.value < fallbackChance, step down one tier and roll again;
//   - draw uniformly inside the tier reached. Duplicate keys (Key*, Key**)
//     stay separate entries, so biome variety is preserved;
//   - no tier at all: the stock "no results" string.
// The raw dictionary value is returned, exactly as stock does (the caller
// localizes it).
//
// This file holds no KSO-specific key names: it fixes a general KSP behavior
// and could be shipped on its own.

using System;
using System.Collections.Generic;
using System.Text;
using KSP.Localization;
using UnityEngine;

namespace KSOScienceResults
{
    internal static class ResultsPatch
    {
        private const string NoResultsTag = "#autoLOC_6002268";

        // Rank of a prefix-matching key: above any contains-only key.
        private const int PrefixRankOffset = 1000000;

        private sealed class Tier
        {
            public int Rank;
            public readonly List<string> Keys = new List<string>();
        }

        private sealed class TierRankComparer : IComparer<Tier>
        {
            public static readonly TierRankComparer Descending = new TierRankComparer();

            public int Compare(Tier a, Tier b)
            {
                return b.Rank.CompareTo(a.Rank);
            }
        }

        // Harmony prefix for ResearchAndDevelopment.GetResults(string subjectID).
        // Returns false (skips the original) only when it produced a result itself.
        private static bool Prefix(string subjectID, ref string __result)
        {
            try
            {
                Settings.EnsureLoaded();
                if (!Settings.Enabled || subjectID == null)
                {
                    return true;
                }
                string[] parts = subjectID.Split('@');
                if (parts.Length != 2 || parts[1] == null)
                {
                    return true;
                }
                ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(parts[0]);
                if (experiment == null || experiment.Results == null)
                {
                    return true;
                }
                __result = Select(experiment, parts[1]);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(KSOScienceResultsAddon.Tag + "selection failed for '" + subjectID
                    + "', falling back to stock: " + e);
                return true;
            }
        }

        // Pure selection logic; "text" is the part of the subject after '@'.
        internal static string Select(ScienceExperiment experiment, string text)
        {
            Dictionary<string, string> results = experiment.Results;
            List<Tier> tiers = new List<Tier>();
            Tier defaults = null;

            foreach (string key in results.Keys)
            {
                string clean = key.TrimEnd('*');
                int rank;
                if (clean == "default")
                {
                    if (defaults == null)
                    {
                        defaults = new Tier();
                        defaults.Rank = -1;
                    }
                    defaults.Keys.Add(key);
                    continue;
                }
                if (clean.Length == 0)
                {
                    rank = 0; // bare "*": matches everything, least specific
                }
                else if (!text.Contains(clean))
                {
                    continue;
                }
                else if (text.StartsWith(clean, StringComparison.Ordinal))
                {
                    rank = PrefixRankOffset + clean.Length;
                }
                else
                {
                    rank = clean.Length;
                }
                FindOrAddTier(tiers, rank).Keys.Add(key);
            }

            tiers.Sort(TierRankComparer.Descending);
            if (defaults != null)
            {
                tiers.Add(defaults);
            }
            if (tiers.Count == 0)
            {
                return experiment.experimentTitle + Localizer.Format(NoResultsTag);
            }

            int level = 0;
            float chance = Settings.FallbackChance;
            if (chance > 0f)
            {
                while (level < tiers.Count - 1 && UnityEngine.Random.value < chance)
                {
                    level++;
                }
            }
            List<string> pool = tiers[level].Keys;
            string chosen = pool[UnityEngine.Random.Range(0, pool.Count)];

            if (Settings.DebugLog)
            {
                Debug.Log(KSOScienceResultsAddon.Tag + Describe(experiment.id, text, tiers, level, chosen));
            }
            return results[chosen];
        }

        private static Tier FindOrAddTier(List<Tier> tiers, int rank)
        {
            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i].Rank == rank)
                {
                    return tiers[i];
                }
            }
            Tier t = new Tier();
            t.Rank = rank;
            tiers.Add(t);
            return t;
        }

        private static string Describe(string experimentId, string text, List<Tier> tiers, int level, string chosen)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(experimentId).Append('@').Append(text).Append(": tiers");
            for (int i = 0; i < tiers.Count; i++)
            {
                Tier t = tiers[i];
                sb.Append(i == level ? " [" : " ");
                if (t.Rank < 0)
                {
                    sb.Append("default");
                }
                else if (t.Rank >= PrefixRankOffset)
                {
                    sb.Append("prefix/").Append(t.Rank - PrefixRankOffset);
                }
                else
                {
                    sb.Append("contains/").Append(t.Rank);
                }
                sb.Append('=').Append(t.Keys.Count);
                if (i == level)
                {
                    sb.Append(']');
                }
            }
            sb.Append(" -> ").Append(chosen);
            return sb.ToString();
        }
    }
}

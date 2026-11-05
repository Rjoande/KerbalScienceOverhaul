// KSOScienceResults - Kerbal Science Overhaul plugin.
//
// Installs a Harmony prefix on ResearchAndDevelopment.GetResults so that the
// most specific result keys (biome > body/situation > situation > default)
// win, with a configurable chance to fall back one tier at a time for
// variety. See ResultsPatch.cs for the algorithm.
//
// Settings: GameData/001_KSO_ScienceRefactor/Plugins/KSOScienceResults.txt
// (ConfigNode syntax, KSO_RESULTS_SETTINGS node). It is a .txt on purpose:
// KSP and ModuleManager ignore it, so editing it never invalidates the
// ModuleManager cache. The plugin reads it straight from disk and reloads it
// whenever the file changes, even mid-game.
//
// Requires Harmony 2 (HarmonyKSP, GameData/000_Harmony).

using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

[assembly: AssemblyTitle("KSOScienceResults")]
[assembly: AssemblyDescription("Specificity-first science result selection for Kerbal Science Overhaul")]
[assembly: AssemblyVersion("1.3.0.0")]
[assembly: KSPAssembly("KSOScienceResults", 1, 3)]

namespace KSOScienceResults
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class KSOScienceResultsAddon : MonoBehaviour
    {
        public const string Version = "1.3.0";
        public const string Tag = "[KSOScienceResults] ";

        private static bool patched;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (patched)
            {
                return;
            }
            try
            {
                ApplyPatch();
                patched = true;
            }
            catch (Exception e)
            {
                // Also reached when 0Harmony.dll is missing (type load failure).
                Debug.LogError(Tag + "patch NOT installed, stock result selection stays in use ("
                    + e.GetType().Name + ": " + e.Message + ")");
            }
        }

        private static void ApplyPatch()
        {
            MethodInfo target = typeof(ResearchAndDevelopment).GetMethod("GetResults",
                BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (target == null)
            {
                throw new MissingMethodException("ResearchAndDevelopment.GetResults(string) not found");
            }
            MethodInfo prefix = typeof(ResultsPatch).GetMethod("Prefix",
                BindingFlags.Static | BindingFlags.NonPublic);
            Harmony harmony = new Harmony("KSOScienceResults");
            harmony.Patch(target, new HarmonyMethod(prefix));
            Debug.Log(Tag + "v" + Version + ": ResearchAndDevelopment.GetResults patched");
        }
    }

    internal static class Settings
    {
        public const string FileName = "KSOScienceResults.txt";
        public const string NodeName = "KSO_RESULTS_SETTINGS";

        private const bool DefaultEnabled = true;
        private const float DefaultFallbackChance = 0.2f;
        private const float DefaultFallbackWeight = 1f;
        private const bool DefaultDebugLog = false;

        public static bool Enabled = DefaultEnabled;
        public static float FallbackChance = DefaultFallbackChance;
        public static float FallbackWeight = DefaultFallbackWeight;
        public static bool DebugLog = DefaultDebugLog;

        private static string path;
        private static bool loaded;
        private static DateTime stamp;

        // Called before every selection. Cheap: one file timestamp check; the file
        // is parsed again only when it changed (or appeared / disappeared).
        public static void EnsureLoaded()
        {
            try
            {
                if (path == null)
                {
                    string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    path = Path.Combine(dir, FileName);
                }
                DateTime now = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                if (loaded && now == stamp)
                {
                    return;
                }
                loaded = true;
                stamp = now;
                Load(now != DateTime.MinValue);
            }
            catch (Exception e)
            {
                loaded = true;
                Debug.LogWarning(KSOScienceResultsAddon.Tag + "could not read " + FileName
                    + ", keeping current settings: " + e.Message);
            }
        }

        private static void Load(bool fileExists)
        {
            Enabled = DefaultEnabled;
            FallbackChance = DefaultFallbackChance;
            FallbackWeight = DefaultFallbackWeight;
            DebugLog = DefaultDebugLog;

            if (!fileExists)
            {
                Debug.Log(KSOScienceResultsAddon.Tag + FileName + " not found, using defaults: " + Describe());
                return;
            }
            ConfigNode root = ConfigNode.Load(path);
            ConfigNode node = root == null ? null : root.GetNode(NodeName);
            if (node == null)
            {
                Debug.LogWarning(KSOScienceResultsAddon.Tag + "no " + NodeName + " node in " + FileName
                    + ", using defaults: " + Describe());
                return;
            }
            node.TryGetValue("enabled", ref Enabled);
            node.TryGetValue("fallbackChance", ref FallbackChance);
            node.TryGetValue("fallbackWeight", ref FallbackWeight);
            node.TryGetValue("debugLog", ref DebugLog);
            FallbackChance = Mathf.Clamp01(FallbackChance);
            if (FallbackWeight < 0f)
            {
                FallbackWeight = 0f;
            }
            Debug.Log(KSOScienceResultsAddon.Tag + "settings: " + Describe());
        }

        private static string Describe()
        {
            return "enabled=" + Enabled + " fallbackChance=" + FallbackChance
                + " fallbackWeight=" + FallbackWeight + " debugLog=" + DebugLog;
        }
    }
}

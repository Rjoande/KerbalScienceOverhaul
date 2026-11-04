// KSOScienceResults - Kerbal Science Overhaul plugin.
//
// Installs a Harmony prefix on ResearchAndDevelopment.GetResults so that the
// most specific result keys (biome > body/situation > situation > default)
// win, with an optional, configurable chance to fall back one tier at a time
// for variety. See ResultsPatch.cs for the algorithm and
// GameData/001_KSO_ScienceRefactor/Plugins/KSOScienceResults.cfg for the
// settings (KSO_RESULTS_SETTINGS node, ModuleManager-patchable).
//
// Requires Harmony 2 (HarmonyKSP, GameData/000_Harmony).

using System;
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
            if (!patched)
            {
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
            GameEvents.OnGameDatabaseLoaded.Add(OnGameDatabaseLoaded);
        }

        public void OnDestroy()
        {
            GameEvents.OnGameDatabaseLoaded.Remove(OnGameDatabaseLoaded);
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

        // Settings are re-read after every database load (startup and Alt-F12 reload).
        private static void OnGameDatabaseLoaded()
        {
            Settings.Invalidate();
        }
    }

    internal static class Settings
    {
        public const string NodeName = "KSO_RESULTS_SETTINGS";

        public static bool Enabled = true;
        public static float FallbackChance = 0f;
        public static bool DebugLog = false;

        private static bool loaded;

        public static void Invalidate()
        {
            loaded = false;
        }

        // Lazy: GameDatabase is not available when the addon starts (Startup.Instantly).
        // Until the database is ready the defaults above apply.
        public static void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }
            GameDatabase db = GameDatabase.Instance;
            if (db == null || !db.IsReady())
            {
                return;
            }
            loaded = true;
            Enabled = true;
            FallbackChance = 0f;
            DebugLog = false;
            try
            {
                ConfigNode[] nodes = db.GetConfigNodes(NodeName);
                if (nodes == null || nodes.Length == 0)
                {
                    Debug.Log(KSOScienceResultsAddon.Tag + "no " + NodeName + " node, using defaults");
                    return;
                }
                // ModuleManager leaves one final node; if several exist the last one wins.
                ConfigNode node = nodes[nodes.Length - 1];
                node.TryGetValue("enabled", ref Enabled);
                node.TryGetValue("fallbackChance", ref FallbackChance);
                node.TryGetValue("debugLog", ref DebugLog);
                if (FallbackChance < 0f)
                {
                    FallbackChance = 0f;
                }
                else if (FallbackChance > 1f)
                {
                    FallbackChance = 1f;
                }
                Debug.Log(KSOScienceResultsAddon.Tag + "settings: enabled=" + Enabled
                    + " fallbackChance=" + FallbackChance + " debugLog=" + DebugLog);
            }
            catch (Exception e)
            {
                Debug.LogWarning(KSOScienceResultsAddon.Tag + "could not read " + NodeName
                    + ", using defaults: " + e.Message);
            }
        }
    }
}

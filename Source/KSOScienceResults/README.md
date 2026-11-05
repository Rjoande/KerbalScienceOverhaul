# KSOScienceResults

The plugin shipped in `GameData/001_KSO_ScienceRefactor/Plugins/`. It patches
`ResearchAndDevelopment.GetResults` (Harmony prefix) so that the most specific
result keys win: biome over body/situation, body/situation over situation,
anything over `default`. Stock KSP throws every key contained in the subject
into one pool and draws uniformly, so a handful of generic `Srf*` / `InSpace*`
entries outnumber the two or three texts written for a biome. The algorithm
is documented at the top of `ResultsPatch.cs`.

Settings live in `GameData/001_KSO_ScienceRefactor/Plugins/KSOScienceResults.txt`
(`KSO_RESULTS_SETTINGS` node, ConfigNode syntax). The plugin reads the file
from disk and reloads it whenever its timestamp changes; being a `.txt`, it is
invisible to KSP's GameDatabase and to ModuleManager's cache hash.

## Building

Both routes compile the same two source files with C# 5 and write the DLL to
`GameData/001_KSO_ScienceRefactor/Plugins/`. Copying it into a KSP install is
up to you. Reference assemblies are read from a KSP 1.12.5 install that has
HarmonyKSP (`GameData/000_Harmony/0Harmony.dll`) installed.

- **Visual Studio / `dotnet build`**: open `KSOScienceResults.slnx` (or the
  csproj) and build Release. The install path defaults to
  `E:\SteamBackup\Kerbal Space Program`; override it with
  `-p:KspRoot="C:\path\to\KSP"` or edit `KspRoot` in the csproj.
- **No SDK**: run `build.bat` (uses the C# compiler bundled with the .NET
  Framework). Set the `KSP` environment variable when your install path
  differs.

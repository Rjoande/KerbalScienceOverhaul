@echo off
rem Builds KSOScienceResults.dll against KSP's own managed assemblies plus
rem HarmonyKSP's 0Harmony.dll (no SDK or NuGet package required - uses the C#
rem compiler bundled with the .NET Framework, which only speaks C# 5; the
rem csproj is pinned to the same language version). The DLL is written into
rem this repo's GameData/001_KSO_ScienceRefactor/Plugins only; copying it into
rem a KSP install is up to you. KSP below is used for the reference
rem assemblies: set it in the environment when your install path differs.
setlocal
if not defined KSP set "KSP=E:\SteamBackup\Kerbal Space Program"
set "MANAGED=%KSP%\KSP_x64_Data\Managed"
set "HARMONY=%KSP%\GameData\000_Harmony\0Harmony.dll"
set "OUTDIR=%~dp0..\..\GameData\001_KSO_ScienceRefactor\Plugins"
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist "%CSC%" (
  echo csc.exe not found at %CSC%
  exit /b 1
)
if not exist "%MANAGED%\Assembly-CSharp.dll" (
  echo Assembly-CSharp.dll not found in %MANAGED%
  exit /b 1
)
if not exist "%HARMONY%" (
  echo 0Harmony.dll not found at %HARMONY%
  exit /b 1
)
if not exist "%OUTDIR%" mkdir "%OUTDIR%"

"%CSC%" /noconfig /nostdlib+ /target:library /optimize+ /nologo ^
 /out:"%OUTDIR%\KSOScienceResults.dll" ^
 /r:"%MANAGED%\mscorlib.dll" ^
 /r:"%MANAGED%\System.dll" ^
 /r:"%MANAGED%\System.Core.dll" ^
 /r:"%MANAGED%\UnityEngine.dll" ^
 /r:"%MANAGED%\UnityEngine.CoreModule.dll" ^
 /r:"%MANAGED%\Assembly-CSharp.dll" ^
 /r:"%HARMONY%" ^
 "%~dp0KSOScienceResults.cs" ^
 "%~dp0ResultsPatch.cs"

if errorlevel 1 (
  echo BUILD FAILED
  exit /b 1
)
echo Built %OUTDIR%\KSOScienceResults.dll
endlocal

# RotatingCameraLCD (Starter)
Space Engineers Plugin Starter (Rewrite of Avaness' CameraLCD) for VS2022.
NuGet-based references (no local SE required), NUnit tests, classic C# (without init).

## Projects
RotatingCameraLCD.Client (net481) – Plugin, SpaceEngineers.ScriptingReferences, optional deploy via SE_DIR.
RotatingCameraLCD.Shared (netstandard2.0) – Parser & Logic.
RotatingCameraLCD.Tests (net8.0) – NUnit Tests.

## Features
Parser only reads [MultiCamera2LCD] section (see example in DEBUG.md).
Thread-safe scan skeleton; LCD sprite rendering with camera name overlay (placeholder).
No C# 9 features → compatible with Space Engineers plugin builds.

### Requirements
Visual Studio 2022 (17.x)
.NET Framework 4.8.1 Developer Pack
(To run) Space Engineers + Pulsar

## References (NuGet)
SpaceEngineers.ScriptingReferences – bundled SE DLLs
Lib.Harmony – optional for later patches
Tests: NUnit, NUnit3TestAdapter, Microsoft.NET.Test.Sdk

## Setup
Unpack ZIP, open solution.
Build (NuGets will be downloaded automatically).
Optional: set SE_DIR so the DLL is copied to %SE_DIR%\Bin64\Plugins\RotatingCameraLCD\.

## Loader (Pulsar)
Install Pulsar, start SE via Pulsar.
Plugins → enable RotatingCameraLCD → restart if necessary.

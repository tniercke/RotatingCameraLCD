# RotatingCameraLCD (Starter)

Space Engineers **Plugin**-Starter (Rewrite von Avaness' CameraLCD) für **VS2022**.  
NuGet‑basierte Referenzen (kein lokales SE nötig), NUnit‑Tests, klassisches C# (ohne `init`).

## Projekte
- `RotatingCameraLCD.Client` (`net481`) – Plugin, `SpaceEngineers.ScriptingReferences`, optionaler Deploy via `SE_DIR`.
- `RotatingCameraLCD.Shared` (`netstandard2.0`) – Parser & Logik.
- `RotatingCameraLCD.Tests` (`net8.0`) – NUnit Tests.

## Features
- Parser liest **nur** `[MultiCamera2LCD]`‑Abschnitt (siehe Beispiel in `DEBUG.md`).
- Thread‑safe Scan‑Skeleton; LCD‑Sprite‑Rendering mit Kamera‑Namens‑Overlay (Platzhalter).
- **Keine** C#‑9 Features → kompatibel für Space‑Engineers‑Plugin‑Builds.

### Requirements
- Visual Studio 2022 (17.x)
- .NET Framework 4.8.1 Developer Pack
- (Zum Ausführen) Space Engineers + **Pulsar**

### Referenzen (NuGet)
- `SpaceEngineers.ScriptingReferences` – gebündelte SE‑DLLs
- `Lib.Harmony` – optional für spätere Patches
- Tests: `NUnit`, `NUnit3TestAdapter`, `Microsoft.NET.Test.Sdk`

### Setup
1. ZIP entpacken, Solution öffnen.
2. Build (NuGets werden automatisch geladen).
3. Optional `SE_DIR` setzen, damit die DLL nach `%SE_DIR%\Bin64\Plugins\RotatingCameraLCD\` kopiert wird.

## Loader (Pulsar)
1. Pulsar installieren, SE **über Pulsar** starten.
2. **Plugins** → **RotatingCameraLCD** aktivieren → ggf. Neustart.
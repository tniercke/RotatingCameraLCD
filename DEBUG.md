# Debugging & Running the Plugin (Pulsar)

## 1) Optionaler Deploy
Wenn `SE_DIR` gesetzt ist, landen Build-Artefakte in:
```
%SE_DIR%\Bin64\Plugins\RotatingCameraLCD\
```

## 2) Start über Pulsar
Space Engineers über **Pulsar** starten → **Plugins** → **RotatingCameraLCD** aktivieren → ggf. Neustart.

## 3) Debuggen mit Visual Studio
- Debug-Build erstellen
- `Debug → Attach to Process…` → `SpaceEngineers.exe`
- Breakpoints in `RotatingCameraLCDPlugin.cs`

## 4) Test-Setup im Spiel
CustomData eines LCDs:
```ini
[MultiCamera2LCD]
LcdSurfaceIndex = 0
RotateEveryXSeconds = 5
NameFontSize = 0.9f
Camera A | Camera #1
Camera B | Camera #2
```
Zwei Kamera-Blöcke entsprechend benennen. Du siehst einen rotierenden **Namens-Overlay** (Video-Feed ist noch TODO).

## 5) Hinweise
- **Keine C# 9 Features** verwendet → keine `IsExternalInit`-Probleme.
- **Game-Thread** beachten: Spiel-API nur via `MyAPIGateway.Utilities.InvokeOnGameThread(...)` ansprechen.
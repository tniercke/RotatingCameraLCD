# Debugging & Running the Plugin (Pulsar)

## 1) Optional Deploy
If `SE_DIR` is set, build artifacts will be copied to:
```
%SE_DIR%\Bin64\Plugins\RotatingCameraLCD\
```

## 2) Starting via Pulsar
Launch Space Engineers through **Pulsar** → go to **Plugins** → enable **RotatingCameraLCD** → restart if necessary.

## 3) Debugging with Visual Studio
- Build the project in **Debug** mode.
- Go to `Debug → Attach to Process…` → select `SpaceEngineers.exe`.
- Set breakpoints in `RotatingCameraLCDPlugin.cs`.

## 4) In-Game Test Setup
Example `CustomData` for an LCD:
```ini
[MultiCamera2LCD]
LcdSurfaceIndex = 0
RotateEveryXSeconds = 5
NameFontSize = 0.9f
Camera A | Camera #1
Camera B | Camera #2
```
Create two camera blocks with the matching names.  
You should see a rotating **camera name overlay** (video feed is still TODO).

## 5) Notes
- **No C# 9 features** are used → avoids `IsExternalInit` issues.
- **Game thread** rule: only call the game API via `MyAPIGateway.Utilities.InvokeOnGameThread(...)`.

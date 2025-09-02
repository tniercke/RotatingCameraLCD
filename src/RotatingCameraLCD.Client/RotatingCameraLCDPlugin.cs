using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RotatingCameraLCD.Shared;

// Game namespaces
using VRage.Plugins;
using Sandbox.ModAPI;
//using Sandbox.ModAPI.Ingame;
using Sandbox.Game.Entities;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace RotatingCameraLCD
{
    /// <summary>
    /// This is the main class for the Rotating Camera-to-LCD Plugin
    /// </summary>
    public sealed class RotatingCameraLCDPlugin : IPlugin, IDisposable
    {
        private readonly ConcurrentDictionary<long, LcdAssignment> _lcds = new ConcurrentDictionary<long, LcdAssignment>();
        private readonly Timer _scanTimer;
        private volatile bool _initDone;

        /// <summary> default Constructor for the class RotatingCameraLCDPlugin </summary>
        public RotatingCameraLCDPlugin()
        {
            _scanTimer = new Timer(_ => SafeScan(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// initialise the Plugin with the actual game instance
        /// </summary>
        /// <param name="gameInstance">an instance of the current game</param>
        public void Init(object gameInstance)
        {
            MyAPIGateway.Utilities.InvokeOnGameThread(() =>
            {
                try
                {
                    _initDone = true;
                    ScheduleNextScan(TimeSpan.FromSeconds(5));
                }
                catch (Exception e)
                {
                    MyAPIGateway.Utilities.ShowMessage("RotatingCameraLCD", $"Init failed: {e.Message}");
                }
            });
        }

        /// <summary>
        /// Update the registered LCDs
        /// </summary>
        public void Update()
        {
            if (!_initDone) return;

            try
            {
                foreach (var kv in _lcds)
                {
                    kv.Value.Tick();
                }
            }
            catch { }
        }

        /// <summary>
        /// Cleanup on disposeal of the plugin
        /// </summary>
        public void Dispose()
        {
            _scanTimer.Dispose();
        }

        private void ScheduleNextScan(TimeSpan due)
        {
            _scanTimer.Change(due, Timeout.InfiniteTimeSpan);
        }

        private void SafeScan()
        {
            MyAPIGateway.Utilities.InvokeOnGameThread(ScanOnce);
        }

        private void ScanOnce()
        {
            try
            {
                var ents = new HashSet<IMyEntity>();
                MyAPIGateway.Entities.GetEntities(ents, e => e is IMyCubeGrid);
                foreach (var grid in ents.OfType<IMyCubeGrid>())
                {
                    var blocks = new List<IMyTerminalBlock>();
                    MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(grid)?.GetBlocks(blocks);

                    foreach (var b in blocks)
                    {
                        if (TryParseConfig(b, out var cfg, out var surface, out var cameras))
                        {
                            var key = b.EntityId;
                            _lcds.AddOrUpdate(key, _ => new LcdAssignment(surface!, cfg, cameras),
                                                  (_, a) => { a.UpdateConfig(surface!, cfg, cameras); return a; });
                        }
                    }
                }
            }
            finally
            {
                ScheduleNextScan(TimeSpan.FromSeconds(10));
            }
        }

        private static bool TryParseConfig(IMyTerminalBlock block, out CustomDataConfig cfg, out IMyTextSurface? surface, out List<IMyCameraBlock> cameras)
        {
            cfg = default!;
            surface = null;
            cameras = new List<IMyCameraBlock>();

            if (block?.CustomData == null)
                return false;

            cfg = CustomDataParser.Parse(block.CustomData);
            if (cfg.CameraMappings.Count == 0)
                return false;

            var provider = block as Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider;
            if (provider == null)
                return false;

            var idx = Math.Min(Math.Max(cfg.LcdSurfaceIndex, 0), provider.SurfaceCount - 1);

            surface = provider?.GetSurface(idx) as Sandbox.ModAPI.IMyTextSurface;

            var ts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(block.CubeGrid);
            foreach (var pair in cfg.CameraMappings)
            {
                var found = new List<IMyTerminalBlock>();
                ts?.SearchBlocksOfName(pair.CameraBlockName, found, f => f is IMyCameraBlock);
                var cam = found.OfType<IMyCameraBlock>().FirstOrDefault();
                if (cam != null) cameras.Add(cam);
            }
            return surface != null && cameras.Count > 0;
        }

        private sealed class LcdAssignment
        {
            private IMyTextSurface _surface;
            private CustomDataConfig _cfg;
            private readonly List<IMyCameraBlock> _cameras = new List<IMyCameraBlock>();
            private int _index;
            private double _time;

            public LcdAssignment(IMyTextSurface surface, CustomDataConfig cfg, List<IMyCameraBlock> cameras)
            {
                _surface = surface;
                _cfg = cfg;
                _cameras.AddRange(cameras);
                _surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
            }

            public void UpdateConfig(IMyTextSurface surface, CustomDataConfig cfg, List<IMyCameraBlock> cameras)
            {
                _surface = surface;
                _cfg = cfg;
                _cameras.Clear();
                _cameras.AddRange(cameras);
            }

            public void Tick()
            {
                _time += 1.0 / 60.0;
                if (_time >= _cfg.RotateEverySeconds)
                {
                    _time = 0;
                    _index = (_index + 1) % Math.Max(1, _cameras.Count);
                }
                DrawPlaceholder();
            }

            private void DrawPlaceholder()
            {
                using (var frame = _surface.DrawFrame())
                {
                    var size = _surface.SurfaceSize;
                    var bg = new VRage.Game.GUI.TextPanel.MySprite()
                    {
                        Type = VRage.Game.GUI.TextPanel.SpriteType.TEXTURE,
                        Data = "SquareSimple",
                        Position = size * 0.5f,
                        Size = size,
                        Color = new VRageMath.Color(0, 0, 0),
                    };
                    frame.Add(bg);

                    var name = _index < _cfg.CameraMappings.Count ? _cfg.CameraMappings[_index].DisplayName : "Camera";
                    frame.Add(VRage.Game.GUI.TextPanel.MySprite.CreateText(name, "White", VRageMath.Color.White, _cfg.NameFontSize));
                }
            }
        }
    }
}
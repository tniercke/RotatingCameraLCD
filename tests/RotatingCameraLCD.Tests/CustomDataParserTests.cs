using NUnit.Framework;
using RotatingCameraLCD.Shared;
using System.Linq;

namespace RotatingCameraLCD.Tests
{
    [TestFixture]
    public class CustomDataParserTests
    {
        [Test]
        public void Parses_Sample_Config()
        {
            var sample = @"
[MultiCamera2LCD]
LcdSurfaceIndex = 0
RotateEveryXSeconds = 5
NameFontSize = 0.9f
Camera-Name A | Camera Block-Name 1
Camera-Name B | Camera Block-Name 2
Camera-Name C | Camera Block-Name 3
Camera-Name D | Camera Block-Name 4
";
            var cfg = CustomDataParser.Parse(sample);

            Assert.That(cfg.LcdSurfaceIndex, Is.EqualTo(0));
            Assert.That(cfg.RotateEverySeconds, Is.EqualTo(5).Within(0.001));
            Assert.That(cfg.NameFontSize, Is.EqualTo(0.9f).Within(0.001));
            Assert.That(cfg.CameraMappings.Count, Is.EqualTo(4));
            Assert.That(cfg.CameraMappings[0].DisplayName, Is.EqualTo("Camera-Name A"));
            Assert.That(cfg.CameraMappings[cfg.CameraMappings.Count - 1].CameraBlockName, Is.EqualTo("Camera Block-Name 4"));
        }
    }
}
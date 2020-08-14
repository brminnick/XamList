using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamList.UnitTests
{
    class MockDeviceInfo : Xamarin.Forms.Internals.DeviceInfo, Xamarin.Essentials.Interfaces.IDeviceInfo
    {
        public override Size PixelScreenSize { get; } = new Size(0, 0);

        public override Size ScaledScreenSize { get; } = new Size(0, 0);

        public override double ScalingFactor { get; } = 1;

        public string Model { get; } = ".NET Core 3.1";

        public string Manufacturer { get; } = "Microsoft";

        public string Name { get; } = ".NET";

        public string VersionString { get; } = "1.0.0";

        public Version Version { get; } = new Version(1, 0, 0, 0);

        public DevicePlatform Platform { get; } = DevicePlatform.Unknown;

        public DeviceIdiom Idiom { get; } = DeviceIdiom.Unknown;

        public DeviceType DeviceType { get; } = DeviceType.Unknown;
    }
}

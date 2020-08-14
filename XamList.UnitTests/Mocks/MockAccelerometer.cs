using System;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    class MockAccelerometer : IAccelerometer
    {
        public bool IsMonitoring { get; private set; }

        public event EventHandler<AccelerometerChangedEventArgs>? ReadingChanged;
        public event EventHandler? ShakeDetected;

        public void Start(SensorSpeed sensorSpeed) => IsMonitoring = true;

        public void Stop() => IsMonitoring = false;
    }
}

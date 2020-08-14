using System;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    class MockVibration : IVibration
    {
        public void Cancel()
        {
        }

        public void Vibrate()
        {
        }

        public void Vibrate(double duration)
        {
        }

        public void Vibrate(TimeSpan duration)
        {
        }
    }
}

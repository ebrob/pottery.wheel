using System;
using Microsoft.SPOT.Hardware;

namespace PotteryWheel
{
    public class Tachometer
    {
        private readonly InterruptPort _port;
        private long _previousTicks;
        private int _rpm;
        public const long DebounceTicks = 500000;

        public Tachometer(InterruptPort port)
        {
            _port = port;
            _port.OnInterrupt += OnInterrupt;
            _previousTicks = DateTime.Now.Ticks;
        }

        public int Rpm
        {
            get { return _rpm; }
        }

        private void OnInterrupt(uint data1, uint data2, DateTime time)
        {
            var oldTicks = _previousTicks;
            var newTicks = time.Ticks;
            var ticksBetween = newTicks - oldTicks;

            if (ticksBetween < DebounceTicks)
                return;

            _rpm = (int) (Helper.TicksPerMinute/ticksBetween);

            _previousTicks = newTicks;
        }
    }
}
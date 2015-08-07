using System;
using Microsoft.SPOT.Hardware;

namespace PotteryWheel
{
    public class Tachometer
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly InterruptPort _port;
        private long _previousTicks;
        public const long DebounceTicks = 500000;
        public const double NumberOfMagnets = 2.0;

        public Tachometer(Cpu.Pin pin)
        {
            _port = new InterruptPort(pin, false, Port.ResistorMode.Disabled,
                Port.InterruptMode.InterruptEdgeLow);
            _port.OnInterrupt += OnInterrupt;
            _previousTicks = DateTime.Now.Ticks;
        }

        public double Rpm { get; private set; }

        private void OnInterrupt(uint data1, uint data2, DateTime time)
        {
            var oldTicks = _previousTicks;
            var newTicks = time.Ticks;
            var ticksBetween = newTicks - oldTicks;

            if (ticksBetween < DebounceTicks)
                return;

            Rpm = (Helper.TicksPerMinute/(double) ticksBetween)/NumberOfMagnets;

            _previousTicks = newTicks;
        }
    }
}
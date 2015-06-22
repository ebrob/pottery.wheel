using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Math = System.Math;
using SLPWM = SecretLabs.NETMF.Hardware.PWM;

namespace PotteryWheel
{
    internal class Wheel
    {
        private const uint MIN_DURATION = 0;
        private const uint MAX_DURATION = 43350; // 85% of 51 msec
        private const uint RANGE_DURATION = MAX_DURATION - MIN_DURATION;
        private const uint PWM_PERIOD = 51*1000; // 51 milliseconds
        private const double GAMMA_EXPONENT = 2.0;

        private readonly OutputPort _led;
        private readonly AnalogInput _pedal;
        private readonly SLPWM _motorSpeed;

        public Wheel()
        {
            _led = new OutputPort(Pins.ONBOARD_LED, false);
            _pedal = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
            _motorSpeed = new SLPWM(Pins.GPIO_PIN_D5);
        }

        public void WarmUp(int seconds = 4)
        {
            var count = seconds*4;
            var on = true;
            for (var idx = 0; idx < count; idx++)
            {
                _led.Write(on);
                Thread.Sleep(250);
                on = !on;
            }
        }

        public void WaitForPedalInZeroPosition()
        {
            while (true)
            {
                var pedalPosition = GetNormalizedPedalPosition();
                if (pedalPosition < 0.01)
                    return;

                // Blink twice quickly
                _led.Write(true);
                Thread.Sleep(100);
                _led.Write(false);
                Thread.Sleep(100);
                _led.Write(true);
                Thread.Sleep(100);
                _led.Write(false);
                Thread.Sleep(500);
            }
        }

        public void Run()
        {
            var duration = MIN_DURATION;
            _led.Write(true);

            while (true)
            {
                var pedalPosition = GetGammaPedalPosition();
                var targetDuration = (uint) (pedalPosition*RANGE_DURATION) + MIN_DURATION;
                if (duration != targetDuration)
                {
                    duration = targetDuration;
                    _motorSpeed.SetPulse(PWM_PERIOD, duration);
                }
                Thread.Sleep(250);
            }
        }

        private double GetNormalizedPedalPosition()
        {
            var normalized = _pedal.ReadRaw()/1024.0;
            return normalized;
        }

        private double GetGammaPedalPosition()
        {
            var normalized = GetNormalizedPedalPosition();
            var gammaAdjusted = Math.Pow(normalized, GAMMA_EXPONENT);
            return gammaAdjusted;
        }
    }
}
using System.Threading;
using MicroLiquidCrystal;
using MicroLiquidCrystal.MicroLiquidCrystal;
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
        private readonly Lcd _lcd;

        public Wheel()
        {
            _led = new OutputPort(Pins.ONBOARD_LED, false);
            _pedal = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
            _motorSpeed = new SLPWM(Pins.GPIO_PIN_D5);

            var provider = new GpioLcdTransferProvider(
                Pins.GPIO_PIN_D11, // RS
                Pins.GPIO_PIN_D10, // Enable
                Pins.GPIO_PIN_D3, // D4
                Pins.GPIO_PIN_D2, // D5
                Pins.GPIO_PIN_D1, // D6
                Pins.GPIO_PIN_D0); // D7
            _lcd = new Lcd(provider);
            _lcd.Begin(16, 2);
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
            _lcd.Clear();
            _lcd.Home();
            _lcd.Write("Put pedal in");
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write("zero position");

            while (true)
            {
                var rawPosition = GetRawPedalPosition();
                var pedalPosition = GetNormalizedPedalPosition(rawPosition);
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
                var rawPosition = GetRawPedalPosition();
                var pedalPosition = GetGammaPedalPosition(rawPosition);
                var targetDuration = (uint) (pedalPosition*RANGE_DURATION) + MIN_DURATION;
                if (duration != targetDuration)
                {
                    duration = targetDuration;
                    _motorSpeed.SetPulse(PWM_PERIOD, duration);

                    _lcd.Clear();
                    _lcd.Home();
                    _lcd.Write("Raw = " + rawPosition);
                    _lcd.SetCursorPosition(0, 1);
                    _lcd.Write("Duration = " + targetDuration);
                }
                Thread.Sleep(250);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private int GetRawPedalPosition()
        {
            return _pedal.ReadRaw();
        }

        private double GetNormalizedPedalPosition(int rawPos)
        {
            var normalized = rawPos/1024.0;
            return normalized;
        }

        private double GetGammaPedalPosition(int rawPos)
        {
            var normalized = GetNormalizedPedalPosition(rawPos);
            var gammaAdjusted = Math.Pow(normalized, GAMMA_EXPONENT);
            return gammaAdjusted;
        }
    }
}
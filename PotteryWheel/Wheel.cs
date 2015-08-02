using System.Threading;
using MicroLiquidCrystal;
using MicroLiquidCrystal.MicroLiquidCrystal;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Math = System.Math;
using SLPWM = SecretLabs.NETMF.Hardware.PWM;

namespace PotteryWheel
{
    public class Wheel
    {
        private const uint WHY_THE_HECK_DIVIDE_BY_THIS = 1;
        private const uint ABSOLUTE_MAX_DURATION = 43350; // 85% of 51 msec
        private const uint PWM_PERIOD = (51*1000); // 51 milliseconds
        private const double GAMMA_EXPONENT = 1.0;

        private const uint SLOWEST_DURATION = 4000;
        private const uint FASTEST_DURATION = 20000;
        private const uint RANGE_DURATION = FASTEST_DURATION - SLOWEST_DURATION;

        private const uint PEDAL_DEAD_ZONE = 50;

        private readonly OutputPort _led;
        private readonly AnalogInput _pedal;
        private readonly SLPWM _motorSpeed;
        private readonly Lcd _lcd;
        private readonly Tachometer _tach;

        public Wheel()
        {
            _led = new OutputPort(Pins.ONBOARD_LED, false);
            _pedal = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
            _motorSpeed = new SLPWM(Pins.GPIO_PIN_D5);

            var tachSensor = new InterruptPort(Pins.GPIO_PIN_D4, false, Port.ResistorMode.Disabled,
                Port.InterruptMode.InterruptEdgeLow);
            _tach = new Tachometer(tachSensor);

            var lcdProvider = new GpioLcdTransferProvider(
                Pins.GPIO_PIN_D7, // RS
                Pins.GPIO_PIN_D6, // Enable
                Pins.GPIO_PIN_D3, // D4
                Pins.GPIO_PIN_D2, // D5
                Pins.GPIO_PIN_D1, // D6
                Pins.GPIO_PIN_D0); // D7
            _lcd = new Lcd(lcdProvider);
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
                if (rawPosition < PEDAL_DEAD_ZONE)
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
            var duration = SLOWEST_DURATION;
            _led.Write(true);

            _lcd.Clear();
            _lcd.Home();
            _lcd.Write(" Raw   Dur  RPM");
            //          0123456789012345

            while (true)
            {
                var rawPosition = GetRawPedalPosition();
                if (rawPosition < PEDAL_DEAD_ZONE)
                {
                    SetPulse(0);
                    _lcd.SetCursorPosition(0, 1);
                    _lcd.Write("* Dead zone *  ");
                    //          0123456789012345
                    Thread.Sleep(125);
                    continue;
                }

                var pedalPosition = GetGammaPedalPosition(rawPosition);
                var targetDuration = (uint) (pedalPosition*RANGE_DURATION) + SLOWEST_DURATION;
                if (duration != targetDuration)
                {
                    duration = targetDuration;
                    SetPulse(duration);

                    _lcd.SetCursorPosition(0, 1);
                    _lcd.Write("Duration = " + targetDuration);

                    var textPos = Helper.PadInteger4(rawPosition);
                    var textDuration = Helper.PadInteger5((int) targetDuration);
                    var textRpm = Helper.PadInteger4(_tach.Rpm);
                    var finalText = textPos + " " + textDuration + " " + textRpm;
                    _lcd.SetCursorPosition(0, 1);
                    _lcd.Write(finalText);
                }
                Thread.Sleep(125);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void SetPulse(uint duration)
        {
            const uint wthPeriod = PWM_PERIOD/WHY_THE_HECK_DIVIDE_BY_THIS;
            const uint wthAbsoluteMax = ABSOLUTE_MAX_DURATION/WHY_THE_HECK_DIVIDE_BY_THIS;

            var wthDuration = duration/WHY_THE_HECK_DIVIDE_BY_THIS;
            if (wthDuration > wthAbsoluteMax)
                wthDuration = wthAbsoluteMax;

            _motorSpeed.SetPulse(wthPeriod, wthDuration);
        }

        private int GetRawPedalPosition()
        {
            return _pedal.ReadRaw();
        }

        private static double GetGammaPedalPosition(int rawPos)
        {
            var normalized = GetNormalizedPedalPosition(rawPos);
            var gammaAdjusted = Math.Pow(normalized, GAMMA_EXPONENT);
            return gammaAdjusted;
        }

        private static double GetNormalizedPedalPosition(int rawPos)
        {
            var adjustedPos = (double) (rawPos - PEDAL_DEAD_ZONE);
            const double adjustedMax = (double) (1024 - PEDAL_DEAD_ZONE);

            var normalized = adjustedPos/adjustedMax;
            return normalized;
        }
    }
}
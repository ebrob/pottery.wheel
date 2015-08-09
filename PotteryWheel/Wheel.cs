using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PotteryWheel
{
    public class Wheel
    {
        private readonly OutputPort _led;
        private readonly Pedal _pedal;
        private readonly Motor _motorSpeed;
        private readonly CharacterDisplay _charDisplay;
        private readonly Tachometer _tach;
        private readonly SpeedFeedback _speedFeedback;

        public Wheel()
        {
            _led = new OutputPort(Pins.ONBOARD_LED, false);
            _pedal = new Pedal(AnalogChannels.ANALOG_PIN_A0);
            _motorSpeed = new Motor(Pins.GPIO_PIN_D5);
            _tach = new Tachometer(Pins.GPIO_PIN_D4);
            _speedFeedback = new SpeedFeedback();

            _charDisplay = new CharacterDisplay(
                Pins.GPIO_PIN_D7,
                Pins.GPIO_PIN_D6,
                Pins.GPIO_PIN_D3,
                Pins.GPIO_PIN_D2,
                Pins.GPIO_PIN_D1,
                Pins.GPIO_PIN_D0);
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
            _charDisplay.PromptToZeroOutPedal();
            while (true)
            {
                if (_pedal.InDeadZone)
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
            _led.Write(true);
            _charDisplay.StartRunMode();

            while (true)
            {
                if (_pedal.InDeadZone)
                {
                    _motorSpeed.SetPulse(0);
                    _charDisplay.TellUserInDeadZone();
                    Thread.Sleep(125);
                    continue;
                }

                var targetRpm = _pedal.GetTargetRpm();

                _speedFeedback.SetCurrentState(_tach.Rpm);
                _speedFeedback.SetPedalPosition(targetRpm);

                _speedFeedback.Recalculate();
                var targetDuration = _speedFeedback.Duration;
                _motorSpeed.SetPulse(targetDuration);
                _charDisplay.DisplayRunStats(_tach.Rpm, targetRpm, targetDuration);
                Thread.Sleep(125);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
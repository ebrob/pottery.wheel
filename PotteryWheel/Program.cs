using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using SLPWM = SecretLabs.NETMF.Hardware.PWM;

namespace PotteryWheel
{
    public class Program
    {
        public static void Main()
        {
            var MIN = 50.0;
            var MAX = 400.0;
            uint MIN_DURATION = 8000; // 8 msec
            uint MAX_DURATION = 43350; // 85% of 51 msec
            uint RANGE_DURATION = MAX_DURATION - MIN_DURATION;

            var led = new OutputPort(Pins.ONBOARD_LED, false);
            var input = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
            var range = MAX - MIN;
            var pwm = new SLPWM(Pins.GPIO_PIN_D5);

            uint period = 51 * 1000; // 51 milliseconds
            uint duration = MIN_DURATION;
            pwm.SetPulse(period, duration);

            /*
            Duty Cycle  Proportion of "on" time vs. the period. Expressed as a percent with 100% being fully on. This is used only with SetDutyCycle and the default clock rate.
            Period      "Peak to Peak" time. This is in microseconds (1/1,000,000 second). Used in SetPulse.
            Duration    Duration of the "on" time for a cycle. This is also in microseconds. This needs to be less than the Period. Used in SetPulse.
            */

            var on = true;
            while (true)
            {
                double normalized = (double)(input.ReadRaw()) / 1024.0;
                double inverseNormalized = (double)(1023 - input.ReadRaw()) / 1024.0;
                int msec = (int)((inverseNormalized * range) + MIN);
                led.Write(on);
                Thread.Sleep(msec);
                on = !on;

                uint targetDuration = (uint)(normalized * (double)RANGE_DURATION) + MIN_DURATION;
                if (duration != targetDuration)
                {
                    duration = targetDuration;
                    pwm.SetPulse(period, duration);
                }
            }
        }
    }
}

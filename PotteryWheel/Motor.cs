using Microsoft.SPOT.Hardware;
using PWM = SecretLabs.NETMF.Hardware.PWM;

namespace PotteryWheel
{
    public class Motor
    {
        private readonly PWM _pwm;
        private const uint WHY_THE_HECK_DIVIDE_BY_THIS = 2; // 1 = Netduino A / 2 = Netduino B
        private const uint ABSOLUTE_MAX_DURATION = 43350; // 85% of 51 msec
        private const uint PWM_PERIOD = (51*1000); // 51 milliseconds

        public Motor(Cpu.Pin pin)
        {
            _pwm = new PWM(pin);
        }

        public void SetPulse(uint duration)
        {
            const uint wthPeriod = PWM_PERIOD/WHY_THE_HECK_DIVIDE_BY_THIS;
            const uint wthAbsoluteMax = ABSOLUTE_MAX_DURATION/WHY_THE_HECK_DIVIDE_BY_THIS;

            var wthDuration = duration/WHY_THE_HECK_DIVIDE_BY_THIS;
            if (wthDuration > wthAbsoluteMax)
                wthDuration = wthAbsoluteMax;

            _pwm.SetPulse(wthPeriod, wthDuration);
        }
    }
}
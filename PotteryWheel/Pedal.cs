using Microsoft.SPOT.Hardware;
using Math = System.Math;

namespace PotteryWheel
{
    public class Pedal
    {
        private readonly AnalogInput _input;
        private const double GAMMA_EXPONENT = 1.0;
        private const uint PEDAL_DEAD_ZONE = 50;

        public Pedal(Cpu.AnalogChannel channel)
        {
            _input = new AnalogInput(channel);
        }

        public bool InDeadZone
        {
            get
            {
                var rawPosition = GetRawPosition();
                return rawPosition < PEDAL_DEAD_ZONE;
            }
        }

        public int GetRawPosition()
        {
            return _input.ReadRaw();
        }

        public double GetAdjustedPosition()
        {
            var raw = GetRawPosition();
            var gamma = GetGammaPedalPosition(raw);
            return gamma;
        }

        public double GetTargetRpm()
        {
            var raw = GetRawPosition();
            var gamma = GetGammaPedalPosition(raw);
            var rpmRange = SpeedFeedback.MaxRpm - SpeedFeedback.MinRpm;
            return gamma*rpmRange;
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
namespace PotteryWheel
{
    public class SpeedFeedback
    {
        private const long SLOWEST_DURATION = 4000;
        private const long FASTEST_DURATION = 20000;
        private const double DURATION_PER_RPM = 40.0;

        private double _targetRpm;
        private double _currentRpm;

        private uint _previousDuration;

        public const double MinRpm = 40.0;
        public const double MaxRpm = 300.0;

        public void SetCurrentState(double rpm)
        {
            _currentRpm = rpm;
        }

        public void SetPedalPosition(double rpm)
        {
            _targetRpm = rpm;
        }

        public void Recalculate()
        {
            _previousDuration = Duration;

            var diffRpm = _targetRpm - _currentRpm;
            var newDuration = _previousDuration + (long) (diffRpm*DURATION_PER_RPM);

            // Range check
            if (newDuration < SLOWEST_DURATION)
                newDuration = SLOWEST_DURATION;
            if (newDuration > FASTEST_DURATION)
                newDuration = FASTEST_DURATION;

            Duration = (uint) newDuration;
        }

        public uint Duration { get; private set; }
    }
}
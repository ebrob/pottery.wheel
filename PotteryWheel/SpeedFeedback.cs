namespace PotteryWheel
{
    public class SpeedFeedback
    {
        private const long SLOWEST_DURATION = 4000;
        private const long FASTEST_DURATION = 20000;
        private const double DELTA_DURATION_PER_RPM = 5.0; // 40.0?

        private double _targetRpm;
        private double _currentRpm;

        public const double MinRpm = 40.0;
        public const double MaxRpm = 300.0;

        public void SetCurrentRpm(double rpm)
        {
            _currentRpm = rpm;
        }

        public void SetTargetRpm(double rpm)
        {
            _targetRpm = rpm;
        }

        public void Recalculate()
        {
            var diffRpm = _targetRpm - _currentRpm;
            var newDuration = Duration + (long) (diffRpm*DELTA_DURATION_PER_RPM);

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
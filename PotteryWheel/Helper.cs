namespace PotteryWheel
{
    public static class Helper
    {
        // One tick equals 100 nanoseconds
        public const long TicksPerMinute = 600000000;

        public static string PadInteger4(int value)
        {
            if (value < 10)
                return "   " + value;
            if (value < 100)
                return "  " + value;
            if (value < 1000)
                return " " + value;
            return value.ToString();
        }

        public static string PadInteger5(int value)
        {
            if (value < 10)
                return "    " + value;
            if (value < 100)
                return "   " + value;
            if (value < 1000)
                return "  " + value;
            if (value < 10000)
                return " " + value;
            return value.ToString();
        }
    }
}
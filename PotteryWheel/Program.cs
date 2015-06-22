namespace PotteryWheel
{
    public class Program
    {
        public static void Main()
        {
            var wheel = new Wheel();
            wheel.WarmUp();
            wheel.WaitForPedalInZeroPosition();
            wheel.Run();
        }
    }
}
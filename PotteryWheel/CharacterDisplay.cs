using MicroLiquidCrystal;
using MicroLiquidCrystal.MicroLiquidCrystal;
using Microsoft.SPOT.Hardware;

namespace PotteryWheel
{
    public class CharacterDisplay
    {
        private readonly Lcd _lcd;

        public CharacterDisplay(Cpu.Pin rs, Cpu.Pin enable, Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
        {
            var lcdProvider = new GpioLcdTransferProvider(rs, enable, d4, d5, d6, d7);
            _lcd = new Lcd(lcdProvider);
            _lcd.Begin(16, 2);
        }

        public void PromptToZeroOutPedal()
        {
            _lcd.Clear();
            _lcd.Home();
            _lcd.Write("Put pedal in");
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write("dead zone");
        }

        public void StartRunMode()
        {
            _lcd.Clear();
            _lcd.Home();
            //_lcd.Write(" Raw   Dur  RPM");
            _lcd.Write(" Tgt  Act    Dur");
            //          0123456789012345
        }

        public void TellUserInDeadZone()
        {
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write("* Dead zone *  ");
            //          0123456789012345
        }

        public void DisplayRunStats(double currentRpm, double targetRpm, uint targetDuration)
        {
            var textTargetRpm = Helper.PadInteger4((int) targetRpm);
            var textCurrentRpm = Helper.PadInteger4((int) currentRpm);
            var textDuration = Helper.PadInteger5((int) targetDuration);
            var finalText = textTargetRpm + " " + textCurrentRpm + "  " + textDuration;
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(finalText);
        }
    }
}
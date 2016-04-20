using System;
using Windows.Devices.I2c;
using GrovePi.Common;

namespace GrovePi.I2CDevices
{
    public interface IRgbLcdDisplay
    {
        IRgbLcdDisplay Initialize();
        IRgbLcdDisplay SetBacklightRgb(byte red, byte green, byte blue);
        IRgbLcdDisplay SetText(string text);
    }

    internal sealed class RgbLcdDisplay : IRgbLcdDisplay
    {
        private const byte RedCommandAddress = 4;
        private const byte GreenCommandAddress = 3;
        private const byte BlueCommandAddress = 2;

        private const byte ControlByteCommand = 0x80;
        private const byte ControlByteData = 0x40;

        /*
        #define LCD_CLEARDISPLAY 0x01
        #define LCD_RETURNHOME 0x02
        #define LCD_ENTRYMODESET 0x04
        #define LCD_DISPLAYCONTROL 0x08
        #define LCD_CURSORSHIFT 0x10
        #define LCD_FUNCTIONSET 0x20
        #define LCD_SETCGRAMADDR 0x40
        #define LCD_SETDDRAMADDR 0x80*/
        private const byte ClearDisplayCommandAddress = 0x01;
        private const byte ReturnHomeCommandAddress = 0x02;
        private const byte EntryModeSetCommandAddress = 0x04;
        private const byte DisplayOnCommandAddress = 0x08;
        private const byte CursorShiftCommandAddress = 0x10;
        private const byte FunctionSetCommandAddress = 0x20;
        private const byte SetCharacterCommandAddress = 0x40;
        private const byte TextCommandAddress = 0x80;

        private const byte NoCursorCommandAddress = 0x04;
        private const byte TwoLinesCommandAddress = 0x28;

        /*/ flags for display entry mode
        #define LCD_ENTRYRIGHT 0x00
        #define LCD_ENTRYLEFT 0x02
        #define LCD_ENTRYSHIFTINCREMENT 0x01
        #define LCD_ENTRYSHIFTDECREMENT 0x00*/
        private const byte LCD_ENTRYRIGHT = 0x00;
        private const byte LCD_ENTRYLEFT = 0x02;
        private const byte LCD_ENTRYSHIFTINCREMENT = 0x01;
        private const byte LCD_ENTRYSHIFTDECREMENT = 0x00;

        /*// flags for display on/off control
        #define LCD_DISPLAYON 0x04
        #define LCD_DISPLAYOFF 0x00
        #define LCD_CURSORON 0x02
        #define LCD_CURSOROFF 0x00
        */
        private const byte LCD_DISPLAYON = 0x04;
        private const byte LCD_DISPLAYOFF = 0x00;
        private const byte LCD_CURSORON = 0x02;
        private const byte LCD_CURSOROFF = 0x00;

        private const byte LCD_2LINE = 0x08;


        internal RgbLcdDisplay(I2cDevice rgbDevice, I2cDevice textDevice)
        {
            if (rgbDevice == null) throw new ArgumentNullException(nameof(rgbDevice));
            if (textDevice == null) throw new ArgumentNullException(nameof(textDevice));

            RgbDirectAccess = rgbDevice;
            TextDirectAccess = textDevice;
        }

        internal I2cDevice RgbDirectAccess { get; }
        internal I2cDevice TextDirectAccess { get; }

        private void Command(byte cmd)
        {
            TextDirectAccess.Write(new[] { ControlByteCommand, cmd });
        }

        private void Data(byte data)
        {
            TextDirectAccess.Write(new[] { ControlByteCommand, data });
        }


        public IRgbLcdDisplay Initialize()
        {
            //FUNCTION_SET
            Delay.Milliseconds(50);

            Command((byte)(FunctionSetCommandAddress | LCD_2LINE));
            Delay.Milliseconds(4);
            Command((byte)(FunctionSetCommandAddress | LCD_2LINE));
            Delay.Milliseconds(1);

            Command((byte)(DisplayOnCommandAddress | LCD_DISPLAYON | LCD_CURSOROFF));

            Command((byte)ClearDisplayCommandAddress);
            Delay.Milliseconds(2);

            Command((byte)(EntryModeSetCommandAddress|LCD_ENTRYLEFT|LCD_ENTRYSHIFTDECREMENT));


            return this;
        }

        public IRgbLcdDisplay SetBacklightRgb(byte red, byte green, byte blue)
        {
            //TODO: Find out what these addresses are for , set const.
            RgbDirectAccess.Write(new byte[] {0, 0});
            RgbDirectAccess.Write(new byte[] {1, 0});
            RgbDirectAccess.Write(new byte[] { DisplayOnCommandAddress, 0xaa});
            RgbDirectAccess.Write(new[] {RedCommandAddress, red});
            RgbDirectAccess.Write(new[] {GreenCommandAddress, green});
            RgbDirectAccess.Write(new[] {BlueCommandAddress, blue});
            return this;
        }

        public IRgbLcdDisplay SetText(string text)
        {
            TextDirectAccess.Write(new[] {TextCommandAddress, ClearDisplayCommandAddress});
            Delay.Milliseconds(50);
            TextDirectAccess.Write(new[] {TextCommandAddress, (byte)(DisplayOnCommandAddress | NoCursorCommandAddress)});
            TextDirectAccess.Write(new[] {TextCommandAddress, TwoLinesCommandAddress});

            var count = 0;
            var row = 0;

            foreach (var c in text)
            {
                if (c.Equals('\n') || count == Constants.GroveRgpLcdMaxLength)
                {
                    count = 0;
                    row += 1;
                    if (row == Constants.GroveRgpLcdRows)
                        break;
                    TextDirectAccess.Write(new byte[] {TextCommandAddress, 0xc0}); //TODO: find out what this address is
                    if (c.Equals('\n'))
                        continue;
                }
                count += 1;
                TextDirectAccess.Write(new[] {SetCharacterCommandAddress, (byte) c});
            }

            return this;
        }
    }
}
 
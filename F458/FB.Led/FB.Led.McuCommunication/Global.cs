using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB.Led.McuCommunication
{
    class Global
    {
        public const string SourceWihteLed = ".\\WHITE_LED_TEST.bmp";
        public const string WhiteLedSlot1 = ".\\Solt1_White_LED.bmp";
        public const string WhiteLedSlot2 = ".\\Solt2_White_LED.bmp";
        public const string SourceRedLed = ".\\RED_LED_TEST.bmp";
        public const string RedLedSlot1 = ".\\Solt1_Red_LED.bmp";
        public const string RedLedSlot2 = ".\\Solt2_Red_LED.bmp";
        public const string SourceGreenLed = ".\\GREED_LED_TEST.bmp";
        public const string GreenLedSlot1 = ".\\Solt1_Green_LED.bmp";
        public const string GreenLedSlot2 = ".\\Solt2_Green_LED.bmp";
        public const string DeviceKeyPath = "D:\\config\\DeviceKey.txt";
        public const string ParameterPath = "D:\\config\\Parameter.txt";

        public static int WhiteExpTime = 0;
        public static int PictureTime = 0;
        public static int CaremaFactor = 0;
        public static int RedExpTime = 0;
        public static int GreenExpTime = 0;

        public static int Up_X = 0;
        public static int Up_Y = 0;
        public static int Down_X = 0;
        public static int Down_Y = 0;


        public static int Noise = 0;
        public static int HeightPix = 0;

        public static int rect_width_up = 0;
        public static int rect_width_dowm = 0;
        public static int rect_height = 0;

        public static int SensorOffSet = 0;
        public static int CoverLimit = 0;
        public static string DeviceKey;
        public static bool ThreadStop = false;
    }
}

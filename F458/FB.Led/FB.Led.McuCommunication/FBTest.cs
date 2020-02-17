using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FB.Led.McuCommunication
{
    class FBTest
    {
        static Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
        static List<string> listKeys = new List<string>();
        public static List<double> Cover1 = new List<double>();
        public static List<double> Cover2 = new List<double>();
        static double slot1_max_press_value = 0;
        static double slot2_max_press_value = 0;
        public static bool DeviceInitial()
        {
            bool isSuccess = false;
            isSuccess = DaHuaCamera.CameraInitial();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("CameraInitial fail.");
                return false;
            }
            isSuccess = ForceSensorClass.OpenForceSensorPort();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Open force sensor fail.");
                return false;
            }
            isSuccess = McuContorlClass.McuInitial();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Mcu initial fail.");
                return false;
            }
            return true;
        }
        public static bool DeviceClose()
        {
            bool isSuccess = false;
            isSuccess = DaHuaCamera.CameraAction(DaHuaCamera.CameraClose);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Camera close fail.");
                return false;
            }
            isSuccess = ForceSensorClass.CloseForceSensorPort();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Close force sensor fail.");
                return false;
            }
            isSuccess = McuContorlClass.McuClose();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Mcu close fail.");
                return false;
            }
            return true;
        }
        public static bool Mcu_Communication(string command)
        {
            bool isSuccess = false;
            isSuccess = McuContorlClass.McuCommunication(command);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Muc comunication fail.\n");
                return false;
            }
            return true;
        }
        public static bool Check_Ready()
        {
            McuContorlClass.CheckReady();
            return true;
        }
        public static bool Ready_Test()
        {
            McuContorlClass.ReadyTest();
            return true;
        }
        public static bool Usb_In()
        {
            bool isSuccess = false;
            isSuccess = McuContorlClass.UsbIn();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("USB in Error.\n");
                return false;
            }
            return true;
        }
        public static bool Usb_Out()
        {
            bool isSuccess = false;
            isSuccess = McuContorlClass.UsbOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("USB out Error.\n");
                return false;
            }
            return true;
        }
        public static bool Cover_Test()
        {
            bool isSuccess = false;
            isSuccess = McuContorlClass.CoverOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Cover Out Error.\n");
                return false;
            }
            isSuccess = McuContorlClass.ElecCoverOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Elec Cover Out Error.\n");
                return false;
            }
            isSuccess = McuContorlClass.ElecCoverIn();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Elec Cover In Error.\n");
                return false;
            }
            Thread CoverTestworkThread = new Thread(new ThreadStart(GetSensorData));
            CoverTestworkThread.Start();
            isSuccess = McuContorlClass.CoverIn();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Cover In Error.\n");
                return false;
            }
            isSuccess = McuContorlClass.CoverOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Cover In Error.\n");
                return false;
            }
            isSuccess = McuContorlClass.ElecCoverOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Elec Cover Out Error.\n");
                return false;
            }
            isSuccess = McuContorlClass.ElecCoverIn();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Elec Cover In Error.\n");
                return false;
            }
            SaveSensorData();
            Console.WriteLine("Slot1_Cover_END_PASS");
            Console.WriteLine("Slot2_Cover_END_PASS");
            Console.WriteLine("COVER_TEST_OK");
            return true;
        }
        public static bool Button_Push()
        {
            bool isSuccess = false;
            isSuccess = McuContorlClass.ButtonIn();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Button In Error.\n");
                return false;
            }
            Thread.Sleep(50);
            isSuccess = McuContorlClass.ButtonOut();
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Button Out Error.\n");
                return false;
            }
            return true;
        }
        public static bool White_Led_Test_OneByOne()
        {
            bool isSuccess = false;
            double GetExpTime = 0;
            isSuccess = DaHuaCamera.CameraSetExptime(Global.WhiteExpTime, out GetExpTime);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Set camera exptime fail.\n");
                return false;
            }
            isSuccess = DaHuaCamera.CameraAction(1, Global.SourceWihteLed);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Camera capture fail.");
                return false;
            }
            isSuccess = ImageProcess.PictureCut(Global.SourceWihteLed, Global.WhiteLedSlot1, Global.WhiteLedSlot2);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Picture cut fail.");
                return false;
            }
            double BrightnessSlot1 = 0;
            isSuccess = ImageProcess.SingleWhiteLedTest(Global.WhiteLedSlot1, Global.Up_X, Global.Up_Y, out BrightnessSlot1, GetExpTime);
            if (!isSuccess)
            {
                LogFile.OpenFile(".//Slot1_White.txt");
                LogFile.WriteToFile("slot1_White_LED_bri=0");
                LogFile.CloseFile();
                Console.WriteLine("slot1_White_LED_bri=0");
                LogFile.WriteLogToFile("Single White Led Test Slot1 fail.\n");
                return false;
            }
            double BrightnessSlot2 = 0;
            isSuccess = ImageProcess.SingleWhiteLedTest(Global.WhiteLedSlot2, Global.Down_X, Global.Down_Y, out BrightnessSlot2, GetExpTime);
            if (!isSuccess)
            {
                LogFile.OpenFile(".//Slot2_White.txt");
                LogFile.WriteToFile("slot2_White_LED_bri=0");
                LogFile.CloseFile();
                Console.WriteLine("slot1_White_LED_bri=0");
                LogFile.WriteLogToFile("Single White Led Test Slot2 fail.\n");
                return false;
            }
            decimal Bri1 = Math.Round(Convert.ToDecimal(BrightnessSlot1), 4, MidpointRounding.AwayFromZero);
            decimal Bri2 = Math.Round(Convert.ToDecimal(BrightnessSlot2), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("slot1_White_LED_bri =" + Bri1);
            Console.WriteLine("slot2_White_LED_bri =" + Bri2);
            LogFile.OpenFile(".//Slot1_White.txt");
            LogFile.WriteToFile("slot1_White_LED_bri =" + Bri1);
            LogFile.CloseFile();
            LogFile.OpenFile(".//Slot2_White.txt");
            LogFile.WriteToFile("slot2_White_LED_bri =" + Bri2);
            LogFile.CloseFile();
            return true;
        }
        public static bool White_Led_Test_Average()
        {
            bool isSuccess = false;
            double GetExpTime = 0;
            isSuccess = DaHuaCamera.CameraSetExptime(Global.WhiteExpTime, out GetExpTime);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Set camera exptime fail.\n");
                return false;
            }
            isSuccess = DaHuaCamera.CameraAction(1, Global.SourceWihteLed);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Camera capture fail.\n");
                return false;
            }
            isSuccess = ImageProcess.PictureCut(Global.SourceWihteLed, Global.WhiteLedSlot1, Global.WhiteLedSlot2);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Picture cut fail.\n");
                return false;
            }
            double BrightnessSlot1 = 0;
            isSuccess = ImageProcess.DevWhiteLedTest(Global.WhiteLedSlot1, Global.Up_X, Global.Up_Y, Global.rect_width_up, Global.HeightPix, out BrightnessSlot1, GetExpTime, Global.Noise);
            if (!isSuccess)
            {
                LogFile.OpenFile(".//Slot1_White.txt");
                LogFile.WriteToFile("slot1_White_LED_avg=0");
                LogFile.CloseFile();
                Console.WriteLine("slot1_White_LED_avg=0");
                LogFile.WriteLogToFile("Dev White Led Test Slot1 fail.\n");
            }

            double BrightnessSlot2 = 0;
            isSuccess = ImageProcess.DevWhiteLedTest(Global.WhiteLedSlot2, Global.Down_X, Global.Down_Y, Global.rect_width_dowm, Global.HeightPix, out BrightnessSlot2, GetExpTime, Global.Noise);
            if (!isSuccess)
            {
                LogFile.OpenFile(".//Slot2_White.txt");
                LogFile.WriteToFile("slot2_White_LED_avg=0");
                LogFile.CloseFile();
                Console.WriteLine("slot2_White_LED_avg=0");
                LogFile.WriteLogToFile("Dev White Led Test Slot2 fail.\n");
            }
            decimal Bri1 = Math.Round(Convert.ToDecimal(BrightnessSlot1), 4, MidpointRounding.AwayFromZero);
            decimal Bri2 = Math.Round(Convert.ToDecimal(BrightnessSlot2), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("slot1_White_LED_avg=" + Bri1);
            Console.WriteLine("slot2_White_LED_avg=" + Bri2);
            LogFile.OpenFile(".//Slot1_White.txt");
            LogFile.WriteToFile("slot1_White_LED_avg=" + Bri1);
            LogFile.CloseFile();
            LogFile.OpenFile(".//Slot2_White.txt");
            LogFile.WriteToFile("slot2_White_LED_avg=" + Bri2);
            LogFile.CloseFile();
            return true;
        }
        public static bool Red_Led_Test()
        {
            bool isSuccess = false;
            double GetExpTime = 0;
            isSuccess = DaHuaCamera.CameraSetExptime(Global.RedExpTime, out GetExpTime);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Set camera exptime fail.\n");
                return false;
            }
            isSuccess = DaHuaCamera.CameraAction(1, Global.SourceRedLed);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Camera capture fail.\n");
                return false;
            }
            isSuccess = ImageProcess.PictureCut(Global.SourceRedLed, Global.RedLedSlot1, Global.RedLedSlot2);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Picture cut fail.\n");
                return false;
            }
            double BrightnessSlot1 = 0;
            double Rslot1 = 0;
            double Gslot1 = 0;
            double Bslot1 = 0;
            isSuccess = ImageProcess.LedColorTest(Global.RedLedSlot1, out BrightnessSlot1, out Rslot1, out Gslot1, out Bslot1, GetExpTime);
            if (!isSuccess)
            {

                Console.WriteLine("Slot1_Red_LED_R = 0");
                Console.WriteLine("Slot1_Red_LED_G = 0");
                Console.WriteLine("Slot1_Red_LED_B = 0");
                Console.WriteLine("slot1_Red_LED_avg = 0");
                LogFile.OpenFile(".\\Slot1_Red.txt");
                LogFile.WriteToFile("Slot1_Red_LED_R = 0");
                LogFile.WriteToFile("Slot1_Red_LED_G = 0");
                LogFile.WriteToFile("Slot1_Red_LED_B = 0");
                LogFile.WriteToFile("slot1_Red_LED_avg = 0");
                LogFile.CloseFile();
                LogFile.WriteLogToFile("Led Color Test Slot1 fail.\n");
                return false;
            }
            decimal Bri1 = Math.Round(Convert.ToDecimal(BrightnessSlot1), 4, MidpointRounding.AwayFromZero);
            decimal R1 = Math.Round(Convert.ToDecimal(Rslot1), 4, MidpointRounding.AwayFromZero);
            decimal G1 = Math.Round(Convert.ToDecimal(Gslot1), 4, MidpointRounding.AwayFromZero);
            decimal B1 = Math.Round(Convert.ToDecimal(Bslot1), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("Slot1_Red_LED_R = " + R1);
            Console.WriteLine("Slot1_Red_LED_G = " + G1);
            Console.WriteLine("Slot1_Red_LED_B = " + B1);
            Console.WriteLine("slot1_Red_LED_avg = " + Bri1);
            LogFile.OpenFile(".\\Slot1_Red.txt");
            LogFile.WriteToFile("Slot1_Red_LED_R = " + R1);
            LogFile.WriteToFile("Slot1_Red_LED_G = " + G1);
            LogFile.WriteToFile("Slot1_Red_LED_B = " + B1);
            LogFile.WriteToFile("slot1_Red_LED_avg = " + Bri1);
            LogFile.CloseFile();


            double BrightnessSlot2 = 0;
            double Rslot2 = 0;
            double Gslot2 = 0;
            double Bslot2 = 0;
            isSuccess = ImageProcess.LedColorTest(Global.RedLedSlot2, out BrightnessSlot2, out Rslot2, out Gslot2, out Bslot2, GetExpTime);
            if (!isSuccess)
            {

                Console.WriteLine("Slot2_Red_LED_R = 0");
                Console.WriteLine("Slot2_Red_LED_G = 0");
                Console.WriteLine("Slot2_Red_LED_B = 0");
                Console.WriteLine("slot2_Red_LED_avg = 0");
                LogFile.OpenFile(".\\Slot2_Red.txt");
                LogFile.WriteToFile("Slot2_Red_LED_R = 0");
                LogFile.WriteToFile("Slot2_Red_LED_G = 0");
                LogFile.WriteToFile("Slot2_Red_LED_B = 0");
                LogFile.WriteToFile("slot2_Red_LED_avg = 0");
                LogFile.CloseFile();
                LogFile.WriteLogToFile("Led Color Test Slot2 fail.\n");
                return false;
            }
            decimal Bri2 = Math.Round(Convert.ToDecimal(BrightnessSlot2), 4, MidpointRounding.AwayFromZero);
            decimal R2 = Math.Round(Convert.ToDecimal(Rslot2), 4, MidpointRounding.AwayFromZero);
            decimal G2 = Math.Round(Convert.ToDecimal(Gslot2), 4, MidpointRounding.AwayFromZero);
            decimal B2 = Math.Round(Convert.ToDecimal(Bslot2), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("Slot2_Red_LED_R = " + R2);
            Console.WriteLine("Slot2_Red_LED_G = " + G2);
            Console.WriteLine("Slot2_Red_LED_B = " + B2);
            Console.WriteLine("slot2_Red_LED_avg = " + Bri2);
            LogFile.OpenFile(".\\Slot2_Red.txt");
            LogFile.WriteToFile("Slot2_Red_LED_R = " + R2);
            LogFile.WriteToFile("Slot2_Red_LED_G = " + G2);
            LogFile.WriteToFile("Slot2_Red_LED_B = " + B2);
            LogFile.WriteToFile("slot2_Red_LED_avg = " + Bri2);
            LogFile.CloseFile();
            return true;
        }
        public static bool Green_Led_Test()
        {
            bool isSuccess = false;
            double GetExpTime = 0;
            isSuccess = DaHuaCamera.CameraSetExptime(Global.GreenExpTime, out GetExpTime);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Set camera exptime fail.\n");
                return false;
            }
            isSuccess = DaHuaCamera.CameraAction(1, Global.SourceGreenLed);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Camera capture fail.\n");
                return false;
            }
            isSuccess = ImageProcess.PictureCut(Global.SourceGreenLed, Global.GreenLedSlot1, Global.GreenLedSlot2);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("Picture cut fail.\n");
                return false;
            }
            double BrightnessSlot1 = 0;
            double Rslot1 = 0;
            double Gslot1 = 0;
            double Bslot1 = 0;
            isSuccess = ImageProcess.LedColorTest(Global.GreenLedSlot1, out BrightnessSlot1, out Rslot1, out Gslot1, out Bslot1, GetExpTime);
            if (!isSuccess)
            {

                Console.WriteLine("Slot1_Green_LED_R = 0");
                Console.WriteLine("Slot1_Green_LED_G = 0");
                Console.WriteLine("Slot1_Green_LED_B = 0");
                Console.WriteLine("slot1_Green_LED_avg = 0");
                LogFile.OpenFile(".\\Slot1_Green.txt");
                LogFile.WriteToFile("Slot1_Green_LED_R = 0");
                LogFile.WriteToFile("Slot1_Green_LED_G = 0");
                LogFile.WriteToFile("Slot1_Green_LED_B = 0");
                LogFile.WriteToFile("slot1_Green_LED_avg = 0");
                LogFile.CloseFile();
                LogFile.WriteLogToFile("Led Color Test Slot1 fail.\n");
                return false;
            }
            decimal Bri1 = Math.Round(Convert.ToDecimal(BrightnessSlot1), 4, MidpointRounding.AwayFromZero);
            decimal R1 = Math.Round(Convert.ToDecimal(Rslot1), 4, MidpointRounding.AwayFromZero);
            decimal G1 = Math.Round(Convert.ToDecimal(Gslot1), 4, MidpointRounding.AwayFromZero);
            decimal B1 = Math.Round(Convert.ToDecimal(Bslot1), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("Slot1_Green_LED_R = " + R1);
            Console.WriteLine("Slot1_Green_LED_G = " + G1);
            Console.WriteLine("Slot1_Green_LED_B = " + B1);
            Console.WriteLine("slot1_Green_LED_avg = " + Bri1);
            LogFile.OpenFile(".\\Slot1_Green.txt");
            LogFile.WriteToFile("Slot1_Green_LED_R = " + R1);
            LogFile.WriteToFile("Slot1_Green_LED_G = " + G1);
            LogFile.WriteToFile("Slot1_Green_LED_B = " + B1);
            LogFile.WriteToFile("slot1_Green_LED_avg = " + Bri1);
            LogFile.CloseFile();


            double BrightnessSlot2 = 0;
            double Rslot2 = 0;
            double Gslot2 = 0;
            double Bslot2 = 0;
            isSuccess = ImageProcess.LedColorTest(Global.GreenLedSlot2, out BrightnessSlot2, out Rslot2, out Gslot2, out Bslot2, GetExpTime);
            if (!isSuccess)
            {

                Console.WriteLine("Slot2_Green_LED_R = 0");
                Console.WriteLine("Slot2_Green_LED_G = 0");
                Console.WriteLine("Slot2_Green_LED_B = 0");
                Console.WriteLine("slot2_Green_LED_avg = 0");
                LogFile.OpenFile(".\\Slot2_Green.txt");
                LogFile.WriteToFile("Slot2_Green_LED_R = 0");
                LogFile.WriteToFile("Slot2_Green_LED_G = 0");
                LogFile.WriteToFile("Slot2_Green_LED_B = 0");
                LogFile.WriteToFile("slot2_Green_LED_avg = 0");
                LogFile.CloseFile();
                LogFile.WriteLogToFile("Led Color Test Slot2 fail.\n");
                return false;
            }
            decimal Bri2 = Math.Round(Convert.ToDecimal(BrightnessSlot2), 4, MidpointRounding.AwayFromZero);
            decimal R2 = Math.Round(Convert.ToDecimal(Rslot2), 4, MidpointRounding.AwayFromZero);
            decimal G2 = Math.Round(Convert.ToDecimal(Gslot2), 4, MidpointRounding.AwayFromZero);
            decimal B2 = Math.Round(Convert.ToDecimal(Bslot2), 4, MidpointRounding.AwayFromZero);
            Console.WriteLine("Slot2_Green_LED_R = " + R2);
            Console.WriteLine("Slot2_Green_LED_G = " + G2);
            Console.WriteLine("Slot2_Green_LED_B = " + B2);
            Console.WriteLine("slot2_Green_LED_avg = " + Bri2);
            LogFile.OpenFile(".\\Slot2_Green.txt");
            LogFile.WriteToFile("Slot2_Green_LED_R = " + R2);
            LogFile.WriteToFile("Slot2_Green_LED_G = " + G2);
            LogFile.WriteToFile("Slot2_Green_LED_B = " + B2);
            LogFile.WriteToFile("slot2_Green_LED_avg = " + Bri2);
            LogFile.CloseFile();
            return true;
        }
        public static void GetSensorData()
        {
            bool StopOne = false;
            bool StopTwo = false;
            Global.ThreadStop = false;
            Cover1.Clear();
            Cover2.Clear();
            while (true)
            {
                ForceSensorClass.GetSensorData();
                double TempForce1;
                double TempForce2;
                TempForce1 = ForceSensorClass.CoverForceOne / 100.0;
                TempForce2 = ForceSensorClass.CoverForceTwo / 100.0;
                if (TempForce1 < 20)
                {
                    Cover1.Add(TempForce1);
                }
                if (TempForce2 < 20)
                {
                    Cover1.Add(TempForce2);
                }
                if (TempForce1 >= Global.CoverLimit && TempForce1 <= 20)
                {
                    McuContorlClass.Cover1Stop();
                    Global.ThreadStop = true;
                    StopOne = true;
                    break;
                }
                if (TempForce2 >= Global.CoverLimit && TempForce2 <= 20)
                {
                    McuContorlClass.Cover2Stop();
                    Global.ThreadStop = true;
                    StopTwo = true;
                    break;
                }
                if (Global.ThreadStop == true)
                {
                    Global.ThreadStop = false;
                    break;
                }
            }
            while (true)
            {
                ForceSensorClass.GetSensorData();
                double TempForce1;
                double TempForce2;
                TempForce1 = ForceSensorClass.CoverForceOne / 100.0;
                TempForce2 = ForceSensorClass.CoverForceTwo / 100.0;
                if (TempForce1 < 20)
                {
                    Cover1.Add(TempForce1);
                }
                if (TempForce2 < 20)
                {
                    Cover1.Add(TempForce2);
                }
                if (TempForce1 >= Global.CoverLimit && TempForce1 <= 20)
                {
                    if (StopOne == false)
                    {
                        McuContorlClass.Cover1Stop();
                        break;
                    }
                }
                if (TempForce2 >= Global.CoverLimit && TempForce2 <= 20)
                {
                    if (StopTwo == false)
                    {
                        McuContorlClass.Cover2Stop();
                        break;
                    }
                }
                if (Global.ThreadStop == true)
                {
                    break;
                }
            }
            return;
        }
        public static bool SaveSensorData()
        {

            double maxCover1 = 0;
            double maxCover2 = 0;
            LogFile.OpenFile(".//Cover1.txt");
            for (int i = 0; i < Cover1.Count; i++)
            {
                if (Cover1[i] > maxCover1)
                {
                    maxCover1 = Cover1[i];
                }
                LogFile.WriteToFile(Cover1[i].ToString());
            }
            LogFile.CloseFile();
            LogFile.OpenFile(".//Cover2.txt");
            for (int i = 0; i < Cover2.Count; i++)
            {
                if (Cover2[i] > maxCover2)
                {
                    maxCover2 = Cover2[i];
                }
                LogFile.WriteToFile(Cover2[i].ToString());
            }
            LogFile.CloseFile();

            Console.WriteLine("Slot1_Cover_MAX =" + maxCover1);
            Console.WriteLine("Slot2_Cover_MAX =" + maxCover2);
            return true;
        }
        public static bool ReadConfigData()
        {
            StreamReader sr1 = new StreamReader("D:\\config\\Slot坐标.txt", Encoding.Default);
            StreamReader sr3 = new StreamReader("D:\\config\\Color坐标.txt", Encoding.Default);
            StreamReader sr4 = new StreamReader("D:\\config\\DeviceKey.txt", Encoding.Default);
            StreamReader sr5 = new StreamReader("D:\\config\\Parameter.txt", Encoding.Default);
            string line;
            List<string> list = new List<string>();
            List<string> list1 = new List<string>();
            while ((line = sr1.ReadLine()) != null)
            {
                list.Add(line.ToString());
            }
            while ((line = sr3.ReadLine()) != null)
            {
                list.Add(line.ToString());
            }
            while ((line = sr4.ReadLine()) != null)
            {
                list1.Add(line.ToString());
            }
            while ((line = sr5.ReadLine()) != null)
            {
                list.Add(line.ToString());
            }
            foreach (string s in list)
            {
                string[] arr = s.Split(':');
                listKeys.Add(arr[0]);
                string[] arr_value = arr[1].Split(',');
                dic.Add(arr[0], arr_value);
            }
            foreach (string s in list1)
            {
                string[] arr = s.Split('=');
                listKeys.Add(arr[0]);
                string[] arr_value = arr[1].Split(',');
                dic.Add(arr[0], arr_value);
            }
            Global.CaremaFactor = GetData(DaHuaCamera.GetDeviceKey());
            Global.CoverLimit = GetData("cover_limit");
            Global.Down_X = GetData("Second_Up_X");
            Global.Down_Y = GetData("Second_Up_Y");
            Global.GreenExpTime = GetData("Gtime");
            Global.HeightPix = GetData("HeightPix");
            Global.Noise = GetData("Noise");
            Global.PictureTime = GetData("picturetime");
            Global.rect_height = GetData("rect_height");
            Global.rect_width_dowm = GetData("rect_width_down");
            Global.rect_width_up = GetData("rect_width");
            Global.RedExpTime = GetData("Rtime");
            Global.SensorOffSet = GetData("sensoroffset");
            Global.Up_X = GetData("First_Up_X");
            Global.Up_Y = GetData("First_Down_Y");
            Global.WhiteExpTime = GetData("Wtime");
            return true;
        }
        public static int GetData(string data)
        {
            string[] value = null;
            dic.TryGetValue(data, out value);
            return Convert.ToInt32(value[0]);
        }


        public static bool OpenLog()
        {
            
            LogFile.OpenLogFile(FileMode.Create);
            return true;
        }
        public static bool CloseLog()
        {
            LogFile.CloseLogFile();
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace FB.Led.McuCommunication
{
    
    class McuContorlClass
    {
        static SerialPort serial = new SerialPort("COM11", 115200, Parity.None, 8, StopBits.One) { NewLine = "\r\n" };
        public static bool McuInitial()
        {
            serial.Open();
            if (!serial.IsOpen)
            {
                LogFile.WriteLogToFile("Mcu Initial fail.\n");
                return false;
            }
            return true;
        }
        public static bool McuClose()
        {
            serial.Close();
            return true;
        }
        public static bool McuCommunication(string command)
        {
            serial.WriteLine(command);
            Thread.Sleep(300);
            string readstr = serial.ReadExisting();
            if (readstr.Length > 0)
            {
                Console.WriteLine(readstr);
                Console.WriteLine(command + "_OK");
            }
            if (command.Contains("VERSION"))
            {
                Console.Write(SoftwareInformation());
            }
            return true;
        }
        public static string CheckReady()
        {
            serial.WriteLine("CHECK_READY");
            Thread.Sleep(300);
            string readstr = serial.ReadExisting();
            if (readstr.Length > 0)
            {
                Console.WriteLine(readstr);

            }

            if (readstr.Contains("DUT1:OK"))
            {
                Console.WriteLine("DUT1:OK");
            }

            if (readstr.Contains("DUT1:ERROR"))
            {
                Console.WriteLine("DUT1:ERROR");
            }

            if (readstr.Contains("DUT2:OK"))
            {
                Console.WriteLine("DUT2:OK");
            }
            if (readstr.Contains("DUT2:ERROR"))
            {
                Console.WriteLine("DUT2:ERROR");
            }        
            return "ok";
        }
        public static bool UsbIn()
        {
            string readStr = "";
            serial.WriteLine("USB_IN1");
            Thread.Sleep(300);
            serial.WriteLine("USB_IN2");
            int time = 0;
            while (true)
            {
                time++;
                Thread.Sleep(50);
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("USB_IN1_OK"))
                {
                    if (readStr.Contains("USB_IN2_OK"))
                    {
                        Console.WriteLine("USB_IN_OK");
                        break;
                    }
                }
                if (readStr.Contains("USB_IN1_OK"))
                {
                    if (readStr.Contains("USB_IN2_ERROR"))
                    {
                        Console.WriteLine("USB_IN2_ERROR");
                        Console.WriteLine("USB_IN_ERROR");
                        return false;
                    }
                }
                if (readStr.Contains("USB_IN1_ERROR"))
                {
                    if (readStr.Contains("USB_IN2_OK"))
                    {
                        Console.WriteLine("USB_IN1_ERROR");
                        Console.WriteLine("USB_IN_ERROR");
                        return false;
                    }
                }
                if (readStr.Contains("USB_IN1_ERROR"))
                {
                    if (readStr.Contains("USB_IN2_ERROR"))
                    {
                        Console.WriteLine("USB_IN_ERROR");
                        break;
                    }
                }
                if (time == 60)
                {
                    Console.WriteLine("USB_IN_ERROR");
                    return false;
                }
            }
            return true;
        }
        public static bool UsbOut()
        {
            string readStr = "";
            serial.Open();
            serial.WriteLine("USB_OUT1");
            Thread.Sleep(10);
            serial.WriteLine("USB_OUT2");
            int time = 0;
            while (true)
            {
                Thread.Sleep(50);
                time++;
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("USB_OUT1_OK"))
                {
                    if (readStr.Contains("USB_OUT2_OK"))
                    {
                        Console.WriteLine("USB_OUT_OK");
                        break;
                    }
                }
                if (readStr.Contains("USB_OUT1_OK"))
                {
                    if (readStr.Contains("USB_OUT2_ERROR"))
                    {
                        Console.WriteLine("USB_OUT2_ERROR");
                        Console.WriteLine("USB_OUT_ERROR");
                        return false;
                    }
                }
                if (readStr.Contains("USB_OUT1_ERROR"))
                {
                    if (readStr.Contains("USB_OUT2_OK"))
                    {
                        Console.WriteLine("USB_OUT1_ERROR");
                        Console.WriteLine("USB_OUT_ERROR");
                        return false;
                    }
                }
                if (readStr.Contains("USB_OUT1_ERROR"))
                {
                    if (readStr.Contains("USB_OUT2_ERROR"))
                    {
                        Console.WriteLine("USB_OUT_ERROR");
                        return false;
                    }
                }
                if (time == 60)
                {
                    Console.WriteLine("USB_OUT_ERROR");
                    return false;
                }
            }
            return true;
        }
        public static bool ReadyTest()
        {
            string readStr = "";
            while (true)
            {
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("START TEST"))
                {
                    Console.WriteLine("START TEST");
                    break;
                }
            }
            return true;
        }
        public static bool CoverIn()
        {
            string readStr = "";
            serial.WriteLine("MOTOR_COVER_IN1");
            Thread.Sleep(50);
            serial.WriteLine("MOTOR_COVER_IN2");
            int time = 0;
            while (true)
            {
                Thread.Sleep(50);

                time++;
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("MOTOR_COVER_IN1_OK"))
                {
                    if (readStr.Contains("MOTOR_COVER_IN1_OK"))
                    {
                        Console.WriteLine("Slot1_Cover_start_PASS");
                        Console.WriteLine("Slot2_Cover_start_PASS");
                        Global.ThreadStop = true;
                        break;
                    }

                }
                if (readStr.Contains("MOTOR_COVER_IN1_ERROR"))
                {
                    if (readStr.Contains("MOTOR_COVER_IN2_OK"))
                    {
                        Console.WriteLine("Slot1_Cover_start_FAILED");
                        Console.WriteLine("Slot2_Cover_start_PASS");

                        Global.ThreadStop = true;
                        break;
                    }

                }
                if (readStr.Contains("MOTOR_COVER_IN1_OK"))
                {
                    if (readStr.Contains("MOTOR_COVER_IN2_ERROR"))
                    {
                        Console.WriteLine("Slot1_Cover_END_PASS");
                        Console.WriteLine("Slot2_Cover_END_FAILED");

                        Global.ThreadStop = true;
                        break;
                    }

                }
                if (readStr.Contains("STOP"))
                {
                    Console.WriteLine("MOTOR_COVER_IN_STOP");
                    Thread.Sleep(3000);
                    break;

                }
                if (time == 100)
                {
                    Console.WriteLine("COVER_TEST_ERROR");
                    Global.ThreadStop = true;
                    break;
                }
            }
            return true;
        }
        public static bool CoverOut()
        {
            string readStr = "";
            serial.WriteLine("MOTOR_COVER_OUT1");
            Thread.Sleep(50);
            serial.WriteLine("MOTOR_COVER_OUT2");
            int time = 0;
            while (true)
            {
                Thread.Sleep(300);
                time++;
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("MOTOR_COVER_OUT1_OK"))
                {
                    if (readStr.Contains("MOTOR_COVER_OUT2_OK"))
                    {

                        Global.ThreadStop = true;
                        break;
                    }
                }
                if (readStr.Contains("MOTOR_COVER_OUT1_ERROR"))
                {
                    if (readStr.Contains("MOTOR_COVER_OUT2_OK"))
                    {

                        Console.WriteLine("COVER_TEST_ERROR");
                        Global.ThreadStop = true;
                        return false;
                    }
                }
                if (readStr.Contains("MOTOR_COVER_OUT1_OK"))
                {
                    if (readStr.Contains("MOTOR_COVER_OUT2_ERROR"))
                    {

                        Console.WriteLine("COVER_TEST_ERROR");
                        Global.ThreadStop = true;
                        return false;
                    }
                }
                if (readStr.Contains("MOTOR_COVER_OUT1_ERROR"))
                {
                    if (readStr.Contains("MOTOR_COVER_OUT2_ERROR"))
                    {

                        Console.WriteLine("COVER_TEST_ERROR");
                        Global.ThreadStop = true;
                        return false;

                    }

                }
                if (time == 160)
                {
                    Console.WriteLine("COVER_TEST_ERROR");
                    Global.ThreadStop = true;
                    return false;
                }
            }
            return true;
        }
        public static bool ButtonIn()
        {
            serial.WriteLine("MOTOR_BUTTON_IN1");
            Thread.Sleep(50);
            serial.WriteLine("MOTOR_BUTTON_IN2");
            return true;
        }
        public static bool ButtonOut()
        {
            serial.WriteLine("MOTOR_BUTTON_OUT1");
            Thread.Sleep(50);
            serial.WriteLine("MOTOR_BUTTON_OUT2");
            return true;
        }
        public static bool ElecCoverIn()
        {
            string readStr = "";
            serial.WriteLine("ELEC_COVER_IN1");
            Thread.Sleep(50);
            serial.WriteLine("ELEC_COVER_IN2");
            int time = 0;
            while (true)
            {
                time++;
                readStr = readStr + serial.ReadExisting();
                Thread.Sleep(300);
                if (readStr.Contains("ELEC_COVER_IN1_OK"))
                {
                    if (readStr.Contains("ELEC_COVER_IN2_OK"))
                    {

                        break;
                    }
                }
                if (readStr.Contains("ELEC_COVER_IN1_ERROR"))
                {
                    if (readStr.Contains("ELEC_COVER_IN2_OK"))
                    {
                        Console.WriteLine("ELEC_COVER_IN_ERROR");

                        return false;
                    }
                }
                if (readStr.Contains("ELEC_COVER_IN1_OK"))
                {
                    if (readStr.Contains("ELEC_COVER_IN2_ERROR"))
                    {

                        Console.WriteLine("ELEC_COVER_IN_ERROR");
                        return false;
                    }
                }
                if (readStr.Contains("ELEC_COVER_IN1_ERROR"))
                {
                    if (readStr.Contains("ELEC_COVER_IN2_ERROR"))
                    {
                        Console.WriteLine("ELEC_COVER_IN_ERROR");
                        return false;
                    }
                }
                if (time == 10)
                {

                    Console.WriteLine("ELEC_COVER_IN_ERROR");
                    return false;
                }
            }
            return true;
        }
        public static bool ElecCoverOut()
        {
            serial.WriteLine("ELEC_COVER_OUT1");
            Thread.Sleep(50);
            serial.WriteLine("ELEC_COVER_OUT2");
            string readStr = "";
            int time = 0;
            while (true)
            {
                time++;
                Thread.Sleep(50);
                readStr = readStr + serial.ReadExisting();
                if (readStr.Contains("ELEC_COVER_OUT1_OK"))
                {
                    if (readStr.Contains("ELEC_COVER_OUT2_OK"))
                    {

                        break;
                    }

                }
                if (readStr.Contains("ELEC_COVER_OUT1_ERROR"))
                {
                    if (readStr.Contains("ELEC_COVER_OUT2_OK"))
                    {

                        Console.WriteLine("COVER_TEST_ERROR");
                        return false;
                    }

                }
                if (readStr.Contains("ELEC_COVER_OUT1_OK"))
                {
                    if (readStr.Contains("ELEC_COVER_OUT2_ERROR"))
                    {

                        Console.WriteLine("COVER_TEST_ERROR");
                        return false;
                    }

                }
                if (readStr.Contains("ELEC_COVER_OUT1_ERROR"))
                {
                    if (readStr.Contains("ELEC_COVER_OUT2_ERROR"))
                    {

                        return false;
                    }

                }
                if (time == 60)
                {
                    Console.WriteLine("COVER_TEST_ERROR");
                }
            }
            return true;
        }
        public static bool Cover1Stop()
        {
            serial.WriteLine("MOTOR_COVER_STOP1");
            Thread.Sleep(20);
            serial.WriteLine("MOTOR_COVER_OUT1");
            return true;
        }
        public static bool Cover2Stop()
        {
            serial.WriteLine("MOTOR_COVER_STOP2");
            Thread.Sleep(20);
            serial.WriteLine("MOTOR_COVER_OUT2");
            return true;
        }
        public static string SoftwareInformation()
        {
            string Information = "V1.3" + "\n";
            string updata1 = "2019-3-6 Optimization code" + "\n";
            string updata2 = "2019-3-9 Change the force sensor paramater" + "\n";
            string updata3 = "2019-3-9 Add force sensor data processing" + "\n";
            string updata4 = "2019-3-11 Expand the calculation range of the image and refine the calculation steps" + "\n";
            string updata5 = "2019-4-19 Changed the calculation function" + "\n";
            string updata6 = "2019-6-17 Changed the force sensor delay time\n";
            string updata7 = "2019-6-17 Changed the forcc sensor row data process\n";
            return (Information + updata1 + updata2 + updata3 + updata4 + updata5 + updata6 + updata7);
        }
    }
}

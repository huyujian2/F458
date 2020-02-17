using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThridLibray;


namespace FB.Led.McuCommunication
{
    class Program
    {
        static void Main(string[] args)
        {

            args = new string[1];
            args[0] = "VERSION";
            if (args.Length != 1)
            {
                Console.WriteLine("The format of parameters is incorrect.");
                return;
            }
            try
            {
                
                FBTest.OpenLog();
                FBTest.ReadConfigData();
                FBTest.DeviceInitial();
                switch (args[0])
                {
                    case "HELP":
                    case "VERSION":
                    case "RESET":              
                    case "USB_IN1":
                    case "USB_IN2":
                    case "USB_OUT1":
                    case "USB_OUT2":
                    case "ELEC_COVER_IN1":
                    case "ELEC_COVER_IN2":
                    case "ELEC_COVER_OUT1":
                    case "ELEC_COVER_OUT2":
                    case "DRAWER_LOCK":
                    case "DRAWER_UNLOCK":
                    case "CHECK_STATUS":
                    case "TEST_DONE":
                        FBTest.Mcu_Communication(args[0]);
                        break;
                   case  "CHECK_READY":
                        FBTest.Check_Ready();
                        break;
                    case "READY":
                        FBTest.Ready_Test();
                        break;
                    case "USB_IN":
                        FBTest.Usb_In();
                        break;
                    case "USB_OUT":
                        FBTest.Usb_Out();
                        break;
                    case "TEST":                      
                        break;
                    case "COVER_TEST":
                        FBTest.Cover_Test();
                        break;
                    case "BUTTON_TEST":
                        FBTest.Button_Push();
                        break;
                    case "WHITE_LED_TEST":
                        FBTest.White_Led_Test_OneByOne();
                        break;
                    case "WHITE_LED_TEST_A":
                        FBTest.White_Led_Test_Average();
                        break;
                    case "GREEN_LED_TEST":
                        FBTest.Green_Led_Test();
                        break;
                    case "RED_LED_TEST":
                        FBTest.Red_Led_Test();
                        break;
                    default:
                        Console.WriteLine("Command error\nPlease input the correct command");
                        break;
                }
                FBTest.DeviceClose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured in serial port communication.");
                Console.WriteLine(ex.Message);
            }
      
        }       
    }
}

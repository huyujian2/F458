using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ThridLibray;

namespace FB.Led.McuCommunication
{
    class DaHuaCamera
    {
        public static int CameraOpen = 0;
        public static int CameraCapture = 1;
        public static int CameraClose = 2;
        static IDevice m_dev;
        static IGrabbedRawData m_frameList;
        public static bool CameraInitial()
        {
            bool isSuccess = false;
            isSuccess = CameraAction(CameraOpen);
            if (!isSuccess)
            {
                LogFile.WriteLogToFile("CameraInitial fail.");
                return false;
            }
            return true;
        }
        public static bool CameraAction(int mode, string saveImagePath = null)
        {
            if(mode == 0)
            {
                List<IDeviceInfo> li = Enumerator.EnumerateDevices();
                if (li.Count > 0)
                {
                    m_dev = Enumerator.GetDeviceByIndex(0);
                    m_dev.Open();
                    if (!m_dev.IsOpen)
                    {
                        LogFile.WriteLogToFile("Open Camera fail.");
                        return false;
                    }
                    Global.DeviceKey = m_dev.DeviceKey;
                    using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                    {

                        p.SetValue(1.0);
                    }
                }
                else
                {
                    LogFile.WriteLogToFile("Don't find camera.");
                    return false;
                }
            }
            else if(mode == 1)
            {
                if(m_dev.IsOpen)
                {
                    m_dev.GrabUsingGrabLoopThread();
                    bool Check = m_dev.WaitForFrameTriggerReady(out m_frameList, Global.PictureTime);
                    if (!Check)
                    {
                        LogFile.WriteLogToFile("Capture frame fail.");
                        return false;
                    }
                    var bitmap = m_frameList.ToBitmap(false);
                    bitmap.Save(saveImagePath);
                }
                else
                {
                    LogFile.WriteLogToFile("Restart the camera.");
                    bool isSuccess = false;
                    for(int i = 0; i <=5;i++)
                    {
                        Thread.Sleep(5000);
                        isSuccess = CameraRestart();
                        if(isSuccess)
                        {
                            LogFile.WriteLogToFile("Restart camera success.");
                            break;
                        }
                        if(i == 5)
                        {
                            LogFile.WriteLogToFile("Restart camera fail.");
                            return false;
                        }
                    }
                    bool Check = m_dev.WaitForFrameTriggerReady(out m_frameList, Global.PictureTime);
                    if (!Check)
                    {
                        LogFile.WriteLogToFile("Capture frame fail.");
                        return false;
                    }
                    var bitmap = m_frameList.ToBitmap(false);
                    bitmap.Save(saveImagePath);
                }

            }
            else if(mode == 2)
            {
                bool isSuccess = m_dev.Close();
                if(!isSuccess)
                {
                    LogFile.WriteLogToFile("Close camera fail.");
                    return false;
                }
            }
            return true;
        }
        public static bool CameraSetExptime(double value, out double getValue)
        {
            using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
            {
                p.SetValue(value);
                getValue = p.GetValue();
            }
            return true;
        }
        public static bool CameraRestart()
        {
            bool isSuccess = CameraAction(CameraOpen);
            if(!isSuccess)
            {
                return false;
            }
            return true;
        }
        public static string GetDeviceKey()
        {
            return m_dev.DeviceKey;
        }
    }
}

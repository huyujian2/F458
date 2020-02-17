using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FB.Led.McuCommunication
{
    class ImageProcess
    {
        static VectorOfPoint maxcontour;
        public static bool SingleWhiteLedTest(string path, int Index_X, int Index_Y, out double brightness,double ExpTime)
        {
            FunctionMaxContour(path);
            if (CvInvoke.ContourArea(maxcontour) > 20)
            {
                Bitmap sourceImage = new Bitmap(path);
                Rectangle myRectangle = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                System.Drawing.Imaging.BitmapData myBitmapData = sourceImage.LockBits(myRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                int PixelNum = myBitmapData.Stride * myBitmapData.Height;
                IntPtr myIntPtr = myBitmapData.Scan0;
                byte[] mybyte = new byte[PixelNum];
                System.Runtime.InteropServices.Marshal.Copy(myIntPtr, mybyte, 0, PixelNum);
                long total_R = 0;
                long total_G = 0;
                long total_B = 0;
                long sum_R = 0;
                long sum_G = 0;
                long sum_B = 0;
                for (int y = Index_Y - 100; y < Index_Y + 150; y++)
                {
                    for (int x = Index_X - 100; x < Index_X + 1200; x++)
                    {
                        Point pt = new Point(x, y);
                        if (CvInvoke.PointPolygonTest(maxcontour, pt, false) >= 0)
                        {
                            if (y % 2 == 0 && x % 2 == 0)
                            {
                                total_R += mybyte[y * myBitmapData.Stride + x * 3 + 2];
                                sum_R++;
                            }
                            else if (y % 2 == 1 && x % 2 == 1)
                            {
                                total_B += mybyte[y * myBitmapData.Stride + x * 3 + 0];
                                sum_B++;
                            }
                            else
                            {
                                total_G += mybyte[y * myBitmapData.Stride + x * 3 + 1];
                                sum_G++;

                            }
                        }

                    }
                }
                double B = (double)total_B / sum_B;
                double R = (double)total_R / sum_R;
                double G = (double)total_G / sum_G;
                brightness = Global.CaremaFactor * (B + R + G) / 3 / ExpTime;

            }
            else
            {
                brightness = 0;
                LogFile.WriteLogToFile("The maxcontour is less than 20.");
            }
            return true;
        }
        public static bool DevWhiteLedTest(string path, int Index_X, int Index_Y, double rect_width, double HeightPix, out double brightness, double ExpTime, int Noise)
        {
            bool isSuccess = false;
            isSuccess = FunctionMaxContour(path);
            if (!isSuccess)
            {
                brightness = 0;
                LogFile.WriteLogToFile("Can not find the contour.");
                return false;
            }
            if (CvInvoke.ContourArea(maxcontour) > 1000)
            {
                Bitmap sourceImage = new Bitmap(path);
                Rectangle myRectangle = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                System.Drawing.Imaging.BitmapData myBitmapData = sourceImage.LockBits(myRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                int PixelNum = myBitmapData.Stride * myBitmapData.Height;
                IntPtr myIntPtr = myBitmapData.Scan0;
                byte[] mybyte = new byte[PixelNum];
                System.Runtime.InteropServices.Marshal.Copy(myIntPtr, mybyte, 0, PixelNum);
                long total_R = 0;
                long total_G = 0;
                long total_B = 0;
                long sum_R = 0;
                long sum_G = 0;
                long sum_B = 0;
                for (int j = Index_Y; j <= Index_Y + HeightPix; j++)
                {
                    for (int k = Index_X; k <= rect_width; k++)
                    {

                        if (j % 2 == 0 && k % 2 == 0)
                        {
                            if (Noise < mybyte[j * myBitmapData.Stride + k * 3 + 2])
                            {
                                total_R += mybyte[j * myBitmapData.Stride + k * 3 + 2];
                                sum_R++;
                            }
                        }
                        else if (j % 2 == 1 && k % 2 == 1)
                        {

                            if (Noise < mybyte[j * myBitmapData.Stride + k * 3 + 0])
                            {
                                total_B += mybyte[j * myBitmapData.Stride + k * 3 + 0];
                                sum_B++;
                            }
                        }
                        else
                        {
                            if (Noise < mybyte[j * myBitmapData.Stride + k * 3 + 1])
                            {
                                total_G += mybyte[j * myBitmapData.Stride + k * 3 + 1];
                                sum_G++;
                            }
                        }
                    }
                }
                double B = (double)total_B / sum_B;
                double R = (double)total_R / sum_R;
                double G = (double)total_G / sum_G;
                brightness = Global.CaremaFactor * (B + R + G) / 3 / ExpTime;
            }
            else
            {
                brightness = 0;
                LogFile.WriteLogToFile("The max coutour is less than 1000.");
                return false;
            }
            return true;
        }
        public static bool LedColorTest(string path, out double brightness, out double R, out double G, out double B, double ExpTime)
        {
            bool isSuccess = false;
            isSuccess = FunctionMaxContour(path);
            if (!isSuccess)
            {
                brightness = 0;
                R = 0;
                G = 0;
                B = 0;
                LogFile.WriteLogToFile(path + "Can not find the contour.");
                return false;
            }
            Mat mat = mat = CvInvoke.Imread(path, ImreadModes.Grayscale);
            List<Point> PointList = new List<Point>();
            for (int i = 0; i < mat.Height / 2; i++)
            {
                for (int j = 0; j < mat.Width; j++)
                {
                    Point pt = new Point(j, i);
                    if (CvInvoke.PointPolygonTest(maxcontour, pt, false) >= 0)
                    {
                        PointList.Add(pt);
                    }

                }
            }
            if (CvInvoke.ContourArea(maxcontour) > 5)
            {
                Bitmap sourceImage = new Bitmap(path);
                Rectangle myRectangle = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                System.Drawing.Imaging.BitmapData myBitmapData = sourceImage.LockBits(myRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                int PixelNum = myBitmapData.Stride * myBitmapData.Height;
                IntPtr myIntPtr = myBitmapData.Scan0;
                byte[] mybyte = new byte[PixelNum];
                System.Runtime.InteropServices.Marshal.Copy(myIntPtr, mybyte, 0, PixelNum);
                int rPixelSum = 0;
                int gPixelSum = 0;
                int bPixelSum = 0;
                int rNum = 0;
                int gNum = 0;
                int bNum = 0;
                for (int i = 0; i < PointList.Count; i++)
                {
                    int xPoint = PointList[i].X;
                    int yPoint = PointList[i].Y;

                    if (xPoint % 2 == 0 && yPoint % 2 == 0)
                    {
                        rNum++;
                        rPixelSum += mybyte[yPoint * myBitmapData.Stride + xPoint * 3 + 2];
                    }
                    else if (xPoint % 2 == 1 && yPoint % 2 == 1)
                    {
                        bNum++;
                        bPixelSum += mybyte[yPoint * myBitmapData.Stride + xPoint * 3 + 0];
                    }
                    else
                    {
                        gNum++;
                        gPixelSum += mybyte[yPoint * myBitmapData.Stride + xPoint * 3 + 1];
                    }
                }
                R = rPixelSum / rNum;
                G = gPixelSum / gNum;
                B = bPixelSum / bNum;
                brightness = Global.CaremaFactor * (R + G + B) / 3.0 / ExpTime;
            }
            else
            {
                brightness = 0;
                R = 0;
                G = 0;
                B = 0;
                LogFile.WriteLogToFile(path + "The maxcontour is less than 5.");
                return false;
            }
            return true;
        }
        public static bool PictureCut(string path, string SaveUpName, string SaveDownName)
        {
            Bitmap bitmap1 = new Bitmap(path);
            Rectangle rect1 = new Rectangle(0, 0, 2448, 1024);
            Rectangle rect2 = new Rectangle(0, 1024, 2448, 1024);
            Bitmap slot1 = bitmap1.Clone(rect1, System.Drawing.Imaging.PixelFormat.DontCare);
            Bitmap slot2 = bitmap1.Clone(rect2, System.Drawing.Imaging.PixelFormat.DontCare);
            slot1.Save(SaveUpName);
            slot2.Save(SaveDownName);
            return true;
        }
        public static bool FunctionMaxContour(string path)
        {
            maxcontour.Clear();
            Mat dnc = new Mat();
            Mat mat = CvInvoke.Imread(path, ImreadModes.Grayscale);
            Mat blurImage = mat.Clone();
            Mat erodeImage = new Mat();
            Size kernalSize = new Size(15, 15);
            Point myPoint = new Point(-1, -1);
            CvInvoke.Blur(mat, blurImage, kernalSize, myPoint);
            Mat structElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, kernalSize, myPoint);
            CvInvoke.MorphologyEx(blurImage, erodeImage, MorphOp.Erode, structElement, myPoint, 1, BorderType.Default, new MCvScalar(255, 0, 0, 255));
            Mat thresholdImage = mat.Clone();
            CvInvoke.Threshold(erodeImage, thresholdImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(thresholdImage, contours, dnc, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
            double maxarea = 0.0;
            for (int index = 0; index < contours.Size; index++)
            {
                double area = CvInvoke.ContourArea(contours[index]);
                if (area > maxarea)
                {
                    maxarea = area;
                    maxcontour = contours[index];
                }
            }
            if (contours.Size == 0)
            {
                return false;
            }
            return true;
        }
    }
}

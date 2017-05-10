using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;

namespace FingerPrint
{
    class GenerateWatermark
    {
        public Bitmap getWatermarkedImage(string path,byte[] hashcode)
        {
            Image newImage = Image.FromFile(path);
            Bitmap img = CreateNonIndexedImage(newImage);
            string hash = "";
            for (int i = 0;i < hashcode.Length;i++)
            {
                hash = hash + hashcode[i].ToString() + "_";
            }
            Console.WriteLine(hash);
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (i < 1 && j < hash.Length)
                    {
                        //Console.WriteLine("R = [" + i + "][" + j + "] = " + pixel.R);
                        //Console.WriteLine("G = [" + i + "][" + j + "] = " + pixel.G);
                        //Console.WriteLine("G = [" + i + "][" + j + "] = " + pixel.B);

                        char letter = Convert.ToChar(hash.Substring(j, 1));
                        int value = Convert.ToInt32(letter);
                        //Console.WriteLine("letter : " + letter + " value : " + value);

                        img.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, value));
                    }

                    if (i == img.Width - 1 && j == img.Height - 1)
                    {
                        img.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, hash.Length));
                    }

                }
            }
            //outputImagePath = @"C:\Users\harsh\Desktop\temp.png";
            //img.Save(outputImagePath);
            return img;
        }

        //public void InnerHashInOuterImage(string[] InnerImage,string[] outerImage)
        //{
        //    int k = 1;
        //    // Finding an Hash sign of an inner image
        //    getHash hash = new getHash();
        //    int left = 1;
        //    int right = 1;
        //    for (int i = 0; i < InnerImage.Length; i++)
        //    {
        //        byte[] hashCode = hash.getSHAofAnImage(InnerImage[i]);

        //        if (right > 10)
        //        {
        //            left++;
        //            right = 1;
        //        }

        //        //watermarking WaterMarkedImages
        //        string outputImagePath;
        //        outputImagePath = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\WaterMarkedImages\InnerHashInOuterImage\" + left.ToString() + "_" + right.ToString() + ".bmp";
        //        //Creating new watermarkerdimage
        //        Bitmap watermarkedImage = getWatermarkedImage(outerImage[i], hashCode);
        //        watermarkedImage.Save(outputImagePath);

        //        //Retrive data from watermarked image
        //        byte[] retrivedWatermark = retriveWatermark(outputImagePath);

        //        //Chacking whether original data is matched with retrived data or not
        //        Validation validate = new Validation();
        //        bool isMatch = validate.compareHash(hashCode, retrivedWatermark);
        //        Console.WriteLine(isMatch.ToString());

        //        right++;
        //    }
        //}

        public void CreateWaterMarkImage(string[] share1, string[] share2, string foldername)
        {
            int k = 1;
            // Finding an Hash sign of an inner image
            getHash hash = new getHash();
            int left = 1;
            int right = 1;
            for (int i = 0; i < share1.Length; i++)
            {
                byte[] hashCode = hash.getSHAofAnImage(share1[i]);

                if (right > 10)
                {
                    left++;
                    right = 1;
                }

                //watermarking WaterMarkedImages
                string outputImagePath;
                outputImagePath = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\WaterMarkedImages\" + foldername + "\\" + left.ToString() + "_" + right.ToString() + ".bmp";
                //Creating new watermarkerdimage
                Bitmap watermarkedImage = getWatermarkedImage(share2[i], hashCode);
                watermarkedImage.Save(outputImagePath);

                //Retrive data from watermarked image
                byte[] retrivedWatermark = retriveWatermark(outputImagePath);

                //Chacking whether original data is matched with retrived data or not
                Validation validate = new Validation();
                bool isMatch = validate.compareHash(hashCode, retrivedWatermark);
                Console.WriteLine(isMatch.ToString());

                right++;
            }
        }

        public Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public byte[] retriveWatermark(string path)
        {
            Bitmap img = new Bitmap(path);
            string message = "";

            Color lastpixel = img.GetPixel(img.Width - 1, img.Height - 1);
            int msgLength = lastpixel.B;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);

                    if (i < 1 && j < msgLength)
                    {
                        int value = pixel.B;
                        char c = Convert.ToChar(value);
                        string letter = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });

                        message = message + letter;
                    }
                }
            }
            message = message.TrimEnd('_');
            Console.WriteLine(message);
            string[] decodeHash = message.Split('_');
            byte[] bytes = new byte[decodeHash.Length];
            for (int i=0;i< decodeHash.Length;i++)
            {
                bytes[i] = Convert.ToByte(decodeHash[i]);
            }
            return bytes;
        }
    }
}

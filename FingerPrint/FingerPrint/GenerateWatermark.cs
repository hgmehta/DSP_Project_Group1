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
        public Bitmap getWatermarkedImage(string path,byte[] hashcode,out string outputImagePath)
        {
            Bitmap img = new Bitmap(path);
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
            outputImagePath = @"C:\Users\harsh\Desktop\temp.png";
            //img.Save(outputImagePath);
            return img;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.Simple;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace FingerPrint
{
    class Program
    {
        static void buildDB(int TotalFiles)
        {
            AfisEngine Afis = new AfisEngine();

            Afis.Threshold = 5;

            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();

            string path = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\InnerImageScore.txt";

            string[] files = new string[TotalFiles];
            string[] outerImages = new string[TotalFiles];
            int k = 0;
            for (int i = 0; i < TotalFiles/10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    files[k] = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Database\" + (i + 1).ToString() + "_" + j.ToString() + ".bmp";
                    outerImages[k] = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Inner Images\ROI_" + (i + 1).ToString() + "_" + j.ToString() + ".png";
                    k++;
                }
            }

            k = 0;

            for (int i = 0; i < files.Length; i++)
            {
                fp1.AsBitmapSource = new BitmapImage(new Uri(files[i], UriKind.RelativeOrAbsolute));
                Person person1 = new Person();
                person1.Fingerprints.Add(fp1);
                Afis.Extract(person1);

                for (int j = 0; j < files.Length; j++)
                {
                    fp2.AsBitmapSource = new BitmapImage(new Uri(outerImages[j], UriKind.RelativeOrAbsolute));
                    Person person2 = new Person();
                    person2.Fingerprints.Add(fp2);

                    Afis.Extract(person2);

                    float score = Afis.Verify(person1, person2);
                    bool match = (score > Afis.Threshold);
                    Console.WriteLine(files[i] + "\n Compare with " + files[j]);
                    Console.WriteLine(score.ToString());
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(score.ToString());
                    }
                    k++;
                }
                Console.WriteLine("\n");
            }
        }

        static void normalizeScore()
        {

            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\InnerImageScore.txt");
            float[] score = new float[lines.Length];
            
            for (int i = 0; i < lines.Length; i++)
            {
                score[i] = float.Parse(lines[i]);
            }

            //Console.WriteLine(max);

            float min = score.Min();
            float max = score.Max();

            for (int i = 0; i < score.Length; i++)
            {
                score[i] = (score[i] - min) / (max - min);
            }


            string Path = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\InnerImageNormalizedScore.txt";


            using (StreamWriter sw = File.AppendText(Path))
            {
                for (int i = 0; i < score.Length; i++)
                { 
                    sw.WriteLine(score[i].ToString());
                }
            }
          //  string s = Console.ReadLine();
        }

        static void test()
        {
            AfisEngine Afis = new AfisEngine();

            Afis.Threshold = (float)100.12;
            Console.WriteLine(Afis.Threshold.ToString());
            string files1 = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Database\01_1.bmp";
            string files2 = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Database\01_2.bmp";

            Fingerprint fp1 = new Fingerprint();
            fp1.AsBitmapSource = new BitmapImage(new Uri(files1, UriKind.RelativeOrAbsolute));

            Fingerprint fp2 = new Fingerprint();
            fp2.AsBitmapSource = new BitmapImage(new Uri(files2, UriKind.RelativeOrAbsolute));

            Person person1 = new Person();
            person1.Fingerprints.Add(fp1);

            Person person2 = new Person();
            person2.Fingerprints.Add(fp2);


            Afis.Extract(person1);
            Afis.Extract(person2);
            float score = Afis.Verify(person1, person1);
            bool match = (score > Afis.Threshold);
            Console.WriteLine(score.ToString());

            string temp = Console.ReadLine();
        }

        static void Main(string[] args)
        {
            /*
            int TotalFiles = 30;
            buildDB(TotalFiles);
            test();
            normalizeScore();

            CropImage c1 = new CropImage();

            Bitmap source = new Bitmap(@"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Database\01_1.bmp");
            Rectangle section = new Rectangle(new Point(12, 50), new Size(150, 150));
            Graphics g = Graphics.FromImage(source);
            Bitmap CroppedImage = c1.CreateCropImage(source, section);
            Application.EnableVisualStyles();
            Application.Run(new Plots(TotalFiles));
            */

            // Finding an Hash sign of an inner image
            getHash hash = new getHash();
            string path = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Inner Images\ROI_1_1.png";
            byte[] hashCode = hash.getSHAofAnImage(path);

            /*
            for (int i=0;i<hashCode.Length;i++)
            {
                Console.WriteLine(hashCode[i].ToString());
            }
            Console.WriteLine("Length of Hash: "+hashCode.Length.ToString());
            Console.ReadLine();
            */

            //Do watermarking
            string outputImagePath;
            GenerateWatermark watermark = new GenerateWatermark();

            //Creating new watermarkerdimage 
            Bitmap watermarkedImage = watermark.getWatermarkedImage(path, hashCode, out outputImagePath);
            watermarkedImage.Save(outputImagePath);
            
            //Retrive data from watermarked image
            byte[] retrivedWatermark = watermark.retriveWatermark(outputImagePath);


            //Chacking whether original data is matched with retrived data or not
            bool value = true;
            if (retrivedWatermark.Length != hashCode.Length)
            {
                value = false;
            }
            if (value)
            {
                for (int i = 0; i < retrivedWatermark.Length; i++)
                {
                    if (retrivedWatermark[i] != hashCode[i])
                    {
                        value = false;
                    }
                }
            }
            Console.WriteLine(value.ToString());
            Console.ReadLine();
        }
    }
}
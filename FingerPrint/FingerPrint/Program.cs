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
        static void buildDB(int TotalFiles, string path, string[] files, string[] share)
        {
            AfisEngine Afis = new AfisEngine();

            Afis.Threshold = 5;

            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();

            int k = 0;

            for (int i = 0; i < files.Length; i++)
            {
                fp1.AsBitmapSource = new BitmapImage(new Uri(files[i], UriKind.RelativeOrAbsolute));
                Person person1 = new Person();
                person1.Fingerprints.Add(fp1);
                Afis.Extract(person1);

                for (int j = 0; j < share.Length; j++)
                {
                    fp2.AsBitmapSource = new BitmapImage(new Uri(share[j], UriKind.RelativeOrAbsolute));
                    Person person2 = new Person();
                    person2.Fingerprints.Add(fp2);

                    Afis.Extract(person2);

                    float score = Afis.Verify(person1, person2);
                    bool match = (score > Afis.Threshold);
                    Console.WriteLine(files[i] + "\n Compare with " + share[j]);
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

        static void normalizeScore(string readFile, string writeFile)
        {

            string[] lines = System.IO.File.ReadAllLines(readFile);
            float[] score = new float[lines.Length];
            
            for (int i = 0; i < lines.Length; i++)
            {
                score[i] = float.Parse(lines[i]);
            }

            float min = score.Min();
            float max = score.Max();

            for (int i = 0; i < score.Length; i++)
            {
                score[i] = (score[i] - min) / (max - min);
            }

            string Path = writeFile;

            using (StreamWriter sw = File.AppendText(Path))
            {
                for (int i = 0; i < score.Length; i++)
                { 
                    sw.WriteLine(score[i].ToString());
                }
            }
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
            GenerateWatermark watermark = new GenerateWatermark();

            int TotalFiles = 90;
            int choice = 0;
            string parentDirectory = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\";
            string readFrom = "";
            string writeTo = "";
            string filename_write = "";
            string filename_read = "";

            string[] InnerImage = new string[TotalFiles];
            string[] files = new string[TotalFiles];
            string[] outerImage = new string[TotalFiles];
            int k = 0;
            for (int i = 0; i < TotalFiles / 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    files[k] = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Database\" + (i + 1).ToString() + "_" + j.ToString() + ".bmp";
                    outerImage[k] = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Outer Images\" + (i + 1).ToString() + "_" + j.ToString() + ".bmp";
                    InnerImage[k] = @"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Inner Images\ROI_" + (i + 1).ToString() + "_" + j.ToString() + ".png";
                    k++;
                }
            }

            Console.WriteLine("1. Generate Score file for original fingerprint");
            Console.WriteLine("2. Generate Score file for outter share fingerprint");
            Console.WriteLine("3. Generate Score file for inner share fingerprint");
            Console.WriteLine("4. Plot FAR and FRR for original fingerprint");
            Console.WriteLine("5. Plot FAR and FRR for outter share fingerprint");
            Console.WriteLine("6. Plot FAR and FRR for inner share fingerprint");
            Console.WriteLine("Select an optrion: ");
            choice = Int32.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    writeTo = "OriginalFingerPrint_" + TotalFiles.ToString() + ".txt";
                    filename_write = parentDirectory + writeTo;
                    buildDB(TotalFiles, filename_write,files,files);

                    readFrom = "OriginalFingerPrint_" + TotalFiles.ToString() + ".txt";
                    writeTo = "NormalizedOriginalFingerPrint_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    filename_write = parentDirectory + writeTo;
                    normalizeScore(filename_read,filename_write);
                    break;
                case 2:
                    writeTo = "OutterShare_" + TotalFiles.ToString() + ".txt";
                    filename_write = parentDirectory + writeTo;
                    buildDB(TotalFiles, filename_write,files, outerImage);

                    readFrom = "OutterShare_" + TotalFiles.ToString() + ".txt";
                    writeTo = "NormalizedOutterShare_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    filename_write = parentDirectory + writeTo;
                    normalizeScore(filename_read, filename_write);
                    break;
                case 3:
                    writeTo = "InnerShare_" + TotalFiles.ToString() + ".txt";
                    filename_write = parentDirectory + writeTo;
                    buildDB(TotalFiles, filename_write,files,InnerImage);

                    readFrom = "InnerShare_" + TotalFiles.ToString() + ".txt";
                    writeTo = "NormalizedInnerShare_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    filename_write = parentDirectory + writeTo;
                    normalizeScore(filename_read, filename_write);
                    break;
                case 4:
                    readFrom = "NormalizedOriginalFingerPrint_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    Application.EnableVisualStyles();
                    Application.Run(new Plots(TotalFiles,filename_read));
                    break;
                case 5:
                    readFrom = "NormalizedOutterShare_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    Application.EnableVisualStyles();
                    Application.Run(new Plots(TotalFiles, filename_read));
                    break;
                case 6:
                    readFrom = "NormalizedInnerShare_" + TotalFiles.ToString() + ".txt";
                    filename_read = parentDirectory + readFrom;
                    Application.EnableVisualStyles();
                    Application.Run(new Plots(TotalFiles, filename_read));
                    break;
                case 7:
                    watermark.CreateWaterMarkImage(InnerImage, outerImage,"InnerHashInOutterShare");
                    break;
                case 8:
                    watermark.CreateWaterMarkImage(outerImage,InnerImage, "OutterHashInInnerShare");
                    break;
                default:
                    break;
            }

            // Finding Bit Error Rate
            /*
            getHash g = new getHash();
            byte[] hash1 = g.getSHAofAnImage(@"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Inner Images\ROI_2_1.png");
            for (int j = 1; j <= 10; j++)
            {
                byte[] hash2 = g.getSHAofAnImage(@"C:\Users\harsh\Documents\Visual Studio 2015\Projects\FingerPrint\FingerPrint\Inner Images\ROI_2_" +j.ToString() +".png");
                string yourByteString = "";
                for (int i = 0;i<hash1.Length;i++)
                {
                    yourByteString = yourByteString + Convert.ToString(hash1[i], 2).PadLeft(8, '0');
                }

                string yourByteString1 = "";
                for (int i = 0; i < hash2.Length; i++)
                {
                    yourByteString1 = yourByteString1 + Convert.ToString(hash2[i], 2).PadLeft(8, '0');
                }
                int Count = 0;
                //Console.WriteLine(yourByteString);
                //Console.WriteLine(yourByteString1);
                for (int i = 0; i < yourByteString.Length; i++)
                {
                    if (yourByteString[i] != yourByteString1[i])
                    {
                        Count++;
                    }
                }
                //Console.WriteLine(yourByteString[0].ToString());
                Console.WriteLine(Count);
                Count = 0;
            }
            */
            //Console.ReadLine();
        }
    }
}
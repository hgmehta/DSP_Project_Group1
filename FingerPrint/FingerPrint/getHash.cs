using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace FingerPrint
{
    class getHash
    {
        public byte[] getSHAofAnImage(string path)
        {
            Bitmap innerImage = new Bitmap(path);
            ImageConverter converter = new ImageConverter();
            //using (Graphics g = Graphics.FromImage(innerImage))
            //{
            //    g.Clear(Color.Red);
            //}
            byte[] rawImageData = converter.ConvertTo(innerImage, typeof(byte[])) as byte[];
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] answer = sha.ComputeHash(rawImageData);
            return answer;
        }
    }
}

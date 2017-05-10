using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrint
{
    class Validation
    {
        public bool compareHash(byte[] hashCode, byte[] retrivedWatermark)
        {
            bool isMatch = true;
            if (retrivedWatermark.Length != hashCode.Length)
            {
                isMatch = false;
            }
            if (isMatch)
            {
                for (int i = 0; i < retrivedWatermark.Length; i++)
                {
                    if (retrivedWatermark[i] != hashCode[i])
                    {
                        isMatch = false;
                    }
                }
            }
            return isMatch;
        }
    }
}

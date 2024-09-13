using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfInformation.lab3.Service
{
    public static class Resizer
    {
        public static List<uint> FromFile(byte[] file, byte resize)
        {
            var result = new List<uint>();

            for (uint i = 0; i<file.Length/resize; i++)
            {
                uint tmp = 0;
                for(uint j = 0; j < resize; j++)
                {
                    tmp <<= 8;
                    tmp += file[i * resize + j];
                }
                result.Add(tmp);
            }

            return result;
        }

        public static byte[] ToFile(List<uint> file, byte resize)
        {
            var result = new byte[file.Count*resize];

            for (int i = 0; i < result.Length; i++)
            {
                var tmp = (file[(i/resize)] & (255 << (byte)(8 * (i % resize)))) >> (byte)(8 * (i % resize));
                var ind = i - i % resize;
                result[ind + (resize - 1 - i % resize)] = (byte)tmp;
            }

            return result;
        }
    }
}

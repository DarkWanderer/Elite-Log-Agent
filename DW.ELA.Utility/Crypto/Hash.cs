using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.Utility.Crypto
{
    public static class Hash
    {
        public static string Sha1(string str)
        {
            using (var sha1 = new SHA1Managed())
                return ByteArrayToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public class RandomStringGenerator
    {
        private static Random r = new Random();

        public static string Generate(int numChar)
        {

            string listOfChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomPassword = "";
            for (int x = 0; x < numChar; x++)
            {
                int position = r.Next(0, 62);
                randomPassword += listOfChars[position];
            }
            return randomPassword;
        }
    }
}

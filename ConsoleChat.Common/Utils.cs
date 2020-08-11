using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleChat.Common
{
    public static class Utils
    {
        public static byte[] Encode(this string str) 
        {
            return Encoding.Unicode.GetBytes(str);
        }
    }
}

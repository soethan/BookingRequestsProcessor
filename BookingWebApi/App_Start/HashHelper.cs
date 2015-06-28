using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BookingWebApi.App_Start
{
    public static class HashHelper
    {
        /// <summary>
        /// 32 bit md5
        /// default encoding is UTF-8
        /// </summary>
        public static string Md532Bit(string plainText, Encoding encoding = null, bool uppercase = true)
        {
            string format = uppercase ? "{0:X2}" : "{0:x2}";
            var result = BytesMD5(plainText, encoding);

            var stringBuilder = new StringBuilder();
            foreach (var b in result)
            {
                stringBuilder.AppendFormat(format, b);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// default encoding is UTF-8
        /// </summary>
        public static byte[] BytesMD5(string data, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return (MD5.Create().ComputeHash(encoding.GetBytes(data)));
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CSharpFlink.Core.Common
{
    public class ZipLib
    {
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (DeflateStream zipStream = new DeflateStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Flush();
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (DeflateStream zipStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Flush();
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] result = Compress(inputBytes);
            return Convert.ToBase64String(result);
        }
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = Decompress(inputBytes);
            return Encoding.UTF8.GetString(depressBytes);
        }
    }
}

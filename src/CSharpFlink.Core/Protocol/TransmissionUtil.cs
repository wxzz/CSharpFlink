using CSharpFlink.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Protocol
{
    public static class TransmissionUtil
    {
        public static string Serialize<T>(T t)
        {
            return SerializeUtil.JsonSerialize(t);
        }

        public static object Deserialize<T>(string json)
        {
            return SerializeUtil.JsonDeserialize<T>(json);
        }

        public static byte[] Compress(byte[] data)
        {
            return ZipLib.Compress(data);
        }

        public static byte[] Decompress(byte[] data)
        {
            return ZipLib.Decompress(data);
        }

        public static byte[] ConvertToBytes(string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        public static string ConvertToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static byte[] SerializeAndCompress<T>(T t)
        {
            string str = Serialize<T>(t);

            str = str.RemoveInvalidChars();

            byte[] data = ConvertToBytes(str);

            return Compress(data);
        }

        public static T[] DecompressAndDeserialize<T>(byte[] bytes)
        {
            List<T> list = new List<T>();
            byte[] data = Decompress(bytes);

            string str = ConvertToString(data);

            int[] indexs = str.GetSplitGroupIndexs("Key", 2);
            if (indexs.Length <= 1)
            {
                T t = (T)Deserialize<T>(str);
                list.Add(t);
            }
            else
            {
                string[] groupMsg = str.GetSplitGroups(indexs, 2);
                foreach (string group in groupMsg)
                {
                    T t = (T)Deserialize<T>(str);
                    list.Add(t);
                }
            }

            return list.ToArray();
        }
    }
}

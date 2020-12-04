using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpFlink.Core.Common
{
    public class ProtobufHelper
    {
        public static byte[] Serialize<T>(T t)
        {
            var stream = new MemoryStream();
            Serializer.Serialize<T>(stream, t);
            var bytes = stream.ToArray();
            stream.Flush();
            stream.Close();
            stream.Dispose();
            return bytes;
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var t = Serializer.Deserialize<T>(stream);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            return t;
        }
    }
}

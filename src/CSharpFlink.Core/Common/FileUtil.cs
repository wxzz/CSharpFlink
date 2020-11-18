using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CSharpFlink.Core.Common
{
    public static class FileUtil
    {
        /// <summary>
        /// 追加内容到指定文件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteAppend(string filePath, string content)
        {
            WriteAppend(filePath,new string[]{content});
        }

        public static void WriteAppend(string filePath, string[] contents, bool newLine = true)
        {
            string content = String.Join(Environment.NewLine, contents);

            if (newLine)
            {
                content += Environment.NewLine;
            }

            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

            WriteAppend(filePath, data);
        }

        public static void WriteAppend(string filePath, byte[] data)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Seek(fs.Length, SeekOrigin.Current);

                fs.Write(data, 0, data.Length);

                fs.Flush();

                fs.Close();
            }
        }

        /// <summary>
        /// 10.7判断两个文件的哈希值是否一致
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        //public static bool CompareFiles(string fileName1, string fileName2)
        //{
        //    using (HashAlgorithm hashAlg = HashAlgorithm.Create())
        //    {
        //        using (FileStream fs1 = new FileStream(fileName1, FileMode.Open), fs2 = new FileStream(fileName2, FileMode.Open))
        //        {
        //            byte[] hashBytes1 = hashAlg.ComputeHash(fs1);
        //            byte[] hashBytes2 = hashAlg.ComputeHash(fs2);

        //            return (BitConverter.ToString(hashBytes1) == BitConverter.ToString(hashBytes2));
        //        }
        //    }
        //}

        /// <summary>
        /// 文件夹路径
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string[] GetFiles(string dirPath)
        {
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            return System.IO.Directory.GetFiles(dirPath);
        }

        /// <summary>
        /// 在指定目录和根目录查找文件，并返回完整路径
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string SearchFile(string dir, string fileName)
        {
            string path = Path.Combine(dir, fileName);
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public static string ReadString(string fileName)
        {
            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        public static byte[] ReadBytes(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool IsExists(string fullpath)
        {
            return System.IO.File.Exists(fullpath);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="fullpath"></param>
        public static void Delete(string fullpath)
        {
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
        }
    }
}

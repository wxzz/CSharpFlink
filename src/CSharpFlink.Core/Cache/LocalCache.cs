using CSharpFlink.Core.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Cache
{
    internal abstract class LocalCache
    {
        protected LocalCache()
        {

        }

        protected string CachePath { get; set; }

        internal string GetFormatFileName(string file)
        {
            return String.Format("{0}.calc", file);
        }

        internal string GetFormatFilePath(string key)
        {
            return System.IO.Path.Combine(CachePath, GetFormatFileName(key));
        }

        //internal void WriteCache(string key,string cache)
        //{
        //    if (!System.IO.Directory.Exists(CachePath))
        //    {
        //        System.IO.Directory.CreateDirectory(CachePath);
        //    }

        //    string filePath = GetFormatFilePath(key);
        //    FileUtil.WriteAppend(filePath, cache);
        //}

        internal void WriteCache(string key, byte[] data)
        {
            if (!System.IO.Directory.Exists(CachePath))
            {
                System.IO.Directory.CreateDirectory(CachePath);
            }

            string filePath = GetFormatFilePath(key);
            FileUtil.WriteAppend(filePath, data);
        }

        internal string ReadCache(string key)
        {
            string filePath = GetFormatFilePath(key);
            if(FileUtil.IsExists(filePath))
            {
                return FileUtil.ReadString(filePath);
            }
            else
            {
                return null;
            }
        }

        internal void DeleteCache(string key)
        {
            string filePath = GetFormatFilePath(key);
            FileUtil.Delete(filePath);
        }

        internal ConcurrentDictionary<string,byte[]> GetCacheDictionary()
        {
            ConcurrentDictionary<string, byte[]> dic = new ConcurrentDictionary<string, byte[]>();
            string[] files=FileUtil.GetFiles(CachePath);
            foreach(string f in files)
            {
                string path = System.IO.Path.Combine(CachePath, f);
                if (FileUtil.IsExists(path))
                {
                    string key = System.IO.Path.GetFileNameWithoutExtension(f);
                    byte[] cache = FileUtil.ReadBytes(path);
                    dic.TryAdd(key, cache);
                }
            }
            return dic;
        }
    }
}

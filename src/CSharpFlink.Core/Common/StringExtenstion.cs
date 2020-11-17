using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpFlink.Core.Common
{
    public static class StringExtenstion
    {
        /// <summary>
        /// 删除所有指定的字符串
        /// </summary>
        /// <param name="destString"></param>
        /// <param name="removeString"></param>
        /// <returns></returns>
        public static string RemoveStrings(this string destString, string removeString)
        {
            while (destString.Contains(removeString))
            {
                destString = destString.Replace(removeString, "");
            }
            return destString;
        }

        public static string RemoveInvalidChars(this string msg)
        {
            return Regex.Replace(msg, @"\s", "");
        }

        public static int[] GetSplitGroupIndexs(this string str, string key="Key", int leftNum=2)
        {
            List<int> indexs = new List<int>();
            bool isFirst = true;
            int pos = 0;
            do
            {
                if (isFirst)
                {
                    pos = str.IndexOf(key, pos);
                    isFirst = false;
                }
                else
                {
                    pos = str.IndexOf(key, pos + key.Length);
                }

                if (pos >= 0)
                {
                    indexs.Add(pos);
                }
            }
            while (pos >= 0);
            return indexs.ToArray();
        }

        public static string[] GetSplitGroups(this string str,int[] indexs,int leftNum)
        {
            if (indexs.Length <= 1)
            {
                return new string[] { str };
            }
            else
            {
                List<string> strArr = new List<string>();
                for (int i = 0; i < indexs.Length; i++)
                {
                    int curIndex = i;
                    int nextIndex = curIndex + 1;

                    int curKeyIndex = indexs[curIndex];
                    int startIndex = curKeyIndex - leftNum;

                    string eleStr = String.Empty;
                    if (nextIndex <= indexs.Length - 1)
                    {
                        int endIndex = indexs[nextIndex];

                        eleStr = str.Substring(startIndex, endIndex - startIndex - leftNum);
                    }
                    else
                    {
                        eleStr = str.Substring(startIndex, str.Length - startIndex);
                    }
                    strArr.Add(eleStr);
                }

                return strArr.ToArray();
            }
        }

        public static string[] GetSplitGroups(this string str, string key, int leftNum)
        {
            List<int> indexs = new List<int>();

            bool isFirst = true;
            int pos = 0;
            do
            {
                if (isFirst)
                {
                    pos = str.IndexOf(key, pos);
                    isFirst = false;
                }
                else
                {
                    pos = str.IndexOf(key, pos + key.Length);
                }

                if (pos >= 0)
                {
                    indexs.Add(pos);
                }
            }
            while (pos >= 0);

            if (indexs.Count <= 1)
            {
                return new string[] { str };
            }
            else
            {
                List<string> strArr = new List<string>();
                for (int i = 0; i < indexs.Count; i++)
                {
                    int curIndex = i;
                    int nextIndex = curIndex + 1;

                    int curKeyIndex = indexs[curIndex];
                    int startIndex = curKeyIndex - leftNum;

                    string eleStr = String.Empty;
                    if (nextIndex <= indexs.Count - 1)
                    {
                        int endIndex = indexs[nextIndex];

                        eleStr = str.Substring(startIndex, endIndex - startIndex - leftNum);
                    }
                    else
                    {
                        eleStr = str.Substring(startIndex, str.Length - startIndex);
                    }
                    strArr.Add(eleStr);
                }

                return strArr.ToArray();
            }
        }
    }
}

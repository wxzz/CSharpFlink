using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Protocol
{
    public class UpTransmisstion
    {
        public UpTransmisstion()
        {

        }

        public string Key { get; set; }

        public UpTransmisstionCode Code { get; set; }

        public string Desc { get; set; }

        public static string GetDesc(UpTransmisstionCode code)
        {
            string desc = "";
            switch(code)
            {
                case UpTransmisstionCode.Ok:
                    desc = "子节点接收成功";
                    break;
                case UpTransmisstionCode.Error:
                    desc = "子节点接收或解析任务失败";
                    break;
            }
            return desc;
        }
    }
}

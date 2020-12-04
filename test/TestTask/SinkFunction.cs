using CSharpFlink.Core.Common;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Sink;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestTask
{
    public class SinkFunction : CSharpFlink.Core.Sink.SinkFunction
    {
        public SinkFunction() : base() { }
        public override void Close()
        {
            
        }

        public override void Invoke(IMetaData[] metaDatas, SinkContext context)
        {
            List<string> list = new List<string>();
            foreach (IMetaData md in metaDatas)
            {
                string val = String.Format("WindowId:{0},TagTime:{1},TagId:{2},TagValue:{3}", md.WindowId, md.TagTime.ToString(), md.TagId, md.TagValue) + Environment.NewLine;
                list.Add(val);
             }
            FileUtil.WriteAppend("sink.txt", list.ToArray());
        }

        public override void Open()
        {
           
        }
    }
}

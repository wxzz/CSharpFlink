using CSharpFlink.Core.Common;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Sink;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestConfluentKafka
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
                string val = String.Format("window_id:{0},tag_time:{1},tag_id:{2},tag_value:{3}", md.window_id, md.tag_time.ToString(), md.tag_id, md.tag_value);
                list.Add(val);
             }
            FileUtil.WriteAppend("sink.txt", list.ToArray());
        }

        public override void Open()
        {
           
        }
    }
}

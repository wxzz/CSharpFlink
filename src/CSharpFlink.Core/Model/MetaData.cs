using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Model
{
    public class MetaData : IMetaData
    {
        public DateTime tag_time { get; set; }
        public string tag_name { get; set; }
        public string tag_id { get; set; }
        public string tag_value { get; set; }
        public string code { get; set; }
        public string ext_value { get; set; }
        public string window_id { get; set; }
    }
}

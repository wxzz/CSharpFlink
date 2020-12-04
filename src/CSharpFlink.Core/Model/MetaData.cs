using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Model
{
    public class MetaData : IMetaData
    {
        public DateTime TagTime { get; set; }
        public string TagName { get; set; }
        public string TagId { get; set; }
        public string TagValue { get; set; }
        public string Code { get; set; }
        public string ExtValue { get; set; }
        public string WindowId { get; set; }
    }
}

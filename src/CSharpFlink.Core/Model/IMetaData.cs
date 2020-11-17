using System;

namespace CSharpFlink.Core.Model
{
    public interface IMetaData
    {
        string window_id { get; set; }
        string code { get; set; }
        string tag_name { get; set; }
        string tag_id { get; set; }
        string tag_value { get; set; }
        string ext_value { get; set; }
        DateTime tag_time { get; set; }
    }
}
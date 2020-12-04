using System;

namespace CSharpFlink.Core.Model
{
    public interface IMetaData
    {
        string WindowId { get; set; }
        string Code { get; set; }
        string TagName { get; set; }
        string TagId { get; set; }
        string TagValue { get; set; }
        string ExtValue { get; set; }
        DateTime TagTime { get; set; }
    }
}
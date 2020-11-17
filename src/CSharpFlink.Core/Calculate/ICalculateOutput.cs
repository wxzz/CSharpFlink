using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculateOutput
    {
        string SessinId { get; set; }

        DateTime OutputDateTime { get; set; }

        IMetaData[] DataSource { get; set; }
    }
}

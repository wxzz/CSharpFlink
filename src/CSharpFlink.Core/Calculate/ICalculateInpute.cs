using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculateInpute
    {
        string SessinId { get; set; }

        DateTime InputeDateTime { get; set; }

        string Script { get; set; }

        IMetaData[] DataSource { get; set; }
    }
}

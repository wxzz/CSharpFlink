using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public class CalculateOutput : ICalculateOutput
    {
        public CalculateOutput():this("",DateTime.Now,new IMetaData[] { })
        { 
        
        }

        public CalculateOutput(string sessinId,DateTime outputDateTime, IMetaData[] dataSource)
        {
            SessinId = sessinId;
            OutputDateTime = outputDateTime;
            DataSource = dataSource;
        }

        public string SessinId { get; set; }
        public DateTime OutputDateTime { get; set; }
        public IMetaData[] DataSource { get; set; }
    }
}

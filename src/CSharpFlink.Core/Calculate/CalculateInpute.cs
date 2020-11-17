using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public class CalculateInpute : ICalculateInpute
    {
        public CalculateInpute():this("","",DateTime.Now,new IMetaData[] { })
        { 
        
        }

        public CalculateInpute(string sessinId,string script,DateTime inputeDateTime,IMetaData[] dataSource)
        {
            SessinId = sessinId;
            InputeDateTime = inputeDateTime;
            DataSource = dataSource;
            Script = script;
        }

        public string SessinId { get; set; }
        public DateTime InputeDateTime { get; set; }
        public IMetaData[] DataSource { get; set; }
        public string Script { get; set; }
    }
}

using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Model;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Sink
{
    public abstract class SinkFunction
    {
        public SinkFunction()
        {
            Properties = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Properties { get; set; }

        public abstract void Open();

        public abstract void Invoke(IMetaData[] metaDatas, SinkContext context);

        public abstract void Close();

        public void SetProperties(string key, string value)
        {
            if (Properties.ContainsKey(key))
            {
                Properties[key] = value;
            }
            else
            {
                Properties.Add(key, value);
            }
        }

        public string GetProperties(string key)
        {
            if (Properties.ContainsKey(key))
            {
                return Properties[key];
            }
            else
            {
                return String.Empty;
            }
        }
    }
}

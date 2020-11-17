using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CSharpFlink.Core.Source
{
    public class SourceContext
    {
        public SourceContext()
        {

        }

        internal event CollectHandler CollectHandler;

        public void Collect(IMetaData[] metaDatas)
        {
            if(CollectHandler!=null)
            {
                CollectHandler(metaDatas);
            }
        }
    }
}

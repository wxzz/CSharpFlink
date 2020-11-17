using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpFlink.Core.Cache
{
    internal class SlaveCache : LocalCache
    {
        internal SlaveCache():base()
        {
            CachePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"slavecache");
        }
    }
}

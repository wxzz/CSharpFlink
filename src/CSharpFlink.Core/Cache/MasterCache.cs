using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpFlink.Core.Cache
{
    internal class MasterCache:LocalCache
    {
        internal MasterCache():base()
        {
            CachePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"mastercache");
        }
    }
}

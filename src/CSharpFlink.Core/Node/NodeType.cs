using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Node
{
    public enum NodeType : byte
    {
        Master=0x00,
        Slave=0x01,
        Both=0x02
    }
}

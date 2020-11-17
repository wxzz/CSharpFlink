using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Channel
{
    public delegate void ConnectHandler(string channelId);

    public delegate void DisconnectHandler(string channelId);

    //public delegate void ReceiveDownTransmisstionHandler(DownTransmission downTransmission);

    public delegate void ReceiveDownTransmisstionHandler(byte[] taskMsg);

    public delegate void ReceiveUpTransmisstionHandler(byte[] upTransmisstion);
}

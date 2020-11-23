using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Channel
{
    public interface IChannelMessageHandler
    {
        void OnConnect(IChannelHandlerContext context);

        void OnDisonnect(IChannelHandlerContext context);

        void OnReceiveTransmisstion(IChannelHandlerContext context, byte[] msg);

        void Send(string channelId, byte[] data, out string remoteInfo);

        int ClientCount { get; }
    }
}

using CSharpFlink.Core.Log;
using CSharpFlink.Core.Protocol;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Channel
{
    public class MasterMessageHandler : ChannelHandlerAdapter
    {
        private IChannelMessageHandler _mh;
        public MasterMessageHandler(IChannelMessageHandler ms)
        {
            _mh = ms;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);

            _mh.OnConnect(context);

        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);

            _mh.OnDisonnect(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            IByteBuffer byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                if (byteBuffer.HasArray)
                {
                    int length = byteBuffer.ReadableBytes;
                    byte[] array = new byte[length];
                    byteBuffer.GetBytes(byteBuffer.ReaderIndex,array);

                    _mh.OnReceiveTransmisstion(context,array);
                }
                else
                {
                    Logger.Log.Info(true, "主节点，没有读到有效的数据");
                }

                ReferenceCountUtil.Release(message);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
            context.CloseAsync();
        }
    }
}

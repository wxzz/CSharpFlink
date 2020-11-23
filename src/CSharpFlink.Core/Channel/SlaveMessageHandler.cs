using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Worker;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Channel
{
    public class SlaveMessageHandler : ChannelHandlerAdapter
    {
        private IChannelMessageHandler _mh;
        public SlaveMessageHandler(IChannelMessageHandler mh)
        {
            _mh = mh;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);

            _mh.OnConnect(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);

            _mh.OnDisonnect(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
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
                    byteBuffer.GetBytes(byteBuffer.ReaderIndex, array);

                    _mh.OnReceiveTransmisstion(context,array);
                }
                else
                {
                    Logger.Log.Info(true, "工作节点，没有读到有效的数据");
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

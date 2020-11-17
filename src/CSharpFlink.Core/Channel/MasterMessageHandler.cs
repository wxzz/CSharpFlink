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
        public MasterMessageHandler()
        {
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
            string id = context.Channel.Id.AsLongText();
            MasterServer.Clients.TryAdd(id, context);
            MasterServer.OnConnect(id);
            Logger.Log.Info(true, "子节点连接:" + context.Channel.RemoteAddress.ToString() + ",子节点数:" + MasterServer.Clients.Count.ToString());
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            IChannelHandlerContext removeChannel;
            string id = context.Channel.Id.AsLongText();
            MasterServer.Clients.TryRemove(id, out removeChannel);
            base.ChannelUnregistered(context);
            MasterServer.OnDisonnect(id);
            Logger.Log.Info(true, "子节点断开:" + context.Channel.RemoteAddress.ToString() + ",子节点数:" + MasterServer.Clients.Count.ToString());
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

                    MasterServer.OnReceiveUpTransmisstion(array);
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

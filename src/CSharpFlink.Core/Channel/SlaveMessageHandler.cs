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
        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            if(SlaveClient.Channel!=null)
            {
                SlaveClient.Channel.CloseAsync();
                SlaveClient.Channel = null;
            }    

            SlaveClient.DoConnect = true;

            Logger.Log.Info(true, "断开到主节点:" + GlobalConfig.Config.MasterIp + " " + GlobalConfig.Config.MasterListenPort.ToString());

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

                    SlaveClient.OnReceiveTask(array);
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

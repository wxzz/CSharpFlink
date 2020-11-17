using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Worker;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Channel
{
    public class MasterServer
    {
        private MultithreadEventLoopGroup _bossGroup;
        private MultithreadEventLoopGroup _workerGroup;
        private ServerBootstrap _bootstrap;
        private IChannel _bootstrapChannel;

        public static ConcurrentDictionary<string, IChannelHandlerContext> Clients = new ConcurrentDictionary<string, IChannelHandlerContext>();

        internal static event ConnectHandler Connect;

        internal static event DisconnectHandler Disconnect;

        internal static event ReceiveUpTransmisstionHandler ReceiveUpTransmisstion;

        public MasterServer()
        {
            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();
            _bootstrap=new ServerBootstrap();
        }

        public void Start()
        {
            try
            {
                _bootstrap.Group(_bossGroup, _workerGroup);
                _bootstrap.Channel<TcpServerSocketChannel>();
                _bootstrap.Option(ChannelOption.SoBacklog, 100);
                _bootstrap.ChildOption(ChannelOption.SoKeepalive, true);
                _bootstrap.Option(ChannelOption.TcpNodelay, true);
                _bootstrap.Option(ChannelOption.SoReuseport, true);
                _bootstrap.ChildOption(ChannelOption.SoReuseport, true);
                _bootstrap.Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);
                _bootstrap.ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);
                _bootstrap.ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("DotNetty-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("DotNetty-dec", new LengthFieldBasedFrameDecoder(GlobalConfig.Config.MaxFrameLength, 0, 4, 0, 4));
                    pipeline.AddLast(new MasterMessageHandler());
                }));

                _bootstrapChannel = _bootstrap.BindAsync(GlobalConfig.Config.MasterListenPort).Result;
                Logger.Log.Info(false, "主节点侦听端口:"+ GlobalConfig.Config.MasterListenPort);
            }
            catch(Exception ex)
            {
                Logger.Log.Error(true,"",ex);
            }
        }

        public void Stop()
        {
            CloseAll();

            if (_bootstrapChannel!=null)
            {
                try
                {
                    _bootstrapChannel.CloseAsync();
                }
                catch
                { }
            }

            if(_bossGroup!=null)
            {
                try
                {
                    _bossGroup.ShutdownGracefullyAsync();
                }
                catch
                { }
            }

            if (_workerGroup != null)
            {
                try
                {
                    _workerGroup.ShutdownGracefullyAsync();
                }
                catch
                { }
            }
        }

        internal static void OnConnect(string id)
        {
            if (Connect != null)
            {
                Connect(id);
            }
        }

        internal static void OnDisonnect(string id)
        {
            if (Connect != null)
            {
                Disconnect(id);
            }
        }

        internal static void OnReceiveUpTransmisstion(byte[] upMsg)
        {
            if(ReceiveUpTransmisstion!=null)
            {
                ReceiveUpTransmisstion(upMsg);
            }
        }

        public static void Send(IWorker worker, byte[] data, out string remoteInfo)
        {
            if(data==null)
            {
                Logger.Log.Info(true, "MasterServer.Send的data参数为空");
                remoteInfo = "";
                return;
            }

            string id = worker.Id;
            if (Clients.ContainsKey(id))
            {
                IChannelHandlerContext context=null;
                if (Clients.TryGetValue(id, out context))
                {
                    if (context != null)
                    {
                        lock (context)
                        {
                            IByteBuffer buffer = Unpooled.WrappedBuffer(data);
                            context.WriteAndFlushAsync(buffer);
                            remoteInfo = context.Channel.RemoteAddress.ToString();
                        }
                    }
                    else
                    {
                        remoteInfo = "未知";
                    }
                }
                else
                {
                    remoteInfo = "未知";
                }
            }
            else
            {
                remoteInfo = "未知";
            }
        }

        public static void CloseAll()
        {
            foreach (KeyValuePair<string, IChannelHandlerContext> kv in Clients)
            {
                try
                {
                    kv.Value.Channel.CloseAsync();
                }
                catch
                {
                }
            }
        }

        public static int ClientCount
        {
            get
            {
                return Clients.Count;
            }
        }
    }
}

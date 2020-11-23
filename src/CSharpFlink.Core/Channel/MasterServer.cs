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
    public class MasterServer : IChannelMessageHandler
    {
        private MultithreadEventLoopGroup _bossGroup;
        private MultithreadEventLoopGroup _workerGroup;
        private ServerBootstrap _bootstrap;
        private IChannel _bootstrapChannel;

        public ConcurrentDictionary<string, IChannelHandlerContext> _Clients;

        internal event ConnectHandler Connect;

        internal event DisconnectHandler Disconnect;

        internal event ReceiveTransmisstionHandler ReceiveTransmisstionHandler;

        private string _localIp = "127.0.0.1";
        private int _localPort = 7007;

        public MasterServer(string ip= "127.0.0.1", int port= 7007)
        {
            _Clients = new ConcurrentDictionary<string, IChannelHandlerContext>();
            _localIp = ip;
            _localPort = port;
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
                    pipeline.AddLast(new MasterMessageHandler((IChannelMessageHandler)this));
                }));

                _bootstrapChannel = _bootstrap.BindAsync(_localPort).Result;
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

        public void OnConnect(IChannelHandlerContext context)
        {
            string id = context.Channel.Id.AsLongText();
            _Clients.TryAdd(id, context);

            if (Connect != null)
            {
                Connect(id);
            }

            Logger.Log.Info(true, "子节点连接:" + context.Channel.RemoteAddress.ToString() + ",子节点数:" + _Clients.Count.ToString());
        }

        public void OnDisonnect(IChannelHandlerContext context)
        {
            string id = context.Channel.Id.AsLongText();
            IChannelHandlerContext removeChannel;
            _Clients.TryRemove(id, out removeChannel);
            if (Connect != null)
            {
                Disconnect(context.Channel.Id.AsLongText());
            }
            Logger.Log.Info(true, "子节点断开:" + context.Channel.RemoteAddress.ToString() + ",子节点数:" + _Clients.Count.ToString());
        }

        public void OnReceiveTransmisstion(IChannelHandlerContext context,byte[] upMsg)
        {
            if(ReceiveTransmisstionHandler!=null)
            {
                ReceiveTransmisstionHandler(upMsg);
            }
        }

        public void Send(string channelId, byte[] data, out string remoteInfo)
        {
            if(data==null)
            {
                Logger.Log.Info(true, "MasterServer.Send的data参数为空");
                remoteInfo = "";
                return;
            }

            if (_Clients.ContainsKey(channelId))
            {
                IChannelHandlerContext context=null;
                if (_Clients.TryGetValue(channelId, out context))
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

        public void CloseAll()
        {
            foreach (KeyValuePair<string, IChannelHandlerContext> kv in _Clients)
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

        public int ClientCount
        {
            get
            {
                return _Clients.Count;
            }
        }
    }
}

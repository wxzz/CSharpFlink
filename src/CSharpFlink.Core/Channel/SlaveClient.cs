using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Channel
{
    public class SlaveClient : IChannelMessageHandler
    {
        private MultithreadEventLoopGroup _bossGroup;
        private Bootstrap _bootstrap=null;

        private Thread _connectThread;

        private bool _connectThreadExit = false;

        private ManualResetEvent _manualResetEvent = new ManualResetEvent(true);

        internal event ReceiveTransmisstionHandler ReceiveTransmisstionHandler;

        private int _connectInterval = 2000;

        private bool DoConnect
        {
            set
            {
                if(value)
                {
                    _manualResetEvent.Set();
                }
                else
                {
                    _manualResetEvent.Reset();
                }
            }
        }

        private IChannel Channel { get; set; }

        private string _remoteIp = "127.0.0.1";
        private int _remotePort = 7007;

        public SlaveClient(string remoteIp= "127.0.0.1", int remotePort=7007)
        {
            _remoteIp = remoteIp;
            _remotePort = remotePort;

            _bossGroup = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();

            _connectThread = new Thread(new ThreadStart(ConnectThread));
            _connectThread.Name = "Connect Thread";
            _connectThread.IsBackground = true;
        }

        private void ConnectThread()
        {
            while (true)
            {
                if (_connectThreadExit)
                {
                    break;
                }

                try
                {
                    _manualResetEvent.WaitOne();
                   
                    _bootstrap.RemoteAddress(IPAddress.Parse(_remoteIp),_remotePort);
                    Task<IChannel> taskConnect = _bootstrap.ConnectAsync();

                    taskConnect.Wait();
                }
                catch(ThreadInterruptedException)
                {
                    break;
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "连接主节点异常-", ex);
                }

                Thread.Sleep(_connectInterval);
            }
        }

        public void Start()
        {
            try
            {
                _bootstrap.Group(_bossGroup);
                _bootstrap.Channel<TcpSocketChannel>();
                _bootstrap.Option(ChannelOption.TcpNodelay, true);
                _bootstrap.Option(ChannelOption.SoKeepalive, true);
                _bootstrap.Option(ChannelOption.SoReuseport, true);
                _bootstrap.Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);
                _bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>((channel) =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("DotNetty-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("DotNetty-dec", new LengthFieldBasedFrameDecoder(GlobalConfig.Config.MaxFrameLength, 0, 4, 0, 4));
                    pipeline.AddLast(new SlaveMessageHandler((IChannelMessageHandler)this));
                   
                }));
                _connectThread.Start();
            }
            catch (Exception ex) 
            {
                Logger.Log.Error(true, "", ex);
            }
        }

        public void Stop()
        {
            if(_connectThread!=null)
            {
                _connectThreadExit = true;
                _manualResetEvent.Set();
                _connectThread.Join(_connectInterval*2);

                try
                {
                    _manualResetEvent.Dispose();
                }
                catch
                { }

                try
                {
                    _connectThread.Interrupt();
                }
                catch
                { }
            }

            if(Channel!=null)
            {
                try
                {
                    Channel.CloseAsync();
                }
                catch
                { }
            }

            if (_bossGroup != null)
            {
                try
                {
                    _bossGroup.ShutdownGracefullyAsync();
                }
                catch
                { }
            }
        }

        public void Send(string channelId, byte[] data, out string remoteInfo)
        {
            remoteInfo = String.Empty;
            if (data == null)
            {
                Logger.Log.Info(true, "SlaveClient.Send的data参数为空");
                return;
            }

            if (Channel!=null)
            {
                IByteBuffer buffer = Unpooled.WrappedBuffer(data);
                Channel.WriteAndFlushAsync(buffer);
                Logger.Log.Info(false, "向主节点发送信息:" + _remoteIp + " " + _remotePort);
            }
            else
            {
                Logger.Log.Info(true, "向主节点发送信息，未获得有效连接");
            }
        }

        public void OnReceiveTransmisstion(IChannelHandlerContext context,byte[] msg)
        {
            if(ReceiveTransmisstionHandler!=null)
            {
                ReceiveTransmisstionHandler(msg);
            }
        }

        public void OnConnect(IChannelHandlerContext context)
        {
            if (Channel == null)
            {
                Channel = context.Channel;

                DoConnect = false;
            }

            Logger.Log.Info(true, "连接到主节点:" + _remoteIp + " " + _remotePort);
        }

        public void OnDisonnect(IChannelHandlerContext context)
        {
            if (Channel != null)
            {
                Channel.CloseAsync();
                Channel = null;
            }

            DoConnect = true;

            Logger.Log.Info(true, "断开到主节点:" + _remoteIp + " " + _remotePort);
        }

        public int ClientCount
        { 
            get
            {
                if(Channel!=null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

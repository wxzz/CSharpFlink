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

namespace CSharpFlink.Core.Channel
{
    public class SlaveClient
    {
        private MultithreadEventLoopGroup _bossGroup;
        private Bootstrap _bootstrap=null;

        private Thread _connectThread;

        private bool _connectThreadExit = false;

        private static ManualResetEvent _manualResetEvent = new ManualResetEvent(true);

        internal static event ReceiveDownTransmisstionHandler ReceiveDownTransmisstion;

        private int _connectInterval = 2000;

        public static bool DoConnect
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

        public static IChannel Channel { get; set; }
        public SlaveClient()
        {
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
                   
                    string masterIp = GlobalConfig.Config.MasterIp;
                    int masterPort = GlobalConfig.Config.MasterListenPort;

                    _bootstrap.RemoteAddress(IPAddress.Parse(masterIp),masterPort);
                    IChannel channel = _bootstrap.ConnectAsync().Result;

                    Channel = channel;

                    DoConnect = false;

                    Logger.Log.Info(true, "连接到主节点:" + GlobalConfig.Config.MasterIp + " " + GlobalConfig.Config.MasterListenPort.ToString());
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
                    pipeline.AddLast(new SlaveMessageHandler());
                   
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

        public static void Send(byte[] data)
        {
            if (data == null)
            {
                Logger.Log.Info(true, "SlaveClient.Send的data参数为空");
                return;
            }

            if (Channel!=null)
            {
                IByteBuffer buffer = Unpooled.WrappedBuffer(data);
                Channel.WriteAndFlushAsync(buffer);
                Logger.Log.Info(false, "向主节点发送信息:" + GlobalConfig.Config.MasterIp + " " + GlobalConfig.Config.MasterListenPort.ToString());
            }
            else
            {
                Logger.Log.Info(true, "向主节点发送信息，未获得有效连接");
            }
        }

        internal static void OnReceiveTask(byte[] taskMsg)
        {
            if(ReceiveDownTransmisstion!=null)
            {
                ReceiveDownTransmisstion(taskMsg);
            }
        }
    }
}

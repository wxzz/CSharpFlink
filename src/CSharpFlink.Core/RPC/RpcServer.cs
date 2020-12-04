using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Task;
using CSharpFlink.Core.Window.Operator;
using Hprose.IO;
using Hprose.RPC;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CSharpFlink.Core.RPC
{
    public class RpcServer : IDisposable
    {
        private static readonly object syncRoot = new object();

        private bool _disposed = false;

        public string IP { get; private set; }
        public int Port { get; private set; }

        private HttpListener _httpServer { get; set; }

        private Service _rpcService { get; set; }

        private IRpcTaskExcute _rpcTaskExcute { get; set; }

        public RpcServer(string ip, int port, IRpcTaskExcute excute)
        {
            IP = ip;

            Port = port;

            _rpcTaskExcute = excute;

            _rpcService = new Service();

            _httpServer = new HttpListener();

            _httpServer.Prefixes.Add(String.Concat("http://", IP, ":", Port.ToString(), "/"));
        }

        public void Start()
        {
            if (_httpServer != null)
            {
                _httpServer.Start();

                _rpcService.Bind(_httpServer);

                TypeManager.Register<RpcContext>("RpcContext");
                TypeManager.Register<MetaData>("MetaData");

                _rpcService.AddInstanceMethods(_rpcTaskExcute, typeof(IRpcTaskExcute).Namespace);

                if (_httpServer.IsListening)
                {
                    Logger.Log.Info(false, "Hprose.RPC服务启动成功,正在侦听端口：" + Port);
                }
            }
            else
            {
                Logger.Log.Warn(false, "请初始化RPC服务");
            }
        }

        public void Stop()
        {
            this.Dispose();
        }

        ~RpcServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_httpServer != null
                   && _httpServer.IsListening)
                {
                    _httpServer.Stop();
                    _httpServer.Close();
                    _httpServer = null;
                }

                if (_rpcService != null)
                {
                    _rpcService.Dispose();
                    _rpcService = null;
                }
            }

            _disposed = true;
        }
    }
}

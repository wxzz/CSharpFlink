using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Window.Operator;
using Hprose.IO;
using Hprose.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.RPC
{
    public class RpcClient:IDisposable,IRpcTaskExcute
    {
        private Client client;
        private bool _disposed = false;

        public RpcClient(string ip = "127.0.0.1", int port = 8007)
        {
            client = new Client(string.Concat("http://", ip, ":", port, "/"));

            TypeManager.Register<RpcContext>("RpcContext");
            TypeManager.Register<MetaData>("MetaData");

            //client.Timeout = TimeSpan.FromSeconds(30);
        }

        public bool Remove(RpcContext[] rpcContexts)
        {
            try
            {              
                return client.UseService<IRpcTaskExcute>(typeof(IRpcTaskExcute).Namespace).Remove(rpcContexts);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "Remove:", ex);
                return false;
            }
        }

        public bool InsertOrUpdate(RpcContext[] rpcContexts)
        {
            try
            {
               return client.UseService<IRpcTaskExcute>(typeof(IRpcTaskExcute).Namespace).InsertOrUpdate(rpcContexts);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "InsertOrUpdate:", ex);
                return false;
            }
        }

        public bool AddMetaData(MetaData[] mds)
        {
            try
            {
                return client.UseService<IRpcTaskExcute>(typeof(IRpcTaskExcute).Namespace).AddMetaData(mds);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "AddMetadata:", ex);
                return false;
            }
        }
        public bool ContainsTaskId(string taskId, CalculateType calculateType)
        {
            try
            {
                return client.UseService<IRpcTaskExcute>(typeof(IRpcTaskExcute).Namespace).ContainsTaskId(taskId,calculateType);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "ContainsTaskId:", ex);
                return false;
            }
        }
        public string[] GetAllTaskId(CalculateType calculateType)
        {
            try
            {
                return client.UseService<IRpcTaskExcute>(typeof(IRpcTaskExcute).Namespace).GetAllTaskId(calculateType);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "ContainsTaskId:", ex);
                return null;
            }
        }
        ~RpcClient()
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
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }
            }
            _disposed = true;
        }     
    }
}

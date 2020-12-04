using CSharpFlink.Core.Common;
using CSharpFlink.Core.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Config
{
    public class GlobalConfig
    {
        private static string _path =System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"cfg","global.cfg");

        private static readonly object SyncObject = new object();

        private static GlobalConfig _GlobalConfig;

        public static string Path
        {
            get
            {
                return _path;
            }

            private set
            {
                _path = value;
            }
        }

        public static void Save(GlobalConfig ds)
        {
            Save(_path, ds);
        }

        protected static void Save(string path, GlobalConfig ds)
        {
            SerializeUtil.XmlSerialize(path, ds);
        }

        public static GlobalConfig Config
        {
            get
            {
                if (_GlobalConfig == null)
                {
                    lock (SyncObject)
                    {
                        if (_GlobalConfig == null)
                        {
                            Load();
                        }
                    }
                }
                return _GlobalConfig;
            }
        }

        public static void Load()
        {
            lock (SyncObject)
            {
                if (!System.IO.File.Exists(Path))
                {
                    string dir = System.IO.Path.GetDirectoryName(Path);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                }

                if (!System.IO.File.Exists(Path))
                {
                    Save(new GlobalConfig());
                }

                _GlobalConfig = SerializeUtil.XmlDeserailize<GlobalConfig>(Path);
            }
        }

        public static void ChangeConfig(string path)
        {
            Path = path;

            Load();
        }

        public GlobalConfig()
        {
            MaxDegreeOfParallelism = 3;
            MasterListenPort = 7007;
            MasterIp = "127.0.0.1";
            NodeType = NodeType.Master;
            RemoteInvokeInterval = 1000;
            RepeatRemoteInvokeInterval = 5000;
            SlaveExcuteCalculateInterval = 1000;
            MaxFrameLength = 524288;//512KB
            WorkerPower = 2;
            RpcIp = "127.0.0.1";
            RpcListenPort = 8007;
        }

        /// <summary>
        /// 并行处理数量
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// 主节点侦听端口
        /// </summary>
        public int MasterListenPort { get; set; }

        /// <summary>
        /// 远程主节点IP
        /// </summary>
        public string MasterIp { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 远程调用检测时间
        /// </summary>
        public int RemoteInvokeInterval { get; set; }

        /// <summary>
        /// 如果发送不成功，重新发送间隔检测时间,缓存
        /// </summary>
        public int RepeatRemoteInvokeInterval{ get; set; }

        /// <summary>
        /// 子节点调用间隔
        /// </summary>
        public int SlaveExcuteCalculateInterval { get; set; }

        /// <summary>
        /// 数据交互最大缓冲
        /// </summary>
        public int MaxFrameLength { get; set; }

        /// <summary>
        /// 工作者能量系数。
        /// </summary>
        public int WorkerPower { get; set; }

        /// <summary>
        /// Rpc侦听端口
        /// </summary>
        public int RpcListenPort { get; set; }

        /// <summary>
        /// Rpc服务IP
        /// </summary>
        public string RpcIp { get; set; }
    }
}

using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Sink;
using CSharpFlink.Core.Source;
using CSharpFlink.Core.Task;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CSharpFlink.Core.Execution
{
    public class ExecutionEnvironment
    {
        internal List<SourceFunctionThread> SourceFunctionThreads;

        internal static List<SinkFunction> Sinks { get; set; }

        public static TaskAssembly TaskAssembly { get; set; }

        static ExecutionEnvironment()
        {
            Sinks = new List<SinkFunction>();
            TaskAssembly = new TaskAssembly();
        }

        internal ExecutionEnvironment(string[] args)
        {
            SourceFunctionThreads = new List<SourceFunctionThread>();

            if(args!=null && args.Length>0)
            {
                Dictionary<string,string> dic = FormatArgs.GetArgsDictionary(args);

                if (dic.ContainsKey(FormatArgs.c_c))
                {
                    string cfgPath = dic[FormatArgs.c_c];

                    if(System.IO.File.Exists(cfgPath))
                    {
                        GlobalConfig.ChangeConfig(cfgPath);
                    }
                }

                TaskAssembly.CheckArgsTaskFile(dic);
            }

            Start();
        }

        public static ExecutionEnvironment GetExecutionEnvironment(string[] args=null)
        {
            return new ExecutionEnvironment(args);
        }

        private MasterNode _masterNode;
        private SlaveNode _slaveNode;

        public IMasterTaskManager TaskManager 
        {
            get
            {
                if (_masterNode != null)
                {
                    return _masterNode.TaskManager;
                }
                else
                {
                    Logger.Log.Info(false, "请执行:Start(NodeType.Master)");
                    return null;
                }
            }
        }

        private void Start()
        {
            NodeType nt=GlobalConfig.Config.NodeType;
            if (nt == NodeType.Master)
            {
                _masterNode = new MasterNode();
                _masterNode.Start();
            }
            else if(nt == NodeType.Slave)
            {
                _masterNode = new MasterNode();
                _slaveNode = new SlaveNode();
                _slaveNode.Start();
            }
            else if(nt==NodeType.Both)
            {
                _masterNode = new MasterNode();
                _masterNode.Start();

                _slaveNode = new SlaveNode();
                _slaveNode.Start();
            }
        }

        public void Stop()
        {
            if(SourceFunctionThreads.Count>0)
            {
                foreach (SourceFunctionThread t in SourceFunctionThreads)
                {
                    try
                    {
                        t.SourceFunction.Cancel();
                    }
                    catch { }

                    try
                    {
                        t.Thread.Join(t.SourceFunction.Interval*2);
                        t.Thread.Interrupt();
                    }
                    catch { }
                }

                SourceFunctionThreads.Clear();
            }

            NodeType nt = GlobalConfig.Config.NodeType;
            if (nt == NodeType.Master)
            {
                if (_masterNode != null)
                {
                    _masterNode.Stop();
                }
            }
            else if (nt == NodeType.Slave)
            {
                if (_slaveNode != null)
                {
                    _slaveNode.Stop();
                }
            }
            else if (nt == NodeType.Both)
            {
                if (_masterNode != null)
                {
                    _masterNode.Stop();
                }

                if (_slaveNode != null)
                {
                    _slaveNode.Stop();
                }
            }
        }

        public void AddSource(SourceFunction sf)
        {
            if(sf!=null)
            {
                try
                {
                    sf.Init();
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "SourceFuntion:", ex);
                }

                sf.Context = new SourceContext();
                sf.Context.CollectHandler += SourceFunction_CollectHandler;
                Thread thread = new Thread(new ParameterizedThreadStart(sf.Run));
                thread.IsBackground = true;
                thread.Name = "SourceFunction:" + sf.GetHashCode().ToString();

                SourceFunctionThread sourceFunctionThread = new SourceFunctionThread(sf,thread);

                SourceFunctionThreads.Add(sourceFunctionThread);
            }
        }

        public void AddSink(SinkFunction sf)
        {
            Sinks.Add(sf);
        }

        public void ExcuteSource()
        {
            foreach(SourceFunctionThread sft in SourceFunctionThreads)
            {
                sft.Thread.Start(sft.SourceFunction.Context);
            }
        }

        public void SourceFunction_CollectHandler(IMetaData[] metaDatas)
        {
            if(_masterNode!=null)
            {
                foreach (IMetaData md in metaDatas)
                {
                    _masterNode.TaskManager.AddMetaData(md.window_id, md);
                }
            }
            else
            {
                Logger.Log.Info(false, "请执行:Start(NodeType.Master)");
            }
        }
    }
}

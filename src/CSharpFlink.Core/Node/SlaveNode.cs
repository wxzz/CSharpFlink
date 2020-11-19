using CSharpFlink.Core.Channel;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Task;
using CSharpFlink.Core.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Node
{
    public class SlaveNode
    {
        private SlaveClient _slaveClient;
        public ISlaveTaskManager TaskManager;
        public SlaveNode()
        {
            _slaveClient = new SlaveClient(GlobalConfig.Config.MasterIp, GlobalConfig.Config.MasterListenPort);
            TaskManager = new SlaveTaskManager();
        }

        public void Start()
        {
            _slaveClient.ReceiveDownTransmisstion += SlaveClient_ReceiveTask;
            _slaveClient.Start();
        }

        public void Stop()
        {
            _slaveClient.ReceiveDownTransmisstion -= SlaveClient_ReceiveTask;
            _slaveClient.Stop();

            TaskManager.Dispose();
        }

        private void SlaveClient_ReceiveTask(byte[] taskMsg)
        {
            TaskManager.AddTask(taskMsg);
        }
    }
}

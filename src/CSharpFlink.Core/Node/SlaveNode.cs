using CSharpFlink.Core.Channel;
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
        //private IWorker _defaultWorker;
        public SlaveNode()
        {
            _slaveClient = new SlaveClient();
            TaskManager = new SlaveTaskManager();
        }

        public void Start()
        {
            SlaveClient.ReceiveDownTransmisstion += SlaveClient_ReceiveTask;
            _slaveClient.Start();
        }

        public void Stop()
        {
            SlaveClient.ReceiveDownTransmisstion -= SlaveClient_ReceiveTask;
            _slaveClient.Stop();

            TaskManager.Dispose();
        }

        private void SlaveClient_ReceiveTask(byte[] taskMsg)
        {
            TaskManager.AddTask(taskMsg);
            //if(_defaultWorker==null)
            //{
            //    _defaultWorker = Worker.Worker.GetDefaultWorker();
            //}

            //_defaultWorker.DoWork(calculateContext);
        }
    }
}

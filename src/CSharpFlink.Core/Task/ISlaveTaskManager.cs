using System;

namespace CSharpFlink.Core.Task
{
    public interface ISlaveTaskManager : IDisposable
    {
        void AddTask(byte[] taskMsg);
    }
}
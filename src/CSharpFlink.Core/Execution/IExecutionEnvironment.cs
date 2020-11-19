using CSharpFlink.Core.Sink;
using CSharpFlink.Core.Source;
using CSharpFlink.Core.Task;

namespace CSharpFlink.Core.Execution
{
    public interface IExecutionEnvironment
    {
        IMasterTaskManager TaskManager { get; }

        void AddSink(SinkFunction sf);

        void AddSource(SourceFunction sf);

        void ExcuteSource();
    }
}
using CSharpFlink.Core.Calculate;

namespace CSharpFlink.Core.Worker
{
    public interface IWorker
    {
        string Id { get; set; }
        string Name { get; set; }

        event PublishCalculateCompleted PublishCalculateCompleted;

        void DoWork(ICalculateContext context);
    }
}
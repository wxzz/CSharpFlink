using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Window;
using System.Threading.Tasks.Dataflow;

namespace CSharpFlink.Core.Task
{
    public interface IGlobalContext
    {
        ActionBlock<ICalculateContext> ActionBlock { get; set; }
    }
}
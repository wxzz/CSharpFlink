using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace CSharpFlink.Core.Task
{
    public class GlobalContext : IGlobalContext
    {
        public GlobalContext(ActionBlock<ICalculateContext> actionBlock)
        {
            ActionBlock = actionBlock;
        }
        public ActionBlock<ICalculateContext> ActionBlock { get; set; }
    }
}

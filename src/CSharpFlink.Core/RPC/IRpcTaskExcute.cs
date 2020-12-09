using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.RPC
{
    public interface IRpcTaskExcute
    {
        bool Remove(RpcContext[] rpcContexts);

        bool InsertOrUpdate(RpcContext[] rpcContexts);

        bool AddMetaData(MetaData[] mds);
        bool ContainsTaskId(string taskId, CalculateType calculateType);
        string[] GetAllTaskId(CalculateType calculateType);
    }
}

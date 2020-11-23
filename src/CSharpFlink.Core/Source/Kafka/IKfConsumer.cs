using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Source.Kafka
{
    internal interface IKfConsumer : IDisposable
    {      
        /// <summary>
        /// 开始
        /// </summary>
        void Start();
        /// <summary>
        /// 消息事件接收
        /// </summary>      
        event Action<Message<string, string>> ReceiveMessage;
        /// <summary>
        /// 正常启动
        /// </summary>
        event Action<string> OnStart;
    }
}

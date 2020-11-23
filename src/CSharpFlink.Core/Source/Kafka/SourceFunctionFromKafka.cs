using Confluent.Kafka;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Source.Kafka
{
    public class SourceFunctionFromKafka:SourceFunction
    {             
        SourceContext _sc;
        IKfConsumer _ConsumerInterface;
        public SourceFunctionFromKafka(string brokerList,string groupId,List<string> topicName):base()
        {
            _ConsumerInterface = new KfConsumer(brokerList, groupId, topicName);          
        }

        public override void Cancel()
        {
            if (_ConsumerInterface != null)
            {
                _ConsumerInterface.Dispose();
                _ConsumerInterface.OnStart -= ConsumerInterface_OnStart;
                _ConsumerInterface.ReceiveMessage -= MessageDeal;
                _ConsumerInterface = null;
            }
        }

        public override void Init()
        {
            _ConsumerInterface.OnStart += ConsumerInterface_OnStart;
            _ConsumerInterface.ReceiveMessage += MessageDeal;
            _ConsumerInterface.Start();
        }

        public override void Run(object context)
        {
            try
            {
                _sc = (SourceContext)context;                   
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "SourceFunctionFromKafka:", ex);
            }
        }
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="msg">Message<TKey, TValue> key可为空，TValue是要获取的值</param>
        public void MessageDeal(Message<string, string> msg)
        {
            try
            {
                IMetaData[] mds = SerializeUtil.JsonDeserialize<IMetaData[]>(msg.Value.ToString());  
                _sc?.Collect(mds);
                Logger.Log.Info(false, string.Concat("消息队列收到消息:key--",msg.Key));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "MessageDeal:获取消息报错", ex);
            }
        }

        private void ConsumerInterface_OnStart(string message)
        {
            Logger.Log.Info(false, message);
            Logger.Log.Info(false, "消息队列服务启动，开始消息接收。。。");
        }     
    }
}

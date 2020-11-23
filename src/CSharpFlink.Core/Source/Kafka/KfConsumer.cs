using Confluent.Kafka;
using System;
using System.Collections.Generic;
using CSharpFlink.Core.Log;
using System.Threading;

namespace CSharpFlink.Core.Source.Kafka
{
    public class KfConsumer : IKfConsumer
    {
        /// <summary>
        /// 主题列表
        /// </summary>
         List<string> _topic { get; }
        /// <summary>
        /// 块列表
        /// </summary>
         string _brokerList { get; }
        /// <summary>
        /// 组ID
        /// </summary>
         string _GroupId { get; }
        /// <summary>
        /// 接收消息事件
        /// </summary>
        public event Action<Message<string, string>> ReceiveMessage;
        /// <summary>
        /// 正常启动
        /// </summary>
        public event Action<string> OnStart;      
        IConsumer<string, string> KafkaSubscribe = null;
        Thread Consumerthread;
        bool runFlag;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_brokerList">服务器Ip端口地址以，分割</param>
        /// <param name="_GroupId">组Id</param>
        /// <param name="topicLst">订阅主题，如果多主题，以，分割</param>
        public KfConsumer(string brokerList,string groupId,List<string> topicLst)
        {
            try
            {
                _topic = new List<string>();
                _topic = topicLst;
                _GroupId = groupId;
                _brokerList = brokerList;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, string.Concat("组编号", _GroupId, ",订阅主题", string.Join(",", _topic), ",消息队列初始化失败,", ex.Message, ex.StackTrace));
            }
        }
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            try
            {
                runFlag = true;
                Consumerthread = new Thread(() => ConsumerMsg());
                Consumerthread.Name = "Confluent.Kafka";
                Consumerthread.IsBackground = true;
                Consumerthread.Start();
                OnStart?.Invoke("组编号" + _GroupId + ",订阅主题" + string.Join(",", _topic));
            }
            catch (Exception ex)
            {
                Logger.Log.Warn(true, string.Concat("组编号", _GroupId, ",主题", string.Join(",", _topic), "消息队列启动失败", ex.Message, ex.StackTrace));
            }
        }
        
        private void KafkaSub()
        {
            var conf = new ConsumerConfig
            {
                GroupId = _GroupId,
                BootstrapServers = _brokerList,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            KafkaSubscribe = new ConsumerBuilder<string, string>(conf)
            .SetErrorHandler((_, e) =>
            {
                Logger.Log.Error(true, string.Concat("组编号", _GroupId, ",主题", string.Join(",", _topic), "消息队列服务异常", e.Reason));
            })
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                Console.WriteLine($"分配分区: [{string.Join(", ", partitions)}]");
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                Console.WriteLine($"取消分配: [{string.Join(", ", partitions)}]");
            })
            .Build();
            List<TopicPartition> topicPartitions = new List<TopicPartition>();
            foreach (var item in _topic)
            {
                topicPartitions.Add(new TopicPartition(item, new Partition()));
            }
            KafkaSubscribe.Assign(topicPartitions);
            KafkaSubscribe.Subscribe(_topic);
        }
        /// <summary>
        /// 消费线程
        /// </summary>
        private void ConsumerMsg()
        {
            try
            {
                KafkaSub();
                using (KafkaSubscribe)
                {
                    while (runFlag)
                    {
                        try
                        {

                            ConsumeResult<string, string> consume;
                            try
                            {
                                consume = KafkaSubscribe.Consume();
                            }
                            catch (Exception e)
                            {
                                Logger.Log.Warn(true, string.Concat("组编号", _GroupId, ",主题", string.Join(",", _topic), "消息队列获取消息失败", e.Message, e.StackTrace));
                                continue;
                            }
                            if (consume.IsPartitionEOF)
                            {
                                Console.WriteLine($"到达主题 {consume.Topic}底部, 分区 {consume.Partition}, 偏移量 {consume.Offset}.");
                                continue;
                            }
                            ReceiveMessage?.Invoke(consume.Message);
                        }
                        catch (ThreadInterruptedException)
                        {
                            break;
                        }
                        catch (ConsumeException e)
                        {
                            Logger.Log.Warn(true, string.Concat("组编号", _GroupId, ",主题", string.Join(",", _topic), "消息队列获取消息失败", e.Message, e.StackTrace));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    KafkaSubscribe.Close();
                }
                catch { }
                Logger.Log.Warn(true, string.Concat("组编号", _GroupId, ",主题", string.Join(",", _topic), "消息队列获取消息失败", e.Message, e.StackTrace));
            }
        }

        public void Dispose()
        {
            try
            {
                runFlag = false;
                Consumerthread.Join(TimeSpan.FromMilliseconds(500));
                Consumerthread.Interrupt();
                Consumerthread = null;
            }
            catch { }
            if (KafkaSubscribe != null)
            {
                try
                {
                    KafkaSubscribe.Close();
                }
                catch { }
                try
                {
                    KafkaSubscribe.Unassign();
                    KafkaSubscribe.Unsubscribe();
                    KafkaSubscribe = null;
                }
                catch { }
            }
        }     
    }
}

using CSharpFlink.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestConfluentKafka
{
    public class KafkaConf
    {
        private static object SyncLook = new object();

        static KafkaConf()
        {
            if (Instance == null)
            {
                lock (SyncLook)
                {
                    if (Instance == null)
                    {
                        Instance = Load();
                    }
                }
            }
        }

        public KafkaConf()
        {
            MQHosts = new string[] { "127.0.0.1:9092" };
            RequestTimeout = 5000;
            TimeToLive = 10000;
            GroupID = "Flink";
            TopicName =new List<string> { "Metdata" };
            Partitions = 12;
        }

        private static readonly string MQConfigPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
    "KafkaConf.cfg");

        public static KafkaConf Instance { get; private set; }

        public static KafkaConf Load()
        {
            KafkaConf config = null;
            if (!System.IO.File.Exists(MQConfigPath))
            {
                config = new KafkaConf();
                Save(config);
            }
            else
            {
                config = SerializeUtil.XmlDeserailize<KafkaConf>(MQConfigPath);
            }
            return config;

        }

        public static void Save(KafkaConf config)
        {
            SerializeUtil.XmlSerialize(MQConfigPath, config);
        }

        public string[] MQHosts { get; set; }
        /// <summary>
        /// 发送数据超时时间
        /// </summary>
        public int RequestTimeout { get; set; }
        /// <summary>
        /// 超过这个时间，MQ不再补发数据到消费者
        /// </summary>
        public int TimeToLive { get; set; }
        /// <summary>
        ///消息消息组编号编号
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public List<string> TopicName { get; set; }
        /// <summary>
        /// 分区数
        /// </summary>
        public int Partitions { get; set; }

    }
}

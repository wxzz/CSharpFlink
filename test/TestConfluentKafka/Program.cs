using Confluent.Kafka;
using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Source.Kafka;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestCommon;

namespace TestConfluentKafka
{
    class Program
    {
        static IExecutionEnvironment executionEnvironment;
        static string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks", "tasks.cfg");

        public static int TaskCount = 12;
        static int _windowInterval = 5;
        static int _delayWindowCount = 2;
        static ICalculate _calculate =null;
        static ProducerConfig config;
        static void Main(string[] args)
        {
            config = new ProducerConfig
            {
                BootstrapServers = string.Join(',', KafkaConf.Instance.MQHosts) //BootstrapServers属性为以逗号隔开的多个代理地址
            };
            Task.Run(() => Produce());
            if (System.IO.File.Exists(Path))
            {
                try
                {
                    TaskCount = int.Parse(System.IO.File.ReadAllText(Path).Trim());
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(true, "读TaskCount异常:", ex);
                }
            }

            executionEnvironment = ExecutionEnvironment.GetExecutionEnvironment(args);

            for (int i = 0; i < TaskCount; i++)
            {
                string key = i.ToString("0000");
                if (!executionEnvironment.TaskManager.ContainsWindow(key))
                {
                    _windowInterval = Calc.GetRandomWindowInterval();
                    _calculate = Calc.GetAggRandomCalculate(key+"_result");
                    executionEnvironment.TaskManager.AddOrUpdateWindowTask(key, $"窗口{key}", _windowInterval, _delayWindowCount, _calculate);
                }
            }

            executionEnvironment.AddSource(new SourceFunctionFromKafka(string.Join(',', KafkaConf.Instance.MQHosts), KafkaConf.Instance.GroupID, KafkaConf.Instance.TopicName));
            executionEnvironment.AddSink(new SinkFunction());
            executionEnvironment.ExcuteSource();

            //while (true)
            //{
            //    var line = Console.ReadLine();
            //    if (line == "exit")
            //    {
            //        executionEnvironment.Stop();                    
            //        break;
            //    }
            //}
            Console.ReadKey();
        }


        static void Produce()
        {
            try
            {
                int partitions = 0;
                while (true)
                {

                    if (partitions == KafkaConf.Instance.Partitions)
                    {
                        partitions = partitions % KafkaConf.Instance.Partitions;
                    }

                    IMetaData[] metaDatas = new IMetaData[]
                    {
                         new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=0.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=0.ToString("0000")
                        },
                          new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=1.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=1.ToString("0000")
                        },
                           new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=2.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=2.ToString("0000")
                        }
                           , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=3.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=3.ToString("0000")
                        },
                          new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=4.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=4.ToString("0000")
                        }
                           , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=5.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=5.ToString("0000")
                        } , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=6.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=6.ToString("0000")
                        }
                         , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=7.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=7.ToString("0000")
                        }
                          , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=8.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=8.ToString("0000")
                        }
                           , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=9.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=9.ToString("0000")
                        }
                             , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=10.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=10.ToString("0000")
                        }
                              , new MetaData
                       {
                    Code=null,
                    ExtValue=null,
                    TagId=11.ToString("0000"),
                    TagValue=new Random().Next(1,100).ToString(),
                    TagTime=DateTime.Now,
                    WindowId=11.ToString("0000")
                        }
                    };
                    string json = SerializeUtil.JsonSerialize(metaDatas);
                    using (var p = new ProducerBuilder<string, string>(config).Build())
                    {
                        // 异步发送消息到主题
                        var result = p.ProduceAsync(new TopicPartition(string.Join(',', KafkaConf.Instance.TopicName), new Partition(partitions)), new Message<string, string> { Key = Guid.NewGuid().ToString("N"), Value = json }).GetAwaiter().GetResult();
                        Console.WriteLine($"发送消息到分区{result.TopicPartition}-key:{result.Key.ToString()}成功！");
                    }
                    partitions++;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (ProduceException<string, string> ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}

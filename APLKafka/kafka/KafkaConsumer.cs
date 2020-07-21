using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APLActions.Models;
using BlazorApp1.Data.kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using static APLActions.Models.AplLog;

namespace APLKafka.kafka
{
    public class KafkaConsumer
    {
        private static ulong _Offset = 0;
        private ILogger _aplLog;
        private List<string> _topics = new List<string>();
        private IConsumer<string, string> _consumer;
        private InfoModel _aplLogMsg = new InfoModel { };

        private KafkaSetting _kafkaSetting;
        public KafkaConsumer(ILogger<KafkaConsumer> logger, KafkaSetting kafkaSetting)
        {
            this._aplLog = logger;
            _kafkaSetting = kafkaSetting;
            //configuration.GetSection("KafkaSetting").Bind(_kafkaSetting);
            Init(_kafkaSetting);
            this.InitTopics(_kafkaSetting);
        }
        public void Init(KafkaSetting kafkaSetting)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSetting.IpPort,
                EnableAutoCommit = true,
                EnablePartitionEof = true,
                GroupId = "bgi_lims_consumer_yzq",
                AutoOffsetReset = AutoOffsetReset.Latest,
            };
            this._consumer = new ConsumerBuilder<string, string>(config).Build();
            Task.Run(() => ReceiveMessageAsync());
        }

        public void InitTopics(KafkaSetting kafkaSetting)
        {
            _topics.Add(this._kafkaSetting.ProducerTopics.DeviceTopic);
            _topics.Add(this._kafkaSetting.ProducerTopics.TaskTopic);
            _topics.Add(this._kafkaSetting.ProducerTopics.StorageTopic);
            _topics.Add(this._kafkaSetting.ConsumerTopics.DeviceTopic);
            _topics.Add(this._kafkaSetting.ConsumerTopics.TaskTopic);
            _topics.Add(this._kafkaSetting.ConsumerTopics.StorageTopic);
        }

        public ConsumeResult<string, string> ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    _consumer.Subscribe(_topics);
                    var result = _consumer.Consume(20000);
                    if (!result.IsPartitionEOF)
                    {
                        
                        _aplLogMsg.Content = result.Message.Value;
                        _aplLog.LogInformation($"{result.Message.Key},topic:{result.Topic},msg:{result.Message.Value}");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _aplLog.LogError($"{ex}");
                }
                Thread.Sleep(100);
            }
            
            
        }
        public ConsumeResult<string, string> ReceiveMessageAsync(string topic)
        {
            while (true)
            {
                try
                {
                    _consumer.Subscribe(topic);
                    var result = _consumer.Consume(20000);
                    if (!result.IsPartitionEOF)
                    {

                        _aplLogMsg.Content = result.Message.Value;
                        _aplLog.LogInformation($"{result.Message.Key},topic:{result.Topic},msg:{result.Message.Value}");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _aplLog.LogError($"{ex}");
                }
                Thread.Sleep(100);
            }


        }
    }
}

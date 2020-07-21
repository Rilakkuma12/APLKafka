using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using BlazorApp1.Data.kafka;
using Microsoft.Extensions.Logging;
using Polly;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Data
{
    public class KafkaProducer
    {
        private static ulong _Offset = 0;
        private ILogger _aplLog;
        private IProducer<string, string> _producer;
        private List<string> _topics = new List<string>();
        
        private KafkaSetting _kafkaSetting;
        public KafkaProducer(ILogger<KafkaProducer> logger, KafkaSetting kafkaSetting) 
        {
            this._aplLog = logger;
            _kafkaSetting = kafkaSetting;
            //configuration.GetSection("KafkaSetting").Bind(_kafkaSetting);
            Init(_kafkaSetting);
            this.InitTopics(_kafkaSetting);
        }
        public void Init(KafkaSetting kafkaSetting)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSetting.IpPort
            };
            this._producer = new ProducerBuilder<string, string>(config).Build();
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

        public List<string> GetTopics()
        {
            return _topics; 
        }

        public bool SendMessageAsync(string key, string topic, string message, bool writeLog = true)
        {
            if (string.IsNullOrEmpty(message) || message == "null")
            {
                _aplLog?.LogWarning("SendMessageAsync message is null");
                return false;
            }
            if (_kafkaSetting.KafkaEnable)
            {
                if (!_topics.Contains(topic))
                {
                    this._aplLog?.LogError($"{key} | send message to bgi failed : topic/{topic} is illegal");
                    return false;
                }
                var msg = new Message<string, string>
                {
                    Key = _Offset++.ToString(),
                    Value = message
                };
                var result = Policy.Handle<Exception>(ex =>
                {
                    if (ex != null)
                    {
                        this._aplLog?.LogError($"{key} | produceAsync error | {ex.ToString()}");
                        return true;
                    }
                    return false;
                })
                .WaitAndRetry(3, i => TimeSpan.FromSeconds(0.5),
                (ex, tm) => { this._aplLog.LogError($"{key}| produceAsync retry {tm} |{ex.ToString()}"); })
                .ExecuteAndCapture(() =>  _producer.ProduceAsync(topic, msg).Wait());

                _aplLog?.LogInformation($"send kafka message, topic:{topic}, msg:{message}");
                if (result.FinalException != null)
                    return false;
                return true;
            }
            else
            {
                return false;
            }

        }
    }

}

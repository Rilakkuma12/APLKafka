using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Data.kafka
{
    public class KafkaSetting
    {
        public string IpPort { get; set; }
        public string ConsumerGroupId { get; set; }
        public ConsumerTopics ConsumerTopics { get; set; }
        public ProducerTopics ProducerTopics { get; set; }
        public bool KafkaEnable { get; set; }

    }
    public class ConsumerTopics
    {
        public string DeviceTopic { get; set; }
        public string TaskTopic { get; set; }
        public string StorageTopic { get; set; }
    }

    public class ProducerTopics
    {
        public string DeviceTopic { get; set; }
        public string TaskTopic { get; set; }
        public string StorageTopic { get; set; }
    }
}

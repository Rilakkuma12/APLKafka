using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APLActions.Models
{
    public class AplLog
    {
        private readonly ILogger _logger;

        public AplLog(ILogger<AplLog> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException($"-0xFF[2615306]0xEE-{nameof(logger)}");
        }


        public void Info(string traceId, string caller, object content, string description)
        {
            var info = new InfoModel { Content = content, Caller = caller, Description = description, TraceId = traceId };
            this._logger.LogInformation(JsonConvert.SerializeObject(info));
        }

        public void Error(string traceId, string caller, object content, string description)
        {
            var error = new ErrorModel { Content = content, Caller = caller, Description = description, TraceId = traceId };
            this._logger?.LogError(JsonConvert.SerializeObject(error));
        }

        public class InfoModel
        {
            public string TraceId { get; set; }

            public string Description { get; set; }

            public string Caller { get; set; }

            public object Content { get; set; }
        }

        public class ErrorModel
        {
            public string TraceId { get; set; }

            public string Caller { get; set; }

            public object Content { get; set; }

            public string Description { get; set; }
        }
    }
}

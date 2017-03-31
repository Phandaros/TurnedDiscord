using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.HttpServer.Json
{
    class RequestInfo
    {
        [JsonProperty]
        public static int RequestNumber { get; set; }
        [JsonProperty]
        public static readonly string StartupTime = DateTime.UtcNow.ToString("R");
        [JsonProperty("ResponseType")]
        public string ResponseCode { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }

        public RequestInfo()
        {
            ++RequestNumber;
            //Default constructor
        }
        public RequestInfo(string message = null, RequestInfoType responseType = RequestInfoType.Success)
        {
            ++RequestNumber;
            Message = message;
            ResponseCode = responseType.ToString();
        }
    }
    enum RequestInfoType
    {
        Success, MethodWithSpecifiedParametersNotFound, FatalError
    }
}

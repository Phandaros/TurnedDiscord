using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnturnedBot.Discord.HttpServer.Json;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord.HttpServer
{
    class ServerMain
    {
        // To enable this so that it can be run in a non-administrator account:
        // Open an Administrator command prompt.
        // netsh http add urlacl http://+:8008/ user=Everyone listen=true

        static HttpListener Listener = null;
        static int RequestNumber = 0;

        public static void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener is not supported on this platform.");
                return;
            }
            Listener = new HttpListener()
            {
                Prefixes = { "http://+:3070/"}
            };
            Listener.Start();

            // Begin waiting for requests.
            Listener.BeginGetContext(GetContextCallback, null);

            Logger.Log("Listening to requests, Prefixes: " + string.Join(", ", Listener.Prefixes));
        }
        static void GetContextCallback(IAsyncResult ar)
        {
            int req = ++RequestNumber;
            // Get the context
            var context = Listener.EndGetContext(ar);

            // listen for the next request
            Listener.BeginGetContext(GetContextCallback, null);

            // get the request
            var requestTime = DateTime.Now;

            string responseToRequest;
            if (context.Request.Url.Segments.Length > 1)
            {
                var methodName = context.Request.Url.Segments[1].Replace("/", "");
                var parameters = context.Request.Url.Segments.Skip(2).Select(s => s.Replace("/", "")).ToArray();
                responseToRequest = CheckForEndpoints(methodName, parameters);
            }
            else
            {
                responseToRequest = JsonConvert.SerializeObject(new RequestInfo(), Formatting.Indented);
            }

            bool isFromBrowser = true;
            var queries = context.Request.QueryString;
            foreach (var query in queries.AllKeys)
            {
                if (query == "unturned")
                    isFromBrowser = false;
            }

            if (isFromBrowser)
                responseToRequest = responseToRequest.Replace("\r\n", "<br>").Replace(" ", "&nbsp;");

            byte[] buffer = Encoding.UTF8.GetBytes(responseToRequest);
            // and send it
            var response = context.Response;
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            Logger.Log("[WebRequest] " + context.Request.RawUrl);
        }
        static string CheckForEndpoints(string methodName, string[] strParams)
        {
            var method = typeof(EndPoints)
                                .GetMethods()
                                .FirstOrDefault(mt => ParamsLenghtIsEqual(mt, methodName, strParams));

            if (method == null) return JsonConvert.SerializeObject(new RequestInfo(responseType:RequestInfoType.MethodWithSpecifiedParametersNotFound), Formatting.Indented);

            object[] parameters = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            object ret = method.Invoke(typeof(EndPoints), parameters);

            return JsonConvert.SerializeObject(ret, Formatting.Indented);
        }
        static bool ParamsLenghtIsEqual(MethodInfo method, string methodName,string[] parameters)
        {
            var attributes = method.GetCustomAttributes(true);
            if (!attributes.Any(attr => attr is EnpointAttribute && (attr as EnpointAttribute).Map.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            var methodParams = method.GetParameters();
            if (methodParams.Length != parameters.Length)
               return false;

            return true;
        }
    }
}

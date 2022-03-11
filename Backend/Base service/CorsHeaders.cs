using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Base_service
{
    //https://stackoverflow.com/questions/60615806/wcf-host-to-add-custom-http-header-to-response

    //Class responsible for checking each incoming message, and sending messages to the console about recieving requests and sending responses
    public class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        readonly Dictionary<string, string> requiredHeaders;
        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            requiredHeaders = headers ?? new Dictionary<string, string>();
        }
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            string displayText = $"Server has received a request to the following address:\n{request.Headers.To.ToString().Split('?')[0]}";
            Console.WriteLine(displayText);
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (!reply.Properties.ContainsKey("httpResponse"))
                reply.Properties.Add("httpResponse", new HttpResponseMessageProperty());

            var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
            foreach (var item in requiredHeaders)
            {
                httpHeader.Headers.Add(item.Key, item.Value);
            }

            WebOperationContext ctx = WebOperationContext.Current;
            if (ctx.IncomingRequest.Method == "OPTIONS")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
            }

                string displayText = "Server responded to request.\n";
            Console.WriteLine(displayText);

        }
    }

    //Custom made behavior, it's function is to put the CORS headers as headers into the responses,
    //because WCF does not have an integrated CORS support like ASP.net does
    public class MyBehaviorAttribute : Attribute, IContractBehavior, IContractBehaviorAttribute
    {
        public Type TargetContract => typeof(MyBehaviorAttribute);

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            var requiredHeaders = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS" },
                { "Access-Control-Allow-Headers", "x-requested-with,content-type, accept" },
                { "Access-Control-Max-Age", "1728000" }
            };

            dispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) { }
    }
}
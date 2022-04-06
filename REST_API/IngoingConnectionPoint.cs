using ESB_ConnectionPoints.PluginsInterfaces;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace REST_API
{
    class IngoingConnectionPoint : IStandartIngoingConnectionPoint
    {
        public IMessageFactory MessageFactory { get; }
        public ESB_ConnectionPoints.PluginsInterfaces.ILogger Logger { get; }
        Server Server = new Server();

        public IngoingConnectionPoint(IServiceLocator serviceLocator)
        {
            if (serviceLocator != null)
            {
                this.Logger = serviceLocator.GetLogger(this.GetType());
                this.MessageFactory = serviceLocator.GetMessageFactory();
            }
        }
        public void Initialize()
        {

        }

        public void Run(IMessageHandler messageHandler, CancellationToken ct)
        {
            string adress = @"http://+:3367/api/";
            Server.Start(adress, ct, messageHandler);
        }
        public void Cleanup()
        {

        }
        public void Dispose()
        {

        }
    }
    class Server
    {
        private int count = 0;
        public async void Start(string listenerPrefix , CancellationToken ct, IMessageHandler messageHandler)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();

            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                string requestBody;
                var requestStream = listenerContext.Request.InputStream;
                using (StreamReader sr = new StreamReader(requestStream, Encoding.UTF8))
                {
                    requestBody = sr.ReadToEnd();
                }

                Message message = new Message();
                message.Body = Encoding.UTF8.GetBytes(requestBody);
                message.Id = Guid.NewGuid();
                message.AddPropertyWithValue("PATH", listenerContext.Request.RawUrl);
                message.AddPropertyWithValue("METHOD", listenerContext.Request.HttpMethod);

                if (!string.IsNullOrEmpty(requestBody))
                    message.Type = "DTP";
                else
                    message.Type = "Service";

                if (messageHandler.HandleMessage(message))
                {
                    listenerContext.Response.StatusCode = 200;
                    listenerContext.Response.Close();
                }
                else
                {
                    listenerContext.Response.StatusCode = 500;
                    listenerContext.Response.Close();
                }    
            }
            if (ct.IsCancellationRequested)
                listener.Stop();
        }
        private async void ProcessRequest(HttpListenerContext listenerContext)
        {

        }
    }
}

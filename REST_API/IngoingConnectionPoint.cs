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
        Server Server;

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
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(adress);
            listener.Start();
            Logger.Debug($"Сервис запущен по адресу {adress}");

            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext listenerContext = listener.GetContext();
                var Method = listenerContext.Request.HttpMethod;
                var Path = listenerContext.Request.RawUrl;
                string requestBody;
                var a = listenerContext.Request.InputStream;
                using (StreamReader sr = new StreamReader(a, Encoding.UTF8))
                {
                    requestBody = sr.ReadToEnd();
                }
                Logger.Debug(requestBody);
                if (!string.IsNullOrEmpty(requestBody))
                {
                    Message message = new Message
                    {
                        Body = Encoding.UTF8.GetBytes(requestBody),
                        Id = Guid.NewGuid(),
                        Type = "DTP"
                    };
                    if (messageHandler.HandleMessage(message))
                    {
                        listenerContext.Response.StatusCode = 200;
                        listenerContext.Response.Close();
                    }
                }           
            }
            if(ct.IsCancellationRequested)
                listener.Stop();
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
        public async void Start(string listenerPrefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                var Method = listenerContext.Request.HttpMethod;
                var Path = listenerContext.Request.RawUrl;
                string requestBody;
                var a = listenerContext.Request.InputStream;
                using (StreamReader sr = new StreamReader(a, Encoding.UTF8))
                {
                    requestBody = sr.ReadToEnd();
                }

                listenerContext.Response.StatusCode = 200;
                listenerContext.Response.Close();
            }
        }
        private async void ProcessRequest(HttpListenerContext listenerContext)
        {

        }
    }
}

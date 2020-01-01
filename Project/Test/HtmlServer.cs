using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class HtmlServer : IDisposable
    {
        private HtmlServer(string url, string html)
        {
            this.Url = url;
            this.Html = html;
        }

        public string Url { get;  }

        public string Html { get; }

        private HttpListener listener;

        private void Start()
        {
            
            if(listener != null && listener.IsListening)
            {
                listener.Abort();
            }

            listener = new HttpListener();
            listener.Prefixes.Add(this.Url);
            listener.Start();

            void callback(IAsyncResult ar)
            {
                var context = listener.EndGetContext(ar);
                listener.BeginGetContext(callback, null);
                var request = context.Request;
                var response = context.Response;

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/html;charset=UTF-8";
                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    writer.Write(this.Html);
                }
                response.Close();
            }
            listener.BeginGetContext(callback, null);
        }
        
        public static HtmlServer Create(string html)
        {
            var address = GetLocalhostAddress();
            var server = new HtmlServer(address, html);
            server.Start();
            return server;
        }

        private static string GetLocalhostAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return $"http://localhost:{port}/";
        }

        public void Close()
        {
            if (listener != null && listener.IsListening)
            {
                listener.Abort();
            }
            listener = null;
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}

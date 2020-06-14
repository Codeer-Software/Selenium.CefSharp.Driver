using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Test
{
    public class TestDirHtmlServer : IDisposable
    {
        public TestDirHtmlServer()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            RootDir = Path.Combine(dir, @"Test");
            this.RootUrl = GetLocalhostAddress();
        }

        public string RootDir { get; } 

        public string RootUrl { get;  }

        private HttpListener listener;

        public void Start()
        {
            if(listener != null && listener.IsListening)
            {
                listener.Abort();
            }

            listener = new HttpListener();
            listener.Prefixes.Add(this.RootUrl);
            listener.Start();

            void callback(IAsyncResult ar)
            {
                var context = listener.EndGetContext(ar);
                listener.BeginGetContext(callback, null);
                var request = context.Request;
                var response = context.Response;

                var fileName = request.Url.ToString().Replace(RootUrl, string.Empty);
                var path = Path.Combine(RootDir, fileName);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/html;charset=UTF-8";
                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    try
                    {
                        var html = File.ReadAllText(path);
                        writer.Write(html);
                    }
                    catch { }
                }
                response.Close();
            }
            listener.BeginGetContext(callback, null);
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

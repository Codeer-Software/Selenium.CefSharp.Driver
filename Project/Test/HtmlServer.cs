using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Test
{
    [SetUpFixture]
    public class HtmlServer
    {
        internal static HtmlServerCore Instance { get; private set; }

        public HtmlServer() { }

        [OneTimeSetUp]
        public void Setup()
        {
            Instance = HtmlServerCore.StartNew();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Instance.Dispose();
            Instance = null;
        }
    }

    public class HtmlServerCore : IDisposable
    {
        string _rootDir;
        HttpListener listener;

        public string RootUrl { get; }

        HtmlServerCore()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            _rootDir = Path.Combine(dir, @"Test\Html");
            RootUrl = GetLocalhostAddress();
        }

        public static HtmlServerCore StartNew()
        {
            var server = new HtmlServerCore();
            server.Start();
            return server;
        }

        public void Close()
        {
            if (listener != null && listener.IsListening)
            {
                listener.Abort();
            }
            listener = null;
        }

        public void Dispose() => Close();

        void Start()
        {
            if (listener != null && listener.IsListening)
            {
                listener.Abort();
            }

            listener = new HttpListener();
            listener.Prefixes.Add(RootUrl);
            listener.Start();

            void callback(IAsyncResult ar)
            {
                var context = listener.EndGetContext(ar);
                listener.BeginGetContext(callback, null);
                var request = context.Request;
                var response = context.Response;

                var fileName = request.Url.ToString().Replace(RootUrl, string.Empty);
                var path = Path.Combine(_rootDir, fileName);

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

        static string GetLocalhostAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return $"http://localhost:{port}/";
        }
    }
}
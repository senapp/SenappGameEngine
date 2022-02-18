using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Senapp.Engine.Networking.Json;

namespace Senapp.Engine.Networking.Server
{
    public class NetworkServer
    {
        private static Thread serverThread;
        public static void Start(string ipAddress = "localhost", int port = 3333, bool optimize = false)
        {
            Port = port;
            IPAddress = ipAddress;
            Optimize = optimize;

            Console.WriteLine($"[SERVER] Trying to start server with settings:");
            Console.WriteLine($"[SERVER] Port: {Port}");
            Console.WriteLine($"[SERVER] IPAddress: {IPAddress}");
            Console.WriteLine($"[SERVER] Optimize: {Optimize}");

            serverThread = new Thread(Run)
            {
                Name = "ServerThread",
            };

            serverThread.Start();
        }

        public static string IPAddress { get; private set; } = "localhost";
        public static int Port { get; private set; } = 3333;
        public static string Url => $"http://{IPAddress}:{Port}/";
        public static bool ServerRunning => serverRunning;
        public static bool Optimize { get; private set; } = false;

        private static HttpListener listener;
        private static readonly List<string> connectedClients = new();
        private static bool serverRunning = false;

        private static async void Run()
        {
            Console.WriteLine("[SERVER] Starting...");
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(Url);
                listener.Start();
                serverRunning = true;

                TryAddGetHandler("connections", Connections);
                TryAddPostHandler("connect", HandleConnection);

                Console.WriteLine("[SERVER] Listening for connections on {0}", Url);

                while (true)
                {
                    HttpListenerContext ctx = await listener.GetContextAsync();

                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    if (!Optimize) Console.WriteLine($"[SERVER] {req.HttpMethod} Request on {req.Url}");

                    if (req.HttpMethod == "POST") 
                    {
                        HandlePost(req, resp).GetAwaiter().GetResult();
                    }
                    else if (req.HttpMethod == "GET")
                    {
                        HandleGet(req, resp).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER][ERROR] {e.Message}");
            }
            finally
            {
                listener?.Close();
            }
        }

        private static NetworkPacket HandleConnection(string clientId)
        {
            if (!connectedClients.Contains(clientId))
            {
                connectedClients.Add(clientId);
                return new NetworkPacket(HttpStatusCode.Created, "connect", JsonConvert.SerializeObject(connectedClients), $"'Client with id {clientId} connected'");
            }
            else
            {
                return new NetworkPacket(HttpStatusCode.Conflict, "connect", JsonConvert.SerializeObject(connectedClients), $"'Client with id {clientId} already connected'");
            }
        }
        private static NetworkPacket Connections()
        {
            return new NetworkPacket(HttpStatusCode.OK, "connections", JsonConvert.SerializeObject(connectedClients), string.Empty);
        }

        private static readonly Dictionary<string, Func<string, NetworkPacket>> postHandlers = new();
        private static readonly Dictionary<string, Func<NetworkPacket>> getHandlers = new();

        public static bool TryAddPostHandler(string postName, Func<string, NetworkPacket> handler)
        {
            try
            {
                postHandlers.Add(postName.ToLower(), handler);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER][ERROR] {e.Message}");
            }

            return false;
        }
        public static bool TryAddGetHandler(string getName, Func<NetworkPacket> handler)
        {
            try
            {
                getHandlers.Add(getName.ToLower(), handler);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER][ERROR] {e.Message}");
            }

            return false;
        }

        private static async Task HandlePost(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string content;
            using (Stream receiveStream = req.InputStream)
            {
                using (StreamReader readStream = new(receiveStream, Encoding.UTF8))
                {
                    content = readStream.ReadToEnd();
                }
            }

            var postType = req.Url.AbsolutePath.Substring(1).ToLower();
            var response = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                if (postHandlers.TryGetValue(postType, out var handler))
                {
                    var result = handler.Invoke(content);
                    response = JsonConvert.SerializeObject(Optimize ? result.Optimize() : result);
                    resp.StatusCode = (int)result.StatusCode;
                }
                else
                {
                    resp.StatusCode = 404;
                    response = JsonConvert.SerializeObject(new NetworkPacket((HttpStatusCode)resp.StatusCode, postType, string.Empty, $"'Invalid request: {postType}'"));
                }
            }
            else
            {
                resp.StatusCode = 404;
                response = JsonConvert.SerializeObject(new NetworkPacket((HttpStatusCode)resp.StatusCode, postType, string.Empty, $"'No data for request: {postType}'"));
            }

            byte[] data = Encoding.UTF8.GetBytes(response);
            resp.ContentType = "text/plain";
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
        private static async Task HandleGet(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var getType = req.Url.AbsolutePath.Substring(1).ToLower();
            var response = string.Empty;
            if (getHandlers.TryGetValue(getType, out var handler))
            {
                var result = handler.Invoke();
                response = JsonConvert.SerializeObject(Optimize ? result.Optimize() : result);
                resp.StatusCode = (int)result.StatusCode;           
            }
            else
            {
                resp.StatusCode = 404;
                response = JsonConvert.SerializeObject(new NetworkPacket((HttpStatusCode)resp.StatusCode, getType, string.Empty, $"'Invalid request: {getType}'"));
            }

            byte[] data = Encoding.UTF8.GetBytes(response);
            resp.ContentType = "text/plain";
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }
    }
}

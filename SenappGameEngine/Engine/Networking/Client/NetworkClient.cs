using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Senapp.Engine.Networking.Json;

namespace Senapp.Engine.Networking.Client
{
    public class NetworkClient
    {
        public static string ClientId { get; private set; }
        public static bool ClientRunning { get; private set; }
        public static bool Optimize { get; private set; } = false;

        public static async void Start(bool optimize = false)
        {
            Optimize = optimize;
            try
            {
                Console.WriteLine($"[CLIENT] Starting...");

                client = new HttpClient();
                ClientId = Guid.NewGuid().ToString();

                var result = await Post(NetworkPacket.CreateRequest("connect", ClientId));
                ClientRunning = true;

                Console.WriteLine($"[CLIENT] Client connected with id {ClientId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[CLIENT][ERROR] {e.Message}");
            }
        }

        public static async Task<NetworkPacket> Post(NetworkPacket packet)
        {
            try
            {
                if (!Optimize) Console.WriteLine($"[CLIENT] Sending POST Request to {packet.Type}...");
                var response = await client.PostAsync(Server.NetworkServer.Url + packet.Type, new StringContent(packet.Data));
                var content = await response.Content.ReadAsStringAsync();

                var networkPacket = JsonConvert.DeserializeObject<NetworkPacket>(content);
                if (response.IsSuccessStatusCode)
                {
                    if (!Optimize) Console.WriteLine($"[CLIENT] '{networkPacket.Type}' Received {Encoding.UTF8.GetByteCount(content)} bytes with message: {networkPacket.Message}");
                }
                else
                {
                    if (!Optimize) Console.WriteLine($"[CLIENT] '{networkPacket.Type}' failed with message: {networkPacket.Message}");
                }

                return networkPacket;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[CLIENT][ERROR] {e.Message}");
                return NetworkPacket.Invalid;
            }
        }
        public static async Task<NetworkPacket> Get(string getName)
        {
            try
            {
                if (!Optimize) Console.WriteLine($"[CLIENT] Sending GET Request to {getName}...");
                var response = await client.GetAsync(Server.NetworkServer.Url + getName);
                var content = await response.Content.ReadAsStringAsync();

                var networkPacket = JsonConvert.DeserializeObject<NetworkPacket>(content);
                if (response.IsSuccessStatusCode)
                {
                    if (!Optimize) Console.WriteLine($"[CLIENT] '{networkPacket.Type}' Received {Encoding.UTF8.GetByteCount(content)} bytes with message: {networkPacket.Message}");
                }
                else
                {
                    if (!Optimize) Console.WriteLine($"[CLIENT] '{networkPacket.Type}' failed with message: {networkPacket.Message}");
                }

                return networkPacket;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[CLIENT][ERROR] {e.Message}");
                return NetworkPacket.Invalid;
            }
        }

        public static bool Subscribe(string getName, Action<NetworkPacket> action)
        {
            subscribtions.TryGetValue(getName, out List<Action<NetworkPacket>> batch);
            if (batch != null)
            {
                if (batch.Contains(action))
                {
                    Console.WriteLine($"[CLIENT] Action already subscribed to {getName}");
                    return false;
                }
                else
                {
                    batch.Add(action);
                    return true;
                }
            }
            else
            {
                List<Action<NetworkPacket>> newBatch = new();
                newBatch.Add(action);
                subscribtions[getName] = newBatch;
                return true;
            }
        }
        public static async Task<bool> Update()
        {
            if (updating || !ClientRunning) return false;

            updating = true;
            foreach (var key in subscribtions.Keys)
            {
                var subs = subscribtions[key];
                var result = await Get(key);

                foreach (var sub in subs)
                {
                    sub.Invoke(result);
                }
            }
            updating = false;
            return true;
        }

        private static HttpClient client;
        private static readonly Dictionary<string, List<Action<NetworkPacket>>> subscribtions = new();
        private static bool updating = false;
    }
}

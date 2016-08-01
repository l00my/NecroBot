using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Attempting to connect to NecroBot WebSocket...");
                new Program().Connect().Wait();
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private async Task Connect()
        {
            var cts = new CancellationTokenSource();
            var socket = new ClientWebSocket();
            string wsUri = "ws://localhost:14251";

            await socket.ConnectAsync(new Uri(wsUri), cts.Token);
            Console.WriteLine(socket.State);

            await Task.Factory.StartNew(async () =>
            {

                var rcvBytes = new byte[128];
                var rcvBuffer = new ArraySegment<byte>(rcvBytes);

                while (true)
                {
                    WebSocketReceiveResult rcvResult = await socket.ReceiveAsync(rcvBuffer, cts.Token);
                    byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                    string rcvMsg = Encoding.UTF8.GetString(msgBytes);
                    Console.WriteLine(rcvMsg);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}

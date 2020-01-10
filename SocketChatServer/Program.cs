using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketChatServer
{
    class Program
    {
        static int port = 3231;
        static void Main(string[] args)
        {
            StartChatServerAsync();

            string s = Console.ReadLine();
        }

        private static async void StartChatServerAsync()
        {
            await Task.Run(() =>
            {
                List<Message> Messages = new List<Message>();
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    listenSocket.Bind(ipPoint);
                    listenSocket.Listen(10);
                    Console.WriteLine("Сервер запущен. Ожидание подключений...");
                    while (true)
                    {
                        Socket handler = listenSocket.Accept();
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        byte[] data = new byte[256];
                        do
                        {
                            bytes = handler.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (handler.Available > 0);
                        var message = JsonConvert.DeserializeObject<Message>(builder.ToString());

                        if (message.Text != string.Empty)
                        {
                            Messages.Add(message);
                            using (var context = new ChatContext())
                            {
                                context.Messages.Add(message);
                                context.SaveChanges(); 
                            }
                        }
                        var answerData = JsonConvert.SerializeObject(Messages);
                        data = Encoding.Unicode.GetBytes(answerData);
                        handler.Send(data);

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            
        }
    }
}
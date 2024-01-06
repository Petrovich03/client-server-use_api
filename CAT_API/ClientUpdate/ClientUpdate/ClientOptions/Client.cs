using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Connect
{
    public class Client
    {
        MemoryStream memoryStream = new MemoryStream();
        public event EventHandler<string> MessageReceived;
        public event EventHandler<MemoryStream> Image;
        public static TcpClient client;
        public NetworkStream stream;
        public string User;
        public string Password;

        public Client(string _user, string _password)
        {
            User = _user;
            Password = _password;
        }

        public void start()
        {
            client = new TcpClient("127.0.0.1", 12345);
            stream = client.GetStream();
            messege_in();
            Thread thread = new Thread(Listen_message);
            thread.Start();

        }

        void give_mess()
        {
            try
            {
                Image?.Invoke(this, memoryStream);
            }
            catch 
            {
                Console.WriteLine("Потеряно соединение с сервером.");
            }
        }

        public void Listen_message()
        {
            while (true)
            {
                try
                {
                    memoryStream = new MemoryStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                        if (stream.DataAvailable == false) // Если поток закончился, выходим из цикла чтения
                        {
                            break;
                        }
                    }

                    byte[] data = memoryStream.ToArray();
                    string message = Encoding.UTF8.GetString(data);
                    message = message.Replace("\0", "");
                    if (message[0] == 'm' && message[1] == 'e' && message[2] == 's' && message[3] == 's')
                    {
                        message = message.Substring(4);
                        Console.WriteLine("Сообщение от сервера: {0}", message);
                        MessageReceived?.Invoke(this, message);//
                    }
                    else
                    {
                        give_mess();
                    }
                }
                catch { break; }
            } 
        }

        public void messege_in()
        {
            string info = User + ":" + Password;
            byte[] datawrite = Encoding.UTF8.GetBytes(info);
            stream.Write(datawrite, 0, datawrite.Length);
        }

        public void message_text(string mess)
        {
            byte[] Text_message = Encoding.UTF8.GetBytes(mess);
            stream.Write(Text_message, 0, Text_message.Length);
        }

        public void Close()
        {
            stream.Close();
            client.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OptionsServer;

namespace Options
{

    public class StartServer
    {
        private List<Clients> users;
        private int Port { get; set; }
        private string Ip { get; set; }

        public StartServer(string ip, int port)
        {
            Port = port;
            Ip = ip;
            users = new List<Clients>();
        }

        public void Start()
        {
            IPAddress ip = IPAddress.Parse(Ip);
            TcpListener tcpListener = new TcpListener(ip, Port);
            tcpListener.Start();
            Console.WriteLine("Сервер запущен на IP: " + ip);

            LoadUsersFromJson();

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("Установлено соединение с клиентом: " + tcpClient.Client.RemoteEndPoint);
                Thread thread = new Thread(HandleStart);
                thread.Start();

                void HandleStart()
                {
                    HandleClient(tcpClient);
                }
            }
        }

        private void LoadUsersFromJson()
        {
            string fileName = "users.json";
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                users = JsonConvert.DeserializeObject<List<Clients>>(json);
                Console.WriteLine("Данные пользователей загружены из файла.");
            }
            else
            {
                Console.WriteLine("Файл данных пользователей не найден. Создан новый список пользователей.");
            }
        }

        private void HandleClient(TcpClient tcpClient)
        {
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                
                

                string[] arr = ReadMessage(stream).Split(':');
                string username = arr[0];
                string password = arr[1];
                password = password.Replace("\0", "");
                Console.WriteLine($"Подключился {username}, пароль {password}");

                Clients user = null;

                foreach (var i in users)
                {
                    if (i.Username == username && i.Password == password)
                    {
                        user = i;
                        break;
                    }
                }

                if (user == null)
                {
                    user = new Clients(username, password);
                    users.Add(user);
                    Console.WriteLine("Новый пользователь создан.");
                }
                else
                {
                    Console.WriteLine("Сообщения пользователя:");
                    foreach (string message in user.Messages)
                    {
                        SendMessage(stream, "mess" + message);
                        Console.WriteLine(message);
                        Thread.Sleep(30);
                    }
                }

                while (true)
                {
                    string message = ReadMessage(stream); // Категория
                    
                    if (string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Клиент отключился.");
                        break;
                    }
                    Console.WriteLine("Полученное сообщение: " + message);
                    string message_text = check(message);
                    //Console.WriteLine(message_text); 

                    if (message_text == "Нет категории")
                    {
                        SendMessage(stream, "messНекорректный запрос");
                    }

                    CatCategory cat = new CatCategory();

                    string category = cat.Category(message_text);
                    
                    CatImage catImage = new CatImage();
                    catImage.give_image(category, stream);

                    user.AddMessage(message);

                    // Отправка картинки
                    
                }

                tcpClient.Close();
                stream.Close();
                SaveUsersToJson();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        private string check(string message)
        {
            string[] cat = { "cat", "кот" };
            string[] word = message.Split(' ');


            foreach (string i in word)
            {
                if (i.Contains(cat[0]) || i.Contains(cat[1]))
                {
                    return message;
                }
            }
            return "Нет категории";
        }

        private string ReadMessage(NetworkStream stream)
        {
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            return Encoding.UTF8.GetString(data, 0, bytesRead);
        }

        private void SendMessage(NetworkStream stream, string message) // Для вывода сообщений пользователя.
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void SaveUsersToJson()
        {
            string jsonToSave = JsonConvert.SerializeObject(users);
            File.WriteAllText("users.json", jsonToSave);
        }
    }
}

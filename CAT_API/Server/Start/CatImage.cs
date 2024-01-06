using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OptionsServer
{
    public class CatImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }


        public async void give_image(string category, NetworkStream stream)
        {
            try
            {
                //string apiKey = ""; // ключ API

                HttpClient client = new HttpClient();
                
                string apiUrl = $"https://api.thecatapi.com/v1/images/search?category_ids={category}";
                //client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    CatImage[] catImages = JsonConvert.DeserializeObject<CatImage[]>(responseContent);

                    if (catImages.Length > 0)
                    {
                        string imageUrl = ""; // URL картинки
                        foreach (CatImage catImage in catImages)
                        {
                            Console.WriteLine("ID: " + catImage.Id);
                            Console.WriteLine("URL: " + catImage.Url);
                            Console.WriteLine("Категория: " + category);
                            Console.WriteLine();
                            imageUrl = catImage.Url;
                        }

                        try
                        {
                            HttpClient client2 = new HttpClient();
                                
                            byte[] imageBytes = await client2.GetByteArrayAsync(imageUrl);

                            stream.Write(imageBytes, 0, imageBytes.Length);
                            Console.WriteLine("Отправка картинки.");     
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Произошла ошибка: " + ex.Message);
                        }
                    }
                    else
                    {
                        byte[] data = Encoding.UTF8.GetBytes("mess" + "Повторите запрос.");
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine("Картинки с котиками не найдены.");
                    }
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes("mess" + "Повторите запрос.");
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Ошибка при получении картинки с котиком.");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }
    }
}
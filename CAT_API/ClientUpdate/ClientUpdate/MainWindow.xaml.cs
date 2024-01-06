using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net.Http;
using Connect;
using Microsoft.Win32;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Client client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void OnMessageReceived(object sender, string message)
        {
            // Run this on the UI thread
            Dispatcher.Invoke(() =>
            {
                tbText.Text += message + "\n";
            });
        }

        byte[] download;

        private void OnImageReceived(object sender, MemoryStream memoryStream)
        {
            // Run this on the UI thread
            try
            {
                Dispatcher.Invoke(() =>
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(memoryStream.ToArray());
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    download = memoryStream.ToArray();

                    image.Source = bitmap;
                });
            }
            catch { }
            
        }


        private void tbConnect_Click(object sender, RoutedEventArgs e)
        {

           
            if (tbConnect.Content == "Отключиться")
            {

                tbText.Text = "";
                tbConnect.Content = "Подключиться";
                tbText.Text += "Вы успешно отключились.\n";
                tbEnter.IsEnabled = false;
                tbLogin.IsEnabled = true;
                tbPassword.IsEnabled = true;
                tbVisible.Visibility = Visibility.Visible;
                image.Source = null;
                client.Close();
            }
            else
            {

                if ((tbLogin.Text == "") || (tbPassword.Text == ""))
                {
                    tbText.Text += "Проверьте логин или пароль!\n";
                    return;
                }

                client = new Client(tbLogin.Text, tbPassword.Text);
                tbConnect.Content = "Отключиться";
                tbEnter.IsEnabled = true;
                tbLogin.IsEnabled = false;
                tbPassword.IsEnabled = false;
                
                tbText.Text += "Успешное подключение!\n";
                client.MessageReceived += OnMessageReceived;
                client.Image += OnImageReceived;
                client.start();
            }

            //string imagePath = @"d:\download2.jpg";
            //try
            //{
            //    BitmapImage bitmap = new BitmapImage();
            //    bitmap.BeginInit();
            //    bitmap.UriSource = new Uri(imagePath);
            //    bitmap.EndInit();

            //    image.Source = bitmap;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
            //}          
        }

        private void tbEnter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                client.message_text(tbEnter.Text);
                tbText.Text += tbEnter.Text + "\n";
                tbEnter.Text = string.Empty;
                tbVisible.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (download == null)
            {
                return;
            }
            byte[] imageBytes = download; // Получаем байты картинки из сервера
            SaveFileDialog dialog = new SaveFileDialog(); // Создаем диалоговое окно сохранения файла
            dialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"; // Устанавливаем фильтр для выбора типа файла
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); // Устанавливаем начальную директорию для сохранения
            if (dialog.ShowDialog() == true) // Открываем диалоговое окно сохранения файла
            {
                string fileName = dialog.FileName; // Получаем путь и имя файла
                using (FileStream stream = new FileStream(fileName, FileMode.Create)) // Создаем поток для записи в файл
                {
                    stream.Write(imageBytes, 0, imageBytes.Length); // Записываем байты картинки в файл
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (client == null)
            {

            }
            else
            {
                client.Close();
            }
            
        }
    }
}

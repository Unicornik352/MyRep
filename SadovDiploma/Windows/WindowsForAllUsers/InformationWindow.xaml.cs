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
using System.Windows.Shapes;
using System.IO;

namespace SadovDiploma.Windows.WindowsForAllUsers
{
    /// <summary>
    /// Логика взаимодействия для InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        public InformationWindow(string textMessage, MessageBoxImage messageBoxImage) // заполнение окна информацией
        {
            InitializeComponent();
            TbMessage.Text = textMessage;
            switch (messageBoxImage)
            {
                case MessageBoxImage.Information:
                    {
                        Title = "Информация";
                        Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Информация.png"));
                        MyImage.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Информация.png"));
                        ButtonOK.Visibility = Visibility.Visible;
                        break;
                    }
                case MessageBoxImage.Error:
                    {
                        Title = "Ошибка";
                        Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Ошибка.png"));
                        MyImage.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Ошибка.png"));
                        ButtonOK.Visibility = Visibility.Visible;
                        break;
                    }
                case MessageBoxImage.Question:
                    {
                        Title = "Вопрос";
                        Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Вопрос.png"));
                        MyImage.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Вопрос.png"));
                        ButtonYes.Visibility = Visibility.Visible;
                        ButtonNo.Visibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        Title = "Успех";
                        Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Галочка.png"));
                        MyImage.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\InfoImages\\" + "Галочка.png"));
                        ButtonOK.Visibility = Visibility.Visible;
                        break;
                    }
            };
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}

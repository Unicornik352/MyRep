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
using SadovDiploma.Entities;
using SadovDiploma.Windows.WindowsForAllUsers;

namespace SadovDiploma.Windows.DepartmentWindows
{
    /// <summary>
    /// Логика взаимодействия для SelectQuantityForPrintWindow.xaml
    /// </summary>
    public partial class SelectQuantityForPrintWindow : Window
    {
        Products ProductForPrint;
        public SelectQuantityForPrintWindow(Products product)
        {
            InitializeComponent();
            ProductForPrint = product;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonGOPrint_Click(object sender, RoutedEventArgs e) // выбор количества наклеек и отправка на страницу проверки
        {
            try
            {
                if (!int.TryParse(TbQuantity.Text, out var quantity) || quantity < 1)
                {
                    new InformationWindow($"Количество наклеек должно быть числом больше 0.", MessageBoxImage.Information).ShowDialog();
                    return;
                }
                new PrintMarkingWindow(ProductForPrint, Convert.ToInt32(TbQuantity.Text)).Show();
                this.Close();
            }
            catch
            {
                new InformationWindow($"Произошла ошибка при инициализации печати.", MessageBoxImage.Error).ShowDialog();
            }
        }
    }
}

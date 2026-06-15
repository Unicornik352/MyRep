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

namespace SadovDiploma.Windows.WindowsForAllUsers
{
    /// <summary>
    /// Логика взаимодействия для FullScreenImageProductWindow.xaml
    /// </summary>
    public partial class FullScreenImageProductWindow : Window
    {
        public FullScreenImageProductWindow(Products product, int hypId) // вывод информации о продукте
        {
            InitializeComponent();
            TbNameProduct.Text = product.name;
            ImageProduct.Source = new BitmapImage(new Uri(product.imagePath));
            TbRunCategory.Text = product.Categories.name;
            TbRunColor.Text = product.color;
            TbRunDepartment.Text = product.Categories.Departments.name;
            TbRunLk.Text = product.id.ToString();
            TbRunSize.Text = product.size;
            TbRunWeight.Text = product.weight.ToString();
            TbRunPurchasePrice.Text = product.purchasePrice.ToString();
            TbRunSellPrice.Text = product.salePrice.ToString();
            TbQuantity.Text = App.DataBase.RemaindProducts
                .Where(x => x.Products.id == product.id && x.hypermarketId == hypId)
                .Select(x => x.quantity)
                .FirstOrDefault()
                .ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

namespace SadovDiploma.Windows.CashierWindows
{
    /// <summary>
    /// Логика взаимодействия для ListProductsWindow.xaml
    /// </summary>
    public partial class ListProductsWindow : Window
    {
        int HypId;
        public ListProductsWindow(int hypId)
        {
            try
            {
                
                InitializeComponent();
                ListProducts = App.DataBase.Products
                    .ToList();
                LoadData();
                FillCheckBoxes();
                CbFilterCategory.SelectedIndex = 0;
                CbFilterDepartment.SelectedIndex = 0;
                HypId = hypId;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка в блоке конструктора. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
        List<Products> ListProducts;
        private void FillCheckBoxes() //заполнение полей выбора
        {
            List<string> listCategories = App.DataBase.Categories
                .Select(x => x.name)
                .ToList();
            List<string> listDepartments = App.DataBase.Departments
                .Select(x => x.name)
                .ToList();
            CbFilterDepartment.Items.Add("Все");
            CbFilterCategory.Items.Add("Все");
            foreach (string category in listCategories)
            {
                CbFilterCategory.Items.Add(category);
            }
            foreach (string department in listDepartments)
            {
                CbFilterDepartment.Items.Add(department);
            }
        }
        private void LoadData()
        {
            try
            {
                ListViewProducts.ItemsSource = ListProducts;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка вывода данных. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        public void StartSearchData() // выборка данных
        {
            try
            {
                ListProducts = App.DataBase.Products.ToList();
                ListProducts = ListProducts
                    .Where(x => x.name.ToLower().Contains(SearchNameProduct.Text.ToLower()))
                    .ToList();
                ListProducts = ListProducts
                    .Where(x => x.id.ToString().Contains(SearchLK.Text))
                    .ToList();
                ListProducts = ListProducts
                    .Where(x => x.description.ToLower().Contains(SearchDescription.Text.ToLower()))
                    .ToList();

                if (CbFilterDepartment.SelectedIndex > 0)
                {
                    ListProducts = ListProducts
                        .Where(x => x.Categories.Departments.name == CbFilterDepartment.SelectedItem.ToString())
                        .ToList();
                }
                if (CbFilterCategory.SelectedIndex > 0)
                {
                    ListProducts = ListProducts
                        .Where(x => x.Categories.name == CbFilterCategory.SelectedItem.ToString())
                        .ToList();
                }    
                LoadData();
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборке данных. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.White;

        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d4d4fa"));
        }
        public Products SelectedProduct;

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchNameProduct.Text = null;
            SearchLK.Text = null;
            SearchDescription.Text = null;
            CbFilterCategory.SelectedIndex = 0;
            CbFilterDepartment.SelectedIndex = 0;
        }

        private void CbFilterDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void CbFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchLK_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListViewProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is Products clickedProduct)
                    {
                        SelectedProduct = clickedProduct;
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборе товара. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is Products clickedProduct)
                    {
                        new FullScreenImageProductWindow(clickedProduct, HypId).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при открытии изображения. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace SadovDiploma.Windows.CashierWindows
{
    /// <summary>
    /// Логика взаимодействия для CashierAddApplicationWindow.xaml
    /// </summary>
    public partial class CashierAddApplicationWindow : Window
    {
        Employees ThisEmployee;
        public CashierAddApplicationWindow(Employees employee)
        {
            try
            {
                InitializeComponent();
                ThisEmployee = employee;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка в блоке конструктора. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }



        private void ButtonCreate_Click(object sender, RoutedEventArgs e) // Создание заявки
        {
            try
            {
                if (TbProduct.Text == "" || TbDepartment.Text == "" || TbDescription.Text == "")
                {
                    new InformationWindow($"Все поля должны быть заполнены. Попробуйте ещё раз.", MessageBoxImage.Information).ShowDialog();
                    return;
                }

                if (CheckBoxDataMatrix.IsChecked == true)
                {
                    if (!int.TryParse(TbQuantityProducts.Text, out int quantity) || quantity < 1)
                    {
                        new InformationWindow($"Количество товара должно быть целым числом больше 0. Попробуйте ещё раз.", MessageBoxImage.Information).ShowDialog();
                        return;
                    }
                    int idStorekeeper = App.DataBase.Employees
                            .Where(x => x.roleId == 2 && x.hypermarketId == ThisEmployee.hypermarketId)
                            .Select(x => x.id)
                            .FirstOrDefault();
                    var statusObj = App.DataBase.StatusesRequestsWriteOff.FirstOrDefault(s => s.id == 1);
                    RequestsWriteOff newRequest = new RequestsWriteOff
                    {
                        dateCreation = DateTime.Now,
                        productId = SelectedProduct.id,
                        creatorId = ThisEmployee.id,
                        statusId = 1,
                        desciption = TbDescription.Text,
                        quantity = Convert.ToInt32(TbQuantityProducts.Text),
                        storekeeperId = idStorekeeper,
                        StatusesRequestsWriteOff = statusObj
                    };
                    App.DataBase.RequestsWriteOff.Add(newRequest);
                }
                else
                {

                    RequestsMarking newRequest = new RequestsMarking
                    {
                        dateCreation = DateTime.Now,
                        productId = SelectedProduct.id,
                        creatorId = ThisEmployee.id,
                        statusId = 1,
                        description = TbDescription.Text,
                    };
                    App.DataBase.RequestsMarking.Add(newRequest);
                }
                App.DataBase.SaveChanges();
                new InformationWindow($"Успешное создание заявки.", MessageBoxImage.None).ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка создания заявки. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        Products SelectedProduct;
        private void TbProduct_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListProductsWindow listProductsWindow = new ListProductsWindow(Convert.ToInt32(ThisEmployee.hypermarketId));
                listProductsWindow.ShowDialog();
                if (listProductsWindow.DialogResult == true)
                {
                    SelectedProduct = listProductsWindow.SelectedProduct;
                    TbProduct.Text = SelectedProduct.name;
                    TbDepartment.Text = SelectedProduct.Categories.Departments.name;
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка выбора продукта. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void TbProduct_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TbProduct_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void CheckBoxDataMatrix_Checked(object sender, RoutedEventArgs e)
        {
            StackPanelDataMatrix.Visibility = Visibility.Visible;
            TbQuantityProducts.Text = "1";
        }

        private void CheckBoxDataMatrix_Unchecked(object sender, RoutedEventArgs e)
        {
            StackPanelDataMatrix.Visibility = Visibility.Collapsed;
        }
    }
}

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

namespace SadovDiploma.Windows.StorekeeperWindows
{
    /// <summary>
    /// Логика взаимодействия для StorekeeperEditRequestWriteOffWindow.xaml
    /// </summary>
    public partial class StorekeeperEditRequestWriteOffWindow : Window
    {
        public StorekeeperEditRequestWriteOffWindow(RequestsWriteOff requestsWriteOff)
        {
            InitializeComponent();
            RequestsWriteOffForEdit = requestsWriteOff;
            LoadData();
        }
        RequestsWriteOff RequestsWriteOffForEdit;


        public void LoadData() // загрузка данных
        {
            try
            {
                TbStorekeeper.Text = RequestsWriteOffForEdit.Employees1.FullName;
                TbCreator.Text = RequestsWriteOffForEdit.Employees.FullName;
                TbDescription.Text = RequestsWriteOffForEdit.desciption;
                TbQuantity.Text = RequestsWriteOffForEdit.quantity.ToString();
                TbLK.Text = RequestsWriteOffForEdit.Products.id.ToString();
                TbNameProduct.Text = RequestsWriteOffForEdit.Products.name;
                TbNumberReq.Text = RequestsWriteOffForEdit.id.ToString();


                ImageProduct.Source = new BitmapImage(new Uri(RequestsWriteOffForEdit.Products.imagePath));

                switch (RequestsWriteOffForEdit.StatusesRequestsWriteOff.name)
                {
                    case "Создана":
                        {
                            TbStatusReq.Foreground = Brushes.Green;
                            ButtonCloseReq.Content = "Начать проверку товара";
                            TbStatusReq.Text = "Заявка создана";
                            break;
                        }
                    case "Проверка товара":
                        {
                            ButtonCloseReq.Content = "Подтвердить списание";
                            TbStatusReq.Text = "Товар на проверке кладовщиком";
                            TbStatusReq.Foreground = Brushes.Blue;
                            break;
                        }
                    case "Товар на складе брака":
                        {
                            TbStatusReq.Text = "Заявка закрыта. Товар списан.";
                            TbStatusReq.Foreground = Brushes.IndianRed;
                            TbQuantity.IsReadOnly = true;
                            ButtonCloseReq.Visibility = Visibility.Collapsed;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Произошла ошибка при загрузке полей.", MessageBoxImage.Error).ShowDialog();
            }
        }


        private void ImageProduct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                new FullScreenImageProductWindow(RequestsWriteOffForEdit.Products, Convert.ToInt32(RequestsWriteOffForEdit.Employees.hypermarketId)).ShowDialog();
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при открытии изображения. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void ButtonCloseReq_Click(object sender, RoutedEventArgs e) // смена статуса заявки и её завершение
        {
            try
            {
                switch (ButtonCloseReq.Content)
                {
                    case "Начать проверку товара":
                        {
                            RequestsWriteOffForEdit.statusId = 2;
                            App.DataBase.SaveChanges();

                            TbStatusReq.Text = "Проверка товара";
                            TbStatusReq.Foreground = Brushes.Blue;
                            ButtonCloseReq.Content = "Подтвердить списание";
                            break;
                        }
                    case "Подтвердить списание":
                        {
                            if (TbQuantity.Text == "" || !int.TryParse(TbQuantity.Text, out int quantity) || quantity < 1)
                            {
                                new InformationWindow($"Количество товара должно быть числом не меньше 1.", MessageBoxImage.Information).ShowDialog();
                                return;
                            }
                            RequestsWriteOffForEdit.dateClose = DateTime.Now;
                            RequestsWriteOffForEdit.statusId = 3;
                            RequestsWriteOffForEdit.quantity = Convert.ToInt32(TbQuantity.Text);
                            var RemaindProduct = App.DataBase.RemaindProducts
                                .FirstOrDefault(x => x.productId == RequestsWriteOffForEdit.productId
                            && RequestsWriteOffForEdit.Employees.hypermarketId == x.hypermarketId);
                            RemaindProduct.quantity = RemaindProduct.quantity - quantity;

                            InformationWindow information = new InformationWindow($"Вы завершили все работы по текущей заявке?\nСумма списания составит {RequestsWriteOffForEdit.Products.purchasePrice * quantity} руб.", MessageBoxImage.Question);
                            information.ShowDialog();
                            if (information.DialogResult == true)
                            {
                                App.DataBase.SaveChanges();
                                new InformationWindow($"Успешное закрытие заявки. Сумма списания составила {RequestsWriteOffForEdit.Products.purchasePrice * quantity} руб."
                                    , MessageBoxImage.None).ShowDialog();
                                this.Close();
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Произошла ошибка в блоке Изменения статуса/Закрытия заявки.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

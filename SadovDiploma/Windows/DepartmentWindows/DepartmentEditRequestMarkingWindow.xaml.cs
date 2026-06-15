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
    /// Логика взаимодействия для DepartmentEditRequestMarkingWindow.xaml
    /// </summary>
    public partial class DepartmentEditRequestMarkingWindow : Window
    {
        RequestsMarking RequestsMarkingForEdit;
        public DepartmentEditRequestMarkingWindow(RequestsMarking requestsMarking)
        {
            InitializeComponent();
            RequestsMarkingForEdit = requestsMarking;
            LoadData();
        }
        
        public void LoadData() // загрузка данных
        {
            try
            {
                CbReason.ItemsSource = App.DataBase.ReasonsRequests
                    .Select(x => x.name)
                    .ToList();
                TbActionsTaken.Text = RequestsMarkingForEdit.actionsTaken;
                TbCreator.Text = RequestsMarkingForEdit.Employees.FullName;
                TbDescription.Text = RequestsMarkingForEdit.description;
                TbExecutor.Text = RequestsMarkingForEdit.executor;
                TbLK.Text = RequestsMarkingForEdit.Products.id.ToString();
                TbNameProduct.Text = RequestsMarkingForEdit.Products.name;
                TbNumberReq.Text = RequestsMarkingForEdit.id.ToString();

                if (RequestsMarkingForEdit.reasonId != null)
                CbReason.SelectedItem = RequestsMarkingForEdit.ReasonsRequests.name;

                ImageProduct.Source = new BitmapImage(new Uri(RequestsMarkingForEdit.Products.imagePath));

                switch (RequestsMarkingForEdit.StatusesRequestsMarking.name)
                {
                    case "Создана":
                        {
                            TbStatusReq.Foreground = Brushes.Green;
                            ButtonCloseReq.Content = "Начать работу";
                            TbStatusReq.Text = "Заявка создана";
                            PrintMarkingButton.Visibility = Visibility.Collapsed;
                            break;
                        }
                    case "В работе":
                        {
                            ButtonCloseReq.Content = "Закрыть заявку";
                            TbStatusReq.Text = "Заявка в работе";
                            TbStatusReq.Foreground = Brushes.Blue;
                            break;
                        }
                    case "Закрыта":
                        {
                            TbStatusReq.Text = "Заявка закрыта";
                            TbStatusReq.Foreground = Brushes.IndianRed;
                            TbActionsTaken.IsReadOnly = true;
                            TbExecutor.IsReadOnly = true;
                            CbReason.IsReadOnly = true;
                            ButtonCloseReq.Visibility = Visibility.Collapsed;
                            PrintMarkingButton.Visibility = Visibility.Collapsed;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Произошла ошибка при загрузке полей.", MessageBoxImage.Error).ShowDialog();
            }
        }
        private void PrintMarkingButton_Click(object sender, RoutedEventArgs e) 
        {
            try
            {
                new SelectQuantityForPrintWindow(RequestsMarkingForEdit.Products).ShowDialog();
            }
            catch (Exception ex)
            {
                new InformationWindow($"Произошла ошибка при печати маркировки.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void ImageProduct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                new FullScreenImageProductWindow(RequestsMarkingForEdit.Products, Convert.ToInt32(RequestsMarkingForEdit.Employees.hypermarketId)).ShowDialog();
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при открытии изображения. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void ButtonCloseReq_Click(object sender, RoutedEventArgs e) // смена статуса заявки и её закрытие
        {
            try
            {
                switch (ButtonCloseReq.Content)
                {
                    case "Начать работу":
                        {
                            RequestsMarkingForEdit.statusId = 2;
                            App.DataBase.SaveChanges();

                            TbStatusReq.Text = "Заявка в работе";
                            TbStatusReq.Foreground = Brushes.BlueViolet;
                            ButtonCloseReq.Content = "Закрыть заявку";
                            PrintMarkingButton.Visibility = Visibility.Visible;
                            break;
                        }
                    case "Закрыть заявку":
                        {
                            if (CbReason.SelectedIndex == -1 || TbExecutor.Text == "" || TbActionsTaken.Text == "")
                            {
                                new InformationWindow($"Заполнены не все обязательные поля.", MessageBoxImage.Information).ShowDialog();
                                return;
                            }
                            RequestsMarkingForEdit.dateClose = DateTime.Now;
                            RequestsMarkingForEdit.statusId = 3;
                            RequestsMarkingForEdit.executor = TbExecutor.Text;
                            RequestsMarkingForEdit.actionsTaken = TbActionsTaken.Text;
                            int idReasonRequest = App.DataBase.ReasonsRequests
                                .Where(x => x.name == CbReason.SelectedItem)
                                .Select(x => x.id)
                                .FirstOrDefault();
                            RequestsMarkingForEdit.reasonId = idReasonRequest;
                            InformationWindow information = new InformationWindow($"Вы завершили все работы по текущей заявке?", MessageBoxImage.Question);
                            information.ShowDialog();
                            if (information.DialogResult == true)
                            {
                                App.DataBase.SaveChanges();
                                new InformationWindow($"Успешное закрытие заявки.", MessageBoxImage.None).ShowDialog();
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

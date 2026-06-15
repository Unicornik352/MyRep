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
using SadovDiploma.Windows.DepartmentWindows;
using SadovDiploma.Windows.WindowsForAllUsers;

namespace SadovDiploma.Windows.StorekeeperWindows
{
    /// <summary>
    /// Логика взаимодействия для StorekeeperWindow.xaml
    /// </summary>
    public partial class StorekeeperWindow : Window
    {
        Employees ThisEmployee;
        public StorekeeperWindow(Employees employee)
        {
            InitializeComponent();
            ThisEmployee = employee;
            if (ThisEmployee.roleId == 2)
            {
                TbStorekeeper.Text = "Кладовщик: " + employee.FullName;
                DeleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                TbStorekeeper.Text = "Старший кассир: " + employee.FullName;
                Title = "Старший кассир - список заявок на списание товара (просмотр)";
                ExitButton.Content = "Закрыть";
            }
            ListApplications = App.DataBase.RequestsWriteOff
                .Where(x => x.Employees.hypermarketId == ThisEmployee.hypermarketId)
                .ToList();
            LoadData();
            FillCheckBoxes();
            CbFilterStatus.SelectedIndex = 0;
            SortDate.SelectedIndex = 0;
        }
        List<RequestsWriteOff> ListApplications;
      
        private void FillCheckBoxes() // заполнение полей выбора
        {
            try
            {
                List<string> listStatuses = App.DataBase.StatusesRequestsWriteOff
                    .Select(x => x.name)
                    .ToList();
                CbFilterStatus.Items.Add("Все");
                foreach (string status in listStatuses)
                {
                    CbFilterStatus.Items.Add(status);
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка загрузки полей выбора. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void LoadData()
        {
            try
            {
                ListViewRequests.ItemsSource = ListApplications;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при загрузке данных. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        public void StartSearchData() // выборка данных
        {
            try
            {
                ListApplications = App.DataBase.RequestsWriteOff
                    .Where(x => x.Employees.hypermarketId == ThisEmployee.hypermarketId)
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.Products.name.ToLower().Contains(SearchName.Text.ToLower()))
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.Products.id.ToString().Contains(SearchLK.Text))
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.id.ToString().Contains(SearchNumberReq.Text))
                    .ToList();
                if (FilterDate.SelectedDate != null)
                { 
                    ListApplications = ListApplications
                        .Where(x => Convert.ToDateTime(x.dateCreation).Date == Convert.ToDateTime(FilterDate.SelectedDate).Date)
                        .ToList();
                }

                if (CbFilterStatus.SelectedIndex > 0)
                {
                    ListApplications = ListApplications
                        .Where(x => x.StatusesRequestsWriteOff.name == CbFilterStatus.SelectedItem.ToString())
                        .ToList();
                }

                switch (SortDate.SelectedIndex)
                {
                    case 1:
                        {
                            ListApplications = ListApplications
                                .OrderBy(x => x.dateCreation)
                                .ToList();
                            break;
                        }
                    case 0:
                        {
                            ListApplications = ListApplications
                                .OrderByDescending(x => x.dateCreation)
                                .ToList();
                            break;
                        }
                }
                LoadData();

            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборке данных. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void SearchNumberReq_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchLK_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void FilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void FilerData_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }


        private void SortDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();

        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if (ExitButton.Content != "Закрыть")
            {
                new AuthorizationWindow().Show();
            }
                this.Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchName.Text = string.Empty;
            SearchNumberReq.Text = string.Empty;
            SearchLK.Text = string.Empty;
            CbFilterStatus.SelectedIndex = 0;
            FilterDate.SelectedDate = null;
            SortDate.SelectedIndex = 0;
        }

        private void ListViewRequests_MouseDoubleClick(object sender, MouseButtonEventArgs e) // выбор заявки
        {
            try
            {
                if (ThisEmployee.roleId == 2)
                {
                    var selectedRequest = ListViewRequests.SelectedItem as RequestsWriteOff;
                    if (selectedRequest != null)
                    {
                        new StorekeeperEditRequestWriteOffWindow(selectedRequest).ShowDialog();
                        StartSearchData();
                    }
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборе заявки. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) // удаление заявки
        {
            try
            {
                var selectedItem = ListViewRequests.SelectedItem as RequestsWriteOff;
                if (selectedItem == null)
                {
                    new InformationWindow($"Заявка для удаления не выбрана. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
                    return;
                }
                if (selectedItem.StatusesRequestsWriteOff.name == "Создана")
                {
                    InformationWindow information = new InformationWindow($"Вы уверены в удалении заявки?", MessageBoxImage.Question);
                    information.ShowDialog();
                    if (information.DialogResult == true)
                    {
                        App.DataBase.RequestsWriteOff.Remove(selectedItem);
                        App.DataBase.SaveChanges();
                        new InformationWindow($@"Успешное удаление заявки.", MessageBoxImage.None).ShowDialog();
                        StartSearchData();
                    }
                }
                else
                {
                    new InformationWindow($"Удалению подлежат заявки только со статусом Создана.", MessageBoxImage.Information).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при удалении заявки. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
}
}

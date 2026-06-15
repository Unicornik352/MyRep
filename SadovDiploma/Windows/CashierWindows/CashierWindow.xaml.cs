using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using SadovDiploma.Windows.StorekeeperWindows;

namespace SadovDiploma.Windows.CashierWindows
{
    /// <summary>
    /// Логика взаимодействия для CashierWindow.xaml
    /// </summary>
    public partial class CashierWindow : Window
    {
        Employees ThisEmployee;
        public CashierWindow(Employees employee)
        {
            InitializeComponent();
            TbName.Text = $"{employee.surname} {employee.name} {employee.patronymic}";
            ThisEmployee = employee;
            ListApplications = App.DataBase.RequestsMarking
                .Where(x => x.creatorId == employee.id)
                .ToList();
            LoadData();
            FillCheckBoxes();
            CbFilterDepartment.SelectedIndex = 0;
            CbFilterStatus.SelectedIndex = 0;
            SortDate.SelectedIndex = 0;
        }

        List<RequestsMarking> ListApplications;
        private void FillCheckBoxes() // заполнение полей выбора
        {
            try
            {
                List<string> listStatuses = App.DataBase.StatusesRequestsMarking
                    .Select(x => x.name)
                    .ToList();
                List<string> listDepartments = App.DataBase.Departments
                    .Select(x => x.name)
                    .ToList();
                CbFilterDepartment.Items.Add("Все");
                CbFilterStatus.Items.Add("Все");
                foreach (string status in listStatuses)
                {
                    CbFilterStatus.Items.Add(status);
                }
                foreach (string department in listDepartments)
                {
                    CbFilterDepartment.Items.Add(department);
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
                DataGridApplications.ItemsSource = ListApplications;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при загрузке данных. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            new CashierAddApplicationWindow(ThisEmployee).ShowDialog();
            StartSearchData();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e) // удаление заявки
        {
            try
            {
                var selectedItem = DataGridApplications.SelectedItem as RequestsMarking;
                if (selectedItem == null)
                {
                    new InformationWindow($"Заявка для удаления не выбрана. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
                    return;
                }
                if (selectedItem.StatusesRequestsMarking.name == "Создана")
                {
                    InformationWindow information = new InformationWindow($"Вы уверены в удалении заявки?", MessageBoxImage.Question);
                    information.ShowDialog();
                    if (information.DialogResult == true)
                    {
                        App.DataBase.RequestsMarking.Remove(selectedItem);
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

        public void StartSearchData() // выборка данных
        {
            try
            {
                ListApplications = App.DataBase.RequestsMarking
                    .Where(x => x.creatorId == ThisEmployee.id)
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.Products.name.ToLower()
                    .Contains(SearchNameProduct.Text.ToLower()))
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.Products.id.ToString()
                    .Contains(SearchLK.Text)).ToList();
                ListApplications = ListApplications
                    .Where(x => x.id.ToString().Contains(SearchNumberApp.Text))
                    .ToList();
                if (FilterDate.SelectedDate != null)
                {
                    ListApplications = ListApplications
                        .Where(x => Convert.ToDateTime(x.dateCreation).Date == Convert.ToDateTime(FilterDate.SelectedDate).Date)
                        .ToList();
                }
                if (!string.IsNullOrWhiteSpace(SearchExecutor.Text))
                {

                    ListApplications = ListApplications
                        .Where(x => x.executor != null && x.executor.ToLower().Contains(SearchExecutor.Text.ToLower()))
                        .ToList();
                }

                if (CbFilterDepartment.SelectedIndex > 0)
                {
                    ListApplications = ListApplications
                        .Where(x => x.Products.Categories.Departments.name == CbFilterDepartment.SelectedItem.ToString())
                        .ToList();
                }
                if (CbFilterStatus.SelectedIndex > 0)
                {
                    ListApplications = ListApplications
                        .Where(x => x.StatusesRequestsMarking.name == CbFilterStatus.SelectedItem.ToString())
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

        private void SearchNameProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchLK_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchNumberApp_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SearchExecutor_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartSearchData();
        }

        private void CbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }
        private void CbFilterDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            this.Close();
        }

        private void CbFilterCreator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void FilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }

        private void SortDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSearchData();
        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            SearchNameProduct.Text = null;
            SearchLK.Text = null;
            SearchNumberApp.Text = null;
            SearchExecutor.Text = null;
            CbFilterStatus.SelectedIndex = 0;
            CbFilterDepartment.SelectedIndex = 0;
            FilterDate.SelectedDate = null;
            SortDate.SelectedIndex = -1;
            LoadData();
        }

        private void ButtonListRequestsWriteOff_Click(object sender, RoutedEventArgs e)
        {
            new StorekeeperWindow(ThisEmployee).ShowDialog();
        }
    }
}

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
using System.Xml.Linq;
using SadovDiploma.Entities;
using SadovDiploma.Windows.WindowsForAllUsers;

namespace SadovDiploma.Windows.DepartmentWindows
{
    /// <summary>
    /// Логика взаимодействия для DepartmentWindow.xaml
    /// </summary>
    public partial class DepartmentWindow : Window
    {
   
        List<RequestsMarking> ListApplications;
        DepartmentAccounts ThisDepartmentAccount;
        public DepartmentWindow(DepartmentAccounts departmentAccount) 
        {
            InitializeComponent();
            ThisDepartmentAccount = departmentAccount;
            TbDepartment.Text = "Торговый отдел: " + departmentAccount.name;
            ListApplications = App.DataBase.RequestsMarking
                .Where(x => x.Employees.hypermarketId == ThisDepartmentAccount.hypermarketId
            && x.Products.Categories.Departments.name == ThisDepartmentAccount.name)
                .ToList();
            LoadData();
            FillCheckBoxes();
            CbFilterStatus.SelectedIndex = 0;
            SortDate.SelectedIndex = 0;
        }
        private void FillCheckBoxes() //заполнение полей выбора
        {
            try
            {
                List<string> listStatuses = App.DataBase.StatusesRequestsMarking
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
                ListApplications = App.DataBase.RequestsMarking
                    .Where(x => x.Employees.hypermarketId == ThisDepartmentAccount.hypermarketId
          && x.Products.Categories.Departments.name == ThisDepartmentAccount.name)
                    .ToList();
                ListApplications = ListApplications
                    .Where(x => x.Products.name.Contains(SearchName.Text))
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
            new AuthorizationWindow().Show();
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
                var selectedRequest = ListViewRequests.SelectedItem as RequestsMarking;
                if (selectedRequest != null)
                {
                    new DepartmentEditRequestMarkingWindow(selectedRequest).ShowDialog();
                    StartSearchData();
                }

            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборе заявки. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
    }
}

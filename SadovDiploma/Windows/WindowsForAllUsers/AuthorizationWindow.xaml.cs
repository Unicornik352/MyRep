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
using SadovDiploma.Windows.CashierWindows;
using SadovDiploma.Windows.DepartmentWindows;
using SadovDiploma.Windows.StorekeeperWindows;
using static System.Net.Mime.MediaTypeNames;

namespace SadovDiploma.Windows.WindowsForAllUsers
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }
        private void AuthButton_Click(object sender, RoutedEventArgs e) // Авторизация
        {
            try
            {
                if (TbLogin.Foreground == Brushes.Gray || PbPassword.Password == "") //если поля авторизации не заполнены
                {
                    new InformationWindow("Все поля обязательны к заполнению.", MessageBoxImage.Information).ShowDialog();
                    return;
                }
                var employee = App.DataBase.Employees
                    .FirstOrDefault(c => c.login == TbLogin.Text && c.password == PbPassword.Password);
                var departmentAccount = App.DataBase.DepartmentAccounts
                    .FirstOrDefault(d => d.login == TbLogin.Text && d.password == PbPassword.Password);
                if (employee == null && departmentAccount == null) // если совпадений в обеих сущностях не найдено
                {
                    new InformationWindow("Неверный логин или пароль.", MessageBoxImage.Error).ShowDialog();
                    return;
                }
                if (departmentAccount != null) // если логин и пароль относятся к торговому отделу
                {
                    new InformationWindow($@"Добро пожаловать, {departmentAccount.name}",
                       MessageBoxImage.None).ShowDialog();
                    new DepartmentWindow(departmentAccount).Show();
                    this.Close();
                }
                if (employee != null) // если логин и пароль относятся к кассиру, кладовщику или аналитику
                {
                    new InformationWindow($@"Добро пожаловать, {employee.surname} {employee.name} {employee.patronymic}",
                        MessageBoxImage.None).ShowDialog();
                    switch (employee.Roles.name)
                    {
                        case "Старший кассир":
                            {
                                new CashierWindow(employee).Show();
                                break;
                            }
                        case "Кладовщик":
                            {
                                new StorekeeperWindow(employee).Show();
                                break;
                            }
                        case "Аналитик":
                            {
                                new AnalystWindow(employee).Show();
                                break;
                            }
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                new InformationWindow("Ошибка авторизации. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void TbLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TbLogin.Foreground == Brushes.Gray)
            {
                TbLogin.Text = "";
                TbLogin.Foreground = Brushes.Black;
            }
        }

        private void TbLogin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                TbLogin.Foreground = Brushes.Gray;
                TbLogin.Text = "Логин";
            }
        }

        private void TbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                TbPassword.Visibility = Visibility.Collapsed;
                PbPassword.Visibility = Visibility.Visible;
                PbPassword.Focus();
            }
        }

        private void PbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                TbPassword.Visibility = Visibility.Visible;
                PbPassword.Visibility = Visibility.Collapsed;
            }
        }
    }
}

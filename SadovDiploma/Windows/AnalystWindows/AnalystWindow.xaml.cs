
using System;
using System.Linq;
using System.Windows;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using SadovDiploma.Entities;
using System.Collections.Generic;
using SadovDiploma.Windows.WindowsForAllUsers;
using System.Windows.Shapes;

namespace SadovDiploma
{
    public partial class AnalystWindow : Window
    {
        public AnalystWindow(Employees employee)
        {
            InitializeComponent();
            TbName.Text = employee.FullName;
            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now;
        }

        private void ComboPeriod_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (DateSelectionPanel == null) return;
                var selectedItem = (ComboPeriod.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();
                DateSelectionPanel.Visibility = (selectedItem == "Свой период") ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка при выборе периода. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e) // генерация сводки
        {
            try
            {
                DateTime start, end;
                CalculateDates(out start, out end);
                if (start > end)
                {
                    new InformationWindow($"Стартовая дата периода не может быть позже конечной даты. Попробуйте ещё раз.", MessageBoxImage.Information).ShowDialog();
                    return;
                }

                var allWriteOffs = App.DataBase.RequestsWriteOff
                    .Where(x => x.dateCreation >= start && x.dateCreation <= end).ToList();

                var allMarkings = App.DataBase.RequestsMarking
                    .Where(x => x.dateCreation >= start && x.dateCreation <= end).ToList();

                var markingStats = allMarkings
                    .GroupBy(x => x.Products.Categories.name)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count).ToList();

                var writeOffStats = allWriteOffs
                    .GroupBy(x => x.Products.Categories.name)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count(),
                        Sum = g.Sum(x => (double)(x.quantity ?? 0) * (double)(x.Products.purchasePrice ?? 0))
                    })
                    .OrderByDescending(x => x.Sum).ToList();

                string report = $"ОТЧЕТ ЗА ПЕРИОД: {start:dd.MM.yyyy HH:mm} - {end:dd.MM.yyyy HH:mm}\n";
                report += "=======================================\n\n";

                report += "--- РАЗДЕЛ 1: МАРКИРОВКА ТОВАРОВ ---\n";
                report += $"Общее количество заявок: {allMarkings.Count}\n";
                foreach (var item in markingStats)
                    report += $"  • {item.Category}: {item.Count} шт.\n";

                report += "\n--- РАЗДЕЛ 2: СПИСАНИЕ ТОВАРОВ ---\n";
                double totalWriteOffSum = writeOffStats.Sum(x => x.Sum);
                report += $"Общее количество заявок: {allWriteOffs.Count}\n";
                report += $"Общая сумма ущерба: {totalWriteOffSum:N2} руб.\n";
                foreach (var item in writeOffStats)
                    report += $"  • {item.Category}: {item.Count} шт. на сумму {item.Sum:N2} руб.\n";

                TxtReport.Text = report;

                UpdateMarkingChart(markingStats);
                UpdateWriteOffChart(writeOffStats);
                reportGenerated = true;
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка формирования отчётности. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        bool reportGenerated = false;
        private void CalculateDates(out DateTime start, out DateTime end) // подсчёт дат для сводки
        {
            var item = (ComboPeriod.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();
            DateTime tempStart = DateTime.Now;
            DateTime tempEnd = DateTime.Now;

            switch (item)
            {
                case "Неделя": tempStart = DateTime.Now.AddDays(-7); break;
                case "Месяц": tempStart = DateTime.Now.AddMonths(-1); break;
                case "Год": tempStart = DateTime.Now.AddYears(-1); break;
                default:
                    tempStart = StartDate.SelectedDate ?? DateTime.Now;
                    tempEnd = EndDate.SelectedDate ?? DateTime.Now;
                    break;
            }

            start = tempStart.Date;
            end = tempEnd.Date.AddDays(1).AddTicks(-1);
        }

        private void UpdateMarkingChart(dynamic data)
        {
            SeriesCollection series = new SeriesCollection();
            foreach (var item in data)
                series.Add(new PieSeries
                {
                    Title = item.Category,
                    Values = new ChartValues<int> { item.Count },
                    DataLabels = true,
                    LabelPoint = p => $"{p.Y} шт."
                });
            PieChartMarking.Series = series;
        }

        private void UpdateWriteOffChart(dynamic data)
        {
            SeriesCollection series = new SeriesCollection();
            foreach (var item in data)
                series.Add(new PieSeries
                {
                    Title = item.Category,
                    Values = new ChartValues<double> { item.Sum },
                    DataLabels = true,
                    LabelPoint = p => $"{p.Y:N0} ₽"
                });
            PieChartWriteOff.Series = series;
        }

        private void BtnExportTxt_Click(object sender, RoutedEventArgs e) // отправка данных в файл
        {
            try
            {
                if (reportGenerated)
                {
                    string path = $"{Directory.GetCurrentDirectory()}\\Reports\\FullReport{DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss")}.txt";
                    File.WriteAllText(path, TxtReport.Text);
                    new InformationWindow($"Текстовый отчёт сохранён в {path}.", MessageBoxImage.None).ShowDialog();
                }
                else
                {
                    new InformationWindow($"Отчёт не сформирован. Сформируйте отчёт и попробуйте ещё раз.", MessageBoxImage.Information).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка сохранения отчёта. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private void BtnExportPng_Click(object sender, RoutedEventArgs e) // отправка диаграмм в пнг
        {
            try
            {
                if (reportGenerated)
                {
                    if (ChartContainer.ActualWidth == 0) return;

                    RenderTargetBitmap bmp = new RenderTargetBitmap((int)ChartContainer.ActualWidth, (int)ChartContainer.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(ChartContainer);

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));

                    string path = $"{Directory.GetCurrentDirectory()}\\Reports\\Charts_Analytics{DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss")}.png";
                    using (Stream stm = File.Create(path)) encoder.Save(stm);
                    new InformationWindow($"Диаграммы сохранены в {path}.", MessageBoxImage.None).ShowDialog();
                }
                else
                {
                    new InformationWindow($"Отчёт не сформирован. Сформируйте отчёт и попробуйте ещё раз.", MessageBoxImage.Information).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new InformationWindow($"Ошибка сохранения диаграмм. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }

        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            this.Close();
        }
    }
}

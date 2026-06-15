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
using Aspose.BarCode.Generation;
using Aspose.BarCode.WPF;
using System.IO;
using System.Windows.Markup;

namespace SadovDiploma.Windows.DepartmentWindows
{
    /// <summary>
    /// Логика взаимодействия для PrintMarkingWindow.xaml
    /// </summary>
    public partial class PrintMarkingWindow : Window
    {
        Products productForPrint;
        int quantityElementsToPrint;
        List<TempClassForPrint> listElementsForPrint =  new List<TempClassForPrint>();
        public PrintMarkingWindow(Products product, int quantity)
        {
            try
            {
                InitializeComponent();
                productForPrint = product;
                quantityElementsToPrint = quantity;
                BarcodeGenerator barcodeGenerator = new BarcodeGenerator(EncodeTypes.Code11, productForPrint.id.ToString());
                barcodeGenerator.Save(Directory.GetCurrentDirectory() + "\\" + "tempBarcode.png", BarCodeImageFormat.Png);
                var imageSource = LoadImageNoLock(Directory.GetCurrentDirectory() + "\\" + "tempBarcode.png");
                for (int i = 0; i < quantityElementsToPrint; i++)
                {
                    listElementsForPrint.Add(new TempClassForPrint(productForPrint.name, imageSource));
                }
                ListViewToPrint.ItemsSource = listElementsForPrint;
                File.Delete(Directory.GetCurrentDirectory() + "\\" + "tempBarcode.png");
            }
            catch (Exception ex)
            {
                new InformationWindow("Ошибка загрузки элементов окна. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }
        private class TempClassForPrint // класс для заполнения listview
        {
            public TempClassForPrint(string nameProduct, ImageSource barcodeSource)
            {
                this.nameProduct = nameProduct;
                this.barcodeSource = barcodeSource;
            }
            public string nameProduct { get; set; }
            public ImageSource barcodeSource { get; set; }
        }

        private BitmapImage LoadImageNoLock(string path)
        {
            var bitmap = new BitmapImage();
            using (var stream = System.IO.File.OpenRead(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e) // печать маркировки
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FixedDocument fixedDoc = new FixedDocument();
                    fixedDoc.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                    WrapPanel pageContent = new WrapPanel();
                    pageContent.Width = printDialog.PrintableAreaWidth;
                    pageContent.Height = printDialog.PrintableAreaHeight;

                    foreach (var item in listElementsForPrint)
                    {

                        var ticketVisual = CreateTicketVisual(item);

                        pageContent.Children.Add(ticketVisual);

                    }

                    FixedPage fp = new FixedPage();
                    fp.Width = fixedDoc.DocumentPaginator.PageSize.Width;
                    fp.Height = fixedDoc.DocumentPaginator.PageSize.Height;
                    fp.Children.Add(pageContent);

                    PageContent pc = new PageContent();
                    ((IAddChild)pc).AddChild(fp);
                    fixedDoc.Pages.Add(pc);

                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Печать штрих-кодов");
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                new InformationWindow("Ошибка отправки на принтер. Попробуйте ещё раз.", MessageBoxImage.Error).ShowDialog();
            }
        }

        private FrameworkElement CreateTicketVisual(TempClassForPrint data) //генерация визуального отображения в печати
        {
            var border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Width = 140,
                Height = 100
            };

            var stack = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };

            stack.Children.Add(new TextBlock
            {
                Text = data.nameProduct,
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap
            });

            stack.Children.Add(new Image
            {
                Source = data.barcodeSource,
                Height = 60 
            });

            border.Child = stack;

            border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            border.Arrange(new Rect(border.DesiredSize));

            return border;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}

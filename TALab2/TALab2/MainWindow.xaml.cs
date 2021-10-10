using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TALab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> ListString = new List<string>();
        static DetAutomat detAutomat;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                result.Text = "";
                ListString = new List<string>();
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    pfile.Text = dialog.FileName;
                    pfile.Width = double.NaN;
                    ReadTextFile(dialog.FileName);
                    detAutomat = new DetAutomat(ListString);
                    Table(detAutomat.GetTable());
                    DisplayRes(detAutomat.CheckDet());
                    DisplayRes("\nПопытка детерменирования\n");
                    detAutomat.NetToDet();
                    DisplayRes("Автомат детерменирован\n");
                    TableDet(detAutomat.GetTable());
                }
            //}
            //catch (Exception ex)
            //{
            //    ErrorDisplay(ex.Message);
            //}

        }
        private  void ReadTextFile(string path)
        {
            string a;
            try
            {
                a =  File.ReadAllText(path);
                string newa = "";
                for (int i = 0; i < a.Length; i++)
                {
                    if(a[i] == '\r')
                    {
                        ListString.Add(newa);
                        newa = "";
                    }
                    else if(a[i]!='\n')
                        newa += a[i];
                }
                if(newa.Length != 0)
                    ListString.Add(newa);
            }
            catch (Exception ex)
            {
                ErrorDisplay(ex.Message);
            }
        }
        public void ErrorDisplay(string message) 
        { 
            result.Text = message;
            result.Foreground = Brushes.Red;
            result.FontSize = 16;
        }
        public void DisplayRes(string message)
        {
            result.Text += message;
            result.Foreground = Brushes.DarkGreen;
            result.FontSize = 16;
        }
        public void Table(StackPanel stack)
        {
            tbl.Children.RemoveRange(0, tbl.Children.Count);
            tbl.Children.Add(stack);
        }
        public void TableDet(StackPanel stack)
        {
            dtbl.Children.RemoveRange(0, tbl.Children.Count);
            dtbl.Children.Add(stack);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                result.Text = "";
                if (ListString.Count == 0)
                    throw new Exception("Автомат не инициализирован");
                if(input.Text == "")
                    throw new Exception("Строка не задана");
                DisplayRes(detAutomat.Parsing(input.Text));
            }
            catch (Exception ex)
            {
                ErrorDisplay(ex.Message);
            }
        }

    }
}

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

namespace Shop
{
    /// <summary>
    /// Логика взаимодействия для SellProductWindow.xaml
    /// </summary>
    public partial class SellProductWindow : Window
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public string BuyerName { get; private set; } = "";
        public bool Success { get; private set; } = false;

        public SellProductWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IdTextBox.Text, out int id) &&
                int.TryParse(QuantityTextBox.Text, out int qty) && qty > 0 &&
                !string.IsNullOrWhiteSpace(BuyerTextBox.Text))
            {
                ProductId = id;
                Quantity = qty;
                BuyerName = BuyerTextBox.Text.Trim();
                Success = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля корректно");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Shop.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shop
{
    public partial class MainWindow : Window
    {
        public AppDbContext _context;
        public ObservableCollection<Product> Products { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            _context.SeedDataIfEmpty();

            LoadProducts();
        }
        private void LoadProducts()
        {
            Products = new ObservableCollection<Product>(_context.Products.ToList());
            ProductsDataGrid.ItemsSource = Products; 
        }

        private void RefreshProducts()
        {
            var sortedProducts = _context.Products
                .OrderBy(p => p.Id)
                .ToList();

            Products.Clear();
            foreach (var product in sortedProducts)
            {
                Products.Add(product);
            }
        }

        private void AddQuantity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productIdStr = Microsoft.VisualBasic.Interaction.InputBox("ID продукта:", "Добавить количество", "");
                if (!int.TryParse(productIdStr, out int productId)) return;

                var quantityStr = Microsoft.VisualBasic.Interaction.InputBox("Количество для добавления:", "Добавить количество", "0");
                if (!int.TryParse(quantityStr, out int quantityToAdd) || quantityToAdd <= 0) return;

                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    product.Quantity += quantityToAdd;
                    _context.SaveChanges();
                    RefreshProducts(); 
                    MessageBox.Show("Количество успешно обновлено.");
                }
                else
                {
                    MessageBox.Show("Продукт не найден.");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка ввода данных.");
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productIdStr = Microsoft.VisualBasic.Interaction.InputBox("ID продукта:", "Продать", "");
                if (!int.TryParse(productIdStr, out int productId)) return;

                var quantityStr = Microsoft.VisualBasic.Interaction.InputBox("Количество для продажи:", "Продать", "0");
                if (!int.TryParse(quantityStr, out int quantityToSell) || quantityToSell <= 0) return;

                var buyerName = Microsoft.VisualBasic.Interaction.InputBox("Имя покупателя:", "Продать", "");

                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null && product.Quantity >= quantityToSell)
                {
                    product.Quantity -= quantityToSell;
                    var amount = quantityToSell * product.Price;

                    _context.SalesHistory.Add(new SaleHistory
                    {
                        BuyerName = buyerName,
                        ProductId = productId,
                        Quantity = quantityToSell,
                        Amount = amount
                    });

                    _context.SaveChanges();
                    RefreshProducts(); 
                    MessageBox.Show("Продажа завершена.");
                }
                else
                {
                    MessageBox.Show("Недостаточно количества или продукт не найден.");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка ввода данных.");
            }
        }
        private void OpenWeights_Click(object sender, RoutedEventArgs e)
        {
            new WeightsWindow(_context).Show();
        }

        private void OpenCompositions_Click(object sender, RoutedEventArgs e)
        {
            new CompositionsWindow(_context).Show();
        }

        private void OpenStorage_Click(object sender, RoutedEventArgs e)
        {
            new StorageWindow(_context).Show();
        }

        private void OpenSales_Click(object sender, RoutedEventArgs e)
        {
            new SalesWindow(_context).Show();
        }
    }
}
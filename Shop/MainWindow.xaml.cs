using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Shop;
using Shop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace VegetableShopApp
{ 
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        public ObservableCollection<Product> Products { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            _context.SeedDataIfEmpty();

            LoadAllProducts(); 
        }

        private void LoadAllProducts()
        {
            Products = new ObservableCollection<Product>(_context.Products.ToList());
            ProductsDataGrid.ItemsSource = Products;
        }

        private void RefreshProducts(string searchText = "")
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string trimmed = searchText.Trim();
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{trimmed}%"));
            }

            var filteredProducts = query.ToList();

            Products.Clear();
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshProducts(SearchTextBox.Text);
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            RefreshProducts();
        }


        private void AddQuantity_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddQuantityWindow();
            if (window.ShowDialog() == true)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == window.ProductId);
                if (product == null)
                {
                    MessageBox.Show("Продукт с таким ID не найден.");
                    return;
                }

                product.Quantity += window.Quantity;
                _context.SaveChanges();

                RefreshProducts(SearchTextBox.Text);
                MessageBox.Show($"Количество успешно увеличено.");
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new SellProductWindow();
            if (window.ShowDialog() == true)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == window.ProductId);
                if (product == null)
                {
                    MessageBox.Show("Продукт с таким ID не найден.");
                    return;
                }

                if (product.Quantity < window.Quantity)
                {
                    MessageBox.Show($"Недостаточно товара. В наличии: {product.Quantity}");
                    return;
                }

                product.Quantity -= window.Quantity;
                decimal amount = window.Quantity * product.Price;

                _context.SalesHistory.Add(new SaleHistory
                {
                    BuyerName = window.BuyerName,
                    ProductId = window.ProductId,
                    Quantity = window.Quantity,
                    Amount = amount
                });

                _context.SaveChanges();

                RefreshProducts(SearchTextBox.Text);
                MessageBox.Show($"Продажа завершена!");
            }
        }


        private void OpenWeights_Click(object sender, RoutedEventArgs e)
        {
            var window = new Universal(_context, typeof(ProductWeight));
            window.ShowDialog();
        }

        private void OpenCompositions_Click(object sender, RoutedEventArgs e)
        {
            var window = new Universal(_context, typeof(ProductComposition));
            window.ShowDialog();
        }

        private void OpenStorage_Click(object sender, RoutedEventArgs e)
        {
            var window = new Universal(_context, typeof(ProductStorageCondition));
            window.ShowDialog();
        }

        private void OpenSales_Click(object sender, RoutedEventArgs e)
        {
            var window = new Universal(_context, typeof(SaleHistory));
            window.ShowDialog();
        }
    }
}
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
            try
            {
                string idInput = Interaction.InputBox("Введите ID продукта:", "Добавить количество", "");
                if (!int.TryParse(idInput, out int productId)) return;

                string qtyInput = Interaction.InputBox("Введите количество для добавления:", "Добавить количество", "0");
                if (!int.TryParse(qtyInput, out int quantityToAdd) || quantityToAdd <= 0)
                {
                    MessageBox.Show("Некорректное количество.");
                    return;
                }

                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    MessageBox.Show("Продукт с таким ID не найден.");
                    return;
                }

                product.Quantity += quantityToAdd;
                _context.SaveChanges();

                RefreshProducts(SearchTextBox.Text); 
                MessageBox.Show($"Количество успешно увеличено. Теперь: {product.Quantity}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string idInput = Interaction.InputBox("Введите ID продукта:", "Продать", "");
                if (!int.TryParse(idInput, out int productId)) return;

                string qtyInput = Interaction.InputBox("Введите количество для продажи:", "Продать", "0");
                if (!int.TryParse(qtyInput, out int quantityToSell) || quantityToSell <= 0)
                {
                    MessageBox.Show("Некорректное количество.");
                    return;
                }

                string buyerName = Interaction.InputBox("Введите имя покупателя:", "Продать", "");

                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    MessageBox.Show("Продукт с таким ID не найден.");
                    return;
                }

                if (product.Quantity < quantityToSell)
                {
                    MessageBox.Show($"Недостаточно товара. В наличии: {product.Quantity}");
                    return;
                }

                product.Quantity -= quantityToSell;
                decimal amount = quantityToSell * product.Price;

                _context.SalesHistory.Add(new SaleHistory
                {
                    BuyerName = buyerName,
                    ProductId = productId,
                    Quantity = quantityToSell,
                    Amount = amount
                });

                _context.SaveChanges();

                RefreshProducts(SearchTextBox.Text);
                MessageBox.Show($"Продажа завершена. Сумма: ${amount}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
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
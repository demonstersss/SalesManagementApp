using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для Universal.xaml
    /// </summary>
    public partial class Universal : Window
    {
        private readonly AppDbContext _context;
        private readonly Type _entityType;
        private readonly string _dbSetName;
        private readonly ObservableCollection<object> _items;

        public Universal(AppDbContext context, Type entityType)
        {
            InitializeComponent();

            _context = context ?? throw new ArgumentNullException(nameof(context));
            _entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            _dbSetName = entityType.Name + "s"; 

            var dbSetProperty = typeof(AppDbContext).GetProperty(_dbSetName,
                BindingFlags.Public | BindingFlags.Instance);

            if (dbSetProperty == null)
                throw new ArgumentException($"DbSet с именем '{_dbSetName}' не найден в AppDbContext.");

            var dbSet = (IQueryable)dbSetProperty.GetValue(_context);

            var entities = dbSet.Cast<object>().ToList();

            _items = new ObservableCollection<object>(entities);
            ReferenceDataGrid.ItemsSource = _items;

            ((INotifyCollectionChanged)_items).CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    var entity = Activator.CreateInstance(_entityType);
                    _context.Add(entity);
                    int index = _items.IndexOf(newItem);
                    if (index >= 0) _items[index] = entity;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    _context.Remove(oldItem);
                }
            }
        }

        private void ReferenceDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                e.Column.IsReadOnly = true;
            }

            var prop = _entityType.GetProperty(e.PropertyName);
            if (prop != null)
            {
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                                  ?? prop.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>()?.DisplayName;

                if (!string.IsNullOrEmpty(displayName))
                    e.Column.Header = displayName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changedEntries = _context.ChangeTracker.Entries()
                    .Where(en => en.State == EntityState.Added ||
                                 en.State == EntityState.Modified ||
                                 en.State == EntityState.Deleted)
                    .Count();

                if (changedEntries == 0)
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _context.SaveChanges();
                MessageBox.Show($"Изменения ({changedEntries}) успешно сохранены в базе данных.",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}\n\nВозможно, нарушена целостность данных.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var hasChanges = _context.ChangeTracker.HasChanges();
            if (hasChanges)
            {
                var result = MessageBox.Show(
                    "Есть несохранённые изменения. Выйти без сохранения?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }

        private void ReferenceDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && ReferenceDataGrid.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Удалить {ReferenceDataGrid.SelectedItems.Count} строк(и)?\nЭто действие нельзя отменить.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    e.Handled = true; 
            }
        }
    }
}

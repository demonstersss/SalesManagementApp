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
    /// Логика взаимодействия для WeightsWindow.xaml
    /// </summary>
    public partial class WeightsWindow : Window
    {
        public WeightsWindow(AppDbContext context)
        {
            InitializeComponent();
            grid.ItemsSource = context.ProductWeights.ToList();
        }
    }
}

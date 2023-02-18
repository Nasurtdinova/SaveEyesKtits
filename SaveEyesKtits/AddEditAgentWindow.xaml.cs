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
using System.Windows.Shapes;

namespace SaveEyesKtits
{
    /// <summary>
    /// Логика взаимодействия для AddEditAgentWindow.xaml
    /// </summary>
    public partial class AddEditAgentWindow : Window
    {
        public Agent CurrentAgent = new Agent();
        public AddEditAgentWindow(Agent selected)
        {
            InitializeComponent();
            if (selected != null)
            {
                CurrentAgent = selected;
                btnRemove.Visibility = Visibility.Visible;
            }
                
            comboTypeAgent.ItemsSource = Connection.Context.AgentType.ToList();
            comboProducts.ItemsSource = Connection.Context.Product.ToList();
            DataContext = CurrentAgent;
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog()== true)
            {
                logoAgent.Source = new BitmapImage(new Uri(file.FileName));
                CurrentAgent.LogoByte = File.ReadAllBytes(file.FileName); 
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentAgent.ID == 0)
                    Connection.Context.Agent.Add(CurrentAgent);
                Connection.Context.SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAgent.ProductSale.Count() == 0)
            {
                if (CurrentAgent.AgentPriorityHistory.Count != 0)
                    Connection.Context.AgentPriorityHistory.RemoveRange(CurrentAgent.AgentPriorityHistory);
                if (CurrentAgent.Shop.Count != 0)
                    Connection.Context.Shop.RemoveRange(CurrentAgent.Shop);
                Connection.Context.Agent.Remove(CurrentAgent);
                Connection.Context.SaveChanges();
                Close();
            }
            else
            {
                MessageBox.Show("Удаление запрещено!");
            }
        }

        private void comboProducts_TextChanged(object sender, TextChangedEventArgs e)
        {
            comboProducts.ItemsSource = Connection.Context.Product.Where(a=>a.Title.ToLower().Contains(comboProducts.Text.ToLower())).ToList();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (comboProducts.SelectedItem != null && !String.IsNullOrEmpty(tbCount.Text) && dpDate.SelectedDate != null)
            {
                ProductSale prod = new ProductSale()
                {
                    Agent = CurrentAgent,
                    Product = comboProducts.SelectedItem as Product,
                    SaleDate = dpDate.SelectedDate.Value,
                    ProductCount = Convert.ToInt32(tbCount.Text), 
                    AgentID = CurrentAgent.ID,
                 ProductID = (comboProducts.SelectedItem as Product).ID
                };
                CurrentAgent.ProductSale.Add(prod);
                lvProductSale.Items.Refresh();
            }
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as ProductSale;
            CurrentAgent.ProductSale.Remove(product);
            lvProductSale.Items.Refresh();
        }

        private void tbCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
    }
}

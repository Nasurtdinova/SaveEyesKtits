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

namespace SaveEyesKtits
{
    public partial class MainWindow : Window
    {
        public List<Agent> FilteredAgents = new List<Agent>();
        public int ActualPage { get; set; }
        public IEnumerable<Agent> PaginatedAgents 
        { 
            get
            {
                spPaginated.Children.Clear();
                spPaginated.Children.Add(new TextBlock { Text = "<" });

                if (FilteredAgents.Count % 10 == 0)
                    for (int i = 1; i<= FilteredAgents.Count / 10; i++)
                        spPaginated.Children.Add(new TextBlock { Text = $"{i}" });
                else
                    for (int i = 1; i <= FilteredAgents.Count / 10 + 1; i++)
                        spPaginated.Children.Add(new TextBlock { Text = $"{i}" });

                spPaginated.Children.Add(new TextBlock { Text = ">" });

                foreach(TextBlock tb in spPaginated.Children)
                    tb.PreviewMouseDown += spPaginated_PreviewMouseDown;

                if (spPaginated.Children.Count != 0 && (spPaginated.Children[ActualPage + 1] as TextBlock).Text != ">" || (spPaginated.Children[ActualPage + 1] as TextBlock).Text != "<")
                    (spPaginated.Children[ActualPage + 1] as TextBlock).TextDecorations = TextDecorations.Underline;

                return FilteredAgents.Skip(ActualPage * 10).Take(10);
            }
        }

        private void spPaginated_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch((sender as TextBlock).Text)
            {
                case "<":
                    if (ActualPage > 0) ActualPage--;
                    Filtered();
                    return;
                case ">":
                    if (ActualPage < (FilteredAgents.Count() / 10 + (Connection.Context.Agent.ToList().Count() % 10 != 0? 1 : 0)) -1) ActualPage++;
                    Filtered();
                    return;
                default:
                    ActualPage = Convert.ToInt32((sender as TextBlock).Text) - 1;
                    Filtered();
                    return;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
           
            var list = new List<string>
            {
                "По умолчанию",
                "Наименование по возрастанию",
                "Наименование по убыванию",
                "Размер скидки по возрастанию",
                "Размер скидки по убыванию",
                "Приоритет по возрастанию",
                "Приоритет по убыванию"
            };
            comboSort.ItemsSource = list;

            var type = Connection.Context.AgentType.ToList();
            type.Insert(0, new AgentType { Title = "Все типы" });
            comboType.ItemsSource = type;

            Filtered();
        }
        public void Filtered()
        {
            FilteredAgents = Connection.Context.Agent.ToList();

            if (comboType.SelectedIndex > 0)
                FilteredAgents = FilteredAgents.Where(a=>a.AgentType == comboType.SelectedItem as AgentType).ToList();

            if (comboSort.SelectedIndex == 1)
                FilteredAgents = FilteredAgents.OrderBy(a=>a.Title).ToList();
            else if (comboSort.SelectedIndex == 2)
                FilteredAgents = FilteredAgents.OrderByDescending(a => a.Title).ToList();
            else if (comboSort.SelectedIndex == 3)
                FilteredAgents = FilteredAgents.OrderBy(a => a.SizeDiscount).ToList();
            else if (comboSort.SelectedIndex == 4)
                FilteredAgents = FilteredAgents.OrderByDescending(a => a.SizeDiscount).ToList();
            else if (comboSort.SelectedIndex == 5)
                FilteredAgents = FilteredAgents.OrderBy(a => a.Priority).ToList();
            else if (comboSort.SelectedIndex == 6)
                FilteredAgents = FilteredAgents.OrderByDescending(a => a.Priority).ToList();

            FilteredAgents = FilteredAgents.Where(a => a.Title.ToLower().Contains(tbSearch.Text.ToLower()) 
            || a.Email.ToLower().Contains(tbSearch.Text.ToLower()) 
            || a.Phone.ToLower().Contains(tbSearch.Text.ToLower())).ToList();

            lvAgents.ItemsSource = PaginatedAgents;
        }

        private void comboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualPage = 0;
            Filtered();
        }

        private void comboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualPage = 0;
            Filtered();
        }

        private void btnAddAgent_Click(object sender, RoutedEventArgs e)
        {
            AddEditAgentWindow win = new AddEditAgentWindow(null);
            win.Show();
            win.Closed += (s, eventarg) =>
            {
                ActualPage = 0;
                Filtered();
            };
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualPage = 0;
            Filtered();
        }

        private void btnUpdatePriority_Click(object sender, RoutedEventArgs e)
        {
            var list = lvAgents.SelectedItems.Cast<Agent>().ToList();
            UpdatePriotiryWindow win = new UpdatePriotiryWindow(list);
            win.Show();
            win.Closed += (s, eventarg) =>
            {
                ActualPage = 0;
                Filtered();
            };
        }

        private void lvAgents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var a = lvAgents.SelectedItem as Agent;
            AddEditAgentWindow win = new AddEditAgentWindow(a);
            win.Show();
            win.Closed += (s, eventarg) =>
            {
                ActualPage = 0;
                Filtered();
            };
        }

        private void lvAgents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAgents.SelectedItem != null)
                btnUpdatePriority.Visibility = Visibility.Visible;
        }
    }
}

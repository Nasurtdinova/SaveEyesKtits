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

namespace SaveEyesKtits
{
    public partial class UpdatePriotiryWindow : Window
    {
        public List<Agent> Agents = new List<Agent>();
        public UpdatePriotiryWindow(List<Agent> selected)
        {
            InitializeComponent();
            tbValue.Text = selected.Max(a => a.Priority).ToString();
            if (selected != null)
                Agents = selected;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbValue.Text))
            {
                foreach (var agent in Agents)
                {
                    agent.Priority += Convert.ToInt32(tbValue.Text);
                    Connection.Context.SaveChanges();
                }
                Close();
            }
        }

        private void tbValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Char.IsLetter(e.Text, 0))
                e.Handled = true;
        }
    }
}

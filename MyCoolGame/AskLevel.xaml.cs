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

namespace MyCoolGame
{
    /// <summary>
    /// Логика взаимодействия для AskLevel.xaml
    /// </summary>
    public partial class AskLevel : Window
    {
        private string level;

        public AskLevel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            level = (sender as Button).Tag.ToString();
        }

        public string GetLevel()
        {
            return level;
        }
    }
}

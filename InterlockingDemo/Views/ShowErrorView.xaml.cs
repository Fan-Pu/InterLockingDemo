using InterlockingDemo.Classes;
using InterlockingDemo.ViewModel;
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

namespace InterlockingDemo.Views
{
    /// <summary>
    /// ShowErrorView.xaml 的交互逻辑
    /// </summary>
    public partial class ShowErrorView : Window
    {
        public ShowErrorView(Structure stru, List<Tuple<string, string>> switch_errors,
            List<Tuple<string, string>> section_errors)
        {
            this.DataContext = new ShowErrorViewModel(this, stru, switch_errors, section_errors);
            InitializeComponent();
        }
    }
}

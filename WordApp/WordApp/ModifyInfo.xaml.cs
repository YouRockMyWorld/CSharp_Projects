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

namespace WordApp
{
    /// <summary>
    /// ModifyInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyInfo : Window
    {
        private System.Collections.ObjectModel.ObservableCollection<ModifyInfoItem> modifyInfoItems = 
            new System.Collections.ObjectModel.ObservableCollection<ModifyInfoItem>();

        public ModifyInfo()
        {
            InitializeComponent();
            DataGrid_ModifyInfo.ItemsSource = modifyInfoItems;
        }

        public void AddItem(object sender, ModifyInfoEventArgs e)
        {
            modifyInfoItems.Add(e.ModifyInfoItem);
            TextBlock_ModifyInfo.Text = $"修改信息(已完成{modifyInfoItems.Count}个)";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace SetApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentLevel = 1;
        private Dictionary<int, List<int>> set ;
        private string result_str;
        public System.Collections.ObjectModel.ObservableCollection<SetRowData> setList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            set = new Dictionary<int, List<int>>();
            //set_DataGrid.ItemsSource = setData.AsDataView();
            setList = new System.Collections.ObjectModel.ObservableCollection<SetRowData>();
            set_DataGrid.ItemsSource = setList;
            box_level.Text = "1";
        }

        private void Click_delete(object sender, RoutedEventArgs e)
        {
            if(set_DataGrid.SelectedIndex != -1)
            {
                //setData.Rows.RemoveAt(set_DataGrid.SelectedIndex);
                SetRowData sr = (SetRowData)set_DataGrid.SelectedItem;
                set.Remove(sr.Level);
                setList.RemoveAt(set_DataGrid.SelectedIndex);  
            }
            set_DataGrid.SelectedIndex = -1;
        }

        private void Click_clear(object sender, RoutedEventArgs e)
        {
            //setData.Clear();
            setList.Clear();
            set.Clear();
            currentLevel = 1;
        }

        private void grid_keydown(object sender, KeyEventArgs e)
        {
            if(set_DataGrid.CurrentColumn.Header.ToString() == "优先级")
            {
                if(e.Key == Key.D0)
                {
                    e.Handled = true;
                }
            }
        }


        private void ban_paste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void levelbox_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 &&
                e.Key != Key.NumPad3 && e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 && 
                e.Key != Key.NumPad7 && e.Key != Key.NumPad8 && e.Key != Key.NumPad9 && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }

        private void setbox_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 && e.Key != Key.NumPad3 && 
                e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 &&e.Key != Key.NumPad7 && 
                e.Key != Key.NumPad8 && e.Key != Key.NumPad9 && e.Key != Key.Back && e.Key != Key.OemComma)
            {
                e.Handled = true;
            }
        }

        private void Click_addset(object sender, RoutedEventArgs e)
        {
            if(box_set.Text != "" && box_level.Text != "")
            {
                string s = box_set.Text;
                List<int> l = new List<int>();
                Parse_set_string(ref s, out l);
                set.Add(Int32.Parse(box_level.Text), l);
                setList.Add(new SetRowData() { Level = Int32.Parse(box_level.Text), SetData = s });
                currentLevel++;
                box_level.Text = currentLevel.ToString();
                box_set.Text = "";
            }
        }

        private void Parse_set_string(ref string str, out List<int> list)
        {
            string[] ss = str.Split(',');
            str = "";
            list = new List<int>();
            foreach(string s in ss)
            {
                if (s != "")
                {
                    int i;
                    if (int.TryParse(s, out i) && !list.Contains(i))
                    {
                            list.Add(i);
                            str = str + s + ",";
                    }
                }
            }
            str = "{" + str.TrimEnd(',') + "}";
        }

        private string listSetToString(List<int> l)
        {
            string str = "";
            foreach(int i in l)
            {
                str = str + i.ToString() + ",";
            }
            return "{" + str.TrimEnd(',') + "}";
        }

        private void compute_result()
        {
            int computer_count = 1;
            List<int> key_list = new List<int>();
            List<List<int>> value_list = new List<List<int>>();
            foreach (var a in set)
            {
                key_list.Add(a.Key);
                value_list.Add(a.Value);
            }
            for(int i = 0; i< set.Count; ++i)
            {
                for(int j = i + 1; j < set.Count; ++j)
                {
                    List<int> tempH = new List<int>();
                    List<int> tempL = new List<int>();
                    result_str += string.Format("第{0}次计算：", computer_count) + "\n";
                    result_str += string.Format("优先级：{0}   集合内容：{1}\n优先级：{2}   集合内容：{3}\n结果：\n",
                        key_list[i].ToString(), listSetToString(value_list[i]), 
                        key_list[j].ToString(), listSetToString(value_list[j]));
                    if (key_list[i] < key_list[j]) 
                    {
                        tempH = value_list[i];
                        tempL = value_list[j];
                    }
                    else if(key_list[i] == key_list[j])
                    {
                        if(value_list[i].Count >= value_list[j].Count)
                        {
                            tempH = value_list[i];
                            tempL = value_list[j];
                        }
                    }
                    else
                    {
                        tempH = value_list[j];
                        tempL = value_list[i];
                    }
                    result_str += string.Format("{0}------>{1}", listSetToString(tempH), listSetToString(tempH)) + "\n";
                    List<int> tempResult = new List<int>();
                    result_str += listSetToString(tempL) + "------>";
                    tempResult = tempH.Intersect(tempL).ToList();
                    tempL = tempL.Except(tempResult).ToList();
                    result_str += listSetToString(tempL) + "\n--------------------------------------------------\n";
                    computer_count++;

                    if(key_list[i] < key_list[j])
                    {
                        value_list[j] = tempL;
                    }
                    else if(key_list[i] == key_list[j])
                    {
                        if (value_list[i].Count >= value_list[j].Count)
                        {
                            value_list[j] = tempL;
                        }
                    }
                    else
                    {
                        value_list[i] = tempL;
                    }

                }
            }
            result_str += "**************************************************\n扣减后集合内容：\n";
            for (int i = 0; i < set.Count; ++i) 
            {
                result_str += string.Format("优先级：{0}      原始集合内容：{1}      扣减后集合内容：{2}\n", key_list[i].ToString(), listSetToString(set.Values.ToList()[i]), listSetToString(value_list[i]));
            }
        }

        private void Click_compute(object sender, RoutedEventArgs e)
        {
            compute_result();
            Result re = new Result(result_str);
            re.ShowDialog();
            result_str = "";
        }
    }

    public class SetRowData : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int level;
        private string setData;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if(value != level)
                {
                    level = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SetData
        {
            get
            {
                return setData;
            }
            set
            {
                if(value != setData)
                {
                    setData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

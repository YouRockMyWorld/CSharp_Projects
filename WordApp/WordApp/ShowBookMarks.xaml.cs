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
    /// ShowBookMarks.xaml 的交互逻辑
    /// </summary>
    public partial class ShowBookMarks : Window
    {
        public delegate void ReturnBookMarksEventHandle(object sender, ReturnBookMarksEventArgs e);
        public event ReturnBookMarksEventHandle ReturnBookMarks;
        public delegate void DeleteAllDocumentBookMarksEventHandle();
        public event DeleteAllDocumentBookMarksEventHandle DeleteAllBookMarks;
        public ShowBookMarks()
        {
            InitializeComponent();
        }

        private void Click_Return_CurrentBookMarks(object sender, RoutedEventArgs e)
        {
            if (BookMarks_DataGrid.SelectedIndex != -1)
            {
                List<string> bookMarks = new List<string>();
                foreach (CurrentBookMark item in BookMarks_DataGrid.SelectedItems)
                {
                    bookMarks.Add(item.Name);
                }
                OnRaiseReturnBookMarksEvent(new ReturnBookMarksEventArgs { BookMarks = bookMarks });
            }
        }

        private void Click_Return_AllBookMarks(object sender, RoutedEventArgs e)
        {
            List<string> bookMarks = new List<string>();
            foreach (CurrentBookMark item in BookMarks_DataGrid.Items)
            {
                bookMarks.Add(item.Name);
            }
            OnRaiseReturnBookMarksEvent(new ReturnBookMarksEventArgs { BookMarks = bookMarks });
        }

        private void OnRaiseReturnBookMarksEvent(ReturnBookMarksEventArgs e)
        {
            //ReturnBookMarksEventHandle handel = ReturnBookMarks;
            //if (handel != null)
            //{
            //    handel(this, e);
            //}
            //语法糖如下
            ReturnBookMarks?.Invoke(this, e);
        }

        private void Click_Delete_AllBookMarks(object sender, RoutedEventArgs e)
        {
            this.Close();
            OnRaiseDeleteAllBookMarksEvent();
        }

        private void OnRaiseDeleteAllBookMarksEvent()
        {
            DeleteAllBookMarks?.Invoke();
        }
    }
}

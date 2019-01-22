using System;
using System.Collections.Generic;
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
using MsWord = Microsoft.Office.Interop.Word;

namespace WordApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ModifyInfoEventHandle(object sender, ModifyInfoEventArgs e);
        public event ModifyInfoEventHandle ModifiedContent;

        private string templatePath = String.Empty;
        private MsWord.Application wordApp = null;
        private MsWord.Document wordDoc = null;
        private DataTable myDataTable = new DataTable("MyTable");
        private List<CurrentBookMark> myBookMarks = new List<CurrentBookMark>();
        private ShowBookMarks showBookMarks = null;
        private ModifyInfo modifyInfo = null;


        public MainWindow()
        {
            InitializeComponent();
            Init();
            DataGrid_Replace.ItemsSource = myDataTable.AsDataView();
        }

        private void Init()
        {
            myDataTable.Columns.Add("序号", Type.GetType("System.Int32"));
            myDataTable.Columns.Add("书签", Type.GetType("System.String"));
            myDataTable.Columns.Add("内容", Type.GetType("System.String"));
        }
        private void MouseEnterOpenArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "打开Word模板";
        }

        private void MouseEnterSaveArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "保存当前修改文档";
        }

        private void MouseLeaveArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "Ready";
        }

        private void MouseEnterExitArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "退出程序";
        }

        private void MouseEnterCloseArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "关闭当前文档";
        }

        private void MouseEnterAboutArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "About";
        }

        private void MouseEnterAddArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "添加一行记录";
        }

        private void MouseEnterDeleteArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "删除当前选中记录";
        }

        private void MouseEnterDeleteAllArea(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "删除全部";
        }

        private void MouseEnterDisplayBookmark(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "显示文档中书签定义";
        }

        private void MouseEnterLoadHistory(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "加载历史/预定义更替模板";
        }

        private void MouseEnterExportHistory(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "导出更替模板设置";
        }

        private void MouseEnter_StackPanel_TemplatePath(object sender, MouseEventArgs e)
        {
            if (templatePath == "")
            {
                status_bar_text.Text = "未加载模板，请先加载模板";
            }
            else
            {
                status_bar_text.Text = "当前已加载模板：" + templatePath;
            }
        }

        private void MouseEnter_DataGrid_Replace(object sender, MouseEventArgs e)
        {
            if (templatePath == "")
            {
                status_bar_text.Text = "未加载模板，请先加载模板";
            }
            else
            {
                status_bar_text.Text = "自定义书签替换";
            }
        }

        private void MouseEnterFuzzyMatch(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "开启模糊匹配并设置首尾匹配字符";
        }

        private void MouseEnterReplace(object sender, MouseEventArgs e)
        {
            status_bar_text.Text = "根据设置修改Word模板内容";
        }

        private void Click_File_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.DefaultExt = ".docx";
                dlg.Filter = "Word 文档(.docx)|*.docx|Word 97-2003 文档(.doc)|*.doc";

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    templatePath = dlg.FileName;
                    OpenFile(templatePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }

        }


        private void OpenFile(string path)
        {
            try
            {
                if (wordApp == null)
                {
                    wordApp = new MsWord.Application();
                }
                if (wordDoc != null)
                {
                    if (!wordDoc.Saved)
                    {
                        MessageBoxResult result = MessageBox.Show("文档未保存，确定放弃更改并打开新文档吗？", "SC-Word Processor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            wordDoc.Close(MsWord.WdSaveOptions.wdDoNotSaveChanges);
                            wordDoc = null;
                        }
                        else return;
                    }
                }
                wordDoc = wordApp.Documents.Add(path);
                TextBlock_TemplatePath.Text = path;
                toolTip.Text = path;
                Menu_File_Close.IsEnabled = true;
                Menu_File_Save.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Click_File_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.DefaultExt = ".docx";
                dlg.Filter = "Word 文档(.docx)|*.docx|Word 97-2003 文档(.doc)|*.doc";
                if (dlg.ShowDialog() == true)
                {
                    wordDoc.SaveAs2(dlg.FileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Click_File_Close(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wordDoc != null)
                {
                    if (!wordDoc.Saved)
                    {
                        MessageBoxResult result = MessageBox.Show("文档未保存，确定退出吗？", "SC-Word Processor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            wordDoc.Close(MsWord.WdSaveOptions.wdDoNotSaveChanges);
                            wordDoc = null;
                            wordApp.Quit();
                            wordApp = null;
                            TextBlock_TemplatePath.Text = "未加载模板";
                            toolTip.Text = "未加载模板，请先加载模板";
                            templatePath = "";
                            Menu_File_Close.IsEnabled = false;
                            Menu_File_Save.IsEnabled = false;
                        }
                        else return;
                    }
                    else
                    {
                        wordDoc.Close();
                        wordDoc = null;
                        wordApp.Quit();
                        wordApp = null;
                        TextBlock_TemplatePath.Text = "未加载模板";
                        toolTip.Text = "未加载模板，请先加载模板";
                        templatePath = "";
                        Menu_File_Close.IsEnabled = false;
                        Menu_File_Save.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Closing_MainWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (wordApp == null)
                {
                    return;
                }
                else if (wordDoc == null)
                {
                    wordApp.Quit();
                    wordApp = null;
                }
                else if (!wordDoc.Saved)
                {
                    MessageBoxResult result = MessageBox.Show("文档未保存，确定退出吗？", "SC-Word Processor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        wordDoc.Close(MsWord.WdSaveOptions.wdDoNotSaveChanges);
                        wordDoc = null;
                        wordApp.Quit();
                        wordApp = null;
                        e.Cancel = false;
                    }
                    if (result == MessageBoxResult.No) e.Cancel = true;
                }
                else
                {
                    wordDoc.Close();
                    wordDoc = null;
                    wordApp.Quit();
                    wordApp = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Click_File_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Click_Help_About(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private void ReCreateId()
        {
            int i = 1;
            foreach (DataRow dr in myDataTable.Rows)
            {
                dr["序号"] = i;
                i++;
            }
        }

        private void Click_Tool_Add(object sender, RoutedEventArgs e)
        {
            DataRow dr = myDataTable.NewRow();
            dr["书签"] = "";
            dr["内容"] = "";
            myDataTable.Rows.Add(dr);
            ReCreateId();
        }

        private void Click_Tool_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGrid_Replace.SelectedIndex != -1)
                {
                    List<int> sel = new List<int>();
                    foreach (var item in DataGrid_Replace.SelectedItems)
                    {
                        DataRowView drv = item as DataRowView;
                        sel.Add((int)drv["序号"]);
                    }
                    sel.Sort();
                    for (int i = sel.Count - 1; i >= 0; i--)
                    {
                        myDataTable.Rows.RemoveAt(sel[i] - 1);
                    }
                    //myDataTable.Rows.RemoveAt(DataGrid_Replace.SelectedIndex);
                }
                DataGrid_Replace.SelectedIndex = -1;
                ReCreateId();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Click_Tool_DeleteAll(object sender, RoutedEventArgs e)
        {
            myDataTable.Clear();
        }

        private void Click_Tool_DisplayBookmark(object sender, RoutedEventArgs e)
        {
            try
            {
                if (showBookMarks != null) return;
                if (templatePath != String.Empty && wordDoc != null)
                {
                    int id = 1;
                    myBookMarks.Clear();
                    foreach (MsWord.Bookmark bk in wordDoc.Bookmarks)
                    {
                        myBookMarks.Add(new CurrentBookMark { Id = id, Name = bk.Name });
                        id++;
                    }
                    showBookMarks = new ShowBookMarks();
                    showBookMarks.Owner = this;
                    showBookMarks.BookMarks_DataGrid.ItemsSource = myBookMarks;
                    showBookMarks.ReturnBookMarks += new ShowBookMarks.ReturnBookMarksEventHandle(HandleReturnBookMarksEvent);
                    showBookMarks.DeleteAllBookMarks += new ShowBookMarks.DeleteAllDocumentBookMarksEventHandle(HandleDeleteAllBookMarks);
                    showBookMarks.Closed += new EventHandler(HandleShowBookMarksWindowClosed);
                    if (myBookMarks.Count == 0)
                    {
                        showBookMarks.TextBlock_BookMarks.Text = "当前文档不存在书签";
                    }
                    showBookMarks.Show();
                }
                else
                {
                    MessageBox.Show("未加载模板，请先打开Word模板", "SC-Information");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void HandleReturnBookMarksEvent(object sender, ReturnBookMarksEventArgs e)
        {
            foreach (string s in e.BookMarks)
            {
                DataRow dr = myDataTable.NewRow();
                dr["书签"] = s;
                dr["内容"] = "";
                myDataTable.Rows.Add(dr);
                ReCreateId();
            }
        }

        private void HandleDeleteAllBookMarks()
        {
            try
            {
                foreach (MsWord.Bookmark bk in wordDoc.Bookmarks)
                {
                    bk.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void HandleShowBookMarksWindowClosed(object sender, EventArgs e)
        {
            showBookMarks = null;
        }

        private void Click_Tool_ExportHistory(object sender, RoutedEventArgs e)
        {
            try
            {
                if (myDataTable.Rows.Count > 0)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string json = jss.Serialize(DataTableToList(myDataTable));

                    Microsoft.Win32.SaveFileDialog sfg = new Microsoft.Win32.SaveFileDialog();
                    sfg.InitialDirectory = Environment.CurrentDirectory;
                    sfg.Filter = "Settings 文件(.sc)|*.sc";

                    if (sfg.ShowDialog() == true)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfg.FileName))
                        {
                            //MSDN StreamWriter
                            sw.Write(json);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void Click_Tool_LoadHistory(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Filter = "Settings 文件(.sc)|*.sc";
                string json;

                if (dlg.ShowDialog() == true)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(dlg.FileName))
                    {
                        json = sr.ReadToEnd();
                    }
                }
                else
                {
                    return;
                }
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<ReplaceSetting> settings = jss.Deserialize<List<ReplaceSetting>>(json);
                SetDataTableFromSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private List<ReplaceSetting> DataTableToList(DataTable dt)
        {
            List<ReplaceSetting> settings = new List<ReplaceSetting>();
            try
            {
                foreach (DataRow dc in dt.Rows)
                {
                    settings.Add(new ReplaceSetting
                    {
                        Id = (int)dc["序号"],
                        BookMarkName = (string)dc["书签"],
                        ReplaceContent = (string)dc["内容"],
                    });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
            return settings;
        }

        private void SetDataTableFromSettings(List<ReplaceSetting> settings)
        {
            foreach (ReplaceSetting item in settings)
            {
                DataRow dr = myDataTable.NewRow();
                dr["书签"] = item.BookMarkName;
                dr["内容"] = item.ReplaceContent;
                myDataTable.Rows.Add(dr);
            }
            ReCreateId();
        }

        private void Checked_FuzzyMatch(object sender, RoutedEventArgs e)
        {
            if (CheckBox_FuzzyMatch.IsChecked == true)
            {
                CheckBox_MatchEnd.Visibility = Visibility.Visible;
                CheckBox_MatchStart.Visibility = Visibility.Visible;
            }
            else
            {
                CheckBox_MatchEnd.Visibility = Visibility.Hidden;
                CheckBox_MatchStart.Visibility = Visibility.Hidden;
            }
        }

        private string GetRegex(string str)
        {
            string pattern = str;
            if (CheckBox_MatchStart.IsChecked == true)
            {
                pattern = "^" + pattern;
            }
            if (CheckBox_MatchEnd.IsChecked == true)
            {
                pattern += "$";
            }
            return pattern;
        }

        private void Click_Edit_Replace(object sender, RoutedEventArgs e)
        {
            try
            {
                if (templatePath == "" || wordDoc == null)
                {
                    MessageBox.Show("未加载模板，请先打开Word模板", "SC-Information");
                    return;
                }
                else
                {
                    if (wordDoc.Bookmarks.Count == 0)
                    {
                        MessageBox.Show("当前Word文档中不存在任何书签", "SC-Information");
                        return;
                    }
                    if (DataGrid_Replace.Items.Count == 0)
                    {
                        MessageBox.Show("当前表格中未设置任何操作内容，请先添加操作数据，再执行此命令", "SC-Information");
                        return;
                    }
                    if (modifyInfo == null)
                    {
                        modifyInfo = new ModifyInfo();
                        modifyInfo.Owner = this;
                        modifyInfo.Closed += new EventHandler(HandleModifyInfoWindowClosed);
                        this.ModifiedContent += new ModifyInfoEventHandle(modifyInfo.AddItem);
                        modifyInfo.Show();
                    }
                    if (CheckBox_FuzzyMatch.IsChecked != true)
                    {
                        ReplaceNormal();
                    }
                    else
                    {
                        ReplaceFuzzy();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：\n" + ex.Message + "\nSource:" + ex.Source + "\nTrace:" + ex.StackTrace);
            }
        }

        private void HandleModifyInfoWindowClosed(object sender, EventArgs e)
        {
            modifyInfo = null;
        }

        private void ReplaceNormal()
        {
            foreach (DataRow dr in myDataTable.Rows)
            {
                string bookMarkName = (string)dr["书签"];
                foreach (MsWord.Bookmark bk in wordDoc.Bookmarks)
                {
                    if (bookMarkName == bk.Name)
                    {
                        bk.Range.Text = (string)dr["内容"];
                        OnRaiseModifyInfoEvent(new ModifyInfoEventArgs { ModifyInfoItem = new ModifyInfoItem { TargetName = (string)dr["书签"], MatchName = bk.Name, ModifyContent = (string)dr["内容"] } });
                    }
                }
            }
        }

        private void ReplaceFuzzy()
        {
            foreach (DataRow dr in myDataTable.Rows)
            {
                string bookMarkName = (string)dr["书签"];
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(GetRegex(bookMarkName));
                foreach (MsWord.Bookmark bk in wordDoc.Bookmarks)
                {
                    if (regex.IsMatch(bk.Name))
                    {
                        bk.Range.Text = (string)dr["内容"];
                        OnRaiseModifyInfoEvent(new ModifyInfoEventArgs { ModifyInfoItem = new ModifyInfoItem { TargetName = (string)dr["书签"], MatchName = bk.Name, ModifyContent = (string)dr["内容"] } });
                    }
                }
            }

        }

        private void OnRaiseModifyInfoEvent(ModifyInfoEventArgs e)
        {
            ModifiedContent?.Invoke(this, e);
        }

    }
}

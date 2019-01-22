using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ExcelConverter
{
    public class ExcelConverter
    {
        public void XlsxToXls()
        {
            string[] xlsx_files = GetFiles("Xlsx文档(.xlsx)|*.xlsx");
            if (xlsx_files.Length == 0) return;
            Application app = new Application();
            app.Visible = false;
            foreach (string path in xlsx_files)
            {
                Workbook workbook = app.Workbooks.Open(path);
                workbook.SaveAs(path.Substring(0, path.Length - 1), XlFileFormat.xlAddIn8);
                Console.WriteLine(path.Substring(0, path.Length - 1) + "   已转换");
                workbook.Close();
            }
            Console.WriteLine("全部转换已完成！");
            app.Quit();
        }

        public void XlsToXlsx()
        {
            string[] xls_files = GetFiles("Xls文档(.xls)|*.xls");
            if (xls_files.Length == 0) return;
            Application app = new Application();
            app.Visible = false;
            foreach (string path in xls_files)
            {
                Workbook workbook = app.Workbooks.Open(path);
                workbook.SaveAs(path + "x", XlFileFormat.xlOpenXMLWorkbook);
                Console.WriteLine(path + "x   已转换");
                workbook.Close();
            }
            Console.WriteLine("全部转换已完成！");
            app.Quit();
        }

        private string[] GetFiles(string default_ext)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = default_ext;
            openFileDialog.Multiselect = true;
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileNames;
            }
            else
            {
                return new string[0];
            }
        }

        //private string[] GetFiles2(string default_ext)
        //{
        //    System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
        //    //Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
        //    openFileDialog.Filter = default_ext;
        //    openFileDialog.Multiselect = true;

        //    System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

        //    if (result == System.Windows.Forms.DialogResult.OK)
        //    {
        //        return openFileDialog.FileNames;
        //    }
        //    else
        //    {
        //        return new string[0];
        //    }
        //}
    }
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("xlsx & xls converter - Liu.sc\n请选择：\n1 : xlsx批量转换为xls.\n2 : xls批量转换为xlsx.\nEsc : Exit.");
                    ConsoleKeyInfo info = Console.ReadKey();
                    Console.WriteLine();
                    switch (info.Key)
                    {
                        case ConsoleKey.NumPad1:
                        case ConsoleKey.D1:
                            ExcelConverter xlsx = new ExcelConverter();
                            //Thread thread = new Thread(new ThreadStart(xlsx.XlsxToXls));
                            //thread.SetApartmentState(ApartmentState.STA);
                            //thread.Start();
                            xlsx.XlsxToXls();
                            break;
                        case ConsoleKey.NumPad2:
                        case ConsoleKey.D2:
                            ExcelConverter xls = new ExcelConverter();
                            //Thread thread2 = new Thread(new ThreadStart(xls.XlsToXlsx));
                            //thread2.SetApartmentState(ApartmentState.STA);
                            //thread2.Start();
                            xls.XlsToXlsx();
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            Console.WriteLine($"{info.Key.ToString()}不对应任何命令！");
                            break;
                    }
                    Console.WriteLine();
                    Console.WriteLine("***********************************************************\n");
                    //Console.WriteLine("Press any key to exit.");
                    //Console.ReadKey();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        
    }
}

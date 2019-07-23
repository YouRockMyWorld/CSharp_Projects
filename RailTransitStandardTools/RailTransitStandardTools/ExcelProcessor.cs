using NPOI;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RailTransitStandardTools
{
    public class ExcelProcessor : IDisposable
    {
        private string _filePath;
        private IWorkbook _workbook;
        private FileStream _fileStream;
        private byte[] _rgb;

        public event EventHandler<ReportPercentageEventArgs> ReportPercentage;

        public ExcelProcessor(string filepath, byte[] rgb)
        {
            _filePath = filepath;
            _rgb = rgb;
            if (_filePath != null)
            {
                _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
                if (_filePath.EndsWith(".xls"))
                {
                    _workbook = new HSSFWorkbook(_fileStream);
                    SetXLSProperties((HSSFWorkbook)_workbook);
                }
                else if (_filePath.EndsWith(".xlsx"))
                {
                    _workbook = new XSSFWorkbook(_fileStream);
                    SetXLSXProperties((XSSFWorkbook)_workbook);
                }

            }
        }

        private Dictionary<string, string> GetData()
        {
            ISheet sheet = _workbook.GetSheetAt(0);
            Dictionary<string, string> data = new Dictionary<string, string>();

            //int lastColNum = sheet.GetRow(0).LastCellNum;
            int lastRowNum = sheet.LastRowNum;
            IRow row = null;

            for (int i = 0; i <= lastRowNum; ++i)
            {
                row = sheet.GetRow(i);
                int last_col = row.LastCellNum;
                for (int j = 0; j <= last_col; ++j)
                {
                    ICell cell = row.GetCell(j);
                    byte[] rgb_byte = cell?.CellStyle?.FillForegroundColorColor?.RGB;
                    if (rgb_byte == null) continue;
                    if (rgb_byte.Length == 3 && rgb_byte[0] == _rgb[0] && rgb_byte[1] == _rgb[1] && rgb_byte[2] == _rgb[2])
                    {
                        System.Diagnostics.Debug.WriteLine(GetCellStringValue(row.GetCell(1)));
                        System.Diagnostics.Debug.WriteLine($"{cell.RowIndex} - {cell.ColumnIndex}");
                        System.Diagnostics.Debug.WriteLine(GetCellStringValue(cell));
                        data.Add(GetCellStringValue(row.GetCell(1)), GetCellStringValue(cell));
                    }
                }
                OnReportPercentage(new ReportPercentageEventArgs { Phase = "解析数据", Percentage = (int)Math.Ceiling((float)i / lastRowNum * 100) });
                System.Diagnostics.Debug.WriteLine((int)Math.Ceiling((float)i / lastRowNum * 100));
            }

            return data;
        }

        public string WriteDataToSheet(string sheetName)
        {
            try
            {
                Dictionary<string, string> data = GetData();

                if (data == null || data?.Count == 0)
                {
                    System.Windows.MessageBox.Show("未发现有效数据", "Error");
                    OnReportPercentage(new ReportPercentageEventArgs { Phase = "进度", Percentage = 0 });
                    return "";
                }
                ISheet sheet = _workbook.CreateSheet(string.IsNullOrWhiteSpace(sheetName) ? "NewSheet" : sheetName);

                ICellStyle headerStyle = _workbook.CreateCellStyle();
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.VerticalAlignment = VerticalAlignment.Center;
                //headerStyle.BorderTop = BorderStyle.Thin;
                //headerStyle.BorderBottom = BorderStyle.Thin;
                //headerStyle.BorderLeft = BorderStyle.Thin;
                //headerStyle.BorderRight = BorderStyle.Thin;
                IFont headerFont = _workbook.CreateFont();
                headerFont.IsBold = true;
                headerFont.FontHeightInPoints = 12;
                headerFont.FontName = "微软雅黑";
                headerStyle.SetFont(headerFont);

                ICellStyle contentStyle = _workbook.CreateCellStyle();
                contentStyle.Alignment = HorizontalAlignment.Center;
                contentStyle.VerticalAlignment = VerticalAlignment.Center;
                contentStyle.BorderTop = BorderStyle.Thin;
                contentStyle.BorderBottom = BorderStyle.Thin;
                contentStyle.BorderLeft = BorderStyle.Thin;
                contentStyle.BorderRight = BorderStyle.Thin;
                IFont contentFont = _workbook.CreateFont();
                contentFont.FontHeightInPoints = 11;
                contentFont.FontName = "微软雅黑";
                contentStyle.SetFont(contentFont);

                int cur_row = 0;
                foreach(var item in data)
                {
                    //写首行数据，编码和名称
                    IRow row = sheet.CreateRow(cur_row);
                    ICell cell_code = row.CreateCell(0, CellType.String);
                    cell_code.CellStyle = headerStyle;
                    cell_code.SetCellValue(item.Key);
                    ICell cell_name = row.CreateCell(1, CellType.String);
                    cell_name.CellStyle = headerStyle;
                    cell_name.SetCellValue(item.Value);
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(cur_row, cur_row, 1, 5);
                    sheet.AddMergedRegion(cellRangeAddress);

                    //写固定数据，每个首行后面4行都一样
                    for(int i = 1;i<= 4; ++i)
                    {
                        IRow fixed_row = sheet.CreateRow(cur_row + i);
                        ICell fixed_cell0 = fixed_row.CreateCell(0, CellType.String);
                        fixed_cell0.CellStyle = contentStyle;
                        fixed_cell0.SetCellValue("几何信息深度");

                        ICell fixed_cell1 = fixed_row.CreateCell(1, CellType.String);
                        fixed_cell1.CellStyle = contentStyle;
                        fixed_cell1.SetCellValue($"G{i}");

                        ICell fixed_cell2 = fixed_row.CreateCell(2, CellType.String);
                        fixed_cell2.CellStyle = contentStyle;

                        ICell fixed_cell3 = fixed_row.CreateCell(3, CellType.String);
                        fixed_cell3.CellStyle = contentStyle;
                        fixed_cell3.SetCellValue("非几何信息深度");

                        ICell fixed_cell4 = fixed_row.CreateCell(4, CellType.String);
                        fixed_cell4.CellStyle = contentStyle;
                        fixed_cell4.SetCellValue($"N{i}");

                        ICell fixed_cell5 = fixed_row.CreateCell(5, CellType.String);
                        fixed_cell5.CellStyle = contentStyle;
                    }
                    CellRangeAddress cellRangeAddress_geo = new CellRangeAddress(cur_row + 1, cur_row + 4, 0, 0);
                    sheet.AddMergedRegion(cellRangeAddress_geo);
                    CellRangeAddress cellRangeAddress_non_geo = new CellRangeAddress(cur_row + 1, cur_row + 4, 3, 3);
                    sheet.AddMergedRegion(cellRangeAddress_non_geo);

                    OnReportPercentage(new ReportPercentageEventArgs { Phase = "写入数据", Percentage = (int)Math.Ceiling((float)cur_row / 5 / data.Count * 100) });
                    System.Diagnostics.Debug.WriteLine((int)Math.Ceiling((float)cur_row / 5 / data.Count * 100));
                    cur_row += 5;
                }

                //int last = sheet.GetRow(1).LastCellNum;
                //for (int i = 0; i < last; ++i)
                //{
                //    OnReportPercentage(new ReportPercentageEventArgs { Phase = "格式化数据", Percentage = (int)Math.Ceiling((float)i / last * 100) });
                //    sheet.AutoSizeColumn(i, true);
                //}
                sheet.SetColumnWidth(0, 26 * 256);
                sheet.SetColumnWidth(1, 7 * 256);
                sheet.SetColumnWidth(2, 7 * 256);
                sheet.SetColumnWidth(3, 20 * 256);
                sheet.SetColumnWidth(4, 7 * 256);
                sheet.SetColumnWidth(5, 7 * 256);

                string savePath = "";
                string rgb = $"R{_rgb[0].ToString()}G{_rgb[1].ToString()}B{_rgb[2].ToString()}";
                if (_filePath.EndsWith(".xls"))
                {
                    savePath = _filePath.Replace(".xls", $"_new_{rgb}.xls");
                }
                else if (_filePath.EndsWith(".xlsx"))
                {
                    savePath = _filePath.Replace(".xlsx", $"_new_{rgb}.xlsx");
                }
                OnReportPercentage(new ReportPercentageEventArgs { Phase = "正在写入文件", Percentage = 0 });
                WriteToFile(savePath);
                OnReportPercentage(new ReportPercentageEventArgs { Phase = "完成", Percentage = 100 });
                return savePath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n------\nTargetSite:\n" + ex.TargetSite.ToString() + "\n------\nStackTrace:\n" + ex.StackTrace, "Error");
                return "";
            }
        }

        private string GetCellStringValue(ICell cell)
        {
            if (cell == null) return "";
            try
            {
                cell.SetCellType(CellType.String);
                return cell.StringCellValue.Trim();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}\nErrorCellAt:Row:{cell.RowIndex} Column:{cell.ColumnIndex}\n{ex.TargetSite}\n{ex.StackTrace}");
                return "";
            }
        }

        private void WriteToFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                _workbook.Write(fileStream);
            }
        }





        private void OnReportPercentage(ReportPercentageEventArgs e)
        {
            ReportPercentage?.Invoke(this, e);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    _workbook = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                _fileStream?.Close();
                _fileStream = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~ExcelProcessor()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class ReportPercentageEventArgs : EventArgs
    {
        public string Phase { get; set; }
        public int Percentage { get; set; }
    }
}

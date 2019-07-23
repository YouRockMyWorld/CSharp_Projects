using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RailTransitStandardTools
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields and Properties
        private string _filePath = default(string);
        private string _savePath = default(string);
        private int _colorR = 112, _colorG = 173, _colorB = 71;
        private int _progressPercentage;
        private string _phase = "进度";
        private BackgroundWorker _bgworker = new BackgroundWorker();

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    NotifyPropertyChanged(nameof(FilePath));
                }
            }
        }

        public int ColorR
        {
            get => _colorR;
            set
            {
                if (_colorR != value)
                {
                    _colorR = value;
                    NotifyPropertyChanged(nameof(ColorR));
                    NotifyPropertyChanged(nameof(ColorString));
                }
            }
        }

        public int ColorG
        {
            get => _colorG;
            set
            {
                if (_colorG != value)
                {
                    _colorG = value;
                    NotifyPropertyChanged(nameof(ColorG));
                    NotifyPropertyChanged(nameof(ColorString));
                }
            }
        }

        public int ColorB
        {
            get => _colorB;
            set
            {
                if (_colorB != value)
                {
                    _colorB = value;
                    NotifyPropertyChanged(nameof(ColorB));
                    NotifyPropertyChanged(nameof(ColorString));
                }
            }
        }

        public string ColorString
        {
            get
            {
                return $"#FF{ColorR.ToString("X2")}{ColorG.ToString("X2")}{ColorB.ToString("X2")}";
            }
        }

        public int ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    NotifyPropertyChanged(nameof(ProgressPercentage));
                }
            }
        }

        public string Phase
        {
            get => _phase;
            set
            {
                if(_phase != value)
                {
                    _phase = value;
                    NotifyPropertyChanged(nameof(Phase));
                }
            }
        }

        public ICommand OpenCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            _bgworker.DoWork += new DoWorkEventHandler(Bgworker_DoWork);
            _bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bgworker_WorkerCompleted);

            OpenCommand = new RelayCommand(Open);
            ExportCommand = new RelayCommand(Export);
        }

        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Open()
        {
            try
            {
                string path = GetOpenFilePath("Excel Document(*.xlsx *.xls)|*.xlsx;*.xls");
                if (path == "")
                {
                    return;
                }
                FilePath = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void Export()
        {
            try
            {
                if (_bgworker.IsBusy)
                {
                    return;
                }
                _bgworker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private string GetOpenFilePath(string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return "";
            }
        }

        private void Bgworker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (System.IO.File.Exists(_savePath))
            {
                MessageBoxResult result = MessageBox.Show($"已生成：\n{_savePath}\n是否打开文件？", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(_savePath);
                }
                _savePath = "";
            }
            GC.Collect();
        }

        private void Bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    return;
                }
                byte[] rgb = new byte[] { Convert.ToByte(ColorR), Convert.ToByte(ColorG), Convert.ToByte(ColorB) };

                using (ExcelProcessor ep = new ExcelProcessor(_filePath, rgb))
                {
                    ep.ReportPercentage += ProgressChanged;
                    _savePath = ep.WriteDataToSheet("构件信息深度表");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void ProgressChanged(object sender, ReportPercentageEventArgs e)
        {
            Phase = e.Phase;
            ProgressPercentage = e.Percentage;
        }
    }
}

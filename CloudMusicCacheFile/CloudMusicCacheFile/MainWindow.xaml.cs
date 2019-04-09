using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CloudMusicCacheFile
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] filenames;
        private const int CHUNK_SIZE = 1024;
        private BackgroundWorker bgworker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            bgworker.WorkerReportsProgress = true;
            bgworker.ProgressChanged += new ProgressChangedEventHandler(bgworker_ProgressChanged);
            bgworker.DoWork += new DoWorkEventHandler(bgworker_DoWork);
            bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgworker_WorkerCompleted);
            bgworker.WorkerSupportsCancellation = true;
        }

        private void b_click_open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".uc";
            dlg.Filter = "uc documents (.uc)|*.uc";
            dlg.Multiselect = true;

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                filenames = dlg.FileNames;
                string files = "选中文件有: ";
                foreach(string s in filenames)
                {
                    files += s + "; ";
                }
                tb_selected_files.Text = files;
                b_start.IsEnabled = true;
            }
        }

        private void b_click_start(object sender, RoutedEventArgs e)
        {
            if (bgworker.IsBusy)
                return;
            b_start.IsEnabled = false;

            bgworker.RunWorkerAsync();
        }

        private void b_click_cancel(object sender, RoutedEventArgs e)
        {
            bgworker.CancelAsync();
        }

        private void Convert(string file_path)
        {
            
            byte[] buffer;
            
            using (FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read))
            {
                string file_to_writer;

                string song_name = Get_song_name(file_path);
                if (song_name == "")
                {
                    file_to_writer = file_path.Replace(".uc", ".mp3");
                }
                else
                {
                    if (song_name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                    {
                        Regex regex = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]");
                        song_name = regex.Replace(song_name, "-");
                    }
                    file_to_writer = Path.Combine(Path.GetDirectoryName(file_path), song_name + ".mp3");
                }



                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(file_to_writer)))
                {
                    
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        int length = (int)fs.Length;
                        int chunk_num = length / CHUNK_SIZE;
                        int chunk_index = 0;
                        buffer = br.ReadBytes(CHUNK_SIZE);
                        while (buffer.Length > 0)
                        {
                            for (int i = 0; i < buffer.Length; ++i)
                            {
                                buffer[i] = (byte)(buffer[i] ^ 0xA3);
                            }

                            bw.Write(buffer);
                            buffer = br.ReadBytes(CHUNK_SIZE);
                            int n = chunk_num / 100;
                            if (n == 0) n = 1;
                            if (chunk_index % n == 0)
                            {
                                bgworker.ReportProgress((int)Math.Ceiling((float)chunk_index / chunk_num * 100));
                            }
                            chunk_index++;
                        }
                    }
                }
            }
            
        }

        private void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            tb_completed_percent.Text = $"{e.ProgressPercentage}%";
        }

        private void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (filenames.Length == 0) return;
            foreach(string path in filenames)
            {
                if (bgworker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Dispatcher.Invoke(new Action(() => tb_cur_file.Text = $"正在处理：{path}"));
                Convert(path);
                Thread.Sleep(100);
            }
        }

        private void bgworker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                return;
            }
            if (!e.Cancelled)
            {
                tb_cur_file.Text = "处理完毕.";
            }
            b_start.IsEnabled = true;
        }

        private string Get_song_name(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            string id = name.Split('-')[0];
            string url = $"http://music.163.com/api/song/detail/?id={id}&ids=%5B{id}%5D";
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] json_result;
            try
            {
                json_result = wc.DownloadData(url);
            }
            catch
            {
                json_result = null;
            }
            string song_name = "";
            if (json_result != null)
            {
                string result = Encoding.GetEncoding("UTF-8").GetString(json_result);
                Regex r = new Regex("{\"songs\":\\[{\"name\":\"(?<name>.*?)\",\"id\"");
                song_name = r.Match(result).Groups["name"].Value;

                Debug.WriteLine(song_name);
            }

            return song_name;

        }
    }
}

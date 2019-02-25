using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonJianGuanPingTaiCMDEntry
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using(Process p =new Process())
                {
                    p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;

                    p.Start();
                    p.StandardInput.WriteLine("python E:\\Projects\\Python_Projects\\JianCePingTai\\MainWindow.py");
                    p.StandardInput.AutoFlush = true;
                    p.StandardInput.WriteLine("exit");
                    Console.Write(p.StandardOutput.ReadToEnd());
                    //Console.Write(p.StandardError.ReadToEnd());
                    //Console.ReadKey();
                    //p.WaitForExit();
                    p.Close();
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}

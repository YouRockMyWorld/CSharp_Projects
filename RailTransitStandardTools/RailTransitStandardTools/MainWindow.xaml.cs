using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RailTransitStandardTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SystemCommandBinding();
            this.DataContext = new MainWindowViewModel();
        }

        private void SystemCommandBinding()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (sender, e) =>
            {
                SystemCommands.MinimizeWindow(this);
            }));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (sender, e) =>
            {
                SystemCommands.MaximizeWindow(this);
            }));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (sender, e) =>
            {
                SystemCommands.RestoreWindow(this);
            }));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (sender, e) =>
            {
                SystemCommands.CloseWindow(this);
            }));
        }
    }
}

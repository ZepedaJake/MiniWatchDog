using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace MiniWatchDog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        List<WatchTarget> targets = new List<WatchTarget>();
        DispatcherTimer tmrMain = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            tmrMain.Tick += new EventHandler(tmrMain_Tick);
            tmrMain.Interval = new TimeSpan(0, 0, 1);
            tmrMain.Start();
            targets = XMLManager.LoadWatchTargets();
            lvTargets.ItemsSource = targets;
            foreach(WatchTarget wt in targets)
            { 
               wt.LastRestart = null;
            }
        }

        private void btnSelectProcess_Click(object sender, RoutedEventArgs e)
        {
            WindowAllProcesses wap = new WindowAllProcesses();
            wap.Owner = this;
            if (wap.ShowDialog() == true)
            {
                targets.Add(wap.selectedProcess);
                lvTargets.ItemsSource = targets;
                Task.Run(() => XMLManager.SaveWatchTargets(targets));
            }
        }

        bool UpdateStatuses()
        {
            List<Process> allProcesses = Process.GetProcesses().ToList();
            foreach (WatchTarget wt in targets)
            {
                if (allProcesses.Where(x => x.ProcessName == wt.ProcessName).Count() <= 0)
                {
                    //this target is not running. start it.
                    wt.Running = false;
                    if(wt.Enabled)
                    {
                        wt.StartProcess();
                        wt.LastRestart = DateTime.Now;
                    }                    
                }
                else
                {
                    wt.Running = true;
                }
            }
            
            return true;
        }

        int refreshCounter = 0;
        private async void tmrMain_Tick(object sender, EventArgs e)
        {
            refreshCounter++;
            if(refreshCounter == 1)
            {
                ellLoad2.Visibility = Visibility.Hidden;
                ellLoad3.Visibility = Visibility.Hidden;
            }
            else if(refreshCounter == 2)
            {
                ellLoad2.Visibility = Visibility.Visible;              
            }
            else if(refreshCounter == 3)
            {
                ellLoad3.Visibility = Visibility.Visible;
                refreshCounter = 0;
                tmrMain.Stop();
                await Task.Run(() => UpdateStatuses());
                lvTargets.Items.Refresh();
                tmrMain.Start();
            }           
        }

        void UpdateListViewColumns()
        {
            double newSize = (lvTargets.ActualWidth - 100) / 3;
            (lvTargets.View as GridView).Columns[1].Width = newSize;
            (lvTargets.View as GridView).Columns[2].Width = newSize;
            (lvTargets.View as GridView).Columns[3].Width = newSize;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateListViewColumns();
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            Task.Run(() => XMLManager.SaveWatchTargets(targets));
        }

        private void btnRemoveProcess_Click(object sender, RoutedEventArgs e)
        {
            if(lvTargets.SelectedItem == null)
            {
                MessageBox.Show("No Process Selected.");
            }
            else
            {               
                WatchTarget wt = (WatchTarget)lvTargets.SelectedItem;
                targets.Remove(wt);
                Task.Run(() => XMLManager.SaveWatchTargets(targets));
                lvTargets.Items.Refresh();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the Watch Dog?\nApplications will not be monitored after closing.","Closing Watch Dog",MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}

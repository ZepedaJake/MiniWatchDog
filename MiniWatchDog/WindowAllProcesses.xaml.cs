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
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;
using System.ComponentModel;

namespace MiniWatchDog
{
    /// <summary>
    /// Interaction logic for WindowAllProcesses.xaml
    /// </summary>
    public partial class WindowAllProcesses : Window
    {
        List<Process> allProcesses = new List<Process>();
        public WatchTarget selectedProcess = new WatchTarget();
        public WindowAllProcesses()
        {
            InitializeComponent();
            //get all running processes
            allProcesses = Process.GetProcesses().ToList();
            UpdateUI();
        }

        void UpdateUI()
        {
            lvProcesses.ItemsSource = allProcesses;
        }

        #region Sorting Stuff
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvProcesses.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion

        private void lvProcesses_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListView list = (System.Windows.Controls.ListView)sender;
            Process p = (Process)list.SelectedItem;
            try
            {
                selectedProcess.Location = p.MainModule.FileName;
                selectedProcess.ProcessName = p.ProcessName;
                selectedProcess.Enabled = true;
                this.DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Unable to Get Process information.\nTry running as Administrator.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
        }       
    }
}

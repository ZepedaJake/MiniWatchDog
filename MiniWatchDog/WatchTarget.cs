using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniWatchDog
{
    public class WatchTarget
    {
        public string ProcessName { get; set; }//name of process to watch
        public string Location { get; set; }//location to exe
        public bool Running { get; set; }//is this process running

        public bool Enabled { get; set; }//bool to disable watch without deleting the watch target
        public DateTime? LastRestart { get; set; }

        public void StartProcess()
        {
            Process pTemp = new Process();
            pTemp.StartInfo = new ProcessStartInfo(Location);
            pTemp.Start();
        }
    }

    
}

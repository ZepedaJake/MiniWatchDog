using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniWatchDog
{
    //used to help prevent running multiple instances
    public sealed class SingleInstance
    {
        public static bool AlreadyRunning()
        {
            bool running = false;
            try
            {
                // Getting collection of process  
                Process currentProcess = Process.GetCurrentProcess();

                // Check with other process already running   
                foreach (var p in Process.GetProcesses())
                {
                    if (p.Id != currentProcess.Id) // Check running process   
                    {
                        if (p.ProcessName.Equals(currentProcess.ProcessName) == true)
                        {
                            p.Close(); 
                            break;
                        }
                    }
                }
            }
            catch { }
            return running;
        }
    }
}

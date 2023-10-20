using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MiniWatchDog
{
    public static class XMLManager
    {
        //This will give us the full name path of the executable file:
        static string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //This will strip just the working path name:
        static string strWorkPath = Path.GetDirectoryName(strExeFilePath);

        public static XmlSerializer xsWatchTargets = new XmlSerializer(typeof(List<WatchTarget>));

        public static List<WatchTarget> LoadWatchTargets()
        {
            string watchTargetsFile = Path.Combine(strWorkPath, "WatchTargets.xml");
            List<WatchTarget> returnMe = new List<WatchTarget>();
            if(File.Exists(watchTargetsFile))
            {
                StreamReader sr = new StreamReader(watchTargetsFile);
                returnMe = (List<WatchTarget>)xsWatchTargets.Deserialize(sr);
                sr.Close();               
            }
            return returnMe;
        }

        public static void SaveWatchTargets(List<WatchTarget> targets)
        {
            string watchTargetsFile = Path.Combine(strWorkPath, "WatchTargets.xml");
            TextWriter tw = new StreamWriter(watchTargetsFile);
            xsWatchTargets.Serialize(tw,targets);
            tw.Close();
        }
    }
}

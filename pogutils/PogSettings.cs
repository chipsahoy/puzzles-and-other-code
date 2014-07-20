using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;

namespace POG.Utils
{
    public class PogSettings
    {
        static Dictionary<String, String> _settings;
        static String _filename;
        static PogSettings()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/pog/";
            System.IO.Directory.CreateDirectory(path);
            _filename = String.Format("{0}fennecsettings.json", path);

            _settings = new Dictionary<String, String>();
            Load();
        }
        static void Save()
        {
            String json = JsonConvert.SerializeObject(_settings, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            {
                using (Stream s = File.Open(_filename, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(json);
                }
            }
        }
        static void Load()
        {
            if(File.Exists(_filename))
                {
                using (Stream s = File.OpenRead(_filename))
                using (StreamReader sr = new StreamReader(s))
                {
                    String json = sr.ReadToEnd();
                    _settings = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
                }
            }
        }
        public static void Write(String name, String value)
        {
            if (_settings.ContainsKey(name))
            {
                if (_settings[name] == value)
                {
                    return;
                }
            }
            _settings[name] = value;
            Save();
        }
        public static void Write(String name, StringCollection value)
        {
        }
        public static String Read(String name, String defaultValue = "")
        {
            if (_settings.ContainsKey(name))
            {
                return _settings[name];
            }
            return defaultValue;
        }
        public static void Read(String name, out StringCollection value)
        {
            StringCollection rc = new StringCollection();
            value = rc;
        }
    }
}

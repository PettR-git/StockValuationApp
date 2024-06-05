using Newtonsoft.Json;
using StockValuationApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StockValuationApp.Main.Utilities
{
    public class StockJsonSerializerSettings
    {
        private JsonSerializerSettings settings;

        public StockJsonSerializerSettings()
        {
            settings = new JsonSerializerSettings();
            AddJsonSerializerSettings();
        }
        public JsonSerializerSettings JsonSettings { get { return settings; } set {  settings = value; } }
        public JsonSerializerSettings AddJsonSerializerSettings()
        {
            if (settings == null)
            {
                settings.Formatting = Formatting.Indented;
            }

            return settings;
        }

    }
}

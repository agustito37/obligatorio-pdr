using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public class SettingsManager
{
    public string ReadSettings(string key)
    {
        try
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? string.Empty;
        }
        catch (ConfigurationErrorsException)
        {
            Console.WriteLine("Error al leer la configuración");
            return string.Empty;
        }
    }
}

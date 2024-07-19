using AIS.Entities;
using System.Linq;

namespace AIS.Admin
{
    public static class SettingsHelper
    {
        public static string GetSettingValue(string settingName)
        {
            using (AISCEntities1 context = new AISCEntities1())
            {
                var setting = context.Settings.FirstOrDefault(s => s.SettingName == settingName);
                return setting?.SettingValue;
            }
        }
    }
}
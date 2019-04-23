
using static WinCopies.SettingsManagement.SettingsManagement;

namespace WinCopies.SettingsManagement
{
    public static class Common
    {

        public static string StartDirectory => GetProperty(new string[] { "common", "startDirectory" });

        public static bool ShowItemsCheckBox => GetBoolResult(new string[] { "common", "showItemsCheckBox" });

        public static bool ShowHiddenItems => GetBoolResult(new string[] { "common", "showHiddenItems" });

        public static bool ShowSystemItems => GetBoolResult(new string[] { "common", "showSystemItems" });

    }
}

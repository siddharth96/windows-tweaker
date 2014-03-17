using System;
using System.Windows;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal static class UserInteractionMarker {
        internal static void SetInteracted(this FrameworkElement uiElement) {
            uiElement.Tag = Constants.HasUserInteracted;
        }

        internal static bool HasUserInteracted(this FrameworkElement uiElement) {
            return uiElement.Tag as Byte? == Constants.HasUserInteracted;
        }

        internal static void SetCompatibleForOsView(this HelpCheckBox chk, WindowsVer.Windows compatibleWindows,
                bool isAvailableInHigherVerOs=true) {
            if (isAvailableInHigherVerOs) {
                if (compatibleWindows >= WindowsVer.Instance.GetName()) return;
                chk.IsEnabled = false;
                chk.HelpText = "This tweak is available for Windows " + WindowsVer.AsString(compatibleWindows) + " and onwards";
            } else if (compatibleWindows != WindowsVer.Instance.GetName()) {
                chk.IsEnabled = false;
                chk.HelpText = "This tweak is available only for Windows " + WindowsVer.AsString(compatibleWindows);
            }
        }
    }
}

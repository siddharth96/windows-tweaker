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
    }
}

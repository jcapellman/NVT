using System;
using System.Windows.Controls;

using MahApps.Metro.Controls;

namespace NVT.Extensions
{
    public static class MahAppsExtensions
    {
        public static void BringToFront(this Flyout flyout)
        {
            Canvas.SetZIndex(flyout, Int32.MaxValue);
        }
    }
}
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Helpers
{
    public static class ApplicationBarHelper
    {
        public static void SetupAppBarIconButton(IApplicationBar appBar, out ApplicationBarIconButton appBarIconButton, string text, string uri, EventHandler callBack)
        {
            // Create a new button and set the text value to the localized string from AppResources.
            appBarIconButton = new ApplicationBarIconButton(new Uri(uri, UriKind.Relative));
            appBarIconButton.Text = text;
            appBarIconButton.Click += callBack;
            appBar.Buttons.Add(appBarIconButton);
        }
        public static void SetupAppBarMenuItem(IApplicationBar appBar, out ApplicationBarMenuItem appBarMenuItem, string text, EventHandler callBack)
        {
            appBarMenuItem = new ApplicationBarMenuItem(text);
            appBarMenuItem.Click += callBack;
            appBar.MenuItems.Add(appBarMenuItem);
        }

        public static void UpdateAppBarIconButton(ApplicationBarIconButton appBarIconButton, string newUri, string text)
        {
            appBarIconButton.IconUri = new Uri(newUri, UriKind.Relative);
            appBarIconButton.Text = text;
        }
    }

}

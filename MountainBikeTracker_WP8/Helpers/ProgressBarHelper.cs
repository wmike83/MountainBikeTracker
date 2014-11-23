using Microsoft.Phone.Shell;

namespace GeoGenius.WindowsPhone8._0.Helpers
{
    /// <summary>
    /// Progress bar helper class for setting and showing the progress bar
    /// </summary>
    public class ProgressBarHelper
    {
        private static void InitializeProgressBar()
        {
            // If progress bar has not been initialized then initialize
            if (SystemTray.ProgressIndicator == null)
            {
                // Start progress bar
                SystemTray.ProgressIndicator = new ProgressIndicator();
            }
        }

        /// <summary>
        /// Progress bar helper method
        /// </summary>
        /// <param name="isVisible">set visible or not</param>
        public static void ProgressBar(bool isVisible)
        {
            InitializeProgressBar();
            // Set as infinite progress
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            // Set as visible
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        /// <summary>
        /// Set the progress bar text
        /// </summary>
        public static string ProgressBarText
        {
            set
            {
                // Initialize progress bar if not already done
                InitializeProgressBar();
                // Set text
                SystemTray.ProgressIndicator.Text = value;
            }
        }
    }
}

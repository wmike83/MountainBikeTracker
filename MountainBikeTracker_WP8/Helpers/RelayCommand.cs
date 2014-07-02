using System;
using System.Windows.Input;

namespace MountainBikeTracker_WP8.Helpers
{
    /// <summary>
    /// Class to relay command from XAML to View Models
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Action Delegates
        /// <summary>
        /// Delegate to be raised when action is taken
        /// </summary>
        Action _TargetExecuteMethod;
        #endregion

        #region Event Handlers
        public event EventHandler CanExecuteChanged = delegate { };
        #endregion

        #region Constructor
        public RelayCommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }
        #endregion

        #region Helper Methods
        public bool CanExecute(object parameter)
        {
            if (this._TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }
        public void Execute(object parameter)
        {
            if (this._TargetExecuteMethod != null)
            {
                this._TargetExecuteMethod();
            }
        }
        #endregion
    }
}

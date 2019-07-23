using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RailTransitStandardTools
{
    public class RelayCommand : ICommand
    {
        private Action mAction;
        private Predicate<object> mPredicate;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public RelayCommand(Action action, Predicate<object> predicate)
        {
            mAction = action;
            mPredicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            if(mPredicate != null)
            {
                return mPredicate(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
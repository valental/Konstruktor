using System.Collections.Generic;

namespace Konstruktor.DataHelpers
{
    public class ActionsHelper : DataObjectBase
    {
        #region Public Properties
        private Stack<List<ActionObject>> recentActions = new Stack<List<ActionObject>>();
        public Stack<List<ActionObject>> RecentActions
        {
            get => recentActions;
            private set { recentActions = value; OnPropertyChanged("RecentActions"); }
        }

        private Stack<List<ActionObject>> undoneActions = new Stack<List<ActionObject>>();
        public Stack<List<ActionObject>> UndoneActions
        {
            get => undoneActions;
            private set { undoneActions = value; OnPropertyChanged("UndoneActions"); }
        }
        #endregion

        #region Methods
        public void Add(List<ActionObject> actions)
        {
            RecentActions.Push(actions);
            OnPropertyChanged("RecentActions");
            UndoneActions = new Stack<List<ActionObject>>();
        }

        public List<ActionObject> Undo()
        {
            var actions = RecentActions.Pop();
            UndoneActions.Push(actions);
            OnPropertyChanged("RecentActions");
            OnPropertyChanged("UndoneActions");
            return actions;
        }

        public List<ActionObject> Redo()
        {
            var actions = UndoneActions.Pop();
            RecentActions.Push(actions);
            OnPropertyChanged("RecentActions");
            OnPropertyChanged("UndoneActions");
            return actions;
        }
        #endregion
    }
}

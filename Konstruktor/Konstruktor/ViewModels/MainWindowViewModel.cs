using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;

using Konstruktor.Controls;
using Konstruktor.DataHelpers;

namespace Konstruktor.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Public Properties
        private readonly List<KeyValuePair<ViewDirection, string>> possibleViews = new List<KeyValuePair<ViewDirection, string>>()
        {
            new KeyValuePair<ViewDirection, string>(ViewDirection.TopView, "Tlocrt"),
            new KeyValuePair<ViewDirection, string>(ViewDirection.FrontView, "Nacrt"),
            new KeyValuePair<ViewDirection, string>(ViewDirection.BackView, "Stražnji nacrt"),
            new KeyValuePair<ViewDirection, string>(ViewDirection.LeftView, "Lijevi bokocrt"),
            new KeyValuePair<ViewDirection, string>(ViewDirection.RightView, "Desni bokocrt")
        };
        public List<KeyValuePair<ViewDirection, string>> PossibleViews => possibleViews;

        private ViewDirection activeView = ViewDirection.TopView;
        public ViewDirection ActiveView
        {
            get => activeView;
            set { activeView = value; OnPropertyChanged("ActiveView"); }
        }

        private bool isGridVisible = false;
        public bool IsGridVisible
        {
            get => isGridVisible;
            set { isGridVisible = value; OnPropertyChanged("IsGridVisible"); }
        }

        public List<int> PossibleSizes { get; }

        private int newCuboidWidth = 1;
        public int NewCuboidWidth
        {
            get => newCuboidWidth;
            set { newCuboidWidth = value; OnPropertyChanged("NewCuboidWidth"); }
        }

        private int newCuboidDepth = 1;
        public int NewCuboidDepth
        {
            get => newCuboidDepth;
            set { newCuboidDepth = value; OnPropertyChanged("NewCuboidDepth"); }
        }

        private int newCuboidHeight = 1;
        public int NewCuboidHeight
        {
            get => newCuboidHeight;
            set { newCuboidHeight = value; OnPropertyChanged("NewCuboidHeight"); }
        }

        private ObservableCollection<Cuboid> cuboids = new ObservableCollection<Cuboid>();
        public ObservableCollection<Cuboid> Cuboids
        {
            get => cuboids;
            set
            {
                cuboids = value;
                OnPropertyChanged("Cuboids");
                CuboidsHelper.ResetIsOnTop(Cuboids);
            }
        }

        private ObservableCollection<Cuboid> cuboidsSelection = new ObservableCollection<Cuboid>();
        public ObservableCollection<Cuboid> CuboidsSelection
        {
            get => cuboidsSelection;
            set { cuboidsSelection = value; OnPropertyChanged("CuboidsSelection"); }
        }

        private ActionsHelper actions = new ActionsHelper();
        public ActionsHelper Actions
        {
            get => actions;
            set { actions = value; OnPropertyChanged("Actions"); }
        }

        private int selectedCuboidId;
        public int SelectedCuboidId
        {
            get => selectedCuboidId;
            set
            {
                if (selectedCuboidId != value)
                {
                    selectedCuboidId = value;
                    if (selectedCuboidId > 0)
                        UpdateSelectionHistory(selectedCuboidId);
                    foreach (Cuboid cuboid in CuboidsSelection)
                    {
                        cuboid.IsSelected = (cuboid.SelectId == selectedCuboidId);
                    }
                    OnPropertyChanged("SelectedCuboidId");
                }
            }
        }

        public LinkedList<int> CuboidUsageHistory { get; set; }
        #endregion

        #region Commands
        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(Save);
                return saveCommand;
            }
        }

        private RelayCommand loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (loadCommand == null)
                    loadCommand = new RelayCommand(Load);
                return loadCommand;
            }
        }

        private RelayCommand removeTopLevelCommand;
        public ICommand RemoveTopLevelCommand
        {
            get
            {
                if (removeTopLevelCommand == null)
                    removeTopLevelCommand = new RelayCommand(RemoveTopLevel);
                return removeTopLevelCommand;
            }
        }

        private RelayCommand removeHighestCommand;
        public ICommand RemoveHighestCommand
        {
            get
            {
                if (removeHighestCommand == null)
                    removeHighestCommand = new RelayCommand(RemoveHighest);
                return removeHighestCommand;
            }
        }

        private RelayCommand undoActionCommand;
        public ICommand UndoActionCommand
        {
            get
            {
                if (undoActionCommand == null)
                    undoActionCommand = new RelayCommand(UndoAction);
                return undoActionCommand;
            }
        }

        private RelayCommand redoActionCommand;
        public ICommand RedoActionCommand
        {
            get
            {
                if (redoActionCommand == null)
                    redoActionCommand = new RelayCommand(RedoAction);
                return redoActionCommand;
            }
        }

        private RelayCommand addCuboidToSelectionCommand;
        public ICommand AddCuboidToSelectionCommand
        {
            get
            {
                if (addCuboidToSelectionCommand == null)
                    addCuboidToSelectionCommand = new RelayCommand(AddCuboidToSelection);
                return addCuboidToSelectionCommand;
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            PossibleSizes = new List<int>();
            for (int i = 1; i <= Settings.MaxSize; i++)
                PossibleSizes.Add(i);

            LoadCuboidsSelection();
        }
        #endregion

        #region Methods
        private void LoadCuboidsSelection()
        {
            CuboidUsageHistory = new LinkedList<int>();
            for (int i = 1; i <= Settings.MaxSize / 2; i++)
            {
                Cuboid cuboid = new Cuboid(i, i, i);
                cuboid.FromSelection = true;
                cuboid.SelectId = i;
                CuboidsSelection.Add(cuboid);
                CuboidUsageHistory.AddLast(i);
            }
        }

        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "Konstrukcija_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), // Default file name
                DefaultExt = ".csv", // Default file extension
                Filter = "CSV Files (*.csv)|*.csv" // Filter files by extension
            };

            // Show save file dialog box
            Nullable<bool> result = saveFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                using (StreamWriter file = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (Cuboid cuboid in Cuboids)
                    {
                        file.WriteLine(cuboid.ToString());
                    }
                }
            }
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                ObservableCollection<Cuboid> tmp = new ObservableCollection<Cuboid>();
                string[] lines = File.ReadAllLines(openFileDialog.FileName);
                foreach (string line in lines)
                {
                    Cuboid cuboid = Cuboid.GetCuboid(line);
                    if (cuboid != null)
                        tmp.Add(cuboid);
                }

                List<ActionObject> actions = new List<ActionObject>();
                foreach (Cuboid cuboid in Cuboids)
                    actions.Add(new ActionObject(cuboid, ActionType.Remove));
                foreach (Cuboid cuboid in tmp)
                    actions.Add(new ActionObject(cuboid, ActionType.Add));
                Actions.Add(actions);

                Cuboids = tmp;
            }
        }

        private void RemoveTopLevel()
        {
            List<Cuboid> list = new List<Cuboid>(Cuboids);
            List<ActionObject> removed = new List<ActionObject>();
            foreach (Cuboid cuboid in list)
            {
                if (cuboid.IsOnTop)
                    removed.Add(new ActionObject(cuboid, ActionType.Remove));
            }
            list.RemoveAll(c => c.IsOnTop);
            Cuboids = new ObservableCollection<Cuboid>(list);
            Actions.Add(removed);
        }

        private void RemoveHighest()
        {
            List<Cuboid> list = new List<Cuboid>(Cuboids);
            List<ActionObject> removed = new List<ActionObject>();
            List<Cuboid> toRemove = new List<Cuboid>();
            for (int i = Settings.MaxSize; i > 0 && removed.Count == 0; i--)
            {
                foreach (Cuboid cuboid in list)
                {
                    if (cuboid.Z + cuboid.Height == i)
                    {
                        removed.Add(new ActionObject(cuboid, ActionType.Remove));
                        toRemove.Add(cuboid);
                    }
                }
            }
            foreach (Cuboid cuboid in toRemove)
                list.Remove(cuboid);
            Cuboids = new ObservableCollection<Cuboid>(list);
            Actions.Add(removed);
        }

        public void RemoveCuboid(Cuboid cuboid)
        {
            if (cuboid.IsOnTop)
            {
                List<Cuboid> list = new List<Cuboid>(Cuboids);
                list.Remove(cuboid);
                Cuboids = new ObservableCollection<Cuboid>(list);
                Actions.Add(new List<ActionObject>() { new ActionObject(cuboid, ActionType.Remove) });
            }
        }

        private void UndoAction()
        {
            ObservableCollection<Cuboid> tmp = new ObservableCollection<Cuboid>();
            foreach (Cuboid cuboid in Cuboids)
                tmp.Add(cuboid);
            foreach (var action in Actions.Undo())
            {
                if (action.Action == ActionType.Add)
                {
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        if (tmp[i].SameValues(action.Cuboid))
                        {
                            tmp.RemoveAt(i);
                            break;
                        }
                    }
                }
                else if (action.Action == ActionType.Remove)
                {
                    tmp.Add(action.Cuboid);
                }
            }
            Cuboids = tmp;
        }

        private void RedoAction()
        {
            ObservableCollection<Cuboid> tmp = new ObservableCollection<Cuboid>();
            foreach (Cuboid cuboid in Cuboids)
                tmp.Add(cuboid);
            foreach (var action in Actions.Redo())
            {
                if (action.Action == ActionType.Remove)
                {
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        if (tmp[i].SameValues(action.Cuboid))
                        {
                            tmp.RemoveAt(i);
                            break;
                        }
                    }
                }
                else if (action.Action == ActionType.Add)
                {
                    tmp.Add(action.Cuboid);
                }
            }
            Cuboids = tmp;
        }

        private void AddCuboidToSelection()
        {
            Cuboid cuboid = CuboidsSelection[CuboidUsageHistory.Last.Value - 1];
            cuboid.Width = NewCuboidWidth;
            cuboid.Depth = NewCuboidDepth;
            cuboid.Height = NewCuboidHeight;
            SelectedCuboidId = cuboid.SelectId;
        }

        private void UpdateSelectionHistory(int lastSelectedId)
        {
            CuboidUsageHistory.Remove(lastSelectedId);
            CuboidUsageHistory.AddFirst(lastSelectedId);
        }
        #endregion
    }
}

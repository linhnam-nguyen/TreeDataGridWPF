using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TreeDataGridWPF.Models
{
    public class TreeNode<T> : INotifyPropertyChanged
    {
        public T Model { get; }
        public int Depth { get; }
        public ObservableCollection<TreeNode<T>> Children { get; }
        public bool HasDummyChild { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); }
        }

        public TreeNode(T model, int depth)
        {
            Model = model;
            Depth = depth;
            Children = new ObservableCollection<TreeNode<T>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

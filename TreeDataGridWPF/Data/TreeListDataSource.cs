using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Data
{
    public class TreeListDataSource<T>
    {
        public ObservableCollection<TreeNode<T>> Roots { get; }
        public ObservableCollection<TreeNode<T>> FlatList { get; } = new ObservableCollection<TreeNode<T>>();
        private readonly Func<T, IEnumerable<T>> _childrenSelector;

        public TreeListDataSource(IEnumerable<T> roots, Func<T, IEnumerable<T>> childrenSelector)
        {
            Roots = new ObservableCollection<TreeNode<T>>();
            _childrenSelector = childrenSelector;
            foreach (var r in roots)
                Roots.Add(new TreeNode<T>(r, 0));
            BuildFlatList();
            HookEvents(Roots);
        }

        private void HookEvents(ObservableCollection<TreeNode<T>> nodes)
        {
            foreach (var node in nodes)
            {
                node.PropertyChanged += Node_PropertyChanged;
                if (node.Children.Count > 0)
                    HookEvents(node.Children);
            }
        }

        private void BuildFlatList()
        {
            FlatList.Clear();
            foreach (var root in Roots)
                AddNodeAndDescendants(root);
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TreeNode<T>.IsExpanded))
            {
                var node = (TreeNode<T>)sender;
                var idx = FlatList.IndexOf(node);
                if (node.IsExpanded)
                {
                    if (node.Children.Count == 0)
                    {
                        foreach (var child in _childrenSelector(node.Model) ?? Array.Empty<T>())
                            node.Children.Add(new TreeNode<T>(child, node.Depth + 1));
                    }
                    InsertDescendants(idx, node.Children);
                }
                else
                {
                    RemoveDescendants(node);
                }
            }
        }

        private void AddNodeAndDescendants(TreeNode<T> node)
        {
            FlatList.Add(node);
            if (node.IsExpanded && node.Children.Count > 0)
                foreach (var child in node.Children)
                    AddNodeAndDescendants(child);
        }

        private void InsertDescendants(int parentIndex, ObservableCollection<TreeNode<T>> children)
        {
            int insertAt = parentIndex + 1;
            foreach (var child in children)
            {
                FlatList.Insert(insertAt++, child);
                if (child.IsExpanded && child.Children.Count > 0)
                    InsertDescendants(FlatList.IndexOf(child), child.Children);
            }
        }

        private void RemoveDescendants(TreeNode<T> node)
        {
            for (int i = 0; i < node.Children.Count; ++i)
            {
                var child = node.Children[i];
                FlatList.Remove(child);
                if (child.IsExpanded)
                    RemoveDescendants(child);
            }
        }
    }
}

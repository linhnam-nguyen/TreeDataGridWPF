using System.Collections.ObjectModel;

namespace TreeDataGridWPF.Demo
{
    public class DemoItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public ObservableCollection<DemoItem> Children { get; set; }
    }

    public static class DemoModels
    {
        public static ObservableCollection<DemoItem> BuildSampleData()
        {
            return new ObservableCollection<DemoItem>
            {
                new DemoItem
                {
                    Name = "Root1", Value = 1,
                    Children = new ObservableCollection<DemoItem>
                    {
                        new DemoItem
                        {
                            Name = "Child1", Value = 2,
                            Children = new ObservableCollection<DemoItem>
                            {
                                new DemoItem
                                {
                                    Name = "Grandchild1", Value = 3,
                                    Children = new ObservableCollection<DemoItem>
                                    {
                                        new DemoItem { Name = "GreatGrandchild1", Value = 4, Children = new ObservableCollection<DemoItem>() }
                                    }
                                },
                                new DemoItem { Name = "Grandchild2", Value = 5, Children = new ObservableCollection<DemoItem>() }
                            }
                        },
                        new DemoItem { Name = "Child2", Value = 6, Children = new ObservableCollection<DemoItem>() }
                    }
                },
                new DemoItem
                {
                    Name = "Root2", Value = 7,
                    Children = new ObservableCollection<DemoItem>
                    {
                        new DemoItem
                        {
                            Name = "Child3", Value = 8,
                            Children = new ObservableCollection<DemoItem>
                            {
                                new DemoItem { Name = "Grandchild3", Value = 9, Children = new ObservableCollection<DemoItem>() }
                            }
                        }
                    }
                }
            };
        }
    }
}

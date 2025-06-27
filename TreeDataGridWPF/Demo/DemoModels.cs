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
                        new DemoItem { Name = "Child1", Value = 2, Children = new ObservableCollection<DemoItem>() }
                    }
                },
                new DemoItem { Name = "Root2", Value = 3, Children = new ObservableCollection<DemoItem>() }
            };
        }
    }
}

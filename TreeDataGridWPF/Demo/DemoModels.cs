using System.Collections.ObjectModel;

namespace TreeDataGridWPF.Demo
{
    public class DemoItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ObservableCollection<DemoItem> Children { get; set; }
    }
    public enum Role { Intern, Engineer, Lead, Manager, Director }

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
                            Name = "Child1", Value = Role.Director,
                            Children = new ObservableCollection<DemoItem>
                            {
                                new DemoItem
                                {
                                    Name = "Grandchild1", Value = (bool) true,
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
        public static ObservableCollection<DemoItem> BuildSampleData2()
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
                                        new DemoItem
                                        {
                                            Name = "GreatGrandchild1", Value = 4,
                                            Children = new ObservableCollection<DemoItem>
                                            {
                                                new DemoItem { Name = "GreatGreatGrandchild1", Value = 5 }
                                            }
                                        }
                                    }
                                },
                                new DemoItem
                                {
                                    Name = "Grandchild2", Value = 6,
                                    Children = new ObservableCollection<DemoItem>
                                    {
                                        new DemoItem { Name = "GreatGrandchild2", Value = 7 },
                                        new DemoItem { Name = "GreatGrandchild3", Value = 8 }
                                    }
                                }
                            }
                        },
                        new DemoItem
                        {
                            Name = "Child2", Value = 9,
                            Children = new ObservableCollection<DemoItem>()
                        },
                        new DemoItem
                        {
                            Name = "Child3", Value = 10,
                            Children = new ObservableCollection<DemoItem>
                            {
                                new DemoItem { Name = "Grandchild3", Value = 11 },
                                new DemoItem { Name = "Grandchild4", Value = 12 },
                                new DemoItem
                                {
                                    Name = "Grandchild5", Value = 13,
                                    Children = new ObservableCollection<DemoItem>
                                    {
                                        new DemoItem { Name = "GreatGrandchild4", Value = 14 },
                                        new DemoItem
                                        {
                                            Name = "GreatGrandchild5", Value = 15,
                                            Children = new ObservableCollection<DemoItem>
                                            {
                                                new DemoItem { Name = "GreatGreatGrandchild2", Value = 16 }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new DemoItem
                {
                    Name = "Root2", Value = 17,
                    Children = new ObservableCollection<DemoItem>
                    {
                        new DemoItem
                        {
                            Name = "Child4", Value = 18,
                            Children = new ObservableCollection<DemoItem>
                            {
                                new DemoItem { Name = "Grandchild6", Value = 19 },
                                new DemoItem { Name = "Grandchild7", Value = 20 }
                            }
                        }
                    }
                },
                new DemoItem
                {
                    Name = "Root3", Value = 21,
                    Children = new ObservableCollection<DemoItem>
                    {
                        new DemoItem
                        {
                            Name = "Child5", Value = 22,
                            Children = new ObservableCollection<DemoItem>()
                        }
                    }
                }
            };
        }
    }
}


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Demo
{

    public enum Role { Intern, Engineer, Lead, Manager, Director }

    public class Address
    {
        public string Street { get; set; }
        public int ZipCode { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public Address HomeAddress { get; set; }
        public List<string> Skills { get; set; }
        public Dictionary<string, double> Scores { get; set; }
    }


    public static class DemoObject
    {
        public static object Build()
        {
            return new Employee
            {
                Name = "Alice",
                Age = 30,
                Role = Role.Engineer,
                IsActive = true,
                HomeAddress = new Address
                {
                    Street = "123 Main St",
                    ZipCode = 75001
                },
                Skills = new List<string> { "C#", "WPF", "BIM" },
                Scores = new Dictionary<string, double>
                {
                    { "Math", 95.5 },
                    { "Physics", 88.0 }
                }
            };
        }
    }

    public static class DemoModels
    {
        public static ObservableCollection<DataModel> BuildSampleData()
        {
            return new ObservableCollection<DataModel>
            {
                new DataModel
                {
                    Name = "Root1", Value = 1,
                    Children = new ObservableCollection<DataModel>
                    {
                        new DataModel
                        {
                            Name = "Child1", Value = Role.Director,
                            Children = new ObservableCollection<DataModel>
                            {
                                new DataModel
                                {
                                    Name = "Grandchild1", Value = (bool) true,
                                    Children = new ObservableCollection<DataModel>
                                    {
                                        new DataModel { Name = "GreatGrandchild1", Value = 4, Children = new ObservableCollection<DataModel>() }
                                    }
                                },
                                new DataModel { Name = "Grandchild2", Value = 5, Children = new ObservableCollection<DataModel>() }
                            }
                        },
                        new DataModel { Name = "Child2", Value = 6, Children = new ObservableCollection<DataModel>() }
                    }
                },
                new DataModel
                {
                    Name = "Root2", Value = 7,
                    Children = new ObservableCollection<DataModel>
                    {
                        new DataModel
                        {
                            Name = "Child3", Value = 8,
                            Children = new ObservableCollection<DataModel>
                            {
                                new DataModel { Name = "Grandchild3", Value = 9, Children = new ObservableCollection<DataModel>() }
                            }
                        }
                    }
                }
            };
        }
        public static ObservableCollection<DataModel> BuildSampleData2()
        {
            return new ObservableCollection<DataModel>
            {
                new DataModel
                {
                    Name = "Root1", Value = 1,
                    Children = new ObservableCollection<DataModel>
                    {
                        new DataModel
                        {
                            Name = "Child1", Value = 2,
                            Children = new ObservableCollection<DataModel>
                            {
                                new DataModel
                                {
                                    Name = "Grandchild1", Value = 3,
                                    Children = new ObservableCollection<DataModel>
                                    {
                                        new DataModel
                                        {
                                            Name = "GreatGrandchild1", Value = 4,
                                            Children = new ObservableCollection<DataModel>
                                            {
                                                new DataModel { Name = "GreatGreatGrandchild1", Value = 5 }
                                            }
                                        }
                                    }
                                },
                                new DataModel
                                {
                                    Name = "Grandchild2", Value = 6,
                                    Children = new ObservableCollection<DataModel>
                                    {
                                        new DataModel { Name = "GreatGrandchild2", Value = 7 },
                                        new DataModel { Name = "GreatGrandchild3", Value = 8 }
                                    }
                                }
                            }
                        },
                        new DataModel
                        {
                            Name = "Child2", Value = 9,
                            Children = new ObservableCollection<DataModel>()
                        },
                        new DataModel
                        {
                            Name = "Child3", Value = 10,
                            Children = new ObservableCollection<DataModel>
                            {
                                new DataModel { Name = "Grandchild3", Value = 11 },
                                new DataModel { Name = "Grandchild4", Value = 12 },
                                new DataModel
                                {
                                    Name = "Grandchild5", Value = 13,
                                    Children = new ObservableCollection<DataModel>
                                    {
                                        new DataModel { Name = "GreatGrandchild4", Value = 14 },
                                        new DataModel
                                        {
                                            Name = "GreatGrandchild5", Value = 15,
                                            Children = new ObservableCollection<DataModel>
                                            {
                                                new DataModel { Name = "GreatGreatGrandchild2", Value = 16 }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new DataModel
                {
                    Name = "Root2", Value = 17,
                    Children = new ObservableCollection<DataModel>
                    {
                        new DataModel
                        {
                            Name = "Child4", Value = 18,
                            Children = new ObservableCollection<DataModel>
                            {
                                new DataModel { Name = "Grandchild6", Value = 19 },
                                new DataModel { Name = "Grandchild7", Value = 20 }
                            }
                        }
                    }
                },
                new DataModel
                {
                    Name = "Root3", Value = 21,
                    Children = new ObservableCollection<DataModel>
                    {
                        new DataModel
                        {
                            Name = "Child5", Value = 22,
                            Children = new ObservableCollection<DataModel>()
                        }
                    }
                }
            };
        }
    }
}


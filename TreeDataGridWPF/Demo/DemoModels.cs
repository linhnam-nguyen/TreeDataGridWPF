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
}


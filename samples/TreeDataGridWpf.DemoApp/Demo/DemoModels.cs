using System;
using System.Collections.Generic;

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
        public List<Employee> Reports { get; set; } = new();

    }


    public static class DemoObjects
    {
        public static object BuildPropertyTree()
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
        public static Employee BuildTableTree()
        {
            // --- CEO ---
            var ceo = new Employee
            {
                Name = "Alice Johnson",
                Age = 50,
                Role = Role.Director,
                IsActive = true,
                HomeAddress = new Address { Street = "1 Executive Blvd", ZipCode = 10001 },
                Skills = new List<string> { "Leadership", "Strategy", "Finance" },
                Scores = new Dictionary<string, double> { { "Leadership", 98.5 }, { "Strategy", 95.2 } },
            };

            // --- Manager under CEO ---
            var manager = new Employee
            {
                Name = "Bob Smith",
                Age = 40,
                Role = Role.Manager,
                IsActive = true,
                HomeAddress = new Address { Street = "22 Business Ave", ZipCode = 94105 },
                Skills = new List<string> { "Management", "C#", "WPF" },
                Scores = new Dictionary<string, double> { { "C#", 87.5 }, { "WPF", 90.3 } },
            };

            // --- Lead engineer under Manager ---
            var lead = new Employee
            {
                Name = "Charlie Nguyen",
                Age = 35,
                Role = Role.Lead,
                IsActive = true,
                HomeAddress = new Address { Street = "9 Tech Road", ZipCode = 75008 },
                Skills = new List<string> { "Revit API", "BHoM", "Automation" },
                Scores = new Dictionary<string, double> { { "Revit API", 96.1 }, { "Automation", 93.4 } },
            };

            // --- Engineers under Lead ---
            var engineer1 = new Employee
            {
                Name = "Diana Lee",
                Age = 29,
                Role = Role.Engineer,
                IsActive = true,
                HomeAddress = new Address { Street = "14 Code St", ZipCode = 60601 },
                Skills = new List<string> { "C#", "Python", "Unit Testing" },
                Scores = new Dictionary<string, double> { { "C#", 85.0 }, { "Python", 80.0 } },
            };

            var engineer2 = new Employee
            {
                Name = "Eric Wong",
                Age = 31,
                Role = Role.Engineer,
                IsActive = false,
                HomeAddress = new Address { Street = "3 Design Blvd", ZipCode = 33101 },
                Skills = new List<string> { "UI Design", "UX Research", "HTML" },
                Scores = new Dictionary<string, double> { { "UI Design", 78.2 }, { "UX Research", 82.4 } },
            };

            // --- Intern under Manager ---
            var intern = new Employee
            {
                Name = "Fiona Garcia",
                Age = 23,
                Role = Role.Intern,
                IsActive = true,
                HomeAddress = new Address { Street = "7 Student Rd", ZipCode = 50000 },
                Skills = new List<string> { "Excel", "Revit", "Data Entry" },
                Scores = new Dictionary<string, double> { { "Revit", 70.0 }, { "Excel", 75.0 } },
            };

            // --- Build hierarchy ---
            lead.Reports.AddRange(new[] { engineer1, engineer2 });
            manager.Reports.AddRange(new[] { lead, intern });
            ceo.Reports.Add(manager);

            return ceo;
        }

        public static List<Employee> BuildFlatList()
        {
            // optional non-hierarchical variant
            return new List<Employee>
            {
                new Employee { Name = "Alice", Age = 50, Role = Role.Director, IsActive = true },
                new Employee { Name = "Bob", Age = 40, Role = Role.Manager, IsActive = true },
                new Employee { Name = "Charlie", Age = 35, Role = Role.Lead, IsActive = true },
                new Employee { Name = "Diana", Age = 29, Role = Role.Engineer, IsActive = true },
                new Employee { Name = "Fiona", Age = 23, Role = Role.Intern, IsActive = false },
            };
        }
    }
}


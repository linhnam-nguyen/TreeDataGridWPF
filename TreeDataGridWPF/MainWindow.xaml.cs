using System;
using System.Windows;
using TreeDataGridWPF.Controls;
using TreeDataGridWPF.Demo;
using TreeDataGridWPF.Models;
using TreeDataGridWPF.TreeEngine;

namespace TreeDataGridWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var obj = DemoObjects.BuildPropertyTree();
            //var data2 = DataModel.ParseDataModel(obj);
            //var ds2 = new TreeListDataSource<DataModel>(data2, x => x.Children);
            //DemoTreeGrid.Build(ds2, typeof(DataModel).GetProperty("Name"), typeof(DataModel).GetProperty("Value"));

            ColumnSpec[] columns = new ColumnSpec[]
            {
                ColumnSpec.ForProperty("Name"),
                ColumnSpec.ForProperty("Age"),
                ColumnSpec.ForProperty("Role"),
                ColumnSpec.ForProperty("IsActive"),
                ColumnSpec.ForLambda(
                    "ZipCode",
                    getter: row => (row as Employee)?.HomeAddress?.ZipCode,
                    setter: (row, v) =>
                    {
                        if (row is Employee e)
                        {
                            if (e.HomeAddress == null) e.HomeAddress = new Address();
                            // Accept int / string
                            if (v is int i) e.HomeAddress.ZipCode = i;
                            else if (int.TryParse(v?.ToString(), out var parsed)) e.HomeAddress.ZipCode = parsed;
                            // else ignore invalid input
                        }
                    }
                ),
            };

            var tree = TreeTableModel.ParseTreeTableModel(
                data: DemoObjects.BuildTableTree(),
                childrenSelector: (e) => (e as Employee).Reports,
                columnSpecs: columns);

            var ds = new TreeListDataSource<TreeTableModel>(tree, x => x.Children);
            DemoTreeGrid.Build(ds, columns);

        }
    }
}

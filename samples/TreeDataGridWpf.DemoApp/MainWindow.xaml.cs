using System;
using System.Windows;
using TreeDataGridWPF.Controls;
using TreeDataGridWPF.Demo;
using TreeDataGridWPF.Models;
using TreeDataGridWPF.TreeEngine;

namespace TreeDataGridWPF.DemoApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
                            e.HomeAddress ??= new Address();
                            if (v is int i)
                            {
                                e.HomeAddress.ZipCode = i;
                            }
                            else if (int.TryParse(v?.ToString(), out var parsed))
                            {
                                e.HomeAddress.ZipCode = parsed;
                            }
                        }
                    }
                ),
            };

            var tree = TreeTableModel.ParseTreeTableModel(
                data: DemoObjects.BuildTableTree(),
                childrenSelector: e => (e as Employee)?.Reports,
                columnSpecs: columns);

            var ds = new TreeListDataSource<TreeTableModel>(tree, x => x.Children);
            DemoTreeGrid.Build(ds, columns);
        }
    }
}

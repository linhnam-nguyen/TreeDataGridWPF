using System.Windows;
using TreeDataGridWPF.TreeEngine;
using TreeDataGridWPF.Models;
using TreeDataGridWPF.Demo;

namespace TreeDataGridWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var obj = DemoObject.Build();
            var data2 = DataModel.ParseDataModel(obj);
            var ds2 = new TreeListDataSource<DataModel>(data2, x => x.Children);
            DemoTreeGrid.Build(ds2, typeof(DataModel).GetProperty("Name"), typeof(DataModel).GetProperty("Value"));
        }
    }
}

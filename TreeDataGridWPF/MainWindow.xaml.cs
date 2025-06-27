using System.Windows;
using TreeDataGridWPF.Data;
using TreeDataGridWPF.Demo;

namespace TreeDataGridWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var data = DemoModels.BuildSampleData();
            var ds = new TreeListDataSource<DemoItem>(data, x => x.Children);
            MyTreeGrid.Build(ds, typeof(DemoItem).GetProperty("Name"), typeof(DemoItem).GetProperty("Value"));
        }
    }
}

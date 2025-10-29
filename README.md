# TreeDataGridWPF

A lightweight, open-source WPF control that combines a TreeView and DataGrid: hierarchical rows, columns, and editable cells. Inspired by Avalonia TreeDataGrid, pure .NET only.

**Features:**
- Hierarchical rows (tree structure)
- Columns with editable cells (public setters)
- Flat list for virtualization
- First column has indentation and expander toggle
- MVVM-friendly, no paid libraries

## Projects
- `src/TreeDataGridWpf` &mdash; the reusable control packaged as a NuGet-ready WPF class library.
- `samples/TreeDataGridWpf.DemoApp` &mdash; a sample application showcasing the control in action.

## Usage Example
See [MainWindow.xaml.cs](./samples/TreeDataGridWpf.DemoApp/MainWindow.xaml.cs) in the demo application for an end-to-end example.

## Build & Pack
```bash
# Restore and build everything
 dotnet build

# Produce the NuGet package (outputs to bin/Release)
 dotnet pack -c Release
```

## License
MIT

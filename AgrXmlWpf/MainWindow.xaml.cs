using System.Windows;

namespace AgrXmlWpf
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainViewModel();
    }
  }
}
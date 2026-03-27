Imports System.Windows
Imports WpfApp2.WpfApp2.ViewModels
Namespace WpfApp2
    Class MainWindow


        Public Sub New(viewModel As MainViewModel
                                                    )
            InitializeComponent()
            DataContext = viewModel


        End Sub
    End Class
End Namespace


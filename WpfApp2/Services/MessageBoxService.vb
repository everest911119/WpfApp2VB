Imports System.Windows
Namespace WpfApp2.Services
    Public Class MessageBoxService
        Implements IMessageBoxService

        Public Sub Show(ByVal message As String, ByVal caption As String, ByVal button As MessageBoxButton) Implements IMessageBoxService.Show
            MessageBox.Show(message, caption, button)
        End Sub
    End Class
End Namespace

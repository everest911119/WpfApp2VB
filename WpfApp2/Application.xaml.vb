Imports System
Imports System.Windows
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Newtonsoft.Json
Imports WpfApp2.WpfApp2
Imports WpfApp2.WpfApp2.Configuration
Imports WpfApp2.WpfApp2.FileHandle
Imports WpfApp2.WpfApp2.Services
Imports WpfApp2.WpfApp2.ViewModels

Class Application


    Private _serviceProvider As IServiceProvider

    Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
        Dim configuration = New ConfigurationBuilder().
            SetBasePath(AppContext.BaseDirectory).
            AddJsonFile("appsettings.json", optional:=False, reloadOnChange:=True).
            Build()

        Dim services = New ServiceCollection()
        services.AddSingleton(Of IConfiguration)(configuration)
        services.AddSingleton(Of IMessageBoxService, MessageBoxService)()
        services.AddScoped(Of JsonFileHandle)()



        services.Configure(Of AppSettings)(configuration.GetSection("AppSettings"))
        services.AddSingleton(Of MainViewModel)()
        services.AddSingleton(Of MainWindow)()

        _serviceProvider = services.BuildServiceProvider()

        Dim mainWindow = _serviceProvider.GetRequiredService(Of MainWindow)()
        mainWindow.Show()

        MyBase.OnStartup(e)
    End Sub
End Class
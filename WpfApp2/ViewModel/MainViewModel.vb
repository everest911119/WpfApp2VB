Imports System
Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Microsoft.Extensions.Options
Imports Microsoft.Win32
Imports WpfApp2.WpfApp2.Configuration

Imports WpfApp2.WpfApp2.FileHandle
Imports WpfApp2.WpfApp2.Model

Namespace WpfApp2.ViewModels
    Public Class MainViewModel
        Inherits ObservableObject

        Private ReadOnly _settings As AppSettings
        Private ReadOnly _jsonFileHandle As JsonFileHandle

        Private _items As ObservableCollection(Of ItemDto)
        Public Property Items As ObservableCollection(Of ItemDto)
            Get
                Return _items
            End Get
            Set(value As ObservableCollection(Of ItemDto))
                SetProperty(_items, value)
            End Set
        End Property

        'Public ReadOnly Property Items As ObservableCollection(Of ItemDto) = New ObservableCollection(Of ItemDto)()

        '<ObservableProperty>
        'Public ReadOnly Property headerText As String = String.Empty

        Private _headerText As String
        Public Property HeaderText As String
            Get
                Return _headerText
            End Get
            Set(value As String)
                ' SetProperty from ObservableObjec NotifyPropertyChanged
                SetProperty(_headerText, value)
            End Set
        End Property

        Public ReadOnly Property LoadCommand As IRelayCommand
        Public ReadOnly Property ExportCommand As IRelayCommand
        Public ReadOnly Property RecalcCommand As IRelayCommand
        Public ReadOnly Property UpdateLengthCommand As IRelayCommand(Of ItemDto)

        Public Sub New(options As IOptions(Of AppSettings), jsonFileHandle As JsonFileHandle)
            _settings = options.Value
            _jsonFileHandle = jsonFileHandle

            HeaderText = _settings.HeaderText

            LoadCommand = New RelayCommand(AddressOf Load)
            ExportCommand = New RelayCommand(AddressOf Export)
            RecalcCommand = New RelayCommand(AddressOf Recalc)
            UpdateLengthCommand = New RelayCommand(Of ItemDto)(AddressOf UpdateLength)
            Items = New ObservableCollection(Of ItemDto)()
        End Sub



        Private Sub Load()
            Items.Clear()

            Dim loadedItems = _jsonFileHandle.LoadItemsFromJson()
            For Each item In loadedItems
                Items.Add(item)
            Next
        End Sub

        Private Sub Export()
            Dim fileName = GetFileName(Items)
            If String.IsNullOrWhiteSpace(fileName) Then
                Return
            End If

            _jsonFileHandle.SaveToCsv(Items.ToList(), fileName)
        End Sub

        Private Sub Recalc()
            _jsonFileHandle.SaveToJson(Items.ToList())
        End Sub

        Private Sub UpdateLength(ByVal item As ItemDto)
            If item Is Nothing Then
                Return
            End If

            If item.LengthMm <= 0 Then
                MessageBox.Show("Length cannot be negative.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim recalculated = _jsonFileHandle.Reclculate(Items.ToList())
            Items.Clear()

            For Each updated In recalculated
                Items.Add(updated)
            Next
        End Sub

        Private Function GetFileName(ByVal items As IReadOnlyCollection(Of ItemDto)) As String
            If items.Count = 0 Then
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information)
                Return String.Empty
            End If

            Dim dialog = New SaveFileDialog With {
                .Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                .FileName = $"InventoryMetric-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv"
            }

            Return If(dialog.ShowDialog() = True, dialog.FileName, String.Empty)
        End Function
    End Class
End Namespace

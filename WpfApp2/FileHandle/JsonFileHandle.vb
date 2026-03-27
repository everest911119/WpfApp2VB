Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports Microsoft.Extensions.Options
Imports System.Windows
Imports WpfApp2.WpfApp2.Model
Imports WpfApp2.WpfApp2.Services
Imports WpfApp2.WpfApp2.Configuration
Namespace WpfApp2.FileHandle
    Public Class JsonFileHandle
        Private ReadOnly _settings As AppSettings
        Private ReadOnly _messageBoxService As IMessageBoxService
        Private ReadOnly SortingDic As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

        Public Sub New(ByVal options As IOptions(Of AppSettings), ByVal messageBoxService As IMessageBoxService)
            _settings = options.Value
            _messageBoxService = messageBoxService

            For i = 0 To _settings.CategoryNames.Length - 1
                SortingDic(_settings.CategoryNames(i)) = i
            Next
        End Sub

        Public Function LoadItemsFromJson() As IReadOnlyList(Of ItemDto)
            Dim filePath = Path.Combine(AppContext.BaseDirectory, _settings.ItemFileName)

            If File.Exists(filePath) = False Then
                _messageBoxService.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK)
                Return Array.Empty(Of ItemDto)()
            End If

            Dim json = New StringBuilder()
            json.Append(File.ReadAllText(filePath))

            Try
                Dim items = JsonSerializer.Deserialize(Of List(Of ItemWithMeter))(json.ToString())

                If items Is Nothing OrElse items.Any(Function(item) item.LengthMm < 0) OrElse items.Any(Function(item) String.IsNullOrEmpty(item.Name)) Then
                    Dim wrongId = If(items?.FirstOrDefault(Function(item) item.LengthMm < 0 OrElse String.IsNullOrEmpty(item.Name))?.Id, -1)
                    _messageBoxService.Show($"Invalid data in JSON file:id: {wrongId} LengthMm must be non-negative and Name must not be empty.",
                        "Error", MessageBoxButton.OK)
                    Return Array.Empty(Of ItemDto)()
                End If

                Dim dtos = CreateDTO(items)
                _messageBoxService.Show("Data successfully loaded", "Success", MessageBoxButton.OK)
                Return dtos
            Catch ex As JsonException
                _messageBoxService.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButton.OK)
                Return Array.Empty(Of ItemDto)()
            End Try
        End Function

        ''' <summary>
        ''' cate category by length and category range defined in appsettings.json
        ''' </summary>
        ''' <param name="lengthMm"></param>
        ''' <returns></returns>
        Private Function GetCategory(ByVal lengthMm As Double) As String
            Dim category = _settings.CategoryNames(0)

            For i = 0 To _settings.CategoryRanges.Length - 1
                If lengthMm > _settings.CategoryRanges(i) Then
                    category = _settings.CategoryNames(i)
                Else
                    Exit For
                End If
            Next

            Return category
        End Function

        ''' <summary>
        ''' create DTO and sort by category and length
        ''' </summary>
        ''' <param name="items"></param>
        ''' <returns></returns>
        Private Function CreateDTO(ByVal items As List(Of ItemWithMeter)) As List(Of ItemDto)
            Dim dtos = items.Select(Function(item) New ItemDto With {
                .Id = item.Id,
                .Name = item.Name,
                .LengthMm = item.LengthMm,
                .Category = GetCategory(item.LengthMm)
            }).OrderByDescending(Function(item) If(SortingDic.ContainsKey(item.Category), SortingDic(item.Category), Integer.MaxValue)).
                ThenBy(Function(item) item.LengthMm).
                ToList()

            Return dtos
        End Function

        ''' <summary>
        ''' sort items
        ''' </summary>
        ''' <param name="items"></param>
        ''' <returns></returns>
        Private Function SortItems(ByVal items As IEnumerable(Of ItemDto)) As List(Of ItemDto)
            Return items.
                OrderByDescending(Function(item) If(SortingDic.ContainsKey(item.Category), SortingDic(item.Category), Integer.MaxValue)).
                ThenBy(Function(item) item.LengthMm).
                ToList()
        End Function

        ''' <summary>
        ''' recalculate category and sort by category and length
        ''' </summary>
        ''' <param name="items"></param>
        ''' <returns></returns>
        Public Function Reclculate(ByVal items As List(Of ItemDto)) As List(Of ItemDto)
            items.ForEach(Sub(item) item.Category = GetCategory(item.LengthMm))
            Return SortItems(items)
        End Function

        ''' <summary>
        ''' save to csv file
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="fileName"></param>
        Public Sub SaveToCsv(ByVal data As List(Of ItemDto), ByVal fileName As String)
            Dim builder = New StringBuilder()
            builder.AppendLine("Id,Name,Length,LengthInch,Category")

            For Each item In data
                builder.AppendLine(String.Join(",", item.Id, item.Name, item.LengthMm, item.LengthInch, item.Category))
            Next

            File.WriteAllText(fileName, builder.ToString(), New UTF8Encoding(True))
        End Sub

        Public Sub SaveToJson(ByVal data As List(Of ItemDto))
            Dim filePath = Path.Combine(AppContext.BaseDirectory, _settings.ItemFileName)
            Dim itemsWithMeter = data.Select(Function(item) New ItemWithMeter With {
                .Id = item.Id,
                .Name = item.Name,
                .LengthMm = item.LengthMm
            }).ToList()

            Try
                Dim json = JsonSerializer.Serialize(itemsWithMeter, New JsonSerializerOptions With {.WriteIndented = True})
                File.WriteAllText(filePath, json)
                _messageBoxService.Show($"Data successfully saved to {filePath}", "Success", MessageBoxButton.OK)
            Catch ex As Exception
                _messageBoxService.Show($"Error saving JSON: {ex.Message}", "Error", MessageBoxButton.OK)
            End Try
        End Sub
    End Class


End Namespace


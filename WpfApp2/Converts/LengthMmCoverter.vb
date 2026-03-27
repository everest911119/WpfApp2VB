
Imports System.Globalization

Namespace WpfApp2.Converts


    Public NotInheritable Class LengthMmConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim lengthValue As Integer

            If TypeOf value Is Integer Then
                lengthValue = CInt(value)
                Return $"{lengthValue} mm"
            End If

            Return String.Empty
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Dim text = TryCast(value, String)

            If text Is Nothing Then
                Return Binding.DoNothing
            End If

            text = text.Replace("mm", String.Empty, StringComparison.OrdinalIgnoreCase).Trim()

            Dim lengthValue As Integer

            If Integer.TryParse(text, NumberStyles.Integer, culture, lengthValue) Then
                Return lengthValue
            End If

            Return Binding.DoNothing
        End Function
    End Class

End Namespace
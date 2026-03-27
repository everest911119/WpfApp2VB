
Namespace WpfApp2.Model
    Public Class ItemDto
        Public Property Id As Integer
        Public Property Name As String = String.Empty
        Public Property LengthMm As Integer

        Public Property LengthInch As String
            Get
                Return ChangeToInch(LengthMm)
            End Get
            Private Set(ByVal value As String)
            End Set
        End Property

        Public Property Category As String = String.Empty

        Private Function ChangeToInch(ByVal lengthMm As Integer) As String
            Dim inchNumber = (lengthMm / CDec(25.4))
            Dim inchByEight = Math.Round(inchNumber * 8, MidpointRounding.AwayFromZero)
            Dim wholeInch = Math.Truncate(inchByEight / 8)
            Dim remainderInch As Decimal = inchByEight Mod 8
            If remainderInch = 0 Then Return $"{wholeInch}"""
            If remainderInch = 4 Then Return $"{wholeInch} 1/2"""
            If remainderInch = 2 Then Return $"{wholeInch} 1/4"""
            If remainderInch = 6 Then Return $"{wholeInch} 3/4"""
            Return $"{wholeInch} {remainderInch}/8"""
        End Function
    End Class

End Namespace

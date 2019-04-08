<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HooveredListBox
    Inherits System.Windows.Forms.ListBox

    'Die Komponente überschreibt den Löschvorgang zum Bereinigen der Komponentenliste.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
            End If
            If _BackColorBrush IsNot Nothing Then _BackColorBrush.Dispose()
            If _ForeColorBrush IsNot Nothing Then _ForeColorBrush.Dispose()
            If _HooveredBackColorBrush IsNot Nothing Then _HooveredBackColorBrush.Dispose()
            If _HooveredBorderPen IsNot Nothing Then _HooveredBorderPen.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Steuerelement-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Komponenten-Designer erforderlich.
    'Das Bearbeiten ist mit dem Komponenten-Designer möglich.
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

End Class

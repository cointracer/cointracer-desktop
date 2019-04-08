<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GainingsDisplay
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GainingsDisplay))
        Me.lblWinLossValue1 = New System.Windows.Forms.Label()
        Me.lblStichtag = New System.Windows.Forms.Label()
        Me.lblTaxable = New System.Windows.Forms.Label()
        Me.lblWinLossValue2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblWinLossValue1
        '
        resources.ApplyResources(Me.lblWinLossValue1, "lblWinLossValue1")
        Me.lblWinLossValue1.Name = "lblWinLossValue1"
        '
        'lblStichtag
        '
        resources.ApplyResources(Me.lblStichtag, "lblStichtag")
        Me.lblStichtag.Name = "lblStichtag"
        '
        'lblTaxable
        '
        resources.ApplyResources(Me.lblTaxable, "lblTaxable")
        Me.lblTaxable.Name = "lblTaxable"
        '
        'lblWinLossValue2
        '
        resources.ApplyResources(Me.lblWinLossValue2, "lblWinLossValue2")
        Me.lblWinLossValue2.Name = "lblWinLossValue2"
        '
        'GainingsDisplay
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblWinLossValue2)
        Me.Controls.Add(Me.lblTaxable)
        Me.Controls.Add(Me.lblStichtag)
        Me.Controls.Add(Me.lblWinLossValue1)
        Me.Name = "GainingsDisplay"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Protected WithEvents lblWinLossValue1 As System.Windows.Forms.Label
    Protected WithEvents lblStichtag As System.Windows.Forms.Label
    Protected WithEvents lblTaxable As System.Windows.Forms.Label
    Protected WithEvents lblWinLossValue2 As System.Windows.Forms.Label

End Class

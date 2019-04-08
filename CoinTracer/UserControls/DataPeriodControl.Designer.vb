<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DataPeriodControl
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
        Me.tbxValue = New System.Windows.Forms.TextBox()
        Me.cbxUnit = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'tbxValue
        '
        Me.tbxValue.Location = New System.Drawing.Point(3, 4)
        Me.tbxValue.MaxLength = 4
        Me.tbxValue.Name = "tbxValue"
        Me.tbxValue.Size = New System.Drawing.Size(26, 20)
        Me.tbxValue.TabIndex = 0
        '
        'cbxUnit
        '
        Me.cbxUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxUnit.FormattingEnabled = True
        Me.cbxUnit.Items.AddRange(New Object() {"Jahr(e)", "Monat(e)", "Tag(e)"})
        Me.cbxUnit.Location = New System.Drawing.Point(35, 4)
        Me.cbxUnit.Name = "cbxUnit"
        Me.cbxUnit.Size = New System.Drawing.Size(62, 21)
        Me.cbxUnit.TabIndex = 1
        '
        'DataPeriodControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.Controls.Add(Me.cbxUnit)
        Me.Controls.Add(Me.tbxValue)
        Me.MinimumSize = New System.Drawing.Size(100, 28)
        Me.Name = "DataPeriodControl"
        Me.Size = New System.Drawing.Size(100, 28)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tbxValue As System.Windows.Forms.TextBox
    Friend WithEvents cbxUnit As System.Windows.Forms.ComboBox

End Class

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ValueStrategySelector
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ValueStrategySelector))
        Me.EnhancedToolTip1 = New CoinTracer.EnhancedToolTip()
        Me.rbFifo = New System.Windows.Forms.RadioButton()
        Me.rbLifo = New System.Windows.Forms.RadioButton()
        Me.rbHifo = New System.Windows.Forms.RadioButton()
        Me.rbLofo = New System.Windows.Forms.RadioButton()
        Me.grpCVS1 = New System.Windows.Forms.GroupBox()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.grpCVS1.SuspendLayout()
        Me.SuspendLayout()
        '
        'rbFifo
        '
        resources.ApplyResources(Me.rbFifo, "rbFifo")
        Me.rbFifo.Checked = True
        Me.rbFifo.Name = "rbFifo"
        Me.rbFifo.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbFifo, resources.GetString("rbFifo.ToolTip"))
        Me.rbFifo.UseVisualStyleBackColor = True
        '
        'rbLifo
        '
        resources.ApplyResources(Me.rbLifo, "rbLifo")
        Me.rbLifo.Name = "rbLifo"
        Me.EnhancedToolTip1.SetToolTip(Me.rbLifo, resources.GetString("rbLifo.ToolTip"))
        Me.rbLifo.UseVisualStyleBackColor = True
        '
        'rbHifo
        '
        resources.ApplyResources(Me.rbHifo, "rbHifo")
        Me.rbHifo.Name = "rbHifo"
        Me.EnhancedToolTip1.SetToolTip(Me.rbHifo, resources.GetString("rbHifo.ToolTip"))
        Me.rbHifo.UseVisualStyleBackColor = True
        '
        'rbLofo
        '
        resources.ApplyResources(Me.rbLofo, "rbLofo")
        Me.rbLofo.Name = "rbLofo"
        Me.EnhancedToolTip1.SetToolTip(Me.rbLofo, resources.GetString("rbLofo.ToolTip"))
        Me.rbLofo.UseVisualStyleBackColor = True
        '
        'grpCVS1
        '
        resources.ApplyResources(Me.grpCVS1, "grpCVS1")
        Me.grpCVS1.Controls.Add(Me.rbLofo)
        Me.grpCVS1.Controls.Add(Me.rbHifo)
        Me.grpCVS1.Controls.Add(Me.rbLifo)
        Me.grpCVS1.Controls.Add(Me.rbFifo)
        Me.grpCVS1.Controls.Add(Me.Panel5)
        Me.grpCVS1.Name = "grpCVS1"
        Me.grpCVS1.TabStop = False
        '
        'Panel5
        '
        resources.ApplyResources(Me.Panel5, "Panel5")
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Name = "Panel5"
        '
        'ValueStrategySelector
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grpCVS1)
        Me.Name = "ValueStrategySelector"
        Me.grpCVS1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents EnhancedToolTip1 As CoinTracer.EnhancedToolTip
    Friend WithEvents grpCVS1 As GroupBox
    Friend WithEvents rbLofo As RadioButton
    Friend WithEvents rbHifo As RadioButton
    Friend WithEvents rbLifo As RadioButton
    Friend WithEvents rbFifo As RadioButton
    Friend WithEvents Panel5 As Panel
End Class

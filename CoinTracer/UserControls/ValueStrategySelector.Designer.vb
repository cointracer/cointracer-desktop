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
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbxAgePref = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.pnlOlderCoins = New System.Windows.Forms.Panel()
        Me.grpCVS2 = New System.Windows.Forms.GroupBox()
        Me.rbLofo2 = New System.Windows.Forms.RadioButton()
        Me.rbHifo2 = New System.Windows.Forms.RadioButton()
        Me.rbLifo2 = New System.Windows.Forms.RadioButton()
        Me.rbFifo2 = New System.Windows.Forms.RadioButton()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.grpCVS1 = New System.Windows.Forms.GroupBox()
        Me.rbLofo = New System.Windows.Forms.RadioButton()
        Me.rbHifo = New System.Windows.Forms.RadioButton()
        Me.rbLifo = New System.Windows.Forms.RadioButton()
        Me.rbFifo = New System.Windows.Forms.RadioButton()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.EnhancedToolTip1 = New CoinTracer.EnhancedToolTip()
        Me.Panel3.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.pnlOlderCoins.SuspendLayout()
        Me.grpCVS2.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.grpCVS1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Panel3, 2)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.cbxAgePref)
        resources.ApplyResources(Me.Panel3, "Panel3")
        Me.Panel3.Name = "Panel3"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'cbxAgePref
        '
        Me.cbxAgePref.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxAgePref.FormattingEnabled = True
        Me.cbxAgePref.Items.AddRange(New Object() {resources.GetString("cbxAgePref.Items"), resources.GetString("cbxAgePref.Items1"), resources.GetString("cbxAgePref.Items2")})
        resources.ApplyResources(Me.cbxAgePref, "cbxAgePref")
        Me.cbxAgePref.Name = "cbxAgePref"
        Me.EnhancedToolTip1.SetToolTip(Me.cbxAgePref, resources.GetString("cbxAgePref.ToolTip"))
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.Panel6, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.pnlOlderCoins, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel4, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel3, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'Panel6
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.Panel6, 2)
        resources.ApplyResources(Me.Panel6, "Panel6")
        Me.Panel6.Name = "Panel6"
        '
        'pnlOlderCoins
        '
        Me.pnlOlderCoins.Controls.Add(Me.grpCVS2)
        resources.ApplyResources(Me.pnlOlderCoins, "pnlOlderCoins")
        Me.pnlOlderCoins.Name = "pnlOlderCoins"
        '
        'grpCVS2
        '
        Me.grpCVS2.Controls.Add(Me.rbLofo2)
        Me.grpCVS2.Controls.Add(Me.rbHifo2)
        Me.grpCVS2.Controls.Add(Me.rbLifo2)
        Me.grpCVS2.Controls.Add(Me.rbFifo2)
        Me.grpCVS2.Controls.Add(Me.Panel7)
        resources.ApplyResources(Me.grpCVS2, "grpCVS2")
        Me.grpCVS2.Name = "grpCVS2"
        Me.grpCVS2.TabStop = False
        '
        'rbLofo2
        '
        resources.ApplyResources(Me.rbLofo2, "rbLofo2")
        Me.rbLofo2.Name = "rbLofo2"
        Me.rbLofo2.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbLofo2, resources.GetString("rbLofo2.ToolTip"))
        Me.rbLofo2.UseVisualStyleBackColor = True
        '
        'rbHifo2
        '
        resources.ApplyResources(Me.rbHifo2, "rbHifo2")
        Me.rbHifo2.Name = "rbHifo2"
        Me.rbHifo2.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbHifo2, resources.GetString("rbHifo2.ToolTip"))
        Me.rbHifo2.UseVisualStyleBackColor = True
        '
        'rbLifo2
        '
        resources.ApplyResources(Me.rbLifo2, "rbLifo2")
        Me.rbLifo2.Name = "rbLifo2"
        Me.rbLifo2.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbLifo2, resources.GetString("rbLifo2.ToolTip"))
        Me.rbLifo2.UseVisualStyleBackColor = True
        '
        'rbFifo2
        '
        resources.ApplyResources(Me.rbFifo2, "rbFifo2")
        Me.rbFifo2.Name = "rbFifo2"
        Me.rbFifo2.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbFifo2, resources.GetString("rbFifo2.ToolTip"))
        Me.rbFifo2.UseVisualStyleBackColor = True
        '
        'Panel7
        '
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Panel7, "Panel7")
        Me.Panel7.Name = "Panel7"
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.grpCVS1)
        resources.ApplyResources(Me.Panel4, "Panel4")
        Me.Panel4.Name = "Panel4"
        '
        'grpCVS1
        '
        Me.grpCVS1.Controls.Add(Me.rbLofo)
        Me.grpCVS1.Controls.Add(Me.rbHifo)
        Me.grpCVS1.Controls.Add(Me.rbLifo)
        Me.grpCVS1.Controls.Add(Me.rbFifo)
        Me.grpCVS1.Controls.Add(Me.Panel5)
        resources.ApplyResources(Me.grpCVS1, "grpCVS1")
        Me.grpCVS1.Name = "grpCVS1"
        Me.grpCVS1.TabStop = False
        '
        'rbLofo
        '
        resources.ApplyResources(Me.rbLofo, "rbLofo")
        Me.rbLofo.Name = "rbLofo"
        Me.rbLofo.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbLofo, resources.GetString("rbLofo.ToolTip"))
        Me.rbLofo.UseVisualStyleBackColor = True
        '
        'rbHifo
        '
        resources.ApplyResources(Me.rbHifo, "rbHifo")
        Me.rbHifo.Name = "rbHifo"
        Me.rbHifo.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbHifo, resources.GetString("rbHifo.ToolTip"))
        Me.rbHifo.UseVisualStyleBackColor = True
        '
        'rbLifo
        '
        resources.ApplyResources(Me.rbLifo, "rbLifo")
        Me.rbLifo.Name = "rbLifo"
        Me.rbLifo.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbLifo, resources.GetString("rbLifo.ToolTip"))
        Me.rbLifo.UseVisualStyleBackColor = True
        '
        'rbFifo
        '
        resources.ApplyResources(Me.rbFifo, "rbFifo")
        Me.rbFifo.Name = "rbFifo"
        Me.rbFifo.TabStop = True
        Me.EnhancedToolTip1.SetToolTip(Me.rbFifo, resources.GetString("rbFifo.ToolTip"))
        Me.rbFifo.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Panel5, "Panel5")
        Me.Panel5.Name = "Panel5"
        '
        'ValueStrategySelector
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ValueStrategySelector"
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.pnlOlderCoins.ResumeLayout(False)
        Me.grpCVS2.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.grpCVS1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cbxAgePref As System.Windows.Forms.ComboBox
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents pnlOlderCoins As System.Windows.Forms.Panel
    Friend WithEvents grpCVS2 As System.Windows.Forms.GroupBox
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents grpCVS1 As System.Windows.Forms.GroupBox
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents rbLofo2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbHifo2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbLifo2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbFifo2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbLofo As System.Windows.Forms.RadioButton
    Friend WithEvents rbHifo As System.Windows.Forms.RadioButton
    Friend WithEvents rbLifo As System.Windows.Forms.RadioButton
    Friend WithEvents rbFifo As System.Windows.Forms.RadioButton
    Friend WithEvents EnhancedToolTip1 As CoinTracer.EnhancedToolTip
End Class

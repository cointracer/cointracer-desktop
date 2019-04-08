<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditTrades
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Dim ModusLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditTrades))
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.pnlOpenTransfers = New System.Windows.Forms.Panel()
        Me.cmdTransferDetectionSettings = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.grpTrades = New System.Windows.Forms.GroupBox()
        Me.cmdMergeAbort = New System.Windows.Forms.Button()
        Me.lblSelectedTransfer = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmdMerge = New System.Windows.Forms.Button()
        Me.dctrlTrades = New CoinTracer.TradeCoreDataControl()
        Me.pnlEditTrades = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ModusComboBox = New System.Windows.Forms.ComboBox()
        ModusLabel = New System.Windows.Forms.Label()
        Me.pnlOpenTransfers.SuspendLayout()
        Me.grpTrades.SuspendLayout()
        Me.pnlEditTrades.SuspendLayout()
        Me.SuspendLayout()
        '
        'ModusLabel
        '
        resources.ApplyResources(ModusLabel, "ModusLabel")
        ModusLabel.Name = "ModusLabel"
        '
        'cmdOK
        '
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        resources.ApplyResources(Me.cmdCancel, "cmdCancel")
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'pnlOpenTransfers
        '
        resources.ApplyResources(Me.pnlOpenTransfers, "pnlOpenTransfers")
        Me.pnlOpenTransfers.Controls.Add(Me.cmdTransferDetectionSettings)
        Me.pnlOpenTransfers.Controls.Add(Me.Label2)
        Me.pnlOpenTransfers.Controls.Add(Me.Label1)
        Me.pnlOpenTransfers.Name = "pnlOpenTransfers"
        '
        'cmdTransferDetectionSettings
        '
        resources.ApplyResources(Me.cmdTransferDetectionSettings, "cmdTransferDetectionSettings")
        Me.cmdTransferDetectionSettings.Name = "cmdTransferDetectionSettings"
        Me.cmdTransferDetectionSettings.UseVisualStyleBackColor = True
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'grpTrades
        '
        resources.ApplyResources(Me.grpTrades, "grpTrades")
        Me.grpTrades.Controls.Add(Me.cmdMergeAbort)
        Me.grpTrades.Controls.Add(Me.lblSelectedTransfer)
        Me.grpTrades.Controls.Add(Me.Label5)
        Me.grpTrades.Controls.Add(Me.cmdMerge)
        Me.grpTrades.Controls.Add(Me.dctrlTrades)
        Me.grpTrades.Name = "grpTrades"
        Me.grpTrades.TabStop = False
        '
        'cmdMergeAbort
        '
        resources.ApplyResources(Me.cmdMergeAbort, "cmdMergeAbort")
        Me.cmdMergeAbort.Name = "cmdMergeAbort"
        Me.cmdMergeAbort.UseVisualStyleBackColor = True
        '
        'lblSelectedTransfer
        '
        resources.ApplyResources(Me.lblSelectedTransfer, "lblSelectedTransfer")
        Me.lblSelectedTransfer.Name = "lblSelectedTransfer"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'cmdMerge
        '
        resources.ApplyResources(Me.cmdMerge, "cmdMerge")
        Me.cmdMerge.Name = "cmdMerge"
        Me.cmdMerge.UseVisualStyleBackColor = True
        '
        'dctrlTrades
        '
        resources.ApplyResources(Me.dctrlTrades, "dctrlTrades")
        Me.dctrlTrades.ControlMode = CoinTracer.TradeCoreDataControl.ControlModes.Undefined
        Me.dctrlTrades.CurrentTradeID = CType(0, Long)
        Me.dctrlTrades.EnableValidation = True
        Me.dctrlTrades.Name = "dctrlTrades"
        Me.dctrlTrades.StartID = CType(0, Long)
        '
        'pnlEditTrades
        '
        resources.ApplyResources(Me.pnlEditTrades, "pnlEditTrades")
        Me.pnlEditTrades.Controls.Add(Me.Label3)
        Me.pnlEditTrades.Controls.Add(Me.Label4)
        Me.pnlEditTrades.Name = "pnlEditTrades"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'ModusComboBox
        '
        Me.ModusComboBox.DisplayMember = "Bezeichnung"
        Me.ModusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ModusComboBox.FormattingEnabled = True
        Me.ModusComboBox.Items.AddRange(New Object() {resources.GetString("ModusComboBox.Items"), resources.GetString("ModusComboBox.Items1")})
        resources.ApplyResources(Me.ModusComboBox, "ModusComboBox")
        Me.ModusComboBox.Name = "ModusComboBox"
        Me.ModusComboBox.ValueMember = "ID"
        '
        'frmEditTrades
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.ModusComboBox)
        Me.Controls.Add(ModusLabel)
        Me.Controls.Add(Me.pnlOpenTransfers)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.grpTrades)
        Me.Controls.Add(Me.pnlEditTrades)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditTrades"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.pnlOpenTransfers.ResumeLayout(False)
        Me.pnlOpenTransfers.PerformLayout()
        Me.grpTrades.ResumeLayout(False)
        Me.grpTrades.PerformLayout()
        Me.pnlEditTrades.ResumeLayout(False)
        Me.pnlEditTrades.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dctrlTrades As CoinTracer.TradeCoreDataControl
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents pnlOpenTransfers As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents grpTrades As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cmdMergeAbort As System.Windows.Forms.Button
    Friend WithEvents cmdMerge As System.Windows.Forms.Button
    Friend WithEvents lblSelectedTransfer As System.Windows.Forms.Label
    Friend WithEvents cmdTransferDetectionSettings As System.Windows.Forms.Button
    Friend WithEvents pnlEditTrades As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ModusComboBox As System.Windows.Forms.ComboBox
End Class

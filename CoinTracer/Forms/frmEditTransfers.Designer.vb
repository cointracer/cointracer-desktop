<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditTransfers
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
        Me.components = New System.ComponentModel.Container()
        Dim lblImportfilter As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditTransfers))
        Dim lblModeFilter As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Me.pnlTop = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblCaption = New System.Windows.Forms.Label()
        Me.btClose = New System.Windows.Forms.Button()
        Me.btEditTrades = New System.Windows.Forms.Button()
        Me.lblTransferlistCaption = New System.Windows.Forms.Label()
        Me.cboImport = New System.Windows.Forms.ComboBox()
        Me.cboTransferType = New System.Windows.Forms.ComboBox()
        Me.grpFilter = New System.Windows.Forms.GroupBox()
        Me.TradesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.lblRowsDisplayed = New System.Windows.Forms.Label()
        Me.grpTransfers = New System.Windows.Forms.GroupBox()
        Me.btAssignTargetPlatform = New System.Windows.Forms.Button()
        Me.cboTargetPlatforms = New System.Windows.Forms.ComboBox()
        Me.btAssignSourcePlatform = New System.Windows.Forms.Button()
        Me.cboSourcePlatforms = New System.Windows.Forms.ComboBox()
        Me.grdTransfers = New CoinTracer.BoundDataGridView()
        Me.ID = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Zeitpunkt = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Account = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Amount = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.SourcePlatform = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.TargetPlatform = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Info = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        lblImportfilter = New System.Windows.Forms.Label()
        lblModeFilter = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Me.pnlTop.SuspendLayout()
        Me.grpFilter.SuspendLayout()
        CType(Me.TradesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTransfers.SuspendLayout()
        CType(Me.grdTransfers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblImportfilter
        '
        resources.ApplyResources(lblImportfilter, "lblImportfilter")
        lblImportfilter.Name = "lblImportfilter"
        '
        'lblModeFilter
        '
        resources.ApplyResources(lblModeFilter, "lblModeFilter")
        lblModeFilter.Name = "lblModeFilter"
        '
        'Label2
        '
        resources.ApplyResources(Label2, "Label2")
        Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Label3, "Label3")
        Label3.Name = "Label3"
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.Label1)
        Me.pnlTop.Controls.Add(Me.lblCaption)
        resources.ApplyResources(Me.pnlTop, "pnlTop")
        Me.pnlTop.Name = "pnlTop"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'lblCaption
        '
        resources.ApplyResources(Me.lblCaption, "lblCaption")
        Me.lblCaption.Name = "lblCaption"
        '
        'btClose
        '
        resources.ApplyResources(Me.btClose, "btClose")
        Me.btClose.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btClose.Name = "btClose"
        Me.btClose.UseVisualStyleBackColor = True
        '
        'btEditTrades
        '
        resources.ApplyResources(Me.btEditTrades, "btEditTrades")
        Me.btEditTrades.Name = "btEditTrades"
        Me.btEditTrades.UseVisualStyleBackColor = True
        '
        'lblTransferlistCaption
        '
        resources.ApplyResources(Me.lblTransferlistCaption, "lblTransferlistCaption")
        Me.lblTransferlistCaption.Name = "lblTransferlistCaption"
        '
        'cboImport
        '
        Me.cboImport.DisplayMember = "Display"
        Me.cboImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboImport, "cboImport")
        Me.cboImport.FormattingEnabled = True
        Me.cboImport.Name = "cboImport"
        Me.cboImport.ValueMember = "ID"
        '
        'cboTransferType
        '
        resources.ApplyResources(Me.cboTransferType, "cboTransferType")
        Me.cboTransferType.DisplayMember = "Display"
        Me.cboTransferType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTransferType.FormattingEnabled = True
        Me.cboTransferType.Items.AddRange(New Object() {resources.GetString("cboTransferType.Items"), resources.GetString("cboTransferType.Items1")})
        Me.cboTransferType.Name = "cboTransferType"
        Me.cboTransferType.ValueMember = "ID"
        '
        'grpFilter
        '
        resources.ApplyResources(Me.grpFilter, "grpFilter")
        Me.grpFilter.Controls.Add(Me.cboImport)
        Me.grpFilter.Controls.Add(Me.cboTransferType)
        Me.grpFilter.Controls.Add(lblModeFilter)
        Me.grpFilter.Controls.Add(lblImportfilter)
        Me.grpFilter.Name = "grpFilter"
        Me.grpFilter.TabStop = False
        '
        'lblRowsDisplayed
        '
        resources.ApplyResources(Me.lblRowsDisplayed, "lblRowsDisplayed")
        Me.lblRowsDisplayed.Name = "lblRowsDisplayed"
        '
        'grpTransfers
        '
        resources.ApplyResources(Me.grpTransfers, "grpTransfers")
        Me.grpTransfers.Controls.Add(Me.btAssignTargetPlatform)
        Me.grpTransfers.Controls.Add(Me.cboTargetPlatforms)
        Me.grpTransfers.Controls.Add(Label3)
        Me.grpTransfers.Controls.Add(Me.btAssignSourcePlatform)
        Me.grpTransfers.Controls.Add(Me.cboSourcePlatforms)
        Me.grpTransfers.Controls.Add(Label2)
        Me.grpTransfers.Controls.Add(Me.lblRowsDisplayed)
        Me.grpTransfers.Controls.Add(Me.grdTransfers)
        Me.grpTransfers.Controls.Add(Me.lblTransferlistCaption)
        Me.grpTransfers.Name = "grpTransfers"
        Me.grpTransfers.TabStop = False
        '
        'btAssignTargetPlatform
        '
        resources.ApplyResources(Me.btAssignTargetPlatform, "btAssignTargetPlatform")
        Me.btAssignTargetPlatform.Name = "btAssignTargetPlatform"
        Me.btAssignTargetPlatform.UseVisualStyleBackColor = True
        '
        'cboTargetPlatforms
        '
        resources.ApplyResources(Me.cboTargetPlatforms, "cboTargetPlatforms")
        Me.cboTargetPlatforms.DisplayMember = "Bezeichnung"
        Me.cboTargetPlatforms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTargetPlatforms.FormattingEnabled = True
        Me.cboTargetPlatforms.Name = "cboTargetPlatforms"
        Me.cboTargetPlatforms.ValueMember = "ID"
        '
        'btAssignSourcePlatform
        '
        resources.ApplyResources(Me.btAssignSourcePlatform, "btAssignSourcePlatform")
        Me.btAssignSourcePlatform.Name = "btAssignSourcePlatform"
        Me.btAssignSourcePlatform.UseVisualStyleBackColor = True
        '
        'cboSourcePlatforms
        '
        resources.ApplyResources(Me.cboSourcePlatforms, "cboSourcePlatforms")
        Me.cboSourcePlatforms.DisplayMember = "Bezeichnung"
        Me.cboSourcePlatforms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSourcePlatforms.FormattingEnabled = True
        Me.cboSourcePlatforms.Name = "cboSourcePlatforms"
        Me.cboSourcePlatforms.ValueMember = "ID"
        '
        'grdTransfers
        '
        Me.grdTransfers.AllowUserToAddRows = False
        Me.grdTransfers.AllowUserToDeleteRows = False
        Me.grdTransfers.AllowUserToOrderColumns = True
        Me.grdTransfers.AllowUserToResizeRows = False
        resources.ApplyResources(Me.grdTransfers, "grdTransfers")
        Me.grdTransfers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdTransfers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ID, Me.Zeitpunkt, Me.Account, Me.Amount, Me.SourcePlatform, Me.TargetPlatform, Me.Info})
        Me.grdTransfers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdTransfers.MultiSelect = False
        Me.grdTransfers.Name = "grdTransfers"
        Me.grdTransfers.ReadOnly = True
        Me.grdTransfers.RowHeadersVisible = False
        Me.grdTransfers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdTransfers.ShowCellErrors = False
        Me.grdTransfers.ShowEditingIcon = False
        Me.grdTransfers.ShowRowErrors = False
        '
        'ID
        '
        Me.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.ID.DataPropertyName = "ID"
        Me.ID.FillWeight = 65.0!
        resources.ApplyResources(Me.ID, "ID")
        Me.ID.Name = "ID"
        Me.ID.ReadOnly = True
        '
        'Zeitpunkt
        '
        Me.Zeitpunkt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.Zeitpunkt.DataPropertyName = "Zeitpunkt"
        Me.Zeitpunkt.FillWeight = 70.9137!
        resources.ApplyResources(Me.Zeitpunkt, "Zeitpunkt")
        Me.Zeitpunkt.Name = "Zeitpunkt"
        Me.Zeitpunkt.ReadOnly = True
        '
        'Account
        '
        Me.Account.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.Account.DataPropertyName = "QuellKonto"
        Me.Account.FillWeight = 70.9137!
        resources.ApplyResources(Me.Account, "Account")
        Me.Account.Name = "Account"
        Me.Account.ReadOnly = True
        '
        'Amount
        '
        Me.Amount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.Amount.DataPropertyName = "QuellBetrag"
        Me.Amount.FillWeight = 70.9137!
        resources.ApplyResources(Me.Amount, "Amount")
        Me.Amount.Name = "Amount"
        Me.Amount.ReadOnly = True
        '
        'SourcePlatform
        '
        Me.SourcePlatform.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.SourcePlatform.DataPropertyName = "QuellPlattform"
        Me.SourcePlatform.FillWeight = 70.9137!
        resources.ApplyResources(Me.SourcePlatform, "SourcePlatform")
        Me.SourcePlatform.Name = "SourcePlatform"
        Me.SourcePlatform.ReadOnly = True
        '
        'TargetPlatform
        '
        Me.TargetPlatform.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.TargetPlatform.DataPropertyName = "ZielPlattform"
        Me.TargetPlatform.FillWeight = 70.9137!
        resources.ApplyResources(Me.TargetPlatform, "TargetPlatform")
        Me.TargetPlatform.Name = "TargetPlatform"
        Me.TargetPlatform.ReadOnly = True
        '
        'Info
        '
        Me.Info.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Info.DataPropertyName = "Info"
        Me.Info.FillWeight = 141.8274!
        resources.ApplyResources(Me.Info, "Info")
        Me.Info.Name = "Info"
        Me.Info.ReadOnly = True
        '
        'frmEditTransfers
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btClose
        Me.Controls.Add(Me.grpFilter)
        Me.Controls.Add(Me.btEditTrades)
        Me.Controls.Add(Me.btClose)
        Me.Controls.Add(Me.pnlTop)
        Me.Controls.Add(Me.grpTransfers)
        Me.MinimizeBox = False
        Me.Name = "frmEditTransfers"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.grpFilter.ResumeLayout(False)
        Me.grpFilter.PerformLayout()
        CType(Me.TradesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTransfers.ResumeLayout(False)
        Me.grpTransfers.PerformLayout()
        CType(Me.grdTransfers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblCaption As System.Windows.Forms.Label
    Friend WithEvents btClose As System.Windows.Forms.Button
    Friend WithEvents btEditTrades As System.Windows.Forms.Button
    Friend WithEvents lblTransferlistCaption As System.Windows.Forms.Label
    Friend WithEvents grdTransfers As CoinTracer.BoundDataGridView
    Friend WithEvents colDisplayContent As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cboImport As System.Windows.Forms.ComboBox
    Friend WithEvents cboTransferType As System.Windows.Forms.ComboBox
    Friend WithEvents grpFilter As System.Windows.Forms.GroupBox
    Friend WithEvents TradesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents lblRowsDisplayed As System.Windows.Forms.Label
    Friend WithEvents grpTransfers As System.Windows.Forms.GroupBox
    Friend WithEvents btAssignTargetPlatform As System.Windows.Forms.Button
    Friend WithEvents cboTargetPlatforms As System.Windows.Forms.ComboBox
    Friend WithEvents btAssignSourcePlatform As System.Windows.Forms.Button
    Friend WithEvents cboSourcePlatforms As System.Windows.Forms.ComboBox
    Friend WithEvents ID As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Zeitpunkt As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Account As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Amount As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents SourcePlatform As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents TargetPlatform As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Info As CoinTracer.DataGridViewAutoFilterTextBoxColumn
End Class

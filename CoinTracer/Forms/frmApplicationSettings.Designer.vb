<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmApplicationSettings
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.components = New System.ComponentModel.Container()
        Dim TolerancePercentLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplicationSettings))
        Dim ToleranceMinutesLabel As System.Windows.Forms.Label
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.lblMinutesToHours = New System.Windows.Forms.Label()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lblCategory = New System.Windows.Forms.Label()
        Me.lbxCategories = New CoinTracer.HooveredListBox()
        Me.pnlTransferDetection = New System.Windows.Forms.Panel()
        Me.grpTransferDetection = New System.Windows.Forms.GroupBox()
        Me.ToleranceMinutesTextBox = New CoinTracer.MinutesTextbox()
        Me.cmdTransferDetection = New System.Windows.Forms.Button()
        Me.TolerancePercentTextBox = New System.Windows.Forms.TextBox()
        Me.lblTransferDetection = New System.Windows.Forms.Label()
        Me.pnlSecurity = New System.Windows.Forms.Panel()
        Me.grpSecurity = New System.Windows.Forms.GroupBox()
        Me.rbApiAsk = New System.Windows.Forms.RadioButton()
        Me.rbApiDontAsk = New System.Windows.Forms.RadioButton()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.pnlOnlineSettings = New System.Windows.Forms.Panel()
        Me.grpOnlineMode = New System.Windows.Forms.GroupBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.rbProxyOn = New System.Windows.Forms.RadioButton()
        Me.rbProxyOff = New System.Windows.Forms.RadioButton()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnlOnline3 = New System.Windows.Forms.Panel()
        Me.rbCoinCoursesOn = New System.Windows.Forms.RadioButton()
        Me.rbCoinCoursesOff = New System.Windows.Forms.RadioButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.pnlOnline2 = New System.Windows.Forms.Panel()
        Me.rbFiatCoursesOn = New System.Windows.Forms.RadioButton()
        Me.rbFiatCoursesOff = New System.Windows.Forms.RadioButton()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.pnlOnline1 = New System.Windows.Forms.Panel()
        Me.rbCheckUpdateOn = New System.Windows.Forms.RadioButton()
        Me.rbCheckUpdateOff = New System.Windows.Forms.RadioButton()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.rbOnlineMode = New System.Windows.Forms.RadioButton()
        Me.rbOfflineMode = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlMessages = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmdOpenLogLocation = New System.Windows.Forms.Button()
        Me.cbxLogLevel = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.grpMeldungen = New System.Windows.Forms.GroupBox()
        Me.grdDataMessages = New System.Windows.Forms.DataGridView()
        Me.MessageQualifier = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MessageDescription = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Action = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.pnlDisplaySettings = New System.Windows.Forms.Panel()
        Me.grpDisplaySettings = New System.Windows.Forms.GroupBox()
        Me.grpBestand = New System.Windows.Forms.GroupBox()
        Me.rbBestandUSD = New System.Windows.Forms.RadioButton()
        Me.rbBestandEUR = New System.Windows.Forms.RadioButton()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.pnlDirectories = New System.Windows.Forms.Panel()
        Me.grpDirectories = New System.Windows.Forms.GroupBox()
        Me.cmdBrowseDBFolder = New System.Windows.Forms.Button()
        Me.tbxDBFolder = New System.Windows.Forms.TextBox()
        Me.rbDBCustom = New System.Windows.Forms.RadioButton()
        Me.rbDBProgram = New System.Windows.Forms.RadioButton()
        Me.rbDBUser = New System.Windows.Forms.RadioButton()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewButtonColumn1 = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.EnhancedToolTip1 = New CoinTracer.EnhancedToolTip()
        TolerancePercentLabel = New System.Windows.Forms.Label()
        ToleranceMinutesLabel = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlTransferDetection.SuspendLayout()
        Me.grpTransferDetection.SuspendLayout()
        Me.pnlSecurity.SuspendLayout()
        Me.grpSecurity.SuspendLayout()
        Me.pnlOnlineSettings.SuspendLayout()
        Me.grpOnlineMode.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.pnlOnline3.SuspendLayout()
        Me.pnlOnline2.SuspendLayout()
        Me.pnlOnline1.SuspendLayout()
        Me.pnlMessages.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.grpMeldungen.SuspendLayout()
        CType(Me.grdDataMessages, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlDisplaySettings.SuspendLayout()
        Me.grpDisplaySettings.SuspendLayout()
        Me.grpBestand.SuspendLayout()
        Me.pnlDirectories.SuspendLayout()
        Me.grpDirectories.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TolerancePercentLabel
        '
        resources.ApplyResources(TolerancePercentLabel, "TolerancePercentLabel")
        TolerancePercentLabel.Name = "TolerancePercentLabel"
        '
        'ToleranceMinutesLabel
        '
        resources.ApplyResources(ToleranceMinutesLabel, "ToleranceMinutesLabel")
        ToleranceMinutesLabel.Name = "ToleranceMinutesLabel"
        '
        'lblMinutesToHours
        '
        resources.ApplyResources(Me.lblMinutesToHours, "lblMinutesToHours")
        Me.lblMinutesToHours.Name = "lblMinutesToHours"
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblCategory)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lbxCategories)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlTransferDetection)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlSecurity)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlOnlineSettings)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlMessages)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlDisplaySettings)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlDirectories)
        '
        'lblCategory
        '
        resources.ApplyResources(Me.lblCategory, "lblCategory")
        Me.lblCategory.Name = "lblCategory"
        '
        'lbxCategories
        '
        resources.ApplyResources(Me.lbxCategories, "lbxCategories")
        Me.lbxCategories.FormattingEnabled = True
        Me.lbxCategories.HooveredBackBolor = System.Drawing.SystemColors.GradientActiveCaption
        Me.lbxCategories.HooveredBorderColor = System.Drawing.SystemColors.MenuHighlight
        Me.lbxCategories.Items.AddRange(New Object() {resources.GetString("lbxCategories.Items"), resources.GetString("lbxCategories.Items1"), resources.GetString("lbxCategories.Items2"), resources.GetString("lbxCategories.Items3"), resources.GetString("lbxCategories.Items4"), resources.GetString("lbxCategories.Items5")})
        Me.lbxCategories.Name = "lbxCategories"
        '
        'pnlTransferDetection
        '
        Me.pnlTransferDetection.Controls.Add(Me.grpTransferDetection)
        resources.ApplyResources(Me.pnlTransferDetection, "pnlTransferDetection")
        Me.pnlTransferDetection.Name = "pnlTransferDetection"
        '
        'grpTransferDetection
        '
        resources.ApplyResources(Me.grpTransferDetection, "grpTransferDetection")
        Me.grpTransferDetection.Controls.Add(Me.ToleranceMinutesTextBox)
        Me.grpTransferDetection.Controls.Add(Me.lblMinutesToHours)
        Me.grpTransferDetection.Controls.Add(Me.cmdTransferDetection)
        Me.grpTransferDetection.Controls.Add(ToleranceMinutesLabel)
        Me.grpTransferDetection.Controls.Add(TolerancePercentLabel)
        Me.grpTransferDetection.Controls.Add(Me.TolerancePercentTextBox)
        Me.grpTransferDetection.Controls.Add(Me.lblTransferDetection)
        Me.grpTransferDetection.Name = "grpTransferDetection"
        Me.grpTransferDetection.TabStop = False
        '
        'ToleranceMinutesTextBox
        '
        Me.ToleranceMinutesTextBox.ExplanationLabel = Me.lblMinutesToHours
        resources.ApplyResources(Me.ToleranceMinutesTextBox, "ToleranceMinutesTextBox")
        Me.ToleranceMinutesTextBox.Name = "ToleranceMinutesTextBox"
        '
        'cmdTransferDetection
        '
        resources.ApplyResources(Me.cmdTransferDetection, "cmdTransferDetection")
        Me.cmdTransferDetection.Name = "cmdTransferDetection"
        Me.cmdTransferDetection.UseVisualStyleBackColor = True
        '
        'TolerancePercentTextBox
        '
        resources.ApplyResources(Me.TolerancePercentTextBox, "TolerancePercentTextBox")
        Me.TolerancePercentTextBox.Name = "TolerancePercentTextBox"
        '
        'lblTransferDetection
        '
        resources.ApplyResources(Me.lblTransferDetection, "lblTransferDetection")
        Me.lblTransferDetection.Name = "lblTransferDetection"
        '
        'pnlSecurity
        '
        Me.pnlSecurity.Controls.Add(Me.grpSecurity)
        resources.ApplyResources(Me.pnlSecurity, "pnlSecurity")
        Me.pnlSecurity.Name = "pnlSecurity"
        '
        'grpSecurity
        '
        resources.ApplyResources(Me.grpSecurity, "grpSecurity")
        Me.grpSecurity.Controls.Add(Me.rbApiAsk)
        Me.grpSecurity.Controls.Add(Me.rbApiDontAsk)
        Me.grpSecurity.Controls.Add(Me.Label6)
        Me.grpSecurity.Name = "grpSecurity"
        Me.grpSecurity.TabStop = False
        '
        'rbApiAsk
        '
        resources.ApplyResources(Me.rbApiAsk, "rbApiAsk")
        Me.rbApiAsk.Name = "rbApiAsk"
        Me.rbApiAsk.TabStop = True
        Me.rbApiAsk.UseVisualStyleBackColor = True
        '
        'rbApiDontAsk
        '
        resources.ApplyResources(Me.rbApiDontAsk, "rbApiDontAsk")
        Me.rbApiDontAsk.Name = "rbApiDontAsk"
        Me.rbApiDontAsk.TabStop = True
        Me.rbApiDontAsk.UseVisualStyleBackColor = True
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'pnlOnlineSettings
        '
        Me.pnlOnlineSettings.Controls.Add(Me.grpOnlineMode)
        resources.ApplyResources(Me.pnlOnlineSettings, "pnlOnlineSettings")
        Me.pnlOnlineSettings.Name = "pnlOnlineSettings"
        '
        'grpOnlineMode
        '
        resources.ApplyResources(Me.grpOnlineMode, "grpOnlineMode")
        Me.grpOnlineMode.Controls.Add(Me.Panel2)
        Me.grpOnlineMode.Controls.Add(Me.pnlOnline3)
        Me.grpOnlineMode.Controls.Add(Me.pnlOnline2)
        Me.grpOnlineMode.Controls.Add(Me.pnlOnline1)
        Me.grpOnlineMode.Controls.Add(Me.rbOnlineMode)
        Me.grpOnlineMode.Controls.Add(Me.rbOfflineMode)
        Me.grpOnlineMode.Controls.Add(Me.Label1)
        Me.grpOnlineMode.Name = "grpOnlineMode"
        Me.grpOnlineMode.TabStop = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.rbProxyOn)
        Me.Panel2.Controls.Add(Me.rbProxyOff)
        Me.Panel2.Controls.Add(Me.Label5)
        resources.ApplyResources(Me.Panel2, "Panel2")
        Me.Panel2.Name = "Panel2"
        '
        'rbProxyOn
        '
        resources.ApplyResources(Me.rbProxyOn, "rbProxyOn")
        Me.rbProxyOn.Name = "rbProxyOn"
        Me.rbProxyOn.TabStop = True
        Me.rbProxyOn.UseVisualStyleBackColor = True
        '
        'rbProxyOff
        '
        resources.ApplyResources(Me.rbProxyOff, "rbProxyOff")
        Me.rbProxyOff.Name = "rbProxyOff"
        Me.rbProxyOff.TabStop = True
        Me.rbProxyOff.UseVisualStyleBackColor = True
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'pnlOnline3
        '
        Me.pnlOnline3.Controls.Add(Me.rbCoinCoursesOn)
        Me.pnlOnline3.Controls.Add(Me.rbCoinCoursesOff)
        Me.pnlOnline3.Controls.Add(Me.Label4)
        resources.ApplyResources(Me.pnlOnline3, "pnlOnline3")
        Me.pnlOnline3.Name = "pnlOnline3"
        '
        'rbCoinCoursesOn
        '
        resources.ApplyResources(Me.rbCoinCoursesOn, "rbCoinCoursesOn")
        Me.rbCoinCoursesOn.Name = "rbCoinCoursesOn"
        Me.rbCoinCoursesOn.TabStop = True
        Me.rbCoinCoursesOn.UseVisualStyleBackColor = True
        '
        'rbCoinCoursesOff
        '
        resources.ApplyResources(Me.rbCoinCoursesOff, "rbCoinCoursesOff")
        Me.rbCoinCoursesOff.Name = "rbCoinCoursesOff"
        Me.rbCoinCoursesOff.TabStop = True
        Me.rbCoinCoursesOff.UseVisualStyleBackColor = True
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'pnlOnline2
        '
        Me.pnlOnline2.Controls.Add(Me.rbFiatCoursesOn)
        Me.pnlOnline2.Controls.Add(Me.rbFiatCoursesOff)
        Me.pnlOnline2.Controls.Add(Me.Label3)
        resources.ApplyResources(Me.pnlOnline2, "pnlOnline2")
        Me.pnlOnline2.Name = "pnlOnline2"
        '
        'rbFiatCoursesOn
        '
        resources.ApplyResources(Me.rbFiatCoursesOn, "rbFiatCoursesOn")
        Me.rbFiatCoursesOn.Name = "rbFiatCoursesOn"
        Me.rbFiatCoursesOn.TabStop = True
        Me.rbFiatCoursesOn.UseVisualStyleBackColor = True
        '
        'rbFiatCoursesOff
        '
        resources.ApplyResources(Me.rbFiatCoursesOff, "rbFiatCoursesOff")
        Me.rbFiatCoursesOff.Name = "rbFiatCoursesOff"
        Me.rbFiatCoursesOff.TabStop = True
        Me.rbFiatCoursesOff.UseVisualStyleBackColor = True
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'pnlOnline1
        '
        Me.pnlOnline1.Controls.Add(Me.rbCheckUpdateOn)
        Me.pnlOnline1.Controls.Add(Me.rbCheckUpdateOff)
        Me.pnlOnline1.Controls.Add(Me.Label2)
        resources.ApplyResources(Me.pnlOnline1, "pnlOnline1")
        Me.pnlOnline1.Name = "pnlOnline1"
        '
        'rbCheckUpdateOn
        '
        resources.ApplyResources(Me.rbCheckUpdateOn, "rbCheckUpdateOn")
        Me.rbCheckUpdateOn.Name = "rbCheckUpdateOn"
        Me.rbCheckUpdateOn.TabStop = True
        Me.rbCheckUpdateOn.UseVisualStyleBackColor = True
        '
        'rbCheckUpdateOff
        '
        resources.ApplyResources(Me.rbCheckUpdateOff, "rbCheckUpdateOff")
        Me.rbCheckUpdateOff.Name = "rbCheckUpdateOff"
        Me.rbCheckUpdateOff.TabStop = True
        Me.rbCheckUpdateOff.UseVisualStyleBackColor = True
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'rbOnlineMode
        '
        resources.ApplyResources(Me.rbOnlineMode, "rbOnlineMode")
        Me.rbOnlineMode.Name = "rbOnlineMode"
        Me.rbOnlineMode.TabStop = True
        Me.rbOnlineMode.UseVisualStyleBackColor = True
        '
        'rbOfflineMode
        '
        resources.ApplyResources(Me.rbOfflineMode, "rbOfflineMode")
        Me.rbOfflineMode.Name = "rbOfflineMode"
        Me.rbOfflineMode.TabStop = True
        Me.rbOfflineMode.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'pnlMessages
        '
        Me.pnlMessages.Controls.Add(Me.GroupBox1)
        Me.pnlMessages.Controls.Add(Me.grpMeldungen)
        resources.ApplyResources(Me.pnlMessages, "pnlMessages")
        Me.pnlMessages.Name = "pnlMessages"
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.cmdOpenLogLocation)
        Me.GroupBox1.Controls.Add(Me.cbxLogLevel)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'cmdOpenLogLocation
        '
        resources.ApplyResources(Me.cmdOpenLogLocation, "cmdOpenLogLocation")
        Me.cmdOpenLogLocation.Name = "cmdOpenLogLocation"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdOpenLogLocation, resources.GetString("cmdOpenLogLocation.ToolTip"))
        Me.cmdOpenLogLocation.UseVisualStyleBackColor = True
        '
        'cbxLogLevel
        '
        resources.ApplyResources(Me.cbxLogLevel, "cbxLogLevel")
        Me.cbxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxLogLevel.FormattingEnabled = True
        Me.cbxLogLevel.Items.AddRange(New Object() {resources.GetString("cbxLogLevel.Items"), resources.GetString("cbxLogLevel.Items1"), resources.GetString("cbxLogLevel.Items2"), resources.GetString("cbxLogLevel.Items3")})
        Me.cbxLogLevel.Name = "cbxLogLevel"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'grpMeldungen
        '
        resources.ApplyResources(Me.grpMeldungen, "grpMeldungen")
        Me.grpMeldungen.Controls.Add(Me.grdDataMessages)
        Me.grpMeldungen.Controls.Add(Me.Label7)
        Me.grpMeldungen.Name = "grpMeldungen"
        Me.grpMeldungen.TabStop = False
        '
        'grdDataMessages
        '
        Me.grdDataMessages.AllowUserToAddRows = False
        Me.grdDataMessages.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.grdDataMessages, "grdDataMessages")
        Me.grdDataMessages.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.grdDataMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdDataMessages.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.MessageQualifier, Me.MessageDescription, Me.Action})
        Me.grdDataMessages.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdDataMessages.Name = "grdDataMessages"
        Me.grdDataMessages.ReadOnly = True
        Me.grdDataMessages.RowHeadersVisible = False
        '
        'MessageQualifier
        '
        Me.MessageQualifier.DataPropertyName = "MessageQualifier"
        Me.MessageQualifier.Frozen = True
        resources.ApplyResources(Me.MessageQualifier, "MessageQualifier")
        Me.MessageQualifier.Name = "MessageQualifier"
        Me.MessageQualifier.ReadOnly = True
        '
        'MessageDescription
        '
        Me.MessageDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.MessageDescription.DataPropertyName = "MessageDescription"
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.MessageDescription.DefaultCellStyle = DataGridViewCellStyle1
        resources.ApplyResources(Me.MessageDescription, "MessageDescription")
        Me.MessageDescription.Name = "MessageDescription"
        Me.MessageDescription.ReadOnly = True
        '
        'Action
        '
        Me.Action.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.Action.DataPropertyName = "Action"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Action.DefaultCellStyle = DataGridViewCellStyle2
        Me.Action.FillWeight = 75.0!
        resources.ApplyResources(Me.Action, "Action")
        Me.Action.Name = "Action"
        Me.Action.ReadOnly = True
        Me.Action.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Action.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'pnlDisplaySettings
        '
        Me.pnlDisplaySettings.Controls.Add(Me.grpDisplaySettings)
        resources.ApplyResources(Me.pnlDisplaySettings, "pnlDisplaySettings")
        Me.pnlDisplaySettings.Name = "pnlDisplaySettings"
        '
        'grpDisplaySettings
        '
        resources.ApplyResources(Me.grpDisplaySettings, "grpDisplaySettings")
        Me.grpDisplaySettings.Controls.Add(Me.grpBestand)
        Me.grpDisplaySettings.Name = "grpDisplaySettings"
        Me.grpDisplaySettings.TabStop = False
        '
        'grpBestand
        '
        Me.grpBestand.Controls.Add(Me.rbBestandUSD)
        Me.grpBestand.Controls.Add(Me.rbBestandEUR)
        Me.grpBestand.Controls.Add(Me.Label8)
        resources.ApplyResources(Me.grpBestand, "grpBestand")
        Me.grpBestand.Name = "grpBestand"
        Me.grpBestand.TabStop = False
        '
        'rbBestandUSD
        '
        resources.ApplyResources(Me.rbBestandUSD, "rbBestandUSD")
        Me.rbBestandUSD.Name = "rbBestandUSD"
        Me.rbBestandUSD.TabStop = True
        Me.rbBestandUSD.UseVisualStyleBackColor = True
        '
        'rbBestandEUR
        '
        resources.ApplyResources(Me.rbBestandEUR, "rbBestandEUR")
        Me.rbBestandEUR.Name = "rbBestandEUR"
        Me.rbBestandEUR.TabStop = True
        Me.rbBestandEUR.UseVisualStyleBackColor = True
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'pnlDirectories
        '
        Me.pnlDirectories.Controls.Add(Me.grpDirectories)
        resources.ApplyResources(Me.pnlDirectories, "pnlDirectories")
        Me.pnlDirectories.Name = "pnlDirectories"
        '
        'grpDirectories
        '
        resources.ApplyResources(Me.grpDirectories, "grpDirectories")
        Me.grpDirectories.Controls.Add(Me.cmdBrowseDBFolder)
        Me.grpDirectories.Controls.Add(Me.tbxDBFolder)
        Me.grpDirectories.Controls.Add(Me.rbDBCustom)
        Me.grpDirectories.Controls.Add(Me.rbDBProgram)
        Me.grpDirectories.Controls.Add(Me.rbDBUser)
        Me.grpDirectories.Controls.Add(Me.Label10)
        Me.grpDirectories.Name = "grpDirectories"
        Me.grpDirectories.TabStop = False
        '
        'cmdBrowseDBFolder
        '
        resources.ApplyResources(Me.cmdBrowseDBFolder, "cmdBrowseDBFolder")
        Me.cmdBrowseDBFolder.Name = "cmdBrowseDBFolder"
        Me.cmdBrowseDBFolder.UseVisualStyleBackColor = True
        '
        'tbxDBFolder
        '
        resources.ApplyResources(Me.tbxDBFolder, "tbxDBFolder")
        Me.tbxDBFolder.Name = "tbxDBFolder"
        '
        'rbDBCustom
        '
        resources.ApplyResources(Me.rbDBCustom, "rbDBCustom")
        Me.rbDBCustom.Name = "rbDBCustom"
        Me.rbDBCustom.TabStop = True
        Me.rbDBCustom.UseVisualStyleBackColor = True
        '
        'rbDBProgram
        '
        resources.ApplyResources(Me.rbDBProgram, "rbDBProgram")
        Me.rbDBProgram.Name = "rbDBProgram"
        Me.rbDBProgram.TabStop = True
        Me.rbDBProgram.UseVisualStyleBackColor = True
        '
        'rbDBUser
        '
        resources.ApplyResources(Me.rbDBUser, "rbDBUser")
        Me.rbDBUser.Name = "rbDBUser"
        Me.rbDBUser.TabStop = True
        Me.rbDBUser.UseVisualStyleBackColor = True
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.cmdCancel)
        Me.Panel1.Controls.Add(Me.cmdOK)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'cmdCancel
        '
        resources.ApplyResources(Me.cmdCancel, "cmdCancel")
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdOK
        '
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "MessageQualifier"
        Me.DataGridViewTextBoxColumn1.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn1, "DataGridViewTextBoxColumn1")
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "MessageDescription"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.DataGridViewTextBoxColumn2.DefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridViewTextBoxColumn2.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn2, "DataGridViewTextBoxColumn2")
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        '
        'DataGridViewButtonColumn1
        '
        Me.DataGridViewButtonColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.DataGridViewButtonColumn1.DataPropertyName = "Action"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.DataGridViewButtonColumn1.DefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridViewButtonColumn1.FillWeight = 75.0!
        Me.DataGridViewButtonColumn1.Frozen = True
        resources.ApplyResources(Me.DataGridViewButtonColumn1, "DataGridViewButtonColumn1")
        Me.DataGridViewButtonColumn1.Name = "DataGridViewButtonColumn1"
        Me.DataGridViewButtonColumn1.ReadOnly = True
        Me.DataGridViewButtonColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewButtonColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'frmApplicationSettings
        '
        Me.AcceptButton = Me.cmdOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmApplicationSettings"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlTransferDetection.ResumeLayout(False)
        Me.grpTransferDetection.ResumeLayout(False)
        Me.grpTransferDetection.PerformLayout()
        Me.pnlSecurity.ResumeLayout(False)
        Me.grpSecurity.ResumeLayout(False)
        Me.grpSecurity.PerformLayout()
        Me.pnlOnlineSettings.ResumeLayout(False)
        Me.grpOnlineMode.ResumeLayout(False)
        Me.grpOnlineMode.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.pnlOnline3.ResumeLayout(False)
        Me.pnlOnline3.PerformLayout()
        Me.pnlOnline2.ResumeLayout(False)
        Me.pnlOnline2.PerformLayout()
        Me.pnlOnline1.ResumeLayout(False)
        Me.pnlOnline1.PerformLayout()
        Me.pnlMessages.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.grpMeldungen.ResumeLayout(False)
        Me.grpMeldungen.PerformLayout()
        CType(Me.grdDataMessages, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlDisplaySettings.ResumeLayout(False)
        Me.grpDisplaySettings.ResumeLayout(False)
        Me.grpBestand.ResumeLayout(False)
        Me.grpBestand.PerformLayout()
        Me.pnlDirectories.ResumeLayout(False)
        Me.grpDirectories.ResumeLayout(False)
        Me.grpDirectories.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents lbxCategories As Cointracer.HooveredListBox
    Friend WithEvents lblCategory As System.Windows.Forms.Label
    Friend WithEvents pnlOnlineSettings As System.Windows.Forms.Panel
    Friend WithEvents grpOnlineMode As System.Windows.Forms.GroupBox
    Friend WithEvents rbOnlineMode As System.Windows.Forms.RadioButton
    Friend WithEvents rbOfflineMode As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pnlOnline3 As System.Windows.Forms.Panel
    Friend WithEvents rbCoinCoursesOn As System.Windows.Forms.RadioButton
    Friend WithEvents rbCoinCoursesOff As System.Windows.Forms.RadioButton
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents pnlOnline2 As System.Windows.Forms.Panel
    Friend WithEvents rbFiatCoursesOn As System.Windows.Forms.RadioButton
    Friend WithEvents rbFiatCoursesOff As System.Windows.Forms.RadioButton
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents pnlOnline1 As System.Windows.Forms.Panel
    Friend WithEvents rbCheckUpdateOn As System.Windows.Forms.RadioButton
    Friend WithEvents rbCheckUpdateOff As System.Windows.Forms.RadioButton
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents rbProxyOn As System.Windows.Forms.RadioButton
    Friend WithEvents rbProxyOff As System.Windows.Forms.RadioButton
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents pnlDirectories As System.Windows.Forms.Panel
    Friend WithEvents grpDirectories As System.Windows.Forms.GroupBox
    Friend WithEvents cmdBrowseDBFolder As System.Windows.Forms.Button
    Friend WithEvents tbxDBFolder As System.Windows.Forms.TextBox
    Friend WithEvents rbDBCustom As System.Windows.Forms.RadioButton
    Friend WithEvents rbDBProgram As System.Windows.Forms.RadioButton
    Friend WithEvents rbDBUser As System.Windows.Forms.RadioButton
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents EnhancedToolTip1 As CoinTracer.EnhancedToolTip
    Friend WithEvents pnlSecurity As System.Windows.Forms.Panel
    Friend WithEvents grpSecurity As System.Windows.Forms.GroupBox
    Friend WithEvents rbApiAsk As System.Windows.Forms.RadioButton
    Friend WithEvents rbApiDontAsk As System.Windows.Forms.RadioButton
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents pnlMessages As System.Windows.Forms.Panel
    Friend WithEvents grpMeldungen As System.Windows.Forms.GroupBox
    Friend WithEvents grdDataMessages As System.Windows.Forms.DataGridView
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewButtonColumn1 As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents pnlDisplaySettings As System.Windows.Forms.Panel
    Friend WithEvents grpDisplaySettings As System.Windows.Forms.GroupBox
    Friend WithEvents grpBestand As System.Windows.Forms.GroupBox
    Friend WithEvents rbBestandUSD As System.Windows.Forms.RadioButton
    Friend WithEvents rbBestandEUR As System.Windows.Forms.RadioButton
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents cbxLogLevel As System.Windows.Forms.ComboBox
    Friend WithEvents pnlTransferDetection As System.Windows.Forms.Panel
    Friend WithEvents grpTransferDetection As System.Windows.Forms.GroupBox
    Friend WithEvents lblTransferDetection As System.Windows.Forms.Label
    Friend WithEvents TolerancePercentTextBox As System.Windows.Forms.TextBox
    Friend WithEvents cmdTransferDetection As System.Windows.Forms.Button
    Friend WithEvents lblMinutesToHours As System.Windows.Forms.Label
    Friend WithEvents cmdOpenLogLocation As System.Windows.Forms.Button
    Friend WithEvents MessageQualifier As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MessageDescription As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Action As System.Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents ToleranceMinutesTextBox As MinutesTextbox
End Class

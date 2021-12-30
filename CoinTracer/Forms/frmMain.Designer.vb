<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
                If _DB IsNot Nothing Then
                    _DB.Dispose()
                End If
                If _TVM IsNot Nothing Then
                    _TVM.Dispose()
                End If
                If _CM IsNot Nothing Then
                    _CM.Dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents DateiToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BeendenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExtrasToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EraseDBToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HilfeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InhaltToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DisclaimerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents InfoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tctlMain As System.Windows.Forms.TabControl
    Friend WithEvents tabDashboard As System.Windows.Forms.TabPage
    Friend WithEvents tabTable As System.Windows.Forms.TabPage
    Friend WithEvents tabReports As System.Windows.Forms.TabPage
    Friend WithEvents tabSettings As System.Windows.Forms.TabPage
    Friend WithEvents imlTabs As System.Windows.Forms.ImageList
    Friend WithEvents grpBestand As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents cmdReloadDash As System.Windows.Forms.Button
    Friend WithEvents dshgrdAbgaenge As CoinTracer.DashboardDataGridView
    Friend WithEvents dshgrdBestaende As CoinTracer.DashboardDataGridView
    Friend WithEvents pnlDonate As System.Windows.Forms.Panel
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents cmdDonateBTC As System.Windows.Forms.Button
    Friend WithEvents lblDonate As System.Windows.Forms.Label
    Friend WithEvents cmdDonateLTC As System.Windows.Forms.Button
    Friend WithEvents gnd1stTab As CoinTracer.GainingsDisplay
    Friend WithEvents grpSettings As System.Windows.Forms.GroupBox
    Friend WithEvents cmdCalculateGainings As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents grpGainings As System.Windows.Forms.GroupBox
    Friend WithEvents gnd2ndTab As CoinTracer.GainingsDisplay
    Friend WithEvents cmdReloadGainings As System.Windows.Forms.Button
    Friend WithEvents tpDashboard As CoinTracer.TimePeriodControl
    Friend WithEvents tpGainings As CoinTracer.TimePeriodControl
    Friend WithEvents cmdReportExport As System.Windows.Forms.Button
    Friend WithEvents cmdReloadReport As System.Windows.Forms.Button
    Friend WithEvents tpReport As CoinTracer.TimePeriodControl
    Friend WithEvents grdReport As System.Windows.Forms.DataGridView
    Friend WithEvents gnd3rdTab As CoinTracer.GainingsDisplay
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents ReleaseNotesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CleanUpDBToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents grpCalculate As System.Windows.Forms.GroupBox
    Friend WithEvents lblTransfersOpen As System.Windows.Forms.Label
    Friend WithEvents cmdTransfers As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents dtpCutOffDay As System.Windows.Forms.DateTimePicker
    Friend WithEvents DatabaseSaveMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents DatabaseLoadMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tctlTables As System.Windows.Forms.TabControl
    Friend WithEvents tabTrades As System.Windows.Forms.TabPage
    Friend WithEvents tabImporte As System.Windows.Forms.TabPage
    Friend WithEvents imlTables As System.Windows.Forms.ImageList
    Friend WithEvents grdImporte As CoinTracer.BoundDataGridView
    Friend WithEvents grdTrades As CoinTracer.BoundDataGridView
    Friend WithEvents cmnTrades As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiEditTrades As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmdTogglePlatforms As System.Windows.Forms.Button
    Friend WithEvents CheckForUpdatesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tabPlattformen As System.Windows.Forms.TabPage
    Friend WithEvents grdPlattformen As CoinTracer.BoundDataGridView
    Friend WithEvents tabKonten As System.Windows.Forms.TabPage
    Friend WithEvents grdKonten As CoinTracer.BoundDataGridView
    Friend WithEvents cmnImporte As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiEraseImport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents grpSzenarien As System.Windows.Forms.GroupBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents cmdSzenarioSave As System.Windows.Forms.Button
    Friend WithEvents cmdSzenarioDelete As System.Windows.Forms.Button
    Friend WithEvents cbxSzenario As CoinTracer.DataBoundComboBox
    Friend WithEvents ThreadsafeStatusStrip1 As CoinTracer.ThreadsafeStatusStrip
    Friend WithEvents tsmiOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents EnhancedToolTip1 As CoinTracer.EnhancedToolTip
    Friend WithEvents lblDebug As System.Windows.Forms.Label
    Friend WithEvents cmnPlattformen As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiEditPlattformen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmnKonten As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiEditKonten As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblRightMouse As System.Windows.Forms.Label
    Friend WithEvents lblSzenarioRpt As System.Windows.Forms.Label
    Friend WithEvents grpReportAdditionalData As System.Windows.Forms.GroupBox
    Friend WithEvents tbxTaxNumber As System.Windows.Forms.TextBox
    Friend WithEvents tbxUserName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmdReportPreview As System.Windows.Forms.Button
    Friend WithEvents tabKurse As System.Windows.Forms.TabPage
    Friend WithEvents grdKurse As CoinTracer.BoundDataGridView
    Friend WithEvents cmnKurse As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmiEditKurse As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn1 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn2 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn3 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents tbxReportAdvice As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ccbReportPlatforms As CoinTracer.CheckComboBox.CheckedComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cbxReportTradeSelection As System.Windows.Forms.ComboBox
    Friend WithEvents cbxReportTransfers As System.Windows.Forms.ComboBox

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle16 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle17 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle18 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle19 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.spltCntDashboard = New System.Windows.Forms.SplitContainer()
        Me.grpBestand = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.tpDashboard = New CoinTracer.TimePeriodControl()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.gnd1stTab = New CoinTracer.GainingsDisplay()
        Me.cmdReloadDash = New System.Windows.Forms.Button()
        Me.spltCntGainings = New System.Windows.Forms.SplitContainer()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdTogglePlatforms = New System.Windows.Forms.Button()
        Me.dshgrdBestaende = New CoinTracer.DashboardDataGridView()
        Me.dshgrdAbgaenge = New CoinTracer.DashboardDataGridView()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.grpDataTimes = New System.Windows.Forms.GroupBox()
        Me.grdDataTimes = New System.Windows.Forms.DataGridView()
        Me.Wallet = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Zeitpunkt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.grpImport = New System.Windows.Forms.GroupBox()
        Me.ShowHistoricImportsCheckbox = New System.Windows.Forms.CheckBox()
        Me.cbxImports = New System.Windows.Forms.ComboBox()
        Me.cmdImport = New System.Windows.Forms.Button()
        Me.grpCourses = New System.Windows.Forms.GroupBox()
        Me.lblCoinCourses = New System.Windows.Forms.Label()
        Me.cmdCoinCourses = New System.Windows.Forms.Button()
        Me.lblCourseUSD = New System.Windows.Forms.Label()
        Me.cmdCourses = New CoinTracer.PaddingButton()
        Me.grpApiImport = New System.Windows.Forms.GroupBox()
        Me.cmdConfigApi = New System.Windows.Forms.Button()
        Me.cmdImportApi = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.DateiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DatabaseSaveMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DatabaseLoadMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.BeendenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtrasToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.CleanUpDBToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.EraseDBToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewDBToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HilfeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DisclaimerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InhaltToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReleaseNotesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LizenzinformationenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.CheckForUpdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.InfoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tctlMain = New System.Windows.Forms.TabControl()
        Me.tabDashboard = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.tabSettings = New System.Windows.Forms.TabPage()
        Me.grpSzenarien = New System.Windows.Forms.GroupBox()
        Me.cbxSzenario = New CoinTracer.DataBoundComboBox()
        Me.cmdSzenarioDelete = New System.Windows.Forms.Button()
        Me.cmdSzenarioSave = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.grpCalculate = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.lblTransfersOpen = New System.Windows.Forms.Label()
        Me.cmdTransfers = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.cmdCalculateGainings = New System.Windows.Forms.Button()
        Me.dtpCutOffDay = New System.Windows.Forms.DateTimePicker()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.grpGainings = New System.Windows.Forms.GroupBox()
        Me.tpGainings = New CoinTracer.TimePeriodControl()
        Me.gnd2ndTab = New CoinTracer.GainingsDisplay()
        Me.cmdReloadGainings = New System.Windows.Forms.Button()
        Me.grpSettings = New System.Windows.Forms.GroupBox()
        Me.vssGlobalStrategy = New CoinTracer.ValueStrategySelector()
        Me.cmdTCSExtended = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.cbxCoins4CoinsAccounting = New System.Windows.Forms.ComboBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cbxWalletAware = New System.Windows.Forms.ComboBox()
        Me.lblHaltefrist = New System.Windows.Forms.Label()
        Me.dpctlHaltefrist = New CoinTracer.DataPeriodControl()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.tabReports = New System.Windows.Forms.TabPage()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.grpReportAdditionalData = New System.Windows.Forms.GroupBox()
        Me.tbxReportAdvice = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbxTaxNumber = New System.Windows.Forms.TextBox()
        Me.tbxUserName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.lblSzenarioRpt = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cmdReportExport = New System.Windows.Forms.Button()
        Me.cbxReportTradeSelection = New System.Windows.Forms.ComboBox()
        Me.cmdReloadReport = New System.Windows.Forms.Button()
        Me.cbxReportTransfers = New System.Windows.Forms.ComboBox()
        Me.grdReport = New System.Windows.Forms.DataGridView()
        Me.ccbReportPlatforms = New CoinTracer.CheckComboBox.CheckedComboBox()
        Me.gnd3rdTab = New CoinTracer.GainingsDisplay()
        Me.tpReport = New CoinTracer.TimePeriodControl()
        Me.cmdReportPreview = New System.Windows.Forms.Button()
        Me.tabTable = New System.Windows.Forms.TabPage()
        Me.lblRightMouse = New System.Windows.Forms.Label()
        Me.tctlTables = New System.Windows.Forms.TabControl()
        Me.tabTrades = New System.Windows.Forms.TabPage()
        Me.grdTrades = New CoinTracer.BoundDataGridView()
        Me.cmnTrades = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiEditTrades = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabImporte = New System.Windows.Forms.TabPage()
        Me.grdImporte = New CoinTracer.BoundDataGridView()
        Me.cmnImporte = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiEraseImport = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabPlattformen = New System.Windows.Forms.TabPage()
        Me.grdPlattformen = New CoinTracer.BoundDataGridView()
        Me.cmnPlattformen = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiEditPlattformen = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiMergePlatformTrades = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabKonten = New System.Windows.Forms.TabPage()
        Me.grdKonten = New CoinTracer.BoundDataGridView()
        Me.cmnKonten = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiEditKonten = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabKurse = New System.Windows.Forms.TabPage()
        Me.grdKurse = New CoinTracer.BoundDataGridView()
        Me.cmnKurse = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiEditKurse = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabBerechnungen = New System.Windows.Forms.TabPage()
        Me.grdBerechnungen = New CoinTracer.BoundDataGridView()
        Me.cmnBerechnungen = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmiViewCalculations = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiEraseCalculation = New System.Windows.Forms.ToolStripMenuItem()
        Me.imlTables = New System.Windows.Forms.ImageList(Me.components)
        Me.imlTabs = New System.Windows.Forms.ImageList(Me.components)
        Me.pnlDonate = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cmdDonateETH = New System.Windows.Forms.Button()
        Me.cmdDonateBCH = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cmdDonateLTC = New System.Windows.Forms.Button()
        Me.cmdDonateBTC = New System.Windows.Forms.Button()
        Me.lblDonate = New System.Windows.Forms.Label()
        Me.lblDebug = New System.Windows.Forms.Label()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewAutoFilterTextBoxColumn4 = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.DataGridViewAutoFilterTextBoxColumn5 = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewAutoFilterTextBoxColumn6 = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewAutoFilterTextBoxColumn7 = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.ThreadsafeStatusStrip1 = New CoinTracer.ThreadsafeStatusStrip()
        Me.EnhancedToolTip1 = New CoinTracer.EnhancedToolTip()
        Me.Vorgang = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Timestamp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Art = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Plattform = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.TypeCoins = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.AmountCoins = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PriceUSD = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Total = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Gesamt_EUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Kurs_EUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Kaufvorgang = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.Kaufdatum = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        Me.CoinAnteil = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OrgPriceEUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.KaufkursEUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VerkaufspreisEUR = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Gaining = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TaxFree = New CoinTracer.DataGridViewAutoFilterTextBoxColumn()
        CType(Me.spltCntDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.spltCntDashboard.Panel1.SuspendLayout()
        Me.spltCntDashboard.Panel2.SuspendLayout()
        Me.spltCntDashboard.SuspendLayout()
        Me.grpBestand.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        CType(Me.spltCntGainings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.spltCntGainings.Panel1.SuspendLayout()
        Me.spltCntGainings.Panel2.SuspendLayout()
        Me.spltCntGainings.SuspendLayout()
        CType(Me.dshgrdBestaende, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dshgrdAbgaenge, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.grpDataTimes.SuspendLayout()
        CType(Me.grdDataTimes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpImport.SuspendLayout()
        Me.grpCourses.SuspendLayout()
        Me.grpApiImport.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.tctlMain.SuspendLayout()
        Me.tabDashboard.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.tabSettings.SuspendLayout()
        Me.grpSzenarien.SuspendLayout()
        Me.grpCalculate.SuspendLayout()
        Me.grpGainings.SuspendLayout()
        Me.grpSettings.SuspendLayout()
        Me.tabReports.SuspendLayout()
        Me.grpReportAdditionalData.SuspendLayout()
        CType(Me.grdReport, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabTable.SuspendLayout()
        Me.tctlTables.SuspendLayout()
        Me.tabTrades.SuspendLayout()
        CType(Me.grdTrades, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnTrades.SuspendLayout()
        Me.tabImporte.SuspendLayout()
        CType(Me.grdImporte, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnImporte.SuspendLayout()
        Me.tabPlattformen.SuspendLayout()
        CType(Me.grdPlattformen, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnPlattformen.SuspendLayout()
        Me.tabKonten.SuspendLayout()
        CType(Me.grdKonten, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnKonten.SuspendLayout()
        Me.tabKurse.SuspendLayout()
        CType(Me.grdKurse, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnKurse.SuspendLayout()
        Me.tabBerechnungen.SuspendLayout()
        CType(Me.grdBerechnungen, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmnBerechnungen.SuspendLayout()
        Me.pnlDonate.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'spltCntDashboard
        '
        resources.ApplyResources(Me.spltCntDashboard, "spltCntDashboard")
        Me.spltCntDashboard.Name = "spltCntDashboard"
        '
        'spltCntDashboard.Panel1
        '
        Me.spltCntDashboard.Panel1.Controls.Add(Me.grpBestand)
        '
        'spltCntDashboard.Panel2
        '
        Me.spltCntDashboard.Panel2.Controls.Add(Me.TableLayoutPanel2)
        '
        'grpBestand
        '
        resources.ApplyResources(Me.grpBestand, "grpBestand")
        Me.grpBestand.Controls.Add(Me.TableLayoutPanel3)
        Me.grpBestand.Name = "grpBestand"
        Me.grpBestand.TabStop = False
        '
        'TableLayoutPanel3
        '
        resources.ApplyResources(Me.TableLayoutPanel3, "TableLayoutPanel3")
        Me.TableLayoutPanel3.Controls.Add(Me.tpDashboard, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel4, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.spltCntGainings, 0, 3)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        '
        'tpDashboard
        '
        Me.tpDashboard.DateFrom = New Date(2009, 1, 1, 0, 0, 0, 0)
        Me.tpDashboard.DateTo = New Date(2015, 4, 10, 0, 0, 0, 0)
        resources.ApplyResources(Me.tpDashboard, "tpDashboard")
        Me.tpDashboard.Name = "tpDashboard"
        '
        'TableLayoutPanel4
        '
        resources.ApplyResources(Me.TableLayoutPanel4, "TableLayoutPanel4")
        Me.TableLayoutPanel4.Controls.Add(Me.gnd1stTab, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.cmdReloadDash, 1, 0)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        '
        'gnd1stTab
        '
        resources.ApplyResources(Me.gnd1stTab, "gnd1stTab")
        Me.gnd1stTab.CutOffDay = New Date(CType(0, Long))
        Me.gnd1stTab.DisableChangedEvent = False
        Me.gnd1stTab.Gainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd1stTab.Name = "gnd1stTab"
        Me.gnd1stTab.PlatformGainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd1stTab.PlatformIDs = ""
        Me.gnd1stTab.TaxableGainings = New Decimal(New Integer() {0, 0, 0, 0})
        '
        'cmdReloadDash
        '
        Me.cmdReloadDash.Image = Global.CoinTracer.My.Resources.Resources.arrow_refresh_small_24x24
        resources.ApplyResources(Me.cmdReloadDash, "cmdReloadDash")
        Me.cmdReloadDash.Name = "cmdReloadDash"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdReloadDash, resources.GetString("cmdReloadDash.ToolTip"))
        Me.cmdReloadDash.UseVisualStyleBackColor = True
        '
        'spltCntGainings
        '
        resources.ApplyResources(Me.spltCntGainings, "spltCntGainings")
        Me.spltCntGainings.Name = "spltCntGainings"
        '
        'spltCntGainings.Panel1
        '
        Me.spltCntGainings.Panel1.Controls.Add(Me.Label1)
        Me.spltCntGainings.Panel1.Controls.Add(Me.cmdTogglePlatforms)
        Me.spltCntGainings.Panel1.Controls.Add(Me.dshgrdBestaende)
        '
        'spltCntGainings.Panel2
        '
        Me.spltCntGainings.Panel2.Controls.Add(Me.dshgrdAbgaenge)
        Me.spltCntGainings.Panel2.Controls.Add(Me.Label7)
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'cmdTogglePlatforms
        '
        resources.ApplyResources(Me.cmdTogglePlatforms, "cmdTogglePlatforms")
        Me.cmdTogglePlatforms.Name = "cmdTogglePlatforms"
        Me.cmdTogglePlatforms.UseVisualStyleBackColor = True
        '
        'dshgrdBestaende
        '
        Me.dshgrdBestaende.AllowUserToAddRows = False
        Me.dshgrdBestaende.AllowUserToDeleteRows = False
        Me.dshgrdBestaende.AllowUserToResizeRows = False
        resources.ApplyResources(Me.dshgrdBestaende, "dshgrdBestaende")
        Me.dshgrdBestaende.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dshgrdBestaende.ColumnName = ""
        Me.dshgrdBestaende.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dshgrdBestaende.GroupBySQL = ""
        Me.dshgrdBestaende.Name = "dshgrdBestaende"
        Me.dshgrdBestaende.OrderBySQL = ""
        Me.dshgrdBestaende.ReadOnly = True
        Me.dshgrdBestaende.RowHeadersVisible = False
        Me.dshgrdBestaende.SelectSQL = ""
        Me.dshgrdBestaende.WhereSQL = ""
        '
        'dshgrdAbgaenge
        '
        Me.dshgrdAbgaenge.AllowUserToAddRows = False
        Me.dshgrdAbgaenge.AllowUserToDeleteRows = False
        Me.dshgrdAbgaenge.AllowUserToResizeRows = False
        resources.ApplyResources(Me.dshgrdAbgaenge, "dshgrdAbgaenge")
        Me.dshgrdAbgaenge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dshgrdAbgaenge.ColumnName = ""
        Me.dshgrdAbgaenge.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dshgrdAbgaenge.ExpensesOnly = True
        Me.dshgrdAbgaenge.GroupBySQL = ""
        Me.dshgrdAbgaenge.Name = "dshgrdAbgaenge"
        Me.dshgrdAbgaenge.OrderBySQL = ""
        Me.dshgrdAbgaenge.ReadOnly = True
        Me.dshgrdAbgaenge.RowHeadersVisible = False
        Me.dshgrdAbgaenge.SelectSQL = ""
        Me.dshgrdAbgaenge.WhereSQL = ""
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.Button1, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.grpDataTimes, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.grpImport, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.grpCourses, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.grpApiImport, 0, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'grpDataTimes
        '
        resources.ApplyResources(Me.grpDataTimes, "grpDataTimes")
        Me.grpDataTimes.Controls.Add(Me.grdDataTimes)
        Me.grpDataTimes.Name = "grpDataTimes"
        Me.grpDataTimes.TabStop = False
        '
        'grdDataTimes
        '
        Me.grdDataTimes.AllowUserToAddRows = False
        Me.grdDataTimes.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.grdDataTimes, "grdDataTimes")
        Me.grdDataTimes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdDataTimes.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Wallet, Me.Zeitpunkt})
        Me.grdDataTimes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdDataTimes.Name = "grdDataTimes"
        Me.grdDataTimes.ReadOnly = True
        Me.grdDataTimes.RowHeadersVisible = False
        '
        'Wallet
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Wallet.DefaultCellStyle = DataGridViewCellStyle1
        Me.Wallet.Frozen = True
        resources.ApplyResources(Me.Wallet, "Wallet")
        Me.Wallet.Name = "Wallet"
        Me.Wallet.ReadOnly = True
        '
        'Zeitpunkt
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Zeitpunkt.DefaultCellStyle = DataGridViewCellStyle2
        Me.Zeitpunkt.Frozen = True
        resources.ApplyResources(Me.Zeitpunkt, "Zeitpunkt")
        Me.Zeitpunkt.Name = "Zeitpunkt"
        Me.Zeitpunkt.ReadOnly = True
        '
        'grpImport
        '
        resources.ApplyResources(Me.grpImport, "grpImport")
        Me.grpImport.Controls.Add(Me.ShowHistoricImportsCheckbox)
        Me.grpImport.Controls.Add(Me.cbxImports)
        Me.grpImport.Controls.Add(Me.cmdImport)
        Me.grpImport.Name = "grpImport"
        Me.grpImport.TabStop = False
        '
        'ShowHistoricImportsCheckbox
        '
        resources.ApplyResources(Me.ShowHistoricImportsCheckbox, "ShowHistoricImportsCheckbox")
        Me.ShowHistoricImportsCheckbox.Name = "ShowHistoricImportsCheckbox"
        Me.ShowHistoricImportsCheckbox.UseVisualStyleBackColor = True
        '
        'cbxImports
        '
        resources.ApplyResources(Me.cbxImports, "cbxImports")
        Me.cbxImports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxImports.FormattingEnabled = True
        Me.cbxImports.Name = "cbxImports"
        '
        'cmdImport
        '
        resources.ApplyResources(Me.cmdImport, "cmdImport")
        Me.cmdImport.Name = "cmdImport"
        Me.cmdImport.UseVisualStyleBackColor = True
        '
        'grpCourses
        '
        Me.grpCourses.Controls.Add(Me.lblCoinCourses)
        Me.grpCourses.Controls.Add(Me.cmdCoinCourses)
        Me.grpCourses.Controls.Add(Me.lblCourseUSD)
        Me.grpCourses.Controls.Add(Me.cmdCourses)
        resources.ApplyResources(Me.grpCourses, "grpCourses")
        Me.grpCourses.Name = "grpCourses"
        Me.grpCourses.TabStop = False
        '
        'lblCoinCourses
        '
        resources.ApplyResources(Me.lblCoinCourses, "lblCoinCourses")
        Me.lblCoinCourses.Name = "lblCoinCourses"
        '
        'cmdCoinCourses
        '
        resources.ApplyResources(Me.cmdCoinCourses, "cmdCoinCourses")
        Me.cmdCoinCourses.Name = "cmdCoinCourses"
        Me.cmdCoinCourses.UseVisualStyleBackColor = True
        '
        'lblCourseUSD
        '
        resources.ApplyResources(Me.lblCourseUSD, "lblCourseUSD")
        Me.lblCourseUSD.Name = "lblCourseUSD"
        '
        'cmdCourses
        '
        resources.ApplyResources(Me.cmdCourses, "cmdCourses")
        Me.cmdCourses.Name = "cmdCourses"
        Me.cmdCourses.PaddingText = "Update € ↔ USD"
        Me.cmdCourses.UseVisualStyleBackColor = True
        '
        'grpApiImport
        '
        Me.grpApiImport.Controls.Add(Me.cmdConfigApi)
        Me.grpApiImport.Controls.Add(Me.cmdImportApi)
        resources.ApplyResources(Me.grpApiImport, "grpApiImport")
        Me.grpApiImport.Name = "grpApiImport"
        Me.grpApiImport.TabStop = False
        '
        'cmdConfigApi
        '
        resources.ApplyResources(Me.cmdConfigApi, "cmdConfigApi")
        Me.cmdConfigApi.Name = "cmdConfigApi"
        Me.cmdConfigApi.UseVisualStyleBackColor = True
        '
        'cmdImportApi
        '
        resources.ApplyResources(Me.cmdImportApi, "cmdImportApi")
        Me.cmdImportApi.Name = "cmdImportApi"
        Me.cmdImportApi.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DateiToolStripMenuItem, Me.ExtrasToolStripMenuItem, Me.HilfeToolStripMenuItem})
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Name = "MenuStrip1"
        '
        'DateiToolStripMenuItem
        '
        Me.DateiToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DatabaseSaveMenuItem, Me.DatabaseLoadMenuItem, Me.ToolStripSeparator12, Me.BeendenToolStripMenuItem})
        Me.DateiToolStripMenuItem.Name = "DateiToolStripMenuItem"
        resources.ApplyResources(Me.DateiToolStripMenuItem, "DateiToolStripMenuItem")
        '
        'DatabaseSaveMenuItem
        '
        Me.DatabaseSaveMenuItem.Image = Global.CoinTracer.My.Resources.Resources.ct_db_save
        Me.DatabaseSaveMenuItem.Name = "DatabaseSaveMenuItem"
        resources.ApplyResources(Me.DatabaseSaveMenuItem, "DatabaseSaveMenuItem")
        '
        'DatabaseLoadMenuItem
        '
        Me.DatabaseLoadMenuItem.Image = Global.CoinTracer.My.Resources.Resources.ct_db_open
        Me.DatabaseLoadMenuItem.Name = "DatabaseLoadMenuItem"
        resources.ApplyResources(Me.DatabaseLoadMenuItem, "DatabaseLoadMenuItem")
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        resources.ApplyResources(Me.ToolStripSeparator12, "ToolStripSeparator12")
        '
        'BeendenToolStripMenuItem
        '
        resources.ApplyResources(Me.BeendenToolStripMenuItem, "BeendenToolStripMenuItem")
        Me.BeendenToolStripMenuItem.Name = "BeendenToolStripMenuItem"
        '
        'ExtrasToolStripMenuItem
        '
        Me.ExtrasToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiOptions, Me.ToolStripSeparator3, Me.CleanUpDBToolStripMenuItem, Me.ToolStripSeparator1, Me.EraseDBToolStripMenuItem, Me.NewDBToolStripMenuItem})
        Me.ExtrasToolStripMenuItem.Name = "ExtrasToolStripMenuItem"
        resources.ApplyResources(Me.ExtrasToolStripMenuItem, "ExtrasToolStripMenuItem")
        '
        'tsmiOptions
        '
        Me.tsmiOptions.Image = Global.CoinTracer.My.Resources.Resources.settings_icon_16x16
        Me.tsmiOptions.Name = "tsmiOptions"
        resources.ApplyResources(Me.tsmiOptions, "tsmiOptions")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'CleanUpDBToolStripMenuItem
        '
        Me.CleanUpDBToolStripMenuItem.Image = Global.CoinTracer.My.Resources.Resources.ct_db_cleanup
        Me.CleanUpDBToolStripMenuItem.Name = "CleanUpDBToolStripMenuItem"
        resources.ApplyResources(Me.CleanUpDBToolStripMenuItem, "CleanUpDBToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'EraseDBToolStripMenuItem
        '
        Me.EraseDBToolStripMenuItem.Image = Global.CoinTracer.My.Resources.Resources.Delete_Database_icon
        Me.EraseDBToolStripMenuItem.Name = "EraseDBToolStripMenuItem"
        resources.ApplyResources(Me.EraseDBToolStripMenuItem, "EraseDBToolStripMenuItem")
        '
        'NewDBToolStripMenuItem
        '
        Me.NewDBToolStripMenuItem.Image = Global.CoinTracer.My.Resources.Resources.Delete_Database_icon
        Me.NewDBToolStripMenuItem.Name = "NewDBToolStripMenuItem"
        resources.ApplyResources(Me.NewDBToolStripMenuItem, "NewDBToolStripMenuItem")
        '
        'HilfeToolStripMenuItem
        '
        Me.HilfeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DisclaimerToolStripMenuItem, Me.InhaltToolStripMenuItem, Me.ReleaseNotesToolStripMenuItem, Me.LizenzinformationenToolStripMenuItem, Me.toolStripSeparator5, Me.CheckForUpdatesToolStripMenuItem, Me.ToolStripSeparator2, Me.InfoToolStripMenuItem})
        Me.HilfeToolStripMenuItem.Name = "HilfeToolStripMenuItem"
        resources.ApplyResources(Me.HilfeToolStripMenuItem, "HilfeToolStripMenuItem")
        '
        'DisclaimerToolStripMenuItem
        '
        Me.DisclaimerToolStripMenuItem.Name = "DisclaimerToolStripMenuItem"
        resources.ApplyResources(Me.DisclaimerToolStripMenuItem, "DisclaimerToolStripMenuItem")
        '
        'InhaltToolStripMenuItem
        '
        Me.InhaltToolStripMenuItem.Name = "InhaltToolStripMenuItem"
        resources.ApplyResources(Me.InhaltToolStripMenuItem, "InhaltToolStripMenuItem")
        '
        'ReleaseNotesToolStripMenuItem
        '
        Me.ReleaseNotesToolStripMenuItem.Name = "ReleaseNotesToolStripMenuItem"
        resources.ApplyResources(Me.ReleaseNotesToolStripMenuItem, "ReleaseNotesToolStripMenuItem")
        '
        'LizenzinformationenToolStripMenuItem
        '
        Me.LizenzinformationenToolStripMenuItem.Name = "LizenzinformationenToolStripMenuItem"
        resources.ApplyResources(Me.LizenzinformationenToolStripMenuItem, "LizenzinformationenToolStripMenuItem")
        '
        'toolStripSeparator5
        '
        Me.toolStripSeparator5.Name = "toolStripSeparator5"
        resources.ApplyResources(Me.toolStripSeparator5, "toolStripSeparator5")
        '
        'CheckForUpdatesToolStripMenuItem
        '
        Me.CheckForUpdatesToolStripMenuItem.Name = "CheckForUpdatesToolStripMenuItem"
        resources.ApplyResources(Me.CheckForUpdatesToolStripMenuItem, "CheckForUpdatesToolStripMenuItem")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'InfoToolStripMenuItem
        '
        Me.InfoToolStripMenuItem.Name = "InfoToolStripMenuItem"
        resources.ApplyResources(Me.InfoToolStripMenuItem, "InfoToolStripMenuItem")
        '
        'tctlMain
        '
        resources.ApplyResources(Me.tctlMain, "tctlMain")
        Me.tctlMain.Controls.Add(Me.tabDashboard)
        Me.tctlMain.Controls.Add(Me.tabSettings)
        Me.tctlMain.Controls.Add(Me.tabReports)
        Me.tctlMain.Controls.Add(Me.tabTable)
        Me.tctlMain.ImageList = Me.imlTabs
        Me.tctlMain.Name = "tctlMain"
        Me.tctlMain.SelectedIndex = 0
        '
        'tabDashboard
        '
        resources.ApplyResources(Me.tabDashboard, "tabDashboard")
        Me.tabDashboard.Controls.Add(Me.TableLayoutPanel1)
        Me.tabDashboard.Name = "tabDashboard"
        Me.tabDashboard.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.spltCntDashboard, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'tabSettings
        '
        Me.tabSettings.Controls.Add(Me.grpSzenarien)
        Me.tabSettings.Controls.Add(Me.grpCalculate)
        Me.tabSettings.Controls.Add(Me.grpGainings)
        Me.tabSettings.Controls.Add(Me.grpSettings)
        resources.ApplyResources(Me.tabSettings, "tabSettings")
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.UseVisualStyleBackColor = True
        '
        'grpSzenarien
        '
        Me.grpSzenarien.Controls.Add(Me.cbxSzenario)
        Me.grpSzenarien.Controls.Add(Me.cmdSzenarioDelete)
        Me.grpSzenarien.Controls.Add(Me.cmdSzenarioSave)
        Me.grpSzenarien.Controls.Add(Me.Label12)
        resources.ApplyResources(Me.grpSzenarien, "grpSzenarien")
        Me.grpSzenarien.Name = "grpSzenarien"
        Me.grpSzenarien.TabStop = False
        '
        'cbxSzenario
        '
        Me.cbxSzenario.DisplayColumnName = Nothing
        Me.cbxSzenario.FormattingEnabled = True
        Me.cbxSzenario.IDColumnName = Nothing
        Me.cbxSzenario.Initializing = True
        resources.ApplyResources(Me.cbxSzenario, "cbxSzenario")
        Me.cbxSzenario.Name = "cbxSzenario"
        Me.cbxSzenario.SelectSQL = Nothing
        Me.cbxSzenario.Sticky = False
        '
        'cmdSzenarioDelete
        '
        Me.cmdSzenarioDelete.Image = Global.CoinTracer.My.Resources.Resources.ct_erase
        resources.ApplyResources(Me.cmdSzenarioDelete, "cmdSzenarioDelete")
        Me.cmdSzenarioDelete.Name = "cmdSzenarioDelete"
        Me.cmdSzenarioDelete.UseVisualStyleBackColor = True
        '
        'cmdSzenarioSave
        '
        Me.cmdSzenarioSave.Image = Global.CoinTracer.My.Resources.Resources.ct_disk
        resources.ApplyResources(Me.cmdSzenarioSave, "cmdSzenarioSave")
        Me.cmdSzenarioSave.Name = "cmdSzenarioSave"
        Me.cmdSzenarioSave.UseVisualStyleBackColor = True
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'grpCalculate
        '
        Me.grpCalculate.Controls.Add(Me.Label15)
        Me.grpCalculate.Controls.Add(Me.lblTransfersOpen)
        Me.grpCalculate.Controls.Add(Me.cmdTransfers)
        Me.grpCalculate.Controls.Add(Me.Label11)
        Me.grpCalculate.Controls.Add(Me.cmdCalculateGainings)
        Me.grpCalculate.Controls.Add(Me.dtpCutOffDay)
        Me.grpCalculate.Controls.Add(Me.Label9)
        resources.ApplyResources(Me.grpCalculate, "grpCalculate")
        Me.grpCalculate.Name = "grpCalculate"
        Me.grpCalculate.TabStop = False
        '
        'Label15
        '
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.Name = "Label15"
        '
        'lblTransfersOpen
        '
        resources.ApplyResources(Me.lblTransfersOpen, "lblTransfersOpen")
        Me.lblTransfersOpen.Name = "lblTransfersOpen"
        '
        'cmdTransfers
        '
        resources.ApplyResources(Me.cmdTransfers, "cmdTransfers")
        Me.cmdTransfers.Name = "cmdTransfers"
        Me.cmdTransfers.UseVisualStyleBackColor = True
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'cmdCalculateGainings
        '
        resources.ApplyResources(Me.cmdCalculateGainings, "cmdCalculateGainings")
        Me.cmdCalculateGainings.Name = "cmdCalculateGainings"
        Me.cmdCalculateGainings.UseVisualStyleBackColor = True
        '
        'dtpCutOffDay
        '
        resources.ApplyResources(Me.dtpCutOffDay, "dtpCutOffDay")
        Me.dtpCutOffDay.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpCutOffDay.MaxDate = New Date(2099, 12, 31, 0, 0, 0, 0)
        Me.dtpCutOffDay.MinDate = New Date(2010, 1, 1, 0, 0, 0, 0)
        Me.dtpCutOffDay.Name = "dtpCutOffDay"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'grpGainings
        '
        Me.grpGainings.Controls.Add(Me.tpGainings)
        Me.grpGainings.Controls.Add(Me.gnd2ndTab)
        Me.grpGainings.Controls.Add(Me.cmdReloadGainings)
        resources.ApplyResources(Me.grpGainings, "grpGainings")
        Me.grpGainings.Name = "grpGainings"
        Me.grpGainings.TabStop = False
        '
        'tpGainings
        '
        Me.tpGainings.CompactAppearance = True
        Me.tpGainings.DateFrom = New Date(2009, 1, 1, 0, 0, 0, 0)
        Me.tpGainings.DateTo = New Date(2015, 4, 9, 0, 0, 0, 0)
        resources.ApplyResources(Me.tpGainings, "tpGainings")
        Me.tpGainings.Name = "tpGainings"
        '
        'gnd2ndTab
        '
        resources.ApplyResources(Me.gnd2ndTab, "gnd2ndTab")
        Me.gnd2ndTab.CutOffDay = New Date(CType(0, Long))
        Me.gnd2ndTab.DisableChangedEvent = False
        Me.gnd2ndTab.Gainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd2ndTab.Name = "gnd2ndTab"
        Me.gnd2ndTab.PlatformGainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd2ndTab.PlatformIDs = ""
        Me.gnd2ndTab.TaxableGainings = New Decimal(New Integer() {0, 0, 0, 0})
        '
        'cmdReloadGainings
        '
        Me.cmdReloadGainings.Image = Global.CoinTracer.My.Resources.Resources.arrow_refresh_small_24x24
        resources.ApplyResources(Me.cmdReloadGainings, "cmdReloadGainings")
        Me.cmdReloadGainings.Name = "cmdReloadGainings"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdReloadGainings, resources.GetString("cmdReloadGainings.ToolTip"))
        Me.cmdReloadGainings.UseVisualStyleBackColor = True
        '
        'grpSettings
        '
        resources.ApplyResources(Me.grpSettings, "grpSettings")
        Me.grpSettings.Controls.Add(Me.vssGlobalStrategy)
        Me.grpSettings.Controls.Add(Me.cmdTCSExtended)
        Me.grpSettings.Controls.Add(Me.Label17)
        Me.grpSettings.Controls.Add(Me.cbxCoins4CoinsAccounting)
        Me.grpSettings.Controls.Add(Me.Label16)
        Me.grpSettings.Controls.Add(Me.cbxWalletAware)
        Me.grpSettings.Controls.Add(Me.lblHaltefrist)
        Me.grpSettings.Controls.Add(Me.dpctlHaltefrist)
        Me.grpSettings.Controls.Add(Me.Label5)
        Me.grpSettings.Controls.Add(Me.Label18)
        Me.grpSettings.Controls.Add(Me.Label14)
        Me.grpSettings.Name = "grpSettings"
        Me.grpSettings.TabStop = False
        '
        'vssGlobalStrategy
        '
        resources.ApplyResources(Me.vssGlobalStrategy, "vssGlobalStrategy")
        Me.vssGlobalStrategy.FineTuningEnabled = True
        Me.vssGlobalStrategy.Name = "vssGlobalStrategy"
        '
        'cmdTCSExtended
        '
        resources.ApplyResources(Me.cmdTCSExtended, "cmdTCSExtended")
        Me.cmdTCSExtended.Name = "cmdTCSExtended"
        Me.cmdTCSExtended.UseVisualStyleBackColor = True
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'cbxCoins4CoinsAccounting
        '
        Me.cbxCoins4CoinsAccounting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxCoins4CoinsAccounting.FormattingEnabled = True
        Me.cbxCoins4CoinsAccounting.Items.AddRange(New Object() {resources.GetString("cbxCoins4CoinsAccounting.Items"), resources.GetString("cbxCoins4CoinsAccounting.Items1")})
        resources.ApplyResources(Me.cbxCoins4CoinsAccounting, "cbxCoins4CoinsAccounting")
        Me.cbxCoins4CoinsAccounting.Name = "cbxCoins4CoinsAccounting"
        Me.EnhancedToolTip1.SetToolTip(Me.cbxCoins4CoinsAccounting, resources.GetString("cbxCoins4CoinsAccounting.ToolTip"))
        '
        'Label16
        '
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Name = "Label16"
        '
        'cbxWalletAware
        '
        Me.cbxWalletAware.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxWalletAware.FormattingEnabled = True
        Me.cbxWalletAware.Items.AddRange(New Object() {resources.GetString("cbxWalletAware.Items"), resources.GetString("cbxWalletAware.Items1")})
        resources.ApplyResources(Me.cbxWalletAware, "cbxWalletAware")
        Me.cbxWalletAware.Name = "cbxWalletAware"
        Me.EnhancedToolTip1.SetToolTip(Me.cbxWalletAware, resources.GetString("cbxWalletAware.ToolTip"))
        '
        'lblHaltefrist
        '
        resources.ApplyResources(Me.lblHaltefrist, "lblHaltefrist")
        Me.lblHaltefrist.Name = "lblHaltefrist"
        '
        'dpctlHaltefrist
        '
        resources.ApplyResources(Me.dpctlHaltefrist, "dpctlHaltefrist")
        Me.dpctlHaltefrist.Name = "dpctlHaltefrist"
        Me.dpctlHaltefrist.VariableID = CType(1, Long)
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'Label18
        '
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Name = "Label18"
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        '
        'tabReports
        '
        Me.tabReports.Controls.Add(Me.Label13)
        Me.tabReports.Controls.Add(Me.grpReportAdditionalData)
        Me.tabReports.Controls.Add(Me.Label8)
        Me.tabReports.Controls.Add(Me.lblSzenarioRpt)
        Me.tabReports.Controls.Add(Me.Label6)
        Me.tabReports.Controls.Add(Me.cmdReportExport)
        Me.tabReports.Controls.Add(Me.cbxReportTradeSelection)
        Me.tabReports.Controls.Add(Me.cmdReloadReport)
        Me.tabReports.Controls.Add(Me.cbxReportTransfers)
        Me.tabReports.Controls.Add(Me.grdReport)
        Me.tabReports.Controls.Add(Me.ccbReportPlatforms)
        Me.tabReports.Controls.Add(Me.gnd3rdTab)
        Me.tabReports.Controls.Add(Me.tpReport)
        Me.tabReports.Controls.Add(Me.cmdReportPreview)
        resources.ApplyResources(Me.tabReports, "tabReports")
        Me.tabReports.Name = "tabReports"
        Me.tabReports.UseVisualStyleBackColor = True
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        '
        'grpReportAdditionalData
        '
        Me.grpReportAdditionalData.Controls.Add(Me.tbxReportAdvice)
        Me.grpReportAdditionalData.Controls.Add(Me.Label4)
        Me.grpReportAdditionalData.Controls.Add(Me.tbxTaxNumber)
        Me.grpReportAdditionalData.Controls.Add(Me.tbxUserName)
        Me.grpReportAdditionalData.Controls.Add(Me.Label3)
        Me.grpReportAdditionalData.Controls.Add(Me.Label2)
        resources.ApplyResources(Me.grpReportAdditionalData, "grpReportAdditionalData")
        Me.grpReportAdditionalData.Name = "grpReportAdditionalData"
        Me.grpReportAdditionalData.TabStop = False
        '
        'tbxReportAdvice
        '
        resources.ApplyResources(Me.tbxReportAdvice, "tbxReportAdvice")
        Me.tbxReportAdvice.Name = "tbxReportAdvice"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'tbxTaxNumber
        '
        resources.ApplyResources(Me.tbxTaxNumber, "tbxTaxNumber")
        Me.tbxTaxNumber.Name = "tbxTaxNumber"
        '
        'tbxUserName
        '
        resources.ApplyResources(Me.tbxUserName, "tbxUserName")
        Me.tbxUserName.Name = "tbxUserName"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'lblSzenarioRpt
        '
        resources.ApplyResources(Me.lblSzenarioRpt, "lblSzenarioRpt")
        Me.lblSzenarioRpt.Name = "lblSzenarioRpt"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'cmdReportExport
        '
        resources.ApplyResources(Me.cmdReportExport, "cmdReportExport")
        Me.cmdReportExport.Name = "cmdReportExport"
        Me.cmdReportExport.UseVisualStyleBackColor = True
        '
        'cbxReportTradeSelection
        '
        Me.cbxReportTradeSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxReportTradeSelection.FormattingEnabled = True
        Me.cbxReportTradeSelection.Items.AddRange(New Object() {resources.GetString("cbxReportTradeSelection.Items"), resources.GetString("cbxReportTradeSelection.Items1")})
        resources.ApplyResources(Me.cbxReportTradeSelection, "cbxReportTradeSelection")
        Me.cbxReportTradeSelection.Name = "cbxReportTradeSelection"
        '
        'cmdReloadReport
        '
        resources.ApplyResources(Me.cmdReloadReport, "cmdReloadReport")
        Me.cmdReloadReport.Name = "cmdReloadReport"
        Me.cmdReloadReport.UseVisualStyleBackColor = True
        '
        'cbxReportTransfers
        '
        Me.cbxReportTransfers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxReportTransfers.FormattingEnabled = True
        Me.cbxReportTransfers.Items.AddRange(New Object() {resources.GetString("cbxReportTransfers.Items"), resources.GetString("cbxReportTransfers.Items1"), resources.GetString("cbxReportTransfers.Items2")})
        resources.ApplyResources(Me.cbxReportTransfers, "cbxReportTransfers")
        Me.cbxReportTransfers.Name = "cbxReportTransfers"
        '
        'grdReport
        '
        Me.grdReport.AllowUserToAddRows = False
        Me.grdReport.AllowUserToDeleteRows = False
        Me.grdReport.AllowUserToOrderColumns = True
        resources.ApplyResources(Me.grdReport, "grdReport")
        Me.grdReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.grdReport.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Vorgang, Me.Timestamp, Me.Art, Me.Plattform, Me.TypeCoins, Me.AmountCoins, Me.PriceUSD, Me.Total, Me.Gesamt_EUR, Me.Kurs_EUR, Me.Kaufvorgang, Me.Kaufdatum, Me.CoinAnteil, Me.OrgPriceEUR, Me.KaufkursEUR, Me.VerkaufspreisEUR, Me.Gaining, Me.TaxFree})
        Me.grdReport.Name = "grdReport"
        Me.grdReport.ReadOnly = True
        Me.grdReport.RowHeadersVisible = False
        Me.grdReport.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.grdReport.ShowCellErrors = False
        '
        'ccbReportPlatforms
        '
        Me.ccbReportPlatforms.CheckOnClick = True
        Me.ccbReportPlatforms.Connection = Nothing
        Me.ccbReportPlatforms.DisplayMember = Nothing
        Me.ccbReportPlatforms.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ccbReportPlatforms.DropDownHeight = 1
        Me.ccbReportPlatforms.FirstLineIsCheckAll = False
        resources.ApplyResources(Me.ccbReportPlatforms, "ccbReportPlatforms")
        Me.ccbReportPlatforms.FormattingEnabled = True
        Me.ccbReportPlatforms.IDColumnName = Nothing
        Me.ccbReportPlatforms.Name = "ccbReportPlatforms"
        Me.ccbReportPlatforms.ReturnEmptyOnAllSelected = True
        Me.ccbReportPlatforms.SelectSQL = Nothing
        Me.ccbReportPlatforms.ValueSeparator = ", "
        '
        'gnd3rdTab
        '
        resources.ApplyResources(Me.gnd3rdTab, "gnd3rdTab")
        Me.gnd3rdTab.CutOffDay = New Date(CType(0, Long))
        Me.gnd3rdTab.DisableChangedEvent = False
        Me.gnd3rdTab.Gainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd3rdTab.Name = "gnd3rdTab"
        Me.gnd3rdTab.PlatformGainings = New Decimal(New Integer() {0, 0, 0, 0})
        Me.gnd3rdTab.PlatformIDs = ""
        Me.gnd3rdTab.ShowPlatformGainings = True
        Me.gnd3rdTab.TaxableGainings = New Decimal(New Integer() {0, 0, 0, 0})
        '
        'tpReport
        '
        Me.tpReport.CompactAppearance = True
        Me.tpReport.DateFrom = New Date(2009, 1, 1, 0, 0, 0, 0)
        Me.tpReport.DateTo = New Date(2015, 4, 9, 0, 0, 0, 0)
        resources.ApplyResources(Me.tpReport, "tpReport")
        Me.tpReport.Name = "tpReport"
        '
        'cmdReportPreview
        '
        resources.ApplyResources(Me.cmdReportPreview, "cmdReportPreview")
        Me.cmdReportPreview.Name = "cmdReportPreview"
        Me.cmdReportPreview.UseVisualStyleBackColor = True
        '
        'tabTable
        '
        Me.tabTable.Controls.Add(Me.lblRightMouse)
        Me.tabTable.Controls.Add(Me.tctlTables)
        resources.ApplyResources(Me.tabTable, "tabTable")
        Me.tabTable.Name = "tabTable"
        Me.tabTable.UseVisualStyleBackColor = True
        '
        'lblRightMouse
        '
        resources.ApplyResources(Me.lblRightMouse, "lblRightMouse")
        Me.lblRightMouse.Name = "lblRightMouse"
        '
        'tctlTables
        '
        resources.ApplyResources(Me.tctlTables, "tctlTables")
        Me.tctlTables.Controls.Add(Me.tabTrades)
        Me.tctlTables.Controls.Add(Me.tabImporte)
        Me.tctlTables.Controls.Add(Me.tabPlattformen)
        Me.tctlTables.Controls.Add(Me.tabKonten)
        Me.tctlTables.Controls.Add(Me.tabKurse)
        Me.tctlTables.Controls.Add(Me.tabBerechnungen)
        Me.tctlTables.ImageList = Me.imlTables
        Me.tctlTables.Name = "tctlTables"
        Me.tctlTables.SelectedIndex = 0
        '
        'tabTrades
        '
        Me.tabTrades.Controls.Add(Me.grdTrades)
        resources.ApplyResources(Me.tabTrades, "tabTrades")
        Me.tabTrades.Name = "tabTrades"
        Me.tabTrades.UseVisualStyleBackColor = True
        '
        'grdTrades
        '
        Me.grdTrades.AllowUserToAddRows = False
        Me.grdTrades.AllowUserToDeleteRows = False
        Me.grdTrades.AllowUserToOrderColumns = True
        Me.grdTrades.AllowUserToResizeRows = False
        Me.grdTrades.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdTrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdTrades.ContextMenuStrip = Me.cmnTrades
        resources.ApplyResources(Me.grdTrades, "grdTrades")
        Me.grdTrades.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdTrades.MultiSelect = False
        Me.grdTrades.Name = "grdTrades"
        Me.grdTrades.ReadOnly = True
        Me.grdTrades.RowHeadersVisible = False
        Me.grdTrades.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdTrades.ShowCellErrors = False
        Me.grdTrades.ShowEditingIcon = False
        Me.grdTrades.ShowRowErrors = False
        '
        'cmnTrades
        '
        Me.cmnTrades.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditTrades})
        Me.cmnTrades.Name = "cmnTrades"
        resources.ApplyResources(Me.cmnTrades, "cmnTrades")
        '
        'tsmiEditTrades
        '
        Me.tsmiEditTrades.Name = "tsmiEditTrades"
        resources.ApplyResources(Me.tsmiEditTrades, "tsmiEditTrades")
        '
        'tabImporte
        '
        Me.tabImporte.Controls.Add(Me.grdImporte)
        resources.ApplyResources(Me.tabImporte, "tabImporte")
        Me.tabImporte.Name = "tabImporte"
        Me.tabImporte.UseVisualStyleBackColor = True
        '
        'grdImporte
        '
        Me.grdImporte.AllowUserToAddRows = False
        Me.grdImporte.AllowUserToDeleteRows = False
        Me.grdImporte.AllowUserToOrderColumns = True
        Me.grdImporte.AllowUserToResizeRows = False
        Me.grdImporte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdImporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdImporte.ContextMenuStrip = Me.cmnImporte
        resources.ApplyResources(Me.grdImporte, "grdImporte")
        Me.grdImporte.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdImporte.MultiSelect = False
        Me.grdImporte.Name = "grdImporte"
        Me.grdImporte.ReadOnly = True
        Me.grdImporte.RowHeadersVisible = False
        Me.grdImporte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdImporte.ShowCellErrors = False
        Me.grdImporte.ShowEditingIcon = False
        Me.grdImporte.ShowRowErrors = False
        '
        'cmnImporte
        '
        Me.cmnImporte.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEraseImport})
        Me.cmnImporte.Name = "cmnImporte"
        resources.ApplyResources(Me.cmnImporte, "cmnImporte")
        '
        'tsmiEraseImport
        '
        Me.tsmiEraseImport.Image = Global.CoinTracer.My.Resources.Resources.undo_20x20
        Me.tsmiEraseImport.Name = "tsmiEraseImport"
        resources.ApplyResources(Me.tsmiEraseImport, "tsmiEraseImport")
        '
        'tabPlattformen
        '
        Me.tabPlattformen.Controls.Add(Me.grdPlattformen)
        resources.ApplyResources(Me.tabPlattformen, "tabPlattformen")
        Me.tabPlattformen.Name = "tabPlattformen"
        Me.tabPlattformen.UseVisualStyleBackColor = True
        '
        'grdPlattformen
        '
        Me.grdPlattformen.AllowUserToAddRows = False
        Me.grdPlattformen.AllowUserToDeleteRows = False
        Me.grdPlattformen.AllowUserToOrderColumns = True
        Me.grdPlattformen.AllowUserToResizeRows = False
        Me.grdPlattformen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdPlattformen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdPlattformen.ContextMenuStrip = Me.cmnPlattformen
        resources.ApplyResources(Me.grdPlattformen, "grdPlattformen")
        Me.grdPlattformen.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdPlattformen.MultiSelect = False
        Me.grdPlattformen.Name = "grdPlattformen"
        Me.grdPlattformen.ReadOnly = True
        Me.grdPlattformen.RowHeadersVisible = False
        Me.grdPlattformen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdPlattformen.ShowCellErrors = False
        Me.grdPlattformen.ShowEditingIcon = False
        Me.grdPlattformen.ShowRowErrors = False
        '
        'cmnPlattformen
        '
        Me.cmnPlattformen.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditPlattformen, Me.tsmiMergePlatformTrades})
        Me.cmnPlattformen.Name = "cmnTrades"
        resources.ApplyResources(Me.cmnPlattformen, "cmnPlattformen")
        '
        'tsmiEditPlattformen
        '
        Me.tsmiEditPlattformen.Name = "tsmiEditPlattformen"
        resources.ApplyResources(Me.tsmiEditPlattformen, "tsmiEditPlattformen")
        '
        'tsmiMergePlatformTrades
        '
        Me.tsmiMergePlatformTrades.Name = "tsmiMergePlatformTrades"
        resources.ApplyResources(Me.tsmiMergePlatformTrades, "tsmiMergePlatformTrades")
        '
        'tabKonten
        '
        Me.tabKonten.Controls.Add(Me.grdKonten)
        resources.ApplyResources(Me.tabKonten, "tabKonten")
        Me.tabKonten.Name = "tabKonten"
        Me.tabKonten.UseVisualStyleBackColor = True
        '
        'grdKonten
        '
        Me.grdKonten.AllowUserToAddRows = False
        Me.grdKonten.AllowUserToDeleteRows = False
        Me.grdKonten.AllowUserToOrderColumns = True
        Me.grdKonten.AllowUserToResizeRows = False
        Me.grdKonten.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdKonten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdKonten.ContextMenuStrip = Me.cmnKonten
        resources.ApplyResources(Me.grdKonten, "grdKonten")
        Me.grdKonten.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdKonten.MultiSelect = False
        Me.grdKonten.Name = "grdKonten"
        Me.grdKonten.ReadOnly = True
        Me.grdKonten.RowHeadersVisible = False
        Me.grdKonten.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdKonten.ShowCellErrors = False
        Me.grdKonten.ShowEditingIcon = False
        Me.grdKonten.ShowRowErrors = False
        '
        'cmnKonten
        '
        Me.cmnKonten.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditKonten})
        Me.cmnKonten.Name = "cmnTrades"
        resources.ApplyResources(Me.cmnKonten, "cmnKonten")
        '
        'tsmiEditKonten
        '
        Me.tsmiEditKonten.Name = "tsmiEditKonten"
        resources.ApplyResources(Me.tsmiEditKonten, "tsmiEditKonten")
        '
        'tabKurse
        '
        Me.tabKurse.Controls.Add(Me.grdKurse)
        resources.ApplyResources(Me.tabKurse, "tabKurse")
        Me.tabKurse.Name = "tabKurse"
        Me.tabKurse.UseVisualStyleBackColor = True
        '
        'grdKurse
        '
        Me.grdKurse.AllowUserToAddRows = False
        Me.grdKurse.AllowUserToDeleteRows = False
        Me.grdKurse.AllowUserToOrderColumns = True
        Me.grdKurse.AllowUserToResizeRows = False
        Me.grdKurse.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdKurse.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdKurse.ContextMenuStrip = Me.cmnKurse
        resources.ApplyResources(Me.grdKurse, "grdKurse")
        Me.grdKurse.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdKurse.MultiSelect = False
        Me.grdKurse.Name = "grdKurse"
        Me.grdKurse.ReadOnly = True
        Me.grdKurse.RowHeadersVisible = False
        Me.grdKurse.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdKurse.ShowCellErrors = False
        Me.grdKurse.ShowEditingIcon = False
        Me.grdKurse.ShowRowErrors = False
        '
        'cmnKurse
        '
        Me.cmnKurse.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditKurse})
        Me.cmnKurse.Name = "cmnTrades"
        resources.ApplyResources(Me.cmnKurse, "cmnKurse")
        '
        'tsmiEditKurse
        '
        Me.tsmiEditKurse.Name = "tsmiEditKurse"
        resources.ApplyResources(Me.tsmiEditKurse, "tsmiEditKurse")
        '
        'tabBerechnungen
        '
        Me.tabBerechnungen.Controls.Add(Me.grdBerechnungen)
        resources.ApplyResources(Me.tabBerechnungen, "tabBerechnungen")
        Me.tabBerechnungen.Name = "tabBerechnungen"
        Me.tabBerechnungen.UseVisualStyleBackColor = True
        '
        'grdBerechnungen
        '
        Me.grdBerechnungen.AllowUserToAddRows = False
        Me.grdBerechnungen.AllowUserToDeleteRows = False
        Me.grdBerechnungen.AllowUserToOrderColumns = True
        Me.grdBerechnungen.AllowUserToResizeRows = False
        Me.grdBerechnungen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.grdBerechnungen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdBerechnungen.ContextMenuStrip = Me.cmnBerechnungen
        resources.ApplyResources(Me.grdBerechnungen, "grdBerechnungen")
        Me.grdBerechnungen.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdBerechnungen.MultiSelect = False
        Me.grdBerechnungen.Name = "grdBerechnungen"
        Me.grdBerechnungen.ReadOnly = True
        Me.grdBerechnungen.RowHeadersVisible = False
        Me.grdBerechnungen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdBerechnungen.ShowCellErrors = False
        Me.grdBerechnungen.ShowEditingIcon = False
        Me.grdBerechnungen.ShowRowErrors = False
        '
        'cmnBerechnungen
        '
        Me.cmnBerechnungen.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiViewCalculations, Me.tsmiEraseCalculation})
        Me.cmnBerechnungen.Name = "cmnImporte"
        resources.ApplyResources(Me.cmnBerechnungen, "cmnBerechnungen")
        '
        'tsmiViewCalculations
        '
        Me.tsmiViewCalculations.Name = "tsmiViewCalculations"
        resources.ApplyResources(Me.tsmiViewCalculations, "tsmiViewCalculations")
        '
        'tsmiEraseCalculation
        '
        Me.tsmiEraseCalculation.Image = Global.CoinTracer.My.Resources.Resources.undo_20x20
        Me.tsmiEraseCalculation.Name = "tsmiEraseCalculation"
        resources.ApplyResources(Me.tsmiEraseCalculation, "tsmiEraseCalculation")
        '
        'imlTables
        '
        Me.imlTables.ImageStream = CType(resources.GetObject("imlTables.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlTables.TransparentColor = System.Drawing.Color.Transparent
        Me.imlTables.Images.SetKeyName(0, "tab_table.png")
        '
        'imlTabs
        '
        Me.imlTabs.ImageStream = CType(resources.GetObject("imlTabs.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlTabs.TransparentColor = System.Drawing.Color.Transparent
        Me.imlTabs.Images.SetKeyName(0, "tab_dash.png")
        Me.imlTabs.Images.SetKeyName(1, "tab_tables.png")
        Me.imlTabs.Images.SetKeyName(2, "tab_report.png")
        Me.imlTabs.Images.SetKeyName(3, "tab_settings.png")
        '
        'pnlDonate
        '
        resources.ApplyResources(Me.pnlDonate, "pnlDonate")
        Me.pnlDonate.Controls.Add(Me.GroupBox2)
        Me.pnlDonate.Name = "pnlDonate"
        '
        'GroupBox2
        '
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Controls.Add(Me.cmdDonateETH)
        Me.GroupBox2.Controls.Add(Me.cmdDonateBCH)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.cmdDonateLTC)
        Me.GroupBox2.Controls.Add(Me.cmdDonateBTC)
        Me.GroupBox2.Controls.Add(Me.lblDonate)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'cmdDonateETH
        '
        resources.ApplyResources(Me.cmdDonateETH, "cmdDonateETH")
        Me.cmdDonateETH.Name = "cmdDonateETH"
        Me.cmdDonateETH.Tag = "ETH"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdDonateETH, resources.GetString("cmdDonateETH.ToolTip"))
        Me.cmdDonateETH.UseVisualStyleBackColor = True
        '
        'cmdDonateBCH
        '
        resources.ApplyResources(Me.cmdDonateBCH, "cmdDonateBCH")
        Me.cmdDonateBCH.Image = Global.CoinTracer.My.Resources.Resources.coin_logo_bch_28px
        Me.cmdDonateBCH.Name = "cmdDonateBCH"
        Me.cmdDonateBCH.Tag = "BCH"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdDonateBCH, resources.GetString("cmdDonateBCH.ToolTip"))
        Me.cmdDonateBCH.UseVisualStyleBackColor = True
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'cmdDonateLTC
        '
        resources.ApplyResources(Me.cmdDonateLTC, "cmdDonateLTC")
        Me.cmdDonateLTC.Image = Global.CoinTracer.My.Resources.Resources.coin_logo_litecoin_28px
        Me.cmdDonateLTC.Name = "cmdDonateLTC"
        Me.cmdDonateLTC.Tag = "LTC"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdDonateLTC, resources.GetString("cmdDonateLTC.ToolTip"))
        Me.cmdDonateLTC.UseVisualStyleBackColor = True
        '
        'cmdDonateBTC
        '
        resources.ApplyResources(Me.cmdDonateBTC, "cmdDonateBTC")
        Me.cmdDonateBTC.Image = Global.CoinTracer.My.Resources.Resources.coin_logo_btc_28px
        Me.cmdDonateBTC.Name = "cmdDonateBTC"
        Me.cmdDonateBTC.Tag = "BTC"
        Me.EnhancedToolTip1.SetToolTip(Me.cmdDonateBTC, resources.GetString("cmdDonateBTC.ToolTip"))
        Me.cmdDonateBTC.UseVisualStyleBackColor = True
        '
        'lblDonate
        '
        resources.ApplyResources(Me.lblDonate, "lblDonate")
        Me.lblDonate.Name = "lblDonate"
        '
        'lblDebug
        '
        resources.ApplyResources(Me.lblDebug, "lblDebug")
        Me.lblDebug.Name = "lblDebug"
        '
        'DataGridViewTextBoxColumn1
        '
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.DataGridViewTextBoxColumn1.DefaultCellStyle = DataGridViewCellStyle14
        Me.DataGridViewTextBoxColumn1.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn1, "DataGridViewTextBoxColumn1")
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        '
        'DataGridViewTextBoxColumn2
        '
        DataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.DataGridViewTextBoxColumn2.DefaultCellStyle = DataGridViewCellStyle15
        Me.DataGridViewTextBoxColumn2.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn2, "DataGridViewTextBoxColumn2")
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        '
        'DataGridViewTextBoxColumn3
        '
        resources.ApplyResources(Me.DataGridViewTextBoxColumn3, "DataGridViewTextBoxColumn3")
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Zeitpunkt"
        resources.ApplyResources(Me.DataGridViewTextBoxColumn4, "DataGridViewTextBoxColumn4")
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        '
        'DataGridViewAutoFilterTextBoxColumn4
        '
        Me.DataGridViewAutoFilterTextBoxColumn4.CaptionAllValues = "(Alles auswählen)"
        Me.DataGridViewAutoFilterTextBoxColumn4.DataPropertyName = "Art"
        resources.ApplyResources(Me.DataGridViewAutoFilterTextBoxColumn4, "DataGridViewAutoFilterTextBoxColumn4")
        Me.DataGridViewAutoFilterTextBoxColumn4.Name = "DataGridViewAutoFilterTextBoxColumn4"
        Me.DataGridViewAutoFilterTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'DataGridViewAutoFilterTextBoxColumn5
        '
        Me.DataGridViewAutoFilterTextBoxColumn5.CaptionAllValues = "(Alles auswählen)"
        Me.DataGridViewAutoFilterTextBoxColumn5.DataPropertyName = "Plattform"
        resources.ApplyResources(Me.DataGridViewAutoFilterTextBoxColumn5, "DataGridViewAutoFilterTextBoxColumn5")
        Me.DataGridViewAutoFilterTextBoxColumn5.Name = "DataGridViewAutoFilterTextBoxColumn5"
        Me.DataGridViewAutoFilterTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.DataPropertyName = "Menge Coins"
        DataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle16.Format = "N6"
        DataGridViewCellStyle16.NullValue = Nothing
        Me.DataGridViewTextBoxColumn5.DefaultCellStyle = DataGridViewCellStyle16
        resources.ApplyResources(Me.DataGridViewTextBoxColumn5, "DataGridViewTextBoxColumn5")
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        '
        'DataGridViewAutoFilterTextBoxColumn6
        '
        Me.DataGridViewAutoFilterTextBoxColumn6.CaptionAllValues = "(Alles auswählen)"
        Me.DataGridViewAutoFilterTextBoxColumn6.DataPropertyName = "Art Coins"
        resources.ApplyResources(Me.DataGridViewAutoFilterTextBoxColumn6, "DataGridViewAutoFilterTextBoxColumn6")
        Me.DataGridViewAutoFilterTextBoxColumn6.Name = "DataGridViewAutoFilterTextBoxColumn6"
        Me.DataGridViewAutoFilterTextBoxColumn6.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'DataGridViewTextBoxColumn6
        '
        Me.DataGridViewTextBoxColumn6.DataPropertyName = "Preis USD"
        resources.ApplyResources(Me.DataGridViewTextBoxColumn6, "DataGridViewTextBoxColumn6")
        Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
        '
        'DataGridViewTextBoxColumn7
        '
        Me.DataGridViewTextBoxColumn7.DataPropertyName = "Preis EUR"
        DataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle17.Format = "N2"
        DataGridViewCellStyle17.NullValue = Nothing
        Me.DataGridViewTextBoxColumn7.DefaultCellStyle = DataGridViewCellStyle17
        resources.ApplyResources(Me.DataGridViewTextBoxColumn7, "DataGridViewTextBoxColumn7")
        Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
        '
        'DataGridViewTextBoxColumn8
        '
        Me.DataGridViewTextBoxColumn8.DataPropertyName = "Kaufdatum Coins"
        resources.ApplyResources(Me.DataGridViewTextBoxColumn8, "DataGridViewTextBoxColumn8")
        Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
        '
        'DataGridViewTextBoxColumn9
        '
        Me.DataGridViewTextBoxColumn9.DataPropertyName = "Kaufpreis EUR"
        DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle18.Format = "N2"
        Me.DataGridViewTextBoxColumn9.DefaultCellStyle = DataGridViewCellStyle18
        resources.ApplyResources(Me.DataGridViewTextBoxColumn9, "DataGridViewTextBoxColumn9")
        Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
        '
        'DataGridViewTextBoxColumn10
        '
        Me.DataGridViewTextBoxColumn10.DataPropertyName = "Gewinn EUR"
        DataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle19.Format = "N2"
        DataGridViewCellStyle19.NullValue = Nothing
        Me.DataGridViewTextBoxColumn10.DefaultCellStyle = DataGridViewCellStyle19
        resources.ApplyResources(Me.DataGridViewTextBoxColumn10, "DataGridViewTextBoxColumn10")
        Me.DataGridViewTextBoxColumn10.Name = "DataGridViewTextBoxColumn10"
        '
        'DataGridViewAutoFilterTextBoxColumn7
        '
        Me.DataGridViewAutoFilterTextBoxColumn7.CaptionAllValues = "(Alles auswählen)"
        Me.DataGridViewAutoFilterTextBoxColumn7.DataPropertyName = "Steuerfrei"
        resources.ApplyResources(Me.DataGridViewAutoFilterTextBoxColumn7, "DataGridViewAutoFilterTextBoxColumn7")
        Me.DataGridViewAutoFilterTextBoxColumn7.Name = "DataGridViewAutoFilterTextBoxColumn7"
        '
        'ThreadsafeStatusStrip1
        '
        resources.ApplyResources(Me.ThreadsafeStatusStrip1, "ThreadsafeStatusStrip1")
        Me.ThreadsafeStatusStrip1.Name = "ThreadsafeStatusStrip1"
        '
        'Vorgang
        '
        Me.Vorgang.DataPropertyName = "Vorgang"
        resources.ApplyResources(Me.Vorgang, "Vorgang")
        Me.Vorgang.Name = "Vorgang"
        Me.Vorgang.ReadOnly = True
        '
        'Timestamp
        '
        Me.Timestamp.DataPropertyName = "Zeitpunkt"
        resources.ApplyResources(Me.Timestamp, "Timestamp")
        Me.Timestamp.Name = "Timestamp"
        Me.Timestamp.ReadOnly = True
        '
        'Art
        '
        Me.Art.CaptionAllValues = "(Alles auswählen)"
        Me.Art.DataPropertyName = "Art"
        resources.ApplyResources(Me.Art, "Art")
        Me.Art.Name = "Art"
        Me.Art.ReadOnly = True
        Me.Art.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Plattform
        '
        Me.Plattform.CaptionAllValues = "(Alles auswählen)"
        Me.Plattform.DataPropertyName = "Plattform"
        resources.ApplyResources(Me.Plattform, "Plattform")
        Me.Plattform.Name = "Plattform"
        Me.Plattform.ReadOnly = True
        Me.Plattform.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'TypeCoins
        '
        Me.TypeCoins.CaptionAllValues = "(Alles auswählen)"
        Me.TypeCoins.DataPropertyName = "Coin-Art"
        Me.TypeCoins.FillWeight = 105.0!
        resources.ApplyResources(Me.TypeCoins, "TypeCoins")
        Me.TypeCoins.Name = "TypeCoins"
        Me.TypeCoins.ReadOnly = True
        Me.TypeCoins.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'AmountCoins
        '
        Me.AmountCoins.DataPropertyName = "Coin-Menge"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle3.Format = "N8"
        DataGridViewCellStyle3.NullValue = Nothing
        Me.AmountCoins.DefaultCellStyle = DataGridViewCellStyle3
        Me.AmountCoins.FillWeight = 102.0!
        resources.ApplyResources(Me.AmountCoins, "AmountCoins")
        Me.AmountCoins.Name = "AmountCoins"
        Me.AmountCoins.ReadOnly = True
        '
        'PriceUSD
        '
        Me.PriceUSD.CaptionAllValues = "(Alles auswählen)"
        Me.PriceUSD.DataPropertyName = "Zahlmittel"
        resources.ApplyResources(Me.PriceUSD, "PriceUSD")
        Me.PriceUSD.Name = "PriceUSD"
        Me.PriceUSD.ReadOnly = True
        Me.PriceUSD.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Total
        '
        Me.Total.DataPropertyName = "Gesamtpreis"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N6"
        DataGridViewCellStyle4.NullValue = Nothing
        Me.Total.DefaultCellStyle = DataGridViewCellStyle4
        resources.ApplyResources(Me.Total, "Total")
        Me.Total.Name = "Total"
        Me.Total.ReadOnly = True
        '
        'Gesamt_EUR
        '
        Me.Gesamt_EUR.DataPropertyName = "Gesamtwert EUR"
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "N2"
        DataGridViewCellStyle5.NullValue = Nothing
        Me.Gesamt_EUR.DefaultCellStyle = DataGridViewCellStyle5
        resources.ApplyResources(Me.Gesamt_EUR, "Gesamt_EUR")
        Me.Gesamt_EUR.Name = "Gesamt_EUR"
        Me.Gesamt_EUR.ReadOnly = True
        '
        'Kurs_EUR
        '
        Me.Kurs_EUR.DataPropertyName = "Kurs EUR"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle6.Format = "N2"
        DataGridViewCellStyle6.NullValue = Nothing
        Me.Kurs_EUR.DefaultCellStyle = DataGridViewCellStyle6
        resources.ApplyResources(Me.Kurs_EUR, "Kurs_EUR")
        Me.Kurs_EUR.Name = "Kurs_EUR"
        Me.Kurs_EUR.ReadOnly = True
        '
        'Kaufvorgang
        '
        Me.Kaufvorgang.CaptionAllValues = "(Alles auswählen)"
        Me.Kaufvorgang.DataPropertyName = "Vorgang Anschaffung"
        resources.ApplyResources(Me.Kaufvorgang, "Kaufvorgang")
        Me.Kaufvorgang.Name = "Kaufvorgang"
        Me.Kaufvorgang.ReadOnly = True
        Me.Kaufvorgang.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Kaufdatum
        '
        Me.Kaufdatum.CaptionAllValues = "(Alles auswählen)"
        Me.Kaufdatum.DataPropertyName = "Anschaffungsdatum"
        DataGridViewCellStyle7.Format = "d"
        DataGridViewCellStyle7.NullValue = "-"
        Me.Kaufdatum.DefaultCellStyle = DataGridViewCellStyle7
        resources.ApplyResources(Me.Kaufdatum, "Kaufdatum")
        Me.Kaufdatum.Name = "Kaufdatum"
        Me.Kaufdatum.ReadOnly = True
        '
        'CoinAnteil
        '
        Me.CoinAnteil.DataPropertyName = "Coin-Anteil"
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "N8"
        DataGridViewCellStyle8.NullValue = Nothing
        Me.CoinAnteil.DefaultCellStyle = DataGridViewCellStyle8
        resources.ApplyResources(Me.CoinAnteil, "CoinAnteil")
        Me.CoinAnteil.Name = "CoinAnteil"
        Me.CoinAnteil.ReadOnly = True
        '
        'OrgPriceEUR
        '
        Me.OrgPriceEUR.DataPropertyName = "Kaufpreis EUR"
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle9.Format = "N2"
        Me.OrgPriceEUR.DefaultCellStyle = DataGridViewCellStyle9
        resources.ApplyResources(Me.OrgPriceEUR, "OrgPriceEUR")
        Me.OrgPriceEUR.Name = "OrgPriceEUR"
        Me.OrgPriceEUR.ReadOnly = True
        '
        'KaufkursEUR
        '
        Me.KaufkursEUR.DataPropertyName = "Kaufkurs EUR"
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle10.Format = "N2"
        DataGridViewCellStyle10.NullValue = Nothing
        Me.KaufkursEUR.DefaultCellStyle = DataGridViewCellStyle10
        resources.ApplyResources(Me.KaufkursEUR, "KaufkursEUR")
        Me.KaufkursEUR.Name = "KaufkursEUR"
        Me.KaufkursEUR.ReadOnly = True
        '
        'VerkaufspreisEUR
        '
        Me.VerkaufspreisEUR.DataPropertyName = "Verkaufspreis EUR"
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle11.Format = "N2"
        DataGridViewCellStyle11.NullValue = Nothing
        Me.VerkaufspreisEUR.DefaultCellStyle = DataGridViewCellStyle11
        Me.VerkaufspreisEUR.FillWeight = 120.0!
        resources.ApplyResources(Me.VerkaufspreisEUR, "VerkaufspreisEUR")
        Me.VerkaufspreisEUR.Name = "VerkaufspreisEUR"
        Me.VerkaufspreisEUR.ReadOnly = True
        '
        'Gaining
        '
        Me.Gaining.DataPropertyName = "Gewinn EUR"
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle12.Format = "N2"
        DataGridViewCellStyle12.NullValue = Nothing
        Me.Gaining.DefaultCellStyle = DataGridViewCellStyle12
        resources.ApplyResources(Me.Gaining, "Gaining")
        Me.Gaining.Name = "Gaining"
        Me.Gaining.ReadOnly = True
        '
        'TaxFree
        '
        Me.TaxFree.CaptionAllValues = "(Alles auswählen)"
        Me.TaxFree.DataPropertyName = "Steuerfrei"
        DataGridViewCellStyle13.NullValue = "-"
        Me.TaxFree.DefaultCellStyle = DataGridViewCellStyle13
        resources.ApplyResources(Me.TaxFree, "TaxFree")
        Me.TaxFree.Name = "TaxFree"
        Me.TaxFree.ReadOnly = True
        '
        'frmMain
        '
        Me.AllowDrop = True
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ThreadsafeStatusStrip1)
        Me.Controls.Add(Me.pnlDonate)
        Me.Controls.Add(Me.tctlMain)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmMain"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.spltCntDashboard.Panel1.ResumeLayout(False)
        Me.spltCntDashboard.Panel2.ResumeLayout(False)
        CType(Me.spltCntDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.spltCntDashboard.ResumeLayout(False)
        Me.grpBestand.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.TableLayoutPanel4.PerformLayout()
        Me.spltCntGainings.Panel1.ResumeLayout(False)
        Me.spltCntGainings.Panel1.PerformLayout()
        Me.spltCntGainings.Panel2.ResumeLayout(False)
        Me.spltCntGainings.Panel2.PerformLayout()
        CType(Me.spltCntGainings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.spltCntGainings.ResumeLayout(False)
        CType(Me.dshgrdBestaende, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dshgrdAbgaenge, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.grpDataTimes.ResumeLayout(False)
        CType(Me.grdDataTimes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpImport.ResumeLayout(False)
        Me.grpImport.PerformLayout()
        Me.grpCourses.ResumeLayout(False)
        Me.grpApiImport.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.tctlMain.ResumeLayout(False)
        Me.tabDashboard.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.tabSettings.ResumeLayout(False)
        Me.grpSzenarien.ResumeLayout(False)
        Me.grpSzenarien.PerformLayout()
        Me.grpCalculate.ResumeLayout(False)
        Me.grpCalculate.PerformLayout()
        Me.grpGainings.ResumeLayout(False)
        Me.grpGainings.PerformLayout()
        Me.grpSettings.ResumeLayout(False)
        Me.grpSettings.PerformLayout()
        Me.tabReports.ResumeLayout(False)
        Me.tabReports.PerformLayout()
        Me.grpReportAdditionalData.ResumeLayout(False)
        Me.grpReportAdditionalData.PerformLayout()
        CType(Me.grdReport, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabTable.ResumeLayout(False)
        Me.tabTable.PerformLayout()
        Me.tctlTables.ResumeLayout(False)
        Me.tabTrades.ResumeLayout(False)
        CType(Me.grdTrades, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnTrades.ResumeLayout(False)
        Me.tabImporte.ResumeLayout(False)
        CType(Me.grdImporte, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnImporte.ResumeLayout(False)
        Me.tabPlattformen.ResumeLayout(False)
        CType(Me.grdPlattformen, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnPlattformen.ResumeLayout(False)
        Me.tabKonten.ResumeLayout(False)
        CType(Me.grdKonten, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnKonten.ResumeLayout(False)
        Me.tabKurse.ResumeLayout(False)
        CType(Me.grdKurse, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnKurse.ResumeLayout(False)
        Me.tabBerechnungen.ResumeLayout(False)
        CType(Me.grdBerechnungen, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmnBerechnungen.ResumeLayout(False)
        Me.pnlDonate.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents spltCntGainings As System.Windows.Forms.SplitContainer
    Friend WithEvents spltCntDashboard As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents grpDataTimes As System.Windows.Forms.GroupBox
    Friend WithEvents grdDataTimes As System.Windows.Forms.DataGridView
    Friend WithEvents Wallet As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Zeitpunkt As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents grpImport As System.Windows.Forms.GroupBox
    Friend WithEvents cbxImports As System.Windows.Forms.ComboBox
    Friend WithEvents cmdImport As System.Windows.Forms.Button
    Friend WithEvents grpCourses As System.Windows.Forms.GroupBox
    Friend WithEvents lblCoinCourses As System.Windows.Forms.Label
    Friend WithEvents cmdCoinCourses As System.Windows.Forms.Button
    Friend WithEvents lblCourseUSD As System.Windows.Forms.Label
    Friend WithEvents cmdCourses As CoinTracer.PaddingButton
    Friend WithEvents grpApiImport As System.Windows.Forms.GroupBox
    Friend WithEvents cmdConfigApi As System.Windows.Forms.Button
    Friend WithEvents cmdImportApi As System.Windows.Forms.Button
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn4 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn5 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn6 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn10 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewAutoFilterTextBoxColumn7 As CoinTracer.DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents ShowHistoricImportsCheckbox As CheckBox
    Friend WithEvents cmdDonateBCH As Button
    Friend WithEvents cmdDonateETH As Button
    Friend WithEvents tabBerechnungen As TabPage
    Friend WithEvents grdBerechnungen As BoundDataGridView
    Friend WithEvents cmnBerechnungen As ContextMenuStrip
    Friend WithEvents tsmiEraseCalculation As ToolStripMenuItem
    Friend WithEvents tsmiViewCalculations As ToolStripMenuItem
    Friend WithEvents NewDBToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LizenzinformationenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cmdTCSExtended As Button
    Friend WithEvents Label17 As Label
    Friend WithEvents cbxCoins4CoinsAccounting As ComboBox
    Friend WithEvents Label16 As Label
    Friend WithEvents cbxWalletAware As ComboBox
    Friend WithEvents lblHaltefrist As Label
    Friend WithEvents dpctlHaltefrist As DataPeriodControl
    Friend WithEvents Label5 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents vssGlobalStrategy As ValueStrategySelector
    Friend WithEvents tsmiMergePlatformTrades As ToolStripMenuItem
    Friend WithEvents Vorgang As DataGridViewTextBoxColumn
    Friend WithEvents Timestamp As DataGridViewTextBoxColumn
    Friend WithEvents Art As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Plattform As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents TypeCoins As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents AmountCoins As DataGridViewTextBoxColumn
    Friend WithEvents PriceUSD As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Total As DataGridViewTextBoxColumn
    Friend WithEvents Gesamt_EUR As DataGridViewTextBoxColumn
    Friend WithEvents Kurs_EUR As DataGridViewTextBoxColumn
    Friend WithEvents Kaufvorgang As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents Kaufdatum As DataGridViewAutoFilterTextBoxColumn
    Friend WithEvents CoinAnteil As DataGridViewTextBoxColumn
    Friend WithEvents OrgPriceEUR As DataGridViewTextBoxColumn
    Friend WithEvents KaufkursEUR As DataGridViewTextBoxColumn
    Friend WithEvents VerkaufspreisEUR As DataGridViewTextBoxColumn
    Friend WithEvents Gaining As DataGridViewTextBoxColumn
    Friend WithEvents TaxFree As DataGridViewAutoFilterTextBoxColumn
End Class

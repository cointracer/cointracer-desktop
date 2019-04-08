<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TradeCoreDataControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            If _TradesTa IsNot Nothing Then
                _TradesTa.Dispose()
            End If
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
        Dim IDLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TradeCoreDataControl))
        Dim SourceIDLabel As System.Windows.Forms.Label
        Dim ImportPlattformIDLabel As System.Windows.Forms.Label
        Dim ZeitpunktLabel As System.Windows.Forms.Label
        Dim TradeTypIDLabel As System.Windows.Forms.Label
        Dim QuellBetragLabel As System.Windows.Forms.Label
        Dim QuellKontoIDLabel As System.Windows.Forms.Label
        Dim ZielBetragLabel As System.Windows.Forms.Label
        Dim ZielKontoIDLabel As System.Windows.Forms.Label
        Dim WertEURLabel As System.Windows.Forms.Label
        Dim BetragNachGebuehrLabel As System.Windows.Forms.Label
        Dim ZeitpunktZielLabel As System.Windows.Forms.Label
        Dim InfoLabel As System.Windows.Forms.Label
        Dim SteuerIrrelevantLabel As System.Windows.Forms.Label
        Dim QuellBetragNachGebuehrLabel As System.Windows.Forms.Label
        Dim SourceKommentarLabel As System.Windows.Forms.Label
        Dim InZeitpunktLabel As System.Windows.Forms.Label
        Me.QuellPlattformIDLabel = New System.Windows.Forms.Label()
        Me.ZielPlattformIDLabel = New System.Windows.Forms.Label()
        Me.TradesBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.TradesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CoinTracerDataSet = New CoinTracer.CoinTracerDataSet()
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.TradesBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.IDTextBox = New CoinTracer.TouchedTextBox()
        Me.SourceIDTextBox = New CoinTracer.TouchedTextBox()
        Me.ZeitpunktDateTimePicker = New CoinTracer.TouchedDateTimePicker()
        Me.QuellBetragTextBox = New CoinTracer.TouchedTextBox()
        Me.ZielBetragTextBox = New CoinTracer.TouchedTextBox()
        Me.WertEURTextBox = New CoinTracer.TouchedTextBox()
        Me.BetragNachGebuehrTextBox = New CoinTracer.TouchedTextBox()
        Me.ZeitpunktZielDateTimePicker = New CoinTracer.TouchedDateTimePicker()
        Me.InfoTextBox = New CoinTracer.TouchedTextBox()
        Me.ImportPlattformIDComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesPlattformenImportBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TradeTypIDComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesTradeTypenBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.QuellPlattformIDComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesPlattformenQuellBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ZielPlattformIDComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesPlattformenZielBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.QuellKontoComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesKontenQuellBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ZielKontoComboBox = New CoinTracer.TouchedComboBox()
        Me.TradesKontenZielBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.grpVon = New System.Windows.Forms.GroupBox()
        Me.QuellBetragNachGebuehrTextBox = New CoinTracer.TouchedTextBox()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.grpTo = New System.Windows.Forms.GroupBox()
        Me.InZeitpunktDateTimePicker = New CoinTracer.TouchedDateTimePicker()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.SteuerIrrelevantCheckBox = New System.Windows.Forms.CheckBox()
        Me.ErrProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.pnlTrade = New System.Windows.Forms.Panel()
        Me.SourceKommentarTextBox = New CoinTracer.TouchedTextBox()
        Me.TradeTypenTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.TradeTypenTableAdapter()
        Me.TradesTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.TradesTableAdapter()
        Me.TableAdapterManager = New CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager()
        IDLabel = New System.Windows.Forms.Label()
        SourceIDLabel = New System.Windows.Forms.Label()
        ImportPlattformIDLabel = New System.Windows.Forms.Label()
        ZeitpunktLabel = New System.Windows.Forms.Label()
        TradeTypIDLabel = New System.Windows.Forms.Label()
        QuellBetragLabel = New System.Windows.Forms.Label()
        QuellKontoIDLabel = New System.Windows.Forms.Label()
        ZielBetragLabel = New System.Windows.Forms.Label()
        ZielKontoIDLabel = New System.Windows.Forms.Label()
        WertEURLabel = New System.Windows.Forms.Label()
        BetragNachGebuehrLabel = New System.Windows.Forms.Label()
        ZeitpunktZielLabel = New System.Windows.Forms.Label()
        InfoLabel = New System.Windows.Forms.Label()
        SteuerIrrelevantLabel = New System.Windows.Forms.Label()
        QuellBetragNachGebuehrLabel = New System.Windows.Forms.Label()
        SourceKommentarLabel = New System.Windows.Forms.Label()
        InZeitpunktLabel = New System.Windows.Forms.Label()
        CType(Me.TradesBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TradesBindingNavigator.SuspendLayout()
        CType(Me.TradesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesPlattformenImportBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesTradeTypenBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesPlattformenQuellBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesPlattformenZielBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesKontenQuellBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradesKontenZielBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpVon.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpTo.SuspendLayout()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTrade.SuspendLayout()
        Me.SuspendLayout()
        '
        'IDLabel
        '
        resources.ApplyResources(IDLabel, "IDLabel")
        IDLabel.Name = "IDLabel"
        '
        'SourceIDLabel
        '
        resources.ApplyResources(SourceIDLabel, "SourceIDLabel")
        SourceIDLabel.Name = "SourceIDLabel"
        '
        'ImportPlattformIDLabel
        '
        resources.ApplyResources(ImportPlattformIDLabel, "ImportPlattformIDLabel")
        ImportPlattformIDLabel.Name = "ImportPlattformIDLabel"
        '
        'ZeitpunktLabel
        '
        resources.ApplyResources(ZeitpunktLabel, "ZeitpunktLabel")
        ZeitpunktLabel.Name = "ZeitpunktLabel"
        '
        'TradeTypIDLabel
        '
        resources.ApplyResources(TradeTypIDLabel, "TradeTypIDLabel")
        TradeTypIDLabel.Name = "TradeTypIDLabel"
        '
        'QuellPlattformIDLabel
        '
        resources.ApplyResources(Me.QuellPlattformIDLabel, "QuellPlattformIDLabel")
        Me.QuellPlattformIDLabel.Name = "QuellPlattformIDLabel"
        '
        'QuellBetragLabel
        '
        resources.ApplyResources(QuellBetragLabel, "QuellBetragLabel")
        QuellBetragLabel.Name = "QuellBetragLabel"
        '
        'QuellKontoIDLabel
        '
        resources.ApplyResources(QuellKontoIDLabel, "QuellKontoIDLabel")
        QuellKontoIDLabel.Name = "QuellKontoIDLabel"
        '
        'ZielPlattformIDLabel
        '
        resources.ApplyResources(Me.ZielPlattformIDLabel, "ZielPlattformIDLabel")
        Me.ZielPlattformIDLabel.Name = "ZielPlattformIDLabel"
        '
        'ZielBetragLabel
        '
        resources.ApplyResources(ZielBetragLabel, "ZielBetragLabel")
        ZielBetragLabel.Name = "ZielBetragLabel"
        '
        'ZielKontoIDLabel
        '
        resources.ApplyResources(ZielKontoIDLabel, "ZielKontoIDLabel")
        ZielKontoIDLabel.Name = "ZielKontoIDLabel"
        '
        'WertEURLabel
        '
        resources.ApplyResources(WertEURLabel, "WertEURLabel")
        WertEURLabel.Name = "WertEURLabel"
        '
        'BetragNachGebuehrLabel
        '
        resources.ApplyResources(BetragNachGebuehrLabel, "BetragNachGebuehrLabel")
        BetragNachGebuehrLabel.Name = "BetragNachGebuehrLabel"
        '
        'ZeitpunktZielLabel
        '
        resources.ApplyResources(ZeitpunktZielLabel, "ZeitpunktZielLabel")
        ZeitpunktZielLabel.Name = "ZeitpunktZielLabel"
        '
        'InfoLabel
        '
        resources.ApplyResources(InfoLabel, "InfoLabel")
        InfoLabel.Name = "InfoLabel"
        '
        'SteuerIrrelevantLabel
        '
        resources.ApplyResources(SteuerIrrelevantLabel, "SteuerIrrelevantLabel")
        SteuerIrrelevantLabel.Name = "SteuerIrrelevantLabel"
        '
        'QuellBetragNachGebuehrLabel
        '
        resources.ApplyResources(QuellBetragNachGebuehrLabel, "QuellBetragNachGebuehrLabel")
        QuellBetragNachGebuehrLabel.Name = "QuellBetragNachGebuehrLabel"
        '
        'SourceKommentarLabel
        '
        resources.ApplyResources(SourceKommentarLabel, "SourceKommentarLabel")
        SourceKommentarLabel.Name = "SourceKommentarLabel"
        '
        'InZeitpunktLabel
        '
        resources.ApplyResources(InZeitpunktLabel, "InZeitpunktLabel")
        InZeitpunktLabel.Name = "InZeitpunktLabel"
        '
        'TradesBindingNavigator
        '
        Me.TradesBindingNavigator.AddNewItem = Nothing
        resources.ApplyResources(Me.TradesBindingNavigator, "TradesBindingNavigator")
        Me.TradesBindingNavigator.BindingSource = Me.TradesBindingSource
        Me.TradesBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.TradesBindingNavigator.CountItemFormat = "of {0}"
        Me.TradesBindingNavigator.DeleteItem = Nothing
        Me.TradesBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.TradesBindingNavigatorSaveItem})
        Me.TradesBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.TradesBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.TradesBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.TradesBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.TradesBindingNavigator.Name = "TradesBindingNavigator"
        Me.TradesBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        '
        'TradesBindingSource
        '
        Me.TradesBindingSource.DataMember = "Trades"
        Me.TradesBindingSource.DataSource = Me.CoinTracerDataSet
        '
        'CoinTracerDataSet
        '
        Me.CoinTracerDataSet.DataSetName = "CoinTracerDataSet"
        Me.CoinTracerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        resources.ApplyResources(Me.BindingNavigatorCountItem, "BindingNavigatorCountItem")
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorMoveFirstItem, "BindingNavigatorMoveFirstItem")
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorMovePreviousItem, "BindingNavigatorMovePreviousItem")
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        resources.ApplyResources(Me.BindingNavigatorSeparator, "BindingNavigatorSeparator")
        '
        'BindingNavigatorPositionItem
        '
        resources.ApplyResources(Me.BindingNavigatorPositionItem, "BindingNavigatorPositionItem")
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        resources.ApplyResources(Me.BindingNavigatorSeparator1, "BindingNavigatorSeparator1")
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorMoveNextItem, "BindingNavigatorMoveNextItem")
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorMoveLastItem, "BindingNavigatorMoveLastItem")
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        resources.ApplyResources(Me.BindingNavigatorSeparator2, "BindingNavigatorSeparator2")
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorAddNewItem, "BindingNavigatorAddNewItem")
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorDeleteItem, "BindingNavigatorDeleteItem")
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        '
        'TradesBindingNavigatorSaveItem
        '
        Me.TradesBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.TradesBindingNavigatorSaveItem, "TradesBindingNavigatorSaveItem")
        Me.TradesBindingNavigatorSaveItem.Name = "TradesBindingNavigatorSaveItem"
        '
        'IDTextBox
        '
        Me.IDTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "ID", True))
        resources.ApplyResources(Me.IDTextBox, "IDTextBox")
        Me.IDTextBox.Name = "IDTextBox"
        Me.IDTextBox.ReadOnly = True
        Me.IDTextBox.Touched = False
        '
        'SourceIDTextBox
        '
        resources.ApplyResources(Me.SourceIDTextBox, "SourceIDTextBox")
        Me.SourceIDTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "SourceID", True))
        Me.SourceIDTextBox.Name = "SourceIDTextBox"
        Me.SourceIDTextBox.Touched = False
        '
        'ZeitpunktDateTimePicker
        '
        resources.ApplyResources(Me.ZeitpunktDateTimePicker, "ZeitpunktDateTimePicker")
        Me.ZeitpunktDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.TradesBindingSource, "Zeitpunkt", True))
        Me.ZeitpunktDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.ZeitpunktDateTimePicker.Name = "ZeitpunktDateTimePicker"
        Me.ZeitpunktDateTimePicker.Touched = False
        '
        'QuellBetragTextBox
        '
        Me.QuellBetragTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "QuellBetrag", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "#,###,##0.00000000"))
        resources.ApplyResources(Me.QuellBetragTextBox, "QuellBetragTextBox")
        Me.QuellBetragTextBox.Name = "QuellBetragTextBox"
        Me.QuellBetragTextBox.Touched = False
        '
        'ZielBetragTextBox
        '
        Me.ZielBetragTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "ZielBetrag", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "#,###,##0.00000000"))
        resources.ApplyResources(Me.ZielBetragTextBox, "ZielBetragTextBox")
        Me.ZielBetragTextBox.Name = "ZielBetragTextBox"
        Me.ZielBetragTextBox.Touched = False
        '
        'WertEURTextBox
        '
        Me.WertEURTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "WertEUR", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "#,###,##0.00000000"))
        resources.ApplyResources(Me.WertEURTextBox, "WertEURTextBox")
        Me.WertEURTextBox.Name = "WertEURTextBox"
        Me.WertEURTextBox.Touched = False
        '
        'BetragNachGebuehrTextBox
        '
        Me.BetragNachGebuehrTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "BetragNachGebuehr", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "#,###,##0.00000000"))
        resources.ApplyResources(Me.BetragNachGebuehrTextBox, "BetragNachGebuehrTextBox")
        Me.BetragNachGebuehrTextBox.Name = "BetragNachGebuehrTextBox"
        Me.BetragNachGebuehrTextBox.Touched = False
        '
        'ZeitpunktZielDateTimePicker
        '
        resources.ApplyResources(Me.ZeitpunktZielDateTimePicker, "ZeitpunktZielDateTimePicker")
        Me.ZeitpunktZielDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.TradesBindingSource, "ZeitpunktZiel", True))
        Me.ZeitpunktZielDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.ZeitpunktZielDateTimePicker.Name = "ZeitpunktZielDateTimePicker"
        Me.ZeitpunktZielDateTimePicker.Touched = False
        '
        'InfoTextBox
        '
        resources.ApplyResources(Me.InfoTextBox, "InfoTextBox")
        Me.InfoTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "Info", True))
        Me.InfoTextBox.Name = "InfoTextBox"
        Me.InfoTextBox.Touched = False
        '
        'ImportPlattformIDComboBox
        '
        Me.ImportPlattformIDComboBox.DataSource = Me.TradesPlattformenImportBindingSource
        Me.ImportPlattformIDComboBox.DisplayMember = "Bezeichnung"
        Me.ImportPlattformIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ImportPlattformIDComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.ImportPlattformIDComboBox, "ImportPlattformIDComboBox")
        Me.ImportPlattformIDComboBox.Name = "ImportPlattformIDComboBox"
        Me.ImportPlattformIDComboBox.Touched = False
        Me.ImportPlattformIDComboBox.ValueMember = "ID"
        '
        'TradesPlattformenImportBindingSource
        '
        Me.TradesPlattformenImportBindingSource.DataSource = Me.TradesBindingSource
        Me.TradesPlattformenImportBindingSource.Sort = ""
        '
        'TradeTypIDComboBox
        '
        Me.TradeTypIDComboBox.DataSource = Me.TradesTradeTypenBindingSource
        Me.TradeTypIDComboBox.DisplayMember = "Bezeichnung"
        Me.TradeTypIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TradeTypIDComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.TradeTypIDComboBox, "TradeTypIDComboBox")
        Me.TradeTypIDComboBox.Name = "TradeTypIDComboBox"
        Me.TradeTypIDComboBox.Touched = False
        Me.TradeTypIDComboBox.ValueMember = "ID"
        '
        'TradesTradeTypenBindingSource
        '
        Me.TradesTradeTypenBindingSource.DataSource = Me.TradesBindingSource
        Me.TradesTradeTypenBindingSource.Sort = ""
        '
        'QuellPlattformIDComboBox
        '
        Me.QuellPlattformIDComboBox.BackColor = System.Drawing.SystemColors.Window
        Me.QuellPlattformIDComboBox.DataSource = Me.TradesPlattformenQuellBindingSource
        Me.QuellPlattformIDComboBox.DisplayMember = "Bezeichnung"
        Me.QuellPlattformIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.QuellPlattformIDComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.QuellPlattformIDComboBox, "QuellPlattformIDComboBox")
        Me.QuellPlattformIDComboBox.Name = "QuellPlattformIDComboBox"
        Me.QuellPlattformIDComboBox.Touched = False
        Me.QuellPlattformIDComboBox.ValueMember = "ID"
        '
        'TradesPlattformenQuellBindingSource
        '
        Me.TradesPlattformenQuellBindingSource.DataMember = "Plattformen"
        Me.TradesPlattformenQuellBindingSource.DataSource = Me.CoinTracerDataSet
        Me.TradesPlattformenQuellBindingSource.Sort = "SortID"
        '
        'ZielPlattformIDComboBox
        '
        Me.ZielPlattformIDComboBox.DataSource = Me.TradesPlattformenZielBindingSource
        Me.ZielPlattformIDComboBox.DisplayMember = "Bezeichnung"
        Me.ZielPlattformIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ZielPlattformIDComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.ZielPlattformIDComboBox, "ZielPlattformIDComboBox")
        Me.ZielPlattformIDComboBox.Name = "ZielPlattformIDComboBox"
        Me.ZielPlattformIDComboBox.Touched = False
        Me.ZielPlattformIDComboBox.ValueMember = "ID"
        '
        'TradesPlattformenZielBindingSource
        '
        Me.TradesPlattformenZielBindingSource.DataSource = Me.TradesBindingSource
        Me.TradesPlattformenZielBindingSource.Sort = ""
        '
        'QuellKontoComboBox
        '
        Me.QuellKontoComboBox.DataSource = Me.TradesKontenQuellBindingSource
        Me.QuellKontoComboBox.DisplayMember = "Bezeichnung"
        Me.QuellKontoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.QuellKontoComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.QuellKontoComboBox, "QuellKontoComboBox")
        Me.QuellKontoComboBox.Name = "QuellKontoComboBox"
        Me.QuellKontoComboBox.Touched = False
        Me.QuellKontoComboBox.ValueMember = "ID"
        '
        'TradesKontenQuellBindingSource
        '
        Me.TradesKontenQuellBindingSource.DataSource = Me.TradesBindingSource
        Me.TradesKontenQuellBindingSource.Sort = ""
        '
        'ZielKontoComboBox
        '
        Me.ZielKontoComboBox.DataSource = Me.TradesKontenZielBindingSource
        Me.ZielKontoComboBox.DisplayMember = "Bezeichnung"
        Me.ZielKontoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ZielKontoComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.ZielKontoComboBox, "ZielKontoComboBox")
        Me.ZielKontoComboBox.Name = "ZielKontoComboBox"
        Me.ZielKontoComboBox.Touched = False
        Me.ZielKontoComboBox.ValueMember = "ID"
        '
        'TradesKontenZielBindingSource
        '
        Me.TradesKontenZielBindingSource.DataSource = Me.TradesBindingSource
        Me.TradesKontenZielBindingSource.Sort = ""
        '
        'grpVon
        '
        resources.ApplyResources(Me.grpVon, "grpVon")
        Me.grpVon.Controls.Add(QuellBetragNachGebuehrLabel)
        Me.grpVon.Controls.Add(Me.QuellBetragNachGebuehrTextBox)
        Me.grpVon.Controls.Add(Me.lblFrom)
        Me.grpVon.Controls.Add(Me.QuellKontoComboBox)
        Me.grpVon.Controls.Add(Me.QuellPlattformIDComboBox)
        Me.grpVon.Controls.Add(QuellKontoIDLabel)
        Me.grpVon.Controls.Add(QuellBetragLabel)
        Me.grpVon.Controls.Add(Me.QuellBetragTextBox)
        Me.grpVon.Controls.Add(Me.QuellPlattformIDLabel)
        Me.grpVon.Controls.Add(ZeitpunktLabel)
        Me.grpVon.Controls.Add(Me.ZeitpunktDateTimePicker)
        Me.grpVon.Name = "grpVon"
        Me.grpVon.TabStop = False
        '
        'QuellBetragNachGebuehrTextBox
        '
        Me.QuellBetragNachGebuehrTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "QuellBetragNachGebuehr", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "#,###,##0.00000000"))
        resources.ApplyResources(Me.QuellBetragNachGebuehrTextBox, "QuellBetragNachGebuehrTextBox")
        Me.QuellBetragNachGebuehrTextBox.Name = "QuellBetragNachGebuehrTextBox"
        Me.QuellBetragNachGebuehrTextBox.Touched = False
        '
        'lblFrom
        '
        resources.ApplyResources(Me.lblFrom, "lblFrom")
        Me.lblFrom.Name = "lblFrom"
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'grpTo
        '
        resources.ApplyResources(Me.grpTo, "grpTo")
        Me.grpTo.Controls.Add(InZeitpunktLabel)
        Me.grpTo.Controls.Add(Me.InZeitpunktDateTimePicker)
        Me.grpTo.Controls.Add(Me.lblTo)
        Me.grpTo.Controls.Add(Me.ZielKontoComboBox)
        Me.grpTo.Controls.Add(Me.ZielPlattformIDComboBox)
        Me.grpTo.Controls.Add(ZeitpunktZielLabel)
        Me.grpTo.Controls.Add(Me.ZeitpunktZielDateTimePicker)
        Me.grpTo.Controls.Add(BetragNachGebuehrLabel)
        Me.grpTo.Controls.Add(Me.BetragNachGebuehrTextBox)
        Me.grpTo.Controls.Add(WertEURLabel)
        Me.grpTo.Controls.Add(Me.WertEURTextBox)
        Me.grpTo.Controls.Add(ZielKontoIDLabel)
        Me.grpTo.Controls.Add(ZielBetragLabel)
        Me.grpTo.Controls.Add(Me.ZielBetragTextBox)
        Me.grpTo.Controls.Add(Me.ZielPlattformIDLabel)
        Me.grpTo.Name = "grpTo"
        Me.grpTo.TabStop = False
        '
        'InZeitpunktDateTimePicker
        '
        resources.ApplyResources(Me.InZeitpunktDateTimePicker, "InZeitpunktDateTimePicker")
        Me.InZeitpunktDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.TradesBindingSource, "InZeitpunkt", True))
        Me.InZeitpunktDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.InZeitpunktDateTimePicker.Name = "InZeitpunktDateTimePicker"
        Me.InZeitpunktDateTimePicker.Touched = False
        '
        'lblTo
        '
        resources.ApplyResources(Me.lblTo, "lblTo")
        Me.lblTo.Name = "lblTo"
        '
        'SteuerIrrelevantCheckBox
        '
        resources.ApplyResources(Me.SteuerIrrelevantCheckBox, "SteuerIrrelevantCheckBox")
        Me.SteuerIrrelevantCheckBox.DataBindings.Add(New System.Windows.Forms.Binding("CheckState", Me.TradesBindingSource, "Steuerirrelevant", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "0"))
        Me.SteuerIrrelevantCheckBox.Name = "SteuerIrrelevantCheckBox"
        Me.SteuerIrrelevantCheckBox.UseVisualStyleBackColor = True
        '
        'ErrProvider
        '
        Me.ErrProvider.ContainerControl = Me
        '
        'pnlTrade
        '
        resources.ApplyResources(Me.pnlTrade, "pnlTrade")
        Me.pnlTrade.Controls.Add(SourceKommentarLabel)
        Me.pnlTrade.Controls.Add(Me.SourceKommentarTextBox)
        Me.pnlTrade.Controls.Add(SteuerIrrelevantLabel)
        Me.pnlTrade.Controls.Add(Me.SteuerIrrelevantCheckBox)
        Me.pnlTrade.Controls.Add(Me.grpTo)
        Me.pnlTrade.Controls.Add(Me.PictureBox1)
        Me.pnlTrade.Controls.Add(Me.grpVon)
        Me.pnlTrade.Controls.Add(Me.TradeTypIDComboBox)
        Me.pnlTrade.Controls.Add(Me.ImportPlattformIDComboBox)
        Me.pnlTrade.Controls.Add(InfoLabel)
        Me.pnlTrade.Controls.Add(Me.InfoTextBox)
        Me.pnlTrade.Controls.Add(TradeTypIDLabel)
        Me.pnlTrade.Controls.Add(ImportPlattformIDLabel)
        Me.pnlTrade.Controls.Add(SourceIDLabel)
        Me.pnlTrade.Controls.Add(Me.SourceIDTextBox)
        Me.pnlTrade.Controls.Add(IDLabel)
        Me.pnlTrade.Controls.Add(Me.IDTextBox)
        Me.pnlTrade.Name = "pnlTrade"
        '
        'SourceKommentarTextBox
        '
        resources.ApplyResources(Me.SourceKommentarTextBox, "SourceKommentarTextBox")
        Me.SourceKommentarTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.TradesBindingSource, "Kommentar", True))
        Me.SourceKommentarTextBox.Name = "SourceKommentarTextBox"
        Me.SourceKommentarTextBox.Touched = False
        '
        'TradeTypenTableAdapter
        '
        Me.TradeTypenTableAdapter.ClearBeforeFill = True
        '
        'TradesTableAdapter
        '
        Me.TradesTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager._VersionsTableAdapter = Nothing
        Me.TableAdapterManager.ApiDatenTableAdapter = Nothing
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.BestaendeTableAdapter = Nothing
        Me.TableAdapterManager.ImporteTableAdapter = Nothing
        Me.TableAdapterManager.KalkulationenTableAdapter = Nothing
        Me.TableAdapterManager.KonfigurationTableAdapter = Nothing
        Me.TableAdapterManager.KontenTableAdapter = Nothing
        Me.TableAdapterManager.KurseTableAdapter = Nothing
        Me.TableAdapterManager.Out2InTableAdapter = Nothing
        Me.TableAdapterManager.PlattformenTableAdapter = Nothing
        Me.TableAdapterManager.SzenarienTableAdapter = Nothing
        Me.TableAdapterManager.TradesTableAdapter = Me.TradesTableAdapter
        Me.TableAdapterManager.TradesWerteTableAdapter = Nothing
        Me.TableAdapterManager.TradeTypenTableAdapter = Me.TradeTypenTableAdapter
        Me.TableAdapterManager.UpdateOrder = CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.ZeitstempelWerteTableAdapter = Nothing
        '
        'TradeCoreDataControl
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlTrade)
        Me.Controls.Add(Me.TradesBindingNavigator)
        Me.Name = "TradeCoreDataControl"
        CType(Me.TradesBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TradesBindingNavigator.ResumeLayout(False)
        Me.TradesBindingNavigator.PerformLayout()
        CType(Me.TradesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesPlattformenImportBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesTradeTypenBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesPlattformenQuellBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesPlattformenZielBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesKontenQuellBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradesKontenZielBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpVon.ResumeLayout(False)
        Me.grpVon.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpTo.ResumeLayout(False)
        Me.grpTo.PerformLayout()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTrade.ResumeLayout(False)
        Me.pnlTrade.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TradesBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TradesBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents IDTextBox As Cointracer.TouchedTextBox
    Friend WithEvents SourceIDTextBox As Cointracer.TouchedTextBox
    Friend WithEvents ZeitpunktDateTimePicker As Cointracer.TouchedDateTimePicker
    Friend WithEvents QuellBetragTextBox As Cointracer.TouchedTextBox
    Friend WithEvents ZielBetragTextBox As Cointracer.TouchedTextBox
    Friend WithEvents WertEURTextBox As Cointracer.TouchedTextBox
    Friend WithEvents BetragNachGebuehrTextBox As Cointracer.TouchedTextBox
    Friend WithEvents ZeitpunktZielDateTimePicker As Cointracer.TouchedDateTimePicker
    Friend WithEvents InfoTextBox As Cointracer.TouchedTextBox
    Friend WithEvents TradesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CoinTracerDataSet As CoinTracer.CoinTracerDataSet
    Friend WithEvents ImportPlattformIDComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradeTypIDComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradesPlattformenImportBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents QuellPlattformIDComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradesPlattformenQuellBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ZielPlattformIDComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradesPlattformenZielBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TradesTradeTypenBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents QuellKontoComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradesKontenQuellBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ZielKontoComboBox As Cointracer.TouchedComboBox
    Friend WithEvents TradesKontenZielBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents grpVon As System.Windows.Forms.GroupBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents grpTo As System.Windows.Forms.GroupBox
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents SteuerIrrelevantCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents QuellBetragNachGebuehrTextBox As Cointracer.TouchedTextBox
    Friend WithEvents ErrProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents pnlTrade As System.Windows.Forms.Panel
    Friend WithEvents TradeTypenTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.TradeTypenTableAdapter
    Friend WithEvents TradesTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.TradesTableAdapter
    Friend WithEvents SourceKommentarTextBox As Cointracer.TouchedTextBox
    Friend WithEvents TableAdapterManager As CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager
    Friend WithEvents InZeitpunktDateTimePicker As TouchedDateTimePicker
    Friend WithEvents QuellPlattformIDLabel As Label
    Friend WithEvents ZielPlattformIDLabel As Label
End Class

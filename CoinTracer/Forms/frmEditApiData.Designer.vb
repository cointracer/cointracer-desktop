<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditApiData
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
        Dim IDLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditApiData))
        Dim ZeitpunktLabel As System.Windows.Forms.Label
        Dim AktivLabel As System.Windows.Forms.Label
        Dim ImportPlattformIDLabel As System.Windows.Forms.Label
        Dim BezeichnungLabel As System.Windows.Forms.Label
        Dim ApiKeyLabel As System.Windows.Forms.Label
        Dim ApiSecretLabel As System.Windows.Forms.Label
        Dim LastImportTimestampLabel As System.Windows.Forms.Label
        Me.CallDelayLabel = New System.Windows.Forms.Label()
        Me.lblCurrencies = New System.Windows.Forms.Label()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.ApiDatenBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.ApiDatenBindingSource = New System.Windows.Forms.BindingSource(Me.components)
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
        Me.IDTextBox = New System.Windows.Forms.TextBox()
        Me.pnlDetails = New System.Windows.Forms.Panel()
        Me.CallDelayNumericUpDown = New CoinTracer.TouchedNumericUpDown(Me.components)
        Me.ccbBitfinexCurrencies = New CoinTracer.CheckComboBox.CheckedComboBox()
        Me.LastImportTimestampDateTimePicker = New System.Windows.Forms.DateTimePicker()
        Me.ApiSecretDecryptedTextBox = New System.Windows.Forms.TextBox()
        Me.ApiKeyDecryptedTextBox = New System.Windows.Forms.TextBox()
        Me.ApiSecretTextBox = New System.Windows.Forms.TextBox()
        Me.ApiKeyTextBox = New System.Windows.Forms.TextBox()
        Me.BezeichnungTextBox = New System.Windows.Forms.TextBox()
        Me.PlattformIDComboBox = New System.Windows.Forms.ComboBox()
        Me.ZeitpunktTextBox = New System.Windows.Forms.TextBox()
        Me.AktivCheckBox = New System.Windows.Forms.CheckBox()
        Me.LastImportTimestampTextBox = New System.Windows.Forms.TextBox()
        Me.lblBitfinex = New System.Windows.Forms.Label()
        Me.lblKraken = New System.Windows.Forms.Label()
        Me.ExtendedInfoTextBox = New System.Windows.Forms.TextBox()
        Me.lblBitcoinDe = New System.Windows.Forms.Label()
        Me.lblHinweise = New System.Windows.Forms.Label()
        Me.ErrProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.pnlEditTrades = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TableAdapterManager = New CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager()
        Me.ApiDatenTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.ApiDatenTableAdapter()
        Me.ApiPlattformenBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ApiPlattformenTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.ApiPlattformenTableAdapter()
        IDLabel = New System.Windows.Forms.Label()
        ZeitpunktLabel = New System.Windows.Forms.Label()
        AktivLabel = New System.Windows.Forms.Label()
        ImportPlattformIDLabel = New System.Windows.Forms.Label()
        BezeichnungLabel = New System.Windows.Forms.Label()
        ApiKeyLabel = New System.Windows.Forms.Label()
        ApiSecretLabel = New System.Windows.Forms.Label()
        LastImportTimestampLabel = New System.Windows.Forms.Label()
        CType(Me.ApiDatenBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ApiDatenBindingNavigator.SuspendLayout()
        CType(Me.ApiDatenBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlDetails.SuspendLayout()
        CType(Me.CallDelayNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEditTrades.SuspendLayout()
        CType(Me.ApiPlattformenBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'IDLabel
        '
        resources.ApplyResources(IDLabel, "IDLabel")
        IDLabel.Name = "IDLabel"
        '
        'ZeitpunktLabel
        '
        resources.ApplyResources(ZeitpunktLabel, "ZeitpunktLabel")
        ZeitpunktLabel.Name = "ZeitpunktLabel"
        '
        'AktivLabel
        '
        resources.ApplyResources(AktivLabel, "AktivLabel")
        AktivLabel.Name = "AktivLabel"
        Me.ToolTip1.SetToolTip(AktivLabel, resources.GetString("AktivLabel.ToolTip"))
        '
        'ImportPlattformIDLabel
        '
        resources.ApplyResources(ImportPlattformIDLabel, "ImportPlattformIDLabel")
        ImportPlattformIDLabel.Name = "ImportPlattformIDLabel"
        '
        'BezeichnungLabel
        '
        resources.ApplyResources(BezeichnungLabel, "BezeichnungLabel")
        BezeichnungLabel.Name = "BezeichnungLabel"
        '
        'ApiKeyLabel
        '
        resources.ApplyResources(ApiKeyLabel, "ApiKeyLabel")
        ApiKeyLabel.Name = "ApiKeyLabel"
        '
        'ApiSecretLabel
        '
        resources.ApplyResources(ApiSecretLabel, "ApiSecretLabel")
        ApiSecretLabel.Name = "ApiSecretLabel"
        '
        'LastImportTimestampLabel
        '
        resources.ApplyResources(LastImportTimestampLabel, "LastImportTimestampLabel")
        LastImportTimestampLabel.Name = "LastImportTimestampLabel"
        '
        'CallDelayLabel
        '
        resources.ApplyResources(Me.CallDelayLabel, "CallDelayLabel")
        Me.CallDelayLabel.Name = "CallDelayLabel"
        '
        'lblCurrencies
        '
        resources.ApplyResources(Me.lblCurrencies, "lblCurrencies")
        Me.lblCurrencies.Name = "lblCurrencies"
        Me.ToolTip1.SetToolTip(Me.lblCurrencies, resources.GetString("lblCurrencies.ToolTip"))
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
        'ApiDatenBindingNavigator
        '
        Me.ApiDatenBindingNavigator.AddNewItem = Nothing
        Me.ApiDatenBindingNavigator.BindingSource = Me.ApiDatenBindingSource
        Me.ApiDatenBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.ApiDatenBindingNavigator.CountItemFormat = "of {0}"
        Me.ApiDatenBindingNavigator.DeleteItem = Nothing
        Me.ApiDatenBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem})
        resources.ApplyResources(Me.ApiDatenBindingNavigator, "ApiDatenBindingNavigator")
        Me.ApiDatenBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.ApiDatenBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.ApiDatenBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.ApiDatenBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.ApiDatenBindingNavigator.Name = "ApiDatenBindingNavigator"
        Me.ApiDatenBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        '
        'ApiDatenBindingSource
        '
        Me.ApiDatenBindingSource.DataMember = "ApiDaten"
        Me.ApiDatenBindingSource.DataSource = Me.CoinTracerDataSet
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
        'IDTextBox
        '
        Me.IDTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "ID", True))
        resources.ApplyResources(Me.IDTextBox, "IDTextBox")
        Me.IDTextBox.Name = "IDTextBox"
        Me.IDTextBox.ReadOnly = True
        Me.IDTextBox.TabStop = False
        '
        'pnlDetails
        '
        resources.ApplyResources(Me.pnlDetails, "pnlDetails")
        Me.pnlDetails.Controls.Add(Me.CallDelayNumericUpDown)
        Me.pnlDetails.Controls.Add(Me.CallDelayLabel)
        Me.pnlDetails.Controls.Add(Me.ccbBitfinexCurrencies)
        Me.pnlDetails.Controls.Add(Me.lblCurrencies)
        Me.pnlDetails.Controls.Add(Me.LastImportTimestampDateTimePicker)
        Me.pnlDetails.Controls.Add(LastImportTimestampLabel)
        Me.pnlDetails.Controls.Add(Me.ApiSecretDecryptedTextBox)
        Me.pnlDetails.Controls.Add(Me.ApiKeyDecryptedTextBox)
        Me.pnlDetails.Controls.Add(ApiSecretLabel)
        Me.pnlDetails.Controls.Add(Me.ApiSecretTextBox)
        Me.pnlDetails.Controls.Add(ApiKeyLabel)
        Me.pnlDetails.Controls.Add(Me.ApiKeyTextBox)
        Me.pnlDetails.Controls.Add(BezeichnungLabel)
        Me.pnlDetails.Controls.Add(Me.BezeichnungTextBox)
        Me.pnlDetails.Controls.Add(Me.PlattformIDComboBox)
        Me.pnlDetails.Controls.Add(ImportPlattformIDLabel)
        Me.pnlDetails.Controls.Add(Me.ZeitpunktTextBox)
        Me.pnlDetails.Controls.Add(Me.AktivCheckBox)
        Me.pnlDetails.Controls.Add(IDLabel)
        Me.pnlDetails.Controls.Add(Me.IDTextBox)
        Me.pnlDetails.Controls.Add(ZeitpunktLabel)
        Me.pnlDetails.Controls.Add(AktivLabel)
        Me.pnlDetails.Controls.Add(Me.LastImportTimestampTextBox)
        Me.pnlDetails.Controls.Add(Me.lblBitfinex)
        Me.pnlDetails.Controls.Add(Me.lblKraken)
        Me.pnlDetails.Controls.Add(Me.ExtendedInfoTextBox)
        Me.pnlDetails.Controls.Add(Me.lblBitcoinDe)
        Me.pnlDetails.Controls.Add(Me.lblHinweise)
        Me.pnlDetails.Name = "pnlDetails"
        '
        'CallDelayNumericUpDown
        '
        resources.ApplyResources(Me.CallDelayNumericUpDown, "CallDelayNumericUpDown")
        Me.CallDelayNumericUpDown.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ApiDatenBindingSource, "CallDelay", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "N0"))
        Me.CallDelayNumericUpDown.Increment = New Decimal(New Integer() {50, 0, 0, 0})
        Me.CallDelayNumericUpDown.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.CallDelayNumericUpDown.Name = "CallDelayNumericUpDown"
        Me.CallDelayNumericUpDown.Touched = False
        '
        'ccbBitfinexCurrencies
        '
        Me.ccbBitfinexCurrencies.CheckOnClick = True
        Me.ccbBitfinexCurrencies.Connection = Nothing
        Me.ccbBitfinexCurrencies.DisplayMember = "Text"
        Me.ccbBitfinexCurrencies.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.ccbBitfinexCurrencies.DropDownHeight = 1
        Me.ccbBitfinexCurrencies.FirstLineIsCheckAll = False
        Me.ccbBitfinexCurrencies.FormattingEnabled = True
        Me.ccbBitfinexCurrencies.IDColumnName = "Value"
        resources.ApplyResources(Me.ccbBitfinexCurrencies, "ccbBitfinexCurrencies")
        Me.ccbBitfinexCurrencies.Name = "ccbBitfinexCurrencies"
        Me.ccbBitfinexCurrencies.SelectSQL = Nothing
        Me.ccbBitfinexCurrencies.ValueMember = "Value"
        Me.ccbBitfinexCurrencies.ValueSeparator = ", "
        '
        'LastImportTimestampDateTimePicker
        '
        resources.ApplyResources(Me.LastImportTimestampDateTimePicker, "LastImportTimestampDateTimePicker")
        Me.LastImportTimestampDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.LastImportTimestampDateTimePicker.Name = "LastImportTimestampDateTimePicker"
        '
        'ApiSecretDecryptedTextBox
        '
        resources.ApplyResources(Me.ApiSecretDecryptedTextBox, "ApiSecretDecryptedTextBox")
        Me.ApiSecretDecryptedTextBox.Name = "ApiSecretDecryptedTextBox"
        '
        'ApiKeyDecryptedTextBox
        '
        resources.ApplyResources(Me.ApiKeyDecryptedTextBox, "ApiKeyDecryptedTextBox")
        Me.ApiKeyDecryptedTextBox.Name = "ApiKeyDecryptedTextBox"
        '
        'ApiSecretTextBox
        '
        resources.ApplyResources(Me.ApiSecretTextBox, "ApiSecretTextBox")
        Me.ApiSecretTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "ApiSecret", True))
        Me.ApiSecretTextBox.Name = "ApiSecretTextBox"
        Me.ApiSecretTextBox.TabStop = False
        '
        'ApiKeyTextBox
        '
        resources.ApplyResources(Me.ApiKeyTextBox, "ApiKeyTextBox")
        Me.ApiKeyTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "ApiKey", True))
        Me.ApiKeyTextBox.Name = "ApiKeyTextBox"
        Me.ApiKeyTextBox.TabStop = False
        '
        'BezeichnungTextBox
        '
        resources.ApplyResources(Me.BezeichnungTextBox, "BezeichnungTextBox")
        Me.BezeichnungTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "Bezeichnung", True))
        Me.BezeichnungTextBox.Name = "BezeichnungTextBox"
        '
        'PlattformIDComboBox
        '
        Me.PlattformIDComboBox.DataSource = Me.ApiPlattformenBindingSource
        Me.PlattformIDComboBox.DisplayMember = "Bezeichnung"
        Me.PlattformIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.PlattformIDComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.PlattformIDComboBox, "PlattformIDComboBox")
        Me.PlattformIDComboBox.Name = "PlattformIDComboBox"
        Me.PlattformIDComboBox.ValueMember = "ID"
        '
        'ZeitpunktTextBox
        '
        resources.ApplyResources(Me.ZeitpunktTextBox, "ZeitpunktTextBox")
        Me.ZeitpunktTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "Zeitpunkt", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "G"))
        Me.ZeitpunktTextBox.Name = "ZeitpunktTextBox"
        Me.ZeitpunktTextBox.ReadOnly = True
        Me.ZeitpunktTextBox.TabStop = False
        '
        'AktivCheckBox
        '
        Me.AktivCheckBox.DataBindings.Add(New System.Windows.Forms.Binding("CheckState", Me.ApiDatenBindingSource, "Aktiv", True))
        resources.ApplyResources(Me.AktivCheckBox, "AktivCheckBox")
        Me.AktivCheckBox.Name = "AktivCheckBox"
        Me.ToolTip1.SetToolTip(Me.AktivCheckBox, resources.GetString("AktivCheckBox.ToolTip"))
        Me.AktivCheckBox.UseVisualStyleBackColor = True
        '
        'LastImportTimestampTextBox
        '
        Me.LastImportTimestampTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "LastImportTimestamp", True))
        resources.ApplyResources(Me.LastImportTimestampTextBox, "LastImportTimestampTextBox")
        Me.LastImportTimestampTextBox.Name = "LastImportTimestampTextBox"
        Me.LastImportTimestampTextBox.TabStop = False
        '
        'lblBitfinex
        '
        resources.ApplyResources(Me.lblBitfinex, "lblBitfinex")
        Me.lblBitfinex.ForeColor = System.Drawing.Color.MidnightBlue
        Me.lblBitfinex.Name = "lblBitfinex"
        '
        'lblKraken
        '
        resources.ApplyResources(Me.lblKraken, "lblKraken")
        Me.lblKraken.ForeColor = System.Drawing.Color.MidnightBlue
        Me.lblKraken.Name = "lblKraken"
        '
        'ExtendedInfoTextBox
        '
        Me.ExtendedInfoTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ApiDatenBindingSource, "ExtendedInfo", True))
        resources.ApplyResources(Me.ExtendedInfoTextBox, "ExtendedInfoTextBox")
        Me.ExtendedInfoTextBox.Name = "ExtendedInfoTextBox"
        Me.ExtendedInfoTextBox.TabStop = False
        '
        'lblBitcoinDe
        '
        resources.ApplyResources(Me.lblBitcoinDe, "lblBitcoinDe")
        Me.lblBitcoinDe.ForeColor = System.Drawing.Color.MidnightBlue
        Me.lblBitcoinDe.Name = "lblBitcoinDe"
        '
        'lblHinweise
        '
        resources.ApplyResources(Me.lblHinweise, "lblHinweise")
        Me.lblHinweise.ForeColor = System.Drawing.Color.MidnightBlue
        Me.lblHinweise.Name = "lblHinweise"
        '
        'ErrProvider
        '
        Me.ErrProvider.ContainerControl = Me
        '
        'pnlEditTrades
        '
        Me.pnlEditTrades.Controls.Add(Me.Label3)
        Me.pnlEditTrades.Controls.Add(Me.Label4)
        resources.ApplyResources(Me.pnlEditTrades, "pnlEditTrades")
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
        'TableAdapterManager
        '
        Me.TableAdapterManager._VersionsTableAdapter = Nothing
        Me.TableAdapterManager.ApiDatenTableAdapter = Me.ApiDatenTableAdapter
        Me.TableAdapterManager.ApiPlattformenTableAdapter = Nothing
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.BestaendeTableAdapter = Nothing
        Me.TableAdapterManager.ImporteTableAdapter = Nothing
        Me.TableAdapterManager.KalkulationenTableAdapter = Nothing
        Me.TableAdapterManager.KonfigurationTableAdapter = Nothing
        Me.TableAdapterManager.KontenAliasesTableAdapter = Nothing
        Me.TableAdapterManager.KontenTableAdapter = Nothing
        Me.TableAdapterManager.KurseTableAdapter = Nothing
        Me.TableAdapterManager.PlattformenTableAdapter = Nothing
        Me.TableAdapterManager.SzenarienTableAdapter = Nothing
        Me.TableAdapterManager.TradesTableAdapter = Nothing
        Me.TableAdapterManager.TradesWerteTableAdapter = Nothing
        Me.TableAdapterManager.TradeTxTableAdapter = Nothing
        Me.TableAdapterManager.TradeTypenTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        '
        'ApiDatenTableAdapter
        '
        Me.ApiDatenTableAdapter.ClearBeforeFill = True
        '
        'ApiPlattformenBindingSource
        '
        Me.ApiPlattformenBindingSource.DataMember = "ApiPlattformen"
        Me.ApiPlattformenBindingSource.DataSource = Me.CoinTracerDataSet
        '
        'ApiPlattformenTableAdapter
        '
        Me.ApiPlattformenTableAdapter.ClearBeforeFill = True
        '
        'frmEditApiData
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.pnlDetails)
        Me.Controls.Add(Me.ApiDatenBindingNavigator)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.pnlEditTrades)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditApiData"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.ApiDatenBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ApiDatenBindingNavigator.ResumeLayout(False)
        Me.ApiDatenBindingNavigator.PerformLayout()
        CType(Me.ApiDatenBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlDetails.ResumeLayout(False)
        Me.pnlDetails.PerformLayout()
        CType(Me.CallDelayNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEditTrades.ResumeLayout(False)
        Me.pnlEditTrades.PerformLayout()
        CType(Me.ApiPlattformenBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents CoinTracerDataSet As CoinTracer.CoinTracerDataSet
    Friend WithEvents ApiDatenBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TableAdapterManager As CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager
    Friend WithEvents ApiDatenBindingNavigator As System.Windows.Forms.BindingNavigator
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
    Friend WithEvents IDTextBox As System.Windows.Forms.TextBox
    Friend WithEvents pnlDetails As System.Windows.Forms.Panel
    Friend WithEvents ErrProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents AktivCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ApiDatenTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.ApiDatenTableAdapter
    Friend WithEvents pnlEditTrades As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ZeitpunktTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PlattformIDComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents ApiSecretTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ApiKeyTextBox As System.Windows.Forms.TextBox
    Friend WithEvents BezeichnungTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ApiKeyDecryptedTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ApiSecretDecryptedTextBox As System.Windows.Forms.TextBox
    Friend WithEvents lblHinweise As System.Windows.Forms.Label
    Friend WithEvents lblBitcoinDe As System.Windows.Forms.Label
    Friend WithEvents lblKraken As System.Windows.Forms.Label
    Friend WithEvents LastImportTimestampTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LastImportTimestampDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblCurrencies As System.Windows.Forms.Label
    Friend WithEvents lblBitfinex As System.Windows.Forms.Label
    Friend WithEvents ExtendedInfoTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ccbBitfinexCurrencies As CheckComboBox.CheckedComboBox
    Friend WithEvents CallDelayLabel As Label
    Friend WithEvents CallDelayNumericUpDown As TouchedNumericUpDown
    Friend WithEvents ApiPlattformenBindingSource As BindingSource
    Friend WithEvents ApiPlattformenTableAdapter As CoinTracerDataSetTableAdapters.ApiPlattformenTableAdapter
End Class

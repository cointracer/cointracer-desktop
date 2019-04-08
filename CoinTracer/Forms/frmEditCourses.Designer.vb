<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditCourses
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditCourses))
        Dim ZeitpunktLabel As System.Windows.Forms.Label
        Dim QuellBetragLabel As System.Windows.Forms.Label
        Dim CalculatedLabel As System.Windows.Forms.Label
        Dim QuellKontoIDLabel As System.Windows.Forms.Label
        Dim ZielBetragLabel As System.Windows.Forms.Label
        Dim ZielKontoIDLabel As System.Windows.Forms.Label
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.KurseBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.KurseBindingSource = New System.Windows.Forms.BindingSource(Me.components)
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
        Me.ZeitpunktDateTimePicker = New System.Windows.Forms.DateTimePicker()
        Me.QuellBetragTextBox = New System.Windows.Forms.TextBox()
        Me.QuellKontoComboBox = New System.Windows.Forms.ComboBox()
        Me.KurseKontenQuellBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ZielBetragTextBox = New System.Windows.Forms.TextBox()
        Me.ZielKontoComboBox = New System.Windows.Forms.ComboBox()
        Me.KurseKontenZielBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.KurseTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.KurseTableAdapter()
        Me.TableAdapterManager = New CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager()
        Me.KontenTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.KontenTableAdapter()
        Me.pnlDetails = New System.Windows.Forms.Panel()
        Me.CalculatedCheckBox = New System.Windows.Forms.CheckBox()
        Me.ErrProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        IDLabel = New System.Windows.Forms.Label()
        ZeitpunktLabel = New System.Windows.Forms.Label()
        QuellBetragLabel = New System.Windows.Forms.Label()
        CalculatedLabel = New System.Windows.Forms.Label()
        QuellKontoIDLabel = New System.Windows.Forms.Label()
        ZielBetragLabel = New System.Windows.Forms.Label()
        ZielKontoIDLabel = New System.Windows.Forms.Label()
        CType(Me.KurseBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.KurseBindingNavigator.SuspendLayout()
        CType(Me.KurseBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.KurseKontenQuellBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.KurseKontenZielBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlDetails.SuspendLayout()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'QuellBetragLabel
        '
        resources.ApplyResources(QuellBetragLabel, "QuellBetragLabel")
        QuellBetragLabel.Name = "QuellBetragLabel"
        '
        'CalculatedLabel
        '
        resources.ApplyResources(CalculatedLabel, "CalculatedLabel")
        CalculatedLabel.Name = "CalculatedLabel"
        Me.ToolTip1.SetToolTip(CalculatedLabel, resources.GetString("CalculatedLabel.ToolTip"))
        '
        'QuellKontoIDLabel
        '
        resources.ApplyResources(QuellKontoIDLabel, "QuellKontoIDLabel")
        QuellKontoIDLabel.Name = "QuellKontoIDLabel"
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
        'KurseBindingNavigator
        '
        Me.KurseBindingNavigator.AddNewItem = Nothing
        Me.KurseBindingNavigator.BindingSource = Me.KurseBindingSource
        Me.KurseBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.KurseBindingNavigator.CountItemFormat = "of {0}"
        Me.KurseBindingNavigator.DeleteItem = Nothing
        Me.KurseBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem})
        resources.ApplyResources(Me.KurseBindingNavigator, "KurseBindingNavigator")
        Me.KurseBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.KurseBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.KurseBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.KurseBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.KurseBindingNavigator.Name = "KurseBindingNavigator"
        Me.KurseBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        '
        'KurseBindingSource
        '
        Me.KurseBindingSource.DataMember = "Kurse"
        Me.KurseBindingSource.DataSource = Me.CoinTracerDataSet
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
        Me.IDTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.KurseBindingSource, "ID", True))
        resources.ApplyResources(Me.IDTextBox, "IDTextBox")
        Me.IDTextBox.Name = "IDTextBox"
        '
        'ZeitpunktDateTimePicker
        '
        resources.ApplyResources(Me.ZeitpunktDateTimePicker, "ZeitpunktDateTimePicker")
        Me.ZeitpunktDateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.KurseBindingSource, "Zeitpunkt", True))
        Me.ZeitpunktDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.ZeitpunktDateTimePicker.Name = "ZeitpunktDateTimePicker"
        '
        'QuellBetragTextBox
        '
        Me.QuellBetragTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.KurseBindingSource, "QuellBetrag", True))
        resources.ApplyResources(Me.QuellBetragTextBox, "QuellBetragTextBox")
        Me.QuellBetragTextBox.Name = "QuellBetragTextBox"
        '
        'QuellKontoComboBox
        '
        Me.QuellKontoComboBox.DataSource = Me.KurseKontenQuellBindingSource
        Me.QuellKontoComboBox.DisplayMember = "Bezeichnung"
        Me.QuellKontoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.QuellKontoComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.QuellKontoComboBox, "QuellKontoComboBox")
        Me.QuellKontoComboBox.Name = "QuellKontoComboBox"
        Me.QuellKontoComboBox.ValueMember = "ID"
        '
        'KurseKontenQuellBindingSource
        '
        Me.KurseKontenQuellBindingSource.DataMember = "Kurse_Konten_Quell"
        Me.KurseKontenQuellBindingSource.DataSource = Me.KurseBindingSource
        Me.KurseKontenQuellBindingSource.Sort = ""
        '
        'ZielBetragTextBox
        '
        Me.ZielBetragTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.KurseBindingSource, "ZielBetrag", True))
        resources.ApplyResources(Me.ZielBetragTextBox, "ZielBetragTextBox")
        Me.ZielBetragTextBox.Name = "ZielBetragTextBox"
        '
        'ZielKontoComboBox
        '
        Me.ZielKontoComboBox.DataSource = Me.KurseKontenZielBindingSource
        Me.ZielKontoComboBox.DisplayMember = "Bezeichnung"
        Me.ZielKontoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ZielKontoComboBox.FormattingEnabled = True
        resources.ApplyResources(Me.ZielKontoComboBox, "ZielKontoComboBox")
        Me.ZielKontoComboBox.Name = "ZielKontoComboBox"
        Me.ZielKontoComboBox.ValueMember = "ID"
        '
        'KurseKontenZielBindingSource
        '
        Me.KurseKontenZielBindingSource.DataMember = "Kurse_Konten_Ziel"
        Me.KurseKontenZielBindingSource.DataSource = Me.KurseBindingSource
        Me.KurseKontenZielBindingSource.Sort = ""
        '
        'KurseTableAdapter
        '
        Me.KurseTableAdapter.ClearBeforeFill = True
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
        Me.TableAdapterManager.KurseTableAdapter = Me.KurseTableAdapter
        Me.TableAdapterManager.Out2InTableAdapter = Nothing
        Me.TableAdapterManager.PlattformenTableAdapter = Nothing
        Me.TableAdapterManager.SzenarienTableAdapter = Nothing
        Me.TableAdapterManager.TradesTableAdapter = Nothing
        Me.TableAdapterManager.TradesWerteTableAdapter = Nothing
        Me.TableAdapterManager.TradeTypenTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.ZeitstempelWerteTableAdapter = Nothing
        '
        'KontenTableAdapter
        '
        Me.KontenTableAdapter.ClearBeforeFill = True
        '
        'pnlDetails
        '
        Me.pnlDetails.Controls.Add(Me.CalculatedCheckBox)
        Me.pnlDetails.Controls.Add(Me.ZielKontoComboBox)
        Me.pnlDetails.Controls.Add(ZielKontoIDLabel)
        Me.pnlDetails.Controls.Add(ZielBetragLabel)
        Me.pnlDetails.Controls.Add(Me.ZielBetragTextBox)
        Me.pnlDetails.Controls.Add(Me.QuellKontoComboBox)
        Me.pnlDetails.Controls.Add(QuellKontoIDLabel)
        Me.pnlDetails.Controls.Add(Me.QuellBetragTextBox)
        Me.pnlDetails.Controls.Add(Me.ZeitpunktDateTimePicker)
        Me.pnlDetails.Controls.Add(IDLabel)
        Me.pnlDetails.Controls.Add(Me.IDTextBox)
        Me.pnlDetails.Controls.Add(ZeitpunktLabel)
        Me.pnlDetails.Controls.Add(QuellBetragLabel)
        Me.pnlDetails.Controls.Add(CalculatedLabel)
        resources.ApplyResources(Me.pnlDetails, "pnlDetails")
        Me.pnlDetails.Name = "pnlDetails"
        '
        'CalculatedCheckBox
        '
        Me.CalculatedCheckBox.DataBindings.Add(New System.Windows.Forms.Binding("CheckState", Me.KurseBindingSource, "Calculated", True))
        resources.ApplyResources(Me.CalculatedCheckBox, "CalculatedCheckBox")
        Me.CalculatedCheckBox.Name = "CalculatedCheckBox"
        Me.ToolTip1.SetToolTip(Me.CalculatedCheckBox, resources.GetString("CalculatedCheckBox.ToolTip"))
        Me.CalculatedCheckBox.UseVisualStyleBackColor = True
        '
        'ErrProvider
        '
        Me.ErrProvider.ContainerControl = Me
        '
        'frmEditCourses
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.pnlDetails)
        Me.Controls.Add(Me.KurseBindingNavigator)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditCourses"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.KurseBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.KurseBindingNavigator.ResumeLayout(False)
        Me.KurseBindingNavigator.PerformLayout()
        CType(Me.KurseBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.KurseKontenQuellBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.KurseKontenZielBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlDetails.ResumeLayout(False)
        Me.pnlDetails.PerformLayout()
        CType(Me.ErrProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents CoinTracerDataSet As CoinTracer.CoinTracerDataSet
    Friend WithEvents KurseBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents KurseTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.KurseTableAdapter
    Friend WithEvents TableAdapterManager As CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager
    Friend WithEvents KurseBindingNavigator As System.Windows.Forms.BindingNavigator
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
    Friend WithEvents ZeitpunktDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents QuellBetragTextBox As System.Windows.Forms.TextBox
    Friend WithEvents QuellKontoComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents ZielBetragTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ZielKontoComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents KurseKontenQuellBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents KurseKontenZielBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents KontenTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.KontenTableAdapter
    Friend WithEvents pnlDetails As System.Windows.Forms.Panel
    Friend WithEvents ErrProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents CalculatedCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class

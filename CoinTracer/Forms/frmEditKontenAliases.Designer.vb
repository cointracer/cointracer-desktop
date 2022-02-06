<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEditKontenAliases
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditKontenAliases))
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.pnlEditTrades = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CoinTracerDataSet = New CoinTracer.CoinTracerDataSet()
        Me.KontenAliasesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.KontenAliasesTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.KontenAliasesTableAdapter()
        Me.TableAdapterManager = New CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager()
        Me.KontenAliasesBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.KontenAliasesBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.CodeDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AliasDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlEditTrades.SuspendLayout()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.KontenAliasesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.KontenAliasesBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.KontenAliasesBindingNavigator.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        'pnlEditTrades
        '
        Me.pnlEditTrades.Controls.Add(Me.Label2)
        Me.pnlEditTrades.Controls.Add(Me.Label1)
        Me.pnlEditTrades.Controls.Add(Me.Label3)
        Me.pnlEditTrades.Controls.Add(Me.Label4)
        resources.ApplyResources(Me.pnlEditTrades, "pnlEditTrades")
        Me.pnlEditTrades.Name = "pnlEditTrades"
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
        'CoinTracerDataSet
        '
        Me.CoinTracerDataSet.DataSetName = "CoinTracerDataSet"
        Me.CoinTracerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'KontenAliasesBindingSource
        '
        Me.KontenAliasesBindingSource.DataMember = "KontenAliases"
        Me.KontenAliasesBindingSource.DataSource = Me.CoinTracerDataSet
        '
        'KontenAliasesTableAdapter
        '
        Me.KontenAliasesTableAdapter.ClearBeforeFill = True
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
        Me.TableAdapterManager.KontenAliasesTableAdapter = Me.KontenAliasesTableAdapter
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
        'KontenAliasesBindingNavigator
        '
        Me.KontenAliasesBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.KontenAliasesBindingNavigator.BindingSource = Me.KontenAliasesBindingSource
        Me.KontenAliasesBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.KontenAliasesBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.KontenAliasesBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.KontenAliasesBindingNavigatorSaveItem})
        resources.ApplyResources(Me.KontenAliasesBindingNavigator, "KontenAliasesBindingNavigator")
        Me.KontenAliasesBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.KontenAliasesBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.KontenAliasesBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.KontenAliasesBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.KontenAliasesBindingNavigator.Name = "KontenAliasesBindingNavigator"
        Me.KontenAliasesBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorAddNewItem, "BindingNavigatorAddNewItem")
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        resources.ApplyResources(Me.BindingNavigatorCountItem, "BindingNavigatorCountItem")
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.BindingNavigatorDeleteItem, "BindingNavigatorDeleteItem")
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
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
        'KontenAliasesBindingNavigatorSaveItem
        '
        Me.KontenAliasesBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.KontenAliasesBindingNavigatorSaveItem, "KontenAliasesBindingNavigatorSaveItem")
        Me.KontenAliasesBindingNavigatorSaveItem.Name = "KontenAliasesBindingNavigatorSaveItem"
        '
        'DataGridView1
        '
        resources.ApplyResources(Me.DataGridView1, "DataGridView1")
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.CodeDataGridViewTextBoxColumn, Me.AliasDataGridViewTextBoxColumn})
        Me.DataGridView1.DataSource = Me.KontenAliasesBindingSource
        Me.DataGridView1.Name = "DataGridView1"
        '
        'CodeDataGridViewTextBoxColumn
        '
        Me.CodeDataGridViewTextBoxColumn.DataPropertyName = "Code"
        resources.ApplyResources(Me.CodeDataGridViewTextBoxColumn, "CodeDataGridViewTextBoxColumn")
        Me.CodeDataGridViewTextBoxColumn.Name = "CodeDataGridViewTextBoxColumn"
        '
        'AliasDataGridViewTextBoxColumn
        '
        Me.AliasDataGridViewTextBoxColumn.DataPropertyName = "Alias"
        resources.ApplyResources(Me.AliasDataGridViewTextBoxColumn, "AliasDataGridViewTextBoxColumn")
        Me.AliasDataGridViewTextBoxColumn.Name = "AliasDataGridViewTextBoxColumn"
        '
        'frmEditKontenAliases
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.KontenAliasesBindingNavigator)
        Me.Controls.Add(Me.pnlEditTrades)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditKontenAliases"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.pnlEditTrades.ResumeLayout(False)
        Me.pnlEditTrades.PerformLayout()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.KontenAliasesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.KontenAliasesBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.KontenAliasesBindingNavigator.ResumeLayout(False)
        Me.KontenAliasesBindingNavigator.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents pnlEditTrades As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents CoinTracerDataSet As CoinTracerDataSet
    Friend WithEvents KontenAliasesBindingSource As BindingSource
    Friend WithEvents KontenAliasesTableAdapter As CoinTracerDataSetTableAdapters.KontenAliasesTableAdapter
    Friend WithEvents TableAdapterManager As CoinTracerDataSetTableAdapters.TableAdapterManager
    Friend WithEvents KontenAliasesBindingNavigator As BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As ToolStripSeparator
    Friend WithEvents KontenAliasesBindingNavigatorSaveItem As ToolStripButton
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents CodeDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents AliasDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
End Class

'  **************************************
'  *
'  * Copyright 2013-2019 Andreas Nebinger
'  *
'  * Lizenziert unter der EUPL, Version 1.2 oder - sobald diese von der Europäischen Kommission genehmigt wurden -
'    Folgeversionen der EUPL ("Lizenz");
'  * Sie dürfen dieses Werk ausschließlich gemäß dieser Lizenz nutzen. Eine Kopie der Lizenz finden Sie hier:
' 
'  * https://joinup.ec.europa.eu/release/eupl/v12  (oder in der in diesem Projekt enthaltenden Datei "Lizenz.txt")
'  
'  * Sofern nicht durch anwendbare Rechtsvorschriften gefordert oder in schriftlicher Form vereinbart, wird die
'    unter der Lizenz verbreitete Software "so wie sie ist", OHNE JEGLICHE GEWÄHRLEISTUNG ODER BEDINGUNGEN -
'    ausdrücklich oder stillschweigend - verbreitet.
'  * Die sprachspezifischen Genehmigungen und Beschränkungen unter der Lizenz sind dem Lizenztext zu entnehmen.
' 
'  =======
'  English
'  =======
'  
'  * Licensed under the EUPL, Version 1.2 or - as soon they will be approved by the European Commission -
'    subsequent versions of the EUPL (the "Licence");
'  * You may not use this work except in compliance with the Licence. You may obtain a copy of the Licence at:
'  
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or in the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmViewCalculations
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
        Dim IDLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewCalculations))
        Dim ZeitpunktLabel As System.Windows.Forms.Label
        Dim CVSLabel As System.Windows.Forms.Label
        Dim ScenarioLabel As System.Windows.Forms.Label
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.CalculationsBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.IDTextBox = New System.Windows.Forms.TextBox()
        Me.CVSTextBox = New System.Windows.Forms.TextBox()
        Me.pnlDetails = New System.Windows.Forms.Panel()
        Me.grdCVS = New System.Windows.Forms.DataGridView()
        Me.CalcDateTextBox = New System.Windows.Forms.TextBox()
        Me.ScenarioTextBox = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Scenario = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Setting = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CalculationsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CoinTracerDataSet = New CoinTracer.CoinTracerDataSet()
        Me.CalculationsTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.VW_BerechnungenTableAdapter()
        Me.TableAdapterManager = New CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager()
        IDLabel = New System.Windows.Forms.Label()
        ZeitpunktLabel = New System.Windows.Forms.Label()
        CVSLabel = New System.Windows.Forms.Label()
        ScenarioLabel = New System.Windows.Forms.Label()
        CType(Me.CalculationsBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CalculationsBindingNavigator.SuspendLayout()
        Me.pnlDetails.SuspendLayout()
        CType(Me.grdCVS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CalculationsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'CVSLabel
        '
        resources.ApplyResources(CVSLabel, "CVSLabel")
        CVSLabel.Name = "CVSLabel"
        '
        'ScenarioLabel
        '
        resources.ApplyResources(ScenarioLabel, "ScenarioLabel")
        ScenarioLabel.Name = "ScenarioLabel"
        '
        'cmdOK
        '
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'CalculationsBindingNavigator
        '
        Me.CalculationsBindingNavigator.AddNewItem = Nothing
        Me.CalculationsBindingNavigator.BindingSource = Me.CalculationsBindingSource
        Me.CalculationsBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.CalculationsBindingNavigator.CountItemFormat = "of {0}"
        Me.CalculationsBindingNavigator.DeleteItem = Nothing
        Me.CalculationsBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2})
        resources.ApplyResources(Me.CalculationsBindingNavigator, "CalculationsBindingNavigator")
        Me.CalculationsBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.CalculationsBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.CalculationsBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.CalculationsBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.CalculationsBindingNavigator.Name = "CalculationsBindingNavigator"
        Me.CalculationsBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
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
        'IDTextBox
        '
        Me.IDTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CalculationsBindingSource, "ID", True))
        resources.ApplyResources(Me.IDTextBox, "IDTextBox")
        Me.IDTextBox.Name = "IDTextBox"
        '
        'CVSTextBox
        '
        resources.ApplyResources(Me.CVSTextBox, "CVSTextBox")
        Me.CVSTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CalculationsBindingSource, "CVS", True))
        Me.CVSTextBox.Name = "CVSTextBox"
        '
        'pnlDetails
        '
        resources.ApplyResources(Me.pnlDetails, "pnlDetails")
        Me.pnlDetails.Controls.Add(Me.grdCVS)
        Me.pnlDetails.Controls.Add(Me.CalcDateTextBox)
        Me.pnlDetails.Controls.Add(ScenarioLabel)
        Me.pnlDetails.Controls.Add(Me.ScenarioTextBox)
        Me.pnlDetails.Controls.Add(Me.CVSTextBox)
        Me.pnlDetails.Controls.Add(IDLabel)
        Me.pnlDetails.Controls.Add(Me.IDTextBox)
        Me.pnlDetails.Controls.Add(ZeitpunktLabel)
        Me.pnlDetails.Controls.Add(CVSLabel)
        Me.pnlDetails.Name = "pnlDetails"
        '
        'grdCVS
        '
        Me.grdCVS.AllowUserToAddRows = False
        Me.grdCVS.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.grdCVS, "grdCVS")
        Me.grdCVS.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.grdCVS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdCVS.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Scenario, Me.Setting})
        Me.grdCVS.Name = "grdCVS"
        Me.grdCVS.ReadOnly = True
        Me.grdCVS.RowHeadersVisible = False
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.grdCVS.RowsDefaultCellStyle = DataGridViewCellStyle1
        '
        'CalcDateTextBox
        '
        Me.CalcDateTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CalculationsBindingSource, "Berechung bis", True))
        resources.ApplyResources(Me.CalcDateTextBox, "CalcDateTextBox")
        Me.CalcDateTextBox.Name = "CalcDateTextBox"
        '
        'ScenarioTextBox
        '
        Me.ScenarioTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CalculationsBindingSource, "Szenario", True))
        resources.ApplyResources(Me.ScenarioTextBox, "ScenarioTextBox")
        Me.ScenarioTextBox.Name = "ScenarioTextBox"
        Me.ToolTip1.SetToolTip(Me.ScenarioTextBox, resources.GetString("ScenarioTextBox.ToolTip"))
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.DataGridViewTextBoxColumn1.FillWeight = 50.0!
        resources.ApplyResources(Me.DataGridViewTextBoxColumn1, "DataGridViewTextBoxColumn1")
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.DataGridViewTextBoxColumn2.FillWeight = 49.23858!
        resources.ApplyResources(Me.DataGridViewTextBoxColumn2, "DataGridViewTextBoxColumn2")
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        '
        'Scenario
        '
        Me.Scenario.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Scenario.FillWeight = 30.0!
        resources.ApplyResources(Me.Scenario, "Scenario")
        Me.Scenario.Name = "Scenario"
        Me.Scenario.ReadOnly = True
        '
        'Setting
        '
        Me.Setting.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Setting.FillWeight = 70.0!
        resources.ApplyResources(Me.Setting, "Setting")
        Me.Setting.Name = "Setting"
        Me.Setting.ReadOnly = True
        '
        'CalculationsBindingSource
        '
        Me.CalculationsBindingSource.DataMember = "VW_Berechnungen"
        Me.CalculationsBindingSource.DataSource = Me.CoinTracerDataSet
        '
        'CoinTracerDataSet
        '
        Me.CoinTracerDataSet.DataSetName = "CoinTracerDataSet"
        Me.CoinTracerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'CalculationsTableAdapter
        '
        Me.CalculationsTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager._VersionsTableAdapter = Nothing
        Me.TableAdapterManager.ApiDatenTableAdapter = Nothing
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.BestaendeTableAdapter = Nothing
        Me.TableAdapterManager.Connection = Nothing
        Me.TableAdapterManager.ImporteTableAdapter = Nothing
        Me.TableAdapterManager.KalkulationenTableAdapter = Nothing
        Me.TableAdapterManager.KonfigurationTableAdapter = Nothing
        Me.TableAdapterManager.KontenTableAdapter = Nothing
        Me.TableAdapterManager.KurseTableAdapter = Nothing
        Me.TableAdapterManager.Out2InTableAdapter = Nothing
        Me.TableAdapterManager.PlattformenTableAdapter = Nothing
        Me.TableAdapterManager.SzenarienTableAdapter = Nothing
        Me.TableAdapterManager.TradesTableAdapter = Nothing
        Me.TableAdapterManager.TradesWerteTableAdapter = Nothing
        Me.TableAdapterManager.TradeTypenTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.ZeitstempelWerteTableAdapter = Nothing
        '
        'frmViewCalculations
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlDetails)
        Me.Controls.Add(Me.CalculationsBindingNavigator)
        Me.Controls.Add(Me.cmdOK)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmViewCalculations"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.CalculationsBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CalculationsBindingNavigator.ResumeLayout(False)
        Me.CalculationsBindingNavigator.PerformLayout()
        Me.pnlDetails.ResumeLayout(False)
        Me.pnlDetails.PerformLayout()
        CType(Me.grdCVS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CalculationsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents CoinTracerDataSet As CoinTracer.CoinTracerDataSet
    Friend WithEvents CalculationsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents CalculationsTableAdapter As CoinTracer.CoinTracerDataSetTableAdapters.VW_BerechnungenTableAdapter
    Friend WithEvents TableAdapterManager As CoinTracer.CoinTracerDataSetTableAdapters.TableAdapterManager
    Friend WithEvents CalculationsBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents IDTextBox As System.Windows.Forms.TextBox
    Friend WithEvents CVSTextBox As System.Windows.Forms.TextBox
    Friend WithEvents pnlDetails As System.Windows.Forms.Panel
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents CalcDateTextBox As TextBox
    Friend WithEvents ScenarioTextBox As TextBox
    Friend WithEvents grdCVS As DataGridView
    Friend WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Friend WithEvents Scenario As DataGridViewTextBoxColumn
    Friend WithEvents Setting As DataGridViewTextBoxColumn
End Class

'  **************************************
'  *
'  * Copyright 2013-2021 Andreas Nebinger
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

Imports System.Reflection

Public Class BoundDataGridView
    Inherits DataGridView
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then
            Try
                If _DBO IsNot Nothing Then
                    _DBO.Dispose()
                    _DBO = Nothing
                End If
                If disposing Then
                    If components IsNot Nothing Then
                        RemoveHandler ContextMenuStrip.Items("mnuItmClearFilters").Click, AddressOf mnuItmClearFilters_Click
                        RemoveHandler ContextMenuStrip.Items("mnuItmCopyTable").Click, AddressOf mnuItmCopyTable_Click
                        RemoveHandler ContextMenuStrip.Items("mnuItmCopyRow").Click, AddressOf mnuItmCopyRow_Click
                        RemoveHandler ContextMenuStrip.Items("mnuItmCopyCell").Click, AddressOf mnuItmCopyCell_Click
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End If
        disposedValue = True
    End Sub

#End Region

#Region "Scale-Awareness Support"

    Friend _currentScaleFactor As SizeF = New SizeF(1.0F, 1.0F)
    Public ReadOnly Property CurrentScaleFactor() As SizeF
        Get
            Return _currentScaleFactor
        End Get
    End Property


    Protected Overrides Sub ScaleControl(factor As SizeF, specified As BoundsSpecified)
        MyBase.ScaleControl(factor, specified)
        ' Record the running scale factor used
        _currentScaleFactor.Width *= factor.Width
        _currentScaleFactor.Height *= factor.Height
    End Sub

#End Region

    Public Event CheckBoxColumnClicked As EventHandler(Of EventArgs)

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        'Benutzerdefinierten Zeichnungscode hier einfügen
        If Not _Initialized Then
            ' Kontextmenü ggf. erstellen & initialisieren
            If ContextMenuStrip Is Nothing Then
                ContextMenuStrip = New ContextMenuStrip()
            End If
            ContextMenuStripInit()
            _Initialized = True
        End If
    End Sub

    Private _DBO As DBObjects
    Private _BS As BindingSource

    Private _Initialized As Boolean

    Private _FilterColumns As List(Of Object)

    Private _SQLStatement As String
    Public ReadOnly Property SQLStatement() As String
        Get
            Return _SQLStatement
        End Get
    End Property

    Private _Cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _Cnn
        End Get
    End Property

    Private _TbA As Object
    Public ReadOnly Property TableAdapter() As Object
        Get
            Return _TbA
        End Get
    End Property


    ''' <summary>
    ''' Bindet das GridView an das SQL-Statement zur übergebenen Connection
    ''' </summary>
    ''' <param name="SQL">SQL-Statement für das Laden des Grids</param>
    ''' <param name="Connection">SQLite-Connection, die verwendet werden soll (auch für Reloads)</param>
    Public Sub BindGrid(SQL As String, Connection As SQLite.SQLiteConnection)
        _SQLStatement = SQL
        _Cnn = Connection
        AutoGenerateColumns = False
    End Sub
    ''' <summary>
    ''' Bindet das GridView an den übergebenen TableAdapter
    ''' </summary>
    ''' <param name="DataTableAdapter">Ein SQLiteDataAdapter oder ein typisierter CoinTracerDataSetTableAdapter</param>
    Public Sub BindGrid(DataTableAdapter As Object)
        _TbA = DataTableAdapter
        AutoGenerateColumns = False
    End Sub

    Private Sub SetBindingSource(DataSource As Object)
        _BS = New BindingSource(DataSource, "")
        Me.DataSource = _BS
    End Sub

    ''' <summary>
    ''' Makes a backup of the current filter and sorting settings
    ''' </summary>
    Public Sub FilterBackup()
        _FilterColumns = New List(Of Object)
        For Each Col In Columns
            If TypeOf Col Is DataGridViewAutoFilterTextBoxColumn Then
                Col.HeaderCell.FilterSettingBackup
                _FilterColumns.Add(Col)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Restores all filter and sorting settings
    ''' </summary>
    Public Sub FilterRestore()
        ' Restore filtering columns
        For Each Col In _FilterColumns
            Col.HeaderCell.FilterSettingRestore
        Next
    End Sub

    ''' <summary>
    ''' Lädt das Grid neu. Vorher muss über 'BindGrid' das zugehörige SQL-Statement gesetzt werden.
    ''' </summary>
    Public Sub Reload(Optional ByVal KeepFilters As Boolean = True)
        If _TbA Is Nothing Then
            If _DBO IsNot Nothing Then
                _DBO = Nothing
            End If
            _DBO = New DBObjects(_SQLStatement, _Cnn, "Table")
            SetBindingSource(_DBO.DataTable)
        Else
            ' Nicht 100% robust - aber besser, als jeden TableAdapter-Typ einzeln abzufragen.
            ' Die unten stehende Zeile ist eine relativ elegante Lösung, um unabhängig vom aktuellen
            ' TableAdapter das notwendige DataTable-Objekt zu definieren. Well done!!
            Dim Tb As Object = Activator.CreateInstance(Assembly.GetExecutingAssembly.FullName,
                                                        Application.ProductName & ".CoinTracerDataSet+" & _TbA.GetType().Name.Replace("TableAdapter", "DataTable")).Unwrap()
            'Dim Tb As Object
            'Dim TaTyp As String = _TbA.GetType.Name
            'If TaTyp.Contains("Plattform") Then
            '    Tb = New CoinTracerDataSet.VW_PlattformenDataTable
            'ElseIf TaTyp.Contains("Importe") Then
            '    Tb = New CoinTracerDataSet.VW_ImporteDataTable
            'ElseIf TaTyp.Contains("Konten") Then
            '    Tb = New CoinTracerDataSet.VW_KontenDataTable
            'Else
            '    Tb = New CoinTracerDataSet.VW_TradesDataTable
            'End If
            If KeepFilters Then FilterBackup()
            _TbA.Fill(DirectCast(Tb, DataTable))
            SetBindingSource(Tb)
            If KeepFilters Then FilterRestore()
        End If
    End Sub

    ''' <summary>
    ''' Removes all column filters from the grid
    ''' </summary>
    Public Sub RemoveFilters()
        DataGridViewAutoFilterTextBoxColumn.RemoveFilter(Me)
    End Sub


    Public Sub New()
        Thread.CurrentThread.CurrentCulture = New CultureInfo(My.Settings.CurrentCulture)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        DoubleBuffered = True
        _Initialized = False
    End Sub

    ''' <summary>
    ''' Initialisiert die Standard-Menü-Items des Kontextmenüs
    ''' </summary>
    Private Sub ContextMenuStripInit()
        With ContextMenuStrip
            ' Prüfen, ob Default-Kontextmenüeinträge schon vorhanden sind
            If .Items.ContainsKey("mnuItmCopyCell") Then
                Exit Sub
            End If
            If .Items.Count > 0 Then
                .Items.Add("-")
            End If
            Dim mnuItem As New ToolStripMenuItem() With {.Text = My.Resources.MyStrings.mnuItmClearFilters,
                                                         .Name = "mnuItmClearFilters",
                                                         .Image = DirectCast(My.Resources.filter_clear, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmClearFilters_Click
            .Items.Add(mnuItem)
            mnuItem = New ToolStripMenuItem() With {.Text = My.Resources.MyStrings.mnuItmCopyCell,
                                                         .Name = "mnuItmCopyCell",
                                                         .Image = DirectCast(My.Resources.edit_copycell_icon_16x16, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmCopyCell_Click
            .Items.Add(mnuItem)
            mnuItem = New ToolStripMenuItem() With {.Text = My.Resources.MyStrings.mnuItmCopyRow,
                                                         .Name = "mnuItmCopyRow",
                                                         .Image = DirectCast(My.Resources.edit_copyrow_icon_16x16, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmCopyRow_Click
            .Items.Add(mnuItem)
            mnuItem = New ToolStripMenuItem() With {.Text = My.Resources.MyStrings.mnuItmCopyTable,
                                                         .Name = "mnuItmCopyTable",
                                                         .Image = DirectCast(My.Resources.edit_copytable_icon_16x16, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmCopyTable_Click
            .Items.Add(mnuItem)
            mnuItem = New ToolStripMenuItem() With {.Text = My.Resources.MyStrings.mnuItmExportTable,
                                                         .Name = "mnuItmExportTable",
                                                         .Image = DirectCast(My.Resources.edit_exporttable_icon_16x16, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmExportTable_Click
            .Items.Add(mnuItem)
        End With
    End Sub

    Private Sub mnuItmCopyTable_Click(sender As Object, e As EventArgs)
        DataGridViewToClipboard(Me)
    End Sub

    Private Sub mnuItmExportTable_Click(sender As Object, e As EventArgs)
        Dim dgv As DataGridView = DirectCast(sender.Owner.SourceControl, DataGridView)
        Dim SFD As New SaveFileDialog
        With SFD
            .Filter = My.Resources.MyStrings.mainMsgSaveTableFilter
            .Title = My.Resources.MyStrings.mainMsgSaveTableTitle
            Dim ControlName As String = dgv.Name
            If ControlName.StartsWith("grd") Then ControlName = ControlName.Substring(3)
            .FileName = String.Format("cointracer_{1}_{0}.csv", Now.ToString("yyyy-MM-dd_HH.mm"), ControlName)
            If .ShowDialog(Me) = DialogResult.OK Then
                Try
                    CsvReaderWriter.WriteDataTableToCsv(TryCast(DataSource.DataSource, DataTable), .FileName, True)
                    MessageBox.Show(My.Resources.MyStrings.mainMsgSaveTableSuccess, My.Resources.MyStrings.mainMsgSaveTableSuccessTitle,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    DefaultErrorHandler(ex, My.Resources.MyStrings.mainMsgSaveTableError & Environment.NewLine & ex.Message)
                End Try
            End If
        End With
    End Sub

    Private Sub mnuItmCopyRow_Click(sender As Object, e As EventArgs)
        If SelectedCells.Count > 0 Then
            DataGridViewToClipboard(Me, CurrentCell.RowIndex)
        End If

    End Sub

    Private Sub mnuItmCopyCell_Click(sender As Object, e As EventArgs)
        Try
            If SelectedCells.Count > 0 Then
                Dim o As New DataObject
                o.SetText(CurrentCell.Value)
                Clipboard.SetDataObject(o, True)
            End If
        Catch ex As Exception
            Debug.Print("mnuItmCopyCell_Click")
        End Try
    End Sub

    Private Sub mnuItmClearFilters_Click(sender As Object, e As EventArgs)
        DataGridViewAutoFilterTextBoxColumn.RemoveFilter(Me)
    End Sub

    Private Sub BoundDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellClick
        Try
            Dim grd As DataGridView = DirectCast(sender, DataGridView)
            If grd.SelectedCells.Count > 0 AndAlso grd.Columns(e.ColumnIndex).GetType().Name = "DataGridViewAutoFilterCheckBoxColumn" AndAlso e.RowIndex >= 0 Then
                RaiseEvent CheckBoxColumnClicked(Me, New EventArgs())
            End If
        Catch ex As Exception
            Debug.Print("BoundDataGridView_CellClick")
        End Try
    End Sub

    Private Sub BoundDataGridView_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles Me.CellMouseDown
        If e.ColumnIndex >= 0 And e.RowIndex >= 0 Then
            CurrentCell = Rows(e.RowIndex).Cells(e.ColumnIndex)
        End If
    End Sub

    Private _NoRecursion As Boolean = False

    ''' <summary>
    ''' Erstellt die notwendigen Spalten mit AutoFilterColumnHeaderCells
    ''' </summary>
    Private Sub BoundDataGridView_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles Me.DataBindingComplete

        If Not _NoRecursion And Columns.Count = 0 Then
            Try
                _NoRecursion = True
                Dim Table As DataTable = TryCast(DataSource.DataSource, DataTable)
                If Table IsNot Nothing Then
                    For Each TableColumn As DataColumn In Table.Columns
                        Dim GridColumn As Object
                        If TableColumn.DataType Is Type.GetType("System.Boolean") Then
                            GridColumn = New DataGridViewAutoFilterCheckBoxColumn
                            GridColumn.CaptionCheckedValues = "Angehakte"
                            GridColumn.CaptionUncheckedValues = "Nicht Angehakte"
                        Else
                            GridColumn = New DataGridViewAutoFilterTextBoxColumn
                        End If
                        With GridColumn
                            .CaptionAllValues = "(Alles auswählen)"
                            .CaptionBlankValues = "(Leere)"
                            .CaptionCancelButton = "Abbrechen"
                            .Name = TableColumn.ColumnName
                            .DataPropertyName = TableColumn.ColumnName
                            .HeaderText = .Name
                            .AutoSizeMode = AutoSizeColumnsMode
                            .ReadOnly = [ReadOnly]
                        End With
                        Columns.Add(GridColumn)
                    Next
                End If
            Catch ex As Exception
                Debug.Print("BoundDataGridView_DataBindingComplete")
            Finally
                _NoRecursion = False
            End Try

        End If
    End Sub
End Class

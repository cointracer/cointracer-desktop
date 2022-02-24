'  **************************************
'  *
'  * Copyright 2013-2022 Andreas Nebinger
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

Imports CoinTracer.DBHelper
Imports CoinTracer.CoinTracerDataSetTableAdapters
Imports CoinTracer.CoinTracerDataSet


Public Class frmEditTransfers

    Private _ImportFilterID As Long
    Public Property ImportFilterID() As Long
        Get
            Return _ImportFilterID
        End Get
        Set(ByVal value As Long)
            _ImportFilterID = value
        End Set
    End Property

    Private _UnclearOnly As Boolean
    Public Property UnclearOnly() As Boolean
        Get
            Return _UnclearOnly
        End Get
        Set(ByVal value As Boolean)
            _UnclearOnly = value
        End Set
    End Property

    Private _Sticky As Boolean
    Public Property Sticky() As Boolean
        Get
            Return _Sticky
        End Get
        Set(ByVal value As Boolean)
            _Sticky = value
        End Set
    End Property


    Private _TradesTA As New VW_TransfersTableAdapter
    Private _TradesTb As New VW_TransfersDataTable
    Private _LastSelected As Long


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _ImportFilterID = -1
        _UnclearOnly = True
        _Sticky = False
        grdTransfers.AutoGenerateColumns = False
        _LastSelected = -1

    End Sub


    Private Sub frmEditTransfers_Load(sender As Object, e As EventArgs) Handles Me.Load

        ' Initialize import filter ComboBox
        Dim SQL As String = String.Format("select 9999999 as Sort, 0 as ID, '{0}' as Display union all " & _
                                          "select i.ID, i.ID, 'ID ' || i.ID || ' | ' || strftime('{1}', i.Zeitpunkt) " & _
                                          "|| ' | ' || Bezeichnung from Importe i " & _
                                          "left join Plattformen p on p.ID = i.PlattformID " & _
                                          "order by Sort desc", _
                                          My.Resources.MyStrings.globalNoFilter, _
                                          My.Resources.MyStrings.dbStrftimeFormat)
        Dim dboImports As New DBObjects(SQL, frmMain.Connection, TableNames._AnyTable)
        cboImport.DataSource = dboImports.DataTable
        If _ImportFilterID > 0 AndAlso cboImport.Items.Count > 1 Then
            cboImport.SelectedIndex = 1
        Else
            cboImport.SelectedIndex = 0
        End If
        dboImports.Dispose()

        ' Initialize mode combobox
        If _UnclearOnly Then
            cboTransferType.SelectedIndex = 1
        Else
            cboTransferType.SelectedIndex = 0
        End If

        ' Initialize platform assignment comboboxes
        Dim PlatformsTb1 As New PlattformenDataTable
        Dim PlatformsTb2 As New PlattformenDataTable
        Dim PlatformsTA As New PlattformenTableAdapter
        PlatformsTA.Fill(PlatformsTb1)
        PlatformsTA.Fill(PlatformsTb2)
        cboSourcePlatforms.DataSource = PlatformsTb1
        cboTargetPlatforms.DataSource = PlatformsTb2
        cboSourcePlatforms.SelectedIndex = 0
        cboTargetPlatforms.SelectedIndex = 0

        ' Load GridView data
        LoadTransferData()

    End Sub

    Private Sub LoadTransferData()
        Dim SQL As String = ""
        Dim Glue As String = ""
        If cboImport.SelectedIndex > 0 Then
            SQL &= " ImportID=" & cboImport.SelectedValue.ToString
            Glue = " and "
        End If
        If cboTransferType.SelectedIndex = 1 Then
            SQL &= Glue & " (ZielplattformID = 0 or QuellPlattformID = 0)"
        End If
        If SQL.Length > 0 Then
            SQL = " where " & SQL
        End If
        ' TradesBindingSource.Filter = SQL
        _TradesTA.FillBySQL(_TradesTb, SQL)
        TradesBindingSource.DataSource = Nothing
        TradesBindingSource.DataSource = _TradesTb
        grdTransfers.DataSource = TradesBindingSource
    End Sub

    Private Sub cboTransferType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTransferType.SelectedIndexChanged
        LoadTransferData()
    End Sub

    Private Sub cboImport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboImport.SelectedIndexChanged
        LoadTransferData()
    End Sub

    Private Sub RefreshSelecedRowDisplay()
        Me.lblRowsDisplayed.Text = "(" & grdTransfers.RowCount & ")"
    End Sub

    Private Sub grdTransfers_RowsChanged(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles grdTransfers.RowsAdded
        RefreshSelecedRowDisplay()
        _LastSelected = -1
    End Sub

    Private Sub grdTransfers_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles grdTransfers.RowsRemoved
        RefreshSelecedRowDisplay()
        _LastSelected = -1
    End Sub

    ''' <summary>
    ''' Extracts all trade entry ids from the gridview and updates the target or source platform ids
    ''' </summary>
    ''' <returns>Number of updated entries, -1 on error</returns>
    Private Function UpdatePlatforms(PlatformID As Long, TargetPlatform As Boolean) As Long
        Const CHUNK As Integer = 100
        Dim Result As Long
        Dim IDs As String = ""
        Dim i As Integer = 0
        Dim Updated As Long = 0
        Dim Total As Long = grdTransfers.RowCount
        Try
            ProgressWaitManager.ShowProgress(Me, String.Format("{0}. {1}...", _
                                                               My.Resources.MyStrings.transfersTransfersBeingUpdated, _
                                                               My.Resources.MyStrings.globalPleaseWait))
            For Each Row As DataGridViewRow In grdTransfers.Rows
                IDs &= Row.Cells(0).Value & ","
                i += 1
                Updated += 1
                If i >= CHUNK Then
                    ' perform the update for this chunk
                    ProgressWaitManager.UpdateProgress(Updated / Total * 100, My.Resources.MyStrings.transfersTransfersBeingUpdated)
                    IDs = IDs.Substring(0, IDs.Length - 1)
                    UpdateTransfersPlatforms(PlatformID, IDs, TargetPlatform)
                    i = 0
                End If
            Next
            If i > 0 Then
                ' perform the update for the last chunk
                ProgressWaitManager.UpdateProgress(Updated / Total * 100, My.Resources.MyStrings.transfersTransfersBeingUpdated)
                IDs = IDs.Substring(0, IDs.Length - 1)
                UpdateTransfersPlatforms(PlatformID, IDs, TargetPlatform)
            End If
            Result = Updated
            ' Reload grid
            grdTransfers.RemoveFilters()
            LoadTransferData()
        Catch ex As Exception
            DefaultErrorHandler(ex)
            Result = -1
        Finally
            ProgressWaitManager.CloseProgress()
        End Try
        Return Result
    End Function

    ''' <summary>
    ''' Updates the QuellPlattformID or ZielPlattformID of the given IDs of Trades table entries. Is being called by UpdatePlatforms().
    ''' </summary>
    ''' <param name="PlatformID">Platform ID that will be written</param>
    ''' <param name="IDs">String containing the IDs of the rows that will be updated (comma separated)</param>
    ''' <param name="TargetPlatform">True, if the ZielPlattformID is to be set. False, if QuellPlattformID has to be set</param>
    Private Sub UpdateTransfersPlatforms(PlatformID As Long, IDs As String, TargetPlatform As Boolean)
        Dim SQL As String
        If TargetPlatform Then
            SQL = "ZielPlattformID"
        Else
            SQL = "QuellPlattformID"
        End If
        SQL = String.Format("update Trades set {0} = {1} where ID in ({2})", _
                            SQL, _
                            PlatformID, _
                            IDs)
        Dim DB As New DBHelper(frmMain.Connection)
        DB.ExecuteSQL(SQL)
    End Sub

    Private Sub btAssignSourcePlatform_Click(sender As Object, e As EventArgs) Handles btAssignSourcePlatform.Click
        If MessageBox.Show(String.Format(My.Resources.MyStrings.transfersUpdateSourcePlatforms, _
                                         cboSourcePlatforms.Text), My.Resources.MyStrings.transfersUpdateSourcePlatformsCaption, _
                           MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
            Dim Updated As Long = UpdatePlatforms(cboSourcePlatforms.SelectedValue, False)
            If Updated >= 0 Then
                _Sticky = True
                MessageBox.Show(String.Format(My.Resources.MyStrings.transfersSourcePlatformsUpdated, _
                                         Updated), My.Resources.MyStrings.transfersUpdatedCaption, _
                                     MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub


    Private Sub btAssignTargetPlatform_Click(sender As Object, e As EventArgs) Handles btAssignTargetPlatform.Click
        If MessageBox.Show(String.Format(My.Resources.MyStrings.transfersUpdateTargetPlatforms, _
                                         cboTargetPlatforms.Text), My.Resources.MyStrings.transfersUpdateTargetPlatformsCaption, _
                           MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
            Dim Updated As Long = UpdatePlatforms(cboTargetPlatforms.SelectedValue, True)
            If Updated >= 0 Then
                _Sticky = True
                MessageBox.Show(String.Format(My.Resources.MyStrings.transfersTargetPlatformsUpdated, _
                                         Updated), My.Resources.MyStrings.transfersUpdatedCaption, _
                                     MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub btEditTrades_Click(sender As Object, e As EventArgs) Handles btEditTrades.Click
        Try
            Dim OpenTransfersForm As New frmEditTrades
            With OpenTransfersForm
                .EditMode = frmEditTrades.TradesEditModes.TransfersOnly
                If _LastSelected >= 0 Then
                    .StartID = _LastSelected
                End If
                .ShowDialog(Me)
                .Dispose()
            End With
            LoadTransferData()
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    Private Sub grdTransfers_SelectionChanged(sender As Object, e As EventArgs) Handles grdTransfers.SelectionChanged
        Dim Grid As BoundDataGridView = DirectCast(sender, BoundDataGridView)
        With Grid
            If .SelectedRows.Count = 1 Then
                _LastSelected = .SelectedRows(0).Cells(0).Value
            End If
            .ClearSelection()
        End With
    End Sub
End Class

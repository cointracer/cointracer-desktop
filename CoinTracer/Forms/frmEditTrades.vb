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

Imports Microsoft.VisualBasic
Imports CoinTracer.CoinTracerDataSet

Public Class frmEditTrades

    Public Enum TradesEditModes
        AllTypes
        TransfersOnly
    End Enum

    Private Const NOTRANSFERSELECTEDLABEL As String = "- kein Transfer ausgewählt -"

    Private _TradeRows() As DataRow
    Private _DBO As DBObjects
    Private _TransferMerger As TransferMerger

    Private _EditMode As TradesEditModes = TradesEditModes.AllTypes
    Public Property EditMode() As TradesEditModes
        Get
            Return _EditMode
        End Get
        Set(ByVal value As TradesEditModes)
            _EditMode = value
            pnlEditTrades.Visible = _EditMode = TradesEditModes.AllTypes
            pnlOpenTransfers.Visible = Not _EditMode = TradesEditModes.AllTypes
            cmdTransferDetectionSettings.Visible = Not _EditMode = TradesEditModes.AllTypes
            If _EditMode = TradesEditModes.AllTypes Then
                ModusComboBox.SelectedIndex = 0
            Else
                ModusComboBox.SelectedIndex = 1
            End If
        End Set
    End Property

    Private _StartID As Long = -1
    Public Property StartID() As Long
        Get
            Return _StartID
        End Get
        Set(ByVal value As Long)
            _StartID = value
        End Set
    End Property

    Private _RecordsModified As Integer
    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return _RecordsModified
        End Get
    End Property

    Private _RowsInsertedOrDeleted As Boolean
    Public ReadOnly Property RowsInsertedOrDeleted() As Boolean
        Get
            Return _RowsInsertedOrDeleted
        End Get
    End Property

    Private Sub frmEditTrades_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _RecordsModified = 0
        _RowsInsertedOrDeleted = False
        ' Daten aus Trades holen - in Abhängigkeit vom Edit-Modus
        ResetTradeData()
        ' Hilfsobjekt für das Zusammenführen von Transfers initialisieren
        _TransferMerger = New TransferMerger(dctrlTrades.TradesTableAdapter, dctrlTrades.TradesBindingSource.DataSource)
        dctrlTrades_CurrentChanged(Nothing, Nothing)
        lblSelectedTransfer.Text = NOTRANSFERSELECTEDLABEL
    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        Try
            dctrlTrades.UpdateDatabase()
            _RecordsModified = dctrlTrades.RecordsModified
            _RowsInsertedOrDeleted = dctrlTrades.RowsInsertedOrDeleted
        Catch ex As Exception
            DefaultErrorHandler(ex, "Fehler beim Speichern der Daten: " & ex.Message)
        End Try
        Close()
    End Sub

    Private Sub cmdCancel_MouseEnter(sender As Object, e As EventArgs) Handles cmdCancel.MouseEnter
        dctrlTrades.EnableValidation = False
    End Sub

    Private Sub cmdCancel_MouseLeave(sender As Object, e As EventArgs) Handles cmdCancel.MouseLeave
        dctrlTrades.EnableValidation = True
    End Sub



    ''' <summary>
    ''' Setzt verfügbare Buttons in Abhängigkeit vom Datenzeiger des Trade-Controls
    ''' </summary>
    Private Sub dctrlTrades_CurrentChanged(sender As Object, e As EventArgs) Handles dctrlTrades.CurrentChanged
        Dim SomeSelected As Boolean = dctrlTrades.CurrentTradeID > 0
        If SomeSelected And _TransferMerger IsNot Nothing Then
            Dim CurRow As TradesRow = dctrlTrades.TradesBindingSource.Current.Row
            If CurRow.TradeTypID = DBHelper.TradeTypen.Transfer Then
                cmdMergeAbort.Enabled = _TransferMerger.HasStoredTransfers
                cmdMerge.Enabled = _TransferMerger.FirstTransferID <> dctrlTrades.CurrentTradeID
                lblSelectedTransfer.Enabled = True
                Label5.Enabled = True
            Else
                cmdMerge.Enabled = False
                cmdMergeAbort.Enabled = False
                lblSelectedTransfer.Enabled = False
                Label5.Enabled = False
            End If
        Else
            cmdMerge.Enabled = False
            cmdMergeAbort.Enabled = False
            lblSelectedTransfer.Enabled = False
            Label5.Enabled = False
        End If
    End Sub


    ''' <summary>
    ''' Transfers sollen zusammengeführt werden
    ''' </summary>
    Private Sub cmdMerge_Click(sender As Object, e As EventArgs) Handles cmdMerge.Click
        If _TransferMerger.HasStoredTransfers Then
            ' TODO: Transfers zusammenführen
            Dim Result As Long
            Dim ResultMessage As String = Nothing
            Try
                If _TransferMerger.CheckTradeMergability(dctrlTrades.CurrentTradeID, Result, ResultMessage) Then
                    ' Alles okay, Transfers zusammenführen
                    If MessageBox.Show(String.Format("Möchten Sie diese beiden Transfers zusammenfassen:{0}{0}Transfer 1: {0}{1}{0}{0}Transfer 2: {0}{2}",
                                                     Environment.NewLine,
                                                     _TransferMerger.FirstTransferDescription,
                                                     _TransferMerger.SecondTransferDescription), "Transfers zusammenfassen",
                                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.OK Then
                        _TransferMerger.MergeTrades()
                        MessageBox.Show("Beide Transfers wurden erfolgreich zusammengefasst!", "Transfers zusammenfassen", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        dctrlTrades.Reload(_TransferMerger.FirstTransferID)
                        cmdMergeAbort_Click(Nothing, Nothing)
                    End If
                Else
                    MsgBoxEx.ShowInFront("Diese beiden Transfers können nicht zusammengefasst werden:" & Environment.NewLine & Environment.NewLine &
                                         ResultMessage & "!", "Transfers zusammenfassen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            Catch ex As Exception
                DefaultErrorHandler(ex, "Beim Zusammenfassen der Transfers ist ein Fehler aufgetreten: " & ex.Message)
                Exit Sub
            End Try
        Else
            ' Aktuellen Transfer merken
            If _TransferMerger.ShowAdvice = Windows.Forms.DialogResult.OK Then
                _TransferMerger.FirstTransferID = dctrlTrades.CurrentTradeID
                _TransferMerger.SecondTransferID = 0
                lblSelectedTransfer.Text = _TransferMerger.FirstTransferDescription
                lblSelectedTransfer.Font = New Font(lblSelectedTransfer.Font, FontStyle.Bold)
                cmdMergeAbort.Enabled = True
                cmdMerge.Text = "Diesen Transfer mit dem unten stehenden zusammenfassen..."
                dctrlTrades_CurrentChanged(Nothing, Nothing)
            End If
        End If
    End Sub

    Private Sub cmdMergeAbort_Click(sender As Object, e As EventArgs) Handles cmdMergeAbort.Click
        If _TransferMerger.HasStoredTransfers Then
            _TransferMerger.ResetTransfers()
            lblSelectedTransfer.Text = NOTRANSFERSELECTEDLABEL
            lblSelectedTransfer.Font = New Font(lblSelectedTransfer.Font, FontStyle.Regular)
            cmdMerge.Text = "Diesen Transfer mit einem anderen zusammenfassen..."
            dctrlTrades_CurrentChanged(Nothing, Nothing)
        End If
    End Sub

    Private Sub cmdTransferDetectionSettings_Click(sender As Object, e As EventArgs) Handles cmdTransferDetectionSettings.Click
        Dim Form As New frmApplicationSettings
        Form.SelectedCategory = 5
        Form.ShowDialog(Me)
        ResetTradeData()
        dctrlTrades_CurrentChanged(Nothing, Nothing)
    End Sub

    Private Sub ModusComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ModusComboBox.SelectedIndexChanged
        Dim Changed As Boolean = (_EditMode = TradesEditModes.AllTypes And ModusComboBox.SelectedIndex <> 0) _
                                 OrElse (_EditMode = TradesEditModes.TransfersOnly And ModusComboBox.SelectedIndex <> 1)
        If Changed Then
            Select Case ModusComboBox.SelectedIndex
                Case 1
                    EditMode = TradesEditModes.TransfersOnly
                Case Else
                    EditMode = TradesEditModes.AllTypes
            End Select
            ResetTradeData()
            dctrlTrades_CurrentChanged(Nothing, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Resets the loaded trade data corresponding to the current filter mode
    ''' </summary>
    Private Sub ResetTradeData()
        Dim SQL As String
        Dim CrtlMode As TradeCoreDataControl.ControlModes
        Select Case _EditMode
            Case TradesEditModes.TransfersOnly
                SQL = "where Entwertet=0 and TradeTypID=" & DBHelper.TradeTypen.Transfer & _
                      " and (QuellPlattformID=0 or ZielPlattformID=0) order by Zeitpunkt, Quellbetrag"
                CrtlMode = TradeCoreDataControl.ControlModes.EditOpenTransfers
            Case Else
                SQL = "where Entwertet=0 order by Zeitpunkt, Quellbetrag"
                CrtlMode = TradeCoreDataControl.ControlModes.EditTrades
        End Select
        dctrlTrades.InitData(frmMain.Connection, SQL, CrtlMode, _StartID)
    End Sub
End Class

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

Imports CoinTracer.CoinValueStrategy
Imports CoinTracer.CoinTracerDataSetTableAdapters
Imports CoinTracer.CoinTracerDataSet
Imports CoinTracer.My.Resources
Imports System.Linq

Public Enum CoinBusinessCases
    _Default = 0
    BuyForCoins
    SellForFiat
    SellForCoins
    TransferExchangeToWallet
    TransferExchangeToExchange
    TransferWalletToExchange
    Withdraw
    Deposit
End Enum


#Region "Custom Exceptions"

''' <summary>
''' Bildet einen Fehler beim Berechnen der TradeValues oder ZeitstempelWerte ab
''' </summary>
<Serializable()>
Public Class TradeValueException
    Inherits System.Exception
    Implements Runtime.Serialization.ISerializable

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class

#End Region

''' <summary>
''' Klasse für das Event eines geänderten CuttOff-Days
''' </summary>
''' <remarks></remarks>
Public Class GainingsCutOffDayEventArgs
    Inherits EventArgs
    Public Property GainingsCutOffDay As Date
    Public Sub New(GainingsCutOffDay As Date)
        Me.GainingsCutOffDay = GainingsCutOffDay
    End Sub
End Class

#Region "Transfer Helper Class"

''' <summary>
''' Hilfsklasse für das Zusammenführen von Trades
''' </summary>
Public Class TransferMerger

    Private _TradesTableAdapter As CoinTracerDataSetTableAdapters.TradesTableAdapter
    Private _TradesTable As TradesDataTable
    Private _KontenTable As KontenDataTable
    Private _PlattformenTable As PlattformenDataTable
    Private _1stTradeRow As TradesRow
    Private _2ndTradeRow As TradesRow
    Private _ToleranceValue As Decimal

    ''' <summary>
    ''' ID des ersten zusammenzuführenden Transfers
    ''' </summary>
    Private _1stTransferID As Long
    Public Property FirstTransferID() As Long
        Get
            Return _1stTransferID
        End Get
        Set(ByVal value As Long)
            _1stTransferID = value
            _1stTradeRow = _TradesTable.FindByID(value)
        End Set
    End Property

    ''' <summary>
    ''' Liefert einen beschreibenden String für den ersten Transfer zurück
    ''' </summary>
    Public ReadOnly Property FirstTransferDescription() As String
        Get
            Return GetTransferDescription(_1stTradeRow)
        End Get
    End Property

    ''' <summary>
    ''' ID des zweiten zusammenzuführenden Transfers
    ''' </summary>
    Private _2ndTransferID As Long
    Public Property SecondTransferID() As Long
        Get
            Return _2ndTransferID
        End Get
        Set(ByVal value As Long)
            _2ndTransferID = value
            _2ndTradeRow = _TradesTable.FindByID(value)
        End Set
    End Property

    ''' <summary>
    ''' Faktor, um den beide Trades bei Ihren Beträgen abweichen dürfen. Default = 0.2
    ''' </summary>
    Public Property ToleranceFactor() As Decimal
        Get
            Return _ToleranceValue
        End Get
        Set(ByVal value As Decimal)
            _ToleranceValue = value
        End Set
    End Property


    ''' <summary>
    ''' Liefert einen beschreibenden String für den zweiten Transfer zurück
    ''' </summary>
    Public ReadOnly Property SecondTransferDescription() As String
        Get
            Return GetTransferDescription(_2ndTradeRow)
        End Get
    End Property

    ''' <summary>
    ''' Prüft, ob es gespeicherte Transfers gibt, die zusammengeführt werden sollen.
    ''' </summary>
    Public ReadOnly Property HasStoredTransfers() As Boolean
        Get
            Return (_1stTransferID > 0) Or (_2ndTransferID > 0)
        End Get
    End Property

    ''' <summary>
    ''' Prüft, ob beide Transfers zusammengeführt werden können. FirstTransferID muss bereits gesetzt sein, die zu prüfende TradeID kann übergeben werden.
    ''' </summary>
    ''' <param name="SecondTransferID">ID des zu prüfenden zweiten Trades/Transfers. Wenn Nothing wird gegen die bereits hinterlegte SecondTransferID geprüft</param>
    ''' <param name="ResultCode">0 = OK / negative Zahl = diverse Fehlermöglichkeiten</param>
    ''' <param name="ResultMessage">Leerstring, wenn OK / Sonst Fehlermeldung.</param>
    ''' <returns>True, wenn beide Trades/Transfers zusammengeführt werden können, sonst False</returns>
    Public Function CheckTradeMergability(Optional ByVal SecondTransferID As Long = -1, _
                                          Optional ByRef ResultCode As Long = 0, _
                                          Optional ByRef ResultMessage As String = Nothing) As Boolean
        Dim Result As Boolean = False
        If SecondTransferID <> -1 Then
            Me.SecondTransferID = SecondTransferID
        End If
        If _1stTradeRow Is Nothing OrElse _2ndTradeRow Is Nothing Then
            ResultCode = -1
            ResultMessage = "Transferdaten konnte nicht abgerufen werden"
        ElseIf _1stTradeRow.TradeTypID <> CInt(DBHelper.TradeTypen.Transfer) OrElse _2ndTradeRow.TradeTypID <> CInt(DBHelper.TradeTypen.Transfer) Then
            ResultCode = -2
            ResultMessage = "Angegebene Trade-ID ist kein Transfer"
        ElseIf _1stTradeRow.ZielKontoID <> _1stTradeRow.QuellKontoID OrElse _2ndTradeRow.ZielKontoID <> _2ndTradeRow.QuellKontoID _
            OrElse _1stTradeRow.ZielKontoID <> _2ndTradeRow.QuellKontoID _
            OrElse _1stTradeRow.ZielKontoID = DBHelper.Konten.Unbekannt Then
            ResultCode = -3
            ResultMessage = "Die ausgewählten Transfers müssen identische Quell- bzw. Zielkonten haben"
        ElseIf GetResultingPlatform(_1stTradeRow.ZielPlattformID, _2ndTradeRow.ZielPlattformID) = PlatformManager.Platforms.Unknown _
            OrElse GetResultingPlatform(_1stTradeRow.QuellPlattformID, _2ndTradeRow.QuellPlattformID) = PlatformManager.Platforms.Unknown _
            OrElse GetResultingPlatform(_1stTradeRow.ZielPlattformID, _2ndTradeRow.ZielPlattformID) = GetResultingPlatform(_1stTradeRow.QuellPlattformID, _2ndTradeRow.QuellPlattformID) Then
            ResultCode = -4
            ResultMessage = "Angegebene Transfers passen hinsichtlich ihrer Ziel- oder Quell-Plattformen nicht zueinander"
        ElseIf _1stTradeRow.ZielBetrag <> _1stTradeRow.QuellBetrag OrElse _2ndTradeRow.ZielBetrag <> _2ndTradeRow.QuellBetrag Then
            ResultCode = -5
            ResultMessage = "Die ausgewählten Transfers müssen jeweils identische Quell- und Zielbeträge haben"
        ElseIf Math.Abs(_1stTradeRow.ZielBetrag - _2ndTradeRow.ZielBetrag) > Math.Max(_1stTradeRow.ZielBetrag, _2ndTradeRow.ZielBetrag) * _ToleranceValue _
            OrElse Math.Abs(_1stTradeRow.QuellBetrag - _2ndTradeRow.QuellBetrag) > Math.Max(_1stTradeRow.QuellBetrag, _2ndTradeRow.QuellBetrag) * _ToleranceValue Then
            ResultCode = -6
            ResultMessage = String.Format("Angegebene Transfers weichen hinsichtlich ihrer Beträge mehr als {0}% voneinander ab", (_ToleranceValue * 100).ToString("#,##0"))
        Else
            Result = True
            ResultCode = 0
            ResultMessage = ""
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Fasst beide Trades zusammen und speichert die Änderungen in der DB. Führt vorher KEINE Prüfung mittels CheckTradeMergability aus! 
    ''' </summary>
    ''' <param name="SecondTransferID">Optional: ID des zweiten Transfers</param>
    ''' <param name="FeeTrade">TradesRow des ggf. angelegten Gebühren-Transfers / Nothing, wenn keine Gebühren angelegt wurden.</param>
    ''' <returns>TradesRow des zusammengeführten Transfers</returns>
    Public Function MergeTrades(Optional ByVal SecondTransferID As Long = -1, _
                                Optional ByRef FeeTrade As TradesRow = Nothing) As TradesRow
        Dim Result As TradesRow = Nothing
        If SecondTransferID <> -1 Then
            Me.SecondTransferID = SecondTransferID
        End If
        ' 2. Transfer umschreiben und "entwerten"
        With _2ndTradeRow
            .RefTradeID = _1stTradeRow.ID
            .Entwertet = True
        End With
        ' 1. Transfer umschreiben
        With _1stTradeRow
            .QuellPlattformID = GetResultingPlatform(.QuellPlattformID, _2ndTradeRow.QuellPlattformID)
            .ZielPlattformID = GetResultingPlatform(.ZielPlattformID, _2ndTradeRow.ZielPlattformID)
            If .InTradeID = 0 AndAlso _2ndTradeRow.InTradeID > 0 Then
                .InTradeID = _2ndTradeRow.InTradeID
            End If
            If .OutTradeID = 0 AndAlso _2ndTradeRow.OutTradeID > 0 Then
                .OutTradeID = _2ndTradeRow.OutTradeID
            End If
            .Zeitpunkt = If(.Zeitpunkt < _2ndTradeRow.Zeitpunkt, .Zeitpunkt, _2ndTradeRow.Zeitpunkt)
            .ZeitpunktZiel = If(.ZeitpunktZiel < _2ndTradeRow.ZeitpunktZiel, _2ndTradeRow.ZeitpunktZiel, .ZeitpunktZiel)
            If .ZielBetrag <> _2ndTradeRow.ZielBetrag Then
                ' Beträge der Transfers weichen voneinander ab -> Gebühren-Trade anlegen
                FeeTrade = _TradesTable.NewTradesRow
                With FeeTrade
                    .TradeTypID = DBHelper.TradeTypen.Gebühr
                    .QuellPlattformID = _1stTradeRow.QuellPlattformID
                    .ZielPlattformID = .QuellPlattformID
                    .QuellBetrag = Math.Max(_1stTradeRow.ZielBetrag, _2ndTradeRow.ZielBetrag) - Math.Min(_1stTradeRow.ZielBetrag, _2ndTradeRow.ZielBetrag)
                    .QuellBetragNachGebuehr = .QuellBetrag
                    .ZielBetrag = .QuellBetrag
                    .QuellKontoID = _1stTradeRow.ZielKontoID
                    .ZielKontoID = _KontenTable.FindByID(.QuellKontoID).GebuehrKontoID
                    .Zeitpunkt = _1stTradeRow.Zeitpunkt
                    .ZeitpunktZiel = _1stTradeRow.ZeitpunktZiel
                    .SourceID = _1stTradeRow.SourceID & "/fee"
                    .WertEUR = 0
                    .BetragNachGebuehr = 0
                    .Entwertet = False
                    .Steuerirrelevant = 0
                    .Info = "Gebühr für Coin-Transfer (Eintrag automatisch erstellt)" & IIf(_1stTradeRow.Info.Length > 0, " / " & _1stTradeRow.Info, "")
                    .OutTradeID = 0
                    .InTradeID = 0
                    .RefTradeID = _1stTradeRow.ID
                    .ImportPlattformID = _1stTradeRow.ImportPlattformID
                    .ImportID = _1stTradeRow.ImportID
                End With
                _TradesTable.AddTradesRow(FeeTrade)
            End If
            .QuellBetrag = Math.Max(.ZielBetrag, _2ndTradeRow.ZielBetrag)
            .QuellBetragNachGebuehr = .QuellBetrag
            .ZielBetrag = .QuellBetrag
            .BetragNachGebuehr = Math.Min(.ZielBetrag, _2ndTradeRow.ZielBetrag)
        End With
        _TradesTableAdapter.Update(_TradesTable)
        Return _1stTradeRow
    End Function

    ''' <summary>
    ''' Setzt den Speicher für gespeicherte Transfers zurück.
    ''' </summary>
    Public Sub ResetTransfers()
        _1stTransferID = 0
        _2ndTransferID = 0
        _1stTradeRow = Nothing
        _2ndTradeRow = Nothing
    End Sub

    ''' <summary>
    ''' Zeigt eine Erklärung zur Vorgehensweise beim Zusammenführen von Transfers an
    ''' </summary>
    Public Function ShowAdvice() As DialogResult
        Return MsgBoxEx.ShowWithNotAgainOption("MergeTransfers", DialogResult.OK, _
                                               "Beim Import von Trade-Daten versucht der " & Application.ProductName & " Ein- und Auszahlungen plattformübergreifend zu erkennen " & _
                                               "und zu Transfers zusammenzufassen. Wenn die automatische Erkennung nicht erfolgreich war, befinden sich ggf. " & _
                                               "zwei Transfer-Einträge in den Trade-Daten, die zu einem einzigen Transfer zusammengefasst werden können." & Environment.NewLine & Environment.NewLine & _
                                               "Beispiel:" & Environment.NewLine & "Transfer A = 5,48 BTC von Plattform 'unbekannt' nach 'Kraken' " & Environment.NewLine & _
                                               "Transfer B = 5,50 BTC von Plattform 'Bitcoin.de' nach 'unbekannt'" & Environment.NewLine & _
                                               "Transfer A und Transfer B können zu einem einzigen Transfer C zusammengefasst werden:" & Environment.NewLine & _
                                               "Transfer C = 5,50 BTC von Plattform 'Bitcoin.de' nach 'Kraken', 0,02 BTC Gebühr" & Environment.NewLine & Environment.NewLine & _
                                               "Genau hierfür dient das Zusammenlegen von Transfers." & Environment.NewLine & Environment.NewLine & _
                                               "Sie haben nun den ersten von zwei zusammenzufassenden Transfers ausgewählt. Im nächsten Schritt wählen Sie bitte den " & _
                                               "Zweiten aus und klicken Sie anschließend auf 'Zusammenfassen'." & Environment.NewLine & Environment.NewLine & _
                                               "Bitte beachten Sie, dass nur solche Transfers zusammengefasst werden können, die bzgl. Betrag oder Ziel bzw. Ursprung " & _
                                               "zueinander passen.", "Transfers zusammenführen", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                               MessageBoxDefaultButton.Button1)
    End Function

    Public Sub New()
        ResetTransfers()
        _ToleranceValue = 0.2
    End Sub

    ''' <summary>
    ''' Initialisiert die Hilfsklasse für das Zusammenführen von Trades mit aktivierter Datenanbindung
    ''' </summary>
    ''' <param name="TradesTableAdapter">TableAdapter für Trades-DataTable</param>
    ''' <param name="DataSet">DataSet für die Trades-Tabelle</param>
    Public Sub New(TradesTableAdapter As CoinTracerDataSetTableAdapters.TradesTableAdapter, _
                   DataSet As CoinTracerDataSet)
        Me.New()
        _TradesTable = DataSet.Trades
        _KontenTable = DataSet.Konten
        _PlattformenTable = DataSet.Plattformen
        _TradesTableAdapter = TradesTableAdapter
    End Sub

    ''' <summary>
    ''' Liefert einen beschreibenden String für die übergebene Trade-ID zurück
    ''' </summary>
    Private Function GetTransferDescription(TradeRow As TradesRow) As String

        Try
            Dim Description As String
            If TradeRow IsNot Nothing Then
                Dim KontenRow As CoinTracerDataSet.KontenRow = _KontenTable.FindByID(TradeRow.QuellKontoID)
                Dim QuellPlattformenRow As CoinTracerDataSet.PlattformenRow = _PlattformenTable.FindByID(TradeRow.QuellPlattformID)
                Dim ZielPlattformenRow As CoinTracerDataSet.PlattformenRow = _PlattformenTable.FindByID(TradeRow.ZielPlattformID)
                Description = String.Format("(ID {0}) {1} | {4} {5} | {2} → {3}", _
                                            TradeRow.ID,
                                            TradeRow.Zeitpunkt.ToString("yyyy-MM-dd hh:mm:ss"), _
                                            QuellPlattformenRow.Bezeichnung, _
                                            ZielPlattformenRow.Bezeichnung, _
                                            TradeRow.QuellBetrag, _
                                            KontenRow.Bezeichnung)
            Else
                Description = "- unbekannt -"
            End If
            Return Description
        Catch ex As Exception
            Return "- Fehler bei der Ermittlung -"
        End Try
    End Function

    ''' <summary>
    ''' Liefert die resultierende Platform zurück, die sich aus den beiden übergebenen ergibt.
    ''' </summary>
    Private Function GetResultingPlatform(Platform1 As PlatformManager.Platforms, Platform2 As PlatformManager.Platforms) As PlatformManager.Platforms
        If Platform1 = PlatformManager.Platforms.Unknown Then
            Return Platform2
        ElseIf Platform2 = PlatformManager.Platforms.Unknown Then
            Return Platform1
        ElseIf Platform1 <> Platform2 Then
            Return PlatformManager.Platforms.Unknown
        Else
            Return Platform1
        End If
    End Function

End Class

#End Region

''' <summary>
''' Klasse für das Berechnen von Gewinnen zu Cryptocoin-Verkäufen und -Transaktionen
''' </summary>
''' <remarks></remarks>
Public Class TradeValueManager
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If _CtDB IsNot Nothing Then
                    _CtDB.Dispose()
                End If
                If _DS IsNot Nothing Then
                    _DS.Dispose()
                End If
                ProgressWaitManager.CloseProgress()
            End If

            ' TO DO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TO DO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TO DO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Private Structure CalculationParameters
        Public CalculationID As Long
        Public LongTermInterval As String
        Public TradeIDsCleared As List(Of Long)
        Public TradeIDsRetries As List(Of Long)
        Public TradesTb As TradesDataTable
        Public TxTA As TradeTxTableAdapter
        Public TxTb As TradeTxDataTable
        Public KontenTb As KontenDataTable
        Public PlattformenTb As PlattformenDataTable
        Public TradeValuesTA As TradesWerteTableAdapter
        Public TradeValuesTb As TradesWerteDataTable
        Public AssignTrades As List(Of TradesRow)
        Public FilterKontoID As Long
        Public FilterPlattformID As Long
        Public FilterZeitpunkt As Date
        Public FilterTxFunction As Func(Of TradeTxRow, Boolean)
        Public SortTxFunction As Func(Of TradeTxRow, Double)
    End Structure

    Private Class LongTermTaxPeriod
        Public IntervalUnit As DateInterval
        Public IntervalValue As Integer
        Public Sub New(ByRef SQLPeriodString As String)
            If SQLPeriodString.Contains("days") Then
                IntervalUnit = DateInterval.Day
            ElseIf SQLPeriodString.Contains("months") Then
                IntervalUnit = DateInterval.Month
            Else
                IntervalUnit = DateInterval.Year
            End If
            Try
                IntervalValue = CInt(SQLPeriodString.Substring(0, SQLPeriodString.IndexOf(" ")))
            Catch ex As Exception
                IntervalValue = 1
            End Try
        End Sub
    End Class

    Public Enum LossTradeActionTypes
        Add = 0
        Delete
    End Enum

    ''' <summary>
    ''' Used for assigning OutTrades to InTrades: if an amount lesser or equal this constant is unassigned, the OutTrade is regarded fully cleared
    ''' </summary>
    Private Const AMOUNT_TOLERANCE = 0.00000009D

    Private _Parentform As frmMain
    Private _TCS As frmMain.TaxCalculationSettings

    ' The following is needed only while assigning OutCoins to InCoins and is held class-wide for performance reasons
    Private _LongTermInterval As LongTermTaxPeriod
    Private _CVS As CoinValueStrategy
    Private _CalcParams As CalculationParameters

    ''' <summary>
    ''' Initalisiert die Progressform für dieses Modul
    ''' </summary>
    ''' <param name="Message">Startnachricht, die angezeigt werden soll.</param>
    Private Sub InitProgressForm(Optional Message As String = "Starte Berechnung. Bitte warten Sie...")
        Try
            DestroyProgressForm()
            ProgressWaitManager.ShowProgress(_Parentform)
            ProgressWaitManager.UpdateProgress(1, Message)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DestroyProgressForm()
        Try
            ProgressWaitManager.CloseProgress()
            'If _WW IsNot Nothing Then
            '    DestroyProgressForm()
            '    _WW = Nothing
            'End If
        Catch ex As Exception
            ' no matter...
        End Try
    End Sub

    Public Event GainingsCutOffDayChanged As EventHandler(Of GainingsCutOffDayEventArgs)

    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _DS.Connection
        End Get
    End Property

    Private _DS As CoinTracerDataSet
    Public ReadOnly Property DataSet() As CoinTracerDataSet
        Get
            Return _DS
        End Get
    End Property

    Private _CtDB As DBHelper
    Public ReadOnly Property CointrackerDB() As DBHelper
        Get
            Return _CtDB
        End Get
    End Property

    Private _SzenarioID As Long
    Public Property SzenarioID() As Long
        Get
            Return _SzenarioID
        End Get
        Set(ByVal value As Long)
            _SzenarioID = value
            RaiseEvent GainingsCutOffDayChanged(Me, New GainingsCutOffDayEventArgs(GetGainingsCutOffDay))
        End Set
    End Property

    ''' <summary>
    ''' Prüft, wie oft der angefragte Business-Case in den Trade-Daten vorkommt.
    ''' </summary>
    ''' <param name="CoinBusinessCase">Coin-Business-Case, auf den die Trade-Daten geprüft werden sollen</param>
    ''' <returns>0, wenn der Business-Case nicht vorkommt, sonst Anzahl der gefundenen Trades</returns>
    Public Function CoinBusinessCasePresent(CoinBusinessCase As CoinBusinessCases) As Long
        Dim DBO As DBObjects
        Dim SQL As String
        Select Case CoinBusinessCase
            Case CoinBusinessCases.SellForFiat
                ' Coin-Verkäufe für Fiat-Währungen
                SQL = "t.TradeTypID=4 and zk.IstFiat=1"
            Case CoinBusinessCases.BuyForCoins
                ' Coin-Käufe mit Coins als Zahlung
                SQL = "t.TradeTypID=3 and not qk.IstFiat"
            Case CoinBusinessCases.Withdraw
                ' Abbuchung von Coins nach extern
                SQL = "t.TradeTypID=5 and not zp.Eigen"
            Case CoinBusinessCases.TransferExchangeToExchange
                ' Transfer Börse zu Börse
                SQL = "t.TradeTypID=5 and qp.Boerse and zp.Boerse"
            Case CoinBusinessCases.TransferExchangeToWallet
                ' Transfer Börse zu Wallet
                SQL = "t.TradeTypID=5 and qp.Boerse and not zp.Boerse and zp.Eigen"
            Case CoinBusinessCases.TransferWalletToExchange
                ' Transfer Wallet zu Börse
                SQL = "t.TradeTypID=5 and not qp.Boerse and qp.Eigen and zp.Boerse"
            Case Else
                SQL = "0"
        End Select

        Try
            DBO = New DBObjects(String.Format("select t.ID from Trades t " &
                                "inner join Konten qk on qk.ID=t.QuellKontoID " &
                                "inner join Konten zk on zk.ID=t.ZielKontoID " &
                                "inner join Plattformen qp on qp.ID=t.QuellPlattformID " &
                                "inner join Plattformen zp on zp.ID=t.ZielPlattformID " &
                                "where {0} and not Entwertet limit 1", SQL), Me.Connection)
            If DBO IsNot Nothing AndAlso DBO.DataTable IsNot Nothing AndAlso DBO.DataTable.Rows.Count > 0 Then
                Return DBO.DataTable.Rows.Count
            Else
                Return 0
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Calculated total of gainings/losses within the given period
    ''' </summary>
    ''' <param name="TaxableGainings">Returns the total of taxable gainings</param>
    ''' <param name="PlatformGainings">Returns the total of gainings only concerning the given PlatformIDs</param>
    ''' <returns>Total of gainings for the period</returns>
    Public Function CalculateGainings(ByVal FromDate As Date,
                                      ByVal ToDate As Date,
                                      Optional ByRef TaxableGainings As Decimal = -1,
                                      Optional ByVal PlatformIDs As String = "",
                                      Optional ByRef PlatformGainings As Decimal = -1) As Decimal
        Try
            Dim GainingsTA As New VW_GainingsTableAdapter
            Dim GainingsTB As New VW_GainingsDataTable
            GainingsTA.Fill(GainingsTB, FromDate, ToDate.AddDays(1), _SzenarioID, "," & PlatformIDs & ",")
            Dim GainingsRow As VW_GainingsRow = GainingsTB.Rows(0)
            If TaxableGainings <> -1 Then
                TaxableGainings = GainingsRow.Gewinn_EUR_steuerpflichtig
            End If
            If PlatformIDs = "" Then
                PlatformGainings = GainingsRow.Gesamtgewinn_EUR
            Else
                PlatformGainings = GainingsRow.Gewinn_EUR_Plattformen
            End If
            Return GainingsRow.Gesamtgewinn_EUR
        Catch ex As Exception
            Return 0
        End Try

    End Function

    ''' <summary>
    ''' Gibt den Tag zurück, bis zu dem Gewinne berechnet wurden
    ''' </summary>
    Public Function GetGainingsCutOffDay() As Date
        Dim KalkTA As New KalkulationenTableAdapter
        Dim KalkTbl As New KalkulationenDataTable
        If KalkTA.FillBySQL(KalkTbl, "where SzenarioID=" & _SzenarioID & " order by ID desc limit 1") > 0 Then
            Return DateAdd(DateInterval.Day, -1, KalkTbl(0).Zeitpunkt)
        Else
            Return DATENULLVALUE
        End If
        KalkTA.Dispose()
    End Function

    ''' <summary>
    ''' Gibt die Anzahl offener Transfer-Aktionen (bis zu einem Stichtag) zurück
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOpenTransfers(Optional ByVal ToDate As Date = DATENULLVALUE) As Long
        Dim DBO As DBObjects
        Dim SQL As String = ""
        If ToDate <> DATENULLVALUE Then
            SQL = " and Zeitpunkt <= '" & ToDate.ToString("yyyy-MM-dd") & "'"
        End If
        DBO = New DBObjects("select count(*) Anzahl from Trades where TradeTypID=5 " &
                            "and (QuellPlattformID=0 or ZielPlattformID=0)" & SQL, _DS.Connection)
        If DBO IsNot Nothing Then
            If DBO.DataTable.Rows.Count > 0 Then
                Return DBO.DataTable.Rows(0)("Anzahl")
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function


    Public Sub New(SQLiteConnection As SQLite.SQLiteConnection,
                   ParentForm As frmMain)
        _CtDB = New DBHelper(SQLiteConnection)
        _DS = New CoinTracerDataSet(SQLiteConnection)
        ' Parentform setzen
        _Parentform = ParentForm
        SzenarioID = 0
        _LastUnclearSpendings = 0
        _TCS = ParentForm.TaxReportSettings()
    End Sub


    ''' <summary>
    ''' Gibt die Kalkulationen-Tabelle zurück
    ''' </summary>
    ''' <remarks></remarks>
    Private Function GetKalkulationenTable() As KalkulationenDataTable
        Dim TA As New KalkulationenTableAdapter
        Dim TB As New KalkulationenDataTable
        TA.FillBySQL(TB, "where SzenarioID=" & _SzenarioID & " order by Zeitpunkt desc")
        Return TB
    End Function

    ''' <summary>
    ''' Löscht die zu einer bestimmten Kalkulation gehörenden Einträge
    ''' </summary>
    Private Sub RollbackCalculation(ByVal KalkulationID As Long)
        Try
            With New TradeTxTableAdapter
                .DeleteByKalkulationID(KalkulationID)
                .RevertByKalkulationID(KalkulationID)
            End With
            With New KalkulationenTableAdapter
                .DeleteByID(KalkulationID)
            End With
        Catch ex As Exception
            Debug.Print(String.Format("Error on RollbackCalculation({0})", KalkulationID))
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Setzt die Gewinnberechnung bis zum angegebenen Stichtag (einschl.) zurück, indem alle Kalkulationen, deren Stichzeitpunkte
    ''' nach dem übergebenen Datum liegen, zurückgesetzt werden. Löst das GainingsCutOffDayChanged-Event aus!
    ''' </summary>
    ''' <param name="ToDate">Zeitpunkt, bis zu dem zurückgesetzt werden muss</param>
    ''' <param name="DeleteCurrentScenario">True, wenn alle Kalkulationen des aktuellen Szenarios gelöscht werden sollen</param>
    ''' <param name="DeleteAllScenarios">True, wenn die Kalkulationen aller Szenarien gelöscht werden sollen</param>
    Public Sub RollbackCalculation(ToDate As Date,
                                   Optional DeleteCurrentScenario As Boolean = False,
                                   Optional DeleteAllScenarios As Boolean = False)
        Dim TA As New KalkulationenTableAdapter
        Dim KalkTB As New KalkulationenDataTable
        Dim WhereSQL As String = ""
        If Not DeleteAllScenarios Then
            WhereSQL = String.Format("where SzenarioID={0}", SzenarioID)
        End If
        If Not DeleteCurrentScenario Then
            WhereSQL &= If(WhereSQL.Length = 0, "where ", " and ") & String.Format("Zeitpunkt>'{0}'", ToDate.ToString("yyyy-MM-dd"))
        End If
        If TA.FillBySQL(KalkTB, WhereSQL & " order by Zeitpunkt desc") > 0 Then
            Dim Row As KalkulationenRow
            For Each Row In KalkTB.Rows
                RollbackCalculation(Row.ID)
            Next
            RaiseEvent GainingsCutOffDayChanged(Me, New GainingsCutOffDayEventArgs(GetGainingsCutOffDay))
        End If
        TA.Dispose()
    End Sub

    ''' <summary>
    ''' Prüft, ob im angefragten Zeitraum Trades in einer anderen Währung als EUR vorhanden sind
    ''' </summary>
    ''' <param name="DateTo">Tag, ab dem gesucht wird - einschließlich</param>
    ''' <param name="DateFrom">Tag, bis zu dem gesucht wird - Achtung: ausschließlich!</param>
    ''' <returns>Anzahl der Fremdwährungs-Trades</returns>
    Public Function CheckNonEurTrades(ByVal DateFrom As Date, DateTo As Date) As Integer
        Return CheckNonEurTrades(String.Format("Zeitpunkt >= '{0}' and Zeitpunkt < '{1}'",
                                          DateFrom.ToString("yyyy-MM-dd"),
                                          DateTo.ToString("yyyy-MM-dd")))
    End Function

    ''' <summary>
    ''' Prüft, ob im angefragten Zeitraum Trades in einer anderen Währung als EUR vorhanden sind
    ''' </summary>
    ''' <param name="WhereSql">Teil des SQL-Statements, das in die Where-Clause eingebaut wird. Beispiel: "Zeitpunkt > '2014-01-01'"</param>
    ''' <returns>Anzahl der Fremdwährungs-Trades</returns>
    Public Function CheckNonEurTrades(ByVal WhereSql As String) As Integer
        Dim SQL As String = String.Format("select count(*) Anzahl from Trades where " &
                                          "((TradeTypID = 3 and QuellKontoID in (102)) or (TradeTypID = 4 and ZielKontoID in (102))) " &
                                          "and {0}", If(WhereSql = "", "1", WhereSql))
        Dim DBO As New DBObjects(SQL, _CtDB.Connection)
        If DBO.DataTable IsNot Nothing Then
            Return DBO.DataTable.Rows(0)("Anzahl")
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Füllt bei Trades in Fremdwährungen den jeweiligen Wert in der Steuer-Währung (aktuell nur EUR)
    ''' </summary>
    Public Sub SetTaxCurrencyValues()
        Dim SQL As String
        Cursor.Current = Cursors.WaitCursor
        Try
            ' Trades in USD -> EUR-Wert behandeln
            SQL = "update Trades set WertEUR=coalesce((select round((t.BetragNachGebuehr / t.{1}Betrag) * t.{2}Betrag / k.ZielBetrag , 8) " &
                  "from Trades t inner join Kurse k on (date(k.Zeitpunkt)=date(t.Zeitpunkt) and k.ZielKontoID=t.{2}KontoID) " &
                  "where t.ID=Trades.ID), 0) " &
                  "where TradeTypID{0} and {2}KontoID=102 and WertEUR=0 and {2}Betrag>0"
            ' Käufe
            _DS.ExecuteSQL(String.Format(SQL, "=3", "Ziel", "Quell"))
            ' Verkäufe & Einzahlungen & Transfers
            _DS.ExecuteSQL(String.Format(SQL, " in (1,4,5)", "Ziel", "Ziel"))
            ' Auszahlungen
            _DS.ExecuteSQL(String.Format(SQL, "=2", "Quell", "Quell"))
        Catch ex As Exception
            Cursor.Current = Cursors.Default
            Throw
        End Try
        Cursor.Current = Cursors.Default
    End Sub

    ''' <summary>
    ''' Schreibt einen neuen Eintrag in Kalkulationen; berücksichtigt, ob ggf. ältere Kalkulationen
    ''' aufgrund abweichender Strategien oder zukünftiger Enddaten rückgängig gemacht werden müssen.
    ''' </summary>
    ''' <param name="AllCoinValueStrategies">Strategie-Strings aller Business-Cases, Pipe-separiert</param>
    ''' <param name="UntilTime">Optional: Zeitpunkt, bis zu dem Transaktionen verarbeitet werden. Wenn leer, dann bis einschl. heute</param>
    ''' <returns>ID des neuen Eintrags in Kalkulationen</returns>
    Private Function WriteCalculationEntry(AllCoinValueStrategies As String,
                                        Optional UntilTime As Date = DATENULLVALUE) As Long
        Dim KalkTA As New KalkulationenTableAdapter
        Dim KalkTB As New KalkulationenDataTable
        ' Pauschal alle Kalkulationen mit späterem oder gleichem Enddatum zurücksetzen
        KalkTA.FillBySQL(KalkTB, String.Format("where SzenarioID={0} and Zeitpunkt >= '{1}' order by Zeitpunkt desc", _SzenarioID, ConvertToSqlDate(UntilTime)))
        For i As Integer = 1 To KalkTB.Count
            Me.RollbackCalculation(KalkTB(i - 1).ID)
        Next
        ' Prüfen, ob die letzte gültige Berechnung mit anderen Einstellungen durchgeführt wurde
        KalkTA.FillBySQL(KalkTB, String.Format("where SzenarioID={0} order by ID limit 1", _SzenarioID))
        If KalkTB.Count > 0 AndAlso Not KalkTB(0).IsCVSNull AndAlso KalkTB(0).CVS <> AllCoinValueStrategies Then
            MsgBoxEx.PatchMsgBox(New String() {"Neuberechnung", "Mit neuen Einstellungen fortfahren"})
            If MessageBox.Show("Achtung: Sie haben bei der letzten Gewinn-/Verlustberechnung andere Berechnungseinstellungen " &
                               "gewählt. Sollen die letzten Berechnungen zurückgesetzt und mit den aktuellen Einstellungen " &
                               "wiederholt werden oder möchten Sie die neuen Einstellungen nur auf die neuen Transaktionen " &
                               "anwenden?" & Environment.NewLine & Environment.NewLine &
                               "Wählen Sie 'Neuberechnung', um die letzten Gewinnberechnungen mit den aktuellen Einstellungen " &
                               "zu wiederholen oder 'Mit neuen Einstellungen fortfahren', um lediglich neue Transaktionen " &
                               "mit den aktuellen Einstellungen zu verarbeiten.", "Geänderte Berechnungseinstellungen",
                               MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.Retry Then
                Me.RollbackCalculation(UntilTime, True)
            End If
        End If
        If KalkTB.Count > 0 OrElse DateAdd(DateInterval.Day, -1, UntilTime) <> Me.GetGainingsCutOffDay Then
            ' Neuen Eintrag schreiben
            KalkTA.Insert(_SzenarioID, UntilTime, AllCoinValueStrategies)
            KalkTA.FillBySQL(KalkTB, "order by ID desc limit 1")
            Return KalkTB(0).ID
        Else
            ' nichts zu tun...
            Return 0
        End If
        KalkTA.Dispose()
    End Function

    Private _LastUnclearSpendings As Integer
    ''' <summary>
    ''' Number of unclear spendings, resulting from last execution of CalculateOutCoinsToInCoins
    ''' </summary>
    Public ReadOnly Property LastUnclearSpendings() As Integer
        Get
            Return _LastUnclearSpendings
        End Get
    End Property

    ''' <summary>
    ''' Calculates the source and tax value of all outgoing coins to the given timestamp by filling up the TradeTx table
    ''' </summary>
    ''' <param name="UntilTime">Timestamp for the calculation. All outgoing coins up to this date will be calculated.</param>
    ''' <returns>Number of clarified outcoin entries</returns>
    Public Function CalculateOutCoinsToInCoins(Optional UntilTime As Date = DATENULLVALUE) As Integer
        Const CALCULATIONCHUNKSECONDS = 20          ' Seconds until the data is written to the database and the calculation starts over again

        Dim RowTotalOutRows As Long = 0
        Dim FromTime As DateTime
        Dim AllWritten As Boolean
        Dim TotalOutRows As Long

        Cursor.Current = Cursors.WaitCursor
        InitProgressForm(My.Resources.MyStrings.calcGainingsInitProgressMessage)
        Try
            ' Write new calculation entry, rollback other calculations if needed
            If UntilTime = DATENULLVALUE Then
                UntilTime = Now
            End If
            _CalcParams.CalculationID = WriteCalculationEntry(_TCS.ToString, UntilTime)
            If _CalcParams.CalculationID > 0 Then       ' Nothing to do if CalculationID = 0!

                FromTime = DATENULLVALUE
                _LastUnclearSpendings = 0
                ' Get open trade rows
                With New TradesTableAdapter With {.ClearBeforeFill = False}
                    _CalcParams.TradesTb = .GetOpenTradesBySzenarioID(_SzenarioID, UntilTime)
                End With
                If (_CalcParams.TradesTb.Rows.Count <> 0) Then

                    ' Fill some base tables
                    With New PlattformenTableAdapter
                        _CalcParams.PlattformenTb = .GetData()
                    End With
                    With New KontenTableAdapter
                        _CalcParams.KontenTb = .GetData()
                    End With
                    _LongTermInterval = New LongTermTaxPeriod(_TCS.LongTermPeriodSQL)

                    ' Initialize TradeTx data
                    _CalcParams.TxTA = New TradeTxTableAdapter With {.ClearBeforeFill = False}
                    _CalcParams.TxTb = _CalcParams.TxTA.GetBySzenarioID(_SzenarioID)
                    _CalcParams.TxTb.MaxID = _CalcParams.TxTA.GetMaxID()

                    ' Initialize TradeTx filter function
                    _CVS = _TCS.CoinValueStrategy
                    If _CVS.WalletAware Then
                        _CalcParams.FilterTxFunction = Function(r As TradeTxRow)
                                                           Return (Not r.Entwertet AndAlso ((Decimal.Compare(r.Betrag, 0) > 0) AndAlso ((r.PlattformID = _CalcParams.FilterPlattformID) AndAlso (r.KontoID = _CalcParams.FilterKontoID)))) AndAlso (DateTime.Compare(r.Zeitpunkt, _CalcParams.FilterZeitpunkt) <= 0)
                                                       End Function
                    Else
                        _CalcParams.FilterTxFunction = Function(r As TradeTxRow)
                                                           Return ((Not r.Entwertet AndAlso ((Decimal.Compare(r.Betrag, 0) > 0) AndAlso (r.KontoID = _CalcParams.FilterKontoID))) AndAlso (DateTime.Compare(r.Zeitpunkt, _CalcParams.FilterZeitpunkt) <= 0))
                                                       End Function
                    End If

                    ' Initialize TradeTx sort function
                    Select Case _CVS.ConsumptionStrategy
                        Case CoinValueStrategies.YoungestFirst
                            _CalcParams.SortTxFunction = Function(r As TradeTxRow)
                                                             Return (DATEMAXVALUE - r.KaufZeitpunkt).TotalMinutes
                                                         End Function
                        Case CoinValueStrategies.CheapestFirst
                            _CalcParams.SortTxFunction = Function(r As TradeTxRow)
                                                             Return Decimal.ToDouble(r.WertEUR) / Decimal.ToDouble(r.Betrag)
                                                         End Function
                        Case CoinValueStrategies.MostExpensiveFirst
                            _CalcParams.SortTxFunction = Function(r As TradeTxRow)
                                                             Return -Decimal.ToDouble(r.WertEUR) / Decimal.ToDouble(r.Betrag)
                                                         End Function
                        Case Else
                            ' FiFo
                            _CalcParams.SortTxFunction = Function(r As TradeTxRow)
                                                             Return (r.KaufZeitpunkt - DATENULLVALUE).TotalMinutes
                                                         End Function
                    End Select

                    AllWritten = False
                    _CalcParams.AssignTrades = New List(Of TradesRow)
                    _CalcParams.TradeIDsCleared = New List(Of Long)
                    _CalcParams.TradeIDsRetries = New List(Of Long)
                    TotalOutRows = _CalcParams.TradesTb.Rows.Count
                    ProgressWaitManager.WithCancelOption = True

                    Dim RowCount As Long

                    Do Until AllWritten

                        FromTime = Now
                        RowCount = 0
                        AllWritten = True

                        ' Loop over every Trades entry
                        For Each Trade In _CalcParams.TradesTb

                            RowTotalOutRows += 1
                            RowCount += 1
                            ProgressWaitManager.UpdateProgress(Math.Round((RowTotalOutRows / TotalOutRows) * 97),
                                                                String.Format(MyStrings.calcGainingsProgressMessage,
                                                                                RowTotalOutRows.ToString(Import.MESSAGENUMBERFORMAT),
                                                                                TotalOutRows.ToString(Import.MESSAGENUMBERFORMAT)))
                            If _CalcParams.TradeIDsCleared.Contains(Trade.ID) Then
                                Continue For
                            End If
                            Try
                                _CalcParams.AssignTrades.Add(Trade)
                                Do Until _CalcParams.AssignTrades.Count = 0
                                    AssignTradesToTx
                                Loop
                            Catch ex As Exception
                                If (_CalcParams.AssignTrades.Count > 0) Then
                                    Debug.Print(("Error while processing Trade ID " & _CalcParams.AssignTrades(0).ID))
                                Else
                                    Debug.Print(("Error while processing Trade ID " & Trade.ID))
                                End If
                            End Try

                            If ProgressWaitManager.Canceled Then
                                ' User has cancelled the calculation
                                ProgressWaitManager.UpdateProgress(Math.Round((RowTotalOutRows / TotalOutRows) * 99),
                                                                    String.Format(MyStrings.calcGainingsFinalWriteToDbMessage, RowCount.ToString(Import.MESSAGENUMBERFORMAT)))
                                Dim KalkulationenTA As New KalkulationenTableAdapter
                                KalkulationenTA.UpdateZeitpunkt(Trade.Zeitpunkt, _CalcParams.CalculationID)
                                _CalcParams.TxTA.Update(_CalcParams.TxTb)
                                KalkulationenTA = Nothing
                                AllWritten = True
                            Else
                                If (Debugger.IsAttached OrElse ((Now - FromTime).TotalSeconds < CALCULATIONCHUNKSECONDS)) Then
                                    Continue For
                                End If
                                ' Time out: Save data to db and fetch next chunk
                                ProgressWaitManager.UpdateProgress(Math.Round((RowTotalOutRows / TotalOutRows) * 97), MyStrings.calcGainingsIntermediateWriteToDbMessage)
                                FromTime = Trade.Zeitpunkt
                                _CalcParams.TxTA.Update(_CalcParams.TxTb)
                                With New TradesTableAdapter With {.ClearBeforeFill = True}
                                    _CalcParams.TradesTb = .GetOpenTradesBySzenarioID(_SzenarioID, New Date?(UntilTime))
                                End With
                                AllWritten = False
                            End If
                        Next

                        If AllWritten Then
                            ' We are done - update TradeTx table and raise event
                            ProgressWaitManager.UpdateProgress(Math.Round((RowTotalOutRows / CDbl(TotalOutRows)) * 99),
                                                               String.Format(MyStrings.calcGainingsFinalWriteToDbMessage,
                                                                             RowCount.ToString(Import.MESSAGENUMBERFORMAT)))
                            _CalcParams.TxTA.Update(_CalcParams.TxTb)
                            RaiseEvent GainingsCutOffDayChanged(Me, New GainingsCutOffDayEventArgs(GetGainingsCutOffDay))
                        End If
                    Loop

                End If
            End If
        Catch ex As Exception
            If (_CalcParams.CalculationID > 0) Then
                RollbackCalculation(_CalcParams.CalculationID)
            End If
            Cursor.Current = Cursors.Default
            DestroyProgressForm()
            Throw
        End Try

        Cursor.Current = Cursors.Default
        DestroyProgressForm()
        Return RowTotalOutRows

    End Function

    ''' <summary>
    ''' Assigns the given Trades row to their corresponding TradeTx entries. Uses the _CalcParams structure to retrieve all parameters 
    ''' needed for the calculation. In case of not being able to assign the value of an outgoing trade completely, this routine will 
    ''' search for helpful TradeRow candidates and put these onto the _CalcParams.AssignTrades stack.
    ''' </summary>
    Private Sub AssignTradesToTx()
        Dim Trade As TradesRow = _CalcParams.AssignTrades(0)
        If Not _CalcParams.TradeIDsCleared.Contains(Trade.ID) Then
            Dim TargetIsFiat As Boolean = _CalcParams.KontenTb.FindByID(Trade.ZielKontoID).IstFiat
            Dim AmountToAssign As Decimal
            If ((Trade.TradeTypID = DBHelper.TradeTypen.Kauf) OrElse ((Trade.TradeTypID = DBHelper.TradeTypen.Transfer) And Not TargetIsFiat)) Then
                ' Buy or crypto transfer from external source: charge values
                AmountToAssign = ChargeTx(Trade)
            ElseIf ((Trade.TradeTypID = DBHelper.TradeTypen.Verkauf) OrElse ((Trade.TradeTypID = DBHelper.TradeTypen.Verlust) And Not TargetIsFiat)) Then
                ' Sell or loss: discharge values
                AmountToAssign = DischargeTx(Trade, False)
            ElseIf (Trade.TradeTypID = DBHelper.TradeTypen.TransferIntern) Then
                ' Internal crypto transfer: shift values form one platform to another
                AmountToAssign = DischargeTx(Trade, True)
            ElseIf (Trade.TradeTypID = DBHelper.TradeTypen.KaufCoin4Coin) Then
                ' Buy crypto with crypto: evaluate value of given coins and assign it to purchased coins
                Dim WertEUR As Decimal?
                AmountToAssign = DischargeTx(Trade, False, WertEUR)
                If AmountToAssign <= AMOUNT_TOLERANCE Then
                    AmountToAssign = ChargeTx(Trade, WertEUR)
                End If
            End If
            If AmountToAssign <= AMOUNT_TOLERANCE Then
                ' Assignment successful: remove Trade from stack
                _CalcParams.AssignTrades.RemoveAt(0)
                _CalcParams.TradeIDsCleared.Add(Trade.ID)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Creates a new tx ledger entry for the given TradesRow object. Always returns 0.
    ''' </summary>
    ''' <param name="Trade">The (ingoing) trade, whose coin value will be charged to the tx ledger.</param>
    ''' <param name="WertEUR">If set, WertEUR is the fiat value of the given crypto. If not set WertEUR from Trade row will be used.</param>
    ''' <returns>0.0</returns>
    Private Function ChargeTx(ByVal Trade As TradesRow,
                              ByVal Optional WertEUR As Decimal? = Nothing) As Decimal
        Dim TxRow As TradeTxRow = _CalcParams.TxTb.NewTradeTxRow
        With TxRow
            .TxID = _CalcParams.TxTb.MaxID
            .SzenarioID = _SzenarioID
            .InKalkulationID = _CalcParams.CalculationID
            .InTradeID = Trade.ID
            .TransferIDHistory = ""
            .PlattformID = Trade.ZielPlattformID
            .KontoID = Trade.ZielKontoID
            .Zeitpunkt = Trade.ZeitpunktZiel
            .KaufZeitpunkt = Trade.InZeitpunkt
            .Betrag = Trade.BetragNachGebuehr
            .WertEUR = IIf(WertEUR Is Nothing, Trade.WertEUR, WertEUR)
            .Entwertet = False
            .IstLangzeit = False
            .IstRest = False
        End With
        _CalcParams.TxTb.Rows.Add(TxRow)
        Return 0
    End Function


    ''' <summary>
    ''' Tries to discharge the tx ledger regarding the given TradesRow object. Returns 0 if successful, the remaining amount otherwise. 
    ''' </summary>
    ''' <param name="Trade">The (outgoing) trade that needs to be discharged from the tx ledger.</param>
    ''' <param name="TransferMode">True, if an internal transfer needs to be discharged. In this case, every single discharged tx ledger entry will immediately
    ''' be copied to the target platform (instead of simply being discharged)</param>
    ''' <param name="SumWertEUR">If set, this variable will hold the WertEUR equivalent of the discharged coin value (used for coin 4 coin trades).</param>
    ''' <returns>0 on success, otherwise the remaining crypto amount.</returns>
    Private Function DischargeTx(ByVal Trade As TradesRow,
                                 ByVal TransferMode As Boolean,
                                 ByRef Optional SumWertEUR As Decimal? = Nothing) As Decimal
        Dim TxDischarged As New List(Of TradeTxRow)
        Dim AmountToAssign As Decimal = Trade.QuellBetrag
        Dim InitialTxRows As Long = _CalcParams.TxTb.Rows.Count
        Dim TransferFee As Decimal = IIf(TransferMode, Trade.QuellBetrag - Trade.BetragNachGebuehr, 0)
        SumWertEUR = 0

        ' Select appropriate tx rows
        _CalcParams.FilterKontoID = Trade.QuellKontoID
        _CalcParams.FilterPlattformID = Trade.QuellPlattformID
        If _CalcParams.TradeIDsRetries.Contains(Trade.ID) Then
            ' This is the 2nd trial: enlarge the time frame for finding TradeTx entries
            _CalcParams.FilterZeitpunkt = Trade.Zeitpunkt.AddMinutes(_TCS.ToleranceMinutes)
        Else
            ' This is the 1st trial: search within normal timeframe
            _CalcParams.FilterZeitpunkt = Trade.Zeitpunkt
        End If
        Dim SelectedTx = _CalcParams.TxTb.Where(_CalcParams.FilterTxFunction).OrderBy(_CalcParams.SortTxFunction)
        For Each TxRow In SelectedTx
            If (TxRow.Betrag > AmountToAssign) Or TransferMode Then
                ' TxRow has more value than needed or we are transferring value: derive rows
                Dim RowAmountToAssign As Decimal = Math.Min(TxRow.Betrag, AmountToAssign)
                Dim DerivedTxRow As TradeTxRow = TxRow.Derive
                With DerivedTxRow
                    .ParentTxID = TxRow.TxID
                    .InKalkulationID = _CalcParams.CalculationID
                    If TransferFee > RowAmountToAssign Then
                        .Betrag = 0
                        TransferFee -= RowAmountToAssign
                    Else
                        .Betrag = RowAmountToAssign - TransferFee
                        TransferFee = 0
                    End If
                    .WertEUR = (TxRow.WertEUR / TxRow.Betrag) * RowAmountToAssign
                    SumWertEUR += .WertEUR
                    If (.InTransferID > 0) Then
                        .TransferIDHistory = DerivedTxRow.InTransferID
                    End If
                    If TransferMode Then
                        .PlattformID = Trade.ZielPlattformID
                        .InTransferID = Trade.ID
                        .IstRest = False
                    Else
                        .Entwertet = True
                        .OutTradeID = Trade.ID
                        .OutKalkulationID = .InKalkulationID
                        If (Trade.TradeTypID <> DBHelper.TradeTypen.Transfer) Then
                            .IstLangzeit = IsNotTaxable(TxRow.KaufZeitpunkt, Trade.Zeitpunkt)
                        End If
                    End If
                End With
                _CalcParams.TxTb.AddTradeTxRow(DerivedTxRow)
                If TxRow.Betrag > AmountToAssign Then
                    ' We have some remaining value
                    Dim RemainTxRow As TradeTxRow = TxRow.Derive
                    With RemainTxRow
                        .InKalkulationID = _CalcParams.CalculationID
                        .Betrag = TxRow.Betrag - AmountToAssign
                        .WertEUR = (TxRow.WertEUR / TxRow.Betrag) * .Betrag
                        .ParentTxID = TxRow.TxID
                    End With
                    _CalcParams.TxTb.AddTradeTxRow(RemainTxRow)
                End If
                TxRow.OutKalkulationID = _CalcParams.CalculationID
                TxRow.Entwertet = True
                AmountToAssign -= RowAmountToAssign
                TxDischarged.Add(TxRow)
            Else
                ' TxRow needs to be discharged completely
                With TxRow
                    .OutTradeID = Trade.ID
                    SumWertEUR += .WertEUR
                    If (Trade.TradeTypID <> DBHelper.TradeTypen.Transfer) Then
                        .IstLangzeit = IsNotTaxable(.KaufZeitpunkt, Trade.Zeitpunkt)
                    End If
                    .OutKalkulationID = _CalcParams.CalculationID
                    .Entwertet = True
                    AmountToAssign -= .Betrag
                End With
                TxDischarged.Add(TxRow)
            End If
            ' Are we done?
            If AmountToAssign <= AMOUNT_TOLERANCE Then
                Exit For
            End If
        Next
        If AmountToAssign <= AMOUNT_TOLERANCE Then
            ' Everything has been assigned, we can leave here
            Return AmountToAssign
        End If
        ' The is some remaining value...
        Dim FoundTradesRow As TradesRow = Nothing
        If _TCS.ToleranceMinutes > 0 And Not _CalcParams.TradeIDsRetries.Contains(Trade.ID) Then
            ' value could not be completely assigned. Try to find possible incoming trades within tolerance timeframe.
            FoundTradesRow = _CalcParams.TradesTb.Where(Function(r)
                                                            Return r.ZeitpunktZiel <= Trade.Zeitpunkt.AddMinutes(_TCS.ToleranceMinutes) AndAlso
                                                                r.ZeitpunktZiel >= Trade.Zeitpunkt AndAlso
                                                                r.ZielKontoID = Trade.QuellKontoID AndAlso
                                                                (r.ZielPlattformID = Trade.QuellPlattformID OrElse Not _CVS.WalletAware)
                                                        End Function).OrderBy(Function(r) r.ZeitpunktZiel).FirstOrDefault
            If FoundTradesRow IsNot Nothing Then
                ' Revert all changes made to table TradeTx
                For Each TxRow In TxDischarged
                    TxRow.Entwertet = False
                    TxRow.OutTradeID = 0
                    TxRow.IstLangzeit = False
                Next
                Do While _CalcParams.TxTb.Count > InitialTxRows
                    _CalcParams.TxTb.Rows.RemoveAt(InitialTxRows)
                Loop
                ' Put found trade onto stack
                _CalcParams.AssignTrades.Insert(0, FoundTradesRow)
                ' Mark this trade for a 2nd trial
                _CalcParams.TradeIDsRetries.Add(Trade.ID)
            End If
        End If
        If FoundTradesRow Is Nothing Then
            ' Trade could not be assigned completely: create a dummy entry in TradeTx
            Dim DummyTxRow = _CalcParams.TxTb.NewTradeTxRow
            With DummyTxRow
                .TxID = _CalcParams.TxTb.MaxID
                .SzenarioID = _SzenarioID
                .InKalkulationID = _CalcParams.CalculationID
                .InTradeID = -1
                .TransferIDHistory = ""
                .KontoID = Trade.QuellKontoID
                .Zeitpunkt = Trade.Zeitpunkt
                .KaufZeitpunkt = .Zeitpunkt
                .Betrag = AmountToAssign
                .WertEUR = 0
                .OutKalkulationID = _SzenarioID
                .IstRest = False
                If TransferMode Then
                    .PlattformID = Trade.ZielPlattformID
                    .InTransferID = Trade.ID
                    .Entwertet = False
                Else
                    .PlattformID = Trade.QuellPlattformID
                    .OutTradeID = Trade.ID
                    .Entwertet = True
                End If
            End With
            _CalcParams.TxTb.Rows.Add(DummyTxRow)
            AmountToAssign = 0
            _LastUnclearSpendings += 1
        End If
        Return AmountToAssign
    End Function


    ''' <summary>
    ''' Checks if the given time period exceeds the taxable short term interval
    ''' </summary>
    ''' <param name="InDate">Date of acquisition</param>
    ''' <param name="OutDate">Date of sale</param>
    ''' <returns>True, if the time period is smaller than the required long term period, false otherwise</returns>
    Private Function IsNotTaxable(ByVal InDate As Date,
                                  ByVal OutDate As Date) As Boolean
        Return DateAdd(_LongTermInterval.IntervalUnit, _LongTermInterval.IntervalValue, Date.FromOADate(Math.Floor(InDate.ToOADate))) <= Date.FromOADate(Math.Floor(OutDate.ToOADate))
    End Function

    ''' <summary>
    ''' Setzt oder löscht die Verlust-Trade-Einträge für eine bestimmte Plattform und einen bestimmten Zeitpunkt
    ''' </summary>
    ''' <param name="Action">Loss-Trades setzen oder löschen?</param>
    ''' <param name="Platform">Auswahl der betroffenen Plattform</param>
    ''' <param name="Timestamp">Zeitpunkt des Loss-Trades</param>
    Public Sub SetLossTrades(Action As LossTradeActionTypes,
                              Platform As PlatformManager.Platforms,
                              Optional Timestamp As Date = DATENULLVALUE)
        Try

            Select Case Action
                Case LossTradeActionTypes.Delete
                    ' Verlust-Einträge löschen
                    Dim DB As DBHelper = New DBHelper(frmMain.Connection)
                    DB.ExecuteSQL(String.Format("DELETE FROM Trades WHERE TradeTypID = {0} And QuellPlattformID = {1}",
                                                                  CInt(DBHelper.TradeTypen.Verlust), CInt(Platform)))
                Case LossTradeActionTypes.Add
                    ' Verlust-Einträge erstellen
                    Dim SQL As String = String.Format("select k.IstFiat, d.KontoID, p.Bezeichnung PlattformBezeichnung, " &
                                "round(sum(case SollHaben when 1 then Betrag else 0 end),8) Haben, " &
                                "round(sum(case SollHaben when 0 then Betrag else 0 end),8) Soll, " &
                                "round(sum(Betrag),8) Bestand " &
                                "from VW_ZugangAbgang d left join Konten k on d.KontoID = k.ID " &
                                "left join Plattformen p on d.Plattform = p.ID " &
                                "where k.Eigen And p.Boerse And d.Plattform = {0} {1}" &
                                "group by KontoID, d.Plattform", DirectCast(Platform, Integer),
                                IIf(Timestamp = DATENULLVALUE, "", "And d.Zeitpunkt < '" & DateAdd(DateInterval.Day, 1, Timestamp).ToString("yyyy-MM-dd") & "' "))
                    Dim DBO As New DBObjects(SQL, frmMain.Connection, DBHelper.TableNames._AnyTable)
                    Dim TradesTa As New TradesTableAdapter
                    Dim TradesTb As New TradesDataTable
                    TradesTa.FillBySQL(TradesTb, "where 0=1")
                    For Each Row As DataRow In DBO.DataTable.Rows
                        If CDec(Row("Bestand")) > 0 Then
                            ' Nur positive Bestände als Verlust eintragen
                            Dim TradesRow As TradesRow = TradesTb.NewTradesRow
                            With TradesRow
                                .SourceID = MD5FromString(Timestamp.ToString("yyyy-MM-dd") & CInt(Platform) & Row("KontoID"))
                                .Zeitpunkt = Timestamp.Date
                                .ZeitpunktZiel = .Zeitpunkt
                                .TradeTypID = DBHelper.TradeTypen.Verlust
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ImportPlattformID = .QuellPlattformID
                                .QuellBetrag = Convert.ToDecimal(Row("Bestand"))
                                .QuellKontoID = Row("KontoID")
                                .ZielBetrag = 0D
                                .ZielKontoID = .QuellKontoID
                                .WertEUR = IIf(.QuellKontoID = DBHelper.Konten.EUR, .QuellBetrag, 0)
                                .Info = String.Format("Verlust aufgrund Betriebseinstellung der Plattform {0}", Row("PlattformBezeichnung"))
                                .BetragNachGebuehr = 0
                                .ImportID = 0
                                .Entwertet = False
                                .Steuerirrelevant = 0
                                .QuellBetragNachGebuehr = .QuellBetrag
                            End With
                            TradesTb.AddTradesRow(TradesRow)
                        End If
                    Next
                    TradesTa.Update(TradesTb)
                    DBO.Dispose()
            End Select
        Catch ex As Exception
            Throw New Exception("Fehler beim Speichern der Verlust-Einträge in die Trades-Tabelle: " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Berechnet alle bestehenden Verlust-Trades neu. Notwendig, wenn Trade-Einträge manuell verändert oder Importe gelöscht wurden.
    ''' </summary>
    Public Sub ResetAllLossTrades()
        Dim SQL As String = "select ID, DownSeit from Plattformen where IstDown = 1"
        Dim DBO As New DBObjects(SQL, _CtDB.Connection, DBHelper.TableNames._AnyTable)
        For Each Row As DataRow In DBO.DataTable.Rows
            SetLossTrades(LossTradeActionTypes.Delete, Row("ID"))
            SetLossTrades(LossTradeActionTypes.Add, Row("ID"), Row("DownSeit"))
        Next
    End Sub

    ''' <summary>
    ''' Berechnet Verlust-Trades einer bestimmten Plattform neu. Notwendig, wenn Trade-Einträge manuell verändert, Importe gelöscht oder durchgeführt wurden.
    ''' </summary>
    ''' <param name="PlatformID">ID der neu zu berechnenden Plattform</param>
    Public Sub ResetPlatformLossTrades(PlatformID As PlatformManager.Platforms)
        Dim SQL As String = "select ID, DownSeit from Plattformen where IstDown = 1 and ID = " & CInt(PlatformID)
        Dim DBO As New DBObjects(SQL, _CtDB.Connection, DBHelper.TableNames._AnyTable)
        For Each Row As DataRow In DBO.DataTable.Rows
            SetLossTrades(LossTradeActionTypes.Delete, Row("ID"))
            SetLossTrades(LossTradeActionTypes.Add, Row("ID"), Row("DownSeit"))
        Next
    End Sub

End Class

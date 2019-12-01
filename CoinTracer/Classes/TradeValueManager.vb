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

Imports CoinTracer.CoinValueStrategy
Imports CoinTracer.CoinTracerDataSetTableAdapters
Imports CoinTracer.CoinTracerDataSet

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
    Private _CVSDictionary As Dictionary(Of Integer, CoinValueStrategy)
    Private _LongTermInterval As LongTermTaxPeriod

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

    Private _CVS() As CoinValueStrategy
    Public ReadOnly Property CoinValueStrategy(CoinBusinessCase As CoinBusinessCases) As CoinValueStrategy
        Get
            If _CVS(CoinBusinessCase) Is Nothing OrElse _CVS(CoinBusinessCase).IsEmpty Then
                Return _CVS(CoinBusinessCases._Default)
            Else
                Return _CVS(CoinBusinessCase)
            End If
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
            _DS.ExecuteSQL("delete from Out2In where KalkulationID=" & KalkulationID)
            _DS.ExecuteSQL("delete from Kalkulationen where ID=" & KalkulationID)
        Catch ex As Exception
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
    ''' Calculates the source and tax value of all outgoing coins to the given timestamp by filling up the Out2In table
    ''' </summary>
    ''' <param name="UntilTime">Timestamp for the calculation. All outgoing coins up to this date will be calculated.</param>
    ''' <returns>Number of clarified outcoin entries</returns>
    Public Function CalculateOutCoinsToInCoins(Optional UntilTime As Date = DATENULLVALUE) As Integer
        Const CALCULATIONCHUNKSECONDS = 20          ' Seconds until the data is written to the database and the calculation starts over again
        Dim OutCoinsTA As VW_OutCoinsTableAdapter
        Dim InCoinsTA As VW_InCoinsTableAdapter
        Dim OutCoinsTb As New VW_OutCoinsDataTable
        Dim InCoinsTb As New VW_InCoinsDataTable
        Dim CalculationID As Long = 0
        Dim RowCount As Long = 0

        Cursor.Current = Cursors.WaitCursor
        InitProgressForm(My.Resources.MyStrings.calcGainingsInitProgressMessage)

        Try
            ' Write new calculation entry, rollback other calculations if needed
            If UntilTime = DATENULLVALUE Then
                UntilTime = Now
            End If
            CalculationID = WriteCalculationEntry(_TCS.ToString, UntilTime)
            If CalculationID > 0 Then      ' Nothing to do if CalculationID = 0!

                Dim OutCoinsSql As String
                Dim InCoinsSql As String
                Dim FromTime As Date = DATENULLVALUE
                Dim OutTradesInCalculation As String = "0,"

                _LastUnclearSpendings = 0
                ' Set Sql Template for OutCoins
                OutCoinsTA = New VW_OutCoinsTableAdapter
                If Not _TCS.WalletAware Then
                    OutCoinsSql = String.Format("and not OutTypID in ({0}, {1}, {2}) ",
                                                CInt(DBHelper.TradeTypen.TransferBörseBörse),
                                                CInt(DBHelper.TradeTypen.TransferBörseWallet),
                                                CInt(DBHelper.TradeTypen.TransferWalletBörse))
                Else
                    OutCoinsSql = ""
                End If
                ' Select all OutCoins that are not completely clear
                OutCoinsSql = "select o.* from VW_OutCoins as o " &
                    "left join Out2In as o2i on (o2i.OutTradeID = o.TradeID and not o2i.IstTransfer and o2i.SzenarioID = " & _SzenarioID & ") " &
                    "where o.Zeitpunkt >= '{0}' and o.Zeitpunkt < '{1}' and o.Betrag > 0 and not o.TradeID in ({2}) " &
                    OutCoinsSql &
                    " group by o.TradeID " &
                    "having sum(ifnull(o2i.MainBetrag, 0)) < o.Betrag " &
                    "order by o.Zeitpunkt, o.TradeID"
                ' Set Sql Template for InCoins
                InCoinsTA = New VW_InCoinsTableAdapter
                If Not _TCS.WalletAware Then
                    InCoinsSql = String.Format("and not InTypID in ({0}, {1}, {2}) ",
                                                CInt(DBHelper.TradeTypen.TransferBörseBörse),
                                                CInt(DBHelper.TradeTypen.TransferBörseWallet),
                                                CInt(DBHelper.TradeTypen.TransferWalletBörse))
                Else
                    InCoinsSql = ""
                End If
                ' Also select all InCoins that are not completely assigned
                InCoinsSql = "select i.* from VW_InCoins As i " &
                    "left join Out2In As o2i on (o2i.InTradeID = i.TradeID and not o2i.IstTransfer and o2i.SzenarioID = " & _SzenarioID & ") " &
                    "where i.Zeitpunkt < '{0}' " & InCoinsSql &
                    "group by i.TradeID " &
                    "having sum(ifnull(o2i.Betrag, 0)) < i.Betrag " &
                    "order by i.Zeitpunkt desc, i.TradeID desc"

                If OutCoinsTA.FillBySQL(OutCoinsTb, String.Format(OutCoinsSql,
                                                                  ConvertToSqlDate(FromTime),
                                                                  ConvertToSqlDate(UntilTime, False),
                                                                  "0")) = 0 Then
                    ' No OutCoins - nothing to do
                    CalculationID = 0
                Else

                    ' Loop over every OutCoin entry (in chunks for the sake of performance)
                    Dim AllWritten As Boolean = False
                    Dim OutToInTA As New Out2InTableAdapter
                    Dim OutToInTb As New Out2InDataTable
                    Dim TotalOutRows As Long
                    Dim ChunkRowCount As Long
                    Dim OutTradeMinOutToInIDs As New Dictionary(Of Long, Long)
                    Dim CVS As CoinValueStrategy
                    _CVSDictionary = New Dictionary(Of Integer, CoinValueStrategy) From {
                        {DBHelper.TradeTypen.Verkauf, _TCS.CoinValueStrategy(CoinBusinessCases.SellForFiat)},
                        {DBHelper.TradeTypen.Verlust, _TCS.CoinValueStrategy(CoinBusinessCases.SellForFiat)},
                        {DBHelper.TradeTypen.KaufCoin4Coin, _TCS.CoinValueStrategy(CoinBusinessCases.BuyForCoins)},
                        {DBHelper.TradeTypen.Transfer, _TCS.CoinValueStrategy(CoinBusinessCases.Withdraw)},
                        {DBHelper.TradeTypen.TransferBörseBörse, _TCS.CoinValueStrategy(CoinBusinessCases.TransferExchangeToExchange)},
                        {DBHelper.TradeTypen.TransferBörseWallet, _TCS.CoinValueStrategy(CoinBusinessCases.TransferExchangeToWallet)},
                        {DBHelper.TradeTypen.TransferWalletBörse, _TCS.CoinValueStrategy(CoinBusinessCases.TransferWalletToExchange)}}
                    _LongTermInterval = New LongTermTaxPeriod(_TCS.LongTermPeriodSQL)

                    TotalOutRows = OutCoinsTb.Rows.Count

                    ' Get all possible InCoins for this set of OutCoins
                    InCoinsTA.FillBySQL(InCoinsTb, String.Format(InCoinsSql,
                                                                 ConvertToSqlDate(UntilTime.AddMinutes(_TCS.ToleranceMinutes), False)))
                    OutToInTA.Fill(OutToInTb)

                    ProgressWaitManager.WithCancelOption = True

                    Do Until AllWritten

                        FromTime = Now
                        ChunkRowCount = 0


                        ' Loop over each OutCoin entry
                        AllWritten = True
                        For Each OutCoinRow As VW_OutCoinsRow In OutCoinsTb
                            RowCount += 1
                            ChunkRowCount += 1
                            ProgressWaitManager.UpdateProgress(RowCount / TotalOutRows * 97,
                                                           String.Format(My.Resources.MyStrings.calcGainingsProgressMessage,
                                                                         RowCount.ToString(Import.MESSAGENUMBERFORMAT), TotalOutRows.ToString(Import.MESSAGENUMBERFORMAT)))
                            If Not OutTradesInCalculation.Contains("," & OutCoinRow.TradeID & ",") Then
                                ' The OutTradeID may have already been cleared (by recursive call), so check this here

                                CVS = _CVSDictionary(OutCoinRow.OutTypID)
                                OutTradesInCalculation &= OutCoinRow.TradeID & ","
                                Try
                                    AssignOutCoinToInCoins(OutCoinRow.Betrag,
                                                           OutCoinRow.TradeID,
                                                           OutCoinRow.KontoID,
                                                           OutCoinRow.PlattformID,
                                                           OutCoinRow.Zeitpunkt,
                                                           OutCoinRow.Betrag,
                                                           InCoinsTb,
                                                           OutToInTb,
                                                           CalculationID,
                                                           CVS,
                                                           New List(Of Long) From {OutCoinRow.TradeID},
                                                           OutTradesInCalculation,
                                                           OutTradeMinOutToInIDs)
                                Catch ex As Exception
                                    Debug.Print("OutTrade ID " & OutCoinRow.TradeID)
                                End Try
                                ' Check if user has canceled
                                If ProgressWaitManager.Canceled Then
                                    ' get out of here!
                                    Dim DBO As New DBObjects(String.Format("update Kalkulationen set Zeitpunkt = '{0}' where ID = {1}",
                                                                       ConvertToSqlDate(OutCoinRow.Zeitpunkt), CalculationID),
                                                         Me.Connection)
                                    AllWritten = True
                                    Exit For
                                End If
                                ' check if we need a break
                                If (Now - FromTime).TotalSeconds >= CALCULATIONCHUNKSECONDS Then
                                    ProgressWaitManager.UpdateProgress(RowCount / TotalOutRows * 97, My.Resources.MyStrings.calcGainingsIntermediateWriteToDbMessage)
                                    OutToInTA.Update(OutToInTb)
                                    ' Load next chunk of OutCoins
                                    FromTime = OutCoinRow.Zeitpunkt
                                    OutCoinsTA.FillBySQL(OutCoinsTb, String.Format(OutCoinsSql,
                                                                               ConvertToSqlDate(FromTime),
                                                                               ConvertToSqlDate(UntilTime, False),
                                                                               OutTradesInCalculation.Substring(0, OutTradesInCalculation.Length - 1)))
                                    AllWritten = False
                                    Exit For
                                End If
                            End If
                        Next
                    Loop

                    ' Write to database
                    ProgressWaitManager.UpdateProgress(RowCount / TotalOutRows * 99, String.Format(My.Resources.MyStrings.calcGainingsFinalWriteToDbMessage,
                                                                                                   ChunkRowCount.ToString(Import.MESSAGENUMBERFORMAT)))
                    OutToInTA.Update(OutToInTb)
                End If
            End If
        Catch ex As Exception
            If CalculationID > 0 Then
                ' Rollback entries of current calculation
                RollbackCalculation(CalculationID)
            End If
            Cursor.Current = Cursors.Default
            DestroyProgressForm()
            Throw
            Exit Function
        End Try

        Cursor.Current = Cursors.Default
        DestroyProgressForm()

        Return RowCount

    End Function


    ''' <summary>
    ''' Assigns the given OutCoin entry to corresponding InCoin entries (according to the given CoinValueStrategies)
    ''' </summary>
    ''' <param name="InCoinsTable">InCoins data table</param>
    ''' <param name="OutTradeIDs">List of OutTradeIDs whose values are currently under assignment, used as stack for recursive calls</param>
    ''' <returns>Amount that still needs to be assigned (return value needed for recursion)</returns>
    Private Function AssignOutCoinToInCoins(ByVal MainAmountToAssign As Decimal,
                                            ByVal OutTradeID As Long,
                                            ByVal OutKontoID As Long,
                                            ByVal OutPlattform As Long,
                                            ByVal OutZeitpunkt As Date,
                                            ByVal AmountToAssign As Decimal,
                                            ByRef InCoinsTable As VW_InCoinsDataTable,
                                            ByRef OutToInTable As Out2InDataTable,
                                            ByVal CalculationID As Long,
                                            ByRef CVS As CoinValueStrategy,
                                            ByRef OutTradeIDs As List(Of Long),
                                            ByRef OutTradesInCalculation As String,
                                            ByRef OutTradeMinOutToInIDs As Dictionary(Of Long, Long)) As Decimal

        Dim MatchTimeMinutesTolerance As Long = 0
        Dim InCoinRows() As VW_InCoinsRow
        Dim Sql As String
        Dim SortSql As String
        Dim OutToInRowCount As Long = OutToInTable.Rows.Count
        Dim OutToInRows() As Out2InRow
        Dim OutToInRow As Out2InRow
        Dim InCoinAssignedAmount As Decimal
        Dim InitialAmountToAssign As Decimal = AmountToAssign
        Dim WertEUR As Decimal
        Dim FactorRow As Decimal

        ' Take care of the various CoinValuePreferences
        Select Case CVS.CoinValueStrategy
            Case CoinValueStrategies.OldestFirst
                SortSql = "Zeitpunkt ASC"
            Case CoinValueStrategies.YoungestFirst
                SortSql = "Zeitpunkt DESC"
            Case CoinValueStrategies.CheapestFirst
                SortSql = "KursEUR ASC"                    ' This is not accurate, because there are no values in transfers or (probably) in sells coins for coins - but anyway...
            Case CoinValueStrategies.MostExpensiveFirst
                SortSql = "KursEUR DESC"                   ' Same applies here...
            Case Else
                ' Standard: Fifo
                SortSql = "Zeitpunkt ASC"
        End Select

        ' Keep track of the Out2In rowcount for later recursive calls
        If Not OutTradeMinOutToInIDs.ContainsKey(OutTradeID) Then
            OutTradeMinOutToInIDs.Add(OutTradeID, OutToInRowCount)
        End If

        Do Until MatchTimeMinutesTolerance < 0                      ' ...just a signal to leave this loop
            ' Loop over corresponding InCoin entries
            If _TCS.WalletAware Then
                Sql = "and PlattformID = " & OutPlattform
            Else
                Sql = ""
            End If
            Sql = String.Format("KontoID = {0} and Zeitpunkt <= '{1}' {2}",
                                OutKontoID,
                                ConvertToSqlDate(OutZeitpunkt.AddMinutes(MatchTimeMinutesTolerance)),
                                Sql)
            InCoinRows = InCoinsTable.Select(Sql, SortSql)
            For Each InCoinRow As VW_InCoinsRow In InCoinRows

                ' Make sure we are not looping here
                If OutTradeIDs.IndexOf(InCoinRow.TradeID) = -1 Then
                    ' Now check if this InCoin entry has some unassigned value
                    InCoinAssignedAmount = 0
                    OutToInRows = OutToInTable.Select(String.Format("InTradeID = {0} and SzenarioID = {1} and not IstTransfer",
                                                                    InCoinRow.TradeID,
                                                                    _SzenarioID))
                    For i As Long = 0 To Math.Min(OutTradeMinOutToInIDs(OutTradeID), OutToInRows.LongLength) - 1
                        If OutToInRows(i).RowState <> DataRowState.Deleted AndAlso (OutToInTable.Rows.IndexOf(OutToInRows(i)) < OutTradeMinOutToInIDs(OutTradeID)) Then
                            InCoinAssignedAmount += OutToInRows(i).Betrag
                        End If
                    Next

                    If InCoinAssignedAmount < InCoinRow.Betrag Then
                        ' There is some amount not assigned yet - so go ahead
                        If InCoinRow.InTypID = DBHelper.TradeTypen.Kauf _
                            Or (InCoinRow.InTypID = DBHelper.TradeTypen.KaufCoin4Coin And _TCS.Coins4CoinsAccounting) _
                            Or InCoinRow.InTypID = DBHelper.TradeTypen.Transfer Then
                            ' InCoin has a taxable value - so assign it
                            InCoinAssignedAmount = Math.Min(InCoinRow.Betrag - InCoinAssignedAmount, AmountToAssign)
                            AddOutToInRow(OutToInTable,
                                          _SzenarioID,
                                          CalculationID,
                                          OutTradeID,
                                          OutTradeID,
                                          InCoinRow.TradeID,
                                          InCoinRow.InZeitpunkt,
                                          MainAmountToAssign * (InCoinAssignedAmount / InitialAmountToAssign),
                                          InCoinAssignedAmount,
                                          InCoinRow.WertEUR * (InCoinAssignedAmount / InCoinRow.Betrag),
                                          1,
                                          True,
                                          False,
                                          IsNotTaxable(InCoinRow.InZeitpunkt, OutZeitpunkt))
                            AmountToAssign -= InCoinAssignedAmount
                            If AmountToAssign <= AMOUNT_TOLERANCE Then
                                Exit For
                            End If
                        ElseIf InCoinRow.InTypID = DBHelper.TradeTypen.KaufCoin4Coin Or
                            (InCoinRow.InTypID And 15) = DBHelper.TradeTypen.Transfer Then
                            ' Buy/sell coin4coin or internal transfer: Determine tax value and write Out2In entries
                            OutToInRows = OutToInTable.Select(String.Format("MainOutTradeID = {0} and SzenarioID = {1}",
                                                                            InCoinRow.TradeID,
                                                                            _SzenarioID), "ID")
                            If OutToInRows.LongLength = 0 Then
                                ' This InTrade/OutTrade has not been assigned yet - so do it now
                                Try
                                    AssignOutCoinToInCoins(InCoinRow.OutBetrag,
                                                           InCoinRow.TradeID,
                                                           InCoinRow.OutKontoID,
                                                           InCoinRow.OutPlattformID,
                                                           InCoinRow.OutZeitpunkt,
                                                           InCoinRow.OutBetrag,
                                                           InCoinsTable,
                                                           OutToInTable,
                                                           CalculationID,
                                                           _CVSDictionary(InCoinRow.InTypID),
                                                           OutTradeIDs,
                                                           OutTradesInCalculation,
                                                           OutTradeMinOutToInIDs)
                                    OutTradesInCalculation &= InCoinRow.TradeID & ","
                                    OutTradeIDs.Add(InCoinRow.TradeID)
                                Catch ex As StackOverflowException
                                    ' We had a problem here!
                                    Debug.Print("Problem in finding amounts! (Recursion)")
                                End Try
                            End If
                            ' Determine tax value ('WertEUR')
                            WertEUR = 0
                            For Each OutToInRow In OutToInRows
                                If Not OutToInRow.IstTransfer Then
                                    WertEUR += OutToInRow.WertEUR
                                End If
                            Next
                            ' Calculate the partial values for the assignment
                            InCoinAssignedAmount = Math.Min(InCoinRow.Betrag - InCoinAssignedAmount, AmountToAssign)
                            If InCoinAssignedAmount = InCoinRow.Betrag Then
                                FactorRow = 1
                            Else
                                FactorRow = InCoinAssignedAmount / InCoinRow.Betrag
                            End If
                            AddOutToInRow(OutToInTable,
                                          _SzenarioID,
                                          CalculationID,
                                          OutTradeID,
                                          OutTradeID,
                                          InCoinRow.TradeID,
                                          InCoinRow.InZeitpunkt,
                                          MainAmountToAssign * (InCoinAssignedAmount / InitialAmountToAssign),
                                          InCoinAssignedAmount,
                                          WertEUR * FactorRow,
                                          1,
                                          True,
                                          (InCoinRow.InTypID And 15) = DBHelper.TradeTypen.Transfer,
                                          IsNotTaxable(InCoinRow.InZeitpunkt, OutZeitpunkt))
                            If (InCoinRow.InTypID And 15) = DBHelper.TradeTypen.Transfer Then
                                ' This InTrade was a transfer, so dig for the corresponding Out2In entries (= the entries, that are no transfers)
                                AssignFromTransfer(OutTradeID,
                                                   OutZeitpunkt,
                                                   MainAmountToAssign * (InCoinAssignedAmount / InitialAmountToAssign),
                                                   InCoinAssignedAmount,
                                                   InCoinRow.TradeID,
                                                   OutToInTable,
                                                   CalculationID)

                            End If


                            AmountToAssign -= InCoinAssignedAmount
                            If AmountToAssign <= AMOUNT_TOLERANCE Then
                                Exit For
                            End If
                        Else
                            ' Some unexpected type of InCoin occured...
                            Debug.Print("Warning: InCoinType " & InCoinRow.InTypID & " encountered at OutTradeID " & OutTradeID)
                        End If
                    End If
                End If
            Next
            If AmountToAssign > AMOUNT_TOLERANCE Then
                ' OutCoin value still not completely assigned
                If MatchTimeMinutesTolerance = 0 AndAlso _TCS.ToleranceMinutes > 0 Then
                    ' Try again, but this time add some tolerance to the cut off time
                    MatchTimeMinutesTolerance = _TCS.ToleranceMinutes
                    ' Erase Out2In entries from first trial
                    For i As Long = OutToInTable.Rows.Count - 1 To OutToInRowCount Step -1
                        If OutToInTable.Rows(i).RowState <> DataRowState.Deleted Then
                            OutToInTable.Rows(i).Delete()
                        End If
                    Next
                Else
                    ' 2nd trial was not successful either, so create "fake" OutToInCoin entry
                    AddOutToInRow(OutToInTable,
                                  _SzenarioID,
                                  CalculationID,
                                  OutTradeID,
                                  OutTradeID,
                                  -1,
                                  OutZeitpunkt.Date, 'Date.FromOADate(Math.Floor(OutZeitpunkt.ToOADate)),
                                  MainAmountToAssign * (AmountToAssign / InitialAmountToAssign),
                                  AmountToAssign,
                                  0,
                                  1,
                                  True,
                                  False,
                                  False)
                    AmountToAssign = 0
                    _LastUnclearSpendings += 1
                    ' leave the loop
                    MatchTimeMinutesTolerance = -1
                End If
            Else
                ' leave the loop
                AmountToAssign = 0
                MatchTimeMinutesTolerance = -1
            End If
        Loop
        ' Remove last OutTradeID from stack
        OutTradeIDs.RemoveAt(OutTradeIDs.Count - 1)
        Return AmountToAssign
    End Function

    ''' <summary>
    ''' Copies (and transforms) the Out2In entries of a certain OutTradeID to the MainOutTradeID. Used as replacement for recursive calls and also because there is (usually)
    ''' no need to dig into the origin of an OutTrade over and over again.
    ''' </summary>
    ''' <param name="MainOutTradeID">ID of the OutTrade whose entries will be written</param>
    ''' <param name="MainAmountToAssign">Amount that needs to be assigned to this OutTrade</param>
    ''' <param name="AmountToAssign">Amount converted into the MainAmount of the OutTrade to copy</param>
    ''' <param name="OutTradeID">ID of the OutTrade whose entries will be copied</param>
    ''' <returns>Tax value ('WertEUR') of OutTradeID</returns>
    Private Function AssignFromTransfer(ByVal MainOutTradeID As Long,
                                        ByVal OutTime As Date,
                                        ByVal MainAmountToAssign As Decimal,
                                        ByVal AmountToAssign As Decimal,
                                        ByVal OutTradeID As Long,
                                        ByRef OutToInTable As Out2InDataTable,
                                        ByVal CalculationID As Long) As Decimal
        Dim OutToInRows() As Out2InRow
        Dim RowAmountToAssign As Decimal
        Dim RowAmount As Decimal
        Dim RowWertEur As Decimal
        Dim SumWertEur As Decimal = 0
        Dim FactorRow As Decimal
        Dim FactorNewToOld As Decimal
        Dim OutToInRow As Out2InRow
        Dim NewOutToInRow As Out2InRow

        ' Factor for 'translation' of former MainBetrag to new MainBetrag amounts (prevent rounding issues)
        If MainAmountToAssign = AmountToAssign Then
            FactorNewToOld = 1
        Else
            FactorNewToOld = MainAmountToAssign / AmountToAssign
        End If

        OutToInRows = OutToInTable.Select(String.Format("MainOutTradeID = {0} and SzenarioID = {1}",
                                                        OutTradeID,
                                                        _SzenarioID), "ID")
        For Each OutToInRow In OutToInRows
            RowAmountToAssign = Math.Min(OutToInRow.MainBetrag, AmountToAssign) ' Currency: 'MainBetrag' of each row read (this will NOT be copied to the new rows)
            ' This is to prevent rounding issues
            If RowAmountToAssign = OutToInRow.MainBetrag Then
                FactorRow = 1
            Else
                FactorRow = (RowAmountToAssign / OutToInRow.MainBetrag)
            End If
            RowAmount = OutToInRow.Betrag * FactorRow
            RowWertEur = OutToInRow.WertEUR * FactorRow
            ' Now copy the entry
            NewOutToInRow = OutToInTable.AddOut2InRow(_SzenarioID,
                                      CalculationID,
                                      MainOutTradeID,
                                      OutToInRow.OutTradeID,
                                      OutToInRow.InTradeID,
                                      OutToInRow.InZeitpunkt,
                                      RowAmountToAssign * FactorNewToOld,
                                      RowAmount,
                                      RowWertEur,
                                      OutToInRow.Level + 1,
                                      OutToInRow.IstFiat,
                                      OutToInRow.IstTransfer,
                                      IsNotTaxable(OutToInRow.InZeitpunkt, OutTime))
            If Not OutToInRow.IstTransfer Then
                MainAmountToAssign -= RowAmountToAssign * FactorNewToOld
                AmountToAssign -= RowAmountToAssign
                SumWertEur += RowWertEur
                If MainAmountToAssign <= AMOUNT_TOLERANCE Then
                    Exit For
                End If
            Else
                ' This is a transfer, so keep on digging
                SumWertEur += AssignFromTransfer(MainOutTradeID,
                                                 OutTime,
                                                 RowAmount,
                                                 RowAmount,
                                                 OutToInRow.InTradeID,
                                                 OutToInTable,
                                                 CalculationID)
            End If
        Next
        Return SumWertEur
    End Function

    ''' <summary>
    ''' Adds a new entry to the OutToIn DataTable for each OutTradeID given
    ''' </summary>
    Private Sub AddOutToInRow(ByRef OutToInTable As Out2InDataTable,
                              ByVal SzenarioID As Long,
                              ByVal KalkulationID As Long,
                              ByVal MainOutTradeID As Long,
                              ByVal OutTradeID As Long,
                              ByVal InTradeID As Long,
                              ByVal InZeitpunkt As Date,
                              ByVal MainBetrag As Decimal,
                              ByVal Betrag As Decimal,
                              ByVal WertEUR As Decimal,
                              ByVal Level As Integer,
                              ByVal IsFiat As Boolean,
                              ByVal IsTransfer As Boolean,
                              Optional ByVal IsNotTaxable As Boolean = False)
        OutToInTable.AddOut2InRow(SzenarioID,
                                  KalkulationID,
                                  MainOutTradeID,
                                  OutTradeID,
                                  InTradeID,
                                  InZeitpunkt,
                                  MainBetrag,
                                  Betrag,
                                  WertEUR,
                                  Level,
                                  IsFiat,
                                  IsTransfer,
                                  IsNotTaxable)
    End Sub

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

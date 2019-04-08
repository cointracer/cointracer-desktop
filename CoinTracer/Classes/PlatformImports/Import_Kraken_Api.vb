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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or within the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports Newtonsoft.Json.Linq
Imports CoinTracer.CoinTracerDataSet

Friend Class Import_Kraken_Api
    Inherits ApiImportBase
    Implements IApiImport

    Private Enum KrakenApiMissingLederErrors
        NoError = 0
        EndOfHistory
        WrongRefId
    End Enum

    Public Sub New(ByRef Import As Import)
        MainImportObject = Import
        Platform = PlatformManager.Platforms.Kraken
    End Sub

    ''' <summary>
    ''' Perform an API import for platform kraken.com
    ''' </summary>
    ''' <returns>Unix timestamp for the latest imported trade</returns>
    Friend Overrides Function PerformImport() As Long

        Dim ClientKraken As New KrakenClient.KrakenClient(ApiKey, ApiSecret, CallDelay)
        Dim KrakenLedger As KrakenClient.KrakenApiLedger
        Dim CurrentYoungestTimestamp As Double = LastImportTimestamp
        Dim EndTimestamp As Long = DateToUnixTimestamp(DateTimeEnd)
        Dim TimeDelta As Single = 0.0001
        Dim KrakenChunkLimit As Integer = 49
        Dim Page As Integer = 1
        Dim ErrCounter As Long = MaxErrors

        Cursor.Current = Cursors.WaitCursor

        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))

        Try
            Dim Record As dtoTradesRecord
            Dim RecordFee As dtoTradesRecord
            Dim ImportRecords As New List(Of dtoTradesRecord)
            Dim AnalyseCounter As Long = 0
            Dim LedgerEntries As Long = 0
            Dim LedgerItem As JProperty
            Dim LedgerItem1st As JProperty
            Dim LedgerItem2 As JProperty
            Dim LedgerEntry As JObject
            Dim LedgerEntry2 As JObject
            Dim KontoRow As KontenRow
            Dim LedgerError As KrakenApiMissingLederErrors
            Dim SkipGetNextLedgerItem As Boolean = False

            ' Kraken-API-Ledgerabruf initialisieren
            EndTimestamp = Math.Max(EndTimestamp, -1)
            KrakenLedger = New KrakenClient.KrakenApiLedger(ClientKraken, LastImportTimestamp, ApiConfigName, EndTimestamp, KrakenChunkLimit)

            LedgerItem = KrakenLedger.GetNextLedgerItem
            AnalyseCounter = 0

            Dim TLO As Import_Kraken.KrakenLineObject
            Dim NextTLO As Import_Kraken.KrakenLineObject
            Dim SourceTLO As Import_Kraken.KrakenLineObject
            Dim TargetTLO As Import_Kraken.KrakenLineObject

            ' Schleife über alle Ledger-Items
            Do Until LedgerItem Is Nothing

                AnalyseCounter += 1
                ProgressWaitManager.UpdateProgress(Math.Min(AnalyseCounter / KrakenChunkLimit, 1) * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgKrakenApiProgess, AnalyseCounter.ToString(Import.MESSAGENUMBERFORMAT)))

                LedgerEntry = JObject.Parse(LedgerItem.Value.ToString)
                WriteLogEntry(String.Format(My.Resources.MyStrings.importLogKrakenLedgerItem,
                                        LedgerItem.ToString,
                                        Environment.NewLine), TraceEventType.Verbose)
                ' Den jeweils jüngsten Timestamp als Aufsetzpunkt für die Rückgabe merken
                If CDbl(LedgerEntry("time").ToString) > CurrentYoungestTimestamp Then
                    CurrentYoungestTimestamp = CDbl(LedgerEntry("time").ToString)
                End If

                Record = New dtoTradesRecord
                RecordFee = Nothing

                With Record
                    Try
                        TLO = New Import_Kraken.KrakenLineObject(MainImportObject, DateFromUnixTimestamp(LedgerEntry("time").ToString),
                                                   LedgerItem.Name,
                                                   LedgerEntry("refid").ToString,
                                                   LedgerEntry("type").ToString,
                                                   LedgerEntry("asset").ToString,
                                                   LedgerEntry("amount").ToString,
                                                   LedgerEntry("fee").ToString)
                        .SourceID = TLO.TxId
                        .Zeitpunkt = TLO.DateTime
                        .ZeitpunktZiel = .Zeitpunkt
                        .ImportPlattformID = Platform
                        .Info = FirstCharToUppercase(TLO.Type) & " " & TLO.Asset
                        KontoRow = MainImportObject.RetrieveAccount(TLO.Asset)
                        Select Case TLO.Type.Substring(0, Math.Min(8, TLO.Type.Length)).ToLower
                            Case "deposit", "transfer"
                                .TradetypID = DBHelper.TradeTypen.Einzahlung
                                If TLO.Asset = "KFEE" Then
                                    .QuellPlattformID = PlatformManager.Platforms.Extern
                                Else
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                End If
                                .ZielPlattformID = Platform
                                .ZielBetrag = TLO.Amount
                                .QuellBetrag = .ZielBetrag
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .BetragNachGebuehr = .ZielBetrag
                                If TLO.Fee > 0 Then
                                    ' Es gibt eine Gebühr, also BetragNachGebühr verringern
                                    .BetragNachGebuehr -= TLO.Fee
                                End If
                                .ZielKontoID = KontoRow.ID
                                .QuellKontoID = .ZielKontoID
                            Case "withdraw"
                                .TradetypID = DBHelper.TradeTypen.Auszahlung
                                .QuellPlattformID = Platform
                                .ZielPlattformID = PlatformManager.Platforms.Unknown
                                .QuellBetrag = Math.Abs(TLO.Amount)
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .ZielBetrag = .QuellBetrag
                                .BetragNachGebuehr = .ZielBetrag
                                .QuellKontoID = KontoRow.ID
                                .ZielKontoID = .QuellKontoID
                                If TLO.Fee > 0 Then
                                    ' Es gibt eine Gebühr, also Zielbetrag erhöhen
                                    .QuellBetrag += TLO.Fee
                                End If
                            Case "trade"
                                ' Nächsten Eintrag dazuholen
                                LedgerError = KrakenApiMissingLederErrors.NoError
                                LedgerItem2 = KrakenLedger.GetNextLedgerItem
                                If LedgerItem2 Is Nothing Then
                                    LedgerError = KrakenApiMissingLederErrors.EndOfHistory
                                End If
                                If LedgerError = KrakenApiMissingLederErrors.NoError Then
                                    LedgerEntry2 = JObject.Parse(LedgerItem2.Value.ToString)
                                    If TLO.RefId <> LedgerEntry2("refid").ToString Then
                                        LedgerError = KrakenApiMissingLederErrors.WrongRefId
                                    End If
                                Else
                                    LedgerEntry2 = Nothing
                                End If
                                ' Prüfen, ob es sich um einen Kraken Fee Eintrag handelt
                                If TLO.Asset = "FEE" AndAlso LedgerError = KrakenApiMissingLederErrors.NoError Then
                                    ' Gebühren-Objekt schon einmal erstellen (wird unten gebraucht)
                                    RecordFee = .Clone()
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.QuellPlattformID = Platform
                                    RecordFee.ZielPlattformID = .QuellPlattformID
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellKontoID = KontoRow.ID
                                    RecordFee.QuellBetrag = Math.Abs(TLO.Fee)
                                    RecordFee.ZielBetrag = RecordFee.QuellBetrag
                                    RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                    RecordFee.ZielPlattformID = Platform
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = String.Format(My.Resources.MyStrings.ImportInfoKrakenTradeFeeCredits, .SourceID)
                                    ' Loop-Variable auf nächste Instanz stellen
                                    LedgerItem1st = LedgerItem      ' nur sicherheitshalber für Log-Einträge merken...
                                    LedgerItem = LedgerItem2
                                    LedgerEntry = LedgerEntry2
                                    TLO = New Import_Kraken.KrakenLineObject(MainImportObject, DateFromUnixTimestamp(LedgerEntry("time").ToString),
                                                   LedgerItem.Name,
                                                   LedgerEntry("refid").ToString,
                                                   LedgerEntry("type").ToString,
                                                   LedgerEntry("asset").ToString,
                                                   LedgerEntry("amount").ToString,
                                                   LedgerEntry("fee").ToString)
                                    LedgerItem2 = KrakenLedger.GetNextLedgerItem
                                    If LedgerItem2 Is Nothing Then
                                        LedgerError = KrakenApiMissingLederErrors.EndOfHistory
                                    End If
                                    If LedgerError = KrakenApiMissingLederErrors.NoError Then
                                        LedgerEntry2 = JObject.Parse(LedgerItem2.Value.ToString)
                                        If TLO.RefId <> LedgerEntry2("refid").ToString Then
                                            LedgerError = KrakenApiMissingLederErrors.WrongRefId
                                        End If
                                    End If
                                End If
                                If LedgerError > KrakenApiMissingLederErrors.NoError Then
                                    If TLO.Amount <= Import_Kraken.KRAKEN_ZEROVALUETRADELIMIT Then
                                        ' The value of this trade is very low: assume the corresponding second entry would be zero
                                        NextTLO = New Import_Kraken.KrakenLineObject(MainImportObject, DateFromUnixTimestamp(LedgerEntry("time").ToString),
                                                                       LedgerItem.Name,
                                                                       LedgerEntry("refid").ToString,
                                                                       LedgerEntry("type").ToString,
                                                                       LedgerEntry("asset").ToString,
                                                                       "0.0",
                                                                       "0.0")
                                        ' set loop variable to the item that has already been fetched
                                        LedgerItem = LedgerItem2
                                        SkipGetNextLedgerItem = True
                                        LedgerError = KrakenApiMissingLederErrors.NoError
                                    Else
                                        Dim ErrorMessage As String = String.Format(My.Resources.MyStrings.importMsgKrakenErrorNoSecondEntry, .SourceID)
                                        ErrorMessage = String.Format(My.Resources.MyStrings.importMsgKrakenApiErrorPrefix, AnalyseCounter.ToString(Import.MESSAGENUMBERFORMAT)) & " " & ErrorMessage
                                        WriteLogEntry(ErrorMessage & " " & My.Resources.MyStrings.importMsgKrakenApiErrorSuffix & LedgerItem.ToString & Environment.NewLine,
                                            TraceEventType.Information)
                                        Throw New Exception(String.Format(My.Resources.MyStrings.importMsgKrakenErrorNoSecondEntry, .SourceID))
                                    End If
                                End If
                                NextTLO = New Import_Kraken.KrakenLineObject(MainImportObject, DateFromUnixTimestamp(LedgerEntry2("time").ToString),
                                                               LedgerItem2.Name,
                                                               LedgerEntry2("refid").ToString,
                                                               LedgerEntry2("type").ToString,
                                                               LedgerEntry2("asset").ToString,
                                                               LedgerEntry2("amount").ToString,
                                                               LedgerEntry2("fee").ToString)

                                Dim QuellKontoRow As KontenRow
                                If TLO.Amount < 0 Then
                                    SourceTLO = TLO
                                    TargetTLO = NextTLO
                                ElseIf NextTLO.Amount <= 0 Then
                                    SourceTLO = NextTLO
                                    TargetTLO = TLO
                                Else
                                    Throw New Exception(String.Format(My.Resources.MyStrings.importMsgKrakenErrorNoNegativeValue, .SourceID))
                                End If
                                QuellKontoRow = MainImportObject.RetrieveAccount(SourceTLO.Asset)
                                KontoRow = MainImportObject.RetrieveAccount(TargetTLO.Asset)
                                .SourceID = SourceTLO.TxId & "-" & TargetTLO.TxId
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = KontoRow.ID
                                .ZielBetrag = TargetTLO.Amount
                                .BetragNachGebuehr = .ZielBetrag
                                .QuellBetrag = Math.Abs(SourceTLO.Amount)
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .QuellKontoID = QuellKontoRow.ID
                                .Info = String.Format("Trade {0} ({1})", KontoRow.Bezeichnung, KontoRow.Code)
                                If KontoRow.IstFiat = False Then
                                    ' Kauf von Coins (gegen Fiat oder andere Coins)
                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                Else
                                    ' Verkauf von - wahrscheinlich - Coins
                                    .TradetypID = DBHelper.TradeTypen.Verkauf
                                End If
                                If TargetTLO.Fee <> 0 Then
                                    ' Es gibt eine Gebühr, also BetragNachGebühr verringern
                                    .BetragNachGebuehr -= Math.Abs(TargetTLO.Fee)
                                End If
                                ' watch out: Kraken can have 2 fees! One on the source-side and the other on the target-side.
                                If SourceTLO.Fee <> 0 Then
                                    ' Gebühr beim QuellBetrag!
                                    .QuellBetrag += Math.Abs(SourceTLO.Fee)
                                End If
                                If .QuellKontoID = DBHelper.Konten.EUR Then
                                    .WertEUR = .QuellBetrag
                                ElseIf .ZielKontoID = DBHelper.Konten.EUR Then
                                    .WertEUR = .BetragNachGebuehr
                                End If
                                ' Record-Fee wenn vorhanden anpassen
                                If RecordFee IsNot Nothing Then
                                    RecordFee.Info = String.Format(RecordFee.Info, .SourceID)
                                    RecordFee.SourceID = .SourceID & "/fee"
                                End If
                            Case Else
                                .DoNotImport = True
                        End Select
                        ' check for fees on target side
                        If RecordFee Is Nothing AndAlso .BetragNachGebuehr < .ZielBetrag Then
                            RecordFee = .Clone()
                            RecordFee.SourceID = .SourceID & "/fee"
                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                            RecordFee.BetragNachGebuehr = 0
                            RecordFee.QuellKontoID = .ZielKontoID
                            RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                            RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                            RecordFee.ZielPlattformID = Platform
                            RecordFee.WertEUR = 0
                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                            RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                            ImportRecords.Add(RecordFee)
                            RecordFee = Nothing
                        End If
                        ' check for fees on source side
                        If RecordFee Is Nothing AndAlso .QuellBetragNachGebuehr < .QuellBetrag Then
                            RecordFee = .Clone()
                            RecordFee.SourceID = .SourceID & "/fee" & IIf(.BetragNachGebuehr < .ZielBetrag, "2", "")
                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                            RecordFee.BetragNachGebuehr = 0
                            RecordFee.QuellKontoID = .QuellKontoID
                            RecordFee.ZielBetrag = .QuellBetrag - .QuellBetragNachGebuehr
                            RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                            RecordFee.ZielPlattformID = Platform
                            RecordFee.WertEUR = 0
                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                            RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                        End If
                    Catch ex As Exception
                        If ApiImportError(ErrCounter, AnalyseCounter, ex) = 0 Then
                            Return 0
                            Exit Function
                        End If
                    End Try

                    If Not .DoNotImport Then
                        ' Record der Liste hinzufügen
                        ImportRecords.Add(Record)
                        If Not RecordFee Is Nothing Then
                            ImportRecords.Add(RecordFee)
                        End If
                    End If

                End With

                If Not SkipGetNextLedgerItem Then
                    LedgerItem = KrakenLedger.GetNextLedgerItem
                    SkipGetNextLedgerItem = False
                End If

            Loop


            ' Import durchführen
            If ImportRecords.Count > 0 Then
                MainImportObject.Import_Records(ImportRecords,
                               String.Format(My.Resources.MyStrings.importMsgApiImportLabel, ApiConfigName),
                               ReadImportdataPercentage,
                               False,
                               True,
                               ApiDatenID,
                               CurrentYoungestTimestamp)
            Else
                DestroyProgressForm()
            End If
            Cursor.Current = Cursors.Default

        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DestroyProgressForm()
            Throw
            Exit Function
        End Try

        If ErrCounter = MaxErrors Then
            Return CurrentYoungestTimestamp
        Else
            Return 0
        End If

    End Function

End Class

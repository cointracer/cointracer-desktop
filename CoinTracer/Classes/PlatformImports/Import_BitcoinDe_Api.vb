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

Imports CoinTracer.CoinTracerDataSet
Imports Newtonsoft.Json.Linq

Friend Class Import_BitcoinDe_Api
    Inherits ApiImportBase
    Implements IApiImport

    Public Sub New(ByRef Import As Import)
        MainImportObject = Import
        Platform = PlatformManager.Platforms.BitcoinDe
    End Sub

    ''' <summary>
    ''' Perform an API import for platform bitcoin.de
    ''' </summary>
    ''' <returns>Unix timestamp for the latest imported trade</returns>
    Friend Overrides Function PerformImport() As Long
        If Not MainImportObject.SilentMode Then
            MsgBoxEx.ShowWithNotAgainOption("ImportBitcoinDeApi", DialogResult.OK, My.Resources.MyStrings.importMsgBitcoinDeApi,
                                My.Resources.MyStrings.importMsgBitcoinDeApiTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Dim PlatformName As String = PlatformManager.PlatformDetailsByID(Platform).Name
        Dim ClientBitcoinDe As New BitcoinDeClient.BitcoinDeClient(ApiKey, ApiSecret)
        Dim BitcoinDeAccInfo As New BitcoinDeClient.BitcoinDeAccountInfo(ExtendedInfo)
        Dim QryLedger As JObject
        Dim QryPage As JObject
        Dim LedgerText As String = ""
        Dim LastImport As Date = DateFromUnixTimestamp(LastImportTimestamp)
        Dim CurrentImport As Date = Now
        Dim NoErrors As Boolean = True

        Cursor.Current = Cursors.WaitCursor

        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))

        Try
            If LastImport.Date < CurrentImport.Date Then

                ' only make request when last request has been at least one day in the past
                Dim AnalyseCounter As Long = 1
                Dim ImportRecords As New List(Of dtoTradesRecord)
                ' Loop each active curreny
                For Each Currency As AccountInfo.CryptoCurrency In BitcoinDeAccInfo.Currencies
                    If Currency.Active Then
                        Dim Page As Integer = 1
                        Dim Response As String
                        Dim MaxPages As Integer
                        Dim MaxTrades As Long = 0
                        Dim FeeAmount As Decimal = 0

                        Dim Record As dtoTradesRecord
                        Dim RecordFee As dtoTradesRecord
                        Dim ErrCounter As Long = MaxErrors
                        Dim PrimaryAccount As KontenRow
                        Dim SecondaryAccount As KontenRow

                        Do
                            ProgressWaitManager.UpdateProgress(String.Format(My.Resources.MyStrings.importMsgBitcoinDeApiProgess, Page, ApiConfigName, Currency.Longname, Environment.NewLine))
                            Response = ClientBitcoinDe.ShowAccountLedger(Currency.Shortname, , LastImport, DateTimeEnd, Page)
                            If Not Response.StartsWith("{""account_ledger"":") Then
                                ' API meldet einen Fehler
                                Throw New Exception(String.Format(My.Resources.MyStrings.importMsgApiServerError, PlatformName, Response))
                                Exit Function
                            End If

                            QryLedger = JObject.Parse(Response)

                            ' Seitenzähler auswerten
                            If Page = 1 Then
                                QryPage = JObject.Parse(QryLedger("page").ToString)
                                MaxPages = QryPage("last").ToString
                                MaxTrades = MaxPages * DirectCast(QryLedger("account_ledger"), ICollection).Count
                            End If

                            If DirectCast(QryLedger("account_ledger"), ICollection).Count > 0 Then

                                ' Alle Ledger-Einträge in String konvertieren
                                Dim NextLedgerItem As JObject
                                For Each LedgerItem As Newtonsoft.Json.Linq.JObject In QryLedger("account_ledger")

                                    ProgressWaitManager.UpdateProgress(Math.Min(AnalyseCounter / MaxTrades, 1) * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgApiAnalyseTrade, AnalyseCounter))

                                    Record = New dtoTradesRecord
                                    RecordFee = Nothing
                                    With Record
                                        Try
                                            ' Debug.Print(LedgerItem.ToString)
                                            .Zeitpunkt = CDate(LedgerItem("date")).ToString("dd-MM-yy HH:mm:ss")
                                            .ZeitpunktZiel = .Zeitpunkt
                                            .ImportPlattformID = Platform
                                            .SourceID = LedgerItem("reference").ToString
                                            Select Case LedgerItem("type").ToString
                                                Case "buy"
                                                    ' Kauf
                                                    Dim TradeEntry As JObject = LedgerItem("trade")
                                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                                    .QuellPlattformID = Platform
                                                    .ZielPlattformID = .QuellPlattformID
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(TradeEntry("primary_currency")("currency").ToString)
                                                    .ZielKontoID = PrimaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(TradeEntry("primary_currency")("before_fee").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = Convert.ToDecimal(TradeEntry("primary_currency")("after_fee").ToString, CultureInfo.InvariantCulture)
                                                    ' adjust real target amount, because buyer only pays half of the fee
                                                    .ZielBetrag = (.ZielBetrag + .BetragNachGebuehr) / 2
                                                    .QuellBetrag = Convert.ToDecimal(TradeEntry("secondary_currency")("after_fee").ToString, CultureInfo.InvariantCulture)
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                    SecondaryAccount = MainImportObject.RetrieveAccount(TradeEntry("secondary_currency")("currency").ToString)
                                                    .QuellKontoID = SecondaryAccount.ID
                                                    If .QuellKontoID = DBHelper.Konten.EUR Then
                                                        .WertEUR = .QuellBetrag
                                                    End If
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeBuy,
                                                                          PrimaryAccount.Code, .ZielBetrag, .QuellBetrag, SecondaryAccount.Code)
                                                    ' Gebühren-Transaktion
                                                    RecordFee = .Clone()
                                                    RecordFee.SourceID = .SourceID & "/fee"
                                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                    RecordFee.ZielKontoID = PrimaryAccount.GebuehrKontoID
                                                    RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                                    RecordFee.WertEUR = 0
                                                    RecordFee.BetragNachGebuehr = 0
                                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                    RecordFee.QuellKontoID = PrimaryAccount.ID
                                                    RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeBuyFee, PrimaryAccount.Code, .SourceID)
                                                    FeeAmount = 0
                                                Case "sell"
                                                    ' Verkauf
                                                    Dim TradeEntry As JObject = LedgerItem("trade")
                                                    .TradetypID = DBHelper.TradeTypen.Verkauf
                                                    .QuellPlattformID = Platform
                                                    .ZielPlattformID = .QuellPlattformID
                                                    SecondaryAccount = MainImportObject.RetrieveAccount(TradeEntry("secondary_currency")("currency").ToString)
                                                    .ZielKontoID = SecondaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(TradeEntry("secondary_currency")("before_fee").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = Convert.ToDecimal(TradeEntry("secondary_currency")("after_fee").ToString, CultureInfo.InvariantCulture)
                                                    .QuellBetrag = Convert.ToDecimal(TradeEntry("primary_currency")("before_fee").ToString, CultureInfo.InvariantCulture)
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                    If .ZielKontoID = DBHelper.Konten.EUR Then
                                                        .WertEUR = .BetragNachGebuehr
                                                    End If
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(TradeEntry("primary_currency")("currency").ToString)
                                                    .QuellKontoID = PrimaryAccount.ID
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeSell,
                                                                          PrimaryAccount.Code, .QuellBetrag, .ZielBetrag, SecondaryAccount.Code)
                                                    ' Gebühren-Transaktion
                                                    RecordFee = .Clone()
                                                    RecordFee.SourceID = .SourceID & "/fee"
                                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                    RecordFee.ZielKontoID = SecondaryAccount.GebuehrKontoID
                                                    RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                                    RecordFee.WertEUR = 0
                                                    RecordFee.BetragNachGebuehr = 0
                                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                    RecordFee.QuellKontoID = SecondaryAccount.ID
                                                    RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeBuyFee, PrimaryAccount.Code, .SourceID)
                                                    FeeAmount = 0
                                                Case "payout"
                                                    ' Auszahlung
                                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                                    .QuellPlattformID = Platform
                                                    .QuellBetragNachGebuehr = -Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .QuellBetrag = .QuellBetragNachGebuehr + FeeAmount
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .QuellKontoID = PrimaryAccount.ID
                                                    .BetragNachGebuehr = .QuellBetragNachGebuehr       ' Bei Auszahlungen steht der Betrag, der am Ziel ankommt, in BetragNachGebuehr!
                                                    .WertEUR = 0
                                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                                    .ZielKontoID = .QuellKontoID
                                                    .ZielBetrag = .QuellBetragNachGebuehr
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDePayout,
                                                                          Currency.Shortname, LedgerItem("reference").ToString)
                                                    ' check if next line has network fee
                                                    NextLedgerItem = LedgerItem.Next
                                                    If NextLedgerItem IsNot Nothing Then
                                                        If NextLedgerItem("type").ToString = "outgoing_fee_voluntary" _
                                                AndAlso LedgerItem("date").ToString = NextLedgerItem("date").ToString Then
                                                            .QuellBetrag = .QuellBetrag - Convert.ToDecimal(NextLedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                        End If
                                                    End If
                                                    FeeAmount = 0
                                                Case "outgoing_fee_voluntary"
                                                    ' network fee for payout
                                                    .SourceID = .SourceID & "/fee"
                                                    .TradetypID = DBHelper.TradeTypen.Gebühr
                                                    .QuellPlattformID = Platform
                                                    .QuellBetrag = -Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .QuellKontoID = PrimaryAccount.ID
                                                    .BetragNachGebuehr = 0
                                                    .WertEUR = 0
                                                    .ZielPlattformID = Platform
                                                    .ZielKontoID = PrimaryAccount.GebuehrKontoID
                                                    .ZielBetrag = .QuellBetrag
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDePayoutNetworkFee,
                                                                          Currency.Shortname, LedgerItem("reference").ToString)
                                                    FeeAmount = .QuellBetrag
                                                Case "inpayment"
                                                    ' Einzahlung
                                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                                    .ZielPlattformID = Platform
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .ZielKontoID = PrimaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = .ZielBetrag
                                                    .WertEUR = 0
                                                    .QuellBetrag = .ZielBetrag
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                    .QuellKontoID = .ZielKontoID
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeInpayment,
                                                                          Currency.Shortname,
                                                                          LedgerItem("reference").ToString)
                                                    FeeAmount = 0
                                                Case "kickback"
                                                    ' Kickback for using the API
                                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                                    .QuellPlattformID = Platform
                                                    .ZielPlattformID = .QuellPlattformID
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .ZielKontoID = PrimaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = .ZielBetrag
                                                    .WertEUR = 0
                                                    .QuellBetrag = 0
                                                    .QuellBetragNachGebuehr = 0
                                                    .QuellKontoID = DBHelper.Konten.EUR
                                                    .Info = LedgerItem("reference").ToString
                                                    .SourceID = .Info
                                                    FeeAmount = 0
                                                Case "initial", "initial_fork", "account_opening"
                                                    ' Account has been initialised
                                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                                    .QuellPlattformID = Platform
                                                    .ZielPlattformID = .QuellPlattformID
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .ZielKontoID = PrimaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = .ZielBetrag
                                                    .WertEUR = 0
                                                    .QuellBetrag = 0
                                                    .QuellBetragNachGebuehr = 0
                                                    .QuellKontoID = DBHelper.Konten.EUR
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeInitialization, Currency.Shortname)
                                                    .SourceID = MD5FromString(.Info & .Zeitpunkt.ToString("dd-MM-yy HH:mm:ss"))
                                                    FeeAmount = 0
                                                    If .ZielBetrag = 0 AndAlso .BetragNachGebuehr = 0 Then
                                                        .DoNotImport = True
                                                    End If
                                                Case "welcome_btc"
                                                    ' welcome BTC for registration
                                                    If Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture) > 0 Then
                                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                                        .QuellPlattformID = Platform
                                                        .ZielPlattformID = .QuellPlattformID
                                                        PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                        .ZielKontoID = PrimaryAccount.ID
                                                        .ZielBetrag = Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                        .BetragNachGebuehr = .ZielBetrag
                                                        .WertEUR = 0
                                                        .QuellBetrag = 0
                                                        .QuellBetragNachGebuehr = 0
                                                        .QuellKontoID = DBHelper.Konten.EUR
                                                        .Info = My.Resources.MyStrings.importInfoBitcoinDeRegistration
                                                        .SourceID = MD5FromString(.Info & .Zeitpunkt.ToString("dd-MM-yy HH:mm:ss"))
                                                    Else
                                                        .DoNotImport = True
                                                    End If
                                                    FeeAmount = 0
                                                Case "affiliate"
                                                    ' affiliate earnings
                                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                                    .QuellPlattformID = Platform
                                                    .ZielPlattformID = .QuellPlattformID
                                                    PrimaryAccount = MainImportObject.RetrieveAccount(Currency.Shortname)
                                                    .ZielKontoID = PrimaryAccount.ID
                                                    .ZielBetrag = Convert.ToDecimal(LedgerItem("cashflow").ToString, CultureInfo.InvariantCulture)
                                                    .BetragNachGebuehr = .ZielBetrag
                                                    .WertEUR = 0
                                                    .QuellBetrag = 0
                                                    .QuellBetragNachGebuehr = 0
                                                    .QuellKontoID = DBHelper.Konten.EUR
                                                    .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeAffiliateEarnings, LedgerItem("reference").ToString)
                                                    .SourceID = MD5FromString(.Info & .Zeitpunkt.ToString("dd-MM-yy HH:mm:ss"))
                                                    FeeAmount = 0
                                                Case Else
                                                    ' Ansonsten nicht importieren
                                                    .DoNotImport = True
                                                    FeeAmount = 0
                                            End Select

                                            If Not .DoNotImport Then
                                                ' Record der Liste hinzufügen
                                                ImportRecords.Add(Record)
                                                If Not RecordFee Is Nothing Then
                                                    ImportRecords.Add(RecordFee)
                                                End If
                                            End If

                                            AnalyseCounter += 1

                                        Catch ex As Exception
                                            If ApiImportError(ErrCounter, AnalyseCounter, ex) = 0 Then
                                                Return 0
                                                Exit Function
                                            End If
                                        End Try
                                    End With

                                Next

                                ' Log this response
                                WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: {0} Import-Records...", ImportRecords.Count), TraceEventType.Information)

                            End If
                            Page += 1
                        Loop Until Page > MaxPages

                    End If
                Next

                ' Import durchführen
                If ImportRecords.Count > 0 Then
                    MainImportObject.Import_Records(ImportRecords,
                               String.Format(My.Resources.MyStrings.importMsgApiImportLabel, ApiConfigName),
                               ReadImportdataPercentage,
                               False,
                               True,
                               ApiDatenID,
                               DateToUnixTimestamp(CurrentImport))

                End If
            End If

            Cursor.Current = Cursors.Default
            DestroyProgressForm()

        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DestroyProgressForm()
            Throw
            Exit Function
        End Try

        If NoErrors Then
            Return DateToUnixTimestamp(CurrentImport)
        Else
            Return 0
        End If

    End Function

End Class

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

Imports System.Linq
Imports CoinTracer.CoinTracerDataSet

Public Class Import_Bitfinex
    Inherits FileImportBase
    Implements IFileImport

    Private Const PLATFORMID = PlatformManager.Platforms.Bitfinex
    Private Const PLATFORMFULLNAME As String = "Bitfinex.com"


    Public Sub New()
        MyBase.New()
    End Sub


    ''' <summary>
    ''' Returns all matching data file headers for this import
    ''' </summary>
    Public Overrides ReadOnly Property PlatformHeaders As ImportFileHelper.MatchingPlatform() Implements IFileImport.PlatformHeaders
        Get
            Dim Result As ImportFileHelper.MatchingPlatform() = {
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitfinex.com (pre 2019)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Currency,Description,Amount,Balance,Date",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitfinex.com (since 2019-01)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "DESCRIPTION,CURRENCY,AMOUNT,BALANCE,DATE,WALLET",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitfinex.com (since 2021-01)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "#,DESCRIPTION,CURRENCY,AMOUNT,BALANCE,DATE,WALLET",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 2}
                }
            Return Result
        End Get
    End Property


    ''' <summary>
    ''' Initializes this import
    ''' </summary>
    ''' <param name="MainImportObject">Reference to the calling import object</param>
    Public Sub New(MainImportObject As Import)
        MyBase.New(MainImportObject)

        Platform = PLATFORMID
        PlatformName = PLATFORMFULLNAME
        CSVAutoDetectEncoding = True
        CSVDecimalPoint = "."c
        CSVDecimalSeparator = ""
        CSVSeparator = ","
        MultiSelectFiles = True
        FileDialogTitle = My.Resources.MyStrings.importOpenFileFilterTitleBitfinex
        ConvertFromUTC = False
    End Sub

    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportBitfinexCom", DialogResult.OK,
                                        My.Resources.MyStrings.importMsgBitfinexCSV,
                                        My.Resources.MyStrings.importMsgBitfinexCSVTitle,
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Perform the actual import for Bitfinex.com files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean
        Dim l As Long
        Dim AllLines As Long
        Dim ErrCounter As Long = MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim KontoRow As KontenRow
        Dim LedgerItems As New List(Of BitfinexClient.BitfinexLedgerItem)
        Dim LedgerItem As BitfinexClient.BitfinexLedgerItem
        Dim LedgerItem2 As BitfinexClient.BitfinexLedgerItem

        AllLines = AllRows.Count
        If AllLines > 0 Then

            Cursor.Current = Cursors.WaitCursor

            ImportRecords = New List(Of dtoTradesRecord)

            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "Bitfinex.com"))

            ' Convert all lines into ledger items
            Dim FeeTx As String
            Dim WithdrawalFeeDescription As String = ""
            Dim Currency2 As String
            Dim DescToLower As String
            Dim SubItems() As String
            Dim Items() As String
            Dim ColCurrency As Byte
            Dim ColDescription As Byte
            Dim ColAmount As Byte
            Dim ColDate As Byte
            Dim DateFormat As String
            Dim RowDate As Date
            Dim RowAmount As Decimal
            Select Case SubType
                Case 2
                    ' column mapping since 2021-01
                    ColCurrency = 2
                    ColDescription = 1
                    ColAmount = 3
                    ColDate = 5
                    DateFormat = "dd-MM-yy HH:mm:ss"
                Case 1
                    ' column mapping since 2019-01
                    ColCurrency = 1
                    ColDescription = 0
                    ColAmount = 2
                    ColDate = 4
                    DateFormat = "dd-MM-yy HH:mm:ss"
                Case Else
                    ' old column mapping before 2019
                    ColCurrency = 0
                    ColDescription = 1
                    ColAmount = 2
                    ColDate = 4
                    DateFormat = "yyyy-MM-dd HH:mm:ss"
            End Select
            For l = 0 To AllLines - 1
                Items = AllRows(l)
                If Items.Length >= 5 Then

                    Items(ColCurrency) = Normalize_Entry(Items(ColCurrency))
                    DescToLower = Items(ColDescription).ToLower
                    RowDate = Date.ParseExact(Items(ColDate), DateFormat, CultureInfo.InvariantCulture)
                    If _ConvertFromUTC Then
                        RowDate = RowDate.ToLocalTime()
                    End If
                    RowAmount = StrToDec(Items(ColAmount))
                    If (DescToLower.StartsWith("deposit") AndAlso Not DescToLower.StartsWith("deposit fee")) Then
                        ' deposit entry
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems.Length >= 3 Then
                            FeeTx = String.Format("Deposit {0} {1}", Items(ColCurrency), SubItems(2))
                        Else
                            FeeTx = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Deposit,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           FeeTx,
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.Contains("canceled withdrawal ") Then
                        ' canceled withdrawal
                        SubItems = Items(ColDescription).Split(" ")
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.CanceledWithdrawal,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           SubItems(3),
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.Contains(" distribution ") Then
                        ' airdrop of a coin (will be treated as a buy for 0 EUR)
                        SubItems = Items(ColDescription).Split(" ")
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Distribution,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           "",
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.Contains(" withdrawal ") AndAlso Not DescToLower.Contains("withdrawal fee") Then
                        ' withdrawal entry
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems.Length >= 3 Then
                            FeeTx = String.Format("Withdrawal {0} {1}", Items(ColCurrency), SubItems(2))
                        Else
                            FeeTx = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Withdrawal,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           FeeTx,
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.StartsWith("exchange") Then
                        ' exchange entry
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems.Length >= 7 Then
                            SubItems(2) = Normalize_Entry(SubItems(2)) ' ...to turn 'MegaIOTA' into 'IOTA' and other stuff
                            FeeTx = StrToDec(SubItems(1)).ToString("#########0.0###", CultureInfo.InvariantCulture) &
                                " " &
                                SubItems(2) & " " & StrToDec(SubItems(6)).ToString("#########0.0###", CultureInfo.InvariantCulture)
                            If SubItems(2) = Items(ColCurrency) Then
                                Currency2 = SubItems(4)
                            Else
                                Currency2 = SubItems(2)
                            End If
                        Else
                            FeeTx = ""
                            Currency2 = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Currency2,
                                                                           BitfinexClient.TransactionTypes.Exchange,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           FeeTx,
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.StartsWith("trading fee") Then
                        ' trading fee entry
                        SubItems = Items(ColDescription).Replace("fees for", "fee").Split(" ")
                        If SubItems.Length >= 6 Then
                            Currency2 = Normalize_Entry(SubItems(3)) ' ...to turn 'MegaIOTA' into 'IOTA'
                            ' there are (up to now) two different kinds of description around. Try to figure out, which one we're dealing with
                            If SubItems(5) = "@" Then
                                FeeTx = StrToDec(SubItems(2)).ToString("#########0.0###", CultureInfo.InvariantCulture) & " " & Currency2 & " " & StrToDec(SubItems(6)).ToString("#########0.0###", CultureInfo.InvariantCulture)
                            Else
                                FeeTx = StrToDec(SubItems(2)).ToString("#########0.0###", CultureInfo.InvariantCulture) & " " & Currency2 & " " & StrToDec(SubItems(5)).ToString("#########0.0###", CultureInfo.InvariantCulture)
                            End If
                        Else
                            FeeTx = ""
                            Currency2 = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Currency2,
                                                                           BitfinexClient.TransactionTypes.Fee,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           FeeTx,
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.StartsWith("deposit fee") Then
                        ' deposit fee entry
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems.Length >= 4 Then
                            FeeTx = String.Format("Deposit {0} #{1}", Items(ColCurrency), SubItems(3))
                        Else
                            FeeTx = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Fee,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           FeeTx,
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.Contains("withdrawal fee") Then
                        ' withdrawal fee entry
                        WithdrawalFeeDescription = Normalize_Entry(Items(ColDescription))     ' save the description here for later referencing
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Fee,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           "",
                                                                           WithdrawalFeeDescription,
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.StartsWith("extraordinary loss") Then
                        ' forced BFX exchange
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems.Length >= 9 Then
                            If SubItems(5) = Items(ColCurrency) Then
                                Currency2 = SubItems(8)
                            Else
                                Currency2 = SubItems(5)
                            End If
                        Else
                            Currency2 = ""
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Currency2,
                                                                           BitfinexClient.TransactionTypes.Exchange,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           "",
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.Contains(" token redemption of ") Then
                        ' crappy BFX token redemption
                        SubItems = Items(ColDescription).Split(" ")
                        If SubItems(0) = Items(ColCurrency) Then
                            Currency2 = ""
                        Else
                            Currency2 = SubItems(0)
                        End If
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Currency2,
                                                                           BitfinexClient.TransactionTypes.Exchange,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           "",
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    ElseIf DescToLower.StartsWith("settlement @") Then
                        ' one of these ugly 'settlements' (automatically performed trades when a currency has a negative balance)
                        LedgerItem = New BitfinexClient.BitfinexLedgerItem(Items(ColCurrency),
                                                                           Items(ColCurrency),
                                                                           BitfinexClient.TransactionTypes.Settlement,
                                                                           RowAmount,
                                                                           RowDate,
                                                                           Items(ColDescription),
                                                                           Items(ColDescription),
                                                                           l)
                        LedgerItems.Add(LedgerItem)
                    Else
                        Debug.Print("unknown line!")
                    End If
                End If
            Next

            Dim FromLedgerItem As BitfinexClient.BitfinexLedgerItem
            Dim ToLedgerItem As BitfinexClient.BitfinexLedgerItem

            ' check if there are cancelled withdrawals and blank out their counterparts
            For Each FromLedgerItem In LedgerItems.Where(Function(a) a.TransactionType = BitfinexClient.TransactionTypes.CanceledWithdrawal)
                For Each ToLedgerItem In LedgerItems.Where(Function(a) a.Description.Contains(FromLedgerItem.FeeTransactionID) And a.TransactionType = BitfinexClient.TransactionTypes.Withdrawal)
                    ToLedgerItem.Processed = True
                    FromLedgerItem.Processed = True
                Next
            Next

            ' now loop over all ledger items

            Dim i2nd As Integer

            AllLines = LedgerItems.Count
            For l = 0 To AllLines - 1
                UpdateProgress(AllLines, l + 1)
                ' ProgressWaitManager.UpdateProgress(l / AllLines * ReadImportdataPercentage, String.Format("Lese Daten ein... ({0}/{1})", l, AllLines))
                LedgerItem = LedgerItems(l)
                Try
                    If Not LedgerItem.Processed Then

                        Record = New dtoTradesRecord
                        RecordFee = Nothing
                        With Record
                            .SourceID = MD5FromString(LedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss") &
                                                      "/" & Normalize_Entry(LedgerItem.Description).ToLower &
                                                      "/" & LedgerItem.Currency1.ToLower &
                                                      "/" & LedgerItem.Amount.ToString("#########0.0###", CultureInfo.InvariantCulture))
                            .Zeitpunkt = LedgerItem.Time
                            .ZeitpunktZiel = .Zeitpunkt
                            .ImportPlattformID = PLATFORMID
                            .Info = LedgerItem.Description
                            KontoRow = MainImportObject.RetrieveAccount(LedgerItem.Currency1)

                            Select Case LedgerItem.TransactionType
                                Case BitfinexClient.TransactionTypes.Deposit
                                    ' deposit
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielPlattformID = PLATFORMID
                                    .ZielBetrag = LedgerItem.Amount
                                    .QuellBetrag = .ZielBetrag
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .BetragNachGebuehr = .ZielBetrag
                                    .ZielKontoID = KontoRow.ID
                                    .QuellKontoID = .ZielKontoID
                                    ' Check for fee entry
                                    i2nd = FindLegerItem(LedgerItems,
                                                                         LedgerItem.Currency1,
                                                                         BitfinexClient.TransactionTypes.Fee,
                                                                         LedgerItem.Time,
                                                                         "",
                                                                         LedgerItem.FeeTransactionID)
                                    If i2nd >= 0 Then
                                        ' create transaction fee record
                                        LedgerItem2 = LedgerItems(i2nd)
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = MainImportObject.RetrieveAccount(LedgerItem2.Currency1).ID
                                        RecordFee.ZielBetrag = -LedgerItem2.Amount
                                        RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                                        RecordFee.QuellPlattformID = PLATFORMID
                                        ' Adjust values of original deposit
                                        .BetragNachGebuehr -= RecordFee.QuellBetrag
                                        LedgerItem2.Processed = True
                                    End If
                                    ' set taxable value (not really needed, since this is a fiat deposit, but anyway...)
                                    If .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If
                                Case BitfinexClient.TransactionTypes.Distribution
                                    ' Coin distribution
                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                    .QuellPlattformID = PLATFORMID
                                    .ZielPlattformID = PLATFORMID
                                    .ZielBetrag = LedgerItem.Amount
                                    .QuellBetrag = 0
                                    .QuellBetragNachGebuehr = 0
                                    .BetragNachGebuehr = .ZielBetrag
                                    .ZielKontoID = KontoRow.ID
                                    .QuellKontoID = AccountManager.Accounts.EUR
                                Case BitfinexClient.TransactionTypes.Withdrawal
                                    ' withdrawal
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .QuellPlattformID = PLATFORMID
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .QuellBetrag = -LedgerItem.Amount
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .ZielBetrag = .QuellBetrag
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellKontoID = KontoRow.ID
                                    .ZielKontoID = .QuellKontoID
                                    ' Check for fee entry / don't use FeeTx index here - the only links between fee an withdrawal are time and currency! :-(
                                    i2nd = FindLegerItem(LedgerItems,
                                                         LedgerItem.Currency1,
                                                         BitfinexClient.TransactionTypes.Fee,
                                                         LedgerItem.Time,
                                                         WithdrawalFeeDescription,
                                                         "")
                                    If i2nd >= 0 Then
                                        ' create transaction fee record
                                        LedgerItem2 = LedgerItems(i2nd)
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = MainImportObject.RetrieveAccount(LedgerItem2.Currency1).ID
                                        RecordFee.ZielBetrag = -LedgerItem2.Amount
                                        RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                                        RecordFee.ZielPlattformID = PLATFORMID
                                        ' Adjust values of original withdrawal
                                        .QuellBetrag += RecordFee.QuellBetrag
                                        LedgerItem2.Processed = True
                                    End If
                                    ' set taxable value (not really needed, since this is a fiat withdrawal, but anyway...)
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    End If
                                Case BitfinexClient.TransactionTypes.Exchange
                                    ' some kind of exchange - find corresponding entry
                                    i2nd = FindLegerItem(LedgerItems,
                                                        IIf(LedgerItem.Currency2.Length > 0, LedgerItem.Currency2, "-" & LedgerItem.Currency1),
                                                        BitfinexClient.TransactionTypes.Exchange,
                                                        LedgerItem.Time,
                                                        LedgerItem.Description,
                                                        "")
                                    If i2nd < 0 Then
                                        Throw New Exception(String.Format("Konnte keine passende Gegenbuchung zu diesem Trade finden: {0}{1}/{2} {3}/{4}" &
                                                                          "{0}{0}(Bitte stellen Sie sicher, dass die Daten *aller* benötigten Währungen eingelesen werden!)",
                                                                          Environment.NewLine,
                                                                          LedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          LedgerItem.Currency1,
                                                                          LedgerItem.Amount,
                                                                          LedgerItem.Description))
                                    End If
                                    LedgerItem2 = LedgerItems(i2nd)
                                    If LedgerItem.Amount < 0 AndAlso LedgerItem2.Amount >= 0 Then
                                        FromLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem.Currency1,
                                                                                               LedgerItem.Currency2,
                                                                                               LedgerItem.TransactionType,
                                                                                               -LedgerItem.Amount,
                                                                                               LedgerItem.Time,
                                                                                               LedgerItem.FeeTransactionID,
                                                                                               LedgerItem.Description)
                                        ToLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem2.Currency1,
                                                                                             LedgerItem2.Currency2,
                                                                                             LedgerItem2.TransactionType,
                                                                                             LedgerItem2.Amount,
                                                                                             LedgerItem2.Time,
                                                                                             LedgerItem2.FeeTransactionID,
                                                                                             LedgerItem2.Description)
                                    ElseIf LedgerItem2.Amount < 0 AndAlso LedgerItem.Amount >= 0 Then
                                        FromLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem2.Currency1,
                                                                                               LedgerItem2.Currency2,
                                                                                               LedgerItem2.TransactionType,
                                                                                               -LedgerItem2.Amount,
                                                                                               LedgerItem2.Time,
                                                                                               LedgerItem2.FeeTransactionID,
                                                                                               LedgerItem2.Description)
                                        ToLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem.Currency1,
                                                                                             LedgerItem.Currency2,
                                                                                             LedgerItem.TransactionType,
                                                                                             LedgerItem.Amount,
                                                                                             LedgerItem.Time,
                                                                                             LedgerItem.FeeTransactionID,
                                                                                             LedgerItem.Description)
                                    Else
                                        Throw New Exception(String.Format("Die Beträge (Buchung und Gegenbuchung) passen bei folgendem Trade nicht zusammen: {0}{1}/{2} {3} bzw. {4}/{5}",
                                                                          Environment.NewLine,
                                                                          LedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          LedgerItem.Currency1,
                                                                          LedgerItem.Amount,
                                                                          LedgerItem2.Amount,
                                                                          LedgerItem.Description))
                                    End If

                                    ' create trade entry
                                    .QuellPlattformID = PLATFORMID
                                    .ZielPlattformID = .QuellPlattformID
                                    .ZielKontoID = MainImportObject.GetAccount(ToLedgerItem.Currency1).ID
                                    .ZielBetrag = ToLedgerItem.Amount
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellBetrag = FromLedgerItem.Amount
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .QuellKontoID = MainImportObject.GetAccount(FromLedgerItem.Currency1).ID
                                    If MainImportObject.GetAccount(.ZielKontoID).IstFiat Then
                                        ' fiat received - it was a sell
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                    Else
                                        ' no fiat received - it was a buy
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                    End If
                                    .SourceID = MD5FromString(FromLedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss") &
                                                      "/" & FromLedgerItem.Currency1.ToLower &
                                                      "/" & FromLedgerItem.Amount.ToString("#########0.0###", CultureInfo.InvariantCulture) &
                                                      "/" & ToLedgerItem.Currency1.ToLower &
                                                      "/" & Normalize_Entry(FromLedgerItem.Description).ToLower)
                                    LedgerItem.Processed = True
                                    LedgerItem2.Processed = True
                                    ' check if there is a ledger entry for fees
                                    If LedgerItem.FeeTransactionID.Length > 0 OrElse LedgerItem2.FeeTransactionID.Length > 0 Then
                                        i2nd = FindLegerItem(LedgerItems,
                                                                             LedgerItem.Currency1,
                                                                             BitfinexClient.TransactionTypes.Fee,
                                                                             LedgerItem.Time,
                                                                             "",
                                                                             LedgerItem.FeeTransactionID)
                                        If i2nd < 0 Then
                                            ' nothing found for first ledger item - search again for second
                                            i2nd = FindLegerItem(LedgerItems,
                                                                             LedgerItem2.Currency1,
                                                                             BitfinexClient.TransactionTypes.Fee,
                                                                             LedgerItem2.Time,
                                                                             "",
                                                                             LedgerItem2.FeeTransactionID)
                                        End If
                                        If i2nd < 0 Then
                                            ' nothing found so far - try again with any currency
                                            i2nd = FindLegerItem(LedgerItems,
                                                                             "",
                                                                             BitfinexClient.TransactionTypes.Fee,
                                                                             LedgerItem.Time,
                                                                             "",
                                                                             LedgerItem.FeeTransactionID)
                                        End If
                                        If i2nd >= 0 Then
                                            ' create transaction fee record
                                            LedgerItem2 = LedgerItems(i2nd)
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellKontoID = MainImportObject.RetrieveAccount(LedgerItem2.Currency1).ID
                                            RecordFee.ZielBetrag = -LedgerItem2.Amount
                                            RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                            RecordFee.WertEUR = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                                            ' Beträge in Trade-Eintrag anpassen
                                            If .QuellKontoID = RecordFee.QuellKontoID Then
                                                ' Gebühr beim Quellbetrag
                                                .QuellBetrag -= RecordFee.QuellBetrag
                                            ElseIf .ZielKontoID = RecordFee.QuellKontoID Then
                                                ' Gebühr beim Zielbetrag
                                                .BetragNachGebuehr -= RecordFee.QuellBetrag
                                            End If
                                            LedgerItem2.Processed = True
                                        End If
                                    End If
                                    ' set taxable value
                                    If .TradetypID = DBHelper.TradeTypen.Verkauf AndAlso .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    ElseIf .TradetypID = DBHelper.TradeTypen.Kauf AndAlso .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    End If
                                Case BitfinexClient.TransactionTypes.Settlement
                                    ' a settlement entry (can be treated like an exchange) - find corresponding entry
                                    i2nd = FindLegerItem(LedgerItems,
                                                         "",
                                                         BitfinexClient.TransactionTypes.Settlement,
                                                         LedgerItem.Time,
                                                         LedgerItem.Description,
                                                         "",
                                                         l)
                                    If i2nd < 0 Then
                                        Throw New Exception(String.Format("Konnte keine passende Gegenbuchung zu diesem Trade finden: {0}{1}/{2} {3}/{4}" &
                                                                          "{0}{0}(Bitte stellen Sie sicher, dass die Daten *aller* benötigten Währungen eingelesen werden!)",
                                                                          Environment.NewLine,
                                                                          LedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          LedgerItem.Currency1,
                                                                          LedgerItem.Amount,
                                                                          LedgerItem.Description))
                                    End If
                                    LedgerItem2 = LedgerItems(i2nd)
                                    If LedgerItem.Amount < 0 AndAlso LedgerItem2.Amount >= 0 Then
                                        FromLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem.Currency1,
                                                                                               LedgerItem.Currency2,
                                                                                               LedgerItem.TransactionType,
                                                                                               -LedgerItem.Amount,
                                                                                               LedgerItem.Time,
                                                                                               LedgerItem.FeeTransactionID,
                                                                                               LedgerItem.Description)
                                        ToLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem2.Currency1,
                                                                                             LedgerItem2.Currency2,
                                                                                             LedgerItem2.TransactionType,
                                                                                             LedgerItem2.Amount,
                                                                                             LedgerItem2.Time,
                                                                                             LedgerItem2.FeeTransactionID,
                                                                                             LedgerItem2.Description)
                                    ElseIf LedgerItem2.Amount < 0 AndAlso LedgerItem.Amount >= 0 Then
                                        FromLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem2.Currency1,
                                                                                               LedgerItem2.Currency2,
                                                                                               LedgerItem2.TransactionType,
                                                                                               -LedgerItem2.Amount,
                                                                                               LedgerItem2.Time,
                                                                                               LedgerItem2.FeeTransactionID,
                                                                                               LedgerItem2.Description)
                                        ToLedgerItem = New BitfinexClient.BitfinexLedgerItem(LedgerItem.Currency1,
                                                                                             LedgerItem.Currency2,
                                                                                             LedgerItem.TransactionType,
                                                                                             LedgerItem.Amount,
                                                                                             LedgerItem.Time,
                                                                                             LedgerItem.FeeTransactionID,
                                                                                             LedgerItem.Description)
                                    Else
                                        Throw New Exception(String.Format("Die Beträge (Buchung und Gegenbuchung) passen bei folgendem Trade nicht zusammen: {0}{1}/{2} {3} bzw. {4}/{5}",
                                                                          Environment.NewLine,
                                                                          LedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          LedgerItem.Currency1,
                                                                          LedgerItem.Amount,
                                                                          LedgerItem2.Amount,
                                                                          LedgerItem.Description))
                                    End If

                                    ' create trade entry
                                    .QuellPlattformID = PLATFORMID
                                    .ZielPlattformID = .QuellPlattformID
                                    .ZielKontoID = MainImportObject.GetAccount(ToLedgerItem.Currency1).ID
                                    .ZielBetrag = ToLedgerItem.Amount
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellBetrag = FromLedgerItem.Amount
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .QuellKontoID = MainImportObject.GetAccount(FromLedgerItem.Currency1).ID
                                    If MainImportObject.GetAccount(.ZielKontoID).IstFiat Then
                                        ' fiat received - it was a sell
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                    Else
                                        ' no fiat received - it was a buy
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                    End If
                                    .SourceID = MD5FromString(FromLedgerItem.Time.ToString("yyyy-MM-dd HH:mm:ss") &
                                                      "/" & FromLedgerItem.Currency1.ToLower &
                                                      "/" & FromLedgerItem.Amount.ToString("#########0.0###", CultureInfo.InvariantCulture) &
                                                      "/" & ToLedgerItem.Currency1.ToLower &
                                                      "/" & Normalize_Entry(FromLedgerItem.Description).ToLower)
                                    LedgerItem.Processed = True
                                    LedgerItem2.Processed = True
                                    ' set taxable value
                                    If .TradetypID = DBHelper.TradeTypen.Verkauf AndAlso .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    ElseIf .TradetypID = DBHelper.TradeTypen.Kauf AndAlso .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    End If

                                Case Else
                                    ' undefined or fee ledger type: do not import
                                    .DoNotImport = True

                            End Select

                            ' add record to import list
                            If Not .DoNotImport Then
                                ImportRecords.Add(Record)
                                If Not RecordFee Is Nothing Then
                                    ImportRecords.Add(RecordFee)
                                End If
                            End If

                        End With
                    End If

                Catch ex As Exception
                    If FileImportError(ErrCounter, l + 1, ex) = 0 Then
                        Return False
                        Exit Function
                    End If
                End Try

            Next l

            MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage, , True)
            Cursor.Current = Cursors.Default

        End If

        Return ErrCounter = MaxErrors

    End Function

    ''' <summary>
    ''' Finds a bitfinex ledger item by the given criteria. Items that are already processed are never found.
    ''' </summary>
    ''' <param name="Currency">Currency to search for. If empty any currency is a vaild match. If this starts with a '-' any currency except the given is found.</param>
    ''' <param name="Description">Description to search for. If empty any descrition is a vaild match.</param>
    ''' <param name="FeeTransactionID">FeeTransactionID to search for. If empty any FeeTransactionID is a vaild match.</param>
    ''' <param name="LedgerItems">List of ledger items that is searched.</param>
    ''' <param name="Time">Time of searched transaction. Must be given!</param>
    ''' <param name="Type">Type of seached transation. Must be given!</param>
    ''' <returns>Index in LedgerItems list of found entry. -1 in any other cases (even errors)</returns>
    Private Function FindLegerItem(ByRef LedgerItems As List(Of BitfinexClient.BitfinexLedgerItem),
                                   ByVal Currency As String,
                                   ByVal Type As BitfinexClient.TransactionTypes,
                                   ByVal Time As DateTime,
                                   ByVal Description As String,
                                   ByVal FeeTransactionID As String,
                                   Optional ByVal IndexToSkip As Long = -1) As Integer
        Try
            Dim NotCurrency As String
            If Currency.StartsWith("-") Then
                NotCurrency = Currency.Substring(1)
                Currency = ""
            Else
                NotCurrency = ""
            End If
            Dim i As Integer = -1
            Dim DeltaSecs As Integer
            For s As Integer = 0 To 30  ' not nice, but seems to be necessary: search within a timeframe of +/- 30 seconds!
                ' set search delta
                DeltaSecs = s
                ' find 
                i = LedgerItems.FindIndex(Function(x) x.TransactionType = Type _
                                          AndAlso x.Processed = False _
                                          AndAlso ((x.Time = DateAdd(DateInterval.Second, DeltaSecs, Time)) OrElse (x.Time = DateAdd(DateInterval.Second, -DeltaSecs, Time))) _
                                          AndAlso (Currency.Length = 0 OrElse x.Currency1 = Currency) _
                                          AndAlso (NotCurrency.Length = 0 OrElse x.Currency1 <> NotCurrency) _
                                          AndAlso (Description.Length = 0 OrElse x.Description = Description) _
                                          AndAlso (FeeTransactionID.Length = 0 OrElse x.FeeTransactionID = FeeTransactionID) _
                                          AndAlso (IndexToSkip = -1 OrElse x.InternalID <> IndexToSkip))
                If i <> -1 Then
                    Exit For
                End If
            Next
            If i <> -1 Then
                Return i
            Else
                Return -1
            End If
        Catch ex As Exception
            Return -1
        End Try

    End Function


    ''' <summary>
    ''' Normalizes (replaces) some expressions used in Bitfinex ledger exports
    ''' </summary>
    ''' <param name="Expression">String containing the terms to be replaced</param>
    ''' <returns>Normalized string</returns>
    Private Function Normalize_Entry(Expression As String) As String
        Dim Replacements() As String = {
            "Mega", "",
            "wallet Exchange", "Exchange wallet",
            "wallet exchange", "Exchange wallet",
            "IOTA", "IOT",
            "QASH", "QSH",
            "QTUM", "QTM",
            "DASH", "DSH",
            "DATA", "DAT"}
        Dim Result As String = Expression
        For i As Integer = 0 To Replacements.Length - 2 Step 2
            Result = Result.Replace(Replacements(i), Replacements(i + 1))
        Next
        Return Result
    End Function

    Private _ConvertFromUTC As Boolean
    ''' <summary>
    ''' Indicates if the datetime values in the csv file are in utc and need to be converted to local time. (This is the case for 'fake'csv imports invoked by api imports)
    ''' </summary>
    Public Property ConvertFromUTC() As Boolean
        Get
            Return _ConvertFromUTC
        End Get
        Set(ByVal value As Boolean)
            _ConvertFromUTC = value
        End Set
    End Property

End Class

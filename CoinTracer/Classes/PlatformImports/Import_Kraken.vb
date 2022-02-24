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
Imports CoinTracer.My.Resources


Public Class Import_Kraken
    Inherits FileImportBase
    Implements IFileImport


    Private Const PLATFORMID = PlatformManager.Platforms.Kraken
    Private Const PLATFORMFULLNAME As String = "Kraken.com"


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
                {.PlatformID = PLATFORMID,                  ' Kraken.com Leger (until about 2020)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = """txid"",""refid"",""time"",""type"",""aclass"",""asset"",""amount"",""fee"",""balance""",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Kraken.com Ledger (2021 and later)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = """txid"",""refid"",""time"",""type"",""subtype"",""aclass"",""asset"",""amount"",""fee"",""balance""",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Kraken.com Trades (2021 and later)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = """txid"",""ordertxid"",""pair"",""time"",""type"",""ordertype"",""price"",""cost"",""fee"",""vol"",""margin"",""misc"",""ledgers""",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 65}
                }
            Return Result
        End Get
    End Property


    ''' <summary>
    ''' Represents a single line from Kraken export files
    ''' </summary>
    Friend Class KrakenLineObject

        Private _Import As Import

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _TxId As String
        Public ReadOnly Property TxId() As String
            Get
                Return _TxId
            End Get
        End Property

        Private _RefId As String
        Public ReadOnly Property RefId() As String
            Get
                Return _RefId
            End Get
        End Property

        Private _Type As String
        Public ReadOnly Property Type() As String
            Get
                Return _Type
            End Get
        End Property

        Private _Asset As String
        Public ReadOnly Property Asset() As String
            Get
                Return _Asset
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Fee As Decimal
        Public Property Fee() As Decimal
            Get
                Return _Fee
            End Get
            Set(ByVal value As Decimal)
                _Fee = value
            End Set
        End Property

        Private _ID As String
        Public ReadOnly Property ID() As String
            Get
                Return _ID
            End Get
        End Property

        Public Sub New(ByRef Import As Import)
            _Import = Import
        End Sub

        Public Sub New(ByRef Import As Import,
                       ByRef DateTime As String,
                       ByRef TxId As String,
                       ByRef RefId As String,
                       ByRef Type As String,
                       ByRef Asset As String,
                       ByRef Amount As String,
                       ByRef Fee As String)
            Me.New(Import)
            _Date = CType(DateTime, Date).ToLocalTime
            _RefId = RefId.Trim
            If TxId.Length < 6 Then
                _TxId = RefId
            Else
                _TxId = TxId.Trim.Substring(0, 6)
            End If
            _Type = Type.Trim
            _Asset = ExtractAssetCode(Asset.Trim)
            If Amount IsNot Nothing AndAlso Amount.Length > 0 Then
                _Amount = Decimal.Parse(Amount, CultureInfo.InvariantCulture)
            Else
                _Amount = 0
            End If
            If Fee IsNot Nothing AndAlso Fee.Length > 0 Then
                _Fee = Decimal.Parse(Fee, CultureInfo.InvariantCulture)
            Else
                _Fee = 0
            End If
            _ID = DateTime & TxId & RefId & Type & Asset & Amount & Fee

        End Sub

    End Class

    Private Const KRAKEN_IMPORT_TRADES = 65 ' SubType for importing trades (instead of legder entries)

    Private Shared Function ExtractAssetCode(ByRef AssetRaw As String) As String
        If AssetRaw.StartsWith("X"c) Or AssetRaw.StartsWith("Z"c) Then
            Return AssetRaw.Substring(1, AssetRaw.Length - 1)
        ElseIf AssetRaw = "KFEE" Then
            Return "FEE"
        Else
            Return AssetRaw
        End If
    End Function

    ''' <summary>
    ''' Initializes this import
    ''' </summary>
    ''' <param name="MainImportObject">Reference to the calling import object</param>
    Public Sub New(MainImportObject As Import)
        MyBase.New(MainImportObject)

        Platform = PLATFORMID
        PlatformName = PLATFORMFULLNAME
        CSVAutoDetectEncoding = False
        CSVEncoding = Text.Encoding.UTF8
        CSVDecimalPoint = "."c
        CSVDecimalSeparator = ""
        CSVSeparator = ","c
        CSVTextqualifier = """"c
        MultiSelectFiles = False
        FileDialogTitle = MyStrings.importOpenFileFilterTitleKraken
        FileDialogFilter = MyStrings.importOpenFileFilterKraken
    End Sub

    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportKrakenCSV", DialogResult.OK,
                                                        String.Format(My.Resources.MyStrings.importMsgKrakenFile, Environment.NewLine),
                                                        My.Resources.MyStrings.importMsgKrakenFileCaption,
                                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Show some information about importing trades instead of ledgers
    ''' </summary>
    Protected Overrides Sub PreImportUserAdvice()
        If SubType = 65 AndAlso Not MainImportObject.SilentMode Then
            ' we are about to import a trades csv
            MsgBoxEx.ShowWithNotAgainOption("ImportKrakenTrades",
                                            DialogResult.OK,
                                            MyStrings.importMsgKrakenTradesCSV,
                                            MyStrings.importMsgKrakenTradesCSVCaption,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information)
        End If
    End Sub
    ''' <summary>
    ''' Perform the actual import for Kraken.com files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean

        Dim l As Long
        Dim AllLines As Long
        Dim ErrCounter As Long = MaxErrors
        Dim Row() As String
        Dim ProcessedTx As New List(Of String)
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As New List(Of dtoTradesRecord)
        Dim KontoRow As KontenRow

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, Platform) = 0 Then
                ' File has the wrong format!
                ImportFileHelper.InvalidFileMessage(FileNames(0))
                Return False
                Exit Function
            End If
            InitProgressForm(String.Format(MyStrings.importMsgImportStarting, PlatformName))
            AllLines = AllRows.Count
            If SubType < KRAKEN_IMPORT_TRADES And AllLines > 0 Then
                ' *** Read ledger entries ***
                Dim ImportLedersTb As New Import_KrakenDataSet.LedgerDataTable
                Dim ImportLedersTempTb As New Import_KrakenDataSet.LedgerDataTable
                ' Fill the ledger import table
                For l = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, l, MyStrings.importMsgKrakenFileReadFile, l * 50 / AllLines)
                        Row = CSV.Rows(l)
                        If SubType = 0 AndAlso Row.Length >= 9 Then
                            ImportLedersTb.AddLedgerRow(Row(0),
                                                        Row(1),
                                                        Row(2),
                                                        Row(3),
                                                        "",
                                                        Row(4),
                                                        ExtractAssetCode(Row(5).Trim),
                                                        Double.Parse(Row(6), CultureInfo.InvariantCulture),
                                                        Double.Parse(Row(7), CultureInfo.InvariantCulture),
                                                        Double.Parse(IIf(Row(8) = String.Empty, "0", Row(8)), CultureInfo.InvariantCulture),
                                                        False)
                        ElseIf Row.Length >= 10 Then
                            ImportLedersTb.AddLedgerRow(Row(0),
                                                        Row(1),
                                                        Row(2),
                                                        Row(3),
                                                        Row(4),
                                                        Row(5),
                                                        ExtractAssetCode(Row(6).Trim),
                                                        Double.Parse(Row(7), CultureInfo.InvariantCulture),
                                                        Double.Parse(Row(8), CultureInfo.InvariantCulture),
                                                        Double.Parse(IIf(Row(9) = String.Empty, "0", Row(9)), CultureInfo.InvariantCulture),
                                                        False)
                        End If
                    Catch ex As Exception
                        If FileImportError(ErrCounter, l, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next
                ' Process the ledger import table
                Try
                    Dim IR2nd As Import_KrakenDataSet.LedgerRow
                    Dim SourceIR As Import_KrakenDataSet.LedgerRow
                    Dim TargetIR As Import_KrakenDataSet.LedgerRow
                    Dim SourceKontoRow As KontenRow
                    Dim DlgResult As DialogResult
                    Dim ZeroValueTradeLimit As Decimal = My.Settings.ImportKrakenZeroValueTradeLimit

                    For Each IR As Import_KrakenDataSet.LedgerRow In ImportLedersTb
                        l = IR.ID + 1
                        UpdateProgress(AllLines, l, MyStrings.importMsgKrakenFileCheckLines, 50 + (l * 50 / AllLines))
                        ' Skip already processed rows
                        If IR.processed Then Continue For
                        IR.processed = True
                        ' Skip strange entries with empty refid
                        If IR.refid = String.Empty Then Continue For
                        ' Skip this row, if it has already been processed (to prevent errors resulting from duplicate rows)
                        If IR.txid <> String.Empty Then
                            If ProcessedTx.Contains(IR.txid) Then Continue For
                            ProcessedTx.Add(IR.txid)
                        End If
                        ' Build ImportRecord entry
                        Record = New dtoTradesRecord
                        RecordFee = Nothing
                        With Record
                            .Zeitpunkt = IR.time
                            .ZeitpunktZiel = .Zeitpunkt
                            .ImportPlattformID = Platform
                            .Info = FirstCharToUppercase(IR.type) & " " & IR.asset
                            KontoRow = MainImportObject.RetrieveAccount(IR.asset)
                            ' shorten the txid for compatibility
                            If Len(IR.txid) > 6 Then
                                IR.txid = IR.txid.Substring(0, 6)
                            End If
                            ' Take care of the "transfer" type: can be a deposit or a withdrawal
                            If IR.type.ToLower = "transfer" Then
                                If IR.amount >= 0 Then
                                    IR.type = "deposit"
                                Else
                                    IR.type = "withdraw"
                                End If
                                IR.amount = Math.Abs(IR.amount)
                            End If
                            Select Case IR.type.Substring(0, Math.Min(8, IR.type.Length)).ToLower
                                Case "deposit"
                                    If IR.txid = "" Then
                                        ' Most deposits appear twice, filter out the one with the empty txid.
                                        .DoNotImport = True
                                    Else
                                        .SourceID = IR.txid
                                        .TradetypID = DBHelper.TradeTypen.Einzahlung
                                        If IR.asset = "FEE" Then
                                            .QuellPlattformID = PlatformManager.Platforms.Extern
                                        Else
                                            .QuellPlattformID = PlatformManager.Platforms.Unknown
                                        End If
                                        .ZielPlattformID = Platform
                                        .ZielBetrag = IR.amount
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                        If IR.fee > 0 Then
                                            ' There is a fee, so BetragNachGebühr needs to be reduced
                                            .BetragNachGebuehr -= IR.fee
                                        End If
                                        .ZielKontoID = KontoRow.ID
                                        .QuellKontoID = .ZielKontoID
                                    End If
                                Case "withdraw"
                                    If IR.txid = "" Then
                                        ' Most withdrawals appear twice, filter out the one with the empty txid.
                                        .DoNotImport = True
                                    Else
                                        .SourceID = IR.txid
                                        .TradetypID = DBHelper.TradeTypen.Auszahlung
                                        .QuellPlattformID = Platform
                                        .ZielPlattformID = PlatformManager.Platforms.Unknown
                                        .QuellBetrag = Math.Abs(IR.amount)
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .ZielBetrag = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                        .QuellKontoID = KontoRow.ID
                                        .ZielKontoID = .QuellKontoID
                                        If IR.fee > 0 Then
                                            '  There is a fee, so BetragNachGebühr needs to be reduced
                                            .QuellBetrag += IR.fee
                                        End If
                                    End If
                                Case "trade"
                                    .SourceID = IR.txid
                                    ' get corresponding ledger row (not kraken fee)
                                    IR2nd = ImportLedersTb.Where(Function(r)
                                                                     Return r.refid = IR.refid And r.asset <> "FEE" And Not r.processed And r.ID <> IR.ID
                                                                 End Function).FirstOrDefault()
                                    If IR2nd Is Nothing Then
                                        ' No matching row found. This is only acceptable for very small amounts
                                        If IR.amount <= ZeroValueTradeLimit Then
                                            DlgResult = DialogResult.Yes
                                        ElseIf MainImportObject.SilentMode Then
                                            DlgResult = DialogResult.Yes
                                        Else
                                            DlgResult = MsgBoxEx.ShowInFront(String.Format(MyStrings.importMsgKrakenWarningNoSecondEntry, IR.txid, l, IR.amount.ToString(Import.INFONUMBERFORMAT, CultureInfo.InvariantCulture), IR.asset),
                                                                             MyStrings.importMsgKrakenWarningNoSecondEntryTitle,
                                                                             MessageBoxButtons.YesNoCancel,
                                                                             MessageBoxIcon.Exclamation,
                                                                             MessageBoxDefaultButton.Button1)
                                        End If
                                        If DlgResult = DialogResult.Yes Then
                                            ' assume the corresponding second entry would be zero
                                            IR2nd = ImportLedersTempTb.AddLedgerRow(IR.txid, IR.refid, IR.time, IR.type, IR.subtype, IR.aclass, "EUR", 0, 0, 0, True)
                                        ElseIf DlgResult = DialogResult.No Then
                                            ' skip this entry
                                            Continue For
                                        Else
                                            ' abort import
                                            Throw New ImportFileException(String.Format(MyStrings.importMsgKrakenErrorNoSecondEntry, .SourceID))
                                        End If
                                    Else
                                        IR2nd.processed = True
                                        IR2nd.txid = IR2nd.txid.Substring(0, 6)
                                    End If
                                    ' Set up the trades entries
                                    If IR.amount < 0 OrElse (IR.amount = 0 And IR2nd.amount > 0) Then
                                        SourceIR = IR
                                        TargetIR = IR2nd
                                        SourceKontoRow = KontoRow
                                        KontoRow = MainImportObject.RetrieveAccount(IR2nd.asset)
                                    ElseIf IR2nd.amount <= 0 Then
                                        SourceIR = IR2nd
                                        TargetIR = IR
                                        SourceKontoRow = MainImportObject.RetrieveAccount(IR2nd.asset)
                                    Else
                                        Throw New Exception(String.Format(MyStrings.importMsgKrakenErrorNoNegativeValue, .SourceID))
                                    End If
                                    .SourceID = SourceIR.txid & "-" & TargetIR.txid & "," & TargetIR.txid & "-" & SourceIR.txid
                                    .QuellPlattformID = Platform
                                    .ZielPlattformID = .QuellPlattformID
                                    .ZielKontoID = KontoRow.ID
                                    .ZielBetrag = TargetIR.amount
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellBetrag = Math.Abs(SourceIR.amount)
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .QuellKontoID = SourceKontoRow.ID
                                    .Info = String.Format("Trade {0} ({1})", KontoRow.Bezeichnung, KontoRow.Code)
                                    If KontoRow.IstFiat AndAlso SourceKontoRow.IstFiat Then
                                        ' fiat vs. fiat always treated as a buy transaction
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                    ElseIf KontoRow.IstFiat = False Then
                                        ' Coins bought (via fiat or coins)
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                    Else
                                        ' Coins sold
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                    End If
                                    If TargetIR.fee <> 0 Then
                                        ' Fee on target side, we have to reduce the target amount
                                        .BetragNachGebuehr -= Math.Abs(TargetIR.fee)
                                    End If
                                    ' watch out: Kraken can have 2 fees! One on the source-side and the other on the target-side.
                                    If SourceIR.fee <> 0 Then
                                        ' Fee on source side. Increase the source amount
                                        .QuellBetrag += Math.Abs(SourceIR.fee)
                                    End If
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If

                                Case Else
                                    .DoNotImport = True
                            End Select

                            ' check for fees on target side
                            If .BetragNachGebuehr < .ZielBetrag Then
                                RecordFee = .Clone()
                                RecordFee.SourceID = .SourceID.Replace(","c, "/"c) & "/fee"
                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                RecordFee.BetragNachGebuehr = 0
                                RecordFee.QuellKontoID = .ZielKontoID
                                RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                RecordFee.ZielPlattformID = Platform
                                RecordFee.WertEUR = 0
                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                RecordFee.Info = String.Format(MyStrings.importInfoTradeFee, .SourceID)
                                ImportRecords.Add(RecordFee)
                                RecordFee = Nothing
                            End If
                            ' check for fees on source side
                            If .QuellBetragNachGebuehr < .QuellBetrag Then
                                RecordFee = .Clone()
                                RecordFee.SourceID = .SourceID.Replace(","c, "/"c) & "/fee" & IIf(.BetragNachGebuehr < .ZielBetrag, "2", "")
                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                RecordFee.BetragNachGebuehr = 0
                                RecordFee.QuellKontoID = .QuellKontoID
                                RecordFee.ZielBetrag = .QuellBetrag - .QuellBetragNachGebuehr
                                RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                RecordFee.ZielPlattformID = Platform
                                RecordFee.WertEUR = 0
                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                RecordFee.Info = String.Format(MyStrings.importInfoTradeFee, .SourceID)
                            End If

                            If RecordFee Is Nothing AndAlso (.TradetypID = DBHelper.TradeTypen.Kauf OrElse .TradetypID = DBHelper.TradeTypen.Verkauf) Then
                                ' check for kraken fee credits row
                                IR2nd = ImportLedersTb.Where(Function(r)
                                                                 Return r.refid = IR.refid And r.asset = "FEE" And Not r.processed And r.ID <> IR.ID
                                                             End Function).FirstOrDefault()
                                If IR2nd IsNot Nothing Then
                                    IR2nd.processed = True
                                    ' create kraken fee import entry
                                    RecordFee = .Clone()
                                    RecordFee.SourceID = .SourceID.Replace(","c, "/"c) & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellKontoID = MainImportObject.GetAccount(IR2nd.asset).ID
                                    RecordFee.QuellBetrag = IR2nd.fee
                                    RecordFee.ZielBetrag = RecordFee.QuellBetrag
                                    RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = String.Format(MyStrings.ImportInfoKrakenTradeFeeCredits, .SourceID)
                                End If
                            End If

                            If Not .DoNotImport Then
                                ' add import record entry
                                ImportRecords.Add(Record)
                                If Not RecordFee Is Nothing Then
                                    ImportRecords.Add(RecordFee)
                                End If
                            End If

                        End With

                    Next

                Catch ex As ImportFileException
                    ' User has decided to abort import
                    ErrCounter = 1
                    FileImportError(ErrCounter, l, ex)
                    Return False
                    Exit Function
                Catch ex As Exception
                    If FileImportError(ErrCounter, l, ex) = 0 Then
                        Return False
                        Exit Function
                    End If
                End Try

            ElseIf SubType >= KRAKEN_IMPORT_TRADES And AllLines > 0 Then
                ' *** Read trade entries ***
                Dim ImportTradesTb As New Import_KrakenDataSet.TradesDataTable
                ' Fill the trades import table
                Try
                    For l = 0 To AllLines - 1
                        UpdateProgress(AllLines, l, MyStrings.importMsgKrakenFileReadFile, l * 50 / AllLines)
                        Row = CSV.Rows(l)
                        If Row.Length >= 13 Then
                            ImportTradesTb.AddTradesRow(Row(0),
                                                    Row(1),
                                                    Row(2),
                                                    Row(3),
                                                    Row(4),
                                                    Row(5),
                                                    Double.Parse(Row(6), CultureInfo.InvariantCulture),
                                                    Double.Parse(Row(7), CultureInfo.InvariantCulture),
                                                    Double.Parse(Row(8), CultureInfo.InvariantCulture),
                                                    Double.Parse(Row(9), CultureInfo.InvariantCulture),
                                                    Double.Parse(Row(10), CultureInfo.InvariantCulture),
                                                    Row(11),
                                                    Row(12),
                                                    False)
                        End If
                    Next
                Catch ex As Exception
                    If FileImportError(ErrCounter, l, ex) = 0 Then
                        Return False
                        Exit Function
                    End If
                End Try

                ' Process the trades import table
                Try
                    Dim LedgerIDs As String()
                    Dim KFeeIndicator As UInt32
                    Dim AssetCode1 As String
                    Dim AssetCode2 As String
                    Dim KontoRow2 As KontenRow
                    For Each IR As Import_KrakenDataSet.TradesRow In ImportTradesTb
                        Try
                            l = IR.ID + 1
                            UpdateProgress(AllLines, l, MyStrings.importMsgKrakenFileCheckLines, 50 + (l * 50 / AllLines))
                            ' Skip already processed rows
                            If IR.processed Then Continue For
                            IR.processed = True
                            ' Skip this row, if it has already been processed (to prevent errors resulting from duplicate rows)
                            If IR.txid <> String.Empty Then
                                If ProcessedTx.Contains(IR.txid) Then Continue For
                                ProcessedTx.Add(IR.txid)
                            End If
                            ' Build ImportRecord entry
                            Record = New dtoTradesRecord
                            RecordFee = Nothing
                            With Record
                                .Zeitpunkt = IR.time
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = Platform
                                If Len(IR.pair) > 6 Then
                                    AssetCode1 = ExtractAssetCode(IR.pair.Substring(0, 4))
                                    AssetCode2 = ExtractAssetCode(IR.pair.Substring(4, 4))
                                Else
                                    AssetCode1 = IR.pair.Substring(0, 3)
                                    AssetCode2 = IR.pair.Substring(3, 3)
                                End If
                                '.Info = FirstCharToUppercase(IR.type) & " " & IR.pair.Substring(1, 3)
                                KontoRow = MainImportObject.RetrieveAccount(AssetCode1)
                                KontoRow2 = MainImportObject.RetrieveAccount(AssetCode2)
                                LedgerIDs = IR.ledgers.Split(","c)
                                ' Shorten ledger ids for backwards compatibility
                                For a As Integer = 0 To LedgerIDs.Length - 1
                                    LedgerIDs(a) = LedgerIDs(a).Substring(0, 6)
                                Next
                                ' Check if we have three legers ids involved, which means the fee is paid via KFEE
                                If LedgerIDs.Length > 2 Then
                                    KFeeIndicator = 1
                                Else
                                    KFeeIndicator = 0
                                End If
                                .SourceID = LedgerIDs(KFeeIndicator) & "-" & LedgerIDs(1 + KFeeIndicator) & "," & LedgerIDs(1 + KFeeIndicator) & "-" & LedgerIDs(KFeeIndicator)
                                Select Case IR.type.ToLower
                                    Case "buy"
                                        .QuellPlattformID = Platform
                                        .ZielPlattformID = .QuellPlattformID
                                        .ZielKontoID = KontoRow.ID
                                        .ZielBetrag = IR.vol
                                        .BetragNachGebuehr = .ZielBetrag
                                        .QuellBetrag = IR.cost
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = KontoRow2.ID
                                        .Info = String.Format("Trade {0} ({1})", KontoRow.Bezeichnung, KontoRow.Code)
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                        If KFeeIndicator = 0 AndAlso IR.fee > 0 Then
                                            ' we have a countable fee -> add it to the cost
                                            .QuellBetrag += IR.fee
                                        End If
                                        If .QuellKontoID = AccountManager.Accounts.EUR Then
                                            .WertEUR = .QuellBetrag
                                        ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                            .WertEUR = .BetragNachGebuehr
                                        End If

                                    Case "sell"
                                        If Not KontoRow2.IstFiat Then
                                            ' we are selling crypto - convert this into a buy
                                            .QuellPlattformID = Platform
                                            .ZielPlattformID = .QuellPlattformID
                                            .ZielKontoID = KontoRow2.ID
                                            .ZielBetrag = IR.cost
                                            .BetragNachGebuehr = .ZielBetrag
                                            .QuellBetrag = IR.vol
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .QuellKontoID = KontoRow.ID
                                            .Info = String.Format("Trade {0} ({1})", KontoRow2.Bezeichnung, KontoRow2.Code)
                                            .TradetypID = DBHelper.TradeTypen.Kauf  ' TODO: Check, because this has been Verkauf before?!?
                                            If KFeeIndicator = 0 AndAlso IR.fee > 0 Then
                                                ' we have a countable fee -> substract from target amount
                                                .BetragNachGebuehr -= IR.fee
                                            End If
                                        Else
                                            ' we are selling crypto for fiat
                                            .QuellPlattformID = Platform
                                            .ZielPlattformID = .QuellPlattformID
                                            .ZielKontoID = KontoRow2.ID
                                            .ZielBetrag = IR.cost
                                            .BetragNachGebuehr = .ZielBetrag
                                            .QuellBetrag = IR.vol
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .QuellKontoID = KontoRow.ID
                                            .Info = String.Format("Trade {0} ({1})", KontoRow.Bezeichnung, KontoRow.Code)
                                            If KontoRow.IstFiat AndAlso KontoRow2.IstFiat Then
                                                ' Special case: always consider fiat vs. fiat buying
                                                .TradetypID = DBHelper.TradeTypen.Kauf
                                            Else
                                                .TradetypID = DBHelper.TradeTypen.Verkauf
                                            End If
                                            If KFeeIndicator = 0 AndAlso IR.fee > 0 Then
                                                ' we have a countable fee -> substract from target amount
                                                .BetragNachGebuehr -= IR.fee
                                            End If
                                        End If
                                        If .QuellKontoID = AccountManager.Accounts.EUR Then
                                            .WertEUR = .QuellBetrag
                                        ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                            .WertEUR = .BetragNachGebuehr
                                        End If
                                    Case Else
                                        .DoNotImport = True
                                End Select

                                ' check for fees on target side
                                If .BetragNachGebuehr < .ZielBetrag Then
                                    RecordFee = .Clone()
                                    RecordFee.SourceID = .SourceID.Replace(","c, "/"c) & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellKontoID = .ZielKontoID
                                    RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                    RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                    RecordFee.ZielPlattformID = Platform
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = String.Format(MyStrings.importInfoTradeFee, .SourceID)
                                    ImportRecords.Add(RecordFee)
                                    RecordFee = Nothing
                                End If
                                ' check for fees on source side
                                If .QuellBetragNachGebuehr < .QuellBetrag Then
                                    RecordFee = .Clone()
                                    RecordFee.SourceID = .SourceID.Replace(","c, "/"c) & "/fee" & IIf(.BetragNachGebuehr < .ZielBetrag, "2", "")
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellKontoID = .QuellKontoID
                                    RecordFee.ZielBetrag = .QuellBetrag - .QuellBetragNachGebuehr
                                    RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                    RecordFee.ZielPlattformID = Platform
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = String.Format(MyStrings.importInfoTradeFee, .SourceID)
                                End If

                                If Not .DoNotImport Then
                                    ' add import record entry
                                    ImportRecords.Add(Record)
                                    If Not RecordFee Is Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If

                            End With
                        Catch ex As Exception
                            If FileImportError(ErrCounter, l, ex) = 0 Then
                                Return False
                                Exit Function
                            End If
                        End Try
                    Next

                Catch ex As Exception
                    If FileImportError(ErrCounter, l, ex) = 0 Then
                        Return False
                        Exit Function
                    End If
                End Try
            End If

            MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage, , True)
            Cursor.Current = Cursors.Default

        End If

        Return ErrCounter = MaxErrors

    End Function

End Class

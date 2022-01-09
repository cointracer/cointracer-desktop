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

Public Class Import_Binance
    Inherits FileImportBase
    Implements IFileImport

    Private Const PLATFORMID = PlatformManager.Platforms.Binance
    Private Const PLATFORMFULLNAME As String = "Binance.com"


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
                {.PlatformID = PLATFORMID,                  ' Trade history
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date(UTC),Market,Type,Price,Amount,Total,Fee,Fee Coin",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Crypto deposit or withdrawal history
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date(UTC),Coin,Amount,TransactionFee,Address,TXID,SourceAddress,PaymentID,Status",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Fiat deposit or withdrawal history
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date(UTC*),Coin,Amount,Status,Payment Method,Indicated Amount,Fee,Order ID",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.LikeMatch,
                 .SubType = 2},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Crypto convert history
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date,Pair,Type,Sell,Buy,Price,Inverse Price,Date Updated,Status",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 3},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Trade history as zip file
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date(UTC),Pair,Side,Price,Executed,Amount,Fee",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 4}
                }
            Return Result
        End Get
    End Property


    Private Class BinanceTradeHistoryRow

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Market As String
        Public ReadOnly Property Market() As String
            Get
                Return _Market
            End Get
        End Property

        Private _Market1Account As KontenRow
        Public ReadOnly Property Market1Account() As KontenRow
            Get
                Return _Market1Account
            End Get
        End Property

        Private _Market2Account As KontenRow
        Public ReadOnly Property Market2Account() As KontenRow
            Get
                Return _Market2Account
            End Get
        End Property

        Private _Type As String
        Public ReadOnly Property Type() As String
            Get
                Return _Type
            End Get
        End Property

        Private _Price As Decimal
        Public ReadOnly Property Price() As Decimal
            Get
                Return _Price
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Total As Decimal
        Public ReadOnly Property Total() As Decimal
            Get
                Return _Total
            End Get
        End Property

        Private _Fee As Decimal
        Public ReadOnly Property Fee() As Decimal
            Get
                Return _Fee
            End Get
        End Property

        Private _FeeCoin As String
        Public ReadOnly Property FeeCoin() As String
            Get
                Return _FeeCoin
            End Get
        End Property

        Private _FeeCoinAccount As KontenRow
        Public ReadOnly Property FeeCoinAccount() As KontenRow
            Get
                Return _FeeCoinAccount
            End Get
        End Property

        Public Sub New(ByRef Import As Import,
                       ByRef DataRow() As String,
                       ByRef Market1 As String,
                       ByRef Market2 As String)
            _Market1Account = Nothing
            _Market2Account = Nothing
            _FeeCoinAccount = Nothing

            _Date = CType(DataRow(0), Date).ToLocalTime
            _Market = DataRow(1).Trim
            _Type = DataRow(2).Trim.ToUpper
            _Price = ParseAbsDecimal(DataRow(3))
            _Amount = ParseAbsDecimal(DataRow(4))
            _Total = ParseAbsDecimal(DataRow(5))
            _Fee = ParseAbsDecimal(DataRow(6))
            _FeeCoin = DataRow(7).Trim

            _Market1Account = Import.RetrieveAccount(Market1)
            _Market2Account = Import.RetrieveAccount(Market2)
            _FeeCoinAccount = Import.RetrieveAccount(_FeeCoin)
        End Sub

    End Class


    Private Class BinanceCryptoFundingHistoryRow

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Coin As String
        Public ReadOnly Property Coin() As String
            Get
                Return _Coin
            End Get
        End Property

        Private _CoinAccount As KontenRow
        Public ReadOnly Property CoinAccount() As KontenRow
            Get
                Return _CoinAccount
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _TransactionFee As Decimal
        Public ReadOnly Property TransactionFee() As Decimal
            Get
                Return _TransactionFee
            End Get
        End Property

        Private _Address As String
        Public ReadOnly Property Address() As String
            Get
                Return _Address
            End Get
        End Property

        Private _TXID As String
        Public ReadOnly Property TXID() As String
            Get
                Return _TXID
            End Get
        End Property

        Private _SourceAddress As String
        Public ReadOnly Property SourceAddress() As String
            Get
                Return _SourceAddress
            End Get
        End Property

        Private _PaymentID As String
        Public ReadOnly Property PaymentID() As String
            Get
                Return _PaymentID
            End Get
        End Property

        Private _Status As String
        Public ReadOnly Property Status() As String
            Get
                Return _Status
            End Get
        End Property

        Public Sub New(ByRef Import As Import,
                       ByRef DataRow() As String)
            _CoinAccount = Nothing
            _Date = CType(DataRow(0), Date).ToLocalTime
            _Coin = DataRow(1).Trim.ToUpper
            _CoinAccount = Import.RetrieveAccount(_Coin)
            _Amount = ParseAbsDecimal(DataRow(2))
            _TransactionFee = ParseAbsDecimal(DataRow(3))
            _Address = DataRow(4)
            _TXID = DataRow(5)
            _SourceAddress = DataRow(6)
            _PaymentID = DataRow(7)
            _Status = DataRow(8).ToLower
        End Sub

    End Class


    Private Class BinanceFiatFundingHistoryRow

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Coin As String
        Public ReadOnly Property Coin() As String
            Get
                Return _Coin
            End Get
        End Property

        Private _CoinAccount As KontenRow
        Public ReadOnly Property CoinAccount() As KontenRow
            Get
                Return _CoinAccount
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Status As String
        Public ReadOnly Property Status() As String
            Get
                Return _Status
            End Get
        End Property

        Private _IndicatedAmount As Decimal
        Public ReadOnly Property IndicatedAmount() As Decimal
            Get
                Return _IndicatedAmount
            End Get
        End Property

        Private _Fee As Decimal
        Public ReadOnly Property Fee() As Decimal
            Get
                Return _Fee
            End Get
        End Property

        Private _OrderID As String
        Public ReadOnly Property OrderID() As String
            Get
                Return _OrderID
            End Get
        End Property

        Public Sub New(ByRef Import As Import,
                       ByRef DataRow() As String)
            _CoinAccount = Nothing
            _Date = CType(DataRow(0), Date)
            _Coin = DataRow(1).Trim.ToUpper
            _CoinAccount = Import.RetrieveAccount(_Coin)
            _Amount = ParseAbsDecimal(DataRow(2))
            _Status = DataRow(3).ToLower
            _IndicatedAmount = ParseAbsDecimal(DataRow(5))
            _Fee = ParseAbsDecimal(DataRow(6))
            _OrderID = DataRow(7)
        End Sub

    End Class


    Private Class BinanceConvertHistoryRow

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Pair As String
        Public ReadOnly Property Pair() As String
            Get
                Return _Pair
            End Get
        End Property

        Private _SellAccount As KontenRow
        Public ReadOnly Property SellAccount() As KontenRow
            Get
                Return _SellAccount
            End Get
        End Property

        Private _SellValue As Decimal
        Public ReadOnly Property SellValue() As Decimal
            Get
                Return _SellValue
            End Get
        End Property

        Private _BuyAccount As KontenRow
        Public ReadOnly Property BuyAccount() As KontenRow
            Get
                Return _BuyAccount
            End Get
        End Property

        Private _BuyValue As Decimal
        Public ReadOnly Property BuyValue() As Decimal
            Get
                Return _BuyValue
            End Get
        End Property

        Private _Status As String
        Public ReadOnly Property Status() As String
            Get
                Return _Status
            End Get
        End Property
        Public Sub New(ByRef Import As Import,
                       ByRef DataRow() As String)
            _SellAccount = Nothing
            _BuyAccount = Nothing
            _Date = CType(DataRow(7), Date).ToLocalTime
            Dim Values() As String = Split(DataRow(3), " ")
            _SellValue = ParseAbsDecimal(Values(0))
            _SellAccount = Import.RetrieveAccount(Values(1))
            Values = Split(DataRow(4), " ")
            _BuyValue = ParseAbsDecimal(Values(0))
            _BuyAccount = Import.RetrieveAccount(Values(1))
            _Status = DataRow(8).ToLower
        End Sub

    End Class


    Private Class BinanceTradeHistoryCsvRow

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Pair As String
        Public ReadOnly Property Pair() As String
            Get
                Return _Pair
            End Get
        End Property

        Private _Pair1Account As KontenRow
        Public ReadOnly Property Pair1Account() As KontenRow
            Get
                Return _Pair1Account
            End Get
        End Property

        Private _Pair2Account As KontenRow
        Public ReadOnly Property Pair2Account() As KontenRow
            Get
                Return _Pair2Account
            End Get
        End Property

        Private _Side As String
        Public ReadOnly Property Side() As String
            Get
                Return _Side
            End Get
        End Property

        Private _Price As Decimal
        Public ReadOnly Property Price() As Decimal
            Get
                Return _Price
            End Get
        End Property

        Private _Executed As Decimal
        Public ReadOnly Property Executed() As Decimal
            Get
                Return _Executed
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Fee As Decimal
        Public ReadOnly Property Fee() As Decimal
            Get
                Return _Fee
            End Get
        End Property

        Private _FeeAccount As KontenRow
        Public ReadOnly Property FeeAccount() As KontenRow
            Get
                Return _FeeAccount
            End Get
        End Property

        Public Sub New(ByRef Import As Import,
                       ByRef DataRow() As String,
                       ByRef Pair1 As String,
                       ByRef Pair2 As String)
            _Pair1Account = Nothing
            _Pair2Account = Nothing
            _FeeAccount = Nothing

            _Date = CType(DataRow(0), Date).ToLocalTime
            _Pair = DataRow(1).Trim.ToUpper
            _Side = DataRow(2).Trim.ToUpper
            _Price = ParseAbsDecimal(DataRow(3))
            _Executed = ParseAbsDecimal(DataRow(4), True)
            _Amount = ParseAbsDecimal(DataRow(5), True)
            _Fee = ParseAbsDecimal(DataRow(6), True)

            _Pair1Account = Import.RetrieveAccount(Pair1)
            _Pair2Account = Import.RetrieveAccount(Pair2)
            _FeeAccount = Import.RetrieveAccount(New String(Array.FindAll(DataRow(6).ToCharArray, Function(c) Not "0123456789.".Contains(c))))
        End Sub

    End Class


    ''' <summary>
    ''' Helper function for parsing decimal strings
    ''' </summary>
    Private Shared Function ParseAbsDecimal(ByVal ValueString As String, Optional ByVal FilterChars As Boolean = False) As Decimal
        If ValueString?.Length > 0 Then
            If Not FilterChars Then
                Return Math.Abs(Decimal.Parse(ValueString, CultureInfo.InvariantCulture))
            Else
                Return Math.Abs(Decimal.Parse(New String(Array.FindAll(ValueString.ToCharArray, Function(c) "0123456789.".Contains(c))), CultureInfo.InvariantCulture))
            End If
        Else
            Return 0
        End If
    End Function


    ' Use this list of base currencies for splitting the market string provided in the export files
    Private _BaseCurrencies() As String = {"AUD", "BIDR", "BKRW", "BNB", "BRL", "BTC", "BUSD", "BVND", "DAI", "DOGE", "ETH", "EUR", "GBP", "GYEN", "IDRT", "NGN", "PAX", "RUB", "TRX", "TRY", "TUSD", "UAH", "USDC", "USDP", "USDS", "USDT", "VAI", "XRP", "ZAR"}


    ''' <summary>
    ''' Split a given Market string into the corresponding currency pair strings
    ''' </summary>
    ''' <returns>True on success, False otherwise</returns>
    Private Function SplitMarket(ByRef Market As String,
                                 ByRef Market1 As String,
                                 ByRef Market2 As String) As Boolean
        If IsNothing(Market) OrElse Market.Length < 6 Then
            Market1 = String.Empty
            Market2 = String.Empty
            Return False
        End If
        For Each BaseCur In _BaseCurrencies
            If Market.ToUpper.EndsWith(BaseCur) Then
                ' we have a match!
                Market2 = BaseCur
                Market1 = Market.Substring(0, Market.Length - BaseCur.Length)
                Return True
            End If
        Next
        ' no known base currency - try to figure out the best result
        Dim BaseLen As Integer
        Select Case Market.Length
            Case 7, 8
                BaseLen = 4
            Case Else
                BaseLen = 3
        End Select
        Market2 = Market.Substring(Market.Length - BaseLen)
        Market1 = Market.Substring(0, Market.Length - BaseLen)
        Return True
    End Function


    ''' <summary>
    ''' Initializes this import
    ''' </summary>
    ''' <param name="MainImportObject">Reference to the calling import object</param>
    Public Sub New(MainImportObject As Import)
        MyBase.New(MainImportObject)

        Platform = PLATFORMID
        PlatformName = PLATFORMFULLNAME
        CSVEncoding = Text.Encoding.UTF8
        CSVAutoDetectEncoding = False
        MultiSelectFiles = False
        CheckFirstLine = True
        CSVDecimalPoint = "."c
        CSVDecimalSeparator = ","c
        CSVSeparator = ","c
        CSVTextqualifier = """"c
        FileDialogTitle = My.Resources.MyStrings.importOpenFileTitle
        FileDialogFilter = My.Resources.MyStrings.importOpenFileFilterBinance
    End Sub


    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportBinance", DialogResult.OK,
                                        My.Resources.MyStrings.importMsgBinanceFile,
                                        My.Resources.MyStrings.importMsgBinanceFileTitle,
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Perform the actual import for Binance files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean Implements IFileImport.ImportContent
        Dim Row() As String
        Dim ErrorCounter As Long = MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim AllLines As Long

        Cursor.Current = Cursors.WaitCursor
        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))

        ImportRecords = New List(Of dtoTradesRecord)
        AllLines = CSV.Rows.Count

        Select Case SubType
            Case 0
                ' *** Trade History ***
                Dim BHO As BinanceTradeHistoryRow
                Dim Market1 As String
                Dim Market2 As String

                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        Market1 = String.Empty
                        Market2 = String.Empty
                        If SplitMarket(Row(1), Market1, Market2) Then
                            BHO = New BinanceTradeHistoryRow(MainImportObject, Row, Market1, Market2)
                            If BHO IsNot Nothing Then
                                Record = New dtoTradesRecord
                                RecordFee = Nothing
                                With Record
                                    .SourceID = MD5FromString(String.Join(";", Row))
                                    .Zeitpunkt = BHO.DateTime
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = Platform
                                    Select Case BHO.Type
                                        Case "BUY"
                                            ' Buy
                                            .TradetypID = DBHelper.TradeTypen.Kauf
                                            .QuellPlattformID = Platform
                                            .ZielPlattformID = .QuellPlattformID
                                            .ZielKontoID = BHO.Market1Account.ID
                                            .ZielBetrag = BHO.Amount
                                            If BHO.Market1Account.ID = BHO.FeeCoinAccount.ID Then
                                                .BetragNachGebuehr = BHO.Amount - BHO.Fee
                                            Else
                                                .BetragNachGebuehr = BHO.Amount
                                            End If
                                            .QuellBetragNachGebuehr = BHO.Total
                                            If BHO.Market2Account.ID = BHO.FeeCoinAccount.ID Then
                                                .QuellBetrag = BHO.Total + BHO.Fee
                                            Else
                                                .QuellBetrag = BHO.Total
                                            End If
                                            .QuellKontoID = BHO.Market2Account.ID
                                            If .QuellKontoID = DBHelper.Konten.EUR Then
                                                .WertEUR = .QuellBetrag
                                            End If
                                            .Info = String.Format(My.Resources.MyStrings.importInfoGenericBuy,
                                                                        BHO.Market1Account.Code, BHO.Amount, BHO.Total, Market2)
                                        Case "SELL"
                                            ' Sell
                                            .TradetypID = DBHelper.TradeTypen.Verkauf
                                            .QuellPlattformID = Platform
                                            .ZielPlattformID = .QuellPlattformID
                                            .ZielKontoID = BHO.Market2Account.ID
                                            .ZielBetrag = BHO.Total
                                            If BHO.Market2Account.ID = BHO.FeeCoinAccount.ID Then
                                                .BetragNachGebuehr = BHO.Total - BHO.Fee
                                            Else
                                                .BetragNachGebuehr = BHO.Total
                                            End If
                                            .QuellBetragNachGebuehr = BHO.Amount
                                            If BHO.Market1Account.ID = BHO.FeeCoinAccount.ID Then
                                                .QuellBetrag = BHO.Amount + BHO.Fee
                                            Else
                                                .QuellBetrag = BHO.Amount
                                            End If
                                            .QuellKontoID = BHO.Market1Account.ID
                                            If .QuellKontoID = DBHelper.Konten.EUR Then
                                                .WertEUR = .QuellBetrag
                                            End If
                                            .Info = String.Format(My.Resources.MyStrings.importInfoGenericSell,
                                                                  BHO.Market1Account.Code, BHO.Amount, BHO.Total, Market2)
                                        Case Else
                                            ' Do not import
                                            .DoNotImport = True
                                    End Select

                                    If BHO.Fee > 0 Then
                                        ' Fee transaction
                                        RecordFee = .Clone()
                                        RecordFee.QuellPlattformID = Platform
                                        RecordFee.ZielPlattformID = Platform
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.ZielKontoID = BHO.FeeCoinAccount.GebuehrKontoID
                                        RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                        RecordFee.WertEUR = 0
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.QuellKontoID = BHO.FeeCoinAccount.ID
                                        RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoGenericFee,
                                                                       BHO.FeeCoin, .SourceID)
                                    End If
                                    If Not .DoNotImport Then
                                        ' Add record to list
                                        ImportRecords.Add(Record)
                                        If Not RecordFee Is Nothing Then
                                            ImportRecords.Add(RecordFee)
                                        End If
                                    End If

                                End With
                            End If
                        End If

                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next i

            Case 1
                ' *** Crypto deposit or withdrawal ***
                Dim IsDeposit As Boolean = FileNames(0).ToLower.Contains("deposit")
                Dim BHO As BinanceCryptoFundingHistoryRow

                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        BHO = New BinanceCryptoFundingHistoryRow(MainImportObject, Row)
                        If BHO IsNot Nothing AndAlso BHO.Status = "Completed" Then
                            Record = New dtoTradesRecord
                            RecordFee = Nothing
                            With Record
                                .SourceID = MD5FromString(String.Join(";", Row))
                                .Zeitpunkt = BHO.DateTime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = Platform
                                .WertEUR = 0
                                If IsDeposit Then
                                    ' Crypto deposit
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielPlattformID = Platform
                                    .ZielKontoID = BHO.CoinAccount.ID
                                    .ZielBetrag = BHO.Amount
                                    .BetragNachGebuehr = .ZielBetrag - BHO.TransactionFee
                                    .QuellBetrag = .ZielBetrag
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .QuellKontoID = .ZielKontoID
                                    .Info = String.Format(My.Resources.MyStrings.importInfoGenericInpayment,
                                                          BHO.Coin, BHO.TXID)
                                Else
                                    ' Crypto withdrawal
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .QuellPlattformID = Platform
                                    .ZielKontoID = BHO.CoinAccount.ID
                                    .QuellKontoID = .ZielKontoID
                                    .ZielBetrag = BHO.Amount
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellBetrag = .ZielBetrag + BHO.TransactionFee
                                    .QuellBetragNachGebuehr = .ZielBetrag
                                    .Info = String.Format(My.Resources.MyStrings.importInfoGenericPayout,
                                                          BHO.Coin, BHO.TXID)
                                End If

                                If BHO.TransactionFee > 0 Then
                                    ' Fee transaction
                                    RecordFee = .Clone()
                                    RecordFee.QuellPlattformID = Platform
                                    RecordFee.ZielPlattformID = Platform
                                    RecordFee.SourceID = .SourceID & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.ZielKontoID = BHO.CoinAccount.GebuehrKontoID
                                    RecordFee.ZielBetrag = BHO.TransactionFee
                                    RecordFee.WertEUR = 0
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.QuellKontoID = BHO.CoinAccount.ID
                                    RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoGenericFee,
                                                                   BHO.Coin, .SourceID)
                                End If
                                If Not .DoNotImport Then
                                    ' Add record to list
                                    ImportRecords.Add(Record)
                                    If Not RecordFee Is Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If
                            End With
                        End If

                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next i

            Case 2
                ' *** Fiat deposit or withdrawal ***
                Dim IsDeposit As Boolean = FileNames(0).ToLower.Contains("deposit")
                Dim BHO As BinanceFiatFundingHistoryRow

                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        BHO = New BinanceFiatFundingHistoryRow(MainImportObject, Row)
                        If BHO IsNot Nothing AndAlso BHO.Status.Contains("success") Then
                            Record = New dtoTradesRecord
                            RecordFee = Nothing
                            With Record
                                .SourceID = MD5FromString(String.Join(";", Row))
                                .Zeitpunkt = BHO.DateTime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = Platform
                                .ZielKontoID = BHO.CoinAccount.ID
                                .QuellKontoID = .ZielKontoID
                                If IsDeposit Then
                                    ' Fiat deposit
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielPlattformID = Platform
                                    .ZielBetrag = BHO.Amount + BHO.Fee
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .ZielBetrag
                                    Else
                                        .WertEUR = 0
                                    End If
                                    .BetragNachGebuehr = BHO.Amount
                                    .QuellBetrag = .ZielBetrag
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .Info = String.Format(My.Resources.MyStrings.importInfoBinanceDepositFiat,
                                                          BHO.Coin, BHO.OrderID)
                                Else
                                    ' Fiat withdrawal
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .QuellPlattformID = Platform
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = BHO.Amount
                                    Else
                                        .WertEUR = 0
                                    End If
                                    .ZielBetrag = BHO.Amount
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellBetrag = .ZielBetrag + BHO.Fee
                                    .QuellBetragNachGebuehr = .ZielBetrag
                                    .Info = String.Format(My.Resources.MyStrings.importInfoGenericPayout,
                                                          BHO.Coin, BHO.OrderID)
                                End If

                                If BHO.Fee > 0 Then
                                    ' Fee transaction
                                    RecordFee = .Clone()
                                    RecordFee.QuellPlattformID = Platform
                                    RecordFee.ZielPlattformID = Platform
                                    RecordFee.SourceID = .SourceID & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.ZielKontoID = BHO.CoinAccount.GebuehrKontoID
                                    RecordFee.ZielBetrag = BHO.Fee
                                    RecordFee.WertEUR = 0
                                    RecordFee.BetragNachGebuehr = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.QuellKontoID = BHO.CoinAccount.ID
                                    RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoGenericFee,
                                                                   BHO.Coin, .SourceID)
                                End If
                                If Not .DoNotImport Then
                                    ' Add record to list
                                    ImportRecords.Add(Record)
                                    If Not RecordFee Is Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If

                            End With
                        End If

                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next i

            Case 3
                ' *** Convert History ***
                Dim BHO As BinanceConvertHistoryRow
                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        BHO = New BinanceConvertHistoryRow(MainImportObject, Row)
                        If BHO IsNot Nothing AndAlso BHO.Status.StartsWith("success") Then
                            Record = New dtoTradesRecord
                            With Record
                                .SourceID = MD5FromString(String.Join(";", Row))
                                .Zeitpunkt = BHO.DateTime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = Platform
                                .TradetypID = DBHelper.TradeTypen.Kauf
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = BHO.BuyAccount.ID
                                .ZielBetrag = BHO.BuyValue
                                .BetragNachGebuehr = .ZielBetrag
                                .QuellKontoID = BHO.SellAccount.ID
                                .QuellBetrag = BHO.SellValue
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .WertEUR = 0
                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericBuy,
                                                      BHO.BuyAccount.Code, BHO.BuyValue, BHO.SellValue, BHO.SellAccount.Code)
                                If Not .DoNotImport Then
                                    ' Add record to list
                                    ImportRecords.Add(Record)
                                End If
                            End With
                        End If
                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next i

            Case 4
                ' *** Trade History as csv from a zip file ***
                Dim BHO As BinanceTradeHistoryCsvRow
                Dim Pair1 As String
                Dim Pair2 As String
                Const DECTOLOG As String = "0.########"

                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        If Row?.Length >= 7 Then
                            Pair1 = String.Empty
                            Pair2 = String.Empty
                            If SplitMarket(Row(1), Pair1, Pair2) Then
                                BHO = New BinanceTradeHistoryCsvRow(MainImportObject, Row, Pair1, Pair2)
                                If BHO IsNot Nothing Then
                                    Record = New dtoTradesRecord
                                    RecordFee = Nothing
                                    With Record
                                        .SourceID = MD5FromString(String.Join(";", {Row(0), Row(1), Row(2),
                                                                              BHO.Price.ToString(DECTOLOG, CultureInfo.InvariantCulture),
                                                                              BHO.Executed.ToString(DECTOLOG, CultureInfo.InvariantCulture),
                                                                              BHO.Amount.ToString(DECTOLOG, CultureInfo.InvariantCulture),
                                                                              BHO.Fee.ToString(DECTOLOG, CultureInfo.InvariantCulture),
                                                                              BHO.FeeAccount.Code.ToUpper}))
                                        .Zeitpunkt = BHO.DateTime
                                        .ZeitpunktZiel = .Zeitpunkt
                                        .ImportPlattformID = Platform
                                        Select Case BHO.Side
                                            Case "BUY"
                                                ' Buy
                                                .TradetypID = DBHelper.TradeTypen.Kauf
                                                .QuellPlattformID = Platform
                                                .ZielPlattformID = .QuellPlattformID
                                                .ZielKontoID = BHO.Pair1Account.ID
                                                .ZielBetrag = BHO.Executed
                                                If BHO.Pair1Account.ID = BHO.FeeAccount.ID Then
                                                    .BetragNachGebuehr = BHO.Executed - BHO.Fee
                                                Else
                                                    .BetragNachGebuehr = BHO.Executed
                                                End If
                                                .QuellBetragNachGebuehr = BHO.Amount
                                                If BHO.Pair2Account.ID = BHO.FeeAccount.ID Then
                                                    .QuellBetrag = BHO.Amount + BHO.Fee
                                                Else
                                                    .QuellBetrag = BHO.Amount
                                                End If
                                                .QuellKontoID = BHO.Pair2Account.ID
                                                If .QuellKontoID = AccountManager.Accounts.EUR Then
                                                    .WertEUR = .QuellBetrag
                                                End If
                                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericBuy,
                                                                      BHO.Pair1Account.Code, BHO.Executed, BHO.Amount, Pair2)
                                            Case "SELL"
                                                ' Sell
                                                .TradetypID = DBHelper.TradeTypen.Verkauf
                                                .QuellPlattformID = Platform
                                                .ZielPlattformID = .QuellPlattformID
                                                .ZielKontoID = BHO.Pair2Account.ID
                                                .ZielBetrag = BHO.Amount
                                                If BHO.Pair2Account.ID = BHO.FeeAccount.ID Then
                                                    .BetragNachGebuehr = BHO.Amount - BHO.Fee
                                                Else
                                                    .BetragNachGebuehr = BHO.Amount
                                                End If
                                                .QuellBetragNachGebuehr = BHO.Executed
                                                If BHO.Pair1Account.ID = BHO.FeeAccount.ID Then
                                                    .QuellBetrag = BHO.Executed + BHO.Fee
                                                Else
                                                    .QuellBetrag = BHO.Executed
                                                End If
                                                .QuellKontoID = BHO.Pair1Account.ID
                                                If .QuellKontoID = DBHelper.Konten.EUR Then
                                                    .WertEUR = .QuellBetrag
                                                End If
                                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericSell,
                                                                      BHO.Pair1Account.Code, BHO.Executed, BHO.Amount, Pair2)
                                            Case Else
                                                ' Do not import
                                                .DoNotImport = True
                                        End Select

                                        If BHO.Fee > 0 Then
                                            ' Fee transaction
                                            RecordFee = .Clone()
                                            RecordFee.QuellPlattformID = Platform
                                            RecordFee.ZielPlattformID = Platform
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.ZielKontoID = BHO.FeeAccount.GebuehrKontoID
                                            RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                            RecordFee.WertEUR = 0
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.QuellKontoID = BHO.FeeAccount.ID
                                            RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoGenericFee,
                                                                           BHO.FeeAccount.Code, .SourceID)
                                        End If
                                        If Not .DoNotImport Then
                                            ' Add record to list
                                            ImportRecords.Add(Record)
                                            If Not RecordFee Is Nothing Then
                                                ImportRecords.Add(RecordFee)
                                            End If
                                        End If

                                    End With
                                End If
                            End If
                        End If

                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try
                Next i
        End Select

        MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage)

        DestroyProgressForm()
        Cursor.Current = Cursors.Default

        Return ErrorCounter = MaxErrors
    End Function

End Class

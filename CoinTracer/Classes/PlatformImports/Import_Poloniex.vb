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

Imports CoinTracer.CoinTracerDataSet
Imports CoinTracer.My.Resources

Public Class Import_Poloniex
    Inherits FileImportBase
    Implements IFileImport

    Private Const PLATFORMID = PlatformManager.Platforms.Poloniex
    Private Const PLATFORMFULLNAME As String = "Poloniex.com"


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
                {.PlatformID = PLATFORMID,                  ' Poloniex #1 (trade history)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date,Market,Category,Type,Price,Amount,Total,Fee,Order Number,Base Total Less Fee,Quote Total Less Fee,Fee Currency,Fee Total",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Poloniex #2 (deposit history)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date,Currency,Amount,Address,Status",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                 New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Poloniex #3 (withdrawal history)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Date,Currency,Amount,Fee Deducted,Amount - Fee,Address,Status",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 2}
                }
            Return Result
        End Get
    End Property


    ''' <summary>
    ''' Represents a single row of a Poloniex deposit history file 
    ''' </summary>
    Private Class DepositLineObject

        Private _ThisImport As Import

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _CurrencyAccount As KontenRow
        Public ReadOnly Property CurrencyAccount() As KontenRow
            Get
                Return _CurrencyAccount
            End Get
        End Property

        Private _Address As String
        Public ReadOnly Property Address() As String
            Get
                Return _Address
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

        Public Sub New(ByRef ThisImport As Import, ByRef ImportRow() As String)
            _ThisImport = ThisImport
            _Date = CDate(ImportRow(0)).ToLocalTime
            _CurrencyAccount = _ThisImport.RetrieveAccount(ImportRow(1).Trim)
            _Amount = Math.Abs(StrToDec(ImportRow(2)))
            _Address = ImportRow(3).Trim
            _Status = ImportRow(4).Trim
        End Sub

    End Class


    ''' <summary>
    ''' Represents a single row of a Poloniex deposit history file 
    ''' </summary>
    Private Class WithdrawalLineObject

        Private _ThisImport As Import

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _CurrencyAccount As KontenRow
        Public ReadOnly Property CurrencyAccount() As KontenRow
            Get
                Return _CurrencyAccount
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _FeeDeducted As Decimal
        Public ReadOnly Property FeeDeducted() As Decimal
            Get
                Return _FeeDeducted
            End Get
        End Property

        Private _AmountLessFee As Decimal
        Public ReadOnly Property AmountLessFee() As Decimal
            Get
                Return _AmountLessFee
            End Get
        End Property

        Private _Address As String
        Public ReadOnly Property Address() As String
            Get
                Return _Address
            End Get
        End Property

        Private _Status As String
        Public ReadOnly Property Status() As String
            Get
                Return _Status
            End Get
        End Property

        Public Sub New(ByRef ThisImport As Import, ByRef ImportRow() As String)
            _ThisImport = ThisImport
            _Date = CDate(ImportRow(0)).ToLocalTime
            _CurrencyAccount = _ThisImport.RetrieveAccount(ImportRow(1).Trim)
            _Amount = Math.Abs(StrToDec(ImportRow(2)))
            _FeeDeducted = Math.Abs(StrToDec(ImportRow(3)))
            _AmountLessFee = Math.Abs(StrToDec(ImportRow(4)))
            _Address = ImportRow(5).Trim
            _Status = ImportRow(6).Trim
        End Sub

    End Class


    ''' <summary>
    ''' Represents a single row of a Poloniex trade history file 
    ''' </summary>
    Private Class TradeLineObject

        Private _ThisImport As Import

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

        Private _Category As String
        Public ReadOnly Property Category() As String
            Get
                Return _Category
            End Get
        End Property

        Private _Type As String
        ''' <summary>
        ''' Trade type, always lower case!
        ''' </summary>
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

        Private _FeeString As String
        Public ReadOnly Property FeeString() As String
            Get
                Return _FeeString
            End Get
        End Property

        Private _OrderNumber As String
        Public ReadOnly Property OrderNumber() As String
            Get
                Return _OrderNumber
            End Get
        End Property

        Private _BaseTotalLessFee As Decimal
        Public ReadOnly Property BaseTotalLessFee() As Decimal
            Get
                Return _BaseTotalLessFee
            End Get
        End Property

        Private _QuoteTotalLessFee As Decimal
        Public ReadOnly Property QuoteTotalLessFee() As Decimal
            Get
                Return _QuoteTotalLessFee
            End Get
        End Property

        Private _FeeCurrencyAccount As KontenRow
        Public ReadOnly Property FeeCurrencyAccount() As KontenRow
            Get
                Return _FeeCurrencyAccount
            End Get
        End Property

        Private _FeeTotal As Decimal
        Public ReadOnly Property FeeTotal() As Decimal
            Get
                Return _FeeTotal
            End Get
        End Property

        Private _Account1st As KontenRow
        Public ReadOnly Property Account1st() As KontenRow
            Get
                Return _Account1st
            End Get
        End Property

        Private _Account2nd As KontenRow
        Public ReadOnly Property Account2nd() As KontenRow
            Get
                Return _Account2nd
            End Get
        End Property

        Public Sub New(ByRef ThisImport As Import, ByRef ImportRow() As String)
            _ThisImport = ThisImport
            _Date = CDate(ImportRow(0)).ToLocalTime
            _Market = ImportRow(1).Trim
            Dim Markets() As String = ImportRow(1).Split("/"c)
            If Markets.Length = 2 Then
                _Account1st = _ThisImport.RetrieveAccount(Markets(0))
                _Account2nd = _ThisImport.RetrieveAccount(Markets(1))
            End If
            _Category = ImportRow(2).Trim
            _Type = ImportRow(3).ToLower.Trim
            _Price = Math.Abs(StrToDec(ImportRow(4)))
            _Amount = Math.Abs(StrToDec(ImportRow(5)))
            _Total = Math.Abs(StrToDec(ImportRow(6)))
            _FeeString = ImportRow(7).Trim
            _OrderNumber = ImportRow(8).Trim
            _BaseTotalLessFee = Math.Abs(StrToDec(ImportRow(9)))
            _QuoteTotalLessFee = Math.Abs(StrToDec(ImportRow(10)))
            _FeeCurrencyAccount = _ThisImport.RetrieveAccount(ImportRow(11))
            _FeeTotal = Math.Abs(StrToDec(ImportRow(12)))
        End Sub

    End Class

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
        CSVSeparator = ","
        CSVDecimalPoint = "."
        CSVDecimalSeparator = ""
        CSVTextqualifier = ""
        CheckFirstLine = True
    End Sub

    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportPoloniexCom", DialogResult.OK,
                                        String.Format(My.Resources.MyStrings.importMsgPoloniexCSV, Environment.NewLine),
                                        My.Resources.MyStrings.importMsgPoloniexCSVTitle,
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Perform the actual import for Poloniex files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean Implements IFileImport.ImportContent
        Dim Row() As String
        Dim ErrorCounter As Long = MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim AllLines As Long = AllRows.Count
        Dim Infotext As String
        Const DATETIMELOGFORMAT As String = "yyyy-MM-dd HH:mm:ss"

        Cursor.Current = Cursors.WaitCursor
        InitProgressForm(String.Format(MyStrings.importMsgImportStarting, PlatformName))
        ImportRecords = New List(Of dtoTradesRecord)

        Select Case SubType
            Case 0
                ' *** Import trades ***
                Dim TLO As TradeLineObject
                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        If Row?.Length >= 13 Then
                            TLO = New TradeLineObject(MainImportObject, Row)
                            If TLO IsNot Nothing Then
                                Record = New dtoTradesRecord
                                RecordFee = Nothing
                                With Record
                                    .SourceID = TLO.OrderNumber & "-" & MD5FromString(TLO.DateTime.ToString(DATETIMELOGFORMAT) &
                                                                                      TLO.Market & TLO.Price & TLO.Amount & TLO.FeeTotal)
                                    .Zeitpunkt = TLO.DateTime
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = Platform
                                    .QuellPlattformID = .ImportPlattformID
                                    .ZielPlattformID = .ImportPlattformID
                                    If TLO.Account2nd.IstFiat AndAlso TLO.Type = "sell" Then
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                        Infotext = MyStrings.importInfoPoloniexSell
                                        .ZielKontoID = TLO.Account2nd.ID
                                        .QuellKontoID = TLO.Account1st.ID
                                        .ZielBetrag = TLO.Total
                                        .BetragNachGebuehr = TLO.BaseTotalLessFee
                                        .QuellBetrag = TLO.Amount
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                    Else
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                        If TLO.Type = "sell" Then
                                            ' Swap of coins vs. coins is always regarded as a buy!
                                            Infotext = MyStrings.importInfoPoloniexSellCoins2Coins
                                            .QuellKontoID = TLO.Account1st.ID
                                            .ZielKontoID = TLO.Account2nd.ID
                                            .ZielBetrag = TLO.Total
                                            .BetragNachGebuehr = TLO.BaseTotalLessFee
                                            .QuellBetrag = TLO.Amount
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                        Else
                                            ' Coins bought with fiat
                                            Infotext = MyStrings.importInfoPoloniexBuy
                                            .ZielKontoID = TLO.Account1st.ID
                                            .QuellKontoID = TLO.Account2nd.ID
                                            .ZielBetrag = TLO.Amount
                                            .BetragNachGebuehr = TLO.QuoteTotalLessFee
                                            .QuellBetrag = TLO.Total
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                        End If

                                    End If
                                    .Info = String.Format(Infotext,
                                                          TLO.Amount.ToString(Import.INFONUMBERFORMAT), TLO.Account1st.Code,
                                                          TLO.Price.ToString(Import.INFONUMBERFORMAT), TLO.Account2nd.Code,
                                                          TLO.FeeString, TLO.Total.ToString(Import.INFONUMBERFORMAT))
                                    If TLO.OrderNumber.Length > 0 Then
                                        .Info &= String.Format(MyStrings.importInfoPoloniexOrderAppendix, TLO.OrderNumber)
                                    End If
                                    ' Set taxable amount
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If
                                    ' Add record to list
                                    ImportRecords.Add(Record)
                                    ' Add fee entry, if needed
                                    If TLO.FeeTotal > 0 Then
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = TLO.FeeCurrencyAccount.ID
                                        RecordFee.ZielBetrag = TLO.FeeTotal
                                        RecordFee.ZielKontoID = TLO.FeeCurrencyAccount.GebuehrKontoID
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = "Gebühr zu Trade Referenz " & .SourceID
                                        ImportRecords.Add(RecordFee)
                                    End If

                                End With
                            End If

                        End If
                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            DestroyProgressForm()
                            Return False
                            Exit Function
                        End If
                    End Try
                Next

            Case 1
                ' *** Deposits history ***
                Dim DLO As DepositLineObject
                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        If Row?.Length >= 5 AndAlso Row(4).ToUpper.StartsWith("COMPLETE") Then
                            DLO = New DepositLineObject(MainImportObject, Row)
                            If DLO IsNot Nothing Then
                                Record = New dtoTradesRecord
                                RecordFee = Nothing
                                With Record
                                    .SourceID = MD5FromString(DLO.DateTime.ToString(DATETIMELOGFORMAT) &
                                                              DLO.CurrencyAccount.Code & DLO.Amount & DLO.Address)
                                    .Zeitpunkt = DLO.DateTime
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = Platform
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielPlattformID = Platform
                                    .Info = String.Format(MyStrings.importInfoPoloniexDeposit, DLO.Address)
                                    .QuellBetrag = DLO.Amount
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .ZielBetrag = .QuellBetrag
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellKontoID = DLO.CurrencyAccount.ID
                                    .ZielKontoID = .QuellKontoID
                                    ' Set taxable amount
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                End With
                            End If

                        End If
                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            DestroyProgressForm()
                            Return False
                            Exit Function
                        End If
                    End Try
                Next

            Case 2
                ' *** Withdrawals history ***
                Dim WLO As WithdrawalLineObject
                For i As Long = 0 To AllLines - 1
                    Try
                        UpdateProgress(AllLines, i + 1)
                        Row = CSV.Rows(i)
                        If Row?.Length >= 7 AndAlso Row(6).ToUpper.StartsWith("COMPLETE") Then
                            WLO = New WithdrawalLineObject(MainImportObject, Row)
                            If WLO IsNot Nothing Then
                                Record = New dtoTradesRecord
                                RecordFee = Nothing
                                With Record
                                    .SourceID = MD5FromString(WLO.DateTime.ToString(DATETIMELOGFORMAT) &
                                                              WLO.CurrencyAccount.Code & -WLO.Amount & WLO.Address)
                                    .Zeitpunkt = WLO.DateTime
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = Platform
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .QuellPlattformID = Platform
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .Info = String.Format(MyStrings.importInfoPoloniexWithdrawal, WLO.Address)
                                    .QuellBetrag = WLO.Amount
                                    .QuellBetragNachGebuehr = WLO.AmountLessFee
                                    .ZielBetrag = WLO.AmountLessFee
                                    .BetragNachGebuehr = .ZielBetrag
                                    .QuellKontoID = WLO.CurrencyAccount.ID
                                    .ZielKontoID = .QuellKontoID
                                    ' Set taxable amount
                                    If .QuellKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .QuellBetrag
                                    ElseIf .ZielKontoID = AccountManager.Accounts.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If
                                    ' Add fee entry, if needed
                                    If WLO.FeeDeducted > 0 Then
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = WLO.CurrencyAccount.ID
                                        RecordFee.ZielBetrag = WLO.FeeDeducted
                                        RecordFee.ZielKontoID = WLO.CurrencyAccount.GebuehrKontoID
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = "Gebühr zu Trade Referenz " & .SourceID
                                        ImportRecords.Add(RecordFee)
                                    End If
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                End With
                            End If

                        End If
                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            DestroyProgressForm()
                            Return False
                            Exit Function
                        End If
                    End Try
                Next

        End Select

        MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage)

        DestroyProgressForm()
        Cursor.Current = Cursors.Default

        Return ErrorCounter = MaxErrors
    End Function

End Class

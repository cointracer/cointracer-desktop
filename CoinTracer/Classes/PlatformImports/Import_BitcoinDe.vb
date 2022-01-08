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

Public Class Import_BitcoinDe
    Inherits FileImportBase
    Implements IFileImport

    Private Const PLATFORMID = PlatformManager.Platforms.BitcoinDe
    Private Const PLATFORMFULLNAME As String = "Bitcoin.de"


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
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #1
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Referenz;""Kurs (€/BTC)"";""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #2
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Referenz;""Kurs (�/BTC)"";""BTC vor Geb�hr"";""EUR vor Geb�hr"";""BTC nach Geb�hr"";""EUR nach Geb�hr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 0},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #3 (since 2017-07)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #4 (since 2017-07)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Geb�hr"";""EUR vor Geb�hr"";""BTC nach Geb�hr"";""EUR nach Geb�hr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #5 (BCH, since 2017-08)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 2},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #6 (ETH, since 2017-10)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 3},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #7 (BTC, since 2018-02, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #8 (BCH, since 2018-02, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 2},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #9 (ETH, since 2018-02, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 3},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #10 (BTG, since 2018-02, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTG vor Gebühr"";""EUR vor Gebühr"";""BTG nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 4},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #11 (BTC, since 2018-02, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 1 + 128},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #12 (BCH, since 2018-02, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 2 + 128},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #13 (ETH, since 2018-02, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 3 + 128},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #14 (BTG, since 2018-02, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTG vor Gebühr"";""EUR vor Gebühr"";""BTG nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.StartsWithMatch,
                 .SubType = 4 + 128},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #15 (Any coin, since 2019-01, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währung;Referenz;???-Adresse;Kurs;""Einheit (Kurs)"";""??? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""??? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.LikeMatch,
                 .SubType = 5},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #16 (Any coin, since 2019-01, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währung;Referenz;???-Adresse;Kurs;""Einheit (Kurs)"";""??? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""??? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.LikeMatch,
                 .SubType = 5 + 128},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #17 (Any 4-letter-coin, since 2019-01, no Fidor.de fees)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währung;Referenz;????-Adresse;Kurs;""Einheit (Kurs)"";""???? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""???? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.LikeMatch,
                 .SubType = 5},
                New ImportFileHelper.MatchingPlatform With
                {.PlatformID = PLATFORMID,                  ' Bitcoin.de #18 (Any 4-letter-coin, since 2019-01, Fidor.de fees included)
                 .PlatformName = PLATFORMFULLNAME,
                 .FilesFirstLine = "Datum;Typ;Währung;Referenz;????-Adresse;Kurs;""Einheit (Kurs)"";""???? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""???? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand",
                 .MatchingType = ImportFileHelper.ImportFileMatchingTypes.LikeMatch,
                 .SubType = 5 + 128}
                }
            Return Result
        End Get
    End Property


    Private Class BitcoinDeLineObject

        Private _Import As Import

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _Type As String
        Public ReadOnly Property Type() As String
            Get
                Return _Type
            End Get
        End Property

        Private _Currencies As String
        Public ReadOnly Property Currencies() As String
            Get
                Return _Currencies
            End Get
        End Property

        Private _Currency1Account As KontenRow
        Public ReadOnly Property Currency1Account() As KontenRow
            Get
                Return _Currency1Account
            End Get
        End Property

        Private _Currency2Account As KontenRow
        Public ReadOnly Property Currency2Account() As KontenRow
            Get
                Return _Currency2Account
            End Get
        End Property

        Private _Reference As String
        Public ReadOnly Property Reference() As String
            Get
                Return _Reference
            End Get
        End Property


        Private _Course As Decimal
        Public ReadOnly Property Course() As Decimal
            Get
                Return _Course
            End Get
        End Property

        Private _Cur1BeforeFee As Decimal
        Public Property Currency1BeforeFee() As Decimal
            Get
                Return _Cur1BeforeFee
            End Get
            Set(ByVal value As Decimal)
                _Cur1BeforeFee = value
            End Set
        End Property

        Private _Cur2BeforeFee As Decimal
        Public Property Currency2BeforeFee() As Decimal
            Get
                Return _Cur2BeforeFee
            End Get
            Set(ByVal value As Decimal)
                _Cur2BeforeFee = value
            End Set
        End Property

        Private _Cur1AfterFee As Decimal
        Public Property Currency1AfterFee() As Decimal
            Get
                Return _Cur1AfterFee
            End Get
            Set(ByVal value As Decimal)
                _Cur1AfterFee = value
            End Set
        End Property

        Private _Cur2AfterFee As Decimal
        Public Property Currency2AfterFee() As Decimal
            Get
                Return _Cur2AfterFee
            End Get
            Set(ByVal value As Decimal)
                _Cur2AfterFee = value
            End Set
        End Property

        Private _Flow As Decimal
        Public Property Flow() As Decimal
            Get
                Return _Flow
            End Get
            Set(ByVal value As Decimal)
                _Flow = value
            End Set
        End Property

        Private _Balance As Decimal
        Public Property Balance() As Decimal
            Get
                Return _Balance
            End Get
            Set(ByVal value As Decimal)
                _Balance = value
            End Set
        End Property

        Public Sub New(ByRef Import As Import)
            _Import = Import
            _Currency1Account = Nothing
            _Currency2Account = Nothing
        End Sub

        Public Sub New(ByRef Import As Import,
                       ByRef Culture As CultureInfo,
                       ByRef DateTime As String,
                       ByRef Type As String,
                       ByRef Currencies As String,
                       ByRef Reference As String,
                       ByRef Course As String,
                       ByRef Currency1BeforeFee As String,
                       ByRef Currency2BeforeFee As String,
                       ByRef Currency1AfterFee As String,
                       ByRef Currency2AfterFee As String,
                       ByRef Flow As String,
                       ByRef Balance As String)
            Me.New(Import)
            _Date = CType(DateTime, Date)
            _Type = Type.Trim
            If Currencies IsNot Nothing AndAlso Currencies.Contains("/") Then
                Dim CurrencyParts() As String = Currencies.Split("/")
                _Currency1Account = _Import.RetrieveAccount(CurrencyParts(0))
                If CurrencyParts.Length > 1 Then
                    _Currency2Account = _Import.RetrieveAccount(CurrencyParts(1))
                End If
            Else
                _Currency1Account = _Import.RetrieveAccount("BTC")
                _Currency2Account = _Import.RetrieveAccount("EUR")
            End If
            If Reference IsNot Nothing Then
                _Reference = Reference.Trim
            End If
            If Course IsNot Nothing AndAlso Course.Length > 0 Then
                _Course = Decimal.Parse(Course, Culture)
            Else
                _Course = 0
            End If
            If Currency1BeforeFee IsNot Nothing AndAlso Currency1BeforeFee.Length > 0 Then
                _Cur1BeforeFee = Decimal.Parse(Currency1BeforeFee, Culture)
            Else
                _Cur1BeforeFee = 0
            End If
            If Currency2BeforeFee IsNot Nothing AndAlso Currency2BeforeFee.Length > 0 Then
                _Cur2BeforeFee = Decimal.Parse(Currency2BeforeFee, Culture)
            Else
                _Cur2BeforeFee = 0
            End If
            If Currency1AfterFee IsNot Nothing AndAlso Currency1AfterFee.Length > 0 Then
                _Cur1AfterFee = Decimal.Parse(Currency1AfterFee, Culture)
            Else
                _Cur1AfterFee = 0
            End If
            If Currency2AfterFee IsNot Nothing AndAlso Currency2AfterFee.Length > 0 Then
                _Cur2AfterFee = Decimal.Parse(Currency2AfterFee, Culture)
            Else
                _Cur2AfterFee = 0
            End If
            If Flow IsNot Nothing AndAlso Flow.Length > 0 Then
                _Flow = Decimal.Parse(Flow, Culture)
            Else
                _Flow = 0
            End If
            If Balance IsNot Nothing AndAlso Balance.Length > 0 Then
                _Balance = Decimal.Parse(Balance, Culture)
            Else
                _Balance = 0
            End If
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
        MultiSelectFiles = False
        CheckFirstLine = True
        FileDialogTitle = My.Resources.MyStrings.importOpenFileTitleBitcoinDe
    End Sub

    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportBitcoinDe", DialogResult.OK,
                                        String.Format(My.Resources.MyStrings.importMsgBitcoinDeCSV, Environment.NewLine),
                                        My.Resources.MyStrings.importMsgBitcoinDeCSVTitle,
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Perform the actual import for Bitcoin.de files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean Implements IFileImport.ImportContent
        Dim Row() As String
        Dim Items() As String
        Dim NextRow() As String
        Dim ErrorCounter As Long = MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim AllLines As Long
        Dim FeeAmount As Decimal = 0
        Dim Currency As String = "BTC"
        Dim HasFidorFees As Boolean = False

        Cursor.Current = Cursors.WaitCursor
        If SubType > 128 Then
            HasFidorFees = True
            SubType -= 128
        End If
        Row = CSV.FirstLine.Split(";"c)
        Items = Row(4 + IIf(SubType = 0, 0, IIf(SubType = 5, 3, 1))).Split(" "c)
        Currency = Items(0).Substring(1)
        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))
        ' Daten in Liste einlesen
        ImportRecords = New List(Of dtoTradesRecord)
        Dim TLO As BitcoinDeLineObject
        Dim FeeTLO As BitcoinDeLineObject
        AllLines = CSV.Rows.Count
        For i As Long = 0 To AllLines - 1
            UpdateProgress(AllLines, i + 1)
            ' ProgressWaitManager.UpdateProgress(i + 1 / AllLines * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgReadingFile, i + 1, AllLines))
            Row = CSV.Rows(i)
            TLO = GetTLO(Row, Currency, HasFidorFees)
            If TLO IsNot Nothing Then
                Record = New dtoTradesRecord
                RecordFee = Nothing
                With Record
                    Try
                        .SourceID = TLO.Reference
                        .Zeitpunkt = TLO.DateTime
                        .ZeitpunktZiel = .Zeitpunkt
                        .ImportPlattformID = Platform
                        Select Case TLO.Type
                            Case "Kauf", "buy"
                                ' Buy
                                .TradetypID = DBHelper.TradeTypen.Kauf
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = TLO.Currency1Account.ID
                                .ZielBetrag = TLO.Currency1AfterFee + (TLO.Currency1BeforeFee - TLO.Currency1AfterFee) / 2  ' retargeting the fees for coins: data says it's 1%, but in reality buyer only has to carry 0.5% (by paying 0.5% less fiat)
                                .BetragNachGebuehr = TLO.Currency1AfterFee
                                .QuellBetrag = TLO.Currency2AfterFee
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .QuellKontoID = TLO.Currency2Account.ID
                                If .QuellKontoID = DBHelper.Konten.EUR Then
                                    .WertEUR = .QuellBetrag
                                End If
                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericBuy,
                                                            TLO.Currency1Account.Code, TLO.Currency1BeforeFee, TLO.Currency2BeforeFee, TLO.Currency2Account.Code)
                                ' Fee transaction
                                RecordFee = .Clone()
                                RecordFee.SourceID = .SourceID & "/fee"
                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                RecordFee.ZielKontoID = TLO.Currency1Account.GebuehrKontoID
                                RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                RecordFee.WertEUR = 0
                                RecordFee.BetragNachGebuehr = 0
                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                RecordFee.QuellKontoID = TLO.Currency1Account.ID
                                RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeBuyFee,
                                                                   TLO.Currency1Account.Code, .SourceID)
                                FeeAmount = 0
                            Case "Verkauf", "sell"
                                ' Sell
                                .TradetypID = DBHelper.TradeTypen.Verkauf
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = TLO.Currency2Account.ID
                                .ZielBetrag = TLO.Currency2BeforeFee
                                .BetragNachGebuehr = TLO.Currency2AfterFee
                                .QuellBetrag = TLO.Currency1BeforeFee
                                .QuellBetragNachGebuehr = .QuellBetrag
                                If .ZielKontoID = DBHelper.Konten.EUR Then
                                    .WertEUR = .BetragNachGebuehr
                                End If
                                .QuellKontoID = TLO.Currency1Account.ID
                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericSell,
                                                          TLO.Currency1Account.Code, TLO.Currency1BeforeFee, TLO.Currency2BeforeFee, TLO.Currency2Account.Code)
                                ' Fee transaction
                                RecordFee = .Clone()
                                RecordFee.SourceID = .SourceID & "/fee"
                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                RecordFee.ZielKontoID = TLO.Currency2Account.GebuehrKontoID
                                RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                RecordFee.WertEUR = 0
                                RecordFee.BetragNachGebuehr = 0
                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                RecordFee.QuellKontoID = TLO.Currency2Account.ID
                                RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeSellFee,
                                                                   TLO.Currency1Account.Code, .SourceID)
                                FeeAmount = 0
                            Case "Auszahlung", "payout"
                                ' Payout
                                .TradetypID = DBHelper.TradeTypen.Auszahlung
                                .QuellPlattformID = Platform
                                .QuellBetragNachGebuehr = -TLO.Flow
                                .QuellBetrag = .QuellBetragNachGebuehr + FeeAmount
                                .QuellKontoID = TLO.Currency1Account.ID
                                .BetragNachGebuehr = .QuellBetragNachGebuehr       ' Bei Auszahlungen steht der Betrag, der am Ziel ankommt, in BetragNachGebuehr!
                                .WertEUR = 0
                                .ZielPlattformID = PlatformManager.Platforms.Unknown
                                .ZielKontoID = .QuellKontoID
                                .ZielBetrag = .QuellBetragNachGebuehr
                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericPayout,
                                                          TLO.Currency1Account.Code, TLO.Reference)
                                ' check if next line has network fee
                                If i < AllLines - 1 Then
                                    NextRow = CSV.Rows(i + 1)
                                    FeeTLO = GetTLO(NextRow, Currency, HasFidorFees)
                                    If FeeTLO IsNot Nothing AndAlso FeeTLO.Reference.ToLower = "netzwerk-nebühr" AndAlso TLO.DateTime = FeeTLO.DateTime Then
                                        .QuellBetrag = .QuellBetrag - FeeTLO.Flow   ' CSV amount is always negative, so substract it!
                                    End If
                                End If
                                FeeAmount = 0
                            Case "Netzwerk-Gebühr", "outgoing_fee_voluntary"
                                ' network fee for payout
                                .SourceID = .SourceID & "/fee"
                                .TradetypID = DBHelper.TradeTypen.Gebühr
                                .QuellPlattformID = Platform
                                .QuellBetrag = -TLO.Flow
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .QuellKontoID = TLO.Currency1Account.ID
                                .BetragNachGebuehr = 0
                                .WertEUR = 0
                                .ZielPlattformID = Platform
                                .ZielKontoID = TLO.Currency1Account.GebuehrKontoID
                                .ZielBetrag = .QuellBetrag
                                .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDePayoutNetworkFee,
                                                          TLO.Currency1Account.Code, TLO.Reference)
                                ' store network fee for next line (in case it is a payout)
                                FeeAmount = .QuellBetrag
                            Case "Einzahlung", "inpayment"
                                ' Inpayment
                                .TradetypID = DBHelper.TradeTypen.Einzahlung
                                .QuellPlattformID = PlatformManager.Platforms.Unknown
                                .ZielPlattformID = Platform
                                .ZielKontoID = TLO.Currency1Account.ID
                                .ZielBetrag = TLO.Flow
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .QuellBetrag = .ZielBetrag
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .QuellKontoID = .ZielKontoID
                                .Info = String.Format(My.Resources.MyStrings.importInfoGenericInpayment,
                                                          TLO.Currency1Account.Code, TLO.Reference)
                                FeeAmount = 0
                            Case "kickback"
                                ' Kickback for using the API
                                .TradetypID = DBHelper.TradeTypen.Kauf
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = TLO.Currency1Account.ID
                                .ZielBetrag = TLO.Flow
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .QuellBetrag = 0
                                .QuellBetragNachGebuehr = 0
                                .QuellKontoID = DBHelper.Konten.EUR
                                .Info = TLO.Reference
                                FeeAmount = 0
                            Case "Registrierung", "Welcome Btc", "Initialisierung"
                                ' Welcome BTC for registration or coin airdrop
                                If TLO.Flow > 0 Then
                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                    .QuellPlattformID = Platform
                                    .ZielPlattformID = Platform
                                    .ZielKontoID = TLO.Currency1Account.ID
                                    .ZielBetrag = TLO.Flow
                                    .BetragNachGebuehr = .ZielBetrag
                                    .WertEUR = 0
                                    .QuellBetrag = 0
                                    .QuellBetragNachGebuehr = 0
                                    .QuellKontoID = DBHelper.Konten.EUR
                                    If TLO.Type.Contains("Initialisierung") Then
                                        .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeInitialization, TLO.Currency1Account.Code)
                                    Else
                                        .Info = My.Resources.MyStrings.importInfoBitcoinDeRegistration
                                    End If
                                    .SourceID = MD5FromString(.Info & .Zeitpunkt.ToString("dd-MM-yy HH:mm:ss"))
                                Else
                                    .DoNotImport = True
                                End If
                                FeeAmount = 0
                            Case "Partnerprogramm"
                                ' Affiliate earnings
                                .TradetypID = DBHelper.TradeTypen.Kauf
                                .QuellPlattformID = Platform
                                .ZielPlattformID = .QuellPlattformID
                                .ZielKontoID = TLO.Currency1Account.ID
                                .ZielBetrag = TLO.Flow
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .QuellBetrag = 0
                                .QuellBetragNachGebuehr = 0
                                .QuellKontoID = DBHelper.Konten.EUR
                                .Info = String.Format(My.Resources.MyStrings.importInfoBitcoinDeAffiliateEarnings, TLO.Reference)
                                .SourceID = MD5FromString(.Info & .Zeitpunkt.ToString("dd-MM-yy HH:mm:ss"))
                                FeeAmount = 0
                            Case Else
                                ' Do not import
                                .DoNotImport = True
                                FeeAmount = 0
                        End Select

                        If Not .DoNotImport Then
                            ' Add record to list
                            ImportRecords.Add(Record)
                            If Not RecordFee Is Nothing Then
                                ImportRecords.Add(RecordFee)
                            End If
                        End If

                    Catch ex As Exception
                        If FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                            Return False
                            Exit Function
                        End If
                    End Try

                End With
            End If

        Next i


        MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage)

        DestroyProgressForm()
        Cursor.Current = Cursors.Default

        Return ErrorCounter = MaxErrors
    End Function

    ''' <summary>
    ''' Internal function: converts a row array into a line object
    ''' </summary>
    Private Function GetTLO(ByVal Row As String(), ByRef Currency As String, ByVal HasFidorFees As Boolean) As BitcoinDeLineObject
        If Row.Length >= 8 Then
            Select Case SubType
                Case 0
                    Return New BitcoinDeLineObject(MainImportObject, CultureInfo.InvariantCulture,
                                                    Row(0),
                                                    Row(1),
                                                    "BTC / EUR",
                                                    Row(2),
                                                    Row(3),
                                                    Row(4),
                                                    Row(5),
                                                    Row(6),
                                                    Row(7),
                                                    Row(8),
                                                    Row(9))
                Case 5
                    If HasFidorFees Then
                        Return New BitcoinDeLineObject(MainImportObject, CultureInfo.InvariantCulture,
                                                    Row(0),
                                                    Row(1),
                                                    Currency & " / " & IIf(Row(9) = "", "EUR", Row(9)),
                                                    Row(3),
                                                    Row(5),
                                                    Row(7),
                                                    Row(8),
                                                    Row(10),
                                                    Row(13),
                                                    Row(14),
                                                    Row(15))
                    Else
                        Return New BitcoinDeLineObject(MainImportObject, CultureInfo.InvariantCulture,
                                                    Row(0),
                                                    Row(1),
                                                    Currency & " / " & IIf(Row(9) = "", "EUR", Row(9)),
                                                    Row(3),
                                                    Row(5),
                                                    Row(7),
                                                    Row(8),
                                                    Row(10),
                                                    Row(11),
                                                    Row(13),
                                                    Row(14))
                    End If
                Case Else
                    If HasFidorFees Then
                        Return New BitcoinDeLineObject(MainImportObject, CultureInfo.InvariantCulture,
                                                    Row(0),
                                                    Row(1),
                                                    Currency & " / EUR",
                                                    Row(3),
                                                    Row(4),
                                                    Row(5),
                                                    Row(6),
                                                    Row(7),
                                                    Row(9),
                                                    Row(10),
                                                    Row(11))
                    Else
                        Return New BitcoinDeLineObject(MainImportObject, CultureInfo.InvariantCulture,
                                                    Row(0),
                                                    Row(1),
                                                    Currency & " / EUR",
                                                    Row(3),
                                                    Row(4),
                                                    Row(5),
                                                    Row(6),
                                                    Row(7),
                                                    Row(8),
                                                    Row(9),
                                                    Row(10))
                    End If
            End Select
        End If
        Return Nothing
    End Function

End Class

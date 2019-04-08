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

Imports CoinTracer.CoinTracerDataSet

Public Class Import_Kraken
    Inherits FileImportBase
    Implements IFileImport

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
            _Asset = Asset.Trim
            _Asset = _Asset.Substring(_Asset.Length - 3)
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

        End Sub

    End Class

    Friend Const KRAKEN_ZEROVALUETRADELIMIT As Decimal = 0.00002   ' Trades below this value do not need a second line in the trade data (assuming the value of this second line would be zero)

    ''' <summary>
    ''' Initializes this import
    ''' </summary>
    ''' <param name="MainImportObject">Reference to the calling import object</param>
    Public Sub New(MainImportObject As Import)
        MyBase.New(MainImportObject)

        Platform = PlatformManager.Platforms.Kraken
        CSVAutoDetectEncoding = False
        CSVEncoding = Text.Encoding.UTF8
        CSVDecimalPoint = "."c
        CSVDecimalSeparator = ""
        CSVSeparator = ","c
        CSVTextqualifier = """"c
        MultiSelectFiles = False
        FileDialogTitle = My.Resources.MyStrings.importOpenFileFilterTitleKraken
    End Sub

    ''' <summary>
    ''' Show an import hint and present the OFD to the user.
    ''' Set FileName and read all file content into AllRows array.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length > 0 OrElse MsgBoxEx.ShowWithNotAgainOption("ImportKrakenCSV", DialogResult.OK,
                                                        String.Format(My.Resources.MyStrings.importMsgKrakenCSV, Environment.NewLine),
                                                        My.Resources.MyStrings.importMsgKrakenCSVCaption,
                                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Return MyBase.OpenFile()
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Perform the actual import for Kraken.com files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean

        Dim LineCount As Long
        Dim l As Long
        Dim AllLines As Long
        Dim ErrCounter As Long = MaxErrors
        Dim Row() As String, NextRow() As String
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim KontoRow As KontenRow

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, Platform) = 0 Then
                ' File has the wrong format!
                ImportFileHelper.InvalidFileMessage(FileNames(0))
                Return False
                Exit Function
            End If
            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))
            ' Process all rows
            If AllRows.Count > 0 Then
                ImportRecords = New List(Of dtoTradesRecord)
                LineCount = 1
                AllLines = CSV.Rows.Count
                Dim TLO As KrakenLineObject
                Dim NextTLO As KrakenLineObject
                Dim SourceTLO As KrakenLineObject
                Dim TargetTLO As KrakenLineObject
                For l = 0 To AllLines - 1
                    Row = CSV.Rows(l)
                    UpdateProgress(AllLines, LineCount)
                    ' ProgressWaitManager.UpdateProgress(LineCount / AllLines * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgReadingFile, l, AllLines))
                    LineCount += 1
                    If Row.Length = 9 Then
                        Try
                            TLO = New KrakenLineObject(MainImportObject, Row(2), Row(0), Row(1), Row(3), Row(5), Row(6), Row(7))
                            Record = New dtoTradesRecord
                            RecordFee = Nothing
                            With Record
                                .SourceID = TLO.TxId
                                .Zeitpunkt = TLO.DateTime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = Platform
                                .Info = FirstCharToUppercase(TLO.Type) & " " & TLO.Asset
                                KontoRow = MainImportObject.RetrieveAccount(TLO.Asset)
                                Select Case TLO.Type.Substring(0, Math.Min(8, TLO.Type.Length)).ToLower
                                    Case "deposit", "transfer"
                                        If TLO.TxId = TLO.RefId Then
                                            ' Ignore all lines that originally had an empty txid, because each deposit appears twice in the csv file
                                            .DoNotImport = True
                                        Else
                                            .TradetypID = DBHelper.TradeTypen.Einzahlung
                                            If TLO.Asset = "FEE" Then
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
                                        End If
                                    Case "withdraw"
                                        If TLO.TxId = TLO.RefId Then
                                            ' Ignore all lines that originally had an empty txid, because each withdrawal appears twice in the csv file
                                            .DoNotImport = True
                                        Else
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
                                        End If
                                    Case "trade"
                                        ' Nächste passende Zeile dazuholen
                                        l += 1
                                        If l <= AllLines - 1 Then
                                            NextRow = CSV.Rows(l)
                                        Else
                                            ' No further lines available
                                            NextRow = Nothing
                                        End If
                                        If Row(1) <> NextRow(1) Then
                                            ' next line does not match
                                            NextRow = Nothing
                                        End If
                                        If NextRow Is Nothing Then
                                            If TLO.Amount <= KRAKEN_ZEROVALUETRADELIMIT Then
                                                ' The value of this trade is very low: assume the corresponding second entry would be zero
                                                NextRow = {Row(0), Row(1), Row(2), Row(3), Row(4), Row(5), "0.0", "0.0", Row(8)}
                                            Else
                                                Throw New Exception(String.Format(My.Resources.MyStrings.importMsgKrakenErrorNoSecondEntry, .SourceID))
                                            End If
                                            l -= 1
                                        End If
                                        NextTLO = New KrakenLineObject(MainImportObject, NextRow(2), NextRow(0), NextRow(1), NextRow(3), NextRow(5), NextRow(6), NextRow(7))
                                        Dim QuellKontoRow As KontenRow
                                        If TLO.Amount < 0 Then
                                            SourceTLO = TLO
                                            TargetTLO = NextTLO
                                            QuellKontoRow = KontoRow
                                            KontoRow = MainImportObject.RetrieveAccount(NextTLO.Asset)
                                        ElseIf NextTLO.Amount <= 0 Then
                                            SourceTLO = NextTLO
                                            TargetTLO = TLO
                                            QuellKontoRow = MainImportObject.RetrieveAccount(NextTLO.Asset)
                                        Else
                                            Throw New Exception(String.Format(My.Resources.MyStrings.importMsgKrakenErrorNoNegativeValue, .SourceID))
                                        End If
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
                                            ' Gebühr beim QuellBetrag! Erhöhen...
                                            .QuellBetrag += Math.Abs(SourceTLO.Fee)
                                        End If
                                        If .QuellKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .QuellBetrag
                                        ElseIf .ZielKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .BetragNachGebuehr
                                        End If

                                    Case Else
                                        .DoNotImport = True
                                End Select
                                ' check for fees on target side
                                If .BetragNachGebuehr < .ZielBetrag Then
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
                                If .QuellBetragNachGebuehr < .QuellBetrag Then
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

                                If RecordFee Is Nothing Then
                                    ' Prüfen, ob es ggf. eine Zeile gibt, in der Kraken Fee Credits verringert werden
                                    If .TradetypID = DBHelper.TradeTypen.Kauf OrElse .TradetypID = DBHelper.TradeTypen.Verkauf Then
                                        l += 1
                                        If l <= AllLines - 1 Then
                                            NextRow = CSV.Rows(l)
                                            If NextRow.Length >= 8 Then
                                                NextTLO = New KrakenLineObject(MainImportObject, NextRow(2), NextRow(0), NextRow(1), NextRow(3), NextRow(5), NextRow(6), NextRow(7))
                                                If NextTLO.Asset = "FEE" AndAlso NextTLO.RefId = TLO.RefId Then
                                                    ' Zeile mit Kraken Fee Credit-Buchung gefunden, entsprechende Gebühren-Buchung anlegen
                                                    RecordFee = .Clone()
                                                    RecordFee.SourceID = .SourceID & "/fee"
                                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                    RecordFee.BetragNachGebuehr = 0
                                                    RecordFee.QuellKontoID = MainImportObject.GetAccount(NextTLO.Asset).ID
                                                    RecordFee.QuellBetrag = NextTLO.Fee
                                                    RecordFee.ZielBetrag = RecordFee.QuellBetrag
                                                    RecordFee.ZielKontoID = MainImportObject.GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                                    RecordFee.WertEUR = 0
                                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                    RecordFee.Info = String.Format(My.Resources.MyStrings.ImportInfoKrakenTradeFeeCredits, .SourceID)
                                                Else
                                                    l -= 1
                                                End If
                                            End If
                                        Else
                                            l -= 1
                                        End If
                                    End If
                                End If

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                    If Not RecordFee Is Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If
                            End With

                        Catch ex As Exception
                            If FileImportError(ErrCounter, l + 1, ex) = 0 Then
                                Return False
                                Exit Function
                            End If
                        End Try

                    End If

                Next l

                MainImportObject.Import_Records(ImportRecords, FileNames(0), ReadImportdataPercentage, , True)
                Cursor.Current = Cursors.Default

            End If
        End If

        Return ErrCounter = MaxErrors

    End Function



End Class

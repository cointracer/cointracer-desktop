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
Imports System.Linq

Public Class Import_CoinTracer
    Inherits FileImportBase
    Implements IFileImport

    ''' <summary>
    ''' Helper structure for holding the column indexes of each value
    ''' </summary>
    Private Structure CoinTracerColumns
        Dim [DateTime] As Short
        Dim Reference As Short
        Dim Info As Short
        Dim SourcePlatform As Short
        Dim SourceCurrency As Short
        Dim SourceAmount As Short
        Dim TargetPlatform As Short
        Dim TargetCurrency As Short
        Dim TargetAmount As Short
        Dim FeePlatform As Short
        Dim FeeCurrency As Short
        Dim FeeAmount As Short
        Dim DateOfAcquisition As Short
        Dim TaxAmount As Short
    End Structure

    Private _Culture As CultureInfo

    Private Class CoinTracerLineObject

        Private _Import As Import

        Private _Date As Date
        Public ReadOnly Property DateTime() As Date
            Get
                Return _Date
            End Get
        End Property

        Private _DateOfAcquisition As Date
        Public ReadOnly Property DateOfAcquisition() As Date
            Get
                Return _Date.Date
            End Get
        End Property

        Private _Reference As String
        Public ReadOnly Property Reference() As String
            Get
                Return _Reference
            End Get
        End Property

        Private _Info As String
        Public ReadOnly Property Info() As String
            Get
                Return _Info
            End Get
        End Property

        Private _SourcePlatform As Long
        Public ReadOnly Property SourcePlatform() As Long
            Get
                Return _SourcePlatform
            End Get
        End Property

        Private _TargetPlatform As Long
        Public ReadOnly Property TargetPlatform() As Long
            Get
                Return _TargetPlatform
            End Get
        End Property

        Private _FeePlatform As Long
        Public ReadOnly Property FeePlatform() As Long
            Get
                Return _FeePlatform
            End Get
        End Property

        Private _SourceCurrency As KontenRow
        Public ReadOnly Property SourceCurrency() As KontenRow
            Get
                Return _SourceCurrency
            End Get
        End Property

        Private _TargetCurrency As KontenRow
        Public ReadOnly Property TargetCurrency() As KontenRow
            Get
                Return _TargetCurrency
            End Get
        End Property

        Private _FeeCurrency As KontenRow
        Public ReadOnly Property FeeCurrency() As KontenRow
            Get
                Return _FeeCurrency
            End Get
        End Property

        Private _SourceAmount As Decimal
        Public ReadOnly Property SourceAmount() As Decimal
            Get
                Return _SourceAmount
            End Get
        End Property

        Private _TargetAmount As Decimal
        Public ReadOnly Property TargetAmount() As Decimal
            Get
                Return _TargetAmount
            End Get
        End Property

        Private _FeeAmount As Decimal
        Public ReadOnly Property FeeAmount() As Decimal
            Get
                Return _FeeAmount
            End Get
        End Property

        Private _TaxAmount As Decimal
        Public ReadOnly Property TaxAmount() As Decimal
            Get
                Return _TaxAmount
            End Get
        End Property

        Public Sub New(ByRef Import As Import)
            _Import = Import
            _SourceCurrency = Nothing
            _TargetCurrency = Nothing
            _FeeCurrency = Nothing
        End Sub

        Public Sub New(ByRef Import As Import,
                       ByRef Culture As CultureInfo,
                       ByRef ColumnSet As CoinTracerColumns,
                       ByRef ColumnArray As String())
            Me.New(Import)
            _Date = CType(ColumnArray(ColumnSet.DateTime), Date)
            If ColumnSet.DateOfAcquisition >= 0 AndAlso IsDate(ColumnArray(ColumnSet.DateOfAcquisition)) Then
                _DateOfAcquisition = CType(ColumnArray(ColumnSet.DateOfAcquisition), Date)
            Else
                _DateOfAcquisition = DATENULLVALUE
            End If
            _Reference = ColumnArray(ColumnSet.Reference).Trim
            _Info = ColumnArray(ColumnSet.Info).Trim
            If ColumnArray(ColumnSet.SourcePlatform).Length > 0 Then
                _SourcePlatform = _Import.RetrievePlatform(ColumnArray(ColumnSet.SourcePlatform))?.ID
            Else
                _SourcePlatform = 0
            End If
            If ColumnArray(ColumnSet.TargetPlatform).Length > 0 Then
                _TargetPlatform = _Import.RetrievePlatform(ColumnArray(ColumnSet.TargetPlatform))?.ID
            Else
                _TargetPlatform = 0
            End If
            If ColumnSet.FeePlatform >= 0 AndAlso ColumnArray(ColumnSet.FeePlatform).Length > 0 Then
                _FeePlatform = _Import.RetrievePlatform(ColumnArray(ColumnSet.FeePlatform))?.ID
            Else
                _FeePlatform = Math.Max(_SourcePlatform, _TargetPlatform)   ' (use the platform that is not zero...)
            End If
            If ColumnArray(ColumnSet.SourceCurrency).Length > 0 Then
                _SourceCurrency = _Import.RetrieveAccount(ColumnArray(ColumnSet.SourceCurrency))
            End If
            If ColumnArray(ColumnSet.TargetCurrency).Length > 0 Then
                _TargetCurrency = _Import.RetrieveAccount(ColumnArray(ColumnSet.TargetCurrency))
            End If
            If ColumnSet.FeeCurrency >= 0 AndAlso ColumnArray(ColumnSet.FeeCurrency).Length > 0 Then
                _FeeCurrency = _Import.RetrieveAccount(ColumnArray(ColumnSet.FeeCurrency))
            End If
            If ColumnArray(ColumnSet.SourceAmount).Length > 0 Then
                _SourceAmount = Decimal.Parse(ColumnArray(ColumnSet.SourceAmount), Culture)
            Else
                _SourceAmount = 0
            End If
            If ColumnArray(ColumnSet.TargetAmount).Length > 0 Then
                _TargetAmount = Decimal.Parse(ColumnArray(ColumnSet.TargetAmount), Culture)
            Else
                _TargetAmount = 0
            End If
            If ColumnSet.FeeAmount >= 0 AndAlso ColumnArray(ColumnSet.FeeAmount).Length > 0 Then
                _FeeAmount = Decimal.Parse(ColumnArray(ColumnSet.FeeAmount), Culture)
            Else
                _FeeAmount = 0
            End If
            If ColumnSet.TaxAmount >= 0 AndAlso ColumnArray(ColumnSet.TaxAmount).Length > 0 Then
                _TaxAmount = Decimal.Parse(ColumnArray(ColumnSet.TaxAmount), Culture)
            Else
                _TaxAmount = 0
            End If
        End Sub

    End Class


    ''' <summary>
    ''' Initializes this import
    ''' </summary>
    ''' <param name="MainImportObject">Reference to the calling import object</param>
    Public Sub New(MainImportObject As Import)
        MyBase.New(MainImportObject)

        Platform = PlatformManager.Platforms.CoinTracer
        CSVAutoDetectEncoding = True
        MultiSelectFiles = False
        CSVSkipFirstLine = False
        FileDialogTitle = My.Resources.MyStrings.importOpenFileTitleCoinTracer
        CheckFirstLine = True
        _Culture = Nothing
    End Sub

    ''' <summary>
    ''' This method is initiated by the base class after reading the file content. Field separators and number format is determined here.
    ''' </summary>
    Friend Overrides Sub AnalyseCsvLines(ByRef Lines As String())
        Dim NumLines As Integer = Lines.LongLength
        CSV.Separator = vbTab
        If NumLines > 0 AndAlso Not Lines(0).Contains(vbTab) Then CSV.Separator = ";"
        Dim Item As String
        Dim IdxPoint As Integer
        Dim IdxComma As Integer
        For l As Integer = 1 To Math.Min(10, Lines.LongLength) - 1
            For Each Item In Lines(l).Split(CSV.Separator)
                If IsNumeric(Item) Then
                    IdxPoint = Item.LastIndexOf("."c)
                    IdxComma = Item.LastIndexOf(","c)
                    If IdxPoint >= 0 OrElse IdxComma >= 0 Then
                        If IdxComma > IdxPoint Then
                            _Culture = New CultureInfo("de")
                        Else
                            _Culture = New CultureInfo("en")
                        End If
                        Exit For
                    End If
                End If
            Next
            If _Culture IsNot Nothing Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' Show a customized import hint and pass on to the default OpenFile routine.
    ''' </summary>
    ''' <returns>true, if file has been opened, false otherwise</returns>
    Protected Overrides Function OpenFile() As Boolean
        If FileNames.Length = 0 OrElse FileNames(0) Is Nothing OrElse FileNames(0).Length = 0 Then
            Dim Dlg As New frmGetCtData
            If Dlg.ShowWithNotAgainOption("ImportCoinTracer", DialogResult.OK) <> DialogResult.OK Then
                Return False
            End If
        End If
        Return MyBase.OpenFile()
    End Function

    ''' <summary>
    ''' Perform the actual import for CoinTracer generic format files
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    Friend Overrides Function ImportContent() As Boolean Implements IFileImport.ImportContent
        Dim Row() As String
        Dim ErrorCounter As Long = MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim AllLines As Long
        Dim i As Long
        Dim FeeAmount As Decimal = 0
        Dim Currency As String = "BTC"

        ' TODO: Dim HasFidorFees As Boolean = False

        If CSV.FileExists AndAlso CSV.Rows.Count > 1 Then
            Cursor.Current = Cursors.WaitCursor
            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))

            ' Determine the column mapping
            Dim Line As String() = CSV.Rows(0)
            Dim ColMap As CoinTracerColumns
            With ColMap
                .DateOfAcquisition = -1
                .DateTime = -1
                .FeeAmount = -1
                .FeeCurrency = -1
                .FeePlatform = -1
                .Info = -1
                .Reference = -1
                .SourceAmount = -1
                .SourceCurrency = -1
                .SourcePlatform = -1
                .TargetAmount = -1
                .TargetCurrency = -1
                .TargetPlatform = -1
                .TaxAmount = -1
            End With
            For i = 0 To Line.Length - 1
                Select Case Line(i).ToLower
                    Case "dateofacquisition"
                        ColMap.DateOfAcquisition = i
                    Case "datetime"
                        ColMap.DateTime = i
                    Case "feeamount"
                        ColMap.FeeAmount = i
                    Case "feecurrency"
                        ColMap.FeeCurrency = i
                    Case "feeplatform"
                        ColMap.FeePlatform = i
                    Case "info"
                        ColMap.Info = i
                    Case "reference"
                        ColMap.Reference = i
                    Case "sourceamount"
                        ColMap.SourceAmount = i
                    Case "sourcecurrency"
                        ColMap.SourceCurrency = i
                    Case "sourceplatform"
                        ColMap.SourcePlatform = i
                    Case "targetamount"
                        ColMap.TargetAmount = i
                    Case "targetcurrency"
                        ColMap.TargetCurrency = i
                    Case "targetplatform"
                        ColMap.TargetPlatform = i
                    Case "taxamount"
                        ColMap.TaxAmount = i
                End Select
            Next

            ' Import actual data
            ImportRecords = New List(Of dtoTradesRecord)
            Dim TLO As CoinTracerLineObject
            AllLines = CSV.Rows.Count
            For i = 1 To AllLines - 1
                ProgressWaitManager.UpdateProgress(i + 1 / AllLines * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgReadingFile, i + 1, AllLines))
                Row = CSV.Rows(i)
                If Row.Length >= 8 Then
                    TLO = New CoinTracerLineObject(MainImportObject,
                                                   _Culture,
                                                   ColMap,
                                                   Row)
                    Record = New dtoTradesRecord
                    RecordFee = Nothing
                    With Record
                        Try
                            If TLO.Reference.Length > 0 Then
                                .SourceID = TLO.Reference
                            Else
                                .SourceID = MD5FromString(TLO.DateTime & TLO.Info & TLO.SourcePlatform & TLO.SourceCurrency.Code & TLO.SourceAmount &
                                                          TLO.TargetPlatform & TLO.TargetCurrency.Code & TLO.TargetAmount)
                            End If
                            .Zeitpunkt = TLO.DateTime
                            .ZeitpunktZiel = .Zeitpunkt
                            .Info = TLO.Info
                            .ImportPlattformID = Platform
                            .QuellPlattformID = TLO.SourcePlatform
                            .QuellKontoID = TLO.SourceCurrency?.ID
                            .QuellBetrag = TLO.SourceAmount
                            .QuellBetragNachGebuehr = TLO.SourceAmount
                            .ZielPlattformID = TLO.TargetPlatform
                            .ZielKontoID = TLO.TargetCurrency?.ID
                            .ZielBetrag = TLO.TargetAmount
                            .BetragNachGebuehr = TLO.TargetAmount
                            If .ZielKontoID = .QuellKontoID OrElse .ZielKontoID = DBHelper.Konten.Unbekannt OrElse .QuellKontoID = DBHelper.Konten.Unbekannt Then
                                If .QuellPlattformID = PlatformManager.Platforms.Bank OrElse .QuellPlattformID = PlatformManager.Platforms.Unknown Then
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .DoNotImport = False
                                ElseIf .ZielPlattformID = PlatformManager.Platforms.Bank OrElse .ZielPlattformID = PlatformManager.Platforms.Unknown Then
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .DoNotImport = False
                                Else
                                    .TradetypID = DBHelper.TradeTypen.Transfer
                                    .DoNotImport = False
                                End If
                            Else
                                If TLO.TargetCurrency.IstFiat = False Then
                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                    .DoNotImport = False
                                ElseIf TLO.TargetCurrency.IstFiat AndAlso TLO.SourceCurrency.IstFiat = False Then
                                    .TradetypID = DBHelper.TradeTypen.Verkauf
                                    .DoNotImport = False
                                Else
                                    .TradetypID = DBHelper.TradeTypen.Transfer
                                    .DoNotImport = False
                                End If
                            End If
                            If TLO.TaxAmount > 0 Then
                                .WertEUR = TLO.TaxAmount
                            ElseIf (.TradetypID = DBHelper.TradeTypen.Einzahlung Or .TradetypID = DBHelper.TradeTypen.Verkauf) AndAlso .ZielKontoID = DBHelper.Konten.EUR Then
                                .WertEUR = .ZielBetrag
                            ElseIf (.TradetypID = DBHelper.TradeTypen.Auszahlung Or .TradetypID = DBHelper.TradeTypen.Kauf Or .TradetypID = DBHelper.TradeTypen.Transfer) AndAlso .QuellKontoID = DBHelper.Konten.EUR Then
                                .WertEUR = .QuellBetrag
                            End If
                            If TLO.FeeAmount > 0 Then
                                ' Create fee record
                                RecordFee = .Clone()
                                RecordFee.SourceID = .SourceID & "/fee"
                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                RecordFee.QuellPlattformID = TLO.FeePlatform
                                RecordFee.ZielPlattformID = RecordFee.QuellPlattformID
                                RecordFee.ZielKontoID = TLO.FeeCurrency.GebuehrKontoID
                                RecordFee.QuellKontoID = TLO.FeeCurrency.ID
                                RecordFee.ZielBetrag = TLO.FeeAmount
                                RecordFee.QuellBetrag = TLO.FeeAmount
                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                RecordFee.WertEUR = 0
                                RecordFee.BetragNachGebuehr = 0
                                RecordFee.Info = String.Format(My.Resources.MyStrings.importInfoTradeFee, .SourceID)
                                ' Adjust target or source amounts, find out where fees apply
                                If RecordFee.QuellKontoID = .ZielKontoID AndAlso RecordFee.QuellPlattformID = .ZielPlattformID Then
                                    ' Assume fee at target of transaction
                                    .ZielBetrag += RecordFee.QuellBetrag
                                ElseIf RecordFee.QuellKontoID = .QuellKontoID AndAlso RecordFee.QuellPlattformID = .QuellPlattformID Then
                                    ' Assume fee at source of transaction
                                    .QuellBetrag += RecordFee.QuellBetrag
                                End If
                            End If
                            If (.TradetypID = DBHelper.TradeTypen.Auszahlung OrElse .TradetypID = DBHelper.TradeTypen.Einzahlung) _
                                AndAlso .QuellPlattformID <> PlatformManager.Platforms.Unknown AndAlso .ZielPlattformID <> PlatformManager.Platforms.Unknown Then
                                ' CoinTracer import exclusive: transform theses kinds of in- or outpayments directly into transfers
                                .TradetypID = DBHelper.TradeTypen.Transfer
                                .InTradeID = -1
                                .OutTradeID = -1
                            End If

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
        End If

        Return ErrorCounter = MaxErrors
    End Function

End Class

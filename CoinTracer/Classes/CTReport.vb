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

Imports System.Linq
Imports CoinTracer.CoinTracerDataSet
Imports CoinTracer.CoinTracerDataSetTableAdapters

''' <summary>
''' Klasse zum Verwalten von Report-Daten
''' </summary>
Public Class CTReport

#Region "Konstanten"

    Private Const CHECKUSDCOLUMNNAME As String = "_IstUsdTrade"
    Protected Friend Const CHECKINPLATFORMCOLUMNNAME As String = "_QuellPlattformID"
    Protected Friend Const CHECKOUTPLATFORMCOLUMNNAME As String = "_ZielPlattformID"

#End Region

#Region "Typen & Strukturen"

    Public Enum ReportTypes
        GainingsReport = 1
        GainingsReportDetailed = 2
    End Enum

    Public Enum ReportTradeSelections
        TaxableOnly = 1
        AllTrades = 2
    End Enum

    Public Class ReportParameters
        Private _FromDate As Date
        Public Property FromDate() As Date
            Get
                Return _FromDate
            End Get
            Set(ByVal value As Date)
                _FromDate = value
            End Set
        End Property
        Private _ToDate As Date
        Public Property ToDate() As Date
            Get
                Return _ToDate
            End Get
            Set(ByVal value As Date)
                _ToDate = value
            End Set
        End Property
        Private _Name As String
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Private _TaxNumber As String
        Public Property TaxNumber() As String
            Get
                Return _TaxNumber
            End Get
            Set(ByVal value As String)
                _TaxNumber = value
            End Set
        End Property
        Private _ReportComment As String
        Public Property ReportComment() As String
            Get
                Return _ReportComment
            End Get
            Set(ByVal value As String)
                _ReportComment = value
            End Set
        End Property
        Private _SelectedPlatforms As String
        Public Property SelectedPlatforms() As String
            Get
                Return _SelectedPlatforms
            End Get
            Set(ByVal value As String)
                _SelectedPlatforms = value
            End Set
        End Property
        Private _ShowTransfers As Integer
        Public Property ShowTransfers() As Integer
            Get
                Return _ShowTransfers
            End Get
            Set(ByVal value As Integer)
                _ShowTransfers = value
            End Set
        End Property
    End Class

#End Region

#Region "Eigenschaften"

    Private _DT As VW_GainingsReport2DataTable

    Private _cnn As SQLite.SQLiteConnection
    Public Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
        Set(value As SQLite.SQLiteConnection)
            _cnn = value
            If _cnn IsNot Nothing Then
                _DS = New CoinTracerDataSet(_cnn)
            Else
                _DS = Nothing
            End If
        End Set
    End Property

    Private _RT As ReportTypes
    Public Property ReportType() As ReportTypes
        Get
            Return _RT
        End Get
        Set(ByVal value As ReportTypes)
            _RT = value
        End Set
    End Property

    Private _RTS As ReportTradeSelections
    Public Property TradeSelection() As ReportTradeSelections
        Get
            Return _RTS
        End Get
        Set(ByVal value As ReportTradeSelections)
            _RTS = value
        End Set
    End Property

    Private _HasUsdTrades As Boolean
    Public Property HasUsdTrades() As Boolean
        Get
            Return _HasUsdTrades
        End Get
        Set(ByVal value As Boolean)
            _HasUsdTrades = value
        End Set
    End Property

    Private _RP As ReportParameters
    Public Property Parameters() As ReportParameters
        Get
            Return _RP
        End Get
        Set(ByVal value As ReportParameters)
            _RP = value
        End Set
    End Property

    Private _PeriodSQL As String
    Public Property PeriodSQL() As String
        Get
            Return _PeriodSQL
        End Get
        Set(ByVal value As String)
            _PeriodSQL = value
            ' Komfortfunktion: FromDate und ToDate bei ReportParamenters setzen
            Dim DateParts() As String = Split(_PeriodSQL, " ")
            _RP.FromDate = DATENULLVALUE
            _RP.ToDate = DATENULLVALUE
            If DateParts.Length = 4 AndAlso DateParts(0) = "BETWEEN" Then
                _RP.FromDate = CDate(DateParts(1).Replace("'", ""))
                _RP.ToDate = DateAdd(DateInterval.Day, -1, CDate(DateParts(3).Replace("'", "")))
                If GainingsCutOffDay <> DATENULLVALUE AndAlso GainingsCutOffDay < _RP.ToDate Then
                    _RP.ToDate = GainingsCutOffDay
                End If
            Else
                If GainingsCutOffDay <> DATENULLVALUE Then
                    _RP.ToDate = GainingsCutOffDay
                End If
            End If
        End Set
    End Property

    Private _PlatformIDs As String
    ''' <summary>
    ''' Liste der Plattform-IDs, nach denen gefiltert werden soll. 
    ''' Wenn leer, gibt es keine Filterung nach Plattform.
    ''' </summary>
    ''' <value>Kommaseparierte Liste aller Plattform-IDs, nach denen gefiltert werden soll.</value>
    ''' <returns></returns>
    Public Property PlatformIDs() As String
        Get
            Return _PlatformIDs
        End Get
        Set(ByVal value As String)
            _PlatformIDs = value.Replace(" ", "")
            If _PlatformIDs.StartsWith(",") Then
                _PlatformIDs = _PlatformIDs.Substring(1, _PlatformIDs.Length - 1)
            End If
            If _PlatformIDs.EndsWith(",") Then
                _PlatformIDs = _PlatformIDs.Substring(0, _PlatformIDs.Length - 1)
            End If
        End Set
    End Property


    Private _GainingsCutOffDay As Date
    Public Property GainingsCutOffDay() As Date
        Get
            Return _GainingsCutOffDay
        End Get
        Set(ByVal value As Date)
            _GainingsCutOffDay = value
            If _GainingsCutOffDay <> DATENULLVALUE AndAlso _RP.ToDate <> DATENULLVALUE AndAlso _GainingsCutOffDay < _RP.ToDate Then
                _RP.ToDate = GainingsCutOffDay
            End If
        End Set
    End Property

    Private _SzenarioID As String
    Public Property SzenarioID() As Long
        Get
            Return _SzenarioID
        End Get
        Set(ByVal value As Long)
            _SzenarioID = value
        End Set
    End Property

    Private _TableName As String
    Public ReadOnly Property TableName() As String
        Get
            Return _TableName
        End Get
    End Property

    Public ReadOnly Property DataTable() As DataTable
        Get
            Return _DT
        End Get
    End Property

#End Region

#Region "Konstruktoren"

    Public Sub New(Connection As SQLite.SQLiteConnection)
        _RP = New ReportParameters
        _DT = New VW_GainingsReport2DataTable
        Me.Connection = Connection
        PeriodSQL = ""
        PlatformIDs = ""
        GainingsCutOffDay = DATENULLVALUE
        _RP.FromDate = DATENULLVALUE
        _RP.ToDate = DATENULLVALUE
    End Sub

#End Region

#Region "Interne Variablen"

    Private _DS As CoinTracerDataSet

#End Region

#Region "Methoden"

    ''' <summary>
    ''' Lädt die Datentabelle neu und liefert die Anzahl Datensätze zurück
    ''' </summary>
    ''' <returns>Anzahl Datensätze</returns>
    Public Function Reload() As Long

        Try
            ' Max date of report is limited by GainingsCutOffDay
            Dim ToDate As Date
            If _RP.ToDate > GainingsCutOffDay Then
                ToDate = GainingsCutOffDay
            Else
                ToDate = _RP.ToDate
            End If
            Dim ReportTA As New VW_GainingsReport2TableAdapter
            ReportTA.FillByTimeScenarioTradeTypePlatforms(_DT,
                                                          _RP.FromDate,
                                                          ToDate.AddDays(1),
                                                          SzenarioID,
                                                          IIf(Me.TradeSelection = ReportTradeSelections.AllTrades, 0, 1),
                                                          Parameters.ShowTransfers,
                                                          PlatformIDs)
            StripHiddenColumns(_DT)
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
        Return _DT.Rows.Count

    End Function

    ''' <summary>
    ''' Takes the DataTable and removes every column whose name begins with an underscore ('_')
    ''' </summary>
    ''' <param name="ReportDataTable">DataTable whose columns need to be erased</param>
    Private Sub StripHiddenColumns(ByRef ReportDataTable As DataTable)
        Dim EraseCols As New List(Of String)
        For Each Col As DataColumn In ReportDataTable.Columns
            If Col.ColumnName.StartsWith("_") Then
                EraseCols.Add(Col.ColumnName)
            End If
        Next
        For Each ColName In EraseCols
            ReportDataTable.Columns.Remove(ColName)
        Next
    End Sub

    ''' <summary>
    ''' Adds all referenced buy trades to the report's DataTable. Useful if we are only showing a certain time frame.
    ''' </summary>
    ''' <returns>Number of trades added to the report.</returns>
    Public Function LoadReferencedTrades() As Long
        Dim TableReference As VW_GainingsReport2DataTable = _DT
        Dim AddedTable As New VW_GainingsReport2DataTable
        Dim TradeIdList As New List(Of Long)
        Dim TradeIdListHistory As New List(Of Long)
        Dim CurrentID As Long
        Dim TotalRowsAdded As Long = 0
        Dim RowsToCheck As Long = _DT.Rows.Count
        While RowsToCheck > 0
            ' Collect all referenced InTrade IDs not being part of the current report table
            For i As Long = 0 To RowsToCheck - 1
                For Each IDString In DirectCast(TableReference.Rows(i), VW_GainingsReport2Row).Vorgang_Anschaffung.Split("(")
                    CurrentID = Val(IDString)
                    If CurrentID > 0 AndAlso Not TradeIdListHistory.Contains(CurrentID) Then
                        If Not AddedTable.Where(Function(r) r.Vorgang = CurrentID).Any AndAlso Not _DT.Where(Function(r) r.Vorgang = CurrentID).Any Then
                            TradeIdList.Add(CurrentID)
                            TradeIdListHistory.Add(CurrentID)
                        End If
                    End If
                Next
            Next
            ' Add these rows to the temporary table
            If TradeIdList.Count > 0 Then
                Dim ReportTableNew As New VW_GainingsReport2DataTable
                With New VW_GainingsReport2TableAdapter With {.ClearBeforeFill = True}
                    .FillByTradeIDs(ReportTableNew, TradeIdList, SzenarioID, Parameters.ShowTransfers)
                End With
                RowsToCheck = ReportTableNew.Rows.Count
                TotalRowsAdded += RowsToCheck
                ReportTableNew.Merge(AddedTable)
                AddedTable = ReportTableNew
                TradeIdList.Clear()
                TableReference = AddedTable
            Else
                RowsToCheck = 0
            End If
        End While
        If TotalRowsAdded > 1 Then
            ' Re-sort the referenced rows
            Dim InsertRowsTable As New VW_GainingsReport2DataTable
            For Each ReportRow As VW_GainingsReport2Row In AddedTable.OrderBy(Function(r As VW_GainingsReport2Row) r.Zeitpunkt).ThenBy(Function(r As VW_GainingsReport2Row) r.Vorgang)
                InsertRowsTable.ImportRow(ReportRow)
            Next
            InsertRowsTable.Merge(_DT)
            _DT = InsertRowsTable
        End If
        Return TotalRowsAdded
    End Function

#End Region

End Class

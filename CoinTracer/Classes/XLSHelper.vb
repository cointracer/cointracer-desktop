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

Imports System.IO

''' <summary>
''' Helper class for handling of XLS and XLSX files
''' </summary>
Public Class XLSHelper
    Implements IDataFileHelper

    Private _Filename As String
    Friend Property Filename() As String Implements IDataFileHelper.Filename
        Get
            Return _Filename
        End Get
        Set(ByVal value As String)
            _Filename = value
        End Set
    End Property

    Private _Filecontent As String
    Friend Property Filecontent() As String Implements IDataFileHelper.Filecontent
        Get
            Return _Filecontent
        End Get
        Set(ByVal value As String)
            _Filecontent = value
        End Set
    End Property

    Private _AutoDetectEncoding As Boolean
    Friend Property AutoDetectEncoding() As Boolean Implements IDataFileHelper.AutoDetectEncoding
        Get
            Return _AutoDetectEncoding
        End Get
        Set(ByVal value As Boolean)
            _AutoDetectEncoding = value
        End Set
    End Property


    Private _Encoding As System.Text.Encoding
    Friend ReadOnly Property Encoding() As System.Text.Encoding Implements IDataFileHelper.Encoding
        Get
            Return _Encoding
        End Get
    End Property

    Private _Separator As String
    Friend Property Separator() As String Implements IDataFileHelper.Separator
        Get
            Return _Separator
        End Get
        Set(ByVal value As String)
            _Separator = value
        End Set
    End Property

    Private _Textqualifier As String
    Friend Property Textqualifier() As String Implements IDataFileHelper.Textqualifier
        Get
            Return _Textqualifier
        End Get
        Set(ByVal value As String)
            _Textqualifier = value
        End Set
    End Property

    Private _DecimalPoint As String
    Friend Property DecimalPoint() As String Implements IDataFileHelper.DecimalPoint
        Get
            Return _DecimalPoint
        End Get
        Set(ByVal value As String)
            _DecimalPoint = value
        End Set
    End Property

    Private _DecimalSeparator As String
    Friend Property DecimalSeparator() As String Implements IDataFileHelper.DecimalSeparator
        Get
            Return _DecimalSeparator
        End Get
        Set(ByVal value As String)
            _DecimalSeparator = value
        End Set
    End Property

    Private _AutoRead1stRowOnly As Boolean
    Public Property AutoReadHeaderOnly() As Boolean
        Get
            Return _AutoRead1stRowOnly
        End Get
        Set(ByVal value As Boolean)
            _AutoRead1stRowOnly = value
        End Set
    End Property

    Private _AllLines As String()
    Private _ReRead As Boolean

    ''' <summary>
    ''' Return a certain line from file
    ''' </summary>
    ''' <param name="i">Line no (0-based)</param>
    Friend ReadOnly Property Line(i As Integer) As String Implements IDataFileHelper.Line
        Get
            Return _AllLines(i)
        End Get
    End Property

    ''' <summary>
    ''' Return total number of lines
    ''' </summary>
    Friend ReadOnly Property LineCount() As Integer Implements IDataFileHelper.LineCount
        Get
            If _AllLines Is Nothing Then
                Return -1
            Else
                Return _AllLines.Length
            End If
        End Get
    End Property


    Private _AllRows As List(Of String())
    ''' <summary>
    ''' Return all rows
    ''' </summary>
    ''' <returns>List of all data lines, each line as an array of strings</returns>
    Public ReadOnly Property Rows() As List(Of String()) Implements IDataFileHelper.Rows
        Get
            Return _AllRows
        End Get
    End Property

    Friend ReadOnly Property FileExists() As Boolean Implements IDataFileHelper.FileExists
        Get
            If _Filecontent IsNot Nothing Then
                Return True
            Else
                Return File.Exists(_Filename)
            End If
        End Get
    End Property

    ''' <summary>
    ''' Read file content into AllLines() (just once!)
    ''' </summary>
    ''' <returns>True, on success or if lines have already been read. False otherwise</returns>
    Friend Function ReadAllLines(Optional SkipFirstRow As Boolean = True) As Boolean
        If _AllLines Is Nothing OrElse _ReRead Then
            _ReRead = False
            _AllRows = New List(Of String())
            Try
                Using FS As New FileStream(_Filename, FileMode.Open, FileAccess.Read)
                    Dim WB As NPOI.SS.UserModel.IWorkbook
                    Dim Filename As String = _Filename.ToLower
                    If Filename.EndsWith(".xlsx") Then
                        WB = New NPOI.XSSF.UserModel.XSSFWorkbook(FS)
                    ElseIf Filename.EndsWith(".xls") Then
                        WB = New NPOI.HSSF.UserModel.HSSFWorkbook(FS)
                    Else
                        ' Cannot determine suitable file type
                        Throw New IOException(My.Resources.MyStrings.xlsErrorInvalidFilename)
                    End If
                    Dim Sheet As NPOI.SS.UserModel.ISheet = WB.GetSheetAt(0)
                    If IsNothing(Sheet) Then Return True
                    Dim LastColNum As Long = -1
                    If Sheet.LastRowNum = 0 Then Return True
                    ' Read 1st row and determine number of cols (filled with data)
                    Dim curRow As NPOI.SS.UserModel.IRow = Sheet.GetRow(0)
                    If IsNothing(curRow) Then Return True
                    Dim curCell As NPOI.SS.UserModel.ICell
                    Dim Lines As New List(Of String)
                    Dim RowArray() As String = {}
                    Dim i As Long
                    For i = 0 To curRow.LastCellNum - 1
                        curCell = curRow.GetCell(i)
                        If IsNothing(curCell) Then
                            Exit For
                        Else
                            LastColNum = i
                            ReDim Preserve RowArray(i)
                            RowArray(i) = GetCellContentString(curCell)
                        End If
                    Next
                    If LastColNum >= 0 Then
                        Lines.Add(Join(RowArray, _Separator))
                        If Not SkipFirstRow Then
                            _AllRows.Add(RowArray)
                        End If
                    Else
                        Return True
                    End If
                    If Not _AutoRead1stRowOnly Then
                        ' read rest of sheet
                        ReDim RowArray(LastColNum)
                        curCell = Nothing
                        For j As Long = 1 To Sheet.LastRowNum
                            curRow = Sheet.GetRow(j)
                            If IsNothing(curRow) OrElse curRow.LastCellNum - 1 < LastColNum Then Exit For
                            ' RowArray = {}
                            For i = 0 To LastColNum
                                curCell = curRow.GetCell(i)
                                If IsNothing(curCell) Then Exit For
                                RowArray(i) = GetCellContentString(curCell)
                            Next
                            If IsNothing(curCell) Then Exit For
                            ' add current row
                            _AllRows.Add(RowArray.Clone)
                            Lines.Add(Join(RowArray, _Separator))
                        Next
                    End If
                    _AllLines = Lines.ToArray
                End Using
                Return True
            Catch ex As IOException
                Throw
            Catch ex As Exception
                Return False
            End Try
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Quick solution for stripping text qualifying ' at the beginning of an Excel cell 
    ''' </summary>
    ''' <param name="Cell">Excel-Cell to process</param>
    ''' <returns>Clean content string</returns>
    Private Function GetCellContentString(ByRef Cell As NPOI.SS.UserModel.ICell) As String
        Dim Content As String = Cell.ToString
        If Content.StartsWith("'") Then
            Return Content.Substring(1)
        Else
            Return Content
        End If
    End Function

    ''' <summary>
    ''' Return first line of file
    ''' </summary>
    ''' <returns>First line of file, Nothing if empty</returns>
    Public Function FirstLine() As String Implements IDataFileHelper.FirstLine
        Dim Result As String = Nothing
        If ReadAllLines() Then
            If _AllLines.Length > 0 Then
                Result = _AllLines(0)
            End If
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Liest alle Zeilen der Datei in Liste Rows und gibt die Anzahl der gelesenen Datensätze zurück
    ''' </summary>
    ''' <param name="SkipFirstLine">True, wenn die erste Zeile übersprungen werden soll (weil sie Spaltenüberschriften enthält)</param>
    ''' <param name="Separator">Trenner zwischen den Feldern</param>
    ''' <param name="Textqualifier">Zeichen, das Textinhalt kennzeichnet</param>
    ''' <param name="DecimalPoint">Zeichen für das Dezimalkomma (wird nur in Funktion StringToDecimal verwendet)</param>
    ''' <param name="DecimalSeparator">Zeichen für die Tausenderguppierung von Dezimalzahlen (wird nur in Funktion StringToDecimal verwendet)</param>
    ''' <returns>Anzahl Datensätze in Rows</returns>
    ''' <remarks></remarks>
    Public Function ReadAllRows(Optional SkipFirstLine As Boolean = True,
                                Optional Separator As String = ",",
                                Optional Textqualifier As String = """",
                                Optional DecimalPoint As String = ".",
                                Optional DecimalSeparator As String = ",") As Long Implements IDataFileHelper.ReadAllRows
        If IsNothing(_AllRows) Then
            _AllRows = New List(Of String())
        End If
        _DecimalPoint = DecimalPoint
        _DecimalSeparator = DecimalSeparator
        _Separator = Separator
        _Textqualifier = Textqualifier
        If _AutoRead1stRowOnly Then
            _AutoRead1stRowOnly = False
            _ReRead = True
        End If
        If ReadAllLines(SkipFirstLine) Then
            Return _AllRows.Count
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Wandelt einen Zahlen-String englischer Notation in einen Decimal-Wert um
    ''' </summary>
    Public Function StringToDecimal(ByVal NumberAsString As String) As Decimal Implements IDataFileHelper.StringToDecimal
        If _DecimalPoint = "A" OrElse _DecimalSeparator = "A" Then
            ' Zahlenformat automatisch feststellen
            If NumberAsString.Contains(",") Then
                If NumberAsString.Contains(".") AndAlso NumberAsString.IndexOf(".") > NumberAsString.IndexOf(",") Then
                    ' Englisch mit Tausendertrenner: diesen entfernen
                    NumberAsString = NumberAsString.Replace(",", "")
                Else
                    ' Deutsch
                    NumberAsString = NumberAsString.Replace(".", "")
                    NumberAsString = NumberAsString.Replace(",", ".")
                End If
            End If
        Else
            ' Tausendertrenner entfernen, nur noch Dezimalpunkt stehen lassen
            NumberAsString = NumberAsString.Replace(_DecimalSeparator, "")
            NumberAsString = NumberAsString.Replace(_DecimalPoint, ".")
        End If
        If NumberAsString.Contains("E") Then
            Return Decimal.Parse(NumberAsString, NumberStyles.Any, CultureInfo.InvariantCulture)
        Else
            Return Decimal.Parse(NumberAsString, CultureInfo.InvariantCulture)
        End If
    End Function

    ''' <summary>
    ''' Definiert eine Callback-Routine für die Vorverarbeitung der einzelnen Datenzeilen 
    ''' </summary>
    ''' <param name="CsvLineProcessRoutine">AddressOf der Rountine</param>
    Friend Sub SetCsvLinePreprocessor(ByVal CsvLineProcessRoutine As CSVHelper.CsvLinePreprocessorDelegate) Implements IDataFileHelper.SetCsvLinePreprocessor
        ' Has no functionality in this class. Only implemented for compatibility with CSVHelper.
    End Sub

    ''' <summary>
    ''' Definiert eine Callback-Routine für die Analyse der Datenzeilen
    ''' </summary>
    ''' <param name="CsvContentAnalyseRoutine">AddressOf der Rountine</param>
    Friend Sub SetCsvContentAnalyser(ByVal CsvContentAnalyseRoutine As CSVHelper.CsvContentAnalyserDelegate) Implements IDataFileHelper.SetCsvContentAnalyser
        ' Has no functionality in this class. Only implemented for compatibility with CSVHelper.
    End Sub

    Public Sub New()
        _AllLines = Nothing
        _AllRows = New List(Of String())
        _Encoding = Text.Encoding.Default
        _DecimalPoint = "."
        _DecimalSeparator = ","
        _Separator = ","
        _Textqualifier = "'"
        _Filename = ""
        _Filecontent = Nothing
        _AutoDetectEncoding = True
        _AutoRead1stRowOnly = False
        _ReRead = False
    End Sub

    Public Sub New(Filename As String, Encoding As System.Text.Encoding, AutoDetectEncoding As Boolean, Filecontent As String)
        Me.New()
        _AutoDetectEncoding = AutoDetectEncoding
        _Encoding = Encoding
        _Filename = Filename
        _Filecontent = Filecontent
    End Sub

    Public Sub New(Filename As String, Encoding As System.Text.Encoding, AutoDetectEncoding As Boolean)
        Me.New()
        _AutoDetectEncoding = AutoDetectEncoding
        _Encoding = Encoding
        _Filename = Filename
    End Sub

    Public Sub New(Filename As String, Encoding As System.Text.Encoding)
        Me.New()
        _Encoding = Encoding
        _Filename = Filename
    End Sub

    Public Sub New(Filename As String, AutoDetectEncoding As Boolean)
        Me.New()
        _AutoDetectEncoding = AutoDetectEncoding
        _Filename = Filename
    End Sub

End Class

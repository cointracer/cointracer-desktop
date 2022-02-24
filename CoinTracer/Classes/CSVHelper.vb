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
''' Hilfsfunktionen für das Einlesen von CSV-Dateien
''' </summary>
Public Class CSVHelper
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

    Public Delegate Sub CsvLinePreprocessorDelegate(ByRef Line As String)
    Private _CsvLinePreprocessor As CsvLinePreprocessorDelegate

    Public Delegate Sub CsvContentAnalyserDelegate(ByRef Lines As String())
    Private _CsvContentAnalyser As CsvContentAnalyserDelegate

    Private _AllLines As String()

    ''' <summary>
    ''' Gibt eine Zeile der Datei zurück
    ''' </summary>
    ''' <param name="i">Zeilennummer, beginnend mit 0</param>
    Friend ReadOnly Property Line(i As Integer) As String Implements IDataFileHelper.Line
        Get
            Return _AllLines(i)
        End Get
    End Property

    ''' <summary>
    ''' Gibt die Anzahl Zeilen der Datei zurück
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
    ''' Alle Datenzeilen der Datei
    ''' </summary>
    ''' <returns>Liste aller Datenzeilen, jede Datenzeile ist als String-Array repräsentiert</returns>
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
    ''' Liest den File-Inhalt in AllLines() (nur einmal!)
    ''' </summary>
    ''' <returns>True, wenn Zeilen gelesen werden konnten oder bereits gelesen waren, sonst False</returns>
    Friend Function ReadAllLines() As Boolean
        If _AllLines Is Nothing Then
            Try
                If _AutoDetectEncoding Then
                    If _Filecontent Is vbNullString Then
                        _Encoding = TextEncodingDetector.DetectTextFileEncoding(_Filename)
                    End If
                    If _Encoding Is Nothing Then
                        _Encoding = Text.Encoding.Default
                    End If
                End If
                If _Filecontent Is vbNullString Then
                    _Filecontent = File.ReadAllText(_Filename, _Encoding) & ""
                End If
                ' Versuch, Zeilenumbrüche möglichst flexibel zu ermitteln
                Dim LFString As String = Environment.NewLine
                If _Filecontent.IndexOf(Environment.NewLine) >= 0 Then
                    LFString = Environment.NewLine
                ElseIf _Filecontent.IndexOf(vbCrLf) >= 0 Then
                    LFString = vbCrLf
                ElseIf _Filecontent.IndexOf(vbCr) >= 0 Then
                    LFString = vbCr
                ElseIf _Filecontent.IndexOf(vbLf) >= 0 Then
                    LFString = vbLf
                Else
                    LFString = Environment.NewLine
                End If
                _AllLines = _Filecontent.Split(LFString)
                If _CsvLinePreprocessor IsNot Nothing Then
                    For i As Integer = 0 To _AllLines.Length - 1
                        _CsvLinePreprocessor(_AllLines(i))
                    Next
                End If
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
    ''' Liefert die erste Zeile der Datei zurück
    ''' </summary>
    ''' <returns>Inhalt der ersten Zeile als String, Nothing wenn Datei nicht vorhanden ist.</returns>
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
    ''' Transformiert das Array _AllLines() in die Liste _AllRows unter Berücksichtigung der eingestellten
    ''' Parameter und gibt die Anzahl Datenzeilen zurück
    ''' </summary>
    ''' <param name="SkipFirstRow">True = erste Zeile soll übersprungen werden, sonst False</param>
    ''' <returns>Anzahl Datenzeilen</returns>
    ''' <remarks>Die gesamte Funktion erscheint vielleicht etwas umständlich, stellt aber sicher, dass z.B. Zeilenumbrüche oder 
    ''' Separatoren innerhalb eines Textfelds korrekt verarbeitet werden</remarks>
    Private Function ProcessAllLines(Optional ByVal SkipFirstRow As Boolean = True) As Long
        Dim RowCount As Integer = 0
        Dim CharCount As Int16
        Dim TextMode As Boolean = False
        Dim EscapedChar As Boolean = False
        Dim LineContent As String
        Dim FieldContent As String = ""
        Dim RowItems() As String = {}

        If ReadAllLines() Then
            ' Call content analyser, if defined
            If _CsvContentAnalyser IsNot Nothing Then
                _CsvContentAnalyser(_AllLines)
            End If
            ' Schleife über alle Zeilen der Datei
            For Each LineContent In _AllLines
                If Not SkipFirstRow Then
                    For CharCount = 0 To LineContent.Length - 1
                        If EscapedChar Then
                            ' wir sind im Escaped-Mode: nächstes Zeichen einfach anfügen
                            FieldContent &= LineContent.Substring(CharCount, 1)
                            EscapedChar = False
                        Else
                            If TextMode Then
                                ' wir sind im Text-Modus: hier können Zeichen escaped werden und Separators werden ignoriert
                                If LineContent.Substring(CharCount, 1) = "\" Then
                                    EscapedChar = True
                                ElseIf LineContent.Substring(CharCount, 1) = _Textqualifier AndAlso LineContent.Substring(CharCount, If(CharCount < LineContent.Length - 1, 2, 1)) <> _Textqualifier & _Textqualifier Then
                                    TextMode = False
                                ElseIf LineContent.Substring(CharCount, If(CharCount < LineContent.Length - 1, 2, 1)) = _Textqualifier & _Textqualifier Then
                                    FieldContent &= _Textqualifier
                                    CharCount += 1
                                Else
                                    FieldContent &= LineContent.Substring(CharCount, 1)
                                End If
                            Else
                                ' wir sind nicht im Text-Modus: Auf Text-Qualifier und Separator reagieren
                                If LineContent.Substring(CharCount, 1) = _Separator Then
                                    ' Neues Feld beginnt: Altes speichern
                                    ReDim Preserve RowItems(RowItems.Length)
                                    RowItems(RowItems.Length - 1) = FieldContent.Trim
                                    FieldContent = ""
                                ElseIf LineContent.Substring(CharCount, 1) = _Textqualifier Then
                                    TextMode = True
                                Else
                                    FieldContent &= LineContent.Substring(CharCount, 1)
                                End If
                            End If
                        End If
                    Next CharCount
                    If TextMode Then
                        FieldContent &= " / "
                    Else
                        ' Zeilenende: nur speichern, wenn wir nicht im Textmode sind
                        ReDim Preserve RowItems(RowItems.Length)
                        RowItems(RowItems.Length - 1) = FieldContent.Trim
                        FieldContent = ""
                        _AllRows.Add(RowItems)
                        RowCount += 1
                        RowItems = {}
                    End If
                Else
                    SkipFirstRow = False
                End If
            Next
        Else
            Throw New Exception("Die Datei '" & _Filename & "' konnte nicht geöffnet/gelesen werden!")
        End If
        Return RowCount

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
    Friend Function ReadAllRows(Optional SkipFirstLine As Boolean = True,
                                Optional Separator As String = ",",
                                Optional Textqualifier As String = """",
                                Optional DecimalPoint As String = ".",
                                Optional DecimalSeparator As String = ",") As Long Implements IDataFileHelper.ReadAllRows
        _AllRows = New List(Of String())
        _DecimalPoint = DecimalPoint
        _DecimalSeparator = DecimalSeparator
        _Separator = Separator
        _Textqualifier = Textqualifier
        Return ProcessAllLines(SkipFirstLine)
    End Function

    ''' <summary>
    ''' Wandelt einen Zahlen-String englischer Notation in einen Decimal-Wert um
    ''' </summary>
    Friend Function StringToDecimal(ByVal NumberAsString As String) As Decimal Implements IDataFileHelper.StringToDecimal
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
    Friend Sub SetCsvLinePreprocessor(ByVal CsvLineProcessRoutine As CsvLinePreprocessorDelegate) Implements IDataFileHelper.SetCsvLinePreprocessor
        _CsvLinePreprocessor = CsvLineProcessRoutine
    End Sub

    ''' <summary>
    ''' Definiert eine Callback-Routine für die Analyse der Datenzeilen
    ''' </summary>
    ''' <param name="CsvContentAnalyseRoutine">AddressOf der Rountine</param>
    Friend Sub SetCsvContentAnalyser(ByVal CsvContentAnalyseRoutine As CsvContentAnalyserDelegate) Implements IDataFileHelper.SetCsvContentAnalyser
        _CsvContentAnalyser = CsvContentAnalyseRoutine
    End Sub

    Public Sub New()
        _AllLines = Nothing
        _AllRows = New List(Of String())
        _Encoding = Text.Encoding.Default
        _DecimalPoint = "."
        _DecimalSeparator = ","
        _Separator = ","
        _Textqualifier = """"
        _Filename = ""
        _Filecontent = Nothing
        _AutoDetectEncoding = True
        _CsvLinePreprocessor = Nothing
        _CsvContentAnalyser = Nothing
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

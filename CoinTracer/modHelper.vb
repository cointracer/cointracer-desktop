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

Imports System.Text
Imports System.Security.Cryptography
Imports System.Reflection

Module modHelper

    Public Const DATENULLVALUE As Date = #12:00:00 AM#
    Public Const DATEMAXVALUE As Date = #2099-12-31 12:00:00 AM#

    Public Sub DefaultErrorHandler(ex As Exception, Optional Message As String = "", Optional EndApplication As Boolean = False, Optional DialogBoxTitle As String = "")
        Cursor.Current = Cursors.Default
        Dim ThisEx As Exception
        If Not ex.InnerException Is Nothing Then
            ThisEx = ex.InnerException
        Else
            ThisEx = ex
        End If
        Dim Msg As String = My.Resources.MyStrings.errorDefaultMessage & Environment.NewLine & Environment.NewLine
        If Message = "" Then
            Msg &= ThisEx.Message
        Else
            Msg &= Message
        End If
        If EndApplication Then
            Msg &= Environment.NewLine & Environment.NewLine & My.Resources.MyStrings.errorDefaultEndApplication
        End If
        Dim Title As String
        If DialogBoxTitle.Length = 0 Then
            Title = My.Resources.MyStrings.errorDefaultTitle
        Else
            Title = DialogBoxTitle
        End If
        MsgBoxEx.BringToFront()
        MessageBox.Show(Msg, Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        WriteLogEntry(Msg, TraceEventType.Error)
        If EndApplication Then
            Application.Exit()
        End If

    End Sub

    ''' <summary>
    ''' Schreibt einen Eintrag in das Application-LogFile unter Berücksichtigung der aktuellen LogLevel-Einstellung
    ''' </summary>
    ''' <param name="message">Nachricht, die geschrieben werden soll</param>
    ''' <param name="severity">Schweregrad der Nachricht</param>
    Public Sub WriteLogEntry(message As String, severity As System.Diagnostics.TraceEventType)
        If My.Settings.LogLevel > 0 AndAlso My.Settings.LogLevel >= severity Then
            My.Application.Log.WriteEntry(Now.ToString("yyyy-MM-dd HH:mm:ss\: ") & message, IIf(severity = TraceEventType.Verbose, TraceEventType.Information, severity))
        End If
    End Sub

    ''' <summary>
    ''' Wandelt einen Zahlen-String englischer Notation in einen Decimal-Wert um
    ''' </summary>
    ''' <param name="NumberAsString">Umzuwandelnde Zahl als String</param>
    ''' <param name="EnglishNotation">True, wenn die Zahl in englischer Notation vorliegt. False bei deutscher Notation (dann könnte man sich den kompletten Funktionsaufruf allerdings auch sparen...)</param>
    Friend Function StrToDec(ByVal NumberAsString As String,
                             Optional ByVal EnglishNotation As Boolean = True) As Decimal
        If EnglishNotation Then
            NumberAsString = NumberAsString.Replace(",", "")
            NumberAsString = NumberAsString.Replace(".", ",")
        Else
            NumberAsString = NumberAsString.Replace(".", "")
        End If
        Return CDec(NumberAsString)
    End Function

    ''' <summary>
    ''' Gibt den MD5-Hash eines Strings als String zurück
    ''' </summary>
    ''' <param name="Source">Quell-String, dessen MD5-Hash berechnet wird</param>
    Friend Function MD5FromString(ByVal Source As String) As String
        Dim Bytes() As Byte
        Dim sb As New StringBuilder()

        'Check for empty string.
        If String.IsNullOrEmpty(Source) Then
            Throw New ArgumentNullException
        End If

        'Get bytes from string.
        Bytes = Encoding.Default.GetBytes(Source)

        'Get md5 hash
        Bytes = MD5.Create().ComputeHash(Bytes)

        'Loop though the byte array and convert each byte to hex.
        For x As Integer = 0 To Bytes.Length - 1
            sb.Append(Bytes(x).ToString("x2"))
        Next

        'Return md5 hash.
        Return sb.ToString()

    End Function

    ''' <summary>
    ''' Kopiert den Tabellen-Inhalt eines DataGridViews in die Zwischenablage
    ''' </summary>
    ''' <param name="DataGridView">Control, dessen Inhalt kopiert werden soll</param>
    ''' <param name="RowNumber">Optional: Zeilennummer, die kopiert werden soll. Wenn leer oder -1, wird gesamte Tabelle kopiert</param>
    ''' <returns>False bei Fehler, sonst True</returns>
    Friend Function DataGridViewToClipboard(DataGridView As DataGridView,
                                            Optional RowNumber As Integer = -1) As Boolean
        Const PROGRESSBARLIMIT = 500    ' if DataGridView has more rows than this, a ProgressWaitManager is shown
        Dim Crs As Cursor
        Dim RetVal As Boolean = True
        Crs = Cursor.Current
        Cursor.Current = Cursors.WaitCursor
        Try
            If frmCopyTableData.ShowDialog() = DialogResult.OK Then

                Dim Separator As String = frmCopyTableData.Separator
                Dim iRow As Integer
                Dim iCol As Integer
                Dim iCols As Integer = DataGridView.Columns.Count
                Dim CurRow As DataGridViewRow
                Dim sb As New StringBuilder()
                If iCols > 0 Then
                    ' put column text qualifiers into array
                    Dim TextQualifiers(iCols - 1) As String
                    For iCol = 0 To iCols - 1
                        If DataGridView.Columns(iCol).ValueType IsNot Nothing Then
                            Select Case DataGridView.Columns(iCol).ValueType.Name
                                Case "String", "DateTime"
                                    TextQualifiers(iCol) = frmCopyTableData.TextQualifier
                                Case Else
                                    TextQualifiers(iCol) = ""
                            End Select
                        Else
                            TextQualifiers(iCol) = ""
                        End If
                        ' and also add column headers if wanted by user
                        If frmCopyTableData.CopyHeaders Then
                            If frmCopyTableData.TextQualifier.Length > 0 Then
                                sb.Append(frmCopyTableData.TextQualifier & DataGridView.Columns(iCol).HeaderText.Replace(frmCopyTableData.TextQualifier, "_") & frmCopyTableData.TextQualifier & Separator)
                            Else
                                sb.Append(DataGridView.Columns(iCol).HeaderText & Separator)
                            End If
                        End If
                    Next iCol
                    If frmCopyTableData.CopyHeaders Then
                        ' truncate first line with column names
                        sb.Length -= Separator.Length
                        sb.Append(Environment.NewLine)
                    End If
                    frmCopyTableData.Close()
                    ' Datenzeile(n) holen
                    Dim RowCount As Long = DataGridView.RowCount
                    If RowCount > PROGRESSBARLIMIT Then
                        ProgressWaitManager.ShowProgress(DataGridView.FindForm(), My.Resources.MyStrings.msgCopyingTable)
                    End If
                    For iRow = IIf(RowNumber < 0, 0, RowNumber) To IIf(RowNumber < 0, DataGridView.RowCount - 1, RowNumber)
                        If RowCount > PROGRESSBARLIMIT Then
                            ProgressWaitManager.UpdateProgress(iRow / RowCount * 100, My.Resources.MyStrings.msgCopyingTable)
                        End If
                        CurRow = DataGridView.Rows(iRow)
                        For iCol = 0 To iCols - 1
                            If CurRow.Cells(iCol).Value IsNot Nothing Then
                                If TextQualifiers(iCol) = "" Then
                                    sb.Append(CurRow.Cells(iCol).Value.ToString.Replace(Separator, "_"))
                                Else
                                    sb.Append(TextQualifiers(iCol) & CurRow.Cells(iCol).Value.ToString.Replace(Separator, "_").Replace(TextQualifiers(iCol), "_") & TextQualifiers(iCol))
                                End If
                            End If
                            sb.Append(Separator)
                        Next iCol
                        sb.Length -= Separator.Length
                        sb.Append(Environment.NewLine)
                    Next iRow
                    ' In Zwischenablage kopieren
                    Dim o As New DataObject
                    o.SetText(sb.ToString)
                    Clipboard.SetDataObject(o, True)
                    If RowCount > PROGRESSBARLIMIT Then
                        ProgressWaitManager.CloseProgress()
                    End If
                End If

            End If

        Catch ex As Exception
            RetVal = False
        End Try
        Cursor.Current = Crs
        Return RetVal
    End Function

    ''' <summary>
    ''' Enables double buffering for DataGridViews to speed up it's rendering.
    ''' </summary>
    ''' <param name="dgv">DataGridView whose DoubeBuffered property will be set to True.</param>
    Friend Sub DataGridViewDoubleBuffer(ByVal dgv As DataGridView)
        dgv.GetType.GetProperty("DoubleBuffered", (BindingFlags.NonPublic Or BindingFlags.Instance)).SetValue(dgv, True, Nothing)
    End Sub

    ''' <summary>
    ''' Konvertiert einen Unix-Timestamp in einen DateTime-Wert
    ''' </summary>
    Friend Function DateFromUnixTimestamp(timestamp As Double) As DateTime
        Dim origin As New DateTime(1970, 1, 1, 0, 0, 0, 0)
        Return origin.AddSeconds(timestamp)
    End Function

    ''' <summary>
    ''' Konvertiert einen DateTime-Wert in einen Unix-Timestamp
    ''' </summary>
    Friend Function DateToUnixTimestamp([date] As DateTime) As Double
        Dim origin As New DateTime(1970, 1, 1, 0, 0, 0, 0)
        Dim diff As TimeSpan = [date] - origin
        Return Math.Floor(diff.TotalSeconds)
    End Function

    ''' <summary>
    ''' Converts a DateTime-Value to an appropriate SQL string
    ''' </summary>
    ''' <param name="date">Date to be converted</param>
    ''' <param name="LowIfNull">True: lowest possible date if date = DATENULLVALUE / False: give back a date far, far in the future</param>
    Friend Function ConvertToSqlDate([date] As DateTime, Optional ByVal LowIfNull As Boolean = True) As String
        If [date] = DATENULLVALUE Then
            If LowIfNull Then
                Return "1990-01-01 00:00:00"
            Else
                Return "2100-01-01 00:00:00"
            End If
        Else
            Return [date].ToString("yyyy-MM-dd HH:mm:ss")
        End If
    End Function

    ''' <summary>
    ''' Converts the first letter of a given String to uppercase (only the very first letter of the whole string, not of every word!)
    ''' </summary>
    Friend Function FirstCharToUppercase(ByVal [String] As String) As String
        Dim Result = [String].ToCharArray
        If Result.Length > 0 Then Result(0) = Char.ToUpperInvariant(Result(0))
        Return New String(Result)
    End Function

End Module

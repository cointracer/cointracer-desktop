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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or in the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports System.IO
Imports System.Linq
Imports System.Data.OleDb

Public Class CsvReaderWriter
    ''' <summary>
    ''' Writes any given DataTable into a CSV file
    ''' </summary>
    ''' <param name="sourceTable">DataTable to be exported</param>
    ''' <param name="CSVFile">Fully qualified name of the output CSV file (will be overwritten, if needed)</param>
    ''' <param name="includeHeaders">True, if the first line shall contain column headers, False otherwise</param>
    Public Shared Sub WriteDataTableToCsv(ByVal sourceTable As DataTable,
                                          ByVal CSVFile As String,
                                          ByVal includeHeaders As Boolean)
        Using writer As StreamWriter = New StreamWriter(CSVFile)
            If (includeHeaders) Then
                Dim headerValues As IEnumerable(Of String) = sourceTable.Columns.OfType(Of DataColumn).Select(Function(column) QuoteValue(column.ColumnName))
                writer.WriteLine(String.Join(",", headerValues))
            End If

            Dim items As IEnumerable(Of String) = Nothing
            For Each row As DataRow In sourceTable.Rows
                items = row.ItemArray.Select(Function(obj) QuoteValue(If(obj?.ToString(), String.Empty)))
                writer.WriteLine(String.Join(",", items))
            Next
            writer.Flush()
        End Using
    End Sub

    ''' <summary>
    ''' Reads a given CSV file into a DataTable object
    ''' </summary>
    ''' <param name="CSVFile">Fully qualified name of the input CSV file</param>
    ''' <param name="includeHeaders">True, if the first line contains column headers, False otherwise</param>
    ''' <returns>The DataTable object containing the CSV file contents</returns>
    Public Shared Function ReadCsvFileToDataTable(ByVal CSVFile As String,
                                                  ByVal includeHeaders As Boolean) As DataTable
        Dim folder As String = Path.GetDirectoryName(CSVFile)
        Dim CnStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & folder & ";Extended Properties=""text;HDR=" & IIf(includeHeaders, "Yes", "No") & ";FMT=Delimited"";"
        Dim ImportTable As New DataTable
        Try
            Using Adp As New OleDbDataAdapter("select * from [" & Path.GetFileName(CSVFile) & "]", CnStr)
                Adp.Fill(ImportTable)
            End Using
        Catch ex As Exception
            Throw New ApplicationException()
        End Try
        Return ImportTable
    End Function

    Private Shared Function QuoteValue(ByVal value As String) As String
        Return String.Concat("""", value.Replace("""", """"""), """")
    End Function
End Class

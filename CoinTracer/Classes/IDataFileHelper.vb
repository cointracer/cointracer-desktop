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

Public Interface IDataFileHelper
    Property Filename() As String
    Property Filecontent() As String
    Property AutoDetectEncoding() As Boolean
    ReadOnly Property Encoding() As System.Text.Encoding
    Property Separator() As String
    Property Textqualifier() As String
    Property DecimalPoint() As String
    Property DecimalSeparator() As String
    ReadOnly Property Line(i As Integer) As String
    ReadOnly Property LineCount() As Integer
    ReadOnly Property Rows() As List(Of String())
    ReadOnly Property FileExists() As Boolean
    Function FirstLine() As String
    Function ReadAllRows(Optional SkipFirstLine As Boolean = True,
                         Optional Separator As String = ",",
                         Optional Textqualifier As String = """",
                         Optional DecimalPoint As String = ".",
                         Optional DecimalSeparator As String = ",") As Long
    Function StringToDecimal(ByVal NumberAsString As String) As Decimal
    Sub SetCsvLinePreprocessor(ByVal CsvLineProcessRoutine As CSVHelper.CsvLinePreprocessorDelegate)
    Sub SetCsvContentAnalyser(ByVal CsvContentAnalyseRoutine As CSVHelper.CsvContentAnalyserDelegate)
End Interface

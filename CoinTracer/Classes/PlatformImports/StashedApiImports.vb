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

Friend Structure ApiImportState
    Dim ImportID As Long
    Dim CurrencyTimestamp As Double
    Dim CurrencyCode As String
    Dim PageCounter As Integer
    Dim ImportData As List(Of Object)
End Structure


Friend NotInheritable Class StashedApiImports
    Private Shared _ApiImports As List(Of ApiImportState)

    ''' <summary>
    ''' Pops a stashed API import by the given ImportID
    ''' </summary>
    ''' <returns>The API import data corresponding to the ImportID, or Nothing, if there was no stashed API import</returns>
    Friend Shared Function PopApiImportByImportId(ByVal ImportID As Long) As ApiImportState?
        If _ApiImports IsNot Nothing Then
            For i As Integer = 0 To _ApiImports.Count - 1
                If _ApiImports(i).ImportID = ImportID Then
                    Dim ReturnImport As New ApiImportState With {.ImportID = ImportID,
                        .CurrencyCode = _ApiImports(i).CurrencyCode,
                        .ImportData = _ApiImports(i).ImportData,
                        .CurrencyTimestamp = _ApiImports(i).CurrencyTimestamp,
                        .PageCounter = _ApiImports(i).PageCounter}
                    _ApiImports.RemoveAt(i)
                    Return ReturnImport
                End If
            Next
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Pushes API import data onto the stack for stashed API imports
    ''' </summary>
    ''' <returns>The total number or stashed imports so far</returns>
    Friend Shared Function PushApiImport(ByRef ApiImport As ApiImportState) As Long
        If _ApiImports Is Nothing Then _ApiImports = New List(Of ApiImportState)
        _ApiImports.Add(New ApiImportState With {.ImportID = ApiImport.ImportID,
                            .ImportData = ApiImport.ImportData,
                            .CurrencyCode# = ApiImport.CurrencyCode,
                            .CurrencyTimestamp = ApiImport.CurrencyTimestamp,
                            .PageCounter = ApiImport.PageCounter})
        Return _ApiImports.Count
    End Function

End Class

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
''' Extended Path Functionality
''' </summary>
Public Class PathEx

    ''' <summary>
    ''' Returns the file name and extension of the given path/file expression. Invalid chars are replaced by the ReplacementChar.
    ''' </summary>
    ''' <param name="PathFileExpression">Fully qualified path and file name</param>
    ''' <param name="ReplacementChar">Char that will replace invalid chars, if there are any</param>
    Public Shared Function GetFileNameReplaced(ByVal PathFileExpression As String, _
                                                   Optional ByVal ReplacementChar As Char = " ") As String
        Dim Result As String = PathFileExpression
        If Result IsNot Nothing Then
            Dim InvalidChars() As Char = Path.GetInvalidFileNameChars
            For Each InvChar As Char In InvalidChars
                Result = Result.Replace(InvChar, ReplacementChar)
            Next
            Result = Path.GetFileName(Result)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Returns the file name and extension of the given path/file expression if possible. If the PathFileExpression contains invalid chars, simply the given string is returend.
    ''' </summary>
    ''' <param name="PathFileExpression">Fully qualified path and file name</param>
    Public Shared Function GetFileNameIfPossible(ByVal PathFileExpression As String) As String
        Dim Result As String = PathFileExpression
        If Result IsNot Nothing Then
            Dim InvalidChars() As Char = Path.GetInvalidPathChars
            Dim HasInvalidChars As Boolean = False
            For Each InvChar As Char In InvalidChars
                If Result.Contains(InvChar) Then
                    HasInvalidChars = True
                    Exit For
                End If
            Next
            If Not HasInvalidChars Then
                Result = Path.GetFileName(Result)
            End If
        End If
        Return Result
    End Function

End Class

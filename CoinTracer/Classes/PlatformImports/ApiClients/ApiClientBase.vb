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

Public MustInherit Class ApiClientBase

    Private _LastApiCallTimestamp As Long

    Private _CallDelay As Long
    ''' <summary>
    ''' Pause between two API calls in milliseconds
    ''' </summary>
    Friend Property CallDelay() As Long
        Get
            Return _CallDelay
        End Get
        Set(ByVal value As Long)
            _CallDelay = value
        End Set
    End Property

    ''' <summary>
    ''' Checks if there has been enough time between now and the latest API call and sends the thread to sleep if needed.
    ''' </summary>
    Friend Sub WaitForNextApiCall()
        If CallDelay > 0 Then
            Dim NowTS As Long
            NowTS = (Date.Now - New DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds
            If NowTS - _LastApiCallTimestamp < _CallDelay Then
                Thread.Sleep(_LastApiCallTimestamp + _CallDelay - NowTS)
            End If
            _LastApiCallTimestamp = (Date.Now - New DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds
        End If
    End Sub

End Class

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

Imports System.Net
Imports System.IO

''' <summary>
''' Einfaches Absetzen von Http-Requests unter Berücksichtigung der Applikations-Einstellungen
''' </summary>
''' <remarks></remarks>
Public Class UriRequest

    Private _URI As String
    Public Property URI() As String
        Get
            Return _URI
        End Get
        Set(ByVal value As String)
            _URI = value
        End Set
    End Property

    ''' <summary>
    ''' Bestimmt, ob für den Request ein Proxy verwendet werden soll. True, wenn die System-Proxyeinstellung verwendet werden soll (Achtung, langsam!). False, wenn Request ohne Proxy abgesetzt werden soll (deutlich schneller)
    ''' </summary>
    Public Property UseProxy() As String
        Get
            Return My.Settings.UseProxy
        End Get
        Set(ByVal value As String)
            My.Settings.UseProxy = value
        End Set
    End Property

    Public Sub New()
        ' Nothing to do...
    End Sub

    Public Sub New(URI As String)
        _URI = URI
    End Sub

    ''' <summary>
    ''' Führt den Request aus und liefert die Antwort als String zurück.
    ''' </summary>
    ''' <param name="URI">URI des Aufrufs</param>
    Public Function Request(Optional URI As String = "") As String
        Dim OldCursor As Cursor
        Dim wc As WebClient
        Dim strm As Stream
        Dim rdr As StreamReader
        Dim Received As String

        If URI <> "" Then
            Me.URI = URI
        End If
        OldCursor = Cursor.Current
        Try
            Cursor.Current = Cursors.WaitCursor
            wc = New WebClient
            If Me.UseProxy Then
                wc.Proxy = WebRequest.DefaultWebProxy
            Else
                wc.Proxy = Nothing
            End If
            Received = wc.DownloadString(Me.URI)
            wc.UseDefaultCredentials = True
            strm = wc.OpenRead(Me.URI)
            rdr = New StreamReader(strm)
            Received = rdr.ReadToEnd
            rdr.Close()
            strm.Close()
            Cursor.Current = Cursors.Default
        Catch ex As Exception
            Cursor.Current = OldCursor
            Throw
            Exit Function
        End Try
        Cursor.Current = OldCursor
        Return Received
    End Function

End Class

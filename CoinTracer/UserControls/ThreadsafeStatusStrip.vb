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

Public Class ThreadsafeStatusStrip

    ''' <summary>
    ''' Setzt den Text des Labels auf dem StatusStrip
    ''' </summary>
    ''' <param name="MessageText">Anzuzeigender Text. Wenn Leerstring, wird auch die ProgressBar ausgeblendet</param>
    Public Sub SetMessageText(ByVal MessageText As String)
        If InvokeRequired Then
            Invoke(New SetMessageTextDelegate(AddressOf DoSetMessageText), New Object() {MessageText})
        Else
            DoSetMessageText(MessageText)
        End If
    End Sub

    Private Delegate Sub SetMessageTextDelegate(ByVal MessageText As String)

    Private Sub DoSetMessageText(ByVal MessageText As String)
        ToolStripStatusLabel1.Text = MessageText
        If MessageText <> "" Then
            DoShowProgressBar()
        Else
            DoHideProgressBar()
        End If
        Me.Refresh()
    End Sub

    ''' <summary>
    ''' Blendet die ProgressBar ein
    ''' </summary>
    Public Sub ShowProgressBar()
        If InvokeRequired Then
            Invoke(New PlainSub(AddressOf DoShowProgressBar))
        Else
            DoShowProgressBar()
        End If
    End Sub

    Private Delegate Sub PlainSub()

    Private Sub DoShowProgressBar()
        With ToolStripProgressBar1
            ' Sieht umständlich aus, ist aber notwendig, um die Bar nicht jedes Mal zurückzusetzen
            If .Style <> ProgressBarStyle.Marquee Then
                .Style = ProgressBarStyle.Marquee
            End If
            If .MarqueeAnimationSpeed <> 100 Then
                .MarqueeAnimationSpeed = 100
            End If
            If Not .Visible Then
                .Visible = True
            End If
        End With
    End Sub

    ''' <summary>
    ''' Blendet die ProgressBar aus
    ''' </summary>
    Public Sub HideProgressBar()
        If InvokeRequired Then
            Invoke(New PlainSub(AddressOf DoHideProgressBar))
        Else
            DoHideProgressBar()
        End If
    End Sub

    Private Sub DoHideProgressBar()
        ToolStripProgressBar1.Visible = False
    End Sub

    ''' <summary>
    ''' Leert das Label und blendet die ProgressBar aus
    ''' </summary>
    Public Sub Clear()
        If InvokeRequired Then
            Invoke(New PlainSub(AddressOf DoClear))
        Else
            DoClear()
        End If
    End Sub

    Private Sub DoClear()
        SetMessageText("")
    End Sub

End Class

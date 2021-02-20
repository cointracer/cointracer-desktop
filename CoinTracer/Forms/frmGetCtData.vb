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

Public Class frmGetCtData

    Private _DialogResult As DialogResult
    Private _MessageBoxQualifier As String

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Process.Start("https://www.cointracer.de/?q=ct-format")
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub cmdOKNotAgain_Click(sender As Object, e As EventArgs) Handles cmdOKNotAgain.Click
        _DialogResult = DialogResult.OK
        MsgBoxEx.SetDefaultDialogResult(_MessageBoxQualifier, DialogResult.OK)
    End Sub

    Function ShowWithNotAgainOption(ByVal MessageBoxQualifier As String,
                                    ByVal DefaultResult As DialogResult) As DialogResult
        _MessageBoxQualifier = MessageBoxQualifier
        Dim Result As DialogResult = MsgBoxEx.GetDefaultDialogResult(MessageBoxQualifier)
        If Result <> DialogResult.None Then
            ' This message box has already been disabled, so return the default result 
            Return Result
        Else
            ' Nothing has been disabled so far: show this form
            cmdOKNotAgain.Text = MsgBoxEx.NotAgainButtonCaption(DefaultResult)
            cmdOK.Focus()
            Me.ShowDialog()
            Result = _DialogResult
        End If

        Return Result
    End Function

    Private Sub frmGetCtData_Load(sender As Object, e As EventArgs) Handles Me.Load
        _DialogResult = DialogResult.Cancel
        ' Find the beginning of the link within the label
        Dim ThisLinkArea As LinkArea
        ThisLinkArea.Length = 30
        ThisLinkArea.Start = LinkLabel1.Text.IndexOf("www.cointracer.de")
        LinkLabel1.LinkArea = ThisLinkArea
    End Sub

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        _DialogResult = DialogResult.OK
    End Sub

End Class

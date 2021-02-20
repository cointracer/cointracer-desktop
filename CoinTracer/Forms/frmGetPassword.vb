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

Public Class frmGetPassword

    Public ReadOnly Property Password() As String
        Get
            Return Me.txtPassword.Text
        End Get
    End Property

    Public Property Explanation() As String
        Get
            Return Me.lblDeclaration.Text
        End Get
        Set(value As String)
            Me.lblDeclaration.Text = value
        End Set
    End Property

    Private _CheckPasswordCriteria As Boolean = False
    Public Property CheckPasswordCriteria() As Boolean
        Get
            Return _CheckPasswordCriteria
        End Get
        Set(ByVal value As Boolean)
            _CheckPasswordCriteria = value
        End Set
    End Property


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        Me.Hide()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.txtPassword.Text = ""
    End Sub

    Private Sub txtPassword_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtPassword.Validating
        If Me.CheckPasswordCriteria Then
            If txtPassword.Text.Trim.Length < 4 Then
                MessageBox.Show("Das Passwort ist zu kurz. Bitte geben Sie ein längeres Passwort ein.", "Passwortprüfung", _
                                MessageBoxButtons.OK, MessageBoxIcon.Hand)
                e.Cancel = True
            End If
        End If
    End Sub
End Class

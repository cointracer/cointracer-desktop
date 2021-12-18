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

Imports System.ComponentModel

Public Class frmSelectPlatform

    Public Property Declaration() As String
        Get
            Return lblDeclaration.Text
        End Get
        Set(ByVal value As String)
            lblDeclaration.Text = value
        End Set
    End Property

    Private Sub frmFileImportSetTarget_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ImportPlattformenTableAdapter.Fill(CoinTracerDataSet.ImportPlattformen)
        If _Platform > 0 Then
            Try
                cbxPlatforms.SelectedValue = _Platform
            Catch ex As Exception
                ' proceed...
            End Try
        End If
    End Sub

    Private Sub cbxPlatforms_Validating(sender As Object, e As CancelEventArgs) Handles cbxPlatforms.Validating
        Platform = cbxPlatforms.SelectedValue
        PlatformName = cbxPlatforms.Text
    End Sub

    Private _Platform As Long
    Public Property Platform() As Long
        Get
            Return _Platform
        End Get
        Set(ByVal value As Long)
            _Platform = value
        End Set
    End Property

    Private _PlatformName As String
    Public Property PlatformName() As String
        Get
            Return _PlatformName
        End Get
        Set(ByVal value As String)
            _PlatformName = value
        End Set
    End Property

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        Hide()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Platform = -1
        Hide()
    End Sub

End Class
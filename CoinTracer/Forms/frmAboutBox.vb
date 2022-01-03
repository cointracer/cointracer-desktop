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

Public NotInheritable Class frmAboutBox

    Private Sub frmAboutBox_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' General information
        Text = String.Format("Info {0}", Application.ProductName)
        LabelProductName.Text = Application.ProductName
        LabelVersion.Text = String.Format("Version {0}", Application.ProductVersion)
        LabelCopyright.Text = "Copyright © " & Now.Year
        LabelCompanyName.Text = "Andreas Nebinger"
        TextBoxDescription.Text = My.Application.Info.Description & Environment.NewLine & Environment.NewLine & CompleteDisclaimer()
        ' Set link label
        Try
            For Each LinkData As String() In {({"NPOI", "https://github.com/nissl-lab/npoi"}),
                                                      ({"Json.NET", "https://www.newtonsoft.com/json"})}
                With LinkLabel1
                    .Links.Add(.Text.IndexOf(LinkData(0)), Len(LinkData(0)), LinkData(1))
                End With
            Next
        Catch ex As Exception
            ' doesn't matter...
        End Try
    End Sub


    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OKButton.Click
        Close()
    End Sub


    Private Sub cmdJsonNetLicense_Click(sender As Object, e As EventArgs) Handles cmdJsonNetLicense.Click, cmdApacheLicense.Click
        Dim Lic As New frmReleaseNotes
        With Lic
            Select Case sender.Name
                Case "cmdJsonNetLicense"
                    .FileName = "License_Json.NET.txt"
                    .Title = "Lizenz Json.NET"
                Case Else
                    .FileName = "License_Apache_v2.txt"
                    .Title = "Lizenz NPOI"
            End Select
            .ShowDialog(Me)
        End With
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Process.Start(e.Link.LinkData)
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub
End Class

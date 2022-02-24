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

Public Class frmCopyTableData

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' write current settings
        Dim SettingValue As String
        If rbCopyHeaders.Checked Then
            SettingValue = "1"
        Else
            SettingValue = "0"
        End If
        If rbSeparatorTab.Checked Then
            SettingValue &= "1"
        Else
            SettingValue &= "0"
        End If
        SettingValue &= cbxTextqualifier.SelectedIndex

        My.Settings.TableExportSettings = SettingValue
        ' hide me
        Me.Hide()
    End Sub

    Public ReadOnly Property Separator() As String
        Get
            If rbSeparatorSemicolon.Checked Then
                Return ";"
            Else
                Return Chr(Keys.Tab)
            End If
        End Get
    End Property

    Public ReadOnly Property TextQualifier() As String
        Get
            Select Case cbxTextqualifier.SelectedIndex
                Case 1
                    Return """"
                Case 2
                    Return "'"
                Case Else
                    Return ""
            End Select
        End Get
    End Property


    Public ReadOnly Property CopyHeaders() As Boolean
        Get
            Return rbCopyHeaders.Checked
        End Get
    End Property

    Private Sub frmCopyTableData_Load(sender As Object, e As EventArgs) Handles Me.Load
        rbCopyHeaders.Checked = (My.Settings.TableExportSettings = "" OrElse My.Settings.TableExportSettings.Substring(0, 1) = "1")
        rbCopyNoHeaders.Checked = Not rbCopyHeaders.Checked
        rbSeparatorTab.Checked = (My.Settings.TableExportSettings = "" OrElse My.Settings.TableExportSettings.Substring(1, 1) = "1")
        rbSeparatorSemicolon.Checked = Not rbSeparatorTab.Checked
        If My.Settings.TableExportSettings.Length < 3 Then
            cbxTextqualifier.SelectedIndex = 1
        Else
            cbxTextqualifier.SelectedIndex = CInt(My.Settings.TableExportSettings.Substring(2, 1))
        End If
    End Sub
End Class

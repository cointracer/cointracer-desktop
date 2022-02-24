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

Public Class frmEditKontenAliases

    Private _StartCode As String = String.Empty
    Public Property StartCode() As String
        Get
            Return _StartCode
        End Get
        Set(ByVal value As String)
            _StartCode = value
        End Set
    End Property

    Private _StartAlias As String = String.Empty
    Public Property StartAlias() As String
        Get
            Return _StartAlias
        End Get
        Set(ByVal value As String)
            _StartAlias = value
        End Set
    End Property

    Private _RecordsModified As Integer = 0
    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return _RecordsModified
        End Get
    End Property


    Private Sub frmEditKontenAliases_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        KontenAliasesTableAdapter.Fill(CoinTracerDataSet.KontenAliases)
        _RecordsModified = 0
        If StartCode <> String.Empty AndAlso StartAlias <> String.Empty Then
            ' find the row clicked by user
            For i = 0 To DataGridView1.RowCount - 1
                If DataGridView1.Rows(i).Cells(0).Value = StartCode AndAlso DataGridView1.Rows(i).Cells(1).Value = StartAlias Then
                    ' DataGridView1.Rows(i).Selected = True
                    DataGridView1.CurrentCell = DataGridView1.Rows(i).Cells(0)
                    Exit For
                End If
            Next
        End If
    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' Änderungen speichern
        Validate()
        KontenAliasesBindingSource.EndEdit()
        TableAdapterManager.UpdateAll(CoinTracerDataSet)
        Close()
    End Sub

    Private Sub DataGridView1_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DataGridView1.RowValidating
        ' check for duplicate entries
        With DirectCast(sender, DataGridView)
            If e.RowIndex < .RowCount AndAlso .Rows(e.RowIndex).Cells(0)?.Value IsNot Nothing AndAlso .Rows(e.RowIndex).Cells(1)?.Value IsNot Nothing Then
                For i = 0 To .RowCount - 1
                    If i <> e.RowIndex Then
                        If .Rows(i).Cells(0).Value = .Rows(e.RowIndex).Cells(0).Value AndAlso .Rows(i).Cells(1).Value = .Rows(e.RowIndex).Cells(1).Value Then
                            MessageBox.Show(String.Format(My.Resources.MyStrings.editAccountAliasesRowErrorDuplicateEntry,
                        .Rows(i).Cells(0).Value, .Rows(i).Cells(1).Value),
                        My.Resources.MyStrings.editAccountAliasesRowErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
                            e.Cancel = True
                            Exit Sub
                        End If
                    End If
                Next
            End If
        End With
    End Sub

End Class

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

Imports CoinTracer.CoinTracerDataSetTableAdapters

Public Class frmEditCourses

    Private _StartID As Long = -1
    Public Property StartID() As Long
        Get
            Return _StartID
        End Get
        Set(ByVal value As Long)
            _StartID = value
        End Set
    End Property

    Private _RecordsModified As Integer = 0
    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return _RecordsModified
        End Get
    End Property


    Private Sub frmEditCourses_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KontenTableAdapter.Fill(Me.CoinTracerDataSet.Konten)
        Me.KurseTableAdapter.Fill(Me.CoinTracerDataSet.Kurse)

        Dim KtTA As New KontenTableAdapter
        KtTA.FillBySQL(Me.CoinTracerDataSet.Konten, "where IstFiat=1 and IstGebuehr = 0")
        KtTA.Dispose()

        ' QuellKontoComboBox
        KurseKontenQuellBindingSource.DataSource = Me.CoinTracerDataSet
        KurseKontenQuellBindingSource.DataMember = "Konten"
        Dim b As New System.Windows.Forms.Binding("SelectedValue", _
             Me.KurseBindingSource, "QuellKontoID", True)
        QuellKontoComboBox.DataBindings.Add(b)

        ' ZielKontoComboBox
        KurseKontenZielBindingSource.DataSource = Me.CoinTracerDataSet
        KurseKontenZielBindingSource.DataMember = "Konten"
        b = New System.Windows.Forms.Binding("SelectedValue", _
             Me.KurseBindingSource, "ZielKontoID", True)
        ZielKontoComboBox.DataBindings.Add(b)

        _RecordsModified = 0
        If _StartID >= 0 Then
            Me.KurseBindingSource.Position = Me.KurseBindingSource.Find("ID", StartID)
        Else
            KurseBindingSource_CurrentChanged(Nothing, Nothing)
        End If

    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' Änderungen speichern
        UpdateData(Me.KurseTableAdapter)
        ' und schließen
        Me.Close()
    End Sub

    Private Sub KurseBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        UpdateData(Me.KurseTableAdapter)
    End Sub

    Private Sub UpdateData(ByRef TbA As CoinTracerDataSetTableAdapters.KurseTableAdapter)
        Try
            Me.KurseBindingSource.EndEdit()
            If Me.CoinTracerDataSet.HasChanges Then
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Deleted) IsNot Nothing Then
                    _RecordsModified = Me.KurseTableAdapter.Update(Me.CoinTracerDataSet.GetChanges(DataRowState.Deleted))
                End If
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Added) IsNot Nothing Then
                    _RecordsModified += Me.KurseTableAdapter.Update(Me.CoinTracerDataSet.GetChanges(DataRowState.Added))
                End If
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Modified) IsNot Nothing Then
                    _RecordsModified += Me.KurseTableAdapter.Update(Me.CoinTracerDataSet.GetChanges(DataRowState.Modified))
                End If
                ' Info an User
                MessageBox.Show("Ihre Änderungen wurden gespeichert.", "Kurse bearbeiten", _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, "Fehler beim Speichern der Daten: " & ex.Message)
        End Try
    End Sub


    Private Sub KurseBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles KurseBindingSource.CurrentChanged
        Dim SomeSelected As Boolean = Me.KurseBindingSource.Position >= 0
        IDTextBox.ReadOnly = True
        BindingNavigatorDeleteItem.Enabled = SomeSelected
        ZeitpunktDateTimePicker.Enabled = SomeSelected
        QuellBetragTextBox.ReadOnly = Not SomeSelected
        ZielBetragTextBox.ReadOnly = Not SomeSelected
        CalculatedCheckBox.Enabled = SomeSelected
        QuellKontoComboBox.Enabled = SomeSelected
        ZielKontoComboBox.Enabled = SomeSelected
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
        Try
            ' Warum auch immer: Validierung passiert nicht automatisch?!
            Dim ValRes As System.ComponentModel.CancelEventArgs = New System.ComponentModel.CancelEventArgs(False)
            pnlDetails_Validating(Nothing, ValRes)
            If Not ValRes.Cancel Then
                Dim DB As New DBHelper(frmMain.Connection)
                Dim NewID As Long = DB.GetMaxID(DBHelper.TableNames.Kurse) + 1
                Dim TmpDv As DataView = New DataView(Me.KurseBindingSource.DataSource.Tables("Kurse"), _
                                                     "ID >= " & NewID, _
                                                     "ID desc", DataViewRowState.CurrentRows)
                If TmpDv.Count > 0 Then
                    NewID = TmpDv(0)("ID") + 1
                End If
                Dim NewRow As Object = Me.KurseBindingSource.AddNew()
                With NewRow.Row
                    .ID = NewID
                    .Zeitpunkt = Today
                    .Quellbetrag = 1
                    .QuellKontoID = CInt(DBHelper.Konten.EUR)
                    .Calculated = False
                End With
                Me.KurseBindingSource.EndEdit()
                Me.KurseBindingSource.Position = Me.KurseBindingSource.Find("ID", NewID)
                KurseBindingSource_CurrentChanged(Nothing, Nothing)
                ZielBetragTextBox.Focus()
            End If

        Catch ex As Exception
            DefaultErrorHandler(ex, "Beim Anlegen des neuen Datensatzes ist ein Fehler aufgetreten: " & ex.Message)
        End Try
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click
        If MessageBox.Show("Möchten Sie diesen Datensatz wirklich löschen?", "Eintrag löschen", _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
            Me.KurseBindingSource.RemoveCurrent()
        End If
    End Sub

    Private Sub pnlDetails_Validated(sender As Object, e As EventArgs) Handles pnlDetails.Validated
        ErrProvider.Clear()
    End Sub

    Private Sub pnlDetails_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles pnlDetails.Validating
        Dim Ctrl As Control = Nothing
        Dim ErrorText As String = ""

        If Me.KurseBindingSource.Count > 0 Then
            If QuellKontoComboBox.SelectedIndex < 0 Then
                Ctrl = QuellKontoComboBox
                ErrorText = "Bitte wählen Sie ein gültiges Konto aus!"
            ElseIf ZielKontoComboBox.SelectedIndex < 0 Then
                Ctrl = ZielKontoComboBox
                ErrorText = "Bitte wählen Sie ein gültiges Konto aus!"
            ElseIf ZielKontoComboBox.SelectedIndex = QuellKontoComboBox.SelectedIndex AndAlso QuellKontoComboBox.SelectedIndex >= 0 Then
                Ctrl = ZielKontoComboBox
                ErrorText = "Quell- und Zielkonto dürfen nicht identisch sein!"
            ElseIf Not IsNumeric(QuellBetragTextBox.Text) OrElse CDec(QuellBetragTextBox.Text) <= 0 Then
                Ctrl = QuellBetragTextBox
                ErrorText = "Bitte geben Sie einen gültigen Betrag ein!"
            ElseIf Not IsNumeric(ZielBetragTextBox.Text) OrElse CDec(ZielBetragTextBox.Text) <= 0 Then
                Ctrl = ZielBetragTextBox
                ErrorText = "Bitte geben Sie einen gültigen Betrag ein!"
            End If

            If Ctrl IsNot Nothing Then
                ErrProvider.Clear()
                e.Cancel = True
                ErrProvider.SetError(Ctrl, ErrorText)
            End If
        End If

    End Sub


    Private Sub cmdCancel_MouseEnter(sender As Object, e As EventArgs) Handles cmdCancel.MouseEnter
        pnlDetails.CausesValidation = False
    End Sub

    Private Sub cmdCancel_MouseLeave(sender As Object, e As EventArgs) Handles cmdCancel.MouseLeave
        pnlDetails.CausesValidation = True
    End Sub

End Class

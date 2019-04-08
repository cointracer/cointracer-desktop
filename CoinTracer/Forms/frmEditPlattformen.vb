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

Imports CoinTracer.CoinTracerDataSet

Public Class frmEditPlattformen

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


    Private Sub frmEditPlattformen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.PlattformenTableAdapter.Fill(Me.CoinTracerDataSet.Plattformen)
        _RecordsModified = 0
        If _StartID >= 0 Then
            Me.PlattformenBindingSource.Position = Me.PlattformenBindingSource.Find("ID", StartID)
        Else
            PlattformenBindingSource_CurrentChanged(Nothing, Nothing)
        End If
    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' Änderungen speichern
        UpdateData(Me.PlattformenTableAdapter)
        ' und schließen
        Me.Close()
    End Sub

    Private Sub PlattformenBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        UpdateData(Me.PlattformenTableAdapter)
    End Sub

    Private Sub UpdateData(ByRef TbA As CoinTracerDataSetTableAdapters.PlattformenTableAdapter)
        Try
            Me.PlattformenBindingSource.EndEdit()
            If Me.CoinTracerDataSet.HasChanges Then
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Deleted) IsNot Nothing Then
                    Dim TmpDS As CoinTracerDataSet = Me.CoinTracerDataSet.GetChanges(DataRowState.Deleted)
                    For Each Row As PlattformenRow In TmpDS.Plattformen.Rows
                        Try
                            If Row("IstDown", DataRowVersion.Original) Then
                                ' Verlust-Trades löschen
                                frmMain.TradeValueManager.SetLossTrades(TradeValueManager.LossTradeActionTypes.Delete, Row("ID", DataRowVersion.Original))
                            End If
                        Catch ex As Exception
                            ' no problem
                        End Try
                    Next
                    _RecordsModified = Me.PlattformenTableAdapter.Update(TmpDS)
                End If
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Added) IsNot Nothing Then
                    Dim TmpDS As CoinTracerDataSet = Me.CoinTracerDataSet.GetChanges(DataRowState.Added)
                    For Each Row As PlattformenRow In TmpDS.Plattformen.Rows
                        If Row.IstDown Then
                            ' Verlust-Trades hinzufügen
                            frmMain.TradeValueManager.SetLossTrades(TradeValueManager.LossTradeActionTypes.Add, Row.ID, Row.DownSeit)
                        End If
                    Next
                    _RecordsModified += Me.PlattformenTableAdapter.Update(TmpDS)
                End If
                If Me.CoinTracerDataSet.GetChanges(DataRowState.Modified) IsNot Nothing Then
                    Dim TmpDS As CoinTracerDataSet = Me.CoinTracerDataSet.GetChanges(DataRowState.Modified)
                    For Each Row As PlattformenRow In TmpDS.Plattformen.Rows
                        ' Verlust-Trades auf jeden Fall löschen
                        frmMain.TradeValueManager.SetLossTrades(TradeValueManager.LossTradeActionTypes.Delete, Row.ID)
                        If Row.IstDown Then
                            ' Verlust-Trades bei Bedarf hinzufügen
                            frmMain.TradeValueManager.SetLossTrades(TradeValueManager.LossTradeActionTypes.Add, Row.ID, Row.DownSeit)
                        End If
                    Next
                    _RecordsModified += Me.PlattformenTableAdapter.Update(TmpDS)
                End If
                ' Info an User
                MessageBox.Show("Ihre Änderungen wurden gespeichert.", "Plattformen bearbeiten", _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, "Fehler beim Speichern der Daten: " & ex.Message)
        End Try
    End Sub


    Private Sub PlattformenBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles PlattformenBindingSource.CurrentChanged
        Dim SomeSelected As Boolean = Me.PlattformenBindingSource.Position >= 0
        If SomeSelected Then
            Dim CurRow As PlattformenRow = Me.PlattformenBindingSource.Current.Row
            Try
                BezeichnungTextBox.ReadOnly = SomeSelected AndAlso CurRow.Fix AndAlso Not CurRow.IsFixNull
            Catch ex As Exception
                BezeichnungTextBox.ReadOnly = False
            End Try
        Else
            BezeichnungTextBox.ReadOnly = True
        End If
        IDTextBox.ReadOnly = BezeichnungTextBox.ReadOnly
        CodeTextBox.ReadOnly = BezeichnungTextBox.ReadOnly
        BeschreibungTextBox.ReadOnly = Not SomeSelected
        SortIDTextBox.ReadOnly = Not SomeSelected
        BoerseCheckBox.Enabled = Not BezeichnungTextBox.ReadOnly
        IstDownCheckBox.Enabled = BoerseCheckBox.Checked
        EigenCheckBox.Enabled = True
        FixCheckBox.Enabled = False
        BindingNavigatorDeleteItem.Enabled = Not BezeichnungTextBox.ReadOnly
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
        Try
            Dim NewID As Long
            Dim TmpDv As DataView = New DataView(Me.CoinTracerDataSet.Tables("Plattformen"), "ID < 900", "ID desc", DataViewRowState.CurrentRows)
            If TmpDv.Count > 0 Then
                NewID = TmpDv(0)("ID").ToString + 1
            Else
                NewID = 1
            End If
            Dim NewRow As Object = Me.PlattformenBindingSource.AddNew()
            With NewRow.Row
                .ID = NewID
                .SortID = NewID
                .Fix = False
                .Eigen = True
                .Boerse = True
                .IstDown = False
            End With
            Me.PlattformenBindingSource.EndEdit()
            Me.PlattformenBindingSource.Position = Me.PlattformenBindingSource.Find("ID", NewID)
            PlattformenBindingSource_CurrentChanged(Nothing, Nothing)
        Catch ex As Exception
            DefaultErrorHandler(ex, "Beim Anlegen des neuen Datensatzes ist ein Fehler aufgetreten: " & ex.Message)
        End Try
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click
        If MessageBox.Show("Möchten Sie diesen Datensatz wirklich löschen?", "Eintrag löschen", _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
            Me.PlattformenBindingSource.RemoveCurrent()
        End If
    End Sub

    Private Sub IstDownCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles IstDownCheckBox.CheckedChanged
        DownSeitDateTimePicker.Visible = IstDownCheckBox.Checked
        DownSeitLabel.Visible = IstDownCheckBox.Checked
        If IstDownCheckBox.Checked Then
            EigenCheckBox.Checked = True
        End If
    End Sub


    Private Sub pnlDownSeit_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles pnlDownSeit.Validating
        If IstDownCheckBox.Checked Then
            ' Sicherstellen, dass nur der Datumsanteil gespeichert wird und dass DBNull-Werte korrekt behandelt werden
            Try
                DownSeitDateTimePicker.Value = DownSeitDateTimePicker.Value.Date
            Catch ex As StrongTypingException
                DownSeitDateTimePicker.Value = Today.Date
            Catch ex As Exception
                Throw
            End Try
        End If
    End Sub

    Private Sub EigenCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles EigenCheckBox.CheckedChanged
        If Not EigenCheckBox.Checked Then
            IstDownCheckBox.Checked = False
        End If
    End Sub


End Class

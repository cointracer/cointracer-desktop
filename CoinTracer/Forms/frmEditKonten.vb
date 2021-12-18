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

Imports CoinTracer.CoinTracerDataSet

Public Class frmEditKonten

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


    Private Sub frmEditKonten_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        KontenTableAdapter.Fill(CoinTracerDataSet.Konten)
        _RecordsModified = 0
        If _StartID >= 0 Then
            KontenBindingSource.Position = KontenBindingSource.Find("ID", StartID)
        Else
            KontenBindingSource_CurrentChanged(Nothing, Nothing)
        End If
    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' Änderungen speichern
        UpdateData(KontenTableAdapter)
        ' und schließen
        Close()
    End Sub

    Private Sub KontenBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Validate()
        UpdateData(KontenTableAdapter)
    End Sub

    Private Sub UpdateData(ByRef TbA As CoinTracerDataSetTableAdapters.KontenTableAdapter)
        Try
            KontenBindingSource.EndEdit()
            If CoinTracerDataSet.HasChanges Then
                If CoinTracerDataSet.GetChanges(DataRowState.Deleted) IsNot Nothing Then
                    _RecordsModified = KontenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Deleted))
                End If
                If CoinTracerDataSet.GetChanges(DataRowState.Added) IsNot Nothing Then
                    _RecordsModified += KontenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Added))
                End If
                If CoinTracerDataSet.GetChanges(DataRowState.Modified) IsNot Nothing Then
                    _RecordsModified += KontenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Modified))
                End If
                ' Info an User
                MessageBox.Show(My.Resources.MyStrings.editMsgSaved, My.Resources.MyStrings.editAccountMsgTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, My.Resources.MyStrings.editMsgSaveError & " " & ex.Message)
        End Try
    End Sub


    Private Sub KontenBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles KontenBindingSource.CurrentChanged
        Dim SomeSelected As Boolean = KontenBindingSource.Position >= 0
        If SomeSelected Then
            Dim CurRow As KontenRow = KontenBindingSource.Current.Row
            Try
                BezeichnungTextBox.ReadOnly = SomeSelected AndAlso CurRow.Fix AndAlso Not CurRow.IsIstFiatNull
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
        GebuehrKontoIDTextBox.ReadOnly = BezeichnungTextBox.ReadOnly
        IstFiatCheckBox.Enabled = False
        IstGebuehrCheckBox.Enabled = Not BezeichnungTextBox.ReadOnly
        EigenCheckBox.Enabled = IstGebuehrCheckBox.Enabled
        FixCheckBox.Enabled = False
        BindingNavigatorDeleteItem.Enabled = Not BezeichnungTextBox.ReadOnly
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
        Try
            'Prüfen, ob es für den aktuellen Datensatz schon ein Gebührenkonto gibt. Wenn nicht, nachfragen, ob eines angelegt werden soll.
            Dim MsgRes As DialogResult
            Dim CurRow As KontenRow = Nothing
            If Val(GebuehrKontoIDTextBox.Text) = 0 AndAlso CodeTextBox.Text.Length > 0 AndAlso Not CodeTextBox.Text.ToUpper.StartsWith("FEE") Then
                MsgRes = MessageBox.Show(My.Resources.MyStrings.editAccountMsgMakeFee,
                                         BindingNavigatorAddNewItem.Text, MessageBoxButtons.YesNoCancel)
                If MsgRes = Windows.Forms.DialogResult.Cancel Then
                    Exit Sub
                ElseIf MsgRes = Windows.Forms.DialogResult.Yes Then
                    ' Aktuelle Daten für Gebühren-Datensatz merken
                    CurRow = KontenBindingSource.Current.Row
                End If
            End If

            Dim NewID As Long
            Dim TmpDv As DataView = New DataView(CoinTracerDataSet.Tables("Konten"),
                                                 If(MsgRes = Windows.Forms.DialogResult.Yes, "", "ID < " & AccountManager.FEEMINACCOUNT),
                                                 "ID desc", DataViewRowState.CurrentRows)
            If TmpDv.Count > 0 Then
                NewID = TmpDv(0)("ID") + 1
            Else
                TmpDv = New DataView(CoinTracerDataSet.Tables("Konten"), "", "ID desc", DataViewRowState.CurrentRows)
                If TmpDv.Count > 0 Then
                Else
                    NewID = NewID = TmpDv(0)("ID") + 1
                End If
                NewID = 1
            End If
            If MsgRes = Windows.Forms.DialogResult.Yes Then
                KontenBindingSource.Current.Row.GebuehrKontoID = NewID
            End If
            Dim NewRow As Object = KontenBindingSource.AddNew()
            With NewRow.Row
                .ID = NewID
                .SortID = NewID
                .Fix = False
                .IstFiat = False
                If MsgRes = Windows.Forms.DialogResult.Yes Then
                    ' neuer Gebühren-Datensatz
                    .Eigen = False
                    .IstGebuehr = True
                    .Code = "fee" & CurRow.Code
                    .Bezeichnung = My.Resources.MyStrings.feeSingular & " " & CurRow.Bezeichnung
                    .Beschreibung = My.Resources.MyStrings.feePlural & "/" & CurRow.Beschreibung
                Else
                    ' "normaler" neuer Datensatz
                    .Eigen = True
                    .IstGebuehr = False
                End If
            End With
            KontenBindingSource.EndEdit()
            KontenBindingSource.Position = KontenBindingSource.Find("ID", NewID)
            KontenBindingSource_CurrentChanged(Nothing, Nothing)
        Catch ex As Exception
            DefaultErrorHandler(ex, My.Resources.MyStrings.editMsgNewError & " " & ex.Message)
        End Try
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click
        Dim GebKonto As Long
        Try
            If Val(KontenBindingSource.Current.Row.GebuehrKontoID) > 0 Then
                GebKonto = KontenBindingSource.Current.Row.GebuehrKontoID
                If MessageBox.Show(My.Resources.MyStrings.editAccountMsgDeleteFee, My.Resources.MyStrings.editMsgDeleteTitle,
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    KontenBindingSource.RemoveCurrent()
                    GebKonto = KontenBindingSource.Find("ID", GebKonto)
                    If GebKonto >= 0 Then
                        KontenBindingSource.RemoveAt(GebKonto)
                    End If
                    KontenBindingSource.EndEdit()
                End If
            ElseIf MessageBox.Show(My.Resources.MyStrings.editMsgDelete, My.Resources.MyStrings.editMsgDeleteTitle,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                KontenBindingSource.RemoveCurrent()
                KontenBindingSource.EndEdit()
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, My.Resources.MyStrings.editMsgDeleteError & " " & ex.Message)
        End Try
    End Sub

End Class

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
Imports CoinTracer.CoinTracerDataSetTableAdapters

Public Class frmEditApiData

    Private Const APIMINDATE As Date = #1/1/2009#

    Private _StartID As Long = -1
    Public Property StartID() As Long
        Get
            Return _StartID
        End Get
        Set(ByVal value As Long)
            _StartID = value
        End Set
    End Property

    Private _TargetTime As Date = DATENULLVALUE
    Public Property TargetTime() As Date
        Get
            Return _TargetTime
        End Get
        Set(ByVal value As Date)
            _TargetTime = value
        End Set
    End Property

    Private _RecordsModified As Integer = 0
    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return _RecordsModified
        End Get
    End Property

    Private _ApiPassword As String
    Private _ApiPwCheck As String
    Private _Crypt As PushPull

    Private _OriginalCurrencies As String
    Private _ApiLabelTouched As Boolean
    Private _NewRows As List(Of Long)


    ''' <summary>
    ''' Prüft, ob ein Passwortschutz vorliegt und fragt dieses ggf. ab. Kann bei Bedarf auch den Passwortschutz zurücksetzen.
    ''' </summary>
    ''' <returns>True, wenn alles gut ist; False bei falschem Passwort oder Abbruch durch den Benutzer</returns>
    ''' <remarks></remarks>
    Public Function ProcessPasswordProtection() As Boolean

        ' Tabellen füllen (wg. Abfrage der Anzahl unten)
        PlattformenTableAdapter.Fill(CoinTracerDataSet.Plattformen)
        ApiDatenTableAdapter.Fill(CoinTracerDataSet.ApiDaten)

        Dim Import As New Import(frmMain.CointracerDatabase)
        Dim DlgRes As DialogResult
        Dim DoLoop As Boolean = False
        Do
            If Not Import.CheckApiPassword() Then
                ' Nach Passwort fragen
                If Import.RequestApiPassword(_ApiPassword) <> Windows.Forms.DialogResult.OK Then
                    DoLoop = False
                    ' Fragen, ob der Passwort-Schutz aufgehoben werden soll
                    MsgBoxEx.PatchMsgBox(New String() {My.Resources.MyStrings.apiDataMsgResetPasswordProtectionButton1, My.Resources.MyStrings.Cancel})
                    If MessageBox.Show(String.Format(My.Resources.MyStrings.apiDataMsgResetPasswordProtection, Environment.NewLine),
                                       My.Resources.MyStrings.apiDataMsgResetPasswordProtectionTitle,
                                       MessageBoxButtons.RetryCancel,
                                       MessageBoxIcon.Exclamation,
                                       MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Retry Then
                        ' Passwortschutz zurücksetzen
                        frmMain.CointracerDatabase.ExecuteSQL("delete from ApiDaten")
                        My.Settings.AskForApiProtection = True
                        MessageBox.Show(My.Resources.MyStrings.apiDataMsgResetPasswordProtectionDone,
                                        My.Resources.MyStrings.apiDataMsgResetPasswordProtectionDoneTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return False
                    Else
                        MessageBox.Show(My.Resources.MyStrings.apiDataMsgResetPasswordProtectionLeft,
                                        My.Resources.MyStrings.apiDataMsgResetPasswordProtectionLeftTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return False
                    End If
                Else
                    ' Prüfen, ob das Passwort stimmt!
                    If Not Import.CheckApiPassword(_ApiPassword) Then
                        MessageBox.Show(My.Resources.MyStrings.apiDataMsgResetPasswordProtectionLeft,
                                        My.Resources.MyStrings.apiDataMsgResetPasswordProtectionLeftTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        DoLoop = True
                    Else
                        DoLoop = False
                    End If
                End If
            Else
                If My.Settings.AskForApiProtection Or ApiDatenBindingSource.Count = 0 Then
                    ' Fragen, ob ein Passwort verwendet werden soll
                    DlgRes = MessageBox.Show(String.Format(My.Resources.MyStrings.apiDataMsgAskPasswordProtection, Environment.NewLine),
                                             My.Resources.MyStrings.apiDataMsgAskPasswordProtectionTitle,
                                             MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                    Select Case DlgRes
                        Case DialogResult.Cancel
                            Return False
                        Case DialogResult.No
                            _ApiPassword = Import.DefaultApiPassword
                        Case Else
                            ' API-Passwort festlegen
                            _ApiPassword = Import.GetNewApiPassword()
                            If _ApiPassword = "" Then
                                Return False
                            Else
                                frmMain.CointracerDatabase.ExecuteSQL("delete from ApiDaten")
                            End If
                    End Select
                    My.Settings.AskForApiProtection = False
                Else
                    _ApiPassword = Import.DefaultApiPassword
                End If
            End If
        Loop While DoLoop

        _Crypt = New PushPull(_ApiPassword)
        _ApiPwCheck = Import.ApiPasswordCheckPhrase

        Return True

    End Function

    Private Sub frmEditApiData_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim PlattformenTA As New PlattformenTableAdapter
        PlattformenTA.FillByApiImports(CoinTracerDataSet.Plattformen)   ' TODO: Remove the binance part when it's time
        PlattformenTA.Dispose()

        _NewRows = New List(Of Long)

        ' PlattformenComboBox
        PlattformenBindingSource.DataSource = CoinTracerDataSet
        PlattformenBindingSource.DataMember = "Plattformen"
        Dim b As New Binding("SelectedValue",
             ApiDatenBindingSource, "PlattformID", True)
        PlattformIDComboBox.DataBindings.Add(b)

        _RecordsModified = 0
        If _StartID >= 0 Then
            ApiDatenBindingSource.Position = ApiDatenBindingSource.Find("ID", StartID)
            If _TargetTime <> DATENULLVALUE Then
                LastImportTimestampDateTimePicker.Value = _TargetTime
                LastImportTimestampDateTimePicker.Focus()
            End If
        Else
            ApiDatenBindingSource_CurrentChanged(Nothing, Nothing)
        End If

    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        ' Änderungen speichern
        UpdateData(ApiDatenTableAdapter)
        ' und schließen
        Close()
    End Sub

    Private Sub UpdateData(ByRef TbA As ApiDatenTableAdapter)
        Try
            ApiDatenBindingSource.EndEdit()
            If CoinTracerDataSet.HasChanges Then
                If CoinTracerDataSet.GetChanges(DataRowState.Deleted) IsNot Nothing Then
                    _RecordsModified = ApiDatenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Deleted))
                End If
                If CoinTracerDataSet.GetChanges(DataRowState.Added) IsNot Nothing Then
                    _RecordsModified += ApiDatenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Added))
                End If
                If CoinTracerDataSet.GetChanges(DataRowState.Modified) IsNot Nothing Then
                    _RecordsModified += ApiDatenTableAdapter.Update(CoinTracerDataSet.GetChanges(DataRowState.Modified))
                End If
                ' Info an User
                MessageBox.Show("Ihre Änderungen wurden gespeichert.", "API-Daten bearbeiten",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, "Fehler beim Speichern der Daten: " & ex.Message)
        End Try
    End Sub


    Private Sub ApiDatenBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles ApiDatenBindingSource.CurrentChanged
        Dim SomeSelected As Boolean = ApiDatenBindingSource.Position >= 0
        IDTextBox.ReadOnly = True
        BezeichnungTextBox.ReadOnly = Not SomeSelected
        ZeitpunktTextBox.ReadOnly = True
        ApiKeyDecryptedTextBox.ReadOnly = Not SomeSelected
        ApiKeyTextBox.ReadOnly = Not SomeSelected
        ApiSecretTextBox.ReadOnly = Not SomeSelected
        ApiSecretDecryptedTextBox.ReadOnly = Not SomeSelected
        AktivCheckBox.Enabled = SomeSelected
        PlattformIDComboBox.Enabled = SomeSelected
        CallDelayNumericUpDown.ReadOnly = Not SomeSelected
        ccbBitfinexCurrencies.Enabled = SomeSelected
        LastImportTimestampDateTimePicker.Enabled = SomeSelected
        BindingNavigatorDeleteItem.Enabled = SomeSelected
        CallDelayNumericUpDown.Touched = False
        _ApiLabelTouched = False
        ' ExtendedInfo auswerten
        _OriginalCurrencies = ExtendedInfoTextBox.Text
        PlattformIDComboBox_SelectedIndexChanged(Nothing, Nothing)
    End Sub


    ''' <summary>
    ''' Reloads the currency combobox with the currencies belonging to the given platform. Also sets the check marks according to the current ExtendedInfo
    ''' </summary>
    ''' <param name="PlatformID">ID of the platform whose currencies shall be displayed</param>
    Private Sub ReloadCurrencyCombobox(PlatformID As PlatformManager.Platforms)
        If PlatformID = 0 OrElse ApiDatenBindingSource.Current Is Nothing Then
            ccbBitfinexCurrencies.Items.Clear()
        Else
            Dim TempAccInfo As AccountInfo
            Dim CurRow As ApiDatenRow = ApiDatenBindingSource.Current.Row
            With ccbBitfinexCurrencies
                .Items.Clear()
                Select Case PlatformID
                    Case PlatformManager.Platforms.Bitfinex
                        If _OriginalCurrencies = "" Then
                            TempAccInfo = New BitfinexClient.BitfinexAccountInfo()
                        Else
                            TempAccInfo = New BitfinexClient.BitfinexAccountInfo(_OriginalCurrencies)
                        End If
                    Case PlatformManager.Platforms.BitcoinDe
                        If _OriginalCurrencies = "" Then
                            TempAccInfo = New BitcoinDeClient.BitcoinDeAccountInfo()
                        Else
                            TempAccInfo = New BitcoinDeClient.BitcoinDeAccountInfo(_OriginalCurrencies)
                        End If
                    Case Else
                        TempAccInfo = Nothing
                End Select
                If TempAccInfo IsNot Nothing Then
                    Dim CheckedCurrencies As String = TempAccInfo.GetCurrencyInfo
                    For i As Integer = 0 To TempAccInfo.Currencies.Length - 1
                        .Items.Add(New With {Key .Text = TempAccInfo.Currencies(i).Longname, Key .Value = TempAccInfo.Currencies(i).Shortname})
                        .SetItemChecked(i, (CheckedCurrencies.Contains(TempAccInfo.Currencies(i).Shortname)))
                    Next
                End If
            End With
        End If
    End Sub


    Private Sub BindingNavigatorAddNewItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
        Try
            ' Warum auch immer: Validierung passiert nicht automatisch?!
            Dim ValRes As ComponentModel.CancelEventArgs = New System.ComponentModel.CancelEventArgs(False)
            pnlDetails_Validating(Nothing, ValRes)
            If Not ValRes.Cancel Then
                Dim DB As New DBHelper(frmMain.Connection)
                ' Nächste ID holen
                Dim NewID As Long = DB.GetMaxID(DBHelper.TableNames.ApiDaten) + 1
                Dim TmpDv As DataView = New DataView(ApiDatenBindingSource.DataSource.Tables("ApiDaten"),
                                                     "ID >= " & NewID,
                                                     "ID desc", DataViewRowState.CurrentRows)
                If TmpDv.Count > 0 Then
                    NewID = TmpDv(0)("ID") + 1
                End If
                Dim NewRow As Object = ApiDatenBindingSource.AddNew()
                With NewRow.Row
                    .ID = NewID
                    .Zeitpunkt = Now
                    .PlattformID = CInt(PlatformManager.Platforms.BitcoinDe)
                    .LastImportTimestamp = 0
                    .Aktiv = True
                    .Salt = _Crypt.EncryptData(.ID & _ApiPwCheck)
                    .CallDelay = 0
                End With
                ' add this row to the list of new rows
                _NewRows.Add(ApiDatenBindingSource.Position)
                ' save & proceed
                ApiDatenBindingSource.EndEdit()
                ApiDatenBindingSource.MoveLast()
                ApiDatenBindingSource_CurrentChanged(Nothing, Nothing)
                ApiKeyDecryptedTextBox.Text = ""
                ApiSecretDecryptedTextBox.Text = ""
                BezeichnungTextBox.Focus()
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, "Beim Anlegen des neuen Datensatzes ist ein Fehler aufgetreten: " & ex.Message)
        End Try
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click
        If MessageBox.Show("Möchten Sie diesen Datensatz wirklich löschen?", "Eintrag löschen",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
            ApiDatenBindingSource.RemoveCurrent()
            ErrProvider.Clear()
        End If
    End Sub

    Private Sub pnlDetails_Validated(sender As Object, e As EventArgs) Handles pnlDetails.Validated
        ErrProvider.Clear()
    End Sub

    Private Sub pnlDetails_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles pnlDetails.Validating
        Dim Ctrl As Control = Nothing
        Dim ErrorText As String = ""

        If ApiDatenBindingSource.Count > 0 Then
            If PlattformIDComboBox.SelectedIndex < 0 Then
                Ctrl = PlattformIDComboBox
                ErrorText = My.Resources.MyStrings.apiDataMissingPlatformError
            ElseIf ApiKeyDecryptedTextBox.Text.Trim.Length < 16 Then
                Ctrl = ApiKeyDecryptedTextBox
                ErrorText = My.Resources.MyStrings.apiDataInvalidKeyError
            ElseIf ApiSecretDecryptedTextBox.Text.Trim.Length < 16 And PlattformIDComboBox.SelectedValue <> CInt(PlatformManager.Platforms.BitcoinDe) Then
                Ctrl = ApiSecretDecryptedTextBox
                ErrorText = My.Resources.MyStrings.apiDataInvalidSecretError
            ElseIf PlattformIDComboBox.SelectedValue = CInt(PlatformManager.Platforms.Bitfinex) AndAlso ccbBitfinexCurrencies.CheckedItems.Count < 2 Then
                Ctrl = ccbBitfinexCurrencies
                ErrorText = My.Resources.MyStrings.apiDataMissingCurrenciesBfxError
            End If
            If BezeichnungTextBox.Text.Trim.Length = 0 Then
                BezeichnungTextBox.Text = String.Format(My.Resources.MyStrings.apiDataDefaultName, IDTextBox.Text)
            End If

            If Ctrl IsNot Nothing Then
                ErrProvider.Clear()
                e.Cancel = True
                ErrProvider.SetError(Ctrl, ErrorText)
            Else
                ApiKeyTextBox.Text = _Crypt.EncryptData(ApiKeyDecryptedTextBox.Text.Trim)
                ApiSecretTextBox.Text = _Crypt.EncryptData(ApiSecretDecryptedTextBox.Text.Trim)
                If LastImportTimestampDateTimePicker.Value < APIMINDATE Then
                    LastImportTimestampTextBox.Text = DateToUnixTimestamp(APIMINDATE)
                Else
                    LastImportTimestampTextBox.Text = DateToUnixTimestamp(LastImportTimestampDateTimePicker.Value)
                End If
                ' Save checked currencies to ExtendedInfo
                If PlattformIDComboBox.SelectedValue = PlatformManager.Platforms.BitcoinDe Or PlattformIDComboBox.SelectedValue = PlatformManager.Platforms.Bitfinex Then
                    Dim CheckedIndices As String = ccbBitfinexCurrencies.GetCheckedItemsIDs
                    Dim TempAccInfo As AccountInfo
                    Select Case PlattformIDComboBox.SelectedValue
                        Case PlatformManager.Platforms.Bitfinex
                            TempAccInfo = New BitfinexClient.BitfinexAccountInfo()
                        Case Else
                            TempAccInfo = New BitcoinDeClient.BitcoinDeAccountInfo()
                    End Select
                    For i As Integer = 0 To TempAccInfo.Currencies.Length - 1
                        TempAccInfo.Currencies(i).Active = CheckedIndices.Contains(TempAccInfo.Currencies(i).Shortname)
                    Next
                    ExtendedInfoTextBox.Text = TempAccInfo.ToString
                End If
            End If
        End If

    End Sub

    Private Sub ApiKeyTextBox_TextChanged(sender As Object, e As EventArgs) Handles ApiKeyTextBox.TextChanged
        ' Entschlüsselter Text in anderes Control
        If DirectCast(sender, TextBox).Text.Trim.Length = 0 Then
            ApiKeyDecryptedTextBox.Text = ""
        Else
            ApiKeyDecryptedTextBox.Text = _Crypt.DecryptData(DirectCast(sender, TextBox).Text)
        End If
    End Sub

    Private Sub ApiSecretTextBox_TextChanged(sender As Object, e As EventArgs) Handles ApiSecretTextBox.TextChanged
        ' Entschlüsselter Text in anderes Control
        If DirectCast(sender, TextBox).Text.Trim.Length = 0 Then
            ApiSecretDecryptedTextBox.Text = ""
        Else
            ApiSecretDecryptedTextBox.Text = _Crypt.DecryptData(DirectCast(sender, TextBox).Text)
        End If
    End Sub


    Private Sub cmdCancel_MouseEnter(sender As Object, e As EventArgs) Handles cmdCancel.MouseEnter
        pnlDetails.CausesValidation = False
    End Sub

    Private Sub cmdCancel_MouseLeave(sender As Object, e As EventArgs) Handles cmdCancel.MouseLeave
        pnlDetails.CausesValidation = True
    End Sub

    Private Sub PlattformIDComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PlattformIDComboBox.SelectedIndexChanged
        Dim CurrenciesVisible As Boolean
        Select Case PlattformIDComboBox.SelectedValue
            Case PlatformManager.Platforms.BitcoinDe
                lblHinweise.Text = "Hinweise"
                lblHinweise.Visible = True
                lblBitcoinDe.Visible = True
                lblKraken.Visible = False
                CurrenciesVisible = True
                CallDelayNumericUpDown.Visible = False
                CallDelayLabel.Visible = False
            Case PlatformManager.Platforms.Kraken
                lblHinweise.Text = "Hinweis"
                lblHinweise.Visible = True
                lblBitcoinDe.Visible = False
                lblKraken.Visible = True
                CurrenciesVisible = False
                CallDelayNumericUpDown.Visible = True
                CallDelayLabel.Visible = True
                If ApiDatenBindingSource.Position >= 0 AndAlso ApiDatenBindingSource.Current.Row.RowState = DataRowState.Added AndAlso Not CallDelayNumericUpDown.Touched Then
                    CallDelayNumericUpDown.Value = KrakenClient.KrakenClient.KRAKEN_API_DEFAULTINTERVAL
                End If
            Case PlatformManager.Platforms.Bitfinex
                lblHinweise.Text = "Hinweis"
                lblHinweise.Visible = True
                lblBitcoinDe.Visible = False
                lblKraken.Visible = False
                CurrenciesVisible = True
                CallDelayNumericUpDown.Visible = True
                CallDelayLabel.Visible = True
                If ApiDatenBindingSource.Position >= 0 AndAlso ApiDatenBindingSource.Current.Row.RowState = DataRowState.Added AndAlso Not CallDelayNumericUpDown.Touched Then
                    CallDelayNumericUpDown.Value = BitfinexClient.BitfinexClient.BITFINEX_API_DEFAULTINTERVAL
                End If
            Case Else
                lblHinweise.Visible = False
                lblBitcoinDe.Visible = False
                lblKraken.Visible = False
                CurrenciesVisible = False
                CallDelayNumericUpDown.Visible = False
                CallDelayLabel.Visible = False
        End Select
        If PlattformIDComboBox.SelectedValue >= 0 AndAlso PlattformIDComboBox.Text.Length > 0 AndAlso
        _NewRows.Contains(ApiDatenBindingSource.Position) AndAlso (Not _ApiLabelTouched Or BezeichnungTextBox.Text.Length = 0) Then
            ' set the default label for api import data
            BezeichnungTextBox.Text = Application.ProductName & "@" & PlattformIDComboBox.Text
        End If
        ccbBitfinexCurrencies.Visible = CurrenciesVisible
        lblCurrencies.Visible = CurrenciesVisible
        If CurrenciesVisible Then
            ReloadCurrencyCombobox(PlattformIDComboBox.SelectedValue)
        End If
        lblBitfinex.Visible = (PlattformIDComboBox.SelectedValue = PlatformManager.Platforms.Bitfinex)
    End Sub

    Private Sub LastImportTimestampTextBox_TextChanged(sender As Object, e As EventArgs) Handles LastImportTimestampTextBox.TextChanged
        With LastImportTimestampTextBox
            If .Text.Length = 0 OrElse DateFromUnixTimestamp(.Text) < APIMINDATE Then
                LastImportTimestampDateTimePicker.Value = APIMINDATE
            Else
                LastImportTimestampDateTimePicker.Value = DateFromUnixTimestamp(.Text)
            End If
        End With
    End Sub

    Private Sub BezeichnungTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles BezeichnungTextBox.KeyPress
        _ApiLabelTouched = True
    End Sub
End Class

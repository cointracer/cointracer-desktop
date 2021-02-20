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

Imports System.IO
Imports CoinTracer.DBInit

Public Class frmApplicationSettings

    Private _DBInit As DBInit
    Private _DataDirectory As String
    Private _OldDataDirectory As String
    Private _MessagesDS As DataSet
    Private _MessagesDT As DataTable

    Private _SelectedCategory As Integer
    Public Property SelectedCategory() As Integer
        Get
            Return lbxCategories.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            _SelectedCategory = value
        End Set
    End Property

    Private Sub frmApplicationSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.LastSettingsCategory = lbxCategories.SelectedIndex
    End Sub


    Private Sub frmApplicationSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        If _SelectedCategory >= 0 And _SelectedCategory < lbxCategories.Items.Count Then
            lbxCategories.SelectedIndex = _SelectedCategory
        ElseIf My.Settings.LastSettingsCategory >= 0 And My.Settings.LastSettingsCategory < lbxCategories.Items.Count Then
            lbxCategories.SelectedIndex = My.Settings.LastSettingsCategory
        Else
            lbxCategories.SelectedIndex = 0
        End If
        _DBInit = New DBInit
        SetFormTitle()
        _DataDirectory = _DBInit.DatabaseDirectory
        _OldDataDirectory = _DataDirectory
        ' Messages-DataTable definieren
        _MessagesDS = New DataSet
        _MessagesDT = New DataTable
        With _MessagesDT.Columns
            .Add(New DataColumn("MessageQualifier", Type.GetType("System.String")))
            .Add(New DataColumn("MessageDescription", Type.GetType("System.String")))
            .Add(New DataColumn("Action", Type.GetType("System.String")))
        End With
        _MessagesDS.Tables.Add(_MessagesDT)
        ' Einstellungen laden
        LoadSettingsToForm()
    End Sub

    Private Sub SetFormTitle()
        Me.Text = ProductName & " - Einstellungen"
        With lbxCategories
            If .SelectedIndex >= 0 Then
                Me.Text &= ": " & .SelectedItem.ToString
            End If
        End With
    End Sub

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        WriteSettings()
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub rbOfflineMode_CheckedChanged(sender As Object, e As EventArgs) Handles rbOfflineMode.CheckedChanged, rbOnlineMode.CheckedChanged
        pnlOnline1.Enabled = Not rbOfflineMode.Checked
        pnlOnline2.Enabled = pnlOnline1.Enabled
        ' pnlOnline3.Enabled = pnlOnline1.Enabled
    End Sub

    Private Sub LoadSettingsToForm(Optional CertainPanel As Integer = -1)
        ' Sicherheit
        If CertainPanel = 0 Or CertainPanel = -1 Then
            rbApiAsk.Checked = My.Settings.AskForApiProtection
            rbApiDontAsk.Checked = Not rbApiAsk.Checked
        End If
        ' Online-Einstellungen
        If CertainPanel = 1 Or CertainPanel = -1 Then
            rbOfflineMode.Checked = My.Settings.OfflineMode
            rbOnlineMode.Checked = Not My.Settings.OfflineMode
            rbProxyOn.Checked = My.Settings.UseProxy
            rbProxyOff.Checked = Not rbProxyOn.Checked
            rbCheckUpdateOn.Checked = My.Settings.CheckVersionOnStart
            rbCheckUpdateOff.Checked = Not rbCheckUpdateOn.Checked
            rbFiatCoursesOn.Checked = My.Settings.CheckFiatCoursesOnStart
            rbFiatCoursesOff.Checked = Not rbFiatCoursesOn.Checked
            rbCoinCoursesOn.Checked = My.Settings.CheckCoinCoursesOnStart
            rbCoinCoursesOff.Checked = Not rbCoinCoursesOn.Checked
        End If
        ' Verzeichnisse und Pfade
        If CertainPanel = 2 Or CertainPanel = -1 Then
            Dim ActualPath As String = _DBInit.DatabaseDirectory
            If ActualPath = _DBInit.DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory) Then
                rbDBUser.Checked = True
                rbDBProgram.Checked = False
                rbDBCustom.Checked = False
            ElseIf ActualPath = _DBInit.DatabaseDirectory(DataBaseDirectories.ApplicationDirectory) Then
                rbDBUser.Checked = False
                rbDBProgram.Checked = True
                rbDBCustom.Checked = False
            Else
                rbDBUser.Checked = False
                rbDBProgram.Checked = False
                rbDBCustom.Checked = True
                tbxDBFolder.Text = My.Settings.DataDirectory
            End If
            rbDBCustom_CheckedChanged(rbDBCustom, New EventArgs)
            EnhancedToolTip1.SetToolTip(rbDBUser, _DBInit.DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory))
            EnhancedToolTip1.SetToolTip(rbDBProgram, _DBInit.DatabaseDirectory(DataBaseDirectories.ApplicationDirectory))
        End If
        ' Meldungen
        If CertainPanel = 3 Or CertainPanel = -1 Then
            Select Case My.Settings.LogLevel
                Case System.Diagnostics.TraceEventType.Error
                    cbxLogLevel.SelectedIndex = 1
                Case System.Diagnostics.TraceEventType.Information
                    cbxLogLevel.SelectedIndex = 2
                Case System.Diagnostics.TraceEventType.Verbose
                    cbxLogLevel.SelectedIndex = 3
                Case Else
                    cbxLogLevel.SelectedIndex = 0
            End Select
            Dim dr As DataRow
            _MessagesDT.Rows.Clear()
            Dim MsgList As List(Of String())
            MsgList = New List(Of String())
            ' *
            ' * Achtung: Hier ist die zentrale Liste aller Meldungen, die auf Wunsch nicht wieder anzeigt werden
            ' * (nicht schön, aber funktioniert, ohne sich ein Bein auszureissen)
            ' *
            MsgList.Add({"ImportAutoDetect", "Tradedaten-Import: Hinweis zur automatischen Erkennung des Dateiformats"})
            MsgList.Add({"ImportBitcoinDe", "Tradedaten-Import: Hinweis zu Bitcoin.de (CSV)"})
            MsgList.Add({"ImportBitcoinDeApi", "Tradedaten-Import: Hinweis zu Bitcoin.de (API-Import)"})
            MsgList.Add({"ImportKrakenCSV", "Tradedaten-Import: Hinweis zu Kraken.com (CSV-Import)"})
            MsgList.Add({"ImportBitfinexCom", "Tradedaten-Import: Hinweis zu Bitfinex.com"})
            MsgList.Add({"ImportBitfinexApiWarning", "Tradedaten-Import: Warnhinweis zum Bitfinex-API-Import"})
            MsgList.Add({"ImportPoloniexCom", "Tradedaten-Import: Hinweis zu Poloniex.com"})
            MsgList.Add({"ImportCoinTracer", "Tradedaten-Import: Hinweis zum CoinTracer-Format"})
            MsgList.Add({"ImportMtGox", "Tradedaten-Import: Hinweis zu Mt. Gox"})
            MsgList.Add({"MergeTransfers", "Bearbeitung von Transfer-Daten: Erklärung der Funktion/Vorgehensweise"})
            MsgList.Add({"CalculateGainingsDespiteUnclearTrades", "Gewinn-/Verlustberechnung: Hinweis zu ungeklärten Transfers"})
            MsgList.Add({"CalculateGainingsDespiteInconsistentWalletWareness", "Gewinn-/Verlustberechnung: Hinweis zu unlogischen Einstellungen Zu-/Abfluss"})
            MsgList.Add({"GoToTransfersAfterImport", "Tradedaten-Import: Frage, ob ungeklärte Transfers bearbeitet werden sollen"})
            ' * ^ Ende der Liste
            For Each MsgBoxItem As String() In MsgList
                If MsgBoxEx.GetDefaultDialogResult(MsgBoxItem(0)) <> Windows.Forms.DialogResult.None Then
                    dr = _MessagesDT.NewRow
                    dr("MessageQualifier") = MsgBoxItem(0)
                    dr("MessageDescription") = MsgBoxItem(1)
                    dr("Action") = "Meldung wieder anzeigen"
                    _MessagesDT.Rows.Add(dr)
                End If
            Next
            ' grdDataMessages.DataSource = Nothing
            grdDataMessages.DataSource = _MessagesDT
            grdDataMessages.Refresh()
        End If
        ' Anzeige-Einstellungen
        If CertainPanel = 4 Or CertainPanel = -1 Then
            rbBestandEUR.Checked = My.Settings.InventoryPricesCurrency = "EUR"
            rbBestandUSD.Checked = Not rbBestandEUR.Checked
        End If
        ' Transfer detection
        If CertainPanel = 5 Or CertainPanel = -1 Then
            ToleranceMinutesTextBox.Text = TransferDetection.MinutesTolerance.ToString("#########0", CultureInfo.InvariantCulture)
            TolerancePercentTextBox.Text = (TransferDetection.AmountPercentTolerance * 100).ToString("#########0", CultureInfo.InvariantCulture)
        End If


    End Sub

    ''' <summary>
    ''' Schreibt die vorgenommenen Änderungen in die Settings.
    ''' </summary>
    Private Sub WriteSettings()
        ' Sicherheit
        My.Settings.AskForApiProtection = rbApiAsk.Checked
        ' Online-Einstellungen
        My.Settings.OfflineMode = rbOfflineMode.Checked
        My.Settings.UseProxy = rbProxyOn.Checked
        My.Settings.CheckVersionOnStart = rbCheckUpdateOn.Checked
        My.Settings.CheckFiatCoursesOnStart = rbFiatCoursesOn.Checked
        My.Settings.CheckCoinCoursesOnStart = rbCoinCoursesOn.Checked
        If rbBestandEUR.Checked Then
            My.Settings.InventoryPricesCurrency = "EUR"
        Else
            My.Settings.InventoryPricesCurrency = "USD"
        End If
        ' Loglevel
        Select Case cbxLogLevel.SelectedIndex
            Case 1
                My.Settings.LogLevel = System.Diagnostics.TraceEventType.Error
            Case 2
                My.Settings.LogLevel = System.Diagnostics.TraceEventType.Information
            Case 3
                My.Settings.LogLevel = System.Diagnostics.TraceEventType.Verbose
            Case Else
                My.Settings.LogLevel = 0
        End Select
        ' Transfer detection
        TransferDetection.MinutesTolerance = CInt(Math.Abs(CInt(ToleranceMinutesTextBox.Text)))
        TransferDetection.AmountPercentTolerance = CInt(Math.Abs(CInt(TolerancePercentTextBox.Text))) / 100
        TransferDetection.Save()
        ' Pfade & Verzeichnisse
        Dim NewConnection As SQLite.SQLiteConnection = Nothing
        Dim DataDirType As DataBaseDirectories
        Try
            If rbDBUser.Checked Then
                ' AppData-Ordner
                DataDirType = DataBaseDirectories.UserAppDataDirectory
                _DataDirectory = _DBInit.DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory)
            ElseIf rbDBProgram.Checked Then
                ' Application-Ordner
                DataDirType = DataBaseDirectories.ApplicationDirectory
                _DataDirectory = _DBInit.DatabaseDirectory(DataBaseDirectories.ApplicationDirectory)
            Else
                ' Custom-Ordner
                DataDirType = DataBaseDirectories.CustomDirectory
                _DataDirectory = tbxDBFolder.Text.Trim
            End If
            If _OldDataDirectory <> _DataDirectory Then
                If _DBInit.SetDatabaseDirectory(NewConnection, DataDirType, _DataDirectory) Then
                    ' Auch die Config-Datei dorthin speichern
                    Dim SettingsProv As New FlexFileSettingsProvider
                    SettingsProv.ChangeSettingsDirectory(_DataDirectory)
                    frmMain.Connection = NewConnection
                    MessageBox.Show("Der Speicherort der lokalen Datenbank wurde erfolgreich geändert.", "Speicherort der Datenbank", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            _DataDirectory = ""
            DefaultErrorHandler(ex, "Fehler beim Ändern des Speicherorts der Datenbank: " & ex.Message & Environment.NewLine &
                                Environment.NewLine & "Der Speicherort wird nicht geändert.")
            Exit Sub
        End Try
    End Sub

    Private Sub lbxCategories_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbxCategories.SelectedIndexChanged
        pnlSecurity.Visible = lbxCategories.SelectedIndex = 0
        pnlOnlineSettings.Visible = lbxCategories.SelectedIndex = 1
        pnlDirectories.Visible = lbxCategories.SelectedIndex = 2
        pnlMessages.Visible = lbxCategories.SelectedIndex = 3
        pnlDisplaySettings.Visible = lbxCategories.SelectedIndex = 4
        pnlTransferDetection.Visible = lbxCategories.SelectedIndex = 5
        SetFormTitle()
    End Sub


    Private Sub rbDBProgram_CheckedChanged(sender As Object, e As EventArgs) Handles rbDBProgram.CheckedChanged
        If DirectCast(sender, RadioButton).Checked Then
            If Not _DBInit.CheckIfWritable(_DBInit.DatabaseDirectory(DataBaseDirectories.ApplicationDirectory)) Then
                MessageBox.Show("In das Programmverzeichnis '" & _DBInit.DatabaseDirectory(DataBaseDirectories.ApplicationDirectory) &
                                "' kann nicht geschrieben werden, daher ist " &
                                "dieser Speicherort für die Datenbank nicht zulässig. Bitte wählen Sie eine andere Einstellung.",
                                "Fehlende Schreibberechtigung", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                LoadSettingsToForm(1)
            End If
        End If
    End Sub

    Private Sub rbDBCustom_CheckedChanged(sender As Object, e As EventArgs) Handles rbDBCustom.CheckedChanged
        tbxDBFolder.Enabled = DirectCast(sender, RadioButton).Checked
        cmdBrowseDBFolder.Enabled = tbxDBFolder.Enabled
    End Sub

    Private Sub cmdBrowseDBFolder_Click(sender As Object, e As EventArgs) Handles cmdBrowseDBFolder.Click
        Dim BFD As New FolderBrowserDialog
        With BFD
            If tbxDBFolder.Text.Length > 0 Then
                .SelectedPath = Path.GetDirectoryName(tbxDBFolder.Text)
            End If
            .ShowNewFolderButton = True
            .Description = "Wählen Sie hier den Ordner aus, in dem die lokale Datenbank gespeichert werden soll."
            If .ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                If _DBInit.CheckIfWritable(.SelectedPath) Then
                    tbxDBFolder.Text = .SelectedPath
                Else
                    MessageBox.Show("In den Ordner '" & .SelectedPath & "' kann nicht geschrieben werden, daher ist " &
                                    "dieser als Speicherort für die Datenbank nicht zulässig. Bitte wählen Sie einen anderen Ordner.",
                                    "Fehlende Schreibberechtigung", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            End If
        End With
    End Sub

    Private Sub grdDataMessages_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles grdDataMessages.CellContentClick
        Dim senderGrid As DataGridView = DirectCast(sender, DataGridView)
        If TypeOf senderGrid.Columns(e.ColumnIndex) Is DataGridViewButtonColumn AndAlso e.RowIndex >= 0 Then
            ' Meldung soll wieder angezeigt werden
            MsgBoxEx.SetDefaultDialogResult(grdDataMessages.Rows(e.RowIndex).Cells(0).Value, Windows.Forms.DialogResult.None)
            ' Grid neu laden
            LoadSettingsToForm(3)
        End If
    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    ''' <summary>
    ''' Just digits in these boxes...
    ''' </summary>
    Private Sub ToleranceTextBoxes_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TolerancePercentTextBox.KeyPress
        If Not Char.IsDigit(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub cmdTransferDetection_Click(sender As Object, e As EventArgs) Handles cmdTransferDetection.Click
        TransferDetection.Save()
        Try
            With frmMain.CointracerDatabase
                .Reset_DataAdapter(DBHelper.TableNames.Kalkulationen)
                If .DataTable(DBHelper.TableNames.Kalkulationen).Rows.Count = 0 OrElse
                    MessageBox.Show(String.Format(My.Resources.MyStrings.settingsMsgTransferdetection, Environment.NewLine),
                                       My.Resources.MyStrings.settingsMsgTransferdetectionTitle,
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Exclamation,
                                       MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                    ' erase all gainings calculations
                    frmMain.TradeValueManager.RollbackCalculation(DATENULLVALUE, , True)
                    ' erase all "raw" transfers
                    Dim SQL As String = String.Format("UPDATE Trades SET RefTradeID = 0, Entwertet = 0 WHERE TradeTypID IN ({0}, {1}) AND RefTradeID IN " &
                                                      "(SELECT ID FROM Trades WHERE TradeTypID = {2} AND (QuellPlattformID = 0 OR ZielplattformID = 0) AND ((InTradeID = 0 AND OutTradeID > 0) OR (InTradeID > 0 AND OutTradeID = 0)))",
                                                      CInt(DBHelper.TradeTypen.Einzahlung),
                                                      CInt(DBHelper.TradeTypen.Auszahlung),
                                                      CInt(DBHelper.TradeTypen.Transfer))
                    frmMain.CointracerDatabase.ExecuteSQL(SQL)
                    SQL = String.Format("DELETE FROM Trades WHERE TradeTypID = {0} AND (QuellPlattformID = 0 OR ZielplattformID = 0) AND ((InTradeID = 0 AND OutTradeID > 0) OR (InTradeID > 0 AND OutTradeID = 0))",
                                                      CInt(DBHelper.TradeTypen.Transfer))
                    frmMain.CointracerDatabase.ExecuteSQL(SQL)
                    ' and finally create transfers again
                    Dim Import As New Import(frmMain.CointracerDatabase)
                    Import.ProcessTransferData()
                    Dim Msg As String = Import.GetProcessedTransfersString()
                    Dim Modified As Boolean = Msg.Length > 0
                    If Msg.Length = 0 Then
                        Msg = My.Resources.MyStrings.settingsMsgTransferdetectionNoTransfers
                    End If
                    MessageBox.Show(Msg,
                                    My.Resources.MyStrings.settingsMsgTransferdetectionTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information)
                    If Modified Then
                        frmMain.RefreshAfterTradesEdit()
                    End If
                End If
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub ToleranceMinutesTextBox_TextChanged(sender As Object, e As EventArgs)
        Try
            Dim TB As TextBox = DirectCast(sender, TextBox)
            If IsNumeric(TB.Text) Then
                Dim Minutes As Integer = Math.Abs(CInt(TB.Text))
                If Minutes >= 60 Then
                    lblMinutesToHours.Text = String.Format("(= {0} {1}",
                                                     Minutes \ 60,
                                                     IIf(Minutes \ 60 = 1, My.Resources.MyStrings.globalHour, My.Resources.MyStrings.globalHours))
                    If Minutes Mod 60 > 0 Then
                        lblMinutesToHours.Text &= String.Format(", {0} {1}",
                                                     Minutes Mod 60,
                                                     IIf(Minutes Mod 60 = 1, My.Resources.MyStrings.globalMinute, My.Resources.MyStrings.globalMinutes))
                    End If
                    lblMinutesToHours.Text &= ")"
                Else
                    lblMinutesToHours.Text = ""
                End If
            Else
                lblMinutesToHours.Text = ""
            End If

        Catch ex As Exception
            ' Don't mind...
        End Try
    End Sub

    Private Sub cmdOpenLogLocation_Click(sender As Object, e As EventArgs) Handles cmdOpenLogLocation.Click
        Process.Start(My.Application.Log.DefaultFileLogWriter.CustomLocation)
    End Sub

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _SelectedCategory = -1
    End Sub

End Class

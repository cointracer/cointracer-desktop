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
Imports CoinTracer.CoinTracerDataSetTableAdapters

''' <summary>
''' Stellt Eingabemöglichkeiten für die "Kerndaten" eines Trade-Eintrags zur Verfügung
''' (Zeitpunkt, Plattform, Betrag, Konto, Betrag Gebühr, Betrag EUR)
''' </summary>
''' <remarks></remarks>
Public Class TradeCoreDataControl

    Public Enum ControlModes
        Undefined = 0
        EditOpenTransfers = 1
        EditTrades = 2
    End Enum

    Public Event CurrentChanged(sender As Object, e As EventArgs)

    Private _LastEdited As Integer
    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return _LastEdited
        End Get
    End Property


    Private _ControlMode As ControlModes
    Public Property ControlMode() As ControlModes
        Get
            Return _ControlMode
        End Get
        Set(ByVal value As ControlModes)
            _ControlMode = value
            ' Trigger read-only of controls depending on ControlMode
            Select Case ControlMode
                Case ControlModes.EditOpenTransfers
                    ImportPlattformIDComboBox.Enabled = False
                    SourceIDTextBox.ReadOnly = True
                    TradeTypIDComboBox.Enabled = False
                    ZeitpunktDateTimePicker.Enabled = False
                    ZeitpunktZielDateTimePicker.Enabled = False
                    InZeitpunktDateTimePicker.Enabled = False
                    QuellBetragTextBox.ReadOnly = True
                    QuellBetragNachGebuehrTextBox.ReadOnly = True
                    ZielBetragTextBox.ReadOnly = True
                    QuellKontoComboBox.Enabled = False
                    ZielKontoComboBox.Enabled = False
                    BetragNachGebuehrTextBox.ReadOnly = True
                    WertEURTextBox.ReadOnly = True
                    BindingNavigatorAddNewItem.Enabled = False
                    BindingNavigatorDeleteItem.Enabled = False
            End Select
        End Set
    End Property

    ''' <summary>
    ''' ID des aktuell ausgewälten Trades. 0, wenn keiner ausgewählt ist oder gerade ein neuer angelegt wird.
    ''' </summary>
    ''' <remarks></remarks>
    Private _CurrentTradeID As Long
    Public Property CurrentTradeID() As Long
        Get
            Return _CurrentTradeID
        End Get
        Set(ByVal value As Long)
            _CurrentTradeID = value
        End Set
    End Property


    ''' <summary>
    ''' Legt fest, ob die Eingaben validiert werden sollen oder nicht. Nützlich für temporäres Abschalten der Validierung, weil das 
    ''' umliegende Formular geschlossen werden soll.
    ''' </summary>
    ''' <value>True, wenn das Validate-Ereignis ausgelöst werden soll, sonst False</value>
    Public Property EnableValidation() As Boolean
        Get
            Return pnlTrade.CausesValidation
        End Get
        Set(ByVal value As Boolean)
            pnlTrade.CausesValidation = value
        End Set
    End Property


    Private _TradesTa As TradesTableAdapter
    Private _Cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _Cnn
        End Get
    End Property

    Private _DS As CoinTracerDataSet
    Public ReadOnly Property DataSet() As CoinTracerDataSet
        Get
            Return _DS
        End Get
    End Property

    Private _StartID As Long
    Public Property StartID() As Long
        Get
            Return _StartID
        End Get
        Set(ByVal value As Long)
            _StartID = value
        End Set
    End Property

    Private _WhereEtcSQL As String
    Private _IsLoading As Boolean


    ''' <summary>
    ''' Lädt das Control anhand der übergebenen Where-Klausel aus der Trades-Tabelle
    ''' </summary>
    ''' <param name="WhereEtcSQL">Teil des SQL-Statement, das an der Stelle "WHERE..." beginnt</param>
    ''' <param name="ControlMode">Gibt an, ob nur Trades bearbeitet werden dürfen oder alle Trades</param>
    ''' <param name="StartID">Optional: ID des Datensatzes, zu dem initial gesprungen werden soll</param>
    Public Sub InitData(ByRef Connection As SQLite.SQLiteConnection, _
                        ByVal WhereEtcSQL As String, _
                        Optional ControlMode As ControlModes = ControlModes.Undefined, _
                        Optional StartID As Long = -1)

        If ControlMode <> ControlModes.Undefined Then
            Me.ControlMode = ControlMode
        End If

        _Cnn = Connection
        _DS = New CoinTracerDataSet(_Cnn)
        _TradesTa = New TradesTableAdapter()
        _TradesTa.FillBySQL(_DS.Trades, WhereEtcSQL)
        _WhereEtcSQL = WhereEtcSQL

        TradesBindingSource.DataSource = _DS
        TradesBindingSource.DataMember = "Trades"

        Dim PlTA As New PlattformenTableAdapter
        PlTA.Fill(_DS.Plattformen)
        Dim KtTA As New KontenTableAdapter
        KtTA.Fill(_DS.Konten)
        Dim TtTA As New TradeTypenTableAdapter
        TtTA.FillBySQL(_DS.TradeTypen, "WHERE ID > 2 AND ID < 16")
        TtTA.Dispose()
        KtTA.Dispose()
        PlTA.Dispose()

        ' ImportPlattformIDComboBox
        TradesPlattformenImportBindingSource.DataSource = _DS
        TradesPlattformenImportBindingSource.DataMember = "Plattformen"
        Dim b As New Binding("SelectedValue",
             Me.TradesBindingSource, "ImportPlattformID", True)
        ImportPlattformIDComboBox.DataBindings.Clear()
        ImportPlattformIDComboBox.DataBindings.Add(b)

        ' QuellPlattformIDComboBox
        TradesPlattformenQuellBindingSource.DataSource = _DS
        TradesPlattformenQuellBindingSource.DataMember = "Plattformen"
        b = New Binding("SelectedValue",
             Me.TradesBindingSource, "QuellPlattformID", True)
        QuellPlattformIDComboBox.DataBindings.Clear()
        QuellPlattformIDComboBox.DataBindings.Add(b)

        ' ZielPlattformIDComboBox
        TradesPlattformenZielBindingSource.DataSource = _DS
        TradesPlattformenZielBindingSource.DataMember = "Plattformen"
        b = New Binding("SelectedValue",
            Me.TradesBindingSource, "ZielPlattformID", True)
        ZielPlattformIDComboBox.DataBindings.Clear()
        ZielPlattformIDComboBox.DataBindings.Add(b)

        ' QuellKontoComboBox
        TradesKontenQuellBindingSource.DataSource = _DS
        TradesKontenQuellBindingSource.DataMember = "Konten"
        b = New Binding("SelectedValue",
             Me.TradesBindingSource, "QuellKontoID", True)
        QuellKontoComboBox.DataBindings.Clear()
        QuellKontoComboBox.DataBindings.Add(b)

        ' ZielKontoComboBox
        TradesKontenZielBindingSource.DataSource = _DS
        TradesKontenZielBindingSource.DataMember = "Konten"
        b = New Binding("SelectedValue",
             Me.TradesBindingSource, "ZielKontoID", True)
        ZielKontoComboBox.DataBindings.Clear()
        ZielKontoComboBox.DataBindings.Add(b)

        ' TradeTypIDComboBox
        TradesTradeTypenBindingSource.DataSource = _DS
        TradesTradeTypenBindingSource.DataMember = "TradeTypen"
        b = New Binding("SelectedValue",
             Me.TradesBindingSource, "TradeTypID", True)
        TradeTypIDComboBox.DataBindings.Clear()
        TradeTypIDComboBox.DataBindings.Add(b)

        ' ID des Start-Datensatzes
        _StartID = StartID
        If _StartID >= 0 Then
            Me.TradesBindingSource.Position = Me.TradesBindingSource.Find("ID", StartID)
        Else
            TradesBindingSource_CurrentChanged(Nothing, Nothing)
        End If

        _LastEdited = 0

    End Sub

    ''' <summary>
    ''' Lädt alle Trades neu und springt ggf. zur angegebenen ID
    ''' </summary>
    ''' <param name="JumpToID">ID des Zieldatensatzes</param>
    ''' <remarks></remarks>
    Public Sub Reload(Optional ByVal JumpToID As Long = 0)
        _TradesTa.FillBySQL(_DS.Trades, _WhereEtcSQL)
        Me.TradesBindingSource.ResetBindings(False)
        If JumpToID > 0 Then
            Me.TradesBindingSource.Position = Me.TradesBindingSource.Find("ID", JumpToID)
        Else
            TradesBindingSource_CurrentChanged(Nothing, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Speichert alle Änderungen in der Datenbank und gibt eine Meldung aus.
    ''' </summary>
    Public Sub UpdateDatabase()
        If _TradesTa IsNot Nothing Then
            TradesBindingSource.EndEdit()
            _LastEdited = _TradesTa.Update(_DS.Trades)
            If _LastEdited > 0 Then
                ' Falls Transaktionen gelöscht wurden: alle Ein- und Auszahlungen, die hierauf verwiesen haben, ebenfalls löschen
                Dim SQL As String
                SQL = String.Format("delete from Trades where ID in (" &
                                    "select t.ID from Trades t left join Trades t2 on t.RefTradeID=t2.ID " &
                                    "where t.TradeTypID in ({0},{1}) and t.RefTradeID > 0 and t2.ID is NULL)",
                                    CInt(DBHelper.TradeTypen.Einzahlung),
                                    CInt(DBHelper.TradeTypen.Auszahlung))
                _DS.ExecuteSQL(SQL)
                ' Info an User
                MessageBox.Show(My.Resources.MyStrings.editMsgSaved, My.Resources.MyStrings.tradesEditTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub PlattformIDComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles QuellPlattformIDComboBox.SelectedValueChanged, ZielPlattformIDComboBox.SelectedValueChanged
        If _ControlMode = ControlModes.EditOpenTransfers Then
            Try
                Dim cbo As ComboBox
                cbo = DirectCast(sender, ComboBox)
                With cbo
                    If .SelectedIndex = 0 Then
                        .BackColor = Color.Red
                    Else
                        .BackColor = Color.White
                    End If
                End With
            Catch ex As Exception
                ' egal...
            End Try
        End If
        If CheckStateManualInput() AndAlso TradeTypIDComboBox.SelectedValue <> 5 Then
            If Not QuellPlattformIDComboBox.Touched Then QuellPlattformIDComboBox.SelectedIndex = DirectCast(sender, TouchedComboBox).SelectedIndex
            If Not ZielPlattformIDComboBox.Touched Then ZielPlattformIDComboBox.SelectedIndex = DirectCast(sender, TouchedComboBox).SelectedIndex
            If Not ImportPlattformIDComboBox.Touched Then ImportPlattformIDComboBox.SelectedIndex = DirectCast(sender, TouchedComboBox).SelectedIndex
        End If
    End Sub

    Private Sub TradesBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles TradesBindingSource.CurrentChanged
        ' Disable input control linking logic
        _IsLoading = True
        ' Controls dis-/enablen
        Dim SomeSelected As Boolean = Me.TradesBindingSource.Position >= 0
        BindingNavigatorDeleteItem.Enabled = _ControlMode = ControlModes.EditTrades And SomeSelected
        BindingNavigatorAddNewItem.Enabled = _ControlMode = ControlModes.EditTrades
        If SomeSelected Then
            Dim CurRow As TradesRow = TradesBindingSource.Current.Row
            _CurrentTradeID = CurRow.ID
            If _ControlMode = ControlModes.EditOpenTransfers Then
                If CurRow.TradeTypID = DBHelper.TradeTypen.Transfer Then
                    QuellPlattformIDComboBox.Enabled = CurRow.Item("QuellPlattformID", DataRowVersion.Original) = 0
                    QuellBetragTextBox.ReadOnly = Not QuellPlattformIDComboBox.Enabled
                    QuellBetragNachGebuehrTextBox.ReadOnly = Not QuellPlattformIDComboBox.Enabled
                    ZielPlattformIDComboBox.Enabled = CurRow.Item("ZielPlattformID", DataRowVersion.Original) = 0
                    ZielBetragTextBox.ReadOnly = Not ZielPlattformIDComboBox.Enabled
                    BetragNachGebuehrTextBox.ReadOnly = ZielBetragTextBox.ReadOnly
                    WertEURTextBox.ReadOnly = ZielBetragTextBox.ReadOnly
                End If
            Else
                ' Normales Bearbeiten von Trades
                QuellPlattformIDComboBox.Enabled = True
                QuellBetragTextBox.ReadOnly = False
                QuellBetragNachGebuehrTextBox.ReadOnly = False
                ZielPlattformIDComboBox.Enabled = True
                ZielBetragTextBox.ReadOnly = False
                BetragNachGebuehrTextBox.ReadOnly = False
                WertEURTextBox.ReadOnly = False
            End If
        Else
            ' Nichts gewählt - alles sperren!
            _CurrentTradeID = 0
            QuellPlattformIDComboBox.Enabled = False
            QuellBetragTextBox.ReadOnly = True
            QuellBetragNachGebuehrTextBox.ReadOnly = True
            ZielPlattformIDComboBox.Enabled = False
            ZielBetragTextBox.ReadOnly = True
            BetragNachGebuehrTextBox.ReadOnly = True
            WertEURTextBox.ReadOnly = True
        End If
        SourceIDTextBox.ReadOnly = Not SomeSelected
        ImportPlattformIDComboBox.Enabled = SomeSelected
        TradeTypIDComboBox.Enabled = SomeSelected
        InfoTextBox.ReadOnly = Not SomeSelected
        SteuerIrrelevantCheckBox.Enabled = SomeSelected
        ZeitpunktDateTimePicker.Enabled = SomeSelected
        InZeitpunktDateTimePicker.Enabled = SomeSelected
        QuellKontoComboBox.Enabled = SomeSelected
        ZeitpunktZielDateTimePicker.Enabled = SomeSelected
        ZielKontoComboBox.Enabled = SomeSelected
        ' Input logic reset
        ImportPlattformIDComboBox.Touched = False
        QuellPlattformIDComboBox.Touched = False
        ZielPlattformIDComboBox.Touched = False
        ZeitpunktDateTimePicker.Touched = False
        InZeitpunktDateTimePicker.Touched = False
        ZeitpunktZielDateTimePicker.Touched = False
        BetragNachGebuehrTextBox.Touched = False
        ZielBetragTextBox.Touched = False
        QuellBetragNachGebuehrTextBox.Touched = False
        QuellBetragTextBox.Touched = False
        ' Custom Event auslösen
        RaiseEvent CurrentChanged(sender, e)
        ' Enable input control logic again
        _IsLoading = False
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorDeleteItem.Click
        If MessageBox.Show("Sind Sie sicher, dass der aktuelle Trade-Eintrag unwiderruflich gelöscht werden soll?",
                           "Trades bearbeiten", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Me.TradesBindingNavigator.BindingSource.RemoveCurrent()
            ErrProvider.Clear()
        End If
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
        Try
            ' Warum auch immer: Validierung passiert nicht automatisch?!
            Dim ValRes As System.ComponentModel.CancelEventArgs = New System.ComponentModel.CancelEventArgs(False)
            pnlTrade_Validating(Nothing, ValRes)
            If Not ValRes.Cancel Then
                Dim DB As New DBHelper(_Cnn)
                Dim NewID As Long = DB.GetMaxID(DBHelper.TableNames.Trades) + 1
                Dim TmpDv As DataView = New DataView(TradesBindingSource.DataSource.Tables("Trades"),
                                                 "ID >= " & NewID,
                                                 "ID desc", DataViewRowState.CurrentRows)
                If TmpDv.Count > 0 Then
                    NewID = TmpDv(0)("ID") + 1
                End If
                Dim NewRow As Object = TradesBindingNavigator.BindingSource.AddNew()
                With NewRow.Row
                    .ID = NewID
                    .ImportID = 0
                    .RefTradeID = 0
                    .InTradeID = 0
                    .OutTradeID = 0
                    .ImportPlattformID = CInt(PlatformManager.Platforms.Unknown)
                    .ZielPlattformID = CInt(PlatformManager.Platforms.Unknown)
                    .QuellPlattformID = CInt(PlatformManager.Platforms.Unknown)
                    .Entwertet = 0
                    .Steuerirrelevant = 0
                    .Zeitpunkt = Now
                    .ZeitpunktZiel = .Zeitpunkt
                    .InZeitpunkt = .Zeitpunkt
                    .TradetypID = CInt(DBHelper.TradeTypen.Undefiniert)
                End With
                TradesBindingNavigator.BindingSource.EndEdit()
                TradesBindingNavigator.BindingSource.Position = TradesBindingNavigator.BindingSource.Find("ID", NewID)
                TradesBindingSource_CurrentChanged(Nothing, Nothing)
                SourceIDTextBox.Focus()
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, "Beim Anlegen des neuen Datensatzes ist ein Fehler aufgetreten: " & ex.Message)
        End Try
    End Sub

    Private Sub BetragNachGebuehrTextBox_Validated(sender As Object, e As EventArgs) Handles BetragNachGebuehrTextBox.Validated
        Dim TB As TextBox = DirectCast(sender, TextBox)
        If IsNumeric(TB.Text) AndAlso IsNumeric(ZielBetragTextBox.Text) Then
            If Val(ZielBetragTextBox.Text) = 0 And Val(TB.Text) <> 0 Then
                ZielBetragTextBox.Text = TB.Text
            End If
        End If
    End Sub

    Private Sub ZielBetragTextBox_Validated(sender As Object, e As EventArgs) Handles ZielBetragTextBox.Validated
        Dim TB As TextBox = DirectCast(sender, TextBox)
        If IsNumeric(TB.Text) AndAlso IsNumeric(BetragNachGebuehrTextBox.Text) Then
            If Val(BetragNachGebuehrTextBox.Text) = 0 And Val(TB.Text) <> 0 Then
                BetragNachGebuehrTextBox.Text = TB.Text
            End If
        End If
    End Sub

    Private Sub QuellBetragNachGebuehrTextBox_Validated(sender As Object, e As EventArgs) Handles QuellBetragNachGebuehrTextBox.Validated
        Dim TB As TextBox = DirectCast(sender, TextBox)
        If IsNumeric(TB.Text) AndAlso IsNumeric(QuellBetragTextBox.Text) Then
            If Val(QuellBetragTextBox.Text) = 0 And Val(TB.Text) <> 0 Then
                QuellBetragTextBox.Text = TB.Text
            End If
        End If
    End Sub

    Private Sub QuellBetragTextBox_Validated(sender As Object, e As EventArgs) Handles QuellBetragTextBox.Validated
        Dim TB As TextBox = DirectCast(sender, TextBox)
        If IsNumeric(TB.Text) AndAlso IsNumeric(QuellBetragNachGebuehrTextBox.Text) Then
            If Val(QuellBetragNachGebuehrTextBox.Text) = 0 And Val(TB.Text) <> 0 Then
                QuellBetragNachGebuehrTextBox.Text = TB.Text
            End If
        End If
    End Sub

    Private Sub pnlTrade_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles pnlTrade.Validating
        Dim Ctrl As Control = Nothing
        Dim ErrorText As String = ""

        If TradesBindingNavigator.BindingSource.Count > 0 Then
            If ImportPlattformIDComboBox.SelectedIndex < 0 Then
                Ctrl = ImportPlattformIDComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorImportPlatform ' "Bitte wählen Sie eine gültige Import-Plattform aus!"
            ElseIf QuellPlattformIDComboBox.SelectedIndex < 0 Then
                Ctrl = QuellPlattformIDComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorSourcePlatform ' "Bitte wählen Sie eine gültige Quell-Plattform aus!"
            ElseIf ZielPlattformIDComboBox.SelectedIndex < 0 Then
                Ctrl = ZielPlattformIDComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorTargetPlatform ' "Bitte wählen Sie eine gültige Ziel-Plattform aus!"
            ElseIf TradeTypIDComboBox.SelectedIndex < 0 Then
                Ctrl = TradeTypIDComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorTradeType ' "Bitte geben Sie eine Transaktions-Art an!"
            ElseIf QuellKontoComboBox.SelectedIndex < 0 Then
                Ctrl = QuellKontoComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorSourceAccount ' "Bitte geben Sie ein Quell-Konto an!"
            ElseIf ZielKontoComboBox.SelectedIndex < 0 Then
                Ctrl = ZielKontoComboBox
                ErrorText = My.Resources.MyStrings.ctrlTradeCoreDataErrorTargetAccount ' "Bitte geben Sie ein Ziel-Konto an!"
            End If
            If Ctrl IsNot Nothing Then
                ErrProvider.Clear()
                e.Cancel = True
                ErrProvider.SetError(Ctrl, ErrorText)
            Else
                If QuellBetragTextBox.Text.Trim.Length = 0 Then
                    QuellBetragTextBox.Text = "0"
                End If
                If QuellBetragNachGebuehrTextBox.Text.Trim.Length = 0 Then
                    QuellBetragNachGebuehrTextBox.Text = "0"
                End If
                If ZielBetragTextBox.Text.Trim.Length = 0 Then
                    ZielBetragTextBox.Text = "0"
                End If
                If BetragNachGebuehrTextBox.Text.Trim.Length = 0 Then
                    BetragNachGebuehrTextBox.Text = "0"
                End If
                If WertEURTextBox.Text.Trim.Length = 0 Then
                    WertEURTextBox.Text = "0"
                End If
                If InZeitpunktDateTimePicker.Value > ZeitpunktZielDateTimePicker.Value Then
                    InZeitpunktDateTimePicker.Value = ZeitpunktZielDateTimePicker.Value.Date()
                End If
                If Not QuellPlattformIDComboBox.Visible AndAlso Not IsNothing(ImportPlattformIDComboBox.SelectedValue) Then
                    ' Make sure the source and target platforms are identical to the import platform
                    QuellPlattformIDComboBox.SelectedValue = ImportPlattformIDComboBox.SelectedValue
                    ZielPlattformIDComboBox.SelectedValue = ImportPlattformIDComboBox.SelectedValue
                End If
                    If SourceIDTextBox.Text.Trim.Length = 0 Then
                        SourceIDTextBox.Text = MD5FromString(ImportPlattformIDComboBox.SelectedIndex &
                                                         ZeitpunktDateTimePicker.ToString &
                                                         QuellPlattformIDComboBox.SelectedIndex &
                                                         QuellBetragTextBox.Text.Trim &
                                                         QuellKontoComboBox.SelectedIndex &
                                                         ZielPlattformIDComboBox.SelectedIndex &
                                                         ZielBetragTextBox.Text.Trim &
                                                         ZielKontoComboBox.SelectedIndex)
                    End If
                End If
            End If
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _IsLoading = False

    End Sub

    ''' <summary>
    ''' Checks if the form is an a state where automatically filled controls by some input logic applies
    ''' </summary>
    ''' <returns>True, if auto sync of controls is applicable, False otherwise</returns>
    Private Function CheckStateManualInput() As Boolean
        Return Not _IsLoading AndAlso TradesBindingSource.Current IsNot Nothing AndAlso TradesBindingSource.Current.Row.RowState = DataRowState.Added
    End Function

    Private Sub ImportPlattformIDComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ImportPlattformIDComboBox.SelectedIndexChanged
        ' Check if the other controls can be kept in sync
        If CheckStateManualInput() Then
            If Not QuellPlattformIDComboBox.Touched Then QuellPlattformIDComboBox.SelectedIndex = DirectCast(sender, TouchedComboBox).SelectedIndex
            If Not ZielPlattformIDComboBox.Touched Then ZielPlattformIDComboBox.SelectedIndex = DirectCast(sender, TouchedComboBox).SelectedIndex
        End If
    End Sub

    Private Sub ZeitpunktDateTimePicker_ValueChanged(sender As Object, e As EventArgs) Handles ZeitpunktDateTimePicker.ValueChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not ZeitpunktZielDateTimePicker.Touched Then ZeitpunktZielDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value
            If Not InZeitpunktDateTimePicker.Touched Then InZeitpunktDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value.Date
        End If
    End Sub

    Private Sub ZeitpunktZielDateTimePicker_ValueChanged(sender As Object, e As EventArgs) Handles ZeitpunktZielDateTimePicker.ValueChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not ZeitpunktDateTimePicker.Touched Then ZeitpunktDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value
            If Not InZeitpunktDateTimePicker.Touched Then InZeitpunktDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value.Date
        End If
    End Sub

    Private Sub InZeitpunktDateTimePicker_ValueChanged(sender As Object, e As EventArgs) Handles InZeitpunktDateTimePicker.ValueChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not ZeitpunktDateTimePicker.Touched Then ZeitpunktDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value
            If Not ZeitpunktZielDateTimePicker.Touched Then ZeitpunktZielDateTimePicker.Value = DirectCast(sender, TouchedDateTimePicker).Value
        End If
    End Sub

    Private Sub QuellBetragTextBox_TextChanged(sender As Object, e As EventArgs) Handles QuellBetragTextBox.TextChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not QuellBetragNachGebuehrTextBox.Touched Then QuellBetragNachGebuehrTextBox.Text = DirectCast(sender, TouchedTextBox).Text
        End If
    End Sub

    Private Sub QuellBetragNachGebuehrTextBox_TextChanged(sender As Object, e As EventArgs) Handles QuellBetragNachGebuehrTextBox.TextChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not QuellBetragTextBox.Touched Then QuellBetragTextBox.Text = DirectCast(sender, TouchedTextBox).Text
        End If
    End Sub

    Private Sub ZielBetragTextBox_TextChanged(sender As Object, e As EventArgs) Handles ZielBetragTextBox.TextChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not BetragNachGebuehrTextBox.Touched Then BetragNachGebuehrTextBox.Text = DirectCast(sender, TouchedTextBox).Text
        End If
    End Sub

    Private Sub BetragNachGebuehrTextBox_TextChanged(sender As Object, e As EventArgs) Handles BetragNachGebuehrTextBox.TextChanged
        ' Check if the other control can be kept in sync
        If CheckStateManualInput() Then
            If Not ZielBetragTextBox.Touched Then ZielBetragTextBox.Text = DirectCast(sender, TouchedTextBox).Text
        End If
    End Sub

    Private Sub TradeTypIDComboBox_SelectedValueChanged(sender As Object, e As EventArgs) Handles TradeTypIDComboBox.SelectedValueChanged
        ' Hide to-and-from platform controls in cases we don't need these
        Try
            Dim TradeType As Integer = DirectCast(sender, TouchedComboBox).SelectedValue
            Dim IsMultiPlatform As Boolean = (TradeType = DBHelper.TradeTypen.Undefiniert) _
                                             OrElse (TradeType = DBHelper.TradeTypen.Einzahlung) _
                                             OrElse (TradeType = DBHelper.TradeTypen.Auszahlung) _
                                             OrElse (TradeType = DBHelper.TradeTypen.Transfer)
            QuellPlattformIDComboBox.Visible = IsMultiPlatform
            ZielPlattformIDComboBox.Visible = IsMultiPlatform
            QuellPlattformIDLabel.Visible = IsMultiPlatform
            ZielPlattformIDLabel.Visible = IsMultiPlatform
        Catch ex As Exception
            ' Don't mind...
            Debug.Print(ex.ToString)
        End Try
    End Sub
End Class

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

Imports CoinTracer.DBHelper
Imports System.IO
Imports System.Environment
Imports System.Net

Public Class frmMain

#Region "Class TaxCalculationSettings"

    ''' <summary>
    ''' Helper class for managing tax calculation settings
    ''' </summary>
    Public Class TaxCalculationSettings

        Private _frmMain As frmMain
        Private _LoadedLongTermPeriod As String
        Private _LoadedCVS As String
        Private _LoadedCoins4CoinsAccounting As Boolean
        Private _LoadedToleranceMinutes As Long
        Private _LoadedWalletAware As Boolean
        Private _AvoidCascade As Boolean

        Public Sub New(FormInstance As frmMain)
            _AvoidCascade = False
            _frmMain = FormInstance
        End Sub

        ''' <summary>
        ''' Gibt die aktuellen Einstellungen des CoinValueStrategie-Selektors als String zurück und schreibt diese optional in My.Settings
        ''' </summary>
        ''' <returns>String mit den Einstellungen aller Reiter</returns>
        Public Overrides Function ToString() As String
            Dim AllVals As String = ""
            AllVals = _frmMain.vssGlobalStrategy.GetValues.ToString & "|" &
            _frmMain.dpctlHaltefrist.ToString & "|" &
            WalletAware.ToString & "|" &
            Coins4CoinsAccounting.ToString & "|" &
            ToleranceMinutes.ToString
            Return AllVals
        End Function

        ''' <summary>
        ''' Set controls of current form according to serialized settings string
        ''' </summary>
        ''' <param name="AllCoinValueStrategies">String containing the settings of all ValueStrategySelectors, followed by general settings: Long term period, wallet awareness and handling of coins vs coins</param>
        Public Sub FromString(AllCoinValueStrategies As String)
            Dim CVS() As String
            CVS = Split(AllCoinValueStrategies, "|")
            _AvoidCascade = True
            If DirectCast(CVS, ICollection).Count >= 6 Then
                _frmMain.vssGlobalStrategy.SetValues(CVS(0))
                If DirectCast(CVS, ICollection).Count > 6 Then
                    _frmMain.dpctlHaltefrist.FromString(CVS(6))
                Else
                    _frmMain.dpctlHaltefrist.FromString("+1 years")
                End If
                If DirectCast(CVS, ICollection).Count > 7 Then
                    WalletAware = CVS(7)
                    _LoadedWalletAware = CVS(7)
                    Coins4CoinsAccounting = CVS(8)
                    _LoadedCoins4CoinsAccounting = CVS(8)
                Else
                    WalletAware = True
                    _LoadedWalletAware = True
                    Coins4CoinsAccounting = False
                    _LoadedCoins4CoinsAccounting = False
                End If
                If DirectCast(CVS, ICollection).Count > 9 Then
                    ToleranceMinutes = CVS(9)
                Else
                    ToleranceMinutes = 10       ' Default value!
                End If
            ElseIf DirectCast(CVS, ICollection).Count = 5 Then
                ' new, stripped features (sincs 2020-12)
                _LoadedWalletAware = CVS(2)
                _LoadedCoins4CoinsAccounting = CVS(3)
                WalletAware = CVS(2)
                Coins4CoinsAccounting = CVS(3)
                _LoadedToleranceMinutes = CVS(4)
                ToleranceMinutes = CVS(4)
                _LoadedCVS = CVS(0)
                _frmMain.vssGlobalStrategy.SetValues(CVS(0))
                _LoadedLongTermPeriod = CVS(1)
                _frmMain.dpctlHaltefrist.FromString(CVS(1))
            End If
            _AvoidCascade = False
        End Sub

        ''' <summary>
        ''' Write current settings to My.Settings
        ''' </summary>
        Public Function WriteSettings() As String
            Dim Settings As String = ToString()
            My.Settings.CoinValueStrategies = Settings
            Return Settings
        End Function

        ''' <summary>
        ''' Read current settings from My.Settings and set controls accordingly
        ''' </summary>
        Public Sub ReadSettings()
            Dim SettingsString As String = Trim(My.Settings.CoinValueStrategies)
            If SettingsString.Length = 0 Then
                SettingsString = ToString()
            End If
            FromString(SettingsString)
        End Sub

        ''' <summary>
        ''' How long (in minutes) may incoming and outgoing coins overlap and still be regared as belonging together?
        ''' </summary>
        Private _ToleranceMinutes As Long
        Public Property ToleranceMinutes() As Long
            Get
                Return _ToleranceMinutes
            End Get
            Set(ByVal value As Long)
                _ToleranceMinutes = value
            End Set
        End Property

        Public Property WalletAware() As Boolean
            Get
                Return _frmMain.cbxWalletAware.SelectedIndex = 0 Or _frmMain.cbxWalletAware.SelectedIndex = -1
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    _frmMain.cbxWalletAware.SelectedIndex = 0
                Else
                    _frmMain.cbxWalletAware.SelectedIndex = 1
                End If
            End Set
        End Property

        Public Property Coins4CoinsAccounting() As Boolean
            Get
                Return _frmMain.cbxCoins4CoinsAccounting.SelectedIndex = 1
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    _frmMain.cbxCoins4CoinsAccounting.SelectedIndex = 1
                Else
                    _frmMain.cbxCoins4CoinsAccounting.SelectedIndex = 0
                End If
            End Set
        End Property

        Public ReadOnly Property LongTermPeriodSQL() As String
            Get
                Return _frmMain.dpctlHaltefrist.ToString
            End Get
        End Property

        Public Function CoinValueStrategy() As CoinValueStrategy
            Dim Result = New CoinValueStrategy(_frmMain.vssGlobalStrategy.GetValues.ToString)
            Result.WalletAware = WalletAware
            Result.Coin4CoinAware = Coins4CoinsAccounting
            Return Result
        End Function

        Public ReadOnly Property Sticky() As Boolean
            Get
                If _AvoidCascade Then
                    Return False
                Else
                    Return (_LoadedCoins4CoinsAccounting <> Coins4CoinsAccounting) _
                        Or (_LoadedWalletAware <> WalletAware) _
                        Or (_LoadedToleranceMinutes <> ToleranceMinutes) _
                        Or (_LoadedLongTermPeriod <> LongTermPeriodSQL) _
                        Or (_LoadedCVS <> _frmMain.vssGlobalStrategy.GetValues.ToString)
                End If
            End Get
        End Property
    End Class

#End Region

#Region "Scale-Awareness Support"

    Private currentScaleFactor As SizeF = New SizeF(1.0F, 1.0F)

    Protected Overrides Sub ScaleControl(factor As SizeF, specified As BoundsSpecified)
        MyBase.ScaleControl(factor, specified)
        ' Record the running scale factor used
        Me.currentScaleFactor.Width *= factor.Width
        Me.currentScaleFactor.Height *= factor.Height
        ' check, if text on buttons fits into width
        For Each btn As PaddingButton In New List(Of PaddingButton) From {cmdCourses}
            With btn
                Dim graphics As Graphics = .CreateGraphics()
                If graphics.MeasureString(.PaddingText, .Font).Width > .Width - .Padding.Left - .Padding.Right Then
                    .Width = graphics.MeasureString(.PaddingText, .Font).Width + .Padding.Left + .Padding.Right
                End If
            End With
        Next
        ' control specific positioning and sizing
        If factor.Width <> 1 Or factor.Height <> 1 Then

        End If
    End Sub

#End Region

    ' Tread-Manager inkl. der verwendeten Job-Arten
    Private WithEvents _TML As ThreadManagerLight
    Private Const TML_JOB_FETCHCOURSES As String = "FetchCourses"
    Private Const TML_JOB_CHECKFORUPDATE As String = "CheckForUpdate"

    Private ShownWarnings As Integer

    ''' <summary>
    ''' Gibt den Thread-Manager der Formulars zurück.
    ''' </summary>
    ''' <returns>TML-Objekt as ThreadManagerLight</returns>
    Public ReadOnly Property TML() As ThreadManagerLight
        Get
            Return _TML
        End Get
    End Property


    Private _DB As DBHelper
    Public ReadOnly Property CointracerDatabase() As DBHelper
        Get
            Return _DB
        End Get
    End Property

    Private _cnn As SQLite.SQLiteConnection
    Public Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
        Set(value As SQLite.SQLiteConnection)
            _cnn = value
        End Set
    End Property

    Private _DBName As String
#If CONFIG = "Debug" Then
    Public Property DatabaseFilename() As String
        Get
            Return _DBName
        End Get
        Set(value As String)
            _DBName = value
        End Set
    End Property
#End If

    Private _TCS As TaxCalculationSettings
    Public ReadOnly Property TaxReportSettings() As TaxCalculationSettings
        Get
            Return _TCS
        End Get
    End Property

    Private _BckGrndImg As Bitmap

    Private _LatestReportFilter As String

    Private _LastImportID As Long

    Private WithEvents _TVM As TradeValueManager
    Public ReadOnly Property TradeValueManager() As TradeValueManager
        Get
            Return _TVM
        End Get
    End Property

    Private _CM As CourseManager
    Public ReadOnly Property CourseManager() As CourseManager
        Get
            Return _CM
        End Get
    End Property

    Private _RP As CTReport
    Public ReadOnly Property Report() As CTReport
        Get
            Return _RP
        End Get
    End Property

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Einige Einstellungen in My.Settings speichern
        My.Settings.Layout_SplitterDistance1 = CLng(spltCntDashboard.SplitterDistance * 1000 / spltCntDashboard.Width)
        My.Settings.Layout_SplitterDistance2 = CLng(spltCntGainings.SplitterDistance * 1000 / spltCntGainings.Height)
        _TCS.WriteSettings()
        My.Settings.LastCvsScenarioID = cbxSzenario.SelectedIndex
        My.Settings.ReportDetail1 = cbxReportTransfers.SelectedIndex.ToString
        If cbxReportTradeSelection.SelectedIndex = 1 Then
            My.Settings.ReportDetail2 = "1"
        Else
            My.Settings.ReportDetail2 = "0"
        End If
        My.Settings.TaxNumber = tbxTaxNumber.Text.Trim
        My.Settings.UserName = tbxUserName.Text.Trim
        My.Settings.ReportComment = tbxReportAdvice.Text.Trim
        If ccbReportPlatforms.AllChecked Then
            My.Settings.ReportLastPlatforms = ""
        Else
            My.Settings.ReportLastPlatforms = ccbReportPlatforms.GetCheckedItemsIDs()
        End If

        ' Hintergrund-Threads schließen
        If _TML IsNot Nothing Then
            _TML.AbortThread()
        End If
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#If CONFIG = "Release" Then
        Me.Button1.Visible = False
#End If

        ' Initialize logging
        My.Application.Log.DefaultFileLogWriter.CustomLocation = GetFolderPath(SpecialFolder.ApplicationData) & "\" & Application.ProductName

        ' Labels & Co.
        lblDonate.Text = String.Format(My.Resources.MyStrings.mainDonateAdvice)
        Me.Text = Application.ProductName & " " & Application.ProductVersion & " (public beta)"
        PlatformManager.LoadImportComboBox(cbxImports, False)

        ' Reset number of shown warnings
        ShownWarnings = 0

        ' Restore settings from My.Settings
        Try
            Dim SplitDis As Long = My.Settings.Layout_SplitterDistance1 * spltCntDashboard.Width / 1000
            If SplitDis > 100 Then
                spltCntDashboard.SplitterDistance = SplitDis
            Else
                spltCntDashboard.SplitterDistance = 613
            End If
            SplitDis = My.Settings.Layout_SplitterDistance2 * spltCntGainings.Height / 1000
            If SplitDis > 50 Then
                spltCntGainings.SplitterDistance = SplitDis
            Else
                spltCntGainings.SplitterDistance = 159
            End If
            ' Initialize values for transfer detection
            TransferDetection.Init()
        Catch ex As Exception
            ' Don't care...
        End Try
        If My.Settings.LastImportMethod >= 0 AndAlso My.Settings.LastImportMethod < cbxImports.Items.Count Then
            cbxImports.SelectedIndex = My.Settings.LastImportMethod
        End If

        ' Init tax report settings
        _TCS = New TaxCalculationSettings(Me)

        If IsNumeric(My.Settings.ReportDetail1) Then
            cbxReportTransfers.SelectedIndex = My.Settings.ReportDetail1
        Else
            cbxReportTransfers.SelectedIndex = 0
        End If
        If My.Settings.ReportDetail2 = "1" Then
            cbxReportTradeSelection.SelectedIndex = 1
        Else
            cbxReportTradeSelection.SelectedIndex = 0
        End If
        tbxTaxNumber.Text = My.Settings.TaxNumber
        tbxUserName.Text = My.Settings.UserName
        tbxReportAdvice.Text = My.Settings.ReportComment

        ' Initialize database and update it if needed
        Try
            Dim DBInitialize As New DBInit
            With DBInitialize
                .InitDatabase(_DBName)
                .UpdateDatabase()
                _cnn = .Connection
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message, True)
            Exit Sub
        End Try

        _DB = New DBHelper(Me.Connection)
        _CM = New CourseManager(Me.Connection)

        ' TradeValueManager initialisieren
        _TVM = New TradeValueManager(_DB.Connection, Me)

        ' Thread-Manager initialisieren
        _TML = New ThreadManagerLight(ThreadsafeStatusStrip1)

        ' Report-Helper initialisieren
        _RP = New CTReport(Me.Connection)

        ' Haltefrist-Control an Datenbank anbinden
        dpctlHaltefrist.InitData(Me.Connection)

        RefreshCourseDisplays()

        ' Zeitauswahl einheitlich voreinstellen
        tpDashboard.InitializeTimeSettings()
        tpGainings.InitializeTimeSettings()
        tpReport.InitializeTimeSettings()

        ' Übersichts-Grids initialisieren
        With dshgrdBestaende
            .LinkDatabase(_DB)
            .LinkTimePeriodControl(tpDashboard)
        End With
        With dshgrdAbgaenge
            .LinkDatabase(_DB)
            .LinkTimePeriodControl(tpDashboard)
        End With
        ReloadDashGrids(True)

        With grdDataTimes
            .AutoGenerateColumns = False
            .Columns(0).DataPropertyName = "Wallet"
            .Columns(1).DataPropertyName = "Zeitpunkt"
        End With
        FillDataTimesGrid()

        ' Gewinn-Displays initialisieren & aktualisieren
        With gnd1stTab
            .LinkTimePeriodControl(tpDashboard)
            .LinkTradeValueManager(_TVM)
        End With
        With gnd2ndTab
            .LinkTimePeriodControl(tpGainings)
            .LinkTradeValueManager(_TVM)
        End With
        With gnd3rdTab
            .LinkTimePeriodControl(tpReport)
            .LinkTradeValueManager(_TVM)
        End With

        ' Initiales Anzeigen des letzten Gaining-Cut-Off-Days
        ' TODO: Check, if we still need this?
        ' _TVM_GainingsCutOffDayChanged(_TVM, New GainingsCutOffDayEventArgs(_TVM.GetGainingsCutOffDay))

        ' Generierung von Spalten in ausgewählten DataGridViews ausschalten
        grdReport.AutoGenerateColumns = False
        DataGridViewDoubleBuffer(grdReport)

        ' Tabellen-Grids initialisieren
        grdTrades.BindGrid(New CoinTracerDataSetTableAdapters.VW_TradesTableAdapter())
        grdImporte.BindGrid(New CoinTracerDataSetTableAdapters.VW_ImporteTableAdapter())
        grdKonten.BindGrid(New CoinTracerDataSetTableAdapters.VW_KontenTableAdapter())
        grdPlattformen.BindGrid(New CoinTracerDataSetTableAdapters.VW_PlattformenTableAdapter())
        grdKurse.BindGrid(New CoinTracerDataSetTableAdapters.VW_KurseTableAdapter())
        grdBerechnungen.BindGrid(New CoinTracerDataSetTableAdapters.VW_BerechnungenTableAdapter())

        ' Szenario-Combobox initialisieren
        cbxSzenario.Initialize(_cnn, "select 0 as Class, 0 as ID, 'Standard' as Bezeichnung, CVS from Szenarien where ID = 0 " &
                    "union all select 1, ID, Bezeichnung, CVS from Szenarien where ID > 0 order by Class, Bezeichnung",
                    , , cmdSzenarioSave, cmdSzenarioDelete)
        If My.Settings.LastCvsScenarioID >= 0 AndAlso cbxSzenario.Items.Count > My.Settings.LastCvsScenarioID Then
            cbxSzenario.SelectedIndex = My.Settings.LastCvsScenarioID
        Else
            cbxSzenario.SelectedIndex = 0
        End If

        ' Plattformen-CheckedComboBox initialisieren
        With ccbReportPlatforms
            .Initialize(_cnn, "select ID, Bezeichnung from " &
                        "(select 0 ID, '- alle Börsen -' Bezeichnung, -1 SortID, 1 Boerse " &
                        "union select ID, Bezeichnung, SortID, Boerse from Plattformen where Boerse=1 order by SortID)", , , True)
            .MaxDropDownItems = 20
            ReloadPlatformsCbx()
            If My.Settings.ReportLastPlatforms.Length = 0 Then
                ' Noch kein Parameter in My.Settings - also alle Plattformen anhaken
                .SetItemListCheckState("", CheckState.Unchecked, CheckState.Checked)
            Else
                ' ...ansonsten nur die bisher Angehakten wieder setzen
                .SetItemListCheckState(My.Settings.ReportLastPlatforms, CheckState.Checked, CheckState.Unchecked)
            End If
            EnhancedToolTip1.SetToolTip(ccbReportPlatforms, .Text)
        End With

        ' Read tax report settings from the settings (thus overriding the scenario settings, which could have been overwritten since last exit)
        _TCS.ReadSettings()


        ' Set LastImportID (just for cmdTransfers...)
        _LastImportID = 0

        ' Online/Offline-Mode verarbeiten
        HandleOfflineMode()

        ' Hintergrundbild für TabPages laden
        _BckGrndImg = My.Resources.ct_icon_clp_lgt

        ' Gewünschte Prozesse zum Start initiieren
        StartInitialBackgroundActions()

    End Sub

    ''' <summary>
    ''' Lädt die Combobox für Szenarien neu
    ''' </summary>
    Private Sub ReloadScenarioCbx()
        cbxSzenario.Reload()
    End Sub

    ''' <summary>
    ''' Lädt die Plattformen-CheckedComboBox neu und setzt den Tooltip entsprechend der selektierten Plattformen
    ''' </summary>
    Private Sub ReloadPlatformsCbx()
        ccbReportPlatforms.Reload(True)
    End Sub

    ''' <summary>
    ''' Schaltet die Buttons für Speichern und Löschen von Szenarien je nach Kontext an oder ab
    ''' </summary>
    Private Sub DisEnableScenarioButtons()
        cmdSzenarioDelete.Enabled = cbxSzenario.SelectedIndex <> 0
        cmdSzenarioSave.Enabled = Not cbxSzenario.Items.Contains(cbxSzenario.Text)
    End Sub

    ''' <summary>
    ''' Zeigt den Datenstand der Wechselkurstabelle(n) in den zugehörigen Labels an
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshCourseDisplays(Optional NumberOfAddedDays As Long = -1)
        Dim LastDate As Date
        Dim Msg As String
        LastDate = _CM.GetCoursesCutOffDay(Konten.EUR, Konten.USD)
        If LastDate = DATENULLVALUE Then
            _TML.SetControlText(lblCourseUSD, My.Resources.MyStrings.mainNoDataAvailable)
        Else
            _TML.SetControlText(lblCourseUSD, String.Format(My.Resources.MyStrings.mainDataAvailableTo, CDate(LastDate).ToString("dd.MM.yyyy")))
        End If
        If NumberOfAddedDays >= 0 Then
            If NumberOfAddedDays > 1 Then
                Msg = String.Format(My.Resources.MyStrings.mainCoursesAddedMany, NumberOfAddedDays)
            ElseIf NumberOfAddedDays = 1 Then
                Msg = My.Resources.MyStrings.mainCoursesAddedOne
            Else
                Msg = My.Resources.MyStrings.mainCoursesAddedNothing
            End If
            MessageBox.Show(Msg, My.Resources.MyStrings.mainUpdateCoursesLabel, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ''' <summary>
    ''' Lädt die Dashboard-Grids neu; ggf. inkl. Setzen der SQL-Statements für das 
    ''' Ein- und Ausblenden von Plattform-Informationen
    ''' </summary>
    ''' <param name="Initialize">True, wenn die Dashgrids (erneut) initialisiert werden sollen</param>
    ''' <param name="InitWithPlatforms">True, wenn Plattform-Informationen angezeigt werden sollen</param>
    ''' <remarks></remarks>
    Private Sub ReloadDashGrids(Optional Initialize As Boolean = False,
                                Optional InitWithPlatforms As Boolean = False)
        If Initialize Then
            If InitWithPlatforms Then
                ' Anzeige von Plattform-Informationen
                With dshgrdBestaende
                    .SetSQL(String.Format("select k.IstFiat, case k.IstFiat when 1 then 'Währung' else 'Coins' end Art, k.Bezeichnung, " &
                            "case p.Eigen when 1 then p.Bezeichnung else '(' || p.Bezeichnung || ')' end PlattformBezeichnung, " &
                            "round(sum(case SollHaben when 1 then Betrag else 0 end),8) Haben, " &
                            "round(sum(case SollHaben when 0 then Betrag else 0 end),8) Soll, " &
                            "round(sum(Betrag),8) Bestand, " &
                            "round(sum(case SollHaben when 1 then ifnull(Betrag{0},0) else 0 end) / sum(case when SollHaben = 1 and Betrag{0} is not Null then _BetragNetto else 0 end),2) Kaufpreis{0}, " &
                            "round(sum(case SollHaben when 0 then ifnull(Betrag{0},0) else 0 end) / sum(case when SollHaben = 0 and Betrag{0} is not Null then Betrag else 0 end),2) Verkaufspreis{0} " &
                            "from VW_ZugangAbgang d left join Konten k on d.KontoID = k.ID " &
                            "left join Plattformen p on d.Plattform = p.ID", My.Settings.InventoryPricesCurrency),
                            "k.Eigen=1", "KontoID, d.Plattform", "k.IstFiat, k.SortID, p.SortID")
                    .Reload()
                    .Columns("Plattform").Visible = True
                    .Columns("Art").Visible = False
                End With
                With dshgrdAbgaenge
                    .SetSQL(String.Format("select k.IstFiat, case k.IstFiat when 1 then 'Währung' else 'Coins' end Art, k.Bezeichnung, p.Bezeichnung, round(sum(case SollHaben when 1 then Betrag else 0 end),8) Haben, " &
                        "round(sum(case SollHaben when 0 then Betrag else 0 end),8) Soll, round(sum(Betrag),8) Bestand, " &
                        "NULL Kauf{0}, NULL Verkauf{0} " &
                        "from VW_ZugangAbgang d left join Konten k on d.KontoID = k.ID " &
                        "left join Plattformen p on d.Plattform = p.ID", My.Settings.InventoryPricesCurrency),
                        "not k.Eigen", "KontoID, d.Plattform", "k.IstFiat, k.SortID, p.SortID")
                    .Reload()
                    .Columns("Plattform").Visible = True
                    .Columns("Art").Visible = False
                    .Columns(7).Visible = False
                    .Columns(8).Visible = False
                End With
            Else
                ' Nur Bestände, ohne Plattform-Informationen
                With dshgrdBestaende
                    .SetSQL(String.Format("select k.IstFiat, case k.IstFiat when 1 then 'Währung' else 'Coins' end Art, k.Bezeichnung, '', round(sum(case SollHaben when 1 then Betrag else 0 end),8) Haben, " &
                        "round(sum(case SollHaben when 0 then Betrag else 0 end),8) Soll, " &
                        "round(sum(Betrag),8) Bestand, " &
                        "round(sum(case SollHaben when 1 then ifnull(Betrag{0},0) else 0 end) / sum(case when SollHaben = 1 and Betrag{0} is not Null then _BetragNetto else 0 end),2) Kaufpreis{0}, " &
                        "round(sum(case SollHaben when 0 then ifnull(Betrag{0},0) else 0 end) / sum(case when SollHaben = 0 and Betrag{0} is not Null then Betrag else 0 end),2) Verkaufspreis{0} " &
                        "from VW_ZugangAbgang d left join Konten k on d.KontoID = k.ID " &
                        "left join Plattformen p on d.Plattform = p.ID", My.Settings.InventoryPricesCurrency),
                        "k.Eigen=1 and p.Eigen=1", "KontoID", "k.IstFiat, k.SortID")
                    .Reload()
                    .Columns("Plattform").Visible = False
                    .Columns("Art").Visible = True
                End With
                With dshgrdAbgaenge
                    .SetSQL("select k.IstFiat, case k.IstFiat when 1 then 'Währung' else 'Coins' end Art, k.Bezeichnung, '', round(sum(case SollHaben when 1 then Betrag else 0 end),8) Haben, " &
                        "round(sum(case SollHaben when 0 then Betrag else 0 end),8) Soll, round(sum(Betrag),8) Bestand, " &
                        "NULL KaufEUR, NULL VerkaufEUR " &
                        "from VW_ZugangAbgang d left join Konten k on d.KontoID = k.ID",
                        "not k.Eigen", "KontoID", "k.IstFiat, k.SortID")
                    .Reload()
                    .Columns("Plattform").Visible = False
                    .Columns("Art").Visible = True
                    .Columns(7).Visible = False
                    .Columns(8).Visible = False
                End With
            End If
        Else
            dshgrdAbgaenge.Reload()
            dshgrdBestaende.Reload()
        End If
    End Sub

    ''' <summary>
    ''' Befüllt/aktualisiert das DataTimes-Grid, also die Tabelle der jüngsten Transaktionen je Plattform/Wallet.
    ''' Außerdem Aktualisierung der Anzahl offener Trades
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillDataTimesGrid()
        _DB.FillDataGridView("select p.Bezeichnung Wallet, strftime('%d.%m.%Y %H:%M:%S', max(t.Zeitpunkt)) Zeitpunkt from Trades t left join Plattformen p on t.ImportPlattformID = p.ID group by p.ID order by p.SortID",
                             grdDataTimes)
        RefreshOpenTransfers()
    End Sub

    Private Sub cmdImport_Click(sender As Object, e As EventArgs) Handles cmdImport.Click

        Dim Fiat As Boolean = False

        If cbxImports.SelectedIndex >= 0 Then
            Dim SelectedPlatform As PlatformManager.Platforms = PlatformManager.GetPlatformFromComboBox(cbxImports, Fiat)
            If Fiat OrElse SelectedPlatform <> PlatformManager.Platforms.Invalid Then
                My.Settings.LastImportMethod = cbxImports.SelectedIndex
                If Fiat Then
                    ' Get fiat course data (USD)
                    Try
                        Dim NewDays As Long = 0
                        Dim CH As New CourseManager(_cnn)
                        RefreshCourseDisplays(CH.ImportCoursesEurUsd())
                    Catch ex As Exception
                        DefaultErrorHandler(ex)
                    End Try
                Else
                    If SelectedPlatform <> PlatformManager.Platforms.Invalid Then
                        ' Perform the platform import
                        PerformFileImport(SelectedPlatform, Nothing)
                    Else
                        ' No valid platform selected
                        MessageBox.Show(My.Resources.MyStrings.mainMsgSelectImportMethod,
                            My.Resources.MyStrings.mainMsgSelectImportMethodTitle, MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
                    End If
                End If
            End If
        Else
            MessageBox.Show(My.Resources.MyStrings.mainMsgSelectImportMethod,
                            My.Resources.MyStrings.mainMsgSelectImportMethodTitle, MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
            Exit Sub
        End If

    End Sub

    ''' <summary>
    ''' Initiates a platform import. This is called by the command button handler and by the DragDrop event handler
    ''' </summary>
    Private Sub PerformFileImport(SelectedPlatform As PlatformManager.Platforms,
                                  ByRef FilesList() As String)
        ' Perform the platform import
        Dim TradeImport As Import = New Import(_DB, _TVM, Me)
        TradeImport.Plattform = SelectedPlatform
        TradeImport.DoImport(FilesList)
        RefreshAfterImport(TradeImport)
    End Sub

    ''' <summary>
    ''' Aktualisiert die Anzeigen nach einem durchgeführten Import (API oder manuell)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshAfterImport(ByRef TradeImport As Import)
        FillDataTimesGrid()
        dshgrdBestaende.Reload()
        dshgrdAbgaenge.Reload()
        If TradeImport.LastTransfersInserted > 0 Then
            If MsgBoxEx.ShowWithNotAgainOption("GoToTransfersAfterImport", Windows.Forms.DialogResult.No,
                                               My.Resources.MyStrings.mainMsgNewTransfers,
                                               My.Resources.MyStrings.mainMsgNewTransfersTitle,
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                               MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                _LastImportID = 1
                cmdTransfers_Click(Nothing, Nothing)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Alle Bewegungsdaten aus der Datenbank löschen!
    ''' </summary>
    Private Sub mnuEraseDB_Click(sender As Object, e As EventArgs) Handles EraseDBToolStripMenuItem.Click
        Dim DlgTitle As String = My.Resources.MyStrings.mainMsgDeleteDBTitle
        If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgDeleteDB1, Environment.NewLine),
                        DlgTitle, MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
            If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgDeleteDB2, Environment.NewLine),
                        DlgTitle, MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                Try
                    Dim TableNames As String() = New String() {"Importe", "Kalkulationen", "Trades", "TradesWerte", "TradeTx"}
                    With _DB
                        For Each Table In TableNames
                            .ExecuteSQL(String.Format("DELETE FROM [{0}]", Table))
                            .ExecuteSQL(String.Format("DELETE FROM sqlite_sequence where [name] = '{0}'", Table))
                        Next
                        .ExecuteSQL("UPDATE Plattformen SET IstDown = 0, DownSeit = NULL")
                        .ExecuteSQL("VACUUM;")
                        Dim InitDB As New DBInit(_cnn)
                        InitDB.InitCourseData()
                        .ReloadDatabase(True)
                    End With
                Catch ex As Exception
                    DefaultErrorHandler(ex, ex.Message)
                End Try
                ReloadDashGrids()
                RefreshCourseDisplays()
                FillDataTimesGrid()
                ClearReportsGrid()
                ReloadTablesTab()
                _TVM_GainingsCutOffDayChanged(_TVM, New GainingsCutOffDayEventArgs(_TVM.GetGainingsCutOffDay))
                MessageBox.Show(My.Resources.MyStrings.mainMsgDeleteDB3, DlgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Completely restore the database!
    ''' </summary>
    Private Sub mnuNewDB_Click(sender As Object, e As EventArgs) Handles NewDBToolStripMenuItem.Click
        Dim DlgTitle As String = My.Resources.MyStrings.mainMsgNewDBTitle
        If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgNewDB1, Environment.NewLine),
                        DlgTitle, MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
            If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgNewDB2, Environment.NewLine),
                        DlgTitle, MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                Try
                    Cursor.Current = Cursors.WaitCursor
                    Dim DBInit As New DBInit
                    ' Just in case: backup current database file
                    My.Computer.FileSystem.CopyFile(DBInit.DatabaseFile, DBInit.DatabaseFile & ".bak", True)
                    ' write the default db file
                    File.WriteAllBytes(DBInit.DatabaseFile & ".default", My.Resources.cointracerDefault)
                    My.Computer.FileSystem.CopyFile(DBInit.DatabaseFile & ".default", DBInit.DatabaseFile, True)
                    ' activate database
                    DBInit.InitDatabase()
                    frmMain_Load(Me, New EventArgs)
                    ReloadTablesTab()
                    Cursor.Current = Cursors.Default
                    MessageBox.Show(My.Resources.MyStrings.mainMsgNewDBSuccess,
                                    My.Resources.MyStrings.mainMsgNewDBSuccessTitle,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    Cursor.Current = Cursors.Default
                    DefaultErrorHandler(ex, String.Format(My.Resources.MyStrings.mainMsgNewDBError, NewLine, ex.Message))
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' Aktualisiert die Anzeigen, wenn sich der Berechnungstag geändert hat
    ''' </summary>
    Private Sub _TVM_GainingsCutOffDayChanged(sender As Object, e As GainingsCutOffDayEventArgs) Handles _TVM.GainingsCutOffDayChanged
        Dim OldDate As Date
        gnd1stTab.Reload(True)
        gnd2ndTab.CloneFrom(gnd1stTab)
        gnd3rdTab.CloneFrom(gnd1stTab)
        OldDate = e.GainingsCutOffDay
        Label15.Text = String.Format(My.Resources.MyStrings.mainGainingsLastCalculatedTo, IIf(OldDate = DATENULLVALUE, "n.n.", OldDate.ToString("dd.MM.yyyy")))
        OldDate = tpReport.DateTo
        If OldDate > e.GainingsCutOffDay Then
            ClearReportsGrid()
        End If
    End Sub

    Private Sub BeendenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BeendenToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub cmdReloadDash_Click(sender As Object, e As EventArgs) Handles cmdReloadDash.Click
        dshgrdBestaende.Reload()
        dshgrdAbgaenge.Reload()
        gnd1stTab.Reload()
    End Sub

    Private Sub tabDashboard_Paint(sender As Object, e As PaintEventArgs) Handles tabDashboard.Paint, tabSettings.Paint, tabReports.Paint
        Dim Pge As TabPage
        Pge = DirectCast(sender, TabPage)
        If _BckGrndImg IsNot Nothing Then
            e.Graphics.DrawImage(_BckGrndImg,
                                 New Rectangle(Pge.Width - _BckGrndImg.Width, 0, _BckGrndImg.Width, _BckGrndImg.Height),
                                 New Rectangle(0, 0, _BckGrndImg.Width, _BckGrndImg.Height), GraphicsUnit.Pixel)
        End If
    End Sub

    Private Sub cmdDonateBTC_Click(sender As Object, e As EventArgs) Handles cmdDonateBTC.Click, cmdDonateLTC.Click, cmdDonateBCH.Click, cmdDonateETH.Click
        Dim Donate As New frmDonate
        Donate.CoinType = DirectCast(sender, Button).Tag
        Donate.ShowDialog()
    End Sub


    Private Sub grdDataTimes_SelectionChanged(sender As Object, e As EventArgs) Handles grdDataTimes.SelectionChanged
        DirectCast(sender, DataGridView).ClearSelection()
    End Sub

    ''' <summary>
    ''' Checks the sanity of the current coinvalue strategy settings and gives advices
    ''' </summary>
    ''' <returns>true, if everything is fine, false otherwise</returns>
    Private Function CheckCoinValueStrategies() As Boolean
        ' ...nothing to do so far...
        Return True
    End Function

    Private Sub cmdCalculateGainings_Click(sender As Object, e As EventArgs) Handles cmdCalculateGainings.Click
        Dim ToDate As Date
        Dim UnweightedDate As String = ""
        ' Zur Sicherheit aktuelle Einstellungen in My.Settings
        _TCS.WriteSettings()
        ' und Haltefrist in Datenbank
        dpctlHaltefrist.UpdateData()
        ' Prüfen, ob es noch ungeklärte Transfers gibt
        If _TVM.GetOpenTransfers > 0 Then
            If MsgBoxEx.ShowWithNotAgainOption("CalculateGainingsDespiteUnclearTrades", Windows.Forms.DialogResult.Yes,
                                               String.Format(My.Resources.MyStrings.mainMsgWarnUnclearTransfers, Environment.NewLine), My.Resources.MyStrings.mainMsgWarnUnclearTransfersTitle, MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If
        End If
        ' Sanity checks
        If Not CheckCoinValueStrategies() Then
            Exit Sub
        End If
        ' Bis-Datum = 1 Tag mehr als ausgewählt
        ToDate = DateAdd(DateInterval.Day, 1, dtpCutOffDay.Value.Date)
        Try
            ' Prüfen, ob es unberechnete USD-Trades gibt
            If _CM.HasUnweightedTrades(Konten.USD, ToDate) Then
                _TVM.SetTaxCurrencyValues()
            End If
            If _CM.HasUnweightedTrades(Konten.USD, ToDate) Then
                Dim LastUsdDate As Date = _CM.GetCoursesCutOffDay(Konten.EUR, Konten.USD)
                If LastUsdDate = DATENULLVALUE Then
                    ' es wurden noch gar keine Wechselkursdaten abgerufen
                    MsgBoxEx.PatchMsgBox(New String() {My.Resources.MyStrings.mainMsgGetCoursesBt1, My.Resources.MyStrings.Cancel})
                    If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgGetCourses1, Environment.NewLine, grpCourses.Text),
                                My.Resources.MyStrings.mainMsgGetCoursesTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Retry Then
                        If cmdCourses.Enabled Then
                            cmdCourses_Click(Nothing, Nothing)
                        End If
                    End If
                    Exit Sub
                End If
                If LastUsdDate < dtpCutOffDay.Value.Date Then
                    MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgGetCourses2, LastUsdDate.ToString("dd.MM.yyyy")),
                                    My.Resources.MyStrings.mainMsgGetCoursesTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ToDate = DateAdd(DateInterval.Day, 1, LastUsdDate)
                End If
            End If
            If _CM.HasUnweightedTrades(Konten.USD, ToDate, UnweightedDate) Then
                MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgGetCourses3, UnweightedDate),
                                My.Resources.MyStrings.mainMsgGetCoursesTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            ' Berechnung durchführen
            _TVM.CalculateOutCoinsToInCoins(ToDate)
            gnd2ndTab.Reload()
            ' Issue a warning when we had some spendings of unclear origin
            If _TVM.LastUnclearSpendings > 0 Then
                MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgCalculationUnclearSpendings,
                                              NewLine,
                                              _TVM.LastUnclearSpendings),
                                My.Resources.MyStrings.mainMsgCalculationUnclearSpendingsTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            MessageBox.Show(My.Resources.MyStrings.mainMsgGotCourses, My.Resources.MyStrings.mainMsgGotCoursesTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As TradeValueException
            ' Nichts mehr tun - Messagebox gab es schon...
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    Private Sub cmdReloadGainings_Click_1(sender As Object, e As EventArgs) Handles cmdReloadGainings.Click
        gnd2ndTab.Reload()
    End Sub

    Private Sub ClearReportsGrid()
        With grdReport
            .DataSource = Nothing
            .Rows.Clear()
            .Tag = ""
            _LatestReportFilter = DATENULLVALUE
            SetReportExportButtonStates()
        End With
    End Sub

    Private Sub cmdReloadReport_Click(sender As Object, e As EventArgs) Handles cmdReloadReport.Click

        If _TVM.GetGainingsCutOffDay = DATENULLVALUE Then
            ' Nothing to load yet
            ClearReportsGrid()
            MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgReportReloadNoCalc,
                                          NewLine,
                                          If(cbxSzenario.SelectedIndex > 0, cbxSzenario.SelectedItem(cbxSzenario.DisplayColumnName).ToString, My.Resources.MyStrings.ScenarioDefault), tabSettings.Text, cmdCalculateGainings.Text),
                                      My.Resources.MyStrings.mainMsgReportReloadNoCalcTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            tctlMain.SelectedTab = tabSettings
        Else
            Try
                Cursor.Current = Cursors.WaitCursor
                ' Report-Helper konfigurieren
                _RP.Parameters.ShowTransfers = (2 - cbxReportTransfers.SelectedIndex)
                _RP.ReportType = CTReport.ReportTypes.GainingsReportDetailed
                _RP.GainingsCutOffDay = _TVM.GetGainingsCutOffDay
                _RP.SzenarioID = _TVM.SzenarioID
                If cbxReportTradeSelection.SelectedIndex = 0 Then
                    _RP.TradeSelection = CTReport.ReportTradeSelections.TaxableOnly
                Else
                    _RP.TradeSelection = CTReport.ReportTradeSelections.AllTrades
                End If
                _RP.Parameters.FromDate = tpReport.DateFrom
                _RP.Parameters.ToDate = tpReport.DateTo
                _RP.PlatformIDs = ccbReportPlatforms.GetCheckedItemsIDs
                gnd3rdTab.PlatformIDs = _RP.PlatformIDs
                ' Report laden
                _RP.Reload()
                If _RP.Parameters.FromDate > "2009-01-01" Then
                    _RP.LoadReferencedTrades()
                End If
                ' DataGridView neu binden und konfigurieren
                With grdReport
                    .SuspendLayout()
                    .DataSource = Nothing
                    .DataSource = New BindingSource(_RP.DataTable, "")
                    ' Make sure that cells containing fiat values display just two decimals, not 6 (this is hacky, but quickly implemented...)
                    Dim Currency As String
                    For r As Integer = 0 To .RowCount - 1
                        Currency = .Rows(r).Cells(6).Value
                        If Currency = "Euro" OrElse Currency = "USD" OrElse Currency = "Dollar" Then
                            .Rows(r).Cells(7).Style.Format = "N2"
                        End If
                    Next
                    ' Apply everything
                    .ResumeLayout()
                End With
                gnd3rdTab.Reload()
                ' Filtereinstellungen merken
                _LatestReportFilter = GetReportFilterString()
                Cursor.Current = Cursors.Default
                SetReportExportButtonStates()
            Catch ex As Exception
                Cursor.Current = Cursors.Default
                DefaultErrorHandler(ex)
            End Try
        End If

    End Sub

    Private Sub cmdReportExport_Click(sender As Object, e As EventArgs) Handles cmdReportExport.Click
        If grdReport.RowCount > 0 Then
            If DataGridViewToClipboard(grdReport) Then
                MessageBox.Show(My.Resources.MyStrings.mainMsgReportToClipboard, My.Resources.MyStrings.mainMsgReportToClipboardTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                ShowDisclaimer()
            End If
        End If
    End Sub

    Private Sub InfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InfoToolStripMenuItem.Click
        frmAboutBox.ShowDialog()
    End Sub

    Public Sub ShowDisclaimer(Optional DoShow As Boolean = False)
        If ShownWarnings < 3 Or DoShow Then
            MessageBox.Show(DisclaimerContent.CompleteDisclaimer, My.Resources.MyStrings.mainMsgDisclaimerTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ShownWarnings += 1
        End If
    End Sub

    Private Sub DisclaimerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DisclaimerToolStripMenuItem.Click
        ShowDisclaimer(True)
    End Sub

    Private Sub InhaltToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InhaltToolStripMenuItem.Click
        Try
            Process.Start("http://www.cointracer.de/?q=dokumentation")
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub ReleaseNotesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReleaseNotesToolStripMenuItem.Click
        frmReleaseNotes.ShowDialog(Me)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Debug.Print(Thread.CurrentThread.CurrentCulture.ToString)
        ' Debug.Print(Thread.CurrentThread.CurrentUICulture.ToString)
        ' Thread.CurrentThread.CurrentUICulture = New CultureInfo("de-DE")
        ' frmEditTransfers.ShowDialog(Me)
    End Sub

    Private Sub cmdCourses_Click(sender As Object, e As EventArgs) Handles cmdCourses.Click
        If _TML.IsRunning() = False Then
            ' Prüfen, ob schon Einträge vorhanden sind (ggf. warnen)
            Dim KursTa As New CoinTracerDataSetTableAdapters.KurseTableAdapter
            Dim KursTb As New CoinTracerDataSet.KurseDataTable
            KursTa.FillBySQL(KursTb, "where QuellKontoID=" & Konten.EUR & " and ZielKontoID=" & Konten.USD)
            If KursTb.Count = 0 Then
                If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgGetCoursesUSD, Environment.NewLine),
                                   My.Resources.MyStrings.mainMsgGetCoursesUSDTitle, MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Information) = DialogResult.Cancel Then
                    KursTa.Dispose()
                    Exit Sub
                End If
            End If
            KursTa.Dispose()
            _TML.SetWorkRoutine(AddressOf DoFetchCourses, TML_JOB_FETCHCOURSES, My.Resources.MyStrings.mainMsgGettingCoursesUSD)
            _TML.StartThread()
        End If
    End Sub

    ''' <summary>
    ''' Holt Wechselkurse ab (für asynchronen Aufruf)
    ''' </summary>
    Private Sub DoFetchCourses(Optional Silent As Boolean = True)
        Try
            Dim NewDays As Long = 0
            Dim CM As New CourseManager(_cnn)
            CM.ThreadManagerLight = _TML
            CM.SilentMode = Silent
            NewDays = CM.FetchCoursesEurUsd()
            RefreshCourseDisplays(NewDays)
        Catch ex As ThreadAbortException
            Cursor.Current = Cursors.Default
            Exit Sub
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    Private Sub CleanUpDBToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CleanUpDBToolStripMenuItem.Click
        Dim BytesSaved As Long = 0
        Try
            Cursor.Current = Cursors.WaitCursor
            Dim DBInitialize As New DBInit
            Dim DBFileInfo As New FileInfo(DBInitialize.DatabaseFile)
            Dim DBFileLengthBefore As Long = DBFileInfo.Length
            _DB.ExecuteSQL("VACUUM;")
            DBFileInfo.Refresh()
            BytesSaved = DBFileLengthBefore - DBFileInfo.Length
        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DefaultErrorHandler(ex)
            Exit Sub
        End Try
        Cursor.Current = Cursors.Default
        Dim Message As String = My.Resources.MyStrings.mainMsgOptimizeDB1
        If BytesSaved > 500 Then
            Message &= String.Format(My.Resources.MyStrings.mainMsgOptimizeDB2, NewLine, Math.Round(CSng(BytesSaved / 1000), 1))
        End If
        MessageBox.Show(Message, My.Resources.MyStrings.mainMsgOptimizeDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>
    ''' Gibt die aktuellen Report-Filtereinstellungen als String zurück
    ''' </summary>
    ''' <returns>String mit Filtereinstellungen</returns>
    Private Function GetReportFilterString() As String
        Return tpReport.DateSql &
            "|" & cbxReportTransfers.SelectedIndex &
            "|" & (cbxReportTradeSelection.SelectedIndex = 1) &
            "|" & ccbReportPlatforms.GetCheckedItemsIDs
    End Function

    Private Sub RefreshOpenTransfers()
        Try
            lblTransfersOpen.Text = _TVM.GetOpenTransfers
            ' cmdTransfers.Enabled = CInt(lblTransfersOpen.Text) > 0
        Catch ex As Exception

        End Try
    End Sub


    ''' <summary>
    ''' Öffnet das Formular zur Bearbeitung offener Trades
    ''' </summary>
    Private Sub cmdTransfers_Click(sender As Object, e As EventArgs) Handles cmdTransfers.Click
        Try
            Dim OpenTransfersForm As New frmEditTransfers
            With OpenTransfersForm
                .UnclearOnly = True
                .ImportFilterID = _LastImportID
                .ShowDialog(Me)
                If .Sticky Then
                    RefreshAfterTradesEdit()
                End If
                _LastImportID = 0
                .Dispose()
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Setzt die Buttons für den Export des Berichts auf aktiv oder inaktiv (abhängig davon, ob der 
    ''' Bericht neu geladen werden muss, weil sich Filtereinstellungen geändert haben)
    ''' </summary>
    Private Sub SetReportExportButtonStates()
        cmdReportPreview.Enabled = _LatestReportFilter = GetReportFilterString()
        cmdReportExport.Enabled = cmdReportPreview.Enabled
    End Sub

    Private Sub cbxReport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxReportTransfers.SelectedIndexChanged, cbxReportTradeSelection.SelectedIndexChanged
        SetReportExportButtonStates()
    End Sub

    Private Sub ccbReportPlatforms_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ccbReportPlatforms.DropDownClosed
        If ccbReportPlatforms.ValueChanged Then
            SetReportExportButtonStates()
            ' Tooltip des Controls neu setzen
            EnhancedToolTip1.SetToolTip(ccbReportPlatforms, ccbReportPlatforms.Text)
        End If
    End Sub

    Private Sub tpReport_SettingChanged(sender As Object, e As EventArgs) Handles tpReport.SettingChanged
        SetReportExportButtonStates()
    End Sub

    ''' <summary>
    ''' Speichern der Datenbank zu Backup-Zwecken
    ''' </summary>
    Private Sub DatabaseSaveMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseSaveMenuItem.Click
        Dim SFD As New SaveFileDialog
        With SFD
            .Filter = My.Resources.MyStrings.mainMsgSaveDBFilter
            .Title = My.Resources.MyStrings.mainMsgSaveDBTitle
            .FileName = String.Format("cointracer_backup_{0}.data", Now.ToString("yyyy-MM-dd_HH.mm"))
            If .ShowDialog(Me) = DialogResult.OK Then
                Try
                    Dim DBInit As New DBInit
                    My.Computer.FileSystem.CopyFile(DBInit.DatabaseFile, .FileName, True)
                    MessageBox.Show(My.Resources.MyStrings.mainMsgSaveDB2, My.Resources.MyStrings.mainMsgSaveDB2Title,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    DefaultErrorHandler(ex, String.Format(My.Resources.MyStrings.mainMsgSaveDBError, NewLine))
                End Try
            End If
        End With
    End Sub



    ''' <summary>
    ''' Öffnen einer Datenbank-Datei (Aktuelle wird als *.bak gespeichert)
    ''' </summary>
    Private Sub DatabaseLoadMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseLoadMenuItem.Click
        Dim OFD As New OpenFileDialog()
        If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgLoadDBWarning, Environment.NewLine),
                           My.Resources.MyStrings.mainMsgLoadDBWarningTitle, MessageBoxButtons.OKCancel,
                           MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.OK Then
            With OFD
                .Filter = My.Resources.MyStrings.mainMsgLoadDBFilter
                .FilterIndex = 1
                .Title = My.Resources.MyStrings.mainMsgLoadDBTitle
                .RestoreDirectory = True
                If .ShowDialog() = DialogResult.OK Then
                    LoadDatabase(.FileName, True)
                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Loads a given sqlite database file as new database for this application
    ''' </summary>
    ''' <param name="Filename">Fully qualified file name of the new database file</param>
    ''' <param name="Verbose">True to display a MessageBox on success, false otherwise</param>
#If CONFIG = "Debug" Then
    Public Sub LoadDatabase(Filename As String,
                             Optional Verbose As Boolean = True)
#Else
    Private Sub LoadDatabase(Filename As String,
                             Optional Verbose As Boolean = True)
#End If
        Try
            Cursor.Current = Cursors.WaitCursor
            Dim DBInit As New DBInit
            ' Okay: Aktuelle Datenbank sicherheitshalber kopieren
            My.Computer.FileSystem.CopyFile(DBInit.DatabaseFile, DBInit.DatabaseFile & ".bak", True)
            ' Und jetzt Backup-Datei als Live-Datei kopieren
            My.Computer.FileSystem.CopyFile(Filename, DBInit.DatabaseFile, True)
            ' Datenbank "einklinken"
            DBInit.InitDatabase()
            frmMain_Load(Me, New EventArgs)
            ReloadTablesTab()
            ClearReportsGrid()
            Cursor.Current = Cursors.Default
            If Verbose Then
                MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgLoadDBSuccess, NewLine),
                            My.Resources.MyStrings.mainMsgLoadDBSuccessTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DefaultErrorHandler(ex, String.Format(My.Resources.MyStrings.mainMsgLoadDBError, NewLine, ex.Message))
        End Try
    End Sub

    Private Sub tctlTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tctlTables.SelectedIndexChanged
        ReloadTablesTab()
    End Sub

    ''' <summary>
    ''' Lädt das Grid in der jeweils angeklickten TabPage aus dem Tables-TabControl neu
    ''' </summary>
    Private Sub ReloadTablesTab()
        Dim Grd As BoundDataGridView
        Select Case tctlTables.SelectedIndex
            Case 1
                ' Importe
                Grd = grdImporte
            Case 2
                ' Plattformen
                Grd = grdPlattformen
            Case 3
                ' Konten
                Grd = grdKonten
            Case 4
                ' Kurse
                Grd = grdKurse
            Case 5
                ' Kurse
                Grd = grdBerechnungen
            Case Else
                ' Standard
                Grd = grdTrades
        End Select
        Grd.Reload()
        If Grd.Name = "grdBerechnungen" AndAlso Grd.ColumnCount >= 4 Then
            Grd.Columns(Grd.ColumnCount - 1).Visible = False
        End If
    End Sub

    Private Sub tctlMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tctlMain.SelectedIndexChanged
        If tctlMain.SelectedIndex = 3 Then
            ' Bei Öffnen des "Tabellen"-Tabs jeweilige Liste neu laden
            ReloadTablesTab()
        End If
    End Sub

    Friend Sub RefreshAfterTradesEdit()
        _DB.Reset_DataAdapter(TableNames.Kalkulationen)
        If _DB.DataTable(TableNames.Kalkulationen).Rows.Count > 0 Then
            If MessageBox.Show(String.Format(My.Resources.MyStrings.mainEditTradesAdvice, Environment.NewLine), My.Resources.MyStrings.mainEditTradesAdviceTitle,
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = Windows.Forms.DialogResult.OK Then
                _TVM.RollbackCalculation(DATENULLVALUE, , True)
            End If
        End If
        _TVM.ResetAllLossTrades()
        ReloadTablesTab()
        FillDataTimesGrid()
        RefreshOpenTransfers()
        dshgrdBestaende.Reload()
        dshgrdAbgaenge.Reload()
    End Sub

    Private Sub tsmiEditTrades_Click(sender As Object, e As EventArgs) Handles tsmiEditTrades.Click
        Dim EditTradesForm As New frmEditTrades
        Dim Grd As BoundDataGridView
        Grd = grdTrades
        Cursor = Cursors.WaitCursor
        With EditTradesForm
            .EditMode = frmEditTrades.TradesEditModes.AllTypes
            If Grd.SelectedCells.Count > 0 Then
                .StartID = Grd.Rows(Grd.CurrentCell.RowIndex).Cells(0).Value
            End If
            If .ShowDialog(Me) = Windows.Forms.DialogResult.OK AndAlso .RecordsModified > 0 Then
                RefreshAfterTradesEdit()
            End If
            .Dispose()
        End With
        Cursor = Cursors.Default
    End Sub

    Private Sub cmdTogglePlatforms_Click(sender As Object, e As EventArgs) Handles cmdTogglePlatforms.Click
        cmdTogglePlatforms.Text = IIf(dshgrdAbgaenge.Columns("Art").Visible, My.Resources.MyStrings.mainHidePlatforms, My.Resources.MyStrings.mainShowPlatforms)
        ReloadDashGrids(True, dshgrdAbgaenge.Columns("Art").Visible)
    End Sub

    ''' <summary>
    ''' Prüft, ob es bereits eine neuere Version zum Download gibt
    ''' </summary>
    Private Sub CheckForUpdatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForUpdatesToolStripMenuItem.Click
        If _TML Is Nothing OrElse _TML.IsRunning(TML_JOB_CHECKFORUPDATE) = False Then
            DoCheckForUpdates(False)
        End If
    End Sub

    ''' <summary>
    ''' Parameterlose Routine für das Prüfen auf Update-Versionen (für asynchrones Laufen...)
    ''' </summary>
    Private Sub DoCheckForUpdates(Optional Silent As Boolean = True)

        Dim URICheck As String = "https://www.cointracer.de/updates/updateinfo.php?lang=de"
        Dim URIDownload As String = "https://www.cointracer.de/?q=download"

        Dim HttpRequest As New UriRequest(URICheck)
        Dim LatestVersion As String
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12


        ' Versionsnummer abholen
        Try
            LatestVersion = HttpRequest.Request
        Catch ex As ThreadAbortException
            Exit Sub
        Catch ex As Exception
            DefaultErrorHandler(ex, String.Format(My.Resources.MyStrings.mainCheckUpdatesError, ex.Message))
            Exit Sub
        End Try

        ' Prüfen, ob gelesene Version neuer ist als die vorhandene
        If LatestVersion.Length >= 11 Then
            Dim KnownIssues As String
            If LatestVersion.Length > 11 Then
                KnownIssues = NewLine & NewLine & LatestVersion.Substring(11)
                LatestVersion = LatestVersion.Substring(0, 11)
            Else
                KnownIssues = ""
            End If
            Dim CurrentVersionParts As String() = Application.ProductVersion.Split(".")
            If CurrentVersionParts.Length = 3 Then
                ReDim Preserve CurrentVersionParts(3)
                CurrentVersionParts(3) = 0
            End If
            If LatestVersion >
            CurrentVersionParts(0).PadLeft(3, "0") & CurrentVersionParts(1).PadLeft(3, "0") & CurrentVersionParts(2).PadLeft(3, "0") & CurrentVersionParts(3).PadLeft(2, "0") Then
                If MessageBox.Show(String.Format(My.Resources.MyStrings.mainCheckUpdates, Environment.NewLine,
                                                CInt(LatestVersion.Substring(0, 3)).ToString,
                                                CInt(LatestVersion.Substring(3, 3)).ToString,
                                                CInt(LatestVersion.Substring(6, 3)).ToString &
                                                If(LatestVersion.EndsWith("00"), "", "." & CInt(LatestVersion.Substring(9, 2)).ToString),
                                                URIDownload,
                                                Application.ProductName),
                                                My.Resources.MyStrings.mainCheckUpdatesTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                    Try
                        Process.Start(URIDownload)
                        Application.Exit()
                    Catch ex As ThreadAbortException
                        Exit Sub
                    Catch ex As Exception
                        DefaultErrorHandler(ex, ex.Message)
                    End Try
                End If
            Else
                LatestVersion = String.Format(My.Resources.MyStrings.mainCheckUpdatesStillLatest,
                                              Application.ProductName,
                                              If(Application.ProductVersion.EndsWith(".0"), Application.ProductVersion.Substring(0, Application.ProductVersion.Length - 2), Application.ProductVersion.ToString))
                If Silent = False Then
                    MessageBox.Show(LatestVersion & KnownIssues, My.Resources.MyStrings.mainCheckUpdatesTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    _TML.MessageText = LatestVersion & IIf(KnownIssues.Length > 0, My.Resources.MyStrings.mainCheckUpdatesMessagesPending, "")
                    Thread.Sleep(4000)
                    _TML.MessageText = ""
                End If
            End If
        End If
    End Sub

    Private Sub tsmiEraseImport_Click(sender As Object, e As EventArgs) Handles tsmiEraseImport.Click
        Dim ImportID As Long = -1
        With grdImporte
            If .SelectedCells.Count > 0 Then
                ImportID = .Rows(.CurrentCell.RowIndex).Cells(0).Value
                Dim ApiDatenID As Long = .Rows(.CurrentCell.RowIndex).Cells(7).Value
                Dim ImportTime As Date = .Rows(.CurrentCell.RowIndex).Cells(2).Value
                If ImportID > 0 AndAlso MessageBox.Show("Folgender Import wird inkl. der zugehörigen Trade-Daten gelöscht: " & Environment.NewLine & Environment.NewLine &
                                   "ID: " & vbTab & vbTab & ImportID & Environment.NewLine &
                                   "Plattform:" & vbTab & vbTab & .Rows(.CurrentCell.RowIndex).Cells(1).Value & Environment.NewLine &
                                   "eingelesen am:" & vbTab & .Rows(.CurrentCell.RowIndex).Cells(2).Value & Environment.NewLine &
                                   "eingelesene Trades:" & vbTab & DirectCast(.Rows(.CurrentCell.RowIndex).Cells(5).Value, Long).ToString("#,###,##0") & Environment.NewLine & Environment.NewLine &
                                   "Sind Sie sicher? Diese Aktion kann nicht rückgängig gemacht werden!",
                                   "Import löschen", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                    Try
                        ' Jüngstes Trade-Datum ermitteln, um ZSW ggf. anzupassen
                        Dim DBO As New DBObjects(String.Format("select coalesce(min(Zeitpunkt), date('2099-01-01')) MinZeit from Trades where ImportID = {0}", ImportID), _cnn)
                        If DBO IsNot Nothing Then
                            If CDate(DBO.DataTable.Rows(0)("MinZeit")) <= _TVM.GetGainingsCutOffDay Then
                                _TVM.RollbackCalculation(CDate(DBO.DataTable.Rows(0)("MinZeit")), False, True)
                                ClearReportsGrid()
                            End If
                            ' Import löschen
                            _DB.ExecuteSQL("update Trades set InTradeID = 0, ZielPlattformID = 0 where InTradeID in " &
                                           "(select ID from Trades where ImportID=" & ImportID & ")")
                            _DB.ExecuteSQL("update Trades set OutTradeID = 0, QuellPlattformID = 0 where OutTradeID in " &
                                           "(select ID from Trades where ImportID=" & ImportID & ")")
                            _DB.ExecuteSQL("delete from Trades where TradeTypID = 5 and QuellPlattformID = 0 and ZielPlattformID = 0")
                            _DB.ExecuteSQL("delete from Trades where ImportID=" & ImportID)
                            ' _DB.ExecuteSQL("update ApiDaten set LastImportTimestamp=0 where PlattformID=(" & _
                            '               "select PlattformID from Importe where ID=" & ImportID & ")")
                            _DB.ExecuteSQL("delete from Importe where ID=" & ImportID)
                            _TVM.ResetAllLossTrades()
                            _DB.ReloadDatabase(True)
                            ' Aktualisierungen und Anzeigen
                            ReloadDashGrids()
                            RefreshCourseDisplays()
                            FillDataTimesGrid()
                            ReloadTablesTab()
                            ' Bei gelöschtem API-Import auf ggf. notwendige Zeitanpassung hinweisen
                            If ApiDatenID > 0 Then
                                If MessageBox.Show(String.Format("Sie haben einen API-Import erfolgreich gelöscht. Bitte achten Sie darauf, " &
                                                "ggf. in den API-Zugangsdaten (ID {0}) die Einstellung 'Daten holen ab:' anzupassen, " &
                                                "um Lücken in der Trade-History zu vermeiden.{1}{1}Möchten Sie die API-Daten jetzt bearbeiten?",
                                                ApiDatenID, Environment.NewLine),
                                                "Import löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.OK Then
                                    Dim ApiFrm As New frmEditApiData
                                    If ApiFrm.ProcessPasswordProtection Then
                                        ApiFrm.StartID = ApiDatenID
                                        ApiFrm.TargetTime = ImportTime
                                        ApiFrm.ShowDialog(Me)
                                    End If
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        DefaultErrorHandler(ex, "Beim Löschen des Imports ID " & ImportID & " ist ein Fehler aufgetreten: " & ex.Message)
                        Exit Sub
                    End Try
                End If
            End If
        End With

    End Sub

    Private Sub cmdSzenarioSave_Click(sender As Object, e As EventArgs) Handles cmdSzenarioSave.Click
        Dim Cancel As Boolean = False
        Dim Present As Boolean = cbxSzenario.LabelInDataSource
        Dim NewLabel As String = cbxSzenario.Text.Trim
        If Present AndAlso cbxSzenario.SelectedIndex < 0 Then
            ' This scenario label already exists - overwrite scenario?
            Cancel = MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgScenarioOverwrite, NewLabel),
                                     My.Resources.MyStrings.mainMsgScenarioSaveTitle,
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No
        End If
        If Not Cancel Then
            ' Save scenario
            SaveScenario(cbxSzenario.Text, Present, cbxSzenario.Text)
        End If
    End Sub

    ''' <summary>
    ''' Save the current scenario details as scenario [ScenarioToSaveLabel]
    ''' </summary>
    ''' <param name="ScenarioToSaveLabel">Name of the scenario. Will be created if not already present</param>
    ''' <param name="AlreadyPresent">True if the scenario already exists, false otherwise</param>
    ''' <param name="ScenarioToLoadLabel">Switch to this scenario after saving</param>
    Private Sub SaveScenario(ScenarioToSaveLabel As String,
                             AlreadyPresent As Boolean,
                             ScenarioToLoadLabel As String)
        Dim TA As New CoinTracerDataSetTableAdapters.SzenarienTableAdapter
        Dim Tb As New CoinTracerDataSet.SzenarienDataTable
        Try
            If AlreadyPresent Then
                If TA.FillBy(Tb, ScenarioToSaveLabel) > 0 Then
                    With DirectCast(Tb.Rows(0), CoinTracerDataSet.SzenarienRow)
                        .CVS = _TCS.ToString
                        .Coins4Coins = _TCS.Coins4CoinsAccounting
                    End With
                    TA.Update(Tb)
                End If
            Else
                TA.Insert(ScenarioToSaveLabel, _TCS.ToString, _TCS.Coins4CoinsAccounting)
            End If
            MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgScenarioSaved, ScenarioToSaveLabel),
                        My.Resources.MyStrings.mainMsgScenarioSaveTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)
            cbxSzenario.Reload(ScenarioToLoadLabel)
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub cmdSzenarioDelete_Click(sender As Object, e As EventArgs) Handles cmdSzenarioDelete.Click
        If cbxSzenario.SelectedValue > 0 Then
            Dim SzenarioName As String = cbxSzenario.Text.Trim
            If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgScenarioDelete, SzenarioName),
                               My.Resources.MyStrings.mainMsgScenarioDeleteTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation,
                               MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                Dim TA As New CoinTracerDataSetTableAdapters.SzenarienTableAdapter
                TA.DeleteByID(cbxSzenario.SelectedValue)
                _TVM.RollbackCalculation(DATENULLVALUE, True)
                cbxSzenario.Reload()
                MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgScenarioDeleted, SzenarioName),
                                My.Resources.MyStrings.mainMsgScenarioDeleteTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub


    ''' <summary>
    ''' Setzt das Sticky-Flag des Szenarios bei Änderung der CoinValueSelector-Einstellungen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub vssSomething_SettingsChanged(sender As Object, e As EventArgs) Handles _
        vssGlobalStrategy.SettingsChanged,
        dpctlHaltefrist.SettingsChanged

        Try
            If sender.GetType Is GetType(ValueStrategySelector) Then
                cbxSzenario.Sticky = cbxSzenario.Sticky Or DirectCast(sender, ValueStrategySelector).Sticky
            Else
                cbxSzenario.Sticky = cbxSzenario.Sticky Or DirectCast(sender, DataPeriodControl).Sticky
            End If
        Catch ex As Exception
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub cbxSzenario_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSzenario.SelectedIndexChanged
        If Not cbxSzenario.Initializing Then
            cbxSzenario.Initializing = True
            Try
                If cbxSzenario.SelectedIndex >= 0 AndAlso cbxSzenario.SelectedItem("CVS").ToString <> "" Then
                    _TCS.FromString(cbxSzenario.SelectedItem("CVS").ToString)
                    _TVM.SzenarioID = cbxSzenario.SelectedItem("ID")
                    ClearReportsGrid()
                End If
            Catch ex As Exception
                DefaultErrorHandler(ex, ex.Message)
            End Try
            cbxSzenario.Initializing = False
        End If
        If cbxSzenario.SelectedIndex >= 0 Then
            lblSzenarioRpt.Text = "Szenario: " & cbxSzenario.SelectedItem("Bezeichnung")
        End If
    End Sub

    ''' <summary>
    ''' Save scenario if there are unsaved changes and we are about to switch to another scenario
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub cbxSzenario_UnsavedChanges(sender As Object, e As DataBoundComboBoxUnsavedChangesEventArgs) Handles cbxSzenario.UnsavedChanges
        If MessageBox.Show(String.Format(My.Resources.MyStrings.mainMsgScenarioUnsavedChanges, e.OriginalValue),
                           My.Resources.MyStrings.mainMsgScenarioSaveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                           MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
            SaveScenario(e.OriginalValue, True, e.ActualValue)
        End If
    End Sub


    ''' <summary>
    ''' Beim Starten (oder Beenden) von asynchronen Threads bestimmte Funktionen der GUI temporär de-/aktivieren.
    ''' </summary>
    Private Sub _TML_BeginThread(sender As Object, e As EventArgs) Handles _TML.BeginThread
        Select Case _TML.WorkRoutineName
            Case TML_JOB_FETCHCOURSES
                cmdCourses.Enabled = False
            Case TML_JOB_CHECKFORUPDATE
                CheckForUpdatesToolStripMenuItem.Enabled = False
        End Select
    End Sub

    Private Sub _TML_EndThread(sender As Object, e As EventArgs) Handles _TML.EndThread
        Select Case _TML.WorkRoutineName
            Case TML_JOB_FETCHCOURSES
                cmdCourses.Enabled = True
            Case TML_JOB_CHECKFORUPDATE
                CheckForUpdatesToolStripMenuItem.Enabled = True
        End Select
    End Sub

    Private Sub tsmiOptions_Click(sender As Object, e As EventArgs) Handles tsmiOptions.Click
        If frmApplicationSettings.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            HandleOfflineMode()
        End If
    End Sub

    ''' <summary>
    ''' Verarbeitet die Einstellung "Offline-Modus" und de-/aktiviert die betroffenen Steuerelemente
    ''' </summary>
    Private Sub HandleOfflineMode()
        Dim OfflineMessage As String = "Diese Funktion steht im Offline-Modus nicht zur Verfügung. Sie können den Offline-Modus " &
            "bei Bedarf unter Extras -> Einstellungen deaktivieren."
        Dim Offline As Boolean = My.Settings.OfflineMode
        For Each Ctrl As Object In New ArrayList() From {cmdCourses, CheckForUpdatesToolStripMenuItem, cmdImportApi}
            Ctrl.Enabled = Not Offline
            ' Bei Bedarf (und passendem Control-Typ) Tooltip ändern
            Ctrl = TryCast(Ctrl, Control)
            If Ctrl IsNot Nothing Then
                If Ctrl.Tag & "" = "" Then
                    Ctrl.Tag = EnhancedToolTip1.GetToolTip(Ctrl)
                    If Ctrl.Tag = "" Then Ctrl.Tag = "[void]"
                End If
                If Offline Then
                    EnhancedToolTip1.SetToolTip(Ctrl, OfflineMessage)
                ElseIf Ctrl.tag & "" <> "" Then
                    If Ctrl.Tag = "[void]" Then
                        EnhancedToolTip1.SetToolTip(Ctrl, "")
                    Else
                        EnhancedToolTip1.SetToolTip(Ctrl, Ctrl.Tag)
                    End If
                End If
            End If
        Next

    End Sub

    Private Sub tsmiEditTables_Click(sender As Object, e As EventArgs) Handles tsmiEditPlattformen.Click,
        tsmiEditKonten.Click,
        tsmiEditKurse.Click,
        tsmiViewCalculations.Click
        Dim ShowForm As Object
        Dim Grd As BoundDataGridView
        Cursor = Cursors.WaitCursor
        Try
            Select Case tctlTables.SelectedIndex    ' sender.Name
                Case 4  ' "tsmiEditKurse"
                    Grd = grdKurse
                    ShowForm = New frmEditCourses
                    _DB.Reset_DataAdapter(TableNames.Kurse)
                Case 2  ' "tsmiEditPlattformen"
                    Grd = grdPlattformen
                    ShowForm = New frmEditPlattformen
                    _DB.Reset_DataAdapter(TableNames.Plattformen)
                Case 5  ' "tsmiViewCalculations"
                    Grd = grdBerechnungen
                    ShowForm = New frmViewCalculations
                    ' _DB.Reset_DataAdapter(TableNames.Kalkulationen)
                Case Else
                    ' Default: Konten
                    Grd = grdKonten
                    ShowForm = New frmEditKonten
                    _DB.Reset_DataAdapter(TableNames.Konten)
            End Select
            Cursor = Cursors.Default
            With ShowForm
                If Grd.SelectedCells.Count > 0 Then
                    .StartID = Grd.Rows(Grd.CurrentCell.RowIndex).Cells(0).Value
                End If
                If .ShowDialog(Me) = DialogResult.OK OrElse .RecordsModified > 0 Then
                    ' Bei geänderten Plattformdaten ggf. Gewinn neu berechnen
                    If .RecordsModified > 0 AndAlso tctlTables.SelectedIndex = 2 Then
                        _DB.Reset_DataAdapter(TableNames.Kalkulationen)
                        If _DB.DataTable(TableNames.Kalkulationen).Rows.Count > 0 Then
                            If MessageBox.Show("Sie haben Plattform-Daten geändert. Um eventuelle Auswirkungen auf die Gewinnberechnung " &
                                               "zu berücksichtigen werden jetzt alle berechneten Daten zurückgesetzt." & Environment.NewLine & Environment.NewLine &
                                               "Klicken Sie nur dann auf Abbrechen, wenn Sie sicher sind, dass die vorgenommenen Änderungen " &
                                               "keine Auswirkungen auf den berechneten Gewinn/Verlust haben!", "Geänderte Plattform-Informationen",
                                               MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = Windows.Forms.DialogResult.OK Then
                                _TVM.RollbackCalculation(DATENULLVALUE, , True)
                            End If
                        End If
                    End If
                    ReloadTablesTab()
                    FillDataTimesGrid()
                    dshgrdBestaende.Reload()
                    dshgrdAbgaenge.Reload()
                End If
                .Dispose()
            End With
        Catch ex As Exception
            Cursor = Cursors.Default
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Startet je nach ApplicationSettings gewünschte Aktionen im Hintergrund
    ''' </summary>
    Private Sub StartInitialBackgroundActions()
        If My.Settings.OfflineMode = False AndAlso (My.Settings.CheckFiatCoursesOnStart OrElse My.Settings.CheckVersionOnStart) Then
            If _TML.IsRunning() = False Then
                _TML.SetWorkRoutine(AddressOf DoInitialBackgroundActions, "StartupActions", "Führe Programmstart-Aktionen aus...")
                _TML.StartThread()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Führt Background-Aktionen zum Programmstart in separatem Thread aus
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DoInitialBackgroundActions()
        Try
            Dim LastCheckDate As Date
            If IsDate(My.Settings.LastCheckVersion) Then
                LastCheckDate = My.Settings.LastCheckVersion
            Else
                LastCheckDate = DATENULLVALUE
            End If
            If My.Settings.CheckVersionOnStart AndAlso DateDiff(DateInterval.Day, LastCheckDate, Now) > 0 Then
                _TML.WorkRoutineName = TML_JOB_CHECKFORUPDATE
                _TML.MessageText = "Prüfe auf neuere Programmversion..."
                _TML.OnBeginThread()
                DoCheckForUpdates()
                _TML.OnEndThread()
                My.Settings.LastCheckVersion = Now
            End If
            If IsDate(My.Settings.LastCheckFiatCourses) Then
                LastCheckDate = My.Settings.LastCheckFiatCourses
            Else
                LastCheckDate = DATENULLVALUE
            End If
            If My.Settings.CheckFiatCoursesOnStart AndAlso DateDiff(DateInterval.Day, LastCheckDate, Now) > 0 Then
                _TML.WorkRoutineName = TML_JOB_FETCHCOURSES
                _TML.MessageText = "Rufe Wechselkursdaten EUR <> US-Dollar ab..."
                _TML.OnBeginThread()
                DoFetchCourses()
                _TML.OnEndThread()
                My.Settings.LastCheckFiatCourses = Now
            End If
        Catch ex As ThreadAbortException
            Exit Sub
        Catch ex As Exception
            DefaultErrorHandler(ex, "Es ist ein Fehler beim Ausführen von Programmstart-Aktionen aufgetreten: " & ex.Message)
        End Try
    End Sub

    Private Sub cmdReportPreview_Click(sender As Object, e As EventArgs) Handles cmdReportPreview.Click
        Cursor.Current = Cursors.WaitCursor
        Dim FrmRep As New frmReportPreview
        If _RP.DataTable IsNot Nothing Then
            Try
                With _RP.Parameters
                    .Name = tbxUserName.Text.Trim
                    .TaxNumber = tbxTaxNumber.Text.Trim
                    .ReportComment = tbxReportAdvice.Text.Trim
                End With
                Cursor.Current = Cursors.Default
                FrmRep.ShowDialog(Me)
            Catch ex As Exception
                Cursor.Current = Cursors.Default
                DefaultErrorHandler(ex, "Fehler beim Erstellen des Berichts: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub cmdImportApi_Click(sender As Object, e As EventArgs) Handles cmdImportApi.Click
        Dim TradeImport As Import = New Import(_DB, _TVM, Me)
        Try

            Dim DoLoop As Boolean = False
            Dim ApiPassword As String = ""
            Do
                If Not TradeImport.CheckApiPassword() Then
                    ' Passwortschutz: nach Passwort fragen
                    If TradeImport.RequestApiPassword(ApiPassword,
                                                      "Bitte geben Sie Ihr Passwort ein, um den API-Datenabruf zu starten.") <> DialogResult.OK Then
                        Exit Sub
                    Else
                        ' Prüfen, ob das Passwort stimmt!
                        If Not TradeImport.CheckApiPassword(ApiPassword) Then
                            MessageBox.Show("Das Passwort ist nicht korrekt!", "Falsches Passwort",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            DoLoop = True
                        Else
                            TradeImport.ApiPassword = ApiPassword
                            DoLoop = False
                        End If
                    End If
                Else
                    ' Kein Passwortschutz: Default-Passwort verwenden
                    ApiPassword = TradeImport.DefaultApiPassword
                End If
            Loop While DoLoop

            TradeImport.DoApiImport()
            RefreshAfterImport(TradeImport)
        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DefaultErrorHandler(ex, ex.Message)
        End Try
    End Sub

    Private Sub cmdConfigApi_Click(sender As Object, e As EventArgs) Handles cmdConfigApi.Click
        Dim ApiFrm As New frmEditApiData
        If ApiFrm.ProcessPasswordProtection Then
            If ApiFrm.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                ' don't know yet...
            End If
        End If
    End Sub

    Private Sub grdTrades_CellContentDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles grdTrades.CellContentDoubleClick
        tsmiEditTrades_Click(sender, Nothing)
    End Sub


    Private Sub grdTrades_CheckBoxColumnClicked(sender As Object, e As EventArgs) Handles grdTrades.CheckBoxColumnClicked
        Try
            If MessageBox.Show("Sie können Trade-Einträge nicht direkt in der Tabellenansicht bearbeiten. " &
                                "Möchten Sie nun in den Bearbeitungs-Modus wechseln?", "Trades bearbeiten",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                tsmiEditTrades_Click(sender, Nothing)
            End If
        Catch ex As Exception
        End Try
    End Sub


    Private Sub grdKonten_CheckBoxColumnClicked(sender As Object, e As EventArgs) Handles grdKonten.CheckBoxColumnClicked,
        grdPlattformen.CheckBoxColumnClicked,
        grdKurse.CheckBoxColumnClicked
        Try
            Dim LabelSng As String
            Dim LabelPlr As String
            Dim tsmi As ToolStripMenuItem
            Select Case DirectCast(sender, BoundDataGridView).Name
                Case "grdKonten"
                    LabelSng = "Konto"
                    LabelPlr = "Konten"
                    tsmi = tsmiEditKonten
                Case "grdPlattformen"
                    LabelSng = "Plattform"
                    LabelPlr = "Plattformen"
                    tsmi = tsmiEditPlattformen
                Case "grdKurse"
                    LabelSng = "Kursdaten"
                    LabelPlr = "Kursdaten"
                    tsmi = tsmiEditKurse
                Case Else
                    LabelSng = ""
                    LabelPlr = ""
                    tsmi = Nothing
            End Select
            If MessageBox.Show(String.Format("Sie können {0}-Einträge nicht direkt in der Tabellenansicht bearbeiten. " &
                                "Möchten Sie nun in den Bearbeitungs-Modus wechseln?", LabelSng), LabelPlr & " bearbeiten",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                tsmiEditTables_Click(tsmi, e)
            End If
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Keep the settings of all gainings controls synchronized
    ''' </summary>
    Private Sub gndSomeTab_Changed(sender As Object, e As EventArgs) Handles gnd1stTab.Changed,
        gnd2ndTab.Changed,
        gnd3rdTab.Changed
        For Each gndCtrl In New GainingsDisplay() {gnd1stTab, gnd2ndTab, gnd3rdTab}
            If gndCtrl.Name <> DirectCast(sender, GainingsDisplay).Name Then
                gndCtrl.CloneFrom(DirectCast(sender, GainingsDisplay))
            End If
        Next
    End Sub


    Public Sub New()

        Thread.CurrentThread.CurrentUICulture = New CultureInfo(My.Settings.CurrentCulture)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _DBName = DBInit.DBDEFAULTNAME

    End Sub

    Private Sub ShowHistoricImportsCheckbox_CheckedChanged(sender As Object, e As EventArgs) Handles ShowHistoricImportsCheckbox.CheckedChanged
        PlatformManager.LoadImportComboBox(cbxImports, DirectCast(sender, CheckBox).Checked)
    End Sub

    ''' <summary>
    ''' Erases and resets a gainings calculation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub tsmiEraseCalculation_Click(sender As Object, e As EventArgs) Handles tsmiEraseCalculation.Click
        With grdBerechnungen
            If .SelectedCells.Count > 0 Then
                Dim CalcID As Long = .Rows(.CurrentCell.RowIndex).Cells(0).Value
                Dim Scenario As String = .Rows(.CurrentCell.RowIndex).Cells(2).Value
                If CalcID > 0 Then
                    Dim CalcTB As New CoinTracerDataSet.KalkulationenDataTable
                    Dim CalcTA As New CoinTracerDataSetTableAdapters.KalkulationenTableAdapter
                    If CalcTA.FillBySQL(CalcTB, "where ID = " & CalcID) > 0 Then
                        Dim CalcRow As CoinTracerDataSet.KalkulationenRow = CalcTB.Rows(0)
                        Dim Message As String
                        Dim ScenarioID As Long = CalcRow.SzenarioID
                        Dim ToDate As Date = CalcRow.Zeitpunkt
                        CalcTA.ClearBeforeFill = True
                        If CalcTA.FillBySQL(CalcTB, String.Format("where Zeitpunkt < '{0}' and SzenarioID = {1} order by Zeitpunkt DESC", ConvertToSqlDate(ToDate), ScenarioID)) > 0 Then
                            ' There is at least one predecessor
                            CalcRow = CalcTB.Rows(0)
                            ToDate = DateAdd(DateInterval.Day, -1, CalcRow.Zeitpunkt)
                            Message = String.Format(My.Resources.MyStrings.mainMsgCalculationRollbackSingle, Scenario, ToDate)
                        Else
                            ' There is no predecessor
                            Message = String.Format(My.Resources.MyStrings.mainMsgCalculationRollbackAll, Scenario)
                            ToDate = DATENULLVALUE
                        End If
                        If MessageBox.Show(Message, My.Resources.MyStrings.mainMsgCalculationRollbackTitle, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                            Dim OrgScenarioID As Long = _TVM.SzenarioID
                            Try
                                _TVM.SzenarioID = ScenarioID
                                _TVM.RollbackCalculation(DateAdd(DateInterval.Day, 2, ToDate), (ToDate = DATENULLVALUE))
                                MessageBox.Show(My.Resources.MyStrings.mainMsgCalculationRollbackConfirmed, My.Resources.MyStrings.mainMsgCalculationRollbackTitle, MessageBoxButtons.OK)
                                ReloadTablesTab()
                            Catch ex As Exception
                                _TVM.SzenarioID = OrgScenarioID
                                DefaultErrorHandler(ex, ex.Message, True)
                            End Try
                        End If
                    End If
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' redirect double clicks to edit command
    ''' </summary>
    Private Sub grdTables_CellDoubleClick(sender As Object, e As EventArgs) Handles grdBerechnungen.CellDoubleClick,
            grdPlattformen.CellDoubleClick,
            grdKonten.CellDoubleClick,
            grdKurse.CellDoubleClick
        Select Case sender.Name
            Case "grdBerechnungen"
                tsmiEditTables_Click(tsmiEraseCalculation, Nothing)
            Case "grdPlattformen"
                tsmiEditTables_Click(tsmiEditPlattformen, Nothing)
            Case "grdKonten"
                tsmiEditTables_Click(tsmiEditKonten, Nothing)
            Case "grdKurse"
                tsmiEditTables_Click(tsmiEditKurse, Nothing)
        End Select
    End Sub
    Private Sub grdTrades_CellDoubleClick(sender As Object, e As EventArgs) Handles grdTrades.CellDoubleClick
        tsmiEditTrades_Click(tsmiEditTrades, Nothing)
    End Sub

    ''' <summary>
    ''' Show TaxCalculationSettings extended settings
    ''' </summary>
    Private Sub cmdTCSExtended_Click(sender As Object, e As EventArgs) Handles cmdTCSExtended.Click
        Dim ExtendedSettings As New frmTCSExtendedSettings
        With ExtendedSettings
            .MinutesTolerance = _TCS.ToleranceMinutes
            If .ShowDialog(Me) = DialogResult.OK Then
                _TCS.ToleranceMinutes = .MinutesTolerance
                cbxSzenario.Sticky = cbxSzenario.Sticky Or _TCS.Sticky
            End If
            .Dispose()
        End With
    End Sub

    ''' <summary>
    ''' take care of the cbxSettings sticky property
    ''' </summary>
    Private Sub cbxWalletAware_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxWalletAware.SelectedIndexChanged
        cbxSzenario.Sticky = cbxSzenario.Sticky Or _TCS.Sticky
    End Sub

    Private Sub cbxCoins4CoinsAccounting_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxCoins4CoinsAccounting.SelectedIndexChanged
        cbxSzenario.Sticky = cbxSzenario.Sticky Or _TCS.Sticky
    End Sub


    ''' <summary>
    ''' Initiate drag'n drop event: set mouse icon
    ''' </summary>
    Private Sub frmMain_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If Not InvokeRequired AndAlso e.Data.GetDataPresent(DataFormats.FileDrop) Then e.Effect = DragDropEffects.Move
    End Sub

    Private Sub frmMain_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        If Not InvokeRequired Then
            Dim DragFiles() As String = e.Data.GetData(DataFormats.FileDrop)
            If DragFiles?.Length > 0 Then
                ' Use these files for import
                PerformFileImport(PlatformManager.Platforms.Unknown, DragFiles)
            End If
        End If
    End Sub


    Private Sub LizenzinformationenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LizenzinformationenToolStripMenuItem.Click
        frmLicense.ShowDialog(Me)
    End Sub

#If CONFIG = "Debug" Then

    ''' <summary>
    ''' For Testing only: switch to a specific database file
    ''' </summary>
    ''' <param name="DatabaseFilename">Fully qualified name of the file to use as the database</param>
    Public Sub UseDatabase(DatabaseFilename As String, Optional Interactive As Boolean = True)
        _DBName = DatabaseFilename
        With New DBInit
            .Interactive = Interactive
            .InitDatabase(DatabaseFilename)
            .UpdateDatabase()
            frmMain_Load(Me, New EventArgs)
        End With
    End Sub

    Private Sub grdTrades_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles grdTrades.CellDoubleClick

    End Sub

    Private Sub grdTables_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles grdPlattformen.CellDoubleClick, grdKurse.CellDoubleClick, grdKonten.CellDoubleClick, grdBerechnungen.CellDoubleClick

    End Sub

#End If

End Class


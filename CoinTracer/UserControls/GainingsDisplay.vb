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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or in the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports System.ComponentModel

Public Class GainingsDisplay

    Public Event Changed(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Private _LabelWin As String = "Realisierter Gewinn:"
    Private _LabelLoss As String = "Realisierter Verlust:"
    Private _LabelPlatformGainings As String = "Auf gewählten Börsen:"
    Private _LabelCutOffDay As String = "Ermittelt bis Stichtag:"
    Private _LabelTaxable As String = "Steuerpflichtig:"
    Private _naLabel As String = " n.a."
    Private _DontRaiseChangedEvent As Boolean = False

    Private _ShowPlatformGainings As Boolean
    <Browsable(True)> _
    <Description("Legt fest, ob unter dem realisierten Gewinn auch der Gewinn selektierter Börsen dargestellt werden soll.")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    <Category("Appearance")> _
    <DefaultValue(False)> _
    Public Property ShowPlatformGainings() As Boolean
        Get
            Return _ShowPlatformGainings
        End Get
        Set(ByVal value As Boolean)
            _ShowPlatformGainings = value
            lblWinLossValue2.Visible = value
            Dim Loc As System.Drawing.Point
            Dim Delta As Single = lblWinLossValue2.Height - 2
            If value Then
                ' Control vergrößern
                Height += Delta
                Loc = lblTaxable.Location
                Loc.Y += Delta
                lblTaxable.Location = Loc
                Loc = lblInfo.Location
                Loc.Y = lblTaxable.Location.Y - 6
                lblInfo.Location = Loc
                Loc = lblStichtag.Location
                Loc.Y += Delta
                lblStichtag.Location = Loc
            Else
                ' Control verkleinern
                Height -= Delta
                Loc = lblTaxable.Location
                Loc.Y -= Delta
                lblTaxable.Location = Loc
                Loc = lblInfo.Location
                Loc.Y = lblTaxable.Location.Y - 6
                lblInfo.Location = Loc
                Loc = lblStichtag.Location
                Loc.Y -= Delta
                lblStichtag.Location = Loc
            End If
        End Set
    End Property


    ''' <summary>
    ''' Darzustellender Gewinn
    ''' </summary>
    Private _Gainings As Decimal
    Public Property Gainings() As Decimal
        Get
            Return _Gainings
        End Get
        Set(ByVal value As Decimal)
            _Gainings = value
            If Gainings >= 0 Then
                lblWinLossValue1.Text = _LabelWin & " " & _Gainings.ToString("N2") & " €"
                lblWinLossValue1.ForeColor = Color.DarkGreen
            Else
                lblWinLossValue1.Text = _LabelLoss & " " & _Gainings.ToString("N2") & " €"
                lblWinLossValue1.ForeColor = Color.DarkRed
            End If
            If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
        End Set
    End Property

    ''' <summary>
    ''' Darzustellender Gewinn auf den selektierten Plattformen
    ''' </summary>
    Private _PlatformGainings As Decimal
    Public Property PlatformGainings() As Decimal
        Get
            Return _PlatformGainings
        End Get
        Set(ByVal value As Decimal)
            _PlatformGainings = value
            lblWinLossValue2.Text = _LabelPlatformGainings & " " & _PlatformGainings.ToString("N2") & " €"
            If PlatformGainings >= 0 Then
                lblWinLossValue2.ForeColor = Color.DarkGreen
            Else
                lblWinLossValue2.ForeColor = Color.DarkRed
            End If
            If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
        End Set
    End Property

    ''' <summary>
    ''' Steuerpflichtiger Betrag
    ''' </summary>
    Private _TaxableGainings As Decimal
    Public Property TaxableGainings() As Decimal
        Get
            Return _TaxableGainings
        End Get
        Set(ByVal value As Decimal)
            _TaxableGainings = value
            lblTaxable.Text = _LabelTaxable & " " & _TaxableGainings.ToString("N2") & " €"
            ' Place info label depending on with of taxable gainings label
            Dim TextLength As Size = TextRenderer.MeasureText(lblTaxable.Text,
                                                              lblTaxable.Font)
            lblInfo.Left = lblTaxable.Left + TextLength.Width - 2
            lblInfo.Visible = True
            If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
        End Set
    End Property

    ''' <summary>
    ''' Stichtag der Gewinnermittlung
    ''' </summary>
    Private _CutOffDay As Date
    Public Property CutOffDay() As Date
        Get
            Return _CutOffDay
        End Get
        Set(ByVal value As Date)
            If value = DATENULLVALUE Then
                ResetDisplay()
            Else
                _CutOffDay = value
                lblStichtag.Text = _LabelCutOffDay & " " & CutOffDay.ToString("dd.MM.yyyy")
            End If
            If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
        End Set
    End Property

    Private _PlatformIDs As String
    ''' <summary>
    ''' Liste der Plattform-IDs, nach denen bei der Darstellung des Plattform-Gewinns gefiltert werden soll. 
    ''' Wenn leer, gibt es keine Filterung nach Plattform.
    ''' </summary>
    ''' <value>Kommaseparierte Liste aller Plattform-IDs, nach denen gefiltert werden soll.</value>
    Public Property PlatformIDs() As String
        Get
            Return _PlatformIDs
        End Get
        Set(ByVal value As String)
            If value IsNot Nothing Then
                _PlatformIDs = value.Replace(" ", "")
                If _PlatformIDs.StartsWith(",") Then
                    _PlatformIDs = _PlatformIDs.Substring(1, _PlatformIDs.Length - 1)
                End If
                If _PlatformIDs.EndsWith(",") Then
                    _PlatformIDs = _PlatformIDs.Substring(0, _PlatformIDs.Length - 1)
                End If
            Else
                _PlatformIDs = ""
            End If
        End Set
    End Property

    Private _ColumnName As String = ""

    Private WithEvents _TPCrtl As TimePeriodControl
    Public Sub LinkTimePeriodControl(ByRef TimePeriodControl As TimePeriodControl, Optional ColumnName As String = "Zeitpunkt")
        _TPCrtl = TimePeriodControl
        _ColumnName = ColumnName
    End Sub

    Public ReadOnly Property TimePeriodControl() As TimePeriodControl
        Get
            Return _TPCrtl
        End Get
    End Property

    Private _TVM As TradeValueManager
    Public Sub LinkTradeValueManager(ByRef TradeValueManager As TradeValueManager)
        _TVM = TradeValueManager
    End Sub

    Public ReadOnly Property TradeValueManager() As TradeValueManager
        Get
            Return _TVM
        End Get
    End Property

    ''' <summary>
    ''' Berechnet den Gewinn für den übergebenen Zeitraum neu und zeigt diesen an
    ''' </summary>
    ''' <param name="DontRaiseChangedEvent">Gibt an, ob das Changed-Event ausgelöst werden soll. Default: False</param>
    Public Sub Reload(Optional ByRef DontRaiseChangedEvent As Boolean = False)
        If _TVM IsNot Nothing And _TPCrtl IsNot Nothing Then
            Dim TaxableGainings As Decimal
            Dim PlatformGainings As Decimal
            If _ShowPlatformGainings Then
                Gainings = _TVM.CalculateGainings(_TPCrtl.DateFrom, _TPCrtl.DateTo, TaxableGainings, PlatformIDs, PlatformGainings)
                Me.PlatformGainings = PlatformGainings
            Else
                Gainings = _TVM.CalculateGainings(_TPCrtl.DateFrom, _TPCrtl.DateTo, TaxableGainings)
            End If
            Me.TaxableGainings = TaxableGainings
            CutOffDay = _TVM.GetGainingsCutOffDay
            If Not _DontRaiseChangedEvent And Not DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
            ' TODO: Entfernen!
            'Me.Gainings = 99.99
            'Me.PlatformGainings = 99.99
            'Me.TaxableGainings = 9.99
        End If

    End Sub

    ''' <summary>
    ''' Setzt Betrag und Stichtag der Anzeige
    ''' </summary>
    ''' <param name="Gainings"></param>
    ''' <param name="CutOffDay"></param>
    ''' <remarks></remarks>
    Public Sub SetValues(Gainings As Decimal, CutOffDay As Date)
        Me.Gainings = Gainings
        Me.CutOffDay = CutOffDay
        If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
    End Sub

    ''' <summary>
    ''' Setzt die Anzeige des Displays auf "n.a."
    ''' </summary>
    ''' <param name="PlatformGainingsonly">Wenn True, wird nur die Anzeige von Plattform-Gewinnen zurückgesetzt</param>
    Public Sub ResetDisplay(Optional ByVal PlatformGainingsonly As Boolean = False)
        lblWinLossValue2.Text = _LabelPlatformGainings & _naLabel
        lblWinLossValue2.ForeColor = Color.Black
        If Not PlatformGainingsonly Then
            lblWinLossValue1.Text = _LabelWin & _naLabel
            lblWinLossValue1.ForeColor = Color.Black
            lblStichtag.Text = _LabelCutOffDay & _naLabel
            lblTaxable.Text = ""
            lblInfo.Visible = False
        End If
        If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
    End Sub

    ''' <summary>
    ''' Indikator, ob das Control den Reset-Zustand hat
    ''' </summary>
    ''' <returns>True, wenn zurückgesetzt, sonst False</returns>
    Public ReadOnly Property IsReset() As Boolean
        Get
            Return lblWinLossValue1.Text = _LabelWin & _naLabel
        End Get
    End Property

    ''' <summary>
    ''' Kopiert die Zeitraum, Gewinn und Stichtagseigenschaften vom übergebenen GainingsDisplay-Control
    ''' </summary>
    Public Sub CloneFrom(ByRef SourceGainingsDisplay As GainingsDisplay)
        _DontRaiseChangedEvent = True
        Dim PeriodSelected As Boolean
        If SourceGainingsDisplay.IsReset Then
            ResetDisplay()
        Else
            Gainings = SourceGainingsDisplay.Gainings
            TaxableGainings = SourceGainingsDisplay.TaxableGainings
            CutOffDay = SourceGainingsDisplay.CutOffDay
            If ShowPlatformGainings AndAlso Not SourceGainingsDisplay.ShowPlatformGainings Then
                ResetDisplay(True)
            Else
                PlatformGainings = SourceGainingsDisplay.PlatformGainings
            End If
            If TimePeriodControl IsNot Nothing AndAlso SourceGainingsDisplay.TimePeriodControl IsNot Nothing Then
                PeriodSelected = SourceGainingsDisplay.TimePeriodControl.RadioButton1.Checked
                TimePeriodControl.dtpFrom.Value = SourceGainingsDisplay.TimePeriodControl.dtpFrom.Value
                TimePeriodControl.dtpTo.Value = SourceGainingsDisplay.TimePeriodControl.dtpTo.Value
                If PeriodSelected Then
                    TimePeriodControl.RadioButton1.Checked = True
                    TimePeriodControl.RadioButton2.Checked = False
                Else
                    TimePeriodControl.RadioButton1.Checked = False
                    TimePeriodControl.RadioButton2.Checked = True
                End If
                If SourceGainingsDisplay.TimePeriodControl.ComboBox1.SelectedIndex >= 0 Then
                    TimePeriodControl.ComboBox1.SelectedIndex = SourceGainingsDisplay.TimePeriodControl.ComboBox1.SelectedIndex
                End If
            End If
        End If
        _DontRaiseChangedEvent = False
    End Sub


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _TVM = Nothing
        _TPCrtl = Nothing
        _ShowPlatformGainings = False

    End Sub

    Private Sub _TPCrtl_SettingChanged(sender As Object, e As EventArgs) Handles _TPCrtl.SettingChanged
        If Not _DontRaiseChangedEvent Then RaiseEvent Changed(Me, New System.EventArgs())
    End Sub

End Class

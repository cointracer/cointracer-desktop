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

Imports CoinTracer.CoinValueStrategy
Imports System.ComponentModel

''' <summary>
''' Control zum Einstellen der CoinValueStrategy für einen bestimmten BusinesCase
''' </summary>
Public Class ValueStrategySelector

#Region "Scale-Awareness Support"

    Private currentScaleFactor As SizeF = New SizeF(1.0F, 1.0F)

    Protected Overrides Sub ScaleControl(factor As SizeF, specified As BoundsSpecified)
        MyBase.ScaleControl(factor, specified)
        ' Record the running scale factor used
        Me.currentScaleFactor.Width *= factor.Width
        Me.currentScaleFactor.Height *= factor.Height
        ' control specific positioning and sizing
        If factor.Width <> 1 Or factor.Height <> 1 Then
            Me.Width *= currentScaleFactor.Width
            grpCVS1.Width *= currentScaleFactor.Width
            grpCVS2.Width *= currentScaleFactor.Width
            cbxAgePref.Left *= currentScaleFactor.Width
        End If
    End Sub

#End Region

    Private _CVSObj As CoinValueStrategy
    Private _AvoidCascade As Boolean = False
    Private _StartEditStrategy As String

    Public Event SettingsChanged As EventHandler(Of EventArgs)

    ''' <summary>
    ''' Setzt die Radiobuttons des Controls anhand des übergebenen Strings
    ''' </summary>
    ''' <param name="ValueString">4 Zahlen, kommasepariert</param>
    ''' <remarks></remarks>
    Public Sub SetValues(ByVal ValueString As String)
        _CVSObj.FromString(ValueString)
        SetValues()
    End Sub

    ''' <summary>
    ''' Lädt die Werte des übergebenen CoinValueStrategy-Objekts in das Control und bindet dieses unmittelbar an.
    ''' </summary>
    ''' <param name="CoinValueStrategyObject"></param>
    Public Sub SetValues(Optional ByRef CoinValueStrategyObject As CoinValueStrategy = Nothing)
        If CoinValueStrategyObject IsNot Nothing AndAlso Not CoinValueStrategyObject.IsEmpty Then
            _CVSObj = CoinValueStrategyObject
        End If
        With _CVSObj
            Select Case .Preferration
                Case CoinValuePreferrations.Above1YearPreferred
                    cbxAgePref.SelectedItem = 1
                Case CoinValuePreferrations.Below1YearPreferred
                    cbxAgePref.SelectedItem = 2
                Case Else
                    cbxAgePref.SelectedItem = 0
            End Select
            _AvoidCascade = True
            Select Case _CVSObj.Below1YearStrategy
                Case CoinValueStrategies.CheapestFirst
                    rbLofo.Checked = True
                Case CoinValueStrategies.MostExpensiveFirst
                    rbHifo.Checked = True
                Case CoinValueStrategies.YoungestFirst
                    rbLifo.Checked = True
                Case CoinValueStrategies.OldestFirst
                    rbFifo.Checked = True
            End Select
            Select Case _CVSObj.Above1YearStrategy
                Case CoinValueStrategies.CheapestFirst
                    rbLofo2.Checked = True
                Case CoinValueStrategies.MostExpensiveFirst
                    rbHifo2.Checked = True
                Case CoinValueStrategies.YoungestFirst
                    rbLifo2.Checked = True
                Case CoinValueStrategies.OldestFirst
                    rbFifo2.Checked = True
            End Select
            StartEdit()
            Me.Invalidate()
        End With
        _AvoidCascade = False
    End Sub

    ''' <summary>
    ''' Liefert die aktuell eingestellte CoinValueStrategy zurück
    ''' </summary>
    Public Function GetValues() As CoinValueStrategy
        With _CVSObj
            Select Case cbxAgePref.SelectedIndex
                Case 1
                    .Preferration = CoinValuePreferrations.Above1YearPreferred
                Case 2
                    .Preferration = CoinValuePreferrations.Below1YearPreferred
                Case Else
                    .Preferration = CoinValuePreferrations.NothingPreferred
            End Select
            If rbFifo.Checked Then
                .Below1YearStrategy = CoinValueStrategies.OldestFirst
            ElseIf rbHifo.Checked Then
                .Below1YearStrategy = CoinValueStrategies.MostExpensiveFirst
            ElseIf rbLifo.Checked Then
                .Below1YearStrategy = CoinValueStrategies.YoungestFirst
            Else
                .Below1YearStrategy = CoinValueStrategies.CheapestFirst
            End If
            If rbFifo2.Checked Then
                .Above1YearStrategy = CoinValueStrategies.OldestFirst
            ElseIf rbHifo2.Checked Then
                .Above1YearStrategy = CoinValueStrategies.MostExpensiveFirst
            ElseIf rbLifo2.Checked Then
                .Above1YearStrategy = CoinValueStrategies.YoungestFirst
            Else
                .Above1YearStrategy = CoinValueStrategies.CheapestFirst
            End If
        End With
        Return _CVSObj
    End Function

    ''' <summary>
    ''' Merkt sich die aktuell eingestellte Strategie und verwendet diese für den "Sticky"-Vergleich
    ''' </summary>
    Public Sub StartEdit()
        _StartEditStrategy = Me.GetValues.ToString
    End Sub

    ''' <summary>
    ''' Ermittelt, ob das Control seit 'StartEdit' verändert wurde.
    ''' </summary>
    Public ReadOnly Property Sticky() As Boolean
        Get
            Return (Me.GetValues.ToString <> _StartEditStrategy)
        End Get
    End Property


    Private Sub rbSomething_CheckedChanged(sender As Object, e As EventArgs) Handles rbFifo.CheckedChanged,
        rbFifo2.CheckedChanged,
        rbHifo.CheckedChanged,
        rbHifo2.CheckedChanged,
        rbLifo.CheckedChanged,
        rbLifo2.CheckedChanged,
        rbLofo.CheckedChanged,
        rbLofo2.CheckedChanged
        If Not _AvoidCascade Then
            RaiseEvent SettingsChanged(Me, New EventArgs())
        End If
    End Sub

    ''' <summary>
    ''' Setzt die Standard-Einstellung ein, nämlich Fifo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ResetControls()
        SetValues("2,2,2,1,0")
    End Sub

    Public Sub New()

        Thread.CurrentThread.CurrentCulture = New CultureInfo(My.Settings.CurrentCulture)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _CVSObj = New CoinValueStrategy

    End Sub

    Private _FineTuning As Boolean
    <Description("Legt fest, ob die unteren 'Finetuning'-Einstellungen aktiviert sein sollen. Wenn False, dann kann nur zwischen FiFo und LiFo gewählt werden.")>
    <Category("Appearance")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property FineTuningEnabled() As Boolean
        Get
            Return _FineTuning
        End Get
        Set(ByVal value As Boolean)
            _FineTuning = value
            cbxAgePref.Enabled = value
            If Not value Then
                cbxAgePref.SelectedIndex = 0
            End If
            rbHifo.Enabled = value
            rbLofo.Enabled = value
            Me.Invalidate()
        End Set
    End Property

    <Description("Legt fest, ob Coins älter oder jünger als 1 Jahr bevozugt werden und getrennte Verbrauchsfolgeverfahren einstellbar sein sollen.")>
    <Category("Appearance")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property CoinPreferration() As CoinValuePreferrations
        Get
            Select Case cbxAgePref.SelectedIndex
                Case 1
                    Return CoinValuePreferrations.Above1YearPreferred
                Case 2
                    Return CoinValuePreferrations.Below1YearPreferred
                Case Else
                    Return CoinValuePreferrations.NothingPreferred
            End Select
        End Get
        Set(ByVal value As CoinValuePreferrations)
            Select Case value
                Case CoinValuePreferrations.Above1YearPreferred
                    cbxAgePref.SelectedIndex = 1
                Case CoinValuePreferrations.Below1YearPreferred
                    cbxAgePref.SelectedIndex = 2
                Case Else
                    cbxAgePref.SelectedIndex = 0
            End Select
            Me.Invalidate()
        End Set
    End Property


    Private Sub cbxAgePref_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxAgePref.SelectedIndexChanged
        If Not _AvoidCascade Then
            pnlOlderCoins.Visible = (DirectCast(sender, ComboBox).SelectedIndex <> 0)
            If pnlOlderCoins.Visible Then
                grpCVS1.Text = My.Resources.MyStrings.cvsGoupLabelBelow1Year
                grpCVS2.Text = My.Resources.MyStrings.cvsGoupLabelAbove1Year
            Else
                grpCVS1.Text = My.Resources.MyStrings.cvsGoupLabelDefault
            End If
            RaiseEvent SettingsChanged(Me, New EventArgs())
        End If
    End Sub

    Private Sub cbxWalletAware_SelectedIndexChanged(sender As Object, e As EventArgs)
        If Not _AvoidCascade Then
            RaiseEvent SettingsChanged(Me, New EventArgs())
        End If
    End Sub

End Class

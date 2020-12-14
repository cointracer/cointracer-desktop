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
        currentScaleFactor.Width *= factor.Width
        currentScaleFactor.Height *= factor.Height
        ' control specific positioning and sizing
        If factor.Width <> 1 Or factor.Height <> 1 Then
            Width *= currentScaleFactor.Width
            grpCVS1.Width *= currentScaleFactor.Width
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
            _AvoidCascade = True
            Select Case _CVSObj.ConsumptionStrategy
                Case CoinValueStrategies.CheapestFirst
                    rbLofo.Checked = True
                Case CoinValueStrategies.MostExpensiveFirst
                    rbHifo.Checked = True
                Case CoinValueStrategies.YoungestFirst
                    rbLifo.Checked = True
                Case CoinValueStrategies.OldestFirst
                    rbFifo.Checked = True
            End Select
            StartEdit()
            Invalidate()
        End With
        _AvoidCascade = False
    End Sub

    ''' <summary>
    ''' Liefert die aktuell eingestellte CoinValueStrategy zurück
    ''' </summary>
    Public Function GetValues() As CoinValueStrategy
        With _CVSObj
            If rbFifo.Checked Then
                .ConsumptionStrategy = CoinValueStrategies.OldestFirst
            ElseIf rbHifo.Checked Then
                .ConsumptionStrategy = CoinValueStrategies.MostExpensiveFirst
            ElseIf rbLifo.Checked Then
                .ConsumptionStrategy = CoinValueStrategies.YoungestFirst
            Else
                .ConsumptionStrategy = CoinValueStrategies.CheapestFirst
            End If
        End With
        Return _CVSObj
    End Function

    ''' <summary>
    ''' Merkt sich die aktuell eingestellte Strategie und verwendet diese für den "Sticky"-Vergleich
    ''' </summary>
    Public Sub StartEdit()
        _StartEditStrategy = GetValues.ToString
    End Sub

    ''' <summary>
    ''' Ermittelt, ob das Control seit 'StartEdit' verändert wurde.
    ''' </summary>
    Public ReadOnly Property Sticky() As Boolean
        Get
            Return (GetValues.ToString <> _StartEditStrategy)
        End Get
    End Property


    Private Sub rbSomething_CheckedChanged(sender As Object, e As EventArgs) Handles rbFifo.CheckedChanged,
        rbHifo.CheckedChanged,
        rbLifo.CheckedChanged,
        rbLofo.CheckedChanged

        If Not _AvoidCascade Then
            RaiseEvent SettingsChanged(Me, New EventArgs())
        End If
    End Sub

    ''' <summary>
    ''' Setzt die Standard-Einstellung ein, nämlich Fifo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ResetControls()
        SetValues(DefaultPropertyString)
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
            rbHifo.Enabled = value
            rbLofo.Enabled = value
            Invalidate()
        End Set
    End Property

End Class

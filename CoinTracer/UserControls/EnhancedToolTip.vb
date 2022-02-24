'  **************************************
'  *
'  * Copyright 2013-2022 Andreas Nebinger
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

''' <summary>
''' Extends the ToolTip class by a property "ShowWhenDisabled"
''' </summary>
''' <remarks></remarks>
Public Class EnhancedToolTip
    Inherits System.Windows.Forms.ToolTip
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    'Das Steuerelement überschreibt den Löschvorgang zum Bereinigen der Komponentenliste.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If _Timer IsNot Nothing Then
                    _Timer.Stop()
                    _Timer = Nothing
                End If
                If _TTControls IsNot Nothing Then
                    For Each c As Control In _TTControls
                        RemoveHandler c.MouseMove, AddressOf Control_MouseMove
                    Next
                End If
                If components IsNot Nothing Then
                    components.Dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region

    Private _ShowWhenDisabled As Boolean
    Private _TTControls As List(Of Control)
    Private _CurrentMouseControl As Control
    Private _TTReadyControl As Control
    Private _TTShowingControl As Control
    Private WithEvents _Timer As System.Timers.Timer

    ''' <summary>
    ''' Determines wether the tool tip is shown even on disabled controls.
    ''' </summary>
    <Browsable(True)> _
    <DefaultValue(True)> _
    <Description("Determines wether the tool tip is shown even on disabled controls.")> _
    Public Property ShowWhenDisabled() As Boolean
        Get
            Return _ShowWhenDisabled
        End Get
        Set(ByVal value As Boolean)
            _ShowWhenDisabled = value
        End Set
    End Property


    Public Sub New()

        MyBase.New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _TTControls = New List(Of Control)
        _TTShowingControl = Nothing
        _CurrentMouseControl = Nothing
        _TTReadyControl = Nothing
        _Timer = New Timers.Timer()
        _Timer.Enabled = False
        Me.ShowWhenDisabled = True

    End Sub

    Private Sub Control_MouseMove(sender As Object, e As MouseEventArgs)
        Dim Ctrl As Control = DirectCast(sender, Control).GetChildAtPoint(e.Location)
        If Ctrl IsNot Nothing Then
            _CurrentMouseControl = Ctrl
            If _TTShowingControl IsNot Nothing AndAlso _TTShowingControl.Name <> Ctrl.Name Then
                ' Aktuell angezeigtes QuickInfo-Fenster ausblenden
                ResetTTShowingControl()
            ElseIf _TTReadyControl IsNot Nothing AndAlso _TTReadyControl.Name <> Ctrl.Name Then
                ' Wartezeit zur Anzeige des QuickInfo-Fensters abbrechen
                ResetTTReadyControl()
            End If
            If _TTReadyControl Is Nothing And _TTShowingControl Is Nothing Then
                Dim ToolTipString As String = Me.GetToolTip(Ctrl)
                If Me.ShowWhenDisabled AndAlso ToolTipString <> "" Then
                    SetToolTipTimer(Ctrl)
                End If
            End If
        Else
            ResetTTReadyControl()
            ResetTTShowingControl()
        End If
    End Sub

    ''' <summary>
    ''' Initiiert den internern Timer für das Anzeigen des ToolTips eines bestimmten Controls
    ''' </summary>
    Private Sub SetToolTipTimer(ToolTipControl As Control)
        _Timer.Stop()
        _TTReadyControl = ToolTipControl
        _Timer.Interval = Me.InitialDelay
        _Timer.Start()
    End Sub


    Private Sub _Timer_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles _Timer.Elapsed
        _Timer.Stop()
        If _TTReadyControl IsNot Nothing Then
            If _CurrentMouseControl.Name = _TTReadyControl.Name Then
                _TTShowingControl = _TTReadyControl
                Me.DoShowQuickInfo()
                _TTReadyControl = Nothing
            Else
                Me.ResetTTReadyControl()
            End If
        End If
    End Sub

    Public Delegate Sub ShowQuickInfoDelegate()

    Private Sub DoShowQuickInfo()
        If _TTShowingControl.InvokeRequired Then
            _TTShowingControl.Invoke(New ShowQuickInfoDelegate(AddressOf DoShowQuickInfo))
        Else
            Me.Show(Me.GetToolTip(_TTShowingControl), _TTShowingControl, _TTShowingControl.PointToClient(Cursor.Position).X, _TTShowingControl.Height, Me.AutoPopDelay)
        End If
    End Sub

    ''' <summary>
    ''' Blendet das QuickInfo-Fenster des aktuell eingestellten ToolTip-Controls aus und stoppt den internen Timer
    ''' </summary>
    Private Sub ResetTTShowingControl()
        If _TTShowingControl IsNot Nothing Then
            Me.Hide(_TTShowingControl)
            _TTShowingControl = Nothing
        End If
    End Sub

    Private Sub ResetTTReadyControl()
        If _TTReadyControl IsNot Nothing Then
            _TTReadyControl = Nothing
            _Timer.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Ordnet QuickInfo-Text dem angegebenen Steuerelement zu.
    ''' </summary>
    ''' <param name="control">Das System.Windows.Forms.Control, dem der QuickInfo-Text zugeordnet werden soll.</param>
    ''' <param name="caption">Der QuickInfo-Text, der angezeigt wird, wenn sich der Zeiger auf dem Steuerelement befindet.</param>
    ''' <remarks></remarks>
    Public Overloads Sub SetToolTip(control As System.Windows.Forms.Control, caption As String)
        Dim Parent As Control = control.Parent
        ' Event-Handler für MouseMove beim Parent hinzufügen
        If Not _TTControls.Contains(Parent) AndAlso Parent IsNot Nothing Then
            _TTControls.Add(Parent)
            AddHandler Parent.MouseMove, AddressOf Control_MouseMove
        End If
        ' Basis SetToolTip aufrufen
        MyBase.SetToolTip(control, caption)
    End Sub

End Class

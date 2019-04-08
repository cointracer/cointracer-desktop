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

Public Class ThreadManagerLight

    Public Event BeginThread As EventHandler(Of EventArgs)
    Public Event EndThread As EventHandler(Of EventArgs)

    Public Delegate Sub WorkRoutineDelegate()
    Private Delegate Sub RoutineWithStringArg(ByVal MessageText As String)

    Private _WorkRoutine As WorkRoutineDelegate
    Private _StatusStrip As ThreadsafeStatusStrip

    Private _WorkRoutineName As String
    ''' <summary>
    ''' Kennung der aktuell laufenden Routine
    ''' </summary>
    Public Property WorkRoutineName() As String
        Get
            Return _WorkRoutineName
        End Get
        Set(ByVal value As String)
            _WorkRoutineName = value
        End Set
    End Property

    Private _MessageText As String
    ''' <summary>
    ''' Setzt den Text, der in der StatusBar angezeigt werden soll oder liefert diesen zurück.
    ''' </summary>
    Public Property MessageText() As String
        Get
            Return _MessageText
        End Get
        Set(ByVal value As String)
            _MessageText = value
            If _StatusStrip IsNot Nothing Then
                _StatusStrip.SetMessageText(value & "   ")
            End If
        End Set
    End Property

    Private _Thread As Thread
    ''' <summary>
    ''' Gibt den Thread zurück, den das Objekt verwaltet
    ''' </summary>
    Public ReadOnly Property Thread() As Thread
        Get
            Return _Thread
        End Get
    End Property

    ''' <summary>
    ''' Gibt die Information zurück, ob der anfragende Thread ein anderer ist als derjenige, in dem die StatusBar erzeugt wurde.
    ''' </summary>
    Public ReadOnly Property InvokeRequired As Boolean
        Get
            If _StatusStrip Is Nothing Then
                Return False
            Else
                Return _StatusStrip.InvokeRequired
            End If
        End Get
    End Property

    Private _KeepEventsInMainThread As Boolean
    ''' <summary>
    ''' Kennzeichnet, ob die Events BeginThread und EndThread im Ursprungs-Thread ausgeführt werden sollen. Default = False
    ''' </summary>
    Public Property KeepEventsInMainThread() As Boolean
        Get
            Return _KeepEventsInMainThread
        End Get
        Set(ByVal value As Boolean)
            _KeepEventsInMainThread = value
        End Set
    End Property



    ''' <summary>
    ''' Initialisiert das Objekt zum einfachen Starten asynchroner Routinen.
    ''' </summary>
    ''' <param name="WorkRoutine">Adresse der auszuführenden Routine.</param>
    ''' <param name="WorkRoutineName">Bezeichnung der WorkRoutine. Kann in Events 'BeginThread' und 'EndThread' verwendet werden.</param>
    ''' <param name="ThreadSafeStatusStrip">ThreadSafeStatusStrip-Control, auf dem Meldungen ausgegeben werden.</param>
    ''' <param name="MessageText">Meldungstext, der beim Start der WorkRoutine in das ThreadSafeStatusStrip-Control geschrieben wird.</param>
    Public Sub New(ByVal WorkRoutine As WorkRoutineDelegate, _
                    Optional ByVal WorkRoutineName As String = "Routine", _
                    Optional ThreadSafeStatusStrip As ThreadsafeStatusStrip = Nothing, _
                    Optional ByVal MessageText As String = "Bitte warten Sie...", _
                    Optional KeepEventsInMainThread As Boolean = False)
        If ThreadSafeStatusStrip IsNot Nothing Then
            _StatusStrip = ThreadSafeStatusStrip
            _KeepEventsInMainThread = KeepEventsInMainThread
        Else
            _KeepEventsInMainThread = False
        End If
        SetWorkRoutine(WorkRoutine, WorkRoutineName, MessageText)
    End Sub

    ''' <summary>
    ''' Initialisiert das Objekt zum einfachen Starten asynchroner Routinen.
    ''' </summary>
    ''' <param name="ThreadSafeStatusStrip">ThreadSafeStatusStrip-Control, auf dem Meldungen ausgegeben werden.</param>
    Public Sub New(ByRef ThreadSafeStatusStrip As ThreadsafeStatusStrip)
        If ThreadSafeStatusStrip IsNot Nothing Then
            _StatusStrip = ThreadSafeStatusStrip
        End If
    End Sub

    ''' <summary>
    ''' Startet die WorkRoutine in einem eigenen Thread (falls dieser nicht schon gestartet war)
    ''' </summary>
    Public Sub StartThread()
        If _Thread Is Nothing Then
            _Thread = New Thread(AddressOf DoWork)
        End If
        If _Thread.IsAlive = False Then
            _Thread.Start()
        End If
    End Sub

    ''' <summary>
    ''' Bricht den Thread ab
    ''' </summary>
    Public Sub AbortThread()
        Dim WasRunning As Boolean = False
        If _Thread IsNot Nothing Then
            WasRunning = _Thread.IsAlive
            _Thread.Abort()
            _Thread = Nothing
            If WasRunning Then
                RaiseEvent EndThread(Me, New EventArgs())
            End If
        End If
    End Sub

    ''' <summary>
    ''' Setzt die Text-Eigenschaft eines Controls (thread safe)
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <remarks></remarks>
    Public Sub SetControlText(Control As Control, Text As String)
        If Control.InvokeRequired Then
            Control.Invoke(Sub() Control.Text = Text)
        Else
            Control.Text = Text
        End If
    End Sub

    ''' <summary>
    ''' Setzt die auszuführenden Routine, die zugehörige Bezeichnung und den anzuzeigenden Text neu.
    ''' </summary>
    ''' <param name="WorkRoutine">Adresse der auszuführenden Routine.</param>
    ''' <param name="WorkRoutineName">Bezeichnung der WorkRoutine. Kann in Events 'BeginThread' und 'EndThread' verwendet werden.</param>
    ''' <param name="MessageText">Meldungstext, der beim Start der WorkRoutine in das ThreadSafeStatusStrip-Control geschrieben wird.</param>
    ''' <remarks></remarks>
    Public Sub SetWorkRoutine(ByVal WorkRoutine As WorkRoutineDelegate, _
                        Optional ByVal WorkRoutineName As String = "Routine", _
                        Optional ByVal MessageText As String = "Bitte warten Sie...")
        AbortThread()
        _WorkRoutine = WorkRoutine
        _WorkRoutineName = WorkRoutineName
        _MessageText = MessageText
    End Sub

    ''' <summary>
    ''' Prüft, ob gerade ein Thread läuft. Wenn WorkRoutineName angegeben ist, wird zusätzlich geprüft, ob die Bezeichnungen überein stimmen.
    ''' </summary>
    ''' <param name="WorkRoutineName">Bezeichnung der zu überprüfenden WorkRoutine.</param>
    ''' <returns>True, wenn ein Thread läuft, sonst False</returns>
    Public Function IsRunning(Optional ByVal WorkRoutineName As String = "") As Boolean
        If WorkRoutineName = "" Then
            Return _Thread IsNot Nothing AndAlso _Thread.IsAlive
        Else
            Return WorkRoutineName = _WorkRoutineName AndAlso _Thread IsNot Nothing AndAlso _Thread.IsAlive
        End If
    End Function

    ''' <summary>
    ''' Führt die WorkRoutine aus, setzt den Text des StatusStrips und löst die zugehörigen Events aus
    ''' </summary>
    Private Sub DoWork()

        If InvokeRequired And Not _KeepEventsInMainThread And _StatusStrip IsNot Nothing Then
            _StatusStrip.Invoke(New RoutineWithStringArg(AddressOf _StatusStrip.SetMessageText), New Object() {_MessageText})
            _StatusStrip.Invoke(New WorkRoutineDelegate(AddressOf DoBeginThread))
            _WorkRoutine()
            _StatusStrip.Invoke(New WorkRoutineDelegate(AddressOf _StatusStrip.Clear))
            _StatusStrip.Invoke(New WorkRoutineDelegate(AddressOf DoEndThread))
        Else
            MessageText = _MessageText
            DoBeginThread()
            _WorkRoutine()
            MessageText = ""
            DoEndThread()
        End If
    End Sub

    Private Sub DoBeginThread()
        RaiseEvent BeginThread(Me, New EventArgs())
    End Sub
    Private Sub DoEndThread()
        RaiseEvent EndThread(Me, New EventArgs())
    End Sub

    ''' <summary>
    ''' Löst das BeginThread-Ereignis aus.
    ''' </summary>
    Public Sub OnBeginThread()
        If InvokeRequired And Not _KeepEventsInMainThread And _StatusStrip IsNot Nothing Then
            _StatusStrip.Invoke(New WorkRoutineDelegate(AddressOf DoBeginThread))
        Else
            DoBeginThread()
        End If
    End Sub

    ''' <summary>
    ''' Löst das EndThread-Ereignis aus.
    ''' </summary>
    Public Sub OnEndThread()
        If InvokeRequired And Not _KeepEventsInMainThread And _StatusStrip IsNot Nothing Then
            _StatusStrip.Invoke(New WorkRoutineDelegate(AddressOf DoEndThread))
        Else
            DoEndThread()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        AbortThread()
    End Sub

End Class

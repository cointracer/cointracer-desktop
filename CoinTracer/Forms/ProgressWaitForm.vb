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

Public Class ProgressWaitForm

    Private Const WM_NCLBUTTONDOWN As Int32 = 161
    Private Const HT_CAPTION As Int32 = 2
    Private Const SW_SHOWNA As Int32 = 8

    Private Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    Private Declare Auto Function ReleaseCapture Lib "user32.dll" () As Boolean

    Private _BaseHeight As Integer
    Private _ParentForm As Form


    Public Sub New(ByVal parentForm As Form)
        InitializeComponent()
        _ParentForm = parentForm
        Left = _ParentForm.Left + (_ParentForm.Width / 2) - (Width / 2)
        Top = _ParentForm.Top + (_ParentForm.Height / 2) - (Height / 2)
        _BaseHeight = Height
        _WithCancel = False
        _Canceled = False
        Me.ShowInTaskbar = True
        'TopMost = True
        'TopLevel = True
        Me.BringToFront()
    End Sub

    Private _WithCancel As Boolean
    Public Property WithCancelOption() As Boolean
        Get
            Return _WithCancel
        End Get
        Set(ByVal value As Boolean)
            _WithCancel = value
            If InvokeRequired Then
                Invoke(New SetCancelOptionDelegate(AddressOf DoSetCancelOption), New Object() {value})
            Else
                DoSetCancelOption(value)
            End If
        End Set
    End Property

    Private _Canceled As Boolean
    Public ReadOnly Property Canceled() As Boolean
        Get
            Return _Canceled
        End Get
    End Property

    Public Shadows Sub Close()
        CloseProgress()
    End Sub

    Private Delegate Sub UpdateProgressDelegate(ByVal progressPercent As Integer, ByVal statusText As String)
    Private Delegate Sub UpdateProgressTextDelegate(ByVal statusText As String)
    Private Delegate Sub CloseProgressDelegate()
    Private Delegate Sub SetCancelOptionDelegate(ByVal WithCancelOption As Boolean)

    Private Sub DoSetCancelOption(ByVal WithCancelOption As Boolean)
        cmdCancel.Visible = WithCancelOption
        If WithCancelOption Then
            ProgressBar.Width = 367
        Else
            ProgressBar.Width = 464
        End If
    End Sub

    ''' <summary>
    ''' Updates the progress bar value and/or status text
    ''' </summary>
    ''' <param name="progressPercent">A value from 0 to 100 representing the precentage complete</param>
    ''' <param name="statusText">Any status text to accompany the progressbar</param>
    Public Sub UpdateProgress(ByVal progressPercent As Integer, ByVal statusText As String)
        If InvokeRequired Then
            Invoke(New UpdateProgressDelegate(AddressOf DoUpdateProgress), New Object() {progressPercent, statusText})
        Else
            DoUpdateProgress(progressPercent, statusText)
        End If
    End Sub
    Private Sub DoUpdateProgress(ByVal progressPercent As Integer, ByVal statusText As String)
        ProgressBar.Value = Math.Min(progressPercent, 100)
        StatusLabel.Text = statusText
    End Sub

    ''' <summary>
    ''' Updates the progress bar status text
    ''' </summary>
    ''' <param name="statusText">Any status text to accompany the progressbar</param>
    Public Sub UpdateProgress(ByVal statusText As String)
        If InvokeRequired Then
            Invoke(New UpdateProgressTextDelegate(AddressOf DoUpdateProgressText), New Object() {statusText})
        Else
            DoUpdateProgressText(statusText)
        End If
    End Sub
    Private Sub DoUpdateProgressText(ByVal statusText As String)
        StatusLabel.Text = statusText
    End Sub
    ''' <summary>
    ''' Closes the ProgressWaitForm - Be sure to call CloseProgress() when finished with this ProgressWaitForm instance
    ''' </summary>
    Public Sub CloseProgress()
        If InvokeRequired Then
            Invoke(New CloseProgressDelegate(AddressOf DoCloseProgress))
        Else
            DoCloseProgress()
        End If
    End Sub

    Private Sub DoCloseProgress()
        MyBase.Close()
        MyBase.Dispose()
    End Sub

    <Runtime.InteropServices.DllImport("User32.dll")>
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As IntPtr
    End Function
    Private Shared Function ShowWindow(ByVal hWnd As IntPtr,
            ByVal nCmdShow As Integer) As IntPtr
    End Function

    Private Sub ProgressWaitForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Try
            Dim procs() As Process =
                Process.GetProcessesByName(My.Application.Info.AssemblyName)
            If procs.Length = 1 Then
                Dim index As Integer
                If CInt(procs(0).MainWindowHandle) <> 0 Then
                    index = 0
                Else
                    index = 1
                End If
                SetForegroundWindow(procs(index).MainWindowHandle)
                ShowWindow(procs(index).MainWindowHandle, SW_SHOWNA)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ProgressWaitForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        ' Check if the user tries to drag the form around
        If e.Button = MouseButtons.Left Then
            ReleaseCapture
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub

    Private Sub Control_MouseDown(sender As Object, e As MouseEventArgs) Handles ProgressBar.MouseDown, StatusLabel.MouseDown
        Me.OnMouseDown(e)   ' Send this event straight to the form event handler
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        _Canceled = True
        DirectCast(sender, Button).Text = My.Resources.MyStrings.waitFormCancelledCaption
        DirectCast(sender, Button).Enabled = False
    End Sub
End Class


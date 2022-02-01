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

Imports System.Text


Public Class MsgBoxEx

    Private Shared _Labels() As String    '' Desired new labels
    Private Shared _LabelIndex As Integer '' Next caption to update
    Private Shared _hWndBox As IntPtr
    Private Shared _hWndButtons() As IntPtr
    Private Shared _HelpCtrl As Control

    ''' <summary>
    ''' Displays a message box and brings it to the front
    ''' </summary>
    Public Shared Function ShowInFront(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton) As DialogResult
        PatchMsgBox(Nothing)
        Return MessageBox.Show(text, caption, buttons, icon, defaultButton)
    End Function
    ''' <summary>
    ''' Displays a message box and brings it to the front
    ''' </summary>
    Public Shared Function ShowInFront(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        PatchMsgBox(Nothing)
        Return MessageBox.Show(text, caption, buttons, icon)
    End Function

    Public Shared Sub PatchMsgBox(ByVal ButtonLabels() As String)
        ''--- Updates message box buttons
        _Labels = ButtonLabels
        Application.OpenForms(0).BeginInvoke(New FindWindowDelegate(AddressOf FindMsgBox), GetCurrentThreadId())
    End Sub

    Public Shared Sub BringToFront()
        PatchMsgBox(Nothing)
    End Sub

    Private Shared Sub FindMsgBox(ByVal tid As Integer)
        ''--- Enumerate the windows owned by the UI thread
        EnumThreadWindows(tid, AddressOf EnumWindow, IntPtr.Zero)
    End Sub

    <Runtime.InteropServices.DllImport("User32.dll")> _
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("User32.dll")> _
    Private Shared Function ShowWindow(ByVal hWnd As IntPtr, _
            ByVal nCmdShow As Integer) As IntPtr
    End Function

    Private Shared Function EnumWindow(ByVal hWnd As IntPtr, ByVal lp As IntPtr) As Boolean
        ''--- Is this the message box?
        Dim sb As New StringBuilder(256)
        GetClassName(hWnd, sb, sb.Capacity)
        If sb.ToString() <> "#32770" Then Return True
        ''--- Got it, now find the buttons and bring to front
        _hWndBox = hWnd
        If _Labels IsNot Nothing AndAlso _Labels.Length > 0 Then
            _LabelIndex = 0
            ReDim _hWndButtons(0)
            EnumChildWindows(hWnd, AddressOf FindButtons, IntPtr.Zero)
        End If
        SetForegroundWindow(_hWndBox)
        ShowWindow(_hWndBox, 8) '8=SW_SHOWNA
        Return False
    End Function

    Private Shared Function FindButtons(ByVal hWnd As IntPtr, ByVal lp As IntPtr) As Boolean
        Dim sb As New StringBuilder(256)
        GetClassName(hWnd, sb, sb.Capacity)
        If sb.ToString() = "Button" And _LabelIndex <= UBound(_Labels) Then
            ' got one, update text
            ReDim Preserve _hWndButtons(_LabelIndex)
            _hWndButtons(_LabelIndex) = hWnd
            SetWindowText(hWnd, _Labels(_LabelIndex))
            AdjustButtonSizeAndPos(hWnd, _Labels(_LabelIndex))
            ' reset the label
            _Labels(_LabelIndex) = ""
            _LabelIndex += 1
        End If
        Return True
    End Function

    Private Shared Function AdjustButtonSizeAndPos(ByVal hWnd As IntPtr, ByVal Caption As String) As Boolean
        Dim wRct As Rectangle
        GetWindowRect(hWnd, wRct)

        Dim hDC As IntPtr = GetWindowDC(hWnd)
        Dim gDC As Graphics = Graphics.FromHdc(hDC)
        Dim fnt As Font = SystemFonts.DialogFont
        Dim stringSizeF As SizeF = gDC.MeasureString("XX" & Caption & "XX", fnt)
        Dim TextWidth As Integer = CType(stringSizeF.Width, Integer)
        If TextWidth > wRct.Width - wRct.X Then
            Dim pnt As Point = New Point(wRct.X, wRct.Y)
            ScreenToClient(_hWndBox, pnt)
            ' Calculate movement of button to left
            Dim xDelta = TextWidth - wRct.Width + wRct.X
            MoveWindow(hWnd, pnt.X - xDelta, pnt.Y, TextWidth, wRct.Height - wRct.Y, False)
            ' do also move all other buttons (if there are any)
            For i As Integer = _LabelIndex - 1 To 0 Step -1
                MoveButtonLeft(_hWndButtons(i), xDelta)
            Next
        End If
        Return True
    End Function

    Private Shared Function MoveButtonLeft(ByVal hWnd As IntPtr, ByVal MoveLeft As Integer) As Boolean
        Dim wRct As Rectangle
        GetWindowRect(hWnd, wRct)
        Dim pnt As Point = New Point(wRct.X, wRct.Y)
        ScreenToClient(_hWndBox, pnt)
        MoveWindow(hWnd, pnt.X - MoveLeft, pnt.Y, wRct.Width - wRct.X, wRct.Height - wRct.Y, False)
        Return True
    End Function

    ''--- P/Invoke declarations
    Private Delegate Sub FindWindowDelegate(ByVal tid As Integer)
    Private Delegate Function EnumWindowDelegate(ByVal hWnd As IntPtr, ByVal lp As IntPtr) As Boolean

#Region "Not-Again-Functions"

    ''' <summary>
    ''' Liefert zu einem Default-DialogResult die entsprechende Button-Caption zurück
    ''' </summary>
    ''' <param name="DefaultValue">Default-DialogResult</param>
    ''' <returns>String, der als Button-Caption verwendet werden kann.</returns>
    Public Shared Function NotAgainButtonCaption(ByVal DefaultValue As DialogResult) As String
        Select Case DefaultValue
            Case DialogResult.Abort, DialogResult.Cancel
                NotAgainButtonCaption = My.Resources.MyStrings.Cancel & My.Resources.MyStrings.globalNotAgainSuffix
            Case DialogResult.Yes
                NotAgainButtonCaption = My.Resources.MyStrings.Yes & My.Resources.MyStrings.globalNotAgainSuffix
            Case DialogResult.Ignore
                NotAgainButtonCaption = My.Resources.MyStrings.Ignore & My.Resources.MyStrings.globalNotAgainSuffix
            Case DialogResult.No
                NotAgainButtonCaption = My.Resources.MyStrings.No & My.Resources.MyStrings.globalNotAgainSuffix
            Case DialogResult.Retry
                NotAgainButtonCaption = My.Resources.MyStrings.Retry & My.Resources.MyStrings.globalNotAgainSuffix
            Case Else
                NotAgainButtonCaption = My.Resources.MyStrings.OK & My.Resources.MyStrings.globalNotAgainSuffix
        End Select
        Return NotAgainButtonCaption
    End Function


    ''' <summary>
    ''' Prüft, ob für den übergebenen MessageBoxQualifier ein Default-DialogResult hinterlegt ist.
    ''' </summary>
    ''' <param name="MessageBoxQualifier">Kürzel des zu prüfenden Dialogs</param>
    ''' <returns>Vorbelegtes DialogResult (wenn Dialog nicht mehr angezeigt werden soll), sonst DialogResult.None.</returns>
    Public Shared Function GetDefaultDialogResult(ByRef MessageBoxQualifier As String) As DialogResult
        Try
            Dim Result As String = My.Settings.MessageBoxSettings
            Dim FindString As String = "|" & MessageBoxQualifier & ","
            If Result.Contains(FindString) Then
                Return Convert.ToInt16(Result.Substring(Result.IndexOf(FindString) + FindString.Length, 1))
            Else
                Return DialogResult.None
            End If
        Catch ex As Exception
            Return DialogResult.None
        End Try
    End Function

    ''' <summary>
    ''' Setzt für den übergebenen MessageBoxQualifier ein Default-DialogResult und bewirkt, dass die entsprechende MessageBox nicht mehr angezeigt wird.
    ''' </summary>
    ''' <param name="MessageBoxQualifier">Kürzel des zu setzenden Dialogs. Wenn dieser wieder angezeigt werden soll, muss DialogResult.None übergeben werden.</param>
    Public Shared Sub SetDefaultDialogResult(ByRef MessageBoxQualifier As String, DefaultDialogResult As DialogResult)
        Dim Result As String = My.Settings.MessageBoxSettings
        Dim FindString As String = "|" & MessageBoxQualifier & ","
        If Result.Contains(FindString) Then
            My.Settings.MessageBoxSettings = My.Settings.MessageBoxSettings.Replace(Result.Substring(Result.IndexOf(FindString), FindString.Length + 1), FindString & DirectCast(DefaultDialogResult, Integer))
        Else
            My.Settings.MessageBoxSettings &= FindString & DirectCast(DefaultDialogResult, Integer)
        End If
    End Sub

    ''' <summary>
    ''' Zeigt eine MessageBox mit einem zusätzlichen Button "[DefaultResult], Meldung nicht mehr anzeigen" an. Wird dieser ausgewählt, wird
    ''' die MessageBox zukünftig nicht mehr angezeigt, sondern sofort das DefaultResult zurückgegeben.
    ''' </summary>
    ''' <param name="MessageBoxQualifier">Eindeutiger Identifizierer für diese Meldung. Bitte daran denken, diesen auch in frmApplicationSettings->LoadSettingsToForm einzupflegen!</param>
    ''' <param name="DefaultResult">Standard-Result, wenn die Meldung durch den User ausgeblendet wurde</param>
    Public Shared Function ShowWithNotAgainOption(ByVal MessageBoxQualifier As String, _
                                               ByVal DefaultResult As DialogResult, _
                                               ByVal Text As String, _
                                               Optional ByVal Caption As String = "%APPNAME%", _
                                               Optional ByVal Buttons As System.Windows.Forms.MessageBoxButtons = MessageBoxButtons.OK, _
                                               Optional ByVal Icon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.None, _
                                               Optional ByVal DefaultButton As System.Windows.Forms.MessageBoxDefaultButton = MessageBoxDefaultButton.Button1) As DialogResult
        If Caption = "%APPNAME%" Then
            Caption = Application.ProductName
        End If

        Dim Result As DialogResult = GetDefaultDialogResult(MessageBoxQualifier)
        If Result <> DialogResult.None Then
            ' Default-DialogResult gesetzt, also direkt zurück liefern, keine MessageBox anzeigen 
            Return Result
        Else
            ' Kein Standard gesetzt, also MessageBox mit Zusatz-Button anzeigen
            Dim NewButtons As System.Windows.Forms.MessageBoxButtons
            Dim ButtonLabels As String()
            Select Case Buttons
                Case MessageBoxButtons.OK
                    NewButtons = MessageBoxButtons.YesNo
                    ButtonLabels = {NotAgainButtonCaption(DefaultResult), My.Resources.MyStrings.OK}
                Case MessageBoxButtons.OKCancel
                    NewButtons = MessageBoxButtons.YesNoCancel
                    ButtonLabels = {NotAgainButtonCaption(DefaultResult), My.Resources.MyStrings.OK, My.Resources.MyStrings.Cancel}
                Case MessageBoxButtons.RetryCancel
                    NewButtons = MessageBoxButtons.YesNoCancel
                    ButtonLabels = {NotAgainButtonCaption(DefaultResult), My.Resources.MyStrings.Retry, My.Resources.MyStrings.Cancel}
                Case MessageBoxButtons.YesNo
                    NewButtons = MessageBoxButtons.YesNoCancel
                    ButtonLabels = {NotAgainButtonCaption(DefaultResult), My.Resources.MyStrings.Yes, My.Resources.MyStrings.No}
                Case Else
                    NewButtons = Buttons
                    ButtonLabels = {}
            End Select
            ' Buttons neu labeln
            If _Labels IsNot Nothing AndAlso _Labels.Length > 0 AndAlso _Labels(0) <> "" Then
                ' MessageBox ist schon gepatcht, ggf. nur erstes Label einschieben
                If ButtonLabels.Length > 0 Then
                    For i As Integer = _Labels.Length - 1 To 1 Step -1
                        _Labels(i) = _Labels(i - 1)
                    Next
                    _Labels(0) = NotAgainButtonCaption(DefaultResult)
                End If
            Else
                ' MessageBox ist nicht gepatcht, also Default-Labels vergeben
                If ButtonLabels.Length > 0 Then
                    MsgBoxEx.PatchMsgBox(ButtonLabels)
                End If
            End If
            ' Ggf. Default-Button verschieben
            If NewButtons <> Buttons Then
                If DefaultButton = MessageBoxDefaultButton.Button1 Then
                    DefaultButton = MessageBoxDefaultButton.Button2
                ElseIf DefaultButton = MessageBoxDefaultButton.Button2 Then
                    DefaultButton = MessageBoxDefaultButton.Button3
                End If
            End If
            ' MessageBox anzeigen
            Result = MessageBox.Show(Text, Caption, NewButtons, Icon, DefaultButton)
            ' Antwort zurück mappen und ggf. DefaultDialogResult setzen
            If NewButtons <> Buttons Then
                If Result = DialogResult.Yes Then
                    ' MessageBox soll nicht mehr angezeigt werden
                    SetDefaultDialogResult(MessageBoxQualifier, DefaultResult)
                    Result = DefaultResult
                ElseIf Result = DialogResult.No Then
                    If Buttons = MessageBoxButtons.OK OrElse Buttons = MessageBoxButtons.OKCancel Then
                        Result = DialogResult.OK
                    ElseIf Buttons = MessageBoxButtons.RetryCancel Then
                        Result = DialogResult.Retry
                    ElseIf Buttons = MessageBoxButtons.YesNo Then
                        Result = DialogResult.Yes
                    End If
                ElseIf Result = DialogResult.Cancel Then
                    If Buttons = MessageBoxButtons.YesNo Then
                        Result = DialogResult.No
                    End If
                End If
            End If

        End If

        Return Result

    End Function

#End Region

#Region "Win32 Imports"

    Private Declare Auto Function EnumThreadWindows Lib "user32.dll" (ByVal tid As Integer, ByVal callback As EnumWindowDelegate, ByVal lp As IntPtr) As Boolean
    Private Declare Auto Function EnumChildWindows Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal callback As EnumWindowDelegate, ByVal lp As IntPtr) As Boolean
    Private Declare Auto Function GetClassName Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal name As StringBuilder, ByVal maxlen As Integer) As Integer
    Private Declare Auto Function GetCurrentThreadId Lib "kernel32.dll" () As Integer
    Private Declare Auto Function SetWindowText Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal text As String) As Boolean
    Private Declare Auto Function GetWindowDC Lib "user32.dll" (ByVal hWnd As IntPtr) As Integer
    Private Declare Auto Function GetWindowRect Lib "user32.dll" (ByVal hWnd As IntPtr, ByRef rect As Rectangle) As Boolean
    Private Declare Auto Function MoveWindow Lib "user32.dll" (ByVal hWnd As IntPtr, _
                                                               ByVal X As Integer, _
                                                               ByVal Y As Integer, _
                                                               ByVal nWidth As Integer, _
                                                               ByVal nHeight As Integer, _
                                                               ByVal Repaint As Boolean) As Boolean
    Private Declare Auto Function ScreenToClient Lib "user32.dll" (ByVal hWnd As IntPtr, ByRef lpPoint As Point) As Boolean


    Private Const WS_VISIBLE As Integer = 268435456
    Private Const WS_CHILD As Integer = 1073741824
    Private Const WS_TABSTOP As Integer = 65536
    Private Const WM_SETFONT As Integer = 48
    Private Const WM_GETFONT As Integer = 49
    Private Const BS_AUTOCHECKBOX As Integer = 3
    Private Const BM_GETCHECK As Integer = 240
    Private Const BST_CHECKED As Integer = 1

    Protected Declare Sub DestroyWindow Lib "user32.dll" (ByVal hwnd As IntPtr)
    Protected Declare Function GetDlgItem Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal id As Integer) As IntPtr
    Protected Declare Function GetClientRect Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal rc As Rectangle) As Integer
    Protected Declare Function _MessageBox Lib "user32.dll" Alias "MessageBox" (ByVal hwnd As IntPtr, ByVal text As String, ByVal caption As String, ByVal options As Integer) As Integer
    Protected Declare Function SendMessage Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    Protected Declare Function CreateWindowEx Lib "user32.dll" (ByVal dwExStyle As Integer, ByVal lpClassName As String, ByVal lpWindowName As String, ByVal dwStyle As Integer, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hWndParent As IntPtr, ByVal hMenu As IntPtr, ByVal hInstance As IntPtr, ByVal lpParam As IntPtr) As IntPtr

#End Region


    Public Sub New()
        _HelpCtrl = New Control
    End Sub

End Class


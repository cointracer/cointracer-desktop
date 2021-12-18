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

Public Class frmGetImportData_CnP

    Public ReadOnly Property ContentOrders() As String
        Get
            Return tbContentOrders.Text
        End Get
    End Property
    Public ReadOnly Property ContentAccounts() As String
        Get
            Return tbContentAccounts.Text
        End Get
    End Property
    Public ReadOnly Property TimeDiff() As Integer
        Get
            Select Case Me.ImportPlatform
                Case Else
                    Return GetTimeDiffComboBoxValue(cbxTimeDiffVircurex)
            End Select
        End Get
    End Property


    Private _ImportPlatform As PlatformManager.Platforms
    Public Property ImportPlatform() As PlatformManager.Platforms
        Get
            Return _ImportPlatform
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _ImportPlatform = value
            SetControlsByPlatform()
        End Set
    End Property

    ''' <summary>
    ''' Zeigt die Controls in Abhängigkeit von der gesetzten Plattform an
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetControlsByPlatform()
        If Me.ImportPlatform = PlatformManager.Platforms.BtcE Then
            lblTextbox.Text = "History:"
            lblTextbox.Visible = True
            lblTextboxKraken.Visible = False
            pnlVircurex.Visible = False
            pnlVircurex2.Visible = False
            lblVircurex2.Visible = False
            btInsertAccounts.Visible = False
            tbContentAccounts.Visible = False
            pnlBtcE.Visible = True
            tbContentOrders.Height = 390
        Else
            ' Vircurex!
            lblTextbox.Text = "Orders:"
            lblTextbox.Visible = True
            lblTextboxKraken.Visible = False
            pnlVircurex.Visible = True
            pnlVircurex2.Visible = True
            lblVircurex2.Visible = True
            btInsertAccounts.Visible = True
            tbContentAccounts.Visible = True
            pnlBtcE.Visible = False
            tbContentOrders.Height = 151
            InitTimeDiffComboBox(cbxTimeDiffVircurex, "ImportSettingTimeDiffVircurex")
        End If
    End Sub


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _ImportPlatform = PlatformManager.Platforms.Unknown

    End Sub

    Private Sub btInsert_Click(sender As Object, e As EventArgs) Handles btInsertOrders.Click, btInsertAccounts.Click
        If My.Computer.Clipboard.ContainsText Then
            Dim TB As TextBox
            Select Case DirectCast(sender, Button).Name
                Case "btInsertAccounts"
                    TB = tbContentAccounts
                Case Else
                    TB = tbContentOrders
            End Select
            If TB.Text.Length > 0 Then
                ' Es ist schon Text in der Box
                MsgBoxEx.PatchMsgBox(New String() {"Ersetzen", "Anhängen"})
                If MessageBox.Show("Die Textbox ist nicht leer. Soll der Inhalt der Zwischenablage den bisherigen Text ersetzen oder " &
                       "soll er an den bisherigen Inhalt angefügt werden?", "Text ersetzen oder anfügen",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                    ' Ersetzen
                    TB.Text = My.Computer.Clipboard.GetText.Trim
                Else
                    ' Anhängen
                    TB.Text = TB.Text.Trim & System.Environment.NewLine & My.Computer.Clipboard.GetText.Trim
                End If
            Else
                TB.Text = My.Computer.Clipboard.GetText
            End If
        End If
    End Sub

    Private Sub frmGetImportData_CnP_Load(sender As Object, e As EventArgs) Handles Me.Load
        SetControlsByPlatform()
    End Sub

    ''' <summary>
    ''' Initialisiert das Control für die Auswahl von Zeitunterschieden
    ''' </summary>
    ''' <param name="ComboBoxControl"></param>
    ''' <param name="SettingsName"></param>
    Private Sub InitTimeDiffComboBox(ByRef ComboBoxControl As ComboBox, SettingsName As String)
        Try
            With ComboBoxControl
                .Items.Clear()
                For i As Integer = +23 To -23 Step -1
                    .Items.Add(String.Format("{0}{1} h", IIf(i < 0, "", "+"), i.ToString("00")))
                Next
                .SelectedIndex = ((.Items.Count - 1) / 2) - My.Settings(SettingsName)
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Gibt den Zeitdifferenz-Wert eines Control für die Auswahl von Zeitunterschieden zurück und speichert diesen ggf. in My.Settings
    ''' </summary>
    ''' <param name="ComboBoxControl">ComboBox-Control für die Zeitunterschied-Angabe</param>
    ''' <param name="SettingsName">Name der Settings-Eigenschaft. Wenn leer, wird nichts geschrieben.</param>
    Private Function GetTimeDiffComboBoxValue(ByRef ComboBoxControl As ComboBox,
                                              Optional SettingsName As String = "") As Long
        Try
            With ComboBoxControl
                Dim Value As Integer = CInt((.Items.Count - 1) / 2 - .SelectedIndex)
                If SettingsName.Length > 0 Then
                    My.Settings(SettingsName) = Value
                End If
                Return Value
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex)
            Return -1
        End Try
    End Function


    Private Sub frmGetImportData_CnP_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible = False And Me.DialogResult = Windows.Forms.DialogResult.OK Then
            If (Me.ImportPlatform = PlatformManager.Platforms.Vircurex Or Me.ImportPlatform = PlatformManager.Platforms.Unknown) Then
                GetTimeDiffComboBoxValue(cbxTimeDiffVircurex, "ImportSettingTimeDiffVircurex")
            End If
        End If
    End Sub

End Class

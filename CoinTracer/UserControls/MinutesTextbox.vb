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

Imports System.ComponentModel

Public Class MinutesTextbox
    Inherits Textbox
    
    Private _ExplanationLabel As Label
    ''' <summary>
    ''' Text that will be displayed with regards to padding settings of the current control
    ''' </summary>
    <Browsable(True)>
    <EditorBrowsable(True)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <DefaultValue("")>
    <Description("Label, in dem die Minutenangabe in Stunden und Minuten angezeigt wird.")>
    Public Property ExplanationLabel() As Label
        Get
            Return _ExplanationLabel
        End Get
        Set(ByVal value As Label)
            _ExplanationLabel = value
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        If _ExplanationLabel IsNot Nothing Then
            If IsNumeric(Me.Text) Then
                Dim Minutes As Integer = Math.Abs(CInt(Me.Text))
                If Minutes >= 60 Then
                    _ExplanationLabel.Text = String.Format("(= {0} {1}",
                                                     Minutes \ 60,
                                                     IIf(Minutes \ 60 = 1, My.Resources.MyStrings.globalHour, My.Resources.MyStrings.globalHours))
                    If Minutes Mod 60 > 0 Then
                        _ExplanationLabel.Text &= String.Format(", {0} {1}",
                                                     Minutes Mod 60,
                                                     IIf(Minutes Mod 60 = 1, My.Resources.MyStrings.globalMinute, My.Resources.MyStrings.globalMinutes))
                    End If
                    _ExplanationLabel.Text &= ")"
                Else
                    _ExplanationLabel.Text = ""
                End If
            Else
                _ExplanationLabel.Text = ""
            End If
        End If
    End Sub

End Class

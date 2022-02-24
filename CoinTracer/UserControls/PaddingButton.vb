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

Public Class PaddingButton
    Inherits Button

    Private _PaddingText As String
    ''' <summary>
    ''' Text that will be displayed with regards to padding settings of the current control
    ''' </summary>
    <Browsable(True)> _
    <EditorBrowsable(True)> _
    <Category("Appearance")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    <DefaultValue("")> _
    <Description("Text des Buttons, der unter Beachtung der eingestellten Padding-Grenzen dargestellt wird.")> _
    Public Property PaddingText() As String
        Get
            Return _PaddingText
        End Get
        Set(ByVal value As String)
            _PaddingText = value
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        Dim StringFormat As StringFormat = New StringFormat()
        StringFormat.Alignment = StringAlignment.Center
        StringFormat.LineAlignment = StringFormat.Alignment
        Dim PaddedRectangle As Rectangle = Me.ClientRectangle
        With PaddedRectangle
            .Y += Me.Padding.Top
            .X += Me.Padding.Left
            .Width -= Me.Padding.Left + Me.Padding.Right
            .Height -= Me.Padding.Top + Me.Padding.Bottom
        End With
        e.Graphics.DrawString(_PaddingText, Me.Font, New SolidBrush(ForeColor), PaddedRectangle, StringFormat)
    End Sub

End Class

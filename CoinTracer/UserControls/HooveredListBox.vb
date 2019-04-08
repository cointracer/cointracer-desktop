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

Imports System.ComponentModel

Public Class HooveredListBox
    Inherits ListBox

    Private _ActiveLine As Integer
    Private _HooverOnMouseOver As Boolean

    ' These are kept here for performance reasons
    Private _HooveredBackColorBrush As SolidBrush
    Private _HooveredBorderPen As Pen
    Private _BackColorBrush As SolidBrush
    Private _ForeColorBrush As SolidBrush

    Public Sub New()

        _HooveredBackColorBrush = New SolidBrush(SystemColors.MenuHighlight)
        _HooveredBorderPen = New Pen(SystemColors.HotTrack)

        _HooverOnMouseOver = True
        _ActiveLine = -1

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _BackColorBrush = New SolidBrush(BackColor)
        _ForeColorBrush = New SolidBrush(ForeColor)

    End Sub

    <Browsable(True)>
    <EditorBrowsable(True)>
    <Category("Behavior")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <DefaultValue(True)>
    <Description("Bestimmt, ob die enthaltenen ListItems bei MouseOver unterlegt werden sollen.")>
    Public Property HooverOnMouseOver() As Boolean
        Get
            Return _HooverOnMouseOver
        End Get
        Set(ByVal value As Boolean)
            _HooverOnMouseOver = value
        End Set
    End Property

    Private _HooveredBackColor As Color
    <Browsable(True)>
    <EditorBrowsable(True)>
    <Category("Appearance")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Description("Hintergrundfarbe für ListItems bei MouseOver")>
    Public Property HooveredBackBolor() As Color
        Get
            Return _HooveredBackColor
        End Get
        Set(ByVal value As Color)
            _HooveredBackColor = value
            _HooveredBackColorBrush.Dispose()
            _HooveredBackColorBrush = New SolidBrush(_HooveredBackColor)
        End Set
    End Property
    Public Sub ResetHooveredBackColor()
        HooveredBorderColor = SystemColors.MenuHighlight
    End Sub
    Private Function ShouldSerializeHooveredBackColor() As Boolean
        Return _HooveredBorderColor <> SystemColors.MenuHighlight
    End Function

    Private _HooveredBorderColor As Color
    <Browsable(True)>
    <EditorBrowsable(True)>
    <Category("Appearance")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Description("Rahmenfarbe für ListItems bei MouseOver")>
    Public Property HooveredBorderColor() As Color
        Get
            Return _HooveredBorderColor
        End Get
        Set(ByVal value As Color)
            _HooveredBorderColor = value
            _HooveredBorderPen.Dispose()
            _HooveredBorderPen = New Pen(_HooveredBorderColor)
        End Set
    End Property
    Public Sub ResetHooveredBorderColor()
        HooveredBorderColor = SystemColors.HotTrack
    End Sub
    Private Function ShouldSerializeHooveredBorderColor() As Boolean
        Return _HooveredBorderColor <> SystemColors.HotTrack
    End Function

    Private Sub HooveredListBox_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Dim ActiveLine As Integer = Me.IndexFromPoint(New Point(e.X, e.Y))
        If _HooverOnMouseOver AndAlso ActiveLine <> _ActiveLine Then
            ' Restore the formerly hoovered item
            DrawSingleItem(_BackColorBrush)
            ' Hoover the new item
            _ActiveLine = ActiveLine
            DrawSingleItem(_HooveredBackColorBrush, True)
        End If
    End Sub

    ''' <summary>
    ''' Draws the item taken from _ActiveLine with the given BackColorBrush (if not selected)
    ''' </summary>
    Private Sub DrawSingleItem(ByRef BackColorBrush As SolidBrush, Optional ByVal DrawRectangle As Boolean = False)
        Dim ItemRect As Rectangle
        Dim ItemGraphics As Graphics = Me.CreateGraphics
        If _ActiveLine >= 0 Then
            ItemRect = GetItemRectangle(_ActiveLine)
            If _ActiveLine <> SelectedIndex Then
                ItemGraphics.FillRectangle(BackColorBrush, ItemRect)
                If DrawRectangle Then
                    ItemGraphics.DrawRectangle(_HooveredBorderPen, ItemRect)
                Else
                    ItemGraphics.DrawRectangle(New Pen(BackColorBrush), ItemRect)
                End If
                ItemGraphics.DrawString(Items(_ActiveLine).ToString, Font, _ForeColorBrush, ItemRect)
            Else
                ItemGraphics.DrawRectangle(New Pen(_BackColorBrush), ItemRect)
            End If
        End If
        ItemGraphics.Dispose()
    End Sub

    Private Sub HooveredListBox_BackColorChanged(sender As Object, e As EventArgs) Handles Me.BackColorChanged
        _BackColorBrush.Dispose()
        _BackColorBrush = New SolidBrush(BackColor)
    End Sub

    Private Sub HooveredListBox_ForeColorChanged(sender As Object, e As EventArgs) Handles Me.ForeColorChanged
        _ForeColorBrush.Dispose()
        _ForeColorBrush = New SolidBrush(ForeColor)
    End Sub

    Private Sub HooveredListBox_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
        ' Restore the formerly hoovered item
        DrawSingleItem(_BackColorBrush)
        _ActiveLine = -1
    End Sub

End Class

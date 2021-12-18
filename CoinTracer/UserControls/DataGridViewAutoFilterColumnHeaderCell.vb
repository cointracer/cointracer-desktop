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
Imports System.Reflection

''' <summary>
''' Provides a drop-down filter list in a DataGridViewColumnHeaderCell.
''' </summary>
Public Class DataGridViewAutoFilterColumnHeaderCell
    Inherits DataGridViewColumnHeaderCell


    ''' <summary>
    ''' The ListBox used for all drop-down lists. 
    ''' </summary>
    Private Shared dropDownListBox As New FilterListBox()

    ''' <summary>
    ''' A list of filters available for the owning column stored as 
    ''' formatted and unformatted string values. 
    ''' </summary>
    Private filters As New System.Collections.Specialized.OrderedDictionary()

    ''' <summary>
    ''' The drop-down list filter value currently in effect for 
    ''' the owning column. 
    ''' </summary>
    Private selectedFilterValue As String = String.Empty

    ''' <summary>
    ''' The complete filter string currently in effect for the owning column. 
    ''' </summary>
    Private currentColumnFilter As String = String.Empty

    ''' <summary>
    ''' All filtered Strings of the current filter
    ''' </summary>
    Private currentColumnFilterItems() As String = {}

    ''' <summary>
    ''' Indicates whether the DataGridView is currently filtered by the 
    ''' owning column.  
    ''' </summary>
    Private filtered As Boolean

    ''' <summary>
    ''' Maximum number of filtered items shown in pull down list
    ''' </summary>
    Private Const MAXFILTERITEMS As Long = &H2000

    ''' <summary>
    ''' Initializes a new instance of the DataGridViewColumnHeaderCell 
    ''' class and sets its property values to the property values of the 
    ''' specified DataGridViewColumnHeaderCell.
    ''' </summary>
    ''' <param name="oldHeaderCell">The DataGridViewColumnHeaderCell to 
    ''' copy property values from.</param>
    Public Sub New(ByVal oldHeaderCell As DataGridViewColumnHeaderCell)

        ContextMenuStrip = oldHeaderCell.ContextMenuStrip
        ErrorText = oldHeaderCell.ErrorText
        Tag = oldHeaderCell.Tag
        ToolTipText = oldHeaderCell.ToolTipText
        Value = oldHeaderCell.Value
        ValueType = oldHeaderCell.ValueType

        ' Use HasStyle to avoid creating a new style object
        ' when the Style property has not previously been set. 
        If oldHeaderCell.HasStyle Then
            Style = oldHeaderCell.Style
        End If

        ' Copy this type's properties if the old cell is an auto-filter cell. 
        ' This enables the Clone method to reuse this constructor. 
        Dim filterCell As DataGridViewAutoFilterColumnHeaderCell =
            TryCast(oldHeaderCell, DataGridViewAutoFilterColumnHeaderCell)
        If filterCell IsNot Nothing Then
            FilteringEnabled = filterCell.FilteringEnabled
            AutomaticSortingEnabled = filterCell.AutomaticSortingEnabled
            DropDownListBoxMaxLines = filterCell.DropDownListBoxMaxLines
            currentDropDownButtonPaddingOffset =
                filterCell.currentDropDownButtonPaddingOffset
        End If

    End Sub

    ''' <summary>
    ''' Initializes a new instance of the DataGridViewColumnHeaderCell 
    ''' class. 
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Creates an exact copy of this cell.
    ''' </summary>
    ''' <returns>An object that represents the cloned 
    ''' DataGridViewAutoFilterColumnHeaderCell.</returns>
    Public Overrides Function Clone() As Object
        Return New DataGridViewAutoFilterColumnHeaderCell(Me)
    End Function

    ''' <summary>
    ''' Called when the value of the DataGridView property changes
    ''' in order to perform initialization that requires access to the 
    ''' owning control and column. 
    ''' </summary>
    Protected Overrides Sub OnDataGridViewChanged()

        ' Continue only if there is a DataGridView. 
        If DataGridView Is Nothing Then
            Return
        End If

        ' Disable sorting and filtering for columns that can't make
        ' effective use of them. 
        If OwningColumn IsNot Nothing Then

            If TypeOf OwningColumn Is DataGridViewImageColumn OrElse
                (TypeOf OwningColumn Is DataGridViewButtonColumn AndAlso
                CType(OwningColumn, DataGridViewButtonColumn).UseColumnTextForButtonValue) _
                OrElse (TypeOf OwningColumn Is DataGridViewLinkColumn AndAlso
                CType(OwningColumn, DataGridViewLinkColumn).UseColumnTextForLinkValue) Then

                AutomaticSortingEnabled = False
                FilteringEnabled = False

            End If

            ' Ensure that the column SortMode property value is not Automatic.
            ' This prevents sorting when the user clicks the drop-down button.
            If OwningColumn.SortMode = DataGridViewColumnSortMode.Automatic Then
                OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic
            End If

        End If

        ' Confirm that the data source meets requirements. 
        VerifyDataSource()

        ' Add handlers to DataGridView events. 
        HandleDataGridViewEvents()

        ' Initialize the drop-down button bounds so that any initial
        ' column autosizing will accommodate the button width. 
        SetDropDownButtonBounds()

        ' Call the OnDataGridViewChanged method on the base class to 
        ' raise the DataGridViewChanged event.
        MyBase.OnDataGridViewChanged()

    End Sub 'OnDataGridViewChanged

    ''' <summary>
    ''' Confirms that the data source, if it has been set, is a BindingSource.
    ''' </summary>
    Private Sub VerifyDataSource()

        ' Continue only if there is a DataGridView and 
        ' its DataSource has been set.
        If DataGridView Is Nothing OrElse
            DataGridView.DataSource Is Nothing Then
            Return
        End If

        ' Throw an exception if the data source is not a BindingSource. 
        Dim data As BindingSource =
            TryCast(DataGridView.DataSource, BindingSource)
        If data Is Nothing Then
            Throw New NotSupportedException(
                "The DataSource property of the containing DataGridView " &
                "control must be set to a BindingSource.")
        End If

    End Sub 'VerifyDataSource

#Region "DataGridView events: HandleDataGridViewEvents, DataGridView event handlers, ResetDropDown, ResetFilter"

    ''' <summary>
    ''' Add handlers to various DataGridView events, primarily to invalidate 
    ''' the drop-down button bounds, hide the drop-down list, and reset 
    ''' cached filter values when changes in the DataGridView require it.
    ''' </summary>
    Private Sub HandleDataGridViewEvents()
        AddHandler DataGridView.Scroll, AddressOf DataGridView_Scroll
        AddHandler DataGridView.ColumnDisplayIndexChanged, AddressOf DataGridView_ColumnDisplayIndexChanged
        AddHandler DataGridView.ColumnWidthChanged, AddressOf DataGridView_ColumnWidthChanged
        AddHandler DataGridView.ColumnHeadersHeightChanged, AddressOf DataGridView_ColumnHeadersHeightChanged
        AddHandler DataGridView.SizeChanged, AddressOf DataGridView_SizeChanged
        AddHandler DataGridView.DataSourceChanged, AddressOf DataGridView_DataSourceChanged
        AddHandler DataGridView.DataBindingComplete, AddressOf DataGridView_DataBindingComplete

        ' Add a handler for the ColumnSortModeChanged event to prevent the
        ' column SortMode property from being inadvertently set to Automatic.
        AddHandler DataGridView.ColumnSortModeChanged, AddressOf DataGridView_ColumnSortModeChanged
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds when 
    ''' the user scrolls horizontally.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A ScrollEventArgs that contains the event data.</param>
    Private Sub DataGridView_Scroll(
        ByVal sender As Object, ByVal e As ScrollEventArgs)
        If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
            ResetDropDown()
        End If
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds when 
    ''' the column display index changes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DataGridView_ColumnDisplayIndexChanged(
        ByVal sender As Object, ByVal e As DataGridViewColumnEventArgs)
        ResetDropDown()
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds when a column width changes
    ''' in the DataGridView control. A width change in any column of the 
    ''' control has the potential to affect the drop-down button location, 
    ''' depending on the current horizontal scrolling position and whether
    ''' the changed column is to the left or right of the current column. 
    ''' It is easier to invalidate the button in all cases. 
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A DataGridViewColumnEventArgs that contains the event data.</param>
    Private Sub DataGridView_ColumnWidthChanged(
        ByVal sender As Object, ByVal e As DataGridViewColumnEventArgs)
        ResetDropDown()
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds when the height of the column headers changes.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">An EventArgs that contains the event data.</param>
    Private Sub DataGridView_ColumnHeadersHeightChanged(
        ByVal sender As Object, ByVal e As EventArgs)
        ResetDropDown()
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds when the size of the DataGridView changes.
    ''' This prevents a painting issue that occurs when the right edge of the control moves 
    ''' to the right and the control contents have previously been scrolled to the right.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">An EventArgs that contains the event data.</param>
    Private Sub DataGridView_SizeChanged(
        ByVal sender As Object, ByVal e As EventArgs)
        ResetDropDown()
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds, hides the drop-down 
    ''' filter list, if it is showing, and resets the cached filter values
    ''' if the filter has been removed. 
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A DataGridViewBindingCompleteEventArgs that contains the event data.</param>
    Private Sub DataGridView_DataBindingComplete(
        ByVal sender As Object, ByVal e As DataGridViewBindingCompleteEventArgs)
        If e.ListChangedType = ListChangedType.Reset Then
            ResetDropDown()
            ResetFilter()
        End If
    End Sub

    ''' <summary>
    ''' Verifies that the data source meets requirements, invalidates the 
    ''' drop-down button bounds, hides the drop-down filter list if it is 
    ''' showing, and resets the cached filter values if the filter has been removed. 
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">An EventArgs that contains the event data.</param>
    Private Sub DataGridView_DataSourceChanged(
        ByVal sender As Object, ByVal e As EventArgs)
        VerifyDataSource()
        ResetDropDown()
        ResetFilter()
    End Sub

    ''' <summary>
    ''' Invalidates the drop-down button bounds and hides the filter
    ''' list if it is showing.
    ''' </summary>
    Private Sub ResetDropDown()
        InvalidateDropDownButtonBounds()
        If dropDownListBoxShowing Then
            HideDropDownList()
        End If
    End Sub

    ''' <summary>
    ''' Resets the cached filter values if the filter has been removed.
    ''' </summary>
    Private Sub ResetFilter()
        If DataGridView Is Nothing Then Return
        Dim source As BindingSource =
            TryCast(DataGridView.DataSource, BindingSource)
        If source Is Nothing OrElse String.IsNullOrEmpty(source.Filter) Then
            filtered = False
            selectedFilterValue = _CaptionAllValues
            currentColumnFilter = String.Empty
            currentColumnFilterItems = {}
        End If
    End Sub

    ''' <summary>
    ''' Throws an exception when the column sort mode is changed to Automatic.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A DataGridViewColumnEventArgs that contains the event data.</param>
    Private Sub DataGridView_ColumnSortModeChanged(
        ByVal sender As Object, ByVal e As DataGridViewColumnEventArgs)

        If e.Column Is OwningColumn AndAlso
            e.Column.SortMode = DataGridViewColumnSortMode.Automatic Then
            Throw New InvalidOperationException(
                "A SortMode value of Automatic is incompatible with " &
                "the DataGridViewAutoFilterColumnHeaderCell type. " &
                "Use the AutomaticSortingEnabled property instead.")
        End If

    End Sub

#End Region 'DataGridView events

    ''' <summary>
    ''' Paints the column header cell, including the drop-down button. 
    ''' </summary>
    ''' <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
    ''' <param name="clipBounds">A Rectangle that represents the area of the DataGridView that needs to be repainted.</param>
    ''' <param name="cellBounds">A Rectangle that contains the bounds of the DataGridViewCell that is being painted.</param>
    ''' <param name="rowIndex">The row index of the cell that is being painted.</param>
    ''' <param name="cellState">A bitwise combination of DataGridViewElementStates values that specifies the state of the cell.</param>
    ''' <param name="value">The data of the DataGridViewCell that is being painted.</param>
    ''' <param name="formattedValue">The formatted data of the DataGridViewCell that is being painted.</param>
    ''' <param name="errorText">An error message that is associated with the cell.</param>
    ''' <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
    ''' <param name="advancedBorderStyle">A DataGridViewAdvancedBorderStyle that contains border styles for the cell that is being painted.</param>
    ''' <param name="paintParts">A bitwise combination of the DataGridViewPaintParts values that specifies which parts of the cell need to be painted.</param>
    Protected Overrides Sub Paint(
        ByVal graphics As Graphics,
        ByVal clipBounds As Rectangle,
        ByVal cellBounds As Rectangle,
        ByVal rowIndex As Integer,
        ByVal cellState As DataGridViewElementStates,
        ByVal value As Object,
        ByVal formattedValue As Object,
        ByVal errorText As String,
        ByVal cellStyle As DataGridViewCellStyle,
        ByVal advancedBorderStyle As DataGridViewAdvancedBorderStyle,
        ByVal paintParts As DataGridViewPaintParts)

        ' Use the base method to paint the default appearance. 
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex,
            cellState, value, formattedValue, errorText, cellStyle,
            advancedBorderStyle, paintParts)

        ' Continue only if filtering is enabled and ContentBackground is 
        ' part of the paint request. 
        If Not FilteringEnabled OrElse (paintParts And
            DataGridViewPaintParts.ContentBackground) = 0 Then
            Return
        End If

        ' Retrieve the current button bounds. 
        Dim buttonBounds As Rectangle = DropDownButtonBounds

        ' Continue only if the buttonBounds is big enough to draw.
        If buttonBounds.Width < 1 OrElse buttonBounds.Height < 1 Then Return

        ' Paint the button manually, using the correct state depending on whether the 
        ' filter list is showing and whether there is a filter in effect 
        ' for the current column. 

        ' Determine the pressed state in order to paint the button 
        ' correctly and to offset the down arrow.
        Dim Brsh As Brush
        Dim Pn As Pen
        If dropDownListBoxShowing Then
            graphics.FillRectangle(SystemBrushes.Highlight, buttonBounds)
            Brsh = SystemBrushes.HighlightText
            Pn = SystemPens.HighlightText
        Else
            Brsh = SystemBrushes.Highlight
            Pn = SystemPens.Highlight
        End If
        graphics.DrawRectangle(New Pen(Color.DarkGray, 1), buttonBounds)

        ' Determine if the column is sorted in order to draw the arrow 
        ' heading in the right direction
        Dim SortDir As SortOrder = SortOrder.None
        If DataGridView.SortedColumn Is OwningColumn Then
            SortDir = DataGridView.SortOrder
        End If

        ' If there is a filter in effect for the column, paint the 
        ' filter symbol and a small triangle. If there is no filter 
        ' in effect, paint the arrow and a small triangle.
        If filtered Then
            ' upper right hand triangle
            graphics.FillPolygon(Brsh, New Point() {
                New Point(
                    buttonBounds.Width \ 2 + buttonBounds.Left + 2,
                    buttonBounds.Height * 3 \ 4 + buttonBounds.Top - 3),
                New Point(
                    buttonBounds.Width \ 4 + buttonBounds.Left + 2,
                    buttonBounds.Height \ 4 + buttonBounds.Top),
                New Point(
                    buttonBounds.Width * 3 \ 4 + buttonBounds.Left + 3,
                    buttonBounds.Height \ 4 + buttonBounds.Top)
            })
            ' rectangle underneath the upper triangle
            graphics.FillRectangle(Brsh, New Rectangle(
                                    buttonBounds.Width \ 2 + buttonBounds.Left + 1,
                                    buttonBounds.Height \ 4 + buttonBounds.Top + 1,
                                    3, buttonBounds.Height * 2 / 4))
            ' sort arrow or small triangle
            If SortDir = SortOrder.Ascending Then
                graphics.DrawLine(Pn,
                                      buttonBounds.Left + 4, buttonBounds.Top + 5,
                                      buttonBounds.Left + 4, buttonBounds.Bottom - 4)
                graphics.DrawLine(Pn,
                                  buttonBounds.Left + 3, buttonBounds.Top + 6,
                                  buttonBounds.Left + 5, buttonBounds.Top + 6)
            ElseIf SortDir = SortOrder.Descending Then
                graphics.DrawLine(Pn,
                                      buttonBounds.Left + 4, buttonBounds.Top + 5,
                                      buttonBounds.Left + 4, buttonBounds.Bottom - 4)
                graphics.DrawLine(Pn,
                                  buttonBounds.Left + 3, buttonBounds.Bottom - 5,
                                  buttonBounds.Left + 5, buttonBounds.Bottom - 5)
            Else
                ' lower left triangle
                graphics.FillPolygon(Brsh, New Point() {
                    New Point(
                        buttonBounds.Left + buttonBounds.Width \ 7,
                        buttonBounds.Bottom - 6),
                    New Point(
                        buttonBounds.Left + buttonBounds.Width \ 7 + 5,
                        buttonBounds.Bottom - 6),
                    New Point(
                        buttonBounds.Left + buttonBounds.Width \ 7 + 2,
                        buttonBounds.Bottom - 6 + 3)
                })
            End If
        Else

            ' Column is not filtered but perhaps sorted
            If SortDir <> SortOrder.None Then
                ' Triangle in lower left corner
                graphics.FillPolygon(SystemBrushes.ControlText, New Point() {
                    New Point(
                        buttonBounds.Left + 3,
                        buttonBounds.Bottom - 6),
                    New Point(
                        buttonBounds.Left + 3 + 5,
                        buttonBounds.Bottom - 6),
                    New Point(
                        buttonBounds.Left + 3 + 2,
                        buttonBounds.Bottom - 6 + 3)
                })
                If SortDir = SortOrder.Ascending Then
                    graphics.DrawLine(SystemPens.ControlText,
                                      buttonBounds.Right - 5, buttonBounds.Top + 4,
                                      buttonBounds.Right - 5, buttonBounds.Bottom - 4)
                    graphics.DrawLine(SystemPens.ControlText,
                                      buttonBounds.Right - 6, buttonBounds.Top + 5,
                                      buttonBounds.Right - 4, buttonBounds.Top + 5)

                Else
                    graphics.DrawLine(SystemPens.ControlText,
                                      buttonBounds.Right - 5, buttonBounds.Top + 4,
                                      buttonBounds.Right - 5, buttonBounds.Bottom - 4)
                    graphics.DrawLine(SystemPens.ControlText,
                                      buttonBounds.Right - 6, buttonBounds.Bottom - 5,
                                      buttonBounds.Right - 4, buttonBounds.Bottom - 5)
                End If
            Else
                ' default big triangle
                graphics.FillPolygon(SystemBrushes.ControlText, New Point() {
                    New Point(
                        buttonBounds.Width \ 2 +
                            buttonBounds.Left,
                        buttonBounds.Height * 3 \ 4 +
                            buttonBounds.Top),
                    New Point(
                        buttonBounds.Width \ 4 +
                            buttonBounds.Left + 1,
                        buttonBounds.Height \ 2 +
                            buttonBounds.Top),
                    New Point(
                        buttonBounds.Width * 3 \ 4 +
                            buttonBounds.Left,
                        buttonBounds.Height \ 2 +
                            buttonBounds.Top)
                })
            End If
        End If

    End Sub 'Paint

    ''' <summary>
    ''' Handles mouse clicks to the header cell, displaying the 
    ''' drop-down list or sorting the owning column as appropriate. 
    ''' </summary>
    ''' <param name="e">A DataGridViewCellMouseEventArgs that contains the event data.</param>
    Protected Overrides Sub OnMouseDown(ByVal e As DataGridViewCellMouseEventArgs)

        Debug.Assert(DataGridView IsNot Nothing, "DataGridView is null")

        ' Continue only if the user did not click the drop-down button 
        ' while the drop-down list was displayed. This prevents the 
        ' drop-down list from being redisplayed after being hidden in 
        ' the LostFocus event handler. 
        If lostFocusOnDropDownButtonClick Then
            lostFocusOnDropDownButtonClick = False
            Return
        End If

        ' Continue only if there are any lines to display
        If DataGridView.DataSource Is Nothing Then
            Return
        End If

        ' Retrieve the current size and location of the header cell,
        ' excluding any portion that is scrolled off screen. 
        Dim cellBounds As Rectangle = DataGridView _
            .GetCellDisplayRectangle(e.ColumnIndex, -1, False)

        ' Continue only if the column is not manually resizable or the
        ' mouse coordinates are not within the column resize zone. 
        If Me.OwningColumn.Resizable = DataGridViewTriState.True AndAlso
            (Me.DataGridView.RightToLeft = RightToLeft.No AndAlso
            cellBounds.Width - e.X < 6 OrElse e.X < 6) Then
            Return
        End If

        ' Unless RightToLeft is enabled, store the width of the portion
        ' that is scrolled off screen. 
        Dim scrollingOffset As Integer = 0
        If Me.DataGridView.RightToLeft = RightToLeft.No AndAlso
            DataGridView.FirstDisplayedScrollingColumnIndex = ColumnIndex Then
            scrollingOffset = DataGridView.FirstDisplayedScrollingColumnHiddenWidth
        End If

        ' Show the drop-down list if filtering is enabled and the mouse click occurred
        ' within the drop-down button bounds. Otherwise, if sorting is enabled and the
        ' click occurred outside the drop-down button bounds, sort by the owning column. 
        ' The mouse coordinates are relative to the cell bounds, so the cell location
        ' and the scrolling offset are needed to determine the client coordinates.
        If e.Button = MouseButtons.Left Then
            If FilteringEnabled AndAlso
                DropDownButtonBounds.Contains(
                e.X + cellBounds.Left - scrollingOffset, e.Y + cellBounds.Top) Then

                ' If the current cell is in edit mode, commit the edit. 
                If DataGridView.IsCurrentCellInEditMode Then
                    ' Commit and end the cell edit.  
                    DataGridView.EndEdit()

                    ' Commit any change to the underlying data source. 
                    Dim source As BindingSource =
                        TryCast(DataGridView.DataSource, BindingSource)
                    If source IsNot Nothing Then
                        source.EndEdit()
                    End If
                End If
                ShowDropDownList()

            ElseIf AutomaticSortingEnabled AndAlso
                DataGridView.SelectionMode <>
                DataGridViewSelectionMode.ColumnHeaderSelect Then

                SortByColumn()

            End If
        End If

        MyBase.OnMouseDown(e)

    End Sub 'OnMouseDown

    ''' <summary>
    ''' Sorts the DataGridView by the current column if AutomaticSortingEnabled is true.
    ''' </summary>
    Private Sub SortByColumn()

        Debug.Assert(DataGridView IsNot Nothing AndAlso
            OwningColumn IsNot Nothing, "DataGridView or OwningColumn is null")

        ' Continue only if the data source supports sorting. 
        Dim sortList As IBindingList =
            TryCast(DataGridView.DataSource, IBindingList)
        If sortList Is Nothing OrElse
            Not sortList.SupportsSorting OrElse
            Not AutomaticSortingEnabled Then
            Return
        End If

        ' Determine the sort direction and sort by the owning column. 
        Dim direction As ListSortDirection = ListSortDirection.Ascending
        If DataGridView.SortedColumn Is OwningColumn AndAlso
            Me.DataGridView.SortOrder = SortOrder.Ascending Then
            direction = ListSortDirection.Descending
        End If
        DataGridView.Sort(OwningColumn, direction)

    End Sub

#Region "drop-down list: Show/HideDropDownListBox, SetDropDownListBoxBounds, DropDownListBoxMaxHeightInternal"

    ''' <summary>
    ''' Indicates whether dropDownListBox is currently displayed 
    ''' for this header cell. 
    ''' </summary>
    Private dropDownListBoxShowing As Boolean

    Private Shared ButtonPanel As New Panel
    Private Shared OKButton As New Button
    Private Shared CancelButton As New Button

    ''' <summary>
    ''' Displays the drop-down filter list. Also creates buttons for OK and Cancel if needed
    ''' </summary>
    Public Sub ShowDropDownList()

        Debug.Assert(DataGridView IsNot Nothing, "DataGridView is null")

        Dim CurrentCursor As Cursor = Cursor.Current
        Cursor.Current = Cursors.WaitCursor

        ' Ensure that the current row is not the row for new records.
        ' This prevents the new row from affecting the filter list and also 
        ' prevents the new row from being added when the filter changes.
        If DataGridView.CurrentRow IsNot Nothing AndAlso
            DataGridView.CurrentRow.IsNewRow Then
            DataGridView.CurrentCell = Nothing
        End If

        ' Populate the filters dictionary, then copy the filter values 
        ' from the filters.Keys collection into the ListBox.Items collection, 
        ' selecting the current filter if there is one in effect. 
        If PopulateFilters() Then
            dropDownListBox.ResetForeColor()
        Else
            dropDownListBox.ForeColor = Color.OrangeRed
        End If

        Dim filterArray As String() = New String(filters.Count - 1) {}
        filters.Keys.CopyTo(filterArray, 0)
        dropDownListBox.Items.Clear()
        ' avoid recursion
        _NoInteraction = True
        Try
            ' load all filter entries and set checked state
            Dim filterItem As String
            For i As Integer = 0 To filterArray.Length - 1
                dropDownListBox.Items.Add(filterArray(i))
                If filterArray(i) = _CaptionCheckedValues Then
                    filterItem = "True"
                ElseIf filterArray(i) = _CaptionUncheckedValues Then
                    filterItem = "False"
                Else
                    filterItem = filterArray(i)
                End If
                If DirectCast(currentColumnFilterItems, ICollection(Of String)).Contains(filterItem) Then
                    dropDownListBox.SetItemChecked(i, True)
                End If
            Next
        Catch ex As Exception
            Debug.Print("DataGridViewAutoFilterColumnHeaderCell_ShowDropDownList")
        Finally
            _NoInteraction = False
        End Try
        dropDownListBox.SelectedItem = selectedFilterValue

        ' Add handlers to dropDownListBox events. 
        HandleDropDownListBoxEvents()

        ' Set the size and location of the dropDownListBox
        SetDropDownListBoxBounds()
        ' Display it
        dropDownListBox.Visible = True
        dropDownListBoxShowing = True
        Debug.Assert(dropDownListBox.Parent Is Nothing,
            "ShowDropDownListBox has been called multiple times before HideDropDownListBox")
        ' Add dropDownListBox and the buttons to the DataGridView. 
        DataGridView.Controls.Add(dropDownListBox)
        DataGridView.Controls.Add(ButtonPanel)

        ' Create the OK and Cancel buttons
        Dim graphics As Graphics = dropDownListBox.CreateGraphics()
        With ButtonPanel
            .SetBounds(dropDownListBox.Location.X,
                       dropDownListBox.Bottom - 1,
                       dropDownListBox.Width + 1,
                       (graphics.MeasureString("OK", .Font).Height + 6) * 2)
        End With
        graphics.Dispose()
        With OKButton
            .Font = New Font(dropDownListBox.Font, FontStyle.Regular)
            .Text = CaptionOKButton
            .SetBounds(0, 0,
                       dropDownListBox.Width,
                       ButtonPanel.Height / 2)
        End With
        With CancelButton
            .Font = OKButton.Font
            .Text = CaptionCancelButton
            .SetBounds(0, OKButton.Height - 1,
                       OKButton.Width,
                       OKButton.Height)
        End With
        ButtonPanel.Controls.Add(OKButton)
        ButtonPanel.Controls.Add(CancelButton)

        ' Display the the Buttons
        ButtonPanel.Visible = True
        OKButton.Visible = True
        CancelButton.Visible = True

        ' Set the input focus to dropDownListBox. 
        dropDownListBox.Focus()

        ' Invalidate the cell so that the drop-down button will repaint
        ' in the pressed state. 
        DataGridView.InvalidateCell(Me)

        Cursor.Current = CurrentCursor

    End Sub

    ''' <summary>
    ''' Hides the drop-down filter list. 
    ''' </summary>
    Public Sub HideDropDownList()

        Debug.Assert(DataGridView IsNot Nothing, "DataGridView is null")

        ' Hide dropDownListBox, remove handlers from its events, and remove 
        ' it from the DataGridView control. 
        dropDownListBoxShowing = False
        dropDownListBox.Visible = False
        OKButton.Visible = False
        CancelButton.Visible = False
        UnhandleDropDownListBoxEvents()
        DataGridView.Controls.Remove(dropDownListBox)
        ButtonPanel.Controls.Remove(OKButton)
        ButtonPanel.Controls.Remove(CancelButton)
        DataGridView.Controls.Remove(ButtonPanel)
        _LastItemClicked = 0

        ' Invalidate the cell so that the drop-down button will repaint
        ' in the unpressed state. 
        DataGridView.InvalidateCell(Me)

    End Sub



    ''' <summary>
    ''' Sets the dropDownListBox size and position based on the formatted 
    ''' values in the filters dictionary and the position of the drop-down 
    ''' button. Called only by ShowDropDownListBox.  
    ''' </summary>
    Private Sub SetDropDownListBoxBounds()

        Debug.Assert(filters.Count > 0, "filters.Count <= 0")

        ' Declare variables that will be used in the calculation, 
        ' initializing dropDownListBoxHeight to account for the 
        ' ListBox borders.
        Dim dropDownListBoxHeight As Integer = 2
        Dim currentWidth As Integer = 0
        Dim dropDownListBoxWidth As Integer = 0
        Dim dropDownListBoxLeft As Integer = 0

        ' For each formatted value in the filters dictionary Keys collection,
        ' add its height to dropDownListBoxHeight and, if it is wider than 
        ' all previous values, set dropDownListBoxWidth to its width.
        Dim graphics As Graphics = dropDownListBox.CreateGraphics()
        Try
            Dim filter As String
            For Each filter In filters.Keys
                Dim stringSizeF As SizeF =
                    graphics.MeasureString(filter, dropDownListBox.Font)
                currentWidth = CType(stringSizeF.Width + 16, Integer)         ' 16 more for the checkbox...
                If dropDownListBoxWidth < currentWidth Then
                    dropDownListBoxWidth = currentWidth
                End If
            Next filter
        Finally
            graphics.Dispose()
        End Try
        dropDownListBoxHeight = (dropDownListBox.GetItemHeight(0) + 5) * filters.Keys.Count + 2

        ' Increase the width to allow for horizontal margins and borders. 
        dropDownListBoxWidth += 6

        If DataGridViewAsObject.GetType().GetProperty("CurrentScaleFactor") IsNot Nothing Then
            dropDownListBoxHeight *= DataGridViewAsObject.CurrentScaleFactor.Width
            dropDownListBoxWidth *= (1 + ((DataGridViewAsObject.CurrentScaleFactor.Width - 1) / 2))
        End If

        ' Constrain the dropDownListBox height to the 
        ' DropDownListBoxMaxHeightInternal value, which is based on 
        ' the DropDownListBoxMaxLines property value but constrained by
        ' the maximum height available in the DataGridView control.
        If dropDownListBoxHeight > DropDownListBoxMaxHeightInternal Then
            dropDownListBoxHeight = DropDownListBoxMaxHeightInternal
            ' If the preferred height is greater than the available height,
            ' adjust the width to accommodate the vertical scroll bar. 
            dropDownListBoxWidth += SystemInformation.VerticalScrollBarWidth
        End If

        ' Calculate the ideal location of the left edge of dropDownListBox 
        ' based on the location of the drop-down button and taking the 
        ' RightToLeft property value into consideration. 
        If Me.DataGridView.RightToLeft = RightToLeft.No Then
            dropDownListBoxLeft = DropDownButtonBounds.Right - dropDownListBoxWidth + 1
        Else
            dropDownListBoxLeft = DropDownButtonBounds.Left - 1
        End If

        ' Determine the left and right edges of the available horizontal
        ' width of the DataGridView control. 
        Dim clientLeft As Integer = 1
        Dim clientRight As Integer = DataGridView.ClientRectangle.Right
        If DataGridView.DisplayedRowCount(False) < DataGridView.RowCount Then
            If Me.DataGridView.RightToLeft = RightToLeft.Yes Then
                clientLeft += SystemInformation.VerticalScrollBarWidth
            Else
                clientRight -= SystemInformation.VerticalScrollBarWidth
            End If
        End If

        ' Adjust the dropDownListBox location and/or width if it would
        ' otherwise overlap the left or right edge of the DataGridView.
        If dropDownListBoxLeft < clientLeft Then
            dropDownListBoxLeft = clientLeft
        End If
        Dim dropDownListBoxRight As Integer =
            dropDownListBoxLeft + dropDownListBoxWidth + 1
        If dropDownListBoxRight > clientRight Then
            If dropDownListBoxLeft = clientLeft Then
                dropDownListBoxWidth -= dropDownListBoxRight - clientRight
            Else
                dropDownListBoxLeft -= dropDownListBoxRight - clientRight
                If dropDownListBoxLeft < clientLeft Then
                    dropDownListBoxWidth -= clientLeft - dropDownListBoxLeft
                    dropDownListBoxLeft = clientLeft
                End If
            End If
        End If

        ' Set the ListBox.Bounds property using the calculated values. 
        dropDownListBox.Bounds = New Rectangle(dropDownListBoxLeft,
                    DropDownButtonBounds.Bottom, dropDownListBoxWidth,
                    dropDownListBoxHeight)


    End Sub 'SetDropDownListBoxBounds

    ''' <summary>
    ''' Gets the actual maximum height of the drop-down list, in pixels.
    ''' The maximum height is calculated from the DropDownListBoxMaxLines 
    ''' property value, but is limited to the available height of the 
    ''' DataGridView control. 
    ''' </summary>
    Protected ReadOnly Property DropDownListBoxMaxHeightInternal() As Integer
        Get
            ' Calculate the height of the available client area
            ' in the DataGridView control, taking the horizontal
            ' scroll bar into consideration and leaving room
            ' for the ListBox bottom border and the two buttons 
            ' beneath the list. 
            Dim graphics As Graphics = dropDownListBox.CreateGraphics()
            Dim dataGridViewMaxHeight As Integer =
                DataGridView.Height - DataGridView.ColumnHeadersHeight - 1 -
                (graphics.MeasureString("OK", dropDownListBox.Font).Height + 6) * 2
            graphics.Dispose()
            ' check wether client width of DataGridView control is smaller than total of
            ' all column widths
            Dim ColWidthTotal As Integer = 0
            For Each Col As Object In DataGridView.Columns
                If Col.Visible = True Then
                    ColWidthTotal += Col.Width
                End If
            Next
            If DataGridView.Width < ColWidthTotal Then
                dataGridViewMaxHeight -= SystemInformation.HorizontalScrollBarHeight
            End If

            ' Calculate the height of the list box, using the combined 
            ' height of all items plus 2 for the top and bottom border. 
            Dim listMaxHeight As Integer =
                dropDownListBoxMaxLinesValue * dropDownListBox.ItemHeight + 2

            ' Return the smaller of the two values. 
            If listMaxHeight < dataGridViewMaxHeight Then
                Return listMaxHeight
            Else
                Return dataGridViewMaxHeight
            End If
        End Get
    End Property

#End Region 'drop-down list

#Region "ListBox events: HandleDropDownListBoxEvents, UnhandleDropDownListBoxEvents, ListBox event handlers"

    ''' <summary>
    ''' Adds handlers to ListBox events for handling mouse
    ''' and keyboard input.
    ''' </summary>
    Private Sub HandleDropDownListBoxEvents()
        AddHandler dropDownListBox.ItemCheck, AddressOf DropDownListBox_ItemCheck
        AddHandler dropDownListBox.LostFocus, AddressOf DropDownListBox_LostFocus
        AddHandler dropDownListBox.KeyDown, AddressOf DropDownListBox_KeyDown
        AddHandler OKButton.MouseClick, AddressOf OKButton_MouseClick
        AddHandler CancelButton.MouseClick, AddressOf CancelButton_MouseClick
    End Sub

    ''' <summary>
    ''' Removes the ListBox event handlers. 
    ''' </summary>
    Private Sub UnhandleDropDownListBoxEvents()
        RemoveHandler dropDownListBox.ItemCheck, AddressOf DropDownListBox_ItemCheck
        RemoveHandler dropDownListBox.LostFocus, AddressOf DropDownListBox_LostFocus
        RemoveHandler dropDownListBox.KeyDown, AddressOf DropDownListBox_KeyDown
        RemoveHandler OKButton.MouseClick, AddressOf OKButton_MouseClick
        RemoveHandler CancelButton.MouseClick, AddressOf CancelButton_MouseClick
    End Sub

    ''' <summary>
    ''' Line lastly clicked in the dropDownListBox (for SHIFT + click operations)
    ''' </summary>
    Private _LastItemClicked As Integer = -1

    ''' <summary>
    ''' Flag determins wether the ItemCheck event handler should react or not
    ''' </summary>
    Private _NoInteraction As Boolean = False

    ''' <summary>
    ''' Handles special behaviour regarding the "(Alles auswählen)" item
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A ItemCheckEventArgs that contains the event data.</param>
    Private Sub DropDownListBox_ItemCheck(sender As Object, e As ItemCheckEventArgs)

        If Not _NoInteraction Then

            Debug.Assert(DataGridView IsNot Nothing, "DataGridView is null")

            With dropDownListBox
                ' avoid recursion
                RemoveHandler dropDownListBox.ItemCheck, AddressOf DropDownListBox_ItemCheck
                _NoInteraction = True
                Try
                    Dim i As Integer
                    If e.Index = 0 Then
                        ' check/uncheck all
                        For i = 1 To .Items.Count - 1
                            .SetItemChecked(i, e.NewValue)
                        Next
                    Else
                        ' handle SHIFT clicks
                        If My.Computer.Keyboard.ShiftKeyDown And _LastItemClicked <> e.Index Then
                            For i = _LastItemClicked To e.Index Step IIf(e.Index > _LastItemClicked, 1, -1)
                                .SetItemChecked(i, e.NewValue)
                            Next
                        End If
                        ' save latest clicked line
                        _LastItemClicked = e.Index
                        ' some other line clicked: uncheck 1st entry ("all"), if needed
                        If e.NewValue = CheckState.Unchecked Then
                            .SetItemChecked(0, False)
                        End If
                    End If
                Catch ex As Exception
                    Debug.Print("DropDownListBox_ItemCheck")
                Finally
                    ' add handler again
                    AddHandler dropDownListBox.ItemCheck, AddressOf DropDownListBox_ItemCheck
                    _NoInteraction = False
                End Try

            End With

        End If

    End Sub

    ''' <summary>
    ''' Indicates whether the drop-down list lost focus because the
    ''' user clicked the drop-down button. 
    ''' </summary>
    Private lostFocusOnDropDownButtonClick As Boolean

    ''' <summary>
    ''' Hides the drop-down list when it loses focus. 
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">An EventArgs that contains the event data.</param>
    Private Sub DropDownListBox_LostFocus(ByVal sender As Object, ByVal e As EventArgs)
        ' If the focus was lost because the user clicked the drop-down
        ' button, store a value that prevents the subsequent OnMouseDown
        ' call from displaying the drop-down list again. 
        If DropDownButtonBounds.Contains(
            DataGridView.PointToClient(
            New Point(Control.MousePosition.X, Control.MousePosition.Y))) Then
            lostFocusOnDropDownButtonClick = True
        End If
        ' prevent hiding the drop down when one of the OK or Cancel buttons has been clicked
        If Not ButtonPanel.ClientRectangle.Contains(ButtonPanel.PointToClient(
            New Point(Control.MousePosition.X, Control.MousePosition.Y))) Then
            HideDropDownList()
        End If
    End Sub

    ''' <summary>
    ''' Handles the ENTER and ESC keys.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">A KeyEventArgs that contains the event data.</param>
    Sub DropDownListBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Enter
                UpdateFilter()
                HideDropDownList()
            Case Keys.Escape
                HideDropDownList()
        End Select
    End Sub

    ''' <summary>
    ''' Handles the 'OK' button of the filter list
    ''' </summary>
    Sub OKButton_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        UpdateFilter()
        HideDropDownList()
    End Sub

    ''' <summary>
    ''' Handles the 'Cancel' button of the filter list
    ''' </summary>
    Sub CancelButton_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        HideDropDownList()
    End Sub

#End Region 'ListBox events

#Region "filtering: PopulateFilters, FilterWithoutCurrentColumn, UpdateFilter, RemoveFilter, AvoidNewRowWhenFiltering, GetFilterStatus"

    ''' <summary>
    ''' Assistant class for text sorting (as a fallback if default sorting fails)
    ''' </summary>
    Class TextSort
        Implements IComparer
        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Return x.ToString.CompareTo(y.ToString)
        End Function
    End Class

    ''' <summary>
    ''' Populates the filters dictionary with formatted and unformatted string
    ''' representations of each unique value in the column, accounting for all 
    ''' filters except the current column's. Also adds special filter options. 
    ''' </summary>
    ''' <returns>True if the filter list contains all element, False if it has been cut off (due to being too long)</returns>
    Private Function PopulateFilters() As Boolean

        Dim AllItemsProcessed As Boolean = True

        ' Continue only if there is a DataGridView.
        If DataGridView Is Nothing OrElse DataGridView.DataSource Is Nothing Then
            Return AllItemsProcessed
        End If

        ' Cast the data source to a BindingSource. 
        Dim data As BindingSource =
            TryCast(DataGridView.DataSource, BindingSource)

        Debug.Assert(data IsNot Nothing AndAlso
            data.SupportsFiltering AndAlso OwningColumn IsNot Nothing,
            "DataSource is not a BindingSource, or does not support filtering, or OwningColumn is null")

        ' Prevent the data source from notifying the DataGridView of changes. 
        data.RaiseListChangedEvents = False

        ' Cache the current BindingSource.Filter value and then change 
        ' the Filter property to temporarily remove any filter for the 
        ' current column. 
        Dim oldFilter As String = data.Filter
        data.Filter = FilterWithoutCurrentColumn(oldFilter)

        ' Reset the filters dictionary and initialize some flags
        ' to track whether special filter options are needed. 
        filters.Clear()
        Dim containsBlanks As Boolean = False
        Dim containsNonBlanks As Boolean = False

        ' Initialize an ArrayList to store the values in their original
        ' types. This enables the values to be sorted appropriately.  
        Dim list As New ArrayList(data.Count)

        ' Retrieve each value and add it to the ArrayList if it isn't
        ' already present. 
        Dim item As Object
        For Each item In data
            Dim value As Object = Nothing

            ' Use the ICustomTypeDescriptor interface to retrieve properties
            ' if it is available otherwise, use reflection. The 
            ' ICustomTypeDescriptor interface is useful to customize 
            ' which values are exposed as properties. For example, the 
            ' DataRowView class implements ICustomTypeDescriptor to expose 
            ' cell values as property values.                
            ' 
            ' Iterate through the property names to find a case-insensitive
            ' match with the DataGridViewColumn.DataPropertyName value.
            ' This is necessary because DataPropertyName is case-
            ' insensitive, but the GetProperties and GetProperty methods
            ' used below are case-sensitive.
            Dim ictd As ICustomTypeDescriptor = TryCast(item, ICustomTypeDescriptor)
            If ictd IsNot Nothing Then

                Dim properties As PropertyDescriptorCollection = ictd.GetProperties()
                For Each prop As PropertyDescriptor In properties

                    If (String.Compare(OwningColumn.DataPropertyName,
                        prop.Name, True,
                        System.Globalization.CultureInfo.InvariantCulture) = 0) Then

                        If prop.PropertyType.FullName = "System.Boolean" Then
                            If prop?.GetValue(item) Then
                                value = _CaptionCheckedValues
                            Else
                                value = CaptionUncheckedValues
                            End If
                        Else
                            value = prop.GetValue(item)
                        End If
                        Exit For

                    End If

                Next

            Else

                Dim properties As PropertyInfo() = item.GetType().GetProperties(
                    BindingFlags.Public Or BindingFlags.Instance)
                For Each prop As PropertyInfo In properties

                    If (String.Compare(OwningColumn.DataPropertyName,
                        prop.Name, True,
                        System.Globalization.CultureInfo.InvariantCulture) = 0) Then

                        If prop.PropertyType.FullName = "System.Boolean" Then
                            If prop.GetValue(item, Nothing) Then
                                value = _CaptionCheckedValues
                            Else
                                value = CaptionUncheckedValues
                            End If
                        Else
                            value = prop.GetValue(item, Nothing)
                        End If
                        Exit For

                    End If
                Next

            End If

            ' Skip empty values, but note that they are present. 
            If value Is Nothing OrElse value Is DBNull.Value Then
                containsBlanks = True
                Continue For
            End If

            ' Add values to the ArrayList if they are not already there.
            If Not list.Contains(value) Then
                list.Add(value)
                If list.Count >= MAXFILTERITEMS Then
                    AllItemsProcessed = False
                    Exit For
                End If
            End If
        Next item

        ' Sort the ArrayList. The default Sort method uses the IComparable 
        ' implementation of the stored values so that string, numeric, and 
        ' date values will all be sorted correctly. 

        Try
            list.Sort()
        Catch ex As InvalidOperationException
            ' Fall back to text sorting if there is standard sorting is not possible
            list.Sort(New TextSort)
        End Try

        ' Convert each value in the ArrayList to its formatted representation
        ' and store both the formatted and unformatted string representations
        ' in the filters dictionary. 
        For Each value As Object In list
            ' Use the cell's GetFormattedValue method with the column's
            ' InheritedStyle property so that the dropDownListBox format
            ' will match the display format used for the column's cells. 
            Dim formattedValue As String = Nothing
            Dim style As DataGridViewCellStyle = OwningColumn.InheritedStyle
            formattedValue = CStr(GetFormattedValue(value, -1, style,
                Nothing, Nothing, DataGridViewDataErrorContexts.Formatting))

            If String.IsNullOrEmpty(formattedValue) Then
                ' Skip empty values, but note that they are present.
                containsBlanks = True
            ElseIf Not filters.Contains(formattedValue) Then
                ' Note whether non-empty values are present. 
                containsNonBlanks = True

                ' For all non-empty values, add the formatted and 
                ' unformatted string representations to the filters 
                ' dictionary.
                filters.Add(formattedValue, value.ToString())
            End If
        Next value

        ' Restore the filter to the cached filter string and 
        ' re-enable data source change notifications. 
        If oldFilter IsNot Nothing Then data.Filter = oldFilter
        data.RaiseListChangedEvents = True

        ' Add special filter options to the filters dictionary
        ' along with null values, since unformatted representations
        ' are not needed. 
        filters.Insert(0, _CaptionAllValues, Nothing)
        If containsBlanks Then
            filters.Add(_CaptionBlankValues, Nothing)
        End If

        Return AllItemsProcessed

    End Function 'PopulateFilters

    ''' <summary>
    ''' Returns a copy of the specified filter string after removing the 
    ''' part that filters the current column, if present. 
    ''' </summary>
    ''' <param name="filter">The filter string to parse.</param>
    ''' <returns>A copy of the specified filter string 
    ''' without the current column's filter.</returns>
    Private Function FilterWithoutCurrentColumn(ByVal filter As String) As String

        ' If there is no filter in effect, return String.Empty. 
        If String.IsNullOrEmpty(filter) Then
            Return String.Empty
        End If

        ' If the column is not filtered, return the filter string unchanged. 
        If Not filtered Then
            Return filter
        End If

        If filter.IndexOf(currentColumnFilter) > 0 Then
            ' If the current column filter is not the first filter, return
            ' the specified filter value without the current column filter 
            ' and without the preceding " AND ". 
            Return filter.Replace(" AND " & currentColumnFilter, String.Empty)
        Else
            If filter.Length > currentColumnFilter.Length Then
                ' If the current column filter is the first of multiple 
                ' filters, return the specified filter value without the 
                ' current column filter and without the subsequent " AND ". 
                Return filter.Replace(currentColumnFilter & " AND ", String.Empty)
            Else
                ' If the current column filter is the only filter, 
                ' return the empty string.
                Return String.Empty
            End If
        End If

    End Function 'FilterWithoutCurrentColumn

    ''' <summary>
    ''' Updates the BindingSource.Filter value based on a user selection
    ''' from the drop-down filter list. 
    ''' </summary>
    Private Sub UpdateFilter()

        ' Cast the data source to an IBindingListView.
        Dim data As IBindingListView =
            TryCast(DataGridView.DataSource, IBindingListView)

        Debug.Assert(data IsNot Nothing AndAlso data.SupportsFiltering,
            "DataSource is not an IBindingListView or does not support filtering")

        ' If the user selection is (Alles auswählen), remove any filter currently 
        ' in effect for the column. 
        'If selectedFilterValue.Equals(_CaptionAllValues) Then
        '    data.Filter = FilterWithoutCurrentColumn(data.Filter)
        '    filtered = False
        '    currentColumnFilter = String.Empty
        '    Return
        'End If

        ' Declare a variable to store the filter string for this column.
        Dim newColumnFilter As String = String.Empty

        ' Store the column name in a form acceptable to the Filter property, 
        ' using a backslash to escape any closing square brackets. 
        Dim columnDataProperty As String = OwningColumn.DataPropertyName.Replace("]", "\]")

        ' Determine the column filter string based on the user selection.
        ' For (Leere) and (NonBlanks), the filter string determines whether
        ' the column value is null or an empty string. Otherwise, the filter
        ' string determines whether the column value is the selected value.
        Dim Itm As New Object
        Dim ItmString As String
        Dim ItmDate As Date
        Dim DateFormats() As String = {"dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy",
                                       "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd"}
        For Each Itm In dropDownListBox.CheckedItems
            If Itm.ToString = _CaptionBlankValues Then
                ItmString = ""
            ElseIf Itm.ToString = _CaptionCheckedValues Then
                ItmString = "True"
            ElseIf Itm.ToString = _CaptionUncheckedValues Then
                ItmString = "False"
            Else
                ItmString = Itm.ToString
            End If
            If ItmString = _CaptionAllValues Then
                ' All values = remove filter
                newColumnFilter = String.Empty
                Exit For
            Else
                If newColumnFilter.Length > 0 Then
                    newColumnFilter &= " OR "
                End If
                If DateTime.TryParseExact(ItmString, DateFormats, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, ItmDate) Then
                    newColumnFilter &= String.Format("([{0}] >= '{1}:00' AND [{0}] <= '{1}:59')",
                                                     columnDataProperty,
                                                     ItmDate.ToString("yyyy-MM-dd HH:mm"))
                Else
                    newColumnFilter &= String.Format("[{0}]='{1}'", columnDataProperty, ItmString.Replace("'", "''"))
                End If

                If String.IsNullOrEmpty(ItmString) Then
                    newColumnFilter &= String.Format(" OR [{0}] IS NULL", columnDataProperty)
                End If
                ReDim Preserve currentColumnFilterItems(currentColumnFilterItems.Length)
                currentColumnFilterItems(currentColumnFilterItems.Length - 1) = ItmString
            End If
        Next

        If newColumnFilter.Length > 0 Then
            newColumnFilter = "(" & newColumnFilter & ")"
            ' Determine the new filter string by removing the previous column 
            ' filter string from the BindingSource.Filter value, then appending 
            ' the new column filter string, using " AND " as appropriate. 
            Dim newFilter As String = FilterWithoutCurrentColumn(data.Filter)
            If String.IsNullOrEmpty(newFilter) Then
                newFilter = newColumnFilter
            Else
                If newColumnFilter.Length > 0 Then
                    newFilter &= " AND " & newColumnFilter
                Else
                    newFilter = ""
                End If

            End If

            ' Set the filter to the new value.
            Try
                data.Filter = newFilter
            Catch ex As InvalidExpressionException
                Throw New NotSupportedException("Invalid expression: " & newFilter, ex)
            End Try

            ' Indicate that the column is currently filtered
            ' and store the new column filter for use by subsequent
            ' calls to the FilterWithoutCurrentColumn method. 
            filtered = True
            currentColumnFilter = newColumnFilter
        Else
            data.Filter = FilterWithoutCurrentColumn(data.Filter)
            filtered = False
            currentColumnFilter = String.Empty
            currentColumnFilterItems = {}
            Return
        End If

    End Sub 'UpdateFilter

    ''' <summary>
    ''' Removes the filter from the BindingSource bound to the specified DataGridView. 
    ''' </summary>
    ''' <param name="dataGridView">The DataGridView bound to the BindingSource to unfilter.</param>
    Public Shared Sub RemoveFilter(ByVal dataGridView As DataGridView)

        If dataGridView Is Nothing Then
            Throw New ArgumentNullException("dataGridView")
        End If

        ' Cast the data source to a BindingSource.
        Dim data As BindingSource =
            TryCast(dataGridView.DataSource, BindingSource)

        ' Confirm that the data source is a BindingSource that 
        ' supports filtering.
        If data Is Nothing OrElse
            data.DataSource Is Nothing OrElse
            Not data.SupportsFiltering Then
            Throw New ArgumentException("The DataSource property of the " &
            "specified DataGridView is not set to a BindingSource " &
            "with a SupportsFiltering property value of true.")
        End If

        ' Ensure that the current row is not the row for new records.
        ' This prevents the new row from being added when the filter changes.
        If dataGridView.CurrentRow IsNot Nothing AndAlso
            dataGridView.CurrentRow.IsNewRow Then
            dataGridView.CurrentCell = Nothing
        End If

        ' Remove the filter. 
        data.Filter = Nothing

    End Sub 'RemoveFilter

    ''' <summary>
    ''' Gets a status string for the specified DataGridView indicating the 
    ''' number of visible rows in the bound, filtered BindingSource, or 
    ''' String.Empty if all rows are currently visible. 
    ''' </summary>
    ''' <param name="dataGridView">The DataGridView bound to the 
    ''' BindingSource to return the filter status for.</param>
    ''' <returns>A string in the format "x of y records found" where x is 
    ''' the number of rows currently displayed and y is the number of rows 
    ''' available, or String.Empty if all rows are currently displayed.</returns>
    Public Shared Function GetFilterStatus(
        ByVal dataGridView As DataGridView) As String

        ' Continue only if the specified value is valid. 
        If dataGridView Is Nothing Then
            Throw New ArgumentNullException("dataGridView")
        End If

        ' Cast the data source to a BindingSource.
        Dim data As BindingSource =
            TryCast(dataGridView.DataSource, BindingSource)

        ' Return String.Empty if there is no appropriate data source or
        ' there is no filter in effect. 
        If String.IsNullOrEmpty(data.Filter) OrElse
            data Is Nothing OrElse
            data.DataSource Is Nothing OrElse
            Not data.SupportsFiltering Then
            Return String.Empty
        End If

        ' Retrieve the filtered row count. 
        Dim currentRowCount As Integer = data.Count

        ' Retrieve the unfiltered row count by 
        ' temporarily unfiltering the data.
        data.RaiseListChangedEvents = False
        Dim oldFilter As String = data.Filter
        data.Filter = Nothing
        Dim unfilteredRowCount As Integer = data.Count
        data.Filter = oldFilter
        data.RaiseListChangedEvents = True

        Debug.Assert(currentRowCount <= unfilteredRowCount,
            "current count is greater than unfiltered count")

        ' Return String.Empty if the filtered and unfiltered counts
        ' are the same, otherwise, return the status string. 
        If currentRowCount = unfilteredRowCount Then
            Return String.Empty
        End If
        Return String.Format("{0} of {1} records found",
            currentRowCount, unfilteredRowCount)

    End Function 'GetFilterStatus

#End Region 'filtering

#Region "button bounds: DropDownButtonBounds, InvalidateDropDownButtonBounds, SetDropDownButtonBounds, AdjustPadding"

    ''' <summary>
    ''' The bounds of the drop-down button, or Rectangle.Empty if filtering 
    ''' is disabled or the button bounds need to be recalculated. 
    ''' </summary>
    Private dropDownButtonBoundsValue As Rectangle = Rectangle.Empty

    ''' <summary>
    ''' The bounds of the drop-down button, or Rectangle.Empty if filtering
    ''' is disabled. Recalculates the button bounds if filtering is enabled
    ''' and the bounds are empty.
    ''' </summary>
    Protected ReadOnly Property DropDownButtonBounds() As Rectangle
        Get
            If Not FilteringEnabled Then
                Return Rectangle.Empty
            End If
            If dropDownButtonBoundsValue = Rectangle.Empty Then
                SetDropDownButtonBounds()
            End If
            Return dropDownButtonBoundsValue
        End Get
    End Property

    ''' <summary>
    ''' Sets dropDownButtonBoundsValue to Rectangle.Empty if it isn't already empty. 
    ''' This indicates that the button bounds should be recalculated. 
    ''' </summary>
    Private Sub InvalidateDropDownButtonBounds()
        If Not dropDownButtonBoundsValue.IsEmpty Then
            dropDownButtonBoundsValue = Rectangle.Empty
        End If
    End Sub

    ''' <summary>
    ''' Sets the position and size of dropDownButtonBoundsValue based on the current 
    ''' cell bounds and the preferred cell height for a single line of header text. 
    ''' </summary>
    Private Sub SetDropDownButtonBounds()

        ' Retrieve the cell display rectangle, which is used to 
        ' set the position of the drop-down button. 
        Dim cellBounds As Rectangle =
            DataGridView.GetCellDisplayRectangle(ColumnIndex, -1, False)

        ' Initialize a variable to store the button edge length,
        ' setting its initial value based on the font height. 
        Dim buttonEdgeLength As Integer = InheritedStyle.Font.Height + 5

        ' Calculate the height of the cell borders and padding.
        Dim borderRect As Rectangle = BorderWidths(
            DataGridView.AdjustColumnHeaderBorderStyle(
            DataGridView.AdvancedColumnHeadersBorderStyle,
            New DataGridViewAdvancedBorderStyle(), False, False))
        Dim borderAndPaddingHeight As Integer = 2 +
            borderRect.Top + borderRect.Height +
            InheritedStyle.Padding.Vertical
        Dim visualStylesEnabled As Boolean =
            Application.RenderWithVisualStyles AndAlso
            DataGridView.EnableHeadersVisualStyles
        If visualStylesEnabled Then
            borderAndPaddingHeight += 3
        End If

        ' Constrain the button edge length to the height of the 
        ' column headers minus the border and padding height. 
        If buttonEdgeLength >
            DataGridView.ColumnHeadersHeight -
            borderAndPaddingHeight Then
            buttonEdgeLength =
                DataGridView.ColumnHeadersHeight - borderAndPaddingHeight
        End If

        ' Constrain the button edge length to the
        ' width of the cell minus three.
        If buttonEdgeLength > cellBounds.Width - 3 Then
            buttonEdgeLength = cellBounds.Width - 3
        End If

        ' Calculate the location of the drop-down button, with adjustments
        ' based on whether visual styles are enabled. 
        Dim topOffset As Integer
        If visualStylesEnabled Then
            topOffset = 5 ' 4
        Else
            topOffset = 1
        End If

        Dim top As Integer = cellBounds.Bottom - buttonEdgeLength - topOffset

        Dim leftOffset As Integer
        If visualStylesEnabled Then
            leftOffset = 3
        Else
            leftOffset = 1
        End If

        Dim left As Integer = 0

        If Me.DataGridView.RightToLeft = RightToLeft.No Then
            left = cellBounds.Right - buttonEdgeLength - leftOffset
        Else
            left = cellBounds.Left + leftOffset
        End If

        ' Set the dropDownButtonBoundsValue value using the calculated 
        ' values, and adjust the cell padding accordingly.  
        dropDownButtonBoundsValue = New Rectangle(left, top, buttonEdgeLength, buttonEdgeLength)
        AdjustPadding((buttonEdgeLength + leftOffset))

    End Sub 'SetDropDownButtonBounds

    ''' <summary>
    ''' Adjusts the cell padding to widen the header by the drop-down button width.
    ''' </summary>
    ''' <param name="newDropDownButtonPaddingOffset">The new drop-down button width.</param>
    Private Sub AdjustPadding(ByVal newDropDownButtonPaddingOffset As Integer)

        ' Determine the difference between the new and current 
        ' padding adjustment.
        Dim widthChange As Integer = newDropDownButtonPaddingOffset -
            currentDropDownButtonPaddingOffset

        ' If the padding needs to change, store the new value and 
        ' make the change.
        If widthChange <> 0 Then
            ' Store the offset for the drop-down button separately from 
            ' the padding in case the client needs additional padding.
            currentDropDownButtonPaddingOffset = newDropDownButtonPaddingOffset

            ' Create a new Padding using the adjustment amount, then add it
            ' to the cell's existing Style.Padding property value. 
            Dim dropDownPadding As Padding = New Padding(0, 0, widthChange, 0)
            Style.Padding =
                Padding.Add(InheritedStyle.Padding, dropDownPadding)
        End If

    End Sub

    ''' <summary>
    ''' The current width of the drop-down button. This field is used to adjust the cell padding.  
    ''' </summary>
    Private currentDropDownButtonPaddingOffset As Integer

#End Region 'button bounds

#Region "public properties: FilteringEnabled, AutomaticSortingEnabled, DropDownListBoxMaxLines" '

    ''' <summary>
    ''' Indicates whether filtering is enabled for the owning column. 
    ''' </summary>
    Private filteringEnabledValue As Boolean = True

    ''' <summary>
    ''' Gets or sets a value indicating whether filtering is enabled.
    ''' </summary>
    <DefaultValue(True)>
    Public Property FilteringEnabled() As Boolean
        Get
            ' Return filteringEnabledValue if there is no DataGridView
            ' or if its DataSource property has not been set. 
            If DataGridView Is Nothing OrElse
                DataGridView.DataSource Is Nothing Then
                Return filteringEnabledValue
            End If

            ' If the DataSource property has been set, return a value that combines 
            ' the filteringEnabledValue and BindingSource.SupportsFiltering values.
            Dim data As BindingSource =
                TryCast(DataGridView.DataSource, BindingSource)
            ' Debug.Assert(data IsNot Nothing)
            If data IsNot Nothing Then
                Return filteringEnabledValue AndAlso data.SupportsFiltering
            Else
                ' Return false if Datasource cannot be interpreted as BindingSource
                Return False
            End If
        End Get

        Set(ByVal value As Boolean)
            ' If filtering is disabled, remove the padding adjustment
            ' and invalidate the button bounds. 
            If Not value Then
                AdjustPadding(0)
                InvalidateDropDownButtonBounds()
            End If

            filteringEnabledValue = value
        End Set
    End Property

    ''' <summary>
    ''' Indicates whether automatic sorting is enabled. 
    ''' </summary>
    Private automaticSortingEnabledValue As Boolean = True

    ''' <summary>
    ''' Gets or sets a value indicating whether automatic sorting is 
    ''' enabled for the owning column. 
    ''' </summary>
    <DefaultValue(True)>
    Public Property AutomaticSortingEnabled() As Boolean
        Get
            Return automaticSortingEnabledValue
        End Get
        Set(ByVal value As Boolean)
            automaticSortingEnabledValue = value
            If OwningColumn IsNot Nothing Then
                If value Then
                    OwningColumn.SortMode =
                        DataGridViewColumnSortMode.Programmatic
                Else
                    OwningColumn.SortMode =
                        DataGridViewColumnSortMode.NotSortable
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' The maximum number of lines in the drop-down list. 
    ''' </summary>
    Private dropDownListBoxMaxLinesValue As Integer = 30

    ''' <summary>
    ''' Gets or sets the maximum number of lines to display in the drop-down filter list. 
    ''' The actual height of the drop-down list is constrained by the DataGridView height. 
    ''' </summary>
    <Browsable(True)>
    <DefaultValue(30)>
    Public Property DropDownListBoxMaxLines() As Integer
        Get
            Return dropDownListBoxMaxLinesValue
        End Get
        Set(ByVal value As Integer)
            dropDownListBoxMaxLinesValue = value
        End Set
    End Property

    Private _CaptionOKButton As String
    ''' <summary>
    ''' Caption of the OK button at the bottom of the filter list
    ''' </summary>
    <Browsable(True)>
    <DefaultValue("OK")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property CaptionOKButton() As String
        Get
            If String.IsNullOrEmpty(_CaptionOKButton) Then
                Return "OK"
            Else
                Return _CaptionOKButton
            End If
        End Get
        Set(ByVal value As String)
            _CaptionOKButton = value
        End Set
    End Property

    Private _CaptionCancelButton As String
    ''' <summary>
    ''' Caption of the cancel button at the bottom of the filter list
    ''' </summary>
    <Browsable(True)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <DefaultValue("Abbrechen")>
    Public Property CaptionCancelButton() As String
        Get
            If String.IsNullOrEmpty(_CaptionCancelButton) Then
                Return "Abbrechen"
            Else
                Return _CaptionCancelButton
            End If
        End Get
        Set(ByVal value As String)
            _CaptionCancelButton = value
        End Set
    End Property

    Private _CaptionAllValues As String = "(Alles auswählen)"
    ''' <summary>
    ''' Caption for the 1st entry in filter list: default "(Alles auswählen)"
    ''' </summary>
    <Browsable(True)>
    <DefaultValue("(Alles auswählen)")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Description("Caption for the first entry in filter list, representing all values of the filter")>
    Public Property CaptionAllValues() As String
        Get
            Return _CaptionAllValues
        End Get
        Set(ByVal value As String)
            _CaptionAllValues = value
        End Set
    End Property

    Private _CaptionBlankValues As String = "(Leere)"
    ''' <summary>
    ''' Caption for the 2nd last entry in filter list: default "(Leere)"
    ''' </summary>
    <Browsable(True)>
    <DefaultValue("(Leere)")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <Description("Caption for the 2nd last entry in filter list, representing all blank values")>
    Public Property CaptionBlankValues() As String
        Get
            Return _CaptionBlankValues
        End Get
        Set(ByVal value As String)
            _CaptionBlankValues = value
        End Set
    End Property

    Private _CaptionCheckedValues As String = "Checked"
    ''' <summary>
    ''' Caption for checkbox columns: all checked values"
    ''' </summary>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property CaptionCheckedValues() As String
        Get
            Return _CaptionCheckedValues
        End Get
        Set(ByVal value As String)
            _CaptionCheckedValues = value
        End Set
    End Property

    Private _CaptionUncheckedValues As String = "Unchecked"
    ''' <summary>
    ''' Caption for checkbox columns: all unchecked values"
    ''' </summary>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property CaptionUncheckedValues() As String
        Get
            Return _CaptionUncheckedValues
        End Get
        Set(ByVal value As String)
            _CaptionUncheckedValues = value
        End Set
    End Property

    ''' <summary>
    ''' Wrapper for DataGridView, to be able to use custom extensions from inheritants of type DGV
    ''' </summary>
    Public ReadOnly Property DataGridViewAsObject() As Object
        Get
            Return DataGridView
        End Get
    End Property



#End Region 'public properties

    ''' <summary>
    ''' Represents a ListBox control used as a drop-down filter list
    ''' in a DataGridView control.
    ''' </summary>
    Private Class FilterListBox
        Inherits CheckedListBox

        ''' <summary>
        ''' Initializes a new instance of the FilterListBox class.
        ''' </summary>
        Public Sub New()
            Visible = False
            IntegralHeight = True
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            TabStop = False
            CheckOnClick = True
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink)
        End Sub

        ''' <summary>
        ''' Indicates that the FilterListBox will handle (or ignore) all 
        ''' keystrokes that are not handled by the operating system. 
        ''' </summary>
        ''' <param name="keyData">A Keys value that represents the keyboard input.</param>
        ''' <returns>true in all cases.</returns>
        Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
            Return True
        End Function

        ''' <summary>
        ''' Processes a keyboard message directly, preventing it from being
        ''' intercepted by the parent DataGridView control.
        ''' </summary>
        ''' <param name="m">A Message, passed by reference, that 
        ''' represents the window message to process.</param>
        ''' <returns>true if the message was processed by the control;
        ''' otherwise, false.</returns>
        Protected Overrides Function ProcessKeyMessage( _
            ByRef m As Message) As Boolean
            Return ProcessKeyEventArgs(m)
        End Function

    End Class 'FilterListBox

End Class 'DataGridViewAutoFilterColumnHeaderCell


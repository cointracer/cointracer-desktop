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

Imports System.Text

Namespace CheckComboBox

    Public Class CheckedComboBox
        Inherits ComboBox

        Class CheckedComboBoxItem
            Private _ID As Long
            Public Property ID() As Long
                Get
                    Return _ID
                End Get
                Set(ByVal value As Long)
                    _ID = value
                End Set
            End Property
            Private _Value As String
            Public Property Value() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    _Value = value
                End Set
            End Property
            Public Sub New(ID As Long, ByVal Value As String)
                _ID = ID
                _Value = Value
            End Sub
            Public Overrides Function ToString() As String
                Return _Value
            End Function
        End Class

        ''' <summary>
        ''' Internal class to represent the dropdown list of the CheckedComboBox
        ''' </summary>
        Shadows Class Dropdown
            Inherits Form

            ' ---------------------------------- internal class CCBoxEventArgs --------------------------------------------
            ''' <summary>
            ''' Custom EventArgs encapsulating value as to whether the combo box value(s) should be assignd to or not.
            ''' </summary>
            Class CCBoxEventArgs
                Inherits EventArgs

                Private _assignValues As Boolean

                Public Property AssignValues As Boolean
                    Get
                        Return _assignValues
                    End Get
                    Set(value As Boolean)
                        _assignValues = value
                    End Set
                End Property

                Private e As EventArgs

                Public Property EventArgs As EventArgs
                    Get
                        Return e
                    End Get
                    Set(value As EventArgs)
                        e = value
                    End Set
                End Property

                Public Sub New(ByVal e As EventArgs, ByVal assignValues As Boolean)
                    MyBase.New()
                    Me.e = e
                    Me._assignValues = assignValues
                End Sub
            End Class

            ' ---------------------------------- internal class CustomCheckedListBox --------------------------------------------
            ''' <summary>
            ''' A custom CheckedListBox being shown within the dropdown form representing the dropdown list of the CheckedComboBox.
            ''' </summary>
            Class CustomCheckedListBox
                Inherits CheckedListBox

                Private curSelIndex As Integer = -1

                Public Sub New()
                    MyBase.New()
                    Me.SelectionMode = SelectionMode.One
                    Me.HorizontalScrollbar = True
                End Sub

                ''' <summary>
                ''' Intercepts the keyboard input, [Enter] confirms a selection and [Esc] cancels it.
                ''' </summary>
                ''' <param name="e">The Key event arguments</param>
                Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
                    If (e.KeyCode = Keys.Enter) Then
                        ' Enact selection.
                        CType(Parent, CheckedComboBox.Dropdown).OnDeactivate(New CCBoxEventArgs(Nothing, True))
                        e.Handled = True
                    ElseIf (e.KeyCode = Keys.Escape) Then
                        ' Cancel selection.
                        CType(Parent, CheckedComboBox.Dropdown).OnDeactivate(New CCBoxEventArgs(Nothing, False))
                        e.Handled = True
                    ElseIf (e.KeyCode = Keys.Delete) Then
                        ' Delete unckecks all, [Shift + Delete] checks all.
                        Dim i As Integer = 0
                        Do While (i < Items.Count)
                            SetItemChecked(i, e.Shift)
                            i = (i + 1)
                        Loop
                        e.Handled = True
                    End If
                    ' If no Enter or Esc keys presses, let the base class handle it.
                    MyBase.OnKeyDown(e)
                End Sub

                Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
                    MyBase.OnMouseMove(e)
                    Dim index As Integer = IndexFromPoint(e.Location)
                    ' Debug.WriteLine("Mouse over item: " + (index >= 0 ? GetItemText(Items[index]) : "None"));
                    If ((index >= 0) _
                                AndAlso (index <> curSelIndex)) Then
                        curSelIndex = index
                        SetSelected(index, True)
                    End If
                End Sub
            End Class

            ' end internal class CustomCheckedListBox
            ' --------------------------------------------------------------------------------------------------------
            ' ********************************************* Data *********************************************
            Private ccbParent As CheckedComboBox

            ' Keeps track of whether checked item(s) changed, hence the value of the CheckedComboBox as a whole changed.
            ' This is simply done via maintaining the old string-representation of the value(s) and the new one and comparing them!
            Private oldStrValue As String = ""

            Public ReadOnly Property ValueChanged As Boolean
                Get
                    Dim newStrValue As String = ccbParent.Text
                    If ((oldStrValue.Length > 0) _
                                AndAlso (newStrValue.Length > 0)) Then
                        Return (oldStrValue.CompareTo(newStrValue) <> 0)
                    Else
                        Return (oldStrValue.Length <> newStrValue.Length)
                    End If
                End Get
            End Property

            ' Array holding the checked states of the items. This will be used to reverse any changes if user cancels selection.
            Private checkedStateArr() As Boolean

            ' Whether the dropdown is closed.
            Private dropdownClosed As Boolean = True

            Private cclb As CustomCheckedListBox

            Public Property List As CustomCheckedListBox
                Get
                    Return cclb
                End Get
                Set(value As CustomCheckedListBox)
                    cclb = value
                End Set
            End Property

            ' ********************************************* Construction *********************************************
            Public Sub New(ByVal ccbParent As CheckedComboBox)
                MyBase.New()
                Me.ccbParent = ccbParent
                InitializeComponent()
                Me.ShowInTaskbar = False
                ' Add a handler to notify our parent of ItemCheck events.
                AddHandler Me.cclb.ItemCheck, AddressOf Me.cclb_ItemCheck
            End Sub

            ' ********************************************* Methods *********************************************
            ' Create a CustomCheckedListBox which fills up the entire form area.
            Private Sub InitializeComponent()
                Me.cclb = New CustomCheckedListBox
                Me.SuspendLayout()
                ' 
                ' cclb
                ' 
                Me.cclb.BorderStyle = System.Windows.Forms.BorderStyle.None
                Me.cclb.Dock = System.Windows.Forms.DockStyle.Fill
                Me.cclb.FormattingEnabled = True
                Me.cclb.Location = New System.Drawing.Point(0, 0)
                Me.cclb.Name = "cclb"
                Me.cclb.Size = New System.Drawing.Size(47, 15)
                Me.cclb.TabIndex = 0
                ' 
                ' Dropdown
                ' 
                Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
                Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
                Me.BackColor = System.Drawing.SystemColors.Menu
                Me.ClientSize = New System.Drawing.Size(47, 16)
                Me.ControlBox = False
                Me.Controls.Add(Me.cclb)
                Me.ForeColor = System.Drawing.SystemColors.ControlText
                Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
                Me.MinimizeBox = False
                Me.Name = "ccbParent"
                Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
                Me.ResumeLayout(False)
            End Sub

            Public Function GetCheckedItemsStringValue() As String
                Dim sb As StringBuilder = New StringBuilder("")
                Dim i As Integer
                If ccbParent.FirstLineIsCheckAll AndAlso cclb.Items.Count > 1 Then
                    If cclb.GetItemChecked(0) Then
                        Return cclb.GetItemText(cclb.Items(0))
                    Else
                        i = 1
                    End If
                Else
                    i = 0
                End If
                Do While (i < cclb.Items.Count)
                    If cclb.GetItemChecked(i) Then
                        sb.Append(cclb.GetItemText(cclb.Items(i))).Append(ccbParent._valueSeparator)
                    End If
                    i += 1
                Loop
                If (sb.Length > 0) Then
                    sb.Remove((sb.Length - ccbParent._valueSeparator.Length), ccbParent._valueSeparator.Length)
                End If
                Return sb.ToString
            End Function

            Public Function GetCheckedItemsIDs() As String
                Dim Value As String = ""
                Dim StartIndex As Integer
                If ccbParent.FirstLineIsCheckAll AndAlso cclb.Items.Count > 0 AndAlso cclb.GetItemChecked(0) Then ' AndAlso cclb.Items(0).IsChecked Then
                    StartIndex = 1
                Else
                    StartIndex = 0
                End If
                For i As Integer = StartIndex To cclb.CheckedItems.Count - 1
                    Value &= CallByName(cclb.CheckedItems(i), ccbParent.IDColumnName, CallType.Get) & ccbParent._valueSeparator
                Next
                If Value.Length > 1 Then
                    Value = Value.Remove(Value.Length - ccbParent._valueSeparator.Length)
                End If
                Return Value
            End Function

            ''' <summary>
            ''' Closes the dropdown portion and enacts any changes according to the specified boolean parameter.
            ''' NOTE: even though the caller might ask for changes to be enacted, this doesn't necessarily mean
            '''       that any changes have occurred as such. Caller should check the ValueChanged property of the
            '''       CheckedComboBox (after the dropdown has closed) to determine any actual value changes.
            ''' </summary>
            ''' <param name="enactChanges"></param>
            Public Sub CloseDropdown(ByVal enactChanges As Boolean)
                If dropdownClosed Then
                    Return
                End If
                ' Debug.WriteLine("CloseDropdown");
                ' Perform the actual selection and display of checked items.
                If enactChanges Then
                    ccbParent.SelectedIndex = -1
                    ' Set the text portion equal to the string comprising all checked items (if any, otherwise empty!).
                    ccbParent.Text = GetCheckedItemsStringValue()
                Else
                    ' Caller cancelled selection - need to restore the checked items to their original state.
                    Dim i As Integer = 0
                    Do While (i < cclb.Items.Count)
                        cclb.SetItemChecked(i, checkedStateArr(i))
                        i = (i + 1)
                    Loop
                End If
                ' From now on the dropdown is considered closed. We set the flag here to prevent OnDeactivate() calling
                ' this method once again after hiding this window.
                dropdownClosed = True
                ' Set the focus to our parent CheckedComboBox and hide the dropdown check list.
                ccbParent.Focus()
                Me.Hide()
                ' Notify CheckedComboBox that its dropdown is closed. (NOTE: it does not matter which parameters we pass to
                ' OnDropDownClosed() as long as the argument is CCBoxEventArgs so that the method knows the notification has
                ' come from our code and not from the framework).
                ccbParent.OnDropDownClosed(New CCBoxEventArgs(Nothing, False))
            End Sub

            Protected Overrides Sub OnActivated(ByVal e As EventArgs)
                ' Debug.WriteLine("OnActivated");
                MyBase.OnActivated(e)
                dropdownClosed = False
                ' Assign the old string value to compare with the new value for any changes.
                oldStrValue = ccbParent.Text
                ' Make a copy of the checked state of each item, in cace caller cancels selection.
                checkedStateArr = New Boolean((cclb.Items.Count) - 1) {}
                Dim i As Integer = 0
                Do While (i < cclb.Items.Count)
                    checkedStateArr(i) = cclb.GetItemChecked(i)
                    i = (i + 1)
                Loop
            End Sub

            Protected Overrides Sub OnDeactivate(ByVal e As EventArgs)
                ' Debug.WriteLine("OnDeactivate");
                MyBase.OnDeactivate(e)
                Dim ce As New CCBoxEventArgs(e, True)
                If (Not (ce) Is Nothing) Then
                    CloseDropdown(ce.AssignValues)
                Else
                    ' If not custom event arguments passed, means that this method was called from the
                    ' framework. We assume that the checked values should be registered regardless.
                    CloseDropdown(True)
                End If
            End Sub

            Private Sub cclb_ItemCheck(ByVal sender As Object, ByVal e As ItemCheckEventArgs)
                If ccbParent.FirstLineIsCheckAll Then
                    ccbParent.FirstLineIsCheckAll = False   ' disable temporarily to prevent eternal loops
                    If e.Index = 0 Then
                        ' un-/check all!
                        For i As Integer = 0 To cclb.Items.Count - 1
                            cclb.SetItemChecked(i, e.NewValue)
                        Next
                    ElseIf e.Index > 0 And e.NewValue = CheckState.Unchecked Then
                        cclb.SetItemChecked(0, e.NewValue)
                    ElseIf e.Index > 0 And e.NewValue = CheckState.Checked Then
                        If cclb.CheckedItems.Count >= cclb.Items.Count - 2 Then
                            cclb.SetItemChecked(0, e.NewValue)
                        End If
                    End If
                    ccbParent.FirstLineIsCheckAll = True
                End If
            End Sub

        End Class

        ' end internal class Dropdown
        ' ******************************** Data ********************************
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ' A form-derived object representing the drop-down list of the checked combo box.
        Private WithEvents _dropdown As Dropdown

        ' The valueSeparator character(s) between the ticked elements as they appear in the 
        ' text portion of the CheckedComboBox.
        Private _valueSeparator As String

        Public Property ValueSeparator As String
            Get
                Return _valueSeparator
            End Get
            Set(value As String)
                _valueSeparator = value
            End Set
        End Property

        Public Property CheckOnClick As Boolean
            Get
                Return _dropdown.List.CheckOnClick
            End Get
            Set(value As Boolean)
                _dropdown.List.CheckOnClick = value
            End Set
        End Property

        Private _DisplayColumnName As String
        Public Shadows Property DisplayMember As String
            Get
                Return _DisplayColumnName
            End Get
            Set(value As String)
                _DisplayColumnName = value
                MyBase.DisplayMember = value
                _dropdown.List.DisplayMember = value
            End Set
        End Property

        Private _IDColumnName As String
        Public Property IDColumnName() As String
            Get
                Return _IDColumnName
            End Get
            Set(ByVal value As String)
                _IDColumnName = value
            End Set
        End Property

        Public Shadows ReadOnly Property Items As CheckedListBox.ObjectCollection
            Get
                Return _dropdown.List.Items
            End Get
        End Property

        Public ReadOnly Property CheckedItems As CheckedListBox.CheckedItemCollection
            Get
                Return _dropdown.List.CheckedItems
            End Get
        End Property

        Public ReadOnly Property CheckedIndices As CheckedListBox.CheckedIndexCollection
            Get
                Return _dropdown.List.CheckedIndices
            End Get
        End Property

        Public ReadOnly Property ValueChanged As Boolean
            Get
                Return _dropdown.ValueChanged
            End Get
        End Property

        Private _FirstLineIsCheckAll As Boolean
        ''' <summary>
        ''' Gibt an, ob die erste Zeile der Drop-Down-List für 'Alle Elemente' steht
        ''' </summary>
        Public Property FirstLineIsCheckAll() As Boolean
            Get
                Return _FirstLineIsCheckAll
            End Get
            Set(ByVal value As Boolean)
                _FirstLineIsCheckAll = value
            End Set
        End Property

        ''' <summary>
        ''' Gibt an, ob alle vorhandenen Items angehakt sind (True) oder nicht (False)
        ''' </summary>
        Public ReadOnly Property AllChecked() As Boolean
            Get
                Return _dropdown.List.CheckedItems.Count = _dropdown.List.Items.Count
            End Get
        End Property

        Private _SelectSQL As String
        Public Property SelectSQL() As String
            Get
                Return _SelectSQL
            End Get
            Set(ByVal value As String)
                _SelectSQL = value
            End Set
        End Property

        Private _Cnn As SQLite.SQLiteConnection
        Public Property Connection() As SQLite.SQLiteConnection
            Get
                Return _Cnn
            End Get
            Set(ByVal value As SQLite.SQLiteConnection)
                _Cnn = value
            End Set
        End Property

        ' Event handler for when an item check state changes.
        Public Event ItemCheck As ItemCheckEventHandler

        ' ******************************** Construction ********************************
        Public Sub New()
            MyBase.New()
            ' We want to do the drawing of the dropdown.
            Me.DrawMode = DrawMode.OwnerDrawVariable
            ' Default value separator.
            Me._valueSeparator = ", "
            ' This prevents the actual ComboBox dropdown to show, although it's not strickly-speaking necessary.
            ' But including this remove a slight flickering just before our dropdown appears (which is caused by
            ' the empty-dropdown list of the ComboBox which is displayed for fractions of a second).
            Me.DropDownHeight = 1
            ' This is the default setting - text portion is editable and user must click the arrow button
            ' to see the list portion. Although we don't want to allow the user to edit the text portion
            ' the DropDownList style is not being used because for some reason it wouldn't allow the text
            ' portion to be programmatically set. Hence we set it as editable but disable keyboard input (see below).
            Me.DropDownStyle = ComboBoxStyle.DropDown
            Me._dropdown = New Dropdown(Me)
            ' CheckOnClick style for the dropdown (NOTE: must be set after dropdown is created).
            Me.CheckOnClick = True
        End Sub

        ''' <summary>
        ''' Erstellt das Control inkl. Datenanbindung
        ''' </summary>
        ''' <param name="Connection">SQL-Connection für das Einlesen der Daten</param>
        ''' <param name="SelectSQL">SQL-Statement für das Selektieren der Daten</param>
        ''' <remarks></remarks>
        Public Sub New(Connection As SQLite.SQLiteConnection, SelectSQL As String)
            Me.New()
            Me.SelectSQL = SelectSQL
            Me.Connection = Connection
            Me.Reload()
        End Sub

        ''' <summary>
        ''' Erstellt das Control inkl. Datenanbindung
        ''' </summary>
        ''' <param name="Connection">SQL-Connection für das Einlesen der Daten</param>
        ''' <param name="SelectSQL">SQL-Statement für das Selektieren der Daten</param>
        ''' <remarks></remarks>
        Public Sub New(Connection As SQLite.SQLiteConnection, _
                       SelectSQL As String, _
                       IDColumnName As String, _
                       DisplayColumnName As String)
            Me.New(Connection, SelectSQL)
            Me.DisplayMember = DisplayColumnName
            Me.IDColumnName = IDColumnName
            Me.Reload()
        End Sub

        ' ******************************** Operations ********************************
        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing _
                        AndAlso (Not (components) Is Nothing)) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Protected Overrides Sub OnDropDown(ByVal e As EventArgs)
            MyBase.OnDropDown(e)
            DoDropDown()
        End Sub

        Private Sub DoDropDown()
            If Not _dropdown.Visible Then
                Dim rect As Rectangle = RectangleToScreen(Me.ClientRectangle)
                _dropdown.Location = New Point(rect.X, (rect.Y + Me.Size.Height))
                Dim count As Integer = _dropdown.List.Items.Count
                If (count > Me.MaxDropDownItems) Then
                    count = Me.MaxDropDownItems
                ElseIf (count = 0) Then
                    count = 1
                End If
                _dropdown.Size = New Size(Me.Size.Width, ((_dropdown.List.ItemHeight * count) _
                                + 2))
                _dropdown.Show(Me)
            End If
        End Sub

        Protected Overrides Sub OnDropDownClosed(ByVal e As EventArgs)
            ' Call the handlers for this event only if the call comes from our code - NOT the framework's!
            ' NOTE: that is because the events were being fired in a wrong order, due to the actual dropdown list
            '       of the ComboBox which lies underneath our dropdown and gets involved every time.
            If (TypeOf e Is Dropdown.CCBoxEventArgs) Then
                MyBase.OnDropDownClosed(e)
            End If
        End Sub

        Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
            If (e.KeyCode = Keys.Down) Then
                ' Signal that the dropdown is "down". This is required so that the behaviour of the dropdown is the same
                ' when it is a result of user pressing the Down_Arrow (which we handle and the framework wouldn't know that
                ' the list portion is down unless we tell it so).
                ' NOTE: all that so the DropDownClosed event fires correctly!                
                OnDropDown(Nothing)
            End If
            ' Make sure that certain keys or combinations are not blocked.
            e.Handled = (Not e.Alt _
                        AndAlso (Not (e.KeyCode = Keys.Tab) _
                        AndAlso Not ((e.KeyCode = Keys.Left) _
                        OrElse ((e.KeyCode = Keys.Right) _
                        OrElse ((e.KeyCode = Keys.Home) _
                        OrElse (e.KeyCode = Keys.End))))))
            MyBase.OnKeyDown(e)
        End Sub

        Protected Overrides Sub OnKeyPress(ByVal e As KeyPressEventArgs)
            e.Handled = True
            MyBase.OnKeyPress(e)
        End Sub

        Public Function GetItemChecked(ByVal index As Integer) As Boolean
            If ((index < 0) _
                        OrElse (index > Items.Count)) Then
                Throw New ArgumentOutOfRangeException("index", "value out of range")
            Else
                Return _dropdown.List.GetItemChecked(index)
            End If
        End Function

        Public Sub SetItemChecked(ByVal index As Integer, ByVal isChecked As Boolean)
            If ((index < 0) OrElse (index > Items.Count)) Then
                Throw New ArgumentOutOfRangeException("index", "value out of range")
            Else
                _dropdown.List.SetItemChecked(index, isChecked)
                ' Need to update the Text.
                Me.Text = _dropdown.GetCheckedItemsStringValue
            End If
        End Sub

        Public Function GetItemCheckState(ByVal index As Integer) As CheckState
            If ((index < 0) _
                        OrElse (index > Items.Count)) Then
                Throw New ArgumentOutOfRangeException("index", "value out of range")
            Else
                Return _dropdown.List.GetItemCheckState(index)
            End If
        End Function

        Public Sub SetItemCheckState(ByVal index As Integer, ByVal state As CheckState)
            If ((index < 0) _
                        OrElse (index > Items.Count)) Then
                Throw New ArgumentOutOfRangeException("index", "value out of range")
            Else
                _dropdown.List.SetItemCheckState(index, state)
                ' Need to update the Text.
                Me.Text = _dropdown.GetCheckedItemsStringValue
            End If
        End Sub

        ''' <summary>
        ''' Setzt den CheckState der übergebenen ID-Liste auf ListedIDstate und alle anderen auf UnlistedIDState
        ''' </summary>
        ''' <param name="IDList">Kommaseparierte Liste aller zu setzenden IDs</param>
        ''' <param name="ListedIDstate">CheckState für alle IDs der Liste</param>
        ''' <param name="UnlistedIDState">CheckState für alle IDs, die nicht in der Liste enthalten sind. Wenn Indeterminate, werden diese nicht verändert.</param>
        Public Sub SetItemListCheckState(ByVal IDList As String, _
                                         ByVal ListedIDstate As CheckState, _
                                         Optional ByVal UnlistedIDState As CheckState = CheckState.Indeterminate)
            IDList = "," & IDList.Replace(" ", "") & ","
            For Index As Integer = 0 To Me.Items.Count - 1
                If IDList.IndexOf("," & CallByName(Me.Items(Index), _IDColumnName, CallType.Get) & ",") >= 0 Then
                    ' Element ist in Liste enthalten
                    _dropdown.List.SetItemCheckState(Index, ListedIDstate)
                ElseIf UnlistedIDState <> CheckState.Indeterminate Then
                    ' Element ist nicht enthalten
                    _dropdown.List.SetItemCheckState(Index, UnlistedIDState)
                End If
            Next
            Me.Text = _dropdown.GetCheckedItemsStringValue

        End Sub

        ''' <summary>
        ''' Liefert die IDs aller angehakten Items zurück
        ''' </summary>
        ''' <returns>Kommaseparierte Liste aller angehakten IDs</returns>
        Public Function GetCheckedItemsIDs() As String
            Return _dropdown.GetCheckedItemsIDs
        End Function

        ''' <summary>
        ''' Initialisiert das Control hinsichtlich der Datenanbindung
        ''' </summary>
        ''' <param name="Connection">SQL-Connection</param>
        ''' <param name="SelectSQL">SQL-Select-Statement für die Selektion der Items</param>
        ''' <param name="IDColumnName">Spaltenname für IDs</param>
        ''' <param name="DisplayColumnName">Spaltenname für anzuzeigende Bezeichnungen</param>
        Public Sub Initialize(Connection As Data.SQLite.SQLiteConnection, _
                              SelectSQL As String, _
                              Optional IDColumnName As String = "ID", _
                              Optional DisplayColumnName As String = "Bezeichnung", _
                              Optional FirstLineIsCheckAll As Boolean = False)
            Me.Connection = Connection
            Me.SelectSQL = SelectSQL
            Me.IDColumnName = IDColumnName
            Me.DisplayMember = DisplayColumnName
            Me.FirstLineIsCheckAll = FirstLineIsCheckAll
        End Sub



        ''' <summary>
        ''' Lädt die Einträge des Controls anhand der Eigenschaften SelectSQL und Connection neu
        ''' </summary>
        ''' <param name="PreserveSelected">Wenn TRUE, bleiben bereits angehakte Werte (wenn weiterhin vorhanden) angehakt</param>
        Public Sub Reload(Optional PreserveSelected As Boolean = True)
            If _Cnn IsNot Nothing AndAlso _Cnn.State = ConnectionState.Open _
                AndAlso _SelectSQL IsNot Nothing AndAlso _SelectSQL.Length > 0 Then
                Try
                    Dim SelIDs As String = ""
                    If PreserveSelected Then
                        SelIDs = Me.GetCheckedItemsIDs
                    End If
                    Dim DBO As DBObjects = New DBObjects(SelectSQL, Connection)
                    Me.Items.Clear()
                    For Each Row As DataRow In DBO.DataTable.Rows
                        Dim Item As New CheckedComboBoxItem(Row(Me.IDColumnName), Row(Me.DisplayMember))
                        Me.Items.Add(Item)
                    Next
                    DBO.Dispose()
                    If PreserveSelected Then
                        Me.SetItemListCheckState(SelIDs, CheckState.Checked, CheckState.Unchecked)
                    End If
                Catch ex As Exception

                End Try
            End If
            ' Passt eigentlich nicht richtig, ist aber praktikabel: Font neu setzen!
            _dropdown.Font = Me.Font
        End Sub

    End Class

End Namespace

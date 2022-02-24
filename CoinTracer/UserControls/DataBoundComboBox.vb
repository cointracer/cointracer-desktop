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

Public Class DataBoundComboBoxUnsavedChangesEventArgs
    Inherits EventArgs
    Public Property OriginalValue As String
    Public Property ActualValue As String
    Public Property AlreadyPresent As Boolean
    Public Sub New(OriginalValue As String, _
                   ActualValue As String, _
                   AlreadyPresent As Boolean)
        Me.OriginalValue = OriginalValue
        Me.ActualValue = ActualValue
        Me.AlreadyPresent = AlreadyPresent
    End Sub
End Class

''' <summary>
''' ComboBox-Element mit Erweiterungen, um gleichzeitig Einträge in einer Tabelle pflegen zu können
''' </summary>
Public Class DataBoundComboBox
    Inherits ComboBox

    Public Event UnsavedChanges As EventHandler(Of DataBoundComboBoxUnsavedChangesEventArgs)

    Private _KeyPressed As Boolean

#Region "Properties"

    Private _IDColumn As String
    Public Property IDColumnName() As String
        Get
            Return _IDColumn
        End Get
        Set(ByVal value As String)
            ValueMember = value
            _IDColumn = value
        End Set
    End Property

    Private _DisplayColumn As String
    Public Property DisplayColumnName() As String
        Get
            Return _DisplayColumn
        End Get
        Set(ByVal value As String)
            DisplayMember = value
            _DisplayColumn = value
        End Set
    End Property

    Private _OriginalLabel As String
    Private _TypedLabel As String

    Public ReadOnly Property LabelSticky() As Boolean
        Get
            Return Text <> "" AndAlso _OriginalLabel <> _TypedLabel AndAlso _OriginalLabel <> ""
        End Get
    End Property

    Private _CustomSticky As Boolean
    Public Property Sticky() As Boolean
        Get
            Return (Not Initializing AndAlso (LabelSticky OrElse _CustomSticky))
        End Get
        Set(value As Boolean)
            If Not Initializing Then
                _CustomSticky = value
                DisEnableButtons()
            End If
        End Set
    End Property

    Private _DeleteButton As Button
    Public ReadOnly Property DeleteButton() As Button
        Get
            Return _DeleteButton
        End Get
    End Property

    Private _SaveButton As Button
    Public ReadOnly Property SaveButton() As Button
        Get
            Return _SaveButton
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

    Private _cnn As Data.SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As Data.SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
    End Property

    ''' <summary>
    ''' Prüft, ob der aktuell eingegebene Text in der Datenquelle bereits vorhanden ist.
    ''' </summary>
    ''' <returns>True, wenn der aktuell angezeigte Text vorhanden ist, sonst False</returns>
    Public ReadOnly Property LabelInDataSource() As Boolean
        Get
            ' Prüfen, ob es den aktuellen Eintrag bereits gibt
            Dim Present As Boolean = (Text.Trim <> "")
            If TryCast(DataSource, DataTable) IsNot Nothing Then
                Dim DV As New DataView(DirectCast(DataSource, DataTable),
                                       String.Format("{0} = '{1}'", _DisplayColumn, MaskedText),
                                       "", DataViewRowState.OriginalRows)
                Present = Present And DV.Count > 0
            Else
                Present = Present And Items.Contains(Text.Trim)
            End If
            Return Present
        End Get
    End Property

    ''' <summary>
    ''' Liefert den aktuell angezeigten String SQL-konform zurück (Hochkommata und Quotes maskiert)
    ''' </summary>
    Public ReadOnly Property MaskedText() As String
        Get
            Return Text.Trim.Replace("'", "\'").Replace("""", "\'")
        End Get
    End Property

    Private _Initializing As Boolean
    ''' <summary>
    ''' Determines if the control is in the process of initialization. This flag is needed to prevent recursive event calls.
    ''' </summary>
    ''' <returns>True in case of initialisation. False otherwise.</returns>
    Public Property Initializing() As Boolean
        Get
            Return _Initializing
        End Get
        Set(ByVal value As Boolean)
            _Initializing = value
        End Set
    End Property

#End Region

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _OriginalLabel = ""
        _CustomSticky = False
        _DeleteButton = Nothing
        _SaveButton = Nothing
        _Initializing = True
        _KeyPressed = False

    End Sub

    ''' <summary>
    ''' Initializes the control
    ''' </summary>
    Public Sub Initialize(Connection As Data.SQLite.SQLiteConnection,
                          SelectSQL As String,
                          Optional IDColumnName As String = "ID",
                          Optional DisplayColumnName As String = "Bezeichnung",
                          Optional SaveButton As Button = Nothing,
                          Optional DeleteButton As Button = Nothing)
        _cnn = Connection
        _SelectSQL = SelectSQL
        Initializing = True
        _KeyPressed = False
        Me.IDColumnName = IDColumnName
        Me.DisplayColumnName = DisplayColumnName
        Reload("")
        Initializing = True
        If SaveButton IsNot Nothing Then
            _SaveButton = SaveButton
        End If
        If DeleteButton IsNot Nothing Then
            _DeleteButton = DeleteButton
        End If
        If Items?.Count > 0 AndAlso SelectedIndex >= 0 Then
            _OriginalLabel = Items(SelectedIndex)(DisplayColumnName).ToString
        Else
            _OriginalLabel = ""
        End If
        _TypedLabel = _OriginalLabel
        Sticky = False
        Initializing = False
    End Sub

#Region "Methods"

    ''' <summary>
    ''' Lädt die Datenquelle des Controls neu
    ''' </summary>
    Public Sub Reload(Optional SelectValue As String = "")
        If Connection IsNot Nothing AndAlso SelectSQL <> "" Then
            Initializing = True
            _KeyPressed = False
            Dim DBO As DBObjects = New DBObjects(SelectSQL, Connection)
            DataSource = DBO.DataTable
            DBO.Dispose()
            DisplayMember = _DisplayColumn
            ValueMember = _IDColumn
            _CustomSticky = False
            If SelectValue <> "" Then
                Try
                    For i As Integer = 0 To Items.Count
                        If Items(i)(_DisplayColumn).ToString = SelectValue Then
                            SelectedItem = Items(i)
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                    Debug.Print("DataBoundComboBox_Reload")
                End Try
            End If
            Initializing = False
        End If
    End Sub

#End Region

#Region "Internal routines"

    ''' <summary>
    ''' Setzt Enabled-Status der verbundenen Buttons
    ''' </summary>
    Private Sub DisEnableButtons()
        If _SaveButton IsNot Nothing Then
            _SaveButton.Enabled = Sticky
        End If
        If _DeleteButton IsNot Nothing Then
            _DeleteButton.Enabled = SelectedIndex > 0
        End If
    End Sub

#End Region

#Region "Control events"

    Private Sub DataBoundComboBox_TextChanged(sender As Object, e As EventArgs) Handles Me.TextChanged
        If _KeyPressed Then
            _KeyPressed = False
            _TypedLabel = Text.Trim
            DisEnableButtons()
        End If
    End Sub

    Private Sub DataBoundComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Me.SelectedIndexChanged
        If Not Initializing Then
            If Sticky Then
                RaiseEvent UnsavedChanges(Me, New DataBoundComboBoxUnsavedChangesEventArgs(_TypedLabel, Text, LabelInDataSource))
            End If
            _OriginalLabel = Text
            _TypedLabel = _OriginalLabel
            Sticky = False
        End If
    End Sub

    Private Sub DataBoundComboBox_Click(sender As Object, e As EventArgs) Handles Me.Click
        _TypedLabel = Text
    End Sub

    Private Sub DataBoundComboBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        ' Keep track of manually entered key strokes
        _KeyPressed = True
    End Sub

#End Region


End Class

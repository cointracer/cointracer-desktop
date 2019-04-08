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

Public Class DashboardDataGridView
    Inherits DataGridView

    Private _MaxCols As Int16 = 0

    Private _Initialized As Boolean

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        'Benutzerdefinierten Zeichnungscode hier einfügen
        If Not _Initialized Then
            ' Kontextmenü ggf. erstellen & initialisieren
            If Me.ContextMenuStrip Is Nothing Then
                Me.ContextMenuStrip = New ContextMenuStrip()
            End If
            ContextMenuStripInit()
            _Initialized = True
        End If
    End Sub

    Private _DB As DBHelper
    Public Sub LinkDatabase(ByRef CointracerDB As DBHelper)
        _DB = CointracerDB
    End Sub

    Private _TPCrtl As TimePeriodControl
    Public Sub LinkTimePeriodControl(ByRef TimePeriodControl As TimePeriodControl, Optional ColumnName As String = "Zeitpunkt")
        _TPCrtl = TimePeriodControl
        _ColumnName = ColumnName
    End Sub

    Public ReadOnly Property TimePeriodControl() As TimePeriodControl
        Get
            Return _TPCrtl
        End Get
    End Property

    Private _ColumnName As String = ""
    Public Property ColumnName() As String
        Get
            Return _ColumnName
        End Get
        Set(ByVal value As String)
            _ColumnName = value
        End Set
    End Property


    Private _SelectSQL As String = ""
    Public Property SelectSQL() As String
        Get
            Return _SelectSQL
        End Get
        Set(ByVal value As String)
            _SelectSQL = value.Trim
        End Set
    End Property

    Private _WhereSQL As String = ""
    Public Property WhereSQL() As String
        Get
            Return _WhereSQL
        End Get
        Set(ByVal value As String)
            _WhereSQL = value.Trim
        End Set
    End Property

    Private _GroupBySQL As String = ""
    Public Property GroupBySQL() As String
        Get
            Return _GroupBySQL
        End Get
        Set(ByVal value As String)
            _GroupBySQL = value.Trim
        End Set
    End Property

    Private _OrderBySQL As String = ""
    Public Property OrderBySQL() As String
        Get
            Return _OrderBySQL
        End Get
        Set(ByVal value As String)
            _OrderBySQL = value.Trim
        End Set
    End Property


    Public Sub SetSQL(ByVal SelectSQL As String, _
                      ByVal WhereSQL As String, _
                      ByVal GroupBySQL As String, _
                      ByVal OrderBySQL As String)
        Me.SelectSQL = SelectSQL
        Me.WhereSQL = WhereSQL
        Me.GroupBySQL = GroupBySQL
        Me.OrderBySQL = OrderBySQL
    End Sub

    Private _BS As BindingSource

    Private Sub SetBindingSource(DataSource As Object, Optional MemberName As String = "")
        _BS = New BindingSource(DataSource, MemberName)
        Me.DataSource = _BS
    End Sub

    ''' <summary>
    ''' Lädt die Daten erneut in das DataGridView
    ''' </summary>
    Public Sub Reload()
        Dim DS As DataSet
        Dim i As Integer
        Dim Tmp As String = ""
        If _DB IsNot Nothing AndAlso _SelectSQL.Length > 0 Then
            If _TPCrtl IsNot Nothing AndAlso _ColumnName.Length > 0 Then
                If _TPCrtl.DateSql.Length > 0 Then
                    Tmp = " and " & _ColumnName & " " & _TPCrtl.DateSql
                End If
            End If
            Try
                InitColumns()
                DS = _DB.DataSet(SelectSQL & " where (" & WhereSQL & ") " & Tmp & " group by " & GroupBySQL & " order by " & OrderBySQL)
                SetBindingSource(DS, DS.Tables(0).TableName)
                ' Spalten binden
                For i = 0 To _MaxCols - 1
                    Me.Columns(i).DataPropertyName = DS.Tables(0).Columns(i).ColumnName
                    ' Besonderheit: Währung in Spaltentitel bei Kauf- und Verkaufspreis-Spalten
                    If i > 6 Then
                        Me.Columns(i).Name = String.Format("Ø {0} {1}", IIf(i = 7, "Kauf", "Verkauf"), IIf(DS.Tables(0).Columns(i).ColumnName.Contains("EUR"), "€", "$"))
                    End If
                Next
                SetCellFormats()
            Catch ex As Exception
                DefaultErrorHandler(ex)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Formatiert Zellen/Zeilen anhand der Information IstFiat
    ''' </summary>
    Private Sub SetCellFormats()
        Dim r As Int16
        Dim FormatString As String
        For r = 0 To Me.Rows.Count - 1
            If Me.Rows(r).Cells(0).Value Then
                FormatString = "N2"
                ' Besonderheit: An- und Verkaufspreise (ab Spalte 7) hier ausblenen
                If Me.Columns.Count > 6 Then
                    For c As Integer = 7 To Me.Columns.Count - 1
                        Me.Rows(r).Cells(c).Value = DBNull.Value
                    Next
                End If
            Else
                FormatString = "#,###,##0.00000000"
                ' Besonderheit: An- und Verkaufspreise (ab Spalte 7) nur zweistellig
                If Me.Columns.Count > 6 Then
                    For c As Integer = 7 To Me.Columns.Count - 1
                        Me.Rows(r).Cells(c).Style.Format = "#,###,##0.00"
                    Next
                End If
            End If
            Me.Rows(r).DefaultCellStyle.Format = FormatString
        Next r
    End Sub

    Protected Overrides ReadOnly Property DefaultSize() As Size
        Get
            Return New Size(448, 145)
        End Get
    End Property

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _DB = Nothing
        _TPCrtl = Nothing

        ' Sonstige Eigenschaften
        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.AllowUserToResizeRows = True
        Me.EditMode = DataGridViewEditMode.EditProgrammatically
        Me.RowHeadersVisible = False
        Me.AutoGenerateColumns = False

    End Sub

    ''' <summary>
    ''' Anlegen der Spalten (muss nicht unbedingt dynamisch passieren, aber so what...)
    ''' </summary>
    Private Sub InitColumns()
        ' Exit Sub
        ' Spalten initialisieren
        If Me.Columns.Count = 0 Then
            Dim Col As DataGridViewColumn = _
                New DataGridViewTextBoxColumn
            With Col
                .Name = "IstFiat"
                .Visible = False
                .Width = 4
                .FillWeight = 4
                .Resizable = DataGridViewTriState.False
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                Me.Columns.Add(Col)
            End With
            Dim ColAF As DataGridViewAutoFilterTextBoxColumn = New DataGridViewAutoFilterTextBoxColumn
            With ColAF
                .Name = "Art"
                .Width = 30
                .FillWeight = 30
                .Resizable = DataGridViewTriState.True
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .CaptionAllValues = "(Alles auswählen)"
                .CaptionBlankValues = "(Leere)"
                .CaptionCancelButton = "Abbrechen"
                Me.Columns.Add(ColAF)
            End With
            ColAF = New DataGridViewAutoFilterTextBoxColumn
            With ColAF
                .Name = "Bezeichnung"
                .Width = 30
                .FillWeight = 30
                .Resizable = DataGridViewTriState.True
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .DefaultCellStyle.Font = New Font(MyBase.Font, FontStyle.Bold)
                .CaptionAllValues = "(Alles auswählen)"
                .CaptionBlankValues = "(Leere)"
                .CaptionCancelButton = "Abbrechen"
                Me.Columns.Add(ColAF)
            End With
            ColAF = New DataGridViewAutoFilterTextBoxColumn
            With ColAF
                .Name = "Plattform"
                .Width = 30
                .FillWeight = 30
                .Resizable = DataGridViewTriState.True
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .DefaultCellStyle.Font = New Font(MyBase.Font, FontStyle.Bold)
                .CaptionAllValues = "(Alles auswählen)"
                .CaptionBlankValues = "(Leere)"
                .CaptionCancelButton = "Abbrechen"
                .Visible = False
                Me.Columns.Add(ColAF)
            End With
            Col = New DataGridViewTextBoxColumn
            With Col
                .Name = "Zugang"
                .Width = 90
                .FillWeight = 90
                .Resizable = DataGridViewTriState.NotSet
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Me.Columns.Add(Col)
            End With
            Col = New DataGridViewTextBoxColumn
            With Col
                .Name = "Abgang"
                .Width = 90
                .FillWeight = 90
                .Resizable = DataGridViewTriState.NotSet
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Me.Columns.Add(Col)
            End With
            Col = New DataGridViewTextBoxColumn
            With Col
                .Name = "Bestand"
                .Width = 90
                .FillWeight = 90
                .Resizable = DataGridViewTriState.NotSet
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                .DefaultCellStyle.Font = New Font(MyBase.Font, FontStyle.Bold)
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Me.Columns.Add(Col)
            End With

            For Each ColName As String In New String() {"Ø Kauf €", "Ø Verkauf €"}
                Col = New DataGridViewTextBoxColumn
                With Col
                    .Name = ColName
                    .Width = 15
                    .FillWeight = 5
                    .Resizable = DataGridViewTriState.True
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                    .SortMode = DataGridViewColumnSortMode.NotSortable
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    .DefaultCellStyle.Format = "#,###,##0.00"
                    Me.Columns.Add(Col)
                End With
            Next

            _MaxCols = Me.Columns.Count
        ElseIf Me.Columns.Count > _MaxCols Then
            Dim c As Int16
            For c = Me.Columns.Count - 1 To _MaxCols Step -1
                Me.Columns(c).Visible = False
            Next c
        End If
    End Sub

    ''' <summary>
    ''' Initialisiert die Standard-Menü-Items des Kontextmenüs
    ''' </summary>
    Private Sub ContextMenuStripInit()
        With Me.ContextMenuStrip
            ' Prüfen, ob Default-Kontextmenüeinträge schon vorhanden sind
            If .Items.ContainsKey("mnuItmCopyCell") Then
                Exit Sub
            End If
            If .Items.Count > 0 Then
                .Items.Add("-")
            End If
            Dim mnuItem As New ToolStripMenuItem() With {.Text = "Gesamte Tabelle kopieren", _
                                                         .Name = "mnuItmCopyTable", _
                                                         .Image = DirectCast(My.Resources.edit_copytable_icon_16x16, Bitmap)}
            AddHandler mnuItem.Click, AddressOf mnuItmCopyTable_Click
            .Items.Add(mnuItem)
        End With
    End Sub

    Private Sub mnuItmCopyTable_Click(sender As Object, e As EventArgs)
        DataGridViewToClipboard(Me)
    End Sub

    Private Sub DashboardDataGridView_SelectionChanged(sender As Object, e As EventArgs) Handles Me.SelectionChanged
        Me.ClearSelection()
    End Sub
End Class

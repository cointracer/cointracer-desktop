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

Imports CoinTracer.DBHelper

Public Class frmGetImportData_CnP_Vircurex

    Public Class ImportLine
        Private _Year As Integer
        Public Property Year() As Integer
            Get
                Return _Year
            End Get
            Set(ByVal value As Integer)
                _Year = value
            End Set
        End Property
        Private _DisplayContent As String
        Public Property DisplayContent() As String
            Get
                Return _DisplayContent
            End Get
            Set(ByVal value As String)
                _DisplayContent = value
            End Set
        End Property
        Private _LineContent As String
        Public Property LineContent() As String
            Get
                Return _LineContent
            End Get
            Set(ByVal value As String)
                _LineContent = value
            End Set
        End Property
        Public Sub New(ByVal Year As Integer, ByVal LineContent As String, Optional ByVal DisplayContent As String = "")
            _Year = Year
            _LineContent = LineContent
            If DisplayContent.Length > 0 Then
                _DisplayContent = DisplayContent
            Else
                _DisplayContent = LineContent
            End If
        End Sub
    End Class

    Private _ImportLines() As ImportLine
    Private _RecurseFlag As Boolean

    Private _ImportPlatform As PlatformManager.Platforms
    Public Property ImportPlatform() As PlatformManager.Platforms
        Get
            Return _ImportPlatform
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _ImportPlatform = value
        End Set
    End Property

    ''' <summary>
    ''' Liefert den um die jeweiligen Jahreszahlen ergänzten Content zurück.
    ''' </summary>
    Public ReadOnly Property ImportContent() As String
        Get
            Dim Content As String = ""
            Dim Line As String
            For i As Integer = 0 To grdImportlines.RowCount - 1
                Line = grdImportlines.Rows(i).Cells(2).Value.ToString
                If IsDate(Line.Substring(0, 8) & grdImportlines.Rows(i).Cells(0).Value.ToString) Then
                    Content &= Line.Insert(8, grdImportlines.Rows(i).Cells(0).Value.ToString & " ") & Environment.NewLine
                Else
                    Content &= Line & Environment.NewLine
                End If
            Next
            Return Content
        End Get
    End Property


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _ImportPlatform = PlatformManager.Platforms.Unknown

    End Sub

    ''' <summary>
    ''' Initialisiert das DataGridView mit den Importzeilen inkl. zugehöriger Jahresangaben
    ''' </summary>
    ''' <param name="ContentString"></param>
    Public Sub InitContent(ByRef ContentString As String)
        Try
            ' Importzeilen inkl. Jahreszahlen aufbereiten
            Dim LastDate As Date = Today
            Dim Lines() As String = ContentString.Split(Environment.NewLine)
            ReDim _ImportLines(Lines.Length - 1)
            For i As Integer = 0 To Lines.Length - 1
                Dim Items() As String = Lines(i).Split(vbTab)
                Dim Subitems() As String = Items(0).Split(" ")
                If Subitems.Length = 3 AndAlso IsDate(String.Format("{0} {1} {2}", Subitems(0), Subitems(1).Replace("Mär", "März"), LastDate.Year)) Then
                    If CDate(String.Format("{0} {1} {2}", Subitems(0), Subitems(1).Replace("Mär", "März"), LastDate.Year)) > LastDate Then
                        LastDate = CDate(String.Format("{0} {1} {2}", Subitems(0), Subitems(1).Replace("Mär", "März"), LastDate.Year - 1))
                    Else
                        LastDate = CDate(String.Format("{0} {1} {2}", Subitems(0), Subitems(1).Replace("Mär", "März"), LastDate.Year))
                    End If
                End If
                _ImportLines(i) = New ImportLine(LastDate.Year, Lines(i), Lines(i).Replace(vbTab, " | "))
            Next
            ' DataGridView anzeigen
            With grdImportlines
                .DataSource = Nothing
                .Rows.Clear()
                Dim CmbCol As DataGridViewComboBoxColumn = DirectCast(.Columns("colYear"), DataGridViewComboBoxColumn)
                CmbCol.Items.Clear()
                For i = Today.Year To 2011 Step -1
                    CmbCol.Items.Add(i)
                Next
                .DataSource = _ImportLines
                CmbCol.DisplayMember = "Year"
                .ReadOnly = False
                .EditMode = DataGridViewEditMode.EditOnEnter
                .Columns("colDisplayLine").ReadOnly = True
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub


    Private Sub grdImportlines_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles grdImportlines.CellValueChanged
        If Not _RecurseFlag AndAlso e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            Try
                ' Nach unten hin alle Jahreszahlen anpassen
                Dim SubItems() As String = grdImportlines.Rows(e.RowIndex).Cells(2).Value.ToString.Replace("Mär", "März").Split(" ")
                Dim LastDate As Date

                If SubItems.Length > 1 AndAlso IsDate(String.Format("{0} {1} {2}", SubItems(0), SubItems(1), grdImportlines.Rows(e.RowIndex).Cells(0).Value.ToString)) Then
                    LastDate = CDate(String.Format("{0} {1} {2}", SubItems(0), SubItems(1), grdImportlines.Rows(e.RowIndex).Cells(0).Value.ToString))
                Else
                    LastDate = CDate(String.Format("31 Dec {0}", grdImportlines.Rows(e.RowIndex).Cells(0).Value.ToString))
                End If

                For i As Integer = e.RowIndex + 1 To grdImportlines.RowCount - 1
                    SubItems = grdImportlines.Rows(i).Cells(2).Value.ToString.Replace("Mär", "März").Split(" ")
                    If SubItems.Length > 1 AndAlso IsDate(String.Format("{0} {1} {2}", SubItems(0), SubItems(1), grdImportlines.Rows(i).Cells(0).Value.ToString)) Then
                        Dim CurYear As Integer = LastDate.Year
                        If CDate(String.Format("{0} {1} {2}", SubItems(0), SubItems(1), CurYear)) > LastDate Then
                            CurYear -= 1
                        End If
                        If CurYear <> grdImportlines.Rows(i).Cells(0).Value Then
                            _RecurseFlag = True
                            grdImportlines.Rows(i).Cells(0).Value = CurYear
                            _RecurseFlag = False
                        End If
                        LastDate = CDate(String.Format("{0} {1} {2}", SubItems(0), SubItems(1), CurYear))
                    End If
                Next
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub grdImportlines_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles grdImportlines.CurrentCellDirtyStateChanged
        If grdImportlines.IsCurrentCellDirty Then
            grdImportlines.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub
End Class

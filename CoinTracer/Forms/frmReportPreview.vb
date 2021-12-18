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

Imports Microsoft.Reporting.WinForms
Imports CoinTracer.CTReport

Public Class frmReportPreview

    Private Class RenderingExtensionListboxItem
        Public RenderingExtension As RenderingExtension
        Public Sub New(RenderingExtension As RenderingExtension)
            Me.RenderingExtension = RenderingExtension
        End Sub
        Public Overrides Function ToString() As String
            Return RenderingExtension.LocalizedName
        End Function
    End Class

    Private DoNotClose As Boolean

    Private _CtRp As CTReport

    Private _MyRDS As ReportDataSource

    Private Sub ConfigReport()
        Dim ReportTemplate As String = Application.StartupPath & "\Reports\"
        Dim Table As DataTable
        Dim TimeSpan As String

        _CtRp = frmMain.Report

        ' Select appropriate report template
        'If _CtRp.HasUsdTrades Then
        '    If _CtRp.TradeSelection = ReportTradeSelections.TaxableOnly Then
        '        ReportTemplate &= "GainingsReportUSD.rdlc"
        '    Else
        '        ReportTemplate &= "GainingsReportUSDAll.rdlc"
        '    End If
        'Else
        '    If _CtRp.TradeSelection = ReportTradeSelections.TaxableOnly Then
        '        ReportTemplate &= "GainingsReport.rdlc"
        '    Else
        '        ReportTemplate &= "GainingsReportAll.rdlc"
        '    End If
        'End If
        ReportTemplate &= "GainingsReport2.rdlc"

        ' Daten laden
        Table = _CtRp.DataTable.Copy
        AdjustColumnNames(DirectCast(Table, DataTable))
        _MyRDS = New ReportDataSource("ReportDataSet", Table)

        ' Zeitraum-String formatieren
        With _CtRp.Parameters
            If .FromDate <> DATENULLVALUE Then
                If .ToDate <> DATENULLVALUE Then
                    TimeSpan = String.Format(My.Resources.MyStrings.timespanFromTo, .FromDate.ToString("dd.MM.yyyy"), .ToDate.ToString("dd.MM.yyyy"))
                Else
                    TimeSpan = String.Format(My.Resources.MyStrings.timespanFrom, .FromDate.ToString("dd.MM.yyyy"))
                End If
            ElseIf .ToDate <> DATENULLVALUE Then
                TimeSpan = String.Format(My.Resources.MyStrings.timespanTo, .ToDate.ToString("dd.MM.yyyy"))
            Else
                TimeSpan = My.Resources.MyStrings.totalInHyphens
            End If
        End With

        ' Report-Viewer aktualisieren
        With ReportViewer1
            .Reset()
            .LocalReport.DataSources.Clear()
            .LocalReport.ReportPath = ReportTemplate
            .LocalReport.SetParameters(New ReportParameter("UserName", _CtRp.Parameters.Name))
            .LocalReport.SetParameters(New ReportParameter("TaxNumber", _CtRp.Parameters.TaxNumber))
            .LocalReport.SetParameters(New ReportParameter("ReportComment", _CtRp.Parameters.ReportComment))
            .LocalReport.SetParameters(New ReportParameter("ReportType", CInt(_CtRp.ReportType)))
            .LocalReport.SetParameters(New ReportParameter("TimeSpan", TimeSpan))
            .LocalReport.SetParameters(New ReportParameter("Application", Application.ProductName & " " & Application.ProductVersion))
            .LocalReport.DataSources.Add(_MyRDS)
            .RefreshReport()
        End With
        ' Exportformate in Listbox
        Dim RendExts() As RenderingExtension
        RendExts = ReportViewer1.LocalReport.ListRenderingExtensions
        For Each RE As RenderingExtension In RendExts
            If Not RE.LocalizedName.Contains("2003") Then
                cbxExportTypes.Items.Add(New RenderingExtensionListboxItem(RE))
            End If
        Next
        cbxExportTypes.SelectedIndex = My.Settings.ReportLastRenderingExtension

    End Sub

    ''' <summary>
    ''' Ändert die Spaltennamen einer Tabelle so, dass keine Leerzeichen mehr enthalten sind. Diese machen leider Probleme im Report, daher diese unschöne Lösung...
    ''' </summary>
    Private Sub AdjustColumnNames(ByRef Table As DataTable)
        For i As Integer = 0 To Table.Columns.Count - 1
            Table.Columns(i).ColumnName = Table.Columns(i).ColumnName.Replace(" ", "").Replace("-", "")
        Next
    End Sub

    Private Sub frmReportPreview_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If DoNotClose Then
            e.Cancel = True
            DoNotClose = False
        End If
    End Sub

    Private Sub frmReportPreview_Load(sender As Object, e As EventArgs) Handles Me.Load
        ConfigReport()
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        DoNotClose = False
    End Sub

    Private Sub cmdPrint_Click(sender As Object, e As EventArgs) Handles cmdPrint.Click
        DoNotClose = True
        If ReportViewer1.PrintDialog() <> Windows.Forms.DialogResult.Cancel Then
            frmMain.ShowDisclaimer()
        End If
    End Sub


    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        DoNotClose = True
        My.Settings.ReportLastRenderingExtension = cbxExportTypes.SelectedIndex
        Try
            With ReportViewer1
                .LocalReport.DisplayName = String.Format(My.Resources.MyStrings.exportReportFileName, Now.ToString("yyyy-MM-dd_HH.mm"))
                Dim Result As DialogResult = .ExportDialog(DirectCast(cbxExportTypes.SelectedItem, RenderingExtensionListboxItem).RenderingExtension)
                If Result <> DialogResult.Cancel Then
                    frmMain.ShowDisclaimer()
                End If
            End With
        Catch ex As Exception
            DefaultErrorHandler(ex)
        End Try
    End Sub

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        DoNotClose = False
        My.Settings.ReportLastRenderingExtension = cbxExportTypes.SelectedIndex
    End Sub
End Class

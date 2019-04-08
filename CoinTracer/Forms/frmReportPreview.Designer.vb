<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmReportPreview
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.cmdPrint = New System.Windows.Forms.Button()
        Me.cmdExport = New System.Windows.Forms.Button()
        Me.cbxExportTypes = New System.Windows.Forms.ComboBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Location = New System.Drawing.Point(1008, 569)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(131, 28)
        Me.cmdOK.TabIndex = 5
        Me.cmdOK.Text = "Schließen"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "CoinTracer.GainingsReportDetailed.rdlc"
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ShowBackButton = False
        Me.ReportViewer1.ShowDocumentMapButton = False
        Me.ReportViewer1.ShowExportButton = False
        Me.ReportViewer1.ShowParameterPrompts = False
        Me.ReportViewer1.ShowRefreshButton = False
        Me.ReportViewer1.ShowStopButton = False
        Me.ReportViewer1.Size = New System.Drawing.Size(1147, 559)
        Me.ReportViewer1.TabIndex = 1
        '
        'cmdPrint
        '
        Me.cmdPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPrint.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdPrint.Image = Global.CoinTracer.My.Resources.Resources.ct_print
        Me.cmdPrint.Location = New System.Drawing.Point(856, 569)
        Me.cmdPrint.Name = "cmdPrint"
        Me.cmdPrint.Size = New System.Drawing.Size(131, 28)
        Me.cmdPrint.TabIndex = 4
        Me.cmdPrint.Text = "Bericht &drucken..."
        Me.cmdPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.cmdPrint.UseVisualStyleBackColor = True
        '
        'cmdExport
        '
        Me.cmdExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdExport.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdExport.Image = Global.CoinTracer.My.Resources.Resources.ct_disk
        Me.cmdExport.Location = New System.Drawing.Point(722, 569)
        Me.cmdExport.Name = "cmdExport"
        Me.cmdExport.Size = New System.Drawing.Size(113, 28)
        Me.cmdExport.TabIndex = 3
        Me.cmdExport.Text = "...&speichern"
        Me.cmdExport.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.cmdExport.UseVisualStyleBackColor = True
        '
        'cbxExportTypes
        '
        Me.cbxExportTypes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbxExportTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxExportTypes.FormattingEnabled = True
        Me.cbxExportTypes.Location = New System.Drawing.Point(619, 572)
        Me.cbxExportTypes.Name = "cbxExportTypes"
        Me.cbxExportTypes.Size = New System.Drawing.Size(96, 23)
        Me.cbxExportTypes.TabIndex = 2
        '
        'Label24
        '
        Me.Label24.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(550, 575)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(64, 15)
        Me.Label24.TabIndex = 0
        Me.Label24.Text = "Bericht als:"
        '
        'frmReportPreview
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.CancelButton = Me.cmdOK
        Me.ClientSize = New System.Drawing.Size(1147, 604)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.cbxExportTypes)
        Me.Controls.Add(Me.cmdExport)
        Me.Controls.Add(Me.cmdPrint)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.cmdOK)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(1163, 643)
        Me.Name = "frmReportPreview"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Berichtsvorschau"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents cmdPrint As System.Windows.Forms.Button
    Friend WithEvents cmdExport As System.Windows.Forms.Button
    Friend WithEvents cbxExportTypes As System.Windows.Forms.ComboBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
End Class

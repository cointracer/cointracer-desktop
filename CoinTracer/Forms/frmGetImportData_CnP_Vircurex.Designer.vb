<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGetImportData_CnP_Vircurex
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGetImportData_CnP_Vircurex))
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.pnlVircurex = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblAnleitung = New System.Windows.Forms.Label()
        Me.btCancel = New System.Windows.Forms.Button()
        Me.btImport = New System.Windows.Forms.Button()
        Me.lblVircurex2 = New System.Windows.Forms.Label()
        Me.grdImportlines = New CoinTracer.BoundDataGridView()
        Me.colYear = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDisplayLine = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colImportLine = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlVircurex.SuspendLayout()
        CType(Me.grdImportlines, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlVircurex
        '
        resources.ApplyResources(Me.pnlVircurex, "pnlVircurex")
        Me.pnlVircurex.Controls.Add(Me.Label4)
        Me.pnlVircurex.Controls.Add(Me.Label1)
        Me.pnlVircurex.Controls.Add(Me.lblAnleitung)
        Me.pnlVircurex.Name = "pnlVircurex"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'lblAnleitung
        '
        resources.ApplyResources(Me.lblAnleitung, "lblAnleitung")
        Me.lblAnleitung.Name = "lblAnleitung"
        '
        'btCancel
        '
        resources.ApplyResources(Me.btCancel, "btCancel")
        Me.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btCancel.Name = "btCancel"
        Me.btCancel.UseVisualStyleBackColor = True
        '
        'btImport
        '
        resources.ApplyResources(Me.btImport, "btImport")
        Me.btImport.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btImport.Name = "btImport"
        Me.btImport.UseVisualStyleBackColor = True
        '
        'lblVircurex2
        '
        resources.ApplyResources(Me.lblVircurex2, "lblVircurex2")
        Me.lblVircurex2.Name = "lblVircurex2"
        '
        'grdImportlines
        '
        resources.ApplyResources(Me.grdImportlines, "grdImportlines")
        Me.grdImportlines.AllowUserToAddRows = False
        Me.grdImportlines.AllowUserToDeleteRows = False
        Me.grdImportlines.AllowUserToOrderColumns = True
        Me.grdImportlines.AllowUserToResizeRows = False
        Me.grdImportlines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.grdImportlines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdImportlines.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colYear, Me.colDisplayLine, Me.colImportLine})
        Me.grdImportlines.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grdImportlines.MultiSelect = False
        Me.grdImportlines.Name = "grdImportlines"
        Me.grdImportlines.ReadOnly = True
        Me.grdImportlines.RowHeadersVisible = False
        Me.grdImportlines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grdImportlines.ShowCellErrors = False
        Me.grdImportlines.ShowEditingIcon = False
        Me.grdImportlines.ShowRowErrors = False
        '
        'colYear
        '
        Me.colYear.DataPropertyName = "Year"
        Me.colYear.FillWeight = 10.0!
        resources.ApplyResources(Me.colYear, "colYear")
        Me.colYear.Name = "colYear"
        Me.colYear.ReadOnly = True
        Me.colYear.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.colYear.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'colDisplayLine
        '
        Me.colDisplayLine.DataPropertyName = "DisplayContent"
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.colDisplayLine.DefaultCellStyle = DataGridViewCellStyle2
        resources.ApplyResources(Me.colDisplayLine, "colDisplayLine")
        Me.colDisplayLine.Name = "colDisplayLine"
        Me.colDisplayLine.ReadOnly = True
        '
        'colImportLine
        '
        Me.colImportLine.DataPropertyName = "LineContent"
        resources.ApplyResources(Me.colImportLine, "colImportLine")
        Me.colImportLine.Name = "colImportLine"
        Me.colImportLine.ReadOnly = True
        '
        'frmGetImportData_CnP_Vircurex
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btCancel
        Me.Controls.Add(Me.grdImportlines)
        Me.Controls.Add(Me.lblVircurex2)
        Me.Controls.Add(Me.btImport)
        Me.Controls.Add(Me.btCancel)
        Me.Controls.Add(Me.pnlVircurex)
        Me.MinimizeBox = False
        Me.Name = "frmGetImportData_CnP_Vircurex"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.pnlVircurex.ResumeLayout(False)
        Me.pnlVircurex.PerformLayout()
        CType(Me.grdImportlines, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pnlVircurex As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblAnleitung As System.Windows.Forms.Label
    Friend WithEvents btCancel As System.Windows.Forms.Button
    Friend WithEvents btImport As System.Windows.Forms.Button
    Friend WithEvents lblVircurex2 As System.Windows.Forms.Label
    Friend WithEvents grdImportlines As CoinTracer.BoundDataGridView
    Friend WithEvents colDisplayContent As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colYear As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents colDisplayLine As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colImportLine As System.Windows.Forms.DataGridViewTextBoxColumn
End Class

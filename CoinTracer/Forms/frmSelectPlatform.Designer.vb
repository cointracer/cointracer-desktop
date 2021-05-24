<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectPlatform
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSelectPlatform))
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.lblDeclaration = New System.Windows.Forms.Label()
        Me.cbxPlatforms = New System.Windows.Forms.ComboBox()
        Me.ImportPlattformenBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.CoinTracerDataSet = New CoinTracer.CoinTracerDataSet()
        Me.ImportPlattformenTableAdapter = New CoinTracer.CoinTracerDataSetTableAdapters.ImportPlattformenTableAdapter()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.ImportPlattformenBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmdOK
        '
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        resources.ApplyResources(Me.cmdCancel, "cmdCancel")
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'lblDeclaration
        '
        resources.ApplyResources(Me.lblDeclaration, "lblDeclaration")
        Me.lblDeclaration.Name = "lblDeclaration"
        '
        'cbxPlatforms
        '
        resources.ApplyResources(Me.cbxPlatforms, "cbxPlatforms")
        Me.cbxPlatforms.DataSource = Me.ImportPlattformenBindingSource
        Me.cbxPlatforms.DisplayMember = "Bezeichnung"
        Me.cbxPlatforms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxPlatforms.FormattingEnabled = True
        Me.cbxPlatforms.Name = "cbxPlatforms"
        Me.cbxPlatforms.ValueMember = "ID"
        '
        'ImportPlattformenBindingSource
        '
        Me.ImportPlattformenBindingSource.DataMember = "ImportPlattformen"
        Me.ImportPlattformenBindingSource.DataSource = Me.CoinTracerDataSet
        '
        'CoinTracerDataSet
        '
        Me.CoinTracerDataSet.DataSetName = "CoinTracerDataSet"
        Me.CoinTracerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ImportPlattformenTableAdapter
        '
        Me.ImportPlattformenTableAdapter.ClearBeforeFill = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'frmFileImportSetTarget
        '
        Me.AcceptButton = Me.cmdOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbxPlatforms)
        Me.Controls.Add(Me.lblDeclaration)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFileImportSetTarget"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.ImportPlattformenBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CoinTracerDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdOK As Button
    Friend WithEvents cmdCancel As Button
    Friend WithEvents lblDeclaration As Label
    Friend WithEvents cbxPlatforms As ComboBox
    Friend WithEvents CoinTracerDataSet As CoinTracerDataSet
    Friend WithEvents ImportPlattformenBindingSource As BindingSource
    Friend WithEvents ImportPlattformenTableAdapter As CoinTracerDataSetTableAdapters.ImportPlattformenTableAdapter
    Friend WithEvents Label1 As Label
End Class

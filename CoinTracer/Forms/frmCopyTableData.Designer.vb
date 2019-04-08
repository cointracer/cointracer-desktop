<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCopyTableData
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCopyTableData))
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.grpSeparator = New System.Windows.Forms.GroupBox()
        Me.rbSeparatorSemicolon = New System.Windows.Forms.RadioButton()
        Me.rbSeparatorTab = New System.Windows.Forms.RadioButton()
        Me.grpHeaders = New System.Windows.Forms.GroupBox()
        Me.rbCopyNoHeaders = New System.Windows.Forms.RadioButton()
        Me.rbCopyHeaders = New System.Windows.Forms.RadioButton()
        Me.grpTextqualifier = New System.Windows.Forms.GroupBox()
        Me.cbxTextqualifier = New System.Windows.Forms.ComboBox()
        Me.grpSeparator.SuspendLayout()
        Me.grpHeaders.SuspendLayout()
        Me.grpTextqualifier.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdCancel
        '
        resources.ApplyResources(Me.cmdCancel, "cmdCancel")
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdOK
        '
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'grpSeparator
        '
        resources.ApplyResources(Me.grpSeparator, "grpSeparator")
        Me.grpSeparator.Controls.Add(Me.rbSeparatorSemicolon)
        Me.grpSeparator.Controls.Add(Me.rbSeparatorTab)
        Me.grpSeparator.Name = "grpSeparator"
        Me.grpSeparator.TabStop = False
        '
        'rbSeparatorSemicolon
        '
        resources.ApplyResources(Me.rbSeparatorSemicolon, "rbSeparatorSemicolon")
        Me.rbSeparatorSemicolon.Name = "rbSeparatorSemicolon"
        Me.rbSeparatorSemicolon.TabStop = True
        Me.rbSeparatorSemicolon.UseVisualStyleBackColor = True
        '
        'rbSeparatorTab
        '
        resources.ApplyResources(Me.rbSeparatorTab, "rbSeparatorTab")
        Me.rbSeparatorTab.Name = "rbSeparatorTab"
        Me.rbSeparatorTab.TabStop = True
        Me.rbSeparatorTab.UseVisualStyleBackColor = True
        '
        'grpHeaders
        '
        resources.ApplyResources(Me.grpHeaders, "grpHeaders")
        Me.grpHeaders.Controls.Add(Me.rbCopyNoHeaders)
        Me.grpHeaders.Controls.Add(Me.rbCopyHeaders)
        Me.grpHeaders.Name = "grpHeaders"
        Me.grpHeaders.TabStop = False
        '
        'rbCopyNoHeaders
        '
        resources.ApplyResources(Me.rbCopyNoHeaders, "rbCopyNoHeaders")
        Me.rbCopyNoHeaders.Name = "rbCopyNoHeaders"
        Me.rbCopyNoHeaders.TabStop = True
        Me.rbCopyNoHeaders.UseVisualStyleBackColor = True
        '
        'rbCopyHeaders
        '
        resources.ApplyResources(Me.rbCopyHeaders, "rbCopyHeaders")
        Me.rbCopyHeaders.Name = "rbCopyHeaders"
        Me.rbCopyHeaders.TabStop = True
        Me.rbCopyHeaders.UseVisualStyleBackColor = True
        '
        'grpTextqualifier
        '
        resources.ApplyResources(Me.grpTextqualifier, "grpTextqualifier")
        Me.grpTextqualifier.Controls.Add(Me.cbxTextqualifier)
        Me.grpTextqualifier.Name = "grpTextqualifier"
        Me.grpTextqualifier.TabStop = False
        '
        'cbxTextqualifier
        '
        resources.ApplyResources(Me.cbxTextqualifier, "cbxTextqualifier")
        Me.cbxTextqualifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxTextqualifier.FormattingEnabled = True
        Me.cbxTextqualifier.Items.AddRange(New Object() {resources.GetString("cbxTextqualifier.Items"), resources.GetString("cbxTextqualifier.Items1"), resources.GetString("cbxTextqualifier.Items2")})
        Me.cbxTextqualifier.Name = "cbxTextqualifier"
        '
        'frmCopyTableData
        '
        Me.AcceptButton = Me.cmdOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.grpTextqualifier)
        Me.Controls.Add(Me.grpHeaders)
        Me.Controls.Add(Me.grpSeparator)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCopyTableData"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.grpSeparator.ResumeLayout(False)
        Me.grpSeparator.PerformLayout()
        Me.grpHeaders.ResumeLayout(False)
        Me.grpHeaders.PerformLayout()
        Me.grpTextqualifier.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents grpSeparator As System.Windows.Forms.GroupBox
    Friend WithEvents rbSeparatorSemicolon As System.Windows.Forms.RadioButton
    Friend WithEvents rbSeparatorTab As System.Windows.Forms.RadioButton
    Friend WithEvents grpHeaders As System.Windows.Forms.GroupBox
    Friend WithEvents rbCopyNoHeaders As System.Windows.Forms.RadioButton
    Friend WithEvents rbCopyHeaders As System.Windows.Forms.RadioButton
    Friend WithEvents grpTextqualifier As System.Windows.Forms.GroupBox
    Friend WithEvents cbxTextqualifier As System.Windows.Forms.ComboBox
End Class

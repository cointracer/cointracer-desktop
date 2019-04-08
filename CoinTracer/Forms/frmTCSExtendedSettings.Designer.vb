<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTCSExtendedSettings
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
        Dim ToleranceMinutesLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTCSExtendedSettings))
        Dim Label1 As System.Windows.Forms.Label
        Me.grpAssignmentTolerance = New System.Windows.Forms.GroupBox()
        Me.ToleranceMinutesTextBox = New CoinTracer.MinutesTextbox()
        Me.lblMinutesToHours = New System.Windows.Forms.Label()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        ToleranceMinutesLabel = New System.Windows.Forms.Label()
        Label1 = New System.Windows.Forms.Label()
        Me.grpAssignmentTolerance.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToleranceMinutesLabel
        '
        resources.ApplyResources(ToleranceMinutesLabel, "ToleranceMinutesLabel")
        ToleranceMinutesLabel.Name = "ToleranceMinutesLabel"
        '
        'Label1
        '
        resources.ApplyResources(Label1, "Label1")
        Label1.Name = "Label1"
        '
        'grpAssignmentTolerance
        '
        Me.grpAssignmentTolerance.Controls.Add(Label1)
        Me.grpAssignmentTolerance.Controls.Add(Me.ToleranceMinutesTextBox)
        Me.grpAssignmentTolerance.Controls.Add(Me.lblMinutesToHours)
        Me.grpAssignmentTolerance.Controls.Add(ToleranceMinutesLabel)
        resources.ApplyResources(Me.grpAssignmentTolerance, "grpAssignmentTolerance")
        Me.grpAssignmentTolerance.Name = "grpAssignmentTolerance"
        Me.grpAssignmentTolerance.TabStop = False
        '
        'ToleranceMinutesTextBox
        '
        Me.ToleranceMinutesTextBox.ExplanationLabel = Me.lblMinutesToHours
        resources.ApplyResources(Me.ToleranceMinutesTextBox, "ToleranceMinutesTextBox")
        Me.ToleranceMinutesTextBox.Name = "ToleranceMinutesTextBox"
        '
        'lblMinutesToHours
        '
        resources.ApplyResources(Me.lblMinutesToHours, "lblMinutesToHours")
        Me.lblMinutesToHours.Name = "lblMinutesToHours"
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
        'frmTCSExtendedSettings
        '
        Me.AcceptButton = Me.cmdOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.grpAssignmentTolerance)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmTCSExtendedSettings"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.grpAssignmentTolerance.ResumeLayout(False)
        Me.grpAssignmentTolerance.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents grpAssignmentTolerance As GroupBox
    Friend WithEvents ToleranceMinutesTextBox As MinutesTextbox
    Friend WithEvents lblMinutesToHours As Label
    Friend WithEvents cmdOK As Button
    Friend WithEvents cmdCancel As Button
End Class

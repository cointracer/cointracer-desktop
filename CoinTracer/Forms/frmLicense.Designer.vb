<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLicense
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLicense))
        Me.tbxRN = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.LabelIntro = New System.Windows.Forms.Label()
        Me.LabelCopyright = New System.Windows.Forms.Label()
        Me.LabelLicenseHeader = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tbxRN
        '
        Me.tbxRN.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbxRN.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbxRN.Location = New System.Drawing.Point(5, 166)
        Me.tbxRN.Multiline = True
        Me.tbxRN.Name = "tbxRN"
        Me.tbxRN.ReadOnly = True
        Me.tbxRN.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbxRN.Size = New System.Drawing.Size(671, 369)
        Me.tbxRN.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.cmdOK)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 534)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(682, 43)
        Me.Panel1.TabIndex = 1
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdOK.Location = New System.Drawing.Point(580, 7)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(96, 29)
        Me.cmdOK.TabIndex = 0
        Me.cmdOK.Text = "&OK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'LabelIntro
        '
        Me.LabelIntro.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelIntro.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelIntro.Location = New System.Drawing.Point(5, 23)
        Me.LabelIntro.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
        Me.LabelIntro.Name = "LabelIntro"
        Me.LabelIntro.Size = New System.Drawing.Size(677, 119)
        Me.LabelIntro.TabIndex = 3
        Me.LabelIntro.Text = resources.GetString("LabelIntro.Text")
        Me.LabelIntro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelCopyright
        '
        Me.LabelCopyright.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelCopyright.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LabelCopyright.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelCopyright.Location = New System.Drawing.Point(5, 3)
        Me.LabelCopyright.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
        Me.LabelCopyright.Name = "LabelCopyright"
        Me.LabelCopyright.Size = New System.Drawing.Size(677, 20)
        Me.LabelCopyright.TabIndex = 4
        Me.LabelCopyright.Text = "Copyright 2013-2022 Andreas Nebinger"
        Me.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelLicenseHeader
        '
        Me.LabelLicenseHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelLicenseHeader.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LabelLicenseHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelLicenseHeader.Location = New System.Drawing.Point(5, 145)
        Me.LabelLicenseHeader.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
        Me.LabelLicenseHeader.Name = "LabelLicenseHeader"
        Me.LabelLicenseHeader.Size = New System.Drawing.Size(677, 20)
        Me.LabelLicenseHeader.TabIndex = 5
        Me.LabelLicenseHeader.Text = "European Union Public Licence (EUPL) v1.2"
        Me.LabelLicenseHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmLicense
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(682, 577)
        Me.Controls.Add(Me.LabelLicenseHeader)
        Me.Controls.Add(Me.LabelCopyright)
        Me.Controls.Add(Me.LabelIntro)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tbxRN)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(588, 452)
        Me.Name = "frmLicense"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Lizenzinformationen"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tbxRN As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents LabelIntro As Label
    Friend WithEvents LabelCopyright As Label
    Friend WithEvents LabelLicenseHeader As Label
End Class

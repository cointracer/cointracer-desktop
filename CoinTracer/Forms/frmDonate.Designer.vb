<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDonate
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDonate))
        Me.cmdCopy = New System.Windows.Forms.Button()
        Me.pcbCointype = New System.Windows.Forms.PictureBox()
        Me.lblDonate = New System.Windows.Forms.Label()
        Me.tbxAdresse = New System.Windows.Forms.TextBox()
        CType(Me.pcbCointype, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmdCopy
        '
        resources.ApplyResources(Me.cmdCopy, "cmdCopy")
        Me.cmdCopy.Name = "cmdCopy"
        Me.cmdCopy.UseVisualStyleBackColor = True
        '
        'pcbCointype
        '
        Me.pcbCointype.Image = Global.CoinTracer.My.Resources.Resources.coin_logo_bch_28px
        Me.pcbCointype.InitialImage = Global.CoinTracer.My.Resources.Resources.bitcoin_sml
        resources.ApplyResources(Me.pcbCointype, "pcbCointype")
        Me.pcbCointype.Name = "pcbCointype"
        Me.pcbCointype.TabStop = False
        '
        'lblDonate
        '
        resources.ApplyResources(Me.lblDonate, "lblDonate")
        Me.lblDonate.Name = "lblDonate"
        '
        'tbxAdresse
        '
        resources.ApplyResources(Me.tbxAdresse, "tbxAdresse")
        Me.tbxAdresse.Name = "tbxAdresse"
        '
        'frmDonate
        '
        Me.AcceptButton = Me.cmdCopy
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblDonate)
        Me.Controls.Add(Me.pcbCointype)
        Me.Controls.Add(Me.cmdCopy)
        Me.Controls.Add(Me.tbxAdresse)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmDonate"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.pcbCointype, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdCopy As System.Windows.Forms.Button
    Friend WithEvents pcbCointype As System.Windows.Forms.PictureBox
    Friend WithEvents lblDonate As System.Windows.Forms.Label
    Friend WithEvents tbxAdresse As System.Windows.Forms.TextBox
End Class

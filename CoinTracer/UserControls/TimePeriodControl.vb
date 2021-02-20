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

Imports System.ComponentModel

Public Class TimePeriodControl

#Region "Scale-Awareness Support"

    Private currentScaleFactor As SizeF = New SizeF(1.0F, 1.0F)

    Protected Overrides Sub ScaleControl(factor As SizeF, specified As BoundsSpecified)
        MyBase.ScaleControl(factor, specified)
        ' Record the running scale factor used
        Me.currentScaleFactor.Width *= factor.Width
        Me.currentScaleFactor.Height *= factor.Height
        ' control specific positioning and sizing
        If factor.Width <> 1 Or factor.Height <> 1 Then
            Me.Width *= currentScaleFactor.Width
            Me.Height *= currentScaleFactor.Height
            ComboBox1.Width *= currentScaleFactor.Width
            ComboBox1.DropDownWidth *= currentScaleFactor.Width
            ComboBox1.Left *= currentScaleFactor.Width
            Label1.Left *= currentScaleFactor.Width
            dtpTo.Width *= currentScaleFactor.Width
            dtpTo.Left *= currentScaleFactor.Width
            dtpFrom.Width *= currentScaleFactor.Width
            dtpFrom.Left *= currentScaleFactor.Width
        End If
    End Sub

#End Region

    Public Event SettingChanged As EventHandler(Of EventArgs)

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        dtpFrom.Enabled = sender.Checked
        dtpTo.Enabled = sender.Checked
        RaiseEvent SettingChanged(Me, New EventArgs)
    End Sub

    ''' <summary>
    ''' Gibt das ausgewählte Startdatum zurück oder legt dieses fest
    ''' </summary>
    <Description("Gibt das ausgewählte Startdatum zurück oder legt dieses fest")> _
    Public Property DateFrom() As Date
        Get
            If RadioButton2.Checked Then
                ' Freier Zeitraum: das ist easy
                Return dtpFrom.Value
            Else
                ' Abhängig von Ausahl
                Select Case ComboBox1.SelectedIndex
                    Case 0
                        ' gesamter Zeitraum
                        Return #1/1/2009#
                    Case 1
                        ' aktuelles Jahr
                        Return Convert.ToDateTime(Today.Year & "-01-01")
                    Case 2
                        ' aktuelles Halbjahr
                        If Now >= CDate(Today.Year & "-07-02") Then
                            Return CDate(Today.Year & "-07-01")
                        Else
                            Return CDate(Today.Year & "-01-01")
                        End If
                    Case 3
                        ' aktuelles Quartal
                        Return CDate(Today.Year & "-" & ((Today.Month - 1) \ 3) * 3 + 1 & "-01")
                    Case 4
                        ' aktueller Monat
                        Return CDate(Today.Year & "-" & Today.Month & "-01")
                    Case 5
                        ' aktuelle Woche
                        Return CDate(DateAdd(DateInterval.Day, 1 - Now.DayOfWeek, Today))
                    Case 6
                        ' letztes Jahr
                        Return Convert.ToDateTime(Year(Now) - 1 & "-01-01")
                    Case 7
                        ' letztes Halbjahr
                        If Now >= CDate(Today.Year & "-07-02") Then
                            Return CDate(Today.Year & "-07-01")
                        Else
                            Return CDate(Today.Year - 1 & "-07-01")
                        End If
                    Case 8
                        ' letztes Quartal
                        Return DateAdd(DateInterval.Quarter, -1, CDate(Today.Year & "-" & ((Today.Month - 1) \ 3) * 3 + 1 & "-01"))
                    Case 9
                        ' letzter Monat
                        Return DateAdd(DateInterval.Month, -1, CDate(Today.Year & "-" & Today.Month & "-01"))
                    Case 10
                        ' letzte Woche
                        Return CDate(DateAdd(DateInterval.Day, 1 - Now.DayOfWeek - 7, Today))
                    Case 11, 12
                        ' vorletztes Jahr / letzte 2 Jahre
                        Return Convert.ToDateTime(Year(Now) - 2 & "-01-01")
                    Case 13
                        ' letzte 2 Monate
                        Return DateAdd(DateInterval.Month, -2, CDate(Today.Year & "-" & Today.Month & "-01"))
                    Case 14
                        ' letzte 2 Wochen
                        Return CDate(DateAdd(DateInterval.Day, 1 - Now.DayOfWeek - 14, Today))
                    Case Else
                        ' weiß der Himmel
                        Return #1/1/2000#
                End Select
            End If
        End Get
        Set(ByVal value As Date)
            RadioButton2.Checked = True
            dtpFrom.Value = value
        End Set

    End Property

    ''' <summary>
    ''' Gibt das ausgewählte Enddatum zurück oder legt dieses fest
    ''' </summary>
    <Description("Gibt das ausgewählte Enddatum zurück oder legt dieses fest")> _
    Public Property DateTo() As Date
        Get
            If RadioButton2.Checked Then
                ' Freier Zeitraum: das ist easy
                Return dtpTo.Value
            Else
                ' Abhängig von Ausahl
                Select Case ComboBox1.SelectedIndex
                    Case 0, 1, 2, 3, 4, 5
                        ' aktuelles irgendwas: immer das aktuelle Datum
                        Return Today
                    Case 6, 12
                        ' letzte x Jahre
                        Return Convert.ToDateTime(Year(Now) - 1 & "-12-31")
                    Case 11
                        ' vorletztes Jahr
                        Return Convert.ToDateTime(Year(Now) - 2 & "-12-31")
                    Case 7
                        ' letztes Halbjahr
                        If Now >= CDate(Today.Year & "-07-02") Then
                            Return CDate(Today.Year & "-06-30")
                        Else
                            Return CDate(Today.Year - 1 & "-12-31")
                        End If
                    Case 8
                        ' letztes Quartal
                        Return DateAdd(DateInterval.Day, -1, CDate(Today.Year & "-" & ((Today.Month - 1) \ 3) * 3 + 1 & "-01"))
                    Case 9, 13
                        ' letzte x Monate
                        Return DateAdd(DateInterval.Day, -1, CDate(Today.Year & "-" & Today.Month & "-01"))
                    Case 10, 14
                        ' letzte x Wochen
                        Return CDate(DateAdd(DateInterval.Day, -Now.DayOfWeek, Today))
                    Case Else
                        ' weiß der Himmel
                        Return #1/1/2099#
                End Select
            End If
        End Get
        Set(ByVal value As Date)
            RadioButton2.Checked = True
            dtpTo.Value = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob ein Zeitraum ausgewählt wurde (TRUE) oder nicht (FALSE)
    ''' </summary>
    <Description("Gibt an, ob ein Zeitraum ausgewählt wurde (TRUE) oder nicht (FALSE)")> _
    Public ReadOnly Property PeriodSelected() As Boolean
        Get
            Return Convert.ToBoolean(Not (RadioButton1.Checked AndAlso ComboBox1.SelectedIndex = 0))
        End Get
    End Property

    ''' <summary>
    ''' Gibt ein SQL-Snipplet zurück, das den übergebenen Feldnamen in einen BETWEEN-Ausdruck einbaut
    ''' </summary>
    ''' <remarks>SQL-Snipplet hat den Aufbau [Feldname] BETWEEN [Startdatum] AND [Enddatum]. Leerstring, wenn gesamter Zeitraum ausgewählt wurde.</remarks>
    <Description("Gibt ein SQL-Snipplet zurück, das den übergebenen Feldnamen in einen BETWEEN-Ausdruck einbaut")>
    Public ReadOnly Property DateSql(Optional ByVal FieldName As String = "") As String
        Get
            If Me.PeriodSelected Then
                Dim SQL As String = ""
                If (FieldName.Trim).Length > 0 Then
                    SQL = FieldName.Trim & " "
                End If
                SQL += String.Format("BETWEEN '{0}' AND '{1}'", _
                                     Me.DateFrom.ToString("yyyy-MM-dd"), _
                                     DateAdd(DateInterval.Day, 1, Me.DateTo).ToString("yyyy-MM-dd"))
                Return SQL
            Else
                Return ""
            End If
        End Get
    End Property

    Private _Compact As Boolean
    <Browsable(True)> _
    <Description("Legt fest, ob das Control kompakt dargestellt werden soll.")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    <Category("Appearance")> _
    <DefaultValue(False)> _
    Public Property CompactAppearance() As Boolean
        Get
            Return _Compact
        End Get
        Set(ByVal value As Boolean)
            _Compact = value
            If _Compact Then
                Me.Width = 240
                Me.Height = 95
                ComboBox1.Width = 107
                ComboBox1.DropDownWidth = 107
                Label1.Location = New Drawing.Point(105, 68)
                dtpTo.Location = New Drawing.Point(132, 66)
            Else
                Me.Width = 315
                Me.Height = 65
                ComboBox1.Width = 181
                ComboBox1.DropDownWidth = 181
                Label1.Location = New Drawing.Point(211, 39)
                dtpTo.Location = New Drawing.Point(239, 37)
            End If
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
        Get
            Return New Size(315, 65)
        End Get
    End Property


    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        RadioButton1.Checked = True
        dtpFrom.Value = DateFrom
        dtpTo.Value = DateTo
        RaiseEvent SettingChanged(Me, New EventArgs)
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        CompactAppearance = False

    End Sub

    ''' <summary>
    ''' Initialisiert die Zeitraum-Voreinstellungen des Controls
    ''' </summary>
    Public Sub InitializeTimeSettings()

        dtpFrom.Value = CDate(Year(Now) & "-01-01")
        dtpTo.Value = CDate(Now)
        dtpFrom.Enabled = False
        dtpTo.Enabled = False

        ComboBox1.SelectedIndex = 0

        RadioButton2.Checked = False
        RadioButton1.Checked = True

    End Sub

    Private Sub dtpFromTo_ValueChanged(sender As Object, e As EventArgs) Handles dtpFrom.ValueChanged, dtpTo.ValueChanged
        RaiseEvent SettingChanged(Me, New EventArgs)
    End Sub

End Class

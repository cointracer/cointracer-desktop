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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or in the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports CoinTracer.CoinTracerDataSetTableAdapters

Public Class DataPeriodControl

    Public Event SettingsChanged As EventHandler(Of EventArgs)

    Private _Cnn As SQLite.SQLiteConnection

    Private _LastValue As String

    Private _DS As CoinTracerDataSet
    Public ReadOnly Property DataSet() As CoinTracerDataSet
        Get
            Return _DS
        End Get
    End Property

    Private _KonfigurationTa As KonfigurationTableAdapter

    Private _VariableID As Long
    Public Property VariableID() As Long
        Get
            Return _VariableID
        End Get
        Set(ByVal value As Long)
            _VariableID = value
        End Set
    End Property


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        ' ID des Eintrags in Tabelle Konfiguration, der den Date-String enthält
        _VariableID = 1
        _LastValue = ""

    End Sub

    Public Sub InitData(ByRef Connection As SQLite.SQLiteConnection, _
                        Optional FromString As String = "", _
                        Optional VariableID As Long = -1)
        Dim Count As Integer

        _Cnn = Connection
        _DS = New CoinTracerDataSet(_Cnn)
        If VariableID > 0 Then
            _VariableID = VariableID
        End If

        _KonfigurationTa = New KonfigurationTableAdapter()
        Count = _KonfigurationTa.FillBySQL(_DS.Konfiguration, String.Format("WHERE ID = {0} LIMIT 1", _VariableID))
        If Count < 0 Then
            ' Fehler beim Laden!
            Throw New ApplicationException("Die Konfigurationstabelle konnte nicht geöffnet werden!")
        ElseIf Count = 0 Then
            ' Kein Eintrag gefunden!
            Throw New ApplicationException(String.Format("In der Konfigurationstabelle konnte kein Eintrag mit der ID {0} gefunden werden!", _VariableID))
        End If

        If FromString <> "" Then
            Me.FromString(FromString)
        End If

    End Sub

    ''' <summary>
    ''' Setzt die Anzahl und Einheit des Controls anhand des übergebenen SQL-Date-Modifiers
    ''' </summary>
    ''' <param name="FromString">Date-Modifier im Sqlite-Format (Beispiel: '+1 years')</param>
    Public Sub FromString(FromString As String)
        Dim Parts() As String
        Try
            Parts = FromString.Split(" ")
            If Parts.Length <> 2 Then
                Throw New Exception
            End If
            tbxValue.Text = Convert.ToInt16(Parts(0))
            Select Case Parts(1)
                Case "years"
                    cbxUnit.SelectedIndex = 0
                Case "months"
                    cbxUnit.SelectedIndex = 1
                Case "days"
                    cbxUnit.SelectedIndex = 2
                Case Else
                    Throw New Exception
            End Select
            _LastValue = FromString
        Catch ex As Exception
            Throw New Exception(String.Format("Fehler bei der Interpretation des SQL-Date-Modifiers '{0}'! Ungültiges Format.", FromString))
        End Try

    End Sub

    ''' <summary>
    ''' Gibt die aktuelle Einstellung des Controls als Sqlite-Date-Modifier-String zurück
    ''' </summary>
    ''' <returns>Sqlite-Date-Modifier-String</returns>
    Public Overrides Function ToString() As String
        Dim Value As Integer
        Try
            Value = Convert.ToInt16(tbxValue.Text.Trim)
        Catch ex As Exception
            Value = 0
        End Try
        If Value > 0 Then
            Dim ReturnValue As String = "+" & Value & " "
            Select Case cbxUnit.SelectedIndex
                Case 2
                    Return ReturnValue & "days"
                Case 1
                    Return ReturnValue & "months"
                Case Else
                    Return ReturnValue & "years"
            End Select
        Else
            ' Default-Wert
            Return "+1 years"
        End If

    End Function

    ''' <summary>
    ''' Speichert die aktuelle Einstellung des Controls in der Datenbank
    ''' </summary>
    Public Sub UpdateData()
        _DS.Konfiguration(0).Wert = Me.ToString
        _KonfigurationTa.Update(_DS)
    End Sub

    Public ReadOnly Property Sticky() As Boolean
        Get
            Return Not _LastValue = Me.ToString
        End Get
    End Property



    Private Sub cbxUnit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbxUnit.KeyPress
        If Not Char.IsDigit(e.KeyChar) Then e.KeyChar = ""
    End Sub

    Private Sub cbxUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxUnit.SelectedIndexChanged
        RaiseEvent SettingsChanged(Me, e)
    End Sub


    Private Sub tbxValue_TextChanged(sender As Object, e As EventArgs) Handles tbxValue.TextChanged
        RaiseEvent SettingsChanged(Me, e)
    End Sub

End Class

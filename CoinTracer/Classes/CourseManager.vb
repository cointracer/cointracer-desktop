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
Imports CoinTracer.CoinTracerDataSet
Imports System.IO

Public Class CourseManager
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If _DS IsNot Nothing Then
                    _DS.Dispose()
                End If
                ProgressWaitManager.CloseProgress()
            End If

            ' TO DO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TO DO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TO DO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Private _FromKontoID As Long
    Public Property FromKontoID() As Long
        Get
            Return _FromKontoID
        End Get
        Set(ByVal value As Long)
            _FromKontoID = value
        End Set
    End Property

    Private _ToKontoID As Long
    Public Property ToKontoID() As Long
        Get
            Return _FromKontoID
        End Get
        Set(ByVal value As Long)
            _ToKontoID = value
        End Set
    End Property

    Private _SilentMode As Boolean
    Public Property SilentMode() As Boolean
        Get
            Return _SilentMode
        End Get
        Set(ByVal value As Boolean)
            _SilentMode = value
        End Set
    End Property

    Private _TML As ThreadManagerLight
    Public Property ThreadManagerLight() As ThreadManagerLight
        Get
            Return _TML
        End Get
        Set(ByVal value As ThreadManagerLight)
            _TML = value
            _SilentMode = True
        End Set
    End Property

    Private _DS As CoinTracerDataSet

    Public Sub New(SQLiteConnection As SQLite.SQLiteConnection)
        _DS = New CoinTracerDataSet(SQLiteConnection)
        _SilentMode = False
    End Sub


    ''' <summary>
    ''' Gibt den Tag zurück, bis zu dem Kurse (beliebige) bekannt sind
    ''' </summary>
    ''' <param name="FromKontoID">ID des Kontos der Ausgangswährung</param>
    ''' <param name="ToKontoID">ID des Kontos der Zielwährung</param>
    Public Function GetCoursesCutOffDay(FromKontoID As DBHelper.Konten, ToKontoID As DBHelper.Konten) As Date
        Dim KursTa As New KurseTableAdapter
        Dim KursTb As New KurseDataTable
        If KursTa.FillBySQL(KursTb, "where QuellKontoID=" & DirectCast(FromKontoID, Integer) & " and ZielKontoID=" & DirectCast(ToKontoID, Integer) & " order by Zeitpunkt desc limit 1") > 0 Then
            Return KursTb(0).Zeitpunkt
        Else
            Return Nothing
        End If
        KursTa.Dispose()
    End Function

    ''' <summary>
    ''' Gibt den Tag zurück, ab dem Kurse (beliebige) bekannt sind (idealerweise 01.01.2009...)
    ''' </summary>
    ''' <param name="FromKontoID">ID des Kontos der Ausgangswährung</param>
    ''' <param name="ToKontoID">ID des Kontos der Zielwährung</param>
    Public Function GetCoursesStartDay(FromKontoID As DBHelper.Konten, ToKontoID As DBHelper.Konten) As Date
        Dim KursTa As New KurseTableAdapter
        Dim KursTb As New KurseDataTable
        If KursTa.FillBySQL(KursTb, "where QuellKontoID=" & DirectCast(FromKontoID, Integer) & " and ZielKontoID=" & DirectCast(ToKontoID, Integer) & " order by Zeitpunkt limit 1") > 0 Then
            Return KursTb(0).Zeitpunkt
        Else
            Return Nothing
        End If
        KursTa.Dispose()
    End Function

    ''' <summary>
    ''' Prüft, ob es in der Trade-Tabelle Trades mit unklarem EUR-Wert gibt
    ''' </summary>
    ''' <param name="CheckCurrency">Währung, auf die geprüft werden soll</param>
    ''' <param name="UntilDate">Datum, bis zu dem geprüft werden soll (ausschließlich!)</param>
    ''' <param name="UnweightedTradeTade">Wenn übergeben, steht hier exemplarisch ein Datum, zu dem Kursinformationen fehlen</param>
    Public Function HasUnweightedTrades(Optional CheckCurrency As DBHelper.Konten = DBHelper.Konten.USD, _
                                        Optional UntilDate As Date = DATENULLVALUE, _
                                        Optional ByRef UnweightedTradeTade As String = Nothing) As Boolean
        Dim TradesTa As New TradesTableAdapter
        Dim TradesTb As New TradesDataTable
        Dim ReturnValue As Boolean
        Dim SQL = String.Format("where ((TradeTypID in (2,4) and ZielKontoID={0} and BetragNachGebuehr>0 and WertEUR=0) " & _
                                " or (TradeTypID in (1,3) and QuellKontoID={0} and QuellBetragNachGebuehr>0 and WertEUR=0))", CLng(CheckCurrency))
        If UntilDate <> DATENULLVALUE Then
            SQL &= String.Format(" and Zeitpunkt<'{0}'", UntilDate.ToString("yyyy-MM-dd"))
        End If
        SQL &= " order by Zeitpunkt limit 1"
        ReturnValue = TradesTa.FillBySQL(TradesTb, SQL) > 0
        If UnweightedTradeTade IsNot Nothing Then
            ' Tage, an denen es ungeklärte Trades gibt, ermitteln
            If ReturnValue Then
                UnweightedTradeTade = TradesTb(0).Zeitpunkt.ToString("dd.MM.yyyy")
            End If
        End If
        TradesTa.Dispose()
        Return ReturnValue
    End Function

    ''' <summary>
    ''' Liest eine CSV-Datei mit Informationen zu EUR/USD-Kursen ein
    ''' </summary>
    ''' <param name="UntilDate"></param>
    ''' <returns>Anzahl der Tage, für die Kursinformationen eingelesen wurden</returns>
    Public Function ImportCoursesEurUsd(Optional UntilDate As Nullable(Of Date) = Nothing) As Long

        Dim frmMessage As New frmGetCourseData
        Dim OFD As New OpenFileDialog()
        Dim AllLines() As String
        Dim Added As Long = 0
        If frmMessage.ShowDialog(frmMain) = DialogResult.OK Then
            ' Datei auswählen
            With OFD
                .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                .FilterIndex = 1
                .Title = "Kursdatendatei der Bundesbank auswählen... (BBEX3.D.USD.EUR.BB.AC.000*.csv)"
                .RestoreDirectory = True
                If .ShowDialog() = DialogResult.OK Then
                    If _SilentMode Then
                        _TML.MessageText = "Lese Kursdaten EUR/USD ein..."
                    Else
                        Cursor.Current = Cursors.WaitCursor
                        ProgressWaitManager.ShowProgress(frmMain)
                        ProgressWaitManager.UpdateProgress(20, "Lese Kursdaten EUR/USD ein...")
                    End If
                    Dim KursTa As New KurseTableAdapter
                    Dim KursTb As New KurseDataTable
                    KursTa.FillBySQL(KursTb, "where QuellKontoID=" & DBHelper.Konten.EUR & " and ZielKontoID=" & DBHelper.Konten.USD)
                    Try
                        ' Datei einlesen
                        AllLines = File.ReadAllLines(.FileName, System.Text.Encoding.Default)
                        If AllLines.Length > 0 Then
                            Added = InsertCoursesEurUsd(AllLines, KursTa, KursTb)
                        End If
                    Catch ex As Exception
                        If _SilentMode Then
                            _TML.MessageText = ""
                        Else
                            Cursor.Current = Cursors.Default
                            ProgressWaitManager.CloseProgress()
                        End If
                        Throw
                        Exit Function
                    End Try
                    If _SilentMode Then
                        _TML.MessageText = ""
                    Else
                        Cursor.Current = Cursors.Default
                    End If

                Else
                    Added = -1
                End If
            End With
        Else
            Added = -1
        End If
        Return Added
    End Function

    ''' <summary>
    ''' Holt USD/EUR-Kurse von der Bundesbank ab
    ''' </summary>
    ''' <param name="UntilDate"></param>
    ''' <returns>Anzahl der Tage, für die Kursinformationen eingelesen wurden</returns>
    Public Function FetchCoursesEurUsd(Optional UntilDate As Nullable(Of Date) = Nothing) As Long

        Dim URI As String = "https://www.bundesbank.de/cae/servlet/StatisticDownload?tsId=BBEX3.D.USD.EUR.BB.AC.000&its_csvFormat=de&its_fileFormat=csv&mode=its"

        Dim LastCOD As Date = GetCoursesCutOffDay(DBHelper.Konten.EUR, DBHelper.Konten.USD)
        Dim HttpRequest As New UriRequest(URI)
        Dim Csv As String
        Dim Added As Long = 0

        If UntilDate > LastCOD OrElse UntilDate Is Nothing Then

            ' Prüfen, ob schon Einträge vorhanden sind (ggf. warnen)
            Dim KursTa As New KurseTableAdapter
            Dim KursTb As New KurseDataTable
            KursTa.FillBySQL(KursTb, "where QuellKontoID=" & DBHelper.Konten.EUR & " and ZielKontoID=" & DBHelper.Konten.USD)
            If KursTb.Count = 0 AndAlso Not _SilentMode Then
                If MessageBox.Show("Sie rufen die Währungskursdaten EUR <> USD zum ersten Mal ab. Es wird jetzt eine Verbindung mit " &
                                   "www.bundesbank.de aufgebaut, um alle verfügbaren Kursdaten abzufragen. " & System.Environment.NewLine & System.Environment.NewLine &
                                   "Bitte beachten Sie, " &
                                   "dass das erste Auffüllen der Kurstabelle etwas Zeit in Anspruch nehmen kann (ein bis " &
                                   "zwei Minuten).", "Erster Kursdatenabruf", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                    KursTa.Dispose()
                    Return -1
                    Exit Function
                End If
            End If
            ' Kurse aus dem Internet laden
            Try
                Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
                If _SilentMode Then
                    _TML.MessageText = "Rufe Kursdaten von www.bundesbank.de ab..."
                Else
                    Cursor.Current = Cursors.WaitCursor
                    ProgressWaitManager.ShowProgress(frmMain)
                    ProgressWaitManager.UpdateProgress(20, "Rufe Kursdaten von www.bundesbank.de ab...")
                End If
                Csv = HttpRequest.Request
            Catch ex As Exception
                If _SilentMode Then
                    _TML.MessageText = ""
                Else
                    Cursor.Current = Cursors.Default
                    ProgressWaitManager.CloseProgress()
                End If
                Throw
                Exit Function
            End Try

            ' In Tabelle Kurse schreiben
            Dim Lines() As String
            Try
                Lines = Split(Csv, vbLf)
            Catch ex As Exception
                ReDim Lines(0)
            End Try

            If Lines.Length > 0 Then
                Added = InsertCoursesEurUsd(Lines, KursTa, KursTb)
            End If

        End If
        If _SilentMode Then
            _TML.MessageText = ""
        Else
            Cursor.Current = Cursors.Default
        End If
        Return Added

    End Function

    ''' <summary>
    ''' Schreibt das übergebene String-Array in die Kurse-Tabelle. Schließt anschließend das WaitWindow und gibt TableAdapter frei.
    ''' </summary>
    ''' <returns>Anzahl der Tage, für die Kursinformationen eingelesen wurden</returns>
    Private Function InsertCoursesEurUsd(ByRef Lines() As String, _
                                         ByRef KurseTa As KurseTableAdapter, _
                                         ByRef KurseTb As KurseDataTable) As Long
        Dim Line As String
        Dim LineNum As Long = 0
        Dim Added As Long = 0
        Dim Items() As String
        Dim FoundRows() As KurseRow

        If _SilentMode Then
            _TML.MessageText = "Lese Kursdaten ein..."
        End If
        Dim CIde As CultureInfo = New CultureInfo("de-DE")
        For Each Line In Lines
            Try
                Items = Split(Line, ";")
            Catch ex As Exception
                ReDim Items(2)
            End Try
            LineNum += 1
            If Not _SilentMode Then
                ProgressWaitManager.UpdateProgress(20 + (60 / DirectCast(Lines, ICollection).Count) * LineNum, "Lese Kursdaten ein...")
            End If
            If IsDate(Items(0)) AndAlso Items(0) >= "2009-01-01" Then
                FoundRows = KurseTb.Select("Zeitpunkt='" & CDate(Items(0)).ToString("yyyy-MM-dd") & "'")
                If FoundRows.Length = 0 Then
                    ' Nur eintragen, wenn noch nicht vorhanden
                    If Items(1) = "." Then
                        Items(1) = "-1"
                    End If
                    ' Zeile eintragen
                    KurseTa.Insert(CDate(Items(0)), 1, DBHelper.Konten.EUR, _
                                  Single.Parse(Items(1), CIde), DBHelper.Konten.USD, 0)
                    Added += 1
                End If
            End If
        Next
        ' Lücken in den Kursen füllen (haben -1 als Kurs) - Yeah, SQL rocks!
        If _SilentMode Then
            _TML.MessageText = "Ergänze fehlende Tage..."
        Else
            ProgressWaitManager.UpdateProgress(90, "Ergänze fehlende Tage...")
        End If
        _DS.ExecuteSQL(String.Format("update Kurse set ZielBetrag = " & _
            "round((coalesce((select ZielBetrag from Kurse k2 " & _
            "where k2.Zeitpunkt<Kurse.Zeitpunkt and k2.ZielBetrag>=0 and k2.QuellKontoID={0} and k2.ZielKontoID={1} " & _
            "order by k2.Zeitpunkt desc limit 1), (select ZielBetrag from Kurse k3 " & _
            "where k3.Zeitpunkt>Kurse.Zeitpunkt and k3.ZielBetrag>=0 and k3.QuellKontoID={0} and k3.ZielKontoID={1} " & _
            "order by k3.Zeitpunkt asc limit 1)) + " & _
            "coalesce((select ZielBetrag from Kurse k3 " & _
            "where k3.Zeitpunkt>Kurse.Zeitpunkt and k3.ZielBetrag>=0 and k3.QuellKontoID={0} and k3.ZielKontoID={1} " & _
            "order by k3.Zeitpunkt asc limit 1), (select ZielBetrag from Kurse k2 " & _
            "where k2.Zeitpunkt<Kurse.Zeitpunkt and k2.ZielBetrag>=0 and k2.QuellKontoID={0} and k2.ZielKontoID={1} " & _
            "order by k2.Zeitpunkt desc limit 1))) / 2, 4), " & _
            "Calculated=1 " & _
            "where Kurse.QuellKontoID={0} and Kurse.ZielKontoID={1} and Kurse.ZielBetrag=-1", _
            CInt(DBHelper.Konten.EUR), CInt(DBHelper.Konten.USD)))
        If _SilentMode Then
            _TML.MessageText = "Kursabruf abgeschlossen!"
        Else
            ProgressWaitManager.UpdateProgress(100, "Kursabruf abgeschlossen!")
        End If
        KurseTa.Dispose()
        If _SilentMode Then
            _TML.MessageText = ""
        Else
            ProgressWaitManager.CloseProgress()
        End If
        Return Added

    End Function

End Class

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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or within the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

<Serializable()>
Public Class CointracerDatabaseException
    Inherits System.Exception
    Implements Runtime.Serialization.ISerializable

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class

''' <summary>
''' Helfer-Klasse für den Zugriff auf die Cointracer-DB
''' </summary>
''' <remarks></remarks>
Public Class DBHelper
    Implements IDisposable

    Public Enum TableNames
        _AnyTable
        Trades
        TradeTypen
        Konten
        Plattformen
        Importe
        ZeitstempelWerte
        TradesWerte
        Kalkulationen
        Szenarien
        Kurse
        ApiDaten
    End Enum

    ' Achtung: die nachfolgenden Enums müssen immer mit den DB-Einträgen synchron bleiben!
    Public Enum TradeTypen
        Undefiniert = 0
        Einzahlung = 1
        Auszahlung = 2
        Kauf = 3
        Verkauf = 4
        Transfer = 5
        Verlust = 7
        Gebühr = 9
        KaufCoin4Coin = 19
        TransferBörseBörse = 37
        TransferWalletBörse = 69
        TransferBörseWallet = 101
    End Enum

    Public Enum Konten  ' this is not meant to be synchronous with the database!
        Fehler = -1
        Unbekannt = 0
        EUR = 101
        USD = 102
        BTC = 201
        LTC = 202
        PPC = 203
        NMC = 204
        FEE = 256
        BCH = 254
        ETH = 253
        feeEUR = 311
        feeUSD = 312
        feeBTC = 321
        feeLTC = 322
        feePPC = 323
        feeNMC = 324
        feeFEE = 376
    End Enum



    Private Const DBNAME As String = "cointracer.data"

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If Not _cnn Is Nothing Then
                    Dim items As Array
                    items = System.Enum.GetValues(GetType(TableNames))
                    Dim item As Long
                    For Each item In items
                        Reset_DataAdapter(item, , True)
                    Next
                    _cnn.Dispose()
                    _cnn = Nothing
                End If
            End If
            _DataAdapter = Nothing
            _DataSet = Nothing
            _DataTable = Nothing
        End If
        Me.disposedValue = True
    End Sub

    ' TO DO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' Schreibt die XSD-Schemadateien für alle Tabellen der DB
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub WriteXSD(Optional XSDFilename As String = "Cointracer_Schema.xsd")
        Dim DBO As New DBObjects("select * from sqlite_master where type in ('table','view') and [name] not like 'sqlite%' order by [type],[name]", _
                                 _cnn)
        If DBO IsNot Nothing Then
            Dim Row As DataRow
            For Each Row In DBO.DataTable.Rows
                Dim TblDBO As New DBObjects(String.Format("select * from {0}", Row("name")), _cnn)
                TblDBO.DataSet.WriteXmlSchema(XSDFilename.Replace(".xsd", "_" & Row("name") & ".xsd"))
            Next Row
        End If
    End Sub

    Private _DataAdapter() As SQLite.SQLiteDataAdapter
    Public ReadOnly Property DataAdapter(Index As TableNames) As SQLite.SQLiteDataAdapter
        Get
            Set_DataAdapter(Index)
            Return _DataAdapter(Index)
        End Get
    End Property

    Private _DataSet() As Data.DataSet
    Public ReadOnly Property DataSet(Index As TableNames) As Data.DataSet
        Get
            Set_DataAdapter(Index)
            Return _DataSet(Index)
        End Get
    End Property
    ''' <summary>
    ''' Gibt das DataSet für eine beliebige SQL-Abfrage zurück.
    ''' </summary>
    ''' <param name="SQL">SQL-String, ausgeführt wird.</param>
    ''' <remarks>Achtung: überschreibt immer die Objekte des Index _AnyTable!</remarks>
    Public ReadOnly Property DataSet(SQL As String) As Data.DataSet
        Get
            Reset_DataAdapter(TableNames._AnyTable, SQL)
            Return _DataSet(TableNames._AnyTable)
        End Get
    End Property

    Private _DataTable() As Data.DataTable
    Public ReadOnly Property DataTable(Index As TableNames) As Data.DataTable
        Get
            Set_DataAdapter(Index)
            Return _DataSet(Index).Tables(0)
        End Get
    End Property
    Public ReadOnly Property DataTable(Index As TableNames, SQL As String) As Data.DataTable
        Get
            Set_DataAdapter(Index, SQL)
            Return _DataSet(Index).Tables(0)
        End Get
    End Property

    Private Sub Set_DataSet(Index As TableNames)
        If _DataSet(Index) Is Nothing Then
            _DataSet(Index) = New Data.DataSet
        End If
    End Sub

    Private Sub Set_DataAdapter(Index As TableNames, Optional SQL As String = "")
        Dim tb As DataTable
        Dim Trials As Int16 = 0
        If _DataAdapter(Index) Is Nothing Then
            If SQL = "" Then
                ' Erst einmal laden: da kein SQL übergeben wurde, gesamte Tabelle laden
                If Index = TableNames._AnyTable Then
                    ' Wenn keine Tabelle und kein SQL angegeben wurde, dann Dummy laden
                    SQL = "select 1"
                Else
                    SQL = "select * from " & System.Enum.GetName(GetType(TableNames), Index)
                End If
            End If
            CheckConnection()
            _DataAdapter(Index) = New SQLite.SQLiteDataAdapter(SQL, _cnn)
            Set_DataSet(Index)
            With _DataAdapter(Index)
                .MissingSchemaAction = MissingSchemaAction.AddWithKey
                While _DataSet(Index).Tables.Count = 0 And Trials < 4
                    Try
                        .Fill(_DataSet(Index), System.Enum.GetName(GetType(TableNames), Index))
                    Catch ex As InvalidOperationException
                        If Left(ex.Message, Len("Connection was closed")) = "Connection was closed" Then
                            _cnn.Open()
                            .Fill(_DataSet(Index), System.Enum.GetName(GetType(TableNames), Index))
                        End If
                    Catch ex As Exception
                        DefaultErrorHandler(ex, "Es ist ein Fehler bei einer Datenbankabfrage aufgetretren! (Tabelle ID " & Index & ")" & _
                                            System.Environment.NewLine & System.Environment.NewLine & ex.Message)
                        Exit Sub
                    End Try
                    Trials += 1
                End While
                tb = _DataSet(Index).Tables(0)
                If tb.Columns.Contains("ID") Then
                    tb.Columns("ID").AutoIncrementSeed = 1
                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Stellt sicher, dass die Connection offen ist - sonst Exception!
    ''' </summary>
    Private Sub CheckConnection()
        Dim Tries As Integer = 0
        While Tries < 3 And _cnn.State <> ConnectionState.Open
            Tries += 1
            _cnn.Open()
        End While
        If _cnn.State <> ConnectionState.Open Then
            Throw New CointracerDatabaseException("Die Verbindung zur Datenbank konnte nicht geöffnet werden.")
        End If
    End Sub

    Private _cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
    End Property

    ''' <summary>
    ''' Schreibt die Änderungen für die übergebene Tabelle zurück in die Datenbank
    ''' </summary>
    ''' <param name="Index">Tabellen-Index</param>
    Public Sub Update(Index As TableNames)
        If Not _DataAdapter(Index) Is Nothing Then
            Dim objCommandBuilder As New SQLite.SQLiteCommandBuilder(_DataAdapter(Index))
            objCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges
            _DataAdapter(Index).Update(_DataSet(Index), System.Enum.GetName(GetType(TableNames), Index))
        End If
    End Sub

    Public Sub Reset_DataAdapter(Index As TableNames, Optional SQL As String = "", Optional JustDispose As Boolean = False)
        If Not _DataTable Is Nothing Then
            If Not _DataTable(Index) Is Nothing Then
                _DataTable(Index) = Nothing
            End If
        End If
        If Not _DataSet Is Nothing Then
            If Not _DataSet(Index) Is Nothing Then
                _DataSet(Index) = Nothing
            End If
        End If
        If Not _DataAdapter Is Nothing Then
            If Not _DataAdapter(Index) Is Nothing Then
                _DataAdapter(Index).Dispose()
                _DataAdapter(Index) = Nothing
            End If
        End If
        If Not JustDispose Then
            Set_DataAdapter(Index, SQL)
        End If
    End Sub

    Public Sub ExecuteSQL(SQL As String)
        CheckConnection()
        Dim cmd As New SQLite.SQLiteCommand(SQL, _cnn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    ''' <summary>
    ''' Ermittelt den höchsten Wert in der Primary-Key-Spalte einer Tabelle
    ''' </summary>
    ''' <param name="Index">Tabelle als Index</param>
    ''' <returns>ID der höchsten verwendeten ID, im Fehlerfall -1</returns>
    Public Function GetMaxID(Index As TableNames) As Long
        Dim DA As SQLite.SQLiteDataAdapter
        Dim DS As New Data.DataSet
        Dim TB As Data.DataTable
        Dim SQL As String
        Dim Ret As Long
        SQL = "select seq from sqlite_sequence where name='" & System.Enum.GetName(GetType(TableNames), Index) & "'"
        Try
            DA = New SQLite.SQLiteDataAdapter(SQL, _cnn)
            DA.Fill(DS)
            TB = DS.Tables(0)
            If TB.Rows.Count > 0 Then
                Ret = TB.Rows(0)("seq")
            Else
                Ret = 0
            End If
            DA.Dispose()
            Return Ret
        Catch ex As Exception
            Return -1
        End Try
    End Function


    Public Sub New(DBConnection As SQLite.SQLiteConnection)

        _cnn = DBConnection
        CheckConnection()
        ReDimArrays()

    End Sub

    Private Sub ReDimArrays()
        ' Array-Variablen dimensionieren
        Dim items As Array
        items = System.Enum.GetValues(GetType(TableNames))
        Dim item As Long, max As Long
        For Each item In items
            If item > max Then
                max = item
            End If
        Next
        ReDim _DataAdapter(max)
        ReDim _DataSet(max)
    End Sub

    ''' <summary>
    ''' Liest alle Tabellen der Datenbank erneut ein
    ''' </summary>
    ''' <param name="WithoutStaticContent">True, um nur die Tabellen Trades, Zeitstempelwerte, Importe und Kalkulationen neu zu laden</param>
    ''' <remarks></remarks>
    Public Sub ReloadDatabase(Optional WithoutStaticContent As Boolean = False)
        Dim items As Array
        items = System.Enum.GetValues(GetType(TableNames))
        Dim item As Long
        For Each item In items
            If WithoutStaticContent Then
                If item = TableNames.Importe Or _
                item = TableNames.Kalkulationen Or _
                item = TableNames.Trades Or _
                item = TableNames.ZeitstempelWerte Or _
                item = TableNames.TradesWerte Or _
                item = TableNames.Kurse Or _
                item = TableNames.ApiDaten Or _
                item = TableNames._AnyTable Then
                    Reset_DataAdapter(item)
                End If
            Else
                Reset_DataAdapter(item)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Füllt ein übergebenes Datagridview mit den Ergebnissen der SQL-Abfrage.
    ''' </summary>
    ''' <param name="SQL">SQL-Query, die gegen die Datenbank ausgeführt wird</param>
    ''' <param name="DataGridView">Zu befüllendes DataGridView</param>
    ''' <returns>DBObjects-Objekt</returns>
    Public Function FillDataGridView(SQL As String, ByRef DataGridView As DataGridView) As DBObjects
        Dim DBO As New DBObjects(SQL, _cnn)
        With DataGridView
            .SuspendLayout()
            .DataSource = Nothing
            .DataSource = DBO.DataTable
            .ResumeLayout()
        End With
        Return DBO
    End Function

End Class

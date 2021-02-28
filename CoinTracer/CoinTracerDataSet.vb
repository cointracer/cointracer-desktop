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

Imports CoinTracer.CoinValueStrategy
Imports CoinTracer.CoinTracerDataSet

Partial Class CoinTracerDataSet

    Private _cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
    End Property

    Public Sub New(Connection As SQLite.SQLiteConnection)
        MyBase.New()
        BeginInit()
        InitClass()
        Dim schemaChangedHandler As Global.System.ComponentModel.CollectionChangeEventHandler = AddressOf SchemaChanged
        AddHandler MyBase.Tables.CollectionChanged, schemaChangedHandler
        AddHandler MyBase.Relations.CollectionChanged, schemaChangedHandler
        EndInit()
        _cnn = Connection
    End Sub

    Public Sub ExecuteSQL(SQL As String)
        If _cnn.State <> ConnectionState.Open Then
            _cnn.Open()
        End If
        Dim cmd As New SQLite.SQLiteCommand(SQL, _cnn)
        cmd.ExecuteNonQuery()
        cmd.Dispose()
    End Sub

    Public Function FillTable(TableName As String, SQL As String) As Integer
        Dim TA As New SQLite.SQLiteDataAdapter(SQL, _cnn)
        Return TA.Fill(Tables(TableName))
        TA.Dispose()
    End Function

    Partial Public Class TradeTxDataTable
        Inherits TypedTableBase(Of TradeTxRow)

        Private _MaxID As Long
        Public Property MaxID() As Long
            Get
                _MaxID += 1
                Return _MaxID
            End Get
            Set(ByVal value As Long)
                _MaxID = value
            End Set
        End Property
    End Class

    Partial Public Class TradeTxRow
        Inherits DataRow
        ''' <summary>
        ''' Creates a new TradeTxRow object based on a given row. IstRest is set to True.
        ''' </summary>
        ''' <returns>The derived TradeTxRow object</returns>
        Public Function Derive() As TradeTxRow
            Dim NewRow As TradeTxRow = tableTradeTx.NewTradeTxRow
            With NewRow
                .TxID = tableTradeTx.MaxID
                .SzenarioID = CLng(Me(tableTradeTx.SzenarioIDColumn))
                .InKalkulationID = CLng(Me(tableTradeTx.InKalkulationIDColumn))
                .InTradeID = CLng(Me(tableTradeTx.InTradeIDColumn))
                .InTransferID = CLng(Me(tableTradeTx.InTransferIDColumn))
                .TransferIDHistory = CStr(Me(tableTradeTx.TransferIDHistoryColumn))
                .PlattformID = CLng(Me(tableTradeTx.PlattformIDColumn))
                .KontoID = CLng(Me(tableTradeTx.KontoIDColumn))
                .Zeitpunkt = CDate(Me(tableTradeTx.ZeitpunktColumn))
                .KaufZeitpunkt = CDate(Me(tableTradeTx.KaufZeitpunktColumn))
                .Betrag = CDec(Me(tableTradeTx.BetragColumn))
                .WertEUR = CDec(Me(tableTradeTx.WertEURColumn))
                .OutTradeID = 0
                .OutKalkulationID = 0
                .ParentTxID = 0
                .IstRest = True
                .Entwertet = False
                .IstLangzeit = False
            End With
            Return NewRow
        End Function
    End Class

End Class

' *
' * Tabellenspezifische TableAdapter-Erweiterungen
' *

Namespace CoinTracerDataSetTableAdapters
    Partial Public Class VW_GainingsReport2TableAdapter
    End Class

    Partial Public Class _VersionsTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet._VersionsDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class KalkulationenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.KalkulationenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class KonfigurationTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.KonfigurationDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class KurseTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.KurseDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class TradesTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.TradesDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class TradesWerteTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.TradesWerteDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class VW_TransfersTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_TransfersDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class ImporteTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.ImporteDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class TradeTypenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.TradeTypenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class KontenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.KontenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class ApiDatenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.ApiDatenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class PlattformenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.PlattformenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class

    Partial Public Class VW_GainingsReportDailyTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_GainingsReportDailyDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class


    Partial Public Class VW_BerechnungenTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_BerechnungenDataTable,
                                  Optional ByVal WhereEtcExpression As String = "") As Integer
            Dim stSelect As String
            Dim Count As Integer
            If (_commandCollection Is Nothing) Or ClearBeforeFill Then
                InitCommandCollection()
            End If
            stSelect = _commandCollection(0).CommandText
            If WhereEtcExpression.ToUpper.StartsWith("SELECT") Then
                ' Completely replace the select statement
                _commandCollection(0).CommandText = WhereEtcExpression
            Else
                ' Append the given expression
                If stSelect.ToUpper.Contains("ORDER BY") Then
                    _commandCollection(0).CommandText = stSelect.Replace("ORDER BY", WhereEtcExpression & " ORDER BY")
                Else
                    _commandCollection(0).CommandText &= " " & WhereEtcExpression
                End If
            End If
            Count = Fill(dataTable)
            Return Count
        End Function
    End Class


    Partial Public Class TradeTxTableAdapter
        Inherits System.ComponentModel.Component
        Public Overloads Function FillOpenValues(ByVal dataTable As TradeTxDataTable,
                                                 ByVal SzenarioID As Long,
                                                 ByVal PlattformID As Long,
                                                 ByVal KontoID As Long,
                                                 ByVal Zeitpunkt As DateTime,
                                                 ByVal ConsumptionStrategy As CoinValueStrategies) As Integer
            Adapter.SelectCommand = CommandCollection(1)
            Adapter.SelectCommand.Parameters(0).Value = SzenarioID
            Adapter.SelectCommand.Parameters(1).Value = PlattformID
            Adapter.SelectCommand.Parameters(2).Value = KontoID
            Adapter.SelectCommand.Parameters(3).Value = Zeitpunkt
            Dim SortSQL As String
            Select Case ConsumptionStrategy
                Case CoinValueStrategies.YoungestFirst
                    SortSQL = "KaufZeitpunkt DESC"
                Case CoinValueStrategies.CheapestFirst
                    SortSQL = "(WertEUR + 0.0000001)/Betrag"
                Case CoinValueStrategies.MostExpensiveFirst
                    SortSQL = "(WertEUR + 0.0000001)/Betrag DESC"
                Case Else
                    SortSQL = "KaufZeitpunkt ASC"
            End Select
            Dim CommandTextBackup As String = Adapter.SelectCommand.CommandText
            Adapter.SelectCommand.CommandText = Strings.Replace(Adapter.SelectCommand.CommandText, "@ConsumptionStrategy", SortSQL)
            If ClearBeforeFill Then
                dataTable.Clear()
            End If
            Dim ReturnValue As Integer = Adapter.Fill(dataTable)
            Adapter.SelectCommand.CommandText = CommandTextBackup
            Return ReturnValue
        End Function
    End Class

    Partial Public Class VW_GainingsReport2TableAdapter
        Inherits System.ComponentModel.Component

        Public Overloads Function FillByTimeScenarioTradeTypePlatforms(ByVal dataTable As VW_GainingsReport2DataTable,
                                                                       ByVal TimeFrom As DateTime,
                                                                       ByVal TimeTo As DateTime,
                                                                       ByVal SzenarioID As Long,
                                                                       ByVal TaxablesOnly As Long,
                                                                       ByVal TradesClass As Long,
                                                                       ByVal PlatformIDs As String) As Integer
            Adapter.SelectCommand = CommandCollection(1)
            Adapter.SelectCommand.Parameters(0).Value = TimeFrom
            Adapter.SelectCommand.Parameters(1).Value = TimeTo
            Adapter.SelectCommand.Parameters(2).Value = SzenarioID
            Adapter.SelectCommand.Parameters(3).Value = TaxablesOnly
            Adapter.SelectCommand.Parameters(4).Value = TradesClass
            If Not String.IsNullOrEmpty(PlatformIDs) Then
                PlatformIDs = String.Format("AND (_QuellPlattformID in ({0}) or _ZielPlattformID in ({0})) ", PlatformIDs)
            End If
            Dim CommandTextBackup As String = Adapter.SelectCommand.CommandText
            Adapter.SelectCommand.CommandText = Strings.Replace(Adapter.SelectCommand.CommandText, "AND (@PlatformIDs = 1)", PlatformIDs)
            If ClearBeforeFill Then
                dataTable.Clear()
            End If
            Dim ReturnValue As Integer = Adapter.Fill(dataTable)
            Adapter.SelectCommand.CommandText = CommandTextBackup
            Return ReturnValue
        End Function

        Public Overloads Function FillByTradeIDs(ByVal dataTable As VW_GainingsReport2DataTable,
                                                 ByVal TradeIdList As List(Of Long),
                                                 ByVal SzenarioID As Long,
                                                 ByVal TradesClass As Long) As Integer
            Adapter.SelectCommand = CommandCollection(2)
            Adapter.SelectCommand.Parameters(1).Value = SzenarioID
            Adapter.SelectCommand.Parameters(2).Value = TradesClass
            Dim CommandTextBackup As String = Adapter.SelectCommand.CommandText
            Adapter.SelectCommand.CommandText = Replace(Adapter.SelectCommand.CommandText, "= @TradeIDs", $"in ({String.Join(",", TradeIdList)})")
            If ClearBeforeFill Then
                dataTable.Clear()
            End If
            Dim ReturnValue As Integer = Adapter.Fill(dataTable)
            Adapter.SelectCommand.CommandText = CommandTextBackup
            Return ReturnValue
        End Function



    End Class


End Namespace


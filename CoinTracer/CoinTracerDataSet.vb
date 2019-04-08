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

    Partial Public Class ZeitstempelWerteTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.ZeitstempelWerteDataTable,
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

    Partial Public Class VW_GainingsReportTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_GainingsReportDataTable,
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


    Partial Public Class VW_InCoinsTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_InCoinsDataTable,
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

    Partial Public Class VW_OutCoinsTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.VW_OutCoinsDataTable,
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

    Partial Public Class Out2InTableAdapter
        Inherits System.ComponentModel.Component
        Public Function FillBySQL(ByVal dataTable As CoinTracerDataSet.Out2InDataTable,
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



End Namespace


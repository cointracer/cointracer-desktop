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

Public Class DBObjects
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If _DataAdapter IsNot Nothing Then
                    _DataAdapter.Dispose()
                    _DataAdapter = Nothing
                End If
                If _DataSet IsNot Nothing Then
                    _DataSet.Dispose()
                    _DataSet = Nothing
                End If
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
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

    Private _DataAdapter As SQLite.SQLiteDataAdapter
    Public ReadOnly Property DataAdapter() As SQLite.SQLiteDataAdapter
        Get
            Return _DataAdapter
        End Get
    End Property

    Private _DataSet As Data.DataSet
    Public ReadOnly Property DataSet() As Data.DataSet
        Get
            Return _DataSet
        End Get
    End Property

    Public ReadOnly Property DataTable() As Data.DataTable
        Get
            If _DataSet IsNot Nothing Then
                Return _DataSet.Tables(0)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private _cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
    End Property

    ''' <summary>
    ''' Schreibt Änderungen zurück in die Datenbank
    ''' </summary>
    Public Sub Update()
        If _DataAdapter IsNot Nothing Then
            Try
                Dim objCommandBuilder As New SQLite.SQLiteCommandBuilder(_DataAdapter)
                objCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges
                _DataAdapter.Update(Me.DataTable)
                objCommandBuilder.Dispose()
            Catch ex As Exception
                DefaultErrorHandler(ex)
            End Try
        End If
    End Sub

    Public Sub New(SQL As String, Connection As SQLite.SQLiteConnection, Optional TableName As String = "Table")
        Try
            _DataAdapter = New SQLite.SQLiteDataAdapter(SQL, Connection)
            _cnn = Connection
            If _cnn.State <> ConnectionState.Open Then
                _cnn.Open()
            End If
            _DataSet = New DataSet
            With _DataAdapter
                .MissingSchemaAction = MissingSchemaAction.AddWithKey
                Try
                    .Fill(_DataSet, TableName)
                Catch ex As Exception
                    If Left(ex.Message, Len("Connection was closed")) = "Connection was closed" Or ex.Message = "Database is not open" Then
                        _cnn.Open()
                        .Fill(_DataSet, TableName)
                    Else
                        Throw
                        Exit Sub
                    End If
                End Try
                If DataTable.Columns.Contains("ID") Then
                    DataTable.Columns("ID").AutoIncrementSeed = 1
                End If
            End With
        Catch ex As Exception
            _DataAdapter = Nothing
            _DataSet = Nothing
        End Try
    End Sub

    Public Sub New(SQL As String, Connection As SQLite.SQLiteConnection, TableIndex As DBHelper.TableNames)
        Me.New(SQL, Connection, System.Enum.GetName(GetType(DBHelper.TableNames), TableIndex))
    End Sub
End Class

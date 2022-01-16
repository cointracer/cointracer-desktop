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

Imports CoinTracer.DBHelper

Public Class dtoTradesRecord
    Implements ICloneable

    Private _ID As Long
    Public Property ID() As Long
        Get
            Return _ID
        End Get
        Set(ByVal value As Long)
            _ID = value
        End Set
    End Property

    Private _SourceID As String
    Public Property SourceID() As String
        Get
            Return _SourceID
        End Get
        Set(ByVal value As String)
            _SourceID = value
        End Set
    End Property

    Private _Zeitpunkt As Date
    Public Property Zeitpunkt() As Date
        Get
            Return _Zeitpunkt
        End Get
        Set(ByVal value As Date)
            _Zeitpunkt = value
        End Set
    End Property

    Private _InZeitpunkt As Date
    Public Property InZeitpunkt() As Date
        Get
            Return _InZeitpunkt
        End Get
        Set(ByVal value As Date)
            _InZeitpunkt = value
        End Set
    End Property

    Private _TradetypID As TradeTypen
    Public Property TradetypID() As TradeTypen
        Get
            Return _TradetypID
        End Get
        Set(ByVal value As TradeTypen)
            _TradetypID = value
        End Set
    End Property

    Private _QuellPlattformID As PlatformManager.Platforms
    Public Property QuellPlattformID() As PlatformManager.Platforms
        Get
            Return _QuellPlattformID
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _QuellPlattformID = value
        End Set
    End Property

    Private _QuellBetrag As Decimal
    Public Property QuellBetrag() As Decimal
        Get
            Return _QuellBetrag
        End Get
        Set(ByVal value As Decimal)
            _QuellBetrag = value
        End Set
    End Property

    Private _QuellBetragNachGebuehr As Decimal
    Public Property QuellBetragNachGebuehr() As Decimal
        Get
            Return _QuellBetragNachGebuehr
        End Get
        Set(ByVal value As Decimal)
            _QuellBetragNachGebuehr = value
        End Set
    End Property

    Private _QuellKontoID As AccountManager.Accounts
    Public Property QuellKontoID() As AccountManager.Accounts
        Get
            Return _QuellKontoID
        End Get
        Set(ByVal value As AccountManager.Accounts)
            _QuellKontoID = value
        End Set
    End Property

    Private _ZielPlattformID As PlatformManager.Platforms
    Public Property ZielPlattformID() As PlatformManager.Platforms
        Get
            Return _ZielPlattformID
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _ZielPlattformID = value
        End Set
    End Property

    Private _ZielBetrag As Decimal
    Public Property ZielBetrag() As Decimal
        Get
            Return _ZielBetrag
        End Get
        Set(ByVal value As Decimal)
            _ZielBetrag = value
        End Set
    End Property

    Private _ZielKontoID As AccountManager.Accounts
    Public Property ZielKontoID() As AccountManager.Accounts
        Get
            Return _ZielKontoID
        End Get
        Set(ByVal value As AccountManager.Accounts)
            _ZielKontoID = value
        End Set
    End Property

    Private _WertEUR As Decimal
    Public Property WertEUR() As Decimal
        Get
            Return _WertEUR
        End Get
        Set(ByVal value As Decimal)
            _WertEUR = value
        End Set
    End Property

    Private _Info As String
    Public Property Info() As String
        Get
            Return _Info
        End Get
        Set(ByVal value As String)
            _Info = value
        End Set
    End Property

    Private _BetragNachGebuehr As Decimal
    Public Property BetragNachGebuehr() As Decimal
        Get
            Return _BetragNachGebuehr
        End Get
        Set(ByVal value As Decimal)
            _BetragNachGebuehr = value
        End Set
    End Property

    Private _ImportPlattformID As PlatformManager.Platforms
    Public Property ImportPlattformID() As PlatformManager.Platforms
        Get
            Return _ImportPlattformID
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _ImportPlattformID = value
        End Set
    End Property

    Private _ImportID As Long
    Public Property ImportID() As Long
        Get
            Return _ImportID
        End Get
        Set(ByVal value As Long)
            _ImportID = value
        End Set
    End Property

    Private _RefTradeID As Long
    Public Property RefTradeID() As Long
        Get
            Return _RefTradeID
        End Get
        Set(ByVal value As Long)
            _RefTradeID = value
        End Set
    End Property

    Private _InTradeID As Long
    Public Property InTradeID() As Long
        Get
            Return _InTradeID
        End Get
        Set(ByVal value As Long)
            _InTradeID = value
        End Set
    End Property

    Private _OutTradeID As Long
    Public Property OutTradeID() As Long
        Get
            Return _OutTradeID
        End Get
        Set(ByVal value As Long)
            _OutTradeID = value
        End Set
    End Property

    Private _ZeitpunktZiel As Date
    Public Property ZeitpunktZiel() As Date
        Get
            Return _ZeitpunktZiel
        End Get
        Set(ByVal value As Date)
            _ZeitpunktZiel = value
        End Set
    End Property

    Private _Entwertet As Boolean
    Public Property Entwertet() As Boolean
        Get
            Return _Entwertet
        End Get
        Set(ByVal value As Boolean)
            _Entwertet = value
        End Set
    End Property

    Private _Steuerirrelevant As Boolean
    Public Property Steuerirrelevant() As Boolean
        Get
            Return _Steuerirrelevant
        End Get
        Set(ByVal value As Boolean)
            _Steuerirrelevant = value
        End Set
    End Property

    ' Feld nicht in Tabelle, wird aber für den Import verwendet
    Private _DoNotImport As Boolean
    Public Property DoNotImport() As Boolean
        Get
            Return _DoNotImport
        End Get
        Set(ByVal value As Boolean)
            _DoNotImport = value
        End Set
    End Property


    Public Function Clone() As Object Implements ICloneable.Clone
        Dim NewObj As New dtoTradesRecord
        ' Alles außer der ID setzen
        With NewObj
            .SourceID = _SourceID
            .Zeitpunkt = _Zeitpunkt
            .InZeitpunkt = _InZeitpunkt
            .TradetypID = _TradetypID
            .QuellPlattformID = _QuellPlattformID
            .QuellBetrag = _QuellBetrag
            .QuellBetragNachGebuehr = _QuellBetragNachGebuehr
            .QuellKontoID = _QuellKontoID
            .ZielPlattformID = _ZielPlattformID
            .ZielBetrag = _ZielBetrag
            .ZielKontoID = _ZielKontoID
            .WertEUR = _WertEUR
            .Info = _Info
            .BetragNachGebuehr = _BetragNachGebuehr
            .ImportPlattformID = _ImportPlattformID
            .ImportID = _ImportID
            .RefTradeID = _RefTradeID
            .InTradeID = _InTradeID
            .OutTradeID = _OutTradeID
            .Entwertet = _Entwertet
            .ZeitpunktZiel = _ZeitpunktZiel
            .Steuerirrelevant = _Steuerirrelevant
            .DoNotImport = _DoNotImport
        End With
        Return NewObj
    End Function

    ''' <summary>
    ''' Gibt ein DataRow-Objekt zur übergebenen Trades-Tabelle zurück
    ''' </summary>
    ''' <param name="RowIdCounter">Optional: Long-Variable, die um 1 erhöht wird (um LAST_INSERT_ID() zu simulieren)</param>
    Public Function GetNewDataRow(Table As Data.DataTable, Optional ByRef RowIdCounter As Long = -1) As Data.DataRow
        Dim Row As Data.DataRow
        Row = Table.NewRow()
        Me.SetDataRow(Row)
        If RowIdCounter <> -1 Then
            RowIdCounter += 1
        End If
        Return Row
    End Function

    ''' <summary>
    ''' Setzt die Felder des übergebenen DataRow-Objekts
    ''' </summary>
    Public Sub SetDataRow(ByRef Row As Data.DataRow)
        Row("SourceID") = _SourceID & ""
        Row("Zeitpunkt") = _Zeitpunkt
        If _InZeitpunkt = DATENULLVALUE Then
            Row("InZeitpunkt") = _Zeitpunkt
        Else
            Row("InZeitpunkt") = _InZeitpunkt
        End If
        Row("TradeTypID") = _TradetypID
        Row("QuellPlattformID") = _QuellPlattformID
        Row("QuellBetrag") = _QuellBetrag
        Row("QuellBetragNachGebuehr") = _QuellBetragNachGebuehr
        Row("QuellKontoID") = _QuellKontoID
        Row("ZielPlattformID") = _ZielPlattformID
        Row("ZielBetrag") = _ZielBetrag
        Row("ZielKontoID") = _ZielKontoID
        Row("WertEUR") = _WertEUR
        Row("Info") = _Info & ""
        Row("BetragNachGebuehr") = _BetragNachGebuehr
        Row("ImportID") = _ImportID
        Row("ImportPlattformID") = _ImportPlattformID
        Row("RefTradeID") = _RefTradeID
        Row("InTradeID") = _InTradeID
        Row("OutTradeID") = _OutTradeID
        Row("ZeitpunktZiel") = _ZeitpunktZiel
        Row("Entwertet") = _Entwertet
        Row("Steuerirrelevant") = _Steuerirrelevant
    End Sub

    ''' <summary>
    ''' Setzt die Werte des TradesRecord-Objekts anhand der übergebenen DataRow
    ''' </summary>
    ''' <param name="Row">DataRow aus der Trades-Tabelle</param>
    Public Sub SetFromDataRow(ByRef Row As Data.DataRow)
        _ID = Row("ID")
        _SourceID = Row("SourceID")
        _Zeitpunkt = Row("Zeitpunkt")
        _InZeitpunkt = Row("InZeitpunkt")
        _TradetypID = Row("TradeTypID")
        _QuellPlattformID = Row("QuellPlattformID")
        _QuellBetrag = Row("QuellBetrag")
        _QuellBetragNachGebuehr = Row("QuellBetragNachGebuehr")
        _QuellKontoID = Row("QuellKontoID")
        _ZielPlattformID = Row("ZielPlattformID")
        _ZielBetrag = Row("ZielBetrag")
        _ZielKontoID = Row("ZielKontoID")
        _WertEUR = Row("WertEUR")
        _Info = Row("Info") & ""
        _BetragNachGebuehr = Row("BetragNachGebuehr")
        _ImportID = If(Row.IsNull("ImportID"), 0, Row("ImportID"))
        _ImportPlattformID = If(Row.IsNull("ImportPlattformID"), 0, Row("ImportPlattformID"))
        _RefTradeID = If(Row.IsNull("RefTradeID"), 0, Row("RefTradeID"))
        _InTradeID = If(Row.IsNull("InTradeID"), 0, Row("InTradeID"))
        _OutTradeID = If(Row.IsNull("OutTradeID"), 0, Row("OutTradeID"))
        _ZeitpunktZiel = Row("ZeitpunktZiel")
        _Steuerirrelevant = If(Row.IsNull("Steuerirrelevant"), 0, Row("Steuerirrelevant"))
        _Entwertet = If(Row.IsNull("Entwertet"), 0, Row("Entwertet"))
    End Sub

    Public Sub New()
        _InZeitpunkt = DATENULLVALUE
    End Sub
    ''' <summary>
    ''' Initialisiert das Objekt mit den Werten der übergebenen Row
    ''' </summary>
    Public Sub New(ByRef Row As Data.DataRow)
        SetFromDataRow(Row)
    End Sub

End Class

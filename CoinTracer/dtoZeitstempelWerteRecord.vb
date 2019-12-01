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

Imports CoinTracer.DBHelper

Public Class dtoZeitstempelWerteRecord

    Public ID As Long
    Public Zeitpunkt As Date
    Public Kaufdatum As Date
    Public PlattformID As PlatformManager.Platforms
    Public Betrag As Decimal
    Public KontoID As Konten
    Public WertEUR As Decimal
    Public InTradeID As Long
    Public OutTradeID As Long
    Public ParentID As Long
    Public SzenarioID As Long
    Public AnlageKalkulationID As Long
    Public EntwertetKalkulationID As Long
    Public Entwertet As Boolean
    Public BestandPlattform As Decimal

    Public Function Clone(Optional ByRef RowIdCounter As Long = -1) As Object
        Dim NewObj As New dtoZeitstempelWerteRecord
        ' Alles außer der ID setzen, Entwertet immer False
        With NewObj
            .Zeitpunkt = Zeitpunkt
            .Kaufdatum = Kaufdatum
            .PlattformID = PlattformID
            .Betrag = Betrag
            .KontoID = KontoID
            .WertEUR = WertEUR
            .InTradeID = InTradeID
            .OutTradeID = OutTradeID
            .ParentID = ParentID
            .SzenarioID = SzenarioID
            .AnlageKalkulationID = AnlageKalkulationID
            .EntwertetKalkulationID = EntwertetKalkulationID
            .Entwertet = False
            .BestandPlattform = BestandPlattform
            If RowIdCounter <> -1 Then
                RowIdCounter += 1
                .ID = RowIdCounter
            End If
        End With
        Return NewObj
    End Function

    ''' <summary>
    ''' Gibt ein DataRow-Objekt zur übergebenen ZeitstempelWerte-Tabelle zurück
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
        Row("Zeitpunkt") = Zeitpunkt
        Row("Kaufdatum") = Kaufdatum
        Row("PlattformID") = PlattformID
        Row("Betrag") = Betrag
        Row("KontoID") = KontoID
        Row("WertEUR") = WertEUR
        Row("InTradeID") = InTradeID
        Row("OutTradeID") = OutTradeID
        Row("ParentID") = ParentID
        Row("SzenarioID") = SzenarioID
        Row("AnlageKalkulationID") = AnlageKalkulationID
        Row("EntwertetKalkulationID") = EntwertetKalkulationID
        Row("Entwertet") = Entwertet
        Row("BestandPlattform") = BestandPlattform
        If ID > 0 Then
            Row("ID") = ID
        End If
    End Sub

    ''' <summary>
    ''' Setzt die Werte des ZeitstempelWerteRecord-Objekts anhand der übergebenen DataRow
    ''' </summary>
    ''' <param name="Row">DataRow aus der ZeitstempelWerte-Tabelle</param>
    Public Sub SetFromDataRow(ByRef Row As Data.DataRow)
        ID = Row("ID")
        Zeitpunkt = Row("Zeitpunkt")
        Kaufdatum = Row("Kaufdatum")
        PlattformID = Row("PlattformID")
        Betrag = Row("Betrag")
        KontoID = Row("KontoID")
        WertEUR = Row("WertEUR")
        InTradeID = Row("InTradeID")
        OutTradeID = Row("OutTradeID")
        ParentID = Row("ParentID")
        SzenarioID = Row("SzenarioID")
        AnlageKalkulationID = Row("AnlageKalkulationID")
        EntwertetKalkulationID = Row("EntwertetKalkulationID")
        Entwertet = Row("Entwertet")
        BestandPlattform = Row("BestandPlattform")
    End Sub

    Public Sub New(Optional ByRef RowIdCounter As Long = -1)
        If RowIdCounter <> -1 Then
            RowIdCounter += 1
            ID = RowIdCounter
        End If
    End Sub
    ''' <summary>
    ''' Initialisiert das Objekt mit den Werten der übergebenen Row
    ''' </summary>
    Public Sub New(ByRef Row As Data.DataRow)
        SetFromDataRow(Row)
    End Sub

End Class

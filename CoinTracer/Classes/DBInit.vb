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

Imports System.IO
Imports System.Environment

''' <summary>
''' Bildet einen Fehler beim Update der Datenbank ab
''' </summary>
<Serializable()>
Public Class DatabaseUpdateException
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
''' Bildet einen Fehler beim Schreibzugriff auf einen Ordner ab
''' </summary>
<Serializable()>
Public Class DirectoryNotWritableException
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
''' Stellt der Anwendung Update-Funktionen zur Verfügung. U.a. können Datenbankschema-Anpassungen über verschiedene Versionen hinweg
''' vorgenommen werden.
''' </summary>
Public Class DBInit
    Implements IDisposable

    Public Enum DataBaseDirectories
        NotSpecified = -1
        UserAppDataDirectory = 0
        ApplicationDirectory
        CustomDirectory
    End Enum


    Private Structure SqlUpdateSequenceStruct
        Dim VersionID As Integer
        Dim UpdateSQL As String
        Dim MessageBoxIcon As System.Windows.Forms.MessageBoxIcon
        Dim Message As String
        Dim ResetExcelTemplates As Boolean
        Dim CustomAction As Integer
        Public Sub New(VersionID As Integer,
                       Optional UpdateSQL As String = "",
                       Optional Message As String = "",
                       Optional MessageBoxIcon As System.Windows.Forms.MessageBoxIcon = Windows.Forms.MessageBoxIcon.Exclamation,
                       Optional ResetExcelTemplates As Boolean = False,
                       Optional CustomAction As Integer = 0)
            Me.VersionID = VersionID
            Me.UpdateSQL = UpdateSQL
            Me.Message = Message
            Me.MessageBoxIcon = MessageBoxIcon
            Me.ResetExcelTemplates = ResetExcelTemplates
            Me.CustomAction = CustomAction
        End Sub
    End Structure

    Public Const DBDEFAULTNAME As String = "cointracer.data"


    ' *** Differenzielle SQL-Schema-Update-Anweisungen für diese Anwendungsversion ***
    Private SqlUpdateCommands() As SqlUpdateSequenceStruct = {
        New SqlUpdateSequenceStruct(3, "drop table if exists [Kurse]"),
        New SqlUpdateSequenceStruct(3, "drop INDEX if exists [IDX_Kurse_ZeitpunktKonten]"),
        New SqlUpdateSequenceStruct(3, "CREATE TABLE [Kurse] ( " & NewLine &
                                       "[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, " & NewLine &
                                       "[Zeitpunkt] TIMESTAMP  NULL, " & NewLine &
                                       "[QuellBetrag] NUMERIC  NULL DEFAULT '1', " & NewLine &
                                       "[QuellKontoID] INTEGER  NULL DEFAULT '101', " & NewLine &
                                       "[ZielBetrag] NUMERIC  NULL, " & NewLine &
                                       "[ZielKontoID] INTEGER  NULL, " & NewLine &
                                       "[Calculated] BOOLEAN DEFAULT '0' NULL)"),
        New SqlUpdateSequenceStruct(3, "CREATE INDEX [IDX_Kurse_ZeitpunktKonten] ON [Kurse] ( " & NewLine &
                                       "[Zeitpunkt]  ASC, " & NewLine &
                                       "[QuellKontoID]  ASC, " & NewLine &
                                       "[ZielKontoID]  ASC)"),
        New SqlUpdateSequenceStruct(3, "INSERT INTO [Plattformen] VALUES(204,'BTC-E.com','BTC-E','BTC-E.com',204,1,1,1);"),
        New SqlUpdateSequenceStruct(3, "ALTER TABLE [Konten] ADD COLUMN [IstGebuehr] BOOLEAN DEFAULT '0' NULL;"),
        New SqlUpdateSequenceStruct(3, "ALTER TABLE [Konten] ADD COLUMN [GebuehrKontoID] INTEGER DEFAULT '0' NULL;"),
        New SqlUpdateSequenceStruct(3, "ALTER TABLE [Kalkulationen] ADD COLUMN [CVS] VARCHAR(150)  NULL;"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET GebuehrKontoID=-1 WHERE ID=-1"),
        New SqlUpdateSequenceStruct(3, "UPDATE Plattformen set Bezeichnung='Extern', Code='Extern', Beschreibung='Externes Ziel, nicht mehr im eigenen Zugriff' where ID=900"),
        New SqlUpdateSequenceStruct(4, "INSERT INTO [Plattformen] VALUES(205,'Bitstamp.net','Bitstamp','Bitstamp.net',205,1,1,1);"),
        New SqlUpdateSequenceStruct(6, "ALTER TABLE [Trades] ADD COLUMN [Steuerirrelevant] BOOLEAN DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(6, "drop INDEX if exists [IDX_Trades_ZeitpunktEntwertet]"),
        New SqlUpdateSequenceStruct(6, "drop INDEX if exists [IDX_Trades_ZeitpunktEntwertetSteuer]"),
        New SqlUpdateSequenceStruct(6, "CREATE INDEX [IDX_Trades_ZeitpunktEntwertetSteuer] ON [Trades] (" & NewLine &
                                       "[Zeitpunkt]  ASC, " & NewLine &
                                       "[Entwertet]  ASC, " & NewLine &
                                       "[Steuerirrelevant]  ASC)"),
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_Konten"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_Konten] AS " & NewLine &
                                       "select " & NewLine &
                                       "ID, " & NewLine &
                                       "Bezeichnung, " & NewLine &
                                       "Code, " & NewLine &
                                       "Beschreibung, " & NewLine &
                                       "IstFiat Fiat, " & NewLine &
                                       "Eigen Eigenbesitz, " & NewLine &
                                       "SortID SortierNr, " & NewLine &
                                       "Fix IstFix, " & NewLine &
                                       "IstGebuehr [IstGebühr], " & NewLine &
                                       "GebuehrKontoID [GebührKontoID] " & NewLine &
                                       "from Konten " & NewLine &
                                       "order by SortID, ID"),
        New SqlUpdateSequenceStruct(6, "ALTER TABLE [Szenarien] ADD COLUMN [CVS] VARCHAR(150) NULL"),
        New SqlUpdateSequenceStruct(6, "delete from [Szenarien] where ID=0"),
        New SqlUpdateSequenceStruct(6, "INSERT INTO [Szenarien] VALUES (0,'Standard','2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1')"),
        New SqlUpdateSequenceStruct(6, "UPDATE Plattformen SET Fix=1 WHERE ID IN (0,900)"),
        New SqlUpdateSequenceStruct(9, "DELETE FROM [Plattformen] WHERE ID = 206"),
        New SqlUpdateSequenceStruct(9, "INSERT INTO [Plattformen] VALUES(206,'Kraken.com','Kraken','Kraken.com',206,1,1,1);"),
        New SqlUpdateSequenceStruct(9, "ALTER TABLE [Trades] ADD COLUMN [QuellBetragNachGebuehr] NUMERIC  NULL"),
        New SqlUpdateSequenceStruct(9, "UPDATE [Trades] SET QuellBetragNachGebuehr = QuellBetrag"),
        New SqlUpdateSequenceStruct(9, "update Trades set QuellBetragNachGebuehr = (" & NewLine &
                                       "select avg(tr.QuellBetrag - tg.QuellBetrag) from Trades tr" & NewLine &
                                       "inner join Trades tg on (tr.SourceID || '/fee' = tg.SourceID)" & NewLine &
                                       "where tr.ID = Trades.ID group by tr.ID)" & NewLine &
                                       "where ID IN (select t.ID from Trades t" & NewLine &
                                       "inner join Trades tg on (t.SourceID || '/fee' = tg.SourceID)" & NewLine &
                                       "where t.QuellKontoID = tg.QuellKontoID and t.ZielKontoID <> tg.QuellKontoID)"),
        New SqlUpdateSequenceStruct(9, "drop table if exists Szenarien2Cvs"),
        New SqlUpdateSequenceStruct(9, "PRAGMA writable_schema = 1"),
        New SqlUpdateSequenceStruct(9, "UPDATE SQLITE_MASTER SET SQL = replace(SQL, '[ZielKontoID] NUMERIC', '[ZielKontoID] INTEGER') WHERE NAME = 'Trades'"),
        New SqlUpdateSequenceStruct(9, "PRAGMA writable_schema = 0"),
        New SqlUpdateSequenceStruct(10, "drop table if exists [TradesWerte]"),
        New SqlUpdateSequenceStruct(10, "drop INDEX if exists [IDX_TradesWerte_TradeSzenario]"),
        New SqlUpdateSequenceStruct(10, "CREATE TABLE [TradesWerte] ( " & NewLine &
                                        "[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL," & NewLine &
                                        "[TradeID] INTEGER  NOT NULL," & NewLine &
                                        "[WertEUR] NUMERIC  NULL," & NewLine &
                                        "[SzenarioID] INTEGER  NOT NULL," & NewLine &
                                        "[AnlageKalkulationID] INTEGER  NULL)"),
        New SqlUpdateSequenceStruct(10, "CREATE INDEX [IDX_TradesWerte_TradeSzenario] ON [TradesWerte] ( " & NewLine &
                                        "[TradeID]  ASC," & NewLine &
                                        "[SzenarioID]  ASC)"),
        New SqlUpdateSequenceStruct(10, "update Trades set WertEUR = 0 where ID in (" &
                                        "select t.ID from Trades t " &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID " &
                                        "where t.TradeTypID in (3,5) " &
                                        "and qk.IstFiat = 0)"),
        New SqlUpdateSequenceStruct(10, "delete from Kalkulationen",
                                    "Bitte beachten Sie, dass aufgrund einer notwendigen Datenstruktur-Änderung alle Gewinnberechnungen zurückgesetzt wurden. " &
                                    "Sie können diese im Reiter 'Berechnungen' aber problemlos erneut durchführen."),
        New SqlUpdateSequenceStruct(11, , , , True),
        New SqlUpdateSequenceStruct(12, , , , True),
        New SqlUpdateSequenceStruct(14, "drop view if exists VW_Kurse"),
        New SqlUpdateSequenceStruct(14, "CREATE VIEW [VW_Kurse] AS" & NewLine &
                                        "select k.ID ID," & NewLine &
                                        "k.Zeitpunkt Zeitpunkt," & NewLine &
                                        "qk.Bezeichnung Konto," & NewLine &
                                        "k.QuellBetrag Betrag," & NewLine &
                                        "zk.Bezeichnung ZielKonto," & NewLine &
                                        "k.ZielBetrag ZielBetrag," & NewLine &
                                        "k.Calculated Errechnet" & NewLine &
                                        "from Kurse k" & NewLine &
                                        "left join Konten qk on k.QuellKontoID = qk.ID" & NewLine &
                                        "left join Konten zk on k.ZielKontoID = zk.ID" & NewLine &
                                        "order by qk.SortID, zk.SortID, k.Zeitpunkt, k.ID"),
        New SqlUpdateSequenceStruct(15, "ALTER TABLE [Plattformen] ADD COLUMN [ApiBaseUrl] VARCHAR(250) NULL;"),
        New SqlUpdateSequenceStruct(15, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] ='https://api.kraken.com/' WHERE ID = {0}", CInt(PlatformManager.Platforms.Kraken))),
        New SqlUpdateSequenceStruct(15, "drop table if exists ApiDaten"),
        New SqlUpdateSequenceStruct(15, "CREATE TABLE [ApiDaten] (" & NewLine &
                                        "[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL," & NewLine &
                                        "[PlattformID] INTEGER  NULL," & NewLine &
                                        "[Zeitpunkt] TIMESTAMP  NOT NULL," & NewLine &
                                        "[Bezeichnung] VARCHAR(150)  NULL," & NewLine &
                                        "[ApiKey] VARCHAR(250)  NULL," & NewLine &
                                        "[ApiSecret] VARCHAR(350)  NULL," & NewLine &
                                        "[Salt] VARCHAR(250)  NULL," & NewLine &
                                        "[Aktiv] BOOLEAN DEFAULT '1'," & NewLine &
                                        "[LastImportTimestamp] INTEGER DEFAULT '0' NULL)"),
        New SqlUpdateSequenceStruct(15, , "Wichtige Information zu Trade-Daten von Kraken.com!" & NewLine & NewLine &
                                    "Im Rahmen der neuen Funktion des Abrufs von Trade-Daten über die API von Kraken.com abzurufen, musste das interne Datenformat " &
                                    "leicht erweitert werden. Sollten Sie mit einer älteren CoinTracer-Version als 0.8.6 bereits Daten von Kraken " &
                                    "importiert haben, so sollten Sie diese Importe sicherheitshalber wieder zurücksetzen. Andernfalls kann es zu " &
                                    "doppelt importierten Trades kommen."),
        New SqlUpdateSequenceStruct(16, "INSERT INTO [Plattformen] VALUES(103,'MultiBit','MultiBit','MultiBit-Wallet-Client',103,1,0,1,NULL);"),
        New SqlUpdateSequenceStruct(19, "DELETE FROM [Plattformen] WHERE ID = 207"),
        New SqlUpdateSequenceStruct(19, "INSERT INTO [Plattformen] VALUES(207,'Bitfinex.com','Bitfinex','Bitfinex.com',207,1,1,1,'https://api.bitfinex.com/');"),
        New SqlUpdateSequenceStruct(20, "drop table if exists Konfiguration"),
        New SqlUpdateSequenceStruct(20, "CREATE TABLE [Konfiguration] (" & NewLine &
                                        "[ID] INTEGER  PRIMARY KEY NOT NULL," & NewLine &
                                        "[Bezeichnung] VARCHAR(150)  NULL," & NewLine &
                                        "[Wert] VARCHAR(250)  NULL)" & NewLine),
        New SqlUpdateSequenceStruct(20, "INSERT INTO [Konfiguration] VALUES(1,'Steuergrenze','+1 years');"),
        New SqlUpdateSequenceStruct(20, "ALTER TABLE [Trades] ADD COLUMN [Kommentar] VARCHAR(200) NULL;"),
        New SqlUpdateSequenceStruct(20, "drop view if exists VW_Trades"),
        New SqlUpdateSequenceStruct(20, "CREATE VIEW [VW_Trades] AS " & NewLine &
                                        "select " & NewLine &
                                        "t.ID ID, " & NewLine &
                                        "t.Zeitpunkt Zeitpunkt, " & NewLine &
                                        "tt.Bezeichnung Transaktion, " & NewLine &
                                        "qp.Bezeichnung QuellPlattform, " & NewLine &
                                        "qk.Bezeichnung QuellKonto, " & NewLine &
                                        "t.QuellBetrag QuellBetrag, " & NewLine &
                                        "t.QuellBetragNachGebuehr QuellBetragNetto, " & NewLine &
                                        "zp.Bezeichnung ZielPlattform, " & NewLine &
                                        "zk.Bezeichnung ZielKonto, " & NewLine &
                                        "t.ZielBetrag ZielBetrag, " & NewLine &
                                        "t.BetragNachGebuehr ZielBetragNetto, " & NewLine &
                                        "t.WertEUR WertEUR, " & NewLine &
                                        "t.SourceID Referenz, " & NewLine &
                                        "t.Info Info, " & NewLine &
                                        "t.Steuerirrelevant SteuerIgnorieren, " & NewLine &
                                        "t.Kommentar " & NewLine &
                                        "from Trades t " & NewLine &
                                        "left join Konten qk on t.QuellKontoID=qk.ID " & NewLine &
                                        "left join Konten zk on t.ZielKontoID=zk.ID " & NewLine &
                                        "left join Plattformen qp on t.QuellPlattformID=qp.ID " & NewLine &
                                        "left join Plattformen zp on t.ZielPlattformID=zp.ID " & NewLine &
                                        "left join Plattformen ip on t.ImportPlattformID=ip.ID " & NewLine &
                                        "left join TradeTypen tt on t.TradeTypID=tt.ID " & NewLine &
                                        "where t.Entwertet = 0 " & NewLine &
                                        "order by t.Zeitpunkt, t.ID"),
        New SqlUpdateSequenceStruct(20, "ALTER TABLE [Plattformen] ADD COLUMN [IstDown] BOOLEAN DEFAULT '0' NULL;"),
        New SqlUpdateSequenceStruct(20, "ALTER TABLE [Plattformen] ADD COLUMN [DownSeit] TIMESTAMP NULL;"),
        New SqlUpdateSequenceStruct(20, "drop view if exists VW_Plattformen"),
        New SqlUpdateSequenceStruct(20, "CREATE VIEW [VW_Plattformen] AS " & NewLine &
                                        "select " & NewLine &
                                        "ID, " & NewLine &
                                        "Bezeichnung, " & NewLine &
                                        "Code, " & NewLine &
                                        "Beschreibung, " & NewLine &
                                        "Boerse [IstBörse], " & NewLine &
                                        "Eigen Eigenbesitz, " & NewLine &
                                        "SortID SortierNr, " & NewLine &
                                        "case IstDown when 0 then null else DownSeit end IstDownSeit, " & NewLine &
                                        "Fix IstFix " & NewLine &
                                        "from Plattformen " & NewLine &
                                        "order by SortID, ID"),
        New SqlUpdateSequenceStruct(20, "delete from [TradeTypen] where ID=7"),
        New SqlUpdateSequenceStruct(20, "INSERT INTO [TradeTypen] VALUES (7,'Verlust','LOSS','Verlust von Coins oder Fiat',7)"),
        New SqlUpdateSequenceStruct(21, "drop view if exists VW_ZugangAbgang"),
        New SqlUpdateSequenceStruct(21, "CREATE VIEW [VW_ZugangAbgang] AS " & NewLine &
                                        "select" & NewLine &
                                        "	ZielBetrag Betrag," & NewLine &
                                        "	ZielKontoID KontoID," & NewLine &
                                        "	ZielPlattformID Plattform," & NewLine &
                                        "	Zeitpunkt, 1 SollHaben," & NewLine &
                                        "	case when TradeTypID = 3 and QuellKontoID = 101 then QuellBetrag else NULL end BetragEUR," & NewLine &
                                        "	case when TradeTypID = 3 and QuellKontoID = 102 then QuellBetrag else NULL end BetragUSD," & NewLine &
                                        "	BetragNachGebuehr _BetragNetto " & NewLine &
                                        "from Trades where Entwertet=0" & NewLine &
                                        "union all select" & NewLine &
                                        "	-QuellBetragNachGebuehr," & NewLine &
                                        "	QuellKontoID," & NewLine &
                                        "	QuellPlattformID," & NewLine &
                                        "	Zeitpunkt," & NewLine &
                                        "	0," & NewLine &
                                        "	case when TradeTypID = 4 and ZielKontoID = 101 then -BetragNachGebuehr when TradeTypID = 7 then 0 else NULL end," & NewLine &
                                        "	case when TradeTypID = 4 and ZielKontoID = 102 then -BetragNachGebuehr when TradeTypID = 7 then 0 else NULL end," & NewLine &
                                        "	-QuellBetragNachGebuehr " & NewLine &
                                        "from Trades" & NewLine &
                                        "where Entwertet = 0" & NewLine &
                                        "order by Zeitpunkt"),
        New SqlUpdateSequenceStruct(22, "UPDATE [Plattformen] SET [IstDown] = 0 WHERE [IstDown] ISNULL"),
        New SqlUpdateSequenceStruct(23, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] = 'https://api.bitcoin.de/v1/' WHERE ID ={0}", CInt(PlatformManager.Platforms.BitcoinDe))),
        New SqlUpdateSequenceStruct(25, "DELETE FROM [Plattformen] WHERE ID = 208"),
        New SqlUpdateSequenceStruct(25, "INSERT INTO [Plattformen] VALUES(208,'Zyado.com','Zyado','Zyado.com',208,1,1,1,NULL,0,NULL);"),
        New SqlUpdateSequenceStruct(26, "alter table [Importe] add column [LastImportTimestamp] INTEGER DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(26, "alter table [Importe] add column [ApiDatenID] INTEGER DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(26, "drop view if exists VW_Importe"),
        New SqlUpdateSequenceStruct(26, "CREATE VIEW [VW_Importe] AS " & NewLine &
                                       "select " & NewLine &
                                       "i.ID ID, " & NewLine &
                                       "p.Bezeichnung Plattform, " & NewLine &
                                       "i.Zeitpunkt Zeitpunkt, " & NewLine &
                                       "i.Dateiname Dateiname, " & NewLine &
                                       "i.PfadDateiname Pfad, " & NewLine &
                                       "i.Eingelesen Eingelesen, " & NewLine &
                                       "i.NichtEingelesen [Übersprungen], " & NewLine &
                                       "i.ApiDatenID " & NewLine &
                                       "from Importe i " & NewLine &
                                       "inner join Plattformen p on i.PlattformID=p.ID " & NewLine &
                                       "order by i.Zeitpunkt, i.ID"),
        New SqlUpdateSequenceStruct(27, "ALTER TABLE [ApiDaten] ADD COLUMN [ExtendedInfo] VARCHAR(100) DEFAULT '' NULL"),
        New SqlUpdateSequenceStruct(27, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] ='https://api.bitfinex.com/' WHERE ID = {0}", CInt(PlatformManager.Platforms.Bitfinex))),
        New SqlUpdateSequenceStruct(VersionID:=32, CustomAction:=1),
        New SqlUpdateSequenceStruct(33, "drop view if exists VW_Transfers"),
        New SqlUpdateSequenceStruct(33, "CREATE VIEW [VW_Transfers] AS " & NewLine &
                                        "select " & NewLine &
                                        "t.ID ID, " & NewLine &
                                        "t.Zeitpunkt Zeitpunkt, " & NewLine &
                                        "tt.Bezeichnung Transaktion, " & NewLine &
                                        "qk.Bezeichnung QuellKonto, " & NewLine &
                                        "t.QuellBetrag QuellBetrag, " & NewLine &
                                        "qp.Bezeichnung QuellPlattform, " & NewLine &
                                        "t.QuellPlattformID, " & NewLine &
                                        "zp.Bezeichnung ZielPlattform, " & NewLine &
                                        "t.ZielPlattformID, " & NewLine &
                                        "t.Info Info, " & NewLine &
                                        "t.ImportID " & NewLine &
                                        "from Trades t " & NewLine &
                                        "left join Konten qk on t.QuellKontoID=qk.ID " & NewLine &
                                        "left join Konten zk on t.ZielKontoID=zk.ID " & NewLine &
                                        "left join Plattformen qp on t.QuellPlattformID=qp.ID " & NewLine &
                                        "left join Plattformen zp on t.ZielPlattformID=zp.ID " & NewLine &
                                        "left join TradeTypen tt on t.TradeTypID=tt.ID " & NewLine &
                                        "where t.Entwertet = 0  and t.TradeTypID = 5" & NewLine &
                                        "order by t.Zeitpunkt, t.ID"),
        New SqlUpdateSequenceStruct(34, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(VersionID:=35, CustomAction:=2),
        New SqlUpdateSequenceStruct(36, "UPDATE Konten SET Code = 'BCH' WHERE Code = 'BCC'"),
        New SqlUpdateSequenceStruct(36, "UPDATE Konten SET Code = 'feeBCH' WHERE Code = 'feeBCC'"),
        New SqlUpdateSequenceStruct(37, "drop view if exists VW_Berechnungen"),
        New SqlUpdateSequenceStruct(37, "CREATE VIEW [VW_Berechnungen] AS " & NewLine &
                                        "select " & NewLine &
                                        "k.ID ID, " & NewLine &
                                        "DATE(k.Zeitpunkt, '-1 days') [Berechung bis], " & NewLine &
                                        "s.Bezeichnung Szenario, " & NewLine &
                                        "k.CVS " & NewLine &
                                        "from Kalkulationen k " & NewLine &
                                        "inner join Szenarien s on k.SzenarioID = s.ID " & NewLine &
                                        "order by k.Zeitpunkt, s.ID"),
        New SqlUpdateSequenceStruct(37, "update Konten set Fix = 1 where Fix = 'Y'"),
        New SqlUpdateSequenceStruct(VersionID:=37, CustomAction:=3),
        New SqlUpdateSequenceStruct(VersionID:=40, CustomAction:=4, Message:=My.Resources.MyStrings.dbUpdateMsgReportReset),
        New SqlUpdateSequenceStruct(VersionID:=43, CustomAction:=-1)    ' <-- just insert the latest db version number here
    }


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If _cnn IsNot Nothing Then
                    _cnn.Dispose()
                End If
            End If
            ' TO DO: free unmanaged resources (unmanaged objects) and override Finalize() below.
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

    Private _ApplicationName As String

    Private _cnn As SQLite.SQLiteConnection
    Public ReadOnly Property Connection() As SQLite.SQLiteConnection
        Get
            Return _cnn
        End Get
    End Property

    Private _LocalDBVersionID As Integer
    Public ReadOnly Property LocalDBVersionID() As Integer
        Get
            Return _LocalDBVersionID
        End Get
    End Property

    Private _DBName As String
    Public ReadOnly Property DatabaseName() As String
        Get
            Return _DBName
        End Get
    End Property

    Public ReadOnly Property DatabaseFile() As String
        Get
            Return DatabaseDirectory & _DBName
        End Get
    End Property

    Public ReadOnly Property DatabaseDirectory(Optional DirectoryType As DataBaseDirectories = DataBaseDirectories.NotSpecified) As String
        Get
            Select Case DirectoryType
                Case DataBaseDirectories.UserAppDataDirectory
                    Return GetFolderPath(SpecialFolder.ApplicationData) & "\" & _ApplicationName & "\"
                Case DataBaseDirectories.ApplicationDirectory
                    Return Path.GetDirectoryName(Application.ExecutablePath) & "\"
                Case Else
                    If My.Settings.DataDirectory.Length = 0 Then
                        Return Me.DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory)
                    Else
                        Return Path.GetDirectoryName(My.Settings.DataDirectory) & "\"
                    End If
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Setzt das DatabaseDirectory neu und erneuert bei Bedarf die übergebene Connection. Wirft im Fehlerfall eine DirectoryNotWritableException.
    ''' </summary>
    ''' <param name="DirectoryPath">Datenbank-Pfad (nur notwendig bei DirectoryType CustomDirectory)</param>
    ''' <param name="DirectoryType">Typ des Datenbank-Pfads</param>
    ''' <param name="Connection">Connection, die ggf. neu gesetzt wird</param>
    ''' <returns>True, wenn die Datenbank erfolgreich verschoben werden musste, sonst False</returns>
    Public Function SetDatabaseDirectory(ByRef Connection As SQLite.SQLiteConnection,
                                         ByVal DirectoryType As DataBaseDirectories,
                                         Optional ByVal DirectoryPath As String = "") As Boolean
        Dim DataDir As String
        Select Case DirectoryType
            Case DataBaseDirectories.UserAppDataDirectory
                DataDir = GetFolderPath(SpecialFolder.ApplicationData) & "\" & _ApplicationName & "\"
            Case DataBaseDirectories.ApplicationDirectory
                DataDir = Path.GetDirectoryName(Application.ExecutablePath) & "\"
            Case Else
                DataDir = DirectoryPath & If(DirectoryPath.EndsWith("\"), "", "\")
        End Select
        If My.Settings.DataDirectory <> DataDir Then
            If CheckIfWritable(DataDir) Then
                Dim TargetDBFile As String = DataDir & DatabaseName
                Try
                    If File.Exists(TargetDBFile) Then
                        File.Copy(TargetDBFile, TargetDBFile & ".bak", True)
                    End If
                    File.Copy(DatabaseFile, TargetDBFile, True)
                Catch ex As Exception
                    Throw New DirectoryNotWritableException(String.Format("Die Datenbank konnte nicht in den Ordner {0} kopiert werden.", DataDir))
                    Return False
                End Try
                My.Settings.DataDirectory = DataDir
                SetConnection(DatabaseFile)
                Connection = _cnn
                Return True
            Else
                Throw New DirectoryNotWritableException(String.Format("Auf den Ordner '{0}' kann nicht schreibend zugegriffen werden.", DataDir))
                Return False
            End If
        Else
            Return False
        End If
    End Function


    ''' <summary>
    ''' Prüft, ob in dem übergebenen Ordner geschrieben werden darf
    ''' </summary>
    ''' <param name="DirectoryPath">Verzeichnis, das überprüft werden soll</param>
    Public Function CheckIfWritable(DirectoryPath As String) As Boolean
        If Not Directory.Exists(DirectoryPath) Then
            Try
                Directory.CreateDirectory(DirectoryPath)
            Catch ex As Exception
                Return False
                Exit Function
            End Try
        End If
        Dim TempFilePath As String = DirectoryPath & If(DirectoryPath.EndsWith("\"), "", "\")
        Randomize()
        Dim TempFileName = MD5FromString(CStr(Rnd()) & CStr(Rnd()))
        TempFilePath &= TempFileName
        Try
            File.WriteAllText(TempFilePath, "check")
        Catch ex As Exception
            Return False
            Exit Function
        End Try
        Dim Writable As Boolean = True
        ' Nicht schön, kommt aber ohne .manifest aus: prüfen, ob die Datei (unter Win7/Vista) im VirtualStore gelandet ist...
        If TempFilePath.Substring(1, 1) = ":" Then
            If File.Exists(GetFolderPath(SpecialFolder.LocalApplicationData) & "\VirtualStore" & TempFilePath.Substring(2)) Then
                Writable = False
            End If
        End If
        File.Delete(TempFilePath)
        Return Writable
    End Function


    ''' <summary>
    ''' Initialisiert die Datenbank, legt sie ggf. an, baut die Verbindung auf und liest aktuelle Version ein
    ''' </summary>
    Public Sub InitDatabase(Optional ByRef DatabaseFilename As String = DBDEFAULTNAME)
        _DBName = DatabaseFilename
        ' Prüfen, ob Datenbank vorhanden ist
        Dim DBFile As String
        DBFile = DatabaseFile
        If Not File.Exists(DBFile) Then
            ' Offenbar der erste Start - ggf. fragen, wohin die Datenbank gespeichert werden soll.
            If CheckIfWritable(DatabaseDirectory(DataBaseDirectories.ApplicationDirectory)) Then
                MsgBoxEx.PatchMsgBox(New String() {My.Resources.MyStrings.initMsgSelectDbFolderOptionUser, My.Resources.MyStrings.initMsgSelectDbFolderOptionGlobal})
                If MessageBox.Show(My.Resources.MyStrings.initMsgSelectDbFolder, My.Resources.MyStrings.initMsgSelectDbFolderTitle,
                                   MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) = DialogResult.Retry Then
                    My.Settings.DataDirectory = DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory)
                Else
                    My.Settings.DataDirectory = DatabaseDirectory(DataBaseDirectories.ApplicationDirectory)
                End If
                DBFile = DatabaseFile
            Else
                My.Settings.DataDirectory = DatabaseDirectory(DataBaseDirectories.UserAppDataDirectory)
            End If
            Dim FFSP As New FlexFileSettingsProvider
            FFSP.UserConfigFile = Path.Combine(DatabaseDirectory, FFSP.GetUserConfigFileName)
        End If
        If Not Directory.Exists(Path.GetDirectoryName(DBFile)) Then
            Try
                Directory.CreateDirectory(Path.GetDirectoryName(DBFile))
            Catch ex As Exception
                Throw
                Exit Sub
            End Try
        End If
        If Not File.Exists(DBFile) Then
            ' Default-Datenbank kopieren
            Try
                File.WriteAllBytes(DBFile, My.Resources.cointracerDefault)
            Catch ex As Exception
                Throw
                Exit Sub
            End Try
        End If

        ' Connection aufbauen
        SetConnection(DBFile)

        ' aktuelle Version holen
        _LocalDBVersionID = GetVersionIDFromDB()

    End Sub

    ''' <summary>
    ''' Setzt die Connection anhand des Datenbank-Files und schreibt die Eigenschaft CoinTracerCS
    ''' </summary>
    ''' <param name="DatabaseFile">Vollständiger Pfad zur Datenbank-Datei</param>
    Private Sub SetConnection(ByVal DatabaseFile As String)
        DatabaseFile = "Data Source=" & DatabaseFile.Trim & ";Version=3;"
        My.Settings.CoinTracerCS = DatabaseFile
        _cnn = New SQLite.SQLiteConnection(DatabaseFile)
        Try
            _cnn.Open()
        Catch ex As Exception
            Throw
            Exit Sub
        End Try
    End Sub

    Public Sub New()
        _DBName = DBDEFAULTNAME
        _cnn = Nothing
        _Interactive = True
#If CONFIG = "Debug" Then
        _ApplicationName = "CoinTracer"
#Else
        _ApplicationName = Application.ProductName
#End If
    End Sub

    Public Sub New(Connection As SQLite.SQLiteConnection)
        Me.New()
        _cnn = Connection
    End Sub

    Private _Interactive As Boolean
    ''' <summary>
    ''' Determines if we are running in user-interactive mode or not
    ''' </summary>
    Public Property Interactive() As Boolean
        Get
            Return _Interactive
        End Get
        Set(ByVal value As Boolean)
            _Interactive = value
        End Set
    End Property

    ''' <summary>
    ''' Liest die höchste Datenbank-Versions-ID aus der lokalen Datenbank
    ''' </summary>
    ''' <returns>Höchste ID aus der Tabelle _Versionen, 0 bei leerer Tabelle</returns>
    Private Function GetVersionIDFromDB() As Integer
        Return QueryDBInteger("select ID from _Versions order by ID desc limit 1")
    End Function

    ''' <summary>
    ''' Executes a database query and returns an integer value
    ''' </summary>
    ''' <param name="SQL">SQL for database query</param>
    ''' <param name="ReturnFieldName">name of the field whose value is returned (first row of query)</param>
    ''' <returns>Value of field 'ReturnFieldName', taken from first row of dataset</returns>
    Private Function QueryDBInteger(SQL As String, Optional ReturnFieldName As String = "ID") As Integer
        Try
            Dim DBO As New DBObjects(SQL, _cnn)
            If DBO IsNot Nothing AndAlso DBO.DataTable.Rows.Count > 0 Then
                Return CInt(DBO.DataTable.Rows(0)(ReturnFieldName))
            Else
                Throw New Exception(String.Format("Die Datenbankabfrage konnte nicht erfolgreich durchgeführt werden!"))
            End If
        Catch ex As Exception
            Throw New Exception(String.Format("Die lokale Datenbank '{0}' konnte nicht geöffnet werden!", DatabaseFile))
        End Try
    End Function

    ''' <summary>
    ''' Führt ein Datenbankupdate auf die aktuelle Version durch
    ''' </summary>
    ''' <returns>True, wenn ein Update notwendig war - sonst False</returns>
    Public Function UpdateDatabase() As Boolean
        Dim RequiredVersionID As Integer = SqlUpdateCommands(SqlUpdateCommands.GetUpperBound(0)).VersionID
        If _LocalDBVersionID < RequiredVersionID Then
            Dim SQLUp As SqlUpdateSequenceStruct
            Dim DBHelp As New DBHelper(_cnn)
            Try
                For Each SQLUp In SqlUpdateCommands
                    If _LocalDBVersionID < SQLUp.VersionID Then
                        If SQLUp.UpdateSQL.Length > 0 Then
                            Try
                                DBHelp.ExecuteSQL(SQLUp.UpdateSQL)
                            Catch ex As Exception
                                ' Fehler 'duplicate column' ignorieren, ansonsten Error werfen
                                If (Not ex.Message.Contains("duplicate column name")) And (Not ex.Message.Contains("already exists")) And (Not ex.Message.Contains(" columns but ")) Then
                                    Throw
                                End If
                            End Try
                        End If
                        If SQLUp.ResetExcelTemplates Then
                            ' Alle Excel-Dateien löschen
                            For Each XltFile As String In Directory.GetFiles(DatabaseDirectory, "*.xlt")
                                File.Delete(XltFile)
                            Next
                        End If
                        If _Interactive AndAlso SQLUp.Message <> "" Then
                            MessageBox.Show(SQLUp.Message, "Datenbank-Update", MessageBoxButtons.OK, SQLUp.MessageBoxIcon)
                        End If
                        If SQLUp.CustomAction > 0 Then
                            ProcessCustomAction(SQLUp.CustomAction)
                        End If
                        If SQLUp.CustomAction <= 0 Then
                            ProcessSqlRessourceStrings(SQLUp.VersionID)
                        End If
                    End If
                Next
                ' Kursdaten ggf. aktualisieren
                Me.InitCourseData()
                ' Check if platforms are all right
                PlatformManager.PlatformsSyncDB(_cnn)
                ' Check if accounts are in sync
                AccountManager.AccountsSyncDB(_cnn)
                ' Versions-ID in Datenbank schreiben
                Dim myBuildInfo As FileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath)
                DBHelp.ExecuteSQL("delete from _Versions")
                Dim SQL As String = "insert into _Versions(ID, Major, Minor, Revision) values(" & RequiredVersionID & "," &
                    myBuildInfo.FileMajorPart & "," &
                    myBuildInfo.FileMinorPart & "," &
                    myBuildInfo.FileBuildPart & ")"
                DBHelp.ExecuteSQL(SQL)
            Catch ex As Exception
                Throw New DatabaseUpdateException(String.Format(My.Resources.MyStrings.initDbUpdateError, NewLine, ex.Message), ex)
            End Try
            Return True
        ElseIf _LocalDBVersionID > RequiredVersionID Then
            ' Warnung bei zu hoher Datenbankversion (offenbar gab es ein Downgrade der Applikation)
            MsgBoxEx.PatchMsgBox(New String() {"Fortfahren", "Beenden"})
            If _Interactive AndAlso MessageBox.Show(String.Format("Achtung: Der {0}-Datenbestand gehört zu einer neueren Version als " &
                               "{1}. Es könnte sein, dass der {0} mit diesen Daten nicht korrekt arbeitet oder inkonsistente " &
                               "Daten erzeugt.", _ApplicationName, Application.ProductVersion) & Environment.NewLine & Environment.NewLine &
                               "Möchten Sie fortfahren oder das Programm jetzt beenden?", "Veraltete Version gestartet",
                               MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) = DialogResult.Retry Then
            Else
                Application.Exit()
            End If
            Return False
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Loops over every My.Resources.SQLs and executes all Statements according to the given DB version
    ''' </summary>
    ''' <param name="DBVersion">Maximum database version. Every SQL statement with version smaller than or equal to this parameter will be executed, so long as it is higher than the current local database version</param>
    Private Sub ProcessSqlRessourceStrings(ByVal DBVersion As Integer)
        Dim ItemResourceSet As Resources.ResourceSet = My.Resources.SQLs.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, True, True)
        Dim ItemEnumerator As IDictionaryEnumerator = ItemResourceSet.GetEnumerator
        Dim SqlStrings As New List(Of String)
        Dim CurrentKey As String
        Dim CurrentSql As String
        Dim CurrentSqlVersion As Integer
        Do While ItemEnumerator.MoveNext
            CurrentKey = ItemEnumerator.Key.ToString
            CurrentSqlVersion = Convert.ToInt32(CurrentKey.Substring(4, 3).Replace("_"c, ""))
            If CurrentKey.StartsWith("db_v") AndAlso CurrentSqlVersion > _LocalDBVersionID AndAlso CurrentSqlVersion <= DBVersion Then
                SqlStrings.Add(ItemEnumerator.Key.ToString)
            End If
        Loop
        If SqlStrings.Count > 0 Then
            SqlStrings.Sort()
            Dim DBHelp As New DBHelper(_cnn)
            For Each CurrentKey In SqlStrings
                CurrentSql = My.Resources.SQLs.ResourceManager.GetObject(CurrentKey).ToString
                If CurrentSql.Length > 0 Then
                    Try
                        DBHelp.ExecuteSQL(CurrentSql)
                    Catch ex As Exception
                        ' ignore error 'duplicate column' (or throw error otherwise)
                        If (Not ex.Message.Contains("duplicate column name")) And (Not ex.Message.Contains("already exists")) And (Not ex.Message.Contains(" columns but ")) Then
                            Throw New DatabaseUpdateException(ex.Message, ex)
                        End If
                    End Try
                End If
            Next
        End If
    End Sub
    ''' <summary>
    ''' Executes custom actions while upgrading the database
    ''' </summary>
    ''' <param name="CustomAction"></param>
    Private Sub ProcessCustomAction(ByVal CustomAction As Integer)
        Select Case CustomAction
            Case 1
                ' transaction index Bitfinex has changed: advice user to re-import in case there is existing Bitfinex data
                If _Interactive AndAlso QueryDBInteger("select count(*) as No from Importe where PlattformID = " & CInt(PlatformManager.Platforms.Bitfinex).ToString, "No") > 0 Then
                    MessageBox.Show("Achtung! Mit dieser Version des " & _ApplicationName & " hat sich das Import-Schema für Bitfinex.com geändert. " &
                                    "Um zu vermeiden, dass Trades doppelt importiert werden, sollten Sie alle bisherigen Importe von Bitfinex.com wieder " &
                                    "rückgängig machen und die Daten erneut importieren." & Environment.NewLine & Environment.NewLine &
                                    "Sie können Importe rückgängig machen, indem Sie im Reiter 'Tabellen' auf 'Importe' klicken und anschließend " &
                                    "per Rechtsklick über der betreffenden Importzeile den Befehl 'Import rückgängig machen' auswählen.",
                                    "Datenbank-Update / Änderung Bitfinex.com", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            Case 2
                ' transaction index Poloniex has changed: advice user to re-import in case there is existing Poloniex data
                If _Interactive AndAlso QueryDBInteger("select count(*) as No from Importe where PlattformID = " & CInt(PlatformManager.Platforms.Poloniex).ToString, "No") > 0 Then
                    MessageBox.Show("Achtung! Mit dieser Version des " & _ApplicationName & " hat sich das Import-Schema für Poloniex.com geändert. " &
                                    "Um zu vermeiden, dass Trades doppelt importiert werden, sollten Sie alle bisherigen Importe von Poloniex.com wieder " &
                                    "rückgängig machen und die Daten erneut importieren." & Environment.NewLine & Environment.NewLine &
                                    "Sie können Importe rückgängig machen, indem Sie im Reiter 'Tabellen' auf 'Importe' klicken und anschließend " &
                                    "per Rechtsklick über der betreffenden Importzeile den Befehl 'Import rückgängig machen' auswählen.",
                                    "Datenbank-Update / Änderung Poloniex.com", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            Case 3
                ' the way of storing active cryptocoins of bitfinex account has changed: translate current values of ExtendedInfo
                Try
                    Dim DBO As New DBObjects(String.Format("select * from ApiDaten where PlattformID = {0}", CInt(PlatformManager.Platforms.Bitfinex)), _cnn, DBHelper.TableNames.ApiDaten)
                    If DBO IsNot Nothing AndAlso DBO.DataTable.Rows.Count > 0 Then
                        For i As Integer = 0 To DBO.DataTable.Rows.Count - 1
                            With DBO.DataTable
                                If .Rows(i)("ExtendedInfo").ToString.Contains("|1|") OrElse .Rows(i)("ExtendedInfo").ToString.Contains("|0|") Then
                                    ' translate ExtendedInfo from format 1|1|0 to BTC|BCH|ETH etc.
                                    Dim BinaryValues() As String = Split(.Rows(i)("ExtendedInfo").ToString, "|")
                                    Dim ShortnameValues As String = ""
                                    If BinaryValues.Length >= 5 Then
                                        If BinaryValues(0) = "1" Then
                                            ShortnameValues &= "USD|"
                                        End If
                                        If BinaryValues(1) = "1" Then
                                            ShortnameValues &= "BTC|"
                                        End If
                                        If BinaryValues(2) = "1" Then
                                            ShortnameValues &= "LTC|"
                                        End If
                                        If BinaryValues(3) = "1" Then
                                            ShortnameValues &= "DRK|"
                                        End If
                                        If BinaryValues(4) = "1" Then
                                            ShortnameValues &= "ETH|"
                                        End If
                                        If BinaryValues.Length >= 9 Then
                                            If BinaryValues(5) = "1" Then
                                                ShortnameValues &= "ETC|"
                                            End If
                                            If BinaryValues(6) = "1" Then
                                                ShortnameValues &= "BFX|"
                                            End If
                                            If BinaryValues(7) = "1" Then
                                                ShortnameValues &= "RRT|"
                                            End If
                                            If BinaryValues(8) = "1" Then
                                                ShortnameValues &= "ZEC|"
                                            End If
                                            If BinaryValues.Length >= 15 Then
                                                If BinaryValues(9) = "1" Then
                                                    ShortnameValues &= "BCH|"
                                                End If
                                                If BinaryValues(10) = "1" Then
                                                    ShortnameValues &= "DASH|"
                                                End If
                                                If BinaryValues(11) = "1" Then
                                                    ShortnameValues &= "EOS|"
                                                End If
                                                If BinaryValues(12) = "1" Then
                                                    ShortnameValues &= "IOT|"
                                                End If
                                                If BinaryValues(13) = "1" Then
                                                    ShortnameValues &= "XMR|"
                                                End If
                                                If BinaryValues(14) = "1" Then
                                                    ShortnameValues &= "NEO|"
                                                End If
                                                If BinaryValues(15) = "1" Then
                                                    ShortnameValues &= "XRP|"
                                                End If
                                            End If
                                        End If
                                    End If
                                    If ShortnameValues.Length > 0 Then
                                        ShortnameValues = ShortnameValues.Substring(0, ShortnameValues.Length - 1)
                                    End If
                                    .Rows(i)("ExtendedInfo") = ShortnameValues
                                End If
                            End With
                        Next
                        DBO.Update()
                    End If
                Catch ex As Exception
                    Throw New Exception(String.Format("Die lokale Datenbank '{0}' konnte nicht geöffnet werden!", DatabaseFile))
                End Try
            Case 4
                ' Gainings calculation has changed: clear the table [Kalkulationen]
                With New CoinTracerDataSetTableAdapters.KalkulationenTableAdapter
                    .DeleteAllRows()
                End With
        End Select
    End Sub

    ''' <summary>
    ''' Initialisiert den Bestand der Wechselkursdaten in der Datenbank
    ''' </summary>
    Public Sub InitCourseData()
        Dim DBHelp As New DBHelper(_cnn)
        Dim SQL As String = ""
        Dim CM As New CourseManager(_cnn)
        Dim CR As New Courses
        Dim ToDate As Date = CM.GetCoursesCutOffDay(DBHelper.Konten.EUR, DBHelper.Konten.USD)
        If ToDate = DATENULLVALUE Then
            ToDate = "2008-12-31"
        End If
        If ToDate < CR.CoursesMaxDate_USDEUR Then
            ' Es gibt neuere Kursdaten-Einträge im Courses-Objekt
            SQL = CR.GetCoursesSQL_From_USDEUR(DateAdd(DateInterval.Day, 1, ToDate))
        End If
        ToDate = CM.GetCoursesStartDay(DBHelper.Konten.EUR, DBHelper.Konten.USD)
        If ToDate > CR.CoursesMinDate_USDEUR Then
            ' Es gibt ältere Kursdaten-Einträge im Courses-Objekt
            SQL &= CR.GetCoursesSQL_To_USDEUR(DateAdd(DateInterval.Day, -1, ToDate))
        End If
        If SQL.Length > 0 Then
            ' Es gibt etwas in die DB zu schreiben...
            SQL = "insert into Kurse(Zeitpunkt, Quellbetrag, QuellkontoID, Zielbetrag, ZielkontoID, Calculated) values " & _
                SQL.Substring(0, SQL.Length - 1)
            DBHelp.ExecuteSQL(SQL)
        End If
    End Sub

End Class

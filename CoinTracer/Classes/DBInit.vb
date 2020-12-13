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
        New SqlUpdateSequenceStruct(2, "alter table Zeitstempelwerte add column [Kaufdatum] DATE  NULL"),
        New SqlUpdateSequenceStruct(2, "update Zeitstempelwerte set Kaufdatum = date(Zeitpunkt)"),
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
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET IstGebuehr=1 WHERE ID IN (311,312,321,322)"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET GebuehrKontoID=311 WHERE ID=101"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET GebuehrKontoID=312 WHERE ID=102"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET GebuehrKontoID=321 WHERE ID=201"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET GebuehrKontoID=322 WHERE ID=202"),
        New SqlUpdateSequenceStruct(3, "UPDATE Konten SET SortID=ID"),
        New SqlUpdateSequenceStruct(3, "INSERT INTO [Konten](ID, Bezeichnung, Code, Beschreibung, IstFiat, IstGebuehr, GebuehrKontoID, Eigen, SortID, Fix) VALUES " & NewLine &
                                       "(203, 'Peercoin','PPC','Peercoin',0,0,323,1,203,0) " & NewLine &
                                       ",(323, 'Gebühr Peercoin','feePPC','Gebühren/Peercoin',0,1,0,0,323,0) " & NewLine &
                                       ",(204, 'Namecoin','NMC','Namecoin',0,0,324,1,204,0) " & NewLine &
                                       ",(324, 'Gebühr Namecoin','feeNMC','Gebühren/Namecoin',0,1,0,0,324,0) " & NewLine &
                                       ",(205, 'Novacoin','NVC','Novacoin',0,0,325,1,205,0) " & NewLine &
                                       ",(325, 'Gebühr Novacoin','feeNVC','Gebühren/Novacoin',0,1,0,0,325,0) " & NewLine &
                                       ",(206, 'Primecoin','XPM','Primecoin',0,0,326,1,206,0) " & NewLine &
                                       ",(326, 'Gebühr Primecoin','feeXPM','Gebühren/Primecoin',0,1,0,0,326,0) " & NewLine &
                                       ",(207, 'Mastercoin','MSC','Mastercoin',0,0,327,1,207,0) " & NewLine &
                                       ",(327, 'Gebühr Mastercoin','feeMSC','Gebühren/Mastercoin',0,1,0,0,327,0) " & NewLine &
                                       ",(208, 'Feathercoin','FTC','Feathercoin',0,0,328,1,208,0) " & NewLine &
                                       ",(328, 'Gebühr Feathercoin','feeFTC','Gebühren/Feathercoin',0,1,0,0,328,0) " & NewLine &
                                       ",(209, 'Terracoin','TRC','Terracoin',0,0,329,1,209,0) " & NewLine &
                                       ",(329, 'Gebühr Terracoin','feeTRC','Gebühren/Terracoin',0,1,0,0,329,0)"),
        New SqlUpdateSequenceStruct(4, "INSERT INTO [Plattformen] VALUES(205,'Bitstamp.net','Bitstamp','Bitstamp.net',205,1,1,1);"),
        New SqlUpdateSequenceStruct(6, "ALTER TABLE [Trades] ADD COLUMN [Steuerirrelevant] BOOLEAN DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(6, "drop INDEX if exists [IDX_Trades_ZeitpunktEntwertet]"),
        New SqlUpdateSequenceStruct(6, "drop INDEX if exists [IDX_Trades_ZeitpunktEntwertetSteuer]"),
        New SqlUpdateSequenceStruct(6, "CREATE INDEX [IDX_Trades_ZeitpunktEntwertetSteuer] ON [Trades] (" & NewLine &
                                       "[Zeitpunkt]  ASC, " & NewLine &
                                       "[Entwertet]  ASC, " & NewLine &
                                       "[Steuerirrelevant]  ASC)"),
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                       "select " & NewLine &
                                       "t.Zeitpunkt Zeitpunkt, " & NewLine &
                                       "tt.Bezeichnung Art, " & NewLine &
                                       "case t.TradeTypID when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung else qp.Bezeichnung end Plattform, " & NewLine &
                                       "case when t.TradeTypID = 3 then round(t.BetragNachGebuehr, 8) when t.TradeTypID in (4, 5) then round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) end [Menge Coins], " & NewLine &
                                       "case t.TradeTypID when 3 then zk.Bezeichnung when 4 then qk.Bezeichnung when 5 then qk.Bezeichnung end [Art Coins], " & NewLine &
                                       "case when t.TradeTypID = 3 then round(sum(t.WertEUR), 2) when t.TradeTypID in (4, 5) then round(coalesce(sum(v.Betrag)/t.QuellBetrag * t.WertEUR, 0), 2) end [Preis EUR], " & NewLine &
                                       "case when t.TradeTypID = 3 then date(t.Zeitpunkt) when t.TradeTypID in (4, 5) then coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) end [Kaufdatum Coins], " & NewLine &
                                       "case t.TradeTypID when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * t.WertEUR - sum(v.WertEUR), 2) as NUMERIC) else '-' end [Gewinn EUR], " & NewLine &
                                       "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer " & NewLine &
                                       "from Trades t " & NewLine &
                                       "inner join TradeTypen tt on t.TradeTypID = tt.ID " & NewLine &
                                       "inner join Plattformen qp on t.QuellPlattformID = qp.ID " & NewLine &
                                       "inner join Plattformen zp on t.ZielPlattformID = zp.ID " & NewLine &
                                       "inner join Konten qk on t.QuellKontoID = qk.ID " & NewLine &
                                       "inner join Konten zk on t.ZielKontoID = zk.ID " & NewLine &
                                       "left join ZeitstempelWerte v on v.OutTradeID = t.ID " & NewLine &
                                       "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 " & NewLine &
                                       "group by t.ID, date(v.Kaufdatum) " & NewLine &
                                       "order by t.Zeitpunkt, t.TradeTypID"),
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_GainingsReportDaily] AS " & NewLine &
                                       "select date(t.Zeitpunkt) Zeitpunkt, Art, Plattform, sum([Menge Coins]) [Menge Coins], " & NewLine &
                                       "[Art Coins], sum([Preis EUR]) [Preis EUR], [Kaufdatum Coins], " & NewLine &
                                       "case Art when 'Verkauf' then sum(t.[Gewinn EUR]) else '-' end [Gewinn EUR], Steuer " & NewLine &
                                       "from VW_GainingsReport t where 1 " & NewLine &
                                       "group by date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins] " & NewLine &
                                       "order by date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_Trades"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_Trades] AS " & NewLine &
                                       "select " & NewLine &
                                       "t.ID ID, " & NewLine &
                                       "t.Zeitpunkt Zeitpunkt, " & NewLine &
                                       "tt.Bezeichnung Transaktion, " & NewLine &
                                       "qp.Bezeichnung QuellPlattform, " & NewLine &
                                       "qk.Bezeichnung QuellKonto, " & NewLine &
                                       "t.QuellBetrag QuellBetrag, " & NewLine &
                                       "zp.Bezeichnung ZielPlattform, " & NewLine &
                                       "zk.Bezeichnung ZielKonto, " & NewLine &
                                       "t.ZielBetrag ZielBetrag, " & NewLine &
                                       "t.BetragNachGebuehr ZielBetragNetto, " & NewLine &
                                       "t.WertEUR WertEUR, " & NewLine &
                                       "t.SourceID Referenz, " & NewLine &
                                       "t.Info Info, " & NewLine &
                                       "t.Steuerirrelevant SteuerIgnorieren " & NewLine &
                                       "from Trades t " & NewLine &
                                       "left join Konten qk on t.QuellKontoID=qk.ID " & NewLine &
                                       "left join Konten zk on t.ZielKontoID=zk.ID " & NewLine &
                                       "left join Plattformen qp on t.QuellPlattformID=qp.ID " & NewLine &
                                       "left join Plattformen zp on t.ZielPlattformID=zp.ID " & NewLine &
                                       "left join Plattformen ip on t.ImportPlattformID=ip.ID " & NewLine &
                                       "left join TradeTypen tt on t.TradeTypID=tt.ID " & NewLine &
                                       "where t.Entwertet = 0 " & NewLine &
                                       "order by t.Zeitpunkt, t.ID"),
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_Importe"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_Importe] AS " & NewLine &
                                       "select " & NewLine &
                                       "i.ID ID, " & NewLine &
                                       "p.Bezeichnung Plattform, " & NewLine &
                                       "i.Zeitpunkt Zeitpunkt, " & NewLine &
                                       "i.Dateiname Dateiname, " & NewLine &
                                       "i.PfadDateiname Pfad, " & NewLine &
                                       "i.Eingelesen Eingelesen, " & NewLine &
                                       "i.NichtEingelesen [Übersprungen] " & NewLine &
                                       "from Importe i " & NewLine &
                                       "inner join Plattformen p on i.PlattformID=p.ID " & NewLine &
                                       "order by i.Zeitpunkt, i.ID"),
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
        New SqlUpdateSequenceStruct(6, "drop view if exists VW_Plattformen"),
        New SqlUpdateSequenceStruct(6, "CREATE VIEW [VW_Plattformen] AS " & NewLine &
                                       "select " & NewLine &
                                       "ID, " & NewLine &
                                       "Bezeichnung, " & NewLine &
                                       "Code, " & NewLine &
                                       "Beschreibung, " & NewLine &
                                       "Boerse [IstBörse], " & NewLine &
                                       "Eigen Eigenbesitz, " & NewLine &
                                       "SortID SortierNr, " & NewLine &
                                       "Fix IstFix " & NewLine &
                                       "from Plattformen " & NewLine &
                                       "order by SortID, ID"),
        New SqlUpdateSequenceStruct(6, "ALTER TABLE [Szenarien] ADD COLUMN [CVS] VARCHAR(150) NULL"),
        New SqlUpdateSequenceStruct(6, "delete from [Szenarien] where ID=0"),
        New SqlUpdateSequenceStruct(6, "INSERT INTO [Szenarien] VALUES (0,'Standard','2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1|2,2,2,1')"),
        New SqlUpdateSequenceStruct(6, "UPDATE Plattformen SET Fix=1 WHERE ID IN (0,900)"),
        New SqlUpdateSequenceStruct(7, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(7, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                       "select " & NewLine &
                                       "v.SzenarioID SzenarioID," & NewLine &
                                       "t.Zeitpunkt Zeitpunkt," & NewLine &
                                       "tt.Bezeichnung Art, " & NewLine &
                                       "case t.TradeTypID when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung else qp.Bezeichnung end Plattform, " & NewLine &
                                       "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins], " & NewLine &
                                       "case t.TradeTypID when 3 then zk.Bezeichnung when 4 then qk.Bezeichnung when 5 then qk.Bezeichnung end [Art Coins], " & NewLine &
                                       "case when t.TradeTypID = 3 then round(sum(t.WertEUR), 2) when t.TradeTypID in (4, 5) then round(coalesce(sum(v.Betrag)/t.QuellBetrag * t.WertEUR, 0), 2) end [Preis EUR], " & NewLine &
                                       "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                       "case t.TradeTypID when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * t.WertEUR - sum(v.WertEUR), 2) as NUMERIC) else '-' end [Gewinn EUR], " & NewLine &
                                       "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer " & NewLine &
                                       "from Trades t " & NewLine &
                                       "inner join TradeTypen tt on t.TradeTypID = tt.ID " & NewLine &
                                       "inner join Plattformen qp on t.QuellPlattformID = qp.ID " & NewLine &
                                       "inner join Plattformen zp on t.ZielPlattformID = zp.ID " & NewLine &
                                       "inner join Konten qk on t.QuellKontoID = qk.ID " & NewLine &
                                       "inner join Konten zk on t.ZielKontoID = zk.ID " & NewLine &
                                       "inner join ZeitstempelWerte v on (v.OutTradeID = t.ID or (v.InTradeID = t.ID and t.TradeTypID = 3))" & NewLine &
                                       "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                       "group by v.SzenarioID, t.ID, date(v.Kaufdatum) " & NewLine &
                                       "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID"),
        New SqlUpdateSequenceStruct(7, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(7, "CREATE VIEW [VW_GainingsReportDaily] AS" & NewLine &
                                       "select SzenarioID, date(t.Zeitpunkt) Zeitpunkt, Art, Plattform, sum([Menge Coins]) [Menge Coins]," & NewLine &
                                       "[Art Coins], sum([Preis EUR]) [Preis EUR], [Kaufdatum Coins], " & NewLine &
                                       "case Art when 'Verkauf' then sum(t.[Gewinn EUR]) else '-' end [Gewinn EUR], Steuer " & NewLine &
                                       "from VW_GainingsReport t where 1" & NewLine &
                                       "group by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]" & NewLine &
                                       "order by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
        New SqlUpdateSequenceStruct(8, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(8, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                       "select " & NewLine &
                                       "v.SzenarioID SzenarioID," & NewLine &
                                       "t.Zeitpunkt Zeitpunkt," & NewLine &
                                       "tt.Bezeichnung Art, " & NewLine &
                                       "case t.TradeTypID when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung else qp.Bezeichnung end Plattform, " & NewLine &
                                       "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins], " & NewLine &
                                       "case t.TradeTypID when 3 then zk.Bezeichnung when 4 then qk.Bezeichnung when 5 then qk.Bezeichnung end [Art Coins], " & NewLine &
                                       "case when t.TradeTypID = 3 then round(sum(t.WertEUR), 2) when t.TradeTypID in (4, 5) then round(coalesce(sum(v.Betrag)/t.QuellBetrag * t.WertEUR, 0), 2) end [Preis EUR], " & NewLine &
                                       "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                       "case t.TradeTypID when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * t.WertEUR - sum(v.WertEUR), 2) as NUMERIC) else '-' end [Gewinn EUR], " & NewLine &
                                       "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer " & NewLine &
                                       "from Trades t " & NewLine &
                                       "inner join TradeTypen tt on t.TradeTypID = tt.ID " & NewLine &
                                       "inner join Plattformen qp on t.QuellPlattformID = qp.ID " & NewLine &
                                       "inner join Plattformen zp on t.ZielPlattformID = zp.ID " & NewLine &
                                       "inner join Konten qk on t.QuellKontoID = qk.ID " & NewLine &
                                       "inner join Konten zk on t.ZielKontoID = zk.ID " & NewLine &
                                       "inner join ZeitstempelWerte v on (v.OutTradeID = t.ID or (v.InTradeID = t.ID and t.TradeTypID in (3, 5)))" & NewLine &
                                       "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                       "group by v.SzenarioID, t.ID, date(v.Kaufdatum) " & NewLine &
                                       "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID"),
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
        New SqlUpdateSequenceStruct(9, "drop view if exists VW_Trades"),
        New SqlUpdateSequenceStruct(9, "CREATE VIEW [VW_Trades] AS " & NewLine &
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
                                       "t.Steuerirrelevant SteuerIgnorieren " & NewLine &
                                       "from Trades t " & NewLine &
                                       "left join Konten qk on t.QuellKontoID=qk.ID " & NewLine &
                                       "left join Konten zk on t.ZielKontoID=zk.ID " & NewLine &
                                       "left join Plattformen qp on t.QuellPlattformID=qp.ID " & NewLine &
                                       "left join Plattformen zp on t.ZielPlattformID=zp.ID " & NewLine &
                                       "left join Plattformen ip on t.ImportPlattformID=ip.ID " & NewLine &
                                       "left join TradeTypen tt on t.TradeTypID=tt.ID " & NewLine &
                                       "where t.Entwertet = 0 " & NewLine &
                                       "order by t.Zeitpunkt, t.ID"),
        New SqlUpdateSequenceStruct(9, "drop table if exists Szenarien2Cvs"),
        New SqlUpdateSequenceStruct(9, "PRAGMA writable_schema = 1"),
        New SqlUpdateSequenceStruct(9, "UPDATE SQLITE_MASTER SET SQL = replace(SQL, '[ZielKontoID] NUMERIC', '[ZielKontoID] INTEGER') WHERE NAME = 'Trades'"),
        New SqlUpdateSequenceStruct(9, "PRAGMA writable_schema = 0"),
        New SqlUpdateSequenceStruct(9, "drop view if exists VW_ZugangAbgang"),
        New SqlUpdateSequenceStruct(9, "CREATE VIEW [VW_ZugangAbgang] AS " & NewLine &
                                       "select ZielBetrag Betrag, ZielKontoID KontoID, ZielPlattformID Plattform, Zeitpunkt, 1 SollHaben" & NewLine &
                                       "from Trades where Entwertet=0" & NewLine &
                                       "union select -QuellBetrag, QuellKontoID, QuellPlattformID, Zeitpunkt, 0" & NewLine &
                                       "from Trades where Entwertet=0" & NewLine &
                                       "order by Zeitpunkt"),
        New SqlUpdateSequenceStruct(10, "drop view if exists VW_ZugangAbgang"),
        New SqlUpdateSequenceStruct(10, "CREATE VIEW [VW_ZugangAbgang] AS " & NewLine &
                                        "select ZielBetrag Betrag, ZielKontoID KontoID, ZielPlattformID Plattform, Zeitpunkt, 1 SollHaben" & NewLine &
                                        "from Trades where Entwertet=0" & NewLine &
                                        "union select -QuellBetragNachGebuehr, QuellKontoID, QuellPlattformID, Zeitpunkt, 0" & NewLine &
                                        "from Trades where Entwertet=0" & NewLine &
                                        "order by Zeitpunkt"),
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
        New SqlUpdateSequenceStruct(10, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(10, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                        "select " & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung else qp.Bezeichnung end Plattform, " & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID when 3 then zk.Bezeichnung when 4 then qk.Bezeichnung when 5 then qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID when 3 then round(sum(vt.WertEUR), 2) when 4 then round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID), 2) when 5 then '-' end [Preis EUR]," & NewLine &
                                        "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC) else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer" & NewLine &
                                        "from Trades t " & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID " & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID " & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID " & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID " & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID " & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5)) or (v.InTradeID = t.ID and t.TradeTypID in (3)))" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
        New SqlUpdateSequenceStruct(10, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(10, "CREATE VIEW [VW_GainingsReportDaily] AS" & NewLine &
                                        "select SzenarioID, date(t.Zeitpunkt) Zeitpunkt, Art, Plattform, sum([Menge Coins]) [Menge Coins]," & NewLine &
                                        "[Art Coins], sum([Preis EUR]) [Preis EUR], [Kaufdatum Coins], sum([Kaufpreis EUR]) [Kaufpreis EUR]," & NewLine &
                                        "case Art when 'Verkauf' then sum(t.[Gewinn EUR]) else '-' end [Gewinn EUR], Steuer" & NewLine &
                                        "from VW_GainingsReport t where 1" & NewLine &
                                        "group by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]" & NewLine &
                                        "order by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
        New SqlUpdateSequenceStruct(10, "update Trades set WertEUR = 0 where ID in (" &
                                        "select t.ID from Trades t " &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID " &
                                        "where t.TradeTypID in (3,5) " &
                                        "and qk.IstFiat = 0)"),
        New SqlUpdateSequenceStruct(10, "delete from Kalkulationen"),
        New SqlUpdateSequenceStruct(10, "delete from ZeitstempelWerte",
                                    "Bitte beachten Sie, dass aufgrund einer notwendigen Datenstruktur-Änderung alle Gewinnberechnungen zurückgesetzt wurden. " &
                                    "Sie können diese im Reiter 'Berechnungen' aber problemlos erneut durchführen."),
        New SqlUpdateSequenceStruct(11, , , , True),
        New SqlUpdateSequenceStruct(12, , , , True),
        New SqlUpdateSequenceStruct(13, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(13, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                        "select " & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung" & NewLine &
                                        "   else qp.Bezeichnung end Plattform," & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then zk.Bezeichnung" & NewLine &
                                        "   when 4 then qk.Bezeichnung" & NewLine &
                                        "   else qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then case t.QuellKontoID when 102 then round(sum(t.QuellBetrag), 2) else '-' end" & NewLine &
                                        "   when 4 then case t.ZielKontoID when 102 then round(sum(v.Betrag)/t.QuellBetrag * sum(t.ZielBetrag)/count(distinct v.ID), 2) else '-' end" & NewLine &
                                        "   else '-' end [Preis USD]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then round(sum(vt.WertEUR), 2)" & NewLine &
                                        "   when 4 then round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID), 2)" & NewLine &
                                        "   else '-' end [Preis EUR]," & NewLine &
                                        "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer" & NewLine &
                                        "from Trades t" & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID" & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID" & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID" & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID" & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID" & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5)) or (v.InTradeID = t.ID and t.TradeTypID in (3)))" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
        New SqlUpdateSequenceStruct(13, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(13, "CREATE VIEW [VW_GainingsReportDaily] AS" & NewLine &
                                        "select SzenarioID," & NewLine &
                                        "date(t.Zeitpunkt) Zeitpunkt," & NewLine &
                                        "Art," & NewLine &
                                        "Plattform," & NewLine &
                                        "sum([Menge Coins]) [Menge Coins]," & NewLine &
                                        "[Art Coins]," & NewLine &
                                        "case max([Preis USD])" & NewLine &
                                        "   when '-' then '-'" & NewLine &
                                        "   else sum([Preis USD]) end [Preis USD]," & NewLine &
                                        "sum([Preis EUR]) [Preis EUR]," & NewLine &
                                        "[Kaufdatum Coins]," & NewLine &
                                        "sum([Kaufpreis EUR]) [Kaufpreis EUR]," & NewLine &
                                        "case Art" & NewLine &
                                        "   when 'Verkauf' then sum(t.[Gewinn EUR])" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "Steuer" & NewLine &
                                        "from VW_GainingsReport t where 1" & NewLine &
                                        "group by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]" & NewLine &
                                        "order by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
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
        New SqlUpdateSequenceStruct(17, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(17, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                        "select " & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung" & NewLine &
                                        "   else qp.Bezeichnung end Plattform," & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then zk.Bezeichnung" & NewLine &
                                        "   when 4 then qk.Bezeichnung" & NewLine &
                                        "   else qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then case t.QuellKontoID when 102 then round(sum(t.QuellBetrag), 2) else '-' end" & NewLine &
                                        "   when 4 then case t.ZielKontoID when 102 then round(sum(v.Betrag)/t.QuellBetrag * sum(t.ZielBetrag)/count(distinct v.ID), 2) else '-' end" & NewLine &
                                        "   else '-' end [Preis USD]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then round(sum(vt.WertEUR), 2)" & NewLine &
                                        "   when 4 then round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID), 2)" & NewLine &
                                        "   else '-' end [Preis EUR]," & NewLine &
                                        "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer" & NewLine &
                                        "from Trades t" & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID" & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID" & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID" & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID" & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID" & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5)) or (v.InTradeID = t.ID and t.TradeTypID in (3,5)))" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
        New SqlUpdateSequenceStruct(18, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(18, "CREATE VIEW [VW_GainingsReport] AS " & NewLine &
                                        "select " & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 5 then qp.Bezeichnung || '->' || zp.Bezeichnung" & NewLine &
                                        "   else qp.Bezeichnung end Plattform," & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then zk.Bezeichnung" & NewLine &
                                        "   when 4 then qk.Bezeichnung" & NewLine &
                                        "   else qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then case t.QuellKontoID when 102 then round(sum(t.QuellBetrag), 2) else '-' end" & NewLine &
                                        "   when 4 then case t.ZielKontoID when 102 then round(sum(v.Betrag)/t.QuellBetrag * sum(t.ZielBetrag)/count(distinct v.ID), 2) else '-' end" & NewLine &
                                        "   else '-' end [Preis USD]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then round(sum(vt.WertEUR), 2)" & NewLine &
                                        "   when 4 then round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID), 2)" & NewLine &
                                        "   else '-' end [Preis EUR]," & NewLine &
                                        "coalesce(date(v.Kaufdatum), date(t.Zeitpunkt)) [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 4 then cast(round(sum(v.Betrag)/t.QuellBetrag * sum(vt.WertEUR)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, '+1 year') <= date(t.Zeitpunkt) then 0 else 1 end Steuer" & NewLine &
                                        "from Trades t" & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID" & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID" & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID" & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID" & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID" & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5)) or (v.InTradeID = t.ID and (t.TradeTypID = 3 or (t.TradeTypID = 5 and not qp.Eigen))))" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
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
        New SqlUpdateSequenceStruct(20, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(20, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(20, "CREATE VIEW [VW_GainingsReport] AS" & NewLine &
                                        "select" & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 5 then qp.Bezeichnung || ' → ' || zp.Bezeichnung" & NewLine &
                                        "   else qp.Bezeichnung end Plattform," & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then zk.Bezeichnung" & NewLine &
                                        "   when 4 then qk.Bezeichnung" & NewLine &
                                        "   when 7 then case zk.IstFiat when 0 then qk.Bezeichnung else '-' end" & NewLine &
                                        "   else qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then case t.QuellKontoID when 102 then round(sum(t.QuellBetrag), 2) else '-' end" & NewLine &
                                        "   when 4 then case t.ZielKontoID when 102 then round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(t.ZielBetrag) as real)/count(distinct v.ID), 2) else '-' end" & NewLine &
                                        "   when 7 then 0" & NewLine &
                                        "   else '-' end [Preis USD]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then round(sum(vt.WertEUR), 2)" & NewLine &
                                        "   when 4 then round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(vt.WertEUR) as real)/count(distinct v.ID), 2)" & NewLine &
                                        "   when 7 then 0" & NewLine &
                                        "   else '-' end [Preis EUR]," & NewLine &
                                        "case" & NewLine &
                                        "   when t.TradeTypID = 7 and zk.IstFiat = 1 then '-'" & NewLine &
                                        "   else strftime('%d.%m.%Y', coalesce(date(v.Kaufdatum), date(t.Zeitpunkt))) end [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 4 then cast(round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(vt.WertEUR) as real)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   when 7 then cast(-round(sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, kf.Wert) <= date(t.Zeitpunkt) then 'ja' else 'nein' end Steuerfrei," & NewLine &
                                        "t.Kommentar Kommentar, " & NewLine &
                                        "t.QuellPlattformID _QuellPlattformID," & NewLine &
                                        "t.ZielPlattformID _ZielPlattformID," & NewLine &
                                        "case" & NewLine &
                                        "   when t.QuellKontoID = 102 or t.ZielKontoID = 102 then 1" & NewLine &
                                        "   else 0 end _IstUsdTrade" & NewLine &
                                        "from Trades t" & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID" & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID" & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID" & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID" & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID" & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5,7)) or (v.InTradeID = t.ID and (t.TradeTypID = 3 or (t.TradeTypID = 5 and not qp.Eigen))))" & NewLine &
                                        "inner join Konfiguration kf on kf.ID = 1" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4, 7) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
        New SqlUpdateSequenceStruct(20, "CREATE VIEW [VW_GainingsReportDaily] AS" & NewLine &
                                        "select SzenarioID," & NewLine &
                                        "date(t.Zeitpunkt) Zeitpunkt," & NewLine &
                                        "Art," & NewLine &
                                        "Plattform," & NewLine &
                                        "sum([Menge Coins]) [Menge Coins]," & NewLine &
                                        "[Art Coins]," & NewLine &
                                        "case max([Preis USD])" & NewLine &
                                        "   when '-' then '-'" & NewLine &
                                        "   else sum([Preis USD]) end [Preis USD]," & NewLine &
                                        "sum([Preis EUR]) [Preis EUR]," & NewLine &
                                        "[Kaufdatum Coins]," & NewLine &
                                        "sum([Kaufpreis EUR]) [Kaufpreis EUR]," & NewLine &
                                        "case Art" & NewLine &
                                        "   when 'Verkauf' then sum(t.[Gewinn EUR])" & NewLine &
                                        "   when 'Verlust' then sum(t.[Gewinn EUR])" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "Steuerfrei," & NewLine &
                                        "group_concat(Kommentar) Kommentar, " & NewLine &
                                        "_QuellPlattformID," & NewLine &
                                        "_ZielPlattformID," & NewLine &
                                        "max(_IstUsdTrade) _IstUsdTrade" & NewLine &
                                        "from VW_GainingsReport t where 1" & NewLine &
                                        "group by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]" & NewLine &
                                        "order by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
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
        New SqlUpdateSequenceStruct(20, "drop view if exists VW_ZugangAbgang"),
        New SqlUpdateSequenceStruct(20, "CREATE VIEW [VW_ZugangAbgang] AS " & NewLine &
                                        "select" & NewLine &
                                        "	ZielBetrag Betrag," & NewLine &
                                        "	ZielKontoID KontoID," & NewLine &
                                        "	ZielPlattformID Plattform," & NewLine &
                                        "	Zeitpunkt, 1 SollHaben," & NewLine &
                                        "	case when TradeTypID = 3 and QuellKontoID = 101 then QuellBetrag else NULL end BetragEUR," & NewLine &
                                        "	case when TradeTypID = 3 and QuellKontoID = 102 then QuellBetrag else NULL end BetragUSD," & NewLine &
                                        "	BetragNachGebuehr _BetragNetto " & NewLine &
                                        "from Trades where Entwertet=0" & NewLine &
                                        "union select" & NewLine &
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
        New SqlUpdateSequenceStruct(22, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] = NULL WHERE ID IN ({0},{1})", CInt(PlatformManager.Platforms.Bitfinex), CInt(PlatformManager.Platforms.BitcoinDe))),
        New SqlUpdateSequenceStruct(22, "UPDATE [Plattformen] SET [IstDown] = 0 WHERE [IstDown] ISNULL"),
        New SqlUpdateSequenceStruct(23, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] = 'https://api.bitcoin.de/v1/' WHERE ID ={0}", CInt(PlatformManager.Platforms.BitcoinDe))),
        New SqlUpdateSequenceStruct(23, "UPDATE [Konten] SET [Bezeichnung] = 'Kraken Fee Credit', [Beschreibung] = 'Kraken Fee Credit' WHERE [Code] = 'FEE' AND [Bezeichnung] <> 'Kraken Fee Credit' AND [ID] BETWEEN 209 AND 299"),
        New SqlUpdateSequenceStruct(23, "UPDATE [Konten] SET [Bezeichnung] = 'Gebühr Kraken Fee Credit', [Beschreibung] = 'Gebühren/Kraken Fee Credit' WHERE [Code] = 'feeFEE' AND [Bezeichnung] <> 'Gebühr Kraken Fee Credit' AND [ID] BETWEEN 329 AND 399"),
        New SqlUpdateSequenceStruct(23, "drop view if exists VW_GainingsReport"),
        New SqlUpdateSequenceStruct(23, "CREATE VIEW [VW_GainingsReport] AS" & NewLine &
                                        "select" & NewLine &
                                        "v.SzenarioID SzenarioID," & NewLine &
                                        "t.Zeitpunkt Zeitpunkt," & NewLine &
                                        "tt.Bezeichnung Art," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 5 then qp.Bezeichnung || ' → ' || zp.Bezeichnung" & NewLine &
                                        "   else qp.Bezeichnung end Plattform," & NewLine &
                                        "round(coalesce(sum(v.Betrag), t.BetragNachGebuehr), 8) [Menge Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then zk.Bezeichnung" & NewLine &
                                        "   when 4 then qk.Bezeichnung" & NewLine &
                                        "   when 7 then case zk.IstFiat when 0 then qk.Bezeichnung else '-' end" & NewLine &
                                        "   else qk.Bezeichnung end [Art Coins]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then case t.QuellKontoID when 102 then round(sum(t.QuellBetrag), 2) else '-' end" & NewLine &
                                        "   when 4 then case t.ZielKontoID when 102 then round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(t.ZielBetrag) as real)/count(distinct v.ID), 2) else '-' end" & NewLine &
                                        "   when 7 then 0" & NewLine &
                                        "   else '-' end [Preis USD]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 3 then round(sum(vt.WertEUR), 2)" & NewLine &
                                        "   when 4 then round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(vt.WertEUR) as real)/count(distinct v.ID), 2)" & NewLine &
                                        "   when 7 then 0" & NewLine &
                                        "   else '-' end [Preis EUR]," & NewLine &
                                        "case" & NewLine &
                                        "   when t.TradeTypID = 7 and zk.IstFiat = 1 then '-'" & NewLine &
                                        "   else strftime('%d.%m.%Y', coalesce(date(v.Kaufdatum), date(t.Zeitpunkt))) end [Kaufdatum Coins]," & NewLine &
                                        "round(sum(v.WertEUR), 2) [Kaufpreis EUR]," & NewLine &
                                        "case t.TradeTypID" & NewLine &
                                        "   when 4 then cast(round(cast(sum(v.Betrag) as real)/t.QuellBetrag * cast(sum(vt.WertEUR) as real)/count(distinct v.ID) - sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   when 7 then cast(-round(sum(v.WertEUR), 2) as NUMERIC)" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "case when t.TradeTypID = 4 and date(v.Kaufdatum, kf.Wert) <= date(t.Zeitpunkt) or t.TradeTypID = 7 then 'ja' else 'nein' end Steuerfrei," & NewLine &
                                        "t.Kommentar Kommentar, " & NewLine &
                                        "t.QuellPlattformID _QuellPlattformID," & NewLine &
                                        "t.ZielPlattformID _ZielPlattformID," & NewLine &
                                        "case" & NewLine &
                                        "   when t.QuellKontoID = 102 or t.ZielKontoID = 102 then 1" & NewLine &
                                        "   else 0 end _IstUsdTrade" & NewLine &
                                        "from Trades t" & NewLine &
                                        "inner join TradeTypen tt on t.TradeTypID = tt.ID" & NewLine &
                                        "inner join Plattformen qp on t.QuellPlattformID = qp.ID" & NewLine &
                                        "inner join Plattformen zp on t.ZielPlattformID = zp.ID" & NewLine &
                                        "inner join Konten qk on t.QuellKontoID = qk.ID" & NewLine &
                                        "inner join Konten zk on t.ZielKontoID = zk.ID" & NewLine &
                                        "inner join ZeitstempelWerte v on ((v.OutTradeID = t.ID and t.TradeTypID in (4,5,7)) or (v.InTradeID = t.ID and (t.TradeTypID = 3 or (t.TradeTypID = 5 and not qp.Eigen))))" & NewLine &
                                        "inner join Konfiguration kf on kf.ID = 1" & NewLine &
                                        "left join TradesWerte vt on (vt.TradeID = t.ID and v.SzenarioID = vt.SzenarioID)" & NewLine &
                                        "where (t.TradeTypID in (3, 4, 7) or (t.TradeTypID = 5 and zk.IstFiat = 0)) and t.Steuerirrelevant=0 and (qp.Eigen=1 or zp.Eigen=1)" & NewLine &
                                        "group by v.SzenarioID, t.ID, date(v.Kaufdatum)" & NewLine &
                                        "order by v.SzenarioID, t.Zeitpunkt, t.TradeTypID, date(v.Kaufdatum)"),
        New SqlUpdateSequenceStruct(23, "drop view if exists VW_GainingsReportDaily"),
        New SqlUpdateSequenceStruct(23, "CREATE VIEW [VW_GainingsReportDaily] AS" & NewLine &
                                        "select SzenarioID," & NewLine &
                                        "date(t.Zeitpunkt) Zeitpunkt," & NewLine &
                                        "Art," & NewLine &
                                        "Plattform," & NewLine &
                                        "sum([Menge Coins]) [Menge Coins]," & NewLine &
                                        "[Art Coins]," & NewLine &
                                        "case max([Preis USD])" & NewLine &
                                        "   when '-' then '-'" & NewLine &
                                        "   else round(sum([Preis USD]), 2) end [Preis USD]," & NewLine &
                                        "round(sum([Preis EUR]), 2) [Preis EUR]," & NewLine &
                                        "[Kaufdatum Coins]," & NewLine &
                                        "round(sum([Kaufpreis EUR]), 2) [Kaufpreis EUR]," & NewLine &
                                        "case Art" & NewLine &
                                        "   when 'Verkauf' then sum(t.[Gewinn EUR])" & NewLine &
                                        "   when 'Verlust' then sum(t.[Gewinn EUR])" & NewLine &
                                        "   else '-' end [Gewinn EUR]," & NewLine &
                                        "Steuerfrei," & NewLine &
                                        "group_concat(Kommentar) Kommentar, " & NewLine &
                                        "_QuellPlattformID," & NewLine &
                                        "_ZielPlattformID," & NewLine &
                                        "max(_IstUsdTrade) _IstUsdTrade" & NewLine &
                                        "from VW_GainingsReport t where 1" & NewLine &
                                        "group by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]" & NewLine &
                                        "order by SzenarioID, date(t.Zeitpunkt), Art, Plattform, [Art Coins], [Kaufdatum Coins]"),
        New SqlUpdateSequenceStruct(24, "DELETE FROM [Konten] WHERE ID IN (256, 376)"),
        New SqlUpdateSequenceStruct(24, "INSERT INTO [Konten](ID, Bezeichnung, Code, Beschreibung, IstFiat, IstGebuehr, GebuehrKontoID, Eigen, SortID, Fix) VALUES " & NewLine &
                                        "(256, 'Kraken Fee Credit','FEE','Kraken Fee Credit',0,0,376,1,256,0) " & NewLine &
                                       ",(376, 'Gebühr Kraken Fee Credit','feeFEE','Gebühren/Kraken Fee Credit',0,1,0,0,376,0)"),
        New SqlUpdateSequenceStruct(25, "DELETE FROM [Plattformen] WHERE ID = 208"),
        New SqlUpdateSequenceStruct(25, "INSERT INTO [Plattformen] VALUES(208,'Zyado.com','Zyado','Zyado.com',208,1,1,1,NULL,0,NULL);"),
        New SqlUpdateSequenceStruct(26, "alter table [Importe] add column [LastImportTimestamp] INTEGER DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(26, "alter table [Importe] add column [ApiDatenID] INTEGER DEFAULT '0' NULL"),
        New SqlUpdateSequenceStruct(26, String.Format("CREATE INDEX [IDX_ZeitstempelWerte_SzenarioID] ON [ZeitstempelWerte]({0}[SzenarioID]  ASC{0});", NewLine)),
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
        New SqlUpdateSequenceStruct(27, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(27, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(27, "INSERT INTO _Variables([Name]) VALUES('ETH')"),
        New SqlUpdateSequenceStruct(27, "INSERT INTO _Variables([Name]) VALUES('feeETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'ETH' ORDER BY ID LIMIT 1) WHERE [Name] = 'ETH'"),
        New SqlUpdateSequenceStruct(27, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeETH' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeETH'"),
        New SqlUpdateSequenceStruct(27, "DELETE FROM Konten WHERE [Code] = 'ETH'"),
        New SqlUpdateSequenceStruct(27, "DELETE FROM Konten WHERE [Code] = 'feeETH'"),
        New SqlUpdateSequenceStruct(27, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'ETH'"),
        New SqlUpdateSequenceStruct(27, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeETH'"),
        New SqlUpdateSequenceStruct(27, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Ether', 'ETH', 'Ether', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETH') FROM _Variables WHERE [Name] = 'ETH'"),
        New SqlUpdateSequenceStruct(27, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Ether', 'feeETH', 'Gebühren/Ether', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeETH'"),
        New SqlUpdateSequenceStruct(27, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETH') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETH') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETH') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETH') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETH') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETH')"),
        New SqlUpdateSequenceStruct(27, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETH') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETH')"),
        New SqlUpdateSequenceStruct(27, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(27, "ALTER TABLE [ApiDaten] ADD COLUMN [ExtendedInfo] VARCHAR(100) DEFAULT '' NULL"),
        New SqlUpdateSequenceStruct(27, String.Format("UPDATE [Plattformen] SET [ApiBaseUrl] ='https://api.bitfinex.com/' WHERE ID = {0}", CInt(PlatformManager.Platforms.Bitfinex))),
        New SqlUpdateSequenceStruct(28, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(28, "CREATE TEMP TABLE _Variables(OldID INTEGER, NewID INTEGER)"),
        New SqlUpdateSequenceStruct(28, "INSERT INTO _Variables([OldID]) SELECT ID FROM Plattformen WHERE ID = 209 AND NOT [Code] LIKE '%Poloniex%'"),
        New SqlUpdateSequenceStruct(28, "UPDATE _Variables SET NewID = (SELECT MAX([ID]) + 1 FROM Plattformen WHERE ID < 300 AND ID > 201 ORDER BY ID DESC LIMIT 1)"),
        New SqlUpdateSequenceStruct(28, "INSERT OR REPLACE INTO Plattformen SELECT [NewID], [Bezeichnung], [Code], [Beschreibung], [SortID], [Fix], [Boerse], [Eigen], [ApiBaseUrl], [IstDown], [DownSeit] FROM Plattformen AS p INNER JOIN _Variables AS v ON p.[ID] = v.[OldID]"),
        New SqlUpdateSequenceStruct(28, "UPDATE Importe SET [PlattformID] = (SELECT [NewID] FROM _Variables) WHERE [PlattformID] = (SELECT [OldID] FROM _Variables)"),
        New SqlUpdateSequenceStruct(28, "UPDATE Trades SET [QuellPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [QuellPlattformID] = (SELECT [OldID] FROM _Variables)"),
        New SqlUpdateSequenceStruct(28, "UPDATE Trades SET [ZielPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [ZielPlattformID] = (SELECT [OldID] FROM _Variables)"),
        New SqlUpdateSequenceStruct(28, "UPDATE Trades SET [ImportPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [ImportPlattformID] = (SELECT [OldID] FROM _Variables)"),
        New SqlUpdateSequenceStruct(28, "UPDATE ZeitstempelWerte SET [PlattformID] = (SELECT [NewID] FROM _Variables) WHERE [PlattformID] = (SELECT [OldID] FROM _Variables)"),
        New SqlUpdateSequenceStruct(28, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(28, "DELETE FROM [Plattformen] WHERE ID = 209"),
        New SqlUpdateSequenceStruct(28, "INSERT INTO [Plattformen] VALUES(209,'Poloniex.com','Poloniex','Poloniex.com',209,1,1,1,NULL,0,NULL);"),
        New SqlUpdateSequenceStruct(29, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(29, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(29, "INSERT INTO _Variables([Name]) VALUES('LSK')"),
        New SqlUpdateSequenceStruct(29, "INSERT INTO _Variables([Name]) VALUES('feeLSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'LSK' ORDER BY ID LIMIT 1) WHERE [Name] = 'LSK'"),
        New SqlUpdateSequenceStruct(29, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeLSK' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeLSK'"),
        New SqlUpdateSequenceStruct(29, "DELETE FROM Konten WHERE [Code] = 'LSK'"),
        New SqlUpdateSequenceStruct(29, "DELETE FROM Konten WHERE [Code] = 'feeLSK'"),
        New SqlUpdateSequenceStruct(29, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'LSK'"),
        New SqlUpdateSequenceStruct(29, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeLSK'"),
        New SqlUpdateSequenceStruct(29, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Lisk', 'LSK', 'Lisk', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeLSK') FROM _Variables WHERE [Name] = 'LSK'"),
        New SqlUpdateSequenceStruct(29, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Lisk', 'feeLSK', 'Gebühren/Lisk', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeLSK'"),
        New SqlUpdateSequenceStruct(29, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'LSK') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'LSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'LSK') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'LSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeLSK') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeLSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeLSK') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeLSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'LSK') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'LSK')"),
        New SqlUpdateSequenceStruct(29, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeLSK') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeLSK')"),
        New SqlUpdateSequenceStruct(29, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(30, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(30, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(30, "INSERT INTO _Variables([Name]) VALUES('XLM')"),
        New SqlUpdateSequenceStruct(30, "INSERT INTO _Variables([Name]) VALUES('feeXLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'XLM' ORDER BY ID LIMIT 1) WHERE [Name] = 'XLM'"),
        New SqlUpdateSequenceStruct(30, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeXLM' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeXLM'"),
        New SqlUpdateSequenceStruct(30, "DELETE FROM Konten WHERE [Code] = 'XLM'"),
        New SqlUpdateSequenceStruct(30, "DELETE FROM Konten WHERE [Code] = 'feeXLM'"),
        New SqlUpdateSequenceStruct(30, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'XLM'"),
        New SqlUpdateSequenceStruct(30, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeXLM'"),
        New SqlUpdateSequenceStruct(30, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Lumen', 'XLM', 'Lumen', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXLM') FROM _Variables WHERE [Name] = 'XLM'"),
        New SqlUpdateSequenceStruct(30, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Lumen', 'feeXLM', 'Gebühren/Lumen', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeXLM'"),
        New SqlUpdateSequenceStruct(30, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XLM') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XLM') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXLM') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXLM') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XLM') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XLM')"),
        New SqlUpdateSequenceStruct(30, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXLM') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXLM')"),
        New SqlUpdateSequenceStruct(30, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(31, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(31, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(31, "INSERT INTO _Variables([Name]) VALUES('REP')"),
        New SqlUpdateSequenceStruct(31, "INSERT INTO _Variables([Name]) VALUES('feeREP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'REP' ORDER BY ID LIMIT 1) WHERE [Name] = 'REP'"),
        New SqlUpdateSequenceStruct(31, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeREP' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeREP'"),
        New SqlUpdateSequenceStruct(31, "DELETE FROM Konten WHERE [Code] = 'REP'"),
        New SqlUpdateSequenceStruct(31, "DELETE FROM Konten WHERE [Code] = 'feeREP'"),
        New SqlUpdateSequenceStruct(31, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'REP'"),
        New SqlUpdateSequenceStruct(31, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeREP'"),
        New SqlUpdateSequenceStruct(31, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Augur Token', 'REP', 'Augur Token', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeREP') FROM _Variables WHERE [Name] = 'REP'"),
        New SqlUpdateSequenceStruct(31, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Augur Token', 'feeREP', 'Gebühren/Augur Token', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeREP'"),
        New SqlUpdateSequenceStruct(31, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'REP') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'REP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'REP') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'REP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeREP') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeREP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeREP') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeREP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'REP') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'REP')"),
        New SqlUpdateSequenceStruct(31, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeREP') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeREP')"),
        New SqlUpdateSequenceStruct(31, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(32, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(32, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('BFX')"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('feeBFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'BFX' ORDER BY ID LIMIT 1) WHERE [Name] = 'BFX'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeBFX' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeBFX'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'BFX'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'feeBFX'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'BFX'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeBFX'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'BFX Token', 'BFX', 'Bitfinex Token', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBFX') FROM _Variables WHERE [Name] = 'BFX'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr BFX Token', 'feeBFX', 'Gebühren/Bitfinex Token', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeBFX'"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BFX') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BFX') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBFX') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBFX') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BFX') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BFX')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBFX') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBFX')"),
        New SqlUpdateSequenceStruct(32, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(32, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(32, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('ETC')"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('feeETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'ETC' ORDER BY ID LIMIT 1) WHERE [Name] = 'ETC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeETC' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeETC'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'ETC'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'feeETC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'ETC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeETC'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Ether Classic', 'ETC', 'Ether Classic', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETC') FROM _Variables WHERE [Name] = 'ETC'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Ether Classic', 'feeETC', 'Gebühren/Ether Classic', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeETC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ETC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ETC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeETC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeETC')"),
        New SqlUpdateSequenceStruct(32, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(32, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(32, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('RRT')"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('feeRRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'RRT' ORDER BY ID LIMIT 1) WHERE [Name] = 'RRT'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeRRT' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeRRT'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'RRT'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'feeRRT'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'RRT'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeRRT'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Recovery Right Token', 'RRT', 'Bitfinex Recovery Right Token', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeRRT') FROM _Variables WHERE [Name] = 'RRT'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Recovery Right Token', 'feeRRT', 'Gebühren/Bitfinex Recovery Right Token', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeRRT'"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'RRT') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'RRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'RRT') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'RRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeRRT') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeRRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeRRT') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeRRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'RRT') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'RRT')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeRRT') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeRRT')"),
        New SqlUpdateSequenceStruct(32, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(32, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(32, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('ZEC')"),
        New SqlUpdateSequenceStruct(32, "INSERT INTO _Variables([Name]) VALUES('feeZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'ZEC' ORDER BY ID LIMIT 1) WHERE [Name] = 'ZEC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeZEC' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeZEC'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'ZEC'"),
        New SqlUpdateSequenceStruct(32, "DELETE FROM Konten WHERE [Code] = 'feeZEC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'ZEC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeZEC'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'ZCash', 'ZEC', 'ZCash', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeZEC') FROM _Variables WHERE [Name] = 'ZEC'"),
        New SqlUpdateSequenceStruct(32, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr ZCash', 'feeZEC', 'Gebühren/ZCash', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeZEC'"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ZEC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ZEC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeZEC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeZEC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'ZEC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'ZEC')"),
        New SqlUpdateSequenceStruct(32, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeZEC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeZEC')"),
        New SqlUpdateSequenceStruct(32, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(VersionID:=32, CustomAction:=1),
        New SqlUpdateSequenceStruct(33, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(33, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(33, "INSERT INTO _Variables([Name]) VALUES('XMR')"),
        New SqlUpdateSequenceStruct(33, "INSERT INTO _Variables([Name]) VALUES('feeXMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'XMR' ORDER BY ID LIMIT 1) WHERE [Name] = 'XMR'"),
        New SqlUpdateSequenceStruct(33, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeXMR' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeXMR'"),
        New SqlUpdateSequenceStruct(33, "DELETE FROM Konten WHERE [Code] = 'XMR'"),
        New SqlUpdateSequenceStruct(33, "DELETE FROM Konten WHERE [Code] = 'feeXMR'"),
        New SqlUpdateSequenceStruct(33, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'XMR'"),
        New SqlUpdateSequenceStruct(33, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeXMR'"),
        New SqlUpdateSequenceStruct(33, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Monero', 'XMR', 'Monero', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXMR') FROM _Variables WHERE [Name] = 'XMR'"),
        New SqlUpdateSequenceStruct(33, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Monero', 'feeXMR', 'Gebühren/Monero', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeXMR'"),
        New SqlUpdateSequenceStruct(33, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XMR') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XMR') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXMR') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXMR') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XMR') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XMR')"),
        New SqlUpdateSequenceStruct(33, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXMR') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXMR')"),
        New SqlUpdateSequenceStruct(33, "DROP TABLE _Variables"),
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
        New SqlUpdateSequenceStruct(34, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(34, "INSERT INTO _Variables([Name]) VALUES('XRP')"),
        New SqlUpdateSequenceStruct(34, "INSERT INTO _Variables([Name]) VALUES('feeXRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'XRP' ORDER BY ID LIMIT 1) WHERE [Name] = 'XRP'"),
        New SqlUpdateSequenceStruct(34, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeXRP' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeXRP'"),
        New SqlUpdateSequenceStruct(34, "DELETE FROM Konten WHERE [Code] = 'XRP'"),
        New SqlUpdateSequenceStruct(34, "DELETE FROM Konten WHERE [Code] = 'feeXRP'"),
        New SqlUpdateSequenceStruct(34, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'XRP'"),
        New SqlUpdateSequenceStruct(34, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeXRP'"),
        New SqlUpdateSequenceStruct(34, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Ripple', 'XRP', 'Ripple Token', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXRP') FROM _Variables WHERE [Name] = 'XRP'"),
        New SqlUpdateSequenceStruct(34, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Ripple', 'feeXRP', 'Gebühren/Ripple Token', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeXRP'"),
        New SqlUpdateSequenceStruct(34, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XRP') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XRP') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXRP') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXRP') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'XRP') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'XRP')"),
        New SqlUpdateSequenceStruct(34, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeXRP') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeXRP')"),
        New SqlUpdateSequenceStruct(34, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(35, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(35, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(35, "INSERT INTO _Variables([Name]) VALUES('BCC')"),
        New SqlUpdateSequenceStruct(35, "INSERT INTO _Variables([Name]) VALUES('feeBCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'BCC' ORDER BY ID LIMIT 1) WHERE [Name] = 'BCC'"),
        New SqlUpdateSequenceStruct(35, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeBCC' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeBCC'"),
        New SqlUpdateSequenceStruct(35, "DELETE FROM Konten WHERE [Code] = 'BCC'"),
        New SqlUpdateSequenceStruct(35, "DELETE FROM Konten WHERE [Code] = 'feeBCC'"),
        New SqlUpdateSequenceStruct(35, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'BCC'"),
        New SqlUpdateSequenceStruct(35, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeBCC'"),
        New SqlUpdateSequenceStruct(35, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Bitcoin Cash', 'BCC', 'Bitcoin Cash', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBCC') FROM _Variables WHERE [Name] = 'BCC'"),
        New SqlUpdateSequenceStruct(35, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Bitcoin Cash', 'feeBCC', 'Gebühren/Bitcoin Cash', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeBCC'"),
        New SqlUpdateSequenceStruct(35, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BCC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BCC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBCC') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBCC') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BCC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BCC')"),
        New SqlUpdateSequenceStruct(35, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBCC') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBCC')"),
        New SqlUpdateSequenceStruct(35, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(VersionID:=35, CustomAction:=2),
        New SqlUpdateSequenceStruct(36, "UPDATE Konten SET Code = 'BCH' WHERE Code = 'BCC'"),
        New SqlUpdateSequenceStruct(36, "UPDATE Konten SET Code = 'feeBCH' WHERE Code = 'feeBCC'"),
        New SqlUpdateSequenceStruct(36, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(36, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, OldValue2 INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(36, "INSERT INTO _Variables([Name]) VALUES('IOT')"),
        New SqlUpdateSequenceStruct(36, "INSERT INTO _Variables([Name]) VALUES('feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'IOT' ORDER BY ID LIMIT 1) WHERE [Name] = 'IOT'"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET OldValue2 = (SELECT ID FROM Konten WHERE [Code] = 'IOTA' ORDER BY ID LIMIT 1) WHERE [Name] = 'IOT'"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeIOT' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeIOT'"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET OldValue2 = (SELECT ID FROM Konten WHERE [Code] = 'feeIOTA' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeIOT'"),
        New SqlUpdateSequenceStruct(36, "DELETE FROM Konten WHERE [Code] IN ('IOT', 'IOTA')"),
        New SqlUpdateSequenceStruct(36, "DELETE FROM Konten WHERE [Code] IN ('feeIOT', 'feeIOTA')"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'IOT'"),
        New SqlUpdateSequenceStruct(36, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeIOT'"),
        New SqlUpdateSequenceStruct(36, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'MegaIOTA', 'IOT', 'MegaIOTA', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') FROM _Variables WHERE [Name] = 'IOT'"),
        New SqlUpdateSequenceStruct(36, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr MegaIOTA', 'feeIOT', 'Gebühren/MegaIOTA', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeIOT'"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE QuellKontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE ZielKontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE QuellKontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE ZielKontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'IOT') WHERE KontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'IOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeIOT') WHERE KontoID = (SELECT [OldValue2] FROM _Variables WHERE [Name] = 'feeIOT')"),
        New SqlUpdateSequenceStruct(36, "DROP TABLE _Variables"),
        New SqlUpdateSequenceStruct(37, "PRAGMA temp_store = 2"),
        New SqlUpdateSequenceStruct(37, "CREATE TEMP TABLE _Variables(Name TEXT PRIMARY KEY, OldValue INTEGER, NewValue INTEGER)"),
        New SqlUpdateSequenceStruct(37, "INSERT INTO _Variables([Name]) VALUES('BTG')"),
        New SqlUpdateSequenceStruct(37, "INSERT INTO _Variables([Name]) VALUES('feeBTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'BTG' ORDER BY ID LIMIT 1) WHERE [Name] = 'BTG'"),
        New SqlUpdateSequenceStruct(37, "UPDATE _Variables SET OldValue = (SELECT ID FROM Konten WHERE [Code] = 'feeBTG' ORDER BY ID LIMIT 1) WHERE [Name] = 'feeBTG'"),
        New SqlUpdateSequenceStruct(37, "DELETE FROM Konten WHERE [Code] = 'BTG'"),
        New SqlUpdateSequenceStruct(37, "DELETE FROM Konten WHERE [Code] = 'feeBTG'"),
        New SqlUpdateSequenceStruct(37, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 256 AND ID > 201 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'BTG'"),
        New SqlUpdateSequenceStruct(37, "UPDATE _Variables SET NewValue = (SELECT MAX([ID]) + 1 FROM Konten WHERE ID < 599 AND ID > 311 ORDER BY ID DESC LIMIT 1) WHERE [Name] = 'feeBTG'"),
        New SqlUpdateSequenceStruct(37, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Bitcoin Gold', 'BTG', 'Bitcoin Gold', 0, 1, [NewValue], 0, 0, (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBTG') FROM _Variables WHERE [Name] = 'BTG'"),
        New SqlUpdateSequenceStruct(37, "INSERT OR REPLACE INTO Konten SELECT [NewValue], 'Gebühr Bitcoin Gold', 'feeBTG', 'Gebühren/Bitcoin Gold', 0, 0, [NewValue], 0, 1, 0 FROM _Variables WHERE [Name] = 'feeBTG'"),
        New SqlUpdateSequenceStruct(37, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BTG') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BTG') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBTG') WHERE QuellKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBTG') WHERE ZielKontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'BTG') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'BTG')"),
        New SqlUpdateSequenceStruct(37, "UPDATE ZeitstempelWerte SET KontoID = (SELECT [NewValue] FROM _Variables WHERE [Name] = 'feeBTG') WHERE KontoID = (SELECT [OldValue] FROM _Variables WHERE [Name] = 'feeBTG')"),
        New SqlUpdateSequenceStruct(37, "DROP TABLE _Variables"),
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
        New SqlUpdateSequenceStruct(37, "update Konten " & NewLine &
                                        "set GebuehrKontoID = (select ID from Konten k2 where k2.Code = 'fee' || Konten.Code) " & NewLine &
                                        "where not Konten.IstFiat and not Konten.Fix and Konten.ID < 311 and Konten.GebuehrKontoID = 399"),
        New SqlUpdateSequenceStruct(37, "insert into Konten(Bezeichnung, Code, Beschreibung, IstFiat, Eigen, Fix, IstGebuehr, GebuehrKontoID) " & NewLine &
                                        "select " & NewLine &
                                        "'Gebühr ' || k.Bezeichnung, " & NewLine &
                                        "'fee' || k.Code, " & NewLine &
                                        "'Gebühr ' || k.Beschreibung, " & NewLine &
                                        "0, 0, 0, 1, 0 " & NewLine &
                                        "from Konten k where k.GebuehrKontoID is NULL and not k.IstFiat and not k.Fix"),
        New SqlUpdateSequenceStruct(37, "update Konten set SortID = ID where IstGebuehr and SortID <> ID"),
        New SqlUpdateSequenceStruct(37, "update Konten " & NewLine &
                                        "set GebuehrKontoID = (select ID from Konten k2 where k2.Code = 'fee' || Konten.Code) " & NewLine &
                                        "where not Konten.IstFiat and not Konten.Fix and Konten.ID < 311 and Konten.GebuehrKontoID is NULL"),
        New SqlUpdateSequenceStruct(VersionID:=37, CustomAction:=3),
        New SqlUpdateSequenceStruct(VersionID:=38, CustomAction:=-1),
        New SqlUpdateSequenceStruct(VersionID:=39, CustomAction:=-1),
        New SqlUpdateSequenceStruct(VersionID:=40, CustomAction:=-1)
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
                        If SQLUp.Message <> "" Then
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
                Throw New DatabaseUpdateException(String.Format(My.Resources.MyStrings.initDbUpdateError, Environment.NewLine, ex.Message), ex)
            End Try
            Return True
        ElseIf _LocalDBVersionID > RequiredVersionID Then
            ' Warnung bei zu hoher Datenbankversion (offenbar gab es ein Downgrade der Applikation)
            MsgBoxEx.PatchMsgBox(New String() {"Fortfahren", "Beenden"})
            If MessageBox.Show(String.Format("Achtung: Der {0}-Datenbestand gehört zu einer neueren Version als " &
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
                If QueryDBInteger("select count(*) as No from Importe where PlattformID = " & CInt(PlatformManager.Platforms.Bitfinex).ToString, "No") > 0 Then
                    MessageBox.Show("Achtung! Mit dieser Version des " & _ApplicationName & " hat sich das Import-Schema für Bitfinex.com geändert. " &
                                    "Um zu vermeiden, dass Trades doppelt importiert werden, sollten Sie alle bisherigen Importe von Bitfinex.com wieder " &
                                    "rückgängig machen und die Daten erneut importieren." & Environment.NewLine & Environment.NewLine &
                                    "Sie können Importe rückgängig machen, indem Sie im Reiter 'Tabellen' auf 'Importe' klicken und anschließend " &
                                    "per Rechtsklick über der betreffenden Importzeile den Befehl 'Import rückgängig machen' auswählen.",
                                    "Datenbank-Update / Änderung Bitfinex.com", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            Case 2
                ' transaction index Poloniex has changed: advice user to re-import in case there is existing Poloniex data
                If QueryDBInteger("select count(*) as No from Importe where PlattformID = " & CInt(PlatformManager.Platforms.Poloniex).ToString, "No") > 0 Then
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

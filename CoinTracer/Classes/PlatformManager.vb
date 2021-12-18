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

Imports CoinTracer.CoinTracerDataSetTableAdapters
Imports CoinTracer.CoinTracerDataSet

''' <summary>
''' Class representing all supported and built-in crypto platforms and their related functionality.
''' </summary>
Public NotInheritable Class PlatformManager

    Public Structure PlatformDetails
        Dim DbId As Integer
        Dim SortNr As Integer
        Dim Name As String
        Dim Code As String
        Dim Description As String
        Dim IsFix As Boolean
        Dim IsTradingPlatform As Boolean
        Dim IsProperty As Boolean
        Dim ApiBaseUrl As String
        Dim ImportTarget As Boolean
        Dim ImportDistinct As Boolean
        Public Sub New(DbId As Integer,
                       Name As String,
                       Code As String,
                       Description As String,
                       SortNr As Integer,
                       Optional IsFix As Boolean = True,
                       Optional IsTradingPlatform As Boolean = True,
                       Optional IsProperty As Boolean = True,
                       Optional ApiBaseUrl As String = "",
                       Optional ImportTarget As Boolean = True,
                       Optional ImportDistinct As Boolean = True)
            Me.DbId = DbId
            Me.Name = Name
            Me.Code = Code
            Me.Description = Description
            Me.SortNr = SortNr
            Me.IsFix = IsFix
            Me.IsProperty = IsProperty
            Me.ApiBaseUrl = ApiBaseUrl
            Me.ImportTarget = ImportTarget
            Me.ImportDistinct = ImportDistinct
        End Sub
    End Structure

    Public Enum Platforms
        Invalid = -1
        Unknown = 0
        Bank = 1
        WalletPrivate = 100
        WalletBTC = 101
        WalletLTC = 102
        MultiBit = 103
        WalletBCH = 104
        MtGox = 201
        BitcoinDe = 202
        Vircurex = 203
        BtcE = 204
        BitstampNet = 205
        Kraken = 206
        Bitfinex = 207
        Zyado = 208
        Poloniex = 209
        Binance = 210
        Extern = 900
        CoinTracer = 901
    End Enum

    ' All valid platforms - keep database ids in sync with enum above!!!
    Private Shared Function GetAllPlatforms() As PlatformDetails()
        Return New PlatformDetails() {
            New PlatformDetails(100, "Privates Wallet", "WalletOwn", "Privates Wallet für Cryptocoins", 100, True, False),
            New PlatformDetails(101, "Wallet BTC", "WalletBTC", "Eigenes Wallet für Bitcoin", 101, True, False, ImportDistinct:=False),
            New PlatformDetails(102, "Wallet LTC", "WalletLTC", "Eigenes Wallet für Litecoin", 102, True, False, ImportDistinct:=False),
            New PlatformDetails(103, "MultiBit", "MultiBit", "MultiBit-Wallet-Client", 104, True, False, ImportDistinct:=False),
            New PlatformDetails(104, "Wallet BCH", "WalletBCH", "Eigenes Wallet für Bitcoin Cash", 103, True, False, ImportDistinct:=False),
            New PlatformDetails(201, "Mt. Gox", "MtGox", "MtGox.com", 201),
            New PlatformDetails(202, "Bitcoin.de", "BitcoinDe", "Bitcoin.de", 202, True, True, True, "https://api.bitcoin.de/v1/"),
            New PlatformDetails(203, "Vircurex", "Vircurex", "Vircurex.com", 203),
            New PlatformDetails(204, "BTC-E.com", "BTC-E", "BTC-E.com", 204),
            New PlatformDetails(205, "Bitstamp.net", "Bitstamp", "Bitstamp.net", 205),
            New PlatformDetails(206, "Kraken.com", "Kraken", "Kraken.com", 206, True, True, True, "https://api.kraken.com/"),
            New PlatformDetails(207, "Bitfinex.com", "Bitfinex", "Bitfinex.com", 207, True, True, True, "https://api.bitfinex.com/"),
            New PlatformDetails(208, "Zyado.com", "Zyado", "Zyado.com", 208),
            New PlatformDetails(209, "Poloniex.com", "Poloniex", "Poloniex.com", 209),
            New PlatformDetails(210, "Binance.com", "Binance", "Binance.com", 210, True, True, True, "https://api.binance.com/"),
            New PlatformDetails(901, "CoinTracer.de", "CoinTracer", "CoinTracer.de", 901, ImportTarget:=False)
        }
    End Function

    ''' <summary>
    ''' Return platform details for a specific platform id
    ''' </summary>
    ''' <param name="PlatformID">ID of the requested platform</param>
    ''' <returns>Corresponding PlatformDetails structure or 'Nothing' if the given id has not been found</returns>
    Public Shared Function PlatformDetailsByID(PlatformID As Long) As PlatformDetails
        Dim ResultingPlatform As PlatformDetails = Nothing
        For Each Platform As PlatformDetails In GetAllPlatforms()
            If Platform.DbId = PlatformID Then
                ResultingPlatform = Platform
                Exit For
            End If
        Next
        Return ResultingPlatform
    End Function

    ''' <summary>
    ''' Loads all implemented import methods into a ComboBox
    ''' </summary>
    ''' <param name="ImportComboBox">Form control to be filled</param>
    ''' <param name="ShowHistoricImports">If true, also old imports from platforms not operating anymore are displayed</param>
    Public Shared Sub LoadImportComboBox(ImportComboBox As ComboBox, ShowHistoricImports As Boolean)
        With ImportComboBox.Items
            Dim OldSelecetd = ImportComboBox.SelectedIndex
            .Clear()
            .Add(My.Resources.MyStrings.importLabelAutomatic)
            .Add(My.Resources.MyStrings.importLabelBitcoinDe)
            .Add(My.Resources.MyStrings.importLabelBitcoinCore)
            .Add(My.Resources.MyStrings.importLabelBitcoinCash)
            .Add(My.Resources.MyStrings.importLabelBitfinex)
            .Add(My.Resources.MyStrings.importLabelBitstamp)
            .Add(My.Resources.MyStrings.importLabelGeneric)
            .Add(My.Resources.MyStrings.importLabelKraken)
            .Add(My.Resources.MyStrings.importLabelLitecoinCore)
            .Add(My.Resources.MyStrings.importLabelPoloniex)
            .Add(My.Resources.MyStrings.importLabelCoursesUsd)
            If ShowHistoricImports Then
                .Add(My.Resources.MyStrings.importLabelSeparator)
                .Add(My.Resources.MyStrings.importLabelBtcE)
                .Add(My.Resources.MyStrings.importLabelMultibit)
                .Add(My.Resources.MyStrings.importLabelMtGox)
                .Add(My.Resources.MyStrings.importLabelVircurex)
                .Add(My.Resources.MyStrings.importLabelZyado)
            End If
            If OldSelecetd < .Count AndAlso OldSelecetd >= 0 Then
                ImportComboBox.SelectedIndex = OldSelecetd
            Else
                ImportComboBox.SelectedIndex = 0
            End If
        End With
    End Sub

    ''' <summary>
    ''' Detect the selected platform from the combo box
    ''' </summary>
    ''' <returns>Selected platform, Platforms.Invalid if no valid platform has been selected. Fiat is set to true, when user has selected to import usd course data.</returns>
    Public Shared Function GetPlatformFromComboBox(ImportComboBox As ComboBox, ByRef Fiat As Boolean) As Platforms
        Dim Result As Platforms = Platforms.Invalid
        Fiat = False
        Select Case ImportComboBox.SelectedIndex
            Case 0
                ' auto detection
                Result = Platforms.Unknown
            Case 1
                ' Bitcoin.de
                Result = Platforms.BitcoinDe
            Case 2
                ' Bitcoin Core
                Result = Platforms.WalletBTC
            Case 3
                ' Bitcoin Core
                Result = Platforms.WalletBCH
            Case 4
                ' Bitfinex.com
                Result = Platforms.Bitfinex
            Case 5
                ' Bitstamp.net
                Result = Platforms.BitstampNet
            Case 6
                ' Generic CSV import
                Result = Platforms.CoinTracer
            Case 7
                ' Kraken CSV
                Result = Platforms.Kraken
            Case 8
                ' Litecoin-Core
                Result = Platforms.WalletLTC
            Case 9
                ' Poloniex
                Result = Platforms.Poloniex
            Case 10
                ' Course data EUR/USD
                Fiat = True
            Case 12
                ' BTC-E
                Result = Platforms.BtcE
            Case 13
                ' MultiBit
                Result = Platforms.MultiBit
            Case 14
                ' Mt. Gox
                Result = Platforms.MtGox
            Case 15
                ' Vircurex
                Result = Platforms.Vircurex
            Case 16
                ' Zyado
                Result = Platforms.Zyado

        End Select
        Return Result
    End Function

    ''' <summary>
    ''' Make sure the databases platform table is in sync with all supported platforms
    ''' </summary>
    Public Shared Sub PlatformsSyncDB(Connection As SQLite.SQLiteConnection)
        Dim PlatformTA As New PlattformenTableAdapter
        Dim PlatformTB As PlattformenDataTable = PlatformTA.GetData
        Try
            For Each Platform As PlatformDetails In GetAllPlatforms()
                Dim PlatformRow As PlattformenRow = PlatformTB.FindByID(Platform.DbId)
                If PlatformRow IsNot Nothing Then
                    If PlatformRow.Code.ToUpper <> Platform.Code.ToUpper Then
                        ' Codes do not match - move old data and insert platform with correct id
                        Dim SQLs(10) As String
                        SQLs(0) = "PRAGMA temp_store = 2"
                        SQLs(1) = "CREATE TEMP TABLE _Variables(OldID INTEGER, NewID INTEGER)"
                        SQLs(2) = String.Format("INSERT INTO _Variables([OldID]) VALUES({0})", Platform.DbId, Platform.Code)
                        SQLs(3) = "UPDATE _Variables SET NewID = (SELECT MAX([ID]) + 1 FROM Plattformen WHERE ID < 500 AND ID > 201 ORDER BY ID DESC LIMIT 1)"
                        SQLs(4) = "INSERT OR REPLACE INTO Plattformen SELECT [NewID], [Bezeichnung], [Code], [Beschreibung], [SortID], [Fix], [Boerse], [Eigen], [ApiBaseUrl], [IstDown], [DownSeit] FROM Plattformen AS p INNER JOIN _Variables AS v ON p.[ID] = v.[OldID]"
                        SQLs(5) = "UPDATE Importe SET [PlattformID] = (SELECT [NewID] FROM _Variables) WHERE [PlattformID] = (SELECT [OldID] FROM _Variables)"
                        SQLs(6) = "UPDATE Trades SET [QuellPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [QuellPlattformID] = (SELECT [OldID] FROM _Variables)"
                        SQLs(7) = "UPDATE Trades SET [ZielPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [ZielPlattformID] = (SELECT [OldID] FROM _Variables)"
                        SQLs(8) = "UPDATE Trades SET [ImportPlattformID] = (SELECT [NewID] FROM _Variables) WHERE [ImportPlattformID] = (SELECT [OldID] FROM _Variables)"
                        SQLs(9) = "UPDATE TradeTx SET [PlattformID] = (SELECT [NewID] FROM _Variables) WHERE [PlattformID] = (SELECT [OldID] FROM _Variables)"
                        SQLs(10) = "DROP TABLE _Variables"
                        Dim SqlCmd As New SQLite.SQLiteCommand(Connection)
                        For Each Statement As String In SQLs
                            SqlCmd.CommandText = Statement
                            SqlCmd.ExecuteNonQuery()
                        Next
                        SqlCmd.Dispose()
                        ' Overwrite current entry with correct values
                        With PlatformRow
                            .Code = Platform.Code
                            .Bezeichnung = Platform.Name
                            .Beschreibung = Platform.Description
                            .Fix = Platform.IsFix
                            .Boerse = Platform.IsTradingPlatform
                            .Eigen = Platform.IsProperty
                            .IstDown = False
                        End With
                        PlatformTA.Update(PlatformRow)
                    End If
                Else
                    ' Platform not present - insert it!
                    Dim Result As Integer = PlatformTA.Insert(Platform.DbId, Platform.Name, Platform.Code, Platform.Description,
                                                              Platform.DbId, Platform.IsFix, Platform.IsTradingPlatform,
                                                              Platform.IsProperty, Platform.ApiBaseUrl, False, Nothing,
                                                              Platform.ImportTarget, Platform.ImportDistinct)
                End If
            Next
        Catch ex As Exception
            Throw New Exception(My.Resources.MyStrings.initDbCheckPlatformsError)
        End Try
    End Sub

End Class

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

''' <summary>
''' Class representing all supported and built-in crypto currencies and their related functionality.
''' </summary>
Public NotInheritable Class AccountManager

    Private Const FEEPREFIX As String = "fee"

    ''' <summary>
    ''' Minimal account ID (= database ID) for fee accounts
    ''' </summary>
    Public Const FEEMINACCOUNT As Integer = 10000

    Public Structure AccountDetails
        Dim DbId As Integer
        Dim SortNr As Integer
        Dim Name As String
        Dim Code As String
        Dim Description As String
        Dim IsFiat As Boolean
        Dim IsFix As Boolean
        Dim IsProperty As Boolean
        Dim IsFee As Boolean
        Dim FeeAccountId As Integer
        Dim AlternativeOldCode As String
        Public Sub New(DbId As Integer,
                       Name As String,
                       Code As String,
                       Description As String,
                       Optional IsFix As Boolean = True,
                       Optional IsFiat As Boolean = True,
                       Optional IsProperty As Boolean = True,
                       Optional IsFee As Boolean = False,
                       Optional FeeAccountId As Integer = 0,
                       Optional AlternativeOldCode As String = Nothing)
            Me.DbId = DbId
            Me.Name = Name
            Me.Code = Code
            Me.Description = Description
            SortNr = DbId
            Me.IsFix = IsFix
            Me.IsFiat = IsFiat
            Me.IsProperty = IsProperty
            Me.IsFee = IsFee
            Me.FeeAccountId = FeeAccountId
            If Not AlternativeOldCode Is Nothing Then Me.AlternativeOldCode = AlternativeOldCode
        End Sub
    End Structure

    Public Enum Accounts    ' Just for coding convenience: keep this in sync with the database IDs from GetAllAccounts below!
        [Error] = -1
        Unknown = 0
        EUR = 101
        USD = 102
        BTC = 201
        LTC = 202
        PPC = 203
        NMC = 204
        NVC = 205
        XPM = 206
        MSC = 207
        FTC = 208
        TRC = 209
        ETH = 210
        LSK = 211
        XLM = 212
        REP = 213
        BFX = 214
        ETC = 215
        RRT = 216
        ZEC = 217
        XMR = 218
        XRP = 219
        BCH = 220
        IOT = 221
        BTG = 222
        BSV = 223
        FEE = 224
        BAT = 233
        BTS = 234
        LINK = 235
        ATOM = 236
        FCT = 237
        ICX = 238
        MAID = 239
        NANO = 240
        OMG = 241
        PAX = 242
        SC = 243
        SAI = 244
        USDT = 245
        USDC = 246
        WAVES = 247
        feeEUR = 10101
        feeUSD = 10102
        feeBTC = 10201
        feeLTC = 10202
        feePPC = 10203
        feeNMC = 10204
        feeNVC = 10205
        feeXPM = 10206
        feeMSC = 10207
        feeFTC = 10208
        feeTRC = 10209
        feeETH = 10210
        feeLSK = 10211
        feeXLM = 10212
        feeREP = 10213
        feeBFX = 10214
        feeETC = 10215
        feeRRT = 10216
        feeZEC = 10217
        feeXMR = 10218
        feeXRP = 10219
        feeBCH = 10220
        feeIOT = 10221
        feeBTG = 10222
        feeBSV = 10223
        feeFEE = 10224
        feeBAT = 10233
        feeBTS = 10234
        feeLINK = 10235
        feeATOM = 10236
        feeFCT = 10237
        feeICX = 10238
        feeMAID = 10239
        feeNANO = 10240
        feeOMG = 10241
        feePAX = 10242
        feeSC = 10243
        feeSAI = 10244
        feeUSDT = 10245
        feeUSDC = 10246
        feeWAVES = 10247
    End Enum

    ' All valid platforms - keep database ids in sync with enum above!!!
    Private Shared Function GetAllAccounts() As AccountDetails()
        Return New AccountDetails() {
            New AccountDetails(-1, "Fehler", "Fehler", "Fehlerkonto", True, False, False, False, -1),
            New AccountDetails(0, "Unbekannt", "Unbekannt", "Unbekanntes Konto (aber kein Fehler)", True, False, False, False, 0),
            New AccountDetails(101, "Euro", "EUR", "Euro", True, True, True, False, 10101),
            New AccountDetails(102, "Dollar", "USD", "US-Dollar", True, True, True, False, 10102),
            New AccountDetails(201, "Bitcoin", "BTC", "Bitcoin", True, False, True, False, 10201),
            New AccountDetails(202, "Litecoin", "LTC", "Litecoin", True, False, True, False, 10202),
            New AccountDetails(203, "Peercoin", "PPC", "Peercoin", False, False, True, False, 10203),
            New AccountDetails(204, "Namecoin", "NMC", "Namecoin", False, False, True, False, 10204),
            New AccountDetails(205, "Novacoin", "NVC", "Novacoin", False, False, True, False, 10205),
            New AccountDetails(206, "Primecoin", "XPM", "Primecoin", False, False, True, False, 10206),
            New AccountDetails(207, "Mastercoin", "MSC", "Mastercoin", False, False, True, False, 10207),
            New AccountDetails(208, "Feathercoin", "FTC", "Feathercoin", False, False, True, False, 10208),
            New AccountDetails(209, "Terracoin", "TRC", "Terracoin", False, False, True, False, 10209),
            New AccountDetails(210, "Ether", "ETH", "Ether", False, False, True, False, 10210),
            New AccountDetails(211, "Lisk", "LSK", "Lisk", False, False, True, False, 10211),
            New AccountDetails(212, "Stellar", "XLM", "Stellar Lumens", False, False, True, False, 10212),
            New AccountDetails(213, "Augur Token", "REP", "Augur Token", False, False, True, False, 10213),
            New AccountDetails(214, "BFX Token", "BFX", "Bitfinex Token", False, False, True, False, 10214),
            New AccountDetails(215, "Ether Classic", "ETC", "Ether Classic", False, False, True, False, 10215),
            New AccountDetails(216, "Recovery Right Token", "RRT", "Bitfinex Recovery Right Token", False, False, True, False, 10216),
            New AccountDetails(217, "ZCash", "ZEC", "ZCash", False, False, True, False, 10217),
            New AccountDetails(218, "Monero", "XMR", "Monero", False, False, True, False, 10218),
            New AccountDetails(219, "Ripple", "XRP", "Ripple Token", False, False, True, False, 10219),
            New AccountDetails(220, "Bitcoin Cash", "BCH", "Bitcoin Cash", False, False, True, False, 10220, "BCC"),
            New AccountDetails(221, "MegaIOTA", "IOT", "MegaIOTA", False, False, True, False, 10221, "IOTA"),
            New AccountDetails(222, "Bitcoin Gold", "BTG", "Bitcoin Gold", False, False, True, False, 10222),
            New AccountDetails(223, "Bitcoin SV", "BSV", "Bitcoin SV", False, False, True, False, 10223),
            New AccountDetails(224, "Kraken Fee Credit", "FEE", "Kraken Fee Credit", False, False, True, False, 10224),
            New AccountDetails(225, "Dogecoin", "XDG", "Dogecoin", False, False, True, False, 10225),
            New AccountDetails(226, "Dash", "DASH", "Dash", False, False, True, False, 10226),
            New AccountDetails(227, "WaterMelon", "MLN", "WaterMelon", False, False, True, False, 10227),
            New AccountDetails(228, "Gnosis", "GNO", "Gnosis", False, False, True, False, 10228),
            New AccountDetails(229, "EOS", "EOS", "EOS", False, False, True, False, 10229),
            New AccountDetails(230, "QTUM", "QTUM", "QTUM", False, False, True, False, 10230),
            New AccountDetails(231, "Cardano", "ADA", "Cardano", False, False, True, False, 10231),
            New AccountDetails(232, "Tezos", "XTZ", "Tezos", False, False, True, False, 10232),
            New AccountDetails(233, "Basic Attention Token", "BAT", "Basic Attention Token", False, False, True, False, 10233),
            New AccountDetails(234, "BitShares", "BTS", "BitShares", False, False, True, False, 10234),
            New AccountDetails(235, "ChainLink", "LINK", "ChainLink", False, False, True, False, 10235),
            New AccountDetails(236, "Cosmos", "ATOM", "Cosmos", False, False, True, False, 10236),
            New AccountDetails(237, "Factom", "FCT", "Factom", False, False, True, False, 10237),
            New AccountDetails(238, "ICON", "ICX", "ICON", False, False, True, False, 10238),
            New AccountDetails(239, "MaidSafeCoin", "MAID", "MaidSafeCoin", False, False, True, False, 10239),
            New AccountDetails(240, "Nano", "NANO", "Nano", False, False, True, False, 10240),
            New AccountDetails(241, "OmiseGo", "OMG", "OmiseGo", False, False, True, False, 10241),
            New AccountDetails(242, "PAX Gold", "PAX", "PAX Gold", False, False, True, False, 10242),
            New AccountDetails(243, "SiaCoin", "SC", "SiaCoin", False, False, True, False, 10243),
            New AccountDetails(244, "Single Collateral DAI", "SAI", "Single Collateral DAI", False, False, True, False, 10244),
            New AccountDetails(245, "Tether", "USDT", "Tether", False, False, True, False, 10245),
            New AccountDetails(246, "USD Coin", "USDC", "USD Coin", False, False, True, False, 10246),
            New AccountDetails(247, "Waves", "WAVES", "Waves", False, False, True, False, 10247)
        }
    End Function


    ''' <summary>
    ''' Make sure the databases platform table is in sync with all supported platforms
    ''' </summary>
    Public Shared Sub AccountsSyncDB(Connection As SQLite.SQLiteConnection)
        Dim AccountTA As New KontenTableAdapter
        Dim AccountTB As New KontenDataTable
        AccountTA.ClearBeforeFill = True
        Try
            Dim SQLs As New List(Of String)
            SQLs.Add("PRAGMA temp_store = 2")
            SQLs.Add("CREATE TEMP TABLE _Variables(OldValue INTEGER, NewValue INTEGER)")
            For Each Account As AccountDetails In GetAllAccounts()
                PrepareSQLs(SQLs,
                            Account,
                            False,
                            AccountTA,
                            AccountTB)
                If Account.FeeAccountId > 0 Then
                    PrepareSQLs(SQLs,
                                Account,
                                True,
                                AccountTA,
                                AccountTB)
                End If
            Next
            ' Execute the statements
            If SQLs.Count > 2 Then
                Dim SqlCmd As New SQLite.SQLiteCommand(Connection)
                For Each Statement As String In SQLs
                    Debug.Print(Statement)
                    SqlCmd.CommandText = Statement
                    SqlCmd.ExecuteNonQuery()
                Next
                SqlCmd.Dispose()
            End If
        Catch ex As Exception
            Throw New Exception(My.Resources.MyStrings.initDbCheckAccountsError)
        End Try
    End Sub

    ''' <summary>
    ''' Prepares the neccessary SQL statements for inserting/updating an account
    ''' </summary>
    ''' <param name="SQLs">List of SQL statements, new statements will be appended</param>
    ''' <param name="Account">Account details to add</param>
    ''' <param name="PrepareFee">True, if the fee account is to be inserted, false otherwise</param>
    Private Shared Sub PrepareSQLs(ByRef SQLs As List(Of String),
                            ByRef Account As AccountDetails,
                            ByVal PrepareFee As Boolean,
                            ByRef AccountTA As KontenTableAdapter,
                            ByRef AccountTB As KontenDataTable)
        Dim AlreadyPresent As Boolean = False
        Dim AllDone As Boolean = False
        Dim ThisCode As String
        Dim ThisDbId As Long
        If PrepareFee Then
            ThisCode = FEEPREFIX & Account.Code
            ThisDbId = Account.FeeAccountId
        Else
            ThisCode = Account.Code
            ThisDbId = Account.DbId
        End If
        ' Check if the requested ID is already taken by another account
        AccountTA.FillByID(AccountTB, ThisDbId)
        If AccountTB.Rows.Count > 0 Then
            If Not AccountTB.Rows(0)("Code").ToString.ToUpper = ThisCode.ToUpper Then
                ' It is! So move the existing account entries...
                AddSql_OldValue(SQLs, ThisDbId)
                SQLs.Add(String.Format("UPDATE _Variables SET NewValue = (SELECT COALESCE(MAX([ID]), {0}) + 1 FROM Konten WHERE ID > {0} AND ID < {1} ORDER BY ID DESC LIMIT 1)",
                                       IIf(PrepareFee, FEEMINACCOUNT, "200"),
                                       IIf(PrepareFee, FEEMINACCOUNT * 10, FEEMINACCOUNT)))
                SQLs.Add("INSERT INTO Konten([ID], [Bezeichnung], [Code], [Beschreibung], [IstFiat], [Eigen], [SortID], [Fix], [IstGebuehr], [GebuehrKontoID])
                         SELECT [NewValue], [Bezeichnung], [Code], [Beschreibung], [IstFiat], [Eigen], [SortID], [Fix], [IstGebuehr], [GebuehrKontoID]
                         FROM Konten INNER JOIN _Variables ON _Variables.OldValue = Konten.ID")
                ' Now update all references
                AddSql_UpdateReferences(SQLs)
            Else
                AlreadyPresent = True
                ' Check if the existing entry is good enough...
                AllDone = PrepareFee OrElse AccountTB.Rows(0)("GebuehrKontoID").ToString = Account.FeeAccountId.ToString
            End If
        End If
        ' Check if the requested code already resides under a different ID
        If Not AlreadyPresent Then
            AccountTA.FillByCode(AccountTB, ThisCode)
            If AccountTB.Rows.Count > 0 Then
                ' There is! So just make all references point to the new ID
                AddSql_OldValue(SQLs, AccountTB.Rows(0)("ID").ToString)
                SQLs.Add(String.Format("UPDATE _Variables SET NewValue = {0}", ThisDbId))
                AddSql_UpdateReferences(SQLs)
                ' Delete this account entry
                SQLs.Add("DELETE FROM Konten WHERE ID = (SELECT OldValue FROM _Variables)")
            End If
        End If
        If Account.AlternativeOldCode IsNot Nothing Then
            ' Check if there is an account carrying the alternative code
            AccountTA.FillByCode(AccountTB, IIf(PrepareFee, "fee", "") & Account.AlternativeOldCode)
            If AccountTB.Rows.Count > 0 Then
                ' There is! So just make all references point to the new ID
                AddSql_OldValue(SQLs, AccountTB.Rows(0)("ID").ToString)
                SQLs.Add(String.Format("UPDATE _Variables SET NewValue = {0}", ThisDbId))
                AddSql_UpdateReferences(SQLs)
                ' Delete this account entry
                SQLs.Add("DELETE FROM Konten WHERE ID = (SELECT OldValue FROM _Variables)")
            End If
        End If
        If Not AllDone Then
            ' Now insert (or update) the account entry
            If PrepareFee Then
                SQLs.Add(String.Format("INSERT OR REPLACE INTO Konten([ID], [Bezeichnung], [Code], [Beschreibung], [IstFiat], [Eigen], [SortID], [Fix], [IstGebuehr], [GebuehrKontoID])
                                        VALUES ({0}, '{1}', '{2}', '{3}', {4}, 0, {0}, {5}, 1, 0)",
                                       ThisDbId, My.Resources.MyStrings.feeSingular & " " & Account.Name, ThisCode,
                                       My.Resources.MyStrings.feePlural & "/" & Account.Name,
                                       IIf(Account.IsFiat, 1, 0),
                                       IIf(Account.IsFix, 1, 0)))
            Else
                SQLs.Add(String.Format("INSERT OR REPLACE INTO Konten([ID], [Bezeichnung], [Code], [Beschreibung], [IstFiat], [Eigen], [SortID], [Fix], [IstGebuehr], [GebuehrKontoID])
                                        VALUES ({0}, '{1}', '{2}', '{3}', {4}, {5}, {0}, {6}, 0, {7})",
                                       ThisDbId, Account.Name, ThisCode, Account.Description,
                                       IIf(Account.IsFiat, 1, 0),
                                       IIf(Account.IsProperty, 1, 0),
                                       IIf(Account.IsFix, 1, 0),
                                       Account.FeeAccountId))
            End If
        End If
    End Sub

    Private Shared Sub AddSql_OldValue(ByRef SQLs As List(Of String),
                                ByVal Value As String,
                                Optional ByVal ValueColumn As String = "OldValue")
        SQLs.Add("DELETE FROM _Variables")
        SQLs.Add(String.Format("INSERT INTO _Variables([{1}]) VALUES('{0}')", Value, ValueColumn))
    End Sub

    Private Shared Sub AddSql_UpdateReferences(ByRef SQLs As List(Of String))
        SQLs.Add("UPDATE Trades SET QuellKontoID = (SELECT [NewValue] FROM _Variables) WHERE [QuellKontoID] = (SELECT [OldValue] FROM _Variables)")
        SQLs.Add("UPDATE Trades SET ZielKontoID = (SELECT [NewValue] FROM _Variables) WHERE [ZielKontoID] = (SELECT [OldValue] FROM _Variables)")
        SQLs.Add("UPDATE Bestaende SET KontoID = (SELECT [NewValue] FROM _Variables) WHERE [KontoID] = (SELECT [OldValue] FROM _Variables)")
        SQLs.Add("UPDATE Kurse SET QuellKontoID = (SELECT [NewValue] FROM _Variables) WHERE [QuellKontoID] = (SELECT [OldValue] FROM _Variables)")
        SQLs.Add("UPDATE Kurse SET ZielKontoID = (SELECT [NewValue] FROM _Variables) WHERE [ZielKontoID] = (SELECT [OldValue] FROM _Variables)")
    End Sub

End Class

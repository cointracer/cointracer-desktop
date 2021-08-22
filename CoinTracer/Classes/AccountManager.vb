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
Imports System.Linq

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
        PAXG = 242
        SC = 243
        SAI = 244
        USDT = 245
        USDC = 246
        WAVES = 247
        AAVE = 248
        ALGO = 249
        ANT = 250
        REPV2 = 251
        BAL = 252
        XBT = 253
        COMP = 254
        CRV = 255
        DAI = 256
        MANA = 257
        FIL = 258
        FLOW = 259
        KAVA = 260
        KEEP = 261
        KSM = 262
        KNC = 263
        MLN = 264
        OXT = 265
        DOT = 266
        STORJ = 267
        SNX = 268
        TBTC = 269
        GRT = 270
        TRX = 271
        UNI = 272
        YFI = 273
        ONEINCH = 274
        ANKR = 275
        AXS = 276
        BADGER = 277
        BAND = 278
        BNT = 279
        CHZ = 280
        CQT = 281
        CTSI = 282
        ENJ = 283
        EWT = 284
        GHST = 285
        INJ = 286
        KAR = 287
        LPT = 288
        LRC = 289
        MATIC = 290
        MINA = 291
        MIR = 292
        MKR = 293
        OCEAN = 294
        OGN = 295
        PERP = 296
        RARI = 297
        REN = 298
        SAND = 299
        SOL = 300
        SRM = 301
        SUSHI = 302
        WBTC = 303
        ZRX = 304
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
        feePAXG = 10242
        feeSC = 10243
        feeSAI = 10244
        feeUSDT = 10245
        feeUSDC = 10246
        feeWAVES = 10247
        feeAAVE = 10248
        feeALGO = 10249
        feeANT = 10250
        feeREPV2 = 10251
        feeBAL = 10252
        feeXBT = 10253
        feeCOMP = 10254
        feeCRV = 10255
        feeDAI = 10256
        feeMANA = 10257
        feeFIL = 10258
        feeFLOW = 10259
        feeKAVA = 10260
        feeKEEP = 10261
        feeKSM = 10262
        feeKNC = 10263
        feeMLN = 10264
        feeOXT = 10265
        feeDOT = 10266
        feeSTORJ = 10267
        feeSNX = 10268
        feeTBTC = 10269
        feeGRT = 10270
        feeTRX = 10271
        feeUNI = 10272
        feeYFI = 10273
        feeONEINCH = 10274
        feeANKR = 10275
        feeAXS = 10276
        feeBADGER = 10277
        feeBAND = 10278
        feeBNT = 10279
        feeCHZ = 10280
        feeCQT = 10281
        feeCTSI = 10282
        feeENJ = 10283
        feeEWT = 10284
        feeGHST = 10285
        feeINJ = 10286
        feeKAR = 10287
        feeLPT = 10288
        feeLRC = 10289
        feeMATIC = 10290
        feeMINA = 10291
        feeMIR = 10292
        feeMKR = 10293
        feeOCEAN = 10294
        feeOGN = 10295
        feePERP = 10296
        feeRARI = 10297
        feeREN = 10298
        feeSAND = 10299
        feeSOL = 10300
        feeSRM = 10301
        feeSUSHI = 10302
        feeWBTC = 10303
        feeZRX = 10304

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
            New AccountDetails(242, "PAX Gold", "PAXG", "PAX Gold", False, False, True, False, 10242),
            New AccountDetails(243, "SiaCoin", "SC", "SiaCoin", False, False, True, False, 10243),
            New AccountDetails(244, "Single Collateral DAI", "SAI", "Single Collateral DAI", False, False, True, False, 10244),
            New AccountDetails(245, "Tether", "USDT", "Tether", False, False, True, False, 10245),
            New AccountDetails(246, "USD Coin", "USDC", "USD Coin", False, False, True, False, 10246),
            New AccountDetails(247, "Waves", "WAVES", "Waves", False, False, True, False, 10247),
            New AccountDetails(248, "Aave", "AAVE", "Aave", False, False, True, False, 10248),
            New AccountDetails(249, "Algorand", "ALGO", "Algorand", False, False, True, False, 10249),
            New AccountDetails(250, "Aragon", "ANT", "Aragon", False, False, True, False, 10250),
            New AccountDetails(251, "Augur v2", "REPV2", "Augur v2", False, False, True, False, 10251),
            New AccountDetails(252, "Balancer", "BAL", "Balancer", False, False, True, False, 10252),
            New AccountDetails(253, "Bitcoin", "XBT", "Bitcoin", False, False, True, False, 10253),
            New AccountDetails(254, "Compound", "COMP", "Compound", False, False, True, False, 10254),
            New AccountDetails(255, "Curve DAO Token", "CRV", "Curve DAO Token", False, False, True, False, 10255),
            New AccountDetails(256, "Dai", "DAI", "Dai", False, False, True, False, 10256),
            New AccountDetails(257, "Decentra​land", "MANA", "Decentra​land", False, False, True, False, 10257),
            New AccountDetails(258, "Filecoin", "FIL", "Filecoin", False, False, True, False, 10258),
            New AccountDetails(259, "Flow", "FLOW", "Flow", False, False, True, False, 10259),
            New AccountDetails(260, "Kava", "KAVA", "Kava", False, False, True, False, 10260),
            New AccountDetails(261, "Keep Network", "KEEP", "Keep Network", False, False, True, False, 10261),
            New AccountDetails(262, "Kusama", "KSM", "Kusama", False, False, True, False, 10262),
            New AccountDetails(263, "Kyber Network", "KNC", "Kyber Network", False, False, True, False, 10263),
            New AccountDetails(264, "Melon", "MLN", "Melon", False, False, True, False, 10264),
            New AccountDetails(265, "Orchid", "OXT", "Orchid", False, False, True, False, 10265),
            New AccountDetails(266, "Polkadot", "DOT", "Polkadot", False, False, True, False, 10266),
            New AccountDetails(267, "Storj", "STORJ", "Storj", False, False, True, False, 10267),
            New AccountDetails(268, "Synthetix", "SNX", "Synthetix", False, False, True, False, 10268),
            New AccountDetails(269, "tBTC", "TBTC", "tBTC", False, False, True, False, 10269),
            New AccountDetails(270, "The Graph", "GRT", "The Graph", False, False, True, False, 10270),
            New AccountDetails(271, "TRON", "TRX", "TRON", False, False, True, False, 10271),
            New AccountDetails(272, "Uniswap", "UNI", "Uniswap", False, False, True, False, 10272),
            New AccountDetails(273, "Yearn Finance", "YFI", "Yearn Finance", False, False, True, False, 10273),
            New AccountDetails(274, "1inch", "1INCH", "1inch", False, False, True, False, 10274),
            New AccountDetails(275, "Ankr", "ANKR", "Ankr", False, False, True, False, 10275),
            New AccountDetails(276, "Axie Infinity Shards", "AXS", "Axie Infinity Shards", False, False, True, False, 10276),
            New AccountDetails(277, "Badger DAO", "BADGER", "Badger DAO", False, False, True, False, 10277),
            New AccountDetails(278, "Band Protocol", "BAND", "Band Protocol", False, False, True, False, 10278),
            New AccountDetails(279, "Bancor", "BNT", "Bancor", False, False, True, False, 10279),
            New AccountDetails(280, "Chiliz", "CHZ", "Chiliz", False, False, True, False, 10280),
            New AccountDetails(281, "Covalent", "CQT", "Covalent", False, False, True, False, 10281),
            New AccountDetails(282, "Cartesi", "CTSI", "Cartesi", False, False, True, False, 10282),
            New AccountDetails(283, "Enjin", "ENJ", "Enjin", False, False, True, False, 10283),
            New AccountDetails(284, "Energy Web Token", "EWT", "Energy Web Token", False, False, True, False, 10284),
            New AccountDetails(285, "Aavegotchi", "GHST", "Aavegotchi", False, False, True, False, 10285),
            New AccountDetails(286, "Injective Protocol", "INJ", "Injective Protocol", False, False, True, False, 10286),
            New AccountDetails(287, "Karura", "KAR", "Karura", False, False, True, False, 10287),
            New AccountDetails(288, "LivePeer", "LPT", "LivePeer", False, False, True, False, 10288),
            New AccountDetails(289, "Loopring", "LRC", "Loopring", False, False, True, False, 10289),
            New AccountDetails(290, "Polygon", "MATIC", "Polygon", False, False, True, False, 10290),
            New AccountDetails(291, "Mina", "MINA", "Mina", False, False, True, False, 10291),
            New AccountDetails(292, "Mirror Protocol", "MIR", "Mirror Protocol", False, False, True, False, 10292),
            New AccountDetails(293, "MakerDAO", "MKR", "MakerDAO", False, False, True, False, 10293),
            New AccountDetails(294, "OCEAN Token", "OCEAN", "OCEAN Token", False, False, True, False, 10294),
            New AccountDetails(295, "Origin Protocol", "OGN", "Origin Protocol", False, False, True, False, 10295),
            New AccountDetails(296, "Perpetual Protocol", "PERP", "Perpetual Protocol", False, False, True, False, 10296),
            New AccountDetails(297, "Rarible", "RARI", "Rarible", False, False, True, False, 10297),
            New AccountDetails(298, "Ren", "REN", "Ren", False, False, True, False, 10298),
            New AccountDetails(299, "Sand", "SAND", "Sand", False, False, True, False, 10299),
            New AccountDetails(300, "Solana", "SOL", "Solana", False, False, True, False, 10300),
            New AccountDetails(301, "Serum", "SRM", "Serum", False, False, True, False, 10301),
            New AccountDetails(302, "Sushi", "SUSHI", "Sushi", False, False, True, False, 10302),
            New AccountDetails(303, "Wrapped Bitcoin", "WBTC", "Wrapped Bitcoin", False, False, True, False, 10303),
            New AccountDetails(304, "0x", "ZRX", "0x", False, False, True, False, 10304)
        }
    End Function


    ''' <summary>
    ''' Make sure the databases platform table is in sync with all supported platforms
    ''' </summary>
    Public Shared Sub AccountsSyncDB(Connection As SQLite.SQLiteConnection)
        Dim AccountTA As New KontenTableAdapter With {.ClearBeforeFill = True}
        Dim AccountTB As New KontenDataTable
        AccountTA.Fill(AccountTB)
        Dim Accounts2Move As New List(Of AccountDetails)
        Dim AccRow As KontenRow
        Dim AccFeeRow As KontenRow
        Dim AccountMap As Collection

        Try
            ' Build account alias map, in case we have to re-organise accounts
            Dim KontenAliasesDT As New KontenAliasesDataTable
            With New KontenAliasesTableAdapter
                .Fill(KontenAliasesDT)
            End With
            AccountMap = New Collection
            For Each KontenAliasRow As KontenAliasesRow In KontenAliasesDT.Rows
                AccountMap.Add(KontenAliasRow.Code, KontenAliasRow._Alias)
            Next

            ' Loop over all fixed accounts an write these if needed
            For Each Account As AccountDetails In GetAllAccounts()
                AccRow = AccountTB.FindByID(Account.DbId)
                If AccRow Is Nothing Then
                    ' Insert new account row
                    AccRow = AccountTB.NewKontenRow
                    With AccRow
                        .ID = Account.DbId
                        .Bezeichnung = Account.Name
                        .Code = Account.Code
                        .Beschreibung = Account.Description
                        .IstFiat = Account.IsFiat
                        .Eigen = True
                        .SortID = Account.SortNr
                        .Fix = Account.IsFix
                        .IstGebuehr = Account.IsFee
                        .GebuehrKontoID = Account.FeeAccountId
                    End With
                    AccountTB.AddKontenRow(AccRow)
                Else
                    If AccRow.Code <> Account.Code Then
                        ' Row already present, codes do not match: save current row for later and overwrite it
                        Accounts2Move.Add(New AccountDetails(AccRow.ID,
                                                             AccRow.Bezeichnung,
                                                             AccRow.Code,
                                                             AccRow.Beschreibung,
                                                             AccRow.Fix,
                                                             AccRow.IstFiat,
                                                             AccRow.Eigen,
                                                             AccRow.IstGebuehr,
                                                             AccRow.GebuehrKontoID))
                        With AccRow
                            .Bezeichnung = Account.Name
                            .Code = Account.Code
                            .Beschreibung = Account.Description
                            .IstFiat = Account.IsFiat
                            .Eigen = True
                            .SortID = Account.SortNr
                            .Fix = Account.IsFix
                            .IstGebuehr = Account.IsFee
                            .GebuehrKontoID = Account.FeeAccountId
                        End With
                    End If
                End If
                If Account.FeeAccountId > 0 Then
                    ' Handle the fee row
                    AccFeeRow = AccountTB.FindByID(Account.FeeAccountId)
                    If AccFeeRow Is Nothing Then
                        ' insert fee row
                        AccFeeRow = AccountTB.NewKontenRow
                        With AccFeeRow
                            .ID = Account.FeeAccountId
                            .Bezeichnung = "Gebühr " & Account.Name
                            .Code = FEEPREFIX & Account.Code
                            .Beschreibung = "Gebühr/" & Account.Description
                            .IstFiat = Account.IsFiat
                            .Eigen = False
                            .SortID = Account.FeeAccountId
                            .Fix = Account.IsFix
                            .IstGebuehr = True
                            .GebuehrKontoID = 0
                        End With
                        AccountTB.AddKontenRow(AccFeeRow)
                    Else
                        If AccFeeRow.Code <> FEEPREFIX & Account.Code Then
                            ' just overwrite current row
                            With AccFeeRow
                                .Bezeichnung = "Gebühr " & Account.Name
                                .Code = FEEPREFIX & Account.Code
                                .Beschreibung = "Gebühr/" & Account.Description
                                .IstFiat = Account.IsFiat
                                .Eigen = False
                                .SortID = Account.FeeAccountId
                                .Fix = Account.IsFix
                                .IstGebuehr = True
                                .GebuehrKontoID = 0
                            End With
                        End If
                    End If
                End If
            Next
            ' Save current entries
            AccountTA.Update(AccountTB)
            Dim CurrentID As Long = AccountTA.GetMaxIdBelow(FEEMINACCOUNT)
            CurrentID += 1
            ' Append all previously overwritten rows, including fee rows
            For Each Account In Accounts2Move
                ' check if code now has another ID
                AccRow = AccountTB.Where(Function(r)
                                             If r.Code = Account.Code Then
                                                 Return True
                                             Else
                                                 If AccountMap.Contains(Account.Code) Then
                                                     Return AccountMap(Account.Code) = r.Code
                                                 Else
                                                     Return False
                                                 End If
                                             End If
                                         End Function).FirstOrDefault()
                If AccRow Is Nothing Then
                    ' Code not present in current table: insert new row(s)
                    AccRow = AccountTB.NewKontenRow
                    With AccRow
                        ' Rewrite all references to the new id
                        RewriteAccountReferences(Account.DbId, CurrentID, Connection)
                        ' now append the accounts row
                        .ID = CurrentID
                        .Bezeichnung = Account.Name
                        .Code = Account.Code
                        .Beschreibung = Account.Description
                        .IstFiat = Account.IsFiat
                        .Eigen = True
                        .SortID = CurrentID
                        .Fix = Account.IsFix
                        .IstGebuehr = Account.IsFee
                        .GebuehrKontoID = IIf(Account.FeeAccountId > 0, CurrentID + FEEMINACCOUNT, Account.FeeAccountId)
                    End With
                    AccountTB.AddKontenRow(AccRow)
                    If Account.FeeAccountId > 0 Then
                        AccFeeRow = AccountTB.FindByID(CurrentID + FEEMINACCOUNT)
                        If AccFeeRow Is Nothing Then
                            ' Insert new correponding fee row
                            AccFeeRow = AccountTB.NewKontenRow
                            With AccFeeRow
                                .ID = CurrentID + FEEMINACCOUNT
                                .Bezeichnung = "Gebühr " & Account.Name
                                .Code = FEEPREFIX & Account.Code
                                .Beschreibung = "Gebühr/" & Account.Description
                                .IstFiat = Account.IsFiat
                                .Eigen = False
                                .SortID = .ID
                                .Fix = Account.IsFix
                                .IstGebuehr = True
                                .GebuehrKontoID = 0
                            End With
                            AccountTB.AddKontenRow(AccFeeRow)
                        ElseIf AccFeeRow.Code <> FEEPREFIX & Account.Code Then
                            ' just overwrite it...
                            With AccFeeRow
                                .Bezeichnung = "Gebühr " & Account.Name
                                .Code = FEEPREFIX & Account.Code
                                .Beschreibung = "Gebühr/" & Account.Description
                                .IstFiat = Account.IsFiat
                                .Eigen = False
                                .Fix = Account.IsFix
                                .IstGebuehr = True
                                .GebuehrKontoID = 0
                            End With
                        End If
                        ' rewrite references to fee account id
                        RewriteAccountReferences(Account.FeeAccountId, CurrentID + FEEMINACCOUNT, Connection)
                    End If
                    CurrentID += 1
                Else
                    ' Account is already present: just rewrite references
                    RewriteAccountReferences(Account.DbId, AccRow.ID, Connection)
                    If Account.FeeAccountId > 0 Then
                        ' also rewrite fee account references
                        RewriteAccountReferences(Account.FeeAccountId, AccRow.GebuehrKontoID, Connection)
                    End If
                End If
            Next
            ' Update table one last time
            AccountTA.Update(AccountTB)
        Catch ex As Exception
            Throw New Exception(My.Resources.MyStrings.initDbCheckAccountsError)
        End Try
    End Sub

    Private Shared Sub RewriteAccountReferences(ByVal OldID As Long, NewID As Long, ByRef Connection As SQLite.SQLiteConnection)
        Dim SQLs As New List(Of String) From {
            "UPDATE Trades SET QuellKontoID = {1} WHERE [QuellKontoID] = {0}",
            "UPDATE Trades SET ZielKontoID = {1} WHERE [ZielKontoID] = {0}",
            "UPDATE Bestaende SET KontoID = {1} WHERE [KontoID] = {0}",
            "UPDATE Kurse SET QuellKontoID = {1} WHERE [QuellKontoID] = {0}",
            "UPDATE Kurse SET ZielKontoID = {1} WHERE [ZielKontoID] = {0}"
        }
        Dim SqlCmd As New SQLite.SQLiteCommand(Connection)
        For Each Statement As String In SQLs
            SqlCmd.CommandText = String.Format(Statement, OldID, NewID)
            SqlCmd.ExecuteNonQuery()
        Next
        SqlCmd.Dispose()
    End Sub
End Class

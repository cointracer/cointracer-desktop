'  **************************************
'  *
'  * Copyright 2013-2022 Andreas Nebinger
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

Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports Newtonsoft.Json.Linq

Namespace KrakenClient

    Public Class KrakenClient
        Inherits ApiClientBase

        ''' <summary>
        ''' Default pause between two calls in milliseconds
        ''' </summary>
        Friend Const KRAKEN_API_DEFAULTINTERVAL As Integer = 5000

        Private _url As String
        Private _version As Integer
        Private _key As String
        Private _secret As String

        Public Sub New(ApiKey As String, ApiSecret As String, CallDelay As Long)
            ' TODO: Work in progress... Version und URL müssen natürlich noch dynamisiert werden!
            _url = "https://api.kraken.com"
            _version = "0"
            Me.CallDelay = CallDelay
            _key = ApiKey
            _secret = ApiSecret
        End Sub

        Private Function QueryPublic(a_sMethod As String, Optional props As String = Nothing) As String

            ' Wait if needed...
            WaitForNextApiCall()

            Dim address As String = String.Format("{0}/{1}/public/{2}", _url, _version, a_sMethod)
            Dim webRequest_1 As HttpWebRequest = DirectCast(WebRequest.Create(address), HttpWebRequest)
            webRequest_1.ContentType = "application/x-www-form-urlencoded"
            webRequest_1.Method = "POST"

            If props IsNot Nothing Then

                Using writer = New StreamWriter(webRequest_1.GetRequestStream())
                    writer.Write(props)
                End Using
            End If

            'Make the request
            Try
                'Wait for RateGate
                '_rateGate.WaitToProceed()

                Using webResponse As WebResponse = webRequest_1.GetResponse()
                    Using str As Stream = webResponse.GetResponseStream()
                        Using sr As New StreamReader(str)
                            Return sr.ReadToEnd
                        End Using
                    End Using
                End Using
            Catch wex As WebException
                Using response As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                    Using str As Stream = response.GetResponseStream()
                        Using sr As New StreamReader(str)
                            If response.StatusCode <> HttpStatusCode.InternalServerError Then
                                Throw
                            End If
                            Return sr.ReadToEnd
                        End Using
                    End Using

                End Using
            End Try
        End Function

        Private Function QueryPrivate(a_sMethod As String, Optional props As String = Nothing) As String

            ' Wait if needed...
            WaitForNextApiCall()

            ' generate a 64 bit nonce using a timestamp at tick resolution
            Dim nonce As Int64 = DateTime.Now.Ticks
            props = "nonce=" & nonce & props


            Dim path As String = String.Format("/{0}/private/{1}", _version, a_sMethod)
            Dim address As String = _url & path
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim webRequest_1 As HttpWebRequest = DirectCast(WebRequest.Create(address), HttpWebRequest)
            webRequest_1.ContentType = "application/x-www-form-urlencoded"
            webRequest_1.Method = "POST"
            webRequest_1.Headers.Add("API-Key", _key)


            Dim base64DecodedSecret As Byte() = Convert.FromBase64String(_secret)

            Dim np As String = nonce & props

            Dim pathBytes As Byte() = Encoding.UTF8.GetBytes(path)
            Dim hash256Bytes As Byte() = sha256_hash(np)
            Dim z As Byte() = New Byte(pathBytes.Length + (hash256Bytes.Length - 1)) {}
            pathBytes.CopyTo(z, 0)
            hash256Bytes.CopyTo(z, pathBytes.Length)

            Dim signature = getHash(base64DecodedSecret, z)

            webRequest_1.Headers.Add("API-Sign", Convert.ToBase64String(signature))

            If props IsNot Nothing Then

                Using writer = New StreamWriter(webRequest_1.GetRequestStream())
                    writer.Write(props)
                End Using
            End If

            'Make the request
            Try
                'Wait for RateGate
                '_rateGate.WaitToProceed()

                Using webResponse As WebResponse = webRequest_1.GetResponse()
                    Using str As Stream = webResponse.GetResponseStream()
                        Using sr As New StreamReader(str)
                            Return sr.ReadToEnd
                        End Using
                    End Using
                End Using
            Catch wex As WebException
                If wex.Response Is Nothing Then
                    Throw
                Else
                    Using response As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                        Using str As Stream = response.GetResponseStream()
                            Using sr As New StreamReader(str)
                                If response.StatusCode <> HttpStatusCode.InternalServerError Then
                                    Throw
                                End If
                                Return sr.ReadToEnd
                            End Using
                        End Using
                    End Using
                End If
            End Try
        End Function

#Region "Public queries"


        'Get public server time
        'This is to aid in approximatig the skew time between the server and client
        Public Function GetServerTime() As String
            Return QueryPublic("Time")
        End Function

        ' Get a public list of active assets and their properties:
        '* 
        '        * Returned assets are keyed by their ISO-4217-A3-X names, example output:
        '        * 
        '        * Array
        '        * (
        '        *     [error] => Array
        '        *         (
        '        *         )
        '        * 
        '        *     [result] => Array
        '        *         (
        '        *             [XBTC] => Array
        '        *                 (
        '        *                     [aclass] => currency
        '        *                     [altname] => BTC
        '        *                     [decimals] => 10
        '        *                     [display_decimals] => 5
        '        *                 )
        '        * 
        '        *             [XLTC] => Array
        '        *                 (
        '        *                     [aclass] => currency
        '        *                     [altname] => LTC
        '        *                     [decimals] => 10
        '        *                     [display_decimals] => 5
        '        *                 )
        '        * 
        '        *             [XXRP] => Array
        '        *                 (
        '        *                     [aclass] => currency
        '        *                     [altname] => XRP
        '        *                     [decimals] => 8
        '        *                     [display_decimals] => 5
        '        *                 )
        '        * 
        '        *             [ZEUR] => Array
        '        *                 (
        '        *                     [aclass] => currency
        '        *                     [altname] => EUR
        '        *                     [decimals] => 4
        '        *                     [display_decimals] => 2
        '        *                 )
        '        *             ...
        '        * )
        '        

        Public Function GetActiveAssets() As String
            Return QueryPublic("Assets")
        End Function

        'Get a public list of tradable asset pairs
        Public Function GetAssetPairs(pairs As List(Of String)) As String
            Dim pairString As New StringBuilder()

            'do nothing, had to pass an empty string.

            If pairs Is Nothing Then
            ElseIf pairs.Count() = 0 Then
                Return Nothing
            Else
                pairString.Append("pair=")

                For Each item As String In pairs
                    pairString.Append(item & ",")
                Next
                'disregard trailing comma    
                pairString.Length -= 1
            End If

            Return QueryPublic("AssetPairs", pairString.ToString())
        End Function

        ' Get public ticker info for BTC/USD pair:
        '*
        '         * Example output:
        '         *
        '         * Array
        '         * (
        '         *     [error] => Array
        '         *         (
        '         *         )
        '         * 
        '         *     [result] => Array
        '         *         (
        '         *             [XBTCZUSD] => Array
        '         *                 (
        '         *                     [a] => Array
        '         *                         (
        '         *                             [0] => 106.09583
        '         *                             [1] => 111
        '         *                         )
        '         * 
        '         *                     [b] => Array
        '         *                         (
        '         *                             [0] => 105.53966
        '         *                             [1] => 4
        '         *                         )
        '         * 
        '         *                     [c] => Array
        '         *                         (
        '         *                             [0] => 105.98984
        '         *                             [1] => 0.13910102
        '         *                         )
        '         * 
        '         *                     ...
        '         *         )
        '         * )
        '         

        Public Function GetTicker(pairs As List(Of String)) As String

            If pairs Is Nothing Then
                Return Nothing
            End If
            If pairs.Count() = 0 Then
                Return Nothing
            End If

            Dim pairString As New StringBuilder("pair=")
            For Each item As String In pairs
                pairString.Append(item & ",")
            Next
            pairString.Length -= 1
            'disregard trailing comma

            Return QueryPublic("Ticker", pairString.ToString())
        End Function


        'Get public order book
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pair">asset pair to get market depth for</param>
        ''' <param name="count">maximum number of ask/bids (optional)</param>
        ''' <returns>
        ''' pair_name = pair name
        '''             asks = ask side array of array entries([price], [volume], [timestamp])
        '''             bids = bid side array of array entries([price], [volume], [timestamp])
        ''' </returns>
        Public Function GetOrderBook(pair As String, Optional count As System.Nullable(Of Integer) = Nothing) As String

            Dim reqs As String = String.Format("pair={0}", pair)

            If count.HasValue Then

                reqs += String.Format("&count={0}", count.Value.ToString())
            End If

            Return QueryPublic("Depth", reqs)

        End Function

        ''' <summary>
        ''' Get recent trades
        ''' </summary>
        ''' <param name="pair">asset pair to get trade data for</param>
        ''' <param name="since">return trade data since given id (exclusive). Unix Time</param>
        ''' <remarks> NOTE: the 'since' parameter is subject to change in the future: it's precision may be modified,
        '''             and it may no longer be representative of a timestamp. The best practice is to base it
        '''             on the 'last' value returned in the result set. 
        '''  </remarks>
        ''' <returns></returns>
        Public Function GetRecentTrades(pair As String, since As Long) As String
            Dim reqs As String = String.Format("pair={0}", pair)


            reqs += String.Format("&since={0}", since.ToString())


            Return QueryPublic("Trades", reqs)
        End Function


        ''' <summary>
        ''' Get recent spread data
        ''' </summary>
        ''' <param name="pair">asset pair to get trade data for</param>
        ''' <param name="since">return trade data since given id (inclusive). Unix Time</param>
        ''' <remarks> NOTE: Note: "since" is inclusive so any returned data with the same time as the previous
        ''' set should overwrite all of the previous set's entries at that time
        '''  </remarks>
        ''' <returns></returns>
        Public Function GetRecentSpreadData(pair As String, since As Long) As String
            Dim reqs As String = String.Format("pair={0}", pair)


            reqs += String.Format("&since={0}", since.ToString())


            Return QueryPublic("Spread", reqs)
        End Function


#End Region

#Region "Private user data queries"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>
        ''' array of asset names and balance amount
        ''' </returns>
        Public Function GetBalance() As String
            Return QueryPrivate("Balance")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aclass">asset class (optional): currency (default)</param>
        ''' <param name="asset">base asset used to determine balance (default = ZUSD)</param>
        ''' <returns>
        ''' tb = trade balance (combined balance of all currencies)
        ''' m = initial margin amount of open positions
        ''' n = unrealized net profit/loss of open positions
        ''' c = cost basis of open positions
        ''' v = current floating valuation of open positions
        ''' e = equity = trade balance + unrealized net profit/loss
        ''' mf = free margin = equity - initial margin (maximum margin available to open new positions)
        ''' ml = margin level = (equity / initial margin) * 100
        ''' </returns>
        Public Function GetTradeBalance(aclass As String, asset As String) As String
            Dim reqs As String = ""
            If String.IsNullOrEmpty(aclass) Then
                reqs += String.Format("&aclass={0}", aclass)
            End If
            If String.IsNullOrEmpty(aclass) Then
                reqs += String.Format("&asset={0}", asset)
            End If

            Return QueryPrivate("TradeBalance", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trades">whether or not to include trades in output (optional.  default = false)</param>
        ''' <param name="userref">restrict results to given user reference id (optional)</param>
        ''' <returns>
        ''' refid = Referral order transaction id that created this order
        '''userref = user reference id
        '''status = status of order:
        '''  pending = order pending book entry
        '''open = open order
        '''closed = closed order
        '''canceled = order canceled
        '''expired = order expired
        '''opentm = unix timestamp of when order was placed
        '''starttm = unix timestamp of order start time (or 0 if not set)
        '''expiretm = unix timestamp of order end time (or 0 if not set)
        '''descr = order description info
        ''' pair = asset pair
        ''' type = type of order (buy/sell)
        ''' ordertype = order type (See Add standard order)
        ''' price = primary price
        ''' price2 = secondary price
        ''' leverage = amount of leverage
        ''' position = position tx id to close (if order is positional)
        ''' order = order description
        ''' close = conditional close order description (if conditional close set)
        '''vol = volume of order (base currency unless viqc set in oflags)
        '''vol_exec = volume executed (base currency unless viqc set in oflags)
        '''cost = total cost (quote currency unless unless viqc set in oflags)
        '''fee = total fee (quote currency)
        '''price = average price (quote currency unless viqc set in oflags)
        '''stopprice = stop price (quote currency, for trailing stops)
        '''limitprice = triggered limit price (quote currency, when limit based order type triggered)
        '''misc = comma delimited list of miscellaneous info
        '''  stopped = triggered by stop price
        '''  touched = triggered by touch price
        '''  liquidated = liquidation
        '''  partial = partial fill
        '''oflags = comma delimited list of order flags
        ''' viqc = volume in quote currency
        '''  plbc = prefer profit/loss in base currency
        '''  nompp = no market price protection 
        '''trades = array of trade ids related to order (if trades info requested and data available)
        ''' </returns>
        Public Function GetOpenOrders(Optional trades As Boolean = False, Optional userref As String = "") As String
            Dim reqs As String = String.Format("&trades={0}", True)

            If Not String.IsNullOrEmpty(userref) Then
                reqs += String.Format("&userref={0}", userref)
            End If

            Return QueryPrivate("OpenOrders", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trades">whether or not to include trades in output (optional.  default = false)</param>
        ''' <param name="userref">restrict results to given user reference id (optional)</param>
        ''' <param name="start">starting unix timestamp or order tx id of results (optional.  exclusive)></param>
        ''' <param name="end">ending unix timestamp or order tx id of results (optional.  inclusive)</param>
        ''' <param name="ofs">result offset</param>
        ''' <param name="closetime"> which time to use (optional) [open close both] (default)</param>
        ''' <returns></returns>
        '''  /// <remarks>Note: Times given by order tx ids are more accurate than unix timestamps. If an order tx id is given for the time, the order's open time is used</remarks>
        Public Function GetClosedOrders(Optional trades As Boolean = False, Optional userref As String = "", Optional start As String = "", Optional [end] As String = "", Optional ofs As String = "", Optional closetime As String = "both") As String
            Dim reqs As String = String.Format("&trades={0}&closetime={1}", trades, closetime)
            If Not String.IsNullOrEmpty(userref) Then
                reqs += String.Format("&useref={0}", userref)
            End If
            If Not String.IsNullOrEmpty(start) Then
                reqs += String.Format("&start={0}", start)
            End If
            If Not String.IsNullOrEmpty([end]) Then
                reqs += String.Format("&end={0}", [end])
            End If
            If Not String.IsNullOrEmpty(ofs) Then
                reqs += String.Format("&ofs={0}", ofs)
            End If


            Return QueryPrivate("ClosedOrders", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="txid">comma delimited list of transaction ids to query info about (20 maximum)</param>
        ''' <param name="trades">whether or not to include trades in output (optional.  default = false)</param>
        ''' <param name="userref">restrict results to given user reference id (optional)</param>
        ''' <returns></returns>
        Public Function QueryOrders(txid As String, Optional trades As Boolean = False, Optional userref As String = "") As String
            Dim reqs As String = String.Format("&txid={0}&trades={1}", txid, trades)
            If Not String.IsNullOrEmpty(userref) Then
                reqs += String.Format("&userref={0}", userref)
            End If

            Return QueryPrivate("QueryOrders", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ofs">result offset</param>
        ''' <param name="type">type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        ''' <param name="trades">whether or not to include trades related to position in output (optional.  default = false)</param>
        ''' <param name="start">starting unix timestamp or trade tx id of results (optional.  exclusive)</param>
        ''' <param name="end">ending unix timestamp or trade tx id of results (optional.  inclusive)</param>
        ''' <returns></returns>
        Public Function GetTradesHistory(Optional ofs As String = "", Optional type As String = "all", Optional trades As Boolean = False, Optional start As String = "", Optional [end] As String = "") As String
            Dim reqs As String = String.Format("&ofs={0}&type={1}&trades={2}", ofs, type, trades)
            If Not String.IsNullOrEmpty(start) Then
                reqs += String.Format("&start={0}", start)
            End If
            If Not String.IsNullOrEmpty([end]) Then
                reqs += String.Format("&end={0}", [end])
            End If
            Return QueryPrivate("TradesHistory", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="txid">comma delimited list of transaction ids to query info about (20 maximum)</param>
        ''' <param name="trades">whether or not to include trades related to position in output (optional.  default = false)</param>
        ''' <returns></returns>
        Public Function QueryTrades(Optional txid As String = "", Optional trades As Boolean = False) As String
            Dim reqs As String = String.Format("&txid={0}&trades={1}", txid, trades)
            Return QueryPrivate("QueryTrades", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="txid">comma delimited list of transaction ids to query info about (20 maximum)</param>
        ''' <param name="docalcs">whether or not to include profit/loss calculations (optional.  default = false)</param>
        ''' <returns>
        ''' position_txid = open position info
        ''' ordertxid = order responsible for execution of trade
        ''' pair = asset pair
        ''' time = unix timestamp of trade
        ''' type = type of order used to open position (buy/sell)
        ''' ordertype = order type used to open position
        ''' cost = opening cost of position (quote currency unless viqc set in oflags)
        ''' fee = opening fee of position (quote currency)
        ''' vol = position volume (base currency unless viqc set in oflags)
        ''' vol_closed = position volume closed (base currency unless viqc set in oflags)
        ''' margin = initial margin (quote currency)
        ''' value = current value of remaining position (if docalcs requested.  quote currency)
        ''' net = unrealized profit/loss of remaining position (if docalcs requested.  quote currency, quote currency scale)
        ''' misc = comma delimited list of miscellaneous info
        ''' oflags = comma delimited list of order flags
        ''' viqc = volume in quote currency
        ''' </returns>
        Public Function GetOpenPositions(Optional txid As String = "", Optional docalcs As Boolean = False) As String
            Dim reqs As String = String.Format("&txid={0}&docalcs={1}", txid, docalcs)
            Return QueryPrivate("OpenPositions", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aclass">asset class (optional): currency (default)</param>
        ''' <param name="asset">comma delimited list of assets to restrict output to (optional.  default = all) </param>
        ''' <param name="type">type of ledger to retrieve (optional):[all(default) deposit withdrawal trade margin]</param>
        ''' <param name="start">starting unix timestamp or ledger id of results (optional.  exclusive)</param>
        ''' <param name="end">ending unix timestamp or ledger id of results (optional.  inclusive)</param>
        ''' <param name="ofs">result offset</param>
        ''' <returns>
        '''ledger_id = ledger info
        '''refid = reference id
        '''time = unx timestamp of ledger
        '''type = type of ledger entry
        '''aclass = asset class
        '''asset = asset
        '''amount = transaction amount
        '''fee = transaction fee
        '''balance = resulting balance
        ''' </returns>
        Public Function GetLedgers(Optional aclass As String = "currency",
                                   Optional asset As String = "all",
                                   Optional type As String = "all",
                                   Optional start As Double = 0,
                                   Optional [end] As Double = 0,
                                   Optional ofs As String = "") As String
            Dim reqs As String = String.Format("&ofs={0}", ofs)
            If Not String.IsNullOrEmpty(aclass) Then
                reqs += String.Format("&aclass={0}", asset)
            End If
            If Not String.IsNullOrEmpty(type) Then
                reqs += String.Format("&type={0}", type)
            End If
            If start > 0 Then
                reqs += String.Format("&start={0}", start.ToString(System.Globalization.CultureInfo.InvariantCulture))
            End If
            If [end] > 0 Then
                reqs += String.Format("&end={0}", [end].ToString(System.Globalization.CultureInfo.InvariantCulture))
            End If
            Return QueryPrivate("Ledgers", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id">comma delimited list of ledger ids to query info about (20 maximum)</param>
        ''' <returns>ledger_id = ledger info.  See Get ledgers info</returns>
        Public Function QueryLedgers(Optional id As String = "") As String
            Dim reqs As String = String.Format("&id={0}", id)
            Return QueryPrivate("QueryLedgers", reqs)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pair">comma delimited list of asset pairs to get fee info on (optional)</param>
        ''' <returns>currency = volume currency
        '''volume = current discount volume
        '''fees = array of asset pairs and fee tier info (if requested)
        '''fee = current fee in percent
        '''minfee = minimum fee for pair (if not fixed fee)
        '''maxfee = maximum fee for pair (if not fixed fee)
        '''nextfee = next tier's fee for pair (if not fixed fee.  nil if at lowest fee tier)
        '''nextvolume = volume level of next tier (if not fixed fee.  nil if at lowest fee tier)
        '''tiervolume = volume level of current tier (if not fixed fee.  nil if at lowest fee tier)
        '''</returns>
        Public Function GetTradeVolume(Optional pair As String = "") As String
            Dim reqs As String = String.Format("&pair={0}", pair)
            Return QueryPrivate("TradeVolume", reqs)
        End Function

#End Region

#Region "Private user trading"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pair">asset pair</param>
        ''' <param name="type">type of order (buy/sell)</param>
        ''' <param name="ordertype">ordertype = order type:
        '''market
        '''limit (price = limit price)
        '''stop-loss (price = stop loss price)
        '''take-profit (price = take profit price)
        '''stop-loss-profit (price = stop loss price, price2 = take profit price)
        '''stop-loss-profit-limit (price = stop loss price, price2 = take profit price)
        '''stop-loss-limit (price = stop loss trigger price, price2 = triggered limit price)
        '''take-profit-limit (price = take profit trigger price, price2 = triggered limit price)
        '''trailing-stop (price = trailing stop offset)
        '''trailing-stop-limit (price = trailing stop offset, price2 = triggered limit offset)
        '''stop-loss-and-limit (price = stop loss price, price2 = limit price)</param>
        ''' <param name="volume">order volume in lots</param>
        ''' <param name="price">price (optional.  dependent upon ordertype)</param>
        ''' <param name="price2">secondary price (optional.  dependent upon ordertype)</param>
        ''' <param name="leverage">amount of leverage desired (optional.  default = none)</param>
        ''' <param name="position">position tx id to close (optional.  used to close positions)</param>
        ''' <param name="oflags">oflags = comma delimited list of order flags (optional):
        '''viqc = volume in quote currency
        '''plbc = prefer profit/loss in base currency
        '''nompp = no market price protection</param>
        ''' <param name="starttm">scheduled start time (optional):
        '''0 = now (default)
        '''+[n] = schedule start time [n] seconds from now
        '''n = unix timestamp of start time</param>
        ''' <param name="expiretm">expiration time (optional):
        ''' 0 = no expiration (default)
        '''+n = expire n seconds from now
        '''n = unix timestamp of expiration time</param>
        ''' <param name="userref">user reference id.  32-bit signed number.  (optional)</param>
        ''' <param name="validate">validate inputs only.  do not submit order (optional)</param>
        ''' <param name="close">optional closing order to add to system when order gets filled:
        '''close[ordertype] = order type
        '''close[price] = price
        '''close[price2] = secondary price</param>
        ''' <returns>
        ''' descr = order description info
        '''order = order description
        '''close = conditional close order description (if conditional close set)
        '''txid = array of transaction ids for order (if order was added successfully)
        ''' </returns>
        Public Function AddOrder(pair As String, type As String, ordertype As String, volume As Decimal, price As System.Nullable(Of Decimal), price2 As System.Nullable(Of Decimal),
            Optional leverage As String = "none", Optional position As String = "", Optional oflags As String = "", Optional starttm As String = "", Optional expiretm As String = "", Optional userref As String = "",
            Optional validate As Boolean = False, Optional close As Dictionary(Of String, String) = Nothing) As String
            Dim reqs As String = String.Format("&pair={0}&type={1}&ordertype={2}&volume={3}&leverage={4}", pair, type, ordertype, volume, leverage)
            If price.HasValue Then
                reqs += String.Format("&price={0}", price.Value)
            End If
            If price2.HasValue Then
                reqs += String.Format("&price2={0}", price2.Value)
            End If
            If Not String.IsNullOrEmpty(position) Then
                reqs += String.Format("&position={0}", position)
            End If
            If Not String.IsNullOrEmpty(starttm) Then
                reqs += String.Format("&starttm={0}", starttm)
            End If
            If Not String.IsNullOrEmpty(expiretm) Then
                reqs += String.Format("&expiretm={0}", expiretm)
            End If
            If Not String.IsNullOrEmpty(oflags) Then
                reqs += String.Format("&oflags={0}", oflags)
            End If
            If Not String.IsNullOrEmpty(userref) Then
                reqs += String.Format("&userref={0}", userref)
            End If
            If validate Then
                reqs += "&validate=true"
            End If
            If close IsNot Nothing Then
                Dim closeString As String = String.Format("&close[ordertype]={0}&close[price]={1}&close[price2]={2}", close("ordertype"), close("price"), close("price2"))
                reqs += closeString
            End If
            Return QueryPrivate("AddOrder", reqs)
        End Function

        'Public Function AddOrder(krakenOrder As KrakenOrder) As JObject
        '    Return AddOrder(pair:=krakenOrder.Pair, type:=krakenOrder.Type, ordertype:=krakenOrder.OrderType, volume:=krakenOrder.Volume, price:=krakenOrder.Price, price2:=krakenOrder.Price2, _
        '        leverage:=If(krakenOrder.Leverage, "none"), position:=If(krakenOrder.Position, String.Empty), oflags:=If(krakenOrder.OFlags, String.Empty), starttm:=If(krakenOrder.Starttm, String.Empty), expiretm:=If(krakenOrder.Expiretm, String.Empty), userref:=If(krakenOrder.Userref, String.Empty), _
        '        validate:=krakenOrder.Validate, close:=krakenOrder.Close)
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="txid">transaction id</param>
        ''' <returns>
        ''' count = number of orders canceled
        '''pending = if set, order(s) is/are pending cancellation
        ''' </returns>
        Public Function CancelOrder(txid As String) As String
            Dim reqs As String = String.Format("&txid={0}", txid)
            Return QueryPrivate("CancelOrder", reqs)
        End Function

#End Region

#Region "Helper methods"

        Private Function sha256_hash(value As [String]) As Byte()
            Using hash As SHA256 = SHA256Managed.Create()
                Dim enc As Encoding = Encoding.UTF8
                Dim result As [Byte]() = hash.ComputeHash(enc.GetBytes(value))
                Return result
            End Using
        End Function

        Private Function getHash(keyByte As Byte(), messageBytes As Byte()) As Byte()
            Using hmacsha512 = New HMACSHA512(keyByte)
                Dim result As Byte() = hmacsha512.ComputeHash(messageBytes)
                Return result
            End Using
        End Function

#End Region

    End Class

    ''' <summary>
    ''' Hilfsklasse für das sequenzielle Abrufen von Ledger-Einträgen via API. 
    ''' Beim Abrufen eines Eintrags wird bei Bedarf die nächste API-Datenseite abgerufen.
    ''' Nutzt Statusanzeigen via ProgressWaitFormManager
    ''' </summary>
    Public Class KrakenApiLedger

        Private _ClientKraken As KrakenClient
        Private _QryLedger As JObject
        Private _PageEntries As Integer
        Private _PageIndex As Integer
        Private _Page As Integer
        Private _ApiConfigName As String
        Private _CurrentYoungestTimestamp As Double
        Private _LastestImportTimestamp As Double
        Private _TimeDelta As Single
        Private _KrakenChunkLimit As Integer
        Private _LastPage As Boolean
        Private _CurrentLedgerItem As Newtonsoft.Json.Linq.JProperty


        Public Sub New(ByRef ClientKraken As KrakenClient,
                       ByVal LastImportTimestamp As Long,
                       ByVal ApiConfigName As String,
                       Optional ByVal EndTimestamp As Long = -1,
                       Optional ByVal KrakenChunkLimit As Integer = 49)
            _QryLedger = Nothing
            _PageEntries = 0
            _PageIndex = 0
            _Page = 0
            _CurrentLedgerItem = Nothing
            If EndTimestamp <= 0 Then
                _CurrentYoungestTimestamp = DateToUnixTimestamp(New Date(2099, 12, 31))
            Else
                _CurrentYoungestTimestamp = EndTimestamp
            End If
            _TimeDelta = 0.0001
            _KrakenChunkLimit = KrakenChunkLimit
            _ClientKraken = ClientKraken
            _LastestImportTimestamp = LastImportTimestamp
            _ApiConfigName = ApiConfigName
        End Sub

        ''' <summary>
        ''' Anzahl Ledger-Einträge in der aktuellen Seite
        ''' </summary>
        Public Property PageEntries() As Integer
            Get
                Return _PageEntries
            End Get
            Set(ByVal value As Integer)
                _PageEntries = value
            End Set
        End Property

        ''' <summary>
        ''' Holt den nächsten Legder-Eintrag ab.
        ''' </summary>
        ''' <returns>Nothing, wenn es keine Ledger-Einträge mehr gibt.</returns>
        Public Function GetNextLedgerItem() As JProperty
            If _ClientKraken Is Nothing Then
                Throw New ApplicationException("Der Kraken-Datenabruf ist nicht initialisiert.")
                Exit Function
            End If
            If _PageIndex = _PageEntries Then
                If _LastPage Then
                    ' wir waren auf der letzten Seite...
                    Return Nothing
                Else
                    ' nächste Seite abholen
                    _LastPage = Not GetNextLedgerPage()
                End If
            End If
            If _CurrentLedgerItem Is Nothing Then
                _CurrentLedgerItem = _QryLedger("result")("ledger").First
            Else
                _CurrentLedgerItem = _CurrentLedgerItem.Next
            End If
            _PageIndex += 1
            ' Ältesten Timestamp merken, um den nächsten Seitenabruf dort aufsetzen zu können
            If _CurrentLedgerItem IsNot Nothing AndAlso CDbl(JObject.Parse(_CurrentLedgerItem.Value.ToString)("time").ToString) < _CurrentYoungestTimestamp Then
                _CurrentYoungestTimestamp = CDbl(JObject.Parse(_CurrentLedgerItem.Value.ToString)("time").ToString)
            End If
            Return _CurrentLedgerItem
        End Function

        ''' <summary>
        ''' Ruft die nächste API-Tradedatenseite ab.
        ''' </summary>
        ''' <returns>True, wenn es (ggf.) noch weitere Seiten gibt. False, wenn es keine weiteren Seiten mehr gibt</returns>
        Private Function GetNextLedgerPage() As Boolean
            _Page += 1
            ProgressWaitManager.UpdateProgress(String.Format("Kraken.com:{2}{1}: Rufe Trade-Daten ({0}. Seite) ab. Bitte warten Sie...", _Page, _ApiConfigName, Environment.NewLine))
            WriteLogEntry(String.Format("Kraken.com API-Aufruf: GetNextLedgerPage: start: '{0}', end: '{1}'", _
                                        _LastestImportTimestamp, _CurrentYoungestTimestamp - _TimeDelta), TraceEventType.Information)
            _QryLedger = JObject.Parse(_ClientKraken.GetLedgers(, , , _LastestImportTimestamp, _CurrentYoungestTimestamp - _TimeDelta))
            If _QryLedger("error").ToString <> "[]" Then
                ' API meldet einen Fehler
                Throw New Exception("Der Kraken-Server meldet einen Fehler: " & _QryLedger("error")(0).ToString)
                Return False
                Exit Function
            End If

            _PageEntries = DirectCast(_QryLedger("result")("ledger"), ICollection).Count
            _PageIndex = 0
            _CurrentLedgerItem = Nothing
            WriteLogEntry(String.Format("Kraken.com API-Aufruf: GetNextLedgerPage: {0} Ledger-Einträge erhalten", _
                                        _PageEntries), TraceEventType.Information)

            Return _PageEntries >= _KrakenChunkLimit

        End Function

    End Class
End Namespace

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

Namespace BitfinexClient

    Friend Enum WalletTypes
        Exchange = 0
        Funding
        Margin
    End Enum

    Friend Enum TransactionTypes
        Undefined = 0
        Exchange
        Fee
        Deposit
        Withdrawal
        Settlement
        Distribution
        CanceledWithdrawal
    End Enum


    ''' <summary>
    ''' Class for handling information about a bitfinex account, namely storing active currencies
    ''' </summary>
    Friend Class BitfinexAccountInfo : Inherits AccountInfo

        Public Sub New(ExtendedInfo As String)
            MyBase.New(New String(,) {{"USD", "US Dollar"},
                       {"EUR", "Euro"},
                       {"ZRX", "0x"},
                       {"ELF", "aelf"},
                       {"AID", "AidCoin"},
                       {"AIO", "Aion"},
                       {"ANT", "Aragon"},
                       {"REP", "Augur"},
                       {"AVT", "Aventus"},
                       {"BAT", "Basic Attention Token"},
                       {"BTC", "Bitcoin"},
                       {"BCH", "Bitcoin Cash"},
                       {"BTG", "Bitcoin Gold"},
                       {"BCI", "BitcoinInterest"},
                       {"RRT", "Bitfinex Recovery Right Token"},
                       {"BFX", "Bitfinex Token"},
                       {"VEE", "BLOCKv"},
                       {"BFT", "BnkToTheFuture"},
                       {"CFI", "Cofound.it"},
                       {"DAD", "DADI"},
                       {"DAI", "Dai Stablecoin"},
                       {"DRK", "Darkcoin"},
                       {"DSH", "Dash"},
                       {"MNA", "Decentraland"},
                       {"DTH", "Dether"},
                       {"EDO", "Eidoo"},
                       {"EOS", "EOS"},
                       {"ETH", "Ether"},
                       {"ETC", "Ether Classic"},
                       {"FUN", "FunFair"},
                       {"GNT", "Golem"},
                       {"RLC", "iExec"},
                       {"IOS", "IOSToken"},
                       {"IOT", "Iota (MIOTA)"},
                       {"KNC", "Kyber"},
                       {"LTC", "Litecoin"},
                       {"LRC", "Loopring"},
                       {"LYM", "Lympo"},
                       {"MKR", "Maker"},
                       {"MTN", "Medicalchain"},
                       {"ETP", "Metaverse (ETP)"},
                       {"MIT", "Mithril"},
                       {"XMR", "Monero"},
                       {"NEO", "NEO"},
                       {"ODE", "Odem"},
                       {"OMG", "OmiseGO"},
                       {"POA", "POA Network"},
                       {"QSH", "QASH"},
                       {"QTM", "Qtum"},
                       {"RDN", "Raiden"},
                       {"RCN", "Rcoin"},
                       {"REQ", "Request Network"},
                       {"XRP", "Ripple"},
                       {"SAN", "Santiment"},
                       {"SNG", "SingularDTV"},
                       {"AGI", "SingularityNET"},
                       {"SPK", "SpankChain"},
                       {"SNT", "Status"},
                       {"XLM", "Stellar Lumen"},
                       {"STJ", "Storj"},
                       {"DAT", "Streamr"},
                       {"TRX", "TRON"},
                       {"TNB", "Time New Bank"},
                       {"UTK", "UTRUST"},
                       {"VEN", "VeChain"},
                       {"XVG", "Verge"},
                       {"WAX", "WAX"},
                       {"YYW", "YOYOW"},
                       {"ZEC", "ZCash"}}, ExtendedInfo)
        End Sub

        Public Sub New()
            Me.New("USD|BTC|LTC|ETH|ETC|BCH|BFX|RRT")
        End Sub

    End Class

    Friend Class BitfinexClient
        Inherits ApiClientBase

        ''' <summary>
        ''' Default pause between two calls in milliseconds
        ''' </summary>
        Friend Const BITFINEX_API_DEFAULTINTERVAL As Integer = 5500

        Private _url As String
        Private _version As Integer
        Private _key As String
        Private _secret As String
        Private _lastapitimestamp As Long

        Public Sub New(ApiKey As String, ApiSecret As String)
            ' TODO: Work in progress... No need to hard-code this...!
            _url = "https://api.bitfinex.com"
            _version = "1"
            _key = ApiKey
            _secret = ApiSecret
            CallDelay = BITFINEX_API_DEFAULTINTERVAL
        End Sub

        Public Sub New(ApiKey As String, ApiSecret As String, MinDelayBetweenApiCalls As Long)
            Me.New(ApiKey, ApiSecret)
            CallDelay = MinDelayBetweenApiCalls
        End Sub

        ''' <summary>
        ''' Calls the given API method, adds parameter if needed.
        ''' </summary>
        ''' <param name="methodsubpath">method name, added to the api base path</param>
        ''' <param name="parameter">parameters for method, if needed. To be formatted as GET parameters</param>
        ''' <param name="limit">limits the number of returned API data objects. Parameter will be omitted if less or equal zero.</param>
        Private Function QueryAPI(ByVal methodsubpath As String,
                                  Optional ByVal parameter As String = "",
                                  Optional ByVal limit As UInt32 = 999999) As String

            ' Wait if needed...
            WaitForNextApiCall()

            Dim Retry As Boolean = False
            Dim Trials As Integer = 0
            Dim hmac_data As String
            Dim hmac_dataBytes As Byte()
            Dim secretBytes As Byte()
            Dim webRequest_1 As HttpWebRequest
            Dim address As String
            Dim payload As String
            Dim path As String
            Dim nonce As Long

            Randomize()

            Do

                ' Log
                WriteLogEntry(String.Format("Bitfinex.com API-Aufruf: QueryAPI: Methode: '{0}', Parameter: '{1}'",
                                            methodsubpath, parameter), TraceEventType.Information)

                ' generate a 64 bit nonce using a timestamp at tick resolution
                nonce = Date.Now.Ticks

                path = String.Format("/v{0}/{1}", _version, methodsubpath)

                If limit > 0 AndAlso Trials = 0 Then
                    parameter &= String.Format("{0}""limit"":{1}", IIf(parameter.Length > 0, ",", ""), limit)
                End If

                payload = String.Format("{{""request"":""{0}"",""nonce"":""{1}""{2}{3}}}", path, nonce.ToString, IIf(parameter.Length > 0, ",", ""), parameter)
                payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload))

                address = _url & path

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                webRequest_1 = DirectCast(WebRequest.Create(address), HttpWebRequest)
                webRequest_1.ContentType = "application/x-www-form-urlencoded"
                webRequest_1.Method = "POST"
                webRequest_1.Headers.Add("X-BFX-APIKEY", _key)
                webRequest_1.Headers.Add("X-BFX-PAYLOAD", payload)

                ' Create the signature 
                secretBytes = Encoding.UTF8.GetBytes(_secret)
                hmac_dataBytes = Encoding.UTF8.GetBytes(payload)
                hmac_data = getStringHex(getHash(secretBytes, hmac_dataBytes)).ToLower
                webRequest_1.Headers.Add("X-BFX-SIGNATURE", hmac_data)

                'Make the request
                Try
                    Using webResponse As WebResponse = webRequest_1.GetResponse()
                        Using str As Stream = webResponse.GetResponseStream()
                            Using sr As New StreamReader(str)
                                hmac_data = sr.ReadToEnd
                                ' Log
                                WriteLogEntry(String.Format("Bitfinex.com API-Rückgabe:  {0}",
                                                        hmac_data), TraceEventType.Information)
                            End Using
                        End Using
                    End Using
                Catch wex As WebException
                    Using response As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                        Using str As Stream = response.GetResponseStream()
                            Using sr As New StreamReader(str)
                                WriteLogEntry(String.Format("Bitfinex.com API-Aufruf: QueryAPI: Server meldet Fehler! ResponseCode: {0}, Rückgabe: '{1}'",
                                    response.StatusCode, sr.ReadToEnd), TraceEventType.Information)
                                ' Check for error 'too many requests' -> if so, pause between 15 and 25 secs and retry
                                'If response.StatusCode = 429 Then
                                '    Thread.Sleep((Rnd() * 10 + 15) * 1000)
                                '    Retry = True
                                '    Trials += 1
                                'Else
                                '    Throw
                                '    Retry = False
                                'End If
                            End Using
                        End Using
                    End Using
                    Throw wex
                    hmac_data = Nothing
                End Try
            Loop While Retry And Trials <= 3
            Return hmac_data
        End Function

#Region "API request methods"

        ''' <summary>
        ''' Abruf der History-Einträge für die gewählte Currency
        ''' </summary>
        ''' <param name="Currency">Currency, die abgefragt wird</param>
        ''' <param name="datetime_start">Startzeitpunkt der abgerufenen Einträge</param>
        ''' <param name="datetime_end">Endzeitpunkt der abgerufenen Einträge</param>
        Friend Function BalanceHistory(ByRef currency As String,
                                       Optional datetime_start As Double = 0,
                                       Optional datetime_end As Double = 0) As String
            Dim parameters As String = ""
            If datetime_start > 0 Then
                parameters &= String.Format("""since"":""{0}""", datetime_start)
            End If
            If datetime_end > 0 Then
                parameters &= IIf(parameters.Length > 0, ",", "") & String.Format("""until"":""{0}""", datetime_end)
            End If
            parameters &= IIf(parameters.Length > 0, ",", "") & String.Format("""currency"":""{0}""", currency)
            Return QueryAPI("history", parameters)
        End Function

#End Region

#Region "Helper methods"

        Private Function sha384_hash(value As [String]) As Byte()
            Using hash As SHA384 = SHA384Managed.Create()
                Dim enc As Encoding = Encoding.UTF8
                Dim result As [Byte]() = hash.ComputeHash(enc.GetBytes(value))
                Return result
            End Using
        End Function

        Private Function getHash(keyByte As Byte(), messageBytes As Byte()) As Byte()
            Using hmacsha384 = New HMACSHA384(keyByte)
                Dim result As Byte() = hmacsha384.ComputeHash(messageBytes)
                Return result
            End Using
        End Function

        Private Function getStringHex(messageBytes As Byte()) As String
            Dim messageString As String = ""
            For i As Integer = 0 To messageBytes.Length - 1
                messageString &= messageBytes(i).ToString("X2")
            Next
            Return messageString
        End Function

#End Region

    End Class

    ''' <summary>
    ''' Holds information about a certain Bitfinex ledger item in a more abstracted way to make further processing easier
    ''' </summary>
    Friend Class BitfinexLedgerItem

        Private _currency1 As String
        Public Property Currency1() As String
            Get
                Return _currency1
            End Get
            Set(ByVal value As String)
                _currency1 = value
            End Set
        End Property

        Private _currency2 As String
        Public Property Currency2() As String
            Get
                Return _currency2
            End Get
            Set(ByVal value As String)
                _currency2 = value
            End Set
        End Property

        Private _wallet As WalletTypes
        Public Property Wallet() As WalletTypes
            Get
                Return _wallet
            End Get
            Set(ByVal value As WalletTypes)
                _wallet = value
            End Set
        End Property

        Private _txtype As TransactionTypes
        Public Property TransactionType() As TransactionTypes
            Get
                Return _txtype
            End Get
            Set(ByVal value As TransactionTypes)
                _txtype = value
            End Set
        End Property

        Private _time As String
        Public Property Time() As DateTime
            Get
                Return _time
            End Get
            Set(ByVal value As DateTime)
                _time = value
            End Set
        End Property

        Private _feetxid As String
        Public Property FeeTransactionID() As String
            Get
                Return _feetxid
            End Get
            Set(ByVal value As String)
                _feetxid = value
            End Set
        End Property

        Private _amount As Decimal
        Public Property Amount() As Decimal
            Get
                Return _amount
            End Get
            Set(ByVal value As Decimal)
                _amount = value
            End Set
        End Property

        Private _description As String
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Private _processed As Boolean
        Public Property Processed() As Boolean
            Get
                Return _processed
            End Get
            Set(ByVal value As Boolean)
                _processed = value
            End Set
        End Property

        Private _internalID As Long
        Public Property InternalID() As Long
            Get
                Return _internalID
            End Get
            Set(ByVal value As Long)
                _internalID = value
            End Set
        End Property

        Public Sub New()
            _currency1 = ""
            _currency2 = ""
            _wallet = WalletTypes.Exchange
            _time = DATENULLVALUE
            _txtype = TransactionTypes.Undefined
            _amount = 0
            _feetxid = ""
            _description = ""
            _processed = False
        End Sub

        Public Sub New(ByVal Currency1 As String,
                       ByVal Currency2 As String,
                       ByVal Type As TransactionTypes,
                       ByVal Amount As Decimal,
                       ByVal Time As DateTime,
                       ByVal FeeTransactionID As String,
                       Optional ByVal Description As String = "",
                       Optional ByVal InternalID As Long = -1,
                       Optional ByVal Wallet As WalletTypes = WalletTypes.Exchange)
            _currency1 = Currency1
            _currency2 = Currency2
            _wallet = Wallet
            _time = Time
            _txtype = Type
            _feetxid = FeeTransactionID
            _amount = Amount
            _description = Description
            _processed = False
            _internalID = InternalID
        End Sub
    End Class
End Namespace

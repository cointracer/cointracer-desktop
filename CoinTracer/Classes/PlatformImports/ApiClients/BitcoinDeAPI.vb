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

Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports Newtonsoft.Json.Linq

Namespace BitcoinDeClient

    Friend Class BitcoinDeAccountInfo : Inherits AccountInfo

        Public Sub New(ExtendedInfo As String)
            MyBase.New(New String(,) {{"BTC", "Bitcoin"},
                       {"BCH", "Bitcoin Cash"},
                       {"BTG", "Bitcoin Gold"},
                       {"ETH", "Ether"},
                       {"BSV", "Bitcoin SV"},
                       {"LTC", "Litecoin"},
                       {"XRP", "Ripple"},
                       {"DOGE", "Dogecoin"}}, ExtendedInfo)
        End Sub

        Public Sub New()
            Me.New("BTC|BCH|BTG|ETH|BSV|LTC|XRP|DOGE")
        End Sub

    End Class

    Friend Class BitcoinDeClient

        Private Const UNLIMITEDCREDITS As Long = 999999
        Private Const APIWAITUNIT As Long = 1050

        Public Enum TradeTypes
            all = 0
            buy
            sell
        End Enum

        Public Enum LedgerTypes
            all = 0
            buy
            sell
            inpayment
            payout
            affiliate
        End Enum

        Public Enum TradeStateValues
            cancelled = -1
            pending = 0
            succesful = 1
        End Enum

        Public Enum ApiHttpResponseCodes
            Success = 200
            BadRequest = 400
            Forbidden = 403
            EntityNotFound = 404
            NotSuccessful = 422
            TooManyRequests = 429
        End Enum

        Private Const APIDATEFORMAT As String = "yyyy-MM-dd\THH:mm:ss\+01:00"

        Private _url As String
        Private _version As Integer
        Private _key As String
        Private _secret As String
        Private _LastApiCallTS As Long
        Private Shared _ApiCredits As Long
        Private _ApiResponse As Newtonsoft.Json.Linq.JObject

        Public Sub New(ApiKey As String, ApiSecret As String)
            ' TODO: Work in progress... Version und URL müssen natürlich noch dynamisiert werden!
            _url = "https://api.bitcoin.de/"
            _version = "4"
            _key = ApiKey
            _secret = ApiSecret
            _ApiCredits = UNLIMITEDCREDITS   ' optimistische Schätzung... wird nach dem ersten API-Call ohnehin aktualisiert
        End Sub

        ''' <summary>
        ''' Checks if there has been enough time between now and the latest API call and sends this thread to sleep if needed.
        ''' Keeps track of the remaining API credits and calculates waiting time if needed.
        ''' </summary>
        ''' <param name="CreditsNeeded">Number of credits the next API call is going to cost.</param>
        Private Sub WaitForNextApiCall(ByVal CreditsNeeded As Integer)
            WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: WaitForNextApiCall: {0} Credits benötigt / Interner Credit-Zähler: {1}",
                                        CreditsNeeded, _ApiCredits), TraceEventType.Information)
            If _ApiCredits < CreditsNeeded Then
                ' Okay, we have to wait!
                Thread.Sleep((CreditsNeeded - _ApiCredits) * APIWAITUNIT)
            End If
            _ApiCredits -= CreditsNeeded
            WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: WaitForNextApiCall beendet. Interner Credit-Zähler: {0}",
                                        _ApiCredits), TraceEventType.Information)
        End Sub

        ''' <summary>
        ''' Analyses the given JSON-String for information about remaining API credits and updates the internal credit counter
        ''' </summary>
        ''' <param name="ResponseString"></param>
        ''' <remarks></remarks>
        Private Sub UpdateCredits(ByRef ResponseString As String,
                                  ByVal CreditsNeeded As Integer)
            Try
                _ApiResponse = JObject.Parse(ResponseString)
                _ApiCredits = CLng(_ApiResponse("credits").ToString) - CreditsNeeded
                WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: UpdateCredits beendet. Rückgabe von Bitcoin.de: '{0}' / Interner Credit-Zähler: {1}",
                                _ApiCredits + CreditsNeeded,
                                _ApiCredits), TraceEventType.Information)
            Catch ex As Exception
                ' not really a problem, but log it...
                WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: UpdateCredits mit Fehlern beendet, konnte aktuelle Credits nicht ermitteln. Rückgabe von Bitcoin.de: '{0}' / Interner Credit-Zähler: {1}",
                                ResponseString.Substring(Math.Max(0, ResponseString.Length - 50)),
                                _ApiCredits), TraceEventType.Error)
            End Try
        End Sub

        ''' <summary>
        ''' Calls the given API method, adds parameter if needed.
        ''' </summary>
        ''' <param name="methodsubpath">method name, added to the api base path</param>
        ''' <param name="parameter">parameters for method, if needed. To be formatted as GET parameters</param>
        ''' <param name="Credits">API credits needed for the method call</param>
        Private Function QueryAPI(ByVal methodsubpath As String,
                                  Optional ByVal parameter As String = "",
                                  Optional ByVal Credits As Integer = 1) As String

            ' Log
            WriteLogEntry(String.Format("Bitcoin.de API-Aufruf: QueryAPI: Methode: '{0}', Parameter: '{1}', Credits: '{2}' / Interner Credit-Zähler: {3}",
                                        methodsubpath, parameter, Credits, _ApiCredits), TraceEventType.Information)
            ' Wait if needed...
            WaitForNextApiCall(Credits)

            ' generate a 64 bit nonce using a timestamp at tick resolution
            Dim nonce As Int64 = DateTime.Now.Ticks

            Dim path As String = String.Format("v{0}/{1}", _version, methodsubpath)

            Dim address As String = _url & path
            If parameter.Length > 0 Then
                address &= "?" & parameter
            End If
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim webRequest_1 As HttpWebRequest = DirectCast(WebRequest.Create(address), HttpWebRequest)
            webRequest_1.ContentType = "application/x-www-form-urlencoded"
            webRequest_1.Method = "GET"
            webRequest_1.Headers.Add("X-API-KEY", _key)
            webRequest_1.Headers.Add("X-API-NONCE", nonce.ToString)

            ' Create the signature 
            Dim hmac_data As String = "GET#" & address & "#" & _key & "#" & nonce & "#d41d8cd98f00b204e9800998ecf8427e"
            Dim secretBytes As Byte() = Encoding.UTF8.GetBytes(_secret)
            Dim hmac_dataBytes As Byte() = Encoding.UTF8.GetBytes(hmac_data)
            hmac_data = getStringHex(getHash(secretBytes, hmac_dataBytes)).ToLower
            webRequest_1.Headers.Add("X-API-SIGNATURE", hmac_data)

            'Make the request
            Dim retry As Boolean = False
            Do
                Try
                    Using webResponse As WebResponse = webRequest_1.GetResponse()
                        Using str As Stream = webResponse.GetResponseStream()
                            Using sr As New StreamReader(str)
                                hmac_data = sr.ReadToEnd
                                UpdateCredits(hmac_data, Credits)
                            End Using
                        End Using
                    End Using
                Catch wex As WebException
                    If IsNothing(wex.Response) Then
                        ' Basic communication error
                        WriteLogEntry(String.Format(My.Resources.MyStrings.importErrorBitcoinDeApiHttpsCall, wex.Message), TraceEventType.Error)
                        hmac_data = Nothing
                        Throw
                    Else
                        ' API server has responded with an error
                        Using response As HttpWebResponse = DirectCast(wex.Response, HttpWebResponse)
                            Using str As Stream = response.GetResponseStream()
                                Using sr As New StreamReader(str)
                                    WriteLogEntry(String.Format(My.Resources.MyStrings.importErrorBitcoinDeApiServer,
                                                  response.StatusCode, sr.ReadToEnd, _ApiCredits), TraceEventType.Information)
                                    If response.StatusCode = ApiHttpResponseCodes.TooManyRequests Then
                                        ' too much, too fast - wait and retry
                                        Thread.Sleep((Credits + 2) * APIWAITUNIT)
                                        _ApiCredits = 0
                                        retry = True
                                    Else
                                        hmac_data = Nothing
                                        Throw
                                    End If
                                End Using
                            End Using
                        End Using
                    End If
                End Try
            Loop While retry
            Return hmac_data
        End Function

#Region "API request methods"

        ''' <summary>
        ''' Es werden abgeschlossene Trades im JSON Format zurückgegeben. Wenn kein Startzeitpunkt gesetzt ist, werden Trades der letzten 7 Tage zurückgeliefert.
        ''' </summary>
        ''' <param name="type">Trade-Typen, die abgerufen werden.</param>
        ''' <param name="state">Status der abgerufenen Trades. 0 = offen / 1 = erfolgreich (Default) / -1 = abgebrochen.</param>
        ''' <param name="date_start">Zeitpunkt, ab dem Daten geliefert werden sollen</param>
        ''' <param name="date_end">Zeitpunkt, bis zu dem Daten geliefert werden sollen</param>
        ''' <param name="page">Seite, die abgerufen werden soll. Default = 1</param>
        Public Function ShowMyTrades(Optional [type] As TradeTypes = TradeTypes.all,
                                  Optional [state] As TradeStateValues = TradeStateValues.succesful,
                                  Optional date_start As Date = DATENULLVALUE,
                                  Optional date_end As Date = DATENULLVALUE,
                                  Optional ByVal page As Integer = 1) As String
            Dim parameters As String = ""
            If [type] <> TradeTypes.all Then
                parameters = String.Format("type={0}&", [type].ToString)
            End If
            parameters &= "state=" & DirectCast([state], Integer)
            If date_start <> DATENULLVALUE Then
                parameters &= "&date_start=" & System.Web.HttpUtility.UrlEncode(date_start.ToString(APIDATEFORMAT))
            End If
            If date_end <> DATENULLVALUE Then
                parameters &= "&date_end=" & System.Web.HttpUtility.UrlEncode(date_end.ToString(APIDATEFORMAT))
            End If
            If page > 1 Then
                parameters &= "&page=" & page
            End If
            Return QueryAPI("trades", parameters, 3)
        End Function

        ''' <summary>
        ''' Abruf des Kontoauszuges (bis einschl. Vortag möglich). Wenn kein Startzeitpunkt gesetzt ist, werden Trades der letzten 10 Tage zurückgeliefert.
        ''' </summary>
        ''' <param name="currency">Währung, die abgerufen werden soll.</param>
        ''' <param name="type">Ledger-Einträge, die abgerufen werden. Leerstring bzw. "all", "buy", "sell", "inpayment", "payout" oder "affiliate"</param>
        ''' <param name="datetime_start">Startzeitpunkt der abgerufenen Einträge</param>
        ''' <param name="datetime_end">Endzeitpunkt der abgerufenen Einträge</param>
        ''' <param name="page">Seite, die abgerufen werden soll. Default = 1</param>
        Public Function ShowAccountLedger(Optional currency As String = "btc",
                                          Optional [type] As LedgerTypes = LedgerTypes.all,
                                          Optional datetime_start As Date = DATENULLVALUE,
                                          Optional datetime_end As Date = DATENULLVALUE,
                                          Optional ByVal page As Integer = 1) As String
            Dim parameters As String = ""
            If [type] <> LedgerTypes.all Then
                parameters &= String.Format("&type={0}", [type].ToString)
            End If
            If datetime_start <> DATENULLVALUE Then
                parameters &= "&datetime_start=" & System.Web.HttpUtility.UrlEncode(CDate(datetime_start).ToString(APIDATEFORMAT))
            End If
            If datetime_end <> DATENULLVALUE Then
                parameters &= "&datetime_end=" & System.Web.HttpUtility.UrlEncode(CDate(datetime_end).ToString(APIDATEFORMAT))
            End If
            If page <> 1 Then
                parameters &= "&page=" & page
            End If
            Return QueryAPI(String.Format("{0}/account/ledger", currency.ToLower), parameters, 3)
        End Function

        ''' <summary>
        ''' Es wird der gewichtete Durchschnittskurs der letzten
        ''' 3 Stunden und der letzten 12 Stunden im JSON-Format wiedergegeben.
        ''' Der Wert "rate_weighted" gibt in der Regel den gewichteten
        ''' Durchschnittskurs der letzten 3 Stunden an.
        ''' Wird eine kritische Masse an Trades in den letzten 3 Stunden unter-
        ''' schritten, dann wird hier der 12 Stunden Durchschnitt zurückgegeben.
        ''' </summary>
        ''' <returns>
        ''' Rückgabewerte:
        '''   * rate_weighted
        '''   * rate_weighted_3h
        '''   * rate_weighted_12h
        ''' </returns>
        Public Function GetRate() As String
            Return QueryAPI("rate")
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
            Using hmacsha256 = New HMACSHA256(keyByte)
                Dim result As Byte() = hmacsha256.ComputeHash(messageBytes)
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
End Namespace

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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or within the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports Newtonsoft.Json.Linq

Friend Class Import_Bitfinex_Api
    Inherits ApiImportBase
    Implements IApiImport

    Public Sub New(ByRef Import As Import)
        MainImportObject = Import
        Platform = PlatformManager.Platforms.Bitfinex
    End Sub

    ''' <summary>
    ''' Perform an API import for platform bitfinex.com
    ''' </summary>
    ''' <returns>Unix timestamp for the latest imported trade</returns>
    Friend Overrides Function PerformImport() As Long
        Const CHUNK_SIZE As Long = 89 * 24 * 60 * 60                                        ' Size of timeframe for API calls (89 days)
        Dim ClientBitfinex As New BitfinexClient.BitfinexClient(ApiKey, ApiSecret, CallDelay)
        Dim BFXAccInfo As New BitfinexClient.BitfinexAccountInfo(ExtendedInfo)
        Dim Currency As AccountInfo.CryptoCurrency = Nothing
        Dim ThisStartTimestamp As Double
        Dim QryLedger As JObject
        Dim CurrentImport As Date = Now
        Dim Result As String = ""
        Dim ErrCounter As Long = MaxErrors

        ' Check for stashed API import
        Dim StashedImport As ApiImportState? = GetStashedApiImport()

        If StashedImport Is Nothing AndAlso Not MainImportObject.SilentMode Then
            If MsgBoxEx.ShowWithNotAgainOption("ImportBitfinexApiWarning", DialogResult.OK,
                                        My.Resources.MyStrings.importMsgBitfinexApiWarning,
                                        My.Resources.MyStrings.importMsgBitfinexApiWarningTitle,
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel Then
                Return -1
            End If
        End If

        Cursor.Current = Cursors.WaitCursor

        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, PlatformName))
        ProgressWaitManager.WithCancelOption = True

        Try
            ' Get History-Entries for all active currencies
            Dim AnalyseCounter As Long = 1
            Dim PageCounter As Integer
            Try
                ' Loop over each active currency
                Dim EndTimestamp As Double = IIf(DateTimeEnd = DATENULLVALUE, DateToUnixTimestamp(Date.Now), DateToUnixTimestamp(DateTimeEnd))
                Dim ThisResult As String
                Dim DoImport As Boolean
                If StashedImport Is Nothing Then
                    DoImport = True
                Else
                    DoImport = False
                    Result = StashedImport.Value.ImportData(0)
                End If
                For Each Currency In BFXAccInfo.Currencies
                    If DoImport OrElse Currency.Shortname = StashedImport.Value.CurrencyCode Then
                        If DoImport Then
                            ThisStartTimestamp = LastImportTimestamp
                            PageCounter = 1
                        Else
                            ThisStartTimestamp = StashedImport.Value.CurrencyTimestamp
                            PageCounter = StashedImport.Value.PageCounter
                            DoImport = True
                        End If
                        If Currency.Active Then
                            ProgressWaitManager.UpdateProgress(String.Format(My.Resources.MyStrings.importMsgBitfinexApiProgess,
                                                                             Environment.NewLine, Currency.Shortname, ApiConfigName))
                            Do
                                If ThisStartTimestamp <> LastImportTimestamp Then
                                    ProgressWaitManager.UpdateProgress(String.Format(My.Resources.MyStrings.importMsgBitfinexApiPageProgess,
                                                                                     Environment.NewLine, Currency.Shortname, ApiConfigName, PageCounter))
                                End If
                                ThisResult = ClientBitfinex.BalanceHistory(Currency.Shortname, ThisStartTimestamp, ThisStartTimestamp + CHUNK_SIZE)
                                ' Check if user has cancelled
                                If ProgressWaitManager.Canceled Then
                                    Throw New ApplicationException(My.Resources.MyStrings.importMsgApiUserAbort)
                                    Exit Function
                                End If
                                If Not ThisResult.StartsWith("[{""currency"":") AndAlso ThisResult <> "[]" Then
                                    ' API has returned an error
                                    Throw New ApplicationException(String.Format(My.Resources.MyStrings.importMsgApiServerError, ThisResult))
                                    Exit Function
                                End If
                                If ThisResult <> "[]" Then
                                    Result &= IIf(Result.Length > 0, ",", "") & ThisResult.Replace("[", "").Replace("]", "")
                                End If
                                ThisStartTimestamp += CHUNK_SIZE
                                PageCounter += 1
                            Loop Until ThisStartTimestamp >= EndTimestamp
                        End If
                    End If
                Next

            Catch ex As Exception
                If Result.Length > 0 Then
                    ' There have been at least some results, so stash the current state
                    StashedApiImports.PushApiImport(New ApiImportState With {.ImportID = ApiDatenID,
                                                    .CurrencyCode = Currency.Shortname,
                                                    .CurrencyTimestamp = ThisStartTimestamp,
                                                    .PageCounter = PageCounter,
                                                    .ImportData = New List(Of Object) From {Result}})
                End If
                ApiImportFatalError(ex, Result.Length > 0)
                Return -1
            End Try

            QryLedger = JObject.Parse("{""items"":[" & Result.Trim & "]}")

            ' Convert all ledger entries for the actual import routine
            Dim Ledger As New List(Of String())
            For Each LedgerItem As JObject In QryLedger("items")
                Try
                    ProgressWaitManager.UpdateProgress(Math.Min(AnalyseCounter / 500, 1) * ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgApiAnalyseHistoryEntry, AnalyseCounter))
                    If LedgerItem("description").ToString.ToLower.Contains("wallet exchange") Then
                        Dim LedgerLine(4) As String
                        LedgerLine(0) = LedgerItem("currency").ToString
                        LedgerLine(1) = LedgerItem("description").ToString.Replace("Trading fees for", "Trading fee").Replace("wallet Exchange", "Exchange wallet").Replace("wallet exchange", "Exchange wallet").Trim
                        LedgerLine(2) = LedgerItem("amount").ToString
                        LedgerLine(3) = LedgerItem("balance").ToString
                        LedgerLine(4) = LedgerItem("timestamp").ToString()
                        LedgerLine(4) = DateFromUnixTimestamp(Convert.ToDouble(LedgerLine(4), CultureInfo.InvariantCulture)).ToString("yyyy-MM-dd HH:mm:ss")
                        Ledger.Add(LedgerLine)
                    End If
                    AnalyseCounter += 1
                Catch ex As Exception
                    If ApiImportError(ErrCounter, AnalyseCounter, ex) = 0 Then
                        Return 0
                        Exit Function
                    End If
                End Try
            Next

            ' Perform "fake" file import
            If Ledger.Count > 0 Then
                Dim BitfinexImport As New Import_Bitfinex(MainImportObject)
                With BitfinexImport
                    .FileNames = {String.Format(My.Resources.MyStrings.importMsgApiImportLabel, ApiConfigName)}
                    .AllRows = Ledger
                    .ConvertFromUTC = True
                    ' .DeserializeAllRows() ' DEBUG!
                    If Not .PerformImport() Then
                        ErrCounter -= 1
                    End If
                End With
            End If

            Cursor.Current = Cursors.Default
            DestroyProgressForm()

        Catch ex As Exception
            Cursor.Current = Cursors.Default
            DestroyProgressForm()
            Throw
            Exit Function
        End Try

        If ErrCounter = MaxErrors Then
            Return DateToUnixTimestamp(CurrentImport)
        Else
            Return 0
        End If


    End Function

End Class

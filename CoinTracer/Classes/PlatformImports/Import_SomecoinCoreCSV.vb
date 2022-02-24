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

Public Class Import_SomecoinCoreCSV

    ''' <summary>
    ''' Perform the actual import for files exported by Bitcoin Core Client, Litecoin Core Client or BitcoinCash Node
    ''' </summary>
    ''' <returns>True on success, false otherwise</returns>
    ''' <param name="FileImport">The calling import object</param>
    ''' <param name="Account">Coin account being imported here</param>
    Friend Shared Function ImportCSV(ByRef FileImport As IFileImport,
                                     Account As AccountManager.Accounts) As Boolean
        Dim Row() As String
        Dim ErrorCounter As Long = FileImport.MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim AllLines As Long
        Dim i As Long

        If FileImport.CSV.FileExists AndAlso FileImport.CSV.Rows.Count > 1 Then
            Cursor.Current = Cursors.WaitCursor
            FileImport.InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, FileImport.PlatformName))

            ' Import actual data
            Dim AskedForIgnoreTransactionConfirmStatus As Boolean = False
            Dim IgnoreTransactionConfirmStatus As Boolean = False
            ImportRecords = New List(Of dtoTradesRecord)
            AllLines = FileImport.CSV.Rows.Count
            For i = 1 To AllLines - 1
                ProgressWaitManager.UpdateProgress(i + 1 / AllLines * FileImport.ReadImportdataPercentage, String.Format(My.Resources.MyStrings.importMsgReadingFile, i + 1, AllLines))
                Row = FileImport.CSV.Rows(i)
                If Row.Length >= 7 Then
                    Record = New dtoTradesRecord
                    RecordFee = Nothing
                    With Record
                        Try
                            If Row(0) = "false" AndAlso Not AskedForIgnoreTransactionConfirmStatus AndAlso Not FileImport.MainImportObject.SilentMode Then
                                ' first time we see a line beginning with "false", so ask what to do
                                MsgBoxEx.PatchMsgBox(New String() {My.Resources.MyStrings.importMsgCoinCoreUnconfirmedAlso, My.Resources.MyStrings.importMsgCoinCoreConfirmedOnly})
                                IgnoreTransactionConfirmStatus = MessageBox.Show(My.Resources.MyStrings.importMsgCoinCoreAskUnconfirmed,
                                    My.Resources.MyStrings.importMsgCoinCoreAskUnconfirmedTitle,
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button1) = DialogResult.Yes
                                AskedForIgnoreTransactionConfirmStatus = True
                            End If

                            If Row(0) = "true" OrElse (Row(0) = "false" And IgnoreTransactionConfirmStatus) Then
                                .SourceID = MD5FromString(Row(6))
                                .Zeitpunkt = Row(1)
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = FileImport.Platform
                                .QuellKontoID = Account
                                .ZielKontoID = .QuellKontoID
                                If Row(2).StartsWith("Empfangen") OrElse Row(2).StartsWith("Received") Then
                                    ' Deposit
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .ZielPlattformID = FileImport.Platform
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = FileImport.CSV.StringToDecimal(Row(5))
                                ElseIf Row(2).StartsWith("Überwiesen") OrElse Row(2).StartsWith("Sent to") Then
                                    ' Withdrawal
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .QuellPlattformID = FileImport.Platform
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = -FileImport.CSV.StringToDecimal(Row(5))
                                Else
                                    .DoNotImport = True
                                End If
                                .QuellBetrag = .ZielBetrag
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .Info = String.Format(My.Resources.MyStrings.importCoreClientTxInfo,
                                                      Row(3),
                                                      Row(4))

                                If Not .DoNotImport Then
                                    ' Add record to list
                                    ImportRecords.Add(Record)
                                End If
                            End If

                        Catch ex As Exception
                            If FileImport.FileImportError(ErrorCounter, i + 1, ex) = 0 Then
                                Return False
                                Exit Function
                            End If
                        End Try

                    End With
                End If

            Next i

            FileImport.MainImportObject.Import_Records(ImportRecords, FileImport.FileNames(0), FileImport.ReadImportdataPercentage)

            FileImport.DestroyProgressForm()
            Cursor.Current = Cursors.Default
        End If

        Return ErrorCounter = FileImport.MaxErrors
    End Function

End Class

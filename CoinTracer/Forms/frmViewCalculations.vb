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

Public Class frmViewCalculations

    Private _StartID As Long = -1
    Public Property StartID() As Long
        Get
            Return _StartID
        End Get
        Set(ByVal value As Long)
            _StartID = value
        End Set
    End Property

    Public ReadOnly Property RecordsModified() As Integer
        Get
            Return False
        End Get
    End Property


    Private Sub frmViewCalculations_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CalculationsTableAdapter.Fill(CoinTracerDataSet.VW_Berechnungen)

        If _StartID >= 0 Then
            CalculationsBindingSource.Position = CalculationsBindingSource.Find("ID", StartID)
        Else
            CalculationsBindingSource_CurrentChanged(Nothing, Nothing)
        End If

    End Sub


    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        Close()
    End Sub

    Private Sub CalculationsBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles CalculationsBindingSource.CurrentChanged
        IDTextBox.ReadOnly = True
        CalcDateTextBox.ReadOnly = True
        ScenarioTextBox.ReadOnly = True
        CVSTextBox.ReadOnly = True
        If CalculationsBindingSource.Position >= 0 Then
            Dim CurrentRow As CoinTracerDataSet.VW_BerechnungenRow = CalculationsBindingSource.Current.Row
            SyncCVSTextBoxLong(CurrentRow.CVS)
        Else
            SyncCVSTextBoxLong("")
        End If
    End Sub

    Private Sub SyncCVSTextBoxLong(CVS As String)
        Dim Values() As String = CVS.Split("|")
        With grdCVS
            .Rows.Clear()
            Try
                If Values.Length >= 7 Then
                    Dim CVSHelper As New CoinValueStrategy(Values(0))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioX2W, CVSHelper.ExplainCVS)
                    CVSHelper.FromString(Values(1))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioW2X, CVSHelper.ExplainCVS)
                    CVSHelper.FromString(Values(2))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioX2X, CVSHelper.ExplainCVS)
                    CVSHelper.FromString(Values(3))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioC4C, CVSHelper.ExplainCVS)
                    CVSHelper.FromString(Values(4))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioC4F, CVSHelper.ExplainCVS)
                    CVSHelper.FromString(Values(5))
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioWithdrawal, CVSHelper.ExplainCVS)
                    ' Translate long term period
                    Dim Periods() As String = Values(6).Split(" ")
                    Dim Period As String = "- undefined -"
                    Dim Value As Long = 0
                    If Periods.Length = 2 Then
                        Select Case Periods(1)
                            Case "days"
                                Period = My.Resources.MyStrings.globalDays
                            Case "weeks"
                                Period = My.Resources.MyStrings.Weeks
                            Case "years"
                                Period = My.Resources.MyStrings.Years
                        End Select
                        Value = CLng(Periods(0))
                    End If
                    .Rows.Add(My.Resources.MyStrings.cvsScenarioLongTermPeriod, Value & " " & Period)
                End If
                If Values.Length >= 10 Then
                    ' Translate wallet aware setting
                    .Rows.Add(My.Resources.MyStrings.cvsWalletAwareness, IIf(Values(7), My.Resources.MyStrings.cvsWalletAware, My.Resources.MyStrings.cvsWalletUnaware))
                    ' Translate coins4coins setting
                    .Rows.Add(My.Resources.MyStrings.cvsCoin4CoinAwareness, IIf(Values(8), My.Resources.MyStrings.cvsCoin4CoinAware, My.Resources.MyStrings.cvsCoin4CoinUnaware))
                    ' Inpayment / outpayment assignment tolerance
                    .Rows.Add(My.Resources.MyStrings.cvsInOutTolerance, Values(9) & " " & IIf(Values(9) = "1", My.Resources.MyStrings.globalMinute, My.Resources.MyStrings.globalMinutes))
                End If
            Catch ex As Exception
                ' No matter...
            End Try
        End With
    End Sub
End Class

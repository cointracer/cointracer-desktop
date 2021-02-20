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

Public Class frmDonate

    Private _Add As String

    Private _CoinType As String
    Public Property CoinType() As String
        Get
            Return _CoinType
        End Get
        Set(ByVal value As String)
            _CoinType = value
        End Set
    End Property

    Private Sub cmdCopy_Click(sender As Object, e As EventArgs) Handles cmdCopy.Click
        Dim o As New DataObject
        o.SetText(_Add)
        Clipboard.SetDataObject(o, True)
        Me.Close()
    End Sub

    Private Sub frmDonate_Load(sender As Object, e As EventArgs) Handles Me.Load

        ' Adresse festlegen
        Dim Adressen() As String
        Select Case _CoinType
            Case "LTC"
                pcbCointype.Image = My.Resources.coin_logo_litecoin_28px
                lblDonate.Text = My.Resources.MyStrings.donateLabelLTC
                Adressen = {"LhczmeiVdfNViKEUufX79MkpQCdYCUCYji",
                            "LcAcM8jBkRmEC6qJoW8Po7BPLByfsewjQC",
                            "LS6GHNXUnmPmx5iJuatGeMaEw4V8uRhoGg",
                            "LNxkUaUPTacbt358dhorhianBp1ggi3jiQ",
                            "LSXU2DqCqPDpKM1z9DrStVdsVkm89drHFC"}
            Case "BCH"
                pcbCointype.Image = My.Resources.coin_logo_bch_28px
                lblDonate.Text = My.Resources.MyStrings.donateLabelBCH
                Adressen = {"1AvbrD1cUTTZfmqUzfbbt2rbcz582jY17d",
                            "1PXiH8YZQLURr9bjrw8ShyXhDrg8gBz5JU",
                            "1NeN7thH9s2cLnv5stZ1fJN7NroykcJAuP",
                            "13RNsgAU9j7PEorFrBdUDo8mRktYxBcd5y",
                            "134oa1Jhe2Dh18eXfEb7Kows9o5yqMS4Rm"}
            Case "ETH"
                pcbCointype.Image = My.Resources.coin_logo_eth_28px
                lblDonate.Text = My.Resources.MyStrings.donateLabelETH
                Adressen = {"0x4c1e17a91bb6651fb7491a53ce6bf19cb7371d7a",
                            "0x2c45c53cdd38ec278de1ec238274131d52f7e7b2",
                            "0x485de87e2f5a27369d86efb722940c9924dcd10d",
                            "0x123973a41ed57fe56cae20beeaf10f9d0355f05b",
                            "0x1de5fc8f7cba126dcb2784893750f6119f0f2e57"}
            Case Else
                pcbCointype.Image = My.Resources.coin_logo_btc_28px
                lblDonate.Text = My.Resources.MyStrings.donateLabelBTC
                Adressen = {"19D3zh4AVXobiVBpo4Wx46aHFmepGVyC3R",
                            "135EEHZp3vCfphkEcgYmVYZ4icfMjeV4rw",
                            "1NUgcgzcx2t95gZR33DxVJAy7LgDdPYeQ",
                            "1Ekk32GENBxjftXC15Z2WjbHaBXXw9KN2a",
                            "15hov47x5qV7Dxi2GKbKi1Rd7XZgUykQNZ"}
        End Select
        Dim Rnd As New Random()
        _Add = Adressen(Rnd.Next(0, UBound(Adressen) + 1))
        tbxAdresse.Text = _Add

        ' Place labels, calculate width and position of form
        Dim TextLength As Size = TextRenderer.MeasureText(lblDonate.Text,
                                                          lblDonate.Font)
        tbxAdresse.Left = lbldonate.Left + TextLength.Width + 5
        TextLength = TextRenderer.MeasureText(tbxAdresse.Text,
                                              tbxAdresse.Font)
        tbxAdresse.Width = TextLength.Width + 3
        Dim OldWidth As Integer = Width
        Width = tbxAdresse.Left + tbxAdresse.Width + cmdCopy.Width + 35
        Left -= (Width - OldWidth) / 2


    End Sub

    Private Sub tbxAdresse_GotFocus(sender As Object, e As EventArgs) Handles tbxAdresse.Click
        DirectCast(sender, TextBox).SelectAll()
    End Sub

End Class

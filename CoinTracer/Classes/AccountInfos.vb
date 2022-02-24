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

Friend MustInherit Class AccountInfo

    Friend Structure CryptoCurrency
        Public Shortname As String
        Public Longname As String
        Public Active As Boolean
    End Structure

    Private _CurrencyArray() As CryptoCurrency
    ''' <summary>
    ''' All available cryptocurrencies
    ''' </summary>
    Public ReadOnly Property Currencies() As CryptoCurrency()
        Get
            Return _CurrencyArray
        End Get
    End Property

    Public Sub New()
        _CurrencyArray = New CryptoCurrency() {}
    End Sub

    ''' <summary>
    ''' Initiates the object and sets available currencies
    ''' </summary>
    ''' <param name="AvailableCurrencies">2-dimensional array of strings, following this scheme: { {"Shortname1", "Longname1"}, {"Shortname2", "Longname2"}, ...}</param>
    Public Sub New(AvailableCurrencies As String(,))
        Me.New
        If AvailableCurrencies.GetLength(0) > 0 Then
            ReDim _CurrencyArray(AvailableCurrencies.GetLength(0) - 1)
            For i As Integer = 0 To AvailableCurrencies.GetLength(0) - 1
                _CurrencyArray(i).Shortname = AvailableCurrencies(i, 0)
                _CurrencyArray(i).Longname = AvailableCurrencies(i, 1)
                _CurrencyArray(i).Active = True
            Next
        End If
    End Sub

    ''' <summary>
    ''' Initiates the object: sets available currencies and the corresponding active flags
    ''' </summary>
    ''' <param name="AvailableCurrencies">2-dimensional array of strings, following this scheme: { {"Shortname1", "Longname1"}, {"Shortname2", "Longname2"}, ...}</param>
    ''' <param name="ExtendedInfo">Pipe separated string of all currencies that will be marked as active. Available currencies that are not included in this string will be marked inactive.</param>
    Public Sub New(AvailableCurrencies As String(,), ExtendedInfo As String)
        Me.New(AvailableCurrencies)
        Me.ExtendedInfo = ExtendedInfo
    End Sub

    Public Property ExtendedInfo() As String
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)
            Try
                Dim ActiveCoins As String() = Split(value, "|")
                For i As Integer = 0 To _CurrencyArray.Length - 1
                    _CurrencyArray(i).Active = False
                Next
                For Each ActiveCoin As String In ActiveCoins
                    If ActiveCoin.Length > 0 Then
                        SetCurrencyActive(ActiveCoin, True)
                    End If
                Next
            Catch ex As Exception
                Throw New ArgumentException("Parameter 'ExtendedInfo' has an invalid format!", ex)
            End Try
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return GetCurrencyInfo()
    End Function

    ''' <summary>
    ''' Returns each active currency (shortname) as a pipe separated string. Inactive currencies as represented as an empty string within pipes.
    ''' </summary>
    Public Function GetCurrencyInfo() As String
        Dim Settings(_CurrencyArray.Length - 1) As String
        For i As Integer = 0 To _CurrencyArray.Length - 1
            If _CurrencyArray(i).Active Then
                Settings(i) = _CurrencyArray(i).Shortname
            Else
                Settings(i) = ""
            End If
        Next
        Return Join(Settings, "|")
    End Function

    ''' <summary>
    ''' Sets the active flag of a given currency
    ''' </summary>
    Public Sub SetCurrencyActive(Shortname As String, Active As Boolean)
        For i As Integer = 0 To _CurrencyArray.Length - 1
            If _CurrencyArray(i).Shortname = Shortname Then
                _CurrencyArray(i).Active = Active
                Exit For
            End If
        Next
    End Sub

End Class

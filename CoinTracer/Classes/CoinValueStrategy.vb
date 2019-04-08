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

Public Class CoinValueStrategy
    Implements ICloneable

    Public Enum CoinValueStrategies
        YoungestFirst = 1
        OldestFirst
        CheapestFirst
        MostExpensiveFirst
    End Enum

    Public Enum CoinValuePreferrations
        Above1YearPreferred = 0
        Below1YearPreferred = 1
        NothingPreferred = 2
    End Enum

    Private _IsEmpty As Boolean
    Public ReadOnly Property IsEmpty() As Boolean
        Get
            Return _IsEmpty
        End Get
    End Property

    Private _Preferration As CoinValuePreferrations
    Public Property Preferration() As CoinValuePreferrations
        Get
            Return _Preferration
        End Get
        Set(ByVal value As CoinValuePreferrations)
            _Preferration = value
            _IsEmpty = False
        End Set
    End Property

    Private _WalletAware As Boolean
    Public Property WalletAware() As Boolean
        Get
            Return _WalletAware
        End Get
        Set(ByVal value As Boolean)
            _WalletAware = value
            _IsEmpty = False
        End Set
    End Property

    Private _Coin4CoinAware As Boolean
    Public Property Coin4CoinAware() As Boolean
        Get
            Return _Coin4CoinAware
        End Get
        Set(ByVal value As Boolean)
            _Coin4CoinAware = value
        End Set
    End Property

    Public Property CoinValueStrategy() As CoinValueStrategies
        Get
            Return Below1YearStrategy
        End Get
        Set(ByVal value As CoinValueStrategies)
            Below1YearStrategy = value
        End Set
    End Property

    Private _Below1YearStrategy As CoinValueStrategies
    Public Property Below1YearStrategy() As CoinValueStrategies
        Get
            Return _Below1YearStrategy
        End Get
        Set(ByVal value As CoinValueStrategies)
            _Below1YearStrategy = value
            _IsEmpty = False
        End Set
    End Property

    Private _Above1YearStrategy As CoinValueStrategies
    Public Property Above1YearStrategy() As CoinValueStrategies
        Get
            Return _Above1YearStrategy
        End Get
        Set(ByVal value As CoinValueStrategies)
            _Above1YearStrategy = value
            _IsEmpty = False
        End Set
    End Property

    Public Overrides Function ToString() As String
        If _IsEmpty Then
            Return ""
        Else
            Return CInt(Preferration) & "," & _Below1YearStrategy & "," & _Above1YearStrategy & "," & IIf(_WalletAware, "1", "0") & "," & IIf(_Coin4CoinAware, "1", "0")
        End If
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim NewObj As New CoinValueStrategy
        With NewObj
            .Above1YearStrategy = _Above1YearStrategy
            .Below1YearStrategy = _Below1YearStrategy
            .Preferration = _Preferration
            .WalletAware = _WalletAware
            .Coin4CoinAware = _Coin4CoinAware
        End With
        Return NewObj
    End Function

    ''' <summary>
    ''' Initialisiert das Objekt direkt mit dem übergebenen String
    ''' </summary>
    ''' <param name="InitString">4 Zahlen, kommasepariert. Entsprechend der Ausgabe der CoinValueStrategy.ToString()-Funktion.</param>
    Public Sub New(ByRef InitString As String)
        FromString(InitString)
    End Sub

    ''' <summary>
    ''' Initialisiert das Objekt und setzt die Default-Strategie (FiFo, Wallet-Aware, keine Präferenz ob älter oder jünger als 1 Jahr)
    ''' </summary>
    Public Sub New()
        FromString("2,2,2,1,0")
        _IsEmpty = True
    End Sub

    ''' <summary>
    ''' Initialisiert das Objekt anhand des übergebenen Strings
    ''' </summary>
    ''' <param name="InitString">4 Zahlen, kommasepariert. Entsprechend der Ausgabe der CoinValueStrategy.ToString()-Funktion.</param>
    Public Sub FromString(ByRef InitString As String)
        Dim Items() As String
        _IsEmpty = True
        Try
            Items = Split(InitString, ",")
            If DirectCast(Items, ICollection).Count >= 4 Then
                _Preferration = Items(0)
                _Below1YearStrategy = Items(1)
                _Above1YearStrategy = Items(2)
                _WalletAware = (Items(3) = "1")
                _IsEmpty = False
                If DirectCast(Items, ICollection).Count = 5 Then
                    _Coin4CoinAware = (Items(4) = "1")
                Else
                    _Coin4CoinAware = False
                End If
            End If
        Catch ex As Exception
            ' In Fehlerfall einfach _IsEmpty setzen
            _IsEmpty = True
        End Try
    End Sub

    ''' <summary>
    ''' Returns an explanatory text for the coin value settings
    ''' </summary>
    Public Function ExplainCVS() As String
        Dim Result As String
        Result = String.Format(My.Resources.MyStrings.cvsNoPreferrationCvs, CVStoString(Below1YearStrategy), CVStoString(Below1YearStrategy, True))
        ' There is no such thing as preferration below/above 1 year any more, so all of this below is disabled:
        'Select Case Preferration
        '    Case CoinValuePreferrations.Above1YearPreferred
        '        If Above1YearStrategy = Below1YearStrategy Then
        '            Result = String.Format(My.Resources.MyStrings.cvsPrefAbove1Year1Cvs, CVStoString(Above1YearStrategy), CVStoString(Above1YearStrategy, True))
        '        Else
        '            Result = String.Format(My.Resources.MyStrings.cvsPrefAbove1Year2Cvs, CVStoString(Above1YearStrategy), CVStoString(Above1YearStrategy, True),
        '                                   CVStoString(Below1YearStrategy), CVStoString(Below1YearStrategy, True))
        '        End If
        '    Case CoinValuePreferrations.Below1YearPreferred
        '        If Above1YearStrategy = Below1YearStrategy Then
        '            Result = String.Format(My.Resources.MyStrings.cvsPrefBelow1Year1Cvs, CVStoString(Below1YearStrategy), CVStoString(Below1YearStrategy, True))
        '        Else
        '            Result = String.Format(My.Resources.MyStrings.cvsPrefBelow1Year2Cvs, CVStoString(Below1YearStrategy), CVStoString(Below1YearStrategy, True),
        '                                   CVStoString(Above1YearStrategy), CVStoString(Above1YearStrategy, True))
        '        End If
        '    Case Else
        '        Result = String.Format(My.Resources.MyStrings.cvsNoPreferrationCvs, CVStoString(Below1YearStrategy), CVStoString(Below1YearStrategy, True))
        'End Select
        'If WalletAware Then
        '    Return Result & " " & My.Resources.MyStrings.cvsWalletAware
        'Else
        '    Return Result & " " & My.Resources.MyStrings.cvsWalletUnaware
        'End If
        'If Coin4CoinAware Then
        '    Return Result & " " & My.Resources.MyStrings.cvsCoin4CoinAware
        'Else
        '    Return Result & " " & My.Resources.MyStrings.cvsCoin4CoinUnaware
        'End If
        Return Result
    End Function

    Private Function CVStoString(cvs As CoinValueStrategies,
                                 Optional GetLongDescription As Boolean = False)
        Select Case cvs
            Case CoinValueStrategies.OldestFirst
                If GetLongDescription Then
                    Return My.Resources.MyStrings.cvsFifoLong
                Else
                    Return My.Resources.MyStrings.cvsFifo
                End If
            Case CoinValueStrategies.YoungestFirst
                If GetLongDescription Then
                    Return My.Resources.MyStrings.cvsLifoLong
                Else
                    Return My.Resources.MyStrings.cvsLifo
                End If
            Case CoinValueStrategies.CheapestFirst
                If GetLongDescription Then
                    Return My.Resources.MyStrings.cvsLofoLong
                Else
                    Return My.Resources.MyStrings.cvsLofo
                End If
            Case Else
                If GetLongDescription Then
                    Return My.Resources.MyStrings.cvsHifoLong
                Else
                    Return My.Resources.MyStrings.cvsHifo
                End If
        End Select
    End Function
End Class

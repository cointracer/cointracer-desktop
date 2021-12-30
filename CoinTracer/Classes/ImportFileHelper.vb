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

Option Compare Text
Imports System.IO

''' <summary>
''' Exception for invalid trading import files
''' </summary>
<Serializable()>
Public Class InvalidImportFileFormatException
    Inherits Exception
    Implements Runtime.Serialization.ISerializable

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class


''' <summary>
''' Hilfsklasse für das Ermitteln der Plattform/des Ursprungs einer Importdatei anhand der ersten Zeile
''' </summary>
Public Class ImportFileHelper

    Public Enum ImportFileMatchingTypes
        ExactMatch = 0
        StartsWithMatch = 1
        RegexMatch = 2
        ContainsAllMatch = 3
    End Enum

    ''' <summary>
    ''' Struktur für gefundene Plattformen
    ''' </summary>
    Public Structure MatchingPlatform
        Dim PlatformID As Long
        Dim PlatformName As String
        Dim FilesFirstLine As String
        Dim MatchingType As ImportFileMatchingTypes
        Dim SubType As Integer
    End Structure

    Private _MatchingPlatforms As MatchingPlatform()
    ''' <summary>
    ''' Array aller gefundenen Plattformen zur Datei
    ''' </summary>
    Public ReadOnly Property MatchingPlatforms As MatchingPlatform()
        Get
            Return _MatchingPlatforms
        End Get
    End Property

    Private _AllPlatforms As MatchingPlatform()
    ''' <summary>
    ''' Array aller bekannten Plattformen
    ''' </summary>
    Public ReadOnly Property Platforms As MatchingPlatform()
        Get
            Return _AllPlatforms
        End Get
    End Property

    Private _InteractiveMode As Boolean
    ''' <summary>
    ''' Determines if the class should present messages to a user (= true) or throw exceptions (= false)
    ''' </summary>
    Public Property InteractiveMode() As Boolean
        Get
            Return _InteractiveMode
        End Get
        Set(ByVal value As Boolean)
            _InteractiveMode = value
        End Set
    End Property

    Public Sub New()
        _MatchingPlatforms = Nothing
        _InteractiveMode = True
    End Sub

    Public Sub New(ByVal Firstlines() As String)
        Me.New()
        ' Check if all lines are equal - throw error if not!
        Dim Same As Boolean = True
        For i As Integer = 1 To Firstlines.Length - 1
            Same = Firstlines(i - 1) = Firstlines(i)
            If Not Same Then Exit For
        Next
        If Not Same Then
            Dim MessageString As String = My.Resources.MyStrings.importMsgMixedFileFormats
            If InteractiveMode Then
                Cursor.Current = Cursors.Default
                MsgBoxEx.BringToFront()
                MessageBox.Show(MessageString,
                            My.Resources.MyStrings.importMsgMixedFileFormatsTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
            Else
                Throw New InvalidImportFileFormatException(MessageString)
            End If
        Else
            FindMachtingPlatforms(Firstlines(0))
        End If
    End Sub

    ''' <summary>
    ''' Initialisiert das Array aller bekannten Plattformen
    ''' </summary>
    Private Sub InitAllPlatforms()
        Dim Cnt As Integer = 0
        ' Bitcoin Core Client (ab 0.15.0 - DE)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Core Client (ab 0.13.0 - DE)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Confirmed"",""Date"",""Type"",""Label"",""Address"",""Betrag (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Core Client (ab 0.13.0 - EN)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Confirmed"",""Date"",""Type"",""Label"",""Address"",""Amount (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Core Client (vor 0.13.0 - ASCII)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Core Client (vor 0.13.0 - UTF8)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag (BTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Core Client - altes Format
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitcoin Core Client - altes Format UTF8
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBTC)
            .PlatformName = "Bitcoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitcoin Cash Node (DE)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBCH)
            .PlatformName = "Bitcoin Cash Node"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (BCH)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBCH)
            .PlatformName = "Bitcoin Cash Node"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (BCH)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin Cash Node (EN)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletBCH)
            .PlatformName = "Bitcoin Cash Node"
            .FilesFirstLine = """Confirmed"",""Date"",""Type"",""Label"",""Address"",""Amount (BCH)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Litecoin Core Client (ab 2017-12 - DE)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (LTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Etikett"",""Adresse"",""Betrag (LTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Litecoin Core Client
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag (LTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Litecoin Core Client
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag (LTC)"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Litecoin Core Client - altes Format
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Bestätigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Litecoin Core Client - altes Format
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.WalletLTC)
            .PlatformName = "Litecoin Core Client"
            .FilesFirstLine = """Best�tigt"",""Datum"",""Typ"",""Bezeichnung"",""Adresse"",""Betrag"",""ID"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Multibit - DE
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.MultiBit)
            .PlatformName = "MultiBit-Wallet (DE)"
            .FilesFirstLine = "Datum,Beschreibung,Betrag (BTC),Betrag (€),Transaktions-Id"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Multibit - EN
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.MultiBit)
            .PlatformName = "MultiBit-Wallet (EN)"
            .FilesFirstLine = "Date,Description,Amount (BTC),Amount ($),Transaction Id"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' MtGox
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.MtGox)
            .PlatformName = "Mt. Gox"
            .FilesFirstLine = "Index,Date,Type,Info,Value,Balance"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin.de #1
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Referenz;""Kurs (€/BTC)"";""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin.de #2
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Referenz;""Kurs (�/BTC)"";""BTC vor Geb�hr"";""EUR vor Geb�hr"";""BTC nach Geb�hr"";""EUR nach Geb�hr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitcoin.de #3 (since 2017-07)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitcoin.de #4 (since 2017-07)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Geb�hr"";""EUR vor Geb�hr"";""BTC nach Geb�hr"";""EUR nach Geb�hr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitcoin.de #5 (BCH, since 2017-08)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 2
        End With
        Cnt += 1
        ' Bitcoin.de #6 (ETH, since 2017-10)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Gebühr"";""EUR nach Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 3
        End With
        Cnt += 1
        ' Bitcoin.de #7 (BTC, since 2018-02, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitcoin.de #8 (BCH, since 2018-02, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 2
        End With
        Cnt += 1
        ' Bitcoin.de #9 (ETH, since 2018-02, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 3
        End With
        Cnt += 1
        ' Bitcoin.de #10 (BTG, since 2018-02, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTG vor Gebühr"";""EUR vor Gebühr"";""BTG nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 4
        End With
        Cnt += 1
        ' Bitcoin.de #11 (BTC, since 2018-02, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTC vor Gebühr"";""EUR vor Gebühr"";""BTC nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1 + 128
        End With
        Cnt += 1
        ' Bitcoin.de #12 (BCH, since 2018-02, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BCH vor Gebühr"";""EUR vor Gebühr"";""BCH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 2 + 128
        End With
        Cnt += 1
        ' Bitcoin.de #13 (ETH, since 2018-02, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""ETH vor Gebühr"";""EUR vor Gebühr"";""ETH nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 3 + 128
        End With
        Cnt += 1
        ' Bitcoin.de #14 (BTG, since 2018-02, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währungen;Referenz;Kurs;""BTG vor Gebühr"";""EUR vor Gebühr"";""BTG nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de-Gebühr"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 4 + 128
        End With
        Cnt += 1
        ' Bitcoin.de #15 (Any coin, since 2019-01, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währung;Referenz;???-Adresse;Kurs;""Einheit (Kurs)"";""??? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""??? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.RegexMatch
            .SubType = 5
        End With
        Cnt += 1
        ' Bitcoin.de #16 (Any coin, since 2019-01, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währung;Referenz;???-Adresse;Kurs;""Einheit (Kurs)"";""??? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""??? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.RegexMatch
            .SubType = 5 + 128
        End With
        Cnt += 1
        ' Bitcoin.de #17 (Any 4-letter-coin, since 2019-01, no Fidor.de fees)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währung;Referenz;????-Adresse;Kurs;""Einheit (Kurs)"";""???? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""???? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.RegexMatch
            .SubType = 5
        End With
        Cnt += 1
        ' Bitcoin.de #18 (Any 4-letter-coin, since 2019-01, Fidor.de fees included)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitcoinDe)
            .PlatformName = "Bitcoin.de"
            .FilesFirstLine = "Datum;Typ;Währung;Referenz;????-Adresse;Kurs;""Einheit (Kurs)"";""???? vor Gebühr"";""Menge vor Gebühr"";""Einheit (Menge vor Gebühr)"";""???? nach Bitcoin.de-Gebühr"";""Menge nach Bitcoin.de-Gebühr"";""Einheit (Menge nach Bitcoin.de-Gebühr)"";""EUR nach Bitcoin.de- und Fidor-Gebühr"";""Zu- / Abgang"";Kontostand"
            .MatchingType = ImportFileMatchingTypes.RegexMatch
            .SubType = 5 + 128
        End With
        Cnt += 1
        ' Bistamp.net
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitstampNet)
            .PlatformName = "Bistamp.net"
            .FilesFirstLine = "Type,Datetime,BTC,USD,BTC Price,FEE"
            .MatchingType = ImportFileMatchingTypes.ExactMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bistamp.net (inkl. Spalte für Subtyp)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitstampNet)
            .PlatformName = "Bistamp.net"
            .FilesFirstLine = "Type,Datetime,BTC,USD,BTC Price,FEE,Sub Type"
            .MatchingType = ImportFileMatchingTypes.ExactMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bistamp.net (inkl. Account & Amount)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.BitstampNet)
            .PlatformName = "Bistamp.net"
            .FilesFirstLine = "Type,Datetime,Account,Amount,Value,Rate,Fee,Sub Type"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 2
        End With
        Cnt += 1
        ' Kraken.com Leger (until about 2020)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Kraken)
            .PlatformName = "Kraken.com"
            .FilesFirstLine = """txid"",""refid"",""time"",""type"",""aclass"",""asset"",""amount"",""fee"",""balance"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Kraken.com Ledger (2021 and later)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Kraken)
            .PlatformName = "Kraken.com"
            .FilesFirstLine = """txid"",""refid"",""time"",""type"",""subtype"",""aclass"",""asset"",""amount"",""fee"",""balance"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Kraken.com Trades (2021 and later)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Kraken)
            .PlatformName = "Kraken.com"
            .FilesFirstLine = """txid"",""ordertxid"",""pair"",""time"",""type"",""ordertype"",""price"",""cost"",""fee"",""vol"",""margin"",""misc"",""ledgers"""
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 65
        End With
        Cnt += 1
        ' Bitfinex.com
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Bitfinex)
            .PlatformName = "Bitfinex.com"
            .FilesFirstLine = "Currency,Description,Amount,Balance,Date"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Bitfinex.com (since 2019-01)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Bitfinex)
            .PlatformName = "Bitfinex.com"
            .FilesFirstLine = "DESCRIPTION,CURRENCY,AMOUNT,BALANCE,DATE,WALLET"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Bitfinex.com (since 2021-01)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Bitfinex)
            .PlatformName = "Bitfinex.com"
            .FilesFirstLine = "#,DESCRIPTION,CURRENCY,AMOUNT,BALANCE,DATE,WALLET"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 2
        End With
        Cnt += 1
        ' Zyado.com
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Zyado)
            .PlatformName = "Zyado.com"
            .FilesFirstLine = "Type;Amount;Rate;Value;Fee;Date"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Poloniex.com (Withdrawal- oder Deposit-History)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Poloniex)
            .PlatformName = "Poloniex.com"
            .FilesFirstLine = "Date,Currency,Amount,Address,Status"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Poloniex.com (Trade-History ohne 'Category' und 'Order Number', wahrscheinlich älteres Format)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Poloniex)
            .PlatformName = "Poloniex.com"
            .FilesFirstLine = "Date,Market,Type,Price,Amount,Total,Fee,Base Total Less Fee,Quote Total Less Fee"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Poloniex.com (Trade-History mit 'Category' und 'Order Number', neueres Format)
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.Poloniex)
            .PlatformName = "Poloniex.com"
            .FilesFirstLine = "Date,Market,Category,Type,Price,Amount,Total,Fee,Order Number,Base Total Less Fee,Quote Total Less Fee"
            .MatchingType = ImportFileMatchingTypes.StartsWithMatch
            .SubType = 0
        End With
        Cnt += 1
        ' Cointracer #1 - mmaximum set of columns
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;FeePlatform;FeeCurrency;FeeAmount;DateOfAcquisition;TaxAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 7
        End With
        Cnt += 1
        ' Cointracer #2 - fee columns & tax amount included, no DateOfAcquisition
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;FeePlatform;FeeCurrency;FeeAmount;TaxAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 5
        End With
        Cnt += 1
        ' Cointracer #3 - no fee columns included, but DateOfAcquisition and tax amount
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;DateOfAcquisition;TaxAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 6
        End With
        Cnt += 1
        ' Cointracer #4 - nothing but tax amount
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;TaxAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 4
        End With
        Cnt += 1
        ' Cointracer #4 - fees and DateOfAcquisition, but no TaxAmount
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;FeePlatform;FeeCurrency;FeeAmount;DateOfAcquisition"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 3
        End With
        Cnt += 1
        ' Cointracer #2 - fee columns included, no DateOfAcquisition, no TaxAmount
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;FeePlatform;FeeCurrency;FeeAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 1
        End With
        Cnt += 1
        ' Cointracer #3 - no fee columns and no TaxAmount included, but DateOfAcquisition
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount;DateOfAcquisition"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 2
        End With
        Cnt += 1
        ' Cointracer #4 - minimal set of columns
        ReDim Preserve _AllPlatforms(Cnt)
        With _AllPlatforms(Cnt)
            .PlatformID = CInt(PlatformManager.Platforms.CoinTracer)
            .PlatformName = "CoinTracer"
            .FilesFirstLine = "Reference;DateTime;Info;SourcePlatform;SourceCurrency;SourceAmount;TargetPlatform;TargetCurrency;TargetAmount"
            .MatchingType = ImportFileMatchingTypes.ContainsAllMatch
            .SubType = 0
        End With
        Cnt += 1
    End Sub

    ''' <summary>
    ''' Findet alle passenden Plattformen zum übergebenen Dateinamen und dem Inhalt der ersten Zeile
    ''' </summary>
    ''' <param name="Firstline">Inhalt der ersten Zeile</param>
    ''' <returns>Anzahl der gefundenen Plattformen</returns>
    Public Function FindMachtingPlatforms(ByVal Firstline As String,
                                          Optional ByVal PlatformToCheck As PlatformManager.Platforms = PlatformManager.Platforms.Unknown) As Integer
        Dim FoundPlatformCounter As Integer = 0
        If Firstline IsNot Nothing Then
            If _AllPlatforms Is Nothing Then InitAllPlatforms()
            For Each Platform In _AllPlatforms
                If ((Platform.MatchingType = ImportFileMatchingTypes.StartsWithMatch AndAlso Platform.FilesFirstLine.StartsWith(Firstline)) _
                    OrElse (Platform.MatchingType = ImportFileMatchingTypes.ExactMatch AndAlso Platform.FilesFirstLine = Firstline) _
                    OrElse (Platform.MatchingType = ImportFileMatchingTypes.RegexMatch AndAlso Firstline Like Platform.FilesFirstLine)) _
                    OrElse (Platform.MatchingType = ImportFileMatchingTypes.ContainsAllMatch AndAlso Array.TrueForAll(Platform.FilesFirstLine.Split(";"c), Function(s) Firstline.ToLower.Contains(s.ToString.ToLower))) _
                    AndAlso (PlatformToCheck = PlatformManager.Platforms.Unknown OrElse Platform.PlatformID = PlatformToCheck) Then
                    ' We've got a match -> put it in MatchingPlatforms array (if not already in it)
                    If IsNothing(_MatchingPlatforms) OrElse IsNothing(Array.Find(_MatchingPlatforms, Function(x) (x.PlatformID = Platform.PlatformID AndAlso x.SubType = Platform.SubType))) Then
                        ReDim Preserve _MatchingPlatforms(FoundPlatformCounter)
                        With _MatchingPlatforms(FoundPlatformCounter)
                            .FilesFirstLine = Platform.FilesFirstLine
                            .PlatformID = Platform.PlatformID
                            .PlatformName = Platform.PlatformName
                            .SubType = Platform.SubType
                        End With
                        FoundPlatformCounter += 1
                    End If
                End If
            Next
        End If
        If IsNothing(_MatchingPlatforms) Then
            Return 0
        Else
            Return _MatchingPlatforms.Length
        End If
    End Function

    ''' <summary>
    ''' Displays a message about an invalid file format to the user
    ''' </summary>
    ''' <param name="Filename">Fully qualified file name</param>
    Public Sub InvalidFileMessage(ByRef Filename As String)
        Dim MessageString As String = String.Format(My.Resources.MyStrings.importMsgInvalidFileFormat, Path.GetFileName(Filename))
        If InteractiveMode Then
            Cursor.Current = Cursors.Default
            MsgBoxEx.BringToFront()
            MessageBox.Show(MessageString,
                            My.Resources.MyStrings.importMsgInvalidFileFormatTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
        Else
            Throw New InvalidImportFileFormatException(MessageString)
        End If
    End Sub

End Class

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
        LikeMatch = 2
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

    Private Shared _AllPlatforms As MatchingPlatform()
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
                MsgBoxEx.ShowInFront(MessageString,
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

        ' Discover and add all platform headers from FileImportBase derived classes
        Dim FileImport As IFileImport
        Dim Headers As MatchingPlatform()
        Dim i As Integer
        For Each ImportType In Reflection.Assembly.GetExecutingAssembly().GetTypes()
            If ImportType.BaseType?.FullName = "CoinTracer.FileImportBase" Then
                Try
                    FileImport = Activator.CreateInstance(ImportType)
                    Headers = FileImport.PlatformHeaders
                    If Headers IsNot Nothing Then
                        ReDim Preserve _AllPlatforms(_AllPlatforms.Length + Headers.Length - 1)
                        For i = 1 To Headers.Length
                            _AllPlatforms(_AllPlatforms.Length - i) = Headers(Headers.Length - i).Copy
                        Next
                    End If
                Catch ex As MissingMethodException
                    ' doesn't matter...
                End Try
            End If
        Next
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
                    OrElse (Platform.MatchingType = ImportFileMatchingTypes.LikeMatch AndAlso Firstline Like Platform.FilesFirstLine)) _
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
            MsgBoxEx.ShowInFront(MessageString,
                                 My.Resources.MyStrings.importMsgInvalidFileFormatTitle,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation)
        Else
            Throw New InvalidImportFileFormatException(MessageString)
        End If
    End Sub

End Class

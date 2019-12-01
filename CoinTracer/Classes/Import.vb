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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or in the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Imports CoinTracer.DBHelper
Imports CoinTracer.CoinTracerDataSet
Imports CoinTracer.CoinTracerDataSetTableAdapters
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq

''' <summary>
''' Exception thrown if there are too many invalid items during a trade data import 
''' </summary>
<Serializable()>
Public Class TradeDataImportException
    Inherits System.Exception
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
''' Implementiert die Klass für das Importieren von Trade-Daten in die Trade-Tabelle
''' </summary>
Public Class Import
    Implements IDisposable

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If _DB IsNot Nothing Then
                    _DB.Dispose()
                End If
                If _KontenTa IsNot Nothing Then
                    _KontenTa.Dispose()
                End If
                If _KontenTb IsNot Nothing Then
                    _KontenTb.Dispose()
                End If
                If _PlattformenTa IsNot Nothing Then
                    _PlattformenTa.Dispose()
                End If
                If _PlattformenTb IsNot Nothing Then
                    _PlattformenTb.Dispose()
                End If
                If _TVM IsNot Nothing Then
                    _TVM.Dispose()
                End If
                ProgressWaitManager.CloseProgress()
            End If
        End If

        disposedValue = True
    End Sub

    ' TO DO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Delegate Sub ImportRecordsSub(ImportRecords As List(Of dtoTradesRecord),
                                  ByVal Filename As String,
                                  ByVal StartPercentage As Integer,
                                  ByVal ProcessFeeEntries As Boolean,
                                  ByVal IgnoreDublicates As Boolean,
                                  ByVal ApiDatenID As Long,
                                  ByVal LastImportTimestamp As Long,
                                  ByVal Verbose As Boolean)

    ''' <summary>
    ''' Hilfsklasse zum Extrahieren von Kontoname und CODE aus einem String im Format "Kontoname (CODE)". Wird vom u.A. Kraken-Import verwendet.
    ''' </summary>
    Private Class AccountString

        Private _CompleteString As String
        ''' <summary>
        ''' Konto-String im Format "Kontoname (CODE)"
        ''' </summary>
        Public Property AccountString() As String
            Get
                Return _CompleteString
            End Get
            Set(ByVal value As String)
                _CompleteString = value.Trim
            End Set
        End Property
        ''' <summary>
        ''' Kontobezeichnung; aus AccountString extrahiert.
        ''' </summary>
        Public ReadOnly Property AccountLongname() As String
            Get
                If _CompleteString.Contains("(") AndAlso _CompleteString.Length > 2 Then
                    Return _CompleteString.Substring(0, _CompleteString.IndexOf("(")).Trim
                Else
                    Return _CompleteString
                End If
            End Get
        End Property
        ''' <summary>
        ''' Konto-Code; aus AccountString extrahiert.
        ''' </summary>
        Public ReadOnly Property AccountCode() As String
            Get
                If _CompleteString.IndexOf("(") < _CompleteString.IndexOf(")") Then
                    Return _CompleteString.Substring(_CompleteString.IndexOf("(") + 1).Trim(")")
                Else
                    Return _CompleteString
                End If
            End Get
        End Property
        Public Sub New()
            _CompleteString = ""
        End Sub
        Public Sub New(ByVal AccountString As String)
            _CompleteString = AccountString.Trim
        End Sub
    End Class

    Friend Const MESSAGENUMBERFORMAT As String = "#,###,##0"

    Private _MaxErrors As Integer = 6

    Private _ReadImportdataPercentage As Integer = 70     ' Für Progressbar: Anteil des Balkens, der für das reine Einlesen der Daten vorgesehen ist (danach kommt das Einlesen in die Datenbank)

    Private _DB As DBHelper

    Private _ImportFileHelper As ImportFileHelper

    Private _ApiPwCheck As String
    Public ReadOnly Property ApiPasswordCheckPhrase() As String
        Get
            Return _ApiPwCheck
        End Get
    End Property

    Private _ApiPassword As String
    Public Property ApiPassword() As String
        Get
            Return _ApiPassword
        End Get
        Set(ByVal value As String)
            _ApiPassword = value
        End Set
    End Property

    Private _DefaultApiPassword As String
    Public ReadOnly Property DefaultApiPassword() As String
        Get
            Return _DefaultApiPassword
        End Get
    End Property


    Private _SilentMode As Boolean
    Public Property SilentMode() As Boolean
        Get
            Return _SilentMode
        End Get
        Set(ByVal value As Boolean)
            _SilentMode = value
        End Set
    End Property

    Private _TML As ThreadManagerLight
    Public Property ThreadManagerLight() As ThreadManagerLight
        Get
            Return _TML
        End Get
        Set(ByVal value As ThreadManagerLight)
            _TML = value
            _SilentMode = True
        End Set
    End Property

    Private _TVM As TradeValueManager
    Public ReadOnly Property TradeValueManager() As TradeValueManager
        Get
            Return _TVM
        End Get
    End Property
    Public Sub LinkTradeValueManager(ByRef TradeValueManager As TradeValueManager)
        _TVM = TradeValueManager
    End Sub

    Private _KontenTa As KontenTableAdapter
    Public ReadOnly Property KontenTableAdapter() As KontenTableAdapter
        Get
            Return _KontenTa
        End Get
    End Property

    Private _KontenTb As KontenDataTable
    Public ReadOnly Property KontenTable() As KontenDataTable
        Get
            Return _KontenTb
        End Get
    End Property

    Private _PlattformenTa As PlattformenTableAdapter
    Public ReadOnly Property PlattformenTableAdapter() As PlattformenTableAdapter
        Get
            Return _PlattformenTa
        End Get
    End Property

    Private _PlattformenTb As PlattformenDataTable
    Public ReadOnly Property PlattformenTable() As PlattformenDataTable
        Get
            Return _PlattformenTb
        End Get
    End Property

    Private _Plattform As PlatformManager.Platforms
    Public Property Plattform() As PlatformManager.Platforms
        Get
            Return _Plattform
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _Plattform = value
        End Set
    End Property

    Private _ZuletztEingelesen As Long = 0
    Public ReadOnly Property ZuletztEingelesen() As Long
        Get
            Return _ZuletztEingelesen
        End Get
    End Property

    Private _ZuletztUeberprungen As Long = 0
    Public ReadOnly Property ZuletztUeberprungen() As Long
        Get
            Return _ZuletztUeberprungen
        End Get
    End Property

    Private _LastTransfersUpdated As Long
    Public Property LastTransfersUpdated() As Long
        Get
            Return _LastTransfersUpdated
        End Get
        Set(ByVal value As Long)
            _LastTransfersUpdated = value
        End Set
    End Property

    Private _LastTransfersInserted As Long
    Public Property LastTransfersInserted() As Long
        Get
            Return _LastTransfersInserted
        End Get
        Set(ByVal value As Long)
            _LastTransfersInserted = value
        End Set
    End Property

    Private _LastImportId As Long
    Public Property LastImportID() As Long
        Get
            Return _LastImportId
        End Get
        Set(ByVal value As Long)
            _LastImportId = value
        End Set
    End Property

    ''' <summary>
    ''' Initalisiert die Progressform für dieses Modul
    ''' </summary>
    ''' <param name="Message">Startnachricht, die angezeigt werden soll.</param>
    Private Sub InitProgressForm(Optional Message As String = "Starte Datenimport. Bitte warten Sie...")
        Try
            DestroyProgressForm()
            ProgressWaitManager.ShowProgress(_Parentform)
            ProgressWaitManager.UpdateProgress(1, Message)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DestroyProgressForm()
        Try
            ProgressWaitManager.CloseProgress()
            'If _WW IsNot Nothing Then
            '    _WW.CloseProgress()
            '    _WW = Nothing
            'End If
        Catch ex As Exception
            ' no matter...
        End Try
    End Sub

    Private _Parentform As Form
    Public Property Parentform() As Form
        Get
            Return _Parentform
        End Get
        Set(ByVal value As Form)
            _Parentform = value
        End Set
    End Property

    ''' <summary>
    ''' Fordert den Benutzer auf, ein API-Passwort festzulegen
    ''' </summary>
    ''' <remarks>Neues Passwort, Leerstring bei Abbruch.</remarks>
    Public Function GetNewApiPassword() As String
        Dim Pw1 As String = ""
        Dim Pw2 As String = ""
        Do
            If RequestApiPassword(Pw1, "Geben Sie hier das Passwort ein, mit dem Sie die API-Zugangsdaten schützen wollen.", True) = DialogResult.OK Then
                If RequestApiPassword(Pw2, "Wiederholen Sie nun das Passwort. Merken Sie es sich bitte gut, denn es kann nicht zurückgesetzt werden, ohne alle Zugangsdaten zu verlieren!") = DialogResult.OK Then
                    If String.Compare(Pw1, Pw2, False) Then
                        MessageBox.Show("Die beiden eingegebenen Passwörter waren nicht identisch!",
                                        "Fehler bei Passworterfassung", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                Else
                    Return ""
                End If
            Else
                Return ""
            End If
        Loop Until Pw1 = Pw2
        Return Pw2
    End Function


    ''' <summary>
    ''' Fragt den Benutzer nach dem API-Passwort
    ''' </summary>
    ''' <param name="Password">Enthält bei Rückgabe das eingegebene Passwort</param>
    ''' <param name="Explanation">Text, der in der Passwort-Dialogbox angezeigt wird</param>
    ''' <param name="StorePwMode">True, wenn ein neues Passwort erfasst werden soll. In diesem Fall werden bestimmte Kriterien wie
    ''' Länge usw. abgefangen.</param>
    ''' <returns>Eingegebenes Passwort im Klartext</returns>
    Public Function RequestApiPassword(ByRef Password As String,
                                       Optional Explanation As String = "",
                                       Optional StorePwMode As Boolean = False) As DialogResult
        Dim GetPasswordForm As New frmGetPassword
        Dim DlgRes As DialogResult
        If Explanation.Length > 0 Then
            GetPasswordForm.Explanation = Explanation
        End If
        GetPasswordForm.CheckPasswordCriteria = StorePwMode
        DlgRes = GetPasswordForm.ShowDialog(frmMain)
        If DlgRes = DialogResult.OK Then
            Password = GetPasswordForm.Password.Trim
        Else
            Password = ""
        End If
        Return DlgRes
    End Function

    ''' <summary>
    ''' Prüft, ob das übergebene Passwort korrekt ist. Wenn kein Passwort übergeben wird, prüft die Routine, ob die API-Daten mit dem Standard-Passwort verschlüsselt sind.
    ''' </summary>
    ''' <param name="Password">Zu prüfendes Passwort. Wenn leer, wird das Default-Passwort zur Prüfung verwendet.</param>
    Public Function CheckApiPassword(Optional ByVal Password As String = "") As Boolean
        Dim ApiDatenTa As New ApiDatenTableAdapter
        Dim ApiDatenTb As New ApiDatenDataTable
        If Password.Length = 0 Then
            Password = _DefaultApiPassword
        End If
        Dim Crypt As New PushPull(Password)
        If ApiDatenTa.Fill(ApiDatenTb) > 0 Then
            Dim ApiRow As ApiDatenRow = ApiDatenTb(0)
            ' Passwort prüfen
            Try
                Return Crypt.DecryptData(ApiRow.Salt) = ApiRow.ID & _ApiPwCheck
            Catch ex As Exception
                Return False
            End Try
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Führt API-Importe für alle konfigurierten API-Schnittstellen durch.
    ''' </summary>
    Public Sub DoApiImport()

        Dim ApiDatenTa As New ApiDatenTableAdapter
        Dim ApiDatenTb As New ApiDatenDataTable
        Dim Crypt As New PushPull(_ApiPassword)
        Dim LastestUnixTimestamp As Double = 0
        If ApiDatenTa.FillBySQL(ApiDatenTb, "WHERE Aktiv > 0 ORDER BY ID") > 0 Then
            Dim ApiKey As String
            Dim ApiSecret As String
            Dim ApiMsg As String = ""
            Dim RowName As String = My.Resources.MyStrings.unknownInHyphens
            Dim ThisImport As IApiImport = Nothing
            For Each ApiRow As ApiDatenRow In ApiDatenTb
                Try
                    RowName = ApiRow.Bezeichnung
                    ' Passwort prüfen
                    If Crypt.DecryptData(ApiRow.Salt) <> ApiRow.ID & _ApiPwCheck Then
                        ' Ungültiges Passwort: Fehler!
                        Throw New ApplicationException(My.Resources.MyStrings.importMsgApiInvalidPassword)
                        Exit Sub
                    End If
                    ApiMsg = My.Resources.MyStrings.unknownInHyphens
                    ApiKey = Crypt.DecryptData(ApiRow.ApiKey)
                    ApiSecret = Crypt.DecryptData(ApiRow.ApiSecret)
                    _ZuletztEingelesen = 0
                    _ZuletztUeberprungen = 0
                    Select Case ApiRow.PlattformID
                        Case PlatformManager.Platforms.Kraken
                            Plattform = PlatformManager.Platforms.Kraken
                            ThisImport = New Import_Kraken_Api(Me)
                            ApiMsg = "Kraken (""" & RowName & """)"
                        Case PlatformManager.Platforms.BitcoinDe
                            Plattform = PlatformManager.Platforms.BitcoinDe
                            ThisImport = New Import_BitcoinDe_Api(Me)
                            ApiMsg = "Bitcoin.de (""" & RowName & """)"
                        Case PlatformManager.Platforms.Bitfinex
                            Plattform = PlatformManager.Platforms.Bitfinex
                            ThisImport = New Import_Bitfinex_Api(Me)
                            ApiMsg = "Bitfinex.com (""" & RowName & """)"
                        Case Else
                            ' just skip...
                    End Select
                    If ThisImport IsNot Nothing Then
                        With ThisImport
                            .ApiKey = ApiKey
                            .ApiSecret = ApiSecret
                            .LastImportTimestamp = ApiRow.LastImportTimestamp
                            .ApiConfigName = ApiRow.Bezeichnung
                            .ApiDatenID = ApiRow.ID
                            .ExtendedInfo = ApiRow.ExtendedInfo
                            .CallDelay = ApiRow.CallDelay
                            LastestUnixTimestamp = .PerformImport()
                        End With
                    End If
                    If LastestUnixTimestamp > 0 Then
                        ApiRow.LastImportTimestamp = Math.Truncate(LastestUnixTimestamp)
                    End If
                    If LastestUnixTimestamp >= 0 Then
                        MsgBoxEx.BringToFront()
                        MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgApiSummary,
                                                  Environment.NewLine,
                                                  ApiMsg,
                                                  _ZuletztEingelesen.ToString(MESSAGENUMBERFORMAT),
                                                  _ZuletztUeberprungen.ToString(MESSAGENUMBERFORMAT),
                                                  IIf(_LastTransfersUpdated > 0 OrElse _LastTransfersInserted > 0, Environment.NewLine & Environment.NewLine & GetProcessedTransfersString(), "")),
                                    My.Resources.MyStrings.importMsgApiSummaryTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Catch ex As Exception
                    Dim ErrorMessage As String = String.Format(My.Resources.MyStrings.importMsgApiServerError, ApiMsg, ex.Message)
                    If SilentMode Then
                        Throw New ApplicationException(ErrorMessage, ex)
                    Else
                        DefaultErrorHandler(ex, ErrorMessage, False, My.Resources.MyStrings.importMsgApiServerErrorTitle)
                    End If
                End Try
            Next
            ' Save new timestamps
            ApiDatenTa.Update(ApiDatenTb)
        Else
            MessageBox.Show(My.Resources.MyStrings.importMsgApiNoConfigs,
                            My.Resources.MyStrings.importMsgApiNoConfigsTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    ''' <summary>
    ''' Performs an interactive file import. Type of import depends on current Plattform property
    ''' </summary>
    Public Sub DoImport(Optional ByRef FilesList() As String = Nothing)

        Dim OFD As New OpenFileDialog()
        Dim Content1 As String = "", Content2 As String, AllRows As New List(Of String())
        Dim FileNames As String() = {}
        Dim SubType As Integer = 0
        Dim NewImport As Boolean = False
        Dim ThisImport As IFileImport

        If _Plattform = PlatformManager.Platforms.Unknown Then
            ' Autodetect the import file format
            If FilesList IsNot Nothing OrElse MsgBoxEx.ShowWithNotAgainOption("ImportAutoDetect", DialogResult.OK,
                                               String.Format(My.Resources.MyStrings.importMsgAutodetectCSV, Application.ProductName),
                                               My.Resources.MyStrings.importMsgAutodetectCSVTitle,
                                               MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                With OFD
                    .Filter = My.Resources.MyStrings.importMsgAutodetectOpenFilter
                    .FilterIndex = 1
                    .Title = My.Resources.MyStrings.importMsgAutodetectOpenTitle
                    .Multiselect = True
                    .RestoreDirectory = True
                    If FilesList IsNot Nothing OrElse .ShowDialog() = DialogResult.OK Then
                        Try
                            Dim FirstLines() As String = {}
                            Dim FirstLine As String
                            If FilesList Is Nothing Then
                                FilesList = .FileNames
                            End If
                            For Each Filename As String In FilesList
                                Using reader As New StreamReader(Filename, Encoding.UTF8)
                                    If Not reader.EndOfStream Then
                                        FirstLine = reader.ReadLine
                                    Else
                                        FirstLine = ""
                                    End If
                                End Using
                                If FirstLine.Length > 0 Then
                                    ReDim Preserve FirstLines(FirstLines.Length)
                                    FirstLines(FirstLines.Length - 1) = FirstLine
                                End If
                            Next

                            If FirstLines.Length > 0 Then
                                _ImportFileHelper = New ImportFileHelper(FirstLines)
                                If _ImportFileHelper.MatchingPlatforms Is Nothing Then
                                    ' No matching platforms...
                                    MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgUnknownFileFormat,
                                                                  IIf(FilesList.Length > 1, "en", "")),
                                                    My.Resources.MyStrings.importMsgUnknownFileFormatTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Exit Sub
                                ElseIf _ImportFileHelper.MatchingPlatforms.Length = 1 Then
                                    ' it worked!
                                    If MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgStartPlatformImport, _ImportFileHelper.MatchingPlatforms(0).PlatformName),
                                                       My.Resources.MyStrings.importMsgStartPlatformImportTitle,
                                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                                        Exit Sub
                                    End If
                                    _Plattform = _ImportFileHelper.MatchingPlatforms(0).PlatformID
                                    SubType = _ImportFileHelper.MatchingPlatforms(0).SubType
                                    FileNames = FilesList

                                    ' Content setzen (all of this can be deleted after the corresponding imports have been refactored...)
                                    If _Plattform = PlatformManager.Platforms.Poloniex Then
                                        ' Besonderheit Poloniex: mehrere Dateien möglich
                                        Content1 = Join(FilesList, ",")
                                    Else
                                        Content1 = .FileName
                                    End If
                                Else
                                    ' More than one platform matching
                                    Dim Message As String = ""
                                    For i As Integer = 0 To UBound(_ImportFileHelper.MatchingPlatforms)
                                        Message &= String.Format("{0}. {1}{2}", i + 1, _ImportFileHelper.MatchingPlatforms(i).PlatformName, Environment.NewLine)
                                    Next
                                    MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgAmbiguosFileFormat, Environment.NewLine,
                                                                  Message, IIf(FilesList.Length > 1, "n", ""),
                                                                  Message, IIf(FilesList.Length > 1, "en", "")),
                                                                  My.Resources.MyStrings.importMsgAmbiguosFileFormatTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Exit Sub
                                End If
                            Else
                                Exit Sub
                            End If
                        Catch ex As Exception
                            DefaultErrorHandler(ex, My.Resources.MyStrings.importMsgReadFileError & ex.Message)
                            Exit Sub
                        End Try
                    Else
                        Exit Sub
                    End If
                End With
            Else
                Exit Sub
            End If
        Else
            _ImportFileHelper = New ImportFileHelper
        End If


        Select Case _Plattform
            Case PlatformManager.Platforms.MtGox
                ' Hinweis
                If MsgBoxEx.ShowWithNotAgainOption("ImportMtGox", DialogResult.OK,
                                                   "Mt. Gox erlaubt den Download von History-Dateien in den Formaten " &
                                   "BTC ('history_BTC.CSV'), EUR ('history_EUR') oder USD ('history_USD.csv'). " &
                                   "Bitte beachten Sie, dass der CoinTracer alle Bitcoin-Trades ausschließlich aus " &
                                   "der history_BTC.csv liest. Die EUR und USD-History-Dateien sollten aber zusätzlich " &
                                   "eingelesen werden, damit Gebühren und Einzahlungen oder Abhebungen korrekt " &
                                   "erfasst werden.", "Datenimport von Mt. Gox", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then

                    ' Datei einlesen
                    If Content1.Length = 0 Then
                        With OFD
                            .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                            .FilterIndex = 1
                            .Title = "Datenexport von Mt. Gox auswählen... (history_BTC/EUR/USD.csv)"
                            .RestoreDirectory = True
                            If .ShowDialog() = DialogResult.OK Then
                                Content1 = .FileName
                                Import_MtGox(Content1)
                            End If
                        End With
                    Else
                        Import_MtGox(Content1, False)
                    End If
                End If
            Case PlatformManager.Platforms.BitstampNet
                If Content1.Length = 0 Then
                    With OFD
                        .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                        .FilterIndex = 1
                        .Title = "Datenexport von Bitstamp.net auswählen... (Transactions*.csv)"
                        .RestoreDirectory = True

                        If .ShowDialog() = DialogResult.OK Then
                            Content1 = .FileName
                            Import_BitstampNet(Content1)
                        End If
                    End With
                Else
                    Import_BitstampNet(Content1, False)
                End If
            Case PlatformManager.Platforms.Vircurex
                ' Formular für Copy & Paste öffnen
                Dim DF As New frmGetImportData_CnP
                DF.ImportPlatform = PlatformManager.Platforms.Vircurex
                If DF.ShowDialog() = DialogResult.OK Then
                    Content1 = DF.ContentOrders.Trim
                    Content2 = DF.ContentAccounts.Trim
                    Dim TimeDiff As Integer = DF.TimeDiff
                    If Content2.Length > 0 Then
                        ' Jahreszahlen zum Vircurex-Import holen
                        Dim FD As New frmGetImportData_CnP_Vircurex
                        FD.InitContent(Content2)
                        If FD.ShowDialog() = DialogResult.OK Then
                            Content2 = FD.ImportContent
                            If Content1.Length > 0 OrElse Content2.Length > 0 Then
                                Import_Vircurex(Content1, Content2, TimeDiff)
                            End If
                        End If
                        FD.Dispose()
                    End If
                    DF.Dispose()
                End If
            Case PlatformManager.Platforms.BtcE
                ' Formular für Copy & Paste öffnen
                Dim DF As New frmGetImportData_CnP
                DF.ImportPlatform = PlatformManager.Platforms.BtcE
                If DF.ShowDialog() = DialogResult.OK Then
                    Content1 = DF.ContentOrders.Trim
                    DF.Dispose()
                    If Content1.Length > 0 Then
                        Import_BtcE(Content1)
                    End If
                End If
            Case PlatformManager.Platforms.WalletBTC
                If Content1.Length = 0 Then
                    With OFD
                        .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                        .FilterIndex = 1
                        .Title = "Datenexport aus dem persönlichen Bitcoin-Qt-Wallet auswählen... (*.csv)"
                        .RestoreDirectory = True
                        If .ShowDialog() = DialogResult.OK Then
                            Content1 = .FileName
                            Import_WalletQT(Content1)
                        End If
                    End With
                Else
                    Import_WalletQT(Content1, False)
                End If
            Case PlatformManager.Platforms.WalletLTC
                If Content1.Length = 0 Then
                    With OFD
                        .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                        .FilterIndex = 1
                        .Title = "Datenexport aus dem persönlichen Litecoin-Qt-Wallet auswählen... (*.csv)"
                        .RestoreDirectory = True
                        If .ShowDialog() = DialogResult.OK Then
                            Content1 = .FileName
                            Import_WalletQT(Content1)
                        End If
                    End With
                Else
                    Import_WalletQT(Content1, False)
                End If
            Case PlatformManager.Platforms.MultiBit
                If Content1.Length = 0 Then
                    With OFD
                        .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                        .FilterIndex = 1
                        .Title = "Datenexport aus dem MultiBit-Client auswählen... (multibit.csv)"
                        .RestoreDirectory = True
                        If .ShowDialog() = DialogResult.OK Then
                            Content1 = .FileName
                            Import_MultiBit(Content1)
                        End If
                    End With
                Else
                    Import_MultiBit(Content1, False)
                End If
            Case PlatformManager.Platforms.Poloniex
                If Content1.Length = 0 Then
                    If MsgBoxEx.ShowWithNotAgainOption("ImportPoloniexCom", DialogResult.OK,
                                                   "Sie können von Poloniex.com drei Arten von History-Daten importieren: Die Trade-History, die Withdrawal-History und die Deposit-History. " &
                                                   "Wählen Sie die gewünschten Dateien im nächsten Dialogfenster entweder mit gedrückter STRG-Taste alle gleichzeitig aus oder importieren Sie diese nacheinander einzeln." & Environment.NewLine & Environment.NewLine &
                                                   "Weitere Hinweise:" & Environment.NewLine &
                                                   "1. Zum Exportieren der Trade-History wählen Sie auf Poloniex.com oben rechts erst ""ORDERS"", dann ""MY TRADE HISTORY & ANALYSIS"". " &
                                                   "Anschließend klicken Sie auf ""Export: Complete Trade History""." & Environment.NewLine &
                                                   "2. Zum Exportieren der Deposit- oder Withdrawal-History wählen Sie oben rechts ""BALANCES"", dann ""HISTORY"". " &
                                                   "Anschließend klicken Sie auf ""Export Complete Deposit (oder Withdrawal) History""." & Environment.NewLine,
                                                   "Datenimport von Poloniex.com", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                        With OFD
                            .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                            .FilterIndex = 1
                            .Title = "Datenexport-Dateien von Poloniex.com auswählen... ([???]History.csv)"
                            .RestoreDirectory = True
                            .Multiselect = True
                            If .ShowDialog() = DialogResult.OK Then
                                Content1 = Join(.FileNames, "|")
                                Import_Poloniex(Content1)
                            End If
                        End With
                    End If
                Else
                    Import_Poloniex(Content1)
                End If
            Case PlatformManager.Platforms.Zyado
                If Content1.Length = 0 Then
                    With OFD
                        .Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*"
                        .FilterIndex = 1
                        .Title = "Datenexport von Zyado.com auswählen... (*.csv)"
                        .RestoreDirectory = True
                        If .ShowDialog() = DialogResult.OK Then
                            Content1 = .FileName
                            Import_Zyado(Content1)
                        End If
                    End With
                Else
                    Import_Zyado(Content1, False)
                End If
            Case Else
                ' Not an 'old' import! Mark as new (2018-04) type
                NewImport = True
        End Select

        If NewImport Then
            ' New kind of file import - refactored for better modularization

            Select Case _Plattform
                Case PlatformManager.Platforms.BitcoinDe
                    ' Bitcoin.de file import
                    ThisImport = New Import_BitcoinDe(Me)
                Case PlatformManager.Platforms.Bitfinex
                    ' Bitfinex file import
                    ThisImport = New Import_Bitfinex(Me)
                Case PlatformManager.Platforms.Kraken
                    ' Kraken file import
                    ThisImport = New Import_Kraken(Me)
                Case PlatformManager.Platforms.CoinTracer
                    ' Cointracer generic file import
                    ThisImport = New Import_CoinTracer(Me)
                Case Else
                    ThisImport = Nothing
            End Select

            If ThisImport IsNot Nothing Then
                If FileNames.Length > 0 Then
                    ThisImport.FileNames = FileNames
                End If
                If AllRows.Count > 0 Then
                    ThisImport.AllRows = AllRows
                End If
                ThisImport.SubType = SubType
                ThisImport.PerformImport()
            End If

        End If

        If _ZuletztEingelesen > 0 Or _ZuletztUeberprungen > 0 Then
            MsgBoxEx.BringToFront()
            MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgSummary,
                                          Environment.NewLine,
                                          _ZuletztEingelesen.ToString(MESSAGENUMBERFORMAT),
                                          _ZuletztUeberprungen.ToString(MESSAGENUMBERFORMAT),
                                          IIf(_LastTransfersUpdated > 0 OrElse _LastTransfersInserted > 0, Environment.NewLine & GetProcessedTransfersString(), "")),
                            My.Resources.MyStrings.importMsgSummaryTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    ''' <summary>
    ''' Importiert die Transaktionsdaten eines lokalen Wallets und schreibt diese in Trades. _Plattform muss entweder WalletBTC oder WalletLTC sein.
    ''' </summary>
    ''' <param name="Filename"></param>
    Private Sub Import_WalletQT(Filename As String,
                                Optional ByVal CheckFirstLine As Boolean = True)

        Dim Row() As String
        Dim LineCount As Integer
        Dim ErrCounter As Integer = _MaxErrors
        Dim IgnoreTransactionConfirmStatus As Boolean = False
        Dim AskedForIgnoreTransactionConfirmStatus As Boolean = False
        Dim Record As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim CSV As New CSVHelper(Filename, System.Text.Encoding.Default)

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                ' Datei hat offenbar falsches Format!
                Dim CoinName As String
                Select Case _Plattform
                    Case PlatformManager.Platforms.WalletLTC
                        CoinName = "Litecoin Core Client"
                    Case Else
                        CoinName = "Bitcoin Core Client"
                End Select
                MsgBoxEx.BringToFront()
                MessageBox.Show("Die Datei hat anscheinend nicht das richtige Format (" & CoinName & " muss in deutscher Sprache laufen!) und kann daher nicht eingelesen werden!",
                                "Ungültige Datei", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
            InitProgressForm("Starte Import der Wallet-Transaktionen. Bitte warten Sie...")
            If CSV.ReadAllRows(True, ",", """", ".", ",") > 0 Then
                ImportRecords = New List(Of dtoTradesRecord)
                Dim AllLines As Long = CSV.Rows.Count
                LineCount = 1
                For Each Row In CSV.Rows
                    ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", LineCount, AllLines))
                    LineCount += 1
                    Record = New dtoTradesRecord
                    With Record
                        Try
                            If Row(0) = "false" AndAlso Not AskedForIgnoreTransactionConfirmStatus Then
                                ' Erstes Vorkommen einer Zeile, die mit "false" beginnt
                                MsgBoxEx.PatchMsgBox(New String() {"Ja, Unbestätigte einlesen", "Nein, nur Bestätigte einlesen"})
                                IgnoreTransactionConfirmStatus = MessageBox.Show("Die Datei enthält unbestätigte Transaktionen. Sollen diese ebenfalls importiert werden " &
                                    "oder möchten Sie nur vollständig bestätigte Transaktionen einlesen?",
                                    "Unbestätigten Transaktionen entdeckt",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button1) = DialogResult.Yes
                                AskedForIgnoreTransactionConfirmStatus = True
                            End If
                            If Row(0) = "true" OrElse (Row(0) = "false" And IgnoreTransactionConfirmStatus) Then
                                .SourceID = MD5FromString(Row(6))
                                .Zeitpunkt = Row(1)
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = _Plattform
                                If _Plattform = PlatformManager.Platforms.WalletBTC Then
                                    .QuellKontoID = DBHelper.Konten.BTC
                                Else
                                    .QuellKontoID = DBHelper.Konten.LTC
                                End If
                                .ZielKontoID = .QuellKontoID
                                If Row(2).StartsWith("Empfangen") OrElse Row(2).StartsWith("Received") Then
                                    ' Einzahlung
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .ZielPlattformID = _Plattform
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = CSV.StringToDecimal(Row(5))
                                ElseIf Row(2).StartsWith("Überwiesen") OrElse Row(2).StartsWith("Sent to") Then
                                    ' Auszahlung
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .QuellPlattformID = _Plattform
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = -CSV.StringToDecimal(Row(5))
                                Else
                                    .DoNotImport = True
                                End If
                                .QuellBetrag = .ZielBetrag
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .Info = Row(3) & " / Adresse " & Row(4)

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                End If
                            End If

                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, "Fehler beim Einlesen der Wallet-Datei in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                    "Einlesen wird abgebrochen.", ""))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    End With

                Next Row

                Import_Records(ImportRecords, Filename, _ReadImportdataPercentage)
            Else
                DestroyProgressForm()
            End If

            Cursor.Current = Cursors.Default

        End If

    End Sub

    ''' <summary>
    ''' Importiert die Transaktionsdaten eines lokalen MultiBit-Clients und schreibt diese in Trades.
    ''' </summary>
    ''' <param name="Filename"></param>
    Private Sub Import_MultiBit(Filename As String,
                                Optional ByVal CheckFirstLine As Boolean = True)

        Dim Row() As String
        Dim LineCount As Long
        Dim ErrCounter As Long = _MaxErrors
        Dim Record As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim CSV As New CSVHelper(Filename, System.Text.Encoding.UTF8, False)
        Dim EnglishNotation As Boolean = False

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                ' Datei hat offenbar falsches Format!
                _ImportFileHelper.InvalidFileMessage(Filename)
                Exit Sub
            End If
            ' Deutsch / Englisch anhand des Subtyps unterscheiden
            If _ImportFileHelper.MatchingPlatforms(0).SubType = 0 Then
                ' deutsches Sprachformat
                EnglishNotation = False
            Else
                ' englisches Sprachformat
                EnglishNotation = True
            End If
            InitProgressForm("Starte Import der MultiBit-Wallet-Transaktionen. Bitte warten Sie...")
            If CSV.ReadAllRows(True, ",", """", If(EnglishNotation, ".", ","), If(EnglishNotation, ",", ".")) > 0 Then
                ImportRecords = New List(Of dtoTradesRecord)
                Dim AllLines As Long = CSV.Rows.Count
                LineCount = 1
                For Each Row In CSV.Rows
                    ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", LineCount, AllLines))
                    LineCount += 1
                    Record = New dtoTradesRecord
                    With Record
                        Try
                            If IsDate(Row(0)) Then
                                .SourceID = Row(4).Trim
                                .Zeitpunkt = Row(0)
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = PlatformManager.Platforms.MultiBit
                                .QuellKontoID = DBHelper.Konten.BTC
                                .ZielKontoID = .QuellKontoID
                                If CSV.StringToDecimal(Row(2)) > 0 Then
                                    ' Einzahlung
                                    .TradetypID = DBHelper.TradeTypen.Einzahlung
                                    .ZielPlattformID = _Plattform
                                    .QuellPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = CSV.StringToDecimal(Row(2))
                                ElseIf CSV.StringToDecimal(Row(2)) < 0 Then
                                    ' Auszahlung
                                    .TradetypID = DBHelper.TradeTypen.Auszahlung
                                    .QuellPlattformID = _Plattform
                                    .ZielPlattformID = PlatformManager.Platforms.Unknown
                                    .ZielBetrag = -CSV.StringToDecimal(Row(2))
                                Else
                                    .DoNotImport = True
                                End If
                                .QuellBetrag = .ZielBetrag
                                .QuellBetragNachGebuehr = .QuellBetrag
                                .BetragNachGebuehr = .ZielBetrag
                                .WertEUR = 0
                                .Info = Row(1) & If(EnglishNotation, " / Wert USD: ", " / Wert EUR: ") & Math.Abs(CSV.StringToDecimal(Row(3)))

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                End If
                            End If

                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, "Fehler beim Einlesen der MultiBit-Wallet-Datei in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                    "Einlesen wird abgebrochen.", ""))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    End With

                Next Row

                Import_Records(ImportRecords, Filename, _ReadImportdataPercentage)
            Else
                DestroyProgressForm()
            End If

            Cursor.Current = Cursors.Default

        End If

    End Sub

    ''' <summary>
    ''' Callback-Funktion für das Vorbereiten der CSV-Dateien für Bitstamp.net. Kurioserweise
    ''' gibt es diese mal mit, mal ohne Quotes bei Textfeldern...
    ''' </summary>
    Private Sub Import_BitstampNet_LinePreprocessor(ByRef Line As String)
        If Line.Trim.StartsWith("""") And Line.Trim.EndsWith("""") And Line.Trim.Length >= 2 Then
            Line = Line.Trim.Substring(1, Line.Trim.Length - 2)
            Line = Line.Replace("""""", """")
        End If
    End Sub

    ''' <summary>
    ''' Zugriff auf einzelne Informationen aus einer Bitstamp-Importzeile unter Berücksichtigung 
    ''' </summary>
    ''' <remarks></remarks>
    Private Class BitstampLineObject

        Private _ThisImport As Import
        Private _CSV As CSVHelper
        Private _AccStr As New AccountString
        Private _ExportType As Byte


        ''' <summary>
        ''' Typ der Export-Datei: 0 = altes Format, 1 = neueres Format inkl. Subtype, 2 = aktuelles Format
        ''' </summary>
        ''' <remarks></remarks>
        Public Property ExportType() As Byte
            Get
                Return _ExportType
            End Get
            Set(ByVal value As Byte)
                If value < 0 OrElse value > 2 Then
                    Throw New ApplicationException(String.Format("{0} ist keine gültige Angabe für eine Bistamp-Export-Versionsnummer", value))
                End If
                _ExportType = value
            End Set
        End Property

        Private _Row As String()
        ''' <summary>
        ''' Inhalt der aktuellen Zeile
        ''' </summary>
        ''' <value>String-Array</value>
        Public Property Row As String()
            Get
                Return _Row
            End Get
            Set(ByVal value As String())
                _Row = value
            End Set
        End Property

        Public ReadOnly Property Type() As String
            Get
                Select Case Row(0).Trim
                    Case "2"
                        Return "Market"
                    Case "1"
                        Return "Withdrawal"
                    Case "0"
                        Return "Deposit"
                    Case Else
                        Return Row(0).Trim
                End Select
            End Get
        End Property

        Public ReadOnly Property Datetime() As Date
            Get
                Return CDate(Row(1))
            End Get
        End Property

        Public ReadOnly Property Account() As String
            Get
                If ExportType = 2 Then
                    Return Row(2).Trim
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property Amount() As Decimal
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(3), 0)
                    If Item.Length > 0 Then
                        Return _CSV.StringToDecimal(Item)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _CSV.StringToDecimal(Row(2))
                End If
            End Get
        End Property

        Public ReadOnly Property AmountKontoRow() As KontenRow
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(3), 1)
                    If Item.Length > 0 Then
                        _AccStr.AccountString = String.Format("{0} ({0})", Item)
                        Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _ThisImport.RetrieveAccount("BTC", "BTC")
                End If
            End Get
        End Property

        Public ReadOnly Property Value() As Decimal
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(4), 0)
                    If Item.Length > 0 Then
                        If Item.Contains("0") OrElse Item.Contains("1") OrElse Item.Contains("2") OrElse Item.Contains("3") OrElse Item.Contains("4") OrElse Item.Contains("5") OrElse Item.Contains("6") OrElse Item.Contains("7") OrElse Item.Contains("8") OrElse Item.Contains("9") Then
                            Return _CSV.StringToDecimal(Item)
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                Else
                    Return _CSV.StringToDecimal(Row(3))
                End If
            End Get
        End Property

        Public ReadOnly Property ValueKontoRow() As KontenRow
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(4), 1)
                    If Item.Length > 0 Then
                        _AccStr.AccountString = String.Format("{0} ({0})", Item)
                        Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _ThisImport.RetrieveAccount("USD", "USD")
                End If
            End Get
        End Property

        Public ReadOnly Property Rate() As Decimal
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(5), 0)
                    If Item.Length > 0 Then
                        Return _CSV.StringToDecimal(Item)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _CSV.StringToDecimal(Row(4))
                End If
            End Get
        End Property

        Public ReadOnly Property RateKontoRow() As KontenRow
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(5), 1)
                    If Item.Length > 0 Then
                        _AccStr.AccountString = String.Format("{0} ({0})", Item)
                        Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _ThisImport.RetrieveAccount("BTC", "BTC")
                End If
            End Get
        End Property

        Public ReadOnly Property Fee() As Decimal
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(6), 0)
                    If Item.Length > 0 Then
                        Return _CSV.StringToDecimal(Item)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _CSV.StringToDecimal(Row(5))
                End If
            End Get
        End Property

        Public ReadOnly Property FeeKontoRow() As KontenRow
            Get
                If _ExportType = 2 Then
                    Dim Item As String = GetItems(Row(6), 1)
                    If Item.Length > 0 Then
                        _AccStr.AccountString = String.Format("{0} ({0})", Item)
                        Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
                    Else
                        Return Nothing
                    End If
                Else
                    Return _ThisImport.RetrieveAccount("USD", "USD")
                End If
            End Get
        End Property

        Public ReadOnly Property SubType() As String
            Get
                If _ExportType = 2 Then
                    Return Row(7).Trim
                ElseIf _ExportType = 1 Then
                    Return Row(6).Trim
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        ''' <summary>
        ''' Teilt einen String nach dem Muster '[Value] [Currency]' in ein String-Array auf.
        ''' </summary>
        ''' <param name="ItemIndex">0 oder 1 - Index des zurückzuliefernden Items</param>
        Private Function GetItems(ByRef ItemString As String,
                                  ByVal ItemIndex As Byte) As String
            If ItemString.Trim = String.Empty Then
                Return String.Empty
            Else
                Dim Result() = Split(ItemString, " ")
                If Result.Length >= ItemIndex Then
                    Return Result(ItemIndex)
                Else
                    Return String.Empty
                End If
            End If
        End Function

        Public Sub New(ByRef ThisImport As Import, ByRef CSV As CSVHelper)
            _CSV = CSV
            _ThisImport = ThisImport
        End Sub

    End Class

    Private Sub Import_BitstampNet(Filename As String,
                                   Optional ByVal CheckFirstLine As Boolean = True)

        Dim Row() As String
        Dim LineCount As Long
        Dim ErrCounter As Long = _MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim CSV As New CSVHelper(Filename, System.Text.Encoding.UTF8, False)
        Dim ImpLn As New BitstampLineObject(Me, CSV)
        CSV.SetCsvLinePreprocessor(AddressOf Import_BitstampNet_LinePreprocessor)

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                ' Datei hat offenbar falsches Format!
                _ImportFileHelper.InvalidFileMessage(Filename)
                Exit Sub
            End If
            ImpLn.ExportType = _ImportFileHelper.MatchingPlatforms(0).SubType
            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "Bitstamp.net"))
            If CSV.ReadAllRows(True, ",", """", "A", "A") > 0 Then
                ImportRecords = New List(Of dtoTradesRecord)
                LineCount = 1
                Dim AllLines As Long = CSV.Rows.Count
                For Each Row In CSV.Rows
                    ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", LineCount, AllLines))
                    LineCount += 1
                    If Row.Length >= 6 Then
                        ImpLn.Row = Row
                        Record = New dtoTradesRecord
                        RecordFee = Nothing
                        With Record
                            Try
                                .SourceID = MD5FromString(ImpLn.Type & ImpLn.Datetime & ImpLn.Account & ImpLn.Amount & ImpLn.Value & LineCount - 1)
                                .Zeitpunkt = ImpLn.Datetime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = PlatformManager.Platforms.BitstampNet
                                Select Case ImpLn.Type
                                    Case "Market"
                                        ' Kauf oder Verkauf
                                        If ImpLn.SubType = "buy" OrElse (ImpLn.SubType = String.Empty And ImpLn.Amount > 0) Then
                                            ' Kauf
                                            .TradetypID = DBHelper.TradeTypen.Kauf
                                            .QuellPlattformID = .ImportPlattformID
                                            .ZielPlattformID = .QuellPlattformID
                                            .ZielKontoID = ImpLn.AmountKontoRow.ID
                                            .ZielBetrag = ImpLn.Amount
                                            .BetragNachGebuehr = .ZielBetrag
                                            .QuellBetrag = Math.Abs(ImpLn.Value) + Math.Abs(ImpLn.Fee)
                                            .QuellBetragNachGebuehr = Math.Abs(ImpLn.Value)
                                            If ImpLn.ValueKontoRow.ID = CInt(DBHelper.Konten.EUR) Then
                                                .WertEUR = .QuellBetrag
                                            Else
                                                .WertEUR = 0
                                            End If
                                            .QuellKontoID = ImpLn.ValueKontoRow.ID
                                            .Info = String.Format("Kauf {0} - Volumen {1} {0} für {2} {3}",
                                                                  ImpLn.AmountKontoRow.Code,
                                                                  Math.Abs(ImpLn.Amount),
                                                                  Math.Abs(ImpLn.Value),
                                                                  ImpLn.ValueKontoRow.Code)
                                            ' Gebühren-Transaktion
                                            If .QuellBetrag > .QuellBetragNachGebuehr Then
                                                RecordFee = .Clone()
                                                RecordFee.SourceID = .SourceID & "/fee"
                                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                RecordFee.ZielKontoID = ImpLn.FeeKontoRow.GebuehrKontoID
                                                RecordFee.ZielBetrag = Math.Abs(ImpLn.Fee)
                                                RecordFee.WertEUR = 0
                                                RecordFee.BetragNachGebuehr = 0
                                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                RecordFee.QuellKontoID = ImpLn.FeeKontoRow.ID
                                                RecordFee.Info = String.Format("Gebühr zu {0}-Kauf Referenz {1}",
                                                                               ImpLn.AmountKontoRow.Code,
                                                                               .SourceID)
                                            End If
                                        Else
                                            ' Verkauf
                                            .TradetypID = DBHelper.TradeTypen.Verkauf
                                            .QuellPlattformID = .ImportPlattformID
                                            .ZielPlattformID = .QuellPlattformID
                                            .QuellKontoID = ImpLn.AmountKontoRow.ID
                                            .QuellBetrag = Math.Abs(ImpLn.Amount)
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .ZielBetrag = Math.Abs(ImpLn.Value)
                                            .BetragNachGebuehr = Math.Abs(ImpLn.Value) - Math.Abs(ImpLn.Fee)
                                            If ImpLn.ValueKontoRow.ID = CInt(DBHelper.Konten.EUR) Then
                                                .WertEUR = .BetragNachGebuehr
                                            Else
                                                .WertEUR = 0
                                            End If
                                            .ZielKontoID = ImpLn.ValueKontoRow.ID
                                            .Info = String.Format("Verkauf {0} - Volumen {1} {0} für {2} {3}",
                                                                  ImpLn.AmountKontoRow.Code,
                                                                  Math.Abs(ImpLn.Amount),
                                                                  Math.Abs(ImpLn.Value),
                                                                  ImpLn.ValueKontoRow.Code)
                                            ' Gebühren-Transaktion
                                            If .ZielBetrag > .BetragNachGebuehr Then
                                                RecordFee = .Clone()
                                                RecordFee.SourceID = .SourceID & "/fee"
                                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                RecordFee.ZielKontoID = ImpLn.FeeKontoRow.GebuehrKontoID
                                                RecordFee.ZielBetrag = Math.Abs(ImpLn.Fee)
                                                RecordFee.WertEUR = 0
                                                RecordFee.BetragNachGebuehr = 0
                                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                RecordFee.QuellKontoID = ImpLn.FeeKontoRow.ID
                                                RecordFee.Info = String.Format("Gebühr zu {0}-Kauf Referenz {1}",
                                                                               ImpLn.AmountKontoRow.Code,
                                                                               .SourceID)
                                            End If
                                        End If
                                    Case "Withdrawal"
                                        ' Auszahlung (Fiat oder Coins)
                                        .TradetypID = DBHelper.TradeTypen.Auszahlung
                                        .QuellPlattformID = .ImportPlattformID
                                        If ImpLn.Amount <> 0 Then
                                            .QuellKontoID = ImpLn.AmountKontoRow.ID
                                            .QuellBetrag = Math.Abs(ImpLn.Amount)
                                            .Info = String.Format("Auszahlung {0}", ImpLn.AmountKontoRow.Code)
                                        Else
                                            ' ...altes Format
                                            .QuellKontoID = ImpLn.ValueKontoRow.ID
                                            .QuellBetrag = Math.Abs(ImpLn.Value)
                                            .Info = String.Format("Auszahlung {0}", ImpLn.ValueKontoRow.Code)
                                        End If
                                        .ZielKontoID = .QuellKontoID
                                        .ZielBetrag = .QuellBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                        If .QuellKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .ZielBetrag
                                        Else
                                            .WertEUR = 0
                                        End If
                                        .ZielPlattformID = PlatformManager.Platforms.Unknown
                                        If ImpLn.Fee <> 0 Then
                                            ' Es gibt eine Gebühr!
                                            .QuellBetrag += Math.Abs(ImpLn.Fee)
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.ZielKontoID = RetrieveAccount(.QuellKontoID).GebuehrKontoID
                                            RecordFee.ZielBetrag = Math.Abs(ImpLn.Fee)
                                            RecordFee.WertEUR = 0
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.QuellKontoID = .QuellKontoID
                                            RecordFee.Info = String.Format("Gebühr zu Auszahlung Referenz {0}",
                                                                           .SourceID)
                                        End If
                                    Case "Deposit"
                                        ' Einzahlung (Fiat oder Coins)
                                        .TradetypID = DBHelper.TradeTypen.Einzahlung
                                        .QuellPlattformID = PlatformManager.Platforms.Unknown
                                        .ZielPlattformID = .ImportPlattformID
                                        If ImpLn.Amount <> 0 Then
                                            .ZielKontoID = ImpLn.AmountKontoRow.ID
                                            .ZielBetrag = Math.Abs(ImpLn.Amount)
                                            .Info = String.Format("Einzahlung {0}", ImpLn.AmountKontoRow.Code)
                                        Else
                                            ' ...altes Format
                                            .ZielKontoID = ImpLn.ValueKontoRow.ID
                                            .ZielBetrag = Math.Abs(ImpLn.Value)
                                            .Info = String.Format("Einzahlung {0}", ImpLn.ValueKontoRow.Code)
                                        End If
                                        .BetragNachGebuehr = .ZielBetrag
                                        If .ZielKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .ZielBetrag
                                        Else
                                            .WertEUR = 0
                                        End If
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = .ZielKontoID
                                    Case Else
                                        ' Ansonsten nicht importieren
                                        .DoNotImport = True
                                End Select

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                    If Not RecordFee Is Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If

                            Catch ex As Exception
                                Cursor.Current = Cursors.Default
                                ErrCounter -= 1
                                DefaultErrorHandler(ex, "Fehler beim Einlesen der Bitstamp.net-Datei in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                    IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                        "Einlesen wird abgebrochen.", ""))
                                If ErrCounter = 0 Then
                                    DestroyProgressForm()
                                    Exit Sub
                                End If
                            End Try

                        End With
                    End If

                Next Row

                Import_Records(ImportRecords, Filename, _ReadImportdataPercentage, , True)

            Else
                DestroyProgressForm()
            End If
            Cursor.Current = Cursors.Default

        End If

    End Sub

    ''' <summary>
    ''' Importiert Daten von Vircurex
    ''' </summary>
    ''' <param name="ContentOrders">String mit Order-History-Daten</param>
    ''' <param name="ContentAccounts">String mit Accounts-History-Daten</param>
    ''' <param name="TimeDiff">Zeituntschied zw. lokaler und Vircurex-Zeit in Stunden</param>
    Private Sub Import_Vircurex(ContentOrders As String,
                                ContentAccounts As String,
                                ByVal TimeDiff As Integer)

        Dim Lines() As String
        Dim Line As String
        Dim Items() As String, SubItems() As String
        Dim LineCount As Long
        Dim ErrCounter As Long = _MaxErrors
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim KontoRow As KontenRow
        Dim EnglishNotation As Integer = 0

        InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "Vircurex.com"))

        ImportRecords = New List(Of dtoTradesRecord)

        If ContentOrders.Length > 0 Then

            Lines = Split(ContentOrders, Environment.NewLine)
            LineCount = 0

            ' Schleife für alle Zeilen im "Orders"-Block (= nur Käufe und Verkäufe)
            For Each Line In Lines
                LineCount += 1
                ProgressWaitManager.UpdateProgress(LineCount / Lines.Length * _ReadImportdataPercentage / 2, "Lese Daten ein...")
                Items = Split(Line, vbTab)
                If DirectCast(Items, ICollection).Count = 9 Then
                    For i As Integer = 0 To Items.Length - 1
                        Items(i) = Items(i).Trim
                    Next
                    Record = New dtoTradesRecord
                    RecordFee = Nothing

                    ' Prüfen, welche Spracheinstellung der User hat
                    If EnglishNotation = 0 Then
                        Select Case Items(2)
                            Case "buy", "sell"
                                EnglishNotation = 1
                            Case "kaufe", "verkaufe"
                                EnglishNotation = 2
                        End Select
                    End If

                    With Record
                        Try
                            .SourceID = Items(0)
                            .Zeitpunkt = DateAdd(DateInterval.Hour, -TimeDiff, CDate(Items(1)))
                            .ZeitpunktZiel = .Zeitpunkt
                            .ImportPlattformID = PlatformManager.Platforms.Vircurex
                            .QuellPlattformID = PlatformManager.Platforms.Vircurex
                            .ZielPlattformID = .QuellPlattformID
                            Select Case Items(2)
                                Case "buy", "kaufe"
                                    ' Kauf
                                    .TradetypID = DBHelper.TradeTypen.Kauf
                                    .ZielKontoID = GetAccount(Items(4)).ID
                                    .ZielBetrag = StrToDec(Items(3), EnglishNotation = 1)
                                    .QuellBetrag = .ZielBetrag * StrToDec(Items(5), EnglishNotation = 1)
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .BetragNachGebuehr = .ZielBetrag - StrToDec(Items(7), EnglishNotation = 1)
                                    .QuellKontoID = GetAccount(Items(6)).ID
                                    If .QuellKontoID = DBHelper.Konten.EUR Then
                                        .WertEUR = StrToDec(Items(5), EnglishNotation = 1) * .BetragNachGebuehr
                                    End If
                                    .Info = Items(4) & " gekauft - Kurs " & Items(5) & " " & Items(6)
                                    ' Gebühren-Transaktion
                                    RecordFee = .Clone()
                                    RecordFee.SourceID = .SourceID & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.BetragNachGebuehr = 0
                                    KontoRow = GetAccount(Items(8))
                                    RecordFee.QuellKontoID = KontoRow.ID
                                    RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                    RecordFee.ZielBetrag = StrToDec(Items(7), EnglishNotation = 1)
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = "Gebühr zu Kauf Referenz " & .SourceID
                                Case "sell", "verkaufe"
                                    ' Verkauf
                                    .TradetypID = DBHelper.TradeTypen.Verkauf
                                    .QuellKontoID = GetAccount(Items(4)).ID
                                    .QuellBetrag = StrToDec(Items(3), EnglishNotation = 1)
                                    .QuellBetragNachGebuehr = .QuellBetrag
                                    .ZielBetrag = .QuellBetrag * StrToDec(Items(5), EnglishNotation = 1)
                                    .BetragNachGebuehr = .ZielBetrag - StrToDec(Items(7), EnglishNotation = 1)
                                    .ZielKontoID = GetAccount(Items(6)).ID
                                    If .ZielKontoID = DBHelper.Konten.EUR Then
                                        .WertEUR = .BetragNachGebuehr
                                    End If
                                    .Info = Items(4) & " verkauft - Kurs " & Items(5) & " " & Items(6)
                                    ' Gebühren-Transaktion
                                    RecordFee = .Clone()
                                    RecordFee.SourceID = .SourceID & "/fee"
                                    RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                    RecordFee.BetragNachGebuehr = 0
                                    KontoRow = GetAccount(Items(8))
                                    RecordFee.QuellKontoID = KontoRow.ID
                                    RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                    RecordFee.ZielBetrag = StrToDec(Items(7), EnglishNotation = 1)
                                    RecordFee.WertEUR = 0
                                    RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                    RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                    RecordFee.Info = "Gebühr zu Verkauf Referenz " & .SourceID
                                Case Else
                                    ' Ansonsten nicht importieren
                                    .DoNotImport = True
                            End Select

                            If Not .DoNotImport Then
                                ' Record der Liste hinzufügen
                                ImportRecords.Add(Record)
                                If Not RecordFee Is Nothing Then
                                    ImportRecords.Add(RecordFee)
                                End If
                            End If

                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, "Fehler beim Einlesen der Orders in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                    "Einlesen wird abgebrochen.", ""))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    End With

                End If

            Next

            If LineCount > 0 And ImportRecords.Count = 0 Then
                ' Fehler: offenbar nur ungültige Zeilen in Orders-Bereich...
                Cursor.Current = Cursors.Default
                DefaultErrorHandler(New Exception, "Fehler beim Einlesen der Orders: Alle Zeilen hatten ein ungültiges Format." & Environment.NewLine & Environment.NewLine &
                                    "Einlesen wird abgebrochen.")
                DestroyProgressForm()
                Exit Sub
            End If

            If ContentAccounts.Length = 0 Then
                ' hier nur importieren, wenn nicht unten bei Accounts...
                Import_Records(ImportRecords, "", _ReadImportdataPercentage)
                Cursor.Current = Cursors.Default

            End If

        End If

        If ContentAccounts.Length > 0 Then

            Lines = Split(ContentAccounts, Environment.NewLine)
            LineCount = 0

            ' Schleife für alle Zeilen im "Accounts"-Block (= nur Ein- und Auszahlungen)
            For Each Line In Lines
                LineCount += 1
                ProgressWaitManager.UpdateProgress(LineCount / Lines.Length * _ReadImportdataPercentage / 2 + (_ReadImportdataPercentage / 2), "Lese Daten ein...")
                Items = Split(Line, vbTab)
                If DirectCast(Items, ICollection).Count = 3 Or DirectCast(Items, ICollection).Count = 4 Then
                    For i As Integer = 0 To Items.Length - 1
                        Items(i) = Items(i).Trim
                    Next
                    ' Sonderfall: Im Datum von Vircurex fehlt die Jahreszahl... X-( - Versuch, dieses intelligent zu ergänzen
                    SubItems = Split(Items(0), " ")
                    If IsDate(Items(0).Replace("Mär", "März")) OrElse DirectCast(SubItems, ICollection).Count = 3 Then
                        If IsDate(Items(0).Replace("Mär", "März")) OrElse IsDate(SubItems(0) & " " & SubItems(1).Replace("Mär", "März")) Then
                            If Not IsDate(Items(0).Replace("Mär", "März")) OrElse DirectCast(SubItems, ICollection).Count = 3 Then
                                If CDate(SubItems(0) & " " & SubItems(1).Replace("Mär", "März")) > Today Then
                                    Items(0) = CDate(SubItems(0) & " " & SubItems(1).Replace("Mär", "März") & " " & Year(Today) - 1 & " " & SubItems(2))
                                Else
                                    Items(0) = CDate(SubItems(0) & " " & SubItems(1).Replace("Mär", "März") & " " & Year(Today) & " " & SubItems(2))
                                End If
                            End If

                            ' Prüfen, welche Spracheinstellung der User hat
                            If EnglishNotation = 0 Then
                                If Items(1).Contains(".") AndAlso (Items(1).Contains(",") = False OrElse Items(1).IndexOf(",") < Items(1).IndexOf(".")) Then
                                    EnglishNotation = 1
                                Else
                                    EnglishNotation = 2
                                End If
                            End If

                            Record = New dtoTradesRecord
                            RecordFee = Nothing

                            With Record
                                Try
                                    .SourceID = MD5FromString(Items(0) & Items(1) & Items(2))
                                    .Zeitpunkt = DateAdd(DateInterval.Hour, -TimeDiff, CDate(Items(0)))
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = PlatformManager.Platforms.Vircurex
                                    ' EnglishNotation = Items(1).Contains(".")
                                    If StrToDec(Items(1), EnglishNotation = 1) < 0 _
                                        OrElse (DirectCast(Items, ICollection).Count > 3 AndAlso Items(3).Contains("Move to reserved funds")) Then
                                        ' Offenbar eine Auszahlung
                                        .TradetypID = DBHelper.TradeTypen.Auszahlung
                                        .QuellBetrag = Math.Abs(StrToDec(Items(1), EnglishNotation = 1))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .ZielBetrag = .QuellBetrag
                                        .BetragNachGebuehr = .QuellBetrag   ' Bei Auszahlungen steht der Betrag, der am Ziel ankommt, in BetragNachGebuehr!
                                        .QuellPlattformID = PlatformManager.Platforms.Vircurex
                                        .ZielPlattformID = PlatformManager.Platforms.Unknown
                                        .QuellKontoID = GetAccount(Items(2)).ID
                                        .ZielKontoID = .QuellKontoID
                                    Else
                                        ' Offenbar eine Einzahlung
                                        .TradetypID = DBHelper.TradeTypen.Einzahlung
                                        .ZielBetrag = StrToDec(Items(1), EnglishNotation = 1)
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                        .QuellPlattformID = PlatformManager.Platforms.Unknown
                                        .ZielPlattformID = PlatformManager.Platforms.Vircurex
                                        .ZielKontoID = GetAccount(Items(2)).ID
                                        .QuellKontoID = .ZielKontoID
                                    End If

                                    If DirectCast(Items, ICollection).Count > 3 Then
                                        .Info = Items(3).Trim
                                    End If

                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)

                                Catch ex As Exception
                                    Cursor.Current = Cursors.Default
                                    ErrCounter -= 1
                                    DefaultErrorHandler(ex, "Fehler beim Einlesen der Konten/Accounts in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                        IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                            "Einlesen wird abgebrochen.", ""))
                                    If ErrCounter = 0 Then
                                        DestroyProgressForm()
                                        Exit Sub
                                    End If
                                End Try

                            End With

                        End If
                    End If

                End If

            Next

            If LineCount > 0 And ImportRecords.Count = 0 And ContentOrders.Length = 0 Then
                ' Fehler: bei Orders nix und offenbar nur ungültige Zeilen in Accounts-Bereich...
                Cursor.Current = Cursors.Default
                DefaultErrorHandler(New Exception, "Fehler beim Einlesen der Konten/Accounts: Alle Zeilen hatten ein ungültiges Format." & Environment.NewLine & Environment.NewLine &
                                    "Einlesen wird abgebrochen.")
                DestroyProgressForm()
                Exit Sub
            End If

            Import_Records(ImportRecords, "", _ReadImportdataPercentage)

            Cursor.Current = Cursors.Default

        End If

    End Sub


    ''' <summary>
    ''' Importiert Daten von BTC-E.com
    ''' </summary>
    ''' <param name="ContentHistory">String mit History-Daten</param>
    Private Sub Import_BtcE(ContentHistory As String)

        Dim Lines() As String
        Dim Line As String
        Dim l As Long
        Dim ErrCounter As Long = _MaxErrors
        Dim Items() As String, SubItems() As String
        Dim LineCount As Long
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim KontoRow As KontenRow

        If ContentHistory.Length > 0 Then

            Cursor.Current = Cursors.WaitCursor

            Lines = Split(ContentHistory, Environment.NewLine)
            LineCount = 0

            ImportRecords = New List(Of dtoTradesRecord)

            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "BTC-E.com"))

            ' Schleife für alle Zeilen
            For l = 0 To DirectCast(Lines, ICollection).Count - 1
                ProgressWaitManager.UpdateProgress(l / Lines.Length * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", l, Lines.Length))
                Line = Lines(l)
                If Line.Length >= 5 AndAlso Left(Line, 1) = "#" AndAlso IsNumeric(Line.Substring(2, 4)) Then
                    ' Uhrzeit aus nächster Zeile dazuholen
                    LineCount += 1
                    Line &= " " & Lines(l + 1)
                    l += 1

                    Items = Split(Line, vbTab)
                    If DirectCast(Items, ICollection).Count = 5 Then
                        ' Comment-Feld zerlegen
                        SubItems = Split(Trim(Items(1)) & " " & Trim(Items(2)), " ")

                        Try
                            If DirectCast(SubItems, ICollection).Count >= 4 Then

                                Record = New dtoTradesRecord
                                RecordFee = Nothing
                                With Record
                                    .SourceID = Items(0)
                                    .Zeitpunkt = DateAdd(DateInterval.Hour, -3, CDate(Items(3))).ToLocalTime
                                    .ZeitpunktZiel = .Zeitpunkt
                                    .ImportPlattformID = PlatformManager.Platforms.BtcE
                                    .Info = Items(2)
                                    If SubItems(DirectCast(SubItems, ICollection).Count - 1) = "payment" Then
                                        ' Einzahlung
                                        .TradetypID = DBHelper.TradeTypen.Einzahlung
                                        .QuellPlattformID = PlatformManager.Platforms.Unknown
                                        .ZielPlattformID = PlatformManager.Platforms.BtcE
                                        .ZielBetrag = StrToDec(SubItems(0))
                                        .ZielKontoID = GetAccount(SubItems(2)).ID
                                        .QuellKontoID = .ZielKontoID
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                    ElseIf SubItems(2) = "Withdrawal" Then
                                        ' Auszahlung
                                        .TradetypID = DBHelper.TradeTypen.Auszahlung
                                        .QuellPlattformID = PlatformManager.Platforms.BtcE
                                        .ZielPlattformID = PlatformManager.Platforms.Unknown
                                        .QuellBetrag = Math.Abs(StrToDec(SubItems(0)))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .ZielBetrag = .QuellBetrag
                                        .BetragNachGebuehr = .QuellBetrag   ' Bei Auszahlungen steht der Betrag, der am Ziel ankommt, in BetragNachGebuehr!
                                        .QuellKontoID = GetAccount(SubItems(3)).ID
                                        .ZielKontoID = .QuellKontoID
                                    ElseIf SubItems(2) = "Buy" Then
                                        ' Kauf (Coins)
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                        .QuellPlattformID = PlatformManager.Platforms.BtcE
                                        .ZielPlattformID = .QuellPlattformID
                                        KontoRow = GetAccount(SubItems(4))
                                        .ZielKontoID = KontoRow.ID
                                        .ZielBetrag = StrToDec(SubItems(3))
                                        If Left(SubItems(0), 1) = "-" Then
                                            ' negative Zahl in 1. Spalte -> also aufgewendete Währung
                                            .BetragNachGebuehr = Math.Round(.ZielBetrag * (1 + StrToDec(SubItems(5).Replace("(", "").Replace(")", "").Replace("%", "")) / 100), 8, MidpointRounding.AwayFromZero)
                                        Else
                                            ' positive Zahl in 1. Spalte -> also erhaltene Währung
                                            .BetragNachGebuehr = StrToDec(SubItems(0))
                                        End If
                                        .QuellBetrag = Math.Round(StrToDec(SubItems(DirectCast(SubItems, ICollection).Count - 2)) * .ZielBetrag, 8, MidpointRounding.AwayFromZero)
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = GetAccount(SubItems(DirectCast(SubItems, ICollection).Count - 1)).ID
                                        If .QuellKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .QuellBetrag
                                        End If
                                        ' Gebühren-Transaktion
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = .ZielKontoID
                                        RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                        RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = "Gebühr zu Kauf Referenz " & .SourceID
                                    ElseIf SubItems(2) = "Sell" Then
                                        ' Verkauf (Coins)
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                        .QuellPlattformID = PlatformManager.Platforms.BtcE
                                        .ZielPlattformID = .QuellPlattformID
                                        .QuellKontoID = GetAccount(SubItems(4)).ID
                                        .QuellBetrag = StrToDec(SubItems(3))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = StrToDec(SubItems(0))
                                        .ZielBetrag = StrToDec(SubItems(13))
                                        KontoRow = GetAccount(SubItems(1))
                                        .ZielKontoID = KontoRow.ID
                                        If .ZielKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .BetragNachGebuehr
                                        End If
                                        ' Gebühren-Transaktion
                                        RecordFee = .Clone()
                                        RecordFee.SourceID = .SourceID & "/fee"
                                        RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                        RecordFee.BetragNachGebuehr = 0
                                        RecordFee.QuellKontoID = .ZielKontoID
                                        RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                        RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                        RecordFee.WertEUR = 0
                                        RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                        RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                        RecordFee.Info = "Gebühr zu Verkauf Referenz " & .SourceID
                                    ElseIf SubItems(2) = "Bought" Or SubItems(2) = "Куплено" Then
                                        ' Entweder Verkauf oder Kauf Coins gegen Coins
                                        ' TODO: Prüfung auf Fiat "sauber", d.h. anhand des Flags in der Kontotabelle!
                                        If SubItems(1) = "EUR" Or SubItems(1) = "USD" Then
                                            ' Verkauf
                                            .TradetypID = DBHelper.TradeTypen.Verkauf
                                            .QuellPlattformID = PlatformManager.Platforms.BtcE
                                            .ZielPlattformID = .QuellPlattformID
                                            .QuellBetrag = StrToDec(SubItems(3))
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .QuellKontoID = GetAccount(SubItems(4)).ID
                                            .ZielBetrag = StrToDec(SubItems(14))
                                            .BetragNachGebuehr = StrToDec(SubItems(0))
                                            KontoRow = GetAccount(SubItems(1))
                                            .ZielKontoID = KontoRow.ID
                                            If .ZielKontoID = DBHelper.Konten.EUR Then
                                                .WertEUR = .BetragNachGebuehr
                                            End If
                                            ' Gebühren-Transaktion
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellKontoID = .ZielKontoID
                                            RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                            RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                            RecordFee.WertEUR = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.Info = "Gebühr zu Verkauf Referenz " & .SourceID
                                        Else
                                            ' Kauf (Coins für Coins)
                                            .TradetypID = DBHelper.TradeTypen.Kauf
                                            .QuellPlattformID = PlatformManager.Platforms.BtcE
                                            .ZielPlattformID = .QuellPlattformID
                                            KontoRow = GetAccount(SubItems(1))
                                            .ZielKontoID = KontoRow.ID
                                            .ZielBetrag = StrToDec(SubItems(14))
                                            .BetragNachGebuehr = StrToDec(SubItems(0))
                                            .QuellBetrag = StrToDec(SubItems(3))
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .QuellKontoID = GetAccount(SubItems(4)).ID
                                            ' Gebühren-Transaktion
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellKontoID = .ZielKontoID
                                            RecordFee.ZielKontoID = KontoRow.GebuehrKontoID
                                            RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                            RecordFee.WertEUR = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.Info = "Gebühr zu Kauf Referenz " & .SourceID
                                        End If
                                    Else
                                        .DoNotImport = True
                                    End If

                                    If Not .DoNotImport Then
                                        ' Record der Liste hinzufügen
                                        ImportRecords.Add(Record)
                                        If Not RecordFee Is Nothing Then
                                            ImportRecords.Add(RecordFee)
                                        End If
                                    End If
                                End With
                            End If

                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, "Fehler beim Einlesen der BTC-E-History in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                    "Einlesen wird abgebrochen.", ""))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    End If
                End If
            Next l

            Import_Records(ImportRecords, "", _ReadImportdataPercentage)
            Cursor.Current = Cursors.Default

        End If

    End Sub

    ''' <summary>
    ''' Represents a single row of a Poloniex trade history file 
    ''' </summary>
    Private Class PoloniexTradeLineObject

        Private _AccStr As New AccountString
        Private _ThisImport As Import

        Private _Date As DateTime
        Public ReadOnly Property DateTime() As DateTime
            Get
                Return _Date
            End Get
        End Property

        Private _Market As String
        Public ReadOnly Property Market() As String
            Get
                Return _Market
            End Get
        End Property

        Private _Category As String
        Public ReadOnly Property Category() As String
            Get
                Return _Category
            End Get
        End Property

        Private _Type As String
        Public ReadOnly Property Type() As String
            Get
                Return _Type
            End Get
        End Property

        Private _Price As Decimal
        Public ReadOnly Property Price() As Decimal
            Get
                Return _Price
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Total As Decimal
        Public ReadOnly Property Total() As Decimal
            Get
                Return _Total
            End Get
        End Property

        Private _Fee As Decimal
        Public ReadOnly Property Fee() As Decimal
            Get
                Return _Fee
            End Get
        End Property

        Private _OrderNumber As String
        Public ReadOnly Property OrderNumber() As String
            Get
                Return _OrderNumber
            End Get
        End Property

        Private _BaseTotalLessFee As Decimal
        Public ReadOnly Property BaseTotalLessFee() As Decimal
            Get
                Return _BaseTotalLessFee
            End Get
        End Property

        Private _QuoteTotalLessFee As Decimal
        Public ReadOnly Property QuoteTotalLessFee() As Decimal
            Get
                Return _QuoteTotalLessFee
            End Get
        End Property

        Private _AccountString1st As String
        Public ReadOnly Property AccountString1st() As String
            Get
                Return _AccountString1st
            End Get
        End Property

        Public ReadOnly Property Account1st() As KontenRow
            Get
                _AccStr.AccountString = String.Format("{0} ({0})", _AccountString1st)
                Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
            End Get
        End Property

        Private _AccountString2nd As String
        Public ReadOnly Property AccountString2nd() As String
            Get
                Return _AccountString2nd
            End Get
        End Property

        Public ReadOnly Property Account2nd() As KontenRow
            Get
                _AccStr.AccountString = String.Format("{0} ({0})", _AccountString2nd)
                Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
            End Get
        End Property

        Public Sub New(ByRef ThisImport As Import, ByRef ImportRow() As String, Optional ByRef SubType As Integer = 0)
            Dim ColOffset As Byte
            If SubType = 0 Then
                ColOffset = 1
            Else
                ColOffset = 0
            End If
            _ThisImport = ThisImport
            _Date = CDate(ImportRow(0)).ToLocalTime
            _Market = ImportRow(1).Trim.Replace("USDT", "USD")
            If SubType = 0 Then
                _Category = ImportRow(2).Trim
            Else
                _Category = "Exchange"
            End If
            _Type = ImportRow(2 + ColOffset).Trim
            _Price = Math.Abs(StrToDec(ImportRow(3 + ColOffset)))
            _Amount = Math.Abs(StrToDec(ImportRow(4 + ColOffset)))
            _Total = Math.Abs(StrToDec(ImportRow(5 + ColOffset)))
            _Fee = Math.Abs(StrToDec(ImportRow(6 + ColOffset).Replace("%", "")))
            If SubType = 0 Then
                _OrderNumber = ImportRow(8).Trim
            Else
                _OrderNumber = ""
            End If
            _BaseTotalLessFee = Math.Abs(StrToDec(ImportRow(7 + ColOffset * 2)))
            _QuoteTotalLessFee = Math.Abs(StrToDec(ImportRow(8 + ColOffset * 2)))
            Dim Currencies() As String = _Market.Split("/")
            If Currencies.Length = 2 Then
                _AccountString1st = Currencies(0)
                _AccountString2nd = Currencies(1)
            Else
                _AccountString1st = ""
                _AccountString2nd = ""
            End If
        End Sub

    End Class

    ''' <summary>
    ''' Represents a single row of a Poloniex deposit or withdrawal history file 
    ''' </summary>
    Private Class PoloniexDepositWithdrawalLineObject

        Private _AccStr As New AccountString
        Private _ThisImport As Import

        Private _Date As DateTime
        Public ReadOnly Property DateTime() As DateTime
            Get
                Return _Date
            End Get
        End Property

        Private _CurrencyString As String
        Public ReadOnly Property CurrencyString() As String
            Get
                Return _CurrencyString
            End Get
        End Property

        Private _Address As String
        Public ReadOnly Property Address() As String
            Get
                Return _Address
            End Get
        End Property

        Private _Amount As Decimal
        Public ReadOnly Property Amount() As Decimal
            Get
                Return _Amount
            End Get
        End Property

        Private _Status As String
        Public ReadOnly Property Status() As String
            Get
                Return _Status
            End Get
        End Property

        Public ReadOnly Property CurrencyAccount() As KontenRow
            Get
                _AccStr.AccountString = String.Format("{0} ({0})", _CurrencyString)
                Return _ThisImport.RetrieveAccount(_AccStr.AccountLongname, _AccStr.AccountCode)
            End Get
        End Property

        Public Sub New(ByRef ThisImport As Import, ByRef ImportRow() As String)
            _ThisImport = ThisImport
            _Date = CDate(ImportRow(0)).ToLocalTime
            _CurrencyString = ImportRow(1).Trim.Replace("USDT", "USD")
            _Amount = Math.Abs(StrToDec(ImportRow(2)))
            _Address = ImportRow(3).Trim
            _Status = ImportRow(4).Trim
        End Sub

    End Class


    ''' <summary>
    ''' Importiert Daten von Poloniex.com
    ''' </summary>
    ''' <param name="FileNames">Einzulesende Dateinamen, kommasepariert</param>
    Private Sub Import_Poloniex(ByRef FileNames As String)

        Const INFONUMBERFORMAT As String = "#,###,##0.########"
        Dim LineCount As Long
        Dim AllLines As Long
        Dim ColOffset As Byte = 1
        Dim ErrCounter As Long = _MaxErrors
        Dim Files() As String, Row() As String
        Dim TradeType As DBHelper.TradeTypen
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim QuellKontoRow As KontenRow, ZielKontoRow As KontenRow
        Dim Infotext As String
        Dim AccStr As New AccountString
        Dim TLO As PoloniexTradeLineObject
        Dim DWLO As PoloniexDepositWithdrawalLineObject

        Files = Split(FileNames, "|")
        For Each Filename As String In Files

            Dim CSV As New CSVHelper(Filename, True)

            If CSV.FileExists Then
                Cursor.Current = Cursors.WaitCursor
                If _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                    ' Datei hat offenbar falsches Format!
                    _ImportFileHelper.InvalidFileMessage(Filename)
                    Exit For
                End If
                ' Für Trade-History-Importe: altes oder neues Format?
                If _ImportFileHelper.MatchingPlatforms(0).SubType = 1 Then
                    ColOffset = 0
                End If
                InitProgressForm(String.Format("Starte Import der Datei '{0}' von Poloniex.com. Bitte warten Sie...",
                                                  Path.GetFileName(Filename)))
                ' Daten in Liste einlesen
                If CSV.ReadAllRows(True, ",", "", ".", ",") > 0 Then
                    ImportRecords = New List(Of dtoTradesRecord)
                    LineCount = 1
                    AllLines = CSV.Rows.Count
                    If Filename.Contains("deposit") Then
                        TradeType = DBHelper.TradeTypen.Einzahlung
                    ElseIf Filename.Contains("withdrawal") Then
                        TradeType = DBHelper.TradeTypen.Auszahlung
                    Else
                        TradeType = DBHelper.TradeTypen.Undefiniert
                    End If
                    For l = 0 To AllLines - 1
                        Try
                            Row = CSV.Rows(l)
                            ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei '{2}' ein... ({0}/{1})", l, AllLines, Path.GetFileName(Filename)))
                            LineCount += 1
                            Select Case TradeType
                                Case DBHelper.TradeTypen.Auszahlung, DBHelper.TradeTypen.Einzahlung
                                    ' Deposit- oder Withdrawal-History
                                    If Row.Length = 5 AndAlso Row(4).ToUpper.StartsWith("COMPLETE") Then
                                        Record = New dtoTradesRecord
                                        DWLO = New PoloniexDepositWithdrawalLineObject(Me, Row)
                                        With Record
                                            .SourceID = MD5FromString(DWLO.DateTime.ToString("yyyy-MM-dd HH:mm:ss") & DWLO.CurrencyString & DWLO.Amount.ToString & DWLO.Address & LineCount - 1)
                                            .Zeitpunkt = DWLO.DateTime
                                            .ZeitpunktZiel = .Zeitpunkt
                                            .ImportPlattformID = PlatformManager.Platforms.Poloniex
                                            .TradetypID = CInt(TradeType)
                                            QuellKontoRow = DWLO.CurrencyAccount
                                            If TradeType = DBHelper.TradeTypen.Auszahlung Then
                                                .QuellPlattformID = PlatformManager.Platforms.Poloniex
                                                .ZielPlattformID = PlatformManager.Platforms.Unknown
                                                .Info = String.Format("Auszahlung auf Adresse {0}", DWLO.Address)
                                            Else
                                                .QuellPlattformID = PlatformManager.Platforms.Unknown
                                                .ZielPlattformID = PlatformManager.Platforms.Poloniex
                                                .Info = String.Format("Einzahlung von Adresse {0}", DWLO.Address)
                                            End If
                                            .QuellBetrag = DWLO.Amount
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .ZielBetrag = .QuellBetrag
                                            .BetragNachGebuehr = .ZielBetrag
                                            .QuellKontoID = QuellKontoRow.ID
                                            .ZielKontoID = .QuellKontoID
                                            ' Record der Liste hinzufügen
                                            ImportRecords.Add(Record)
                                        End With
                                    End If
                                Case Else
                                    ' Trade-History
                                    If Row.Length >= 9 + ColOffset Then
                                        Record = New dtoTradesRecord
                                        TLO = New PoloniexTradeLineObject(Me, Row, _ImportFileHelper.MatchingPlatforms(0).SubType)
                                        With Record
                                            ' .SourceID = MD5FromString(Row(0) & Row(1) & Row(2 + ColOffset) & Row(3 + ColOffset) & Row(4 + ColOffset) & Row(5 + ColOffset) & LineCount - 1)
                                            .SourceID = MD5FromString(TLO.DateTime.ToString("yyyy-MM-dd HH:mm:ss") & TLO.Market & TLO.Type & TLO.Price.ToString & TLO.Amount.ToString & LineCount - 1)
                                            .Zeitpunkt = TLO.DateTime
                                            .ZeitpunktZiel = .Zeitpunkt
                                            .ImportPlattformID = PlatformManager.Platforms.Poloniex
                                            .QuellPlattformID = .ImportPlattformID
                                            .ZielPlattformID = .ImportPlattformID
                                            QuellKontoRow = TLO.Account1st
                                            ZielKontoRow = TLO.Account2nd
                                            If ZielKontoRow.IstFiat AndAlso TLO.Type = "Sell" Then
                                                .TradetypID = DBHelper.TradeTypen.Verkauf
                                                Infotext = "Verkauf von {0} {1} / Kurs {2} {3} / {4}% Gebühr"
                                                .ZielKontoID = ZielKontoRow.ID
                                                .QuellKontoID = QuellKontoRow.ID
                                                .ZielBetrag = TLO.Total
                                                .BetragNachGebuehr = TLO.BaseTotalLessFee
                                                .QuellBetrag = TLO.Amount
                                                .QuellBetragNachGebuehr = .QuellBetrag
                                            Else
                                                .TradetypID = DBHelper.TradeTypen.Kauf
                                                If TLO.Type = "Sell" Then
                                                    ' Swap of coins vs. coins is always regarded as a buy!
                                                    Infotext = "Kauf von {5} {3} / Preis {0} {1} / {4}% Gebühr"
                                                    .QuellKontoID = QuellKontoRow.ID
                                                    .ZielKontoID = ZielKontoRow.ID
                                                    .ZielBetrag = TLO.Total
                                                    .BetragNachGebuehr = TLO.BaseTotalLessFee
                                                    .QuellBetrag = TLO.Amount
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                Else
                                                    Infotext = "Kauf von {0} {1} / Kurs {2} {3} / {4}% Gebühr"
                                                    .ZielKontoID = QuellKontoRow.ID
                                                    .QuellKontoID = ZielKontoRow.ID
                                                    .ZielBetrag = TLO.Amount
                                                    .BetragNachGebuehr = TLO.QuoteTotalLessFee
                                                    .QuellBetrag = TLO.Total
                                                    .QuellBetragNachGebuehr = .QuellBetrag
                                                End If

                                            End If
                                            .Info = String.Format(Infotext,
                                                                  TLO.Amount.ToString(INFONUMBERFORMAT), TLO.AccountString1st,
                                                                  TLO.Price.ToString(INFONUMBERFORMAT), TLO.AccountString2nd,
                                                                  TLO.Fee.ToString(INFONUMBERFORMAT), TLO.Total.ToString(INFONUMBERFORMAT))
                                            If TLO.OrderNumber.Length > 0 Then
                                                .Info &= String.Format(" / Order {0}", TLO.OrderNumber)
                                            End If
                                            ' Record der Liste hinzufügen
                                            ImportRecords.Add(Record)
                                            ' Gebühren-Eintrag
                                            If .BetragNachGebuehr < .ZielBetrag Then
                                                RecordFee = .Clone()
                                                RecordFee.SourceID = .SourceID & "/fee"
                                                RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                                RecordFee.BetragNachGebuehr = 0
                                                RecordFee.QuellKontoID = .ZielKontoID
                                                RecordFee.ZielBetrag = .ZielBetrag - .BetragNachGebuehr
                                                RecordFee.ZielKontoID = GetAccount(RecordFee.QuellKontoID).GebuehrKontoID
                                                RecordFee.WertEUR = 0
                                                RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                                RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                                RecordFee.Info = "Gebühr zu Trade Referenz " & .SourceID
                                                ImportRecords.Add(RecordFee)
                                            End If
                                        End With
                                    End If
                            End Select


                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, String.Format("Fehler beim Einlesen der Datei '{0}' in Zeile {1}:{2}{3}{4}",
                                                                  Path.GetFileName(Filename),
                                                                  l + 1,
                                                                  Environment.NewLine,
                                                                  ex.Message,
                                                                  IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                                      "Einlesen wird abgebrochen.", "")))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    Next l

                    Import_Records(ImportRecords, Filename, _ReadImportdataPercentage, , True, , , (Files.Length = 1))
                    Cursor.Current = Cursors.Default

                End If
            End If

        Next

    End Sub

    Private Enum MtGoxHistoryType
        HistoryBTC = 1
        HistoryEUR = 2
        HistoryUSD = 3
    End Enum

    ''' <summary>
    ''' Achtung: Import ist nicht fertig!
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Private Sub Import_Zyado(Filename As String,
                             Optional ByVal CheckFirstLine As Boolean = True)

        Dim Row() As String
        Dim ErrCounter As Long = _MaxErrors
        Dim LineCount As Long
        Dim Record As dtoTradesRecord
        Dim RecordFee As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim CSV As New CSVHelper(Filename, System.Text.Encoding.UTF8, True)
        Dim LineFee As Decimal

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                ' Datei hat offenbar falsches Format!
                _ImportFileHelper.InvalidFileMessage(Filename)
                Exit Sub
            End If
            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "Zyado.com"))
            ' Daten in Liste einlesen
            If CSV.ReadAllRows(True, ";", "", ",", ".") > 0 Then

                ImportRecords = New List(Of dtoTradesRecord)
                LineCount = 1
                Dim AllLines As Long = CSV.Rows.Count
                For Each Row In CSV.Rows
                    ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", LineCount, AllLines))
                    LineCount += 1
                    If Row.Length >= 5 Then

                        Record = New dtoTradesRecord
                        With Record
                            Try
                                RecordFee = Nothing
                                .SourceID = MD5FromString(Row(0) & Row(1) & Row(3) & Row(4) & Row(5))
                                .Zeitpunkt = CDate(Row(5))
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = PlatformManager.Platforms.Zyado
                                .QuellPlattformID = .ImportPlattformID
                                .ZielPlattformID = .QuellPlattformID
                                LineFee = CSV.StringToDecimal(Row(4).Replace(" BTC", "").Replace(" €", ""))
                                '.Info = Row(1)
                                Select Case Row(0)
                                    Case "Bought"
                                        ' Kauf
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                        .ZielKontoID = DBHelper.Konten.BTC
                                        .ZielBetrag = CSV.StringToDecimal(Row(1))
                                        .BetragNachGebuehr = .ZielBetrag - LineFee
                                        .QuellBetrag = CSV.StringToDecimal(Row(3).Substring(0, Row(3).Length - 2))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = DBHelper.Konten.EUR
                                        .WertEUR = .QuellBetrag
                                        .Info = String.Format("Kauf BTC - Volumen {0} BTC für {1} EUR / Kurs {2}", .ZielBetrag, .QuellBetrag, Row(2))
                                        ' Gebühren-Transaktion
                                        If .ZielBetrag <> .BetragNachGebuehr Then
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.ZielKontoID = DBHelper.Konten.feeBTC
                                            RecordFee.ZielBetrag = LineFee
                                            RecordFee.WertEUR = 0
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.QuellKontoID = DBHelper.Konten.BTC
                                            RecordFee.Info = "Gebühr zu BTC-Kauf Referenz " & .SourceID
                                        End If
                                    Case "Sold"
                                        ' Verkauf
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                        .QuellBetrag = CSV.StringToDecimal(Row(1))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = DBHelper.Konten.BTC
                                        .ZielBetrag = CSV.StringToDecimal(Row(3).Substring(0, Row(3).Length - 2))
                                        .BetragNachGebuehr = .ZielBetrag - LineFee
                                        .ZielKontoID = DBHelper.Konten.EUR
                                        .WertEUR = .BetragNachGebuehr
                                        .Info = String.Format("Verkauf BTC - Volumen {0} BTC für {1} EUR / Kurs {2}", .ZielBetrag, .QuellBetrag, Row(2))
                                        ' Gebühren-Transaktion
                                        If .ZielBetrag <> .BetragNachGebuehr Then
                                            RecordFee = .Clone()
                                            RecordFee.SourceID = .SourceID & "/fee"
                                            RecordFee.TradetypID = DBHelper.TradeTypen.Gebühr
                                            RecordFee.ZielKontoID = DBHelper.Konten.feeEUR
                                            RecordFee.ZielBetrag = LineFee
                                            RecordFee.WertEUR = 0
                                            RecordFee.BetragNachGebuehr = 0
                                            RecordFee.QuellBetrag = RecordFee.ZielBetrag
                                            RecordFee.QuellBetragNachGebuehr = RecordFee.QuellBetrag
                                            RecordFee.QuellKontoID = .ZielKontoID
                                            RecordFee.Info = "Gebühr zu BTC-Verkauf Referenz " & .SourceID
                                        End If
                                    Case Else
                                        ' Ansonsten nicht importieren
                                        .DoNotImport = True
                                End Select

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                    If RecordFee IsNot Nothing Then
                                        ImportRecords.Add(RecordFee)
                                    End If
                                End If

                            Catch ex As Exception
                                Cursor.Current = Cursors.Default
                                ErrCounter -= 1
                                DefaultErrorHandler(ex, "Fehler beim Einlesen der Zyado.com-Datei in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                    IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                        "Einlesen wird abgebrochen.", ""))
                                If ErrCounter = 0 Then
                                    DestroyProgressForm()
                                    Exit Sub
                                End If
                            End Try

                        End With
                    End If

                Next Row

                Import_Records(ImportRecords, Filename, _ReadImportdataPercentage, , True)
            Else
                DestroyProgressForm()
            End If

            Cursor.Current = Cursors.Default

        End If

    End Sub

    Private Sub Import_MtGox(Filename As String,
                             Optional ByVal CheckFirstLine As Boolean = True)

        Dim HistType As MtGoxHistoryType
        Dim Row() As String
        Dim ErrCounter As Long = _MaxErrors
        Dim Tmp() As String
        Dim LineCount As Long
        Dim Record As dtoTradesRecord
        Dim ImportRecords As List(Of dtoTradesRecord)
        Dim CSV As New CSVHelper(Filename, System.Text.Encoding.UTF8, False)

        If CSV.FileExists Then
            Cursor.Current = Cursors.WaitCursor
            If CheckFirstLine AndAlso _ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, _Plattform) = 0 Then
                ' Datei hat offenbar falsches Format!
                _ImportFileHelper.InvalidFileMessage(Filename)
            End If
            InitProgressForm(String.Format(My.Resources.MyStrings.importMsgImportStarting, "Mt. Gox"))
            ' Daten in Liste einlesen
            If CSV.ReadAllRows(True, ",", """", ".", ",") > 0 Then

                ' Art der History-Datei anhand des Namens ermitteln
                If Filename.Contains("_USD") Then
                    HistType = MtGoxHistoryType.HistoryUSD
                ElseIf Filename.Contains("_EUR") Then
                    HistType = MtGoxHistoryType.HistoryEUR
                Else
                    HistType = MtGoxHistoryType.HistoryBTC
                End If
                ImportRecords = New List(Of dtoTradesRecord)
                ' Array-List sortieren (Reihenfolge der Zeilen ist bei MtGox mal so, mal so...)
                CSV.Rows.Sort(Function(x, y) Val(x(0)).CompareTo(Val(y(0))))
                Dim AllLines As Long = CSV.Rows.Count
                LineCount = 1
                For Each Row In CSV.Rows
                    ProgressWaitManager.UpdateProgress(LineCount / AllLines * _ReadImportdataPercentage, String.Format("Lese Datei ein... ({0}/{1})", LineCount, AllLines))
                    LineCount += 1
                    Record = New dtoTradesRecord
                    With Record
                        Try
                            If Row.Length <> 1 Then
                                .SourceID = Row(0)
                                .Zeitpunkt = CDate(Row(1)).ToLocalTime
                                .ZeitpunktZiel = .Zeitpunkt
                                .ImportPlattformID = PlatformManager.Platforms.MtGox
                                .Info = Row(3)
                                ' Info in Worte zerlegen, vorher  &nbsp; durch normales Space ersetzen (unten schwer zu erkennen)
                                Tmp = Split(.Info.Replace(" ", " "), " ")
                                Select Case Row(2)
                                    Case "in"
                                        ' Kauf
                                        .TradetypID = DBHelper.TradeTypen.Kauf
                                        .QuellPlattformID = PlatformManager.Platforms.MtGox
                                        .ZielPlattformID = .QuellPlattformID
                                        .ZielKontoID = GetAccount(Tmp(0)).ID
                                        .ZielBetrag = CSV.StringToDecimal(Row(4))
                                        .BetragNachGebuehr = .ZielBetrag
                                        If Tmp.Length = 7 Then
                                            ' Wahrscheinlich Dollar
                                            If IsNumeric(Left(Tmp(6), 1)) Then
                                                .QuellBetrag = Math.Round(CSV.StringToDecimal(Tmp(6)) * .ZielBetrag, 8, MidpointRounding.AwayFromZero)
                                                .QuellBetragNachGebuehr = .QuellBetrag
                                                .QuellKontoID = DBHelper.Konten.Fehler
                                            Else
                                                .QuellBetrag = Math.Round(CSV.StringToDecimal(Tmp(6).Substring(1)) * .ZielBetrag, 8, MidpointRounding.AwayFromZero)
                                                .QuellBetragNachGebuehr = .QuellBetrag
                                                .QuellKontoID = GetAccount(Left(Tmp(6), 1)).ID
                                            End If
                                        Else
                                            ' Wahrscheinlich EUR
                                            .QuellBetrag = Math.Round(CSV.StringToDecimal(Tmp(6)) * .ZielBetrag, 8, MidpointRounding.AwayFromZero)
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .QuellKontoID = GetAccount(Tmp(7)).ID

                                        End If
                                        If .QuellKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .QuellBetrag
                                        End If
                                    Case "fee"
                                        ' Gebühr
                                        'If HistType = MtGoxHistoryType.HistoryBTC OrElse (PrevRow.Length >= 3 AndAlso (PrevRow(2) = "deposit" OrElse PrevRow(2) = "withdraw")) Then
                                        .TradetypID = DBHelper.TradeTypen.Gebühr
                                        .QuellPlattformID = PlatformManager.Platforms.MtGox
                                        .ZielPlattformID = .QuellPlattformID
                                        .ZielBetrag = CSV.StringToDecimal(Row(4))
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        Select Case HistType
                                            Case MtGoxHistoryType.HistoryEUR
                                                .QuellKontoID = DBHelper.Konten.EUR
                                                .SourceID = "EUR/" & .SourceID
                                            Case MtGoxHistoryType.HistoryUSD
                                                .QuellKontoID = DBHelper.Konten.USD
                                                .SourceID = "USD/" & .SourceID
                                            Case Else
                                                .QuellKontoID = DBHelper.Konten.BTC
                                        End Select
                                        .ZielKontoID = GetAccount(.QuellKontoID).GebuehrKontoID
                                    Case "out"
                                        ' Verkauf oder Abhebung
                                        .TradetypID = DBHelper.TradeTypen.Verkauf
                                        .QuellPlattformID = PlatformManager.Platforms.MtGox
                                        .ZielPlattformID = .QuellPlattformID
                                        .QuellBetrag = CSV.StringToDecimal(Row(4))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .QuellKontoID = GetAccount(Tmp(0)).ID
                                        If Tmp.Length = 7 Then
                                            ' Wahrscheinlich Dollar
                                            If IsNumeric(Left(Tmp(6), 1)) Then
                                                .ZielBetrag = Math.Round(CSV.StringToDecimal(Tmp(6)) * .QuellBetrag, 8, MidpointRounding.AwayFromZero)
                                                .ZielKontoID = DBHelper.Konten.Fehler
                                            Else
                                                .ZielBetrag = Math.Round(CSV.StringToDecimal(Tmp(6).Substring(1)) * .QuellBetrag, 8, MidpointRounding.AwayFromZero)
                                                .ZielKontoID = GetAccount(Left(Tmp(6), 1)).ID
                                            End If
                                        ElseIf Tmp.Length > 7 Then
                                            ' Wahrscheinlich EUR
                                            .ZielBetrag = Math.Round(CSV.StringToDecimal(Tmp(6)) * .QuellBetrag, 8, MidpointRounding.AwayFromZero)
                                            .ZielKontoID = GetAccount(Tmp(7)).ID
                                        Else
                                            ' Wahrscheinlich eine Abhebung
                                            .TradetypID = DBHelper.TradeTypen.Auszahlung
                                            .ZielPlattformID = PlatformManager.Platforms.Unknown
                                            .QuellBetrag = CSV.StringToDecimal(Row(4))
                                            .QuellBetragNachGebuehr = .QuellBetrag
                                            .ZielBetrag = .QuellBetrag
                                            Select Case HistType
                                                Case MtGoxHistoryType.HistoryEUR
                                                    .QuellKontoID = DBHelper.Konten.EUR
                                                    .SourceID = "EUR/" & .SourceID
                                                Case MtGoxHistoryType.HistoryUSD
                                                    .QuellKontoID = DBHelper.Konten.USD
                                                    .SourceID = "USD/" & .SourceID
                                                Case Else
                                                    .QuellKontoID = DBHelper.Konten.BTC
                                            End Select
                                            .ZielKontoID = .QuellKontoID
                                        End If
                                        .BetragNachGebuehr = .ZielBetrag
                                        If .ZielKontoID = DBHelper.Konten.EUR Then
                                            .WertEUR = .ZielBetrag
                                        End If
                                    Case "withdraw"
                                        ' Auszahlung
                                        .TradetypID = DBHelper.TradeTypen.Auszahlung
                                        .QuellPlattformID = PlatformManager.Platforms.MtGox
                                        .ZielPlattformID = PlatformManager.Platforms.Unknown
                                        .QuellBetrag = CSV.StringToDecimal(Row(4))
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .ZielBetrag = .QuellBetrag
                                        .BetragNachGebuehr = .QuellBetrag   ' Bei Auszahlungen steht der Betrag, der am Ziel ankommt, in BetragNachGebuehr!
                                        Select Case HistType
                                            Case MtGoxHistoryType.HistoryEUR
                                                .QuellKontoID = DBHelper.Konten.EUR
                                                .SourceID = "EUR/" & .SourceID
                                            Case MtGoxHistoryType.HistoryUSD
                                                .QuellKontoID = DBHelper.Konten.USD
                                                .SourceID = "USD/" & .SourceID
                                            Case Else
                                                .QuellKontoID = DBHelper.Konten.BTC
                                        End Select
                                        .ZielKontoID = .QuellKontoID
                                    Case "deposit"
                                        ' Einzahlung
                                        .TradetypID = DBHelper.TradeTypen.Einzahlung
                                        .QuellPlattformID = PlatformManager.Platforms.Unknown
                                        .ZielPlattformID = PlatformManager.Platforms.MtGox
                                        .ZielBetrag = CSV.StringToDecimal(Row(4))
                                        .QuellBetrag = .ZielBetrag
                                        .QuellBetragNachGebuehr = .QuellBetrag
                                        .BetragNachGebuehr = .ZielBetrag
                                        Select Case HistType
                                            Case MtGoxHistoryType.HistoryEUR
                                                .ZielKontoID = DBHelper.Konten.EUR
                                                .SourceID = "EUR/" & .SourceID
                                            Case MtGoxHistoryType.HistoryUSD
                                                .ZielKontoID = DBHelper.Konten.USD
                                                .SourceID = "USD/" & .SourceID
                                            Case Else
                                                .ZielKontoID = DBHelper.Konten.BTC
                                        End Select
                                        .QuellKontoID = .ZielKontoID
                                    Case Else
                                        ' Ansonsten nicht importieren
                                        .DoNotImport = True
                                End Select

                                If Not .DoNotImport Then
                                    ' Record der Liste hinzufügen
                                    ImportRecords.Add(Record)
                                End If

                            End If

                        Catch ex As Exception
                            Cursor.Current = Cursors.Default
                            ErrCounter -= 1
                            DefaultErrorHandler(ex, "Fehler beim Einlesen der Mt.-Gox-History in Zeile " & LineCount & ":" & Environment.NewLine & ex.Message &
                                                IIf(ErrCounter = 0, Environment.NewLine & Environment.NewLine & "Es sind zu viele Fehler aufgetreten. " &
                                                    "Einlesen wird abgebrochen.", ""))
                            If ErrCounter = 0 Then
                                DestroyProgressForm()
                                Exit Sub
                            End If
                        End Try

                    End With

                Next Row

                Import_Records(ImportRecords, Filename, _ReadImportdataPercentage, True)
            Else
                DestroyProgressForm()
            End If

            Cursor.Current = Cursors.Default

        End If

    End Sub

    ''' <summary>
    ''' Schreibt alle in ImportRecords übergebenen Einträge in die Trades-Tabelle. Stellt dabei sicher, dass bereits vorhandene Datensätze nicht erneut importiert werden. Wenn FileName übergeben wird, erfolgt zusätzlich ein Eintrag in Importe.
    ''' </summary>
    ''' <param name="ImportRecords">Liste der zu importierenden dtoTradeRecords</param>
    ''' <param name="Filename">Name der eingelesenen Datei (wird in Tabelle Importe geschrieben)</param>
    ''' <param name="StartPercentage">Für ProgressWaitWindow: Anfangsprozentsatz, ab dem der Fortschrittsbalken weitergezeichnet wird</param>
    ''' <param name="ProcessFeeEntries">Wenn True, werden für neu hinzugekommene Fee-Einträge die jeweils zugehörigen Transaktion angepasst: BetragNachGebuehr und WertEUR werden ggf. verringert (notwendig für Mt. Gox)</param>
    ''' <param name="IgnoreDuplicates">Wenn True, werden Zeilen mit gleicher SourceID innerhalb der übergebenen dtoTradeRecords-Liste 1:1 (also auch mehrfach) eingelesen. Notwendig z.B. für BitstampNet.</param>
    ''' <remarks>Im Anschluss an den Import erfolgt ggf. die automatische Weiterverarbeitung ein Ein- und Auszahlungen sowie die Kontrolle, ob die Gewinn-/Verlustberechnung zurückgesetzt werden muss</remarks>
    Friend Sub Import_Records(ImportRecords As List(Of dtoTradesRecord),
                               Optional ByVal Filename As String = "",
                               Optional ByVal StartPercentage As Integer = 70,
                               Optional ByVal ProcessFeeEntries As Boolean = False,
                               Optional ByVal IgnoreDuplicates As Boolean = False,
                               Optional ByVal ApiDatenID As Long = 0,
                               Optional ByVal LastImportTimestamp As Long = 0,
                               Optional ByVal Verbose As Boolean = True)

        Dim IR As dtoTradesRecord
        Dim ImporteTb As New ImporteDataTable
        Dim ImporteTa As New ImporteTableAdapter
        Dim Tbl As DataTable
        Dim FoundRow As DataRow
        Dim StartTradeRow As Long, MaxTradeID As Long, l As Long
        Dim HasTransfers As Boolean = False
        Dim MinDate As Date = #12/31/2099#
        Dim CheckDoublesDict As New Dictionary(Of String, Long)

        _ZuletztEingelesen = 0
        _ZuletztUeberprungen = 0
        _LastImportId = 0
        Tbl = _DB.DataTable(TableNames.Trades)
        StartTradeRow = Tbl.Rows.Count
        MaxTradeID = _DB.GetMaxID(TableNames.Trades)
        ' Collection für Überprüfen auf Doppelungen aufbauen
        For Each FoundRow In Tbl.Rows
            If FoundRow("ImportPlattformID") = CInt(_Plattform) Then
                Try
                    CheckDoublesDict.Add(FoundRow("SourceID"), 1)
                Catch ex As ArgumentException
                    ' no matter...
                End Try
            End If
        Next
        For Each IR In ImportRecords
            With IR
                ProgressWaitManager.UpdateProgress(StartPercentage + (_ZuletztEingelesen + _ZuletztUeberprungen) / ImportRecords.Count * (100 - StartPercentage), String.Format("Verarbeite Datensätze... ({0}/{1})", _ZuletztEingelesen + _ZuletztUeberprungen, ImportRecords.Count))
                If Not .DoNotImport Then
                    If CheckDoublesDict.ContainsKey(IR.SourceID) Then
                        ' SourceID schon vorhanden -> nicht imporieren!
                        _ZuletztUeberprungen += 1
                    Else
                        ' noch nicht vorhanden -> importieren
                        IR.ImportID = 0
                        Tbl.Rows.Add(IR.GetNewDataRow(Tbl))
                        If IR.Zeitpunkt < MinDate Then
                            MinDate = IR.Zeitpunkt
                        End If
                        _ZuletztEingelesen += 1
                        If .TradetypID = DBHelper.TradeTypen.Kauf Then
                        ElseIf .TradetypID = DBHelper.TradeTypen.Auszahlung Or .TradetypID = DBHelper.TradeTypen.Einzahlung Then
                            HasTransfers = True
                        End If
                        If Not IgnoreDuplicates Then
                            CheckDoublesDict.Add(IR.SourceID, 0)
                        End If
                    End If
                Else
                    _ZuletztUeberprungen += 1
                End If

            End With

        Next
        If _ZuletztEingelesen > 0 OrElse ApiDatenID > 0 Then
            ' Eintrag in Importe
            ImporteTa.Insert(PlattformID:=_Plattform,
                             Dateiname:=PathEx.GetFileNameIfPossible(Filename),
                             PfadDateiname:=Filename,
                             Zeitpunkt:=DateTime.Now,
                             Eingelesen:=_ZuletztEingelesen,
                             NichtEingelesen:=_ZuletztUeberprungen,
                             ApiDatenID:=ApiDatenID,
                             LastImportTimestamp:=LastImportTimestamp)
            ImporteTa.FillBySQL(ImporteTb, "order by ID desc limit 1")
            _LastImportId = ImporteTb(0).ID
            ProgressWaitManager.UpdateProgress(String.Format("Schreibe {0} Einträge in die Datenbank. Bitte warten Sie...", _ZuletztEingelesen.ToString(MESSAGENUMBERFORMAT)))
            ' ID des Imports in Trades (unschön, weil per Schleife statt SQL...)
            If _LastImportId >= 0 Then
                For l = StartTradeRow To Tbl.Rows.Count - 1
                    If Tbl.Rows(l)("ImportID") = 0 Then
                        Tbl.Rows(l)("ImportID") = _LastImportId
                    End If
                Next
            End If
            _DB.Update(TableNames.Trades)
            ' Für Mt. Gox: müssen Gebühren-Einträge verarbeitet werden?
            If ProcessFeeEntries Then
                Me.ProcessFeeEntries(MaxTradeID)
            End If
            ' Wenn es neue Einzahlungen gibt, zugehörige Transfer-Datensätze anlegen
            If HasTransfers Then
                ProcessTransferData()
            Else
                _LastTransfersInserted = 0
                _LastTransfersUpdated = 0
            End If
            If _TVM IsNot Nothing Then
                ' Sicherheitshalber Verlust-Einträge für die aktuelle Platform neu berechnen
                _TVM.ResetPlatformLossTrades(_Plattform)
                ' Ggf. Gewinnberechnung des TradeValueObjekts korrigieren
                _TVM.RollbackCalculation(MinDate, False, True)
            End If
        End If

        DestroyProgressForm()

        If ImportRecords.Count = 0 AndAlso ApiDatenID = 0 AndAlso Verbose Then
            MsgBoxEx.BringToFront()
            MessageBox.Show("Es wurden keine Trades importiert, weil keine gültigen Datenzeilen enthalten waren (oder erkannt werden konnten).",
                            "Nichts importiert", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    ''' <summary>
    ''' Wertet Gebühren-Einträge aus: zugehörige Trades werden in BetragNachGebuehr sowie WertEUR ggf. angepasst
    ''' </summary>
    ''' <param name="MinTradeID">Minimale ID der zu verarbeitenden Gebühren-Einträge</param>
    ''' <remarks></remarks>
    Private Sub ProcessFeeEntries(MinTradeID As Long)
        Dim SQL As String
        SQL = "update Trades set BetragNachGebuehr = ZielBetrag - " &
            "(select QuellBetrag from Trades g " &
            "where g.TradeTypID = 9 And substr(g.Info, 1, 35) = substr(Trades.Info, 1, 35) " &
            "and g.ImportPlattformID = Trades.ImportPlattformID " &
            "and g.QuellKontoID = Trades.ZielKontoID limit 1) " &
            "where substr(Info,1,35) in (select substr(tg.Info,1,35) " &
            "from Trades tg where tg.TradeTypID = 9 and tg.QuellKontoID = Trades.ZielKontoID {0}) " &
            "and TradeTypID <> 9 {1}"
        ' Anpassung für alle Gebühren-Einträge > MinTradeID
        _DB.ExecuteSQL(String.Format(SQL, "and tg.ID>" & MinTradeID, ""))
        ' Anpassung für alle Trade-Einträge > MinTradeID (Falls Gebühren-Eintrag schon älter ist)
        _DB.ExecuteSQL(String.Format(SQL, "", "and ID>" & MinTradeID))
        ' Dann die Ein- und Auszahlungen
        SQL = "update Trades set BetragNachGebuehr = ZielBetrag - " &
            "(select QuellBetrag from Trades g " &
            "where g.TradeTypID = 9 And g.Info = 'Fees for ' || Trades.Info " &
            "and g.ImportPlattformID = Trades.ImportPlattformID " &
            "and g.QuellKontoID = Trades.ZielKontoID limit 1) " &
            "where 'Fees for ' || Trades.Info in (select tg.Info " &
            "from Trades tg where tg.TradeTypID = 9 and tg.QuellKontoID = Trades.ZielKontoID {0}) " &
            "and TradeTypID <> 9 {1}"
        ' Anpassung für alle Gebühren-Einträge > MinTradeID
        _DB.ExecuteSQL(String.Format(SQL, "and tg.ID>" & MinTradeID, ""))
        ' Anpassung für alle Trade-Einträge > MinTradeID (Falls Gebühren-Eintrag schon älter ist)
        _DB.ExecuteSQL(String.Format(SQL, "", "and ID>" & MinTradeID))
        ' Anpassen des WertEUR
        _DB.ExecuteSQL("update Trades set WertEUR = BetragNachGebuehr where TradeTypID <> 9 and WertEUR > 0 and ZielKontoID = 101")
    End Sub

    ''' <summary>
    ''' Verarbeitet neu in Trades geschriebene Einzahlungen und Abbuchungen, indem zugehörige Transfer-Datensätze geschrieben oder aktualisiert werden. 
    ''' </summary>
    Friend Sub ProcessTransferData()

        Dim TradesTableAdapter As New CoinTracerDataSetTableAdapters.TradesTableAdapter
        Dim tbTrades As Data.DataTable
        Dim FoundTrades As New CoinTracerDataSet.TradesDataTable
        Dim SQL As String
        Dim TradeRec As dtoTradesRecord, TransRec As dtoTradesRecord, GebRec As dtoTradesRecord
        Dim InsertCounter As Long = 0, MaxID As Long
        Dim UpdateCounter As Long = 0
        Dim Result(1) As Long
        Dim trNum As Long

        Cursor.Current = Cursors.WaitCursor

        _DB.Reset_DataAdapter(TableNames._AnyTable, "select t.* from Trades t " &
                                   "where t.TradeTypID in (" & DBHelper.TradeTypen.Auszahlung & "," & DBHelper.TradeTypen.Einzahlung & "," & DBHelper.TradeTypen.Transfer & ") " &
                                   "and Entwertet = 0 " &
                                   "order by t.Zeitpunkt")
        tbTrades = _DB.DataTable(TableNames._AnyTable)
        If tbTrades.Rows.Count = 0 Then
            Exit Sub
        End If

        MaxID = _DB.GetMaxID(TableNames.Trades)
        ' Schleife über alle "offenen" Ein-, Auszahlungen & Transfers
        For trNum = 0 To tbTrades.Rows.Count - 1
            TradeRec = New dtoTradesRecord(tbTrades.Rows(trNum))
            With TradeRec
                If .TradetypID = DBHelper.TradeTypen.Einzahlung Then

                    ' Offene *Einzahlung* - Transfer-Eintrag mit Auszahlungsinformationen suchen
                    SQL = "where QuellKontoID=" & .ZielKontoID & " and TradeTypID=" & DBHelper.TradeTypen.Transfer & " and " &
                        " QuellPlattformID<>" & .ZielPlattformID & " and (" &
                        "InTradeID=0 or InTradeID=-1) and (" &
                        "QuellBetrag>=" & Convert.ToString(.ZielBetrag - .ZielBetrag * TransferDetection.AmountPercentTolerance).Replace(",", ".") & " and " &
                        "QuellBetrag<=" & Convert.ToString(.ZielBetrag + .ZielBetrag * TransferDetection.AmountPercentTolerance).Replace(",", ".") & ") and (" &
                        "Zeitpunkt>='" & DateAdd(DateInterval.Minute, -TransferDetection.MinutesTolerance, .Zeitpunkt).ToString("yyyy-MM-dd HH:mm:ss") & "' and " &
                        "Zeitpunkt<='" & DateAdd(DateInterval.Minute, TransferDetection.MinutesTolerance, .Zeitpunkt).ToString("yyyy-MM-dd HH:mm:ss") & "') " &
                        "order by abs(Quellbetrag < " & .ZielBetrag.ToString("########0.0#######", CultureInfo.InvariantCulture) & ")"
                    TradesTableAdapter.FillBySQL(FoundTrades, SQL)
                    If FoundTrades.Count >= 1 Then
                        ' Passenden Transfer gefunden!
                        UpdateCounter += 1
                        TransRec = New dtoTradesRecord(FoundTrades(0))
                        If .ZielBetrag < TransRec.QuellBetrag Then
                            ' Einzahlungsbetrag ist geringer als der ursprüngliche Auszahlungsbetrag, wir brauchen einen Gebühren-Eintrag!
                            GebRec = TransRec.Clone
                            GebRec.TradetypID = DBHelper.TradeTypen.Gebühr
                            GebRec.ZielPlattformID = GebRec.QuellPlattformID
                            GebRec.QuellBetrag = TransRec.QuellBetrag - .ZielBetrag
                            GebRec.QuellBetragNachGebuehr = GebRec.QuellBetrag
                            GebRec.ZielBetrag = GebRec.QuellBetrag
                            GebRec.ZielKontoID = GetAccount(TransRec.QuellKontoID).GebuehrKontoID
                            GebRec.SourceID = TransRec.SourceID & "/fee"
                            GebRec.WertEUR = 0
                            GebRec.BetragNachGebuehr = 0
                            GebRec.Info = "Gebühr für Coin-Transfer (Eintrag automatisch erstellt)"
                            GebRec.OutTradeID = 0
                            GebRec.InTradeID = 0
                            GebRec.RefTradeID = TransRec.ID
                            GebRec.ImportID = .ImportID
                            tbTrades.Rows.Add(GebRec.GetNewDataRow(tbTrades, MaxID))
                            ' QuellBetragNachGebuehr der Transaktion verringern (QuellBetrag muss aber so bleiben!)
                            TransRec.QuellBetragNachGebuehr = .ZielBetrag
                        End If
                        ' Transfer-Datensatz und Trade-Datensatz anpassen
                        .Entwertet = 1
                        .RefTradeID = TransRec.ID
                        .SetDataRow(tbTrades.Rows(trNum))
                        TransRec.ZielKontoID = .ZielKontoID
                        TransRec.ZielPlattformID = .ZielPlattformID
                        TransRec.ZielBetrag = .ZielBetrag
                        TransRec.BetragNachGebuehr = .BetragNachGebuehr
                        TransRec.WertEUR = 0
                        TransRec.ZeitpunktZiel = .Zeitpunkt
                        TransRec.InTradeID = .ID
                        TransRec.SetDataRow(FoundTrades(0))
                        TradesTableAdapter.Update(FoundTrades(0))
                    ElseIf FoundTrades.Count = 0 Then
                        ' Kein Transfer da? Anlegen mit Einzahlungsinformationen!
                        InsertCounter += 1
                        TransRec = TradeRec.Clone
                        TransRec.WertEUR = 0
                        TransRec.InTradeID = .ID
                        TransRec.SourceID = MD5FromString(.SourceID & .ImportPlattformID.ToString)
                        TransRec.TradetypID = DBHelper.TradeTypen.Transfer
                        TransRec.QuellPlattformID = .QuellPlattformID
                        TransRec.QuellBetrag = .ZielBetrag
                        TransRec.QuellBetragNachGebuehr = TransRec.QuellBetrag
                        TransRec.WertEUR = 0
                        TransRec.Info = .Info & IIf(.Info.Length > 0, " ", "") & "(Transfer-Eintrag automatisch erstellt)"
                        TransRec.RefTradeID = 0
                        TransRec.OutTradeID = 0
                        TransRec.Entwertet = 0
                        TransRec.ZeitpunktZiel = .Zeitpunkt
                        tbTrades.Rows.Add(TransRec.GetNewDataRow(tbTrades, MaxID))
                        .RefTradeID = MaxID
                        .Entwertet = 1
                        .SetDataRow(tbTrades.Rows(trNum))
                    End If

                ElseIf .TradetypID = DBHelper.TradeTypen.Auszahlung Then

                    ' Offene *Auszahlung* - Transfer-Eintrag mit Einzahlungsinformationen suchen
                    SQL = "where ZielKontoID=" & .QuellKontoID & " and TradeTypID=" & DBHelper.TradeTypen.Transfer & " and " &
                        " ZielPlattformID<>" & .QuellPlattformID & " and (" &
                        "OutTradeID=0 or OutTradeID=-1) and (" &
                        "ZielBetrag>=" & Convert.ToString(.QuellBetrag - .QuellBetrag * TransferDetection.AmountPercentTolerance).Replace(",", ".") & " and " &
                        "ZielBetrag<=" & Convert.ToString(.QuellBetrag + .QuellBetrag * TransferDetection.AmountPercentTolerance).Replace(",", ".") & ") and (" &
                        "Zeitpunkt>='" & DateAdd(DateInterval.Minute, -TransferDetection.MinutesTolerance, .Zeitpunkt).ToString("yyyy-MM-dd HH:mm:ss") & "' and " &
                        "Zeitpunkt<='" & DateAdd(DateInterval.Minute, TransferDetection.MinutesTolerance, .Zeitpunkt).ToString("yyyy-MM-dd HH:mm:ss") & "') " &
                        "order by abs(ZielBetrag < " & .QuellBetrag.ToString("########0.0#######", CultureInfo.InvariantCulture) & ")"
                    TradesTableAdapter.FillBySQL(FoundTrades, SQL)
                    If FoundTrades.Count >= 1 Then
                        ' Found at least one matching transfer (first one is the most likely)
                        UpdateCounter += 1
                        TransRec = New dtoTradesRecord(FoundTrades(0))
                        If .BetragNachGebuehr > TransRec.ZielBetrag Then
                            ' Auszahlungsbetrag ist höher als der angekommene Einzahlungsbetrag: wir brauchen einen Gebühren-Eintrag!
                            GebRec = TransRec.Clone
                            GebRec.TradetypID = DBHelper.TradeTypen.Gebühr
                            GebRec.QuellPlattformID = .QuellPlattformID
                            GebRec.ZielPlattformID = GebRec.QuellPlattformID
                            GebRec.QuellBetrag = .QuellBetragNachGebuehr - TransRec.ZielBetrag
                            GebRec.QuellBetragNachGebuehr = GebRec.QuellBetrag
                            GebRec.ZielBetrag = GebRec.QuellBetrag
                            GebRec.QuellKontoID = TransRec.ZielKontoID
                            GebRec.ZielKontoID = GetAccount(TransRec.ZielKontoID).GebuehrKontoID
                            GebRec.SourceID = TransRec.SourceID & "/fee"
                            GebRec.WertEUR = 0
                            GebRec.BetragNachGebuehr = 0
                            GebRec.Info = "Gebühr für Coin-Transfer (Eintrag automatisch erstellt)" & IIf(.Info.Length > 0, " / " & .Info, "")
                            GebRec.OutTradeID = 0
                            GebRec.InTradeID = 0
                            GebRec.RefTradeID = TransRec.ID
                            GebRec.ImportPlattformID = .ImportPlattformID
                            GebRec.ImportID = .ImportID
                            tbTrades.Rows.Add(GebRec.GetNewDataRow(tbTrades, MaxID))
                            ' Einzahlungs-Betrag um Gebühr erhöhen (BetragNachGebuehr muss aber so bleiben!)
                            .QuellBetragNachGebuehr = TransRec.ZielBetrag
                            ' TransRec.ZielBetrag = .BetragNachGebuehr
                        Else
                            GebRec = New dtoTradesRecord
                        End If
                        ' Transfer-Datensatz und Trade-Datensatz anpassen
                        .Entwertet = 1
                        .RefTradeID = TransRec.ID
                        .SetDataRow(tbTrades.Rows(trNum))
                        TransRec.QuellKontoID = .QuellKontoID
                        TransRec.QuellPlattformID = .QuellPlattformID
                        TransRec.QuellBetrag = .QuellBetrag
                        TransRec.BetragNachGebuehr = .QuellBetrag - GebRec.QuellBetrag
                        TransRec.WertEUR = 0
                        TransRec.OutTradeID = .ID
                        TransRec.Zeitpunkt = .Zeitpunkt
                        TransRec.SetDataRow(FoundTrades(0))
                        TradesTableAdapter.Update(FoundTrades(0))
                    ElseIf FoundTrades.Count = 0 Then
                        ' Kein Transfer da? Anlegen mit Auszahlungsinformationen!
                        InsertCounter += 1
                        TransRec = TradeRec.Clone
                        TransRec.WertEUR = 0
                        TransRec.OutTradeID = .ID
                        TransRec.SourceID = MD5FromString(.SourceID & .ImportPlattformID.ToString)
                        TransRec.TradetypID = DBHelper.TradeTypen.Transfer
                        TransRec.ZielPlattformID = PlatformManager.Platforms.Unknown
                        TransRec.ZielBetrag = .BetragNachGebuehr
                        TransRec.WertEUR = 0
                        TransRec.Info = .Info & IIf(.Info.Length > 0, " ", "") & "(Transfer-Eintrag automatisch erstellt)"
                        TransRec.RefTradeID = 0
                        TransRec.InTradeID = 0
                        TransRec.Entwertet = 0
                        TransRec.ZeitpunktZiel = .Zeitpunkt
                        tbTrades.Rows.Add(TransRec.GetNewDataRow(tbTrades, MaxID))
                        .RefTradeID = MaxID
                        .Entwertet = 1
                        .SetDataRow(tbTrades.Rows(trNum))
                    End If

                End If
            End With
        Next
        _DB.Update(TableNames._AnyTable)

        Cursor.Current = Cursors.Default

        _LastTransfersInserted = InsertCounter
        _LastTransfersUpdated = UpdateCounter

    End Sub


    ''' <summary>
    ''' Returns a message string according to the latest processing of transfers entries. Always empty, if no transfers have been updated or inserted.
    ''' </summary>
    Friend Function GetProcessedTransfersString() As String
        Dim Msg As String = ""
        If _LastTransfersUpdated > 0 Then
            Msg = My.Resources.MyStrings.importMsgTransfersUpdated & vbTab & _LastTransfersUpdated.ToString(MESSAGENUMBERFORMAT) & Environment.NewLine
        End If
        If _LastTransfersInserted > 0 Then
            Msg &= My.Resources.MyStrings.importMsgTransfersInserted & vbTab & _LastTransfersInserted.ToString(MESSAGENUMBERFORMAT) & Environment.NewLine
        End If
        If Msg.Length > 0 Then
            Msg = My.Resources.MyStrings.importMsgTransfers1stLine & Environment.NewLine & Msg
        End If
        Return Msg
    End Function
    ''' <summary>
    ''' Gibt zum übergebenen String das zugehörige Konto als KontenRow zurück
    ''' </summary>
    ''' <param name="String">Ein Ausdruck wie "EUR", "BTC", "€" usw.</param>
    ''' <returns>Fehlerkonto im Falle eines unbekannten Strings</returns>
    Friend Function GetAccount(ByRef [String] As String) As KontenRow
        Dim FoundRows() As KontenRow
        [String] = [String].Trim
        If [String] = "€" Then
            [String] = "EUR"
        ElseIf [String] = "$" Then
            [String] = "USD"
        End If
        FoundRows = _KontenTb.Select(String.Format("Bezeichnung='{0}' or Code='{0}'", [String].Replace("'", "_")))
        If DirectCast(FoundRows, ICollection).Count >= 1 Then
            Return FoundRows(0)
        Else
            Return _KontenTb.FindByID(-1)
        End If
    End Function
    Friend Function GetAccount(ByVal ID As Long) As KontenRow
        Return _KontenTb.FindByID(ID)
    End Function

    ''' <summary>
    ''' Gibt zur übergebenen Plattform-Bezeichnung die passende PlattformenRow zurück. Wenn kein Eintrag in PlatformManager.Platforms gefunden wurde, wird einer erzeugt.
    ''' </summary>
    ''' <param name="Platform">ID, Bezeichnung oder CODE der Plattform</param>
    ''' <returns>PlattformenRow der gefundenen Plattform</returns>
    Friend Function RetrievePlatform(ByRef Platform As String) As PlattformenRow
        Dim FoundRows() As PlattformenRow
        Platform = Platform.Trim
        If Integer.TryParse(Platform, Nothing) Then
            ' Plattform als numerische ID übergeben
            FoundRows = _PlattformenTb.Select("ID =" & Platform)
            If DirectCast(FoundRows, ICollection).Count >= 1 Then
                Return FoundRows(0)
            Else
                ' Nicht gefunden - also anlegen
                Dim NewRow As PlattformenRow = _PlattformenTb.NewRow()
                With NewRow
                    .ID = Platform
                    .Bezeichnung = "Plattform " & Platform
                    .Code = .Bezeichnung
                    .Beschreibung = .Bezeichnung
                    .SortID = Platform
                    .Fix = False
                    .Boerse = True
                    .Eigen = True
                End With
                _PlattformenTb.Rows.Add(NewRow)
                _PlattformenTa.Update(_PlattformenTb)
                Return NewRow
            End If
        Else
            ' Plattform als String übergeben
            FoundRows = _PlattformenTb.Select(String.Format("Bezeichnung='{0}' or Code='{0}'", Platform.Replace("'", "_")))
            If DirectCast(FoundRows, ICollection).Count >= 1 Then
                Return FoundRows(0)
            Else
                ' Nicht gefunden - also anlegen
                Dim ID As Long = 300
                FoundRows = _PlattformenTb.Select("ID < 900")
                If DirectCast(FoundRows, ICollection).Count >= 1 Then
                    ID = FoundRows(UBound(FoundRows)).ID + 1
                End If
                Dim NewRow As PlattformenRow = _PlattformenTb.NewRow()
                With NewRow
                    .ID = ID
                    .Bezeichnung = Platform
                    .Code = Platform
                    .Beschreibung = Platform
                    .SortID = ID
                    .Fix = False
                    .Boerse = True
                    .Eigen = True
                End With
                _PlattformenTb.Rows.Add(NewRow)
                _PlattformenTa.Update(_PlattformenTb)
                Return NewRow
            End If
        End If
    End Function

    ''' <summary>
    ''' Gibt zur übergebenen Konto-Bezeichnung die passende KontenRow zurück. Wenn kein Eintrag in Konten gefunden wurde, wird einer erzeugt.
    ''' </summary>
    ''' <param name="Account">ID, Bezeichnung oder CODE des Kontos</param>
    ''' <param name="AccountShort">CODE des Kontos. Wenn Leerstring, wird der in Account übergebene String bei einer Neuanlage sowohl für die Bezeichnung als auch für den CODE verwendet.</param>
    ''' <returns>KontenRow des gefundenen Kontos</returns>
    Friend Function RetrieveAccount(ByVal Account As String,
                                     Optional ByVal AccountShort As String = "") As KontenRow
        Dim FoundRows() As KontenRow
        Account = Account.Trim
        If Integer.TryParse(Account, Nothing) Then
            ' Konto als numerische ID übergeben
            FoundRows = _KontenTb.Select("ID =" & Account)
            If DirectCast(FoundRows, ICollection).Count >= 1 Then
                Return FoundRows(0)
            Else
                Return _KontenTb.FindByID(-1)
            End If
        Else
            ' Konto als String übergeben
            If Account = "€" Then
                Account = "EUR"
            ElseIf Account = "$" Then
                Account = "USD"
            ElseIf Account = "US Dollar" Then
                Account = "USD"
            ElseIf Account = "XBT" Then
                Account = "BTC"
            ElseIf Account = "BCC" Then
                Account = "BCH"
            End If
            FoundRows = _KontenTb.Select(String.Format("Bezeichnung='{0}' or Code='{0}'", Account.Replace("'", "_")))
            If DirectCast(FoundRows, ICollection).Count >= 1 Then
                Return FoundRows(0)
            Else
                ' Nicht gefunden - also inkl. Gebührenkonto anlegen
                Dim ID As Long = 1
                Dim FeeID As Long = 2
                FoundRows = _KontenTb.Select("ID < 311")
                If DirectCast(FoundRows, ICollection).Count = 0 Then
                    FoundRows = _KontenTb.Select("")
                End If
                ID = FoundRows(UBound(FoundRows)).ID + 1
                FoundRows = _KontenTb.Select("")
                FeeID = FoundRows(UBound(FoundRows)).ID + 1
                If FeeID = ID Then FeeID += 1
                Dim NewRow As KontenRow = _KontenTb.NewRow()
                With NewRow
                    .ID = FeeID
                    .Bezeichnung = "Gebühr " & Account
                    .Code = "fee" & If(AccountShort = "", Account, AccountShort)
                    .Beschreibung = "Gebühren/" & Account
                    .IstFiat = False
                    .Eigen = False
                    .SortID = FeeID
                    .Fix = False
                    .IstGebuehr = True
                    .GebuehrKontoID = 0
                End With
                _KontenTb.Rows.Add(NewRow)
                NewRow = _KontenTb.NewRow()
                With NewRow
                    .ID = ID
                    .Bezeichnung = Account
                    .Code = If(AccountShort = "", Account, AccountShort)
                    .Beschreibung = Account
                    .IstFiat = False
                    .Eigen = True
                    .SortID = ID
                    .Fix = False
                    .IstGebuehr = False
                    .GebuehrKontoID = FeeID
                End With
                _KontenTb.Rows.Add(NewRow)
                _KontenTa.Update(_KontenTb)
                Return NewRow
            End If
        End If
    End Function

#If CONFIG = "Debug" Then

#Region "Test Entry Routines"

    Public Sub Test_FileImport(Platform As PlatformManager.Platforms,
                               Filename As String,
                               Optional ByVal CheckFirstLine As Boolean = True,
                               Optional ByVal SubType As Integer = 0)
        Dim ThisImport As IFileImport = Nothing
        Dim FileNames(0) As String
        Select Case Platform
            Case PlatformManager.Platforms.BitcoinDe
                ThisImport = New Import_BitcoinDe(Me)
                ' Import_BitcoinDe(Filename, CheckFirstLine, SubType)
            Case PlatformManager.Platforms.Kraken
                ThisImport = New Import_Kraken(Me)
                ' Import_Kraken(Filename, CheckFirstLine)
            Case PlatformManager.Platforms.Bitfinex
                ThisImport = New Import_Bitfinex(Me)
            Case PlatformManager.Platforms.CoinTracer
                ThisImport = New Import_CoinTracer(Me)
        End Select

        If ThisImport IsNot Nothing Then
            FileNames(0) = Filename
            ThisImport.FileNames = FileNames
            ThisImport.PerformImport()
        End If

    End Sub

    Public Sub Test_ApiImport(Platform As PlatformManager.Platforms,
                              ByVal ApiKey As String,
                              ByVal ApiSecret As String,
                              ByVal ApiExtendedInfo As String,
                              ByVal DateTimeStart As Date,
                              ByVal DateTimeEnd As Date,
                              ByVal ApiConfigName As String,
                              ByVal ApiDatenID As Long)
        Dim ThisImport As IApiImport = Nothing
        Select Case Platform
            Case PlatformManager.Platforms.BitcoinDe
                ThisImport = New Import_BitcoinDe_Api(Me)
            Case PlatformManager.Platforms.Kraken
                ThisImport = New Import_Kraken_Api(Me)
                ThisImport.CallDelay = KrakenClient.KrakenClient.KRAKEN_API_DEFAULTINTERVAL
                'ApiImport_Kraken(ApiKey,
                '                 ApiSecret,
                '                 DateToUnixTimestamp(DateTimeStart),
                '                 ApiConfigName,
                '                 ApiDatenID,
                '                 DateToUnixTimestamp(DateTimeEnd))
            Case PlatformManager.Platforms.Bitfinex
                ThisImport = New Import_Bitfinex_Api(Me)
                ThisImport.CallDelay = 6000
        End Select
        If ThisImport IsNot Nothing Then
            Dim LogLevel As TraceEventType = My.Settings.LogLevel
            My.Settings.LogLevel = TraceEventType.Verbose
            With ThisImport
                .ApiKey = ApiKey
                .ApiSecret = ApiSecret
                .LastImportTimestamp = DateToUnixTimestamp(DateTimeStart)
                .ApiConfigName = ApiConfigName
                .ApiDatenID = ApiDatenID
                .ExtendedInfo = ApiExtendedInfo
                .DateTimeEnd = DateTimeEnd
                .PerformImport()
            End With
            My.Settings.LogLevel = LogLevel
        End If

    End Sub

    Public Property Test_ImportFileHelper() As ImportFileHelper
        Get
            Return _ImportFileHelper
        End Get
        Set(ByVal value As ImportFileHelper)
            _ImportFileHelper = value
        End Set
    End Property

#End Region

#End If

    Public Sub New(ByRef Database As DBHelper)
        _DB = Database
        _KontenTa = New KontenTableAdapter()
        _KontenTb = New KontenDataTable()
        _KontenTa.Fill(_KontenTb)
        _PlattformenTa = New PlattformenTableAdapter
        _PlattformenTb = New PlattformenDataTable
        _PlattformenTa.Fill(_PlattformenTb)
        _SilentMode = False
        _ApiPassword = "VerwendeDiesesWortAusKeinemGrund§"
        _DefaultApiPassword = _ApiPassword
        _ApiPwCheck = "%AuchDiesesPasswortDientKeinemEchtenZweck!"
    End Sub

    Public Sub New(ByRef Database As DBHelper, _
                   ByRef TradeValueManager As TradeValueManager)
        Me.New(Database)
        _TVM = TradeValueManager
    End Sub

    Public Sub New(ByRef Database As DBHelper, _
                   ByRef TradeValueManager As TradeValueManager, _
                   ByRef ParentForm As Form)
        Me.New(Database)
        _TVM = TradeValueManager
        _Parentform = ParentForm
    End Sub

End Class

''' <summary>
''' Helper class for getting and setting properties needed for joining deposits and withdrawals as transfers
''' </summary>
NotInheritable Class TransferDetection

    Private Const DEFAULTMINUTESGAP As Integer = 23 * 60
    Private Const DEFAULTAMOUNTPERCENTGAP As Decimal = 0.15

    Shared _MinutesGap As Integer
    Shared _PercentGap As Decimal

    Private Sub New()
        _MinutesGap = DEFAULTMINUTESGAP
        _PercentGap = DEFAULTAMOUNTPERCENTGAP
    End Sub

    Public Shared Sub Init()
        Dim Settings() As String = Split(My.Settings.TransferDetection, "|")
        If Not Settings Is Nothing AndAlso Settings.Length > 1 Then
            _MinutesGap = CInt(Settings(0))
            _PercentGap = Math.Abs(StrToDec(Settings(1)))
        Else
            ' default value
            _MinutesGap = DEFAULTMINUTESGAP
            _PercentGap = DEFAULTAMOUNTPERCENTGAP
        End If
    End Sub

    Public Shared Property MinutesTolerance() As Integer
        Get
            Return _MinutesGap
        End Get
        Set(ByVal value As Integer)
            _MinutesGap = value
        End Set
    End Property

    Public Shared Property AmountPercentTolerance() As Decimal
        Get
            Return _PercentGap
        End Get
        Set(ByVal value As Decimal)
            _PercentGap = value
        End Set
    End Property

    Public Shared Sub Save()
        My.Settings.TransferDetection = _MinutesGap.ToString & "|" & _PercentGap.ToString("###0.0###", CultureInfo.InvariantCulture)
    End Sub


End Class

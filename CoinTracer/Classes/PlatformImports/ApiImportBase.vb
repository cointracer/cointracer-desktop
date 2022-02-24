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

Imports CoinTracer
''' <summary>
''' Base class for all api import classes. Not to be instanciated!
''' </summary>
Friend MustInherit Class ApiImportBase
    Implements IApiImport

    Public Sub New()
        Platform = PlatformManager.Platforms.Unknown
        DateTimeEnd = DATENULLVALUE
        ReadImportdataPercentage = 75
    End Sub

    Private _ApiConfigName As String
    Friend Property ApiConfigName As String Implements IApiImport.ApiConfigName
        Get
            Return _ApiConfigName
        End Get
        Set(value As String)
            _ApiConfigName = value
        End Set
    End Property

    Private _ApiDatenID As Long
    Friend Property ApiDatenID As Long Implements IApiImport.ApiDatenID
        Get
            Return _ApiDatenID
        End Get
        Set(value As Long)
            _ApiDatenID = value
        End Set
    End Property

    Private _ApiKey As String
    Friend Property ApiKey As String Implements IApiImport.ApiKey
        Get
            Return _ApiKey
        End Get
        Set(value As String)
            _ApiKey = value
        End Set
    End Property

    Private _ApiSecret As String
    Friend Property ApiSecret As String Implements IApiImport.ApiSecret
        Get
            Return _ApiSecret
        End Get
        Set(value As String)
            _ApiSecret = value
        End Set
    End Property

    Private _DateTimeEnd As Date
    Friend Property DateTimeEnd As Date Implements IApiImport.DateTimeEnd
        Get
            Return _DateTimeEnd
        End Get
        Set(value As Date)
            _DateTimeEnd = value
        End Set
    End Property

    Private _ExtendedInfo As String
    Friend Property ExtendedInfo As String Implements IApiImport.ExtendedInfo
        Get
            Return _ExtendedInfo
        End Get
        Set(value As String)
            _ExtendedInfo = value
        End Set
    End Property

    Private _LastImportTimestamp As Long
    Friend Property LastImportTimestamp As Long Implements IApiImport.LastImportTimestamp
        Get
            Return _LastImportTimestamp
        End Get
        Set(value As Long)
            _LastImportTimestamp = value
        End Set
    End Property

    Private _MainImportObject As Import
    Friend Property MainImportObject() As Import Implements IApiImport.MainImportObject
        Get
            Return _MainImportObject
        End Get
        Set(value As Import)
            _MainImportObject = value
        End Set
    End Property

    Private _Platform As PlatformManager.Platforms
    Friend Property Platform() As PlatformManager.Platforms Implements IApiImport.Platform
        Get
            Return _Platform
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _Platform = value
            PlatformName = PlatformManager.PlatformDetailsByID(Platform).Name
        End Set
    End Property

    Private _PlatformName As String
    Friend Property PlatformName As String Implements IApiImport.PlatformName
        Get
            Return _PlatformName
        End Get
        Set(value As String)
            _PlatformName = value
        End Set
    End Property

    Private _MaxErrors As Integer
    Friend Property MaxErrors() As Integer Implements IApiImport.MaxErrors
        Get
            Return _MaxErrors
        End Get
        Set(ByVal value As Integer)
            _MaxErrors = value
        End Set
    End Property

    Private _ErrorCounter As Integer
    Friend Property ErrorCounter() As Integer
        Get
            Return _ErrorCounter
        End Get
        Set(ByVal value As Integer)
            _ErrorCounter = value
        End Set
    End Property

    Private _ReadImportdataPercentage As Integer
    Friend Property ReadImportdataPercentage() As Integer Implements IApiImport.ReadImportdataPercentage
        Get
            Return _ReadImportdataPercentage
        End Get
        Set(ByVal value As Integer)
            _ReadImportdataPercentage = value
        End Set
    End Property

    Private _CallDelay As Long
    ''' <summary>
    ''' Pause between two API calls in milliseconds
    ''' </summary>
    Friend Property CallDelay() As Long Implements IApiImport.CallDelay
        Get
            Return _CallDelay
        End Get
        Set(ByVal value As Long)
            _CallDelay = value
        End Set
    End Property

    Friend MustOverride Function PerformImport() As Long Implements IApiImport.PerformImport


    ''' <summary>
    ''' Initializes the progress form for this import
    ''' </summary>
    ''' <param name="Message">Initial message to be displayed</param>
    Protected Sub InitProgressForm(Optional Message As String = "")
        Try
            DestroyProgressForm()
            ProgressWaitManager.ShowProgress(_MainImportObject.Parentform)
            If Message.Length = 0 Then
                Message = My.Resources.MyStrings.importMsgStartFileimport
            End If
            ProgressWaitManager.UpdateProgress(1, Message)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Clear and unload the progress form
    ''' </summary>
    Protected Sub DestroyProgressForm()
        Try
            ProgressWaitManager.CloseProgress()
        Catch ex As Exception
            ' no matter...
        End Try
    End Sub

    ''' <summary>
    ''' Default error handler for errors occuring while analysing api trade records. Displays error messages if necessary 
    ''' and destroys the progress form if ErrorCounter = 0
    ''' </summary>
    ''' <param name="ErrorCounter">Current error counter</param>
    ''' <param name="RecordNumber">Number of the record in which the error occured</param>
    ''' <param name="ex">Exception (message will be displayed)</param>
    ''' <returns></returns>
    Protected Function ApiImportError(ByRef ErrorCounter As Long,
                                      ByVal RecordNumber As Long,
                                      ByRef ex As Exception,
                                      Optional ByVal ErrorMessage As String = "")
        Cursor.Current = Cursors.Default
        ErrorCounter -= 1
        If ErrorMessage = "" Then
            ErrorMessage = String.Format(My.Resources.MyStrings.importMsgInvalidApiDataAtRecord,
                                                       Environment.NewLine,
                                                       RecordNumber.ToString(Import.MESSAGENUMBERFORMAT),
                                                       ex.Message)
        End If
        If ErrorCounter = 0 Then
            ErrorMessage &= Environment.NewLine & Environment.NewLine & My.Resources.MyStrings.importMsgInvalidDataAborting
            DestroyProgressForm()
            If MainImportObject.SilentMode Then
                Throw New TradeDataImportException(ErrorMessage)
            End If
        End If
        If Not MainImportObject.SilentMode Then
            DefaultErrorHandler(ex, ErrorMessage, False, My.Resources.MyStrings.importMsgApiServerErrorTitle)
        End If
        Return ErrorCounter
    End Function

    ''' <summary>
    ''' Default error handler for fatal import errors leading to import aborts
    ''' </summary>
    ''' <param name="ex">Original exception</param>
    ''' <param name="ImportStashed">When True, a hint about stashed import data is appended to the error message</param>
    Protected Sub ApiImportFatalError(ByRef ex As Exception, ByVal ImportStashed As Boolean)
        Cursor.Current = Cursors.Default
        DestroyProgressForm()
        Dim ErrorMessage As String = String.Format(My.Resources.MyStrings.importMsgApiFatalError, Environment.NewLine,
                                                   PlatformName, ex.Message)
        If ImportStashed Then ErrorMessage &= Environment.NewLine & Environment.NewLine & My.Resources.MyStrings.importMsgApiStashed
        If MainImportObject.SilentMode Then
            Throw ex
        Else
            DefaultErrorHandler(ex, ErrorMessage,, My.Resources.MyStrings.importMsgApiFatalErrorTitle)
        End If
    End Sub

    ''' <summary>
    ''' Checks if there is a stashed API import for this platform. If so, the user is asked wether this import shall be continued.
    ''' </summary>
    ''' <returns>Nothing when no import is to be continued, a valid ApiImportState otherwise</returns>
    Protected Function GetStashedApiImport() As ApiImportState?
        Dim StashedImport As ApiImportState? = StashedApiImports.PopApiImportByImportId(ApiDatenID)
        If StashedImport Is Nothing Then
            Return StashedImport
        Else
            If Not MainImportObject.SilentMode Then
                MsgBoxEx.PatchMsgBox(New String() {My.Resources.MyStrings.globalContinue, My.Resources.MyStrings.Retry})
                If MessageBox.Show(String.Format(My.Resources.MyStrings.importMsgApiResume, Environment.NewLine,
                                                 PlatformName, My.Resources.MyStrings.globalContinue, My.Resources.MyStrings.Retry),
                                   My.Resources.MyStrings.importMsgApiResumeTitle,
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.No Then
                    StashedImport = Nothing
                End If
            End If

        End If
        Return StashedImport
    End Function

End Class

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

Imports CoinTracer

''' <summary>
''' Exception for invalid trading import files
''' </summary>
<Serializable()>
Public Class ImportFileException
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
''' Base class for all file import classes. Not to be instanciated!
''' </summary>
Public MustInherit Class FileImportBase
    Implements IFileImport

    Public Sub New()
        Platform = PlatformManager.Platforms.Unknown
        ReadImportdataPercentage = 75
        CheckFirstLine = True
        MaxErrors = 6
        ErrorCounter = 0
        ImportFileHelper = New ImportFileHelper
        MultiSelectFiles = False
        MixedFileFormatsAllowed = False
        FileDialogFilter = My.Resources.MyStrings.importOpenFileFilterCSV
        FileNames = {}
        CSVAutoDetectEncoding = True
        CSVEncoding = Text.Encoding.UTF8
        CSVSkipFirstLine = True
        CSVSeparator = ";"
        CSVDecimalPoint = "."
        CSVDecimalSeparator = ","
        CSVTextqualifier = """"
    End Sub

    Public Sub New(MainImportObject As Import)
        Me.New()
        _MainImportObject = MainImportObject
    End Sub

    Private _MainImportObject As Import
    Public Property MainImportObject() As Import Implements IFileImport.MainImportObject
        Get
            Return _MainImportObject
        End Get
        Set(value As Import)
            _MainImportObject = value
        End Set
    End Property

    Private _IFH As ImportFileHelper
    Friend Property ImportFileHelper() As ImportFileHelper Implements IFileImport.ImportFileHelper
        Get
            Return _IFH
        End Get
        Set(ByVal value As ImportFileHelper)
            _IFH = value
        End Set
    End Property

    Private _SubType As Integer
    Public Property SubType() As Integer Implements IFileImport.SubType
        Get
            Return _SubType
        End Get
        Set(ByVal value As Integer)
            _SubType = value
        End Set
    End Property

    Private _FileDialogTitle As String
    Public Property FileDialogTitle() As String
        Get
            Return _FileDialogTitle
        End Get
        Set(ByVal value As String)
            _FileDialogTitle = value
        End Set
    End Property

    Private _FileDialogFilter As String
    Public Property FileDialogFilter() As String
        Get
            Return _FileDialogFilter
        End Get
        Set(ByVal value As String)
            _FileDialogFilter = value
        End Set
    End Property

    Private _MultiSelectFiles As Boolean
    Public Property MultiSelectFiles() As Boolean Implements IFileImport.MultiSelectFiles
        Get
            Return _MultiSelectFiles
        End Get
        Set(ByVal value As Boolean)
            _MultiSelectFiles = value
            ' Auto-switch the default titles (singe file / multiple files)
            If value AndAlso (IsNothing(FileDialogTitle) OrElse FileDialogTitle = My.Resources.MyStrings.importOpenFileTitle) Then
                FileDialogTitle = My.Resources.MyStrings.importOpenMultipleFilesTitel
            ElseIf Not value AndAlso (IsNothing(FileDialogTitle) OrElse FileDialogTitle = My.Resources.MyStrings.importOpenMultipleFilesTitel) Then
                FileDialogTitle = My.Resources.MyStrings.importOpenFileTitle
            End If
        End Set
    End Property

    Private _MixedFileFormatsAllowed As Boolean
    Public Property MixedFileFormatsAllowed() As Boolean Implements IFileImport.MixedFileFormatsAllowed
        Get
            Return _MixedFileFormatsAllowed
        End Get
        Set(ByVal value As Boolean)
            _MixedFileFormatsAllowed = value
        End Set
    End Property

    Private _CSVAutoDetectEncoding As Boolean
    Public Property CSVAutoDetectEncoding() As Boolean
        Get
            Return _CSVAutoDetectEncoding
        End Get
        Set(ByVal value As Boolean)
            _CSVAutoDetectEncoding = value
        End Set
    End Property

    Private _CSVEncoding As Text.Encoding
    Public Property CSVEncoding() As Text.Encoding
        Get
            Return _CSVEncoding
        End Get
        Set(ByVal value As Text.Encoding)
            _CSVEncoding = value
        End Set
    End Property

    Private _CSVSkipFirstLine As Boolean
    Public Property CSVSkipFirstLine() As Boolean
        Get
            Return _CSVSkipFirstLine
        End Get
        Set(ByVal value As Boolean)
            _CSVSkipFirstLine = value
        End Set
    End Property

    Private _CSVSeparator As String
    Public Property CSVSeparator() As String
        Get
            Return _CSVSeparator
        End Get
        Set(ByVal value As String)
            _CSVSeparator = value
        End Set
    End Property

    Private _CSVTextqualifier As String
    Public Property CSVTextqualifier() As String
        Get
            Return _CSVTextqualifier
        End Get
        Set(ByVal value As String)
            _CSVTextqualifier = value
        End Set
    End Property

    Private _CSVDecimalPoint As String
    Public Property CSVDecimalPoint() As String
        Get
            Return _CSVDecimalPoint
        End Get
        Set(ByVal value As String)
            _CSVDecimalPoint = value
        End Set
    End Property

    Private _CSVDecimalSeparator As String
    Public Property CSVDecimalSeparator() As String
        Get
            Return _CSVDecimalSeparator
        End Get
        Set(ByVal value As String)
            _CSVDecimalSeparator = value
        End Set
    End Property

    Private _FileNames As String()
    Public Property FileNames As String() Implements IFileImport.FileNames
        Get
            Return _FileNames
        End Get
        Set(ByVal value As String())
            _FileNames = value
        End Set
    End Property

    Private _CheckFirstLine As Boolean
    Public Property CheckFirstLine() As Boolean Implements IFileImport.CheckFirstLine
        Get
            Return _CheckFirstLine
        End Get
        Set(ByVal value As Boolean)
            _CheckFirstLine = value
        End Set
    End Property

    Private _Content As String
    Public Property Content() As String Implements IFileImport.Content
        Get
            Return _Content
        End Get
        Set(ByVal value As String)
            _Content = value
        End Set
    End Property

    Private _AllRows As List(Of String())
    Public Property AllRows() As List(Of String()) Implements IFileImport.AllRows
        Get
            Return _AllRows
        End Get
        Set(ByVal value As List(Of String()))
            _AllRows = value
        End Set
    End Property

    Private _MaxErrors As Integer
    Public Property MaxErrors() As Integer Implements IFileImport.MaxErrors
        Get
            Return _MaxErrors
        End Get
        Set(ByVal value As Integer)
            _MaxErrors = value
        End Set
    End Property

    Private _ErrorCounter As Integer
    Public Property ErrorCounter() As Integer
        Get
            Return _ErrorCounter
        End Get
        Set(ByVal value As Integer)
            _ErrorCounter = value
        End Set
    End Property

    Private _ReadImportdataPercentage As Integer
    Public Property ReadImportdataPercentage() As Integer Implements IFileImport.ReadImportdataPercentage
        Get
            Return _ReadImportdataPercentage
        End Get
        Set(ByVal value As Integer)
            _ReadImportdataPercentage = value
        End Set
    End Property

    Private _CSV As CSVHelper
    Public Property CSV() As CSVHelper Implements IFileImport.CSV
        Get
            Return _CSV
        End Get
        Set(ByVal value As CSVHelper)
            _CSV = value
        End Set
    End Property

    Private _Platform As PlatformManager.Platforms
    Public Property Platform() As PlatformManager.Platforms Implements IFileImport.Platform
        Get
            Return _Platform
        End Get
        Set(ByVal value As PlatformManager.Platforms)
            _Platform = value
            PlatformName = PlatformManager.PlatformDetailsByID(Platform).Name
        End Set
    End Property

    Private _PlatformName As String
    Public Property PlatformName As String Implements IFileImport.PlatformName
        Get
            Return _PlatformName
        End Get
        Set(value As String)
            _PlatformName = value
        End Set
    End Property

    ''' <summary>
    ''' Imports the given platform data into the database
    ''' </summary>
    Friend MustOverride Function ImportContent() As Boolean Implements IFileImport.ImportContent

    ''' <summary>
    ''' Initiates the import process for this platform.
    ''' If no content is defined, an Open File Dialog is presented. Otherwise the given content is imported directly.
    ''' </summary>
    Friend Function PerformImport() As Boolean Implements IFileImport.PerformImport
        If (Content IsNot Nothing AndAlso Content.Length > 0) OrElse (AllRows IsNot Nothing AndAlso AllRows.Count > 0) Then
            Return ImportContent()
        Else
            If OpenFile() Then
                If Not (PlatformManager.PlatformDetailsByID(Platform).ImportDistinct OrElse MainImportObject.SilentMode) Then
                    ' this could be an import for a different platform, so present the platform choice form
                    With New frmSelectPlatform
                        .Platform = Platform
                        .Declaration = My.Resources.MyStrings.platformSelectImport
                        .ShowDialog()
                        If .Platform >= 0 Then
                            Platform = .Platform
                            MainImportObject.Plattform = .Platform
                        Else
                            Return False
                        End If
                    End With
                End If
                PreImportUserAdvice()
                Return ImportContent()
            Else
                Return False
            End If
        End If
    End Function

    ''' <summary>
    ''' Default function for opening transaction data files - ususally overwritten by derived platform specific classes.
    ''' Sets FileName and reads all file content into AllRows array
    ''' </summary>
    ''' <returns>true, if files have been opened, false otherwise</returns>
    Protected Overridable Function OpenFile() As Boolean
        Dim Result As Boolean = True
        If FileNames.Length = 0 OrElse FileNames(0) Is Nothing OrElse FileNames(0).Length = 0 Then
            ' no file name given yet
            Dim OFD As New OpenFileDialog()
            With OFD
                .Filter = FileDialogFilter
                .FilterIndex = 1
                .Title = FileDialogTitle
                .RestoreDirectory = True
                .Multiselect = MultiSelectFiles
                If .ShowDialog() = DialogResult.OK Then
                    FileNames = .FileNames
                End If
            End With

        End If
        If FileNames.Length > 0 AndAlso FileNames(0) IsNot Nothing AndAlso FileNames(0).Length > 0 Then
            AllRows = New List(Of String())
            Dim ThisFileName As String = ""
            Dim FirstlinesSame As Boolean = True
            Dim PreviousFirstline As String = ""
            Try
                If ImportFileHelper Is Nothing Then ImportFileHelper = New ImportFileHelper()
                ImportFileHelper.InteractiveMode = Not MainImportObject.SilentMode
                ' Read complete content into AllRows
                For Each ThisFileName In FileNames
                    If CSVAutoDetectEncoding Then
                        CSV = New CSVHelper(ThisFileName, True)
                    Else
                        CSV = New CSVHelper(ThisFileName, CSVEncoding)
                    End If
                    CSV.SetCsvContentAnalyser(AddressOf AnalyseCsvLines)
                    If CheckFirstLine Then
                        ' Check if first line matches to platform
                        If ImportFileHelper.FindMachtingPlatforms(CSV.FirstLine, Platform) = 0 Then
                            ' File has wrong format!
                            Result = False
                            ImportFileHelper.InvalidFileMessage(ThisFileName)
                            Exit For
                        Else
                            SubType = ImportFileHelper.MatchingPlatforms(0).SubType
                        End If
                    End If

                    If CSV.ReadAllRows(CSVSkipFirstLine, CSVSeparator, CSVTextqualifier, CSVDecimalPoint, CSVDecimalSeparator) Then
                        ' check if first lines match
                        If Not MixedFileFormatsAllowed Then
                            If PreviousFirstline.Length > 0 Then
                                FirstlinesSame = FirstlinesSame And (PreviousFirstline = CSV.FirstLine)
                                If Not FirstlinesSame Then
                                    Throw New ImportFileException(My.Resources.MyStrings.importMsgMixedFileFormats)
                                End If
                            End If
                            PreviousFirstline = CSV.FirstLine
                        End If
                        AllRows.AddRange(CSV.Rows)
                    Else
                        Result = False
                        Exit For
                    End If
                Next
            Catch ex As ImportFileException
                Result = False
                If MainImportObject.SilentMode Then
                    Throw New ImportFileException(ex.Message)
                Else
                    DefaultErrorHandler(ex, ex.Message, False, My.Resources.MyStrings.importMsgMixedFileFormatsTitle)
                End If
            Catch ex As Exception
                Result = False
                Dim ErrorMessage As String = String.Format(My.Resources.MyStrings.importOpenFileError,
                                                      ThisFileName,
                                                      ex.Message)
                If MainImportObject.SilentMode Then
                    Throw New ImportFileException(ErrorMessage, ex)
                Else
                    DefaultErrorHandler(ex, ErrorMessage)
                End If
            End Try
        Else
            Result = False
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Show a platform specific user advice before performing the actual import
    ''' </summary>
    Protected Overridable Sub PreImportUserAdvice() Implements IFileImport.PreImportUserAdvice
        ' nothing to do here in base...
    End Sub

    ''' <summary>
    ''' You could place some code for analysing the CSV file content here. If needed, this must be overwritten in derived class
    ''' </summary>
    ''' <param name="Lines">Array holding all lines from the csv file</param>
    Friend Overridable Sub AnalyseCsvLines(ByRef Lines As String())
        ' nothing to do here in base...
    End Sub

    ''' <summary>
    ''' Initializes the progress form for this import
    ''' </summary>
    ''' <param name="Message">Initial message to be displayed</param>
    Protected Sub InitProgressForm(Optional Message As String = "") Implements IFileImport.InitProgressForm
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
    ''' Update the progress form by displaying the current line number in a progress bar
    ''' </summary>
    ''' <param name="AllLines">Total number of all lines in file</param>
    ''' <param name="Line">Current line number</param>
    ''' <param name="Message">Message to display. Defaults to MyStrings.importMsgReadingFile</param>
    Protected Sub UpdateProgress(ByVal AllLines As Long,
                                 ByVal Line As Long,
                                 Optional Message As String = "")
        If Message = "" Then
            Message = My.Resources.MyStrings.importMsgReadingFile
        End If
        ProgressWaitManager.UpdateProgress(Line / AllLines * ReadImportdataPercentage, String.Format(Message, Line.ToString(Import.MESSAGENUMBERFORMAT), AllLines.ToString(Import.MESSAGENUMBERFORMAT)))
    End Sub

    ''' <summary>
    ''' Clear the progress form again
    ''' </summary>
    Protected Sub DestroyProgressForm() Implements IFileImport.DestroyProgressForm
        Try
            ProgressWaitManager.CloseProgress()
        Catch ex As Exception
            ' no matter...
        End Try
    End Sub

    ''' <summary>
    ''' Default error handler for errors occuring while reading an import file. Displays error messages if necessary 
    ''' and destroys the progress form if ErrorCounter = 0
    ''' </summary>
    ''' <param name="ErrorCounter">Current error counter</param>
    ''' <param name="Line">Line number in which the error occured</param>
    ''' <param name="ex">Exception (message will be displayed)</param>
    ''' <returns></returns>
    Protected Friend Function FileImportError(ByRef ErrorCounter As Long,
                                       ByVal Line As Long,
                                       ByRef ex As Exception) As Long Implements IFileImport.FileImportError
        Cursor.Current = Cursors.Default
        ErrorCounter -= 1
        Dim ErrorMessage As String = String.Format(My.Resources.MyStrings.importMsgInvalidDataInLine,
                                                       Environment.NewLine,
                                                       Line.ToString(Import.MESSAGENUMBERFORMAT),
                                                       ex.Message)
        If ErrorCounter = 0 Then
            ErrorMessage &= Environment.NewLine & Environment.NewLine & My.Resources.MyStrings.importMsgInvalidDataAborting
            DestroyProgressForm()
            If MainImportObject.SilentMode Then
                Throw New TradeDataImportException(ErrorMessage)
            End If
        End If
        If Not MainImportObject.SilentMode Then
            DefaultErrorHandler(ex, ErrorMessage)
        End If
        Return ErrorCounter
    End Function

#If CONFIG = "Debug" Then
    ''' <summary>
    ''' For debugging: Write AllRows to debug window...
    ''' </summary>
    Friend Sub DeserializeAllRows()
        Dim Result As String = ""
        For i As Integer = 0 To AllRows.Count - 1
            Result &= String.Join(";", AllRows(i)) & Environment.NewLine
        Next
        Debug.Print(Result)
    End Sub
#End If
End Class

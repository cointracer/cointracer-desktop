﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Dieser Code wurde von einem Tool generiert.
'     Laufzeitversion:4.0.30319.42000
'
'     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
'     der Code erneut generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "Funktion zum automatischen Speichern von My.Settings"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property LastImportMethod() As Integer
            Get
                Return CType(Me("LastImportMethod"),Integer)
            End Get
            Set
                Me("LastImportMethod") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property ReportDetail1() As String
            Get
                Return CType(Me("ReportDetail1"),String)
            End Get
            Set
                Me("ReportDetail1") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property CoinValueStrategies() As String
            Get
                Return CType(Me("CoinValueStrategies"),String)
            End Get
            Set
                Me("CoinValueStrategies") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property OfflineMode() As Boolean
            Get
                Return CType(Me("OfflineMode"),Boolean)
            End Get
            Set
                Me("OfflineMode") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property CheckVersionOnStart() As Boolean
            Get
                Return CType(Me("CheckVersionOnStart"),Boolean)
            End Get
            Set
                Me("CheckVersionOnStart") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property CheckFiatCoursesOnStart() As Boolean
            Get
                Return CType(Me("CheckFiatCoursesOnStart"),Boolean)
            End Get
            Set
                Me("CheckFiatCoursesOnStart") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property CheckCoinCoursesOnStart() As Boolean
            Get
                Return CType(Me("CheckCoinCoursesOnStart"),Boolean)
            End Get
            Set
                Me("CheckCoinCoursesOnStart") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property LastCheckFiatCourses() As String
            Get
                Return CType(Me("LastCheckFiatCourses"),String)
            End Get
            Set
                Me("LastCheckFiatCourses") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property LastCheckVersion() As String
            Get
                Return CType(Me("LastCheckVersion"),String)
            End Get
            Set
                Me("LastCheckVersion") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property LastCheckCoinCourses() As String
            Get
                Return CType(Me("LastCheckCoinCourses"),String)
            End Get
            Set
                Me("LastCheckCoinCourses") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property LastCvsScenarioID() As Integer
            Get
                Return CType(Me("LastCvsScenarioID"),Integer)
            End Get
            Set
                Me("LastCvsScenarioID") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property UserName() As String
            Get
                Return CType(Me("UserName"),String)
            End Get
            Set
                Me("UserName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property TaxNumber() As String
            Get
                Return CType(Me("TaxNumber"),String)
            End Get
            Set
                Me("TaxNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property UseProxy() As Boolean
            Get
                Return CType(Me("UseProxy"),Boolean)
            End Get
            Set
                Me("UseProxy") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property ImportSettingKraken() As Integer
            Get
                Return CType(Me("ImportSettingKraken"),Integer)
            End Get
            Set
                Me("ImportSettingKraken") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property ReportLastRenderingExtension() As Integer
            Get
                Return CType(Me("ReportLastRenderingExtension"),Integer)
            End Get
            Set
                Me("ReportLastRenderingExtension") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property DataDirectory() As String
            Get
                Return CType(Me("DataDirectory"),String)
            End Get
            Set
                Me("DataDirectory") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property AskForApiProtection() As Boolean
            Get
                Return CType(Me("AskForApiProtection"),Boolean)
            End Get
            Set
                Me("AskForApiProtection") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property MessageBoxSettings() As String
            Get
                Return CType(Me("MessageBoxSettings"),String)
            End Get
            Set
                Me("MessageBoxSettings") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property ReportComment() As String
            Get
                Return CType(Me("ReportComment"),String)
            End Get
            Set
                Me("ReportComment") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property ReportDetail2() As String
            Get
                Return CType(Me("ReportDetail2"),String)
            End Get
            Set
                Me("ReportDetail2") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property ImportSettingTimeDiffVircurex() As Integer
            Get
                Return CType(Me("ImportSettingTimeDiffVircurex"),Integer)
            End Get
            Set
                Me("ImportSettingTimeDiffVircurex") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property ReportLastPlatforms() As String
            Get
                Return CType(Me("ReportLastPlatforms"),String)
            End Get
            Set
                Me("ReportLastPlatforms") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("EUR")>  _
        Public Property InventoryPricesCurrency() As String
            Get
                Return CType(Me("InventoryPricesCurrency"),String)
            End Get
            Set
                Me("InventoryPricesCurrency") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("613")>  _
        Public Property Layout_SplitterDistance1() As Long
            Get
                Return CType(Me("Layout_SplitterDistance1"),Long)
            End Get
            Set
                Me("Layout_SplitterDistance1") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("159")>  _
        Public Property Layout_SplitterDistance2() As Long
            Get
                Return CType(Me("Layout_SplitterDistance2"),Long)
            End Get
            Set
                Me("Layout_SplitterDistance2") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property LogLevel() As Integer
            Get
                Return CType(Me("LogLevel"),Integer)
            End Get
            Set
                Me("LogLevel") = value
            End Set
        End Property

        <Global.System.Configuration.UserScopedSettingAttribute(),
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),
         Global.System.Configuration.DefaultSettingValueAttribute("de-DE")>
        Public Property CurrentCulture() As String
            Get
                Return CType(Me("CurrentCulture"),String)
            End Get
            Set
                Me("CurrentCulture") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property TransferDetection() As String
            Get
                Return CType(Me("TransferDetection"),String)
            End Get
            Set
                Me("TransferDetection") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property TableExportSettings() As String
            Get
                Return CType(Me("TableExportSettings"),String)
            End Get
            Set
                Me("TableExportSettings") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property LastSettingsCategory() As Integer
            Get
                Return CType(Me("LastSettingsCategory"),Integer)
            End Get
            Set
                Me("LastSettingsCategory") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property RecentFilesList() As String
            Get
                Return CType(Me("RecentFilesList"),String)
            End Get
            Set
                Me("RecentFilesList") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.CoinTracer.My.MySettings
            Get
                Return Global.CoinTracer.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace

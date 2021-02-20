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

Imports System.Security.Permissions
Imports System.Configuration
Imports System.Collections.Specialized
Imports System.IO
Imports System.Xml
Imports Microsoft.Win32

<PermissionSetAttribute(SecurityAction.InheritanceDemand, Name:="FullTrust")> _
<PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")> _
Public Class FlexFileSettingsProvider
    Inherits SettingsProvider

    Private Shared _SettingsRoot As String
    Private Shared _UserScopeNode As String
    Private Shared _ApplicationScopeNode As String
    Private Shared _RegistryKey As String
    Private Shared _SettingsXML As Xml.XmlDocument


    Private Shared _UserConfigFile As String
    ''' <summary>
    ''' File name of the specified user config file
    ''' </summary>
    Public Property UserConfigFile() As String
        Get
            Return _UserConfigFile
        End Get
        Set(ByVal value As String)
            If value <> _UserConfigFile Then
                _UserConfigFile = value
                SetUserConfigFileToRegistry()
            End If
        End Set
    End Property

    Private ReadOnly Property SettingsXML() As Xml.XmlDocument
        Get
            'If we dont hold an xml document, try opening one.  
            'If it doesnt exist then create a new one ready.
            If _SettingsXML Is Nothing Then
                _SettingsXML = New Xml.XmlDocument

                Try
                    _SettingsXML.Load(UserConfigFile)
                Catch ex As Exception
                    'Create new document
                    Dim dec As XmlDeclaration = _SettingsXML.CreateXmlDeclaration("1.0", "utf-8", String.Empty)
                    _SettingsXML.AppendChild(dec)

                    Dim nodeRoot As XmlNode

                    nodeRoot = _SettingsXML.CreateNode(XmlNodeType.Element, _SettingsRoot, "")
                    _SettingsXML.AppendChild(nodeRoot)
                End Try
            End If

            Return _SettingsXML
        End Get
    End Property

    Public Overrides Property ApplicationName() As String
        Get
            ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
        End Get
        Set(ByVal value As String)
            ' Do nothing.
        End Set
    End Property


    Public Overrides Sub SetPropertyValues(ByVal context As SettingsContext, ByVal PropertyValues As SettingsPropertyValueCollection)
        ' Iterate through the settings to be stored
        ' Only dirty settings are included in PropertyValues, and only ones relevant to this provider
        For Each propval As SettingsPropertyValue In PropertyValues
            SetValue(propval)
        Next
        Try
            SetUserConfigFileToRegistry()
            SettingsXML.Save(UserConfigFile)
        Catch ex As Exception
            ' Ignore if cant save, device been ejected
        End Try
    End Sub

    Public Overrides Function GetPropertyValues(ByVal context As SettingsContext, ByVal Properties As SettingsPropertyCollection) As SettingsPropertyValueCollection
        'Create new collection of values
        Dim values As SettingsPropertyValueCollection = New SettingsPropertyValueCollection()

        'Iterate through the settings to be retrieved
        For Each setting As SettingsProperty In Properties
            Dim value As SettingsPropertyValue = New SettingsPropertyValue(setting)
            value.IsDirty = False
            value.SerializedValue = GetValue(setting)
            values.Add(value)
        Next
        Return values
    End Function

    Public Function GetUserConfigFileName() As String
        Return Application.ProductName.ToLower & "_user.config"
    End Function

    ''' <summary>
    ''' Changes the location of the settings file. Creates a backup at new location, if needed
    ''' </summary>
    ''' <param name="NewSettingsDirectory">New Directory for config file - has to be writable!</param>
    Public Sub ChangeSettingsDirectory(ByVal NewSettingsDirectory As String)
        If Path.GetDirectoryName(NewSettingsDirectory) <> Path.GetDirectoryName(UserConfigFile) Then
            Dim NewFile As String = Path.Combine(NewSettingsDirectory, GetUserConfigFileName)
            If File.Exists(NewFile) Then
                ' new file already exits - copy it to *.bak
                Try
                    File.Copy(NewFile, NewFile & ".bak", True)
                Catch ex As Exception
                    ' keep calm...
                End Try
            End If
            UserConfigFile = NewFile
        End If
    End Sub

    ''' <summary>
    ''' Reads the registry key HKCU\Software\IT Nebinger\CoinTracer\global\ConfigFile and sets it's value as UserConfigFile
    ''' </summary>
    Private Sub GetUserConfigFileFromRegistry()
        Dim ConfigFile As String = ""
        Dim RegKey As RegistryKey = Registry.CurrentUser.OpenSubKey(_RegistryKey)
        If RegKey IsNot Nothing Then
            ConfigFile = RegKey.GetValue("ConfigFileLocation", "")
        End If
        If ConfigFile.Length > 0 Then
            _UserConfigFile = ConfigFile
        End If
    End Sub

    ''' <summary>
    ''' Writes the UserConfigFile into the registry key HKCU\Software\IT Nebinger\CoinTracer\global\ConfigFile 
    ''' </summary>
    Private Sub SetUserConfigFileToRegistry()
        Dim RegKey As RegistryKey
        RegKey = Registry.CurrentUser.OpenSubKey(_RegistryKey, True)
        If RegKey Is Nothing Then
            Dim SubKeys() As String = Split(_RegistryKey, "\")
            Dim KeyPath As String = ""
            For i As Integer = 0 To SubKeys.Length - 2
                KeyPath &= If(KeyPath.Length = 0, "", "\") & SubKeys(i)
                RegKey = Registry.CurrentUser.OpenSubKey(KeyPath, True)
                If RegKey IsNot Nothing Then
                    RegKey.CreateSubKey(SubKeys(i + 1))
                    RegKey.Close()
                End If
            Next
            RegKey = Registry.CurrentUser.OpenSubKey(_RegistryKey, True)
        End If
        RegKey.SetValue("ConfigFileLocation", _UserConfigFile)
    End Sub

    Public Sub New()
        _SettingsRoot = ApplicationName & "_Settings"
        _UserScopeNode = "UserSettings"
        _ApplicationScopeNode = "AppSettings"
        _RegistryKey = String.Format("Software\IT Nebinger\{0}\Global", Application.ProductName)
        ' reminder: don't set default user config file!
    End Sub

    Public Overrides Sub Initialize(ByVal name As String, ByVal values As NameValueCollection)
        GetUserConfigFileFromRegistry()
        MyBase.Initialize(Me.ApplicationName, values)
    End Sub

    Private Function GetValue(ByVal Setting As SettingsProperty) As String
        Dim ReturnValue As String = ""

        Try
            ReturnValue = SettingsXML.SelectSingleNode(_SettingsRoot & "/" & _
                                               If(IsUser(Setting), _UserScopeNode, _ApplicationScopeNode) & "/" & _
                                               Setting.Name).InnerText
        Catch ex As Exception
            If Not Setting.DefaultValue Is Nothing Then
                ReturnValue = Setting.DefaultValue.ToString
            Else
                ReturnValue = ""
            End If
        End Try

        Return ReturnValue
    End Function

    Private Sub SetValue(ByVal PropertyValue As SettingsPropertyValue)

        Dim MachineNode As Xml.XmlElement
        Dim SettingNode As Xml.XmlElement
        Dim SubScope As String = If(IsUser(PropertyValue.Property), _UserScopeNode, _ApplicationScopeNode)

        Try
            SettingNode = DirectCast(SettingsXML.SelectSingleNode(_SettingsRoot & "/" & _
                                                                  SubScope & "/" & _
                                                                  PropertyValue.Name), XmlElement)
        Catch ex As Exception
            SettingNode = Nothing
        End Try

        ' check to see if the node exists, if so then set it's new value
        If Not SettingNode Is Nothing Then
            SettingNode.InnerText = PropertyValue.SerializedValue.ToString
        Else
            Try
                MachineNode = DirectCast(SettingsXML.SelectSingleNode(_SettingsRoot & "/" & SubScope), XmlElement)
            Catch ex As Exception
                MachineNode = SettingsXML.CreateElement(SubScope)
                SettingsXML.SelectSingleNode(_SettingsRoot).AppendChild(MachineNode)
            End Try

            If MachineNode Is Nothing Then
                MachineNode = SettingsXML.CreateElement(SubScope)
                SettingsXML.SelectSingleNode(_SettingsRoot).AppendChild(MachineNode)
            End If

            SettingNode = SettingsXML.CreateElement(PropertyValue.Name)
            SettingNode.InnerText = PropertyValue.SerializedValue.ToString
            MachineNode.AppendChild(SettingNode)
        End If
    End Sub

    ''' <summary>
    ''' Determines wether the property has been marked for user scope
    ''' </summary>
    Private Function IsUser(ByVal [Property] As SettingsProperty) As Boolean
        For Each d As DictionaryEntry In [Property].Attributes
            Dim a As Attribute = DirectCast(d.Value, Attribute)
            If TypeOf a Is System.Configuration.UserScopedSettingAttribute Then
                Return True
            End If
        Next
        Return False
    End Function


End Class

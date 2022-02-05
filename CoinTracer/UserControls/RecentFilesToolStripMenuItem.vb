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


Imports System.ComponentModel
Imports CoinTracer.My.Resources

Public Class RecentFilesToolStripMenuItem
    Inherits ToolStripMenuItem

    Public Event FileEntryClicked(ByVal Filename As String)

    <Browsable(True)>
    <EditorBrowsable(True)>
    <Category("Behavior")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <DefaultValue("RecentFilesList")>
    <Description("Name des Settings-Items, in dem die Dateiliste gespeichert wird.")>
    Private _SettingsItemName As String
    Public Property SettingsItemName() As String
        Get
            Return _SettingsItemName
        End Get
        Set(ByVal value As String)
            _SettingsItemName = value
        End Set
    End Property

    <Browsable(True)>
    <EditorBrowsable(True)>
    <Category("Behavior")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    <DefaultValue("10")>
    <Description("Maximale Anzahl von Dateinamen, die aufgelistet werden.")>
    Private _MaxItems As Integer
    Public Property MaxItems() As Integer
        Get
            Return _MaxItems
        End Get
        Set(ByVal value As Integer)
            _MaxItems = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        _SettingsItemName = "RecentFilesList"
        _MaxItems = 10
        _cmnRecentFiles = New ContextMenuStrip()
        DropDown = _cmnRecentFiles
    End Sub

    Private _cmnRecentFiles As ContextMenuStrip

    ''' <summary>
    ''' Pushes a file onto the stack of recently used files
    ''' </summary>
    ''' <param name="Filename">Fully qualified name of file to add</param>
    Public Sub AddFile(Filename As String)
        With My.Settings
            If .Item(_SettingsItemName).ToString.Length = 0 Then
                .Item(_SettingsItemName) = Filename
            Else
                ' Check if file is already in list
                If Not .Item(_SettingsItemName).ToString.Contains(Filename) Then
                    ' Add file
                    Dim FilesList As String() = (Filename & "|" & .Item(_SettingsItemName).ToString).Split("|"c)
                    If FilesList.Length > _MaxItems Then
                        Array.Resize(FilesList, _MaxItems)
                    End If
                    .Item(_SettingsItemName) = String.Join("|", FilesList)
                End If
            End If
        End With
        UpdateMenuEntries()
    End Sub

    ''' <summary>
    ''' Update the menu entries for the recent files list
    ''' </summary>
    Public Sub UpdateMenuEntries()
        Dim FilesList As String() = My.Settings.RecentFilesList.Split("|"c)
        With _cmnRecentFiles
            .Items.Clear()
            If FilesList?.Length > 0 AndAlso FilesList(0).Length > 0 Then
                ' (re)build menu entries
                Visible = True
                For i = 0 To FilesList.Length - 1
                    .Items.Add(i + 1 & ": " & FilesList(i), Nothing, New EventHandler(AddressOf RecentFilesClickHandler))
                    With .Items.Item(.Items.Count - 1)
                        .Tag = FilesList(i)
                    End With
                Next
                ' Add separator and delete entry
                .Items.Add(New ToolStripSeparator)
                .Items.Add(MyStrings.mnuItmClearRecentDbFiles, ct_erase, New EventHandler(AddressOf ClearRecentFilesClickHandler))
            Else
                ' Disable menu entry
                Visible = False
            End If
        End With
    End Sub

    ''' <summary>
    ''' Handles Click on one of the recent database files menu entries.
    ''' Loads the corresponding database file.
    ''' </summary>
    Private Sub RecentFilesClickHandler(sender As Object, e As EventArgs)
        Dim Item As ToolStripDropDownItem = TryCast(sender, ToolStripDropDownItem)
        If Item?.Tag?.ToString.Length > 0 Then
            RaiseEvent FileEntryClicked(Item.Tag)
        End If
    End Sub

    ''' <summary>
    ''' Clears the recent files list
    ''' </summary>
    Private Sub ClearRecentFilesClickHandler(sender As Object, e As EventArgs)
        My.Settings.Item(_SettingsItemName) = String.Empty
        UpdateMenuEntries()
    End Sub

End Class

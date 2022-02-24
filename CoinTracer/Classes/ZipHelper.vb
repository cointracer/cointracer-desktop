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

Imports System.IO

Public Class ZipHelper

    ''' <summary>
    ''' All valid file endings for transaction data files
    ''' </summary>
    Shared Function GetValidTransactionFilesEndings() As String
        ' must be lower case!
        Return ".csv;.xls;.xlsx;"
    End Function

    ''' <summary>
    ''' Extracts all files using the file name filter from a zip file and
    ''' returns the list of extracted files.
    ''' </summary>
    ''' <param name="ZipFileName">Full path and name of zip file to extract</param>
    ''' <param name="FileFilter">Valid file types, separated by ';'. List must end with an semicolon!</param>
    ''' <param name="TempFilesPrefix">If given, this variable will be set to the prefix of all extracted files. Can be used so erase these later.</param>
    ''' <returns></returns>
    Shared Function ExtractDataFiles(ByRef ZipFileName As String,
                                     ByRef FileFilter As String,
                                     Optional ByRef TempFilesPrefix As String = Nothing) As List(Of String)

        Dim ZipFile = New ICSharpCode.SharpZipLib.Zip.ZipFile(ZipFileName)
        Dim OutFile As String
        Dim Prefix = "ct_tmp_" & MD5FromString(Guid.NewGuid().ToString).Substring(0, 5) & "_"
        Dim FileList As New List(Of String)
        For Each Entry As ICSharpCode.SharpZipLib.Zip.ZipEntry In ZipFile
            If Entry.IsFile AndAlso FileFilter.Contains(Path.GetExtension(Entry.Name).ToLower & ";") Then
                Using ZipStream As New BinaryReader(ZipFile.GetInputStream(Entry))
                    OutFile = Path.GetDirectoryName(ZipFileName) & Path.DirectorySeparatorChar & Prefix & Path.GetFileName(Entry.Name)
                    Using OutStream As New BinaryWriter(File.Create(OutFile))
                        ZipStream.BaseStream.CopyTo(OutStream.BaseStream)
                        FileList.Add(OutFile)
                    End Using
                End Using
            End If
        Next
        TempFilesPrefix = Path.GetDirectoryName(ZipFileName) & Path.DirectorySeparatorChar & Prefix
        Return FileList
    End Function

End Class

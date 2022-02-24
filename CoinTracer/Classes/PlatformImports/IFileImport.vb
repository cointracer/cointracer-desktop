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

Friend Interface IFileImport
    Property MainImportObject As Import
    Property ImportFileHelper As ImportFileHelper
    Property Platform As PlatformManager.Platforms
    Property PlatformName As String
    Property SubType As Integer
    ReadOnly Property PlatformHeaders As ImportFileHelper.MatchingPlatform()
    Property FileNames As String()
    Property CheckFirstLine As Boolean
    Property CSV() As IDataFileHelper
    Property MultiSelectFiles As Boolean
    Property MixedFileFormatsAllowed As Boolean
    Property Content As String
    Property AllRows As List(Of String())
    Property MaxErrors As Integer
    Property ReadImportdataPercentage As Integer
    Function ImportContent() As Boolean
    Function PerformImport() As Boolean
    Sub PreImportUserAdvice()
    Sub InitProgressForm(Optional Message As String = "")
    Sub DestroyProgressForm()
    Function FileImportError(ByRef ErrorCounter As Long,
                             ByVal Line As Long,
                             ByRef ex As Exception) As Long
End Interface

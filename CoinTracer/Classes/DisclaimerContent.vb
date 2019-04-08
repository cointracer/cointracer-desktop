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
'  * https://joinup.ec.europa.eu/release/eupl/v12  (or within the file "License.txt", which is part of this project)
'  
'  * Unless required by applicable law or agreed to in writing, software distributed under the Licence is
'    distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  * See the Licence for the specific language governing permissions and limitations under the Licence.
'  *
'  **************************************

Friend Module DisclaimerContent

    Friend Function NotErrorFreeNoWarranty() As String
        Return "Bitte beachten Sie auch, dass sich diese Software noch im Entwicklungsstadium befindet. Sie wurde zwar nach bestem Wissen und Gewissen entwickelt, aber dennoch: " & _
            "Fehler können nicht ausgeschlossen werden - sie sind im Gegenteil " & _
            "sogar wahrscheinlich. Sie verwenden diese Software auf eigenes Risiko, für finanzielle und sonstige Schäden, die sich aus dem " & _
            "Gebrauch ergeben, kann der Autor keine Haftung übernehmen. Darüber hinaus wird der " & Application.ProductName & " so wie er ist kostenfrei zur Verfügung " & _
            "gestellt. Ein Anspruch auf Fehlerkorrekturen, Gewährleistung oder Weiterentwicklung ist nicht gegeben."
    End Function

    Friend Function NoWarrantyLine() As String
        Return "Achtung: Keine Gewähr!"
    End Function

    Friend Function TaxingUnclear() As String
        Return "Die Besteuerung von Gewinnen aus dem Handel mit Bitcoins und anderen Cryptocoins ist immer noch Neuland, zum aktuellen Zeitpunkt " &
               "(April 2019) gibt es, abgesehen von der Vorgabe, generell das FiFo-Verfahren anzuwenden, z.T. noch keinen Konsens über allgemein verbindliche Regelungen. Der " & Application.ProductName & " ist ein Tool zum Erstellen " &
               "von Gewinn-/Verlust-Auswertungen für den privaten (!) Handel mit Coins und betrachtet diesen aus der Perspektive privater Veräußerungsgeschäfte. " &
               "Der " & Application.ProductName & " erlaubt eine Vielzahl von " &
               "Einstellungsmöglichkeiten und Berechnungsverfahren - zunächst einmal unabhängig davon, ob diese zu einem zukünftigen " &
               "Zeitpunkt als zulässig oder unzulässig erklärt werden!" & Environment.NewLine & Environment.NewLine &
               "Konsultieren Sie daher bei konkreten steuerlichen und/oder Rechtsfragen immer Ihren Steuerberater! " & Application.ProductName &
               " ist als private Initiative des Autors mit dem Zweck entstanden, tabellarische Zusammenstellungen von Handelsaktivitäten einfach und " &
               "automatisiert erzeugen zu können. Die Applikation ist keine geprüfte Steuersoftware!"
    End Function

    Friend Function CompleteDisclaimer() As String
        Return NoWarrantyLine() & Environment.NewLine & Environment.NewLine &
            TaxingUnclear() & Environment.NewLine & Environment.NewLine &
            NotErrorFreeNoWarranty()
    End Function

End Module

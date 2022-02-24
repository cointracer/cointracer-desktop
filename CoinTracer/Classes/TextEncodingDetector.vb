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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO

Public NotInheritable Class TextEncodingDetector

    Private Sub New()
    End Sub


    ' Einzulesende Menge Bytes, um das Encoding heuristisch zu ermitteln (beliebig gewählt)
    Const _defaultHeuristicSampleSize As Long = 2047

    Public Shared Function DetectTextFileEncoding(InputFilename As String) As Encoding
        Using textfileStream As FileStream = File.OpenRead(InputFilename)
            Return DetectTextFileEncoding(textfileStream, _defaultHeuristicSampleSize)
        End Using
    End Function

    Public Shared Function DetectTextFileEncoding(InputFileStream As FileStream, _
                                                  Optional HeuristicSampleSize As Long = -1) As Encoding
        Dim uselessBool As Boolean = False
        If HeuristicSampleSize = -1 Then HeuristicSampleSize = _defaultHeuristicSampleSize
        Return DetectTextFileEncoding(InputFileStream, _defaultHeuristicSampleSize, uselessBool)
    End Function

    Public Shared Function DetectTextFileEncoding(InputFileStream As FileStream, _
                                                  Optional HeuristicSampleSize As Long = -1, _
                                                  Optional ByRef HasBOM As Boolean = False) As Encoding
        If InputFileStream Is Nothing Then
            Throw New ArgumentNullException("Kein gültiger Filestream übergeben!", "InputFileStream")
        End If

        If Not InputFileStream.CanRead Then
            Throw New ArgumentException("Übergebener Filestream kann nicht gelesen werden!", "InputFileStream")
        End If

        If Not InputFileStream.CanSeek Then
            Throw New ArgumentException("In übergebenem Filestream kann nicht gesucht werden!", "InputFileStream")
        End If

        Dim encodingFound As Encoding = Nothing

        Dim originalPos As Long = InputFileStream.Position

        InputFileStream.Position = 0

        If HeuristicSampleSize = -1 Then HeuristicSampleSize = _defaultHeuristicSampleSize

        'First read only what we need for BOM detection
        Dim bomBytes As Byte() = New Byte(If(InputFileStream.Length > 4, 4, InputFileStream.Length) - 1) {}
        InputFileStream.Read(bomBytes, 0, bomBytes.Length)

        encodingFound = DetectBOMBytes(bomBytes)

        If encodingFound IsNot Nothing Then
            InputFileStream.Position = originalPos
            HasBOM = True
            Return encodingFound
        End If


        'BOM Detection failed, going for heuristics now.
        ' create sample byte array and populate it
        Dim sampleBytes As Byte() = New Byte(If(HeuristicSampleSize > InputFileStream.Length, InputFileStream.Length, HeuristicSampleSize) - 1) {}
        Array.Copy(bomBytes, sampleBytes, bomBytes.Length)
        If InputFileStream.Length > bomBytes.Length Then
            InputFileStream.Read(sampleBytes, bomBytes.Length, sampleBytes.Length - bomBytes.Length)
        End If
        InputFileStream.Position = originalPos

        'test byte array content
        encodingFound = DetectUnicodeInByteSampleByHeuristics(sampleBytes)

        HasBOM = False
        Return encodingFound
    End Function

    Public Shared Function DetectTextByteArrayEncoding(TextData As Byte()) As Encoding
        Dim uselessBool As Boolean = False
        Return DetectTextByteArrayEncoding(TextData, uselessBool)
    End Function

    Public Shared Function DetectTextByteArrayEncoding(TextData As Byte(), ByRef HasBOM As Boolean) As Encoding
        If TextData Is Nothing Then
            Throw New ArgumentNullException("Kein gültiges Text-Byte-Array übergeben!", "TextData")
        End If

        Dim encodingFound As Encoding = Nothing

        encodingFound = DetectBOMBytes(TextData)

        If encodingFound IsNot Nothing Then
            HasBOM = True
            Return encodingFound
        Else
            'test byte array content
            encodingFound = DetectUnicodeInByteSampleByHeuristics(TextData)

            HasBOM = False
            Return encodingFound
        End If
    End Function

    Public Shared Function GetStringFromByteArray(TextData As Byte(), DefaultEncoding As Encoding) As String
        Return GetStringFromByteArray(TextData, DefaultEncoding, _defaultHeuristicSampleSize)
    End Function

    Public Shared Function GetStringFromByteArray(TextData As Byte(), DefaultEncoding As Encoding, MaxHeuristicSampleSize As Long) As String
        If TextData Is Nothing Then
            Throw New ArgumentNullException("Kein gültiges Text-Byte-Array übergeben!", "TextData")
        End If

        Dim encodingFound As Encoding = Nothing

        encodingFound = DetectBOMBytes(TextData)

        If encodingFound IsNot Nothing Then
            'For some reason, the default encodings don't detect/swallow their own preambles!!
            Return encodingFound.GetString(TextData, encodingFound.GetPreamble().Length, TextData.Length - encodingFound.GetPreamble().Length)
        Else
            Dim heuristicSample As Byte() = Nothing
            If TextData.Length > MaxHeuristicSampleSize Then
                heuristicSample = New Byte(MaxHeuristicSampleSize - 1) {}
                Array.Copy(TextData, heuristicSample, MaxHeuristicSampleSize)
            Else
                heuristicSample = TextData
            End If

            encodingFound = If(DetectUnicodeInByteSampleByHeuristics(TextData), DefaultEncoding)
            Return encodingFound.GetString(TextData)
        End If
    End Function


    Public Shared Function DetectBOMBytes(BOMBytes As Byte()) As Encoding
        If BOMBytes Is Nothing Then
            Throw New ArgumentNullException("Kein gültiges BOM-Byte-Array übergeben!", "BOMBytes")
        End If

        If BOMBytes.Length < 2 Then
            Return Nothing
        End If

        If BOMBytes(0) = &HFF AndAlso BOMBytes(1) = &HFE AndAlso (BOMBytes.Length < 4 OrElse BOMBytes(2) <> 0 OrElse BOMBytes(3) <> 0) Then
            Return Encoding.Unicode
        End If

        If BOMBytes(0) = &HFE AndAlso BOMBytes(1) = &HFF Then
            Return Encoding.BigEndianUnicode
        End If

        If BOMBytes.Length < 3 Then
            Return Nothing
        End If

        If BOMBytes(0) = &HEF AndAlso BOMBytes(1) = &HBB AndAlso BOMBytes(2) = &HBF Then
            Return Encoding.UTF8
        End If

        If BOMBytes(0) = &H2B AndAlso BOMBytes(1) = &H2F AndAlso BOMBytes(2) = &H76 Then
            Return Encoding.UTF7
        End If

        If BOMBytes.Length < 4 Then
            Return Nothing
        End If

        If BOMBytes(0) = &HFF AndAlso BOMBytes(1) = &HFE AndAlso BOMBytes(2) = 0 AndAlso BOMBytes(3) = 0 Then
            Return Encoding.UTF32
        End If

        If BOMBytes(0) = 0 AndAlso BOMBytes(1) = 0 AndAlso BOMBytes(2) = &HFE AndAlso BOMBytes(3) = &HFF Then
            Return Encoding.GetEncoding(12001)
        End If

        Return Nothing
    End Function

    Public Shared Function DetectUnicodeInByteSampleByHeuristics(SampleBytes As Byte()) As Encoding
        Dim oddBinaryNullsInSample As Long = 0
        Dim evenBinaryNullsInSample As Long = 0
        Dim suspiciousUTF8SequenceCount As Long = 0
        Dim suspiciousUTF8BytesTotal As Long = 0
        Dim likelyUSASCIIBytesInSample As Long = 0

        'Cycle through, keeping count of binary null positions, possible UTF-8 
        ' sequences from upper ranges of Windows-1252, and probable US-ASCII 
        ' character counts.

        Dim currentPos As Long = 0
        Dim skipUTF8Bytes As Integer = 0

        While currentPos < SampleBytes.Length
            'binary null distribution
            If SampleBytes(currentPos) = 0 Then
                If currentPos Mod 2 = 0 Then
                    evenBinaryNullsInSample += 1
                Else
                    oddBinaryNullsInSample += 1
                End If
            End If

            'likely US-ASCII characters
            If IsCommonUSASCIIByte(SampleBytes(currentPos)) Then
                likelyUSASCIIBytesInSample += 1
            End If

            'suspicious sequences (look like UTF-8)
            If skipUTF8Bytes = 0 Then
                Dim lengthFound As Integer = DetectSuspiciousUTF8SequenceLength(SampleBytes, currentPos)

                If lengthFound > 0 Then
                    suspiciousUTF8SequenceCount += 1
                    suspiciousUTF8BytesTotal += lengthFound
                    skipUTF8Bytes = lengthFound - 1
                End If
            Else
                skipUTF8Bytes -= 1
            End If

            currentPos += 1
        End While

        '1: UTF-16 LE - in english / european environments, this is usually characterized by a 
        ' high proportion of odd binary nulls (starting at 0), with (as this is text) a low 
        ' proportion of even binary nulls.
        ' The thresholds here used (less than 20% nulls where you expect non-nulls, and more than
        ' 60% nulls where you do expect nulls) are completely arbitrary.

        If ((evenBinaryNullsInSample * 2.0) / SampleBytes.Length) < 0.2 AndAlso ((oddBinaryNullsInSample * 2.0) / SampleBytes.Length) > 0.6 Then
            Return Encoding.Unicode
        End If


        '2: UTF-16 BE - in english / european environments, this is usually characterized by a 
        ' high proportion of even binary nulls (starting at 0), with (as this is text) a low 
        ' proportion of odd binary nulls.
        ' The thresholds here used (less than 20% nulls where you expect non-nulls, and more than
        ' 60% nulls where you do expect nulls) are completely arbitrary.

        If ((oddBinaryNullsInSample * 2.0) / SampleBytes.Length) < 0.2 AndAlso ((evenBinaryNullsInSample * 2.0) / SampleBytes.Length) > 0.6 Then
            Return Encoding.BigEndianUnicode
        End If


        '3: UTF-8 - Martin Dürst outlines a method for detecting whether something CAN be UTF-8 content 
        ' using regexp, in his w3c.org unicode FAQ entry: 
        ' http://www.w3.org/International/questions/qa-forms-utf-8
        ' adapted here for C#.
        Dim potentiallyMangledString As String = Encoding.ASCII.GetString(SampleBytes)
        Dim UTF8Validator As New Regex("\A(" + "[\x09\x0A\x0D\x20-\x7E]" + "|[\xC2-\xDF][\x80-\xBF]" + "|\xE0[\xA0-\xBF][\x80-\xBF]" + "|[\xE1-\xEC\xEE\xEF][\x80-\xBF]{2}" + "|\xED[\x80-\x9F][\x80-\xBF]" + "|\xF0[\x90-\xBF][\x80-\xBF]{2}" + "|[\xF1-\xF3][\x80-\xBF]{3}" + "|\xF4[\x80-\x8F][\x80-\xBF]{2}" + ")*\z")
        If UTF8Validator.IsMatch(potentiallyMangledString) Then
            'Unfortunately, just the fact that it CAN be UTF-8 doesn't tell you much about probabilities.
            'If all the characters are in the 0-127 range, no harm done, most western charsets are same as UTF-8 in these ranges.
            'If some of the characters were in the upper range (western accented characters), however, they would likely be mangled to 2-byte by the UTF-8 encoding process.
            ' So, we need to play stats.

            ' The "Random" likelihood of any pair of randomly generated characters being one 
            ' of these "suspicious" character sequences is:
            ' 128 / (256 * 256) = 0.2%.
            '
            ' In western text data, that is SIGNIFICANTLY reduced - most text data stays in the <127 
            ' character range, so we assume that more than 1 in 500,000 of these character 
            ' sequences indicates UTF-8. The number 500,000 is completely arbitrary - so sue me.
            '
            ' We can only assume these character sequences will be rare if we ALSO assume that this
            ' IS in fact western text - in which case the bulk of the UTF-8 encoded data (that is 
            ' not already suspicious sequences) should be plain US-ASCII bytes. This, I 
            ' arbitrarily decided, should be 80% (a random distribution, eg binary data, would yield 
            ' approx 40%, so the chances of hitting this threshold by accident in random data are 
            ' VERY low). 

            'suspicious sequences
            'all suspicious, so cannot evaluate proportion of US-Ascii
            If (suspiciousUTF8SequenceCount * 500000.0 / SampleBytes.Length >= 1) AndAlso (SampleBytes.Length - suspiciousUTF8BytesTotal = 0 OrElse likelyUSASCIIBytesInSample * 1.0 / (SampleBytes.Length - suspiciousUTF8BytesTotal) >= 0.8) Then
                Return Encoding.UTF8
            End If
        End If

        Return Nothing
    End Function

    Private Shared Function IsCommonUSASCIIByte(testByte As Byte) As Boolean
        'lf
        'cr
        'tab
        'common punctuation
        'digits
        'common punctuation
        'capital letters
        'common punctuation
        'lowercase letters
        If testByte = &HA OrElse testByte = &HD OrElse testByte = &H9 OrElse (testByte >= &H20 AndAlso testByte <= &H2F) OrElse (testByte >= &H30 AndAlso testByte <= &H39) OrElse (testByte >= &H3A AndAlso testByte <= &H40) OrElse (testByte >= &H41 AndAlso testByte <= &H5A) OrElse (testByte >= &H5B AndAlso testByte <= &H60) OrElse (testByte >= &H61 AndAlso testByte <= &H7A) OrElse (testByte >= &H7B AndAlso testByte <= &H7E) Then
            'common punctuation
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function DetectSuspiciousUTF8SequenceLength(SampleBytes As Byte(), currentPos As Long) As Integer
        Dim lengthFound As Integer = 0

        If SampleBytes.Length >= currentPos + 1 AndAlso SampleBytes(currentPos) = &HC2 Then
            If SampleBytes(currentPos + 1) = &H81 OrElse SampleBytes(currentPos + 1) = &H8D OrElse SampleBytes(currentPos + 1) = &H8F Then
                lengthFound = 2
            ElseIf SampleBytes(currentPos + 1) = &H90 OrElse SampleBytes(currentPos + 1) = &H9D Then
                lengthFound = 2
            ElseIf SampleBytes(currentPos + 1) >= &HA0 AndAlso SampleBytes(currentPos + 1) <= &HBF Then
                lengthFound = 2
            End If
        ElseIf SampleBytes.Length >= currentPos + 1 AndAlso SampleBytes(currentPos) = &HC3 Then
            If SampleBytes(currentPos + 1) >= &H80 AndAlso SampleBytes(currentPos + 1) <= &HBF Then
                lengthFound = 2
            End If
        ElseIf SampleBytes.Length >= currentPos + 1 AndAlso SampleBytes(currentPos) = &HC5 Then
            If SampleBytes(currentPos + 1) = &H92 OrElse SampleBytes(currentPos + 1) = &H93 Then
                lengthFound = 2
            ElseIf SampleBytes(currentPos + 1) = &HA0 OrElse SampleBytes(currentPos + 1) = &HA1 Then
                lengthFound = 2
            ElseIf SampleBytes(currentPos + 1) = &HB8 OrElse SampleBytes(currentPos + 1) = &HBD OrElse SampleBytes(currentPos + 1) = &HBE Then
                lengthFound = 2
            End If
        ElseIf SampleBytes.Length >= currentPos + 1 AndAlso SampleBytes(currentPos) = &HC6 Then
            If SampleBytes(currentPos + 1) = &H92 Then
                lengthFound = 2
            End If
        ElseIf SampleBytes.Length >= currentPos + 1 AndAlso SampleBytes(currentPos) = &HCB Then
            If SampleBytes(currentPos + 1) = &H86 OrElse SampleBytes(currentPos + 1) = &H9C Then
                lengthFound = 2
            End If
        ElseIf SampleBytes.Length >= currentPos + 2 AndAlso SampleBytes(currentPos) = &HE2 Then
            If SampleBytes(currentPos + 1) = &H80 Then
                If SampleBytes(currentPos + 2) = &H93 OrElse SampleBytes(currentPos + 2) = &H94 Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &H98 OrElse SampleBytes(currentPos + 2) = &H99 OrElse SampleBytes(currentPos + 2) = &H9A Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &H9C OrElse SampleBytes(currentPos + 2) = &H9D OrElse SampleBytes(currentPos + 2) = &H9E Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &HA0 OrElse SampleBytes(currentPos + 2) = &HA1 OrElse SampleBytes(currentPos + 2) = &HA2 Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &HA6 Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &HB0 Then
                    lengthFound = 3
                End If
                If SampleBytes(currentPos + 2) = &HB9 OrElse SampleBytes(currentPos + 2) = &HBA Then
                    lengthFound = 3
                End If
            ElseIf SampleBytes(currentPos + 1) = &H82 AndAlso SampleBytes(currentPos + 2) = &HAC Then
                lengthFound = 3
            ElseIf SampleBytes(currentPos + 1) = &H84 AndAlso SampleBytes(currentPos + 2) = &HA2 Then
                lengthFound = 3
            End If
        End If

        Return lengthFound
    End Function

End Class

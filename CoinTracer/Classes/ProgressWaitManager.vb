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

Public Class ProgressWaitManager

    Private Shared _ParentForm As Form
    Private Shared _CurrentProgressForm As ProgressWaitForm

    Private Class DoLoadProgressParams
        Public ParentForm As Form
        Public LabelText As String
        Public Sub New(ParentForm As Form, ByVal LabelText As String)
            Me.ParentForm = ParentForm
            Me.LabelText = LabelText
        End Sub
    End Class

    ''' <summary>
    ''' Creates a new ProgressWaitForm and displays it to the user
    ''' </summary>
    ''' <param name="parentForm">The form from which this method is being called</param>
    ''' <returns>A ProgressWaitForm object whose UpdateProgress method can be used to display progress to the user</returns>
    ''' <remarks></remarks>
    Public Shared Function ShowProgress(ByVal parentForm As Form, _
                                        Optional LabelText As String = "Bitte warten Sie...") As ProgressWaitForm
        If _CurrentProgressForm IsNot Nothing Then
            Throw New ProgressWaitFormException("Es wird bereits ein Benachrichtigungs-Formular angezeigt.")
            Exit Function
        End If
        _ParentForm = parentForm
        System.Threading.ThreadPool.QueueUserWorkItem(AddressOf DoLoadProgress, New DoLoadProgressParams(parentForm, LabelText))
        While _CurrentProgressForm Is Nothing OrElse _CurrentProgressForm.Visible = False
            System.Threading.Thread.Sleep(5)
        End While
        Return _CurrentProgressForm

    End Function

    Private Shared Sub DoLoadProgress(ByVal DoLoadProgressParams As DoLoadProgressParams)
        Dim f As New ProgressWaitForm(DoLoadProgressParams.ParentForm)
        _CurrentProgressForm = f
        _CurrentProgressForm.StatusLabel.Text = DoLoadProgressParams.LabelText
        f.ShowDialog()
    End Sub

    ''' <summary>
    ''' Updates the progress bar value and/or status text
    ''' </summary>
    ''' <param name="progressPercent">A value from 0 to 100 representing the precentage complete</param>
    ''' <param name="statusText">Any status text to accompany the progressbar</param>
    Public Shared Sub UpdateProgress(ByVal progressPercent As Integer, ByVal statusText As String)
        If _CurrentProgressForm Is Nothing Then
            Throw New ProgressWaitFormException("Es gibt kein Benachrichtigungs-Formular, das aktualisiert werden könnte.")
            Exit Sub
        Else
            _CurrentProgressForm.UpdateProgress(progressPercent, statusText)
        End If
    End Sub

    ''' <summary>
    ''' Updates the progress bar status text
    ''' </summary>
    ''' <param name="statusText">Any status text to accompany the progressbar</param>
    Public Shared Sub UpdateProgress(ByVal statusText As String)
        If _CurrentProgressForm Is Nothing Then
            Throw New ProgressWaitFormException("Es gibt kein Benachrichtigungs-Formular, das aktualisiert werden könnte.")
            Exit Sub
        Else
            _CurrentProgressForm.UpdateProgress(statusText)
        End If
    End Sub

    ''' <summary>
    ''' Indicates if the cancel button has been pressed
    ''' </summary>
    Public Shared ReadOnly Property Canceled() As Boolean
        Get
            If _CurrentProgressForm Is Nothing Then
                Throw New ProgressWaitFormException("Es gibt kein Benachrichtigungs-Formular, das abgefragt werden könnte.")
            Else
                Return _CurrentProgressForm.Canceled
            End If
        End Get
    End Property

    ''' <summary>
    ''' Sets wether a CANCEL button is visible or not
    ''' </summary>
    Public Shared Property WithCancelOption() As Boolean
        Get
            If _CurrentProgressForm Is Nothing Then
                Throw New ProgressWaitFormException("Es gibt kein Benachrichtigungs-Formular, das abgefragt werden könnte.")
            Else
                Return _CurrentProgressForm.WithCancelOption
            End If
        End Get
        Set(ByVal value As Boolean)
            If _CurrentProgressForm Is Nothing Then
                Throw New ProgressWaitFormException("Es gibt kein Benachrichtigungs-Formular, das aktualisiert werden könnte.")
            Else
                _CurrentProgressForm.WithCancelOption = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Closes the ProgressWaitForm - Be sure to call CloseProgress() when finished with this ProgressWaitForm instance
    ''' </summary>
    Public Shared Sub CloseProgress()
        If _CurrentProgressForm IsNot Nothing Then
            _CurrentProgressForm.CloseProgress()
            _CurrentProgressForm = Nothing
            _ParentForm.BringToFront()
        End If
    End Sub

End Class

''' <summary>
''' Bildet einen Fehler bei der Anzeige der ProgressWaitForm ab
''' </summary>
<Serializable()>
Public Class ProgressWaitFormException
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

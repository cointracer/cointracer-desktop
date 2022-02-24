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

Imports System.ComponentModel

''' <summary>
''' Represents a DataGridViewTextBoxColumn with a drop-down filter list accessible from the header cell.  
''' </summary>
Public Class DataGridViewAutoFilterTextBoxColumn
    Inherits DataGridViewTextBoxColumn

    ''' <summary>
    ''' Initializes a new instance of the DataGridViewAutoFilterTextBoxColumn class.
    ''' </summary>
    Public Sub New()
        MyBase.DefaultHeaderCellType = GetType(DataGridViewAutoFilterColumnHeaderCell)
        MyBase.SortMode = DataGridViewColumnSortMode.Programmatic
    End Sub

#Region "public properties that hide inherited, non-virtual properties: DefaultHeaderCellType and SortMode"

    ''' <summary>
    ''' Returns the AutoFilter header cell type. This property hides the 
    ''' non-virtual DefaultHeaderCellType property inherited from the 
    ''' DataGridViewBand class. The inherited property is set in the 
    ''' DataGridViewAutoFilterTextBoxColumn constructor. 
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows ReadOnly Property DefaultHeaderCellType() As Type
        Get
            Return GetType(DataGridViewAutoFilterColumnHeaderCell)
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the sort mode for the column and prevents it from being 
    ''' set to Automatic, which would interfere with the proper functioning 
    ''' of the drop-down button. This property hides the non-virtual 
    ''' DataGridViewColumn.SortMode property from the designer. The inherited 
    ''' property is set in the DataGridViewAutoFilterTextBoxColumn constructor.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), Browsable(False), _
    DefaultValue(DataGridViewColumnSortMode.Programmatic)> _
    Public Shadows Property SortMode() As DataGridViewColumnSortMode
        Get
            Return MyBase.SortMode
        End Get
        Set(ByVal value As DataGridViewColumnSortMode)
            If value = DataGridViewColumnSortMode.Automatic Then
                Throw New InvalidOperationException( _
                    "A SortMode value of Automatic is incompatible with " & _
                    "the DataGridViewAutoFilterColumnHeaderCell type. " & _
                    "Use the AutomaticSortingEnabled property instead.")
            Else
                MyBase.SortMode = value
            End If
        End Set
    End Property

#End Region 'public properties that hide inherited, non-virtual properties

#Region "public properties: FilteringEnabled, AutomaticSortingEnabled, DropDownListBoxMaxLines"

    ''' <summary>
    ''' Gets or sets a value indicating whether filtering is enabled for this column. 
    ''' </summary>
    <DefaultValue(True)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property FilteringEnabled() As Boolean
        Get
            ' Return the header-cell value.
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).FilteringEnabled
        End Get
        Set(ByVal value As Boolean)
            ' Set the header-cell property. 
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).FilteringEnabled = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether automatic sorting is enabled for this column. 
    ''' </summary>
    <DefaultValue(True)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property AutomaticSortingEnabled() As Boolean
        Get
            ' Return the header-cell value.
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).AutomaticSortingEnabled
        End Get
        Set(ByVal value As Boolean)
            ' Set the header-cell property.
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).AutomaticSortingEnabled = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the maximum height of the drop-down filter list for this column. 
    ''' </summary>
    <DefaultValue(30)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property DropDownListBoxMaxLines() As Integer
        Get
            ' Return the header-cell value.
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).DropDownListBoxMaxLines
        End Get
        Set(ByVal value As Integer)
            ' Set the header-cell property.
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).DropDownListBoxMaxLines = value
        End Set
    End Property

    ''' <summary>
    ''' Caption of the OK button at the bottom of the filter list
    ''' </summary>
    <Browsable(True)> _
    <DefaultValue("OK")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property CaptionOKButton() As String
        Get
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionOKButton
        End Get
        Set(ByVal value As String)
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionOKButton = value
        End Set
    End Property

    ''' <summary>
    ''' Caption of the cancel button at the bottom of the filter list
    ''' </summary>
    <Browsable(True)> _
    <DefaultValue("Abbrechen")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public Property CaptionCancelButton() As String
        Get
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionCancelButton
        End Get
        Set(ByVal value As String)
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionCancelButton = value
        End Set
    End Property

    ''' <summary>
    ''' Caption for the 1st entry in filter list: default "(Alles ausw�hlen)"
    ''' </summary>
    <Browsable(True)> _
    <DefaultValue("(Alles ausw�hlen)")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    <Description("Caption for the first entry in filter list, representing all values of the filter")> _
    Public Property CaptionAllValues() As String
        Get
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionAllValues
        End Get
        Set(ByVal value As String)
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionAllValues = value
        End Set
    End Property

    ''' <summary>
    ''' Caption for the 2nd last entry in filter list: default "(Leere)"
    ''' </summary>
    <Browsable(True)> _
    <DefaultValue("(Leere)")> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    <Description("Caption for the 2nd last entry in filter list, representing all blank values")> _
    Public Property CaptionBlankValues() As String
        Get
            Return CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionBlankValues
        End Get
        Set(ByVal value As String)
            CType(HeaderCell, DataGridViewAutoFilterColumnHeaderCell).CaptionBlankValues = value
        End Set
    End Property

#End Region 'public properties

#Region "public, static, convenience methods: RemoveFilter, GetFilter, SetFilter and GetFilterStatus"

    ''' <summary>
    ''' Removes the filter from the BindingSource bound to the specified DataGridView. 
    ''' </summary>
    ''' <param name="dataGridView">The DataGridView bound to the BindingSource to unfilter.</param>
    Public Shared Sub RemoveFilter(ByVal dataGridView As DataGridView)
        DataGridViewAutoFilterColumnHeaderCell.RemoveFilter(dataGridView)
    End Sub

    ''' <summary>
    ''' Gets a status string for the specified DataGridView indicating the 
    ''' number of visible rows in the bound, filtered BindingSource, or 
    ''' String.Empty if all rows are currently visible. 
    ''' </summary>
    ''' <param name="dataGridView">The DataGridView bound to the 
    ''' BindingSource to return the filter status for.</param>
    ''' <returns>A string in the format "x of y records found" where x is 
    ''' the number of rows currently displayed and y is the number of rows 
    ''' available, or String.Empty if all rows are currently displayed.</returns>
    Public Shared Function GetFilterStatus(ByVal dataGridView As DataGridView) As String
        Return DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(dataGridView)
    End Function

#End Region 'public, static, convenience methods

End Class 'DataGridViewAutoFilterTextBoxColumn


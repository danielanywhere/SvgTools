REM  *****  BASIC  *****
REM * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
REM * 
REM * This program is free software: you can redistribute it and/or modify
REM * it under the terms of the GNU General Public License as published by
REM * the Free Software Foundation, either version 3 of the License, or
REM * (at your option) any later version.
REM * 
REM * This program is distributed in the hope that it will be useful,
REM * but WITHOUT ANY WARRANTY; without even the implied warranty of
REM * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
REM * GNU General Public License for more details.
REM * 
REM * You should have received a copy of the GNU General Public License
REM * along with this program.  If not, see <https://www.gnu.org/licenses/>.

Sub RenderTestsMacro
	dim oColumn as object
	dim oColumns as object
	dim oDispatcher as object
	dim oDoc as object
	dim oFrame as object
	dim oSheet as object
	
	' Get the current document
	oDoc = ThisComponent
	
	' Select the sheet
	oSheet = oDoc.Sheets.getByName("ImpliedFormDesignAXamlRenderTests")
	oDoc.CurrentController.setActiveSheet(oSheet)
	
	' Hide column B
	oColumns = oSheet.Columns
	oColumn = oColumns.getByIndex(1)
	oColumn.IsVisible = false
	
	' Export as HTML
	oDoc.storeToURL("file:///C:/Temp/Report-RenderTests.html", _
		Array(MakePropertyValue("FilterName", "HTML (StarCalc)")))
	
	' Close the document and application
	oDoc.close(true)
End Sub

' Helper function to set properties
Function MakePropertyValue(cName As string, uValue) As object
dim oPropertyValue as new com.sun.star.beans.PropertyValue

	oPropertyValue.Name = cName
	oPropertyValue.Value = uValue
	MakePropertyValue = oPropertyValue
End Function

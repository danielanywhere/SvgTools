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

Sub ExportAvaloniaXamlInitStringsToFile()

dim char as string
dim content as string
dim controlCount as long
dim controlIndex as long
dim controls() as string
dim crlf as string
dim fileNumber as integer
dim filePath as string
dim lastColIndex as long
dim lastRowIndex as long
dim lineContent as string
dim oCell as object
dim oDoc as object
dim oSheet as object
dim properties() as string
dim propertyCount as long
dim propertyIndex as long
dim quote as string
dim tab as string
    
	crlf = chr(13) & chr(10)
	tab = chr(9)
	quote = chr(34)
 
	oDoc = ThisComponent
	oSheet = oDoc.Sheets.getByName("ControlPropertiesMatrix")
	
	' --- Find last property column in row 1 ---
	lastColIndex = 1
	do while oSheet.getCellByPosition(lastColIndex, 0).String <> ""
		lastColIndex = lastColIndex + 1
	loop
	propertyCount = lastColIndex - 1
	
	redim properties(propertyCount - 1)
	for propertyIndex = 1 To propertyCount
		properties(propertyIndex - 1) = oSheet.getCellByPosition(propertyIndex, 0).String
	next propertyIndex
	
	' --- Find last control row in column A ---
	lastRowIndex = 1
	do while oSheet.getCellByPosition(0, lastRowIndex).String <> ""
		lastRowIndex = lastRowIndex + 1
	loop
	controlCount = lastRowIndex - 1
	
	redim controls(controlCount - 1)
	for controlIndex = 2 To controlCount + 1
		controls(controlIndex - 2) = oSheet.getCellByPosition(0, controlIndex - 1).String
	next controlIndex
	
	' --- Build C# code ---
	content = content & "/// <summary>" & crlf & _
		"/// Complete list of recognized control classes." & crlf & _
		"/// </summary>" & crlf & _
		"private static " & _
		"List<string> mXamlControlNames = new List<string>()" & crlf & _
		"{" & crlf
	if controlCount > 0 then
		content = content & tab
		for controlIndex = 0 To controlCount - 1
			content = content & quote & controls(controlIndex) & quote
			if controlIndex < controlCount - 1 then
				content = content & ","
				if controlIndex mod 4 = 3 then
					content = content & crlf & tab
				else
					content = content & " "
				end if
			end if
		next controlIndex
		content = content & crlf & "};" & crlf & crlf
	end if
	
	content = content & "/// <summary>" & crlf & _
		"/// Complete list of common properties." & crlf & _
		"/// </summary>" & crlf & _
		"private static " & _
		"List<string> mXamlPropertyNames = new List<string>()" & crlf & _
		"{" & crlf
	if propertyCount > 0 then
		content = content & tab
		for propertyIndex = 0 To propertyCount - 1
			content = content & quote & properties(propertyIndex) & quote
			if propertyIndex < propertyCount - 1 then
				content = content & ","
				if propertyIndex mod 4 = 3 then
					content = content & crlf & tab
				else
					content = content & " "
				end if
			end if
		next propertyIndex
		content = content & crlf & "};" & crlf & crlf
	end if
	
	content = content & "/// <summary>" & crlf & _
		"/// Supported properties per control, as " & crlf & _
		"/// [controlIndex * propertyCount + propertyIndex]." & crlf & _
		"/// </summary>" & crlf & _
		"private static " & _
		"string mXamlControlProperties = " & crlf
	if controlCount > 0 and propertyCount > 0 then
		content = content & tab
		for controlIndex = 0 To controlCount - 1
			lineContent = ""
			for propertyIndex = 0 To propertyCount - 1
				oCell = oSheet.getCellByPosition(propertyIndex + 1, controlIndex + 1)
				char = oCell.String
				if len(char) > 0 then
					char = left(char, 1)
				else
					char = "."
				end if
				lineContent = lineContent & char
			next propertyIndex
			content = content & """" & lineContent & """"
			if controlIndex < controlCount - 1 then
				content = content & " +" & crlf & tab
			else
				content = content & ";"
			end if
		next controlIndex
	else
		content = content & """"""
	end if
	content = content & crlf & crlf
	
	' --- Write to file ---
	filePath = "C:\Temp\GeneratedCode-XamlInitStrings.txt"
	fileNumber = FreeFile
	open filePath for output as #fileNumber
	print #fileNumber, content
	close #fileNumber
	
	msgbox "Initialization strings exported to " & filePath
End Sub

:: GenerateBetaReport.cmd
:: Generate the component-level status report of the curren beta test.
SET SOURCE=..\Docs\SvgToolsWorksheet.ods
SET TARGET=C:\Temp\RenderTestsReport.html
SET PATTERN=ReportPostProcessing.json

SET FAR=C:\Files\Dropbox\Develop\Active\FindAndReplace\FindAndReplace\bin\Debug\net6.0\FindAndReplace.exe
SET LOFFICE=C:\Program Files\LibreOffice\program

:: Generate the report file.
"%LOFFICE%\soffice" --calc "%SOURCE%" "vnd.sun.star.script:Standard.Module1.RenderTestsMacro?language=Basic&location=application"

:: HTML Clean-Up
"%FAR%" /wait "/files:%TARGET%" "/patternfile:%PATTERN%"

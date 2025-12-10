:: RenameReferenceAudio.cmd
:: Rename the reference audio files to be project-specific.
SET FAR=C:\Files\Dropbox\Develop\Active\FindAndReplace\FindAndReplace\bin\Debug\net6.0\FindAndReplace.exe
SET FLT=C:\Files\Dropbox\Develop\Active\FileTools\FileTools\bin\Debug\net7.0-windows\FileTools.exe

"%FLT%" /action:Batch /configfile:FileToolsRenameReferenceAudioFiles.json
"%FAR%" /wait /patternfile:FindAndReplaceStoryboardAudioFiles.json /files:../Storyboards/CodeFirstMethodProblem.storyboard.json


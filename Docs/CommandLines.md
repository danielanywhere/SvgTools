# Example Command Lines

Following are some example command lines.

<p>&nbsp;</p>

## General Purpose Commands

Apply Transforms to File

```
/wait /action:applytransforms /workingpath:C:\Temp /infile:FormDesign003.svg /outfile:FormDesign003c-ApplyTransforms.svg
```

Dereference Links on a File

```
/wait /action:dereferencelinks /workingpath:C:\Temp /infile:FormDesign003.svg /outfile:FormDesign003d-DereferenceLinks.svg
```

Perform a clean up on a file.

```
/wait /action:cleanupsvg /workingpath:C:\Temp /infile:FormDesign003.svg /outfile:FormDesign003f-CleanupSvg.svg
```

Calculate an Individual Transform

```
/wait /action:calculatetransform "/properties:[{'Name':'transform','Value':'matrix(1,0,0,1.0027638,-4.2151489,-3.7945675)'},{'Name':'x','Value':'529.21484'},{'Name':'y','Value':'113.54385'},{'Name':'width','Value':'12.998965'},{'Name':'height','Value':'954.19031'}]"
```

Run the batch file WorkingSvg-SvgTools01.json

```
/wait /action:batch /configfile:C:\Temp\WorkingSvg-SvgTools01.json /workingpath:C:\Temp
```

Test precision setting values

```
/wait /testprecision:107.65932%,0 /testprecision:107.65932px,3 /testprecision:93451.65932rem,-2
```

<p>&nbsp;</p>

## Implicit Form Design

Enumerate the elements in an Implicit Form Design file

```
svgtools /wait /action:ImpliedDesignEnumerateControls /infile:ProjectTaskImpliedDesign.svg /workingpath:C:/Files/Dropbox/Develop/Shared/SvgTools/Drawings
```

Render Avalonia XAML file (.axaml) from an Implicit Form Design file

```
svgtools /wait /action:ImpliedDesignToAvaloniaXaml /infile:ProjectTaskImpliedDesign.svg /outfile:../Experiments/Output/ProjectTask/Avalonia/ProjectTask.axaml /styleworksheet:Styles/Avalonia/ProjectTaskImpliedDesignStyles.json /workingpath:C:/Files/Dropbox/Develop/Shared/SvgTools/Drawings
```


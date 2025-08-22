# Dan's SVG Tools Console Application

<img src="Images/SvgToolsBanner001.png" width="100%" alt="SVG Tools Page Banner" />

This application was created to perform commonly-needed tasks related to SVG files, like those exported from Inkscape, Adobe Illustrator, Canva, Figma, or others.

Although it will offer a very long list of features in the near future, this version only includes a few of those activities that directly support another project I am currently working on. Please stay tuned for what I hope will be a continuing stream of improvements.

<p>&nbsp;</p>

## Current Toolset

The application currently supports the following actions.

-   **ApplyTransforms**. This action dereferences all linked objects in the document, applies transforms to every object in the hierarchy, and removes all of the transforms.<p>&nbsp;</p>
-   **CalculateTransform**. Display the result of an SVG transform like matrix, scale, rotate, etc., applied to caller-supplied x, y, width and height variables. This action has no effect on the file.<p>&nbsp;</p>
-   **CleanupSvg**. This action dereferences all linked objects, applies transforms, purges unreferenced items from the defs section, and rounds all values to the specified decimal precision (default precision = 3 if not specified).<p>&nbsp;</p>
-   **DereferenceLinks**. This action serves the purpose of encapsulating as many of the referenced shapes from the defs section as possible into the actual implementation targets from where they have been referenced. This allows each object to be handled freely and independently, either in a visual or text editor.<p>&nbsp;</p>
-   **OpenWorkingSvg**. Only supported in batch mode, this action opens an SVG file so multiple actions can be taken during the same session.<p>&nbsp;</p>
-   **PurgeDefs**. This action removes unreferenced elements from the defs section.<p>&nbsp;</p>
-   **RoundAllValues**. This action rounds all values to the specified precision, according to number of decimal places, and including whole ones and tens places if the precision is 0 or negative, respectively.<p>&nbsp;</p>
-   **SaveWorkingSvg**. Only supported in batch mode, this action saves the changes to the current working SVG in memory.<p>&nbsp;</p>
-   **SortSymbols**. This action action alphabetically sorts the symbols list in the defs section of the SVG.

<p>&nbsp;</p>

## Version Notes

Files will be uploaded within the next couple of days. Please stand-by.

This version is currently available as command-line application only, but work is proceeding that will expose this functionality for programmatic use in other applications.

(This ReadMe file was created in LibreOffice Write!)

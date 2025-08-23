using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	SvgActionTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of available actions for SVGs.
	/// </summary>
	public enum SvgActionTypeEnum
	{
		/// <summary>
		/// No action specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Apply all appropriate transformations in the document.
		/// </summary>
		ApplyTransforms,
		/// <summary>
		/// Convert the SVG UI Design Art to XAML.
		/// </summary>
		ArtToXaml,
		/// <summary>
		/// Read a configuration file to load a batch of SVG actions.
		/// </summary>
		Batch,
		/// <summary>
		/// Clean up the specified SVG file by dereferencing links,
		/// applying transforms, purging unused defs, and rounding all
		/// values.
		/// </summary>
		CleanupSvg,
		/// <summary>
		/// Solve an individual transform for the provided properties.
		/// </summary>
		CalculateTransform,
		///// <summary>
		///// Clear the input files collection.
		///// </summary>
		//ClearInputFiles,
		/// <summary>
		/// Find objects used by reference through href, such as through the use
		/// element, create local instances of those objects contained by the user,
		/// and remove the href attributes.
		/// </summary>
		DereferenceLinks,
		///// <summary>
		///// Draw the image specified by ImageName onto the working image at the
		///// location specified by user properties Left and Top.
		///// </summary>
		//DrawImage,
		///// <summary>
		///// Open the image file specified in the current input file. Name it
		///// in the local images collection with the name specified in the
		///// user property ImageName.
		///// </summary>
		//FileOpenImage,
		///// <summary>
		///// Open each image from the range and place the image specified in
		///// InputFilename at the options specified by Left, Top, Width, and Height.
		///// </summary>
		//FileOverlayImage,
		///// <summary>
		///// Save the working image to the currently specified OutputFile.
		///// </summary>
		//FileSaveImage,
		///// <summary>
		///// Run the Actions collection of the action through all of the files
		///// currently loaded in the InputFiles collection, setting the
		///// CurrentFile property for each pass.
		///// </summary>
		//ForEachFile,
		///// <summary>
		///// Run one or more conditions to determine whether the sub-actions
		///// of the action should be run.
		///// </summary>
		//If,
		///// <summary>
		///// Set the background color of the working image, overlaying the
		///// previous contents on the new background.
		///// </summary>
		//ImageBackground,
		///// <summary>
		///// Clear all images from the Images collection.
		///// </summary>
		//ImagesClear,
		/// <summary>
		/// Open the working SVG file to allow multiple operations to be completed
		/// in the same session.
		/// </summary>
		OpenWorkingSvg,
		/// <summary>
		/// Remove unused elements from the defs section.
		/// </summary>
		PurgeDefs,
		/// <summary>
		/// Round all values to the precision supplied in the 'Precision'
		/// user property.
		/// </summary>
		RoundAllValues,
		///// <summary>
		///// Run the sequence specified in the 'SequenceName' user property.
		///// </summary>
		//RunSequence,
		/// <summary>
		/// Save the current working SVG to the specified output filename.
		/// </summary>
		SaveWorkingSvg,
		///// <summary>
		///// Set the current working image to the one with the local name
		///// found in the user property ImageName.
		///// </summary>
		//SetWorkingImage,
		///// <summary>
		///// Scale the image to a new size, as specified in user properties
		///// Width and Height.
		///// </summary>
		//SizeImage,
		/// <summary>
		/// Sort the symbols alphabetically in the defs section.
		/// </summary>
		SortSymbols
	}
	//*-------------------------------------------------------------------------*

}

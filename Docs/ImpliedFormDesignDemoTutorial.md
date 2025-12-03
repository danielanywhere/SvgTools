# Demo Tutorial - Create a Wizard Application

Following is a tutorial representing the checklist used to create the forms used in the **ImpliedFormDesignWizard** application, a part of this project that helps to speed your process of creating cross-platform desktop Graphical User Interface applications and requiring only a tiny fraction of the time and effort currently required by every other widely-known approach.

NOTE: This page is currently under construction pending the completion of the Implied Form Design Wizard application.

<p>&nbsp;</p>

## Important Take-Away

This tutorial took much longer to write, and might even take longer to read, than the act of creating the target application and forms it describes.

<p>&nbsp;</p>

## Create and Configure the Application
The demo application is created on Linux in the community version of **JetBrains Rider** using the **Avalonia Rider** plugin, but could be created using a similar process on Windows in the community version of Microsoft Visual Studio as well.

For more information about configuring **JetBrains Rider** to host **Avalonia UI** projects, please see <https://docs.avaloniaui.net/docs/reference/jetbrains-rider-ide/jetbrains-rider-setup>

 - Open JetBrains Rider.
 - In the **Welcome to JetBrains Rider** dialog, click **New Solution**.
 - Under **Custom Templates**, select **Avalonia .NET App**.
 - Set the desired **Solution Name**, **Project Name**, and **Solution Directory**, as well as any other options that match your preferences. In this tutorial, the **Application Name** and **Solution Name** are both set to **ImpliedFormDesignWizard**.
 - Click **Create**.
 - In **Solution Explorer**, right-click the project node and from the context menu, select **Add** / **Directory**. Name the new directory **Assets**.
 - Select the **Assets** directory and create two more directories: **Fonts** and **Images**.
 - Download the **Roboto** font set from Google, and unzip the .TTF files into the **Assets/Fonts** folder.
 - Copy the logo image from **Images/WizardIcon01.png** in this repository to your solution's **Assets/Images** folder.
 - Open your .CSPROJ file for editing. On Rider, this is accomplished by right-clicking the project name in **Solution Explorer**, then selecting **Edit** / **Edit** *{ProjectName}***.csproj**.
 - Under **Project** / **PropertyGroup**, set the nodes `<Nullable>disable</Nullable>` and `<ImplicitUsings>disable</ImplicitUsings>`. One or more of them may need to be added.
 - Add the following **ItemGroup**. You might find that Rider has already created a version of this when you created the directories, and may need only to append the wildcard symbols.

```
		<ItemGroup>
			<AvaloniaResource Include="Assets\Images\**" />
			<AvaloniaResource Include="Assets\Fonts\**" />
		</ItemGroup>
```

 - Save and close the .CSPROJ file.
 - Open **App.xaml.cs** and update the **Initialize** method to resemble the following by adding the font loader.

```
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
			//	Automatically use the resource-embedded font when called for by
			//	name.
			FontManager.Current.AddFontCollection(
				new EmbeddedFontCollection(
					new Uri("fonts:App", UriKind.Absolute),
					new Uri(
						"avares://ImpliedFormDesignWizard/Assets/Fonts",
						UriKind.Absolute)));
		}
```

 - In the method **OnFrameworkInitializationCompleted**, change the name of the default loaded window to **frmImpliedFormDesignWizard**, as shown in the following excerpt.

```
		public override void OnFrameworkInitializationCompleted()
		{
			if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new frmImpliedFormDesignWizard();
			}

			base.OnFrameworkInitializationCompleted();
		}
```

<p>&nbsp;</p>

## Wizard Page 1
Estimated time (from scratch, assuming the logo image is already created): 10 minutes

The reference drawing for this step can be found in this repository at **../Drawings/ImpliedFormDesignWizard01.svg**.

 - In **Inkscape**, draw the first page of the wizard. Name the file **ImpliedFormDesignWizard01.svg**.
 - Use a large text heading, a multi-line text with formatting, an image and buttons.
 - Set the unique IDs of the **Cancel** and **Next** buttons to **btnCancel01** and **btnNext01**, respectively.
 - Use a sample command line similar to the following, making any adjustments necessary so it will work on your own machine, to convert the form to XAML.

```
svgtools /wait /action:ImpliedDesignToAvaloniaXaml /infile:ImpliedFormDesignWizard01.svg /outfile:../Experiments/Output/ImpliedFormDesignWizard/Avalonia/frmImpliedFormDesignWizardPage01.axaml /styleworksheet:Styles/Avalonia/ImpliedFormDesignWizardStyles.json /workingpath:C:/Files/Dropbox/Develop/Shared/SvgTools/Drawings
```

<p>&nbsp;</p>

## Wizard Page 2
Estimated time: 5 minutes

The reference drawing for this step can be found in this repository at **../Drawings/ImpliedFormDesignWizard02.svg**.

 - Create a file copy of the first drawing. Name the new file **ImpliedFormDesignWizard02.svg**.
 - In **Inkscape**, overwrite the heading text with **Select Drawing**.
 - Delete the multi-line text block.
 - Place a TextBox and a Button with an ellipsis. Give the textbox and button control unique, memorable IDs, like **txtSelectDrawing** and **btnSelectDrawing**, respectively.
 - Group the TextBox, Button, and ellipsis label, then give that group the label **TextWithHelper**.
 - Change the text of the **Cancel** button to **Back** and set its ID to **btnBack02**.
 - Change the ID of the **Next** button to **btnNext02**.
 - Use a sample command line similar to the following, making any adjustments necessary so it will work on your own machine, to convert the form to XAML.

```
svgtools /wait /action:ImpliedDesignToAvaloniaXaml /infile:ImpliedFormDesignWizard02.svg /outfile:../Experiments/Output/ImpliedFormDesignWizard/Avalonia/frmImpliedFormDesignWizardPage02.axaml /styleworksheet:Styles/Avalonia/ImpliedFormDesignWizardStyles.json /workingpath:C:/Files/Dropbox/Develop/Shared/SvgTools/Drawings
```

<p>&nbsp;</p>

## Join Forms
We are going to merge the base forms from above to create a single main form for containing multiple pages.

 - Use a sample command line similar to the following, making any adjustments necessary so it will work on your own machine, to merge the base XAML forms into a single, new composite form.

```
svgtools /wait /action:XamlMergeContents /infile:frmImpliedFormDesignWizardPage01.axaml /infile:frmImpliedFormDesignWizardPage02.axaml /outfile:frmImpliedFormDesignWizard.axaml /workingpath:C:/Files/Dropbox/Develop/Shared/SvgTools/Experiments/Output/ImpliedFormDesignWizard/Avalonia /properties:[{'name':'CreateBackingFile','value':'true'}]
```

In the above command, note that the input filename (infile) could also be specified just once with a '?' wildcard in the second digit's position, indicating that any character is valid for that position. When explicitly specifying each filename, however, we have full control over the order in which those forms appear in the output.

Using the action **XamlMergeContents**, all of the matching files in the folder will be combined and written to the output file **frmImpliedFormDesignWizard.axaml**.

<p>&nbsp;</p>

## Update Application
Follow these steps to update the application with the prepared multi-panel form.

 - Copy **frmImpliedFormDesignWizard.axaml** and **frmImpliedFormDesignWizard.axaml.cs** from your Experiments/Output/ImpliedFormDesignWizard/Avalonia folder to the project folder of your Avalonia project.
 - On the code-behind file, add the following handler for the **OnWindowLoaded** event.

```
		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			Trace.WriteLine("Window Loaded");
		}
```
 - On the XAML **Window** object, add the property **Loaded="OnWindowLoaded"**.
 - Give each of the primary RelativePanel objects (direct children of the Window), known x:Name values (e.g. "Page1", "Page2", etc.).
 - At the same time as updating the names, add the property IsVisible="True" for the first panel and IsVisible="False" for the rest.


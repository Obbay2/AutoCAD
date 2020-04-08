using Autodesk.AutoCAD.Runtime;
using System.Windows;

[assembly: ExtensionApplication(null)]
[assembly: CommandClass(typeof(CADApp.CAD))]

// This application implements a command called open_mtext
// This will let the user easily replace text in the file

//To use Project1.dll:
//1. Start AutoCAD and open a drawing.
//2. Type netload and select Project1.dll.

// Please add the References acdbmgd.dll,acmgd.dll,
// Autodesk.AutoCAD.Interop.dll and Autodesk.AutoCAD.Interop.Common.dll
// before trying to build this project.
namespace CADApp
{
    class CAD
    {
		[CommandMethod("open_mtext")]
		static public void Open()
		{
			new MainWindow().Show();
		}
	}
}

// UndertaleModTool script for importing obj_mobilecontrols event GML files.
// NOTE: UndertaleModTool scripts are C# .csx files, not .asl files.
//
// Expected filenames:
//   gml_Object_obj_mobilecontrols_Create_0.gml
//   gml_Object_obj_mobilecontrols_Step_0.gml
//   gml_Object_obj_mobilecontrols_Draw_0.gml
//   gml_Object_obj_mobilecontrols_Draw_64.gml
//   etc.
//
// The filename determines the event. Keep the standard UMT GML filename format.

using System;
using System.IO;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModLib.Compiler;

EnsureDataLoaded();

string objectName = "obj_mobilecontrols";
string importFolder = PromptChooseDirectory();

if (string.IsNullOrWhiteSpace(importFolder))
    throw new ScriptException("No GML folder was selected.");

string[] files = Directory.GetFiles(
    importFolder,
    "gml_Object_" + objectName + "_*.gml",
    SearchOption.AllDirectories
);

if (files.Length == 0)
    throw new ScriptException(
        "No files named gml_Object_" + objectName + "_*.gml were found in the selected folder."
    );

// Make sure the destination object exists.
var targetObject = Data.GameObjects.ByName(objectName);
if (targetObject == null)
{
    targetObject = new UndertaleGameObject()
    {
        Name = Data.Strings.MakeString(objectName)
    };
    Data.GameObjects.Add(targetObject);
}

// Queue all matching event code for import. CodeImportGroup uses the standard
// gml_Object_<object>_<event>_<subtype> naming convention to link code to events.
var importGroup = new CodeImportGroup(Data);

int importedCount = 0;
foreach (string file in files.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
{
    string codeName = Path.GetFileNameWithoutExtension(file);
    string source = File.ReadAllText(file);

    if (string.IsNullOrWhiteSpace(source))
    {
        ScriptMessage("Skipping empty file: " + Path.GetFileName(file));
        continue;
    }

    importGroup.QueueReplace(codeName, source);
    importedCount++;
}

if (importedCount == 0)
    throw new ScriptException("All matching GML files were empty; nothing was imported.");

importGroup.Import();

ScriptMessage(
    "Imported " + importedCount + " GML event file(s) for " + objectName + "."
    + Environment.NewLine
    + "Check the Game Objects tab and save a backup copy of the modified game."
);

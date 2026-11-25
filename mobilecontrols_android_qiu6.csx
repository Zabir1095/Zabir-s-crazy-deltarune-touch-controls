// mobilecontrols.csx
// For Genouka UndertaleModTool 0.9.2.0-qiu.6 (Android/Avalonia).
//
// Put files in a folder with names such as:
// gml_Object_obj_mobilecontrols_Create_0.gml
// gml_Object_obj_mobilecontrols_Step_0.gml
// gml_Object_obj_mobilecontrols_Draw_0.gml
// gml_Object_obj_mobilecontrols_Draw_64.gml
//
// The object "obj_mobilecontrols" must already exist in the game.
// CodeImportGroup parses the standard GML object-event filenames and links
// imported code to the matching object events.

using System;
using System.IO;
using System.Linq;
using UndertaleModLib.Compiler;

EnsureDataLoaded();

string importFolder = PromptChooseDirectory();

if (string.IsNullOrWhiteSpace(importFolder))
    throw new ScriptException("Folder selection was cancelled.");

if (!Directory.Exists(importFolder))
    throw new ScriptException("The selected folder does not exist.");

string[] files = Directory.GetFiles(
    importFolder,
    "gml_Object_obj_mobilecontrols_*.gml",
    SearchOption.AllDirectories
)
.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
.ToArray();

if (files.Length == 0)
    throw new ScriptException(
        "No matching files were found. Expected filenames like " +
        "gml_Object_obj_mobilecontrols_Create_0.gml."
    );

if (Data.GameObjects.ByName("obj_mobilecontrols") == null)
    throw new ScriptException(
        "The game does not contain an object named obj_mobilecontrols. " +
        "Create/import that object first, then run this script again."
    );

int queued = 0;
int skipped = 0;

UndertaleModLib.Compiler.CodeImportGroup importGroup = new(Data);

foreach (string file in files)
{
    string codeName = Path.GetFileNameWithoutExtension(file);
    string sourceCode = File.ReadAllText(file);

    if (string.IsNullOrWhiteSpace(sourceCode))
    {
        skipped++;
        continue;
    }

    importGroup.QueueReplace(codeName, sourceCode);
    queued++;
}

if (queued == 0)
    throw new ScriptException("All matching GML files were empty.");

importGroup.Import();

ScriptMessage(
    "Mobile controls import finished."
    + Environment.NewLine
    + "Object: obj_mobilecontrols"
    + Environment.NewLine
    + "Imported event files: " + queued
    + Environment.NewLine
    + "Skipped empty files: " + skipped
    + Environment.NewLine
    + "Review the object's events, then save a NEW copy of the game."
);

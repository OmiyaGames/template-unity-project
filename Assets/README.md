This is the Assets folder, required by Unity.  All files and folders in this project shows up in the Project Pane.

This folder contains the following folders:

Scenes:
Where all the scenes are stored.

Prefabs:
Where all the prefabs are stored.

Models:
Where all the 3D assets, especially models are stored

Models/Animations:
Where all the animations are stored

Models/Textures:
Where all the images specifically used for texturing 3D models are stored.
Putting textures here makes it easier for the auto-generated materials to find its corresponding textures.

Models/Materials:
Where all the materials used in models are used.
Auto-generated material are created here.

Images:
Where all the image assets used in a 2D context (such as particles) are stored.
If an image is not used in a model, it goes here!

Images/GUI:
Where all the image assets used in GUI are stored.

Sounds:
Where all audio and sound effects are stored.

Sounds/Music:
Where music are stored, especially background music.

Sounds/Voices:
Where voices acting are stored.

Resources:
A Unity folder where any file is accessible from script as if going through a normal filesystem.
WARNING: loading assets from this folder is sloooooooooow!

Standard Assets:
A Unity folder where imported assets are usually placed.
Scripts in this folder are compiled first, allowing Javascript codes in the Scripts folder to access C# code.

Plugins:
A Unity folder where plugins are installed.

Shaders:
Where shader scripts are stored.

Scripts:
Where the rest of the scripts are stored.

Scripts/Editor:
A Unity folder where scripts extending any UnityEditor are placed in to compile.
Basically, any script adding or changing a menu goes here.

Scripts/Gizmos:
A Unity folder where scripts with OnDrawGizmos() function is defined.
Basically, any script drawing debug symbols in the Editor pane goes here.

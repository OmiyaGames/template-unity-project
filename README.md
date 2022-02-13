# [Omiya Games](https://www.omiyagames.com/) - Template Unity Project

[![Documentation](https://github.com/OmiyaGames/template-unity-project/workflows/Host%20DocFX%20Documentation/badge.svg)](https://omiyagames.github.io/template-unity-project/)  [![Ko-fi Badge](https://img.shields.io/badge/donate-ko--fi-29abe0.svg?logo=ko-fi)](https://ko-fi.com/I3I51KS8F) [![License Badge](https://img.shields.io/github/license/OmiyaGames/template-unity-project)](/LICENSE.md)

This is a template Unity project [Omiya Games](https://www.omiyagames.com/)' uses to start their game project.

## Project Layout

In this Unity project, the following folders has the following roles:

- Required by Unity:
	- Assets
		- A folder required by Unity.  Contains assets used for the game.  See the [README.md](/Assets/README.md) in that folder for more details.
	- Packages
	- ProjectSettings
- Optional, visible:
	- Builds~
		- A folder where its contents are ignored by Git.  Typically where game builds are created in.
	- UnimportedAssets~
		- A folder that Git versions, but Unity doesn't detect.  Typically where unfinished art assets are placed in.
	- Documentation~
		- Folder containing files on generating documentation for this project.
- Optional, hidden:
	- .github
		- Files for Github-specific features.
	- .vscode
		- Files for Visual Code-specific features.

## More Info

Some packages this template uses:

- [Embed WebGL Template](https://openupm.com/packages/com.omiyagames.embedwebgltemplate/)
- [Cryptography](https://openupm.com/packages/com.omiyagames.cryptography/)
- [Common](https://openupm.com/packages/com.omiyagames.common/)

Other resources:

- [API Documentation](https://omiyagames.github.io/template-unity-project/api)
- [Change Log](https://omiyagames.github.io/template-unity-project/manual/changelog.html)

In this Unity project, the following folders has the following roles:

.fossil_settings:
A folder used by Fossil.  Contains settings for Fossil.
See also: http://www.fossil-scm.org/fossil/doc/trunk/www/settings.wiki

Assets:
A folder required by Unity.  Contains assets used for the game.

ProjectSettings:
A folder required by Unity.  Contains settings for Unity.

Builds:
A folder where its contents are ignored by Fossil.  Typically where game builds are created in.

UnimportedAssets:
A folder that Fossil versions, but Unity doesn't detect.  Typically where unfinished art assets are placed in.

Library:
A folder required by Unity.  It's contents are very cryptic and mysterious.  I wouldn't recommend touching it.
Ignored by Fossil.

Temp:
A folder used by Unity.  Unity creates it when an editor opens.  I wouldn't recommend touching it.
Ignored by Fossil.



As a reminder, I put a README.txt in each folder to describe it's purpose (except for Library and Temp, because that would be dangerous).
Also, it makes it easier to check those folders into Fossil.

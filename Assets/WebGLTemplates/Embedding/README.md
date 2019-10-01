This folder is a template that is selectable in the Unity WebGL Player settings.
It's intend to make uploading to web portals -- such as Itch.io, GameJolt, Newgrounds, and Kongregate -- a lot easier.

If this is an exported folder intended to be zipped for upload, take the following into consideration:


1) Do not zip or upload the "AcceptedDomains" folder. It simply contains files used to replace the file, "TemplateData/domains".

2) Do not zip or upload this README.txt, either.

3) If you are using WebLocationChecker script (by default, this project *is*), and decided on using the asset bundle file, remember to replace the "TemplateData/domains" file to the asset bundle that contains the domains you want your game to support. The "AcceptedDomains" folder contains some examples you can copy & paste.

3) If you are *not* using the WebLocationChecker script, you can delete "TemplateData/domains".


By default, the file, "TemplateData/domains" is a copy of "AcceptedDomains/unity-cloud".

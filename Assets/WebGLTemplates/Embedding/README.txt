This folder is a template that is selectable in the Unity WebGL Player settings.
It's intend to make uploading to web portals -- such as Itch.io, GameJolt, Newgrounds, and Kongregate -- a lot easier.

If this is an exported folder intended to be zipped for upload, take the following into consideration:
1) Do not zip or upload the "AcceptedDomains" folder.
    It simply contains files used to replace the "TemplateData/domains" or "TemplateData/domains.txt" files.
2) Do not zip or upload this README.txt, either.
3) If you are not using the WebLocationChecker script, you can delete "TemplateData/domains" and "TemplateData/domains.txt".
    They're not needed.
4) If you are using WebLocationChecker, and decided on using the asset bundle file, remember to replace the "TemplateData/domains"
    to the asset bundle that contains the domains you want your game to support.
    The "AcceptedDomains" folder contains some examples you can copy & paste.
    Also, you can delete "TemplateData/domains.txt", as that's for the text option (below).
5) If you are using WebLocationChecker, and decided on using the text file, remember to update the "TemplateData/domains.txt"
    to the domains you want your game to support.
    The "AcceptedDomains" folder contains some examples you can copy & paste.
    Also, you can delete "TemplateData/domains", as that's for the asset bundle option (above).

By default, the "TemplateData/domains.txt" and "TemplateData/domains" files are copies of
"AcceptedDomains/unity-cloud.txt" and "AcceptedDomains/unity-cloud" respectively.

This folder is a template that is selectable in the Unity WebGL Player settings.  It's a template made to be easily uploaded to any web portals.

If this is an exported folder intended to be zipped for upload, there might be a few things you may want to know:
1) Do not zip the "AcceptedDomains" folder.  It simply contains files used to replace the "TemplateData/domains" or "TemplateData/domains.txt" files.
2) If you are not using the WebLocationChecker script, you can delete "TemplateData/domains" or "TemplateData/domains.txt"; they're not needed.
3) If you are using WebLocationChecker, and decided on using the text file, remember to update the "TemplateData/domains.txt" to the domains you want your game to support.  The "AcceptedDomains" folder contains some examples you can copy & paste.  Also, you can delete "TemplateData/domains", as that's for the asset bundle option (below).
3) If you are using WebLocationChecker, and decided on using the asset bundle file, remember to replace the "TemplateData/domains" to the asset bundle that contains the domains you want your game to support.  The "AcceptedDomains" folder contains some examples you can copy & paste.  Also, you can delete "TemplateData/domains.txt", as that's for the text option (above).

By default, the "TemplateData/domains.txt" and "TemplateData/domains" files are the same copies as "AcceptedDomains/unity-cloud.txt" and "AcceptedDomains/unity-cloud" respectively.

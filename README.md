# AcdseeHelper

## Introduction
This project contains a number of tools that automate some of my photography workflows with ACDSee.  
They help with all kinds of processing like HDR and stereoscopic.  
ACDSee allows you to configure external editors and invoke them with the selected pictures.
So why not write my own 'external editors' to optimize my workflow :)

## Setup
I work with a fixed folder layout, which looks like this:  
![Folder layout.jpg](Docs/Folder%20layout.jpg)

Open the External Editors tool with the _Tools/External Editors/Configure Editors_ pull-down menu.
This pulldown menu can also be used to invoke the editors.
![External editors.jpg](Docs/External%20editors.jpg)
## The Tools

### MoveHDR
This tool reads the bracketing settings from the exif data and moves the file to the corresponding HDR source folder

### MoveStereo
For stereoscopic photography, I use the cha-cha method.  
I ise StereoPhotoMaker to assemble side-by-side stereo pictures.

My processing work is as follows:
- Select the left pictures
- Invoke the MoveStereo tool
  - This moves the left pictures to _Project/Stereo source/Left_
  - It selects all the pictures that were shot right after the left pictures, and moves them to _Project/Stereo source/Right_
- After that it's just a matter of firing up StereoPhotoMaker and starting a multi-conversion job.

## Acknowledgements
This project uses code from these repo's:
- Exif data parsing from [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet) by Drew Noakes

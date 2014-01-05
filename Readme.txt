Sorts pictures from a single directory (drop out of the camera) to a directory tree (MainPicDir / Year / Month).

Might become customizable some day.

GPL licensed.


* File creation date is considered by default

* Exif shooting date retrieved for jpg file

* For files named like this (as android phone names it's pics and vids):
	20120612_124502.jpg, 
	the file name is considered for the date.


Syntax:

SortPicture.exe <inputdir> <output dir>

File SortPicture.exe.config contains some settings regarding the type of files considered by the program


ToDo: 

- extract the shooting date from .mov and .mp4 video files.
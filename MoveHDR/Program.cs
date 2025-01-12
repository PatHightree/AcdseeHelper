using System.IO.Enumeration;
using MetadataExtractor;
using Directory = MetadataExtractor.Directory;

const string relativeHdrSourceFolderPath = "HDR source";

MoveHDR();
return;

void MoveHDR()
{
    List<string> args = Environment.GetCommandLineArgs().ToList();
    // Remove first arg, the executable path
    args.RemoveAt(0);
    
    foreach (string pictureFullPath in args)
    {
        FileInfo fileInfo = new FileInfo(pictureFullPath);
        int bracketCount = GetBracketSettings(fileInfo);
        if (bracketCount > 0)
        {
            // The output files of HDR processing still have the 'Bracket Settings' tag, avoid moving these
            if (FileSystemName.MatchesSimpleExpression("*HDR(*).*", pictureFullPath)) continue;
            
            // Determine the bracket count and construct corresponding destination path, then move file
            DirectoryInfo pictureFolder = fileInfo.Directory;
            string fileName = fileInfo.Name;
            string destFolder = Path.Combine(pictureFolder.ToString(), Path.Combine(relativeHdrSourceFolderPath, bracketCount.ToString()));
            string destFullPath = Path.Combine(destFolder, fileName);
            if (System.IO.Directory.Exists(destFolder))
            {
                File.Move(pictureFullPath, destFullPath);
            }
        }
    }
}

// Read 'Bracket Settings' from camera brand specific 'makernote' section in exif data
int GetBracketSettings(FileInfo _fileInfo)
{
    IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(_fileInfo.FullName);
    foreach (var directory in directories)
    foreach (var tag in directory.Tags)
    {
        if (tag.Name.ToLowerInvariant() != "bracket settings") continue;
        
        int bracketCount = 0;
        return int.TryParse(tag.Description[0].ToString(), out bracketCount) ? bracketCount : 0;
    }
    return 0;
}

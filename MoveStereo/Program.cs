const string relativeStereoLeftPath = "Stereo source/Left";
const string relativeStereoRightPath = "Stereo source/Right";

MoveStereo();
return;

void MoveStereo()
{
    // using StreamWriter logFile = new StreamWriter(Path.Combine("c:\\AcdseeHelper", "MoveHDR.log"));

    List<string> args = Environment.GetCommandLineArgs().ToList();
    // Remove first arg, the executable path
    args.RemoveAt(0);
    
    // Todo: Add robust method to find project folder

    string extension = new FileInfo(args.First()).Extension;
    
    // Collect all files in the project folder
    DirectoryInfo projectFolder = new DirectoryInfo(Path.GetDirectoryName(args[0]));
    List<FileInfo> fileInfos = projectFolder.GetFiles().ToList();
    
    fileInfos = fileInfos.FindAll(_f => _f.Extension == extension);
    
    // Construct left and right destination folders
    DirectoryInfo leftDestDirectory = new DirectoryInfo(Path.Combine(projectFolder.FullName, relativeStereoLeftPath));
    DirectoryInfo rightDestDirectory = new DirectoryInfo(Path.Combine(projectFolder.FullName, relativeStereoRightPath));
    
    foreach (string leftPicturePath in args)
    {
        // Find files
        FileInfo leftPicture = new FileInfo(leftPicturePath);
        int leftIndex = fileInfos.FindIndex(_f => _f.FullName == leftPicture.FullName);
        FileInfo rightPicture = fileInfos[leftIndex+1];
        
        // Copy left file and move right file to source folders
        string destLeftFullName = Path.Combine(leftDestDirectory.FullName, leftPicture.Name);
        string destRightFullName = Path.Combine(rightDestDirectory.FullName, rightPicture.Name);
        // logFile.WriteLine($"{leftPicture.FullName} -> {destLeftFullName}");
        // logFile.WriteLine($"{rightPicture.FullName} -> {destRightFullName}");
        File.Copy(leftPicture.FullName, destLeftFullName);
        File.Move(rightPicture.FullName, destRightFullName);
    }
}

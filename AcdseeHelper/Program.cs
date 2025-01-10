using System.IO.Enumeration;
using MetadataExtractor;
using Directory = MetadataExtractor.Directory;

// See https://aka.ms/new-console-template for more information
const string SnsHdrProFilePath = @"c:\program files\sns-hdr pro 2\sns-hdr pro.exe";
const string SnsHdrProDefaultArgs = "-q -default -jpeg -srgb ";
const string RelativeHdrSourceFolderPath = "HDR source";

// Executable name determines it's function
switch (Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs().First()))
{
    case "MoveHDR":
        MoveHDR();
        break;
    case "HDR5": 
        RunSnsHdrPro(5);
        break;
    default : return;
}
return;

void MoveHDR()
{
    List<string> args = Environment.GetCommandLineArgs().ToList();
    // Remove first arg, the executable path
    args.RemoveAt(0);
    
    // using StreamWriter logFile = new StreamWriter(Path.Combine("c:\\AcdseeHelper", "MoveHDR.log"));
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
            string destFolder = Path.Combine(pictureFolder.ToString(), Path.Combine(RelativeHdrSourceFolderPath, bracketCount.ToString()));
            string destFullPath = Path.Combine(destFolder, fileName);
            if (System.IO.Directory.Exists(destFolder))
            {
                // logFile.WriteLine($"{pictureFullPath} -> {destFullPath}");
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
        if (tag.Name.ToLowerInvariant() == "bracket settings")
        {
            int bracketCount = 0;
            int.TryParse(tag.Description[0].ToString(), out bracketCount);
            return bracketCount;
        }
    }
    return 0;
}

void RunSnsHdrPro(int _imageCount)
{
    Queue<string> arguments = new Queue<string>(Environment.GetCommandLineArgs());
    arguments.Dequeue();
    if (arguments.Count % _imageCount != 0)
        return;
    int jobCount = arguments.Count / _imageCount;
    for (int job = 0; job < jobCount; job++)
    {
        string firstFilename = arguments.First();
        string jobArguments = SnsHdrProDefaultArgs + SnsHdrProOutputFileArgument(_imageCount, firstFilename);
        for (int j = 0; j < _imageCount; j++)
            jobArguments += $"\"{arguments.Dequeue()}\" ";
        Console.WriteLine(jobArguments);
        using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
        {
            pProcess.StartInfo.FileName = SnsHdrProFilePath;
            pProcess.StartInfo.Arguments = jobArguments;
            //pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            pProcess.WaitForExit();
            Console.WriteLine(output);
        }
    }

    string SnsHdrProOutputFileArgument(int _imageCount, string _firstFilename)
    {
        return $"-o {Path.GetFullPath(_firstFilename)}/{Path.GetFileNameWithoutExtension(_firstFilename)}-HDR({_imageCount}){Path.GetExtension(_firstFilename)} ";
    }
}

using ExifLib;

// See https://aka.ms/new-console-template for more information
const string SnsHdrProFilePath = @"c:\program files\sns-hdr pro 2\sns-hdr pro.exe";
const string SnsHdrProDefaultArgs = "-q -default -jpeg -srgb ";
const float MinHdrTimeSpan = 0.5f;

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

void MoveHDR()
{
    TimeSpan minSpan = TimeSpan.FromSeconds(MinHdrTimeSpan);
    DateTime prevTime = default(DateTime);
    int consecutive = 0;

    List<string> args = Environment.GetCommandLineArgs().ToList();
    // First arg is executable path
    args.RemoveAt(0);
    
    using StreamWriter logFile = new StreamWriter(Path.Combine("c:\\Temp", "MoveHDR.log"));
    foreach (string arg in args)
    {
        FileInfo fileInfo = new FileInfo(arg);
        using ExifReader exifReader = new ExifReader(fileInfo.FullName);
        // exifReader.GetTagValue(ExifTags.DateTimeDigitized, out DateTime pictureTime);
        exifReader.GetTagValue(ExifTags.DateTimeOriginal, out DateTime pictureTime);
        // exifReader.GetTagValue(ExifTags.SubsecTime, out DateTime pictureTime);
        
        // // According to https://exiv2.org/tags-panasonic.html this should yield Exif.Panasonic.BracketSettings, but no
        // exifReader.GetTagValue(69, out int bracketSettings);
        // exifReader.GetTagValue(0x0045, out int bracketSettings);

        exifReader.GetTagValue(ExifTags.MakerNote, out object makerNote);
        
        Console.WriteLine(fileInfo.Name);
        logFile.WriteLine( $"{fileInfo.Name} : {pictureTime}");
        
        if (prevTime == default)
        {
            prevTime = pictureTime;
            continue;
        }
        TimeSpan delta = pictureTime - prevTime;
        if (delta < minSpan)
        {
            // Consecutive file
            consecutive++;
            prevTime = pictureTime;
        }
        else
        {
            Console.WriteLine();
            logFile.WriteLine(consecutive.ToString());
            consecutive = 0;
            prevTime = default;
        }
    }
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

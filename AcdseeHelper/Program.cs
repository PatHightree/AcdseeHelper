using ExifLib;

// See https://aka.ms/new-console-template for more information
const string SnsHdrProFilePath = @"c:\program files\sns-hdr pro 2\sns-hdr pro.exe";
const string SnsHdrProDefaultArgs = "-q -default -jpeg -srgb ";

// Executable name determines it's function
switch (Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs().First()))
{
    case "HDR5": 
        RunSnsHdrPro(5);
        break;
    default : return;
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

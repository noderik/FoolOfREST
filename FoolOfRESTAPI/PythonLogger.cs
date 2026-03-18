using System.Diagnostics;

internal class PythonLogger
{

    public string PythonScriptPath { get; set; }
    public string PythonScriptDirectory { get; set; }
    public string PythonExecutablePath { get; set; }
    public Process PythonProcess { get; init; }

    public PythonLogger()
    {
        string cwd = Directory.GetCurrentDirectory();
        PythonScriptDirectory = Path.Join(cwd, "TelegramLogger");
        string pythonVenvDirectory = Path.Join(PythonScriptDirectory, ".venv");

        if (Path.Exists(Path.Join(pythonVenvDirectory, "bin")) == true)
        {
            PythonExecutablePath = Path.Join(PythonScriptDirectory, ".venv", "bin", "python");
        }
        else if (Path.Exists(Path.Join(pythonVenvDirectory, "Scrpits")) == true)
        {
            PythonExecutablePath = Path.Join(PythonScriptDirectory, ".venv", "Scripts", "python");
        }
        else
        {
            throw new FileNotFoundException("Unable to find python executable inside .venv folder");
        }
        PythonScriptPath = Path.Join(PythonScriptDirectory, "main.py");
        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = PythonExecutablePath,
            Arguments = PythonScriptPath,
            WorkingDirectory = PythonScriptDirectory,
        };
        PythonProcess = Process.Start(processInfo) ?? throw new ArgumentNullException("Python logger didn't start properly.");
    }

    ~PythonLogger()
    {
        PythonProcess.Kill();
    }

}

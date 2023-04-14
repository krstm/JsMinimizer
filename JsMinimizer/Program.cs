using System.Diagnostics;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        string folderPath = "";

        string[] jsFiles = Directory.GetFiles(folderPath, "*.js", SearchOption.AllDirectories);

        foreach (string file in jsFiles)
        {
            string backupName = file.Substring(file.LastIndexOf("\\") + 1, file.LastIndexOf(".") - file.LastIndexOf("\\") - 1) + "_backup.js";
            string backupPath = file.Substring(0, file.LastIndexOf("\\")) + "\\" + backupName;

            if (!File.Exists(backupPath))
            {
                File.Copy(file, backupPath);
                Console.WriteLine("{0} yedeklendi.", file);
            }

            string compressedPath = Path.GetTempFileName();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c uglifyjs " + file + " --compress --mangle --output ascii_only=true -o " + compressedPath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            Encoding encoding = GetEncoding(file);

            using (StreamWriter writer = new StreamWriter(file, false, encoding))
            {
                using (StreamReader reader = new StreamReader(compressedPath))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }

            File.Delete(compressedPath);

            Console.WriteLine("{0} minimize edildi.", file);
        }
    }

    public static Encoding GetEncoding(string filename)
    {
        using (var reader = new StreamReader(filename, Encoding.Default, true))
        {
            if (reader.Peek() >= 0)
                reader.Read();

            return reader.CurrentEncoding;
        }
    }
}
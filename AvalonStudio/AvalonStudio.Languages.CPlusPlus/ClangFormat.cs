using AvalonStudio.Platforms;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace AvalonStudio.Languages.CPlusPlus
{
    public class ClangFormat
    {
        public string Text { get; private set; }
        public uint Cursor { get; private set; }

        public static XDocument FormatXml(string text, uint offset, uint length, uint cursor, ClangFormatSettings settings)
        {
            var startInfo = new ProcessStartInfo();
            var resultText = string.Empty;

            startInfo.FileName = Path.Combine(Platform.NativeFolder, "clang-format" + Platform.ExecutableExtension);
            startInfo.Arguments = string.Format("-offset={0} -length={1} -cursor={2} -style=\"{3}\" -output-replacements-xml",
                offset, length, cursor, settings);

            // Hide console window
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true; //we can get the erros text now.
            startInfo.RedirectStandardInput = true;
            startInfo.CreateNoWindow = true;

            using (var process = Process.Start(startInfo))
            {
                using (var streamWriter = process.StandardInput)
                {
                    streamWriter.Write(text);
                }

                using (var streamReader = process.StandardOutput)
                {
                    resultText = streamReader.ReadToEnd();
                }
            }

            return XDocument.Parse(resultText);
        }

        public static XDocument FormatXml(string fileName, string text, uint offset, uint length, uint cursor)
        {
            var startInfo = new ProcessStartInfo();
            var resultText = string.Empty;
            startInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            startInfo.FileName = Path.Combine(Platform.NativeFolder, "clang-format" + Platform.ExecutableExtension);
            startInfo.Arguments = string.Format("-offset={0} -length={1} -cursor={2} -fallback-style=none -assume-filename=\"{3}\" -style=file -output-replacements-xml",
                offset, length, cursor, fileName);

            // Hide console window
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true; //we can get the erros text now.
            startInfo.RedirectStandardInput = true;
            startInfo.CreateNoWindow = true;

            using (var process = Process.Start(startInfo))
            {
                using (var streamWriter = process.StandardInput)
                {
                    streamWriter.Write(text);
                }

                using (var streamReader = process.StandardOutput)
                {
                    resultText = streamReader.ReadToEnd();
                }
            }

            if(string.IsNullOrEmpty(resultText))
            {
                return null;
            }

            return XDocument.Parse(resultText);
        }
    }
}
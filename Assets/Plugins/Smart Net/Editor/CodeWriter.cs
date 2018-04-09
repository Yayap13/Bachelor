using System.IO;
using System.Text;

namespace SmartNet
{
    internal class CodeWriter
    {
        private StringBuilder sb = new StringBuilder(1024);

        public int IndentLevel { get; private set; }

        public CodeWriter()
        {
        }

        public CodeWriter(int indentLevel)
        {
            IndentLevel = indentLevel;
        }

        public void WriteIndent()
        {
            for (var i = 0; i < IndentLevel; ++i)
            {
                sb.Append("\t");
            }
        }

        public void Write(char c)
        {
            sb.Append(c);
        }

        public void Write(string str)
        {
            sb.Append(str);
        }

        public void WriteLine(string line, bool autoIndent = true)
        {
            if (autoIndent && line.Length > 0)
            {
                if (line[0] == '}')
                {
                    RemoveIndent();
                }
            }

            WriteIndent();

            sb.Append(line);

            sb.Append("\n");

            if (autoIndent && line.Length > 0)
            {
                if (line[0] == '{')
                {
                    AddIndent();
                }
            }
        }

        public void AddIndent()
        {
            ++IndentLevel;
        }

        public void RemoveIndent()
        {
            --IndentLevel;
        }

        public void WriteToFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, sb.ToString());
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
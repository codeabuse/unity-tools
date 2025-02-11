using System.IO;
using System.Text.RegularExpressions;

namespace Codeabuse
{
    public static class PathTools
    {
        public static readonly char[] FORBIDDEN_NAME_CHARACTERS = Path.GetInvalidFileNameChars();
        public static readonly Regex EscapeForbidden = new Regex(
                $"[{Regex.Escape(new string(FORBIDDEN_NAME_CHARACTERS))}]");

        public static bool TrimAndValidateFileName(string name, out string trimmed)
        {
            return IsValidFileName(trimmed = name.Trim());
        }

        public static bool IsValidFileName(string name)
        {
            return !string.IsNullOrEmpty(name) && !EscapeForbidden.IsMatch(name);
        }
    }
}
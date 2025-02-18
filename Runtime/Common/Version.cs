using System;
using System.Text;

namespace Codeabuse
{
    public readonly struct Version : IComparable<Version>
    {
        private readonly uint[] _registers;

        public Version(params uint[] registers)
        {
            if (registers.Length == 0)
                throw new ArgumentException("Version must have at least one digit");
            _registers = registers;
        }

        public uint[] Registers => _registers;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendJoin('.', _registers);
            return sb.ToString();
        }

        /// <summary>
        /// Alphabetical symbols are not supported. Only numerical representations, like "1.0.0".
        /// </summary>
        public static bool TryParse(string versionString, out Version version)
        {
            version = default;
            if (string.IsNullOrEmpty(versionString))
            {
                return false;
            }
            var split = versionString.Split('.');
            var registers = new uint[split.Length];
            for (var i = 0; i < split.Length; i++)
            {
                if (uint.TryParse(split[i], out registers[i]))
                    continue;
                return false; //TODO: add support for alphabetical symbols (i.e. "1.0.1a")
            }

            version = new Version(registers);
            return true;
        }

        public int CompareTo(Version other)
        {
            var minRegister = Math.Min(Registers.Length, other.Registers.Length);
            for (var i = 0; i < minRegister; i++)
            {
                var diff = (int)Registers[i] - (int)other.Registers[i];
                if (diff != 0)
                    return diff;
            }

            return 0;
        }
    }
}
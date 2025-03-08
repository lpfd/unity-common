using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Leap.Forward.Unity.Common
{
    public class GeneratorBase
    {
        private static readonly HashSet<char> InvalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars().Concat(new[] { '<', '>' }));

        public GeneratorBase(ITypeSymbol typeSymbolInfo, GeneratorExecutionContext context)
        {
            ClassSymbol = typeSymbolInfo;
            Context = context;
        }

        public ITypeSymbol ClassSymbol { get; }
        public GeneratorExecutionContext Context { get; }

        protected string GetClassNameWithoutNamespace(ITypeSymbol typeSymbol)
        {
            var fullClassName = typeSymbol.ToString();
            return fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
        }

        protected static string SanitizeFileName(string fullClassName)
        {
            var sb = new StringBuilder(fullClassName.Length);
            foreach (var c in fullClassName)
            {
                if (InvalidFileNameChars.Contains(c))
                    sb.Append('_');
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

    }

}
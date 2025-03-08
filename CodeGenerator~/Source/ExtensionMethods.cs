using Microsoft.CodeAnalysis;
using System.Linq;

namespace Leap.Forward.Unity.Common
{
    public static class ExtensionMethods
    {
        public static bool Is(this ISymbol? symbol, NameAndNamespace name)
        {
            if (symbol == null)
                return false;
            if (symbol.Name != name.Name)
                return false;
            if (symbol.ContainingNamespace == null)
                return string.IsNullOrEmpty(name.Namespace);
            if (symbol.ContainingNamespace.IsGlobalNamespace)
                return string.IsNullOrEmpty(name.Namespace);
            return symbol.ContainingNamespace.ToDisplayString() == name.Namespace;
        }

        public static bool ImplementsInterface(this ITypeSymbol? typeSymbol, NameAndNamespace name)
        {
            if (typeSymbol == null)
                return false;

            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                if (@interface.Is(name))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ImplementsInterface(this ITypeSymbol? typeSymbol, NameAndNamespace name, out ITypeSymbol genericArgument)
        {
            genericArgument = null;
            if (typeSymbol == null)
                return false;

            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                if (@interface.IsGenericType && @interface.Is(name))
                {
                    genericArgument = @interface.TypeArguments.First();
                    return true;
                }
            }

            return false;
        }

    }

}
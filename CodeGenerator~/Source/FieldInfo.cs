using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Leap.Forward.Unity.Common
{
    public class FieldInfo
    {
        public FieldInfo(IFieldSymbol symbol)
        {
            Symbol = symbol;
            Name = symbol.Name;
            FieldType = symbol.Type.ToDisplayString();

            var syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxReference != null)
            {
                var syntaxNode = syntaxReference.GetSyntax() as VariableDeclaratorSyntax;
                bool hasInitializer = syntaxNode?.Initializer != null;
            }
            var fieldName = Name.Trim('_');
            PropertyName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);

            // Iterate through all interfaces implemented by the field's type
            foreach (var iface in symbol.Type.AllInterfaces)
            {
                // Check if the interface is generic and named "IList"
                if (iface is INamedTypeSymbol namedInterface &&
                    namedInterface.Name == "ICollection" &&
                    namedInterface.TypeArguments.Length == 1)
                {
                    // Verify the interface's namespace is "System.Collections.Generic"
                    var namespaceName = namedInterface.ContainingNamespace.ToDisplayString();
                    if (namespaceName == "System.Collections.Generic")
                    {
                        CollectionElementType = namedInterface.TypeArguments[0].ToDisplayString();
                        break;
                    }
                }
            }
        }

        public string Name { get; }

        public string PropertyName { get; }

        public string FieldType { get; }

        public string CollectionElementType { get; }

        public IFieldSymbol Symbol { get; set; }
    }
}
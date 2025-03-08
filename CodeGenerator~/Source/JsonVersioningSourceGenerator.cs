using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Leap.Forward.Unity.Common
{
    [Generator]
    public class JsonVersioningSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                // Get the root of the syntax tree
                var root = syntaxTree.GetRoot();

                // Find all class declarations
                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDeclaration in classDeclarations)
                {
                    var typeSymbolInfo = compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration) as ITypeSymbol;

                    if (typeSymbolInfo == null)
                        continue;

                    var newtofsoftAttr = typeSymbolInfo.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name == "JsonVersionAttribute");
                    if (newtofsoftAttr != null)
                    {
                        var gen = new NewtonsoftSerializerGenerator(typeSymbolInfo, newtofsoftAttr, context);
                        gen.Generate();

                    }
                }
            }

        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }

}
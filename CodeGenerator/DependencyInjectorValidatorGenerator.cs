using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace CodeGenerator;
[Generator]
public class DependencyInjectorValidatorGenerator : IIncrementalGenerator
{
    List<string> controllerTypes = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //#if DEBUG
        //        Debugger.Launch();
        //#endif
        IncrementalValuesProvider<ClassDeclarationSyntax> provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (SyntaxNode syntaxNode, CancellationToken cancellationToken) => syntaxNode is ClassDeclarationSyntax,
            transform: (GeneratorSyntaxContext generatorSyntaxContext, CancellationToken cancellationToken) =>
            {
                var symbol = generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(generatorSyntaxContext.Node);
                if (symbol is INamedTypeSymbol)
                {
                    INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)symbol;
                    if (namedTypeSymbol.BaseType.Name == "ControllerBase")
                    {
                        StringBuilder sb = new();
                        sb.Insert(0, namedTypeSymbol.Name);
                        INamespaceSymbol nameSpace = namedTypeSymbol.ContainingNamespace;
                        //sb.Insert(0, ".");
                        //sb.Insert(0, nameSpace.Name);
                        while (nameSpace != null &&
                            nameSpace.ContainingNamespace != null)
                        {
                            sb.Insert(0, ".");
                            sb.Insert(0, nameSpace.Name);
                            nameSpace = nameSpace.ContainingNamespace;
                        }
                        controllerTypes.Add(sb.ToString());
                        return (ClassDeclarationSyntax)generatorSyntaxContext.Node;
                    }
                }
                return null;
            }
            )
            .Where(x => x != null)!;

        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            (sourceProductionContext, source) =>
            Execute(sourceProductionContext, source.Left, source.Right));
    }

    private void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> typeList)
    {

        StringBuilder sb = new StringBuilder();

        sb.Append(
                $$"""
                using Microsoft.AspNetCore.Mvc;
                using Serilog;
                using System.Collections.Generic;
                using System.Text;
                namespace SampleSourceGenerator
                {
                    public static class DependencyInjectorValidator
                    {
                        public static DependencyInjectionValidationResult Validate(WebApplication app)
                        {
                            var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
                            StringBuilder message = new();
                            bool isError = false;
                            using (var scope = scopeFactory.CreateScope())
                            {

            """);
        foreach (string controllerName in controllerTypes)
        {
            //var controllerType = .ToString;
            sb.Append($$"""
                                try{
                                    var controller = scope.ServiceProvider.GetServices(Type.GetType("{{controllerName}}"));
                                } catch(InvalidOperationException ex){
                                    isError = true;
                                    message.AppendLine(ex.Message);
                                }
                """);
        }

        sb.Append("""
                            }
                            return new DependencyInjectionValidationResult(isError, message.ToString());
                        }
                    }

                    public class DependencyInjectionValidationResult
                    {
                        public bool IsError {get;}
                        public string Message {get;}

                        public DependencyInjectionValidationResult(bool isError, string message)
                        {
                            IsError = isError;
                            Message = message;
                        }
                    }
                }
                """); ;

        context.AddSource("DependencyInjectorValidator.g.cs", sb.ToString());
    }
}

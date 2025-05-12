using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace CodeGenerator
{
    [Generator]
    public class ControllerGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //#if DEBUG
            //            //Debugger.Launch();
            //#endif
            IncrementalValuesProvider<ClassDeclarationSyntax> provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (SyntaxNode syntaxNode, CancellationToken cancellationToken) => syntaxNode is ClassDeclarationSyntax,
                transform: (GeneratorSyntaxContext generatorSyntaxContext, CancellationToken cancellationToken) => (ClassDeclarationSyntax)generatorSyntaxContext.Node

                )
                .Where(x => x != null);

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
            var code = """
                using Microsoft.AspNetCore.Mvc;
                namespace WebApp.Controllers
                {
                    [ApiController]
                    [Route("[controller]")]
                    public class GeneratedController : ControllerBase
                    {
                        [HttpGet()]
                        public string Hello()
                        {
                            return "Hello World!";
                        }
                    }
                }
                """;

            context.AddSource("ClassNames.g.cs", code);
        }
    }
}

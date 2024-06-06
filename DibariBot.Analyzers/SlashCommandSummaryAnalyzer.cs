using System.Collections.Immutable;
using System.Linq;
using Discord.Interactions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DibariBot.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SlashCommandSummaryAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "DA0001";
    private const string Title = "Slash command parameter is missing a description";
    private const string MessageFormat = "Slash command parameter '{0}' is missing a description";
    private const string Description = "Slash command parameters should have a description.";
    private const string Category = "Usage";

#pragma warning disable RS2008 // Enable analyzer release tracking
    private static DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
#pragma warning restore RS2008 // Enable analyzer release tracking

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration)!;
        var methodAttributes = methodSymbol.GetAttributes();
        if (!methodAttributes.Any(a => a.AttributeClass?.Name == nameof(SlashCommandAttribute)))
        {
            return;
        }

        foreach (var parameter in methodDeclaration.ParameterList.Parameters)
        {
            var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter);
            var summaryAttribute = parameterSymbol!.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass!.Name.ToString() == nameof(SummaryAttribute));

            if (summaryAttribute is null)
            {
                var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), parameter.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
                continue;
            }
        }

        // TODO: check if the summary has the description set
    }
}
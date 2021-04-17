using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoobSwarm.Generators
{
    [Generator]
    public class NotifyPropertyChangedGenerator : ISourceGenerator
    {
        private const string fieldAttribute = @"
using System;
namespace NoobSwarm.Avalonia
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""PropertyChangedAvaloniaGenerator_DEBUG"")]
    sealed class PropertyChangedAvaloniaAttribute : Attribute
    {
        public PropertyChangedAvaloniaAttribute()
        {
        }
        public string PropertyName { get; set; }
    }
}
";
        private const string classAttribute = @"
using System;
namespace NoobSwarm.Avalonia
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""ImplementPropertyChangedAvaloniaGenerator_DEBUG"")]
    sealed class ClassPropertyChangedAvaloniaAttribute : Attribute
    {
        public ClassPropertyChangedAvaloniaAttribute(Type typeName)
        {
            TypeName = typeName;
        }
        public Type TypeName { get; }
    }
}
";


        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("PropertyChangedAvaloniaAttribute", fieldAttribute);
                i.AddSource("ClassPropertyChangedAvaloniaAttribute", classAttribute);
            });

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            // get the added attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("NoobSwarm.Avalonia.PropertyChangedAvaloniaAttribute");
            INamedTypeSymbol notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
            INamedTypeSymbol reactiveUiSymbol = context.Compilation.GetTypeByMetadataName("ReactiveUI.IReactiveObject");


            foreach (var group in receiver.Classes)
            {
                string classSource = ProcessClass(group.Key as INamedTypeSymbol, group.Value, notifySymbol, reactiveUiSymbol, attributeSymbol);
                context.AddSource($"{group.Key.Name}_autoNotifyAvalonia.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, SyntaxReceiver.ClassThingy thingy, ISymbol notifySymbol, INamedTypeSymbol reactiveUiSymbol, INamedTypeSymbol attributeSymbol)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            bool isReactiveObj = classSymbol.AllInterfaces.Contains(reactiveUiSymbol);
            bool isNotifyPropChanged = classSymbol.AllInterfaces.Contains(notifySymbol);
            string additionalUsings = string.Empty;
            if (isReactiveObj)
                additionalUsings = "using ReactiveUI;";
            else 
                additionalUsings = "using System.Runtime.CompilerServices;";

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
{additionalUsings}
namespace {namespaceName}
{{
    public partial class {classSymbol.Name}");

            // if the class doesn't implement INotifyPropertyChanged already, add it
            if (!isReactiveObj)
            {
                if (!isNotifyPropChanged)
                {
                    source.Append($@" : {notifySymbol.ToDisplayString()} {{
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
                }
                else
                {

                    source.Append($@"{{
            protected T RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
            {{
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return value;
                field = value;
                ProperyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return value;
            }}");
                }
            }
            else
                source.Append(" {");

            foreach (IPropertySymbol propSymbol in thingy.Properties)
            {
                ProcessProp(source, propSymbol);
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in thingy.Fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            source.Append("} }");
            return source.ToString();
        }


        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the AutoNotify attribute from the field, and any associated data
            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.Append($@"
        public {fieldType} {propertyName} 
        {{
            get => this.{fieldName};
            set => this.RaiseAndSetIfChanged(ref {fieldName}, value);
        }}
");

            string chooseName(string fieldName, TypedConstant overridenNameOpt)
            {
                if (!overridenNameOpt.IsNull)
                {
                    return overridenNameOpt.Value.ToString();
                }

                fieldName = fieldName.TrimStart('_');
                if (fieldName.Length == 0)
                    return string.Empty;

                if (fieldName.Length == 1)
                    return fieldName.ToUpper();

                return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
            }

        }

        private void ProcessProp(StringBuilder source, IPropertySymbol propSymbol)
        {
            // get the name and type of the field
            string propName = propSymbol.Name;
            ITypeSymbol fieldType = propSymbol.Type;

            string fieldName = "_" + char.ToLower(propName[0]) + propName.Substring(1);

            source.Append($@"
        public {fieldType} {propName} 
        {{
            get => this.{fieldName};
            set => this.RaiseAndSetIfChanged(ref {fieldName}, value);
        }}

        private {fieldType} {fieldName};
");
        }


        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public class ClassThingy
            {
                public ClassThingy(ISymbol classSymbol)
                {
                    ClassSymbol = classSymbol;
                }
                public ISymbol ClassSymbol { get; }
                public List<IPropertySymbol> Properties { get; } = new List<IPropertySymbol>();
                public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();
            }
            public Dictionary<ISymbol, ClassThingy> Classes { get; } = new();
            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // any field with at least one attribute is a candidate for property generation
                if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    foreach (VariableDeclaratorSyntax variable in fieldDeclarationSyntax.Declaration.Variables)
                    {
                        // Get the symbol being declared by the field, and keep it if its annotated
                        IFieldSymbol fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                        if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "NoobSwarm.Avalonia.PropertyChangedAvaloniaAttribute"))
                        {
                            var symbol = fieldSymbol.ContainingType as ISymbol;
                            if (!Classes.TryGetValue(symbol, out var thingy))
                                Classes[symbol] = thingy = new ClassThingy(symbol);
                            thingy.Fields.Add(fieldSymbol);
                        }
                    }
                }
                else if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.AttributeLists.Count > 0)
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    var symbolAttr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass.ToDisplayString() == "NoobSwarm.Avalonia.ClassPropertyChangedAvaloniaAttribute");
                    if (symbolAttr is not null)
                    {
                        if (!Classes.TryGetValue(symbol, out var thingy))
                            Classes[symbol] = thingy = new ClassThingy(symbol);

                        var typeToGenerateFor = symbolAttr.ConstructorArguments.First().Value as ITypeSymbol;
                        var tmp = (ClassDeclarationSyntax)typeToGenerateFor.DeclaringSyntaxReferences.First().GetSyntax();
                        foreach (var p in tmp.Members)
                        //foreach (var propSymbol in typeToGenerateFor..OfType<IPropertySymbol>()) Mach was du meinst
                        {
                            var prop = context.SemanticModel.GetDeclaredSymbol(p) as IPropertySymbol;

                            thingy.Properties.Add(prop);
                            // Get the symbol being declared by the field, and keep it if its annotated

                            //Properties.Add(propSymbol);
                        }
                    }
                }

            }
        }
    }
}

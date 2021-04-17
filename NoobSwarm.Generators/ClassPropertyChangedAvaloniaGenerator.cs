//using Microsoft.CodeAnalysis.Text;
//using Microsoft.CodeAnalysis;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace NoobSwarm.Generators
//{
//    [Generator]
//    public class ClassPropertyChangedAvaloniaGenerator : ISourceGenerator
//    {
//        private const string attributeText = @"
//using System;
//namespace NoobSwarm.Avalonia
//{

//    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
//    [System.Diagnostics.Conditional(""ImplementPropertyChangedAvaloniaGenerator_DEBUG"")]
//    sealed class ClassPropertyChangedAvaloniaAttribute : Attribute
//    {
//        public ClassPropertyChangedAvaloniaAttribute(Type typeName)
//        {
//            TypeName = typeName;
//        }
//        public Type TypeName { get; }
//    }
//}
//";


//        public void Initialize(GeneratorInitializationContext context)
//        {

//            // Register the attribute source
//            context.RegisterForPostInitialization((i) => i.AddSource("ClassPropertyChangedAvaloniaAttribute", attributeText));

//            // Register a syntax receiver that will be created for each generation pass
//            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
//        }

//        public void Execute(GeneratorExecutionContext context)
//        {
//            // retrieve the populated receiver 
//            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
//                return;

//            // get the added attribute, and INotifyPropertyChanged
//            INamedTypeSymbol notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
//            INamedTypeSymbol reactiveUiSymbol = context.Compilation.GetTypeByMetadataName("ReactiveUI.IReactiveObject");

//            // group the fields by class, and generate the source
//            foreach (var group in receiver.Properties)
//            {
//                string classSource = ProcessClass(group.Key as INamedTypeSymbol, group.Value, notifySymbol, reactiveUiSymbol);
//                context.AddSource($"{group.Key.Name}_autoNotifyAvalonia.cs", SourceText.From(classSource, Encoding.UTF8));
//            }
//        }

//        private string ProcessClass(INamedTypeSymbol classSymbol, List<IPropertySymbol> props, ISymbol notifySymbol, INamedTypeSymbol reactiveUiSymbol)
//        {
//            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
//            {
//                return null; //TODO: issue a diagnostic that it must be top level
//            }

//            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
//            bool isReactiveObj = classSymbol.AllInterfaces.Contains(reactiveUiSymbol);
//            bool isNotifyPropChanged = classSymbol.AllInterfaces.Contains(notifySymbol);
//            string additionalUsings = string.Empty;
//            if (isReactiveObj)
//                additionalUsings = "using ReactiveUI;";


//            // begin building the generated source
//            StringBuilder source = new StringBuilder($@"
//{additionalUsings}
//namespace {namespaceName}
//{{
//    public partial class {classSymbol.Name}");

//            // if the class doesn't implement INotifyPropertyChanged already, add it
//            if (!isReactiveObj)
//            {
//                if (!isNotifyPropChanged)
//                {
//                    source.Append($@" : {notifySymbol.ToDisplayString()} {{
//            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
//                }
//                else
//                {

//                    source.Append($@"{{
//            protected T RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
//            {{
//                if (EqualityComparer<T>.Default.Equals(field, value))
//                    return value;
//                field = value;
//                ProperyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//                return value;
//            }}");
//                }
//            }
//            else
//                source.Append(" {");
//            // create properties for each field 
//            foreach (IPropertySymbol propSymbol in props)
//            {
//                ProcessProp(source, propSymbol);
//            }

//            source.Append("} }");
//            return source.ToString();
//        }

//        private void ProcessProp(StringBuilder source, IPropertySymbol propSymbol)
//        {
//            // get the name and type of the field
//            string propName = propSymbol.Name;
//            ITypeSymbol fieldType = propSymbol.Type;

//            string fieldName = "_" + char.ToLower(propName[0]) + propName.Substring(1);

//            source.Append($@"
//public {fieldType} {propName} 
//{{
//    get 
//    {{
//        return this.{fieldName};
//    }}
//    set
//    {{
//        this.RaiseAndSetIfChanged(ref {fieldName}, value);
//    }}
//}}

//private {fieldType} {fieldName};
//");
//        }

//        /// <summary>
//        /// Created on demand before each generation pass
//        /// </summary>
//        class SyntaxReceiver : ISyntaxContextReceiver
//        {
//            public Dictionary<ISymbol, List<IPropertySymbol>> Properties { get; } = new();

//            /// <summary>
//            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
//            /// </summary>
//            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//            {

//                // any field with at least one attribute is a candidate for property generation
//                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
//                    && classDeclarationSyntax.AttributeLists.Count > 0)
//                {
//                    var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
//                    var symbolAttr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass.ToDisplayString() == "NoobSwarm.Avalonia.ClassPropertyChangedAvaloniaAttribute");
//                    if (symbolAttr is not null)
//                    {
//                        Properties[symbol] = new List<IPropertySymbol>();
//                        var typeToGenerateFor = symbolAttr.ConstructorArguments.First().Value as ITypeSymbol;
//                        var tmp = (ClassDeclarationSyntax)typeToGenerateFor.DeclaringSyntaxReferences.First().GetSyntax();
//                        foreach (var p in tmp.Members)
//                        //foreach (var propSymbol in typeToGenerateFor..OfType<IPropertySymbol>()) Mach was du meinst
//                        {
//                            var prop = context.SemanticModel.GetDeclaredSymbol(p) as IPropertySymbol;

//                            Properties[symbol].Add(prop);
//                            // Get the symbol being declared by the field, and keep it if its annotated

//                            //Properties.Add(propSymbol);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}

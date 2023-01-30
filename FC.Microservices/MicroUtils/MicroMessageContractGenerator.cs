using System.Text;
using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using Type = System.Type;

namespace FCMicroservices.MicroUtils;

public class MicroMessageContractGenerator
{
    public MicroMessageContracts Generate()
    {
        var allTypes = AssemblyUtils.SearchTypes();
        var types = allTypes
            .Where(x =>
                x.HasAttribute<CommandAttribute>() ||
                x.HasAttribute<QueryAttribute>() ||
                x.HasAttribute<MicroMessageAttribute>()
            )
            .Where(x => !x.IsAbstract)
            // .Where(x => x.IsPublic)
            .Where(x => x.IsClass)
            .ToList();

        var result = new MicroMessageContracts();
        var contracts = new List<MicroMessageContract>();
        foreach (var type in types)
        {
            var contract = GenerateContract(type);
            contracts.Add(contract);
        }

        contracts = contracts
            .OrderBy(x => x.Namespace)
            .ThenBy(x => x.File).ToList();

        result.Messages.AddRange(contracts);
        result.Total = contracts.Count();

        return result;
    }

    private CSharpGeneratorSettings CodeSettings(string ns)
    {
        return new CSharpGeneratorSettings
        {
            Namespace = ns,
            HandleReferences = false,
            DateType = "System.DateTime",
            ClassStyle = CSharpClassStyle.Poco,
            // TODO : TemplateFactory = Template icin liquid dili kullanilmis. Ekleyebiliriz bize ozel istersek
            GenerateDataAnnotations = false,
            GenerateDefaultValues = false,
            SchemaType = SchemaType.OpenApi3,
            DictionaryBaseType = "System.Collections.Generic.IDictionary",
            DictionaryType = "System.Collections.Generic.IDictionary",
            JsonLibrary = CSharpJsonLibrary.NewtonsoftJson,
            ArrayType = "System.Collections.Generic.IEnumerable",
            ArrayBaseType = "System.Collections.Generic.IEnumerable",
            InlineNamedAny = true,
            InlineNamedArrays = true,
            ExcludedTypeNames = new[]
            {
                "Command", "CommandReply", "Query", "QueryReply", "QueryFilter"
            }
        };
    }

    private MicroMessageContract GenerateContract(Type type)
    {
        var (content, sampleJson) = GenerateCode(type);
        var contentAsBinary = Encoding.UTF8.GetBytes(content);
        var sample = Struct.Parser.ParseJson("{}");
        if (!string.IsNullOrWhiteSpace(sampleJson)) sample = Struct.Parser.ParseJson(sampleJson);

        return new MicroMessageContract
        {
            FullName = type.Namespace + "." + type.Name,
            File = type.Name + ".cs",
            Namespace = type.Namespace,
            Sample = sample,
            Content = ByteString.CopyFrom(contentAsBinary)
        };
    }

    public (string content, string sampleJson) GenerateCode(Type type)
    {
        var schema = JsonSchema.FromType(type);
        schema.Description =
            $"{type.Name}\nTicimax Micro Messages\n({type.Assembly.FullName})\nGenerated At : {DateTime.Now}\n";
        var ns = $"{type.Namespace}";

        var settings = CodeSettings(ns);
        schema.SchemaVersion = ns;
        var customResolver = new CustomResolver(settings);
        var content = new CSharpGenerator(schema, settings, customResolver)
            .GenerateFile();

        var sampleJson = schema.ToSampleJson()?.ToString();
        return (content, sampleJson);
    }

    public class CustomResolver : CSharpTypeResolver
    {
        public CustomResolver(CSharpGeneratorSettings settings) : base(settings)
        {
        }

        public override string Resolve(JsonSchema schema, bool isNullable, string typeNameHint)
        {
            if (schema.ActualProperties.Count() == 0)
                schema.Properties["__dont_use"] = new JsonSchemaProperty
                {
                    Description =
                        "Kullanmayin. Islevi olmayan ozellik. Sinif bos oldugunda tip olusmasi icin gecici ozellik verildi",
                    Type = JsonObjectType.String
                };
            return base.Resolve(schema, isNullable, typeNameHint);
        }
    }
}
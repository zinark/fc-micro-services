using System.Diagnostics;
using System.Reflection;

namespace FCMicroservices.Utils;

public static class AssemblyUtils
{
    public static string API_TITLE => Assembly.GetEntryAssembly().GetName().Name;
    public static string API_VERSION => Assembly.GetEntryAssembly().GetName().Version.ToString();
    public static string API_FULL_NAME => API_TITLE + " " + API_VERSION;

    public static IEnumerable<Type> SearchTypes()
    {
        var restirectedNames = new List<string>()
        {
            "System.",
            "Microsoft.",
            "Newtonsoft.",
            "OpenTelemetry",
            "Swashbuckle.",
            "Grpc.",
            "Google.",
            "Npgsql.",
            "EFCore.",
            "NJsonSchema"
        };

        var q = AppDomain.CurrentDomain.GetAssemblies().AsQueryable();
        foreach (var name in restirectedNames)
        {
            q = q.Where(x => !x.FullName.StartsWith(name));
        };

        var asmlist = q.ToArray();


        var refList = new List<Assembly>();
        refList.AddRange(asmlist);

        var entryAsm = Assembly.GetEntryAssembly();

        //foreach (var entryAsm in asmlist)
        //{
        foreach (var refAsm in entryAsm.GetReferencedAssemblies())
        {
            if (startsWith(refAsm.Name, restirectedNames)) continue;


            var refNames = refList.Select(x => x.GetName().Name).ToList();

            if (refNames.Contains(refAsm.Name)) continue;

            var loaded = Assembly.Load(refAsm);
            refList.Add(loaded);
        }
        //}

        var result = SearchTypes(refList.ToArray());
        return result;
    }

    private static bool startsWith(string? name, List<string> restirectedNames)
    {
        foreach (var rest in restirectedNames)
        {
            if (name.StartsWith(rest)) return true;
        }
        return false;
    }

    public static IEnumerable<Type> SearchTypes(params Assembly[] assemblies)
    {
        var result = new List<Type>();

        Dictionary<string, Assembly> asmDict = new();

        var asmList = new List<Assembly>() { Assembly.GetCallingAssembly() };
        asmList.AddRange(assemblies);

        foreach (Assembly asm in asmList)
        {
            asmDict[asm.FullName] = asm;
        }

        foreach (Assembly asm in asmDict.Values)
        {
            var types = ListTypes(asm);
            if (types != null && types.Length > 0)
            {
                result.AddRange(types);
            }

        }

        return result;
    }

    public static Type[] ListTypes(Assembly asm)
    {
        try
        {
            var result = asm.GetExportedTypes()
                .Where(x => x.IsClass == true)
                .ToArray();
            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"INFO : {asm.FullName} assembly'sinden type listesi alirken problem olustu. {ex.Message}");
        }
        return null;
    }

    public static bool HasAttribute<T>(this Type type) where T : Attribute
    {
        var attr = type.GetCustomAttribute<T>();
        if (attr != null) return true;
        return false;
    }
}

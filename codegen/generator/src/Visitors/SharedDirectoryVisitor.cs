using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using System.IO;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Routes select generated types into the shared source tree so regenerated code matches the new layout.
/// </summary>
public class SharedDirectoryVisitor : ScmLibraryVisitor
{
    protected override TypeProvider VisitType(TypeProvider type)
    {
        var fileName = Path.GetFileName(type.RelativeFilePath);
        if (string.IsNullOrEmpty(fileName))
        {
            return type;
        }

        switch (type.Name)
        {
            case "OpenAIContext":
                type.Update(relativeFilePath: Path.Combine("src", "Shared", "Generated", "Models", fileName));
                break;
            case "ChangeTrackingDictionary":
            case "ChangeTrackingList":
                type.Update(relativeFilePath: Path.Combine("src", "Shared", "Generated", "Internal", fileName));
                break;
            case "Optional":
                type.Update(relativeFilePath: Path.Combine("src", "Shared", "Utility", fileName));
                break;
        }

        return type;
    }
}

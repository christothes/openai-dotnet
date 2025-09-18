using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Moderations;

internal static partial class ModerationApplicableInputKindsExtensions
{
    internal static IList<ModerationApplicableInputKinds> ToInternalApplicableInputKinds(this ModerationApplicableInputKinds inputKinds)
    {
        List<ModerationApplicableInputKinds> internalInputKinds = [];
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Text))
        {
            internalInputKinds.Add(ModerationApplicableInputKinds.Text);
        }
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Image))
        {
            internalInputKinds.Add(ModerationApplicableInputKinds.Image);
        }
        // if (inputKinds.HasFlag(ModerationInputKinds.Audio))
        // {
        //     internalInputKinds.Add("audio");
        // }
        return internalInputKinds;
    }

     internal static IList<BinaryData> ToInternalApplicableInputKindsBinaryData(this ModerationApplicableInputKinds inputKinds)
    {
        List<BinaryData> internalInputKinds = [];
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Text))
        {
            internalInputKinds.Add(new BinaryData("text"));
        }
        if (inputKinds.HasFlag(ModerationApplicableInputKinds.Image))
        {
            internalInputKinds.Add(new BinaryData("image"));
        }
        // if (inputKinds.HasFlag(ModerationInputKinds.Audio))
        // {
        //     internalInputKinds.Add("audio");
        // }
        return internalInputKinds;
    }


    internal static ModerationApplicableInputKinds FromInternalApplicableInputKinds(IEnumerable<string> internalInputKinds)
    {
        ModerationApplicableInputKinds result = 0;
        foreach (string internalInputKind in internalInputKinds ?? [])
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "text"))
            {
                result |= ModerationApplicableInputKinds.Text;
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "image"))
            {
                result |= ModerationApplicableInputKinds.Image;
            }
            // else if (StringComparer.OrdinalIgnoreCase.Equals(internalInputKind, "audio"))
            // {
            //     result |= ModerationInputKinds.Audio;
            // }
            else
            {
                result |= ModerationApplicableInputKinds.Other;
            }
        }
        return result;
    }

    internal static string ToSerialString(this ModerationApplicableInputKinds value)
        => throw new NotImplementedException();

    internal static ModerationApplicableInputKinds ToModerationApplicableInputKinds(this string value)
        => throw new NotImplementedException();
}
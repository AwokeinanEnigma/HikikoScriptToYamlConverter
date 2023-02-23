using System.Diagnostics;

namespace HikikoScriptToYamlConverter;

public enum TextModifierType
{
    WavyGrowing,
    Growing,
    SmallerFont,
    Jumping,
    Shaking,
    WackyWeirdAndStraightUpStrangeText,
}
public class TextModifierReader
{


    public static Dictionary<TextModifierType, string> ModifierToText = new Dictionary<TextModifierType, string>()
    {
        { TextModifierType.WavyGrowing, "[~]" },
        { TextModifierType.Growing, "[+]" },
        { TextModifierType.SmallerFont, "[-]" },
        { TextModifierType.Jumping, "[^]" },
        { TextModifierType.Shaking, "[#]" },
        // Note to self, I might need to do something differently here because this doesn't apply to a whole text string, and it can be in segments
        { TextModifierType.WackyWeirdAndStraightUpStrangeText, "*" },
    };



    public struct ModifierData
    {
        public TextModifierType type;
        public int startIndex;
        
        // for debug purposes
        public override string ToString()
        {
            return $"Type: {type}, Start Index: {startIndex}";
        }
    }

    /// <summary>
    /// This finds all text modifiers. DOESN'T APPLY THEM!
    /// </summary>
    /// <param name="text">Man.</param>
    /// <returns>A list containing all text modifiers in the string.</returns>
    private List<ModifierData> CreateModifierData(string text)
    {
        List<ModifierData> types = new List<ModifierData>();
        foreach (var modifier in ModifierToText)
        {
            if (text.Contains(modifier.Value))
            {
                types.Add(new ModifierData()
                {
                    type = modifier.Key,
                    startIndex = text.IndexOf(modifier.Value),
                });
            }
        }

        if (types.Count < 0)
        {
            DBG.Log("No text modifiers for the current string!");
        }
        return types;
    }

    public static string RemoveModifiers(string text)
    {
        string newText = text;
        foreach (var modifier in ModifierToText)
        {
            newText = newText.Replace(modifier.Value, string.Empty);
        }
        // do ya damn thing
        return newText;
    }

    /// <summary>
    /// This reads a string, finds any text modifiers in it, and applies it to the text.
    /// </summary>
    public List<ModifierData> FindModifiers(string text)
    {
        List<ModifierData> modifiers = CreateModifierData(text);
        return modifiers;
    }
}
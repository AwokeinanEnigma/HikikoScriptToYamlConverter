using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HikikoScriptToYamlConverter;

// This class is built on A LOT of assumptions!
public class MessageProcessor
{
    // the writers don't put the actual name of the faceset, instead they put some shortened version of it
    // this converts the shortened version to the actual name
    public static Dictionary<string, string> WritersNamesToFileNames = new Dictionary<string, string>()
    {
        { "SU", "SU_DreamSunny" },
        { "HI", "MainCharacter_Hikiko" },
        { "MCDW", "MainCharacters_DreamWorld" },
        { "BA", "BA_DreamBasil" },
        
    };
    
    public struct MessageData
    {
        public string text;
        public string name;
        public string faceSet;
        public int faceIndex;
    }
    
    public MessageData Read(string text)
    {
        // colon is the separator between the name and the actual dialogue

        int seperator = text.IndexOf(":");
        
        if (seperator > 0)
        {
            // there's two scenarios here:
            // 1. there's a portrait
            // 2. there's no portrait
            string name = text.Substring(0, seperator);
            
            int potentialPortraitStart = name.IndexOf("[");
            int potentialPortraitEnd = name.IndexOf("]");

            #region  Portrait

            if (potentialPortraitStart > 0 && potentialPortraitEnd > 0)
            {
                // there's a portrait
                // we use the magical +1 to include the closing bracket
                string portrait = name.Substring(potentialPortraitStart,
                    potentialPortraitEnd - potentialPortraitStart + 1);
                // this is basically something like "[SU 1]"

                string fileName = null;
                foreach (string writerName in WritersNamesToFileNames.Keys)
                {
                    if (portrait.Contains(writerName))
                    {
                        // we found the writer's name in the portrait
                        // now we need to replace the writer's name with the actual file name
                        fileName = WritersNamesToFileNames[writerName];
                        break;
                    }
                }

                if (fileName != null)
                {

                    string faceIndex = Regex.Match(portrait, @"\d+").Value;
                    int faceIndexInt = int.Parse(faceIndex);
                    
                    //DBG.Log($"File name is {fileName} and index is {faceIndex}");
                    MessageData data = new MessageData()
                    {
                        faceIndex = faceIndexInt,
                        faceSet = fileName,
                        // a magic +2 to remove the colon and the space after it!
                        text = text.Substring(seperator + 2),
                        name = name.Substring(0, potentialPortraitStart)
                    };
                    return data;
                }
                else
                {
                    DBG.LogError($"Writer used an unknown faceset. Text: {portrait}", new Exception("Unknown faceset."));
                }
            }
            #endregion
            #region  No portrait
            else
            {
                MessageData data = new MessageData()
                {
                    faceIndex = 0,
                    faceSet = null,
                    // a magic +2 to remove the colon and the space after it!
                    text = text.Substring(seperator + 2),
                    name = name.Substring(0, seperator)
                };
                return data;
            }
            #endregion
        }
        else
        {
            DBG.LogError($"Either the text starts with a colon, or there's no colon separating dialogue! Either case is not allowed! Line: {text}", new Exception("Colon misuse."));
        }
        
        return default;
    }
}
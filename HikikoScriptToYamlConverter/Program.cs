using System;
using System.Diagnostics;

namespace HikikoScriptToYamlConverter
{
    // the new template is shit! fuck .net 6.0!
    internal class Program
    {
        public static YAML yaml;
        public static MessageProcessor Processor;
        public static TextModifierReader reader;
     
        static void Main(string[] args)
        {
            YAML.Initialize();
            
            // create shit
            reader = new TextModifierReader();
            Processor = new MessageProcessor();
            yaml = new YAML();

            DBG.Initialize();
            
            try
            {
                if (args.Length > 0)
                {
                    DBG.LogWarning($"He cums radiation. {args[0]}");
                    string[] text =
                        File.ReadAllLines(args[0]);
                    foreach (string line in text)
                    {
                        MessageProcessor.MessageData data = Processor.Read(line);
                        List<TextModifierReader.ModifierData> modifiedText = reader.FindModifiers(data.text);
                        yaml.ApplyAndWrite(data, modifiedText);
                    }
                    yaml.Finialize();
                    
                }
                else
                {
                    DBG.LogWarning("You must drag and drop a text file onto the .exe to use this!");
                }
            }
            catch (Exception ex)
            {
                DBG.DumpLogs();
                DBG.LogError("AN ERROR HAS OCCURRED!", null);
                DBG.LogError("PLEASE SEND THE FOLLOWING LINES OF TEXT TO ENIGMA", null);
                DBG.LogError("===EXCEPTION===", ex);
            }

            // this stops the programming from automatically exiting
            Console.ReadLine();
        }
    }
}
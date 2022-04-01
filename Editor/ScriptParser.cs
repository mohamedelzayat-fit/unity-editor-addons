using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FITEditorAddons.Editor
{
    public class ScriptParser
    {
        public static void InsertNamespace(string FilePath, string GenNamespace)
        {
            List<string> ScriptLines = new List<string>();
            
            if (!CheckNamespace(ref ScriptLines, FilePath))
                return;
            
            //  Incase the script file is not empty we need to find the correct insertion spot
            //  If it is empty, then we can just add the namespace declaration to the list
            if (ScriptLines.Count != 0)
            {
                int InsertIndex = ScriptLines.FindIndex(str => str.Contains("class") && !str.Contains("//"));
                ScriptLines.Insert(InsertIndex, $"namespace {GenNamespace}");
                ScriptLines.Insert(InsertIndex + 1, "{");
    
                // InsertIndex + 2 in order to not indent namespace and its opening bracket
                for (int i = InsertIndex + 2; i < ScriptLines.Count; i++)
                    ScriptLines[i] = "    " + ScriptLines[i];
            
                ScriptLines.Add("}");
            }
            else
            {
                ScriptLines.Add($"namespace {GenNamespace}");
                ScriptLines.Add("{");
                ScriptLines.Add("");
                ScriptLines.Add("}");
            }
            

            WriteToScript(ScriptLines.ToArray(), FilePath);
            
            Debug.Log($"Edited in a namespace for script: {FilePath}");
        }
    
        private static void WriteToScript(string[] ScriptLines, string FilePath)
        {
            using (var sw = new StreamWriter(FilePath, false))
            {
                foreach (var Line in ScriptLines)
                {
                    sw.WriteLine(Line);
                }
            }
        }
    
        private static List<string> ReadFromScript(string FilePath)
        {
            List<string> Lines = new List<string>();
            
            using (var sr = new StreamReader(FilePath))
            {
                var Line = "";
                
                while ((Line = sr.ReadLine()) != null)
                {
                    if (Line.StartsWith("namespace") )
                        return null;

                    Lines.Add(Line);
                }
            }

            return Lines;
        }

        private static bool CheckNamespace(ref List<string> ScriptLines, string FilePath)
        {
            ScriptLines = ReadFromScript(FilePath);

            if (ScriptLines == null)
            {
                Debug.Log($"Script already contains namespace: {FilePath}");
                return false;
            }

            return true;
        }
    }
}


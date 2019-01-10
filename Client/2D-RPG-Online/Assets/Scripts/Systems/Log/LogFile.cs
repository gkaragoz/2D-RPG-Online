using UnityEngine;
using System.IO;

public class LogFile {

    private const string FILE_NAME = "log.txt";

    public static void WriteString(string logString) {
        string path = "Assets/Resources/" + FILE_NAME;

        if (!File.Exists(path)) {
            Debug.Log("[LogFile] WRITE EVENT>> " + FILE_NAME + " file is creating on path: " + path);
        }

        FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

        StreamWriter writer = new StreamWriter(file);
        writer.WriteLine(logString);
        writer.Close();
    }

    /*DebugLOG notes.
        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = (TextAsset)Resources.Load(FILE_NAME.Split('.')[0]);

        //Print the text from the file
        //Debug.Log(asset.text);
    */
}
 
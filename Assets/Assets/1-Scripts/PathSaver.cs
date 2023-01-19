using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class PathSaver : MonoBehaviour
{
    string path = "Path/To/Save/Location";

    //public static bool SavePaths(GameObject obj)
    //{
    //    string data;
    //    //Translate GameObject into path
    //    string assetPath = AssetDatabase.GetAssetPath((GameObject)data);
    //    if (assetPath == null) return false;
    //    else data = assetPath;

    //    //Encrypt Save Data (not nessecary, but nice to have)
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    formatter.Serialize(stream, data);

    //    stream.Close();

    //    return true;
    //}


    //public static object Load()
    //{
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);


    //        data = formatter.Deserialize(stream) as string
    //             SavedGameObject = (GameObject)AssetDatabase.LoadAssetAtPath((string)data[key].Key, typeof(GameObject));

    //        stream.Close();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Save file not found.");
    //        return null;
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;
using System;

public class DialogLibrary
{
    public class MissingDialogException : Exception { }

    public const string VERSION = "1";
    public const string MARKER_TAG = "zola";
    public const string FOLDER_NAME = "Dialogues";

    string locale = "en-US"; // Can be changed for LOC

    Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

    public void Rebuild()
    {
        dialogs.Clear();

        var path = Path.Combine(Application.streamingAssetsPath, FOLDER_NAME, locale);
        foreach (var element in Directory.GetFiles(path))
        {
            if (!element.ToLower().EndsWith(".xml")) continue;

            Debug.Log("Loading file " + element);

            var doc = new XmlDocument();
            doc.Load(element);
            var root = doc.SelectSingleNode("zola");

            foreach (var node in root.ChildNodes.Cast<XmlNode>())
            {
                if (node.Name != "dialog") continue;
                dialogs[node.Attributes["id"].Value] = new Dialog(node);
            }
        }
    }

    public Dialog Get(string id)
    {
        if (!dialogs.ContainsKey(id))
        {
            throw new MissingDialogException();
        }

        return dialogs[id];
    }

    public Dialog this[string id] {
        get {
            return Get(id);
        }
    }
}

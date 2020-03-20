using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml;
using System;

public class Dialog {
    public bool isAutomatic = false;
    public List<IElement> elements = new List<IElement>();
    public float intervalMultiplier = 1f;

    static readonly List<Tuple<string, string>> interpolations = new List<Tuple<string, string>>() {
        new Tuple<string, string>("<big>", "<size=150%>"),
        new Tuple<string, string>("<huge>", "<size=220%>"),
        new Tuple<string, string>("<small>", "<size=70%>"),
        new Tuple<string, string>("<tiny>", "<size=30%>"),

        new Tuple<string, string>("</big>", "</size>"),
        new Tuple<string, string>("</huge>", "</size>"),
        new Tuple<string, string>("</small>", "</size>"),
        new Tuple<string, string>("</tiny>", "</size>"),

        new Tuple<string, string>("<color value=", "<color="),
        new Tuple<string, string>("<nonbreakable>", "<nobr>"),	
        new Tuple<string, string>("<br />", Environment.NewLine),
        new Tuple<string, string>("<break />", Environment.NewLine),
        new Tuple<string, string>("	", "")
    };

    public interface IElement
    {

    }

    public class Line : IElement
    {
        public readonly string rawXML = "";
        public readonly string convertedXML = "";
        public readonly string originalPureText = "";

        public bool containsRumble = false;
        public bool containsWave = false;
        public bool containsFlash = false;

        public Line(XmlNode cnt)
        {
            rawXML = cnt.InnerXml;
            originalPureText = cnt.InnerText;

            // Tag interpolation
            convertedXML = rawXML.Replace(Environment.NewLine, "");
            foreach (var interpolation in interpolations)
            {
                convertedXML = convertedXML.Replace(interpolation.Item1, interpolation.Item2);
            }
            Debug.Log("Created new line with cxml " + convertedXML);
        }
    }

    public class Pause : IElement
    {
        public readonly float miliseconds = 0f;
        public Pause(float miliseconds)
        {
            this.miliseconds = miliseconds;
        }
    }

    public Dialog(XmlNode contents)
    {
        this.isAutomatic = contents.Attributes["automatic"] != null ? contents.Attributes["automatic"].Value == "true" : false;
        this.intervalMultiplier = contents.Attributes["speed"] != null ? GetMultiplier(contents.Attributes["speed"].Value) : 1f;

        foreach (var dialogElement in contents.ChildNodes)
        {
            var dialNode = (XmlNode)dialogElement;
            switch (dialNode.Name)
            {
                default:
                    throw new Exception("Unknown dialog node: " + dialNode.Name);

                case "pause":
                    elements.Add(new Pause(XmlConvert.ToSingle(dialNode.Attributes["miliseconds"].Value)));
                    break;

                case "line":
                    var line = new Line(dialNode);
                    var tags = dialNode.ChildNodes.Cast<XmlNode>().Select(o => { return o.Name; });
                    line.containsRumble = tags.Contains("rumble");
                    line.containsWave = tags.Contains("wave");
                    line.containsFlash = tags.Contains("flash");
                    elements.Add(line);
                    break;
            }
        }
    }

    public static float GetMultiplier(string str)
    {
        switch (str)
        {
            case "slow":
                return 0.3f;
            case "fast":
                return 3f;
            case "ultrafast":
                return 5f;
            case "ultraslow":
                return 0.1f;
        }
        throw new Exception("Unknown keyword: " + str);
    }
}

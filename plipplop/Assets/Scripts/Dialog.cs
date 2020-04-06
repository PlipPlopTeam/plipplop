
using UnityEngine;

using System.IO;
using System.Linq;
using System.Xml;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Dialog {
    public bool isAutomatic = false;
    public List<IElement> elements = new List<IElement>();
    public float intervalMultiplier = 1f;
    public readonly string talker = "???";

    static readonly List<string> TMProSupportedTags = new List<string>()
    {
        "size", "color", "nobr", "font", "mark"
    };

    static readonly List<string> flowControlTags = new List<string>()
    {
        "pause", "whole", "instantaneous", "slow", "fast", "shock"
    };

    public static readonly List<string> vertexFXTags = new List<string> { 
        "rumble", "wave" 
    };

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
        new Tuple<string, string>("<highlight color=", "<mark="),
        new Tuple<string, string>("</highlight>", "</mark>"),
        new Tuple<string, string>("<nonbreakable>", "<nobr>"),	
        new Tuple<string, string>("<br />", Environment.NewLine),
        new Tuple<string, string>("<break />", Environment.NewLine)
    };

    static readonly List<Tuple<string, string>> spaceInterpolations = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("\t", ""),
        new Tuple<string, string>(Encoding.ASCII.GetString(new byte[]{ 9 }), "")
    };

    public struct Event
    {
        public string name;
        public int startChar;
        public int endChar;
    }

    public interface IElement {}

    public class Line : IElement
    {
        public readonly string tmpReadyXml;
        public readonly string pureText;
        public readonly int length = 0;
        public readonly string talker = string.Empty;

        public List<Event> events = new List<Event>();

        public Line(XmlNode cnt)
        {
            this.pureText = cnt.InnerText;
            var xml = cnt.InnerXml;
            if (cnt.Attributes["talker"] != null) this.talker = cnt.Attributes["talker"].Value;

            // Tag interpolation
            tmpReadyXml = xml.Replace(Environment.NewLine, "");
            foreach (var interpolation in interpolations)
            {
                tmpReadyXml = tmpReadyXml.Replace(interpolation.Item1, interpolation.Item2);
            }
            foreach (var interpolation in spaceInterpolations)
            {
                tmpReadyXml = tmpReadyXml.Replace(interpolation.Item1, interpolation.Item2);
                pureText = pureText.Replace(interpolation.Item1, interpolation.Item2);
            }

           // Color interpolation
           var match = Regex.Match(tmpReadyXml, "(\")(?:#[AF0-9]{6})(\")");
            while (match != null && match.Success && match.Length > 0)
            {
                if (match.Value.Length == 9)
                {
                    tmpReadyXml = tmpReadyXml.Replace(match.Value, match.Value.Substring(1, 7));
                }
                match = match.NextMatch();
            }

            var breaks = Regex.Matches(tmpReadyXml, Environment.NewLine).Count;

            length = pureText.Length + breaks +1;

            // Event detection
            int charIndex = 0;
            bool inTag = false;
            StringBuilder buffer = new StringBuilder();
            StringBuilder totalText = new StringBuilder();

            while (charIndex < tmpReadyXml.Length)
            {
                // Detect XML tag start
                if (tmpReadyXml[charIndex] == '<')
                {
                    inTag = true;
                    charIndex++;
                    continue;
                }
                if (tmpReadyXml[charIndex] == '>')
                {
                    inTag = false;
                    var tag = buffer.ToString();
                    var rawTag = tag;
                    buffer.Clear();

                    bool isClosingTag = false;
                    bool isSelfClosing = false;

                    if (tag[0] == '/')
                    {
                        isClosingTag = true;
                    }
                    else if (tag.Contains('/'))
                    {
                        isSelfClosing = true;
                    }

                    tag = tag.Replace("/", "").Replace(" ", "").Split('=')[0]; // Changes <color value="1  into  color

                    if (TMProSupportedTags.Contains(tag))
                    {
                        totalText.Append("<" + rawTag);
                    }
                    else if (flowControlTags.Contains(tag) || vertexFXTags.Contains(tag))
                    {
                        if (isClosingTag)
                        {
                            charIndex++;
                            continue;
                        }

                        var evnt = new Event() { name = tag, startChar = totalText.Length, endChar = -1 };
                        if (!isSelfClosing)
                        {
                            int cursor = charIndex;
                            int pureCursor = totalText.Length;
                            bool foundExit = false;
                            while (!foundExit)
                            {
                                cursor++;
                                pureCursor++;
                                if (tmpReadyXml[cursor] == '<')
                                {
                                    var newCursor = cursor+1;
                                    if (tmpReadyXml[newCursor] == '/')
                                    {
                                        newCursor++;
                                        var bld = new StringBuilder();
                                        while(tmpReadyXml[newCursor] != '>')
                                        {
                                            bld.Append(tmpReadyXml[newCursor]);
                                            newCursor++;
                                        }
                                        if (bld.ToString() == evnt.name)
                                        {
                                            evnt.endChar = pureCursor-2; // small offset, necessary for visual coherence
                                            foundExit = true;
                                        }
                                    }
                                }
                            }
                        }

                        events.Add(evnt);

                        charIndex++;
                        continue;
                    }
                }

                if (inTag)
                {
                    buffer.Append(tmpReadyXml[charIndex]);
                }
                else
                {
                    totalText.Append(tmpReadyXml[charIndex]);
                }

                charIndex++;
            }

            tmpReadyXml = totalText.ToString();
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
        this.talker = contents.Attributes["talker"] != null ? contents.Attributes["talker"].Value : "???";

        foreach (var dialogElement in contents.ChildNodes)
        {
            var dialNode = (XmlNode)dialogElement;
            switch (dialNode.Name)
            {
                default:
                    throw new Exception("Unknown dialog node: " + dialNode.Name+"");

                case "pause":
                    elements.Add(new Pause(XmlConvert.ToSingle(dialNode.Attributes["miliseconds"].Value)));
                    break;

                case "line":
                    var line = new Line(dialNode);
                    var tags = dialNode.ChildNodes.Cast<XmlNode>().Select(o => { return o.Name; });
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

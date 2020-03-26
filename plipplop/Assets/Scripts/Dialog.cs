
using UnityEngine;

using System.IO;
using System.Linq;
using System.Xml;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Dialog {
    public bool isAutomatic = false;
    public List<IElement> elements = new List<IElement>();
    public float intervalMultiplier = 1f;

    static readonly List<string> TMProSupportedTags = new List<string>()
    {
        "size", "color", "nobr", "font"
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
        new Tuple<string, string>("<highlight value=", "<mark="),
        new Tuple<string, string>("<nonbreakable>", "<nobr>"),	
        new Tuple<string, string>("<br />", Environment.NewLine),
        new Tuple<string, string>("<break />", Environment.NewLine),
        new Tuple<string, string>("	", "")
    };

    public struct Event
    {
        public string name;
        public int startChar;
        public int endChar;
    }

    public interface IElement
    {

    }

    public class Line : IElement
    {
        public readonly string tmpReadyXml;
        public readonly string pureText;

        public List<Event> events = new List<Event>();

        public Line(XmlNode cnt)
        {
            this.pureText = cnt.InnerText;
            var xml = cnt.InnerXml;

            // Tag interpolation
            tmpReadyXml = xml.Replace(Environment.NewLine, "");
            foreach (var interpolation in interpolations)
            {
                tmpReadyXml = tmpReadyXml.Replace(interpolation.Item1, interpolation.Item2);
            }

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
                        totalText.Append("<" + (isClosingTag ? "/" : "") + rawTag + ">");
                    }
                    else if (flowControlTags.Contains(tag) || vertexFXTags.Contains(tag))
                    {
                        if (isClosingTag)
                        {
                            charIndex++;
                            continue;
                        }

                        var evnt = new Event() { name = tag, startChar = totalText.Length-1, endChar = -1 };
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
                                            evnt.endChar = pureCursor-3; // small offset, necessary for visual coherence
                                            Debug.Log("Event " + tag + " starts at " + evnt.startChar + " and ends at " + evnt.endChar);
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

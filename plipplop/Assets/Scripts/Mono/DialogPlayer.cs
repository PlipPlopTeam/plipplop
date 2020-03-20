using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

public class DialogPlayer : MonoBehaviour
{
    public float baseInterval = 0.1f;
    public float pauseInterval = 0.4f;
    public float fastSpeedMultiplier = 2f;
    public float slowSpeedMultiplier = 0.2f;

    public Dictionary<string, Tuple<int, int>> vertexFXs = new Dictionary<string, Tuple<int, int>>();

    bool isPlaying = false;
    bool isWaitingDelay = false;
    bool isWaitingForInput = false;
    int currentLineIndex = 0;
    int rawCharIndex = 0;
    Dialog currentDialogue = null;
    TextMeshProUGUI textMesh;
    Coroutine teletypeRoutine;

    readonly List<string> vertexFX = new List<string> { "rumble", "wave" };
    readonly List<string> TMProSupportedTags = new List<string>()
    {
        "size", "color", "nobr", "font", 
        "/size", "/color", "/nobr", "/font"
    };

    int slowingDown = 0;
    int goingFaster = 0;
    DialogLibrary library;

    bool isSlowingDown { get { return slowingDown > 0; } }
    bool isGoingFaster { get { return goingFaster > 0; } }
    Dialog.IElement currentElement { get { return currentDialogue.elements[currentLineIndex]; } }
    Dialog.Line currentLine { get { return (Dialog.Line)currentElement; } }
    float fastInterval { get { return baseInterval / fastSpeedMultiplier; } }
    float slowInterval { get { return baseInterval / slowSpeedMultiplier; } }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (Game.i)
        {
            library = Game.i.library.dialogs;
        }
        else
        {
            library = new DialogLibrary();
            library.Rebuild();
        }
        PlaySampleDialogue();
    }

    public void LoadDialogue(string id)
    {
        currentLineIndex = 0;
        rawCharIndex = 0;
        currentDialogue = library[id];
        textMesh.text = "";
    }

    public void Play()
    {
        isPlaying = true;
        currentLineIndex = -1;
        Next();
    }

    public void Next()
    {
        Debug.Log("NEXT!");
        if (teletypeRoutine != null) StopCoroutine(teletypeRoutine);
        vertexFXs.Clear();
        rawCharIndex = 0;
        currentLineIndex++;
        if (currentLineIndex >= currentDialogue.elements.Count)
        {
            isPlaying = false;
            return;
        }
        if (currentElement is Dialog.Pause)
        {
            Debug.Log("wait!");
            WaitFor(((Dialog.Pause)currentElement).miliseconds);
        }
        else if (currentElement is Dialog.Line)
        {
            Debug.Log("teletyping...");
            textMesh.text = "";
            teletypeRoutine = StartCoroutine(Teletype());
        }
    }

    public void PlaySampleDialogue()
    {
        LoadDialogue("test");
        Play();
    }

    private void Update()
    {
        if (isWaitingDelay) return;

        if (Input.GetKeyDown(KeyCode.Space) || (Game.i && Game.i.player.mapping.IsPressed(EAction.ACTION)))
        {
            if (isWaitingForInput || currentElement is Dialog.Pause)
            {
                isWaitingForInput = false;
                Next();
            }
            else
            {
                rawCharIndex = currentLine.convertedXML.Length;
                isWaitingForInput = true;
            }
        }

        UpdateVisibleLetters();
    }

    IEnumerator Teletype()
    {
        var line = currentLine;
        Debug.Log("Starting teletype");

        // Content control
        // TODO: Random, Autodialog, Dialog speeds, OUTLINE

        while (rawCharIndex < line.convertedXML.Length)
        {
            // Detect XML tag start
            if (line.convertedXML[rawCharIndex] == '<')
            {
                rawCharIndex++;
                StringBuilder buffer = new StringBuilder();
                while (line.convertedXML[rawCharIndex] != '>')
                {
                    buffer.Append(line.convertedXML[rawCharIndex]);
                    rawCharIndex++;
                }

                var tag = buffer.ToString();

                // Detect vertex FX tags
                if (vertexFX.Contains(tag))
                {
                    int start = rawCharIndex;
                    int rawIndex = rawCharIndex;
                    StringBuilder effectBuffer = new StringBuilder();
                    bool isInTag = false;
                    while (!effectBuffer.ToString().EndsWith("</" + tag + ">"))
                    {
                        rawIndex++;

                        if (line.convertedXML[rawIndex] == '<')
                            isInTag = true;

                        if (!isInTag)
                        {
                            rawIndex++;
                        }

                        if (line.convertedXML[rawIndex] == '>')
                            isInTag = false;

                        if (rawIndex >= line.convertedXML.Length)
                        {
                            throw new Exception("Tag " + tag + " seems to never end. Line " + currentLineIndex + ".");
                        }
                        effectBuffer.Append(line.convertedXML[rawIndex]);
                    }
                    int end = rawIndex;

                    vertexFXs.Add(tag, new Tuple<int, int>(start, end));
                }

                // Flow control
                else if (new List<string>() { "pause /", "whole", "instantaneous", "slow", "fast", "shock /" }.Contains(tag))
                {
                    switch (tag)
                    {
                        case "pause /":
                            yield return new WaitForSeconds(pauseInterval);
                            break;

                        case "shock /":
                            // <Insert shock FX>
                            break;

                        case "slow":
                            slowingDown++;
                            break;

                        case "fast":
                            goingFaster++;
                            break;

                        case "whole":
                        case "instantaneous":
                            // Skipping until I meet an ending
                            StringBuilder charBuffer = new StringBuilder();
                            bool isInTag = false;
                            while (!charBuffer.ToString().EndsWith("</" + tag + ">"))
                            {
                                rawCharIndex++;

                                if (line.convertedXML[rawCharIndex] == '<')
                                    isInTag = true;

                                if (!isInTag)
                                {
                                    rawCharIndex++;
                                }

                                if (line.convertedXML[rawCharIndex] == '>')
                                    isInTag = false;

                                if (rawCharIndex >= line.convertedXML.Length)
                                {
                                    throw new Exception("Tag " + tag + " seems to never end. Line " + currentLineIndex + ".");
                                }

                                charBuffer.Append(line.convertedXML[rawCharIndex]);
                            }
                            break;
                    }
                }
                else if (tag == "/slow")
                {
                    slowingDown--;
                }
                else if (tag == "/fast")
                {
                    goingFaster--;
                }
            }
            yield return new WaitForSeconds(currentDialogue.intervalMultiplier * (isGoingFaster ? fastInterval : (isSlowingDown ? slowInterval : baseInterval)));

            rawCharIndex++;
        }
        Debug.Log("Ending teletype because " + rawCharIndex + " > " + line.convertedXML.Length);
        isWaitingForInput = true;
    }

    IEnumerator WaitFor(float miliseconds)
    {
        isWaitingDelay = true;
        yield return new WaitForSeconds(miliseconds);
        isWaitingDelay = false;
    }

    void UpdateVisibleLetters()
    {
        if (currentElement is Dialog.Pause)
        {
            return;
        }

        StringBuilder builder = new StringBuilder();
        var line = currentLine;

        bool isInTag = false;
        var currentTag = "";
        int i = 0;
        while (i < rawCharIndex || isInTag) { 
            if (line.convertedXML[i] == '<')
            {
                isInTag = true;
                i++;
                continue;
            }
            if (line.convertedXML[i] == '>')
            {
                isInTag = false;
                var tagHead = currentTag.Split('=')[0];

                if (TMProSupportedTags.Contains(tagHead))
                {
                    builder.Append("<" + currentTag + ">");
                }

                currentTag = "";
                i++;
                continue;
            }

            if (isInTag)
            {
                currentTag += line.convertedXML[i];
            }
            else
            {
                builder.Append(line.convertedXML[i]);
            }

            i++;
        }


        textMesh.text = builder.ToString();
    }
}

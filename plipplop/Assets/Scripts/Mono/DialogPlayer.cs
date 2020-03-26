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
    public float scaleSpeed = 2f;

    public Dictionary<string, List<Tuple<int, int>>> vertexFXs = new Dictionary<string, List<Tuple<int, int>>>();
    public RectTransform parent;

    bool isPlaying = false;
    bool isWaitingDelay = false;
    bool isWaitingForInput = false;
    int currentLineIndex = 0;
    int currentCharIndex = 0;
    Dialog currentDialogue = null;
    TextMeshProUGUI textMesh;
    Coroutine teletypeRoutine;

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
        LoadDialogue(library[id]);
    }

    public void LoadDialogue(Dialog dialog)
    {
        currentLineIndex = 0;
        currentCharIndex = 0;
        textMesh.text = "";
        currentDialogue = dialog;
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
        currentCharIndex = 0;
        textMesh.maxVisibleCharacters = 0;
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
        if (!isPlaying)
        {
            parent.localScale = Vector3.Slerp(parent.localScale, Vector3.zero, scaleSpeed * Time.deltaTime);
            if (Game.i && Game.i.dialogToBeGrabbed != null) {
                LoadDialogue(Game.i.dialogToBeGrabbed);
                Game.i.dialogToBeGrabbed = null;
                Play();
            }
            return;
        }
        parent.localScale = Vector3.Slerp(parent.localScale, Vector3.one, scaleSpeed * Time.deltaTime);

        if (isWaitingDelay) return;

        if (Input.GetKeyDown(KeyCode.Space) || (Game.i && Game.i.player.mapping.IsPressed(EAction.TOGGLE_LEGS)))
        {
            if (isWaitingForInput || currentElement is Dialog.Pause)
            {
                isWaitingForInput = false;
                Next();
            }
            else
            {
                currentCharIndex = currentLine.pureText.Length;
                isWaitingForInput = true;
            }
        }

        UpdateVisibleLetters();
    }

    IEnumerator Teletype()
    {
        var line = currentLine;

        // Content control
        // TODO: Random, Autodialog, Dialog speeds, OUTLINE

        // Let's first look for events of type vertexFX
        vertexFXs.Clear();
        foreach (var evnt in line.events.FindAll(o => Dialog.vertexFXTags.Contains(o.name)))
        {
            if (!vertexFXs.ContainsKey(evnt.name)) {
                vertexFXs.Add(evnt.name, new List<Tuple<int, int>>());
            }

            vertexFXs[evnt.name].Add(new Tuple<int, int>(evnt.startChar, evnt.endChar));
        }

        while (currentCharIndex < line.pureText.Length)
        {
            // Detect XML tag start

            var startingEvents = line.events.FindAll(o => o.startChar == currentCharIndex);
            foreach (var startingEvent in startingEvents)
            {
                switch (startingEvent.name)
                {
                    case "pause":
                        yield return new WaitForSeconds(pauseInterval);
                        break;

                    case "shock":
                        yield return new WaitForSeconds(pauseInterval);
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
                        while (true)
                        {
                            currentCharIndex++;
                            var ends = line.events.FindAll(o => o.name == startingEvent.name && o.endChar == currentCharIndex);
                            if (ends.Count > 0)
                            {
                                break;
                            }

                            if (currentCharIndex >= line.pureText.Length)
                            {
                                throw new Exception("Tag " + startingEvent.name + " seems to never end. Line " + currentLineIndex + ".");
                            }
                        }
                        break;
                }
            }

            var endingEvents = line.events.FindAll(o => o.endChar == currentCharIndex);
            foreach (var end in endingEvents)
            {
                switch (end.name)
                {
                    case "slow":
                        slowingDown--;
                        break;

                    case "fast":
                        goingFaster--;
                        break;
                }
            }

            currentCharIndex++;
            yield return new WaitForSeconds(currentDialogue.intervalMultiplier * (isGoingFaster ? fastInterval : (isSlowingDown ? slowInterval : baseInterval)));
        }

        Debug.Log("Ending teletype because " + currentCharIndex + " > " + line.pureText.Length);
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

        textMesh.text = currentLine.tmpReadyXml;
        textMesh.maxVisibleCharacters = currentCharIndex;
    }
}

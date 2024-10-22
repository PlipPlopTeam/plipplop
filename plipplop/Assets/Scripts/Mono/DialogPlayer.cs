﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class DialogPlayer : MonoBehaviour
{
    public float baseInterval = 0.1f;
    public float pauseInterval = 0.4f;
    public float periodInterval = 0.5f;
    public float fastSpeedMultiplier = 2f;
    public float slowSpeedMultiplier = 0.2f;
    public float scaleSpeed = 2f;
    public string pausingChars = ",.:";
    public bool playTestDialogue = false;

    public Dictionary<string, List<Tuple<int, int>>> vertexFXs = new Dictionary<string, List<Tuple<int, int>>>();
    public Image prompt;
    public TextMeshProUGUI nameTag;
    public RectTransform parent;
    public Action triggerVFX;
    public int lineIndex { get { return currentLineIndex; } }
    public int charIndex { get { return currentCharIndex; } }

    public bool isPlaying { private set; get; } = false;
    bool isWaitingDelay = false;
    bool isWaitingForInput = false;
    int currentLineIndex = 0;
    int currentCharIndex = 0;
    Dialog currentDialogue = null;
    Coroutine teletypeRoutine;
    public TextMeshProUGUI textMesh { private set; get; }

    int slowingDown = 0;
    int goingFaster = 0;
    Action callback = null;
    DialogLibrary library;
    DialogEffect dfx;

    bool isSlowingDown { get { return slowingDown > 0; } }
    bool isGoingFaster { get { return goingFaster > 0; } }
    Dialog.IElement currentElement { get { return currentDialogue.elements[currentLineIndex]; } }
    Dialog.Line currentLine { get { return (Dialog.Line)currentElement; } }
    float fastInterval { get { return baseInterval / fastSpeedMultiplier; } }
    float slowInterval { get { return baseInterval / slowSpeedMultiplier; } }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        dfx = GetComponent<DialogEffect>();
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
        if (playTestDialogue)
        {
            PlaySampleDialogue();
        }
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
        nameTag.text = dialog.talker;
        currentDialogue = dialog;
    }

    public void Play(System.Action callback=null)
    {
        if (Game.i) Game.i.player.Paralyze();
        this.callback = callback;
        isPlaying = true;
        currentLineIndex = -1;
        Next();
    }

    public void Next()
    {
        if (teletypeRoutine != null) StopCoroutine(teletypeRoutine);
        vertexFXs.Clear();
        currentCharIndex = 0;
        textMesh.maxVisibleCharacters = 0;
        slowingDown = 0;
        goingFaster = 0;
        currentLineIndex++;
        if (currentLineIndex >= currentDialogue.elements.Count)
        {
            EndDialogue();
            return;
        }
        if (currentElement is Dialog.Pause)
        {
            WaitFor(((Dialog.Pause)currentElement).miliseconds);
        }
        else if (currentElement is Dialog.Line)
        {
            textMesh.text = "";
            if (!string.IsNullOrEmpty(currentLine.talker)) nameTag.text = currentLine.talker;
            teletypeRoutine = StartCoroutine(Teletype());
        }
    }

    public void EndDialogue()
    {
        isPlaying = false;
        DialogHooks.currentInterlocutor = null;
        if (Game.i) Game.i.player.Deparalyze();
        if (callback != null) callback.Invoke();
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
            if (DialogHooks.dialogToBeGrabbed != null) {

                LoadDialogue(DialogHooks.dialogToBeGrabbed);
                Play(DialogHooks.callback);

                DialogHooks.callback = null;
                DialogHooks.dialogToBeGrabbed = null;
            }
            return;
        }

        nameTag.SetLayoutDirty();

        parent.localScale = Vector3.Slerp(parent.localScale, Vector3.one, scaleSpeed * Time.deltaTime);

        if (isWaitingDelay) return;

        if (isWaitingForInput)
        {
            prompt.color = Color.Lerp(prompt.color, Color.white, Time.deltaTime);
        }
        else
        {
            prompt.color = new Color(1f, 1f, 1f, 0f);
        }

        // Fast skip
        if (!currentDialogue.isAutomatic && (Input.GetKeyDown(KeyCode.Space) || (Game.i && Game.i.player.mapping.IsPressed(EAction.TOGGLE_LEGS))))
        {
            if (isWaitingForInput || currentElement is Dialog.Pause)
            {
                isWaitingForInput = false;
                Next();
            }
            else
            {
                currentCharIndex = currentLine.pureText.Length+2;
                isWaitingForInput = true;
            }
        }

        UpdateVisibleLetters();
    }

    IEnumerator Teletype()
    {
        var line = currentLine;

        // Content control
        // TODO: Random, Autodialog, Dialog speeds

        // Let's first look for events of type vertexFX
        vertexFXs.Clear();
        foreach (var evnt in line.events.FindAll(o => Dialog.vertexFXTags.Contains(o.name)))
        {
            if (!vertexFXs.ContainsKey(evnt.name)) {
                vertexFXs.Add(evnt.name, new List<Tuple<int, int>>());
            }

            vertexFXs[evnt.name].Add(new Tuple<int, int>(evnt.startChar, evnt.endChar));
        }

        while (currentCharIndex <= line.length)
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

                            if (currentCharIndex >= line.length)
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

            if (line.pureText.Length > currentCharIndex+1 && pausingChars.Contains(line.pureText[currentCharIndex].ToString()))
            {
                currentCharIndex++;
                yield return new WaitForSeconds(periodInterval); // Pause on period
                currentCharIndex--;
            }

            if (currentCharIndex < line.pureText.Length) {
                DialogHooks.currentPronouncedLetter = line.pureText[currentCharIndex].ToString();
                SoundPlayer.Play("sfx_rock_voice", .2f,UnityEngine.Random.Range(.8f,1.2f));
            }

            currentCharIndex++;
            yield return new WaitForSeconds(currentDialogue.intervalMultiplier * (isGoingFaster ? fastInterval : (isSlowingDown ? slowInterval : baseInterval)));
        }

        if (currentDialogue.isAutomatic) {
            yield return new WaitForSeconds(Mathf.Min(3f, line.length * 0.33f));
            Next();

        }
        else {
            isWaitingForInput = true;
        }

    }

    IEnumerator WaitFor(float miliseconds)
    {
        isWaitingDelay = true;
        yield return new WaitForSeconds(miliseconds);
        isWaitingDelay = false;
        Next();
    }

    void UpdateVisibleLetters()
    {
        if (currentLineIndex >= currentDialogue.elements.Count) return;
        if (currentElement is Dialog.Pause)
        {
            return;
        }

        textMesh.text = currentLine.tmpReadyXml;
        if (dfx == null)
        {
            textMesh.maxVisibleCharacters = currentCharIndex;
        }
    }
}

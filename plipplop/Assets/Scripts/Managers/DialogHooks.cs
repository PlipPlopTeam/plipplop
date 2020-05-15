using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Messy class for all dialog related purposes
public static class DialogHooks
{
    public static Dialog dialogToBeGrabbed;
    public static string currentPronouncedLetter = string.Empty;
    public static TalkableCharacter currentInterlocutor = null;
    public static System.Action callback = null;

}

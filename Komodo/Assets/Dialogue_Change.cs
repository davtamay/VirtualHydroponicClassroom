using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Dialogue",  menuName = "CustomSO/Dialogue")]
public class Dialogue_Change : ScriptableObject {

    [Header("Main Dialogue Set")]
   // public bool hasTextOptionsPresent;
    public Dialogue_Data[] dialogueDataMessages;
    [Space]
    [Header ("Set Response Event and Settings")]
    [Multiline]
    public string correctMessage;
    public UnityEvent onCompleteDialogue;
    [Multiline]
    public string inCorrectMessage;
    public UnityEvent onIncompleteDialogue;
    
    public bool isComplete;
    public int CorrectOption;

}
public enum Response{CORRECT = 1, WRONG = 0, INDIFERENT = -1 }//new Continue = 0; correct 1; wrong 2
[System.Serializable]
public class Dialogue_Data
{
    public string description;

    [Multiline]
    public string mainText;
    [Space]
    public bool isOptionChoiceAvailable;
    [Multiline]
    public string discussion_text_Option1;
    [Multiline]
    public string discussion_text_Option2;

    [Space]
    [Header("Responce Correct/Wrong PathWays = 0: To continue; 1: Option_1, 2: Option_2 ")]
    public int CorrectOption = 1;
    //0 for wrong //1 for right //2 for third option?

    [Space]
    [Header("If null, no image ")]
    public Sprite imageToDisplay;

    [Space]
    public UnityEvent onSelectOption;
    [Space]

    public bool isFinishDialogue = false;

    public bool isComplete;

    [Header("Branching Responce Pathway")]
    public bool isBranch;
    public Dialogue_Change dialogueChange_option_1;
    public Dialogue_Change dialogueChange_option_2;

}

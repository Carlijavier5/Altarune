using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour {

    public event System.Action OnDialogueEnd;

    private enum State { Idle, Writing, Awaiting }
    private State state;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private RectTransform dialogueBox, npcTitle;
    [SerializeField] private DialogueCharacterResolver characterResolver;
    [SerializeField] private Vector2 fontBounds;
    [SerializeField] private float textGrowthMult, audioInterval,
                                   anchorSpeed;

    private DialogueData currDialogue;
    private string currStr;

    private int currLine, currLetter;

    private bool[] growthSet;
    private float[] sizeSet;

    private float audioCD;

    void Awake() {
        dialogueBox.localScale = new Vector3(dialogueBox.localScale.x, 0, dialogueBox.localScale.z);
        dialogueBox.gameObject.SetActive(false);
        ResetText();
    }

    void Update() {
        if (state == State.Writing
            && audioCD <= 0) PlayDialogueAudio();
        if (Input.anyKeyDown) {
            switch (state) {
                case State.Writing:
                    StopAllCoroutines();
                    textMesh.text = currStr;
                    state = State.Awaiting;
                    break;
                case State.Awaiting:
                    StopAllCoroutines();
                    if (LoadNextLine()) {
                        StartCoroutine(IPlayDialogue());
                    } else {
                        StartCoroutine(IEndDialogue());
                    } break;
            }
        }
        audioCD = Mathf.MoveTowards(audioCD, 0, Time.unscaledDeltaTime);
    }

    private void PlayDialogueAudio() {
        audioCD = audioInterval;
    }

    public void DoDialogue(DialogueData data) {
        dialogueBox.gameObject.SetActive(true);
        currDialogue = data;
        currLine = -1;
        StartCoroutine(IBeginDialogue());
    }

    private IEnumerator IBeginDialogue() {
        while (dialogueBox.localScale.y < 1) {
            dialogueBox.localScale = new Vector3(dialogueBox.localScale.x,
                                                 Mathf.MoveTowards(dialogueBox.localScale.y, 1,
                                                                   Time.unscaledDeltaTime * 5),
                                                 dialogueBox.localScale.z);
            yield return null;
        } LoadNextLine();
        yield return new WaitForSeconds(0.2f);
        PlayDialogue();
    }

    private IEnumerator IEndDialogue() {
        state = State.Idle;
        ResetText();
        characterResolver.HideCharacters();
        while (dialogueBox.localScale.y > 0) {
            dialogueBox.localScale = new Vector3(dialogueBox.localScale.x,
                                                 Mathf.MoveTowards(dialogueBox.localScale.y, 0,
                                                                   Time.unscaledDeltaTime * 5),
                                                 dialogueBox.localScale.z);
            yield return null;
        } yield return new WaitForSeconds(0.5f) ;
        OnDialogueEnd?.Invoke();
    }

    private void PlayDialogue() {
        StopAllCoroutines();
        StartCoroutine(IPlayDialogue());
    }

    private IEnumerator IPlayDialogue() {
        state = State.Writing;
        while (true) {
            string str = "<line-height=100%>";
            for (int i = 0; i < currLetter && i < currStr.Length; i++) {
                if (currStr[i] == ' ') {
                    if (!growthSet[i]) {
                        currLetter++;
                        growthSet[i] = true;
                    }
                    str += " ";
                    continue;
                }
                if (!growthSet[i]) {
                    sizeSet[i] = Mathf.MoveTowards(sizeSet[i], fontBounds.y, Time.unscaledDeltaTime * textGrowthMult);
                    if (sizeSet[i] >= fontBounds.y) {
                        growthSet[i] = true;
                        currLetter++;
                    }
                } else {
                    sizeSet[i] = Mathf.MoveTowards(sizeSet[i], fontBounds.x, Time.unscaledDeltaTime * textGrowthMult * 0.5f);
                }
                if (state != State.Awaiting
                    && currLetter >= currStr.Length) {
                    state = State.Awaiting;
                }
                str += $"<size={sizeSet[i]}>{currStr[i]}</size>";
            }
            textMesh.text = str;
            yield return null;
        }
    }

    private bool LoadNextLine() {
        bool hasNextLine = ++currLine < currDialogue.lines.Length;
        if (hasNextLine) {
            currLetter = 1;
            DialogueLine line = currDialogue.lines[currLine];
            characterResolver.UpdateCharacters(line);
            currStr = line.lineText;
            growthSet = new bool[currStr.Length];
            sizeSet = new float[currStr.Length];
        }
        return hasNextLine;
    }

    private void ResetText() {
        currLetter = 1;
        currLine = -1;
        currStr = "";
        growthSet = null;
        sizeSet = null;
        currDialogue = null;
    }
}
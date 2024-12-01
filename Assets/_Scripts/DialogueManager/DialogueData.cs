using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject {
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine {
    [TextArea] public string lineText;
    public CharacterData leftCharacter, rightCharacter;
    public LeftOrRight leftOrRight;
}
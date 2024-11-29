using UnityEngine;

public class DialogueCharacterResolver : MonoBehaviour {

    [SerializeField] private DialogueCharacterController rightCharacter,
                                                         leftCharacter;

    public void UpdateCharacters(DialogueLine line) {
        rightCharacter.UpdateCharacter(line.rightCharacter,
                                       line.leftOrRight == LeftOrRight.Right);
        leftCharacter.UpdateCharacter(line.leftCharacter,
                                      line.leftOrRight == LeftOrRight.Left);
    }

    public void HideCharacters() {
        rightCharacter.UpdateCharacter(null, false);
        leftCharacter.UpdateCharacter(null, false);
    }
}
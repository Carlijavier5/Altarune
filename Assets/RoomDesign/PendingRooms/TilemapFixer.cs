using System.Collections.Generic;
using UnityEngine;

public class TilemapFixer : MonoBehaviour {

    public void DisplaceTiles(Vector3 offset) {
        foreach (Transform t in transform) {
            t.position += offset;
        }
    }

    public void UnparentChildren() {
        Stack<Transform> stack = new();
        foreach (Transform tParent in transform) {
            foreach (Transform tChild in tParent) {
                stack.Push(tChild);
            }
        }
        while (stack.TryPop(out Transform tChild)) {
            tChild.SetParent(transform);
        }
    }
}
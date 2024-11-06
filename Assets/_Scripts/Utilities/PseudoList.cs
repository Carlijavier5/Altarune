using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PseudoList : IEnumerable<MonoBehaviour> {
    public List<MonoBehaviour> innerList;

    public MonoBehaviour this[int x] {
        get {
            return innerList[x];
        }
    }
    public void RemoveAt(int index) {
        innerList.RemoveAt(index);
    }

    public IEnumerator<MonoBehaviour> GetEnumerator() {
        return innerList.GetEnumerator();
    }
    // im not sure this works...
    IEnumerator IEnumerable.GetEnumerator() {
        return innerList.GetEnumerator();
    }
}
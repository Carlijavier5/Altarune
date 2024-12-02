using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaBar : MonoBehaviour {

    [SerializeField] private PlayerManaBarSlot[] slots;
    [SerializeField] private ManaSource manaSource;
}

public class PlayerManaBarSlot : MonoBehaviour {

    [SerializeField] private Image image;
}
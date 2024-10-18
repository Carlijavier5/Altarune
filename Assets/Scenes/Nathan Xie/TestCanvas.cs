using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanvas : MonoBehaviour
{

    public event System.Action<int> OnDamageTaken;
    public event System.Action<int> OnHealReceived;
    public event System.Action<int> OnEntityInit; 

    public int Health = 10;
    // Start is called before the first frame update
    void Start()
    {
        OnEntityInit?.Invoke(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageClicked(){
        Health -= 1;
        OnDamageTaken?.Invoke(1);
    }

    public void HealthClicker(){
        Health += 1;
        OnHealReceived?.Invoke(1);
    }
}

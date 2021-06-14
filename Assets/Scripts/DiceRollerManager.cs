using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollerManager : MonoBehaviour
{
    public DiceRoller[] dices;
    // Start is called before the first frame update
    private static List<KeyCode> targets = new List<KeyCode>{
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6
    };
    
    void Start()
    {
        
    }
    List<int> prevInputs = new List<int>();
    // Update is called once per frame
    private void Update() {
        var broken = false;
        for (var i = 0; i < dices.Length;i++){
            if(dices[i].Running) return;
            broken |= dices[i].Failed;
        }
        if(!broken) HandleInput();
        else
        {
            Roll();
        }
    }
    void Roll(){
        for (var i = 0; i < dices.Length;i++){
            if(dices[i].Running) return;
        }
        prevInputs.Clear();
        for (var i = 0; i < dices.Length;i++){
            dices[i].Roll();
        }
    }
    void HandleInput()
    {
        
        if(Input.GetKeyDown(KeyCode.Space)){
            Roll();
        }
        for (var i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(targets[i]))
            {
                prevInputs.Add(i);
                if (prevInputs.Count == dices.Length)
                {
                    for (var j = 0; j < dices.Length;j++){
                        if(dices[j].Running){
                            prevInputs.Clear();
                            return;
                        }
                    }
                    for (var j = 0; j < dices.Length;j++)
                        dices[j].Rerun(prevInputs[j]);
                    prevInputs.Clear();
                }
                break;
            }
        }
    }
}

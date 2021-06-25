using UnityEngine;
using System.Collections.Generic;
namespace DiceRoller
{


public class DiceRollerInput : MonoBehaviour
{
    public DiceManager manager;
    List<int> prevInputs = new List<int>();
    private static List<KeyCode> targets = new List<KeyCode>{
    KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6
};
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.Record();
        }
        for (var i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(targets[i]))
            {
                prevInputs.Add(i);
                if (prevInputs.Count == manager.dices.Length)
                {
                    manager.Roll(prevInputs.ToArray());
                    prevInputs.Clear();
                }
                break;
            }
        }
    }
}
}
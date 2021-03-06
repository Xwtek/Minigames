using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace DiceRoller
{
    public class DiceManager : MonoBehaviour
    {
        public Dice[] dices;
        private Queue<RecordedAnimation[]> diceAnimations = new Queue<RecordedAnimation[]>();
        private int pcount;
        public int recordCount = 20;
        private bool diceRolled = false;
        private bool first = true;
        int[] currentInput;
        bool changed = false;

private void Update()
{
    pcount = diceAnimations.Count;
    if (changed) CancelAll();
    if (!FinishedAll) return;
    if (diceAnimations.Count < recordCount)
    {
        var recorded = GetCurrentRecordedPath();
        if (first) first = false; // Don't use the first recording as it's garbage
        else if (recorded != null)
        {
            diceAnimations.Enqueue(recorded);
            Debug.Log("Recording the dice animation");
        }
        else
        {
            Debug.Log("Discarding the dice animation due to error");
        }
    }
    if (currentInput == null)
    {
        if (diceAnimations.Count < recordCount) RecordAll();
        Debug.Log("Start to record");
    }
    else if (diceAnimations.Count > 0 && !diceRolled)
    {
        diceRolled = true;
        RerunAll(currentInput);
        Debug.Log("Starts rerolling with dice configuration : " + string.Join(", ", currentInput.Select(x => x.ToString())));
    }
    else if (diceAnimations.Count == 0 && !diceRolled)
    {
        diceRolled = false;
        RecordAll();
        Debug.Log("Recording dice instead due to not enough baked animations");
    };
    changed = false;
}
public void Roll(int[] roll)
{
    changed = true;
    currentInput = roll;
    diceRolled = false;
}
public void Record()
{
    changed = currentInput == null;
    currentInput = null;
}
        /// <summary>
        /// Returns all if all dice animations are stopped
        /// </summary>
        public bool FinishedAll => dices.All(x => x.finished);
        /// <summary>
        /// Cancel all animation of dices
        /// </summary>
        void CancelAll()
        {
            foreach (var dice in dices)
            {
                dice.cancel = true;
            }
        }
        /// <summary>
        /// Make all dices roll with a specified number
        /// </summary>
        /// <param name="roll"> The target number of each dice </param>
        void RerunAll(int[] roll)
        {
            DiceShown = true;
            var currRecord = diceAnimations.Dequeue();
            for (var i = 0; i < dices.Length; i++)
            {
                dices[i].Reroll(currRecord[i], roll[i]);
            }
        }
        private bool diceShown = true;
        private bool DiceShown
        {
            get => diceShown;
            set
            {
                if (DiceShown == value) return;
                diceShown = value;
                foreach (var dice in dices)
                {
                    dice.ShowDice = value;
                }
            }
        }
        /// <summary>
        /// Record all dice animations
        /// </summary>
        void RecordAll()
        {
            DiceShown = false;
            for (var i = 0; i < dices.Length; i++)
            {
                dices[i].Record();
            }
        }
        RecordedAnimation[] GetCurrentRecordedPath()
        {
            var result = dices.Select(x => x.recordedAnimation).ToArray();
            if (result.Any(x => x == null)) return null;
            return result;
        }
    }

}
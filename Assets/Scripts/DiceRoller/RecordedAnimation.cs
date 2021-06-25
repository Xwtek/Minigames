using System.Collections.Generic;
using UnityEngine;
namespace DiceRoller{
    [System.Serializable]
public class RecordedAnimation
{
    public List<(Vector3, Quaternion)> recording = new List<(Vector3, Quaternion)>();
    public int resultFace;
    public void Record(Vector3 position, Quaternion rotation)
    {
        recording.Add((position, rotation));
    }
}
}
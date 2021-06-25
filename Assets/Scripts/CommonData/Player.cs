using UnityEngine;
namespace CommonData
{
[CreateAssetMenu(fileName = "Player", menuName = "Game/Player", order = 0)]
public class Player : ScriptableObject
{
    public Texture2D image;
    public string playerName;
}
}
using UnityEditor;
using UnityEngine;
using SnakeLadder;
public class LayASquare{
    [MenuItem("Helper/Lay a square")]
    public static void LayBoard(){
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Square.prefab");
        Debug.Log(prefab.GetType());
        for (var i = 0; i < 10; i++){
            for (var j = 0; j < 10; j++){
                var laid = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                laid.transform.position = new Vector3(i, j, 0);
                var text = laid.GetComponentInChildren<TextMesh>();
                text.text = SnakeLadderHelper.FromCoordinate(new Unity.Mathematics.int2(i, j)).ToString();
            }
        }
    }
}
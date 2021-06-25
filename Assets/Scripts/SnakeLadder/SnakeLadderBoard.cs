using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CommonData;
using Unity.Mathematics;
using Random = UnityEngine.Random;
namespace SnakeLadder
{
    public class SnakeLadderBoard : MonoBehaviour
    {
        SnakeLadderLogic logic;
        public Token[] tokens;
        public GameObject tokenPrefab;
        public Transform tokenStorage;
        public float distance;
        public float timePerBox;

        public bool finished = true;
        private bool IsMoving => tokens.Any((token) => token.IsMoving);
        public int currentPlayer => logic.currentPlayer;
        public int playerCount => tokens.Length;
        public void NextPlayer(){
            logic.NextPlayer();
        }
        public void PlayNewGame(Player[] players, (int, int)[] traps, bool takeback)
        {
            logic = new SnakeLadderLogic(players.Length, traps, takeback);
            finished = true;
            tokens = players.Select((player, index) => PlaceToken(player.name, player.image, index)).ToArray();
        }
        private Token PlaceToken(string name, Texture2D image, int index){
            var placed = Instantiate(tokenPrefab, Vector3.zero, Quaternion.identity, tokenStorage);
            var token = placed.GetComponent<Token>();
            token.image = image;
            token.name = name;
            token.MoveNext(0, logic.playerPos.Length + 1, index+1, distance, 0f);
            return token;
        }
        private void MoveTo(int targetSquare, bool finalMove){
            var origSquare = logic.playerPos[logic.currentPlayer];
            var origToken = new BitSet();
            var origTokenCount = 0;
            var toToken = new BitSet();
            var toTokenCount = 0;
            for (var i = 0; i < logic.playerPos.Length; i++)
            {
                if (i == logic.currentPlayer) continue;
                else if (logic.playerPos[i] == origSquare) { origToken[i] = true; origTokenCount++; }
                else if (logic.playerPos[i] == targetSquare) { toToken[i] = true; toTokenCount++; }
            }
            var origTokenProcessed = 0;
            var toTokenProcessed = 0;
            for (var i = 0; i < logic.playerPos.Length; i++){
                if (i == logic.currentPlayer)
                {
                    if(finalMove && toTokenCount > 0) tokens[i].MoveNext(targetSquare, toTokenCount + 2, 1, distance, timePerBox);
                    else tokens[i].MoveNext(targetSquare, toTokenCount + 1, 0, distance, timePerBox);
                }
                else if (logic.playerPos[i] == origSquare)
                {
                    origTokenProcessed++;
                    if (origTokenCount == 1) origTokenProcessed = 0;
                    tokens[i].MoveNext(origSquare, origTokenCount + 1, origTokenProcessed, distance, timePerBox);
                }
                else if (logic.playerPos[i] == targetSquare)
                {
                    toTokenProcessed++;
                    if(finalMove) tokens[i].MoveNext(targetSquare, toTokenCount + 2, toTokenProcessed + 1, distance, timePerBox);
                    else tokens[i].MoveNext(targetSquare, toTokenCount + 1, toTokenProcessed, distance, timePerBox);
                }
            }
        }
        public void NextTurn(int2 roll){
            if(!finished || logic == null) return;
            StartCoroutine(TokenCoroutine(roll));
        }
        private IEnumerator TokenCoroutine(int2 diceroll)
        {
            finished = false;
            foreach (var movement in logic.Roll(diceroll))
            {
                while (IsMoving) yield return null;
                MoveTo(movement.target, movement.final);
            }
            while (IsMoving) yield return null;
            finished = true;
        }
    }
}
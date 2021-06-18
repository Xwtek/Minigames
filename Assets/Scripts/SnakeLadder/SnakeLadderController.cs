using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CommonData;
using Unity.Mathematics;
using Random = UnityEngine.Random;
namespace SnakeLadder
{
    public class SnakeLadderController : MonoBehaviour
    {
        SnakeLadderLogic logic;
        Token[] tokens;
        MoveResult? currAnim;
        public GameObject tokenPrefab;
        public Transform tokenStorage;
        public float distance;
        public float timePerBox;
        AnimationState animationState;
        public bool focus = true;
        private void Update()
        {
            NextTurn();
            if(focus){

            }
        }
        public void PlayNewGame(Player[] players, (int, int)[] traps, bool takeback)
        {
            logic = new SnakeLadderLogic(players.Length, traps, takeback);
            currAnim = null;
            tokens = players.Select((player, index) => PlaceToken(player.name, player.image, index)).ToArray();
        }
        private Token PlaceToken(string name, Texture2D image, int index){
            var placed = Instantiate(tokenPrefab, Vector3.zero, Quaternion.identity, tokenStorage);
            var token = placed.GetComponent<Token>();
            token.texture = image;
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
        private void NextTurn(){
            if(coroutineRunning || logic == null) return;
            StartCoroutine(TokenCoroutine(new int2(Random.Range(1,7), Random.Range(1,7))));
        }
        private IEnumerator TokenCoroutine(int2 diceroll)
        {
            cancelCoroutine = false;
            coroutineRunning = true;
            foreach (var movement in logic.Roll(diceroll))
            {
                while (IsMoving)
                {
                    if (cancelCoroutine)
                    {

                        coroutineRunning = false;
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                MoveTo(movement.target, movement.final);
            }
            while (IsMoving)
            {
                if (cancelCoroutine)
                {
                    coroutineRunning = false;
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            coroutineRunning = false;
        }
        bool coroutineRunning = false;
        bool cancelCoroutine = false;
        private bool IsMoving => tokens.Any((token) => token.IsMoving);
        enum AnimationState{
            Forward = 1,
            Takeback = 2,
            Trap = 3,
            None = 0
        }
    }
}
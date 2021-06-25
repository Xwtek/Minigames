using UnityEngine;
using System.Collections;
using DiceRoller;
using Unity.Mathematics;
using CommonData;
using Random = UnityEngine.Random;
namespace SnakeLadder
{
    public class SnakeLadderController : MonoBehaviour
    {
        public SnakeLadderBoard controller;
        public SnakeLadderCamera slCamera;
        public DiceManager roller;

        int? focused;
        bool keepTrack;
        public int playerIndex;
        bool rollTime;
        bool canZoom;
        bool zoomedAway;
        bool started;
        bool turnFinished = true;
        public float waitTime;
        private void Update()
        {
            if (started)
            {
                ChangeZoom();
                if (turnFinished) StartCoroutine(Turn());
            }
        }
        public void PlayNewGame(Player[] players, (int, int)[] traps, bool takeback)
        {
            controller.PlayNewGame(players, traps, takeback);
            started = true;
        }
        private void ChangeZoom()
        {
            if (Input.GetButtonDown("GlobalView"))
            {
                focused = keepTrack ? null as int? : controller.currentPlayer;
                keepTrack = focused.HasValue;
                zoomedAway = false;
            }
            else if (Input.GetButtonDown("Horizontal") && canZoom)
            {
                if (focused == null) focused = controller.currentPlayer;
                focused += (int)(Mathf.Sign(Input.GetAxisRaw("Horizontal")));
                while (focused < 0) focused += controller.playerCount;
                while (focused >= controller.playerCount) focused -= controller.playerCount;
                keepTrack = focused == controller.currentPlayer;
                zoomedAway = focused != controller.currentPlayer;
            }
            if (keepTrack) focused = controller.currentPlayer;
            if (focused == null) slCamera.focusOn = null;
            else slCamera.focusOn = controller.tokens[focused.Value];
        }
        IEnumerator Turn()
        {
            turnFinished = false;
            canZoom = playerIndex == controller.currentPlayer;
            var roll = new int2(Random.Range(1, 7), Random.Range(1, 7));
            //if (playerIndex == controller.currentPlayer)
            //{
            //    while (zoomedAway || !Input.GetButtonDown("Go")) yield return null;
            //}
            roller.Roll(new int[] { roll.x - 1, roll.y - 1 });
            yield return null;
            while (!roller.FinishedAll) yield return null;
            //if (playerIndex == controller.currentPlayer)
            //{
            //    while (zoomedAway || !Input.GetButtonDown("Go")) yield return null;
            //}
            //else
            {
                yield return new WaitForSeconds(waitTime);
            }
            roller.Record();
            canZoom = false;
            controller.NextTurn(roll);
            yield return null;
            while (!controller.finished) yield return null;
            canZoom = playerIndex == controller.currentPlayer;
            //if (playerIndex == controller.currentPlayer)
            //{
            //    while (!Input.GetButtonDown("Go")) yield return null;
            //}
            //else
            {
                yield return new WaitForSeconds(waitTime);
            }
            controller.NextPlayer();
            turnFinished = true;
        }
    }
}
using UnityEngine;
using CommonData;
namespace SnakeLadder
{
    public class DummyInitializer : MonoBehaviour
    {
        public Player[] players;
        public SnakeLadderController controller;
        bool initialized = false;
        private void Start()
        {
            if (controller == null) controller = GetComponent<SnakeLadderController>();
        }
        private void Update()
        {
            if (!initialized)
            {
                controller.PlayNewGame(players, new (int, int)[]{
                    (7, 14), (21, 28), (35, 42), (49, 56), (63, 70), (77, 84),
                    (12, 6), (24, 18), (36, 30), (60, 54), (72, 66), (96, 90)
                }, true);
                initialized = true;
            }
        }
    }
}
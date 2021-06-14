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
                controller.PlayNewGame(players, new (int, int)[0], true);
                initialized = true;
            }
        }
    }
}
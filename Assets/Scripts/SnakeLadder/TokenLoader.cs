using UnityEngine;
namespace SnakeLadder
{
    public class TokenLoader : MonoBehaviour
    {
        public Token target;
        string url = "/home/xwtek/TestAvatars/84930.png";
        private void Start()
        {
            var texture = new Texture2D(2,2);
            var bytes = System.IO.File.ReadAllBytes(url);
            texture.LoadImage(bytes);
            target.image = texture;
        }
    }
}
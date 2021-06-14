using Unity.Mathematics;
using UnityEngine;
using System;
namespace SnakeLadder
{
    public static class SnakeLadderHelper
    {
        /// <summary>
        /// Translate the box number from the coordinate number.
        /// The board runs bottom to top and alternatingly right to left and left to right 
        /// </summary>
        /// <param name="coordinate"> Coordinate in form of (x, y) where bottom-left is (0,0)</param>
        /// <returns>Zero based numbering of the box </returns>
        public static int FromCoordinate(int2 coordinate)
        {
            return coordinate.y * 10 + ((coordinate.y & 1) == 0 ? coordinate.x : 9 - coordinate.x);
        }
        /// <summary>
        /// Translate the box number into the coordinate number.
        /// The board runs bottom to top and alternatingly right to left and left to right 
        /// </summary>
        /// <param name="coordinate">Zero based numbering of the box</param>
        /// <returns>Coordinate in form of (x, y) where bottom-left is (0,0)</returns>
        public static int2 ToCoordinate(int index)
        {
            var pair = new int2(index % 10, index / 10);
            if ((pair.y & 1) == 1) pair.x = 9 - pair.x;
            return pair;
        }
        public static Vector3 ToZoomingCoordinate(this int2 boardCoordinate, float zOrder){
            return new Vector3(boardCoordinate.x, boardCoordinate.y, zOrder);
        }
    }
}
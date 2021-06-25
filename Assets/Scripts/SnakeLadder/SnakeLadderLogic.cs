using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class SnakeLadderLogic{
    public int currentPlayer = 0;
    public int[] playerPos;
    public (int, int)[] traps;
    public bool takeback;
    public IEnumerable<TokenMovement> Roll(int2 diceroll)
    {
        if(playerWon.HasValue) yield break;
        var moveBy = 1;
        var offset = math.csum(diceroll);
        var currPos = playerPos[currentPlayer];
        while(offset > 1){
            offset--;
            if(currPos == 99) moveBy = -1;
            if(moveBy == -1 && !takeback) yield break;
            currPos += moveBy;
            if(currPos == 100) Debug.Log("No!");
            yield return new TokenMovement { target = currPos, final = false };
            playerPos[currentPlayer] = currPos;
        }
        if(currPos == 99) moveBy = -1;
        if(moveBy == -1 && !takeback) yield break;
        currPos += moveBy;
        var trap = TrapLeadsInto(currPos);
        yield return new TokenMovement { target = currPos, final = !trap.HasValue };
        playerPos[currentPlayer] = currPos;
        if(trap.HasValue){
            yield return new TokenMovement { target = trap.Value, final = true };
            playerPos[currentPlayer] = trap.Value;
        }
    }
    int? TrapLeadsInto(int place){
        for (int i = 0; i < traps.Length; i++){
            if(traps[i].Item1 == place) return traps[i].Item2;
        }
        return null;
    }
    public SnakeLadderLogic(int maxPlayer, (int, int)[] traps, bool takeback){
        this.playerPos = new int[maxPlayer];
        CheckTrapValid(traps);
        this.traps = traps;
        this.takeback = takeback;
    }
    public int? playerWon;

    internal static void CheckTrapValid((int, int)[] traps){
        uint s66t99 = 0;
        ulong s2t65 = 0;
        for (var i = 0; i < traps.Length; i++){
            var (src, dst) = traps[i];
            IsContained(src, ref s66t99, ref s2t65, $"traps[{i}].Item1");
            IsContained(dst, ref s66t99, ref s2t65, $"traps[{i}].Item2");
        }
    }
    internal static void IsContained(int index, ref uint higher, ref ulong lower, string indication){
        if(index < 1 ||index > 99) throw new ArgumentOutOfRangeException(indication, index, "The box number is out of range.");
        else if(index > 64){
            var mask = 1u << (index - 65);
            if((higher & mask)>0) throw new ArgumentException("Cannot use this box because it has been used for earlier traps", indication);
            higher |= mask;
        }else{
            var mask = 1ul << (index - 1);
            if((lower & mask)>0) throw new ArgumentException("Cannot use this box because it has been used for earlier traps", indication);
            lower |= mask;
        }
    }
    public void NextPlayer(){
        currentPlayer++;
        if(currentPlayer == playerPos.Length) currentPlayer = 0;
    }
}
public struct TokenMovement{
    public int target;
    public bool final;
}
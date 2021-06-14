using System;
namespace CommonData
{
    public struct BitSet
    {
        ulong backing;
        public bool this[int index]
        {
            get => index >= 0 && index < 64 ? (backing & (1ul << index)) > 0 : throw new IndexOutOfRangeException();
            set
            {
                if (index < 0 || index > 63) throw new IndexOutOfRangeException();
                if (value)
                {
                    backing |= 1ul << index;
                }
                else
                {
                    backing &= ~(1ul << index);
                }
            }
        }
        public int Count
        {
            get
            {
                var bitcount = backing;
                var pat = 0x5555555555555555u;
                bitcount = bitcount & pat + bitcount & (pat << 1);
                pat = 0x3333333333333333u;
                bitcount = bitcount & pat + bitcount & (pat << 2);
                pat = 0x0f0f0f0f0f0f0f0fu;
                bitcount = bitcount & pat + bitcount & (pat << 4);
                pat = 0x00ff00ff00ff00ffu;
                bitcount = bitcount & pat + bitcount & (pat << 8);
                pat = 0x0000ffff0000ffffu;
                bitcount = bitcount & pat + bitcount & (pat << 16);
                pat = 0x00000000ffffffffu;
                bitcount = bitcount & pat + bitcount & (pat << 16);
                return (int) bitcount;
            }
        }
        public BitSet(ulong backing) { this.backing = backing; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraTCG.Common.Util
{
    internal static class StringHash
    {
        // have to use baked-in hash function since String.GetHashCode() isn't
        // consistent across versions or platforms
        // Uses the 32-bit FNV-1a hash
        // https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        public static uint StableStringHash(string input) 		
        {
            uint hash = 2166136261;
            for (int i = 0; i < input.Length; i++)
            {
                hash ^= input[i];
                hash *= 16777619;
            }
            return hash;
        }
    }
}

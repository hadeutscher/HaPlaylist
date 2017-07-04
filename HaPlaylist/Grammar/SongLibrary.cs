using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HaPlaylist.Grammar
{
    public class SongLibrary
    {
        private IEnumerable<Song> songs;
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static long GetRandom(long loBound, long hiBound)
        {
            byte[] data = new byte[8];
            rng.GetBytes(data);
            ulong randLong = BitConverter.ToUInt64(data, 0);
            return loBound + (long)(randLong % (ulong)(hiBound - loBound));
        }

        public static List<T> Shuffle<T>(IEnumerable<T> source)
        {
            List<T> list = source.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                int swap_i = (int)GetRandom(i, list.Count);
                T temp = list[i];
                list[i] = list[swap_i];
                list[swap_i] = temp;
            }
            return list;
        }

        public SongLibrary(IEnumerable<Song> songs)
        {
            this.songs = songs;
        }

        public IEnumerable<Song> Select(BooleanExpression expression)
        {
            return songs.Where(x => expression.Evaluate(x.EvaluationContext));
        }

        public List<Song> RandomSelect(BooleanExpression expression)
        {
            return Shuffle(Select(expression));
        }

        public IEnumerable<Song> Songs { get => songs; set => songs = value; }
    }
}

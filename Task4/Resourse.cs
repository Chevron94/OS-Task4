using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Task4
{
    class Resourse
    {
        int number;
        int count;
        int free;
        public Resourse(int _number, int max_count)
        {
            number = _number;
            Random rnd = new Random();
            count = rnd.Next(max_count)+1;
            free = count;
            Thread.Sleep(1);
        }
        public int Number
        { get { return number; } }

        public int Count
        {
            get { return count; }
        }

        public int Free
        {
            get { return free; }
        }

        public void Free_Resourse(int count_free)
        {
            free += count_free;
        }

        public bool TryGetResource(int cnt)
        {
            if (free < cnt)
                return false;
            else
            {
                free -= cnt;
                return true;
            }
        }
    }
}

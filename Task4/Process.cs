using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Task4
{
    struct ResHav
    {
        public int ResName;
        public int CntUse;
    }
    class Process
    {
        int number;
        ResHav[] resources;
        int c;
        public Process(int _num, Resourse[] _count_resources)
        {
            number = _num;
            Random rnd = new Random();
            int tmp = rnd.Next(2, 4);
           // tmp = 2;
            resources = new ResHav[2*tmp];
            HashSet<int> used = new HashSet<int>();
            int x = 0;
            int count_add = 0;
            for (int i = 0; i<2*tmp; i++)
            {
                do
                {
                    bool good = true;
                    do
                    {
                        x = rnd.Next(-1, 2) * (rnd.Next(1000) % _count_resources.Length + 1);
                        if (x == 0 || x < 0 && !used.Contains(Math.Abs(x)))
                            good = false;
                        else good = true;
                        if (count_add == tmp && x > 0)
                            good = false;
                    }
                    while (!good);
                }while (x==0 || used.Contains(x));
                ResHav tmp_res = new ResHav() { ResName = x };
                if (x > 0)
                {
                    tmp_res.CntUse = rnd.Next(_count_resources[x - 1].Count)+1;
                }
                else
                {
                    foreach (ResHav r in resources)
                    {
                        if (r.ResName == -x)
                            tmp_res.CntUse = r.CntUse;
                    }
                }
                resources[i] = tmp_res;
                used.Add(x);
                if (x > 0)
                    count_add++;
            }
            c = 0;
            Thread.Sleep(100);
        }
        public int Number
        {
            get { return number; }
            set{number = value;}
        }

        public ResHav[] Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public ResHav Next_Resource()
        {
                if (c < resources.Length)
                {
                    ResHav res = resources[c];
                    c++;
                    return res;
                }
                else return new ResHav() { ResName = 0 }; ;
        }

        public void Reset()
        {
            c = 0;
        }

        public ResHav Seek_Next()
        {

                if (c == resources.Length)
                    return new ResHav(){ResName = 0};
                else return resources[c];
        }
    }
}

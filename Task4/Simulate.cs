using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Task4
{ 
    enum State {Work, Locked, Finished};
    struct Enabled
    {
        public Process Process;
        public State Locked;

        public void Reset()
        {
            Process.Reset();
            Locked = State.Work;
        }
    }

    class Simulate
    {
        Enabled[] Process; // Процессы
        List<int>[] RATABLE; // Таблица распред ресурсов:   ресурс - процессы.
        int[][] PWTABLE;// Таблица заблокированных процессов: процесс - ресурс    0 - процесс свободен
        DataGridView DG_PATABLE;
        DataGridView DG_PWTABLE;
        ListBox lb;
        Resourse[] Resourses;
        bool stop;
        int k1 = 0;
        //Избавление от печати одного и того же
        List<List<string>> Deadlocks = new List<List<string>>();
        List<string> current;
        //
        public Simulate(ref DataGridView _ra, ref DataGridView _pt, ref ListBox _lb)
        {
            DG_PATABLE = _ra;
            DG_PWTABLE = _pt;
            lb = _lb;
            stop = false;
        }
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="i">номер ресурса</param>
        /// <param name="watch">отображать лог</param>
        void Free_Some_Resource(ResHav i, int num_proc, bool watch = true)
        {
            Resourses[i.ResName - 1].Free_Resourse(i.CntUse);
            DG_PATABLE.Rows[i.ResName - 1].Cells[1].Value = Resourses[i.ResName - 1].Free;
            Application.DoEvents();
            if (watch)
            {
                lb.Items.Add("Процесс " + (num_proc).ToString() + " освободил " + i.CntUse.ToString()+" Едениц ресурса " +i.ResName.ToString());
                
                Application.DoEvents();
                Thread.Sleep(500);
            }
            else current.Add("Процесс " + (num_proc).ToString() + " освободил " + i.CntUse.ToString() + " Едениц ресурса" + i.ResName.ToString());
            if (Process[num_proc - 1].Process.Seek_Next().ResName == 0)
                Process[num_proc - 1].Locked = State.Finished;
            RATABLE[i.ResName-1].Remove(num_proc);
            
            int numproc = 0;
            int c_r = 0;
            for (int j = 0; j < PWTABLE.Length && numproc == 0; j++)
            {
                if (PWTABLE[j][0] == i.ResName && PWTABLE[j][1] <= Resourses[i.ResName-1].Free)
                {
                    DG_PWTABLE.Rows[j].Cells[1].Value = null;
                    DG_PWTABLE.Rows[j].Cells[2].Value = null;
                    Application.DoEvents();
                    numproc = j+1;
                    PWTABLE[j][0] = 0;
                    c_r = PWTABLE[j][1];
                    PWTABLE[j][1] = 0;
                    RATABLE[i.ResName - 1].Add(numproc);
                    Resourses[i.ResName - 1].TryGetResource(c_r);
                }
            }

            if (numproc != 0)
            {
                Process[numproc-1].Locked = State.Work;
                if (watch)
                {
                    lb.Items.Add("Процесс " + (numproc).ToString() + " был разблокирован ");
                    lb.Items.Add("Процесс " + (numproc).ToString() + " получил " + c_r.ToString()+ " едениц ресурса " + i.ResName.ToString());

                    Application.DoEvents();
                    Thread.Sleep(1000);
                }
                else
                {
                    current.Add("Процесс " + (numproc).ToString() + " был разблокирован ");
                    current.Add("Процесс " + (numproc).ToString() + " получил " + c_r.ToString() + " едениц ресурса " + i.ResName.ToString());
                }
                DG_PATABLE.Rows[i.ResName - 1].Cells[1].Value = Resourses[i.ResName-1].Free;
               // RATABLE[i - 1] = numproc;
                Application.DoEvents();
                
            }

        }
        /// <summary>
        /// Инициализация Процессов и таблиц
        /// </summary>
        public void PrepareToStart()
        {
            Random rnd = new Random();
            int c_process = rnd.Next(1000)%3+2;
            int c_resources = rnd.Next(1000)%2+3;
            //c_process = 3;
            //c_resources = 3;
            RATABLE = new List<int>[c_resources];
            for (int i = 0; i < c_resources; i++)
                RATABLE[i] = new List<int>();
            Resourses = new Resourse[c_resources];
            for (int i = 0; i < c_resources; i++)
            {
                Resourses[i] = new Resourse(i+1,5);
            }
            PWTABLE = new int[c_process][];
            for (int i = 0; i < c_process; i++)
                PWTABLE[i] = new int[2];
            Process = new Enabled[c_process];
            DG_PATABLE.Rows.Clear();
            DG_PWTABLE.Rows.Clear();
            DG_PWTABLE.RowCount = c_process;
            DG_PATABLE.RowCount = c_resources;
            for (int i = 0; i < c_resources; i++)
            {
                DG_PATABLE.Rows[i].Cells[0].Value = i+1;
                DG_PATABLE.Rows[i].Cells[1].Value = Resourses[i].Free;
            }
            for (int i = 0; i < c_process; i++)
            {
                Process Process_tmp = new Process(i + 1, Resourses);
                Process[i] = new Enabled() { Locked = State.Work, Process = Process_tmp};
                DG_PWTABLE.Rows[i].Cells[0].Value = i+1;
                DG_PWTABLE.Rows[i].Cells[1].Value = null;
                PWTABLE[i][0] = 0;
                PWTABLE[i][1] = 0;
            }
             
        }
        /// <summary>
        /// Симуляция случайных процессов
        /// </summary>
        public void Run()
        {
            List<int> a = new List<int>();
            while (!stop)
            {
                PrepareToStart();
                lb.Items.Add("Process:");
                foreach (Enabled e in Process)
                {
                    string tmp = e.Process.Number.ToString() + '\t';
                    foreach (ResHav i in e.Process.Resources)
                        tmp += " " + i.ResName.ToString() + " ("+i.CntUse.ToString()+") " ;
                    lb.Items.Add(tmp);
                }
                Application.DoEvents();
                Application.DoEvents();
                while (Next_Request(true,true,ref a))
                {
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    Random rnd = new Random();
                }
                bool All_Locked = false;
                for (int i = 0; i < Process.Length && !All_Locked; i++)
                {
                    if (PWTABLE[i][1] != 0)
                        All_Locked = true;
                }
                if (All_Locked)
                    lb.Items.Add("DEADLOCK!");
                else lb.Items.Add("GOOD FINISH!");
                Thread.Sleep(5000);

            }
        }
        void swap(int[] p,int i,int j) // замена двух элементов в векторе местами
        {
	        int tmp=p[i];
	        p[i]=p[j];
	        p[j]=tmp;
        }
        bool next_permutation(ref int[] a) 
    {
        int n = a.Length;
        int j = n-2;
        while (j!=-1 && a[j] >= a[j+1]) j--;
        if (j == -1)
        return false; // a - last permutation
        int k = n - 1;
        while (a[j] >= a[k]) k--;
        swap(a,j,k);
     
      // reverse back [j+1, n-1]
        int l = j + 1, r = n - 1;
        while (l<r)
            swap(a,l++,r--);
        return true;
    }
        void Clear_Tables()
        {
            for (int i = 0; i< RATABLE.Length; i++)
                RATABLE[i].Clear();
            Array.Clear(PWTABLE, 0, PWTABLE.Length);
            for (int i = 0; i < DG_PATABLE.RowCount; i++)
            {
                DG_PATABLE.Rows[i].Cells[1].Value = null;
            }
            for (int i = 0; i < DG_PWTABLE.RowCount; i++)
            {
                DG_PWTABLE.Rows[i].Cells[1].Value = null;
                DG_PWTABLE.Rows[i].Cells[2].Value = null;
            }
            Application.DoEvents();
        }
        /// <summary>
        /// Алгоритм поиска дедлоков
        /// </summary>
        /// <param name="random">Случайный выбор следующего действия(либо задан)</param>
        /// <param name="visible">Отображать лог</param>
        /// <param name="comb">Заданный порядок действий(если random == false)</param>
        /// <returns>Остались ли еще не обработанные действия</returns>
        bool Next_Request(bool random, bool visible,ref List<int> comb)
        {
            Random rnd = new Random();
            int next_process = -1;
            ResHav next_resource = new ResHav() { ResName = 0 };
            if (!random)
            {
                if (comb.Count == 0) // Есть ли процессы
                {
                    return false;
                }
            }
            else 
            {
                bool workers = false;
                foreach (Enabled e in Process)
                    workers = workers | (e.Locked == State.Work);
                if (!workers)
                    return false;
            }
            // ресурсы: 1..N
            // процессы 1..N
            if (random)
            {
                do
                {
                    next_process = rnd.Next(10000) % Process.Length + 1;
                    next_resource.ResName = 0;
                    if (Process[next_process - 1].Locked == State.Work)
                        next_resource = Process[next_process - 1].Process.Next_Resource();
                } while (Process[next_process - 1].Locked == State.Finished || Process[next_process - 1].Locked == State.Locked || next_resource.ResName == 0);
            }
            else
            {
                int cur_step = 0;
                do
                {
                    next_process = comb[cur_step];
                    cur_step++;
                    next_resource = new ResHav() { ResName = 0 };
                    if (Process[next_process - 1].Locked == State.Work)
                        next_resource = Process[next_process - 1].Process.Next_Resource();

                } while (Process[next_process - 1].Locked == State.Finished || Process[next_process - 1].Locked == State.Locked);
                comb.Remove(next_process);
            }
            if (next_resource.ResName < 0)
            {
                Free_Some_Resource(new ResHav() { ResName = Math.Abs(next_resource.ResName),CntUse = next_resource.CntUse},next_process, visible);
                return true;
            }
            if (Resourses[next_resource.ResName-1].Free >= next_resource.CntUse) //если ресурс свободен
            {
                RATABLE[next_resource.ResName-1].Add(next_process);
                Resourses[next_resource.ResName - 1].TryGetResource(next_resource.CntUse);
                DG_PATABLE.Rows[next_resource.ResName - 1].Cells[1].Value = Resourses[next_resource.ResName-1].Free;
                Application.DoEvents();
                if (visible)
                {
                    
                    lb.Items.Add("Процесс " + (next_process).ToString() + " получил " + next_resource.CntUse.ToString() +" едениц ресурса "+ next_resource.ResName.ToString());
                }
                else current.Add("Процесс " + (next_process).ToString() + " получил " + next_resource.CntUse.ToString() +" едениц ресурса " + next_resource.ToString());
                return true;
            }
           // HashSet<int> watched = new HashSet<int>();
            PWTABLE[next_process - 1][0] = next_resource.ResName;
            PWTABLE[next_process - 1][1] = next_resource.CntUse;
            Process[next_process - 1].Locked = State.Locked;
            DG_PWTABLE.Rows[next_process - 1].Cells[1].Value = next_resource.ResName;
            DG_PWTABLE.Rows[next_process - 1].Cells[2].Value = next_resource.CntUse;
            Application.DoEvents();
            if (visible)
            {

                lb.Items.Add("Процесс " + (next_process).ToString() + " был заблокирован по ресурсу " + (next_resource.ResName).ToString());
            }
            else current.Add("Процесс " + (next_process).ToString() + " был заблокирован по ресурсу " + (next_resource.ResName).ToString());
            bool all_locked = false;
            for (int i = 0; i < Process.Length; i++)
            {
                if (Process[i].Locked == State.Work)
                    all_locked = true ;
            }
            /*
            int proc_haver = RATABLE[next_resource-1]; // Процесс, владеющий ресурсом next_resource
            do
            {
                int wait_res = PWTABLE[proc_haver - 1]; //ресурс, который ожидается процессом proc_haver
                if (wait_res == 0)
                {
                    return true;
                }
                int proc_haver_haver = RATABLE[wait_res-1];

                if (next_process == proc_haver_haver)
                    return false;
                watched.Add(proc_haver);
                proc_haver = proc_haver_haver;
            } while (watched.Count != PWTABLE.Length);
             */

            return all_locked;

        }
        bool IsEqualLists(List<string> a, List<string> b)
        {
            for (int i = 0; i < b.Count; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }
        bool IsRepeat()
        {
            bool res = false;
            foreach (List<string> a in Deadlocks)
            {
                res = res || IsEqualLists(a, current);
            }
            return res;
        }
        /// <summary>
        /// Прерывание алгоритмов
        /// </summary>
        public bool Stop
        {
            get { return stop; }
            set { stop = value; }
        }
    }
}

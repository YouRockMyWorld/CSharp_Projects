using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionFind
{
    class QuickFind
    {
        public MyPoint[,] id;

        public QuickFind(int n)
        {
            Count = n;
            id = new MyPoint[11, 11];
            for (int i = 0; i <= 10; ++i)
            {
                for (int j = 0; j <= 10; ++j)
                {
                    id[i, j] = new MyPoint { X = i * 50, Y = j * 50 };
                }
            }
        }

        public QuickFind(List<Connection> lists)
        {

        }

        public int Count { get; private set; }

        public bool Connected(MyPoint p, MyPoint q)
        {
            //System.Diagnostics.Debug.WriteLine($"Connect:[{p.X/50},{p.Y/50}]:{p} <---> [{q.X / 50},{q.Y / 50}]:{q} , {Find(p).ValueEquals(Find(q))}");
            return Find(p).ValueEquals(Find(q));
        }


        public MyPoint Find(MyPoint p)
        {
            //System.Diagnostics.Debug.WriteLine($"Find:{p} , id[{p.X / 50},{p.Y / 50}]:{id[p.X / 50, p.Y / 50]}");
            return id[p.X / 50, p.Y / 50];
        }

        public void Union(MyPoint p, MyPoint q)
        {
            MyPoint pID = new MyPoint(Find(p));
            MyPoint qID = new MyPoint(Find(q));

            if (pID.ValueEquals(qID))
            {
                return;
            }

            for (int i = 0; i <= 10; ++i)
            {
                for (int j = 0; j <= 10; ++j)
                {
                    if (id[i, j].ValueEquals(pID))
                    {
                        //System.Diagnostics.Debug.Write($"id[{i},{j}]:{id[i, j].X},{id[i, j].Y} ---> ");
                        id[i, j].X = qID.X;
                        id[i, j].Y = qID.Y;
                        //System.Diagnostics.Debug.Write($"id[{i},{j}]:{id[i, j].X},{id[i, j].Y}\n");
                    }
                }
            }

            Count--;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MainInGame
{
    [System.Serializable]
	public class Point
	{
		public int x { get; set; }
		public int y { get; set; }
		public int z { get { return -x-y; } set { y = -x-value; } }

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static bool operator == (Point a, Point b)
		{
			return (a.x == b.x) && (a.y == b.y);
		}
		public static bool operator != (Point a, Point b)
		{
			return a != b;
		}

        // Для определения расстояния между точками. Стабильно работает для точек одной линии, для рандомных точек - неизвестно. short - т.к. не используется для точек расстояние которых больше 9
        public static short operator -(Point a, Point b)
        {
            return (short)Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        // Для определения расстояния между точками. Стабильно работает для точек одной линии, для рандомных точек - неизвестно. short - т.к. не используется для точек расстояние которых больше 9
        public int RingNum()
        {
            return (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z)) / 2;
        }

        public override string ToString()
        {
            return x.ToString() + "*" + y.ToString();
        }
        public Vector2 GetCoord2D()
        {
            float gcx = 0.9f * x;
            float gcy = -1.06695f * (y + x/2f);
            return new Vector2(gcx, gcy);
        }

        public Point GetAround(int pos)
        {
            // top rightTop rightBottom bottom leftBottom leftTop
            switch (pos)
            {
                case 0:
                    return new Point(x, y - 1);
                case 1:
                    return new Point(x + 1, y - 1);
                case 2:
                    return new Point(x + 1, y);
                case 3:
                    return new Point(x, y + 1);
                case 4:
                    return new Point(x - 1, y + 1);
                case 5:
                    return new Point(x - 1, y);
            }
            return null;
        }
    }

    [System.Serializable]
    public class Typple2<T1, T2>
    {
        public T1 x1 { get; set; }
        public T2 x2 { get; set; }
        internal Typple2(T1 x1, T2 x2)
        {
            this.x1 = x1;
            this.x2 = x2;
        }
    }
};
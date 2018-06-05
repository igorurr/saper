using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainInGame
{
    public class BoCoCell : MonoBehaviour
    {
        public static Grid grd;
        public static GameManager gm;
        public static BotController[] bcs;
        public OneHit oh;   // задаётся в инспекторе юнити

        public bool isInitialized;

        // Точки на направлениях, по ним же определяем кому принадлежит направление - массив 6 векторов
        private List<OneHit>[] pointsInLine;

        // Расстояние до ближайшей точки на линии
        private short[] nearestPointInLine;

        public float score;

        // Use this for initialization
        void Start()
        {
            score = 0;
            isInitialized = false;
        }
        public void Initialize()
        {
            isInitialized = true;

            pointsInLine = new List<OneHit>[6];
            nearestPointInLine = new short[6];

            for (int i = 0; i < 6; i++) {
                pointsInLine[i] = new List<OneHit>();
                nearestPointInLine[i] = 4;
            }
        }

        public void CheckInitialize()
        {
            if ( !isInitialized )
                Initialize();
        }

        // Update is called once per frame
        void Update()
        {

        }





        // Кто владеет данным направлением: 0-никто 1-синий 2-крассный 3-зелёный
        public int WhoLeaderInSubLine( int subline )
        {
            return 1;
        }

        /*   добавить в данную субточку точку(p), в которую только что сходили, для учёта веса
         *   гарантируется, что p действительно даходится на subline,
         *   так же гарантируется, что расстояние между p и текущей точкой действительно равно dist
         *   и что если данная функция вызывается, значит точку действительно надо добавить
        */
        public void PushPoint ( OneHit p, int subLine, short dist )
        {
            pointsInLine[subLine].Add( p );

            if ( dist > nearestPointInLine[subLine] )
                nearestPointInLine[subLine] = dist;
        }

        public void DestroySubLine ( int subLine )
        {
            pointsInLine[subLine].Clear();
            nearestPointInLine[subLine] = 4;
        }




        public void ReCountScore()
        {

            for(int i = 0; i < bcs.Length; i++)
            {
                bcs[i].ScorePointsEditPush( score, oh.ToString() );
            }
        }
    }
};
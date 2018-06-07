using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainInGame
{
    public class BotController
    {
        public static GameManager gm;

        // цвет бота
        public int myColor; // 1 - синий, 2 - крассный, 3 - зелёный

        // сколько клеток в одном направлении из 6 есть смысл проверять на возможность сходить
        private short maxCheckedLine;

        public System.Random rnd;

        public float coeffBotHit;
        public float coeffPlayerHit;

        public List<Typple2<float, string>> scorePoints;

        public BotController(int color)
        {
            myColor = color;

            rnd = new System.Random();

            // score, point в виде строки
            scorePoints = new List<Typple2<float, string>>();
        }

        // Анализируем ход в текущую точку, не важно чей, функция сама определит
        public void ReanalyseAfterHit(OneHit start)
        {

            // Чья точка в данной клетке (true - значит игрока, то есть моя)
            bool iBot = ( myColor == start.state );
            
            for (short subLine = 0; subLine < 6; subLine++)
            {
                short subLineRever = ReverseLine(subLine);

                // была ли на subLine обнаружена точка цвет которой отличается от цвета start (враг для start)
                bool wasEnemyPoint = false;
                // была ли обнаружена какая либо точка
                bool wasPoint = false;
                
                // Текущая субточка
                Point curPoint = start.coord.GetAround(subLine);
                for (short i=0; i < maxCheckedLine; i++)
                {
                    OneHit curPointOH = gm.grd.FindPoint(curPoint);
                    BoCoCell curPointBK = null;// curPointOH.bcc;
                    curPointBK.CheckInitialize();

                    if( curPointBK.WhoLeaderInSubLine(subLineRever) != start.state )
                        curPointBK.DestroySubLine( subLineRever );

                    if (!wasEnemyPoint)
                    {
                        if (curPointOH.whoShodil != 0 && curPointOH.whoShodil != start.whoShodil)
                        {
                            wasEnemyPoint = true;
                        }
                        else
                            // добавляем свои субточки для добавленной точки (сделанный ход)
                            curPointBK.PushPoint( start, subLineRever, i );
                    }

                    curPointBK.ReCountScore();

                    // Текущая субточка
                    curPoint = curPoint.GetAround( subLine );
                }
            }
        }

        public OneHit ScorePointsGetTop()
        {
            //Debug.Log( scorePoints[0].x1 );
            //Debug.Log( scorePoints[0].x2 );

            return gm.grd.FindPointStr(scorePoints[0].x2);
        }

        public void ScorePointsDelete(string point)
        {
            int i = ScorePointsFindByPoint(point);
            if (i != -1)
                scorePoints.RemoveAt(i);
        }

        public void ScorePointsEditPush(float score, string point)
        {
            // Почему здесь ищем по точке? потому что значения как правило не будут совпадать у точек, а если будут то их будет несколько
            int i = ScorePointsFindByPoint(point);
            if (i != -1)
                scorePoints.RemoveAt(i);

            i = ScorePointsFindByScore(score);
            if (i != -1 && i != scorePoints.Count)
                scorePoints.Insert(i, new Typple2<float, string>(score, point));
            else
                scorePoints.Add(new Typple2<float, string>(score, point));
        }

        private int ScorePointsFindByPoint(string point)
        {
            for (int i = 0; i < scorePoints.Count; i++)
                if (scorePoints[i].x2 == point)
                    return i;
            return -1;
        }

        private int ScorePointsFindByScore(float score)
        {
            // - если длина массива равна нулю - искать нечего;
            if (scorePoints.Count == 0)
            {
                return -1;
            }
            // - если искомый элемент больше первого элемента массива, значит, его в массиве нет;
            if (score > scorePoints[0].x1)
            {
                return 0;
            }
            // - если искомый элемент меньше последнего элемента массива, значит, его в массиве нет.
            if (score < scorePoints[scorePoints.Count - 1].x1)
            {
                return scorePoints.Count;
            }

            // Приступить к поиску.
            // Номер первого элемента в массиве.
            int first = 0;
            // Номер элемента массива, СЛЕДУЮЩЕГО за последним
            int last = scorePoints.Count;

            // Если просматриваемый участок не пуст, first < last
            while (first < last)
            {
                int mid = first + (last - first) / 2;

                if (score >= scorePoints[mid].x1)
                    last = mid;
                else
                    first = mid + 1;
            }

            // Теперь last может указывать на искомый элемент массива.
            return last;
        }



        public short ReverseLine(short line)
        {
            return (short)((line + 3) % 6);
        }
    }
};

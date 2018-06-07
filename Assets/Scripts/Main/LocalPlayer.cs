using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainInGame
{
    public class Player {
        public static GameManager gm;

        public int idUser;

        // урон за всю игру
        public int allDamage;
        // здоровье за всю игру
        public int allHills;
        // текущий остаток здоровья - текущее здоровье
        public int CurHills { get { int ret=allHills-allDamage; return ret > 0 ? ret : 0; } }

        // Сколько ячеек было открыто игроком
        public int countOpenCells;
        // Сколько ячеек прошёл игрок
        public int countWayCells;
        // Сколько играл игрок

        // Сколько бомб взорвал игрок
        public int countBombs;

        // игрок мёртв?
        public bool playerDead;

        // позиция игрока на поле
        public OneHit p;



        public Player(int idUser)
        {
            this.idUser = idUser;

            allDamage = 0;
            allHills = gm.startHill;
            gm.cam.PlayerUpdateHills( idUser, allHills );

            countOpenCells = 0;
            countWayCells = 0;

            playerDead = false;
        }



        
        public void SubDamage( int count )
        {
            allDamage += count;
            gm.cam.PlayerUpdateHills( idUser, CurHills);
        }

        public void DeviDamage( float coef )
        {
            allDamage += (int)(CurHills*coef+0.5);
            gm.cam.PlayerUpdateHills( idUser, CurHills);
        }

        public void AddHill( int count )
        {
            allHills += count;
            gm.cam.PlayerUpdateHills( idUser, CurHills);
        }

        public void MultHill( float coef )
        {
            allHills += (int)(CurHills*coef+0.5);
            gm.cam.PlayerUpdateHills( idUser, CurHills);
        }



        public void CountBombsIncrement()
        {
            countBombs++;
        }

        public void CountOpenCellsAdd( int count )
        {
            countOpenCells += count;
        }

        public void CountWayCellsAdd( int count )
        {
            countWayCells += count;
        }




        private void CheckDeath()
        {
            if ( CurHills <= 0 )
            {
                playerDead = true;
                p.SetStateDead();
            }
        }
    }



    public class Bot : Player
    {
        public Bot(int idUser) : base(idUser)
        {

        }

        public void ReanalyseAfterHit(OneHit p) { }
    }



    public class InterPlayer : Player
    {
        public InterPlayer(int idUser) : base(idUser)
        {

        }

    }
}
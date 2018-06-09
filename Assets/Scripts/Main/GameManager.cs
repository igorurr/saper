using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainInGame
{
    public class GameManager : MonoBehaviour
    {
        public CameraScr cam;
        public Grid grd;

        public int countPlayers;        // Количество всех игроков в катке. И боты и игроки локалки и мультиплееры, все. 2 или 3
        public int countInterPlayers;   // Количество интернет игроков в игре
        public int countBots;           // Без комментариев.
        public int CountRealPlayers {get{ return countPlayers - countInterPlayers - countBots; } }

        public Player[] players;         // массив всех игроков с порядком кто каким ходит

        public int playerCurHit;    // 0 1 2

        /*
            0 - вероятность появления
            1 - вероятность появления умноющего/делящего здоровья/урона
        */
        public List<float> likenessBomb;
        public List<float> likenessHill;
        /*
            0,1 - диаппазон урона/здоровья для прибавления/вычитания
            2,3 - диаппазон урона/здоровья для умножения/деления - проценты
        */
        public List<int> bombDiappason;
        public List<int> hillDiappason;

        // стартовое количество здоровья
        public int startHill;

        // умерли все
        public bool gameOver;



        // Use this for initialization
        void Start ()
        {
            gameOver = false;

            OneHit.gm = this;
            Player.gm = this;

            countInterPlayers = LocalDB._def_CountInterPlayers;
            countBots = LocalDB._def_CountBots;
            countPlayers = LocalDB._def_CountPlayers;

            players = new Player[countPlayers];

            playerCurHit = 0;

            // я, друг и бот на телефоне - обоим с локали надо назначить цвета
            if ( countPlayers==3 && CountRealPlayers==2 )
            {
                // 1,2:3  1,3:2  2,3:1
                players[LocalDB._def_ColorPlayer1] = new Player(LocalDB._def_ColorPlayer1);
                players[LocalDB._def_ColorPlayer2] = new Player(LocalDB._def_ColorPlayer2);
                int botId = 6 - LocalDB._def_ColorPlayer1 - LocalDB._def_ColorPlayer2;
                players[botId] = new Bot(botId);
            }
            // если есть боты или интернет игроки создаём для них классы, для локального игрока,
            // он единственный, тоже создаём класс
            else if ( countBots>0 && countInterPlayers>0 )
            {
                players[LocalDB._def_ColorPlayer1] = new Player(LocalDB._def_ColorPlayer1);

                /*
                for(int i=0; i<countBots; i++)
                {
                    // потом код допишу
                    players[i] = new Bot();
                }

                for(int i=0; i<countInterPlayers; i++)
                {
                    // потом код допишу
                    players[i] = new InterPlayer();
                }

                */
            }
            else // все на локале
            {
                for (int i = 0; i<CountRealPlayers; i++)
                {
                    players[i] = new Player(i);
                    cam.PlayerUpdateHills( i );
                }
            }

            // теперь инициализируем всё что в grid зависит от GameManager
            //grd.Initialize(); помни об особенностях юнити
            SpawnPlayers();

            GetHit();

            OnStart();
        }
	
	    // Update is called once per frame
	    void Update () {
            OnUpdate();
	    }



        



        // открывает поле при клике в данную точку
        public void SpawnPlayers()
        {
            if ( countPlayers == 1 )
            {
                players[0].p = grd.FindPoint(new Point(0, 0));
            }
            if ( countPlayers == 2 )
            {
                players[0].p = grd.FindPoint(new Point(1, -1));
                players[1].p = grd.FindPoint(new Point(-1, 1));
            }
            if ( countPlayers == 3 )
            {
                players[0].p = grd.FindPoint(new Point(0, -1));
                players[1].p = grd.FindPoint(new Point(-1, 1));
                players[2].p = grd.FindPoint(new Point(1, 0));
            }

            for(int i=0; i< countPlayers; i++)
            {
                players[i].p.SetState(i+3);
                players[i].p.whoShodil = i+1;
            }
        }






        /*
         *      Если ходет живой чел за данным телефоном - вызывается MakeHitLocalPlayer,
         *      если ходит бот или чувак по интернетику - GetHit
         *      после всех этих действий вызывается UpdateAllAfterHit для обновления префабов, вызовов реанализаторов ботов и прочего.
         *      
         *      После хода живого чела бесконечной рекурсией вызывается GetHit, где ходит бот или интернет чувак, в зависимости от того что в orderHits[i] обозначено как null,
         *      рекурсия разравается когда должен ходить локальный игрок
         *      UpdateAllAfterHit() вызывается после каждого хода.
         * */

        // Это работает для всех режимов игры. и мультиплеер и против ботов. Локальный игрок - это тот кто может ходить с устройства и не является ботом.
        public void MakeHitLocalPlayer( OneHit p )
        {
            if( !MakeHit( p, playerCurHit+1 ) )
                return;

            UpdateAllAfterHit(p);

            GetHit();
        }

        // Для ботов и мультиплеерных игроков
        public void GetHit()
        {
            /*if( orderHits[playerCurHit].x1==null && orderHits[playerCurHit].x2 == null )
                return;     // Долен ходить локальный игрок

            OneHit p;

            if ( orderHits[playerCurHit].x1 != null )
            {   // ходит бот
                BotController bc = orderHits[playerCurHit].x1;
                p = bc.ScorePointsGetTop();
                MakeHit(p, playerCurHit+1);

                UpdateAllAfterHit(p);
            }
            else
            {   // очевидно - ход игрока в инетике

            }
            

            GetHit();*/
        }

        public void UpdateAllAfterHit( OneHit p )
        {
            // меняем позицию игрока
            p.SetState( players[playerCurHit].p.state );
            players[playerCurHit].p.SetState(2);
            players[playerCurHit].p = p;


            for (int i=0; i<countPlayers; i++)
            {
                // Боты перерасчитывают свои веса
                if( players[i].GetType() == typeof(Bot) )
                    ((Bot)players[i]).ReanalyseAfterHit(p);
            }

            // Определяем кто ходит
            int pch = playerCurHit; //игрок который только что ходил
            do {
                // playerCurHit:playerCurHit   0:1 1:2 2:0    или 0:1 1:0
                playerCurHit = (playerCurHit + 1) % countPlayers;
            }
            // пропускаем всех мёртвых игроков, они больше не ходят
            while ( playerCurHit!=pch && players[playerCurHit].playerDead );

            if( playerCurHit==pch && players[playerCurHit].playerDead)
            {// все умерли - конец игры
                gameOver = true;
                cam.GameOverDeclare(pch);
                return;
            }

            // в камере меняем иконку игрока который ходит
            cam.UpdateImgPlayerCurHit();
        }

        public virtual bool MakeHit(OneHit point, int player)
        {
            return (gameOver) ? false : grd.MakeHit( point, player );
        }



        



        // принимает значения damageHill и damageDecrement из OneHit, обрабатывает их, вычитает игроку здоровье
        public void UpdateDamage()
        {
            // Если условие выполнится - бомба делящая здоровье, иначе - вычитающая
            if ( Random.value > likenessBomb[1] )
            {
                int damage = (int) (Random.value * (bombDiappason[1] - bombDiappason[0]) + bombDiappason[0]+0.5);
                players[playerCurHit].SubDamage( damage );
            }
            else
            {
                float damage = (Random.value * (bombDiappason[3] - bombDiappason[2]) + bombDiappason[2])/100f;
                players[playerCurHit].DeviDamage( damage );
            }
            players[playerCurHit].CountBombsIncrement();
        }

        public void UpdateHill()
        {
            // Если условие выполнится - бомба делящая здоровье, иначе - вычитающая
            if ( Random.value > likenessHill[1] )
            {
                int hill = (int) ( Random.value * (hillDiappason[1] - hillDiappason[0]) + hillDiappason[0]+0.5);
                players[playerCurHit].AddHill( hill );
            }
            else
            {
                float hill = (Random.value * (hillDiappason[3] - hillDiappason[2]) + hillDiappason[2])/100f;
                players[playerCurHit].MultHill( hill );
            }

        }





        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
};
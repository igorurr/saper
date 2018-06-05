using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainInGame
{
    public class Player {

        // урон за всю игру
        int allDamage;

        // здоровье за всю игру
        int curHills;

        // игрок мёртв?
        bool playerDead;

        // позиция игрока на поле
        OneHit position;



        public Player()
        {
            allDamage = 0;
            curHills = 100;
            playerDead = false;
        }
    }
}
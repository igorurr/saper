using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MainInGame
{
    public class OneHit : MonoBehaviour {
	    public static Grid grd;
        public static GameManager gm;
        public BoCoCell bcc;   // задаётся в инспекторе юнити

        public int state;           // соответствует grd.cellsPrefabs
        public int whoShodil;		// кто сходил в данную клетку? 0-никто 1-синий 2-красный 3-зелёный

	    public Point coord; // Трёхмерные координаты данной точки
        //public OneHit[] around; От этой штуки пришлось отказаться. Невозможно инициализировать значения этой переменной на старте. Плюс мы ищем точки не только вокруг данной, при создании точек требуется поиск среди всех

        // Мышка была нажата, потенциально можем сделать ход в ячейку,
        // если она не покинула ячейку, как только покинет будет false
        public bool mouseDidDown;


        // что находится в этой ячейке, бомба, если да то какая или аптечка.
        // 0-ничего 1-хилка 2-бомба
        public int damageHill;
        public int countBombs;

        // текст в ячейке куда пишется сколько там цифр
        public Text fieldCountBombs;
        // картинка что в ячейке
        public GameObject fieldBomb;
        // картинка что в ячейке
        public GameObject fieldHill;

        // 0-никто не пометил 1-синий 2-красный 3-зелёный
        public int playerMarkBomb;

        void Start()
        {
            // не работай здесь через старт вообще - нахуй его, реализовывай Create и вызывай там где надо
            // start работает в другом потоке походу
        }

        public void Initialize( Point coord ){
		    this.coord = coord;
            SetState(1);

            damageHill = 0;
            countBombs = 0;
            playerMarkBomb = 0;
        }




        public void SetState(int newState)
        {
            GetComponent<SpriteRenderer>().sprite = grd.cellsPrefabs[newState];
            state = newState;
        }

        public void SetStateDead()
        {
            GetComponent<SpriteRenderer>().sprite = grd.cellsPrefabs[state+3];
        }




        // эти 3 функции просто присваивают значения, не больше.
        public void CreateBomb()
        {
            damageHill = 2;
            countBombs = 0;
        }

        public void IncrementAroundBombs()
        {
            if ( damageHill == 2 )
                return;
            countBombs++;
        }

        public void CreateHill()
        {
            damageHill = 1;
        }




        private void OnMouseDown() {
		    if ( grd.gm.cam.CheckClickInUI() )
			    return;
			
		    mouseDidDown = true;

            StartCoroutine(_OnLongPressMouse());
        }

        private void OnMouseExit() {
		    if (!mouseDidDown)
			    return;

            grd.gm.cam.StartMovie();

		    mouseDidDown = false;
	    }

        private void OnMouseUp() {
            //Debug.Log (string.Format ( "x:{0} y:{1} z:{2} st:{3} pr:", coord.x, coord.y, coord.z, state, pref.ToString() ));
            if ( mouseDidDown )
                grd.gm.MakeHitLocalPlayer( this );

            grd.gm.cam.StopMovie();

		    mouseDidDown = false;
	    }

        private void OnLongPressMouse()
        {
            if (mouseDidDown == false)
                return;

            mouseDidDown = false;
            grd.MarkBomb( this );
        }
        private IEnumerator _OnLongPressMouse()
        {
            yield return new WaitForSeconds(0.5f);

            OnLongPressMouse();
        }



        public static int GetCurentTimestamp()
        {
            DateTime dt = DateTime.UtcNow;
            return (
                dt.Millisecond +
                dt.Second * 1000 +
                dt.Minute * 60 * 1000 +
                dt.Hour * 60 * 60 * 1000 +
                dt.Day * 24 * 60 * 60 * 1000
            );
        }
    }
};
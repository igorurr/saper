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

        public int state;           // соответствует grd.cellsPrefabs
        public int whoShodil;		// кто сходил в данную клетку? 0-никто 1-синий 2-красный 3-зелёный

	    public Point coord; // Трёхмерные координаты данной точки
        
        // Мышка была нажата, потенциально можем сделать ход в ячейку,
        // если она не покинула ячейку, как только покинет будет false
        public bool mouseDidDown;
        
        // что находится в этой ячейке, бомба, если да то какая или аптечка.
        // 0-ничего  1-хилка  2-бомба  3-взятая хилка  4-взорваная бомба
        public int damageHill;
        public int countBombs;

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





        // возвращает что тут находится, вызывается когда игрок ходит в данную ячейку
        // здесь же происходит перекраска ячейки
        public int MakeHit( int player )
        {
            Color cl = GetPlayerColor(player);

            // если в ячейке бомба - рисуем бомбу
            if (damageHill == 2)
            {
                // префаб бомбы на сцене
                GameObject goBomb = GetBombObject();

                goBomb.GetComponent<Image>().color = cl;
                goBomb.SetActive(true);
            }

            // если в ячейке таблетка - рисуем её
            else if (damageHill == 1)
            {
                // префаб таблетки на сцене
                GameObject goHill = GetHillObject();

                goHill.GetComponent<Image>().color = cl;
                goHill.SetActive(true);
            }

            int oldDamageHill = damageHill;
            damageHill = 0;

            return oldDamageHill;
        }

        // возвращает булево значение - стоит ли перестать делать ячейки вокруг активными
        public bool PointVisible()
        {
            SetState(2);

            // если здесь есть таблетка - сигнализируем
            if ( damageHill == 1 )
            {
                GameObject goHill = GetHillObject();
                
                goHill.SetActive(true);
            }

            // если вокруг есть бомбы рисуем в ячейке текст
            if ( countBombs > 0 )
            {
                GetCountBombsText().text = countBombs.ToString();
                return true;
            }

            return false;
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

        


        // помечает бомбу и говорит была ли она помечена
        public bool MarkBomb()
        {
            if (state != 1)
                return false;

            // Проверяем, что вокруг текущей точки есть открытые
            int numOpenCell = 0;
            for( ; numOpenCell < 6; numOpenCell++ )
            {
                // если ячейки не существует, если она уже открыта или если закрыта
                if ( grd.FindPoint(coord.GetAround(numOpenCell)).state >= 2 )
                    break;
            }
            if( numOpenCell == 6 )
                return false;
            

            playerMarkBomb = gm.playerCurHit + 1;

            GameObject go = GetBombObject();
            go.GetComponent<Image>().color = GetPlayerColor(playerMarkBomb);
            go.SetActive(true);

            return true;
        }

        public void UnMarkBomb()
        {
            playerMarkBomb = 0;
            GetBombObject().SetActive(false);
        }

        private void OnLongPressMouse()
        {
            if (mouseDidDown == false)
                return;

            mouseDidDown = false;
            

            if ( playerMarkBomb == 0 )
            {
                if( MarkBomb() )
                    Handheld.Vibrate();
            }
            else
            {
                UnMarkBomb();
                Handheld.Vibrate();
            }
        }

        private IEnumerator _OnLongPressMouse()
        {
            yield return new WaitForSeconds(0.5f);

            OnLongPressMouse();
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




        // Цвет текущего игрока
        public Color GetPlayerColor(int p)
        {
             return new Color( p==2?0.76f:0, p==3?0.6f:0, p==1?0.76f:0, 1);
        }

        // канвас в котором бомбы текст и хилки
        public Transform GetCanvas()
        {
            return transform.GetChild(0);
        }

        // текст в ячейке куда пишется сколько там цифр
        public Text GetCountBombsText()
        {
            return transform.GetChild(0).GetChild(0).GetComponent<Text>();
        }

        // картинка бомбы в данной ячейке
        public GameObject GetBombObject()
        {
            return transform.GetChild(0).GetChild(1).gameObject;
        }
        
        // картинка таблетки в данной ячейке
        public GameObject GetHillObject()
        {
            return transform.GetChild(0).GetChild(2).gameObject;
        }
    }
};
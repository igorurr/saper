using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MainInGame
{
    public class Grid : MonoBehaviour
    {
        public GameManager gm;

        private Dictionary<string, OneHit> points;

        // cellPrefabs:
        //      0:неактивно 1:доступно для хода несхожено 2:схожено
        //      3:синий 4:красный 5:зелёный
        //      6:синийDead 7:крассныйDead 8:зелёныйDead
        //      9:синийMenu 10:крассныйMenu 11:зелёныйMenu
        public List<Sprite> cellsPrefabs;
        public List<Sprite> cellTypesPrefabs;

        public GameObject[] lastHitedPoint;

        // Эталон ячеек
        public GameObject mainCell;
        
        public int blackCells;  // толщина пустых ячеек вокруг занятых

        private void Start()
        {
            int orientation = LocalDB._def_GexagonsOrientation;
            // если ориентация камеры вертикальная - поворачиваем текст в ячейке на 30 градусов
            if (orientation == 1)
                mainCell.GetComponent<OneHit>().fieldCountBombs.transform.parent.Rotate(Vector3.forward, 30);

            OneHit.grd = this;
            BoCoCell.grd = this;
            points = new Dictionary<string, OneHit>();

            // Создаём новую точку в 000 координатах и получаем её объект на сцене
            Point zeroPoint = new Point(0, 0);
            OneHit newCellElem = NewCell(zeroPoint);

            // заполняем полупрозрачное поле
            AddBlackField(zeroPoint);

            // открываем поле
            OpenField(newCellElem);

            // открываем ячейки

            OnStart();
        }
        // продолжение старта, только запускается параллельно ему сразу после запуска GameManager.start
        public void Initialize()
        {
            // появляются игроки
            SpawnPlayers();
        }

        private void Update()
        {
            OnUpdate();
        }




        // совершить ход в данную точку. Финальная - завершительная часть хода.
        public bool MakeHit(OneHit point, int player)
        {
            //player: 1 2 3

            // проверяем можно ли сюда ходить
            if ( point.state!=1 && point.state!=2 || point.playerMarkBomb!=0 )
                return false;
            if ( point.state==1 )
            {
                int i = 0;
                for( ; i<6; i++ )
                    if( FindPoint(point.coord.GetAround(i)).state > 1 )
                        break;
                if (i == 6)
                    return false;
            }
            
            // если в ячейке бомба - рисуем бомбу
            if (point.damageHill == 2)
            {
                point.fieldBomb.SetActive(true);
            }

            // если в ячейке аптечка - рисуем её
            else if (point.damageHill == 1)
            {
                point.fieldHill.SetActive(true);
            }

            // ходим

            // создать чёрное поле
            AddBlackField(point.coord);

            // смотрим какие ячейки теперь свободны
            OpenField(point);

            // Обновляем точку
            lastHitedPoint[player - 1].GetComponent<OneHit>().SetState(2);
            point.SetState(player + 2);

            point.whoShodil = player;
            lastHitedPoint[player - 1] = point.gameObject;

            return true;
        }

        // открывает поле при клике в данную точку
        public void OpenField(OneHit p)
        {
            // если ячейки не существует, если она уже открыта или если закрыта
            if (p == null || p.damageHill==2 || p.state==2 || p.state==0)
                return;

            p.SetState(2);

            // если вокруг есть бомбы рисуем в ячейке текст
            if( p.countBombs > 0 )
            {
                p.fieldCountBombs.text = p.countBombs.ToString();
                return;
            }

            for (int i = 0; i < 6; i++)
                OpenField( FindPoint( p.coord.GetAround(i) ) );
        }

        // открывает поле при клике в данную точку
        public void SpawnPlayers()
        {
            lastHitedPoint = new GameObject[gm.countPlayers];

            if ( gm.countPlayers == 1 )
            {
                lastHitedPoint[0] = FindPoint(new Point(0, 0)).gameObject;
            }
            if ( gm.countPlayers == 2 )
            {
                lastHitedPoint[0] = FindPoint(new Point(1, -1)).gameObject;
                lastHitedPoint[1] = FindPoint(new Point(-1, 1)).gameObject;
            }
            if ( gm.countPlayers == 3 )
            {
                lastHitedPoint[0] = FindPoint(new Point(0, -1)).gameObject;
                lastHitedPoint[1] = FindPoint(new Point(-1, 1)).gameObject;
                lastHitedPoint[2] = FindPoint(new Point(1, 0)).gameObject;
            }

            for(int i=0; i< gm.countPlayers; i++)
            {
                OneHit p = lastHitedPoint[i].GetComponent<OneHit>();
                p.SetState(i+3);
                p.whoShodil = i+1;
            }
        }





        private void AddBlackField(Point point)
        {
            List<OneHit> loh = new List<OneHit>(); // list one hits ))))))))))))))))))))

            for (int x = -blackCells; x <= blackCells; x++)
                for (int y = -blackCells; y <= blackCells; y++)
                {
                    // В 3мерных координатах 2мерной сетки сумма трёх координат должна обнуляться,
                    // иначе возникнет левая ячейка которой не должно быть
                    // x+y+z = 0 = x+y-(x+y)
                    int z = -(x + y);
                    if (Math.Abs(z) > blackCells)
                        continue;

                    Point curPoint = new Point(point.x + x, point.y + y);
                    if (FindPoint(curPoint) == null)
                    {
                        // создаём и сразу добавляем
                        loh.Add( NewCell(curPoint) );
                    }

                }
            CreateBombHill(loh);
        }

        public OneHit NewCell(Point point)
        {
            GameObject newEl = Instantiate(mainCell, point.GetCoord2D(), Quaternion.identity);
            newEl.name = point.ToString();
            newEl.transform.SetParent(transform);

            OneHit ohp = newEl.GetComponent<OneHit>();

            ohp.Initialize(point);
            points.Add(newEl.name, ohp);

            return newEl.GetComponent<OneHit>();
        }
        
        public void CreateBombHill( List<OneHit> loh )
        {
            for (int i=0; i<loh.Count; i++)
            {
                OneHit oh = loh[i];

                if (oh.coord.RingNum() < 3)
                {
                    //oh.damageHill = 0;
                    continue;
                }

                float rnd = UnityEngine.Random.value;
                if (rnd < gm.likenessBomb)
                {   // выпала бомба
                    oh.CreateBomb();

                    for (int j = 0; j < 6; j++)
                    {
                        OneHit coh = FindPoint(oh.coord.GetAround(j));
                        if (coh != null)
                            coh.IncrementAroundBombs();
                    }

                    continue;
                }

                rnd = UnityEngine.Random.value;
                if (rnd < gm.likenessHill)
                {   // выпала аптечка
                    oh.CreateHill();
                    continue;
                }

                //oh.damageHill = 0;
            }
        }




        public void MarkBomb(OneHit point)
        {
            if ( point.state != 1 )
                return;

            Handheld.Vibrate();

            if ( point.playerMarkBomb == 0 )
            {
                point.playerMarkBomb = gm.playerCurHit + 1;
                point.fieldBomb.SetActive(true);
            }
            else
            {
                point.playerMarkBomb = 0;
                point.fieldBomb.SetActive(false);
            }
        }





        public OneHit FindPoint(Point point)
        {
            return FindPointStr(point.ToString());
        }
        // Есть куски кода где надо искать по строке а не по точке
        public OneHit FindPointStr(string pstr)
        {
            if (!points.ContainsKey(pstr))
                return null;

            return points[pstr];
        }



        // чекнуть на предмет проёба






        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
};
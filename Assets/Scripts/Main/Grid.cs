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

        // Эталон ячеек
        public GameObject mainCell;
        
        public int blackCells;  // толщина пустых ячеек вокруг занятых

        private void Start()
        {
            int orientation = LocalDB._def_GexagonsOrientation;
            // если ориентация камеры вертикальная - поворачиваем текст в ячейке на 30 градусов
            if (orientation == 1)
            {
                mainCell.GetComponent<OneHit>().GetCountBombsObject().transform.Rotate(Vector3.forward, 30);
                mainCell.GetComponent<OneHit>().GetBombMarkHillObject().transform.Rotate(Vector3.forward, 30);
            }

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
                for (; i < 6; i++)
                {
                    OneHit oh = FindPoint(point.coord.GetAround(i));
                    if ( oh && oh.state>1 )
                        break;
                }
                if (i == 6)
                    return false;
            }

            int res = point.MakeHit( player );
            if ( res == 1 )
                gm.UpdateHill();
            else if ( res == 2 )
                gm.UpdateDamage();

            // создать чёрное поле
            AddBlackField(point.coord);

            // открываем ячейки, добавляем их количество к текущему юзеру
            gm.players[player-1].CountOpenCellsAdd( OpenField(point, res!=2) ); 

            return true;
        }

        // открывает поле при клике в данную точку
        public int OpenField(OneHit p, bool needCountAroundBombs=true)
        {
            /*for (int i = 0; i < 6; i++)
                if ( FindPoint( p.coord.GetAround(i) ).damageHill==2 )
                    FindPoint( p.coord.GetAround(i) ).GetBombObject().SetActive(true);*/

            // если ячейки не существует, если она уже открыта или если закрыта
            // если точка была помечена как бомба - убираем пометку
            if (p == null || p.damageHill==2 || p.state!=1 || p.playerMarkBomb==1 )
                return 0;
            
            for (int i = 0; i < 6; i++)
            {
                // Вокруг отустствуeт одна или более точек - текущую не показываем
                if ( !FindPoint(p.coord.GetAround(i)) )
                    return 0;
            }

            p.SetState(2);

            // Сколько бомб вокруг точки
            int countBombs = 0;
            for (int i = 0; i < 6; i++)
            {
                OneHit oh = FindPoint(p.coord.GetAround(i));
                if (oh.damageHill == 2 || oh.damageHill == 4)
                    countBombs++;
            }
            
            // если вокруг есть таблетки - показываем их
            p.CheckHill();

            // показываем сколько вокруг бомб
            if ( countBombs>0 && p.damageHill!=4 )
            {
                p.DrawAroundBombs(countBombs);
                return 1;
            }

            // считаем количество открытых точек
            int ret = 0;
            for (int i = 0; i < 6; i++)
            {
                ret += OpenField( FindPoint(p.coord.GetAround(i)) );
            }

            return ret;
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
                if (rnd < gm.likenessBomb[0])
                {   // выпала бомба
                    oh.CreateBomb();
                    continue;
                }

                rnd = UnityEngine.Random.value;
                if (rnd < gm.likenessHill[0])
                {   // выпала аптечка
                    oh.CreateHill();
                    continue;
                }

                //oh.damageHill = 0;
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
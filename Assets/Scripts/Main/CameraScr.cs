using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MainInGame
{
    public class CameraScr : MonoBehaviour
    {
        public GameManager gm;

        // Префаб слева сверху - кто сейчас ходит
        public Image imgPlayerCurHit;

        // предыдущие координаты камеры, коэффициент-скорость предвижения
        private Vector3 oldxy;
        public float moveCoeff;

        // есть ли движение, прозрачная стена блокирует доступ к ячейкам
        public bool movie;
        public GameObject transparentWall;
        public bool transparentWallOpened;

        // основное игровое меню
        public GameObject gameMenu;
        public bool gameMenuOpened;

        // ориентация камеры. 0-горизонтальная 1-вертикальная
        public int orientation;



        // Use this for initialization
        void Start()
        {
            Camera camera = gameObject.GetComponent<Camera>();
            camera.ResetAspect();

            orientation = LocalDB._def_GexagonsOrientation;
            // если ориентация камеры вертикальная - поворачиваем камеру на 30 градусов
            if (orientation == 1)
                camera.transform.Rotate(Vector3.forward, 30);

            OnStart();
        }

        // Update is called once per frame
        void Update()
        {
            if (movie)
                OnMovie();

            OnUpdate();
        }



        public void OnMovie()
        {
            Vector3 dxy = Input.mousePosition - oldxy;
            oldxy = Input.mousePosition;

            dxy *= moveCoeff;
            // если ориентация камеры вертикальная - домножаем на матрицу поворота на 30 градусов
            if (orientation == 1)
                dxy = Quaternion.AngleAxis(30, Vector3.forward) * dxy;

            Vector3 cp = gameObject.transform.position;
            transform.position = new Vector3(cp.x - dxy.x, cp.y - dxy.y, cp.z);
        }

        public void StartMovie()
        {
            oldxy = Input.mousePosition;
            movie = true;
        }

        public void StopMovie()
        {
            movie = false;
        }




        public bool CheckClickInUI()
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);

            return results.Count > 0;
        }



        public virtual void GoToMyPoint()
        {
           // Vector3 newCoord = gm.grd.cellsPrefabs[(gm.playerCurHit + 2) % 3].transform.position;
            //transform.position = new Vector3(newCoord.x, newCoord.y, transform.position.z);
        }



        public void UpdateImgPlayerCurHit()
        {
            // 0:9 1:10 2:11
            imgPlayerCurHit.sprite = gm.grd.cellsPrefabs[gm.playerCurHit + 9];
        }


        // при клике по элементам над TransparentWall слика по TransparentWall не происходит
        public virtual void ClickTransparentWall()
        {
            _CloseGameMenu();
            _CloseTransparentWall();
        }

        public void SwitchGameMenu()
        {
            _OpenTransparentWall();

            if (gameMenuOpened)
                _CloseGameMenu();
            else
                _OpenGameMenu();
        }

        public virtual void _OpenGameMenu()
        {
            gameMenuOpened = true;
            gameMenu.SetActive(true);
        }

        public virtual void _CloseGameMenu()
        {
            gameMenuOpened = false;
            gameMenu.SetActive(false);
        }

        public void _OpenTransparentWall()
        {
            transparentWallOpened = true;
            transparentWall.SetActive(true);
        }

        public void _CloseTransparentWall()
        {
            transparentWallOpened = false;
            transparentWall.SetActive(false);
        }





        public void NewGame()
        {
            SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ToMainMenu()
        {
            SceneManager.LoadScene("Main");
        }





        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
};
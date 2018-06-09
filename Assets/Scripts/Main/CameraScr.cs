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

        // основное меню
        public GameObject gameMenu;
        public bool gameMenuOpened;

        // 0:синего   1:крассного   2:зелёного
        public List<Text> allHillsPlayersMenuValues;
        public List<Text> allDamagePlayersMenuValues;
        public List<Text> curHillsPlayersMenuValues;
        public List<Text> curHillsPlayersSceneValues;

        public Text lastHillDamageTxt;
        public GameObject lastDamageImg;
        public GameObject lastHillImg;

        public GameObject menuObject;
        public Image winMenuWinPlayer;
        public GameObject winMenuLineObject;

        // ориентация камеры. 0-горизонтальная 1-вертикальная
        public int orientation;



        // Use this for initialization
        void Start()
        {
            Camera camera = gameObject.GetComponent<Camera>();
            camera.ResetAspect();

            for(int i=0; i<LocalDB._def_CountPlayers; i++)
                curHillsPlayersSceneValues[i].gameObject.SetActive(true);

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




        // Обновить последнюю хилку/урон полученный игроком
        public void GetTableParamsPlayers( int player, bool isBomb, string txt )
        {
            lastHillDamageTxt.text = txt;

            GameObject imgBombHill = isBomb ? lastDamageImg : lastHillImg;
            if( isBomb )
            {
                imgBombHill = lastDamageImg;
                lastDamageImg.SetActive(true);
                lastHillImg.SetActive(false);
            }
            else
            {
                imgBombHill = lastDamageImg;
                lastHillImg.SetActive(true);
                lastDamageImg.SetActive(false);
            }

            imgBombHill.GetComponent<Image>().color = OneHit.GetPlayerColor( player );
            lastHillDamageTxt.color = OneHit.GetPlayerColor( player );
        }

        // у игрока player обновить здоровье
        public void PlayerUpdateHills( int player )
        {
            allHillsPlayersMenuValues[player].text = gm.players[player].allHills.ToString();
            curHillsPlayersMenuValues[player].text = gm.players[player].CurHills.ToString();
            curHillsPlayersSceneValues[player].text = gm.players[player].CurHills.ToString();
        }
        // у игрока player обновить урон
        public void PlayerUpdateDamage(int player)
        {
            allDamagePlayersMenuValues[player].text = gm.players[player].allDamage.ToString();
            curHillsPlayersMenuValues[player].text = gm.players[player].CurHills.ToString();
            curHillsPlayersSceneValues[player].text = gm.players[player].CurHills.ToString();
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



        // из обычного игрового меню сделать победное
        public void GameOverDeclare( int winPlayer )
        {
            // 0:9 1:10 2:11
            winMenuWinPlayer.sprite = gm.grd.cellsPrefabs[winPlayer+9];

            menuObject.SetActive(true);
            winMenuLineObject.SetActive(true);
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
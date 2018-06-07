using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ModeGame
{
    // Название мода которое отображается в меню
    public string nameMode;
    // Количество всех игроков
    public int countPlayers;
    // количество ботов
    public int countBots;
    // количество инет игроков
    public int countInterPlayers;

    /*
        одиночка                    1 0 0
        вдвоём на телефоне          2 0 0
        вдвоём по сети              2 0 1   ВыборЦвета1
        вдвоём с ботом              2 1 0   ВЦ1
        втроём на телефоне          3 0 0
        втроём по сети              3 - -   ВЦ1
        я, друг и бот на телефоне   3 1 0   ВЦ2
        я с ботами                  3 2 0   ВЦ1
    */

    public ModeGame(string nm, int cp, int cb, int gi)
    {
        nameMode = nm;
        countPlayers = cp;
        countBots = cb;
        countInterPlayers = gi;
    }
}

public class MainMenu : MonoBehaviour {
    // Список всех модов игры с разными игроками, разным количеством реальных и интернет игроков.
    public List<ModeGame> mods;
    // текущий мод игры
    public int curMode;

    public List<Sprite> cellPrefabs;    // Префабы синий красный зелёный
    public Image outImage1;            // Image на который вешаем префаб myColorInGame 1 игрока.
    public Image outImage2;            // Image на который вешаем префаб myColorInGame 2 игрока.

    public Text fieldCountPlayers;
    public GameObject fieldOrientation;    // ориентация шестиугольников на сетке. Реализуется поворотом камеры на 30 градусов. 0-горизонтальная 1-вертикальная
    
    public int modeGame;
    public Text fieldModeGame;       // режим игры   0 - бот   1 - игрок против игрока на этом устройстве   2 - мультиплеер

    public GameObject mainMenuScn;      // Объект главное меню на сцене
    public GameObject recordsScn;      // Объект рекорды на сцене



    // Use this for initialization
    void Start()
    {

        GetStartMod();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void GetStartMod()
    {
        curMode = 0;
        for(; curMode<mods.Count; curMode++)
            if (
                mods[curMode].countPlayers == LocalDB._def_CountPlayers &&
                mods[curMode].countBots == LocalDB._def_CountBots &&
                mods[curMode].countInterPlayers == LocalDB._def_CountInterPlayers
            )
                break;

        if( curMode == mods.Count )
            curMode = 0;

        UpdateFieldCountPlayers();
        UpdateFieldGexagonsOrientation();
        UpdateFieldTextMod();
    }




    private void UpdateFieldCountPlayers()
    {
        fieldCountPlayers.text = LocalDB._def_CountPlayers.ToString();
    }

    public void UpdateFieldGexagonsOrientation()
    {
        if ( CheckClickInUI() )
            return;

        if ( LocalDB._def_GexagonsOrientation == 1 )
            fieldOrientation.transform.rotation = Quaternion.Euler(0, 0, 30);
        else
            fieldOrientation.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void UpdateFieldTextMod()
    {
        fieldModeGame.text = mods[curMode].nameMode;
    }




    // Следующий мод с обновлением всех полей
    private void ModeNext()
    {
        int i = curMode + 1;
        for (; mods[i].countPlayers != LocalDB._def_CountPlayers; i = (i + 1) % mods.Count) { }
        curMode = i;

        UpdateFieldCountPlayers();
        UpdateFieldGexagonsOrientation();
        UpdateFieldTextMod();
    }

    public void CountPlayersNext()
    {
        LocalDB._def_CountPlayers = (LocalDB._def_CountPlayers) % 3 + 1;
        ModeNext();
    }






    public void SwapOrientation()
    {
        LocalDB._def_GexagonsOrientation = (LocalDB._def_GexagonsOrientation == 1) ? 0 : 1;
        UpdateFieldGexagonsOrientation();
    }

    private void CheckColorsValues()
    {
        // Для режима 2 игрока: если либо 1 либо у 2 игрока цвет зелёный - поменять на противоположный цвет  либо 1 игрока соответственно
        if( LocalDB._def_CountPlayers == 2 )
            if( LocalDB._def_ColorPlayer1 == 3 )
                if( LocalDB._def_ColorPlayer2 == 2 )
                    LocalDB._def_ColorPlayer1 = 1;
                else
                    LocalDB._def_ColorPlayer1 = 2;
            else if( LocalDB._def_ColorPlayer2 == 3 )
                if( LocalDB._def_ColorPlayer1 == 2 )
                    LocalDB._def_ColorPlayer2 = 1;
                else
                    LocalDB._def_ColorPlayer2 = 2;
        else if ( LocalDB._def_CountPlayers == 1 )
             LocalDB._def_ColorPlayer2 = 1;

        SetColor1(LocalDB._def_ColorPlayer1);
        SetColor2(LocalDB._def_ColorPlayer2);
    }
    
    // Если у одного из игроков при переключении на режим 2 игрока остался зелёный префаб - заменить на противоположный другому игроку
    private void CheckColorsVisible()
    {
        if ( LocalDB._def_CountPlayers==1 || LocalDB._def_CountBots==0 && LocalDB._def_CountInterPlayers==0 )
        {// режим игры на телефоне без ботов, 1 игрок
            outImage1.transform.parent.gameObject.SetActive(false);
            outImage2.transform.parent.gameObject.SetActive(false);
        }
        else if ( LocalDB._def_CountInterPlayers>0 || LocalDB._def_CountPlayers==2 && LocalDB._def_CountBots==0 )
        {// мультиплеер, 2 игрока, 1 из них - бот
            outImage1.transform.parent.gameObject.SetActive(true);
            outImage2.transform.parent.gameObject.SetActive(false);
        }
        else
        {// Когда имеем 2 игроков по локалке и 1 бот
            outImage1.transform.parent.gameObject.SetActive(true);
            outImage2.transform.parent.gameObject.SetActive(true);
        }
    }

    public void SwapColor1()
    {
        // Проверяем, если новый цвет 1 игрока равен цвету второго игрока, меняем цвет второго игрока на прошлый цвет первого.
        //   1:2   2:3   3:1   or   1:2   2:1
        int newColorPlayer1 = LocalDB._def_ColorPlayer1 % LocalDB._def_CountPlayers + 1;
        if (newColorPlayer1 == LocalDB._def_ColorPlayer2)
        {
            SetColor2(LocalDB._def_ColorPlayer1);
        }

        SetColor1(newColorPlayer1);
    }
    public void SwapColor2()
    {
        int newColorPlayer2 = LocalDB._def_ColorPlayer2 % LocalDB._def_CountPlayers + 1;
        if (newColorPlayer2 == LocalDB._def_ColorPlayer1)
        {
            SetColor1(LocalDB._def_ColorPlayer2);
        }

        SetColor2(newColorPlayer2);
    }

    // Непосредственно изменить цвет игрока. Взять и изменить вместе с префабом на сцене.
    private void SetColor1(int color)
    {
        LocalDB._def_ColorPlayer1 = color;
        // -1 потому что синему должен соответствовать 1 элемент а красному 2, в реальности элементы массива отсчитываются с 0
        outImage1.sprite = cellPrefabs[color - 1];
    }
    private void SetColor2(int color)
    {
        LocalDB._def_ColorPlayer2 = color;
        outImage2.sprite = cellPrefabs[color - 1];
    }





    public bool CheckClickInUI()
    {
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        
        return results.Count > 4;
    }













    /*private void SetCountPlayers()
    {
        fieldCountPlayers.text = LocalDB._def_CountPlayers.ToString();
    }

    // Зависимость от количества реальных игроков, типа игры (mode game), 
    private void SetModeGame(int newMode)
    {
        /*
         *      Если 2 игрока, то всё норм - берём modeGame и как есть прикручиваем к LocalDB._def_ModeGame
         *      Если 3 - то там надо пошаманить.
         * *
        if (LocalDB._def_CountPlayers == 3)
        {
            if (newMode == 0)
            {
                LocalDB._def_ModeGame = 0;
                LocalDB._def_CountRealPlayers = 1;
            }
            else if (newMode == 1)
            {
                LocalDB._def_ModeGame = 1;
                LocalDB._def_CountRealPlayers = 2;
            }
            else if (newMode == 2)
            {
                LocalDB._def_ModeGame = 1;
                LocalDB._def_CountRealPlayers = 3;
            }
            else
            { // modeGame == 3
                LocalDB._def_ModeGame = 2;
            }
        } else   // LocalDB._def_CountPlayers == 2
        {
            LocalDB._def_ModeGame = newMode % 3;

            if( newMode == 0 )
                LocalDB._def_CountRealPlayers = 1;
            else // при newMod==2 это значение может позже измениться
                LocalDB._def_CountRealPlayers = 2;
        }

        fieldModeGame.text = sceneTxt[LocalDB._def_CountPlayers - 2][newMode];
    }

    // Если у одного из игроков при переключении на режим 2 игрока остался зелёный префаб - заменить на противоположный другому игроку
    private void CheckColorsVisible()
    {
        // Подробнее про всё это читай в руководстве, тут сходу врятли что то поймёшь
        if( LocalDB._def_ModeGame != 1 ){
            // мультиплеер, либо один игрок и 1 или 2 бота
            outImage1.transform.parent.gameObject.SetActive(true);
            outImage2.transform.parent.gameObject.SetActive(false);
        }
        else if( LocalDB._def_CountPlayers == LocalDB._def_CountRealPlayers ){ // && LocalDB._def_ModeGame == 1
            // покрывает режим игры на данном устройстве, когда нету ботов вообще (2 или 3 игрока и все на локалке)
            outImage1.transform.parent.gameObject.SetActive(false);
            outImage2.transform.parent.gameObject.SetActive(false);
        }
        else  // Когда имеем 2 игроков по локалке и 1 бот
        {
            outImage1.transform.parent.gameObject.SetActive(true);
            outImage2.transform.parent.gameObject.SetActive(true);
        }
        return;
    }



    public void SwapCountPlayers()
    {
        LocalDB._def_CountPlayers = (LocalDB._def_CountPlayers == 3) ? 2 : 3;

        SetModeGame( modeGame );
        CheckColorsVisible();
        CheckColorsValues();
        SetCountPlayers();
    }



    public void SwapModeGame()
    {
        modeGame = (modeGame + 1) % LocalDB._def_CountPlayers;

        SetModeGame( modeGame );
        CheckColorsVisible();
    }



    // Просто берём и меняем
    public void SwapOrientation()
    {
        LocalDB._def_GexagonsOrientation = (LocalDB._def_GexagonsOrientation == 1) ? 0 : 1;
        SetOrientation();
    }
    private void SetOrientation()
    {
        fieldOrientation.text = (LocalDB._def_GexagonsOrientation == 0) ? sceneTxt[2][0] : sceneTxt[2][1];
    }

    public void SwapGameInWin()
    {
        LocalDB._def_GameInWin = (LocalDB._def_GameInWin == 1) ? 2 : 1;
        SetGameInWin();
    }
    private void SetGameInWin()
    {
        fieldGameWin.text = (LocalDB._def_GameInWin == 1) ? sceneTxt[3][0] : sceneTxt[3][1];
    }





    public void NewNormalGame()
    {
        SceneManager.LoadScene("GameNormal");
    }
    public void NewGameInTime()
    {
        SceneManager.LoadScene("GameInTime");
    }
    public void NewGameInHits()
    {
        SceneManager.LoadScene("GameInHits");
    }*/
    public void NewNormalGame()
    {
        SceneManager.LoadScene("GameNormal");
    }




    public void GoMenu2Records()
    {
        mainMenuScn.SetActive(false);
        recordsScn.SetActive(true);
    }
    public void GoRecords2Menu()
    {
        mainMenuScn.SetActive(true);
        recordsScn.SetActive(false);
    }

    public void GoToRurricGamesRu()
    {
        Application.OpenURL("https://rurricgames.ru/");
    }
}

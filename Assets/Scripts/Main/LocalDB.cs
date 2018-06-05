using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalDB
{
    // здесь не должно происходить автоматического изменения __def_ColorPlayer1 и __def_ColorPlayer2, за это отвечает GameManager
    private static int __def_ColorPlayer1;
    private static int __def_ColorPlayer2;

    private static int __def_CountPlayers;
    private static int __def_CountBots;

    private static int __def_ModeGame;

    private static int __def_GameInWin;

    private static int __def_GexagonsOrientation;


    
    public static int _def_ColorPlayer1         { get { return __def_ColorPlayer1; }        set { PlayerPrefs.SetInt("_def_ColorPlayer1", value); PlayerPrefs.Save();        __def_ColorPlayer1 = value; } }
    public static int _def_ColorPlayer2         { get { return __def_ColorPlayer2; }        set { PlayerPrefs.SetInt("_def_ColorPlayer2", value); PlayerPrefs.Save();        __def_ColorPlayer2 = value; } }

    public static int _def_CountPlayers         { get { return __def_CountPlayers; }        set { PlayerPrefs.SetInt("_def_CountPlayers", value); PlayerPrefs.Save();        __def_CountPlayers = value; } }
    public static int _def_CountBots            { get { return __def_CountBots; }           set { PlayerPrefs.SetInt("_def_CountBots", value); PlayerPrefs.Save();           __def_CountBots = value; } }

    public static int _def_ModeGame             { get { return __def_ModeGame; }            set { PlayerPrefs.SetInt("_def_ModeGame", value); PlayerPrefs.Save();            __def_ModeGame = value; } }

    public static int _def_GameInWin            { get { return __def_GameInWin; }           set { PlayerPrefs.SetInt("_def_GameInWin", value); PlayerPrefs.Save();           __def_GameInWin = value; } }

    public static int _def_GexagonsOrientation  { get { return __def_GexagonsOrientation; } set { PlayerPrefs.SetInt("_def_GexagonsOrientation", value); PlayerPrefs.Save(); __def_GexagonsOrientation = value; } }



    static LocalDB()
    {
        // значения по-умолчанию задаются здесь!
        __def_ColorPlayer1 = PlayerPrefs.GetInt("_def_ColorPlayer1", 1);
        __def_ColorPlayer2 = PlayerPrefs.GetInt("_def_ColorPlayer2", 2);

        __def_CountPlayers = PlayerPrefs.GetInt("_def_CountPlayers", 3);
        __def_CountBots = PlayerPrefs.GetInt("_def_CountBots", 1);

        __def_ModeGame = PlayerPrefs.GetInt("_def_ModeGame", 0);

        __def_GameInWin = PlayerPrefs.GetInt("_def_GameInWin", 1);

        __def_GexagonsOrientation = PlayerPrefs.GetInt("_def_GexagonsOrientation", 1);
    }
}
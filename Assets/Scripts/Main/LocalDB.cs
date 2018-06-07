using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalDB
{
    // ����� �� ������ ����������� ��������������� ��������� __def_ColorPlayer1 � __def_ColorPlayer2, �� ��� �������� GameManager
    private static int __def_ColorPlayer1;
    private static int __def_ColorPlayer2;

    private static int __def_CountPlayers;
    private static int __def_CountBots;
    private static int __def_CountInterPlayers;

    private static int __def_GameInWin;

    private static int __def_GexagonsOrientation;


    // 1 2 3
    public static int _def_ColorPlayer1         { get { return __def_ColorPlayer1; }        set { PlayerPrefs.SetInt("_def_ColorPlayer1", value); PlayerPrefs.Save();        __def_ColorPlayer1 = value; } }
    public static int _def_ColorPlayer2         { get { return __def_ColorPlayer2; }        set { PlayerPrefs.SetInt("_def_ColorPlayer2", value); PlayerPrefs.Save();        __def_ColorPlayer2 = value; } }

    public static int _def_CountPlayers         { get { return __def_CountPlayers; }        set { PlayerPrefs.SetInt("_def_CountPlayers", value); PlayerPrefs.Save();        __def_CountPlayers = value; } }
    public static int _def_CountBots            { get { return __def_CountBots; }           set { PlayerPrefs.SetInt("_def_CountBots", value); PlayerPrefs.Save();           __def_CountBots = value; } }
    public static int _def_CountInterPlayers    { get { return __def_CountInterPlayers; }   set { PlayerPrefs.SetInt("_def_CountInterPlayers", value); PlayerPrefs.Save();   __def_CountInterPlayers = value; } }

    public static int _def_GameInWin            { get { return __def_GameInWin; }           set { PlayerPrefs.SetInt("_def_GameInWin", value); PlayerPrefs.Save();           __def_GameInWin = value; } }

    public static int _def_GexagonsOrientation  { get { return __def_GexagonsOrientation; } set { PlayerPrefs.SetInt("_def_GexagonsOrientation", value); PlayerPrefs.Save(); __def_GexagonsOrientation = value; } }



    static LocalDB()
    {
        // �������� ��-��������� �������� �����!
        __def_ColorPlayer1 = PlayerPrefs.GetInt("_def_ColorPlayer1", 1);
        __def_ColorPlayer2 = PlayerPrefs.GetInt("_def_ColorPlayer2", 2);

        __def_CountPlayers = PlayerPrefs.GetInt("_def_CountPlayers", 1);
        __def_CountBots = PlayerPrefs.GetInt("_def_CountBots", 0);
        __def_CountInterPlayers = PlayerPrefs.GetInt("_def_CountInterPlayers", 0);

        __def_GameInWin = PlayerPrefs.GetInt("_def_GameInWin", 1);

        __def_GexagonsOrientation = PlayerPrefs.GetInt("_def_GexagonsOrientation", 1);
    }
}
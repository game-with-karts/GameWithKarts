using UnityEngine;

public class Constants {
    public const string PlayerNameKey = "PlayerName";
    public const string StartFinishTag = "Start Finish";

    #if UNITY_EDITOR
    public const string USERNAME_KEY = "debug_Username";
    public const string LOGIN_INFO_KEY = "debug_LoginInfo";
    public const string LOGIN_REQUEST_KEY = "debug_LoginRequest";
    public static string RECORDS_KEY => $"debug_Records";
    #else
    public const string USERNAME_KEY = "Username";
    public const string LOGIN_INFO_KEY = "LoginInfo";
    public const string LOGIN_REQUEST_KEY = "LoginRequest";
    public static string RECORDS_KEY => $"Records";
    #endif
}

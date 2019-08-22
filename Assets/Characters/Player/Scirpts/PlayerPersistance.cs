using UnityEngine;

class PlayerPersistance
{
    public static void SaveInfo(PlayerMechanics player)
    {
        PlayerPrefs.SetFloat("x", player.transform.position.x);
        PlayerPrefs.SetFloat("y", player.transform.position.y);
        PlayerPrefs.SetFloat("z", player.transform.position.z);
    }

    public static PlayerInfo LoadInfo()
    {
        float x = PlayerPrefs.GetFloat("x");
        float y = PlayerPrefs.GetFloat("y");
        float z = PlayerPrefs.GetFloat("z");

        PlayerInfo playerInfo = new PlayerInfo();

        playerInfo.position = new Vector3(x, y , z);

        return playerInfo;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WalletConnected : MonoBehaviour
{
    public void ChangeToSceneShopAndPlay()
    {
        SceneManager.LoadScene("ShopAndPlay");
    }
}

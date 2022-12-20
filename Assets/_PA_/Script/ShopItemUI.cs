using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public string item;
    public string productID;
    public int rewarded = 100;
    public float price = 100;
    public InputField email; 
    public Text textInfo;

    public AudioClip soundRewarded;

    public Text priceTxt, rewardedTxt, rewardTimeCountDownTxt;

    public void Buy()
    {

        if(email.text.IndexOf('@') <= 0){
            textInfo.text = "Agregar un correo electronico valido";
            textInfo.color = Color.red;
        }
        else{
			var GetNZ = gameObject.AddComponent<ShopNZ>();
			GetNZ.Shop(productID, price.ToString(), rewarded.ToString(), email.text);
            }
    }
}

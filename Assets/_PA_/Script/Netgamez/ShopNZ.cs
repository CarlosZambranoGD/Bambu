using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using SimpleJSON;

public class ShopNZ : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject popup;
    string serviceID;
    string precio;
    string gemas;
    string correo;
    // Start is called before the first frame update
    
    public void Shop(string id, string price, string gems, string email){
        serviceID = id;
        precio = price;
        gemas = gems;
        correo = email;

        StartCoroutine(ShoppingAPI());

    }

    IEnumerator ShoppingAPI() {
    WWWForm form = new WWWForm();
    form.AddField("metodo", "VenderServicio");
    form.AddField("ID", GlobalValue.token);
    form.AddField("ServicioID", "122333444455555");
    form.AddField("Correo", correo);
    form.AddField("Valor", precio);
    form.AddField("Gemas", gemas);

    using (UnityWebRequest www = UnityWebRequest.Post("https://facilservicios.com/servicioDesarrollo.php", form)) {
    
        yield return www.SendWebRequest();

    //    www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.result.ToString());
                Debug.Log(www.error);
                gameObject.GetComponent<ShopItemUI>().textInfo.color = Color.red;
                gameObject.GetComponent<ShopItemUI>().textInfo.text = "Ha ocurrido un error en la conexión con el servicio.";
            }

        else {
            JSONNode data = JSON.Parse(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);

                if(data["Error"]== 1){

                    gameObject.GetComponent<ShopItemUI>().textInfo.color = Color.yellow;
                    gameObject.GetComponent<ShopItemUI>().textInfo.text = data["ErrorDescripcion"];
                   //popup = GameObject.Find("MainMenu");
                  //GameObject child = popup.transform.GetChild(7).gameObject;
                  //child.SetActive(true);
                   // popup.SetActive(false);
                }

                else if (data["Respuesta"] == 1) {
                    Debug.Log("todo OK");
                    gameObject.GetComponent<ShopItemUI>().textInfo.color = Color.green;
                    gameObject.GetComponent<ShopItemUI>().textInfo.text = "Se ha realizado la compra satisfactoriamente.";
                    GlobalValue.NZCoins += int.Parse(gemas);
                }
            }
        }

    }
}

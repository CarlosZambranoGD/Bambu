using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class ShoppingAgoratSkin : MonoBehaviour
{
    public GameObject popup;
    public static string productID;
    int productPrice;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Shop(string id, int price){
        productPrice = price;
        productID = id;
        StartCoroutine(ShoppingAPI());

    }

    public IEnumerator ShoppingAPI() {
    WWWForm form = new WWWForm();
    form.AddField("metodo", "VenderServicio");
    form.AddField("ID", GlobalValue.token);
    form.AddField("ServicioID", productID);
    /* form.AddField("PrimerNombre", "nombre");
    form.AddField("SegundoNombre", "segundo nombre");
    form.AddField("PrimerApellido", "apellido");
    form.AddField("SegundoApellido", "segundo");
    form.AddField("Identificacion", "123456789");
    form.AddField("Celular", "31154579521");
    form.AddField("Departamento", "antioquia");
    form.AddField("Ciudad", "Medellin");
    form.AddField("Direccion", "cr 0 # 1 - 0");
    form.AddField("Pasarela", "NetGamezMonedas");  */
    

    using (UnityWebRequest www = UnityWebRequest.Post("https://facilservicios.com/servicioDesarrollo.php", form)) {
    
        yield return www.SendWebRequest();

    //    www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.result.ToString());
                Debug.Log(www.error);
            }

        else {
            JSONNode data = JSON.Parse(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);

                if(data["Error"] == 1){

                    Debug.Log("true shop");
                    popup = GameObject.Find("MainMenu");
                    GameObject child = popup.transform.GetChild(7).gameObject;
                    var image = child.transform.GetChild(0).gameObject;
                    var popuptext =image.transform.GetChild(0).gameObject;
                    popuptext.GetComponent<Text>().text = data["ErrorDescripcion"];
                    child.SetActive(true);
                    // popup.SetActive(false);
                }

                else {
                  GlobalValue.NZCoins -= productPrice;
                  gameObject.GetComponent<MainMenu_ChracterChoose>().DoUnlock();
                }
/* 
            for (int i = 0; i < data["ListaObjetos"].Count; i++){
            JSONNode datos = data["ListaObjetos"][i];      
                
                if (datos["tipocampo"] == "FDC.Seguridad.Usuario.Saldo") {
                      if (datos["valor3"] == "NetGamezMonedas") {
                        saldo = datos["valor"] - datos["valor2"];
                     }
                }
           } */

            }
        }

    }
}

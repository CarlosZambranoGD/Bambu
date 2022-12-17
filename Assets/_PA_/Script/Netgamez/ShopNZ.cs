using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using SimpleJSON;

public class ShopNZ : MonoBehaviour
{
    // Start is called before the first frame update
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
    form.AddField("ServicioID", "122333444455555");
    

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

                if(data["error"]= "1"){
                   popup = GameObject.Find("MainMenu");
                  GameObject child = popup.transform.GetChild(7).gameObject;
                  child.SetActive(true);
                   // popup.SetActive(false);
                }

                else {
                    Debug.Log("todo OK");
                }
            }
        }

    }
}

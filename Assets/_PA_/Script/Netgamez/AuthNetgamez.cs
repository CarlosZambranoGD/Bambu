using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class AuthNetgamez : MonoBehaviour
{

    public Text log;
    //public Text tokenLog;
    string arguments = "";
    string packageName;

    public GameObject NetGamezButton;
    public GameObject Exit;
    // Start is called before the first frame update

    void Awake(){
        Exit.SetActive(false);
    }

    void Start()
    {

        try {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            AndroidJavaObject extras = intent.Call<AndroidJavaObject> ("getExtras"); 
            //text = extras.Call<string> ("getString", "NZtoken");
            arguments = intent.Call<string> ("getDataString");

            
            GlobalValue.token = arguments; 
            log.text = "Obteniendo acesso...";
            //tokenLog.text = "Token: "+GlobalValue.token+ " ---";
            if ((GlobalValue.token == "") || (GlobalValue.token == null)) {
                StartCoroutine(GetNetGamezApp());
                }

            else {
                var auth = gameObject.AddComponent<APIRequestNetgamez>();
            }
        }

        catch{
            Exit.SetActive(true);
            log.text = "Te pedimos disculpas, para poder ingresar al juego debes usar una cuenta daviplata";



            //StartCoroutine(GetNetGamezApp());
        }
    }

    public IEnumerator GetNetGamezApp() 
    {

        WWWForm form = new WWWForm();
        form.AddField("metodo", "NombrePaqueteNetGamez");
        using (UnityWebRequest www = UnityWebRequest.Post("https://facilservicios.com/servicioDesarrollo.php", form)) {

        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            log.text = "No logró obtener respuesta, revisa tu conexión.";
        } 

        else {
                log.text = "primero debes instalar la aplicación de Netgamez para jugar.";
                JSONNode data = JSON.Parse(www.downloadHandler.text);
                
                string dataText = data["Mensaje"];
                packageName = dataText + "&Codigo=AgoratAdventure&Nombre=Agorat Adventture";

                Debug.Log(www.downloadHandler.text);
                Debug.Log(dataText);
                Open();
            }
        }

    }
    
     public void Open()
    {
         StartCoroutine(GetURL());
    }

    IEnumerator GetURL(){
        yield return new WaitForSeconds(1);        
        Application.OpenURL(packageName);
        Application.Quit();
        Exit.SetActive(true);
    }
}


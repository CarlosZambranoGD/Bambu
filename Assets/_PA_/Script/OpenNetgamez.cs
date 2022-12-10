using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;


public class OpenNetgamez : MonoBehaviour
{

    public string token;
    string getToken = "123456";
    public string Data;

    void Start() {

        StartCoroutine(GetAll());
    }

    public IEnumerator GetAll() 
    {
        UnityWebRequest www = UnityWebRequest.Get("https://facilservicios.com/servicioDesarrollo.php?metodo=NombrePaqueteNetGamez"); 
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError){
            Debug.Log(www.error);
        }
        else{
        JSONNode data = JSON.Parse(www.downloadHandler.text);
        string dataText = data["Mensaje"];

        Data = dataText;
        print(Data);

        }

    }
    // Start is called before the first frame update
    public void Open()
    {

        if(token == "")
         Application.OpenURL("market://launch?id=" + Data + "&url=" + Data + ".MainActivity&Codigo=NezPanda");

         else if (token == getToken){
            LoadScene();
         }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene() {
        SceneManager.LoadScene("MainMenu");
    }
}

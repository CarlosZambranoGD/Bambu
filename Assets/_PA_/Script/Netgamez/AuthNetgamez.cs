using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class AuthNetgamez : MonoBehaviour
{
    public bool printLog;
    public Text textInfo;
    public InputField inputText;
    public bool Focus;

    void Awake(){
///////////////////////////////////////////////////////////////Experimental Process
/*      Name oFormProperties = new Name() { Nombre = "Juan",Apellido="De León", Edad = 31, Estatura = 1.80M };
        PropertyInfo[] FormProperties = typeof(Name).GetProperties();
        Debug.Log(FormProperties[0].GetValue(oFormProperties).ToString()); */
//////////////////////////////////////////////////////////////////////////////////
    }//Awake

        private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if(Focus){
                // La aplicación ha obtenido el enfoque (está en primer plano)
                Debug.Log("La aplicación está en primer plano");
                intent();
            }
        }
        else
        {
            // La aplicación ha perdido el enfoque (está en segundo plano)
            Debug.Log("La aplicación está en segundo plano");
        }
    }

    void Start(){
        inputText = gameObject.GetComponent<IntroManager>().oCode[1].transform.GetChild(3).GetComponent<InputField>();
        textInfo = gameObject.GetComponent<IntroManager>().oCode[0].transform.GetChild(0).GetComponent<Text>();
        LogState(0,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
        intent();
    }//Start

    void intent(){
        Focus = false;
        gameObject.GetComponent<IntroManager>().screenInfo(0);
        textInfo.text = "Bienvenido a Agorat Adventure";
        LogState(1,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
        try {
            LogState(2,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras"); 
            string arguments = intent.Call<string> ("getDataString");
            LogState(3,arguments,false, System.Reflection.MethodBase.GetCurrentMethod().Name);
            PlayerPrefs.SetString("token", arguments);
            LogState(4,PlayerPrefs.GetString("token"),false, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if ((PlayerPrefs.GetString("token", "") == "") || (GlobalValue.token == null)) {
                LogState(5,"",true, System.Reflection.MethodBase.GetCurrentMethod().Name);
                GetTokenToCache();
            } else {
                LogState(6,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
                var auth = gameObject.AddComponent<APIRequestNetgamez>();
            }
        } catch {
            LogState(7,"",true, System.Reflection.MethodBase.GetCurrentMethod().Name);
            GetTokenToCache();
        }
    }//Intent

    void GetTokenToCache(){
        textInfo.text = "Obteniendo Acceso";
        LogState(8,PlayerPrefs.GetString("token", ""),false, System.Reflection.MethodBase.GetCurrentMethod().Name);
        if ((PlayerPrefs.GetString("token", "") == "") || (GlobalValue.token == null)) {
            LogState(5,"",true, System.Reflection.MethodBase.GetCurrentMethod().Name);
            AutoPaste();
        } else {
            LogState(6,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
            var auth = gameObject.AddComponent<APIRequestNetgamez>();
        }
    }//Cache

    void AutoPaste(){
        LogState(9,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
        TextEditor textEditor = new TextEditor();
        textEditor.multiline = true;
        textEditor.Paste();
        string code = textEditor.text;
        if((code == "") ||(code == null)) {
            LogState(10,"",true, System.Reflection.MethodBase.GetCurrentMethod().Name);
        } else {
            LogState(11,code,false, System.Reflection.MethodBase.GetCurrentMethod().Name);
            StartCoroutine(Upload(code));
        }
    }

    void GetTokenToCode(){
        LogState(12,"",false, System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    public void PressedCode(){
        StartCoroutine(Upload(inputText.text));
    }

    IEnumerator Upload(string codeNetgamez) {
        gameObject.GetComponent<IntroManager>().screenInfo(0);
        textInfo.text = "Obteniendo Acceso";
        WWWForm form = new WWWForm();
        form.AddField("Metodo", "ConfirmarIdentidad");
        form.AddField("TokenNZ", "123123123");
        form.AddField("Codigo", codeNetgamez);
        Debug.Log(codeNetgamez);
        using (UnityWebRequest www = UnityWebRequest.Post("https://netgamez.co/servicio.php", form)) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log("Error de servidor o conexión a internet en adquirir token desde código");
                Debug.Log(www.error);
                gameObject.GetComponent<IntroManager>().screenInfo(1);
                Focus = true;
            } else {
                JSONNode data = JSON.Parse(www.downloadHandler.text);
                if(data["Error"] == 1){
                    textInfo.text = "El código obtenido no es valido";
                    yield return new WaitForSeconds(2);
                    gameObject.GetComponent<IntroManager>().screenInfo(1);
                    Focus = true;


                } else {
                    Debug.Log("Ha adquirido el token correctamente");
                    PlayerPrefs.SetString("token", data["token"]);
                    var auth = gameObject.AddComponent<APIRequestNetgamez>();
                }
            }
        }
    }

    void LogState (int state, string value, bool error, string functionName){
        if(printLog){
            string message = "";
            switch(state){
                case 0: message ="Iniciando Autenticación netgamez"; break;
                case 1: message ="intentando obtener valor desde la aplicación de netgamez"; break;
                case 2: message ="Realizando proceso de obtención de token desde la aplicación de netgamez"; break;
                case 3: message ="Token obtenido desde la aplicación de netgamez: " + value; break;
                case 4: message ="Token Almacenado en caché: " + value; break;
                case 5: message ="El valor obtenido y almacenado es vacío o nulo"; break;
                case 6: message ="Realizar validación de token con servicio 'RenovarSesion'"; break;
                case 7: message ="Ha ocurrido un error con el proceso de ejecución"; break;
                case 8: message ="Obteniendo token guardado en caché: " + value; break;
                case 9: message ="Obteniendo código copiado en portapeles"; break;
                case 10: message ="El código obtenido del portapapeles es vacío o nulo"; break;
                case 11: message ="El código obtenido del portapapeles es: "+ value; break;
                case 12: message ="Realizar validación de código con servicio 'ObtenerCodigo'"; break;
            }
            if(!error){
            Debug.Log(functionName + " - " + "Log: " + message +" - "+ "Message State Code: " + state);
            } else {
            Debug.LogWarning(functionName + " - " + "Log: " + message +" - "+ "Message State Code: " + state);
            }
        }
    }//LogState
}

/* class Name
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public decimal Estatura { get; set; }
    public string Apellido { get; set; }
} */


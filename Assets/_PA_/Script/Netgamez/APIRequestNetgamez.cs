using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class APIRequestNetgamez : MonoBehaviour
{
    public Text log;
    Text textInfo;
    InputField inputText;
    void Start()
    {
        textInfo = gameObject.GetComponent<AuthNetgamez>().textInfo;
        inputText = gameObject.GetComponent<AuthNetgamez>().inputText;
        StartCoroutine(Upload());
    }

    IEnumerator Upload() {
    WWWForm form = new WWWForm();
    form.AddField("metodo", "RenovarSesion");
    form.AddField("SesionID", PlayerPrefs.GetString("token"));
    Debug.Log(PlayerPrefs.GetString("token"));
    textInfo.text = "Accediendo a juego";

    using (UnityWebRequest www = UnityWebRequest.Post("https://facilservicios.com/servicioDesarrollo.php", form)) {

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
                log.text = www.error;
                PlayerPrefs.SetString("token", "");
                textInfo.text = "ha habido un problema de conexión, intentalo nuevamente";
                yield return new WaitForSeconds(2);
                gameObject.GetComponent<IntroManager>().screenInfo(1);
                gameObject.GetComponent<AuthNetgamez>().Focus = true;
            }

        else {

           JSONNode data = JSON.Parse(www.downloadHandler.text);
           //Debug.Log(www.downloadHandler.text);
            int saldo = 0;

            if(data["Error"] == "-1"){
                PlayerPrefs.SetString("token", "");
                textInfo.text = data["ErrorDescripcion"];
                yield return new WaitForSeconds(2);
                gameObject.GetComponent<IntroManager>().screenInfo(1);
                gameObject.GetComponent<AuthNetgamez>().Focus = true;

            }

            else {
                //gameObject.GetComponent<AuthNetgamez>().log.GetComponent<Text>().text = "Obteniendo acceso...";
                for (int i = 0; i < data["ListaObjetos"].Count; i++){
                JSONNode datos = data["ListaObjetos"][i];      
                    
                    if (datos["tipocampo"] == "FDC.Seguridad.Usuario.Saldo") {

                        if (datos["valor3"] == "NetGamezMonedas") {
                            saldo = datos["valor"] - datos["valor2"];
                        }
                        if (datos["valor3"] == "Agorat.EspadaCobre") {
                            GlobalValue.character2 = datos["valor"];
                        }
                        if (datos["valor3"] == "Agorat.EspadaAcero") {
                            GlobalValue.character3 = datos["valor"];
                        }
                        if (datos["valor3"] == "Agorat.EspadaFuego") {
                            GlobalValue.character4 = datos["valor"];                     
                        }
                    }
                }

            GlobalValue.NZCoins = saldo;
            //gameObject.GetComponent<AuthNetgamez>().log.GetComponent<Text>().text = "Abriendo juego";
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("MainMenu");

            }

/*             string value = data["ListaObjetos"].AsArray[5].AsObject["valor"].ToString();
            string value2 = data["ListaObjetos"].AsArray[5].AsObject["valor2"].ToString(); 
            //GlobalValue.SavedCoins = System.Convert.ToInt32(value) - System.Convert.ToInt32(value2);

            DebugLog.text = data["ListaObjetos"].ToString(); */
        }

    }

    }

}//Class

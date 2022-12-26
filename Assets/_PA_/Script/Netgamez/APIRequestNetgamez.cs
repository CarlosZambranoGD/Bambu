using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class APIRequestNetgamez : MonoBehaviour
{
    public Text log;
    void Start()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload() {
    WWWForm form = new WWWForm();
    form.AddField("metodo", "RenovarSesion");
    form.AddField("SesionID", GlobalValue.token);

    using (UnityWebRequest www = UnityWebRequest.Post("https://facilservicios.com/servicioDesarrollo.php", form)) {

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
                log.text = www.error;
            }

        else {
           JSONNode data = JSON.Parse(www.downloadHandler.text);
           //Debug.Log(www.downloadHandler.text);
            int saldo = 0;

            if(data["Error"] == "-1"){
                //Debug.Log(www.downloadHandler.text);
                gameObject.GetComponent<AuthNetgamez>().log.GetComponent<Text>().text = data["ErrorDescripcion"];
                gameObject.GetComponent<AuthNetgamez>().Exit.SetActive(true);
            }

            else {
                gameObject.GetComponent<AuthNetgamez>().log.GetComponent<Text>().text = "Obteniendo acceso...";
                for (int i = 0; i < data["ListaObjetos"].Count; i++){
                JSONNode datos = data["ListaObjetos"][i];      
                    
                    if (datos["tipocampo"] == "FDC.Seguridad.Usuario.Saldo") {
                        if (datos["valor3"] == "NetGamezMonedas") {
                            saldo = datos["valor"] - datos["valor2"];
                        }

                        if (datos["valor3"] == "Agorat.EspadaDeAcero") {
                            GlobalValue.character2 = datos["valor"];
                        }
                    }
                }

            GlobalValue.NZCoins = saldo;
            gameObject.GetComponent<AuthNetgamez>().log.GetComponent<Text>().text = "Abriendo juego";
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

    IEnumerator Quit(){
        
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

}//Class

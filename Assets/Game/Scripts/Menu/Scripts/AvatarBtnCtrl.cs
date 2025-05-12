using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarBtnCtrl : MonoBehaviour
{
    private int indice;
    public GameObject[] ListaAvatars;

    // Controlador de botones de selecci?n de avatars
    void Start()
    {
    }

    public void CambiarIzquierda()
    {
        // Desactivar el avatar actual
        ListaAvatars[indice].SetActive(false);

        indice--;
        if (indice < 0)
            indice = ListaAvatars.Length - 1;

        // Cambiar al nuevo Avatar
        ListaAvatars[indice].SetActive(true);
    }
    public void CambiarDerecha()
    {
        // Desactivar el avatar actual
        ListaAvatars[indice].SetActive(false);

        indice++;
        if (indice >= ListaAvatars.Length)
            indice = 0;

        // Activar el nuevo avatar
        ListaAvatars[indice].SetActive(true);
    }

    public void ConfirmarAvatar()
    {
        // Guardar el avatar seleccionado.
        PlayerPrefs.SetInt("PersonajeSeleccionado", indice);
        Debug.Log("Guardado: " + indice);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button startBtn;


    private void Awake() {
        startBtn.gameObject.SetActive(false);

        hostBtn.onClick.AddListener(() => {
            TestLobby.CreateLobby();        
            hostBtn.gameObject.SetActive(false);
            clientBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(true);
        });
        clientBtn.onClick.AddListener(() => { 
            TestLobby.QuickJoinLobby();
            clientBtn.gameObject.SetActive(false); //ToDo: check if actually logged
            hostBtn.gameObject.SetActive(false);
        });
        startBtn.onClick.AddListener(() => {
            //ToDo: check if the second player in
            TestLobby.StartGame();
            startBtn.gameObject.SetActive(false);
        });
    }
}

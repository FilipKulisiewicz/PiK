using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Netcode.Transports.UTP;

public class TestRelay : MonoBehaviour
{
    public static TestRelay Instance { get; private set; }
	    
    private void Awake() {
        Instance = this;
    }
    
    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1); //1 means 2 players (host + 1 guest)
            Debug.Log("RelayCreated");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation. AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return null;
        }
    }
    
    public async void JoinRelay(string joinCode) {
        try {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}

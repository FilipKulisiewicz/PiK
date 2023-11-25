using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterController : NetworkBehaviour {

    private Character character;
    
    private float holdDownStartTime;
    public bool isOnLeft = true;
    float angularVelocity;
    Vector3 projectileAimPoint;
    Vector3 projectileSpawnPoint;
   
    override public void OnNetworkSpawn(){ //make a character attaching script
        if (IsHost){
            character = GameObject.Find("Cat").GetComponent<Character>();
            isOnLeft = true;
        }
        else if(!IsHost && IsClient){
            character = GameObject.Find("Dog").GetComponent<Character>();
            isOnLeft = false;
        }
    }
    
    private void Update() {
        if(!IsOwner){
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            // Mouse Down, start holding
            holdDownStartTime = Time.time;
        }

        if (Input.GetMouseButton(0)) {
            // Mouse still down, show force
            float holdDownTime = Time.time - holdDownStartTime;
            character.ShowForce(CalculateHoldDownForce(holdDownTime));
        }

        if (Input.GetMouseButtonUp(0)) {
            // Mouse Up, Launch!
            float holdDownTime = Time.time - holdDownStartTime;
            character.HideForce();
            projectileSpawnPoint = character.GetComponent<Transform>().position + new Vector3(0.0f, 1.0f, 0.0f); //new Vector3 (0.0f, 0.0f, 0.0f);
            projectileAimPoint = projectileSpawnPoint;
            
            if (isOnLeft == true){
                projectileAimPoint += new Vector3 (1.0f, 1.0f, 0.0f); 
                angularVelocity = -80.0f;
            }
            else{
                projectileAimPoint += new Vector3 (-1.0f, 1.0f, 0.0f);
                angularVelocity = 80.0f;
            }
            float force = CalculateHoldDownForce(holdDownTime);
            SpawnProjectileServerRpc(projectileSpawnPoint, projectileAimPoint, force, angularVelocity);
        }
    }

    public static float MAX_FORCE = 45.0f;
    public static float MinForceFactor = 0.10f;
    public static float maxForceHoldDownTime = 1.5f;
    private float CalculateHoldDownForce(float holdTime) { //ToDo improve for more satisfaction    
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force =  Mathf.Clamp01(holdTimeNormalized + MinForceFactor) * MAX_FORCE;
        return force;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileServerRpc(Vector3 projectileSpawnPoint, Vector3 projectileAimPoint, float force, float angularVelocity){ 
        Transform projectileTransform = character.Throw(projectileSpawnPoint, projectileAimPoint, force, angularVelocity);
        projectileTransform.GetComponent<NetworkObject>().Spawn(true);
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace SnowballProject
{
    public class Player : NetworkBehaviour
    {
        public static Player localPlayer;
        [SyncVar]
        public string matchID;
        [SyncVar]
        public int playerIndex;

        public NetworkMatchChecker networkMatchChecker;

        public GameObject uILobby;

        private void Start()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
            if (isLocalPlayer)
            {
                uILobby = GameObject.Find("---UI");
                localPlayer = this;               
            }
            else
            {
                UILobby.instance.SpawnPlayerUIPrefab(this);
            }
        }

        // HOST GAME

        public void HostGame()
        {
            string matchID = MatchMaker.GetRandomMatchID();
            CmdHostGame(matchID);
        }

        [Command]
        void CmdHostGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.instance.HostGame(_matchID, gameObject, out playerIndex))
            {
                Debug.Log($"Game Hosted Successfully");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetHostGame(true, _matchID);
            }
            else
            {
                Debug.Log($"<color = red> Game Hosted Failed/<color>");
                TargetHostGame(false, _matchID);
            }
        }
        [TargetRpc]
        void TargetHostGame(bool success, string _matchID)
        {
            Debug.Log($"Match ID: (matchID) == (_matchID)");
            UILobby.instance.HostSuccess(success);
        }

        // JOIN GAME

        public void JoinGame(string _inputID)
        {
            CmdJoinGame(_inputID);
        }

        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
            {
              //  Debug.Log($"Game Hosted Successfully");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetJoinGame(true, _matchID);
            }
            else
            {
                Debug.Log($"<color = red> Game Hosted Failed/<color>");
                TargetJoinGame(false, _matchID);
            }
        }
        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID)
        {
            matchID = _matchID;
            Debug.Log($"Match ID: (matchID) == (_matchID)");
            UILobby.instance.JoinSuccess(success, matchID);
        }

        // BEGIN GAME

        public void BeginGame()
        {
            CmdBeginGame();
        }

        [Command]
        void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(matchID);                       
        }

        public void StartGame()
        {
            TargetBeginGame();
        }

        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log($"Match ID: (matchID) == (_matchID)");
            Respawn(gameObject);
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
            Cursor.lockState = CursorLockMode.Locked;
            uILobby.SetActive(false);           
        }

        // RESPAWN IN ONE OF THE SPAWN POSITIONS

        void Respawn(GameObject go)
        {
           cmdRespawn(go);
        }
        [Command]
        void cmdRespawn(GameObject go)
        {
            rpcRespawn(go);
        }
        [ClientRpc]
        void rpcRespawn(GameObject go)
        {           
            Transform newPos = NetworkManager.singleton.GetStartPosition();
            go.transform.position = newPos.transform.position;
            go.transform.rotation = newPos.transform.rotation;           
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Simple controller class for managing the player movement, input and shooting
/// </summary>

namespace Game
{
    [RequireComponent(typeof(PlayerMover), typeof(PlayerInput), typeof(PlayerWeapon))]
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera = null;

        private PlayerMover playerMover;
        private PlayerInput playerInput;
        private PlayerWeapon playerWeapon;

        public static string playerTagName = "Player";

        private void Awake()
        {
            playerMover = GetComponent<PlayerMover>();
            playerInput = GetComponent<PlayerInput>();
            playerWeapon = GetComponent<PlayerWeapon>();
            EnablePlayer(true);
        }

        private void Update()
        {
            //if (GameManager.IsGameOver())
            //{
            //    return;
            //}

            playerWeapon.IsFireButtonDown = playerInput.IsFiring;

        }

        private void FixedUpdate()
        {
            // hide the player if destroyed
            //if (GameManager.IsGameOver())
            //{
            //    EnablePlayer(false);
            //    return;
            //}

            // get the keyboard input converted to camera space
            Vector3 input = playerInput.GetCameraSpaceInputDirection(sceneCamera);

            // use input to move the player
            playerMover.MovePlayer(input);

            // aim the turret to the mouse position
            playerMover.AimAtMousePosition(sceneCamera);
        }

        // toggle all GameObjects associated with the Player tag
        public static void EnablePlayer(bool state)
        {
            GameObject[] allPlayerObjects = GameObject.FindGameObjectsWithTag(playerTagName);
            foreach (GameObject go in allPlayerObjects)
            {
                go.SetActive(state);
            }
        }
    }
}
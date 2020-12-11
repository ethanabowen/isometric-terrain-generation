using UnityEngine;
using UnityEngine.UIElements;

namespace Player {
    public class ShootProjectile : MonoBehaviour {
        public GameObject projectile;
        public float speed;


        public void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Vector3 inertia = GetInertia();
                var isoContoller = transform.GetComponent<IsometricPlayerMovementController>();
                //always 'project' out from in front of the character
                Vector3 isoForward = CalculateIsoForward(isoContoller);
                GameObject bullet =
                    Instantiate(projectile, transform.position + isoForward, Quaternion.identity);
                Vector3 force = (inertia + isoForward * speed) * isoContoller.movementSpeed;
                bullet.GetComponent<Rigidbody2D>().AddForce(force);

                /* TODO: destory projectiles after certain time or collison */
            }
        }

        /// <summary>
        /// Vector based on characters current (aka last) facing direction
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        private Vector3 CalculateIsoForward(IsometricPlayerMovementController isoContoller) {
            Vector2 lastDirection = isoContoller.isoRenderer.lastDirectionAsVector;

            Vector3 isoDirectionalForward = new Vector3(1, 1, 0);

            if (lastDirection.x == 0) {
                isoDirectionalForward.x = 0;
            }

            if (lastDirection.x < 0) {
                isoDirectionalForward.x = -1;
            }

            if (lastDirection.y == 0) {
                isoDirectionalForward.y = 0;
            }

            if (lastDirection.y < 0) {
                isoDirectionalForward.y = -1;
            }

            return isoDirectionalForward;
        }

        /// <summary>
        /// Get Interia based on current player input (movement)
        /// </summary>
        /// <returns></returns>
        private Vector3 GetInertia() {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            
            Vector3 inputVector = new Vector3(horizontalInput, verticalInput, 0);
            inputVector = Vector3.ClampMagnitude(inputVector, 1);
            
            return inputVector * speed;
        }
    }
}
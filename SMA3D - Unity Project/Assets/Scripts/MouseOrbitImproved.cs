/* 
 * Description: Improved version of the original MouseOrbit script. Zooms with the mousewheel and uses linecast to make sure that object isn't behind anything.
 * Author: Veli V
 * Url: https://wiki.unity3d.com/index.php/MouseOrbitImproved
 */
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class MouseOrbitImproved : MonoBehaviour
    {
        public float Distance = 5.0f;
        public float DistanceMax = 15f;

        public float DistanceMin = .5f;

        private Rigidbody _rigidbody;
        public Transform Target;

        private float _x;
        public float XSpeed = 20.0f;
        private float _y;
        public float YMaxLimit = 80f;

        public float YMinLimit = -20f;
        public float YSpeed = 80.0f;

        private void Start()
        {
            var angles = transform.eulerAngles;
            _x = angles.y;
            _y = angles.x;

            _rigidbody = GetComponent<Rigidbody>();

            if (_rigidbody != null)
                _rigidbody.freezeRotation = true;
            UpdateCameraPosition();
        }

        private void LateUpdate()
        {
            if (Target && Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
                UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            _x += Input.GetAxis("Mouse X") * XSpeed * Distance * 0.02f;
            _y -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;

            _y = ClampAngle(_y, YMinLimit, YMaxLimit);

            var rotation = Quaternion.Euler(_y, _x, 0);

            Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * 5, DistanceMin, DistanceMax);

            RaycastHit hit;
            if (Physics.Linecast(Target.position, transform.position, out hit))
                Distance -= hit.distance;
            var negDistance = new Vector3(0.0f, 0.0f, -Distance);
            var position = rotation * negDistance + Target.position;

            transform.rotation = rotation;
            transform.position = position;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private Vector3 velocity;
    private Rigidbody rb;
    private Vector3 rotation;
    private float cameraRotationX = 0f;
    private Vector3 thrusterVelocity;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float cameraRotationLimit = 85f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }   
    
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }   
    
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    public void ApplyThruster(Vector3 _thrusterVelocity)
    {
        thrusterVelocity = _thrusterVelocity;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformRotation()
    {

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation) );

        currentCameraRotationX -= cameraRotationX;

        //On bloque la rotation up et down de la camera
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        //Rotation de la camera
        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void PerformMovement()
    {
        //Si le joueur bouge
        if(velocity != Vector3.zero)
        {
            //Position actuel + mouvement + ? 
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        //Si le jetPack est utilisé
        if(thrusterVelocity != Vector3.zero)
        {
            rb.AddForce(thrusterVelocity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
}

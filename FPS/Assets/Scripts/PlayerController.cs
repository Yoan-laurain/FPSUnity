using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;    
    
    [SerializeField]
    private float mouseSensitivityX = 3f;   
    
    [SerializeField]
    private float mouseSensitivityY = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    private PlayerMotor motor;

    private ConfigurableJoint joint;

    private Animator animator;

    [Header("JointOptions")]

    [SerializeField]
    private float jointSpring = 20f;

    [SerializeField]
    private float jointMaxForce = 50f;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    private void Update()
    {

        // -1 = Touche négative , 0 = Pas de mouvement , 1 = Touche positive

        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        //On récupère la direction ou on veut aller

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        // Calculer la vélocité du mouvement de notre joueur

        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        //Jouer animation thruster

        animator.SetFloat("ForwardVelocity",zMove);

        motor.Move(velocity);

        // On calcul la rotation du joueur en un Vector3

        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;

        motor.Rotate(rotation);


        // On calcul la rotation de la camera en un Vector3

        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * mouseSensitivityY;

        motor.RotateCamera(cameraRotationX);



        // JETPACK //

        Vector3 thrusterVelocity = Vector3.zero;

        //SAUT

        if(Input.GetButton("Jump"))
        {
            thrusterVelocity = Vector3.up * thrusterForce;

            //Enlève la gravité
            SetJointSettings(0f);
        }
        else
        {
            //Ré-active la gravité
            SetJointSettings(jointSpring);
        }

        motor.ApplyThruster(thrusterVelocity);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}

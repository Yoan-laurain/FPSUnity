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

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;

    private float thrusterFuelAmount = 1f;

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

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    private void Update()
    {
        RaycastHit _hit;


        if(Physics.Raycast(transform.position,Vector3.down,out _hit,100f) )
        {
            joint.targetPosition = new Vector3( 0f , -_hit.point.y , 0f );
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }


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

        if(Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            //On consomme le fuel
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                // Puissance de décolage
                thrusterVelocity = Vector3.up * thrusterForce;

                //Enlève la gravité
                SetJointSettings(0f);
            }
        }
        else
        {
            // On regen le fuel
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            //Ré-active la gravité
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        motor.ApplyThruster(thrusterVelocity);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}

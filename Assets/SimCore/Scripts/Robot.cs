using ROS2;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public ArticulationBody moto_joint_1;
    public ArticulationBody moto_joint_2;
    public ArticulationBody moto_joint_3;
    public ArticulationBody moto_joint_4;
    public GameObject robot_body;

    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<tf2_msgs.msg.TFMessage> tf_pub;

    private float[] targetSpeed;

    void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
        targetSpeed = new float[4];
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("HelloDotSim");
                ros2Node.CreateSubscription<std_msgs.msg.Float32>("/motor_1", msg => { targetSpeed[0] = msg.Data; });
                ros2Node.CreateSubscription<std_msgs.msg.Float32>("/motor_2", msg => { targetSpeed[1] = msg.Data; });
                ros2Node.CreateSubscription<std_msgs.msg.Float32>("/motor_3", msg => { targetSpeed[2] = msg.Data; });
                ros2Node.CreateSubscription<std_msgs.msg.Float32>("/motor_4", msg => { targetSpeed[3] = msg.Data; });
                tf_pub = ros2Node.CreatePublisher<tf2_msgs.msg.TFMessage>("/tf");
            }

            moto_joint_1.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[0]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_2.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[1]) * 200 * Random.Range(0.9f, 1.1f)); 
            moto_joint_3.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[2]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_4.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[3]) * 200 * Random.Range(0.9f, 1.1f));


            // publish tf
            var pose_robot = new geometry_msgs.msg.TransformStamped();
            pose_robot.Header.Frame_id = "map";
            pose_robot.Child_frame_id =  "base_link";
            pose_robot.Transform = GlobalObjTramsformToROS2(robot_body);

            var tf = new tf2_msgs.msg.TFMessage
            {
                Transforms = new geometry_msgs.msg.TransformStamped[1] { pose_robot }
            };
            tf_pub.Publish(tf);
        }
    }

    float CheckError(float input)
    {
        if(input > 1.0){return (float)1.0;}
        if (input < -1.0){return (float)-1.0;}

        return (float)input;
    }

    geometry_msgs.msg.Transform GlobalObjTramsformToROS2(GameObject obj)
    {
        var transform_rt = new geometry_msgs.msg.Transform();
        transform_rt.Translation.X = obj.transform.position.x;
        transform_rt.Translation.Y = obj.transform.position.z;
        transform_rt.Translation.Z = obj.transform.position.y;
        transform_rt.Rotation.X = obj.transform.eulerAngles.x;
        transform_rt.Rotation.Y = obj.transform.eulerAngles.y;
        transform_rt.Rotation.Z = obj.transform.eulerAngles.z;
        return transform_rt;
    }
}

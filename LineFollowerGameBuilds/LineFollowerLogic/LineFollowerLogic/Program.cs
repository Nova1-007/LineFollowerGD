using LineFollowerClient;

public class MyClass : Client
{
    public static void Main()
    {
        MyClass myClass = new MyClass();
        myClass.MyClient();
    }

    protected override void LineFollowerLogic()
    {
        int left_motor;
        int right_motor;
        int speed = 20;
        if (OuterLeftSensor() == true & OuterRightSensor() == true)
        {
            left_motor = speed;
            right_motor = speed;
        }
        else if (OuterLeftSensor())
        {
            left_motor = 0;
            right_motor = speed;
        }
        else if (OuterRightSensor())
        {
            right_motor = 0;
            left_motor = speed;
        }
        else
        {
            left_motor = -20;
            right_motor = -20;
        }

        arrayBuilder(left_motor, right_motor);
    }

}

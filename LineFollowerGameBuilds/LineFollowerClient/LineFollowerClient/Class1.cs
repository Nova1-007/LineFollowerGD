namespace LineFollowerClient
{
    using System;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class Client
    {
        private const string serverIP = "127.0.0.1";
        private const int port = 8888;
        private TcpClient client;
        private bool isConnected = false;
        private bool shouldRun = true; // Flag to control the update loop
        private int left_motor;
        private int right_motor;
        private bool[] sensor_array = new bool[5];
        private int speed = 100;
        private bool run_code = false;
        private int[] motor_array = new int[] { 0, 0 };

        public void MyClient()
        {
            ConnectToServer();
            StartUpdateLoop();
            Console.WriteLine("Client started. Type 'stop' to stop the client.");

            string input;
            do
            {
                input = Console.ReadLine();
            } while (input != "stop");

            Stop();
        }


        public void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverIP, port);
                Console.WriteLine("Connected to server");
                isConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void Disconnect()
        {
            if (client != null)
            {
                client.Close();
                isConnected = false;
                Console.WriteLine("Disconnected from server");
            }
        }

        private void Reconnect()
        {
            Disconnect();
            ConnectToServer();
        }

        private void StartUpdateLoop()
        {
            while (shouldRun)
            {
                if (!isConnected)
                {
                    Reconnect(); // Attempt to reconnect if not connected
                    continue;
                }
                if (isConnected)
                {
                    ReceiveDataFromServer();
                    SendDataToServer();
                }



            }
        }

        private void ReceiveDataFromServer()
        {
            try
            {
                byte[] buffer = new byte[6];
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    bool[] boolArray = new bool[6]; // Create a boolean array to store received data
                    for (int i = 0; i < boolArray.Length; i++)
                    {
                        boolArray[i] = (buffer[i] != 0);
                    }
                    for (int i = 0; i < sensor_array.Length; i++)
                    {
                        sensor_array[i] = boolArray[i];
                    }
                    run_code = boolArray[5];
                    Console.WriteLine("Received boolean array from server: " + string.Join(", ", boolArray));
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while receiving data from server: " + e.Message);
                Disconnect(); // Disconnect on error
            }
        }

        private void SendDataToServer()
        {
            try
            {
                int[] intArray = motor_array;
                byte[] data = new byte[intArray.Length * sizeof(int)];
                Buffer.BlockCopy(intArray, 0, data, 0, data.Length);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Integer array sent to server");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while sending data to server: " + e.Message);
                Disconnect(); // Disconnect on error
            }
        }

        private void Stop()
        {
            shouldRun = false;
            Disconnect();
        }

        public bool OuterLeftSensor()
        {
            return sensor_array[0];
        }
        public bool InnerLeftSensor()
        {
            return sensor_array[1];
        }
        public bool MiddleSensor()
        {
            return sensor_array[2];
        }
        public bool InnerRightSensor()
        {
            return sensor_array[3];
        }
        public bool OuterRightSensor()
        {
            return sensor_array[4];
        }

        public void arrayBuilder(int left_motor, int right_motor)
        {
            motor_array[0] = left_motor;
            motor_array[1] = right_motor;
        }

        protected abstract void LineFollowerLogic();
    }
}

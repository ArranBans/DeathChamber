using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace DeathChamberNetworking
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
        private bool[] inputs;

        public Player(int _id, string _username, Vector3 _spawnPos)
        {
            id = _id;
            username = _username;
            position = _spawnPos;
            rotation = Quaternion.Identity;

            inputs = new bool[4];
        }

        public void Update()
        {
            Vector2 _inputDirection = Vector2.Zero;
            if(inputs[0])
            {
                _inputDirection.Y += 1;
            }
            if(inputs[1])
            {
                _inputDirection.X -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.Y -= 1;
            }
            if (inputs[3])
            {
                _inputDirection.X += 1;
            }
            
            
            Move(_inputDirection);
            Console.WriteLine($"{inputs[0]},{inputs[1]}, {inputs[2]}, {inputs[3]}, ");
        }

        private void Move(Vector2 _inputDirection)
        {
            
            Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, -1, 0)));

            Vector3 _movedirection = _right * _inputDirection.X + _forward * _inputDirection.Y;
            position += _movedirection * moveSpeed;


            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] _inputs, Quaternion _rotation)
        {
            
            inputs = _inputs;
            rotation = _rotation;
        }
    }
}

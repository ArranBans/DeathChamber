using System;
using System.Threading;

namespace DeathChamberNetworking
{
    class Program
    {
        private static bool isRunning = false;
        static void Main(string[] args)
        {
            Console.Title = "DeathChamberServer";
            isRunning = true;

            Thread mainthread = new Thread(new ThreadStart(MainThread));
            mainthread.Start();

            Server.Start(3, 5225);

            //Console.ReadKey();
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started!!! Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextloop = DateTime.Now;

            while(isRunning)
            {
                while(_nextloop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextloop = _nextloop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(_nextloop > DateTime.Now)
                    {
                        Thread.Sleep(_nextloop - DateTime.Now);
                    }
                }
            }
        }
    }
}

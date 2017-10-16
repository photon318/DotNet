using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace PQueues
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to parallel queue processing tests.");
            Console.WriteLine($"Running on {Environment.ProcessorCount} CPU's.");


            // FIFO test with tasks


            // Assign
            const int count = 10000;

            var list = new List<int>();
            var tasks = new List<Task>();
            var range = Enumerable.Range(0, 1000);
 
            // Act

            foreach (var number in range)
            {
                //Console.Write($"{number} ");

                tasks.Add(Task.Factory.StartNew(() => list.Add(number), TaskCreationOptions.PreferFairness));
            }
            /*
            while (list.Count > count)
            {
                //Console.WriteLine(listserial.Count);
            };
            */
            Task.WhenAll(tasks);

            int prev = 0;
            int prevcnt = 0;
            foreach (int it in list)
            {
                if (prev == 0)
                {
                    prev = it;
                }
                else
                {
                    if (it - 1 != prev)
                        ++prevcnt;

                    prev = it;
                }
                Console.Write($"{it} ");
            }
            Console.WriteLine($"");
            Console.WriteLine($"{prevcnt}");

            Console.WriteLine($"Sequence: {list.Count} ");
            Console.WriteLine($"Sequence: {range.SequenceEqual(list)} ");

            // Assert

            var queueserial = new SerialQueue();
            var listserial = new List<int>();
            var tasksserial = new List<Task>();
            var rangeserial = Enumerable.Range(0, 10000);

            // Act
            int t = 0;
            foreach (var number in rangeserial)
            {
                tasksserial.Add(queueserial.Enqueue(() => listserial.Add(number)));
                t = number;
            }

            Console.WriteLine(listserial.Count);
            Task.WhenAll(tasksserial);
            Console.WriteLine(t);

            while (listserial.Count != count) {
                //Console.WriteLine(listserial.Count);
            };
            // Assert

            Console.WriteLine($"Sequence: {rangeserial.SequenceEqual(listserial)} ");

            // Task t = new Task.Factory.StartNew();



            var queuemulti = new SerialQueue();
            var listmulti = new List<int>();

            // Act

            var counter = 0;
            //bool done = false;
            for (int i = 0; i < count; i++)
            {
                Task.Run(() => {
                    queuemulti.Enqueue(() => listmulti.Add(counter++));
                });
            }

            //done = true;

            while (listmulti.Count != count) { };

            // Assert

            Console.WriteLine($"Sequence: {listmulti.SequenceEqual(Enumerable.Range(0, count))}");
            Console.Read();
        }
    }
}

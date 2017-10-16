using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Console.WriteLine("Parallel queue processing tests.");
            Console.WriteLine($"Running on {Environment.ProcessorCount} CPU's.");

            // Samples initialization 
            const int PARSE_SAMPLES = 100000;

            var parsesource = new List<string>();
            for (int b = 0; b < PARSE_SAMPLES; b++)
                parsesource.Add($"{b}|dasdasd|ddwdadsada|dasdasds|dsadas|dasdasdasd|dasdadasdasdsadasdadasd|dasdasdsad|dadasdsadasd|dsada|dsad|ds0adsadsada|sadsadsad|322312|sasas|123131231312312|dsds|dasdsadsdsada|dadasdasdasdas|dasdsadsadads|sdasd|dasd|");

            Stopwatch timer = new Stopwatch();
            List<string[]> parse_results = new List<string[]>();


            // Singlethread base test
            timer.Start();
            foreach (string line in parsesource)
                parse_results.Add(line.Split('|')); 

            timer.Stop();

            var sequence = parse_results.Select(x => x[0]).OfType<int>().ToList();
            //var int_seq = from parsed in parse_results select parsed[0].OfType<int>().ToList();
            Console.WriteLine($"C:{parse_results.Count} Base split: {timer.ElapsedMilliseconds} ms. Sequence: {sequence.SequenceEqual(sequence)}");



            var parce_tasks = new List<Task>();
            parse_results.Clear();

            timer.Restart();
            foreach (string line in parsesource)
            {
                parce_tasks.Add(Task.Factory.StartNew(() => parse_results.Add(line.Split('|')), TaskCreationOptions.PreferFairness));
            }

            Task.WaitAll(parce_tasks.ToArray());


            timer.Stop();

            sequence = parse_results.Select(x => x[0]).OfType<int>().ToList();
            Console.WriteLine($"C:{parse_results.Count} Multithread split: {timer.ElapsedMilliseconds} ms. Sequence: {sequence.SequenceEqual(sequence)}");

            List<Task> tasks_serial = new List<Task>();
            parse_results.Clear();
            SerialQueue queue_serial_parse = new SerialQueue();


            timer.Restart();

            foreach (string line in parsesource)
            {
                tasks_serial.Add(queue_serial_parse.Enqueue(() => parse_results.Add(line.Split('|'))));
            }

            Task.WaitAll(tasks_serial.ToArray());


            timer.Stop();

            sequence = parse_results.Select(x => x[0]).OfType<int>().ToList();

            Console.WriteLine($"C:{parse_results.Count} Serial split: {timer.ElapsedMilliseconds} ms. Sequence: {sequence.SequenceEqual(sequence)}");

            SerialQueue queue_asynch_parse = new SerialQueue();
            parse_results.Clear();

            timer.Restart();

            foreach (string line in parsesource)
            {
                Task.Run(() => {
                    queue_asynch_parse.Enqueue(() => parse_results.Add(line.Split('|')));
                });
            }

            while (parse_results.Count < PARSE_SAMPLES)
            {
                //Thread.Sleep(50);//Console.WriteLine(listserial.Count);
            };
            timer.Stop();

            sequence = parse_results.Select(x => x[0]).OfType<int>().ToList();

            Console.WriteLine($"C:{parse_results.Count} Multi serial split: {timer.ElapsedMilliseconds} ms. Sequence: {sequence.SequenceEqual(sequence)}");
            /*
            // Assign
            const int count = 10000;

            var list = new List<int>();
            var tasks = new List<Task>();
            var range = Enumerable.Range(0, 1000);

            foreach (var number in range)
            {
                tasks.Add(Task.Factory.StartNew(() => list.Add(number), TaskCreationOptions.PreferFairness));
            }

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
            */
        }
    }
}

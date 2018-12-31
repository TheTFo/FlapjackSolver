using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlapjackSolver
{
    class Program
    {
        static uint[] MASKS;
        static uint SUCCESS_STATE;
        
        class State
        {
            public uint Data { get; set; }
            public ushort Steps { get; set; }
            public ushort Next { get; set; }
        }
        
        static int counter = 0;
        static State worstState = null;

        private static void BuildState(State[] states, State root)
        {
            var queue = new Queue<State>();
            states[root.Data] = root;
            worstState = root;
            queue.Enqueue(root);

            while (queue.TryDequeue(out State currentState))
            {
                counter++;
                for (ushort i = 0; i < MASKS.Length; i++)
                {
                    var newData = currentState.Data ^ MASKS[i];
                    if (states[newData] != null)
                    {
                        continue;
                    }
                    
                    var newState = new State
                    {
                        Data = newData,
                        Steps = (ushort)(currentState.Steps + 1),
                        Next = i
                    };

                    states[newData] = newState;

                    if (worstState.Steps < newState.Steps)
                    {
                        worstState = newState;
                    }

                    queue.Enqueue(newState);
                }
            }
        }

        private static uint[] GenerateMasks(ushort n)
        {
            var result = new uint[n * n];
            for (int i = 0; i < n * n; i++)
            {
                uint mask = 0;

                // left
                if (i % n > 0)
                {
                    mask ^= (uint)(1 << (i - 1));
                }

                // right
                if (i % n < (n - 1))
                {
                    mask ^= (uint)(1 << (i + 1));
                }

                // up
                if (i >= n)
                {
                    mask ^= (uint)(1 << (i - n));
                }

                // down
                if (i + n < n * n)
                {
                    mask ^= (uint)(1 << (i + n));
                }

                // self
                mask ^= (uint)(1 << i);

                result[i] = mask;
            }
            return result.Reverse().ToArray();
        }

        static void Main(string[] args)
        {
            Console.Write("Enter n: ");
            ushort n = Convert.ToUInt16(Console.ReadLine());
            MASKS = GenerateMasks(n);
            SUCCESS_STATE = (uint)(1 << n * n) - 1;

            Console.WriteLine($"{Convert.ToString(SUCCESS_STATE, 2).PadLeft(3 * 3, '0')}");

            var states = new State[SUCCESS_STATE + 1];
            BuildState(states, new State {
                Data = SUCCESS_STATE,
                Next = ushort.MaxValue,
                Steps = 0
            });

            Console.WriteLine($"Count: {counter}");
            Console.WriteLine($"You are never more than {worstState.Steps} from winning!");
            Console.WriteLine($"That board looks like: {Convert.ToString(worstState.Data, 2).PadLeft(n * n, '0')}");
            
            if (states.Any(s => s == null))
            {
                Console.WriteLine("There are some states that cannot be won.");
            }

            while (true)
            {
                Console.Write("Enter your board (Underscores between rows): ");
                var input = Console.ReadLine();
                if (input.ToLower() == "done")
                {
                    return;
                }
                var key = Convert.ToUInt32(input.Replace("_", ""), 2);
                var currentState = states[key];
                Console.WriteLine($"{currentState.Steps} moves to succeed.");
                while (currentState.Next < ushort.MaxValue)
                {
                    Console.WriteLine($"Push {currentState.Next}");
                    currentState = states[currentState.Data ^ MASKS[currentState.Next]];
                }
            }
        }
    }
}

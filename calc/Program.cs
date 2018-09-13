using System;

namespace calc {
    public class Program {
        public static void Main(string[] args) {
            try {
                if (Input.CheckInput(args)) {
                    var equations = Input.ParseInput(args);
                    equations[0].Build();
                    equations[1].Build();
                    equations[0].Merge(equations[1]);

                    Console.WriteLine("X = {0}", equations[0].Solve());
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: {0}", ex.Message);
            } finally {
                Console.ReadKey();
            }
        }
    }
}
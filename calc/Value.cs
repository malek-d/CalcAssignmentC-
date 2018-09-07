using System.Linq;

namespace calc {

    /// <summary>
    /// Object to hold actual numbers, variables(X) & numbers with variables
    /// Each non-operator value passed through CLI becomes a Value
    /// </summary>
    public class Value {
        public Coefficient Coefficient { get; }
        public int Number { get; }

        public Value(string number) {
            if (number.Contains('X')) {
                Coefficient = Coefficient.X;
                Number = ParseCoefficient(number);
            } else {
                Coefficient = Coefficient.Constant;
                Number = int.Parse(number);
            }
        }

        private int ParseCoefficient(string number) {
            if (!number.Equals("X")) {
                var numbers = number.Split('X'); // Separates number from X
                if (numbers[0].Equals("+") || numbers[0].Equals("-")) {
                    numbers[1] = "1";
                }
                return int.Parse(string.Join("", numbers));
            } 
            // no number in front of X
            return 1;
        }
    }
}

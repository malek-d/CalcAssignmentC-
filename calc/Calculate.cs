using System;
using System.Linq;

namespace calc {
    public static class Calculate {

        /// Takes numbers and operator arrays and calculates based of BODMAS principles
        public static double CalculateInput(double[] numbers, char[] operators) {

            if (operators.Length == 0 && numbers.Length != 0) { // just the single number
                return numbers[0];
            }

            string[] orderOfOperators = {"*/%", "+-"};
            var result = 0.0;

            //find all operators in BODMAS priority
            foreach (var order in orderOfOperators) {
                for (int i = 0; i < operators.Length; i++) {
                    if (order.Contains(operators[i])) {
                        // Pushes the result to the right of the equation after calculating 
                        // e.g. 4 + 12 / 12 -> 4 + 0 + 1 -> 4 + 0 + 1 -> 0  + 4 + 1 -> 0 + 0 + 5
                        var calculation = Calculation(numbers[i], operators[i], numbers[i + 1]);
                        numbers[i] = 0;
                        operators[i] = '+';
                        numbers[i + 1] = calculation;
                        result = calculation;
                    }
                }
            }
            return result;
        }

         ///Following function handles actual calculation of variables/numbers
         ///Check for possible calculation error
         ///Then calculate based on operation passed
        private static double Calculation(double first, char operation, double second) {
            double result = 0;
            //Handle dividebyZero error
            if ((operation == '/' || operation == '%') && second == 0) { 
                throw new Exception(Errors.DivideByZero);
            } 

            // Check for overflow
            if (second > 0) {
                if (first > Int32.MaxValue - second || first < Int32.MinValue + second) {
                    throw new Exception(Errors.OutOfIntegerRange);
                }
            } else {
                if (first > Int32.MaxValue + second || first < Int32.MinValue - second) {
                    throw new Exception(Errors.OutOfIntegerRange);
                }
            }

            switch (operation) {
                case '*':
                    result = first * second;
                    break;
                case '/':
                    result = first / second;
                    break;
                case '%':
                    result = first % second;
                    break;
                case '+':
                    result = first + second;
                    break;
                case '-':
                    result = first - second;
                    break;
            }

            return result;
        }
    }
}

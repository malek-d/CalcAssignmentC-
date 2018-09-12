using System;
using System.Collections.Generic;
using System.Linq;

namespace calc {

    /// <summary>
    /// Handles the equation as a whole by building equations 
    /// which holds both constants & X's, merges two equations (left and right side)
    /// and solves equation
    /// </summary>
    public class Equation {
        private List<Value> _numbers;
        private List<char> _operators;
        private double _a; // X part of the equation
        private double _b; // constant part of the equation

        public Equation() {
            _numbers = new List<Value>();
            _operators = new List<char>();
            _a = 0;
            _b = 0;
        }

        public void Add(char operation) {
            _operators.Add(operation);
        }

        public void Add(string value) {
            _numbers.Add(new Value(value));
        }

        // For adding/expanding brackets into an equation
        public void Add(Equation equation, string modifier, char operation) {
            _numbers.Add(equation._numbers[0]);
            AddModifier(modifier, operation);
            for (var i = 0; i < equation._numbers.Count-1; i++) {
                // modifier essentially expands the equation
                _operators.Add(equation._operators[i]);
                _numbers.Add(equation._numbers[i+1]);
                AddModifier(modifier, operation);

            }
        }

        //
        private void AddModifier(string modifier, char operation) {
            Add(modifier);
            Add(operation);
        }

        // Clean up the equation to determine parameters for linear equation
        // e.g. X + 10 + 2X + 2 -> _a = 3, _b = 12
        public void Build() {
            //Add a 0 to a trailing + or -
            if (_operators.Count != 0 && _operators.Count == _numbers.Count) {
                if (_operators[_operators.Count - 1] == '+' || _operators[_operators.Count - 1] == '-') {
                    _numbers.Add(new Value("0"));
                }
            }

            // Separate equation with their respective parameters 
            // (X with X's [or if there's a * or / or %])
            var xNumbers = new List<double>();
            var xOperators = new List<char>();
            var numbers = new List<double>();
            var operators = new List<char>();

            for (int i = 0; i < _numbers.Count; i++) {
                if (_numbers[i].Coefficient == Coefficient.X) {
                    if (xNumbers.Count == 0 && i == 0) {
                        // empty list and start of loop, just add number
                        xNumbers.Add(_numbers[i].Number);
                    } else if (xNumbers.Count == 0) {
                        // if operator before is negative make number negative
                        xNumbers.Add(_operators[i-1] == '-' ? -_numbers[i].Number : _numbers[i].Number);
                    } else {
                        // add number and operator alongside
                        xNumbers.Add(_numbers[i].Number);
                        xOperators.Add(_operators[i-1]);
                    }
                } else {
                    if (i < _numbers.Count - 1 && "*/%".Contains(_operators[i]) &&
                        _numbers[i + 1].Coefficient == Coefficient.X) { // if next operator is */% and the next number is an X
                        xNumbers.Add(_numbers[i].Number);
                        if (xOperators.Count != 0) { // only add operator if list is populated
                            xOperators.Add(_operators[i - 1]);
                        }
                    } else if (i > 0 && "*/%".Contains(_operators[i-1]) &&
                        _numbers[i-1].Coefficient == Coefficient.X) { // if previous operator is */% and previous number is an X
                        xNumbers.Add(_numbers[i].Number);
                        xOperators.Add(_operators[i-1]);
                    } else {
                        if (numbers.Count == 0 && i == 0) {
                            // same logic as above
                            numbers.Add(_numbers[i].Number);
                        } else if (numbers.Count == 0) {
                            // same logic as above
                            numbers.Add(_operators[i - 1] == '-' ? -_numbers[i].Number : _numbers[i].Number);
                        } else {
                            // same logic as above
                            numbers.Add(_numbers[i].Number);
                            operators.Add(_operators[i-1]);
                        }
                    }
                }
            }

            _a = Calculate.CalculateInput(xNumbers.ToArray(), xOperators.ToArray()); //_a will now hold the calculated value of X for this equation
            _b = Calculate.CalculateInput(numbers.ToArray(), operators.ToArray()); //_b holds the calculated value for constants
        }

        // Merge two equations together
        public void Merge(Equation equation) {
            _a -= equation._a;
            _b -= equation._b;
        }

        public int Solve() {
            if (_a != 0) {
                return (int)Math.Round(-_b / _a);
            }
            return (int)-_b;
        }
    }
}

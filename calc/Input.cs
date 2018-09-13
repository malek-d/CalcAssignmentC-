using System;
using System.Linq;

namespace calc {
    public static class Input {
        public static bool CheckInput(string[] args) {
            var equalsCount = 0; // only one equal sign
            var variableCount = 0; // needs at least one variable
            var wasNumber = false; // checks if the previous argument was a number

            if (!isValidFirstArgument(args[0])) { //Assignment specifies first argument should be 'calc'
                throw new Exception(Errors.InvalidFirstArgument);
            }

            for (int i = 1; i < args.Length; i++) {
                if (args[i].Length == 1) { // e.g. *
                    if (!IsValidCharacter(args[i][0])) {
                        throw new Exception(Errors.InvalidCharacter);
                    }

                    if (args[i][0] == '=') {
                        equalsCount++;
                        if (i == args.Length - 1 || i == 0) { // when an equal sign is at the start or end of an equation
                            throw new Exception(Errors.InvalidEqualPlacement);
                        }
                    }

                    if (IsNumber(args[i][0]) && wasNumber) {
                        throw new Exception(Errors.InvalidEquation);
                    }

                    if (IsNumber(args[i][0])) {
                        wasNumber = true;
                    }

                    if (args[i][0] == 'X') {
                        variableCount++;
                    }

                    if (IsOperator(args[i][0]) || args[i][0] == '=' || args[i][0] == '(' || args[i][0] == ')') {
                        wasNumber = false;
                    }
                } else { // e.g. -34X
                    for (int j = 0; j < args[i].Length; j++) {
                        if (!IsValidCharacter(args[i][j])) {
                            throw new Exception(Errors.InvalidCharacter);
                        }

                        if (args[i][j] == 'X') {
                            variableCount++;
                        }

                        if (IsNumber(args[i][j]) && !wasNumber) { // the current argument contains a number
                            wasNumber = true;
                        }
                    }
                }
            }

            if (equalsCount != 1) {
                throw new Exception(Errors.IncorrectEqualSigns);
            }

            if (variableCount == 0) {
                throw new Exception(Errors.IncorrectVariables);
            }

            return true;
        }

        public static Equation[] ParseInput(string[] args) {
            Equation leftSide = new Equation();

            var currentSide = new Equation();
            var equals = false;
            var brackets = false; // check if brackets is open
            var bracketEquation = new Equation();
            var bracketModifier = "1";
            var bracketOperator = '*';
            for (int i = 0; i < args.Length; i++) {
                if (equals) {      
                    leftSide = currentSide;
                    currentSide = new Equation(); // switches side of the equation (current side is now right side)
                    equals = false; // will no longer switch on loop
                }
                bool wasNumber = false; // check if argument is a number
                foreach (var c in args[i]) {
                    if (c == '=') {
                        equals = true;
                    } else if (IsNumber(c) && !wasNumber) { 
                        wasNumber = true;
                    } else if (IsOperator(c) && args[i].Length == 1) {
                        // is a single operator and not a modifier operator ( - as opposed to -4X )
                        wasNumber = false;
                        var operation = c;

                        //Check to see if multiple operators existing concurrently
                        if (i < args.Length - 1 && args[i+1].Length == 1 && IsUnaryOperator(args[i][0]) && IsUnaryOperator(args[i+1][0])) {
                            operation = args[i][0] == args[i + 1][0] ? '+' : '-'; 
                            i++;
                            if (i + 1 < args.Length - 1 && args[i + 1].Length == 1 && IsUnaryOperator(args[i + 1][0])) {
                                operation = operation == args[i + 1][0] ? '+' : '-';
                                i++;
                            }
                        }
                        if (brackets) {
                            bracketEquation.Add(operation);
                        } else {
                            currentSide.Add(operation);
                        }
                    } else if (c == '(') {
                        wasNumber = false;
                        brackets = true;
                        bracketEquation = new Equation(); //renew variable incase of multiple brackets
                    } else if (c == ')') { // close bracket
                        if (i + 1 < args.Length && IsMultiDivide(args[i + 1][0])) {
                            bracketModifier = args[i + 2];
                            bracketOperator = args[i + 1][0];
                            i += 2; // skip the next two arguments as you add them to the bracket
                        }
                        wasNumber = false;
                        brackets = false;
                        currentSide.Add(bracketEquation, bracketModifier, bracketOperator);
                    }
                }
                if (wasNumber) {
                    if (brackets) { 
                        bracketEquation.Add(args[i]);
                    } else if (i < args.Length-1 && args[i + 1][0] == '(') { // next character is an open bracket
                        bracketModifier = args[i];
                    } else {
                        currentSide.Add(args[i]);
                    }
                }
            }
            return new []{ leftSide, currentSide};
        }
        
        private static bool isValidFirstArgument(String s){
            return s.Equals("calc");
        }

        private static bool IsUnaryOperator(char c) {
            return "+-".Contains(c);
        }

        private static bool IsOperator(char c) {
            return "+-*/%".Contains(c);
        }

        private static bool IsMultiDivide(char c) {
            return "*/%".Contains(c);
        }

        private static bool IsNumber(char c) {
            return "0123456789X".Contains(c);
        }

        private static bool IsValidCharacter(char c) {
            return "0123456789+-*/%()=X".Contains(c);
        }
    }
}

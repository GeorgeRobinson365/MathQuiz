

// In C# create program to fit following requirements:  
// Application for primary school children to complete maths questions. Pupils will be presented with maths problem and they must provide answer. Students will be asked 20 questions. 
// First 10 questions basic arithmetic of two numbers with single operator. Make sure division isnt decimal result for first 10 questions. 
// Next 10 questions will be harder using 3 numbers with mix of two operators. Results should be displayed by operator type in first 10 questions and overall for last 10 questions. All questions must be generated randomly. All numbers in questions should be between 1 and 50 inclusive. 
// No question will use two numbers below 10
// Results shown with following criteria : 17 -20 distinction, 0-4 fail, 5-10 pass, 11-16 merit
// Seperate report for teacher showing results created as file named "Resultreport.txt" locally 
// All students should sign in using name
// Time limit of 45min offered, with option for 60min
// First set of 10 questions should feature atleast two of each operator. 
// Should parse text e.g. "The answer is 42" and should check the integer value in that string
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class MathQuiz
{
    static void Main()
    {
        Console.WriteLine("Welcome to George's Math Quiz");

        // Initalise results variables
        int correctAnswersFirst10 = 0;
        int correctAnswersLast10 = 0;
        int[] correctAnswersByOperator = new int[4]; // 0 for +, 1 for -, 2 for *, 3 for /

        //Create random object
        Random random = new Random();

        // Generate a list of operators with at least two of each for the first 10 questions
        List<char> operatorsForFirst10 = new List<char> { '+', '+', '-', '-', '*', '*', '/', '/' };
         // Fill the remaining two with random operators
        operatorsForFirst10.Add(GetRandomOperator(random));
        operatorsForFirst10.Add(GetRandomOperator(random));

        // Shuffle the list to randomize the order of operators
        operatorsForFirst10 = operatorsForFirst10.OrderBy(x => random.Next()).ToList();

        //Prompt for Users name
        Console.Write("Please enter your name: ");
        string studentName = Console.ReadLine();

        // Ask if reasonable adjustments are required
        bool extraTime = ReasonableAdjustment();

        // Set time limit
        int timeLimitMinutes = extraTime ? 60 : 45;
        Stopwatch stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"Your time limit of {timeLimitMinutes} minutes has started!");

        // Check time limit
        if (stopwatch.Elapsed.TotalMinutes > timeLimitMinutes)
        {
            Console.WriteLine("Time's up! Let's see how you did.");
        }
        //IF time has not elapsed then ask questions
        else {

        // First 10 questions: Basic arithmetic with two numbers and one operator
        for (int i = 0; i< 10; i++)
        {
            int num1,num2;
            char operatorType = operatorsForFirst10[i];

            if (operatorType == '/')
            {
                // Ensure no decimal results for division
                num2 = random.Next(1, 11); // Denominator
                int multiplier = random.Next(1, 11); // To ensure the result is a whole number
                num1 = num2 * multiplier; // Numerator
            }
            else
            {
                num1 = random.Next(1, 51);
                num2 = random.Next(1, 51);
            }

            int correctAnswer = CalculateAnswer(num1, num2, operatorType);
            Console.WriteLine($"Question {i+1}: {num1} {operatorType} {num2} = ?");
            int userAnswer = GetValidAnswer("Please enter your answer: ");

            if (userAnswer == correctAnswer)
            {
                correctAnswersFirst10++;
                correctAnswersByOperator[GetOperatorIndex(operatorType)]++;
            }
        }

        // Next 10 questions: Harder questions with three numbers and two operators
        for (int i = 0; i < 10; i++)
        {
            int num1 = random.Next(1, 51);
            int num2 = random.Next(1, 51);
            int num3 = random.Next(1, 51);
            char operator1 = GetRandomOperator(random);
            char operator2 = GetRandomOperator(random);

            int correctAnswer = CalculateAnswer(num1, num2, num3, operator1, operator2);
            Console.WriteLine($"Question {i+11}: {num1} {operator1} {num2} {operator2} {num3} = ?");
            int userAnswer = GetValidAnswer("Please enter your answer: ");

            if (userAnswer == correctAnswer)
            {
                correctAnswersLast10++;
            }
        }
        }

        // Display results, this is outside time check if statement
        Console.WriteLine("\nResults:");
        Console.WriteLine($"Addition Correct: {correctAnswersByOperator[0]}");
        Console.WriteLine($"Subtraction Correct: {correctAnswersByOperator[1]}");
        Console.WriteLine($"Multiplication Correct: {correctAnswersByOperator[2]}");
        Console.WriteLine($"Division Correct: {correctAnswersByOperator[3]}");
        Console.WriteLine($"Overall Correct in Last 10 Questions: {correctAnswersLast10}");
        int overallscore = (correctAnswersByOperator[0] + correctAnswersByOperator[1] + correctAnswersByOperator[2] + correctAnswersByOperator[3] + correctAnswersLast10);
        if(overallscore >16) {
            Console.WriteLine($"Overall score: {overallscore} /20, this is a distinction!"); }
        else if(overallscore >10){
            Console.WriteLine($"Overall score: {overallscore} /20, this is a merit!"); }
        else if(overallscore >4){
            Console.WriteLine($"Overall score: {overallscore}/20, this is a pass!"); }
        else{
            Console.WriteLine($"Overall score: {overallscore}/20, this is a fail!"); }

        // After the quiz is complete and you've calculated the results, write them to a file for teachers
        WriteResultsToFile(studentName, correctAnswersByOperator, correctAnswersLast10, extraTime);

        Console.WriteLine("\nThank you for playing the Math Quiz!");


    }
    //Method to generate random operator
    static char GetRandomOperator(Random random)
    {
        char[] operators = new char[] { '+', '-' , '*' , '/' };
        return operators[random.Next(operators.Length)];
    }


    //Method to calculate answer for simple arithmetic
    static int CalculateAnswer(int num1, int num2, char operatorType)
    {
        return operatorType switch
        {
            '+' => num1 + num2,
            '-' => num1 - num2,
            '*' => num1 * num2,
            '/' => num1 / num2,
            _ => 0,
        };
    }

    //Overloaded method to calculate answer for complex arithmetic questions
    static int CalculateAnswer(int num1, int num2, int num3, char operator1, char operator2)
    {
        int firstResult = CalculateAnswer(num1, num2, operator1);
        return CalculateAnswer(firstResult, num3, operator2);
    }

    //Method mapping each operator to specific index
    static int GetOperatorIndex(char operatorType)
    {
        return operatorType switch
        {
            '+' => 0,
            '-' => 1,
            '*' => 2,
            '/' => 3,
            _ => throw new ArgumentException("Invalid operator") // Handle unexpected operator           
        };
    }

    //Method for asking about reasonable adjustments
    static bool ReasonableAdjustment()
    {
        while (true)
        {
            Console.Write("Do you require reasonable adjustments (Y/N)? ");
            string response = Console.ReadLine().ToUpper();
            if (response == "Y") return true;
            if (response == "N") return false;
            Console.WriteLine("Invalid input. Please enter Y or N.");
        }
    }

    //Method to ensure input answer contains integer else reprompt question
    static int GetValidAnswer(string prompt)
    {
    int? number;
    do
    {
        Console.Write(prompt);
        string userInput = Console.ReadLine();
        number = ExtractNumber(userInput);

        if (!number.HasValue)
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }
    while (!number.HasValue);

    return number.Value;
    
    }

    //Method to extract number from answer string
    static int? ExtractNumber(string input)
    {
    Match match = Regex.Match(input, @"\d+");
    if (match.Success)
    {
        return int.Parse(match.Value);
    }
    return null; // Using null to indicate no valid number found
    }

    //Method to write result to file for teachers
    static void WriteResultsToFile(string studentName, int[] correctAnswersByOperator, int correctAnswersLast10, bool extraTime)
    {
        string path = "Resultreport.txt";
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine($"Student Name: {studentName}");
            sw.WriteLine($"Time Adjustment: {(extraTime ? "60 minutes" : "45 minutes")}");
            sw.WriteLine("Results by Operator Type:");
            sw.WriteLine($"Addition (+) Correct: {correctAnswersByOperator[0]}");
            sw.WriteLine($"Subtraction (-) Correct: {correctAnswersByOperator[1]}");
            sw.WriteLine($"Multiplication (*) Correct: {correctAnswersByOperator[2]}");
            sw.WriteLine($"Division (/) Correct: {correctAnswersByOperator[3]}");
            sw.WriteLine($"Overall Correct in Last 10 Questions: {correctAnswersLast10}");

            int overallscore = (correctAnswersByOperator[0] + correctAnswersByOperator[1] + correctAnswersByOperator[2] + correctAnswersByOperator[3] + correctAnswersLast10);
            if(overallscore >16) {
                sw.WriteLine($"Overall score: {overallscore} /20, this is a distinction!"); }
            else if(overallscore >10){
                sw.WriteLine($"Overall score: {overallscore} /20, this is a merit!"); }
            else if(overallscore >4){
                sw.WriteLine($"Overall score: {overallscore}/20, this is a pass!"); }
            else{
                sw.WriteLine($"Overall score: {overallscore}/20, this is a fail!"); }
        }
    }





}
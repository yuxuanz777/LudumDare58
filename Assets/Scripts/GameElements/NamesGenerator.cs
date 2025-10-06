using UnityEngine;

public static class NameGenerator
{
    // Add a random roman numeral at the end for more fun 

    static readonly string[] A = { "Elden", "Super", "Hollow", "Silent", "Cyber", "Monster", "Stardrew", "God of", "Baba Is", "Mid Seier's" };
    static readonly string[] B = { "Ring", "Mario", "Knight", "Hill", "Punk", "Hunter", "Valley", "War", "You", "Civilization" };
    static readonly string[] C = { "Simulator", "Odessey", "Remastered", "2077", "Deluxe", "Origins", "DLC" };
    static readonly string[] Genres = { "RPG", "Horror", "Puzzle", "Strategy", "Simulation" };
    static readonly string[] RomanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII" };

    public static string FunnyName(){
        string funnyName = $"{A[Random.Range(0, A.Length)]} {B[Random.Range(0, B.Length)]}";
        if (Random.value < 0.5f) // 50% chance to add a roman numeral
            funnyName += $" {RomanNumerals[Random.Range(0, RomanNumerals.Length)]}";
        if (Random.value < 0.3f) // 30% chance to add a C part
            funnyName += $" {C[Random.Range(0, C.Length)]}";
        return funnyName;
    }

    public static string RandomGenre() =>
        Genres[Random.Range(0, Genres.Length)];
}
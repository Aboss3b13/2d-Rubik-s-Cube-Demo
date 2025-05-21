using System;

class Program
{
    static void Main()
    {
        // Initialize faces: W = White, Y = Yellow, O = Orange, R = Red, G = Green, B = Blue
        string[,] U = InitFace("W");
        string[,] D = InitFace("Y");
        string[,] L = InitFace("O");
        string[,] R = InitFace("R");
        string[,] F = InitFace("G");
        string[,] B = InitFace("B");

        // Main loop
        while (true)
        {
            Console.Clear();
            DisplayCube(U, D, L, R, F, B);
            Console.Write("Enter move (R, L, U, D, F, B) or R' for counterclockwise (or Q to quit): ");
            string move = Console.ReadLine().ToUpper();
            if (move == "Q") break;

            bool reverse = move.Contains("'");
            string baseMove = move.Replace("'", "");
            PerformMove(ref U, ref D, ref L, ref R, ref F, ref B, baseMove, reverse);
        }
    }

    static string[,] InitFace(string color)
    {
        return new string[,] {
            { color, color, color },
            { color, color, color },
            { color, color, color }
        };
    }

    // Map a facelet code to a colored block
    static string Colorize(string color)
    {
        return color switch
        {
            "W" => "\u001b[37m■\u001b[0m",       // White
            "Y" => "\u001b[33m■\u001b[0m",       // Yellow
            "O" => "\u001b[38;5;208m■\u001b[0m", // Orange (256-color)
            "R" => "\u001b[31m■\u001b[0m",       // Red
            "G" => "\u001b[32m■\u001b[0m",       // Green
            "B" => "\u001b[34m■\u001b[0m",       // Blue
            _ => color
        };
    }

    static void DisplayCube(string[,] U, string[,] D, string[,] L, string[,] R, string[,] F, string[,] B)
    {
        // Up face (8-space indent)
        Console.WriteLine("        {0} {1} {2}", Colorize(U[0, 0]), Colorize(U[0, 1]), Colorize(U[0, 2]));
        Console.WriteLine("        {0} {1} {2}", Colorize(U[1, 0]), Colorize(U[1, 1]), Colorize(U[1, 2]));
        Console.WriteLine("        {0} {1} {2}", Colorize(U[2, 0]), Colorize(U[2, 1]), Colorize(U[2, 2]));

        // Middle slice: Left, Front, Right, Back
        for (int row = 0; row < 3; row++)
        {
            Console.WriteLine(
                "{0} {1} {2}   {3} {4} {5}   {6} {7} {8}   {9} {10} {11}",
                Colorize(L[row, 0]), Colorize(L[row, 1]), Colorize(L[row, 2]),
                Colorize(F[row, 0]), Colorize(F[row, 1]), Colorize(F[row, 2]),
                Colorize(R[row, 0]), Colorize(R[row, 1]), Colorize(R[row, 2]),
                Colorize(B[row, 0]), Colorize(B[row, 1]), Colorize(B[row, 2])
            );
        }

        // Down face (8-space indent)
        Console.WriteLine("        {0} {1} {2}", Colorize(D[0, 0]), Colorize(D[0, 1]), Colorize(D[0, 2]));
        Console.WriteLine("        {0} {1} {2}", Colorize(D[1, 0]), Colorize(D[1, 1]), Colorize(D[1, 2]));
        Console.WriteLine("        {0} {1} {2}", Colorize(D[2, 0]), Colorize(D[2, 1]), Colorize(D[2, 2]));
    }

    static void RotateFace(ref string[,] face, bool clockwise)
    {
        string[,] temp = (string[,])face.Clone();
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                face[i, j] = clockwise
                    ? temp[2 - j, i]
                    : temp[j, 2 - i];
    }

    static void PerformMove(
        ref string[,] U, ref string[,] D, ref string[,] L,
        ref string[,] R, ref string[,] F, ref string[,] B,
        string move, bool reverse)
    {
        bool cw = !reverse;
        switch (move)
        {
            case "R":
                RotateFace(ref R, cw);
                RotateColumns(ref U, ref F, ref D, ref B, 2, cw, true);
                break;
            case "L":
                RotateFace(ref L, cw);
                RotateColumns(ref U, ref B, ref D, ref F, 0, cw, true);
                break;
            case "U":
                RotateFace(ref U, cw);
                RotateRows(ref B, ref R, ref F, ref L, 0, cw);
                break;
            case "D":
                RotateFace(ref D, cw);
                RotateRows(ref F, ref R, ref B, ref L, 2, cw);
                break;
            case "F":
                RotateFace(ref F, cw);
                RotateColumnsForFace(ref U, ref R, ref D, ref L, cw);
                break;
            case "B":
                RotateFace(ref B, cw);
                RotateColumnsForBack(ref U, ref L, ref D, ref R, cw);
                break;
            default:
                Console.WriteLine("Invalid move.");
                break;
        }
    }

    static void RotateRows(ref string[,] A, ref string[,] B, ref string[,] C, ref string[,] D, int row, bool cw)
    {
        string[] tmp = new string[3];
        for (int i = 0; i < 3; i++) tmp[i] = A[row, i];

        if (cw)
        {
            for (int i = 0; i < 3; i++) A[row, i] = D[row, i];
            for (int i = 0; i < 3; i++) D[row, i] = C[row, i];
            for (int i = 0; i < 3; i++) C[row, i] = B[row, i];
            for (int i = 0; i < 3; i++) B[row, i] = tmp[i];
        }
        else
        {
            for (int i = 0; i < 3; i++) A[row, i] = B[row, i];
            for (int i = 0; i < 3; i++) B[row, i] = C[row, i];
            for (int i = 0; i < 3; i++) C[row, i] = D[row, i];
            for (int i = 0; i < 3; i++) D[row, i] = tmp[i];
        }
    }

    static void RotateColumns(ref string[,] A, ref string[,] B, ref string[,] C, ref string[,] D, int col, bool cw, bool isVertical)
    {
        string[] tmp = new string[3];
        for (int i = 0; i < 3; i++) tmp[i] = A[i, col];

        if (cw)
        {
            for (int i = 0; i < 3; i++) A[i, col] = D[2 - i, col];
            for (int i = 0; i < 3; i++) D[i, col] = C[i, col];
            for (int i = 0; i < 3; i++) C[i, col] = B[i, col];
            for (int i = 0; i < 3; i++) B[i, col] = tmp[2 - i];
        }
        else
        {
            for (int i = 0; i < 3; i++) A[i, col] = B[i, col];
            for (int i = 0; i < 3; i++) B[i, col] = C[i, col];
            for (int i = 0; i < 3; i++) C[i, col] = D[2 - i, col];
            for (int i = 0; i < 3; i++) D[i, col] = tmp[2 - i];
        }
    }

    static void RotateColumnsForFace(ref string[,] U, ref string[,] R, ref string[,] D, ref string[,] L, bool cw)
    {
        string[] tmp = new string[3];
        for (int i = 0; i < 3; i++) tmp[i] = U[2, i];

        if (cw)
        {
            for (int i = 0; i < 3; i++) U[2, i] = L[2 - i, 2];
            for (int i = 0; i < 3; i++) L[i, 2] = D[0, i];
            for (int i = 0; i < 3; i++) D[0, i] = R[2 - i, 0];
            for (int i = 0; i < 3; i++) R[i, 0] = tmp[i];
        }
        else
        {
            for (int i = 0; i < 3; i++) U[2, i] = R[i, 0];
            for (int i = 0; i < 3; i++) R[i, 0] = D[0, 2 - i];
            for (int i = 0; i < 3; i++) D[0, i] = L[i, 2];
            for (int i = 0; i < 3; i++) L[i, 2] = tmp[2 - i];
        }
    }

    static void RotateColumnsForBack(ref string[,] U, ref string[,] L, ref string[,] D, ref string[,] R, bool cw)
    {
        string[] tmp = new string[3];
        for (int i = 0; i < 3; i++) tmp[i] = U[0, i];

        if (cw)
        {
            for (int i = 0; i < 3; i++) U[0, i] = R[i, 2];
            for (int i = 0; i < 3; i++) R[i, 2] = D[2, 2 - i];
            for (int i = 0; i < 3; i++) D[2, i] = L[i, 0];
            for (int i = 0; i < 3; i++) L[i, 0] = tmp[2 - i];
        }
        else
        {
            for (int i = 0; i < 3; i++) U[0, i] = L[2 - i, 0];
            for (int i = 0; i < 3; i++) L[i, 0] = D[2, i];
            for (int i = 0; i < 3; i++) D[2, i] = R[2 - i, 2];
            for (int i = 0; i < 3; i++) R[i, 2] = tmp[i];
        }
    }
}
   
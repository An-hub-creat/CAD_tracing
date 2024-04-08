using System;
using System.Collections.Generic;

namespace CADTracing
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] field = InitializeField(20, 20); // Initialize a 20x20 field
            MarkBarrier(field, 5, 10); // Mark a barrier at position (5, 10)
            MarkBarrier(field, 15, 10); // Mark a barrier at position (15, 10)

            Console.WriteLine("Wave propagation with 11 consecutive digits:");
            PropagateWave(field, 0, 0, 11);
            PrintField(field);

            ResetField(field); // Reset field for the next propagation

            Console.WriteLine("Wave propagation with 22 consecutive digits:");
            PropagateWave(field, 0, 0, 22);
            PrintField(field);

            Console.WriteLine("Backtracing the path:");
            List<Tuple<int, int>> path = BacktraceWave(field, 19, 19);
            PrintPath(field, path);
        }

        static int[,] InitializeField(int rows, int cols)
        {
            return new int[rows, cols];
        }

        static void MarkBarrier(int[,] field, int row, int col)
        {
            if (row < 0 || row >= field.GetLength(0) || col < 0 || col >= field.GetLength(1))
            {
                Console.WriteLine("Invalid barrier position.");
                return;
            }
            field[row, col] = -1;
        }

        static void PropagateWave(int[,] field, int startRow, int startCol, int consecutiveDigits)
        {
            if (startRow < 0 || startRow >= field.GetLength(0) || startCol < 0 || startCol >= field.GetLength(1))
            {
                Console.WriteLine("Invalid start position.");
                return;
            }

            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            Dictionary<Tuple<int, int>, int> waveLevels = new Dictionary<Tuple<int, int>, int>();

            queue.Enqueue(new Tuple<int, int>(startRow, startCol));
            waveLevels.Add(new Tuple<int, int>(startRow, startCol), 1);

            while (queue.Count > 0 && waveLevels.Count < consecutiveDigits)
            {
                Tuple<int, int> current = queue.Dequeue();
                int row = current.Item1;
                int col = current.Item2;

                if (row < 0 || row >= field.GetLength(0) || col < 0 || col >= field.GetLength(1) || field[row, col] == -1)
                {
                    continue; // Skip if out of bounds or barrier
                }

                int currentLevel = waveLevels[current];
                field[row, col] = currentLevel;

                // Enqueue neighboring cells
                EnqueueNeighbor(queue, waveLevels, new Tuple<int, int>(row + 1, col), currentLevel + 1);
                EnqueueNeighbor(queue, waveLevels, new Tuple<int, int>(row - 1, col), currentLevel + 1);
                EnqueueNeighbor(queue, waveLevels, new Tuple<int, int>(row, col + 1), currentLevel + 1);
                EnqueueNeighbor(queue, waveLevels, new Tuple<int, int>(row, col - 1), currentLevel + 1);
            }
        }

        static void EnqueueNeighbor(Queue<Tuple<int, int>> queue, Dictionary<Tuple<int, int>, int> waveLevels, Tuple<int, int> neighbor, int level)
        {
            if (neighbor.Item1 >= 0 && neighbor.Item1 < 20 && neighbor.Item2 >= 0 && neighbor.Item2 < 20 && !waveLevels.ContainsKey(neighbor))
            {
                queue.Enqueue(neighbor);
                waveLevels.Add(neighbor, level);
            }
        }

        static List<Tuple<int, int>> BacktraceWave(int[,] field, int endRow, int endCol)
        {
            List<Tuple<int, int>> path = new List<Tuple<int, int>>();
            Tuple<int, int> current = new Tuple<int, int>(endRow, endCol);
            int currentLevel = field[endRow, endCol];

            if (currentLevel == 0) return path; // No path found

            while (currentLevel > 1)
            {
                path.Add(current);

                // Find the neighboring cell with the previous level
                current = FindPreviousLevelCell(field, current, currentLevel - 1);

                if (current == null)
                {
                    break; // Path not found
                }

                currentLevel--;
            }

            path.Reverse(); // Reverse the path to start from the source
            return path;
        }

        static Tuple<int, int> FindPreviousLevelCell(int[,] field, Tuple<int, int> current, int level)
        {
            int row = current.Item1;
            int col = current.Item2;

            if (row > 0 && field[row - 1, col] == level) return new Tuple<int, int>(row - 1, col);
            if (row < field.GetLength(0) - 1 && field[row + 1, col] == level) return new Tuple<int, int>(row + 1, col);
            if (col > 0 && field[row, col - 1] == level) return new Tuple<int, int>(row, col - 1);
            if (col < field.GetLength(1) - 1 && field[row, col + 1] == level) return new Tuple<int, int>(row, col + 1);

            return null;
        }

        static void ResetField(int[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] > 0)
                    {
                        field[i, j] = 0;
                    }
                }
            }
        }

        static void PrintField(int[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == -1)
                    {
                        Console.Write("X ");
                    }
                    else if (field[i, j] == 0)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write($"{field[i, j]} ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void PrintPath(int[,] field, List<Tuple<int, int>> path)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == -1)
                    {
                        Console.Write("X ");
                    }
                    else if (path.Contains(new Tuple<int, int>(i, j)))
                    {
                        Console.Write("* ");
                    }
                    else if (field[i, j] == 0)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write($"{field[i, j]} ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}

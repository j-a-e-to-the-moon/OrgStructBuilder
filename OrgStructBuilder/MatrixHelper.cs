using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrgStructBuilder
{
    internal class MatrixHelper
    {
        // 주대각성분을 1로 채움
        public static void FillDiagonal(double[,] matrix)
        {
            int size = matrix.GetLength(0);
            for (int i = 0; i < size; i++) matrix[i, i] = 1.0;
        }

        // 행렬 곱셈 (C = A * B)
        public static double[,] Multiply(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            double[,] result = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        result[i, j] += A[i, k] * B[k, j];
            return result;
        }

        // 깊은 복사
        public static double[,] DeepCopy(double[,] matrix)
        {
            return (double[,])matrix.Clone();
        }

        // 두 행렬의 차이가 Epsilon 이내인지 확인 (영행렬 확인)
        public static bool IsConverged(double[,] A, double[,] B, double epsilon = 1e-9)
        {
            int n = A.GetLength(0);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (Math.Abs(A[i, j] - B[i, j]) > epsilon) return false;
            return true;
        }
    }
}

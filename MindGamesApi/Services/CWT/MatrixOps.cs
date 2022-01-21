using System;
using System.Collections.Generic;

//disabled as this is converted code that I do not want to maintain and has always worked
#pragma warning disable

namespace MindGamesApi.Services.CWT;

/// <summary>
///     Class responsibility: Provide methods for basic vector and matrix
///     computations.
///     @author max
/// </summary>
public class MatrixOps
{
    // /////////////////// add ///////////////////////

    public static ComplexNumber?[][] add(ComplexNumber[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.add(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] add(ComplexNumber[][] A, double[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.add(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] add(double[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.add(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static double[][] add(double[][] A, double[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] C = new double[m][n];
        var C = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = A[i][j] + B[i][j];
            }
        }

        return C;
    }

    public static ComplexNumber[] add(ComplexNumber[] v1, ComplexNumber[] v2)
    {
        var n = v1.Length;
        var v1Pv2 = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            v1Pv2[i] = ComplexCalc.add(v1[i], v2[i]);
        }

        return v1Pv2;
    }

    public static ComplexNumber[] add(double[] v1, ComplexNumber[] v2)
    {
        var n = v1.Length;
        var v1Pv2 = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            v1Pv2[i] = ComplexCalc.add(v1[i], v2[i]);
        }

        return v1Pv2;
    }

    public static ComplexNumber[] add(ComplexNumber[] v1, double[] v2)
    {
        var n = v1.Length;
        var v1Pv2 = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            v1Pv2[i] = ComplexCalc.add(v1[i], v2[i]);
        }

        return v1Pv2;
    }

    public static double[] add(double[] v1, double[] v2)
    {
        var n = v1.Length;
        var v1Pv2 = new double[n];

        for (var i = 0; i < n; i++)
        {
            v1Pv2[i] = v1[i] + v2[i];
        }

        return v1Pv2;
    }

    public static double[] backCalculateX(double[][] upperTriangularMatrix, double[] b)
    {
        var n = upperTriangularMatrix[0].Length;
        b[n - 1] = b[n - 1] / upperTriangularMatrix[n - 1][n - 1];

        for (var i = n - 2; i >= 0; i += -1)
        {
            double temp = 0;

            for (var j = n - 1; j >= i + 1; j += -1)
            {
                temp = temp - upperTriangularMatrix[i][j] * b[j];
            }

            temp = temp + b[i];
            b[i] = temp / upperTriangularMatrix[i][i];
        }

        return b;
    }

    public static ComplexNumber[] backCalculateX(double[][] upperTriangularMatrix, ComplexNumber[] b)
    {
        var n = upperTriangularMatrix[0].Length;
        b[n - 1] = ComplexCalc.divide(b[n - 1], upperTriangularMatrix[n - 1][n - 1]);

        for (var i = n - 2; i >= 0; i += -1)
        {
            var temp = new ComplexNumber();

            for (var j = n - 1; j >= i + 1; j += -1)
            {
                temp = ComplexCalc.subtract(temp, ComplexCalc.multiply(upperTriangularMatrix[i][j], b[j]));
            }

            temp = ComplexCalc.add(temp, b[i]);
            b[i] = ComplexCalc.divide(temp, upperTriangularMatrix[i][i]);
        }

        return b;
    }

    public static ComplexNumber?[] backCalculateX(ComplexNumber[][] upperTriangularMatrix, double[] b)
    {
        var n = upperTriangularMatrix[0].Length;
        var cB = toComplexNumber(b);
        cB[n - 1] = ComplexCalc.divide(cB[n - 1], upperTriangularMatrix[n - 1][n - 1]);

        for (var i = n - 2; i >= 0; i += -1)
        {
            var temp = new ComplexNumber();

            for (var j = n - 1; j >= i + 1; j += -1)
            {
                temp = ComplexCalc.subtract(temp, ComplexCalc.multiply(upperTriangularMatrix[i][j], cB[j]));
            }

            temp = ComplexCalc.add(temp, cB[i]);
            cB[i] = ComplexCalc.divide(temp, upperTriangularMatrix[i][i]);
        }

        return cB;
    }

    public static ComplexNumber[] backCalculateX(ComplexNumber[][] upperTriangularMatrix, ComplexNumber[] b)
    {
        var n = upperTriangularMatrix[0].Length;
        b[n - 1] = ComplexCalc.divide(b[n - 1], upperTriangularMatrix[n - 1][n - 1]);

        for (var i = n - 2; i >= 0; i += -1)
        {
            var temp = new ComplexNumber();

            for (var j = n - 1; j >= i + 1; j += -1)
            {
                temp = ComplexCalc.subtract(temp, ComplexCalc.multiply(upperTriangularMatrix[i][j], b[j]));
            }

            temp = ComplexCalc.add(temp, b[i]);
            b[i] = ComplexCalc.divide(temp, upperTriangularMatrix[i][i]);
        }

        return b;
    }

    public static ComplexNumber[] conjugate(ComplexNumber[] v)
    {
        var n = v.Length;
        var conj = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            conj[i] = ComplexCalc.conjugate(v[i]);
        }

        return conj;
    }

    public static ComplexNumber[][] conjugate(ComplexNumber[][] A)
    {
        var m = A.Length;
        var conj = new ComplexNumber[m][];

        for (var i = 0; i < m; i++)
        {
            var n = A[i].Length;
            conj[i] = new ComplexNumber[n];

            for (var j = 0; j < n; j++)
            {
                conj[i][j] = ComplexCalc.conjugate(A[i][j]);
            }
        }

        return conj;
    }

    public static ComplexNumber?[][] conjugateTranspose(ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] conjT = new ComplexNumber[n][m];
        var conjT = RectangularArrays.RectangularComplexNumberArray(n, m);

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                conjT[i][j] = ComplexCalc.conjugate(A[j][i]);
            }
        }

        return conjT;
    }

    public static double[][] createVandermonde(double[] x, int order)
    {
        var m = x.Length;
        var n = order;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] V = new double[m][n + 1];
        var V = RectangularArrays.RectangularDoubleArray(m, n + 1);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j <= n; j++)
            {
                V[i][j] = Math.Pow(x[i], j);
            }
        }

        return V;
    }

    public static ComplexNumber?[][] createVandermonde(ComplexNumber[] x, int order)
    {
        var m = x.Length;
        var n = order;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] V = new ComplexNumber[m][n + 1];
        var V = RectangularArrays.RectangularComplexNumberArray(m, n + 1);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j <= n; j++)
            {
                V[i][j] = ComplexCalc.pow(x[i], j);
            }
        }

        return V;
    }

    public static double[] deepCopy(double[] v)
    {
        var n = v.Length;
        var copy = new double[n];

        for (var j = 0; j < n; j++)
        {
            copy[j] = v[j];
        }

        return copy;
    }

    public static double[][] deepCopy(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] copy = new double[m][n];
        var copy = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                copy[i][j] = A[i][j];
            }
        }

        return copy;
    }

    public static ComplexNumber[] deepCopy(ComplexNumber[] v)
    {
        var n = v.Length;
        var copy = new ComplexNumber[n];

        for (var j = 0; j < n; j++)
        {
            copy[j] = v[j];
        }

        return copy;
    }

    public static ComplexNumber?[][] deepCopy(ComplexNumber?[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] copy = new ComplexNumber[m][n];
        var copy = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                copy[i][j] = A[i][j];
            }
        }

        return copy;
    }

    public static List<double[]> deepCopy(List<double[]> A)
    {
        var copy = new List<double[]>();

        foreach (var vector in A)
        {
            var len = vector.Length;
            var cVector = new double[len];

            for (var i = 0; i < len; i++)
            {
                cVector[i] = vector[i];
            }

            copy.Add(cVector);
        }

        return copy;
    }

    /// <param name="n"> </param>
    /// <returns> Identity matrix I[n,n] </returns>
    public static double[][] eye(int n)
    {
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] eye = new double[n][n];
        var eye = RectangularArrays.RectangularDoubleArray(n, n);

        for (var i = 0; i < n; i++)
        {
            eye[i][i] = 1.0;
        }

        return eye;
    }

    /// <param name="n"> </param>
    /// <returns> Identity matrix I[n,n] </returns>
    public static ComplexNumber?[][] eyeComplexNumber(int n)
    {
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] eye = new ComplexNumber[n][n];
        var eye = RectangularArrays.RectangularComplexNumberArray(n, n);
        initialize(eye);

        for (var i = 0; i < n; i++)
        {
            //eye[i][i].Real = 1.0;
            eye[i][i] = new ComplexNumber(1.0, eye[i][i].Imaginary);
        }

        return eye;
    }

    public static ComplexNumber[] getColumnAsVector(ComplexNumber[][] A, int colIndex)
    {
        var m = A.Length;
        var col = new ComplexNumber[m];

        for (var i = 0; i < m; i++)
        {
            col[i] = A[i][colIndex];
        }

        return col;
    }

    public static double[] getColumnAsVector(double[][] A, int colIndex)
    {
        var m = A.Length;
        var col = new double[m];

        for (var i = 0; i < m; i++)
        {
            col[i] = A[i][colIndex];
        }

        return col;
    }

    public static double[][] getImaginaryPart(ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] realPart = new double[m][n];
        var realPart = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                realPart[i][j] = A[i][j].Imaginary;
            }
        }

        return realPart;
    }

    public static double[] getImaginaryPart(ComplexNumber[] v)
    {
        var m = v.Length;

        var realPart = new double[m];

        for (var i = 0; i < m; i++)
        {
            realPart[i] = v[i].Imaginary;
        }

        return realPart;
    }

    public static double[][] getRealPart(ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] realPart = new double[m][n];
        var realPart = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                realPart[i][j] = A[i][j].Real;
            }
        }

        return realPart;
    }

    public static double[] getRealPart(ComplexNumber[] v)
    {
        var m = v.Length;

        var realPart = new double[m];

        for (var i = 0; i < m; i++)
        {
            realPart[i] = v[i].Real;
        }

        return realPart;
    }

    public static ComplexNumber[] getRowAsVector(ComplexNumber[][] A, int rowIndex)
    {
        var n = A[0].Length;
        Console.Write(A.Length + "\n");
        Console.Write(A[0].Length + "\n");
        var col = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            col[i] = A[rowIndex][i];
        }

        return col;
    }

    public static double[] getRowAsVector(double[][] A, int rowIndex)
    {
        var n = A[0].Length;
        Console.Write(A.Length + "\n");
        Console.Write(A[0].Length + "\n");
        var col = new double[n];

        for (var i = 0; i < n; i++)
        {
            col[i] = A[rowIndex][i];
        }

        return col;
    }

    // ************************* Hadamand product ************************

    public static ComplexNumber?[][] Hadamard(ComplexNumber[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] Hadamard(ComplexNumber[][] A, double[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] Hadamard(double[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(A[i][j], B[i][j]);
            }
        }

        return C;
    }

    public static double[][] Hadamard(double[][] A, double[][] B)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] C = new double[m][n];
        var C = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = A[i][j] * B[i][j];
            }
        }

        return C;
    }

    public static ComplexNumber[] Hadamard(double[] A, ComplexNumber[] B)
    {
        var m = A.Length;
        var C = new ComplexNumber[m];

        for (var i = 0; i < m; i++)
        {
            C[i] = ComplexCalc.multiply(A[i], B[i]);
        }

        return C;
    }

    public static ComplexNumber[] Hadamard(ComplexNumber[] A, double[] B)
    {
        var m = A.Length;
        var C = new ComplexNumber[m];

        for (var i = 0; i < m; i++)
        {
            C[i] = ComplexCalc.multiply(A[i], B[i]);
        }

        return C;
    }

    public static ComplexNumber[] Hadamard(ComplexNumber[] A, ComplexNumber[] B)
    {
        var m = A.Length;
        var C = new ComplexNumber[m];

        for (var i = 0; i < m; i++)
        {
            C[i] = ComplexCalc.multiply(A[i], B[i]);
        }

        return C;
    }

    public static double[] Hadamard(double[] A, double[] B)
    {
        var m = A.Length;
        var C = new double[m];

        for (var i = 0; i < m; i++)
        {
            C[i] = A[i] * B[i];
        }

        return C;
    }

    ////////////Array Management////////////////

    /// <summary>
    ///     When a ComplexNumber[] is created (new ComplexNumber[n]()), its elements
    ///     are null. If a function includes an assignment where an null element
    ///     appears on the right-hand side of the assignment (e.g. c = c + a*b) prior
    ///     to instance creation an error will be thrown. This method initializes the
    ///     array with zeros and prevents the error.
    /// </summary>
    public static ComplexNumber?[] initialize(ComplexNumber?[] v)
    {
        var n = v.Length;

        for (var i = 0; i < n; i++)
        {
            v[i] = new ComplexNumber();
        }

        return v;
    }

    /// <summary>
    ///     When a ComplexNumber[][] is created (new ComplexNumber[m][n]()), its
    ///     elements are null. If a function includes an assignment where a null
    ///     element appears on the right-hand side of the assignment (e.g. c = c +
    ///     a*b) prior to instance creation an error will be thrown. This method
    ///     initializes the array and prevents the error.
    /// </summary>
    public static ComplexNumber?[][] initialize(ComplexNumber?[][] A)
    {
        var m = A.Length;

        for (var i = 0; i < m; i++)
        {
            A[i] = initialize(A[i]);
        }

        return A;
    }

    /// <param name="real">
    ///     a vector of real numbers that may be a fixed-stride
    ///     representation of a matrix
    /// </param>
    /// <returns> a CUDA compatible ComplexNumber interleaved </returns>
    public static ComplexNumber[] interleavedRealToComplexNumber(float[] real)
    {
        var len = real.Length;
        var result = new ComplexNumber[len / 2];

        for (var i = 0; i < len; i += 2)
        {
            result[i / 2] = new ComplexNumber(real[i], real[i + 1]);
            //result[i / 2].Real = real[i];
            //result[i / 2].Imaginary = real[i + 1];
        }

        return result;
    }

    /// <returns>
    ///     gives an upper bound on the relative error due to rounding in
    ///     floating point arithmetic
    /// </returns>
    public static double MachineEpsilonDouble()
    {
        var eps = 1.0;

        do
        {
            eps /= 2.0;
        }
        while (1.0 + eps / 2.0 != 1.0);

        return eps;
    }

    public static double matrixFNorm(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        double norm = 0;

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                norm += Math.Pow(A[i][j], 2);
            }
        }

        norm = Math.Pow(norm, 0.5);

        return norm;
    }

    /// <param name="v">
    ///     vector v[n] as double[]
    /// </param>
    /// <returns> matrix F norm of v </returns>
    public static double matrixFNorm(ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        double norm = 0;

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                norm += Math.Pow(ComplexCalc.modulus(A[i][j]), 2);
            }
        }

        norm = Math.Pow(norm, 0.5);

        return norm;
    }

    // min / max

    public static double max(List<double[]> vectorList)
    {
        var max = double.NegativeInfinity;

        for (var i = 0; i < vectorList.Count; i++)
        {
            for (var j = 0; j < vectorList[i].Length; j++)
            {
                if (vectorList[i][j] > max)
                {
                    max = vectorList[i][j];
                }
            }
        }

        return max;
    }

    public static double max(double[] A)
    {
        var n = A.Length;
        var max = double.NegativeInfinity;

        for (var i = 0; i < n; i++)
        {
            if (A[i] > max)
            {
                max = A[i];
            }
        }

        return max;
    }

    public static double max(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        var max = double.NegativeInfinity;

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (A[i][j] > max)
                {
                    max = A[i][j];
                }
            }
        }

        return max;
    }

    public static int max(int[] vector)
    {
        var max = vector[0];

        for (var i = 1; i < vector.Length; i++)
        {
            if (vector[i] > max)
            {
                max = vector[i];
            }
        }

        return max;
    }

    /// <param name="plots">
    ///     p sets of xy data as a double[p][index][] array where, for p
    ///     sets, plots[0:p-1][0] = x and plots[0:p-1][1] = y
    /// </param>
    /// <param name="index">
    ///     0 for x and 1 for y
    /// </param>
    /// <returns> maximum of all x or y </returns>
    public static double max(double[][][] plots, int index)
    {
        var p = plots.Length;
        var max = double.NegativeInfinity;

        for (var i = 0; i < p; i++)
        {
            var val = plots[i][index];
            var temp = MatrixOps.max(val);

            if (temp > max)
            {
                max = temp;
            }
        }

        return max;
    }

    public static double min(List<double[]> vectorList)
    {
        var min = double.PositiveInfinity;

        for (var i = 0; i < vectorList.Count; i++)
        {
            for (var j = 0; j < vectorList[i].Length; j++)
            {
                if (vectorList[i][j] < min)
                {
                    min = vectorList[i][j];
                }
            }
        }

        return min;
    }

    public static double min(double[] A)
    {
        var n = A.Length;
        var min = double.PositiveInfinity;

        for (var i = 0; i < n; i++)
        {
            if (A[i] < min)
            {
                min = A[i];
            }
        }

        return min;
    }

    public static double min(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        var min = double.PositiveInfinity;

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (A[i][j] < min)
                {
                    min = A[i][j];
                }
            }
        }

        return min;
    }

    public static int min(int[] vector)
    {
        var min = vector[0];

        for (var i = 1; i < vector.Length; i++)
        {
            if (vector[i] < min)
            {
                min = vector[i];
            }
        }

        return min;
    }

    /// <param name="plots">
    ///     p sets of xy data as a double[p][index][] array where, for p
    ///     sets, plots[0:p-1][0] = x and plots[0:p-1][1] = y
    /// </param>
    /// <param name="index">
    ///     0 for x and 1 for y
    /// </param>
    /// <returns> minimum of all x or y </returns>
    public static double min(double[][][] plots, int index)
    {
        var p = plots.Length;
        var min = double.PositiveInfinity;

        for (var i = 0; i < p; i++)
        {
            var val = plots[i][index];
            var temp = MatrixOps.min(val);

            if (temp < min)
            {
                min = temp;
            }
        }

        return min;
    }

    //Operations

    public static double[][] modulus(ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] absA = new double[m][n];
        var absA = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                absA[i][j] = ComplexCalc.modulus(A[i][j]);
            }
        }

        return absA;
    }

    public static double[] modulus(ComplexNumber[] v)
    {
        var n = v.Length;
        var absV = new double[n];

        for (var i = 0; i < n; i++)
        {
            absV[i] = ComplexCalc.modulus(v[i]);
        }

        return absV;
    }

    // /////////////////// multiply ///////////////////////

    public static ComplexNumber?[][] multiply(ComplexNumber[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var p = A[0].Length;
        var n = B[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);
        initialize(C);

        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < p; k++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = ComplexCalc.add(C[i][j], ComplexCalc.multiply(A[i][k], B[k][j]));
                }
            }
        }

        return C;
    }

    public static ComplexNumber?[][] multiply(double[][] A, ComplexNumber[][] B)
    {
        var m = A.Length;
        var p = A[0].Length;
        var n = B[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);
        initialize(C);

        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < p; k++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = ComplexCalc.add(C[i][j], ComplexCalc.multiply(A[i][k], B[k][j]));
                }
            }
        }

        return C;
    }

    public static ComplexNumber?[][] multiply(ComplexNumber[][] A, double[][] B)
    {
        var m = A.Length;
        var p = A[0].Length;
        var n = B[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);
        initialize(C);

        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < p; k++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = ComplexCalc.add(C[i][j], ComplexCalc.multiply(A[i][k], B[k][j]));
                }
            }
        }

        return C;
    }

    public static double[][] multiply(double[][] A, double[][] B)
    {
        var m = A.Length;
        var p = A[0].Length;
        var n = B[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] C = new double[m][n];
        var C = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var k = 0; k < p; k++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = C[i][j] + A[i][k] * B[k][j];
                }
            }
        }

        return C;
    }

    public static double[] multiply(double[][] A, double[] b)
    {
        var m = A.Length;
        var n = A[0].Length;
        var v = new double[m];

        for (var k = 0; k < n; k++)
        {
            for (var i = 0; i < m; i++)
            {
                v[i] += A[i][k] * b[k];
            }
        }

        return v;
    }

    public static ComplexNumber?[] multiply(ComplexNumber[][] A, double[] b)
    {
        var m = A.Length;
        var n = A[0].Length;
        var v = new ComplexNumber?[m];
        initialize(v);

        for (var k = 0; k < n; k++)
        {
            for (var i = 0; i < m; i++)
            {
                v[i] = ComplexCalc.add(v[i], ComplexCalc.multiply(A[i][k], b[k]));
            }
        }

        return v;
    }

    public static ComplexNumber?[] multiply(double[][] A, ComplexNumber[] b)
    {
        var m = A.Length;
        var n = A[0].Length;
        var v = new ComplexNumber?[m];
        initialize(v);

        for (var k = 0; k < n; k++)
        {
            for (var i = 0; i < m; i++)
            {
                v[i] = ComplexCalc.add(v[i], ComplexCalc.multiply(A[i][k], b[k]));
            }
        }

        return v;
    }

    public static ComplexNumber?[] multiply(ComplexNumber[][] A, ComplexNumber[] b)
    {
        var m = A.Length;
        var n = A[0].Length;
        var v = new ComplexNumber?[m];
        initialize(v);

        for (var k = 0; k < n; k++)
        {
            for (var i = 0; i < m; i++)
            {
                v[i] = ComplexCalc.add(v[i], ComplexCalc.multiply(A[i][k], b[k]));
            }
        }

        return v;
    }

    /// <param name="x">
    ///     the sequence to pad
    /// </param>
    /// <returns>
    ///     If necessary, expanded sequence such that its length is an even
    ///     power of 2 by adding additional zero values.
    /// </returns>
    public static double[] padPow2(double[] x)
    {
        var sizeIn = x.Length;
        var log2N = Math.Log(sizeIn) / Math.Log(2);
        var ceiling = Math.Ceiling(log2N);

        if (log2N < ceiling)
        {
            log2N = ceiling;
            var sizePad = (int)Math.Pow(2, log2N);
            var padX = new double[sizePad];

            for (var i = 0; i < sizePad; i++)
            {
                if (i < sizeIn)
                {
                    padX[i] = x[i];
                }
                else
                {
                    padX[i] = 0;
                }
            }

            return padX;
        }

        return x;
    }

    public static double[][] padPow2(double[][] xy)
    {
        var sizeIn = xy[0].Length;
        var log2N = Math.Log(sizeIn) / Math.Log(2);
        var ceiling = Math.Ceiling(log2N);

        if (log2N < ceiling)
        {
            log2N = ceiling;
            var sizePad = (int)Math.Pow(2, log2N);
            double[][] padXY = { new double[sizePad], new double[sizePad] };
            var dx = padXY[0][1] - padXY[0][0];

            for (var i = 0; i < sizePad; i++)
            {
                if (i < sizeIn)
                {
                    padXY[0][i] = xy[0][i];
                    padXY[1][i] = xy[1][i];
                }
                else
                {
                    padXY[0][i] = padXY[0][i - 1] + dx;
                    padXY[1][i] = 0;
                }
            }

            return padXY;
        }

        return xy;
    }

    // /////////////////// scale ///////////////////////

    public static ComplexNumber?[][] scale(ComplexNumber alpha, ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] alphaA = new ComplexNumber[m][n];
        var alphaA = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                alphaA[i][j] = ComplexCalc.multiply(alpha, A[i][j]);
            }
        }

        return alphaA;
    }

    public static ComplexNumber?[][] scale(double alpha, ComplexNumber[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] alphaA = new ComplexNumber[m][n];
        var alphaA = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                alphaA[i][j] = ComplexCalc.multiply(alpha, A[i][j]);
            }
        }

        return alphaA;
    }

    public static ComplexNumber?[][] scale(ComplexNumber alpha, double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] alphaA = new ComplexNumber[m][n];
        var alphaA = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                alphaA[i][j] = ComplexCalc.multiply(alpha, A[i][j]);
            }
        }

        return alphaA;
    }

    public static double[][] scale(double alpha, double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] alphaA = new double[m][n];
        var alphaA = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                alphaA[i][j] = alpha * A[i][j];
            }
        }

        return alphaA;
    }

    public static ComplexNumber[] scale(ComplexNumber alpha, ComplexNumber[] v)
    {
        var n = v.Length;
        var alphaV = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            alphaV[i] = ComplexCalc.multiply(alpha, v[i]);
        }

        return alphaV;
    }

    public static ComplexNumber[] scale(double alpha, ComplexNumber[] v)
    {
        var n = v.Length;
        var alphaV = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            alphaV[i] = ComplexCalc.multiply(alpha, v[i]);
        }

        return alphaV;
    }

    public static ComplexNumber[] scale(ComplexNumber alpha, double[] v)
    {
        var n = v.Length;
        var alphaV = new ComplexNumber[n];

        for (var i = 0; i < n; i++)
        {
            alphaV[i] = ComplexCalc.multiply(alpha, v[i]);
        }

        return alphaV;
    }

    public static double[] scale(double alpha, double[] v)
    {
        var n = v.Length;
        var alphaV = new double[n];

        for (var i = 0; i < n; i++)
        {
            alphaV[i] = alpha * v[i];
        }

        return alphaV;
    }

    /// <param name="A"> A[rows][columns] </param>
    /// <returns> ArrayList of doubles[] such that each double[] is a row from A </returns>
    public static List<double[]> toArraylist(double[][] A)
    {
        var listRows = new List<double[]>();

        for (var i = 0; i < A.Length; i++)
        {
            listRows.Add(A[i]);
        }

        return listRows;
    }

    public static ComplexNumber?[][] toComplexNumber(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] cA = new ComplexNumber[m][n];
        var cA = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                cA[i][j] = new ComplexNumber(A[i][j], 0);
            }
        }

        return cA;
    }

    public static ComplexNumber?[] toComplexNumber(double[] v)
    {
        var n = v.Length;
        var cV = new ComplexNumber?[n];

        for (var i = 0; i < n; i++)
        {
            cV[i] = new ComplexNumber(v[i], 0);
        }

        return cV;
    }

    public static double[][] toDouble(List<double[]> columnVectors)
    {
        var m = columnVectors[0].Length;
        var n = columnVectors.Count;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] matrix = new double[m][n];
        var matrix = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                matrix[i][j] = columnVectors[j][i];
            }
        }

        return matrix;
    }

    // ////////////////// Transposition ////////////

    public static ComplexNumber?[][] transpose(ComplexNumber?[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] AT = new ComplexNumber[n][m];
        var AT = RectangularArrays.RectangularComplexNumberArray(n, m);

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                AT[i][j] = A[j][i];
            }
        }

        return AT;
    }

    public static double[][] transpose(double[][] A)
    {
        var m = A.Length;
        var n = A[0].Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] AT = new double[n][m];
        var AT = RectangularArrays.RectangularDoubleArray(n, m);

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                AT[i][j] = A[j][i];
            }
        }

        return AT;
    }

    // //// norms

    public static double vector2Norm(double[] v)
    {
        var n = v.Length;
        double norm = 0;

        for (var i = 0; i < n; i++)
        {
            norm += Math.Pow(v[i], 2);
        }

        norm = Math.Pow(norm, 0.5);

        return norm;
    }

    public static double vector2Norm(ComplexNumber[] v)
    {
        var n = v.Length;
        double norm = 0;

        for (var i = 0; i < n; i++)
        {
            norm += Math.Pow(ComplexCalc.modulus(v[i]), 2);
        }

        norm = Math.Pow(norm, 0.5);

        return norm;
    }

    /// <param name="v">
    ///     vector v[n] as double[]
    /// </param>
    /// <returns> InfinityNorm(v) ... absolute value of highest magnitude element </returns>
    public static double vectorInfinityNorm(double[] v)
    {
        double lFinity = 0;
        var n = v.Length;
        lFinity = Math.Abs(v[0]);

        for (var i = 1; i < n; i++)
        {
            var abs = Math.Abs(v[i]);

            if (abs > lFinity)
            {
                lFinity = abs;
            }
        }

        return lFinity;
    }

    public static double vectorInfinityNorm(ComplexNumber[] vector)
    {
        // Linfty
        // Norm
        var n = vector.Length;
        double max = 0;

        for (var i = 0; i < n; i++)
        {
            var mod = ComplexCalc.modulus(vector[i]);

            if (mod > max)
            {
                max = mod;
            }
        }

        return max;
    }

    // ///// inner product

    /// <param name="a">
    ///     a[n]
    /// </param>
    /// <param name="b">
    ///     a[n]
    /// </param>
    /// <returns> ab[n] = a dot b </returns>
    public static double vectorInnerProduct(double[] a, double[] b)
    {
        var n = a.Length;
        double ab = 0;

        for (var i = 0; i < n; i++)
        {
            ab += a[i] * b[i];
        }

        return ab;
    }

    public static ComplexNumber vectorInnerProduct(ComplexNumber[] a, double[] b)
    {
        var n = a.Length;
        var ab = new ComplexNumber();

        for (var i = 0; i < n; i++)
        {
            ab = ComplexCalc.add(ab, ComplexCalc.multiply(a[i], b[i]));
        }

        return ab;
    }

    public static ComplexNumber vectorInnerProduct(double[] a, ComplexNumber[] b)
    {
        var n = a.Length;
        var ab = new ComplexNumber();

        for (var i = 0; i < n; i++)
        {
            ab = ComplexCalc.add(ab, ComplexCalc.multiply(a[i], b[i]));
        }

        return ab;
    }

    public static ComplexNumber vectorInnerProduct(ComplexNumber[] a, ComplexNumber[] b)
    {
        var n = a.Length;
        var ab = new ComplexNumber();
        ;

        for (var i = 0; i < n; i++)
        {
            ab = ComplexCalc.add(ab, ComplexCalc.multiply(a[i], b[i]));
        }

        return ab;
    }

    // /// tensor product

    public static double[][] vectorOuterProduct(double[] v1, double[] v2)
    {
        var m = v1.Length;
        var n = v2.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] C = new double[m][n];
        var C = RectangularArrays.RectangularDoubleArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] += v1[i] * v2[j];
            }
        }

        return C;
    }

    public static ComplexNumber?[][] vectorOuterProduct(ComplexNumber[] v1, double[] v2)
    {
        var m = v1.Length;
        var n = v2.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(v1[i], v2[j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] vectorOuterProduct(double[] v1, ComplexNumber[] v2)
    {
        var m = v1.Length;
        var n = v2.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(v1[i], v2[j]);
            }
        }

        return C;
    }

    public static ComplexNumber?[][] vectorOuterProduct(ComplexNumber[] v1, ComplexNumber[] v2)
    {
        var m = v1.Length;
        var n = v2.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] C = new ComplexNumber[m][n];
        var C = RectangularArrays.RectangularComplexNumberArray(m, n);

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                C[i][j] = ComplexCalc.multiply(v1[i], v2[j]);
            }
        }

        return C;
    }

    public static double[][] vectorToColumnMatrix(double[] v)
    {
        var n = v.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: double[][] row = new double[n][1];
        var row = RectangularArrays.RectangularDoubleArray(n, 1);

        for (var i = 0; i < n; i++)
        {
            row[i][0] = v[i];
        }

        return row;
    }

    public static ComplexNumber?[][] vectorToColumnMatrix(ComplexNumber?[] v)
    {
        var n = v.Length;
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] row = new ComplexNumber[n][1];
        var row = RectangularArrays.RectangularComplexNumberArray(n, 1);

        for (var i = 0; i < n; i++)
        {
            row[i][0] = v[i];
        }

        return row;
    }

    public static double[][] vectorToRowMatrix(double[] v)
    {
        var n = v.Length;
        double[][] row = { new double[n] };

        for (var i = 0; i < n; i++)
        {
            row[0][i] = v[i];
        }

        return row;
    }

    public static ComplexNumber[][] vectorToRowMatrix(ComplexNumber[] v)
    {
        var n = v.Length;
        ComplexNumber[][] row = { new ComplexNumber[n] };

        for (var i = 0; i < n; i++)
        {
            row[0][i] = v[i];
        }

        return row;
    }

    //Helper class added by Java to C# Converter:

    //----------------------------------------------------------------------------------------
    //	Copyright © 2007 - 2021 Tangible Software Solutions, Inc.
    //	This class can be used by anyone provided that the copyright notice remains intact.
    //
    //	This class includes methods to convert Java rectangular arrays (jagged arrays
    //	with inner arrays of the same length).
    //----------------------------------------------------------------------------------------
    internal static class RectangularArrays
    {
        public static ComplexNumber?[][] RectangularComplexNumberArray(int size1, int size2)
        {
            var newArray = new ComplexNumber?[size1][];

            for (var array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new ComplexNumber?[size2];
            }

            return newArray;
        }

        public static double[][] RectangularDoubleArray(int size1, int size2)
        {
            var newArray = new double[size1][];

            for (var array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new double[size2];
            }

            return newArray;
        }
    }
}

using System;

namespace MindGamesApi.Services.CWT;

public static class Hermite
{
    /// <summary>
    ///     ***********************************************************************
    ///     Representation of Hn as C[0] + C[1]*X + ... + C[N]*X^N
    ///     Input parameters:
    ///     N   -   polynomial degree, n>=0
    ///     Output parameters:
    ///     C   -   coefficients
    ///     ************************************************************************
    /// </summary>
    public static void HermiteCoefficients(int n)
    {
        int i;

        var c = new double[n + 1];

        for (i = 0; i <= n; i++)
        {
            c[i] = 0;
        }

        c[n] = Math.Exp(n * Math.Log(2));

        for (i = 0; i <= n / 2 - 1; i++)
        {
            c[n - 2 * (i + 1)] = -(c[n - 2 * i] * (n - 2 * i) * (n - 2 * i - 1) / 4 / (i + 1));
        }
    }

    /// <summary>
    ///     ***********************************************************************
    ///     Summation of Hermite polynomials using Clenshaw's recurrence formula.
    ///     This routine calculates
    ///     c[0]*H0(x) + c[1]*H1(x) + ... + c[N]*HN(x)
    ///     Parameters:
    ///     n   -   degree, n>=0
    ///     x   -   argument
    ///     Result:
    ///     the value of the Hermite polynomial at x
    ///     ************************************************************************
    /// </summary>
    public static double HermiteSum(double[] c, int n, double x)
    {
        double result = 0;
        double b1;
        double b2;
        int i;

        b1 = 0;
        b2 = 0;

        for (i = n; i >= 0; i--)
        {
            result = 2 * (x * b1 - (i + 1) * b2) + c[i];
            b2 = b1;
            b1 = result;
        }

        return result;
    }

    /// <summary>
    ///     ***********************************************************************
    ///     Calculation of the value of the Hermite polynomial.
    ///     Parameters:
    ///     n   -   degree, n>=0
    ///     x   -   argument
    ///     Result:
    ///     the value of the Hermite polynomial Hn at x
    ///     ************************************************************************
    /// </summary>
    public static double PhysicistHermitePoly(int n, double x)
    {
        double result = 0;
        int i;
        double a;
        double b;

        //
        // Prepare A and B
        //
        a = 1;
        b = 2 * x;

        //
        // Special cases: N=0 or N=1
        //
        if (n == 0)
        {
            result = a;

            return result;
        }

        if (n == 1)
        {
            result = b;

            return result;
        }

        //
        // General case: N>=2
        //
        for (i = 2; i <= n; i++)
        {
            result = 2 * x * b - 2 * (i - 1) * a;
            a = b;
            b = result;
        }

        return result;
    }

    // added by M Bishop
    public static double ProbabilistHermitePoly(int n, double z)
    {
        double m = n;
        var pHPoly = PhysicistHermitePoly(n, z / Math.Pow(2, .5));
        var result = pHPoly / Math.Pow(2, m / 2);

        return result;
    }
}

using System;

namespace MindGamesApi.Services.CWT;

public static class Gamma
{
    public static double Factorial(int val)
    {
        if (val == 0)
        {
            return 1;
        }

        double result = val;
        int i;

        for (i = 1; i <= val - 1; i++)
        {
            result = result * (val - (double)i);
        }

        return result;
    }

    public static double PerformGamma(double x)
    {
        double[] p =
        {
            0.99999999999999709182,
            57.156235665862923517,
            -59.597960355475491248,
            14.136097974741747174,
            -0.49191381609762019978,
            .33994649984811888699e-4,
            .46523628927048575665e-4,
            -.98374475304879564677e-4,
            .15808870322491248884e-3,
            -.21026444172410488319e-3,
            .21743961811521264320e-3,
            -.16431810653676389022e-3,
            .84418223983852743293e-4,
            -.26190838401581408670e-4,
            .36899182659531622704e-5
        };
        var g = 4.7421875;

        if (x < 0.5)
        {
            return Math.PI / (Math.Sin(Math.PI * x) * PerformGamma(1 - x));
        }

        x -= 1;
        var a = p[0];
        var t = x + g + 0.5;

        for (var i = 1; i < p.Length; i++)
        {
            a += p[i] / (x + i);
        }

        return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
    }
}

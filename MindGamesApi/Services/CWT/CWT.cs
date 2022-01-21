using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MindGamesApi.Services.CWT;

public static class Cwt
{
    public enum Wavelet
    {
        Morlet,
        Dog,
        Paul
    }

    public static double[] CwtReconstruct(Wavelet mother, double param, double[][] rwt, double[] scales, double dj, double dt, double signalMean)
    {
        var psi0C = GetEmpiricalFactors(mother, param);

        double factor = 0;

        if (psi0C[0] is double)
        {
            var psi0 = (double)psi0C[0]!; //todo might not be safe
            var c = (double)psi0C[1]!;
            factor = dj * Math.Sqrt(dt) / (psi0 * c);
        }
        else
        {
            ComplexNumber? cFactor;
            var psi0 = (ComplexNumber)psi0C[0]!; //todo: consider if this is actually safe to do
            var c = (ComplexNumber)psi0C[1]!;
            cFactor = ComplexCalc.divide(dj * Math.Sqrt(dt), ComplexCalc.multiply(psi0, c));

            if (cFactor != null)
            {
                factor = cFactor.Real;
            }
        }

        var y = new double[rwt.Length];

        for (var i = 0; i < rwt.Length; i++)
        {
            double summer = 0;

            for (var j = 0; j < scales.Length; j++)
            {
                summer += rwt[i][j] / Math.Sqrt(scales[j]);
            }

            y[i] = summer * factor + signalMean;
        }

        return y;
    }

    public static double[]? GetSelectedParamChoices(Wavelet wavelet)
    {
        double[]? validParams = null;

        if (wavelet == Wavelet.Morlet)
        {
            validParams = new[] { 5, 5.336, 6, 7, 8, 10, 12, 14, 16, 20 };
        }
        else if (wavelet == Wavelet.Paul)
        {
            validParams = new double[] { 4, 5, 6, 7, 8, 10, 16, 20, 30, 40 };
        }
        else if (wavelet == Wavelet.Dog)
        {
            validParams = new double[] { 2, 4, 5, 6, 8, 12, 16, 20, 30, 60 };
        }

        return validParams;
    }

    public static List<object> PerformCwt(double[] y, double dt, Wavelet mother, double param, double s0, double dj, int jtot)
    {
        var n = y.Length;

        var wspcmf = new List<object>();
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: ComplexNumber[][] wT = new ComplexNumber[jtot][n];
        var wT = RectangularArrays.RectangularComplexNumberArray(jtot, n);
        MatrixOps.initialize(wT);
        var scale = new double[jtot];
        var period = new double[jtot];
        var coi = new double[n];
        double signalMean = 0;

        for (var i = 0; i < n; i++)
        {
            signalMean = signalMean + y[i];
        }

        signalMean = signalMean / n;
        var yNoDc = new double[n];

        for (var i = 0; i < n; i++)
        {
            yNoDc[i] = y[i] - signalMean;
        }

        var yFft = Fft.FftRadixCpu(yNoDc);

        var w = new double[n];
        w[0] = 0;
        var df = 2.0 * Math.PI / (n * dt);

        for (var i = 2; i <= n / 2 + 1; i++)
        {
            w[i - 1] = (i - 1.0) * df;
        }

        for (var i = n / 2 + 2; i <= n; i++)
        {
            w[i - 1] = -w[n - i + 1];
        }

        // main wavelet loop
        for (var j = 0; j < jtot; j++)
        {
            scale[j] = s0 * Math.Pow(2.0, j * dj);
            var daughter = GetDaughter(mother, param, scale[j], w, dt);

            // Convolution(f, g) = ifft( fft(f) X fft(g) )
            var product = new ComplexNumber?[n];

            for (var k = 0; k <= n - 1; k++)
            {
                product[k] = ComplexCalc.multiply(daughter[k], yFft[k]);
            }

            wT[j] = Fft.IfftRadixCpu(product);
        }

        // compute period and coi vectors
        double fourierFactor = 0;
        double eFold = 0;

        switch (mother.ToString())
        {
            case "Morlet":

                fourierFactor = 4.0 * Math.PI / (param + Math.Sqrt(2.0 + Math.Pow(param, 2)));
                eFold = fourierFactor / Math.Sqrt(2.0);

                break;

            case "Paul":

                fourierFactor = 4.0 * Math.PI / (2 * param + 1);
                eFold = fourierFactor * Math.Sqrt(2.0);

                break;

            case "Dog":

                fourierFactor = 2 * Math.PI * Math.Sqrt(2 / (2 * param + 1));
                eFold = fourierFactor / Math.Sqrt(2.0);

                break;
        }

        for (var i = 0; i < jtot; i++)
        {
            period[i] = scale[i] * fourierFactor;
        }

        for (var i = 1; i <= (n + 1) / 2; i++)
        {
            coi[i - 1] = eFold * dt * (i - 1);
            coi[n - 1 - i + 1] = coi[i - 1];
        }

        wspcmf.Add(wT);
        wspcmf.Add(scale);
        wspcmf.Add(period);
        wspcmf.Add(coi);
        wspcmf.Add(signalMean);
        wspcmf.Add(fourierFactor);

        return wspcmf;
    }

    private static ComplexNumber?[] GetDaughter(Wavelet mother, double param, double scaleJ, double[] waveNumbers, double dt)
    {
        var n = waveNumbers.Length;

        var daughter = new ComplexNumber?[n];

        if (mother == Wavelet.Morlet)
        {
            var norm = Math.Sqrt(2.0 * Math.PI * scaleJ / dt) * Math.Pow(Math.PI, -0.25);

            for (var k = 0; k <= n / 2; k++)
            {
                var expnt = -0.5 * Math.Pow(scaleJ * waveNumbers[k] - param, 2);
                daughter[k] = new ComplexNumber(norm * Math.Exp(expnt), 0);
            }

            for (var k = n / 2 + 1; k < n; k++)
            {
                daughter[k] = ComplexCalc.zero();
                //double expnt = -0.5 * Math.Pow((scaleJ * waveNumbers[k] - param), 2);
                //daughter[k] = new ComplexNumber(norm * Math.Exp(expnt), 0);
            }
        }

        if (mother == Wavelet.Paul)
        {
            var f = Gamma.Factorial(2 * (int)param - 1);
            var norm = Math.Sqrt(2 * Math.PI * scaleJ / dt);
            norm *= Math.Pow(2, param) / Math.Sqrt(param * f);

            for (var k = 0; k <= n / 2; k++)
            {
                var expnt = -scaleJ * waveNumbers[k];
                daughter[k] = new ComplexNumber(norm * Math.Pow(scaleJ * waveNumbers[k], param) * Math.Exp(expnt), 0);
            }

            for (var k = n / 2 + 1; k < n; k++)
            {
                daughter[k] = ComplexCalc.zero();
            }
        }

        // Odd DOGs are ComplexNumber.
        if (mother == Wavelet.Dog)
        {
            var nf1 = Math.Sqrt(2 * Math.PI * scaleJ / dt) * Math.Sqrt(1 / Gamma.PerformGamma(param + 0.5));
            var nf2 = ComplexCalc.pow(ComplexCalc.i(), param);
            var norm = ComplexCalc.multiply(-nf1, nf2);

            for (var k = 0; k < n; k++)
            {
                var sw = scaleJ * waveNumbers[k];
                var d1 = ComplexCalc.multiply(norm, Math.Pow(sw, param));
                var d2 = Math.Exp(-Math.Pow(sw, 2) / 2);
                daughter[k] = ComplexCalc.multiply(d1, d2);
            }
        }

        return daughter;
    }

    private static List<object?> GetEmpiricalFactors(Wavelet mother, double param)
    {
        double dt = 1;
        var n = 2;
        var s0 = 0.01;
        var dj = 0.1;
        var jtot = 120;

        if (mother == Wavelet.Dog)
        {
            n = 256;
            jtot = 160;
        }

        var deltaFn = new double[n];
        {
            for (var i = 0; i < deltaFn.Length; i++)
            {
                deltaFn[i] = 0;
            }

            deltaFn[0] = 1;
        }

        var wspcmf = PerformCwt(deltaFn, dt, mother, param, s0, dj, jtot);

        var wave = (ComplexNumber[][])wspcmf[0];
        var scale = (double[])wspcmf[1];
        var fourierFactor = (double)wspcmf[5];
        Debug.WriteLine(mother + " " + param + ": Fourier factor " + fourierFactor);

        var rWave = MatrixOps.getRealPart(wave);

        for (var j = 0; j < scale.Length; j++)
        {
            for (var i = 0; i < deltaFn.Length; i++)
            {
                rWave[i][j] = rWave[i][j] / Math.Sqrt(scale[j]);
            }
        }

        var sums = new double[deltaFn.Length];

        for (var i = 0; i < deltaFn.Length; i++)
        {
            sums[i] = 0;

            for (var j = 0; j < scale.Length; j++)
            {
                sums[i] += rWave[i][j];
            }
        }

        // double max = MatrixOps.vectorInfinityNorm(sums);
        var hi = MatrixOps.max(sums);
        var lo = MatrixOps.min(sums);
        var max = hi;

        if (Math.Abs(lo) > hi)
        {
            max = lo;
        } // signed val of infinity norm

        var sqrtDt = Math.Sqrt(dt);
        double psi0 = 0;
        var cPsi0 = ComplexCalc.zero();

        switch (mother)
        {
            case Wavelet.Morlet:

                psi0 = 1 / Math.Pow(Math.PI, 0.25);

                break;

            case Wavelet.Paul:

                var m = (int)param;
                var numerator = ComplexCalc.multiply(ComplexCalc.multiply(ComplexCalc.pow(ComplexCalc.i(), m), Math.Pow(2, m)), Gamma.Factorial(m));
                var denominator = Math.Sqrt(Math.PI * Gamma.Factorial(2 * m));
                cPsi0 = ComplexCalc.divide(numerator, denominator);

                if (cPsi0 != null)
                {
                    psi0 = cPsi0.Real;
                }

                break;

            case Wavelet.Dog:

                m = (int)param;
                psi0 = m + 0.5;

                if (m % 2 != 0)
                {
                    // Asymmetric!
                    psi0 = Hermite.ProbabilistHermitePoly(m, 1) / Math.Sqrt(Gamma.PerformGamma(psi0));
                }
                else
                {
                    psi0 = Hermite.ProbabilistHermitePoly(m, 0) / Math.Sqrt(Gamma.PerformGamma(psi0));
                }

                psi0 = psi0 * Math.Pow(-1, m + 1);

                break;
        }

        var psi0C = new List<object?>();

        if (mother == Wavelet.Paul && param % 2 != 0)
        {
            var result = max * dj * sqrtDt;
            var c = ComplexCalc.divide(result, cPsi0);
            psi0C.Add(cPsi0);
            psi0C.Add(c);
            Debug.WriteLine(mother + " " + param + ": psi0 = (" + cPsi0?.Real + ", " + cPsi0?.Imaginary + "i), C = (" + c?.Real + ", " + c?.Imaginary + ")");
        }
        else
        {
            var c = max * dj * sqrtDt / psi0;
            psi0C.Add(psi0);
            psi0C.Add(c);
            Debug.WriteLine(mother + " " + param + ": psi0 = " + psi0 + ", C = " + c);
        }

        //Debug.WriteLine();

        return psi0C;
    }

    //Helper class added by Java to C# Converter:

    //----------------------------------------------------------------------------------------
    //	Copyright © 2007 - 2021 Tangible Software Solutions, Inc.
    //	This class can be used by anyone provided that the copyright notice remains intact.
    //
    //	This class includes methods to convert Java rectangular arrays (jagged arrays
    //	with inner arrays of the same length).
    //----------------------------------------------------------------------------------------
    private static class RectangularArrays
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
    }
}

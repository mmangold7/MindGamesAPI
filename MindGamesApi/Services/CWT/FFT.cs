using System;

namespace MindGamesApi.Services.CWT;

public static class Fft
{
    /// <param name="ft">
    ///     time domain function
    /// </param>
    /// <returns> frequency domain function </returns>
    public static ComplexNumber?[] FftRadixCpu(ComplexNumber?[] ft)
    {
        var n = ft.Length;
        var hf = new ComplexNumber?[n];
        MatrixOps.initialize(hf);

        if (n == 1)
        {
            hf[0] = ft[0];

            return hf;
        }

        var even = new ComplexNumber?[n / 2];
        var odd = new ComplexNumber?[n / 2];

        for (var k = 0; k < n / 2; k++)
        {
            even[k] = ft[2 * k];
            odd[k] = ft[2 * k + 1];
        }

        var q = FftRadixCpu(even);
        var r = FftRadixCpu(odd);

        for (var k = 0; k < n / 2; k++)
        {
            var kth = -2 * k * Math.PI / n;
            var wk = new ComplexNumber(Math.Cos(kth), Math.Sin(kth));

            hf[k] = ComplexCalc.add(q[k], ComplexCalc.multiply(wk, r[k]));
            hf[k + n / 2] = ComplexCalc.subtract(q[k], ComplexCalc.multiply(wk, r[k]));
        }

        return hf;
    }

    /// <param name="ftD">
    ///     time domain function
    /// </param>
    /// <returns> frequency domain function </returns>
    public static ComplexNumber?[] FftRadixCpu(double[] ftD)
    {
        var ft = MatrixOps.toComplexNumber(ftD);

        return FftRadixCpu(ft);
    }

    /// <param name="hf">
    ///     frequency domain function
    /// </param>
    /// <returns> time domain function </returns>
    public static ComplexNumber?[] IfftRadixCpu(ComplexNumber?[] hf)
    {
        var n = hf.Length;

        // take conjugate
        for (var i = 0; i < n; i++)
        {
            hf[i] = ComplexCalc.conjugate(hf[i]);
        }

        // compute forward FFT
        var ft = FftRadixCpu(hf);

        // take conjugate again
        for (var i = 0; i < n; i++)
        {
            ft[i] = ComplexCalc.conjugate(ft[i]);
        }

        // divide by N
        var rcrpln = 1.0 / n;

        for (var i = 0; i < n; i++)
        {
            ft[i] = ComplexCalc.multiply(ft[i], rcrpln);
        }

        return ft;
    }
}

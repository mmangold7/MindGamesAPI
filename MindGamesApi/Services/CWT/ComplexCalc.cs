using System;

//disabled as this is converted code that I do not want to maintain and has always worked
#pragma warning disable

namespace MindGamesApi.Services.CWT;

public static class ComplexCalc
{
    private static ComplexNumber? negativeI;

    public static double absoluteSquare(ComplexNumber? z)
    {
        var result = new ComplexNumber();
        result = multiply(z, conjugate(z));

        return result.Real;
    }

    public static ComplexNumber? add(ComplexNumber? x, ComplexNumber? y) => new(x.Real + y.Real, x.Imaginary + y.Imaginary);

    public static ComplexNumber? add(double x, ComplexNumber? y) => add(toComplexNumber(x), y);

    public static ComplexNumber? add(ComplexNumber? x, double y) => add(x, toComplexNumber(y));

    public static ComplexNumber? add(double x, double y) => toComplexNumber(x + y);

    public static ComplexNumber? arcCos(ComplexNumber? z) => subtract(divide(pi(), two()), arcSin(z));

    public static ComplexNumber? arcCosh(ComplexNumber? z) => ln(add(z, multiply(pow(add(z, minusOne()), pointFive()), pow(add(z, 1), pointFive()))));

    public static ComplexNumber? arcSin(ComplexNumber? z)
    {
        var result = new ComplexNumber();
        var f = new ComplexNumber();
        f = ln(add(multiply(i(), z), pow(subtract(1, pow(z, two())), pointFive())));
        result = multiply(negativeI, f);

        return result;
    }

    public static ComplexNumber? arcSinh(ComplexNumber? z) => ln(add(z, pow(add(pow(z, two()), 1), pointFive())));

    public static ComplexNumber? arcTan(ComplexNumber? z)
    {
        var result = new ComplexNumber();
        result = subtract(ln(subtract(1, multiply(i(), z))), ln(add(1, multiply(i(), z))));
        result = multiply(divide(i(), two()), result);

        return result;
    }

    public static ComplexNumber? arcTanh(ComplexNumber? z) => divide(subtract(ln(add(1, z)), ln(subtract(1, z))), two());

    public static double arg(ComplexNumber? z)
    {
        if (z.Real == 0)
        {
            if (z.Imaginary < 0)
            {
                return -.5 * Math.PI;
            }

            if (z.Imaginary > 0)
            {
                return .5 * Math.PI;
            }

            return 0;
        }

        if (z.Imaginary == 0)
        {
            if (z.Real > 0)
            {
                return 0;
            }

            return Math.PI;
        }

        return Math.Atan2(z.Imaginary, z.Real);
    }

    public static double arg(double z)
    {
        if (z >= 0)
        {
            return 0;
        }

        return Math.PI;
    }

    public static ComplexNumber? conjugate(ComplexNumber? z)
    {
        var result = new ComplexNumber(z.Real, -z.Imaginary);

        return result;
    }

    public static ComplexNumber? conjugate(double z) => toComplexNumber(z);

    public static ComplexNumber? cos(ComplexNumber z)
    {
        var num1 = new ComplexNumber(-z.Imaginary, z.Real);
        var num2 = new ComplexNumber(z.Imaginary, -z.Real);

        return divide(add(exp(num1), exp(num2)), 2);
    }

    public static ComplexNumber? cos(double z) => toComplexNumber(Math.Cos(z));

    public static ComplexNumber? cosh(ComplexNumber? z)
    {
        var neg = new ComplexNumber(-z.Real, -z.Imaginary);

        return divide(add(exp(z), exp(neg)), two());
    }

    public static ComplexNumber? divide(ComplexNumber? num, ComplexNumber? denom)
    {
        var divisor = Math.Pow(denom.Real, 2) + Math.Pow(denom.Imaginary, 2);
        var resultReal = (num.Real * denom.Real + num.Imaginary * denom.Imaginary) / divisor;
        var resultImaginary = (num.Imaginary * denom.Real - num.Real * denom.Imaginary) / divisor;

        return new ComplexNumber(resultReal, resultImaginary);
    }

    public static ComplexNumber? divide(double num, ComplexNumber? denom) => divide(toComplexNumber(num), denom);

    public static ComplexNumber? divide(ComplexNumber? num, double denom) => divide(num, toComplexNumber(denom));

    public static ComplexNumber? divide(double num, double denom) => toComplexNumber(num / denom);

    public static ComplexNumber? e()
    {
        var e = new ComplexNumber(Math.E, 0);

        return e;
    }

    public static ComplexNumber? exp(double z) => pow(e(), z);

    public static ComplexNumber? exp(ComplexNumber? z) => pow(e(), z);

    public static ComplexNumber? i()
    {
        var sqrtMinus1 = new ComplexNumber(0, 1.0);

        return sqrtMinus1;
    }

    public static bool? isEqual(ComplexNumber x, ComplexNumber y)
    {
        if (x.Real == y.Real && x.Imaginary == y.Imaginary)
        {
            return true;
        }

        return false;
    }

    public static bool? isZero(ComplexNumber? z)
    {
        if (z.Real == 0 && z.Imaginary == 0)
        {
            return true;
        }

        return false;
    }

    public static ComplexNumber? ln(ComplexNumber? z)
    {
        var resultReal = Math.Log(modulus(z));
        var resultImaginary = arg(z);

        return new ComplexNumber(resultReal, resultImaginary);
    }

    public static ComplexNumber? ln(double x) => ln(toComplexNumber(x));

    public static ComplexNumber? logBaseB(ComplexNumber? z, ComplexNumber? Base)
    {
        var result = new ComplexNumber();
        result = divide(ln(z), ln(Base));

        return result;
    }

    public static ComplexNumber MinusI()
    {
        var minusI = new ComplexNumber(0, -1);

        return minusI;
    }

    public static ComplexNumber? minusOne()
    {
        var cM1 = new ComplexNumber(-1.0, 0);

        return cM1;
    }

    public static double modulus(ComplexNumber? z) => Math.Pow(Math.Pow(z.Real, 2) + Math.Pow(z.Imaginary, 2), 0.5);

    public static double modulus(double z) => Math.Abs(z);

    public static ComplexNumber? multiply(ComplexNumber? x, ComplexNumber? y)
    {
        var resultReal = x.Real * y.Real - x.Imaginary * y.Imaginary;
        var resultImaginary = x.Real * y.Imaginary + x.Imaginary * y.Real;

        return new ComplexNumber(resultReal, resultImaginary);
    }

    public static ComplexNumber? multiply(double x, ComplexNumber? y) => multiply(toComplexNumber(x), y);

    public static ComplexNumber? multiply(ComplexNumber? x, double y) => multiply(x, toComplexNumber(y));

    public static ComplexNumber? multiply(double x, double y) => toComplexNumber(x * y);

    public static ComplexNumber one()
    {
        var one = new ComplexNumber(1.0, 0);

        return one;
    }

    public static ComplexNumber? pi()
    {
        var pi = new ComplexNumber(Math.PI, 0);

        return pi;
    }

    public static ComplexNumber? pointFive()
    {
        var point5 = new ComplexNumber(0.5, 0);

        return point5;
    }

    public static ComplexNumber pow(double z, double exponent) => new(Math.Pow(z, exponent), 0);

    public static ComplexNumber? pow(ComplexNumber? z, double exponent)
    {
        var result = new ComplexNumber();
        var resultReal = 0;
        var resultImaginary = 0;

        if (z.Real > 0 && z.Imaginary == 0)
        {
            resultImaginary = 0;
            resultReal = (int)Math.Pow(z.Real, exponent);
        }

        double iDbl = 0;
        int i;
        int j;

        if (exponent >= 1 || exponent <= -1)
        {
            iDbl = Math.Abs((int)exponent);
            i = (int)iDbl; // truncate
            result = z;

            for (j = 1; j <= i - 1; j++)
            {
                result = multiply(z, result);
            }

            var newPow = Math.Abs(exponent) - iDbl;

            if (newPow != 0)
            {
                var r = modulus(z);
                var coeff = Math.Pow(r, newPow);
                var arg = ComplexCalc.arg(z);
                var tempReal = coeff * Math.Cos(arg * newPow);
                var tempImaginary = coeff * Math.Sin(arg * newPow);
                result = multiply(new ComplexNumber(tempReal, tempImaginary), result);
            }

            if (exponent < 0)
            {
                return divide(1, result);
            }

            return result;
        }

        {
            var r = modulus(z);
            var coeff = Math.Pow(r, exponent);
            var arg = ComplexCalc.arg(z);

            if (z.Real < 0 && z.Imaginary == 0 && (exponent == 0.5 || exponent == -0.5))
            {
                resultReal = 0; // cos(pi/2) should be 0 but isn't in floating
                // point precision
            }
            else
            {
                resultReal = (int)(coeff * Math.Cos(arg * exponent));
            }

            resultImaginary = (int)(coeff * Math.Sin(arg * exponent));

            return new ComplexNumber(resultReal, resultImaginary);
        }
    }

    public static ComplexNumber? pow(ComplexNumber? z, ComplexNumber? exponent)
    {
        if (exponent.Imaginary == 0)
        {
            return pow(z, exponent.Real);
        }

        var coeff = Math.Pow(Math.Pow(z.Real, 2) + Math.Pow(z.Imaginary, 2), exponent.Real / 2) * Math.Exp(-exponent.Imaginary * arg(z));
        var resultReal = coeff * Math.Cos(exponent.Real * arg(z) + exponent.Imaginary * Math.Log(Math.Pow(z.Real, 2) + Math.Pow(z.Imaginary, 2)) / 2);
        var resultImaginary = coeff * Math.Sin(exponent.Real * arg(z) + exponent.Imaginary * Math.Log(Math.Pow(z.Real, 2) + Math.Pow(z.Imaginary, 2)) / 2);

        return new ComplexNumber(resultReal, resultImaginary);
    }

    public static ComplexNumber? pow(double z, ComplexNumber? exponent) => pow(toComplexNumber(z), exponent);

    public static ComplexNumber? reciprocal(ComplexNumber? z) => divide(1, z);

    public static ComplexNumber? reciprocal(double z) => toComplexNumber(1 / z);

    public static ComplexNumber? sign(ComplexNumber? z)
    {
        if ((bool)isZero(z))
        {
            return zero();
        }

        return divide(z, modulus(z));
    }

    public static ComplexNumber? sin(ComplexNumber z)
    {
        var denom = new ComplexNumber(0, 2);
        var num1 = new ComplexNumber(-z.Imaginary, z.Real);
        var num2 = new ComplexNumber(z.Imaginary, -z.Real);

        return divide(subtract(exp(num1), exp(num2)), denom);
    }

    public static ComplexNumber? sin(double z) => toComplexNumber(Math.Sin(z));

    public static ComplexNumber? sinh(ComplexNumber? z)
    {
        var negReal = -z.Real;
        var negImaginary = -z.Imaginary;

        return divide(subtract(exp(z), exp(new ComplexNumber(negReal, negImaginary))), two());
    }

    public static ComplexNumber? subtract(ComplexNumber? x, ComplexNumber? y) => new(x.Real - y.Real, x.Imaginary - y.Imaginary);

    public static ComplexNumber? subtract(double x, ComplexNumber? y) => subtract(toComplexNumber(x), y);

    public static ComplexNumber? subtract(ComplexNumber? x, double y) => subtract(x, toComplexNumber(y));

    public static ComplexNumber? subtract(double x, double y) => toComplexNumber(x - y);

    public static ComplexNumber? tan(ComplexNumber z) => divide(sin(z), cos(z));

    public static ComplexNumber? tanh(ComplexNumber? z) => divide(sinh(z), cosh(z));

    public static ComplexNumber? toComplexNumber(double real)
    {
        var result = new ComplexNumber(real, 0);

        return result;
    }

    public static ComplexNumber[] toComplexNumber(double[] real)
    {
        var len = real.Length;
        var result = new ComplexNumber[len];

        for (var i = 0; i < len; i++)
        {
            //result[i].Real = real[i];
            result[i] = new ComplexNumber(real[i], result[i].Imaginary);
        }

        return result;
    }

    public static ComplexNumber? two()
    {
        var two = new ComplexNumber(2.0, 0);

        return two;
    }

    public static ComplexNumber? zero()
    {
        var zero = new ComplexNumber();

        return zero;
    }
}

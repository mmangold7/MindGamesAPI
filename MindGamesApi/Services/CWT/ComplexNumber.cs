using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MindGamesApi.Services.CWT;

public class ComplexNumber
{
    public readonly double Imaginary;

    public readonly double Real;

    public ComplexNumber()
    {
        this.Real = 0;
        this.Imaginary = 0;
    }

    public ComplexNumber(double real, double imaginary)
    {
        this.Real = real;
        this.Imaginary = imaginary;
    }

    public string ToCString()
    {
        string i0 = "", i1 = "", i2 = "";

        if (this.Imaginary != 0)
        {
            i1 = "i";
            var magI = Math.Abs(this.Imaginary);
            i2 = Convert.ToString(magI, CultureInfo.InvariantCulture);

            if (this.Imaginary < 0)
            {
                i0 = " -";
            }
            else
            {
                i0 = " +";
            }
        }

        var str = Convert.ToString(this.Real, CultureInfo.InvariantCulture) + i0 + i1 + i2;

        return str;
    }

    //// <summary>
    //// Attempts to parse a string representation of a complex number.
    //// 
    //// A string representation of a double will parse to a complex
    //// representation. Otherwise, the input must delimit real from imaginary
    //// parts with either [space]+ or [space]-(use ### +i### and don't use ###
    //// i### or ###+i### ) The real part may be absent but prefer 0 [+/-]i###. If
    //// the string value ends with i then this parser can interpret the string
    //// provided the real and imaginary parts are delimited properly (e.g. ###
    //// +###i will work).
    //// </summary>
    //// <param name="strVal"> </param>
    //public virtual string From
    //{
    //    set
    //    {
    //        if (isNumeric(value))
    //        {
    //            Real = double.Parse(value);
    //            Imaginary = 0;
    //        }
    //        else
    //        {
    //            value = value.ToLower();
    //            value = value.Trim();
    //            value = value.Replace("j", "i");
    //            value = value.Replace("i ", "i");
    //            value = value.Replace(" i", "i");
    //            if (value[0] == "i"[0])
    //            {
    //                value = "+i";
    //            }
    //            if (value[1] == "i"[0])
    //            {
    //                value = "0 " + value;
    //            }
    //            if (value.EndsWith("i", StringComparison.Ordinal))
    //            {
    //                value = value.Replace(" +", " +i");
    //                value = value.Replace(" -", " -i");
    //                value.Substring(0, value.Length - 2);
    //            }
    //            string[] value = value.Split(" ", true);
    //            Real = double.Parse(value[0]);
    //            double sign = 1.0;
    //            string[] img = value[1].Split("i", true);
    //            if (img[0].Contains("-"))
    //            {
    //                sign = -1.0;
    //            }
    //            if (img.Length == 1)
    //            {
    //                Imaginary = sign;
    //            }
    //            else
    //            {
    //                Imaginary = sign * double.Parse(img[1]);
    //            }
    //        }
    //    }
    //}

    //// 
    //// <param name="str">
    ////            a string </param>
    //// <returns> True if pattern is regex match "-?\\d+(\\.\\d+)?(E-?\\d+)?" i.e.
    ////         a number with optional '-', decimal point or E+/-. </returns>
    //public static bool isNumeric(string str)
    //{
    //    return str.matches("-?\\d+(\\.\\d+)?(E-?\\d+)?");
    //}
}

internal static class StringHelper
{
    //--------------------------------------------------------------------------------
    //	These methods are used to replace calls to the Java String.getBytes methods.
    //--------------------------------------------------------------------------------
    public static sbyte[] GetBytes(this string self) => GetSBytesForEncoding(Encoding.UTF8, self);

    public static sbyte[] GetBytes(this string self, Encoding encoding) => GetSBytesForEncoding(encoding, self);

    public static sbyte[] GetBytes(this string self, string encoding) => GetSBytesForEncoding(Encoding.GetEncoding(encoding), self);

    //-----------------------------------------------------------------------------
    //	These methods are used to replace calls to some Java String constructors.
    //-----------------------------------------------------------------------------
    public static string NewString(sbyte[] bytes) => NewString(bytes, 0, bytes.Length);

    public static string NewString(sbyte[] bytes, int index, int count) => Encoding.UTF8.GetString((byte[])(object)bytes, index, count);

    public static string NewString(sbyte[] bytes, string encoding) => NewString(bytes, 0, bytes.Length, encoding);

    public static string NewString(sbyte[] bytes, int index, int count, string encoding) => NewString(bytes, index, count, Encoding.GetEncoding(encoding));

    public static string NewString(sbyte[] bytes, Encoding encoding) => NewString(bytes, 0, bytes.Length, encoding);

    public static string NewString(sbyte[] bytes, int index, int count, Encoding encoding) => encoding.GetString((byte[])(object)bytes, index, count);

    //------------------------------------------------------------------------------
    //	This method is used to replace most calls to the Java String.split method.
    //------------------------------------------------------------------------------
    public static string[] Split(this string self, string regexDelimiter, bool trimTrailingEmptyStrings)
    {
        var splitArray = Regex.Split(self, regexDelimiter);

        if (trimTrailingEmptyStrings)
        {
            if (splitArray.Length > 1)
            {
                for (var i = splitArray.Length; i > 0; i--)
                {
                    if (splitArray[i - 1].Length > 0)
                    {
                        if (i < splitArray.Length)
                        {
                            Array.Resize(ref splitArray, i);
                        }

                        break;
                    }
                }
            }
        }

        return splitArray;
    }

    //------------------------------------------------------------------------------------
    //	This method is used to replace calls to the 2-arg Java String.startsWith method.
    //------------------------------------------------------------------------------------
    public static bool StartsWith(this string self, string prefix, int toffset) => self.IndexOf(prefix, toffset, StringComparison.Ordinal) == toffset;

    private static sbyte[] GetSBytesForEncoding(Encoding encoding, string s)
    {
        var sbytes = new sbyte[encoding.GetByteCount(s)];
        encoding.GetBytes(s, 0, s.Length, (byte[])(object)sbytes, 0);

        return sbytes;
    }
}

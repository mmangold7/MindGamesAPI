using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MindGamesApi.Models;
using Python.Runtime;

namespace MindGamesApi.Helpers
{
    public static class PythonMathHelper
    {
        public static double[][] DoSingleCWT(List<ChannelDataPacket> channelData)
        {
            dynamic signal = Py.Import("scipy.signal");
            dynamic np = Py.Import("numpy");

            if (channelData != null && channelData.Any())
            {
                var sig = channelData.Select(cd => cd.Volts).ToArray();
                dynamic npsig = np.fromiter(sig, np.@float);
                dynamic widths = np.linspace(1.0, 15.0, 100); //good idea
                dynamic cwtmatr = signal.cwt(npsig, signal.ricker, widths);
                var result = (double[][])cwtmatr;
                return result;
            }
            else
            {
                return new double[256][];
            }
        }

        public static List<double[][]> DoMultiCWT(List<List<ChannelDataPacket>> channelData)
        {
            dynamic signal = Py.Import("scipy.signal");
            dynamic np = Py.Import("numpy");

            if (channelData != null && channelData.Any() && channelData.First() != null && channelData.First().Any())
            {
                List<double[][]> channelResults = new List<double[][]>();

                foreach (var channel in channelData)
                {
                    var sig = channel.Select(cd => cd.Volts).ToArray();
                    dynamic npsig = np.fromiter(sig, np.@float);
                    dynamic widths = np.linspace(1.0, 15.0, 50); //good idea
                    dynamic cwtmatr = signal.cwt(npsig, signal.ricker, widths);
                    var result = (double[][])cwtmatr;
                    channelResults.Add(result);
                }

                return channelResults;
            }
            else
            {
                return new List<double[][]>();
            }
        }

        public static double[][] DoFFT(List<List<ChannelDataPacket>> allChannelData)
        {
            using (Py.GIL())
            {
                dynamic fft = Py.Import("numpy.fft");
                dynamic np = Py.Import("numpy");

                if (allChannelData != null && allChannelData.First().Any())
                {
                    var result = new double[8][];
                    result[0] = new double[251];
                    result[1] = new double[251];
                    result[2] = new double[251];
                    result[3] = new double[251];
                    result[4] = new double[251];
                    result[5] = new double[251];
                    result[6] = new double[251];
                    result[7] = new double[251];
                    foreach (var r in result)
                    {
                        for (var it = 0; it < 251; it++)
                        {
                            r[it] = 0;
                        }
                    }

                    var i = 0;
                    foreach (var channelData in allChannelData)
                    {
                        var sig = channelData.Select(cd => cd.Volts);
                        dynamic npsig = np.fromiter(sig, np.@float);

                        dynamic fftResult = fft.rfft(npsig);
                        double[] realFft = new double[251];

                        int j = 0;
                        foreach (var complex in fftResult)
                        {
                            var parsedReal = (double)np.real(complex);
                            realFft[j] = Math.Abs(parsedReal);
                            j++;
                        }
                        result[i] = realFft.ToArray();
                        i++;
                    }

                    return result;
                }
                else
                {
                    return new double[8][];
                }
            }
        }

        public static double GetMaxArrayValue(double[][] entriesarray)
        {
            double runningtotal = 0;
            int entrycount = 0;
            double max = double.MinValue;
            foreach (double[] arr in entriesarray)
            {
                foreach (double entry in arr)
                {
                    runningtotal += entry;
                    entrycount++;
                    if (entry > max)
                    {
                        max = entry;
                    }
                }
            }
            return max;
        }

        public static double GetMinArrayValue(double[][] entriesarray)
        {
            double runningtotal = 0;
            int entrycount = 0;
            double min = double.MaxValue;
            foreach (double[] arr in entriesarray)
            {
                foreach (double entry in arr)
                {
                    runningtotal += entry;
                    entrycount++;
                    if (entry < min)
                    {
                        min = entry;
                    }
                }
            }
            return min;
        }

        public static bool DoRandomForestClassification(double[][] data)
        {
            dynamic pd = Py.Import("pandas");
            dynamic joblib = Py.Import("joblib");

            //dynamic RandomForestClassifier = joblib.load("forest_clf_working.pkl");
            dynamic RandomForestClassifier = joblib.load("forest_clf_all_channels.pkl");

            dynamic dataFrame = pd.DataFrame(data.SelectMany(d => d)).transpose();

            bool prediction = RandomForestClassifier.predict(dataFrame);

            return prediction;
        }

        public static EyesClosedPrediction DoEyesClosedClassificationCWT(double[] data)
        {
            dynamic pd = Py.Import("pandas");
            dynamic joblib = Py.Import("joblib");

            dynamic RandomForestClassifier = joblib.load("real_eyes_closed_detector_8_2_21.pkl");

            dynamic dataFrame = pd.DataFrame(data).transpose();

            dynamic prediction = RandomForestClassifier.predict_proba(dataFrame);

            var eyesOpenProbability = (double)(prediction[0][0]);
            var eyesClosedProbability = (double)(prediction[0][1]);

            var decimalEyesOpenProbability = Convert.ToDecimal(eyesOpenProbability);
            var decimalEyesClosedProbability = Convert.ToDecimal(eyesClosedProbability);

            return new EyesClosedPrediction() {
                EyesOpenProbability = decimalEyesOpenProbability,
                EyesClosedProbability = decimalEyesClosedProbability
            };
        }

        public static EyesClosedPrediction DoEyesClosedClassificationFFT(double[] data)
        {
            using (Py.GIL())
            {
                dynamic pd = Py.Import("pandas");
                dynamic joblib = Py.Import("joblib");

                dynamic RandomForestClassifier = joblib.load("forest_clf_jason_eyes_closed_12-18-21.pkl");

                dynamic dataFrame = pd.DataFrame(data).transpose();

                dynamic prediction = RandomForestClassifier.predict_proba(dataFrame);

                var eyesOpenProbability = (double)(prediction[0][0]);
                var eyesClosedProbability = (double)(prediction[0][1]);

                var decimalEyesOpenProbability = Convert.ToDecimal(eyesOpenProbability);
                var decimalEyesClosedProbability = Convert.ToDecimal(eyesClosedProbability);

                return new EyesClosedPrediction()
                {
                    EyesOpenProbability = decimalEyesOpenProbability,
                    EyesClosedProbability = decimalEyesClosedProbability
                };
            }
        }

        public static List<double[][]> DoAllCWT(CwtInput cwtInput)
        {
            var datas = new List<List<ChannelDataPacket>>()
            {
                cwtInput.Channel1Data,
                cwtInput.Channel2Data,
                cwtInput.Channel3Data,
                cwtInput.Channel4Data,
                cwtInput.Channel5Data,
                cwtInput.Channel6Data,
                cwtInput.Channel7Data,
                cwtInput.Channel8Data
            };

            var result = new List<double[][]>();

            dynamic signal = Py.Import("scipy.signal");
            dynamic np = Py.Import("numpy");

            foreach (var channelData in datas)
            {
                if (channelData != null && channelData.Any())
                {
                    var sig = channelData.Select(cd => cd.Volts).ToArray();
                    dynamic npsig = np.fromiter(sig, np.@float);
                    dynamic widths = np.linspace(1.0, 15.0, 100); //good idea
                    dynamic cwtmatr = signal.cwt(npsig, signal.ricker, widths);
                    var cwtResult = (double[][])cwtmatr;
                    result.Add(cwtResult);
                }
                else
                {
                    result.Add(new double[100][]);
                }
            }

            return result;
        }
    }
}

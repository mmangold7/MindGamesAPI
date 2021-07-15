using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MindGamesApi.Models;
using Python.Runtime;

namespace MindGamesApi.Helpers
{
    public static class PythonMathHelper
    {
        public static double[][] DoFFT(double[][] allChannelData)
        {
            dynamic fft = Py.Import("scipy.fft");
            dynamic np = Py.Import("numpy");

            var result = new double[8][];

            if (allChannelData != null && allChannelData.First().Any())
            {
                var i = 0;
                foreach (var channelData in allChannelData)
                {
                    dynamic npsig = np.fromiter(channelData, np.@float);
                    dynamic fftResult = fft.fft(npsig);
                    Complex[] complexFft = new Complex[channelData.Length];
                    int j = 0;
                    foreach (var complex in fftResult)
                    {
                        var parsedComplex = (double)complex;
                        complexFft[j] = new Complex(parsedComplex, 0);
                        j++;
                    }
                    result[i] = complexFft.Select(c => c.Real).ToArray();
                    i++;
                }
            }

            return result;
        }

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
                    dynamic widths = np.linspace(1.0, 15.0, 100); //good idea
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

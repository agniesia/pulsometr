using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pulsometr
{
    class DataAnalisys
    {
        public List<double> listMeanRed;
        public int PulseEND = 0;
        public String PulseInformation = "";
        public DataAnalisys(List<double> listMeanRed)
        {
            this.listMeanRed = listMeanRed;
        }
        public void  DataManage(){
            PulseInformation = "Your pulse is measured now...";
            smoothingDate(11, 2.7);
            fourier();

        }
        private void smoothingDate(int SizeMask,double sigma){
            //size is all of lengh
            var gauss=new AForge.Math.Gaussian(sigma);
            var kernel=gauss.Kernel(SizeMask);
            var kernelSum=kernel.Sum();
            int halfSize=(int)Math.Floor(SizeMask/2.0);

            
            double suma = 0;
            // ten kod dodaje zera na brzegach . nie jest to koniecznie
            List<double> temp = listMeanRed.ToList();


            var templistMeanRed = temp.ToArray();
            int powerOfSmoothing=1;
            while (powerOfSmoothing>0)
            {
                for (int i = halfSize; i < listMeanRed.Count - halfSize; i++)
                {
                    suma = 0;
                    for (int k = 0; k < 2 * halfSize; k++)
                        suma += kernel.ElementAt(k) *temp.ElementAt(i);
                    templistMeanRed[i]= suma / kernelSum;
                }
               listMeanRed = (List<double>)(templistMeanRed.ToList());

               powerOfSmoothing--;
            }
            if(listMeanRed.Count>SizeMask)
            {
                for (int i = 0; i < halfSize; i++)
                {
                    listMeanRed.Remove(i);
                    listMeanRed.RemoveAt(listMeanRed.Count() - 1);
                }
            }
            else
            {
                PulseInformation = "Measurement is failed, try again!";
            }

            //tempecg = dateECG.ToList();
            //for (int i = 0; i < SizeMask; i++)
            //    tempecg.RemoveAt(0);
            //tempecg.Reverse();
            //for (int i = 0; i < SizeMask; i++)
            //    tempecg.RemoveAt(0);
            //tempecg.Reverse();
            //dateECG = tempecg.ToArray();
            //return dateECG;
        }
        private void fourier()
        {

            try
            {
                AForge.Math.Complex[] complex = new AForge.Math.Complex[listMeanRed.Count];
                for (int i = 0; i < listMeanRed.Count; i++)
                {
                    complex[i] = new AForge.Math.Complex(HannaWindow(listMeanRed.ElementAt(i), listMeanRed.Count), 0);
                }
                var potega = Math.Log(complex.Length, 2);
                var dlugosc_transforaty = Math.Pow(2, Math.Floor(potega));
                int brzeg = (int)(complex.Length - dlugosc_transforaty);
                AForge.Math.Complex[] complexReal = new AForge.Math.Complex[complex.Length - brzeg];
                for (int i = 0; i < complex.Length - brzeg; i++)
                {
                    complexReal[i] = complex[i + (int)brzeg / 2];
                }
                var a = Math.Log(complexReal.Length, 2);

                AForge.Math.FourierTransform.FFT(complexReal, AForge.Math.FourierTransform.Direction.Forward);


                var ModulList = modul(complexReal);
                if (ModulList.Count / 2 > 5)
                    for (int i = 0; i < (ModulList.Count / 2) + 5; i++)
                        ModulList.Remove(i);
                else
                {
                    PulseInformation = "Measurement is failed, try again";
                }





                var theBigestFrq = ModulList.IndexOf(ModulList.Max());
                var maximumFreq = complexReal.Length / 20.0;
                maximumFreq = maximumFreq / ModulList.Count;
                PulseEND = (int)((theBigestFrq + 1) * maximumFreq);
            }
            catch (Exception ex)
            {
                PulseInformation = "Measurement is failed, try again";
            }
            
        }
        private List<double> modul(AForge.Math.Complex[] c)
        {
            List<double> Modul = new List<double>(c.Length);
            for (int i = 0; i < c.Length; i++)
            {
                Modul.Add(c[i].Magnitude);
            }
            return Modul;
        }
        private double HannaWindow(double count,int N)
        {
            return 0.5*(1-Math.Cos(2*Math.PI*count/(N-1)));
        }
    }
}

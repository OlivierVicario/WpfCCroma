using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace Couleur
{
    public struct XYZ
    {
        public double X, Y, Z;

        public XYZ(double p1, double p2, double p3)
        {
            X = p1;
            Y = p2;
            Z = p3;
        }
    }
    public struct RGB
    {
        public double R, G, B;

        public RGB(double p1, double p2, double p3)
        {
            R = p1;
            G = p2;
            B = p3;
        }
    }
    public struct HLS
    {
        public double H, L, S;

        public HLS(double p1, double p2, double p3)
        {
            H = p1;
            L = p2;
            S = p3;
        }
    }
    public struct Lab
    {
        public double L, a, b;

        public Lab(double p1, double p2, double p3)
        {
            L = p1;
            a = p2;
            b = p3;

        }
    }
    public struct LCH
    {
        public double L, C, H;

        public LCH(double p1, double p2, double p3)
        {
            L = p1;
            C = p2;
            H = p3;
        }
    }

    public class CIE
    {
        //RGB —> XYZ
        static XYZ RGBtoXYZ(RGB aRGB)
        {
            XYZ aXYZ;

            double var_R = (aRGB.R / 255.0); //R from 0 to 255
            double var_G = (aRGB.G / 255.0); //G from 0 to 255
            double var_B = (aRGB.B / 255.0); //B from 0 to 255

            if (var_R > 0.04045) var_R = Math.Pow(((var_R + 0.055) / 1.055), 2.4);//2.4=gamma sRGB?
            else var_R = var_R / 12.92;
            if (var_G > 0.04045) var_G = Math.Pow(((var_G + 0.055) / 1.055), 2.4);
            else var_G = var_G / 12.92;
            if (var_B > 0.04045) var_B = Math.Pow(((var_B + 0.055) / 1.055), 2.4);
            else var_B = var_B / 12.92;

            var_R = var_R * 100;
            var_G = var_G * 100;
            var_B = var_B * 100;

            //Observer. = 2°, Illuminant = D65
            aXYZ.X = var_R * 0.412453 + var_G * 0.357580 + var_B * 0.180423;
            aXYZ.Y = var_R * 0.212671 + var_G * 0.715160 + var_B * 0.072169;
            aXYZ.Z = var_R * 0.019334 + var_G * 0.119193 + var_B * 0.950227;

            return aXYZ;
        }
        //XYZ —> CIE-L*ab
        static Lab XYZtoLab(XYZ aXYZ)
        {
            Lab aLab;

            const double ref_X = 95.047D;
            const double ref_Y = 100.000D;
            const double ref_Z = 108.883D;

            double var_X = aXYZ.X / ref_X; //ref_X = 95.047 Observer= 2°, Illuminant= D65
            double var_Y = aXYZ.Y / ref_Y; //ref_Y = 100.000
            double var_Z = aXYZ.Z / ref_Z; //ref_Z = 108.883

            //double tiers = 1 / 3;
            if (var_X > 0.008856) var_X = Math.Pow(var_X, (1.0 / 3.0));
            else var_X = (7.787 * var_X) + (16.0 / 116.0);
            if (var_Y > 0.008856) var_Y = Math.Pow(var_Y, (1.0 / 3.0));
            else var_Y = (7.787 * var_Y) + (16.0 / 116.0);
            if (var_Z > 0.008856) var_Z = Math.Pow(var_Z, (1.0 / 3.0));
            else var_Z = (7.787 * var_Z) + (16.0 / 116.0);
            aLab.L = (116 * var_Y) - 16;
            aLab.a = 500 * (var_X - var_Y);
            aLab.b = 200 * (var_Y - var_Z);

            return aLab;
        }
        //CIE-L*ab —> CIE-L*CH°
        static LCH LabtoLCH(Lab aLab)
        {
            LCH aLCH;

            double var_H = Math.Atan2(aLab.b, aLab.a); //Quadrant by signs
            if (var_H > 0) var_H = (var_H / Math.PI) * 180;
            else var_H = 360 - (Math.Abs(var_H) / Math.PI) * 180;

            aLCH.L = aLab.L;
            aLCH.C = Math.Sqrt(Math.Pow(aLab.a, 2) + Math.Pow(aLab.b, 2));
            aLCH.H = var_H;

            return aLCH;
        }
        //CIE-L*CH° —>CIE-L*ab//CIE-H° from 0 to 360°
        static Lab LCHtoLab(LCH aLCH)
        {
            Lab aLab;

            aLab.L = aLCH.L;
            aLab.a = Math.Cos((aLCH.H * Math.PI / 180.0)) * aLCH.C;
            aLab.b = Math.Sin((aLCH.H * Math.PI / 180.0)) * aLCH.C;

            return aLab;
        }
        //CIE-L*ab —> XYZ
        static XYZ LabtoXYZ(Lab aLab)
        {
            XYZ aXYZ;

            const double ref_X = 95.047D;
            const double ref_Y = 100.000D;
            const double ref_Z = 108.883D;

            double var_Y = (aLab.L + 16) / 116.0;
            double var_X = aLab.a / 500.0 + var_Y;
            double var_Z = var_Y - aLab.b / 200.0;

            if (Math.Pow(var_Y, 3) > 0.008856) var_Y = Math.Pow(var_Y, 3.0);
            else var_Y = (var_Y - 16.0 / 116.0) / 7.787;
            if (Math.Pow(var_X, 3) > 0.008856) var_X = Math.Pow(var_X, 3.0);
            else var_X = (var_X - 16.0 / 116.0) / 7.787;
            if (Math.Pow(var_Z, 3) > 0.008856) var_Z = Math.Pow(var_Z, 3.0);
            else var_Z = (var_Z - 16.0 / 116.0) / 7.787;

            aXYZ.X = ref_X * var_X; //ref_X = 95.047 Observer= 2°, Illuminant= D65
            aXYZ.Y = ref_Y * var_Y; //ref_Y = 100.000
            aXYZ.Z = ref_Z * var_Z; //ref_Z = 108.883

            return aXYZ;
        }
        //XYZ —> RGB
        static RGB XYZtoRGB(XYZ aXYZ)
        {
            RGB aRGB;

            double var_X = aXYZ.X / 100.0; //X from 0 to 95.047 (Observer = 2°, Illuminant = D65)
            double var_Y = aXYZ.Y / 100.0; //Y from 0 to 100.000
            double var_Z = aXYZ.Z / 100.0; //Z from 0 to 108.883

            double var_R = var_X * 3.240479 + var_Y * -1.537150 + var_Z * -0.498535;
            double var_G = var_X * -0.969256 + var_Y * 1.875992 + var_Z * 0.041556;
            double var_B = var_X * 0.055648 + var_Y * -0.204043 + var_Z * 1.057311;

            if (var_R > 0.0031308) var_R = 1.055 * Math.Pow(var_R, (1.0 / 2.4)) - 0.055;//2.4=gamma sRGB?
            else var_R = 12.92 * var_R;
            if (var_G > 0.0031308) var_G = 1.055 * Math.Pow(var_G, (1.0 / 2.4)) - 0.055;
            else var_G = 12.92 * var_G;
            if (var_B > 0.0031308) var_B = 1.055 * Math.Pow(var_B, (1.0 / 2.4)) - 0.055;
            else var_B = 12.92 * var_B;

            aRGB.R = var_R * 255;
            aRGB.G = var_G * 255;
            aRGB.B = var_B * 255;

            return aRGB;
        }
        //Color -> CIE-L*ab
        static public Lab ColortoLab(Color aColor)
        {
            RGB aRGB;
            aRGB.R = aColor.R;
            aRGB.G = aColor.G;
            aRGB.B = aColor.B;
            XYZ aXYZ = RGBtoXYZ(aRGB);
            return XYZtoLab(aXYZ);
        }

        static public LCH ColortoLCH(Color aColor)
        {
            Lab aLab = ColortoLab(aColor);
            return LabtoLCH(aLab);
        }

        static public Color LabtoColor(Lab aLab)
        {
            XYZ aXYZ = LabtoXYZ(aLab);
            RGB aRGB;


            try
            {
                aRGB = XYZtoRGB(aXYZ);
                if ((aRGB.R >= 0 && aRGB.R <= 255) && (aRGB.G >= 0 && aRGB.G <= 255) && (aRGB.B >= 0 && aRGB.B <= 255))
                {
                    return Color.FromRgb((byte)Math.Round(aRGB.R), (byte)Math.Round(aRGB.G), (byte)Math.Round(aRGB.B));
                }
                else return Color.FromArgb(0, 0, 0, 0);

            }
            catch (ArithmeticException)
            {
                return Color.FromArgb(0, 0, 0, 0);
            }
        }

        static public Color LCHtoColor(LCH aLCH)
        {
            Lab aLab = LCHtoLab(aLCH);
            return LabtoColor(aLab);
        }

        static public double labE(Color color1, Color color2)
        {
            Lab lab1 = ColortoLab(color1);
            Lab lab2 = ColortoLab(color2);

            return Math.Sqrt(Math.Pow(lab2.L - lab1.L, 2.0) + Math.Pow(lab2.a - lab1.a, 2.0) + Math.Pow(lab2.b - lab1.b, 2.0));
        }
        static public double abE(Color color1, Color color2)
        {
            Lab lab1 = ColortoLab(color1);
            Lab lab2 = ColortoLab(color2);

            return Math.Sqrt(Math.Pow(lab2.a - lab1.a, 2.0) + Math.Pow(lab2.b - lab1.b, 2.0));
        }

        static public double lcE(Color color1, Color color2)
        {
            LCH LCH1 = ColortoLCH(color1);
            LCH LCH2 = ColortoLCH(color2);

            return Math.Sqrt(Math.Pow(LCH2.L - LCH1.L, 2.0) + Math.Pow(LCH2.C - LCH1.C, 2.0));
        }

        static public Color moyenLabtoColor(Color color1, Color color2)
        {
            Lab lab1 = ColortoLab(color1);
            Lab lab2 = ColortoLab(color2);
            double L = (lab1.L + lab2.L) / 2.0;
            double a = (lab1.a + lab2.a) / 2.0;
            double b = (lab1.b + lab2.b) / 2.0;
            Lab lab = new Lab(L, a, b);
            return LabtoColor(lab);
        }
    }


    public class Labs : List<Lab>
    {
        public Labs(List<Color> colors)
        {
            Lab aLab;
            foreach (Color c in colors)
            {
                aLab = CIE.ColortoLab(c);
                this.Add(aLab);
            }
        }
    }

    public class TLabs
    {
        public class TLab : IComparable
        {
            public Lab lab;

            public TLab(Lab p_lab)
            {
                lab = p_lab;
            }

            int IComparable.CompareTo(object o)
            {
                TLab tl = (TLab)o;
                switch (TLabs.clef)
                {
                    case "L":
                        return lab.L.CompareTo(tl.lab.L);

                    case "a":
                        return lab.a.CompareTo(tl.lab.a);

                    case "b":
                        return lab.b.CompareTo(tl.lab.b);

                    default:
                        return lab.L.CompareTo(tl.lab.L);
                }
            }
        }

        public List<TLab> tlabs;
        public List<Lab> sorties;
        public static string clef;

        public TLabs(List<Lab> p_labs, string p_clef)
        {
            tlabs = new List<TLab>();
            sorties = new List<Lab>();
            TLab aTLab;
            for (int i = 0; i < p_labs.Count; i++)
            {
                aTLab = new TLab(p_labs[i]);
                tlabs.Add(aTLab);
            }
            clef = p_clef;
            tlabs.Sort();
            Lab aLab; ;
            for (int i = 0; i < p_labs.Count; i++)
            {
                aLab = tlabs[i].lab;
                sorties.Add(aLab);
            }
        }
    }

    public class LCHs : List<LCH>
    {
        public LCHs(List<Color> colors)
        {
            LCH aLCH;
            foreach (Color c in colors)
            {
                aLCH = CIE.ColortoLCH(c);
                this.Add(aLCH);
            }
        }

    }

}

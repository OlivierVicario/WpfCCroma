using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfCCroma
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Color> couleurs = new List<Color>();
        /// <summary>
        /// couleurs des secteurs du cercle chromatique
        /// </summary>
        Color[,] secteurs;
        int nFuseaux = 12;
        int nCouronnes = 12;
        int angle = 180;
        int contraste = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rsContrasteSat.LowerValue = rsContrasteVal.LowerValue = 0;
            rsContrasteSat.HigherValue = rsContrasteVal.HigherValue = 100;
            remplissageSecteurs(nFuseaux, nCouronnes, rsContrasteSat.LowerValue, rsContrasteSat.HigherValue, rsContrasteVal.LowerValue, rsContrasteVal.HigherValue, 0, 0);
            FondCercleChromatique fond = new FondCercleChromatique(dessinCChro.ActualWidth, secteurs.GetLength(0), secteurs.GetLength(1));
            dessinCChro.Children.Add(fond);
            CercleChromatique cChromatique = new CercleChromatique(secteurs, dessinCChro.ActualWidth);
            dessinCChro.Children.Add(cChromatique);
        }


        private void remplissageSecteurs(int nFuseaux, int nCouronnes, double minSaturation, double maxSaturation, double minValeur, double maxValeur, double sweepAngleV, double sweepAngleS)
        {
            secteurs = new Color[nFuseaux, nCouronnes];
            double pasTeinte = 360.0 / nFuseaux;
            double pasSaturation = (maxSaturation - minSaturation) / nCouronnes + 1;
            //double pasValeur = (maxValeur - minValeur) / nCouronnes;
            double h = 0; double hS, hV;
            double s;
            double v;

            for (int f = 0; f < nFuseaux; f++)
            {
                hS = (h + sweepAngleS) % 360;
                if (hS < 180) s = minSaturation + (maxSaturation - minSaturation) * (hS / 180.0); else s = minSaturation + (maxSaturation - minSaturation) * ((360 - hS) / 180.0);

                hV = (h + sweepAngleV) % 360;
                if (hV < 180) v = minValeur + (maxValeur - minValeur) * (hV / 180.0); else v = minValeur + (maxValeur - minValeur) * ((360 - hV) / 180.0);

                int c = (int)((s - minSaturation) / pasSaturation);
                Couleur.LCH aLCH = new Couleur.LCH(v, s, h);
                secteurs[f, c] = Couleur.CIE.LCHtoColor(aLCH);

                h += pasTeinte % 360;
            }
        }



        private void MiseAJour()
        {
            dessinCChro.Children.Clear();

            if ((!(rsContrasteVal == null)) && (!(slAngleV == null)))
            {
                remplissageSecteurs(nFuseaux, nCouronnes, rsContrasteSat.LowerValue, rsContrasteSat.HigherValue, rsContrasteVal.LowerValue, rsContrasteVal.HigherValue, slAngleS.Value, slAngleV.Value);

                FondCercleChromatique fond = new FondCercleChromatique(dessinCChro.ActualWidth, secteurs.GetLength(0), secteurs.GetLength(1));
                dessinCChro.Children.Add(fond);
                CercleChromatique cChromatique = new CercleChromatique(secteurs, dessinCChro.ActualWidth);
                dessinCChro.Children.Add(cChromatique);
                displaySwatches(secteurs);
            }
        }

        private void displaySwatches(Color[,] secteurs)
        {
            if (spSwatches != null)
            {
                spSwatches.Children.Clear();
                for (int f = 0; f < nFuseaux; f++)
                {
                    for (int c = 0; c < nCouronnes; c++)
                    {
                        if (secteurs[f, c].A != 0)
                        {
                            Rectangle r = new Rectangle();
                            r.Height = r.Width = 80;
                            r.Stroke = new SolidColorBrush(Colors.Transparent);
                            r.StrokeThickness = 10;
                            r.Fill = new SolidColorBrush(secteurs[f, c]);
                            spSwatches.Children.Add((Rectangle)r);
                        }
                    }
                }
            }
        }

        private void cPlus_Click(object sender, RoutedEventArgs e)
        {
            if (nCouronnes < 24) { nCouronnes++; tBNCouronnes.Text = nCouronnes.ToString(); MiseAJour(); }
        }

        private void cMoins_Click(object sender, RoutedEventArgs e)
        {
            if (nCouronnes > 6) { nCouronnes--; tBNCouronnes.Text = nCouronnes.ToString(); MiseAJour(); }
        }

        private void fPlus_Click(object sender, RoutedEventArgs e)
        {
            if (nFuseaux < 24) { nFuseaux++; tBNFuseaux.Text = nFuseaux.ToString(); MiseAJour(); }
        }

        private void fMoins_Click(object sender, RoutedEventArgs e)
        {
            if (nFuseaux > 6) { nFuseaux--; tBNFuseaux.Text = nFuseaux.ToString(); MiseAJour(); }
        }


        private void slAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(slAngleS == null))
            {
                lbAngleS.Content = Math.Round(slAngleS.Value);
                MiseAJour();
            }
        }



        private void rsContrasteSat_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            lbminS.Content = Math.Round(rsContrasteSat.LowerValue);
            MiseAJour();
        }

        private void rsContrasteSat_HigherValueChanged(object sender, RoutedEventArgs e)
        {

            if (lbmaxS != null)
            {
                lbmaxS.Content = Math.Round(rsContrasteSat.HigherValue);
                MiseAJour();
            }

        }

        private void slAngleVal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(slAngleV == null))
            {
                lbAngleV.Content = Math.Round(slAngleV.Value);
                MiseAJour();
            }
        }

        private void rsContrasteVal_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            if (lbmaxV != null)
            {
                lbmaxV.Content = Math.Round(rsContrasteVal.HigherValue);
                MiseAJour();
            }
        }

        private void rsContrasteVal_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            lbminV.Content = Math.Round(rsContrasteVal.LowerValue);
            MiseAJour();
        }
    }
}

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
            slContrasteMinS.Value = slContrasteMinV.Value = 0;
            slContrasteMaxS.Value = slContrasteMaxV.Value = 100;
            remplissageSecteurs(nFuseaux, nCouronnes, slContrasteMinS.Value, slContrasteMaxS.Value, slContrasteMinV.Value, slContrasteMaxV.Value, 0, 0);
            FondCercleChromatique fond = new FondCercleChromatique(dessinCChro.ActualWidth, secteurs.GetLength(0), secteurs.GetLength(1));
            dessinCChro.Children.Add(fond);
            CercleChromatique cChromatique = new CercleChromatique(secteurs, dessinCChro.ActualWidth);

            dessinCChro.Children.Add(cChromatique);

            dessinCChro.Cursor = Cursors.Cross;
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

            if ((!(slContrasteMinV == null)) && (!(slAngleV == null)))
            {
                remplissageSecteurs(nFuseaux, nCouronnes, slContrasteMinS.Value, slContrasteMaxS.Value, slContrasteMinV.Value, slContrasteMaxV.Value, slAngleS.Value, slAngleV.Value);

                FondCercleChromatique fond = new FondCercleChromatique(dessinCChro.ActualWidth, secteurs.GetLength(0), secteurs.GetLength(1));
                dessinCChro.Children.Add(fond);



                CercleChromatique cChromatique = new CercleChromatique(secteurs, dessinCChro.ActualWidth);

                dessinCChro.Children.Add(cChromatique);
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
            if (!(slAngleS == null)) MiseAJour();
        }

        private void slContrasteMinS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slContrasteMaxS.Value < slContrasteMinS.Value) slContrasteMaxS.Value = slContrasteMinS.Value;
            if (!(slContrasteMinS == null)) MiseAJour();
        }

        private void slContrasteMaxS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slContrasteMinS.Value > slContrasteMaxS.Value) slContrasteMinS.Value = slContrasteMaxS.Value;
            if (!(slContrasteMaxS == null)) MiseAJour();
        }

        private void slContrasteMinV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slContrasteMaxV.Value < slContrasteMinV.Value) slContrasteMaxV.Value = slContrasteMinV.Value;
            if (!(slContrasteMinV == null)) MiseAJour();
        }

        private void slContrasteMaxV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slContrasteMinV.Value > slContrasteMaxV.Value) slContrasteMinV.Value = slContrasteMaxV.Value;
            if (!(slContrasteMaxV == null)) MiseAJour();
        }
    }
}

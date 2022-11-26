using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WpfCCroma
{
    public class CercleChromatique : FrameworkElement
    {
        private VisualCollection _visuals;

        /// <summary>
        /// classe graphique d'affichage du cercle chromatique
        /// </summary>
        /// <param name="couleurs">tableau de couleurs  lignes,colonnes correspondant à fuseaux,couronnes</param>
        /// <param name="lCote">dimension du carré dans lequel le cercle chromatique est inscrit</param>
        public CercleChromatique(Color[,] couleurs,double lCote)
        {
            _visuals = new VisualCollection(this);

            int nFuseaux=couleurs.GetLength(0);
            int nCouronnes = couleurs.GetLength(1);

            Point Centre = new Point(lCote / 2.0, lCote / 2.0);
            Point debut, fin;
            double angle;
            double distance;
            double sweepAngle = 360.0 / (nFuseaux * 2);
            double eppaisseur = lCote / (nCouronnes * 2);

            for(int f=0;f<nFuseaux;f++)
            {
                for(int c=0;c<nCouronnes;c++)
                {
                    angle = f * 2 * sweepAngle;
                    distance = eppaisseur * 0.5 + c * eppaisseur;

                    DrawingVisual dv = new DrawingVisual();
                    _visuals.Add(dv);

                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        base.OnRender(dc);

                        SolidColorBrush brosse = new SolidColorBrush(couleurs[f,c]);
                        Pen crayon = new Pen(brosse, eppaisseur);

                        PathGeometry geometrie = new PathGeometry();
                        PathFigure figure = new PathFigure();

                        double X = Centre.X + Math.Cos((angle - sweepAngle) * Math.PI / 180.0) * distance;
                        double Y = Centre.Y - Math.Sin((angle - sweepAngle) * Math.PI / 180.0) * distance;
                        debut = new Point(X, Y);
                        figure.StartPoint = debut;

                        X = Centre.X + Math.Cos((angle + sweepAngle) * Math.PI / 180.0) * distance;
                        Y = Centre.Y - Math.Sin((angle + sweepAngle) * Math.PI / 180.0) * distance;
                        fin = new Point(X, Y);

                        ArcSegment arc = new ArcSegment(fin,
                                                        new Size(distance, distance),
                                                        0,
                                                        false,
                                                        SweepDirection.Counterclockwise,
                                                        true);
                        figure.Segments.Add(arc);
                        geometrie.Figures.Add(figure);
                        dc.DrawGeometry(brosse, crayon, geometrie);
                    }
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }


    }
}

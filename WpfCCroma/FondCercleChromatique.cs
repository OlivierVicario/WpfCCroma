using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfCCroma
{
    internal class FondCercleChromatique : FrameworkElement
    {
        private VisualCollection _visuals;

        /// <summary>
        /// classe graphique d'affichage du fond du cercle chromatique
        /// </summary>
        /// 
        /// <param name="lCote">dimension du carré dans lequel le cercle chromatique est inscrit</param>
        public FondCercleChromatique(double lCote, int nFuseaux, int nCouronnes)
        {
            _visuals = new VisualCollection(this);

            //int nFuseaux = couleurs.GetLength(0);
            //int nCouronnes = couleurs.GetLength(1);

            Point Centre = new Point(lCote / 2.0, lCote / 2.0);
            Point debutArc, finArc;
            //Point debutRayon, finRayon;
            double angle;
            double distance;
            double sweepAngle = 360.0 / (nFuseaux * 2);
            double eppaisseur = lCote / (nCouronnes * 2);

            SolidColorBrush brosse = new SolidColorBrush(Colors.Transparent);
            Pen crayon = new Pen(new SolidColorBrush(Colors.LightGray), 0.5);

            for (int f = 0; f < nFuseaux; f++)
            {
                angle = f * 2 * sweepAngle;

                DrawingVisual dv0 = new DrawingVisual();
                _visuals.Add(dv0);

                using (DrawingContext dc = dv0.RenderOpen())
                {
                    base.OnRender(dc);

                    LineGeometry rayon = new LineGeometry();
                    rayon.StartPoint = Centre;
                    double X = Centre.X + Math.Cos((sweepAngle + angle) * Math.PI / 180.0) * lCote / 2.0;
                    double Y = Centre.Y - Math.Sin((sweepAngle + angle) * Math.PI / 180.0) * lCote / 2.0;
                    rayon.EndPoint = new Point(X, Y);

                    dc.DrawGeometry(brosse, crayon, rayon);
                }


                for (int c = 0; c < nCouronnes; c++)
                {

                    distance = eppaisseur * 1 + c * eppaisseur;
                    DrawingVisual dv = new DrawingVisual();
                    _visuals.Add(dv);
                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        base.OnRender(dc);

                        PathGeometry geometrie = new PathGeometry();
                        PathFigure figure = new PathFigure();

                        double X = Centre.X + Math.Cos((angle - sweepAngle) * Math.PI / 180.0) * distance;
                        double Y = Centre.Y - Math.Sin((angle - sweepAngle) * Math.PI / 180.0) * distance;
                        debutArc = new Point(X, Y);
                        figure.StartPoint = debutArc;

                        X = Centre.X + Math.Cos((angle + sweepAngle) * Math.PI / 180.0) * distance;
                        Y = Centre.Y - Math.Sin((angle + sweepAngle) * Math.PI / 180.0) * distance;
                        finArc = new Point(X, Y);

                        ArcSegment arc = new ArcSegment(finArc,
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


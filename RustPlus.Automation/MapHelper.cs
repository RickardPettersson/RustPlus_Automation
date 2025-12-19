using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Point = System.Drawing.Point;

namespace RustPlus_Automation
{
    public class MapHelper
    {
        private const int PAD_WORLD = 2000;

        public static Point WorldToImagePx(int worldSize, float x, float y, int imageWidth, int imageHeight)
        {
            double totalWorld = worldSize + PAD_WORLD;    // z.B. 4500 + 2000 = 6500
            double halfPad = PAD_WORLD * 0.5;            // z.B. 1000

            // Rust kann dir X/Y < 0 oder > worldSize schicken → wir kappen nur auf das *erweiterte* Intervall
            double xx = Math.Clamp(x, -halfPad, worldSize + halfPad);
            double yy = Math.Clamp(y, -halfPad, worldSize + halfPad);

            var worldRectPx = ComputeWorldRectFromWorldSize(imageWidth, imageHeight, worldSize, PAD_WORLD);

            // jetzt müssen wir auf das *gesamte* (Welt + Wasser) Quadrat im Bild normalisieren,
            // nicht nur auf das innere _worldRectPx.
            //
            // Aus ComputeWorldRectFromWorldSize wissen wir:
            // - das innere Weltquadrat hat die Breite _worldRectPx.Width
            // - dieses innere Quadrat entspricht dem Anteil worldSize / (worldSize + PAD_WORLD)
            // ⇒ daraus können wir die "volle" Seitenlänge im Bild wiederherleiten:
            double fullSidePx = worldRectPx.Width * (totalWorld / worldSize);   // also: innerWidth * (6500 / 4500) z.B.

            // dieses volle Quadrat ist wieder mittig im Bild
            // (genau wie oben ox/oy berechnet wurden)
            double imgW = imageWidth;
            double imgH = imageHeight;
            double fullOx = (imgW - fullSidePx) / 2.0;
            double fullOy = (imgH - fullSidePx) / 2.0;

            // und jetzt ganz normal: verschieben um halfPad, dann auf fullSidePx skalieren
            double u = fullOx + ((xx + halfPad) / totalWorld) * fullSidePx;
            double v = fullOy + (((worldSize - yy) + halfPad) / totalWorld) * fullSidePx;

            return new Point((int)u, (int)v);
        }

        private static Rect ComputeWorldRectFromWorldSize(int imgW, int imgH, int worldSize, int padWorld = 2000)
        {
            if (worldSize <= 0) return new Rect(0, 0, imgW, imgH); // fallback

            int minSidePx = Math.Min(imgW, imgH);
            double scale = (double)worldSize / (worldSize + padWorld); // fraction of the image occupied by the world
            double sidePx = minSidePx * scale;

            double ox = (imgW - sidePx) / 2.0; // centered
            double oy = (imgH - sidePx) / 2.0;

            return new Rect(ox, oy, sidePx, sidePx);
        }


        public static bool IsPointInCircle(
            float pointX, float pointY,
            float centerX, float centerY,
            float radius)
        {
            // Calculate squared distance between point and circle center
            float dx = pointX - centerX;
            float dy = pointY - centerY;
            float distanceSquared = dx * dx + dy * dy;

            float radiusSquared = radius * radius;

            // Point is inside or on the circle if distance² <= radius²
            return distanceSquared <= radiusSquared;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Render2
{
    public class NewMath
    {
        public static float Round(float x,float min,float max)
        {
             return (x < min) ? min : ((x > max) ? max : x);
        }
        public static Color Round(Color x, float min, float max)
        {
            Color y = new Color();

            y.r = (x.r < min) ? min : ((x.r > max) ? max : x.r);
            y.g = (x.g < min) ? min : ((x.g > max) ? max : x.g);
            y.b = (x.b < min) ? min : ((x.b > max) ? max : x.b);

            return y;
        }

        public static float Interp(float a,float b,float t)
        {
            float c;
            c = a * (1 - t) + t * b;
            return c;
        }
        public static Color Interp(Color a, Color b, float t)
        {
            Color c = new Color();
            c.r = Interp(a.r, b.r, t);
            c.g = Interp(a.g, b.g, t);
            c.b = Interp(a.b, b.b, t);
            return c;
        }
        public static Vector3 Interp(Vector3 a, Vector3 b, float t)
        {
            Vector3 c = new Vector3();
            c.x = Interp(a.x, b.x, t);
            c.y = Interp(a.y, b.y, t);
            c.z = Interp(a.z, b.z, t);
            c.w = Interp(a.w, b.w, t);
            return c;
        }
        public static Vectex Interp(Vectex a, Vectex b, float t)
        {
            Vectex c = new Vectex();
            c.pos = Interp(a.pos, b.pos, t);
            c.color = Interp(a.color, b.color, t);
            c.lightcolor = Interp(a.lightcolor, b.lightcolor, t);
            c.nomal = Interp(a.nomal, b.nomal, t);
            c.uvs.u = Interp(a.uvs.u, b.uvs.u, t);
            c.uvs.v = Interp(a.uvs.v, b.uvs.v, t);
            c.rhw = Interp(a.rhw, b.rhw, t);
            return c;
        }
    }
}

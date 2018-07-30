using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render2
{
    //变换矩阵信息
    public class Transform
    {
        public Matrix4 world, view, projection, transform;
        public int width, height;

        public Transform(int _width,int _height)
        {
            this.width = _width;
            this.height = _height;
            this.world = new Matrix4();
            this.view = new Matrix4();
            this.projection = new Matrix4();
        }
        public void SetTransform()
        {
            this.transform = this.world * this.view * this.projection;
            
        }
    }

    public class Matrix4
    {
        public float[,] m;

        public Matrix4()
        {
            m = new float[4, 4];
        }
        //矩阵设置
        public void SetZero()
        {
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    m[i, j] = 0;
                }
        }
        public void SetIdentity()
        {
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    if (i == j)
                        m[i, j] = 1f;
                    else
                        m[i, j] = 0;
                }
        }
        public void SetTranslate(float x, float y, float z)
        {
            SetIdentity();
            m[3, 0] = x;
            m[3, 1] = y;
            m[3, 2] = z;
        }
        //坐标变换矩阵设置    
        public void SetScale(float x, float y, float z)
        {
            SetIdentity();
            m[0, 0] = x;
            m[1, 1] = y;
            m[2, 2] = z;
        }
        /// <summary>
        /// 绕x,y,z所在向量旋转
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="theta"></param>
        public void SetRotate(float x, float y, float z, float theta)
        {
            float qsin = (float)(System.Math.Sin(theta * 0.5f));
            float qcos = (float)(System.Math.Cos(theta * 0.5f));
            Vector3 v1 = new Vector3(x, y, z, 1.0f);
            float w = qcos;
            v1.Normalized();
            x = v1.x * qsin;
            y = v1.y * qsin;
            z = v1.z * qsin;
            m[0, 0] = 1 - 2 * y * y - 2 * z * z;
            m[1, 0] = 2 * x * y - 2 * w * z;
            m[2, 0] = 2 * x * z + 2 * w * y;
            m[0, 1] = 2 * x * y + 2 * w * z;

            m[1, 1] = 1 - 2 * x * x - 2 * z * z;
            m[2, 1] = 2 * y * z - 2 * w * x;
            m[0, 2] = 2 * x * z - 2 * w * y;
            m[1, 2] = 2 * y * z + 2 * w * x;

            m[2, 2] = 1 - 2 * x * x - 2 * y * y;
            m[0, 3] = m[1, 3] = m[2, 3] = m[3, 0] = m[3, 1] = m[3, 2] = 0f;
            m[3, 3] = 1f;
        }
        /// <summary>
        /// 设置view矩阵
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="at"></param>
        /// <param name="up"></param>
        public void SetLookat(Vector3 eye, Vector3 at, Vector3 up)
        {
            Vector3 u, v, w;
            w = at - eye;
            w.Normalized();
            u = Vector3.Cross(up, w);
            u.Normalized();
            v = Vector3.Cross(w, u);

            m[0, 0] = u.x;
            m[1, 0] = u.y;
            m[2, 0] = u.z;
            m[3, 0] = -Vector3.Dot(u, eye);

            m[0, 1] = v.x;
            m[1, 1] = v.y;
            m[2, 1] = v.z;
            m[3, 1] = -Vector3.Dot(v, eye);

            m[0, 2] = w.x;
            m[1, 2] = w.y;
            m[2, 2] = w.z;
            m[3, 2] = -Vector3.Dot(w, eye);

            m[0, 3] = m[1, 3] = m[2, 3] = 0;
            m[3, 3] = 1f;
        }
        /// <summary>
        /// D3D透视 fovy 为角度，aspect为宽高比r/t，zn和zf为近远视面
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspect"></param>
        /// <param name="zn"></param>
        /// <param name="zf"></param>
        public void SetPerspective(float fovy, float aspect, float zn, float zf)
        {
            float fax = 1f / (float)Math.Tan(fovy * 0.5f);
            SetZero();
            m[0, 0] = (float)(fax / aspect);//(n/t)/(r/t)=n/r
            m[1, 1] = (float)(fax);//n/t
            m[2, 2] = zf / (zf - zn);
            m[3, 2] = -zn * zf / (zf - zn);
            m[2, 3] = 1;
        }

        //矩阵转置求逆等
        public Matrix4 Transpose()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = i; j < 4; j++)
                {
                    float temp = m[i, j];
                    m[i, j] = m[j, i];
                    m[j, i] = temp;
                }
            }
            return this;
        }
        public Matrix4 Inverse()
        {
            float a = Determinate();
            if (a == 0)
            {
                return null;
            }
            Matrix4 adj = GetAdjoint();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    adj.m[i, j] = adj.m[i, j] / a;
                }
            }
            return adj;

        }
        public Matrix4 GetAdjoint()
        {
            int x, y;
            float[,] tempM = new float[3, 3];
            Matrix4 result = new Matrix4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int t = 0; t < 3; ++t)
                        {
                            x = k >= i ? k + 1 : k;
                            y = t >= j ? t + 1 : t;

                            tempM[k, t] = m[x, y];
                        }
                    }
                    result.m[i, j] = (float)System.Math.Pow(-1, (1 + j) + (1 + i)) * Determinate(tempM, 3);
                }
            }
            return result.Transpose();
        }
        public float Determinate()
        {
            return Determinate(m, 4);
        }
        private float Determinate(float[,] _m, int n)
        {
            if (n == 1)
            {
                return _m[0, 0];
            }
            else
            {
                float result = 0;
                float[,] tempM = new float[n - 1, n - 1];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n - 1; j++)
                    {
                        for (int k = 0; k < n - 1; k++)
                        {
                            int x = j + 1;
                            int y = k >= i ? k + 1 : k;
                            tempM[j, k] = _m[x, y];
                        }
                    }

                    result += (float)System.Math.Pow(-1, 1 + (1 + i)) * _m[0, i] * Determinate(tempM, n - 1);
                }
                return result;
            }
        }

        //矩阵算术
        static public Matrix4 operator +(Matrix4 a, Matrix4 b)
        {
            Matrix4 c = new Matrix4();
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    c.m[i, j] = a.m[i, j] + b.m[i, j];
                }
            return c;
        }
        static public Matrix4 operator -(Matrix4 a, Matrix4 b)
        {
            Matrix4 c = new Matrix4();
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    c.m[i, j] = a.m[i, j] - b.m[i, j];
                }
            return c;
        }
        static public Matrix4 operator *(Matrix4 a, Matrix4 b)
        {
            Matrix4 c = new Matrix4();
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    c.m[i, j] = (a.m[i, 0] * b.m[0, j] + a.m[i, 1] * b.m[1, j]
                                 + a.m[i, 2] * b.m[2, j] + a.m[i, 3] * b.m[3, j]);
                }
            return c;
        }
        static public Matrix4 operator /(Matrix4 a, float f)
        {
            Matrix4 c = new Matrix4();
            int i, j;
            for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    c.m[i, j] = a.m[i, j] * f;
                }
            return c;
        }
        static public Vector3 operator *(Vector3 v1, Matrix4 a)
        {
            Vector3 v2 = new Vector3();
            v2.x = v1.x * a.m[0, 0] + v1.y * a.m[1, 0] + v1.z * a.m[2, 0] + v1.w * a.m[3, 0];
            v2.y = v1.x * a.m[0, 1] + v1.y * a.m[1, 1] + v1.z * a.m[2, 1] + v1.w * a.m[3, 1];
            v2.z = v1.x * a.m[0, 2] + v1.y * a.m[1, 2] + v1.z * a.m[2, 2] + v1.w * a.m[3, 2];
            v2.w = v1.x * a.m[0, 3] + v1.y * a.m[1, 3] + v1.z * a.m[2, 3] + v1.w * a.m[3, 3];
            return v2;
        }
    }
}

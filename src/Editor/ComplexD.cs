using System;
using System.Diagnostics;

namespace Appalachia.Audio
{
    // System.Numerics is not available
    public class ComplexD
    {
        public ComplexD(double real, double imag)
        {
            this.real = real;
            this.imag = imag;
        }

        #region Fields and Autoproperties

        public double imag;
        public double real;

        #endregion

        public static ComplexD Add(ComplexD a, ComplexD b)
        {
            return new(a.real + b.real, a.imag + b.imag);
        }

        public static ComplexD Add(ComplexD a, double b)
        {
            return new(a.real + b, a.imag);
        }

        public static ComplexD Add(double a, ComplexD b)
        {
            return new(a + b.real, b.imag);
        }

        public static ComplexD Div(ComplexD a, ComplexD b)
        {
            var d = (b.real * b.real) + (b.imag * b.imag);
            var s = 1.0 / d;
            return new ComplexD(
                ((a.real * b.real) + (a.imag * b.imag)) * s,
                ((a.imag * b.real) - (a.real * b.imag)) * s
            );
        }

        public static ComplexD Div(double a, ComplexD b)
        {
            var d = (b.real * b.real) + (b.imag * b.imag);
            var s = a / d;
            return new ComplexD(s * b.real, -s * b.imag);
        }

        public static ComplexD Div(ComplexD a, double b)
        {
            var s = 1.0 / b;
            return new ComplexD(a.real * s, a.imag * s);
        }

        public static ComplexD Exp(double omega)
        {
            return new(Math.Cos(omega), Math.Sin(omega));
        }

        public static ComplexD Mul(ComplexD a, ComplexD b)
        {
            return new((a.real * b.real) - (a.imag * b.imag), (a.real * b.imag) + (a.imag * b.real));
        }

        public static ComplexD Mul(ComplexD a, double b)
        {
            return new(a.real * b, a.imag * b);
        }

        public static ComplexD Mul(double a, ComplexD b)
        {
            return new(a * b.real, a * b.imag);
        }

        [DebuggerStepThrough]
        public static ComplexD operator +(ComplexD a, ComplexD b)
        {
            return Add(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator +(ComplexD a, double b)
        {
            return Add(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator +(double a, ComplexD b)
        {
            return Add(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator /(ComplexD a, ComplexD b)
        {
            return Div(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator /(ComplexD a, double b)
        {
            return Div(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator /(double a, ComplexD b)
        {
            return Div(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator *(ComplexD a, ComplexD b)
        {
            return Mul(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator *(ComplexD a, double b)
        {
            return Mul(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator *(double a, ComplexD b)
        {
            return Mul(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator -(ComplexD a, ComplexD b)
        {
            return Sub(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator -(ComplexD a, double b)
        {
            return Sub(a, b);
        }

        [DebuggerStepThrough]
        public static ComplexD operator -(double a, ComplexD b)
        {
            return Sub(a, b);
        }

        public static ComplexD Pow(ComplexD a, double b)
        {
            var p = Math.Atan2(a.imag, a.real);
            var m = Math.Pow(a.Mag2(), b * 0.5f);
            return new ComplexD(m * Math.Cos(p * b), m * Math.Sin(p * b));
        }

        public static ComplexD Sub(ComplexD a, ComplexD b)
        {
            return new(a.real - b.real, a.imag - b.imag);
        }

        public static ComplexD Sub(ComplexD a, double b)
        {
            return new(a.real - b, a.imag);
        }

        public static ComplexD Sub(double a, ComplexD b)
        {
            return new(a - b.real, -b.imag);
        }

        public double Mag()
        {
            return Math.Sqrt(Mag2());
        }

        public double Mag2()
        {
            return (real * real) + (imag * imag);
        }
    }
}

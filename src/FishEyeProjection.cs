#region UICode
double Amount1 = 0; // [-180,180] X Direction
double Amount2 = 0; // [-90,90] Y Direction
double Amount3 = 0; // [-180,180] Z Direction
byte Amount4 = 0; // [1] Projection|Equidistance|Equisolid-angle|Orthographic
double Amount5 = 1; // [0.5,3] Magnification
bool Amount6 = false; // [0,1] Invert convert
int Amount7 = 1; // [1,16] MonteCarlo
#endregion

// MenuSetting
//   Effect Menu
//   Submenu: THETA
//   Name: FishEye
//   Title: THETA - FishEye
//   Icon: FishEyeProjection.png
// Support Information
//   Author: @aitch_two
//   DLL Version: 1.2
//   URL: https://github.com/aitch-two/THETATools

enum kind {
	EqDistProjection = 0,	// Equidistance Projection
	EqSangleProjection,		// Equisolid Angle Projection
	OrthoProjection,		// Orthographic Projection
	test					// Equisolid Angle Projection
};

void Render(Surface dst, Surface src, Rectangle rect)
{
    Mat3d eqRectMat = Mat3d.unit;
    Mat3d persMat = Mat3d.rotY(Math.PI * Amount1 / 180)
        * Mat3d.rotX(Math.PI * Amount2 / 180)
        * Mat3d.rotZ(Math.PI * Amount3 / 180);
    kind k = (kind)Amount4;
    double mag = Amount5;
    bool inv = Amount6;
    int montecarlo = Amount7;

    Projection dstProjection;
    Projection srcProjection;
    switch (k) {
	case kind.EqSangleProjection:
	    if (!inv) {
	        srcProjection = new EqRectProjection(src, eqRectMat);
        	dstProjection = new EqSangleProjection(dst, persMat, mag);
        } else {
        	srcProjection = new EqSangleProjection(src, persMat, mag);
	        dstProjection = new EqRectProjection(dst, eqRectMat);
        }
    	break;
	case kind.OrthoProjection:
	    if (!inv) {
	        srcProjection = new EqRectProjection(src, eqRectMat);
        	dstProjection = new OrthoProjection(dst, persMat, mag);
        } else {
        	srcProjection = new OrthoProjection(src, persMat, mag);
	        dstProjection = new EqRectProjection(dst, eqRectMat);
        }
    	break;
	default:
	    if (!inv) {
	        srcProjection = new EqRectProjection(src, eqRectMat);
        	dstProjection = new EqDistProjection(dst, persMat, mag);
        } else {
        	srcProjection = new EqDistProjection(src, persMat, mag);
	        dstProjection = new EqRectProjection(dst, eqRectMat);
        }
    	break;
    }
    foreach(Projection.Pixel p in dstProjection.pixels(rect)) {
        Col4d c = Col4d.zero;
        for (int i = 0; i < montecarlo; i++) {
            c += srcProjection.getColor(p.vector());
        }
        p.setColor(c / montecarlo);
    }
}

class EqDistProjection : Projection {
    private readonly double mag;
    private readonly double a;
	private static readonly Vec3d unitZ = new Vec3d(0, 0, 1);

    public EqDistProjection(Surface fb, Mat3d mat, double mag) : base(fb, mat, false) {
    	this.mag = mag;
    	this.a = Math.PI / fb.Height;
    }

    protected override Vec3d vec2d2vec3d(Vec2d i) {
        try {
            Vec2d s = (1 / mag) * (i - center);
            double dist = s.abs() * a;
	    	if (dist > Math.PI) {
	    		return Vec3d.zero;
	    	}
	    	double azimuth = Math.Atan2(s.y, s.x);
            return Mat3d.rotZ(-azimuth) * Mat3d.rotY(-dist) * unitZ;
        } catch(ArithmeticException) {
    		return unitZ;
        }
    }

    protected override Vec2d vec3d2vec2d(Vec3d vec) {
        try {
            double dist = Math.Acos((unitZ ^ vec) / vec.abs()) / a;
        	double azimuth = Math.Atan2(vec.y, vec.x);
        	return center + (new Vec2d(mag * dist * Math.Cos(azimuth), mag * dist * Math.Sin(azimuth)));
        } catch(ArithmeticException) {
            return center;
        }
    }
}

class EqSangleProjection : Projection {
    private readonly double mag;
    private readonly double a;
	private static readonly Vec3d unitZ = new Vec3d(0, 0, 1);

    public EqSangleProjection(Surface fb, Mat3d mat, double mag) : base(fb, mat, false) {
    	this.mag = mag;
    	this.a = Math.PI / fb.Height;
    }

    protected override Vec3d vec2d2vec3d(Vec2d i) {
        try {
            Vec2d s = (1 / mag) * (i - center);
            double dist = 2 * Math.Asin(s.abs() * a / 2);
	    	if (dist > Math.PI) {
	    		return Vec3d.zero;
	    	}
	    	double azimuth = Math.Atan2(s.y, s.x);
            return Mat3d.rotZ(-azimuth) * Mat3d.rotY(-dist) * unitZ;
        } catch(ArithmeticException) {
    		return unitZ;
        }
    }

    protected override Vec2d vec3d2vec2d(Vec3d vec) {
        try {
            double dist = Math.Sqrt(2 - 2 * (unitZ ^ vec) / vec.abs()) / a;
        	double azimuth = Math.Atan2(vec.y, vec.x);
        	return center + (new Vec2d(mag * dist * Math.Cos(azimuth), mag * dist * Math.Sin(azimuth)));
        } catch(ArithmeticException) {
            return center;
        }
    }
}

class OrthoProjection : Projection {
    private readonly double mag;
    private readonly double a;
	private static readonly Vec3d unitZ = new Vec3d(0, 0, 1);

    public OrthoProjection(Surface fb, Mat3d mat, double mag) : base(fb, mat, false) {
    	this.mag = mag;
    	this.a = Math.PI / fb.Height;
    }

    protected override Vec3d vec2d2vec3d(Vec2d i) {
        try {
            Vec2d s = (1 / mag) * (i - center);
            double t = s.abs() * a;
			if (1.0 <= t) {
	    		return Vec3d.zero;
	    	}
            double dist = Math.Asin(t);
	    	double azimuth = Math.Atan2(s.y, s.x);
            return Mat3d.rotZ(-azimuth) * Mat3d.rotY(-dist) * unitZ;
        } catch(ArithmeticException) {
    		return unitZ;
        }
    }

    protected override Vec2d vec3d2vec2d(Vec3d vec) {
        try {
            if (vec.z <= 0) {
                return Vec2d.invalid;
            }
        	double dist = Math.Sin(Math.Acos((unitZ ^ vec) / vec.abs())) / a;
        	double azimuth = Math.Atan2(vec.y, vec.x);
        	return center + (new Vec2d(mag * dist * Math.Cos(azimuth), mag * dist * Math.Sin(azimuth)));
        } catch(ArithmeticException) {
            return center;
        }
    }
}

class EqRectProjection : Projection {
	private static readonly Vec3d minusY = new Vec3d(0, -1, 0);
	private readonly double a, b;
	private readonly Vec2d centerBottom, centerTop;

    public EqRectProjection(Surface fb, Mat3d mat) : base(fb, mat, true) {
    	a = -2.0 * Math.PI / fb.Width;
    	b = -Math.PI / fb.Height;
    	centerBottom = new Vec2d(center.x, fb.Bounds.Bottom);
    	centerTop = new Vec2d(center.x, fb.Bounds.Top);
    }

    protected override Vec3d vec2d2vec3d(Vec2d i) {
		return Mat3d.rotY(a * i.x) * Mat3d.rotX(b * i.y) * minusY;
    }

    protected override Vec2d vec3d2vec2d(Vec3d vec) {
        try {
            return new Vec2d(
                center.x - Math.Atan2(vec.x, vec.z) / a,
                fb.Bounds.Top - Math.Acos((vec ^ minusY) / vec.abs()) / b);
        } catch(ArithmeticException) {
            if (vec.y > 0) {
                return centerBottom;
            } else {
                return centerTop;
            }
        }
    }
}

//region common Projection
abstract class Projection {
    private static readonly Col4d blank = new Col4d(0, 0, 0, 0);
    protected Surface fb;
    private readonly Mat3d mat;
    private readonly Mat3d inv;
    protected readonly Vec2d center;
    private readonly bool cyclic;

    public Projection(Surface fb, Mat3d mat, bool cyclic) {
        this.fb = fb;
        this.mat = mat;
        this.inv = 1.0 / mat;
        this.center = new Vec2d(
            (fb.Bounds.Left + fb.Bounds.Right) / 2.0,
            (fb.Bounds.Top + fb.Bounds.Bottom) / 2.0);
        this.cyclic = cyclic;
    }

    protected abstract Vec3d vec2d2vec3d(Vec2d i);
    protected abstract Vec2d vec3d2vec2d(Vec3d vec);

    public Col4d getColor(Vec3d vec) {
    	if (vec.isZero()) {
    		return blank;
    	}
        Vec2d i = vec3d2vec2d(inv * vec);
        if (! i.isValid()) {
        	return blank;
        }
        int xmin = (int)Math.Floor(i.x); int xmax = xmin + 1; double xflac = i.x - xmin;
        int ymin = (int)Math.Floor(i.y); int ymax = ymin + 1; double yflac = i.y - ymin;
        if (cyclic) {
            while (xmin < fb.Bounds.Left)    xmin += fb.Width;
            while (fb.Bounds.Right <= xmin)  xmin -= fb.Width;
            while (xmax < fb.Bounds.Left)    xmax += fb.Width;
            while (fb.Bounds.Right <= xmax)  xmax -= fb.Width;
            while (ymin < fb.Bounds.Top)     ymin += fb.Height;
            while (fb.Bounds.Bottom <= ymin) ymin -= fb.Height;
            while (ymax < fb.Bounds.Top)     ymax += fb.Height;
            while (fb.Bounds.Bottom <= ymax) ymax -= fb.Height;
        }
        Col4d c00 = (fb.Bounds.Contains(xmin, ymin)) ? new Col4d(fb[xmin, ymin]) : blank;
        Col4d c10 = (fb.Bounds.Contains(xmax, ymin)) ? new Col4d(fb[xmax, ymin]) : blank;
        Col4d c01 = (fb.Bounds.Contains(xmin, ymax)) ? new Col4d(fb[xmin, ymax]) : blank;
        Col4d c11 = (fb.Bounds.Contains(xmax, ymax)) ? new Col4d(fb[xmax, ymax]) : blank;
        return
                  (1.0 - xflac) * (1.0 - yflac) * c00
                + (xflac)       * (1.0 - yflac) * c10
                + (1.0 - xflac) * (yflac)       * c01
                + (xflac)       * (yflac)       * c11;
    }
    private void setColor(int x, int y, Col4d c) {
        if (fb.Bounds.Contains(x, y)) {
            fb[x, y] = c.toColorBgra();
        }
    }
    public IEnumerable<Pixel> pixels(Rectangle rect) {
        for (int y = rect.Top; y < rect.Bottom; y++) {
            for (int x = rect.Left; x < rect.Right; x++) {
                yield return new Pixel(this, x, y);
            }
        }
    }
    public class Pixel {
        private readonly Projection projection;
        public readonly int x, y;
        private static Random rnd = new Random();

        public Pixel(Projection projection, int x, int y) {
            this.projection = projection;
            this.x = x;
            this.y = y;
        }
        public Vec3d vector() {
            return projection.mat
                    * projection.vec2d2vec3d(
                        new Vec2d(this.x + rnd.NextDouble(),
                            this.y + rnd.NextDouble()));
        }
        public void setColor(Col4d c) {
            projection.setColor(this.x, this.y, c);
        }
    }
}

//region common Col4d
class Col4d {
    public static readonly Col4d zero = new Col4d(0, 0, 0, 0);
    public double r, g, b, a;
    public Col4d(double r, double g, double b, double a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public Col4d(ColorBgra c) {
        this.r = c.R;
        this.g = c.G;
        this.b = c.B;
        this.a = c.A;
    }
    public static Col4d operator + (Col4d s, Col4d t) {
        return new Col4d(s.r + t.r, s.g + t.g, s.b + t.b, s.a + t.a);
    }
    public static Col4d operator * (double d, Col4d s) {
        return new Col4d(d * s.r, d * s.g, d * s.b, d * s.a);
    }
    public static Col4d operator / (Col4d s, double d) {
        return new Col4d(s.r / d, s.g / d, s.b / d, s.a / d);
    }
    public ColorBgra toColorBgra() {
        ColorBgra ret = new ColorBgra();
        int ir, ig, ib, ia;
        ret.R = (byte)(((ir = (int)this.r) < 0) ? 0 : ((ir <= 255) ? ir : 255));
        ret.G = (byte)(((ig = (int)this.g) < 0) ? 0 : ((ig <= 255) ? ig : 255));
        ret.B = (byte)(((ib = (int)this.b) < 0) ? 0 : ((ib <= 255) ? ib : 255));
        ret.A = (byte)(((ia = (int)this.a) < 0) ? 0 : ((ia <= 255) ? ia : 255));
        return ret;
    }
}
//endregion

//region common Vec2d
class Vec2d {
    public static readonly Vec2d invalid = new Vec2d(Int32.MinValue - 1.0, Int32.MinValue - 1.0);
    public double x, y;
    public Vec2d(double x, double y) {
        this.x = x;
        this.y = y;
    }
    public bool isValid() {
    	return Int32.MinValue <= x && x <= Int32.MaxValue
				&& Int32.MinValue <= y && y <= Int32.MaxValue;
    }
    public static Vec2d operator + (Vec2d a, Vec2d b) {
        return new Vec2d((a.x + b.x), (a.y + b.y));
    }
    public static Vec2d operator * (double f, Vec2d a) {
        return new Vec2d((f * a.x), (f * a.y));
    }
    public static Vec2d operator - (Vec2d a) {
        return -1.0 * a;
    }
    public static Vec2d operator - (Vec2d a, Vec2d b) {
        return a + -b;
    }
    public static double operator ^ (Vec2d a, Vec2d b) {
        return (a.x * b.x) + (a.y * b.y);
    }
    public double abs2() {
        return this ^ this;
    }
    public double abs() {
        return Math.Sqrt(this ^ this);
    }
}
//endregion

//region common Vec3d
class Vec3d {
    public static readonly Vec3d zero = new Vec3d(0, 0, 0);
    public double x, y, z;
    public Vec3d(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public bool isZero() {
    	return ((this.x == 0) && (this.y == 0) && (this.z == 0));
    }
    public static Vec3d operator + (Vec3d a, Vec3d b) {
        return new Vec3d((a.x + b.x), (a.y + b.y), (a.z + b.z));
    }
    public static Vec3d operator * (double f, Vec3d a) {
        return new Vec3d((f * a.x), (f * a.y), (f * a.z));
    }
    public static Vec3d operator - (Vec3d a) {
        return -1.0 * a;
    }
    public static Vec3d operator - (Vec3d a, Vec3d b) {
        return a + -b;
    }
    public static double operator ^ (Vec3d a, Vec3d b) {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }
    public double abs2() {
        return this ^ this;
    }
    public double abs() {
        return Math.Sqrt(this ^ this);
    }
    public static Vec3d operator * (Vec3d a, Vec3d b) {
        return new Vec3d(
            (a.y * b.z - b.y * a.z),
            (a.z * b.x - b.z * a.x),
            (a.x * b.y - b.x * a.y));
    }
}

class Mat3d {
	public static readonly Mat3d unit = new Mat3d(1, 0, 0, 0, 1, 0, 0, 0, 1);
    public double[,] m;
    public Mat3d(
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22) {
        this.m = new double[,] {
            {m00, m01, m02},
            {m10, m11, m12},
            {m20, m21, m22}
        };
    }
    public Mat3d(Vec3d v0, Vec3d v1, Vec3d v2) {
        this.m = new double[,]{
            {v0.x, v1.x, v2.x},
            {v0.y, v1.y, v2.y},
            {v0.z, v1.z, v2.z}
        };
    }

    public static Mat3d operator * (Mat3d a, Mat3d b) {
        return new Mat3d(
            a.m[0, 0] * b.m[0, 0] + a.m[0, 1] * b.m[1, 0] + a.m[0, 2] * b.m[2, 0],
            a.m[0, 0] * b.m[0, 1] + a.m[0, 1] * b.m[1, 1] + a.m[0, 2] * b.m[2, 1],
            a.m[0, 0] * b.m[0, 2] + a.m[0, 1] * b.m[1, 2] + a.m[0, 2] * b.m[2, 2],

            a.m[1, 0] * b.m[0, 0] + a.m[1, 1] * b.m[1, 0] + a.m[1, 2] * b.m[2, 0],
            a.m[1, 0] * b.m[0, 1] + a.m[1, 1] * b.m[1, 1] + a.m[1, 2] * b.m[2, 1],
            a.m[1, 0] * b.m[0, 2] + a.m[1, 1] * b.m[1, 2] + a.m[1, 2] * b.m[2, 2],

            a.m[2, 0] * b.m[0, 0] + a.m[2, 1] * b.m[1, 0] + a.m[2, 2] * b.m[2, 0],
            a.m[2, 0] * b.m[0, 1] + a.m[2, 1] * b.m[1, 1] + a.m[2, 2] * b.m[2, 1],
            a.m[2, 0] * b.m[0, 2] + a.m[2, 1] * b.m[1, 2] + a.m[2, 2] * b.m[2, 2]);
    }
    public static Vec3d operator * (Mat3d a, Vec3d v) {
        return new Vec3d(
            a.m[0, 0] * v.x + a.m[0, 1] * v.y + a.m[0, 2] * v.z,
            a.m[1, 0] * v.x + a.m[1, 1] * v.y + a.m[1, 2] * v.z,
            a.m[2, 0] * v.x + a.m[2, 1] * v.y + a.m[2, 2] * v.z);
    }
    public static Mat3d operator * (double d, Mat3d a) {
        return new Mat3d(
            d * a.m[0, 0], d * a.m[0, 1], d * a.m[0, 2],
            d * a.m[1, 0], d * a.m[1, 1], d * a.m[1, 2],
            d * a.m[2, 0], d * a.m[2, 1], d * a.m[2, 2]);
    }
    public static Mat3d operator / (double d, Mat3d a) {
        double det =
            + a.m[0, 0] * a.m[1, 1] * a.m[2, 2]
            + a.m[1, 0] * a.m[2, 1] * a.m[0, 2]
            + a.m[2, 0] * a.m[0, 1] * a.m[1, 2]
            - a.m[0, 0] * a.m[2, 1] * a.m[1, 2]
            - a.m[2, 0] * a.m[1, 1] * a.m[0, 2]
            - a.m[1, 0] * a.m[0, 1] * a.m[2, 2];
        return new Mat3d(
            d * (a.m[1, 1] * a.m[2, 2] - a.m[1, 2] * a.m[2, 1]) / det,
            d * (a.m[0, 2] * a.m[2, 1] - a.m[0, 1] * a.m[2, 2]) / det,
            d * (a.m[0, 1] * a.m[1, 2] - a.m[0, 2] * a.m[1, 1]) / det,

            d * (a.m[1, 2] * a.m[2, 0] - a.m[1, 0] * a.m[2, 2]) / det,
            d * (a.m[0, 0] * a.m[2, 2] - a.m[0, 2] * a.m[2, 0]) / det,
            d * (a.m[0, 2] * a.m[1, 0] - a.m[0, 0] * a.m[1, 2]) / det,

            d * (a.m[1, 0] * a.m[2, 1] - a.m[1, 1] * a.m[2, 0]) / det,
            d * (a.m[0, 1] * a.m[2, 0] - a.m[0, 0] * a.m[2, 1]) / det,
            d * (a.m[0, 0] * a.m[1, 1] - a.m[0, 1] * a.m[1, 0]) / det);
    }
    public static Mat3d rotX(double rad) {
        return new Mat3d(
            1, 0, 0,
            0, Math.Cos(rad), Math.Sin(rad),
            0, -(Math.Sin(rad)), Math.Cos(rad));
    }
    public static Mat3d rotY(double rad) {
        return new Mat3d(
            Math.Cos(rad), 0, -(Math.Sin(rad)),
            0, 1, 0,
            Math.Sin(rad), 0, Math.Cos(rad));
    }
    public static Mat3d rotZ(double rad) {
        return new Mat3d(
            Math.Cos(rad), Math.Sin(rad), 0,
            -(Math.Sin(rad)), Math.Cos(rad), 0,
            0, 0, 1);
    }
}
//endregion
//endregion


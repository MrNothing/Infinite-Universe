using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SimplexNoiseGenerator
{
	public enum NoiseType
	{
		Simple,
		Fractal,
		Coherent
	}
	
	// Simplex noise in 2D, 3D and 4D
	private static Vector3[] grad3 = new Vector3[12] {
		new Vector3(1,1,0),new Vector3(-1,1,0),new Vector3(1,-1,0),new Vector3(-1,-1,0),
		new Vector3(1,0,1),new Vector3(-1,0,1),new Vector3(1,0,-1),new Vector3(-1,0,-1),
		new Vector3(0,1,1),new Vector3(0,-1,1),new Vector3(0,1,-1),new Vector3(0,-1,-1)};
	
	private const int GradientSizeTable = 256;
	
	// To remove the need for index wrapping, float the permutation table length
	private int[] perm = new int[512];

	public SimplexNoiseGenerator(){
		Seed (0);
	}

	public SimplexNoiseGenerator(int seed){
		Seed (seed);
	}

	public SimplexNoiseGenerator(string seed){
		Seed (seed.Length);
	}
	
	public void Seed(int seed){
		int prevSeed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = seed;
		int i, j, k;
		for (i = 0 ; i < GradientSizeTable ; i++)
		{
			perm[i] = (byte)i;
		}
		
		while (--i != 0)
		{
			k = perm[i];
			j = UnityEngine.Random.Range(0, GradientSizeTable);
			perm[i] = perm[j];
			perm[j] = (byte)k;
		}
		
		for (i = 0 ; i < GradientSizeTable; i++)
		{
			perm[GradientSizeTable + i] = perm[i];
		}
		
		UnityEngine.Random.seed = prevSeed;
	}
	
	public float coherentNoise(float x, float y, float z, int octaves=1, float frequency = 25, float amplitude = 0.5f, float test1=0, float test2=0) {
		return coherentNoise(new Vector3(x,y,z),octaves,frequency,amplitude);
	}
	
	public float coherentNoise(Vector3 position, int octaves, float frequency,float amplitude){
		float gain = 1.0f;
		float sum = 0.0f;
		float ifrq = 1f / frequency;
		for(int i = 0; i < octaves; i++){
			sum +=  Noise(position * gain * ifrq) / gain;
			gain *= 2.0f;
		}
		return sum * amplitude;
	}
	
	
	public float coherentNoise2(float x, float y, float z, int octaves=1, float multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f) {
		return coherentNoise2(new Vector3(x,y,z),octaves,multiplier,amplitude,lacunarity,persistence);
	}
	
	public float coherentNoise2(Vector3 position, int octaves=1, float multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f) {
		position /= multiplier;
		float val = 0;
		for (int n = 0; n < octaves; n++) {
			val += Noise(position) * amplitude;
			position *= lacunarity;
			amplitude *= persistence;
		}
		return val;
	}
	
	// 3D simplex noise
	public float Noise(float x, float y, float z){
		return Noise(new Vector3(x,y,z));
	}
	
	
	public float Noise(Vector3 position)
	{
		float c0=0f, c1=0f, c2=0f, c3=0f;
		// Noise contributions from the four corners
		// Skew the input space to determine which simplex cell we're in
		
		float s = (position.x+position.y+position.z)*0.3333333f;
		// Very nice and simple skew factor for 3D
		
		int i = position.x > -s ? (int)(position.x+s) : (int)(position.x+s)-1;
		int j = position.y > -s ? (int)(position.y+s) : (int)(position.y+s)-1;
		int k = position.z > -s ? (int)(position.z+s) : (int)(position.z+s)-1;
		
		// Very nice and simple unskew factor, too
		float t = (i+j+k)*0.1666666f;
		
		// The x,y,z distances from the cell origin
		float x0 = position.x-(i-t);
		float y0 = position.y-(j-t);
		float z0 = position.z-(k-t);
		
		// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
		// Determine which simplex we are in.
		int i1=0, j1=0, k1=0; // Offsets for second corner of simplex in (i,j,k) coords
		int i2=1, j2=1, k2=1; // Offsets for third corner of simplex in (i,j,k) coords
		
		if(x0>=y0) {
			if(y0>=z0) { i1=1; k2=0; } // X Y Z order
			else if(x0>=z0) { i1=1; j2=0; } // X Z Y order
			else { k1=1; j2=0; } // Z X Y order
		} else { // x0<y0
			if(y0<z0) { k1=1; i2=0; } // Z Y X order
			else if(x0<z0) { j1=1; i2=0; } // Y Z X order
			else { j1=1; k2=0; } // Y X Z order
		}
		
		// Offsets for second corner in (x,y,z) coords
		float x1 = x0 - i1 + 0.1666666f;
		float y1 = y0 - j1 + 0.1666666f;
		float z1 = z0 - k1 + 0.1666666f;
		
		// Offsets for third corner in (x,y,z) coords
		float x2 = x0 - i2 + 0.3333333f;
		float y2 = y0 - j2 + 0.3333333f;
		float z2 = z0 - k2 + 0.3333333f;
		
		// Offsets for last corner in (x,y,z) coords
		float x3 = x0 - 0.5f;
		float y3 = y0 - 0.5f;
		float z3 = z0 - 0.5f;
		
		// Work out the hashed gradient indices of the four simplex corners
		int ii = i & 255;
		int jj = j & 255;
		int kk = k & 255;
		Vector3 v;
		
		// Calculate the contribution from the four corners
		float t0 = 0.5f - x0*x0 - y0*y0 - z0*z0;
		if(t0>=0){
			v = grad3[perm[ii+perm[jj+perm[kk]]] % 12];
			t0 *= t0;
			c0 = t0 * t0 * (v.x*x0+v.y*y0+v.z*z0);
		}
		
		float t1 = 0.5f - x1*x1 - y1*y1 - z1*z1;
		if(t1>=0){
			v = grad3[perm[ii+i1+perm[jj+j1+perm[kk+k1]]] % 12];
			t1 *= t1;
			c1 = t1 * t1 * (v.x*x1+v.y*y1+v.z*z1);
		}
		
		float t2 = 0.5f - x2*x2 - y2*y2 - z2*z2;
		if(t2>=0){
			v = grad3[perm[ii+i2+perm[jj+j2+perm[kk+k2]]] % 12];
			t2 *= t2;
			c2 = t2 * t2 * (v.x*x2+v.y*y2+v.z*z2);
		}
		
		float t3 = 0.5f - x3*x3 - y3*y3 - z3*z3;
		if(t3>=0){
			v = grad3[perm[ii+1+perm[jj+1+perm[kk+1]]] % 12];
			t3 *= t3;
			c3 = t3 * t3 * (v.x*x3+v.y*y3+v.z*z3);
		}
		
		// Add contributions from each corner to get the final noise value.
		// The result is scaled to stay just inside [-1,1]
		return 32.0f*(c0 + c1 + c2 + c3);
	}
}

/* 
public class SimplexNoiseGenerator {
private int[] A = new int[3];
private float s, u, v, w;
private int i, j, k;
private float onethird = 0.333333333f;
private float onesixth = 0.166666667f;
private int[] T;
 
public SimplexNoiseGenerator() {
if (T == null) {
System.Random rand = new System.Random();
T = new int[8];
for (int q = 0; q < 8; q++)
T[q] = rand.Next();
}
}
 
public SimplexNoiseGenerator(string seed) {
T = new int[8];
string[] seed_parts = seed.Split(new char[] {' '});
 
for(int q = 0; q < 8; q++) {
int b;
try {
b = int.Parse(seed_parts[q]);
} catch {
b = 0x0;
}
T[q] = b;
}
}
 
public SimplexNoiseGenerator(int[] seed) { // {0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a}
T = seed;
}
 
public string GetSeed() {
string seed = "";
 
for(int q=0; q < 8; q++) {
seed += T[q].ToString();
if(q < 7)
seed += " ";
}
 
return seed;
}
 
public float coherentNoise(float x, float y, float z, int octaves=1, int multiplier = 25, float amplitude = 0.5f, float lacunarity = 2, float persistence = 0.9f) {
Vector3 v3 = new Vector3(x,y,z)/multiplier;
float val = 0;
for (int n = 0; n < octaves; n++) {
val += noise(v3.x,v3.y,v3.z) * amplitude;
v3 *= lacunarity;
amplitude *= persistence;
}
return val;
}
 
public int getDensity(Vector3 loc) {
float val = coherentNoise(loc.x, loc.y, loc.z);
return (int)Mathf.Lerp(0,255,val);
}
 
// Simplex Noise Generator
public float noise(float x, float y, float z) {
s = (x + y + z) * onethird;
i = fastfloor(x + s);
j = fastfloor(y + s);
k = fastfloor(z + s);
 
s = (i + j + k) * onesixth;
u = x - i + s;
v = y - j + s;
w = z - k + s;
 
A[0] = 0; A[1] = 0; A[2] = 0;
 
int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;
 
return kay(hi) + kay(3 - hi - lo) + kay(lo) + kay(0);
}
 
float kay(int a) {
s = (A[0] + A[1] + A[2]) * onesixth;
float x = u - A[0] + s;
float y = v - A[1] + s;
float z = w - A[2] + s;
float t = 0.6f - x * x - y * y - z * z;
int h = shuffle(i + A[0], j + A[1], k + A[2]);
A[a]++;
if (t < 0) return 0;
int b5 = h >> 5 & 1;
int b4 = h >> 4 & 1;
int b3 = h >> 3 & 1;
int b2 = h >> 2 & 1;
int b1 = h & 3;
 
float p = b1 == 1 ? x : b1 == 2 ? y : z;
float q = b1 == 1 ? y : b1 == 2 ? z : x;
float r = b1 == 1 ? z : b1 == 2 ? x : y;
 
p = b5 == b3 ? -p : p;
q = b5 == b4 ? -q : q;
r = b5 != (b4 ^ b3) ? -r : r;
t *= t;
return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
}
 
int shuffle(int i, int j, int k) {
return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
}
 
int b(int i, int j, int k, int B) {
return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
}
 
int b(int N, int B) {
return N >> B & 1;
}
 
int fastfloor(float n) {
return n > 0 ? (int)n : (int)n - 1;
}
}*/
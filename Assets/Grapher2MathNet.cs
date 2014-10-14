using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics;
using System;

public class Grapher2MathNet : MonoBehaviour {
	
	public enum FunctionOption {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		QuadraticForm
	}
	
	private delegate float FunctionDelegate (Vector3 p, float t);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple,
		QuadraticForm
	};
	
	public FunctionOption function;
	
	[Range(10, 100)]
	public int resolution = 50;

	[Range(1, 50)]
	public int iterationCount = 30;

	[Range(-0.5f, 0.5f)]
	public float xStart = -0.2f;

	[Range(-0.5f, 0.5f)]
	public float zStart = -.4f;

	private int currentResolution;
	private ParticleSystem.Particle[] points;
	private ParticleSystem.Particle[] optimizationPoints;
	
	private void CreateGridPoints () {
		currentResolution = resolution;
		points = new ParticleSystem.Particle[resolution * resolution + resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for (int x = 0; x < resolution; x++) {
			for (int z = 0; z < resolution; z++) {
				Vector3 p = new Vector3(increment * (x - resolution / 2.0f), 0f, increment * (z - resolution / 2.0f));
				points[i].position = p;
				points[i].color = new Color(p.x + increment * resolution / 2.0f, 0f, p.z + increment * resolution / 2.0f);
				points[i++].size = 0.1f;
			}
		}
		for (int t = 0; t < resolution; t++) {
			Vector3 p = new Vector3(0f, 0f, 0f);
			points[i].position = p;
			points[i].color = new Color(increment * resolution / 2, increment * resolution / 2, increment * resolution / 2);
			points[i++].size = 0.15f;
		}

	}

	//private void CreateOptimizationPoints () {
	//	currentResolution = resolution;
	//	optimizationPoints = new ParticleSystem.Particle[resolution];
	//	float increment = 1f / (resolution - 1);
	//	int i = 0;
	//	for (int t = 0; t < resolution; t++) {
	//		Vector3 p = new Vector3(0f, 0f, 0f);
	//		optimizationPoints[i].position = p;
	//		optimizationPoints[i].color = new Color(p.x + increment * resolution / 2, 0f, p.z + increment * resolution / 2);
	//		optimizationPoints[i++].size = 0.15f;
	//	}
	//}
	
	void Update () {
		if (currentResolution != resolution || points == null) {
			CreateGridPoints();
		}
		//if (currentResolution != resolution || optimizationPoints == null) {
		//	CreateOptimizationPoints();
		//}
		FunctionDelegate f = functionDelegates[(int)function];
		float t = Time.timeSinceLevelLoad;
		//function graph steps
		for (int i = 0; i < points.Length; i++) {
			Vector3 p = points [i].position;
			p.y = f (p, t);
			points[i].position = p;
			Color c = points [i].color;
			c.g = p.y;
			points[i].color = c;
		}
	
		//optimization steps
		double xx =  (double) Mathf.Sin (0.5f * t + .1f);
		double zz =  (double) Mathf.Cos(0.5f * t);
		double xz = (double) 0.1f * Mathf.Sin (0.25f * t);
		Matrix a = new Matrix(new double[][] {
			new double[] { xx, xz },
			new double[] { xz, zz } });

		Matrix currentPoint = new Matrix(new double[][] {
			new double[] {xStart},
			new double[] {zStart}});
		Matrix lastPoint = currentPoint.Clone();

		double[] ts = new double[iterationCount + 1];
		double[] xs = new double[iterationCount + 1];
		double[] zs = new double[iterationCount + 1];
		xs[0] = currentPoint.GetArray()[0][0];
		zs[0] = currentPoint.GetArray()[1][0];

		for (int i = 0; i < iterationCount; i++) {
			ts[i] = i;
			currentPoint = lastPoint - 0.1f * a * lastPoint;
		
			xs[i + 1] = currentPoint.GetArray()[0][0];
			zs[i + 1] = currentPoint.GetArray()[1][0];
			lastPoint = currentPoint.Clone();
		}
		ts[iterationCount] = iterationCount;
		
		IInterpolationMethod xInterp = Interpolation.Create(ts, xs);
		IInterpolationMethod zInterp = Interpolation.Create(ts, zs);

		float increment = ((float) iterationCount) / ((float) (resolution - 1));

		for (int i = 0; i < resolution; i++) {
			float curInc = increment * i;
			float debug = (float) xInterp.Interpolate(curInc);
			Vector3 p = new Vector3((float) xInterp.Interpolate(curInc), 0.0f, (float) zInterp.Interpolate(curInc));
			p.y = f(p, t); 
			points[i].position = p;
			Color c = points [i].color;
			c.g = p.y;
			points[i].color = c;
		}
		
		particleSystem.SetParticles(points, points.Length);
		//particleSystem.SetParticles(optimizationPoints, optimizationPoints.Length);


		//Matrix m = new Matrix(new double[][] {
		//	new double[] { 10.0, -18.0 },
		//	new double[] { 6.0, -11.0 } });
		
		// alternative way to create the matrix:
		// double[][] data = Matrix.CreateMatrixData(2, 2);
		// data[0][0] = 10.0;
		// data[1][0] = 6.0;
		// data[0][1] = -18.0;
		// data[1][1] = -11.0;
		// Matrix m = new Matrix(data);
		
		//EigenvalueDecomposition eigen = m.EigenvalueDecomposition;
		
		//Complex[] eigenValues = eigen.EigenValues;
		// eigenvalues: 1, -2
		
		//Matrix eigenVectors = eigen.EigenVectors;
		
		// eigenvectors: [0.894...,0.447...] and [6.708...,4.473...]
		
		// alternative way to access the eigenvalues witout the Complex type:
		// double[] eigenValuesReal = eigen.RealEigenvalues; // real part
		// double[] eigenValuesImag = eigen.ImagEigenvalues; // imaginary part
	}
	
	private static float Linear (Vector3 p, float t) {
		return p.x;
	}
	
	private static float Exponential (Vector3 p, float t) {
		return p.x * p.x;
	}
	
	private static float Parabola (Vector3 p, float t){
		return 1f - p.x * p.x * p.z * p.z;
	}

	private static float QuadraticForm (Vector3 p, float t){
		double xx =  (double) Mathf.Sin (0.5f * t + .1f);
		double zz =  (double) Mathf.Cos(0.5f * t);
		double xz = (double) 0.1f * Mathf.Sin (0.25f * t);
		Matrix a = new Matrix(new double[][] {
			new double[] { xx, xz },
			new double[] { xz, zz } });
		Matrix v = new Matrix(new double[][] {
			new double[] {p.x},
			new double[] {p.z}});
		Matrix vt = v.Clone();
		vt.Transpose();
		Matrix v3 = vt * a * v;
		return (float) (0.5 * v3.GetArray()[0][0]);
	}
	
	private static float Sine (Vector3 p, float t){
		return 0.50f +
			0.25f * Mathf.Sin(4 * Mathf.PI * p.x + 4 * t) * Mathf.Sin(2 * Mathf.PI * p.z + t) +
				0.10f * Mathf.Cos(3 * Mathf.PI * p.x + 5 * t) * Mathf.Cos(5 * Mathf.PI * p.z + 3 * t) +
				0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}
	
	private static float Ripple (Vector3 p, float t){
		p.x -= 0.5f;
		p.z -= 0.5f;
		float squareRadius = p.x * p.x + p.z * p.z;
		return 0.5f + Mathf.Sin(15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
	}
}
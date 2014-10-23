using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics;
using System;

public class OptimizationPlot : MonoBehaviour {
	
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
	
	public FunctionOption function = FunctionOption.QuadraticForm;

	public enum DescentOption {
		GradientDescent,
		NewtonsMethod,
		BacktrackingGradientDescent
	}
	
	private delegate Matrix DescentDelegate (Matrix m, float t);
	private static FunctionDelegate[] descentDelegates = {
		//GradientDescent,
		//NewtonsMethod,
		//BacktrackingGradientDescent
	};
	
	public DescentOption descent;// = DescentOption.GradientDescent;
	
	[Range(0.05f, 5.0f)]
	public float learningRate = 0.25f;
	
	[Range(10, 500)]
	public int resolution = 50;
	
	[Range(1, 100)]
	public int iterationCount = 100;
	
	[Range(-0.5f, 0.5f)]
	public float xStart = -0.2f;
	
	[Range(-0.5f, 0.5f)]
	public float zStart = -.4f;
	
	private int currentResolution;
	private ParticleSystem.Particle[] optimizationPoints;
	
	private void CreateOptimizationPoints () {
		currentResolution = resolution;
		optimizationPoints = new ParticleSystem.Particle[resolution];
		float increment = 1f / (resolution - 1);
		float logInc = Mathf.Log (increment);
		int i = 0;
		for (int t = 0; t < resolution; t++) {
			Vector3 p = new Vector3(0f, 0f, 0f);
			optimizationPoints[i].position = p;
			optimizationPoints[i].color = new Color(increment * resolution / 2, increment * resolution / 2, increment * resolution / 2);
			optimizationPoints[i++].size = 0.05f;
		}
		
	}

	
	void Update () {
		if (currentResolution != resolution || optimizationPoints == null) {
			CreateOptimizationPoints();
		}
		FunctionDelegate f = functionDelegates[(int)function];
		float t = Time.timeSinceLevelLoad;

		//optimization steps
		
		Matrix a = QuadraticFormMatrix(t);
		
		Matrix currentPoint = new Matrix(new double[][] {
			new double[] {xStart},
			new double[] {zStart}});
		Matrix currentGradient;
		Matrix currentHessianInv;
		Matrix lastPoint = currentPoint.Clone();
		
		double[] ts = new double[iterationCount + 1];
		double[] xs = new double[iterationCount + 1];
		double[] zs = new double[iterationCount + 1];
		xs[0] = currentPoint.GetArray()[0][0];
		zs[0] = currentPoint.GetArray()[1][0];
		
		for (int i = 0; i < iterationCount; i++) {
			ts[i] = i;
			currentGradient = a * lastPoint;
			//currentHessianInv = a.Inverse();
			//currentPoint = lastPoint - ((double) learningRate) * currentHessianInv * currentGradient;
			currentPoint = lastPoint - ((double) learningRate) * currentGradient;
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
			Vector3 p = new Vector3((float) xInterp.Interpolate(curInc), 0.0f, (float) zInterp.Interpolate(curInc));
			p.y = f(p, t); 
			optimizationPoints[i].position = p;
			Color c = optimizationPoints [i].color;
			c.g = ((float) i) / ((float) resolution);
			c.b = ((float) curInc) / ((float) iterationCount);
			//c.g = 1.0f / Mathf.Exp((float) i / 15.0f);
			optimizationPoints[i].color = c;
		}
		
		particleSystem.SetParticles(optimizationPoints, optimizationPoints.Length);

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
	
	private static Matrix QuadraticFormMatrix(float t){
		double xx = (double) Mathf.Sin (t) + 1.5f;
		double zz = (double) Mathf.Cos (t) + 1.5f;
		double xz = (double) 0.5f * Mathf.Sin (1.8f * t) + 0.2f;
		Matrix a = new Matrix (new double[][] {
			new double[] { xx, xz },
			new double[] { xz, zz } });
		return a;
	}
	
	private static float QuadraticForm (Vector3 p, float t){
		Matrix a = QuadraticFormMatrix (t);
		Matrix v = new Matrix(new double[][] {
			new double[] {p.x},
			new double[] {p.z}});
		Matrix vt = v.Clone();
		vt.Transpose();
		Matrix v3 = 0.5 * vt * a * v;
		return (float) (v3.GetArray()[0][0] + 0.1);
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
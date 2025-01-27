using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RingsEnemy : MonoBehaviour {
    private float outerScale = 4.0f;
    private float outerRadius1 = 101f;
    private float outerRadius2 = 103f;
    private float outerRadians1 = 0.0f;
    private float outerRadians2 = 1.0f;
    
    private float middleScale = 2.0f;
    private float middleRadius1 = 89f;
    private float middleRadius2 = 91f;
    private float middleRadians1 = 2.0f;
    private float middleRadians2 = 3.0f;

    private float innerScale = 1.0f;
    private float innerRadius1 = 83f;
    private float innerRadius2 = 79f;
    private float innerRadians1 = 4.0f;
    private float innerRadians2 = 5.0f;
    
    [SerializeField]
    private Transform outerTransform;
    [SerializeField]
    private Transform middleTransform;
    [SerializeField]
    private Transform innerTransform;
    
    Vector3 Sample(float radius1, float radius2, float radians1, float radians2) {
        float x1 = radius1 * Mathf.Cos(radians1);
        float y1 = radius1 * Mathf.Sin(radians1);
        float x2 = radius2 * Mathf.Cos(radians2);
        float y2 = radius2 * Mathf.Sin(radians2);

        float u1 = Mathf.PerlinNoise(x1, y1);
        float u2 = Mathf.PerlinNoise(x2, y2);

        float u1Z = (u1 * 2) - 1;
        float a = Mathf.Sqrt((1 - u1Z) * (1 + u1Z));
        
        float u2T = u2 * 2 * Mathf.PI;

        float ux = a * Mathf.Cos(u2T);
        float uy = a * Mathf.Sin(u2T);
        
        return new Vector3(ux, uy, u1Z);
    }
    
    Vector3 SampleOuter() => Sample(outerRadius1, outerRadius2, outerRadians1, outerRadians2);
    Vector3 SampleMiddle() => Sample(middleRadius1, middleRadius2, middleRadians1, middleRadians2);
    Vector3 SampleInner() => Sample(innerRadius1, innerRadius2, innerRadians1, innerRadians2);
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // TODO I need to refactor this to not be so cringe
        outerRadians1 += Time.deltaTime / (outerScale * outerRadius1);
        outerRadians1 %= Mathf.PI * 2;
        outerRadians2 += Time.deltaTime / (outerScale * outerRadius2);
        outerRadians2 %= Mathf.PI * 2;
        middleRadians1 += Time.deltaTime / (middleScale * middleRadius1);
        middleRadians1 %= Mathf.PI * 2;
        middleRadians2 += Time.deltaTime / (middleScale * middleRadius2);
        middleRadians2 %= Mathf.PI * 2;
        innerRadians1 += Time.deltaTime / (innerScale * innerRadius1);
        innerRadians1 %= Mathf.PI * 2;
        innerRadians2 += Time.deltaTime / (innerScale * innerRadius2);
        innerRadians2 %= Mathf.PI * 2;
        
        Vector3 outer = SampleOuter();
        this.outerTransform.localRotation = Quaternion.LookRotation(outer);
        
        Vector3 middle = SampleMiddle();
        this.middleTransform.localRotation = Quaternion.LookRotation(middle);
        
        Vector3 inner = SampleInner();
        this.innerTransform.localRotation = Quaternion.LookRotation(inner);
    }
}

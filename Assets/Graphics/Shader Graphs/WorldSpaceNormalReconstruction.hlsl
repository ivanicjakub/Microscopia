//float3 viewSpacePosAtScreenUV(in float2 uv,in float linearDepth)
//{
//    float3 viewSpaceRay = mul(unity_CameraInvProjection, float4(uv * 2.0 - 1.0, 1.0, 1.0) * _ProjectionParams.z);
//    return viewSpaceRay * linearDepth;
//}

//float3 viewSpacePosAtPixelPosition(in float2 vpos,in float2 depthTexelSize,in float linearDepth)
//{
//    float2 uv = vpos * depthTexelSize;
//    return viewSpacePosAtScreenUV(uv, linearDepth);
//}

//float3 viewNormalAtPixelPosition(in float2 vpos,in float2 depthTexelSize,in float linearDepth)
//{
//    // get current pixel's view space position
//    float3 viewSpacePos_c = viewSpacePosAtPixelPosition(vpos + float2( 0.0, 0.0), depthTexelSize, linearDepth);

//    // get view space position at 1 pixel offsets in each major direction
//    float3 viewSpacePos_r = viewSpacePosAtPixelPosition(vpos + float2( 1.0, 0.0), depthTexelSize, linearDepth);
//    float3 viewSpacePos_u = viewSpacePosAtPixelPosition(vpos + float2( 0.0, 1.0), depthTexelSize, linearDepth);

//    // get the difference between the current and each offset position
//    float3 hDeriv = viewSpacePos_r - viewSpacePos_c;
//    float3 vDeriv = viewSpacePos_u - viewSpacePos_c;

//    // get view space normal from the cross product of the diffs
//    float3 viewNormal = normalize(cross(hDeriv, vDeriv));

//    return viewNormal;
//}

//half3 GammaToLinearSpace (half3 sRGB)
//{
//    return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
//}

//void WorldSpaceNormal_float(in float4 ScreenPosition,in float2 DepthTexelSize,in float LinearDepth, out float3 WorldNormal)
//{
//    half3 viewNormal = viewNormalAtPixelPosition(ScreenPosition.xy, DepthTexelSize, LinearDepth);
//    half3 wNormal = mul((float3x3)unity_MatrixInvV, viewNormal);
//    WorldNormal = half4(GammaToLinearSpace(wNormal.xyz * 0.5 + 0.5), 1.0);
//}

#if !defined(SHADERGRAPH_PREVIEW)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
#endif
//sampler2D _CameraNormalsTexture;

void ReconstructNormalLQ_float(in float3 VertexWorldPosition, in float2 uv, out half3 WorldNormal)
{
      #if !defined(SHADERGRAPH_PREVIEW)
        WorldNormal = SampleSceneNormals(uv);
        #else
         WorldNormal = float3(0, 0, 0);
         #endif


    //#if defined(_SOURCE_DEPTH_LOW)
        //WorldNormal = half3(normalize(cross(ddy(VertexWorldPosition), ddx(VertexWorldPosition))));
    //#else
    //    float2 delta = float2(_SourceSize.zw * 2.0);

        //pixelDensity = rcp(pixelDensity);

        //// Sample the neighbour fragments
        //float2 lUV = float2(-delta.x, 0.0) * pixelDensity;
        //float2 rUV = float2(delta.x, 0.0) * pixelDensity;
        //float2 uUV = float2(0.0, delta.y) * pixelDensity;
        //float2 dUV = float2(0.0, -delta.y) * pixelDensity;

        //float3 l1 = float3(uv + lUV, 0.0); l1.z = SampleAndGetLinearEyeDepth(l1.xy); // Left1
        //float3 r1 = float3(uv + rUV, 0.0); r1.z = SampleAndGetLinearEyeDepth(r1.xy); // Right1
        //float3 u1 = float3(uv + uUV, 0.0); u1.z = SampleAndGetLinearEyeDepth(u1.xy); // Up1
        //float3 d1 = float3(uv + dUV, 0.0); d1.z = SampleAndGetLinearEyeDepth(d1.xy); // Down1

        //// Determine the closest horizontal and vertical pixels...
        //// horizontal: left = 0.0 right = 1.0
        //// vertical  : down = 0.0    up = 1.0
        //#if defined(_SOURCE_DEPTH_MEDIUM)
        //     uint closest_horizontal = l1.z > r1.z ? 0 : 1;
        //     uint closest_vertical   = d1.z > u1.z ? 0 : 1;
        //#else
        //    float3 l2 = float3(uv + lUV * 2.0, 0.0); l2.z = SampleAndGetLinearEyeDepth(l2.xy); // Left2
        //    float3 r2 = float3(uv + rUV * 2.0, 0.0); r2.z = SampleAndGetLinearEyeDepth(r2.xy); // Right2
        //    float3 u2 = float3(uv + uUV * 2.0, 0.0); u2.z = SampleAndGetLinearEyeDepth(u2.xy); // Up2
        //    float3 d2 = float3(uv + dUV * 2.0, 0.0); d2.z = SampleAndGetLinearEyeDepth(d2.xy); // Down2

        //    const uint closest_horizontal = abs( (2.0 * l1.z - l2.z) - linearDepth) < abs( (2.0 * r1.z - r2.z) - linearDepth) ? 0 : 1;
        //    const uint closest_vertical   = abs( (2.0 * d1.z - d2.z) - linearDepth) < abs( (2.0 * u1.z - u2.z) - linearDepth) ? 0 : 1;
        //#endif

        //// Calculate the triangle, in a counter-clockwize order, to
        //// use based on the closest horizontal and vertical depths.
        //// h == 0.0 && v == 0.0: p1 = left,  p2 = down
        //// h == 1.0 && v == 0.0: p1 = down,  p2 = right
        //// h == 1.0 && v == 1.0: p1 = right, p2 = up
        //// h == 0.0 && v == 1.0: p1 = up,    p2 = left
        //// Calculate the view space positions for the three points...
        //half3 P1;
        //half3 P2;
        //if (closest_vertical == 0)
        //{
        //    P1 = half3(closest_horizontal == 0 ? l1 : d1);
        //    P2 = half3(closest_horizontal == 0 ? d1 : r1);
        //}
        //else
        //{
        //    P1 = half3(closest_horizontal == 0 ? u1 : r1);
        //    P2 = half3(closest_horizontal == 0 ? l1 : u1);
        //}

        //// Use the cross product to calculate the normal...
        //WorldNormal = half3(normalize(cross(ReconstructViewPos(P2.xy, P2.z) - vpos, ReconstructViewPos(P1.xy, P1.z) - vpos)));
    //#endif
}
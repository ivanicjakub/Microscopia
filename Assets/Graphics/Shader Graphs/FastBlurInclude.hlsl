static const half curve[7] = { 0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205 };  // gauss'ish blur weights

static const half4 curve4[7] = { half4(0.0205,0.0205,0.0205,0), half4(0.0855,0.0855,0.0855,0), half4(0.232,0.232,0.232,0),
	half4(0.324,0.324,0.324,1), half4(0.232,0.232,0.232,0), half4(0.0855,0.0855,0.0855,0), half4(0.0205,0.0205,0.0205,0) };

void BlurSceneColor_float(in float4 ScreenPosition, in float BlurStrength, out float3 BlurredSceneColor)
{
	half2 uv = ScreenPosition; 
	half2 netFilterWidth = 0.005;  
	half2 coords = uv - netFilterWidth * BlurStrength;  
			
	half4 color = 0;
	for( int l = 0; l < 7; l++ )
	  {   
		half4 tap = half4(SHADERGRAPH_SAMPLE_SCENE_COLOR(uv).rgb, 1);
		color += tap * curve4[l];
		coords += netFilterWidth;
  	}
	BlurredSceneColor = color;
}
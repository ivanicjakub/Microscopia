#define COLLISIONSCOUNT 8

float4 _CollisionPositions[COLLISIONSCOUNT];
float4 _CollisionVectors[COLLISIONSCOUNT];

void DestructionPosition_float(float3 ObjectPosition, float3 ObejctNormal, float MaxDistance, float DestructionPower, out float3 DestructionObjectPosition, out float3 NewNormal)
{
	float3 newPosition = ObjectPosition;
	float3 newNormal = ObejctNormal;
	for(int i=0; i < COLLISIONSCOUNT; i++)
	{
		float3 colPos = _CollisionPositions[i];
		float mag = length(colPos);
		float d = distance(ObjectPosition, colPos);

		if(mag < 0.001f || d > MaxDistance)
			continue;

		if(mag > 0.01f)
		{
			float magFac = d / MaxDistance;
			float4 colVector = _CollisionVectors[i];
			newPosition = lerp(ObjectPosition, newPosition * magFac * colVector, DestructionPower);
			newNormal = lerp(ObejctNormal, magFac * colVector, DestructionPower);
		}
	}
	DestructionObjectPosition = newPosition;
	NewNormal = newNormal;
}

void DestructionPixelFactor_float(float3 ObjectPosition, float MaxDistance, float DestructionPower, out float DestructionPixelFactor)
{
	float factor = 0;
	for(int i=0; i < COLLISIONSCOUNT; i++)
	{
		float3 colPos = _CollisionPositions[i];
		float mag = length(colPos);
		float d = distance(ObjectPosition, colPos);

		if(mag < 0.001f || d > MaxDistance)
			continue;

		if(mag > 0.01f)
		{
			float magFac = d / MaxDistance;
			factor = magFac * DestructionPower;
		}
	}
	DestructionPixelFactor = factor;
}
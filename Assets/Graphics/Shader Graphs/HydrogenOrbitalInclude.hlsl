// Associated Laguerre polynomials
float L(int n, int l, float x) {
    if (n == 2 && l == 0) return 1.0 - 0.5 * x;
    if (n == 3 && l == 0) return 1.0 - 2.0/3.0 * x + 2.0/27.0 * x * x;
    if (n == 3 && l == 1) return 2.0 - 2.0/3.0 * x;
    if (n == 4 && l == 0) return 1.0 - 3.0/4.0 * x + 1.0/8.0 * x * x - 1.0/192.0 * x * x * x;
    if (n == 4 && l == 1) return 3.0 - 1.0/4.0 * x + 1.0/48.0 * x * x;
    if (n == 4 && l == 2) return 3.0 - 1.0/6.0 * x;
    return 1.0; // Default fallback
}

// Spherical harmonics
float Y(int l, int m, float theta, float phi, float Pi) {
    if (l == 0) return 1.0 / sqrt(4.0 * Pi);
        
    if (l == 1) {
        if (m == 0) return sqrt(3.0/(4.0*Pi)) * cos(theta);
        if (abs(m) == 1) return sqrt(3.0/(8.0*Pi)) * sin(theta) * (m == 1 ? cos(phi) : sin(phi));
    }
        
    if (l == 2) {
        if (m == 0) return sqrt(5.0/(16.0*Pi)) * (3.0 * cos(theta) * cos(theta) - 1.0);
        if (abs(m) == 1) return sqrt(15.0/(8.0*Pi)) * sin(theta) * cos(theta) * (m == 1 ? cos(phi) : sin(phi));
        if (abs(m) == 2) return sqrt(15.0/(32.0*Pi)) * sin(theta) * sin(theta) * (m == 2 ? cos(2.0*phi) : sin(2.0*phi));
    }
        
    if (l == 3) {
        if (m == 0) return sqrt(7.0/(16.0*Pi)) * (5.0 * cos(theta) * cos(theta) * cos(theta) - 3.0 * cos(theta));
        if (abs(m) == 1) return sqrt(21.0/(64.0*Pi)) * sin(theta) * (5.0 * cos(theta) * cos(theta) - 1.0) * (m == 1 ? cos(phi) : sin(phi));
        if (abs(m) == 2) return sqrt(105.0/(32.0*Pi)) * sin(theta) * sin(theta) * cos(theta) * (m == 2 ? cos(2.0*phi) : sin(2.0*phi));
        if (abs(m) == 3) return sqrt(35.0/(64.0*Pi)) * sin(theta) * sin(theta) * sin(theta) * (m == 3 ? cos(3.0*phi) : sin(3.0*phi));
    }
        
    return 0.0;
}

// Calculate factorial
float factorial(int n) {
    float result = 1.0;
    for(int i = 2; i <= n; i++)
        result *= float(i);
    return result;
}

    // Radial part
float R(int n, int l, float r, float a0) {
    float norm = sqrt(pow(2.0/(n*a0), 3) * factorial(n-l-1)/(2.0*n*factorial(n+l)));
    float rho = 2.0 * r / (n * a0);
    return norm * pow(rho, l) * exp(-rho/2.0) * L(n, l, rho);
}


void HydrogenOrbital_float(float3 Position, float Time, int PrincipalN, int OrbitalL, int MagneticM, float Pi, float SqrtPi, out float Probability)
{
    // Convert to spherical coordinates
    float r = length(Position);
    float theta = acos(Position.z / (r + 0.000001));
    float phi = atan2(Position.y, Position.x);
    
    // Normalize radius for better visualization
    float a0 = 1.0; // Bohr radius
    r = r / a0;
    
    // Calculate wave function
    float psi = R(PrincipalN, OrbitalL, r, a0) * Y(OrbitalL, MagneticM, theta, phi, Pi);
    
    // Calculate probability density
    Probability = psi * psi;
    
    // Normalize and enhance visualization
    Probability = saturate(Probability * 5.0);
    
    // Add subtle animation
    Probability *= 1.0 + 0.1 * sin(Time * 2.0 + r * 3.0);
}

void OrbitalColor_float(float Probability, out float4 Color)
{
    // Define colors for gradient
    float4 purpleColor = float4(0.2, 0.0, 0.4, 1.0);
    float4 orangeColor = float4(1.0, 0.6, 0.0, 1.0);
    float4 whiteColor = float4(1.0, 1.0, 1.0, 1.0);
    
    // Create multi-step gradient
    float4 finalColor;
    if(Probability < 0.3)
        finalColor = lerp(purpleColor, orangeColor, Probability / 0.3);
    else if(Probability < 0.7)
        finalColor = lerp(orangeColor, whiteColor, (Probability - 0.3) / 0.4);
    else
        finalColor = whiteColor;
        
    Color = finalColor;
}
#pragma once
#define TWOPI (2*3.14159)
#define BITS  (20)
#define ONE   (1<<BITS)

inline int add(int a, int b)
{
	int c = a+b;
	return c;
}
inline int mul(int a, int b)
{
	int c;
	__int64 cc = a;
	cc *= b;
	c = cc / ONE;
	return c;
}
inline int mix(int *pA, int *pB, int *pY, int nSize)
{
	for (int i = 0; i < nSize; i++)
	{
		pY[i] = mul(pA[i], pB[i]);
	}
	return 0;
}
#include "math.h"
inline int absolute(int *pX, int *pY, int nSize)
{
	for (int i = 0; i < nSize; i++)
	{
		pY[i] = abs(pX[i]);
	}
	return 0;
}
inline int modulate(int *pC, int *pM, int *pY, int m, int nSize)
{
	for (int i = 0; i < nSize; i++)
	{
		int tt = ONE;
		// pY[i] = mul(mul(pM[i], m) + ONE, pC[i] / 2);
		__int64 mm = pM[i];
		__int64 cc = pC[i];
		__int64 pp =  (mm + ONE)*cc;
		pY[i] = pp / ONE;
	}
	return 0;
}

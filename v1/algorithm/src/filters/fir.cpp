#include "fir.h"

#include "ls_math.h"
#include <stdio.h>
#if false
int fir::coef_read(char *fileName, int nSize, int *pC)
{
	int i = 0;
	FILE *fp = fopen(fileName, "r");
	for (i = 0; i < nSize; i++)
	{
		char ss[100];
		char ss1[10];
		int ret = fscanf(fp, "%s%s%d", ss, ss1, &pC[i]);
		if (ret != 3) 
			break;
	}
	nOrder = i-1;
	fclose(fp);

	return i;
}
#endif

fir::fir(int Order, int* pCoef1)
{
	nOrder = Order;
	pState = new int[Order+1];
	for (int i = 0; i < nOrder; i++)
	{
		pState[i] = 0;
	}
	pCoef = pCoef1;
}
int fir::State_write(int sample)
{
	int i;
	for (i = 0; i < nOrder; i++)
	{
		pState[nOrder - i] = pState[nOrder - i - 1];
	}
	pState[0] = sample;
	return 0;
}
int fir::process(int *pX, int* pY, int Samples)
{
	int outIdx = 0;
	for (int j = 0; j < Samples; j+=2, outIdx++)
	{
		State_write(pX[j]);
		pY[outIdx] = 0;
		for (int i = 0; i <= nOrder; i++)
			pY[outIdx] += mul(pCoef[i] , pState[i]);              /* compute current output sample \(y\) */
	}

	return 0;
}

fir::~fir()
{
}

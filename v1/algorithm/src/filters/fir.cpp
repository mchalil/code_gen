#include "ls_code_gen_api.h"
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

#define FIR_MULT(a, b) ((a)*(b))


fir::fir(Integer Order, tFract32* pCoef1)
{
	nOrder = Order;
	pState = new tSamples[TICK_SZ + nOrder - 1];

	/* memset(pState, 0, sizeof(pState)); */
	/* explicitly clear memory */
	for (IndexInt i = 0; i < TICK_SZ + nOrder - 1; i++)
	{
		pState[i] = 0;
	}
	pCoef = pCoef1;
}

Integer fir::StateAddSamples(tSamples* pValues, Integer nCount, Integer nOffset)
{
	IndexInt i;

	for (i = 0; i < nCount; i++)
	{
		pState[nOffset + i] = pValues[i];
	}
	return 0;
}

Integer fir::StateShiftSamplesLeft(Integer nCount, Integer nOffset)
{
	IndexInt i;

	for (i = 0; i < nCount; i++)
	{
		pState[i] = pState[nOffset + i];
	}
	return 0;
}

Integer fir::process(tSamples *pX, tSamples* pY, Integer nCount)
{
	IndexInt outIdx = 0;

	/* add the new nCount samples to the end of the state buffer */
	StateAddSamples(pX, nCount, nOrder - 1);

	for (IndexInt n = 0; n < nCount; n++) {
		tFract32 *pCoefTemp = pCoef;
		tSamples *pSamples =  &pState[nOrder - 1 + n];
		tSamples acc = 0;

		for (IndexInt k = 0; k < nOrder; k++) {
			acc += FIR_MULT((*pCoefTemp++) , (*pSamples--));
		}
		pY[n] = acc;
	}

	/* move nOrder-1 state samples from end to beginnig to new samples can be added
	during the next call */
	StateShiftSamplesLeft(nOrder - 1, nCount);

	return 0;
}

fir::~fir()
{
	delete(pState);
}
